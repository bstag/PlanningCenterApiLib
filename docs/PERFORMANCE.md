# Performance Guide

## Overview
This guide provides best practices and optimization strategies for maximizing performance when using the Planning Center API SDK for .NET.

## Performance Monitoring

### Built-in Performance Tracking
The SDK includes automatic performance monitoring through the ServiceBase pattern:

```csharp
// Automatic performance logging is enabled by default
var people = await peopleService.ListAsync();
// Logs: "PeopleService.ListAsync completed in 245ms [CorrelationId: abc123]"
```

### Custom Performance Monitoring
```csharp
public class PerformanceMonitoringService
{
    private readonly ILogger<PerformanceMonitoringService> _logger;
    private readonly IPeopleService _peopleService;
    
    public async Task<TimeSpan> MeasureOperationAsync(Func<Task> operation)
    {
        var stopwatch = Stopwatch.StartNew();
        await operation();
        stopwatch.Stop();
        
        _logger.LogInformation("Operation completed in {Duration}ms", 
            stopwatch.ElapsedMilliseconds);
            
        return stopwatch.Elapsed;
    }
}
```

## Memory Optimization

### 1. Use Streaming for Large Datasets

**❌ Memory Intensive:**
```csharp
// Loads all people into memory at once
var allPeople = await peopleService.GetAllAsync();
foreach (var person in allPeople) // 10,000+ people in memory
{
    await ProcessPersonAsync(person);
}
```

**✅ Memory Efficient:**
```csharp
// Processes one person at a time
await foreach (var person in peopleService.StreamAsync())
{
    await ProcessPersonAsync(person); // Only 1 person in memory
}
```

### 2. Pagination Control

**Optimize Page Size:**
```csharp
// Small pages for real-time processing
var queryParams = new QueryParameters 
{ 
    PerPage = 25, // Faster response, more requests
    Include = ["emails", "phone_numbers"] // Only needed data
};

// Large pages for batch processing
var batchParams = new QueryParameters 
{ 
    PerPage = 100, // Slower response, fewer requests
    Include = [] // Minimal data for better performance
};
```

### 3. Selective Data Loading

**Load Only Required Fields:**
```csharp
// ❌ Loads all person data
var people = await peopleService.ListAsync();

// ✅ Loads only specific fields
var people = await peopleService.ListAsync(new QueryParameters
{
    Include = ["emails"], // Only include emails
    Fields = new Dictionary<string, string[]>
    {
        ["person"] = ["first_name", "last_name", "email"]
    }
});
```

## Caching Strategies

### 1. Enable Built-in Caching

```csharp
// Configure caching in Program.cs
builder.Services.Configure<PlanningCenterOptions>(options =>
{
    options.EnableCaching = true;
    options.DefaultCacheExpiration = TimeSpan.FromMinutes(5);
});
```

### 2. Custom Caching Implementation

```csharp
public class CachedPeopleService
{
    private readonly IPeopleService _peopleService;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachedPeopleService> _logger;
    
    public async Task<Person> GetPersonAsync(string id)
    {
        var cacheKey = $"person:{id}";
        
        if (_cache.TryGetValue(cacheKey, out Person cachedPerson))
        {
            _logger.LogDebug("Cache hit for person {PersonId}", id);
            return cachedPerson;
        }
        
        var person = await _peopleService.GetAsync(id);
        
        _cache.Set(cacheKey, person, TimeSpan.FromMinutes(10));
        _logger.LogDebug("Cached person {PersonId}", id);
        
        return person;
    }
}
```

### 3. Cache Invalidation

```csharp
public class SmartCachedService
{
    public async Task UpdatePersonAsync(string id, Person person)
    {
        // Update the person
        var updated = await _peopleService.UpdateAsync(id, person);
        
        // Invalidate related cache entries
        _cache.Remove($"person:{id}");
        _cache.Remove($"person:list"); // If you cache lists
        
        return updated;
    }
}
```

## Parallel Processing

### 1. Safe Parallel Operations

```csharp
public async Task ProcessMultiplePeopleAsync(IEnumerable<string> personIds)
{
    // Services are thread-safe
    var semaphore = new SemaphoreSlim(10); // Limit concurrent requests
    
    var tasks = personIds.Select(async id =>
    {
        await semaphore.WaitAsync();
        try
        {
            return await _peopleService.GetAsync(id);
        }
        finally
        {
            semaphore.Release();
        }
    });
    
    var people = await Task.WhenAll(tasks);
}
```

### 2. Batch Processing with Rate Limiting

```csharp
public async Task ProcessLargeDatasetAsync(IEnumerable<string> ids)
{
    const int batchSize = 50;
    const int delayBetweenBatches = 1000; // 1 second
    
    var batches = ids.Chunk(batchSize);
    
    foreach (var batch in batches)
    {
        var tasks = batch.Select(id => _peopleService.GetAsync(id));
        var results = await Task.WhenAll(tasks);
        
        await ProcessBatchResultsAsync(results);
        
        // Rate limiting
        await Task.Delay(delayBetweenBatches);
    }
}
```

## HTTP Client Optimization

### 1. Connection Pooling

```csharp
// The SDK automatically configures optimal HTTP client settings
// But you can customize if needed:
builder.Services.Configure<PlanningCenterOptions>(options =>
{
    options.RequestTimeout = TimeSpan.FromSeconds(30);
    options.MaxRetryAttempts = 3;
});

// For high-throughput scenarios:
builder.Services.ConfigureHttpClientDefaults(builder =>
{
    builder.ConfigureHttpClient(client =>
    {
        client.DefaultRequestHeaders.ConnectionClose = false; // Keep connections alive
    });
});
```

### 2. Request Compression

```csharp
// Enable compression for large payloads
builder.Services.ConfigureHttpClientDefaults(builder =>
{
    builder.ConfigureHttpClient(client =>
    {
        client.DefaultRequestHeaders.AcceptEncoding.Add(
            new StringWithQualityHeaderValue("gzip"));
        client.DefaultRequestHeaders.AcceptEncoding.Add(
            new StringWithQualityHeaderValue("deflate"));
    });
});
```

## Database Integration Performance

### 1. Efficient Data Synchronization

```csharp
public class EfficientSyncService
{
    public async Task SyncPeopleAsync()
    {
        var lastSyncTime = await GetLastSyncTimeAsync();
        
        // Only fetch updated records
        var queryParams = new QueryParameters
        {
            Where = $"updated_at>'{lastSyncTime:yyyy-MM-ddTHH:mm:ssZ}'",
            OrderBy = "updated_at",
            PerPage = 100
        };
        
        await foreach (var person in _peopleService.StreamAsync(queryParams))
        {
            await UpsertPersonToDbAsync(person);
        }
        
        await UpdateLastSyncTimeAsync(DateTime.UtcNow);
    }
}
```

### 2. Bulk Database Operations

```csharp
public async Task BulkSyncAsync()
{
    var batchSize = 1000;
    var batch = new List<Person>(batchSize);
    
    await foreach (var person in _peopleService.StreamAsync())
    {
        batch.Add(person);
        
        if (batch.Count >= batchSize)
        {
            await BulkInsertToDbAsync(batch);
            batch.Clear();
        }
    }
    
    // Process remaining items
    if (batch.Count > 0)
    {
        await BulkInsertToDbAsync(batch);
    }
}
```

## Fluent API Performance

### 1. Efficient Query Building

```csharp
// ✅ Efficient: Build query once, execute once
var query = client.People
    .Where(p => p.FirstName == "John")
    .Include(p => p.Emails)
    .OrderBy(p => p.LastName)
    .Take(50);
    
var results = await query.ToListAsync();

// ❌ Inefficient: Multiple API calls
var allPeople = await client.People.ToListAsync(); // Gets all people
var johns = allPeople.Where(p => p.FirstName == "John").ToList(); // Filters in memory
```

### 2. Streaming with Fluent API

```csharp
// Memory-efficient streaming
await foreach (var person in client.People
    .Where(p => p.Status == "active")
    .StreamAsync())
{
    await ProcessPersonAsync(person);
}
```

## Performance Monitoring and Metrics

### 1. Custom Metrics Collection

```csharp
public class PerformanceMetricsService
{
    private readonly IMetrics _metrics;
    
    public async Task<T> MeasureAsync<T>(string operationName, Func<Task<T>> operation)
    {
        using var timer = _metrics.Measure.Timer.Time("api_operation_duration", 
            new MetricTags("operation", operationName));
            
        try
        {
            var result = await operation();
            _metrics.Measure.Counter.Increment("api_operation_success", 
                new MetricTags("operation", operationName));
            return result;
        }
        catch (Exception ex)
        {
            _metrics.Measure.Counter.Increment("api_operation_error", 
                new MetricTags("operation", operationName, "error_type", ex.GetType().Name));
            throw;
        }
    }
}
```

### 2. Application Insights Integration

```csharp
public class TelemetryService
{
    private readonly TelemetryClient _telemetryClient;
    
    public async Task TrackApiCallAsync(string operation, Func<Task> apiCall)
    {
        var stopwatch = Stopwatch.StartNew();
        var success = false;
        
        try
        {
            await apiCall();
            success = true;
        }
        finally
        {
            stopwatch.Stop();
            
            _telemetryClient.TrackDependency(
                "PlanningCenterAPI", 
                operation, 
                DateTime.UtcNow.Subtract(stopwatch.Elapsed), 
                stopwatch.Elapsed, 
                success);
        }
    }
}
```

## Performance Best Practices

### 1. General Guidelines

- **Use streaming for large datasets** (>1000 records)
- **Enable caching for frequently accessed data**
- **Limit concurrent requests** (10-20 max)
- **Use appropriate page sizes** (25-100 records)
- **Include only necessary fields and relationships**
- **Implement proper error handling and retries**
- **Monitor and log performance metrics**

### 2. Specific Scenarios

#### Real-time Applications
```csharp
// Use smaller page sizes for faster response
var queryParams = new QueryParameters { PerPage = 25 };

// Enable aggressive caching
builder.Services.Configure<PlanningCenterOptions>(options =>
{
    options.EnableCaching = true;
    options.DefaultCacheExpiration = TimeSpan.FromMinutes(1);
});
```

#### Batch Processing
```csharp
// Use larger page sizes for efficiency
var queryParams = new QueryParameters { PerPage = 100 };

// Use streaming to avoid memory issues
await foreach (var item in service.StreamAsync(queryParams))
{
    await ProcessItemAsync(item);
}
```

#### Data Synchronization
```csharp
// Use incremental sync with timestamps
var queryParams = new QueryParameters
{
    Where = $"updated_at>'{lastSync:yyyy-MM-ddTHH:mm:ssZ}'",
    OrderBy = "updated_at",
    PerPage = 100
};
```

## Performance Testing

### 1. Load Testing Setup

```csharp
[Fact]
public async Task LoadTest_GetPeople_ShouldHandleConcurrentRequests()
{
    const int concurrentRequests = 50;
    const int requestsPerThread = 10;
    
    var tasks = Enumerable.Range(0, concurrentRequests)
        .Select(async _ =>
        {
            for (int i = 0; i < requestsPerThread; i++)
            {
                await _peopleService.ListAsync(new QueryParameters { PerPage = 25 });
            }
        });
    
    var stopwatch = Stopwatch.StartNew();
    await Task.WhenAll(tasks);
    stopwatch.Stop();
    
    var totalRequests = concurrentRequests * requestsPerThread;
    var requestsPerSecond = totalRequests / stopwatch.Elapsed.TotalSeconds;
    
    _output.WriteLine($"Completed {totalRequests} requests in {stopwatch.Elapsed.TotalSeconds:F2}s");
    _output.WriteLine($"Requests per second: {requestsPerSecond:F2}");
    
    Assert.True(requestsPerSecond > 10, "Should handle at least 10 requests per second");
}
```

### 2. Memory Usage Testing

```csharp
[Fact]
public async Task MemoryTest_StreamingVsBatch_ShouldUseConstantMemory()
{
    var initialMemory = GC.GetTotalMemory(true);
    
    // Test streaming approach
    var count = 0;
    await foreach (var person in _peopleService.StreamAsync())
    {
        count++;
        if (count >= 1000) break;
    }
    
    var streamingMemory = GC.GetTotalMemory(true);
    var memoryIncrease = streamingMemory - initialMemory;
    
    _output.WriteLine($"Memory increase with streaming: {memoryIncrease / 1024 / 1024:F2} MB");
    
    // Memory increase should be minimal (< 10MB)
    Assert.True(memoryIncrease < 10 * 1024 * 1024, "Streaming should use constant memory");
}
```

## Troubleshooting Performance Issues

### 1. Common Performance Problems

#### Slow API Responses
```csharp
// Check if you're requesting too much data
var queryParams = new QueryParameters
{
    PerPage = 25, // Reduce page size
    Include = ["emails"], // Limit includes
    Fields = new Dictionary<string, string[]>
    {
        ["person"] = ["first_name", "last_name"] // Limit fields
    }
};
```

#### High Memory Usage
```csharp
// Replace batch loading with streaming
// ❌ High memory
var allPeople = await peopleService.GetAllAsync();

// ✅ Low memory
await foreach (var person in peopleService.StreamAsync())
{
    // Process immediately
}
```

#### Rate Limiting Issues
```csharp
// Implement exponential backoff
public async Task<T> WithRetryAsync<T>(Func<Task<T>> operation)
{
    var delay = TimeSpan.FromSeconds(1);
    
    for (int attempt = 0; attempt < 5; attempt++)
    {
        try
        {
            return await operation();
        }
        catch (PlanningCenterApiRateLimitException)
        {
            await Task.Delay(delay);
            delay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * 2);
        }
    }
    
    throw new InvalidOperationException("Max retry attempts exceeded");
}
```

### 2. Performance Monitoring

```csharp
// Add performance logging
builder.Services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

// Monitor correlation IDs for request tracking
// Check logs for patterns like:
// "PeopleService.ListAsync completed in 245ms [CorrelationId: abc123]"
```

---

*Last Updated: 2024-11-20*