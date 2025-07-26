# Best Practices Guide

This guide covers best practices, patterns, and recommendations for using the Planning Center API SDK for .NET effectively in production applications.

## Table of Contents

- [Authentication Best Practices](#authentication-best-practices)
- [Performance Optimization](#performance-optimization)
- [Error Handling Patterns](#error-handling-patterns)
- [Logging and Monitoring](#logging-and-monitoring)
- [Caching Strategies](#caching-strategies)
- [Data Processing Patterns](#data-processing-patterns)
- [Testing Strategies](#testing-strategies)
- [Production Deployment](#production-deployment)
- [Security Considerations](#security-considerations)
- [Code Organization](#code-organization)

## Authentication Best Practices

### 1. Secure Token Management

```csharp
// ✅ DO: Use secure configuration
public class PlanningCenterConfig
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddPlanningCenterApiClient(options =>
        {
            // Use configuration or environment variables
            options.PersonalAccessToken = configuration["PlanningCenter:PAT"]
                ?? Environment.GetEnvironmentVariable("PLANNING_CENTER_PAT")
                ?? throw new InvalidOperationException("Planning Center PAT is required");
        });
    }
}

// ❌ DON'T: Hardcode tokens
services.AddPlanningCenterApiClientWithPAT("app-id:secret"); // Never do this!
```

### 2. Environment-Specific Configuration

```csharp
// appsettings.Development.json
{
  "PlanningCenter": {
    "PersonalAccessToken": "dev-app-id:dev-secret",
    "EnableDetailedLogging": true,
    "RequestTimeout": "00:01:00"
  }
}

// appsettings.Production.json
{
  "PlanningCenter": {
    "PersonalAccessToken": "prod-app-id:prod-secret",
    "EnableDetailedLogging": false,
    "RequestTimeout": "00:00:30",
    "EnableCaching": true,
    "MaxRetryAttempts": 5
  }
}
```

### 3. Token Validation and Health Checks

```csharp
public class PlanningCenterHealthCheck : IHealthCheck
{
    private readonly IPeopleService _peopleService;
    private readonly ILogger<PlanningCenterHealthCheck> _logger;
    
    public PlanningCenterHealthCheck(IPeopleService peopleService, ILogger<PlanningCenterHealthCheck> logger)
    {
        _peopleService = peopleService;
        _logger = logger;
    }
    
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Simple API call to verify authentication
            var result = await _peopleService.ListAsync(new QueryParameters { PerPage = 1 });
            
            return HealthCheckResult.Healthy("Planning Center API is accessible");
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, "Planning Center authentication failed");
            return HealthCheckResult.Unhealthy("Planning Center authentication failed", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Planning Center API health check failed");
            return HealthCheckResult.Degraded("Planning Center API is experiencing issues", ex);
        }
    }
}

// Register health check
services.AddHealthChecks()
    .AddCheck<PlanningCenterHealthCheck>("planning-center");
```

## Performance Optimization

### 1. Use Streaming for Large Datasets

```csharp
// ✅ DO: Use streaming for large datasets
public async Task ProcessAllPeopleAsync()
{
    var processedCount = 0;
    
    await foreach (var person in _peopleService.StreamAsync())
    {
        await ProcessPersonAsync(person);
        processedCount++;
        
        if (processedCount % 100 == 0)
        {
            _logger.LogInformation("Processed {Count} people", processedCount);
        }
    }
}

// ❌ DON'T: Load everything into memory
public async Task ProcessAllPeopleBadAsync()
{
    var allPeople = await _peopleService.GetAllAsync(); // Could be 50,000+ records!
    
    foreach (var person in allPeople)
    {
        await ProcessPersonAsync(person);
    }
}
```

### 2. Optimize Query Patterns

```csharp
// ✅ DO: Use specific queries with filtering
public async Task<List<Person>> GetActiveMembersAsync()
{
    return await _client.Fluent().People
        .Where(p => p.Status == "active")
        .Where(p => p.MembershipStatus == "member")
        .Include("emails") // Only include what you need
        .OrderBy(p => p.LastName)
        .GetAllAsync();
}

// ❌ DON'T: Fetch everything and filter client-side
public async Task<List<Person>> GetActiveMembersBadAsync()
{
    var allPeople = await _peopleService.GetAllAsync();
    
    return allPeople
        .Where(p => p.Status == "active" && p.MembershipStatus == "member")
        .ToList();
}
```

### 3. Parallel Processing

```csharp
// ✅ DO: Process multiple operations in parallel
public async Task<DashboardData> GetDashboardDataAsync()
{
    var tasks = new[]
    {
        GetRecentPeopleAsync(),
        GetRecentDonationsAsync(),
        GetUpcomingEventsAsync(),
        GetServicePlansAsync()
    };
    
    var results = await Task.WhenAll(tasks);
    
    return new DashboardData
    {
        RecentPeople = results[0],
        RecentDonations = results[1],
        UpcomingEvents = results[2],
        ServicePlans = results[3]
    };
}

// ✅ DO: Use SemaphoreSlim for controlled concurrency
private readonly SemaphoreSlim _semaphore = new(5); // Max 5 concurrent operations

public async Task ProcessPeopleInBatchesAsync(IEnumerable<string> personIds)
{
    var tasks = personIds.Select(async id =>
    {
        await _semaphore.WaitAsync();
        try
        {
            return await _peopleService.GetAsync(id);
        }
        finally
        {
            _semaphore.Release();
        }
    });
    
    var results = await Task.WhenAll(tasks);
}
```

### 4. Efficient Pagination

```csharp
// ✅ DO: Use pagination for UI scenarios
public async Task<PagedResult<Person>> GetPeoplePageAsync(int page, int pageSize = 25)
{
    var parameters = new QueryParameters
    {
        PerPage = pageSize,
        Offset = (page - 1) * pageSize,
        Where = new Dictionary<string, object> { ["status"] = "active" },
        OrderBy = "last_name,first_name"
    };
    
    return await _peopleService.ListAsync(parameters);
}

// ✅ DO: Use streaming for background processing
public async Task ProcessAllActiveAsync()
{
    var query = _client.Fluent().People
        .Where(p => p.Status == "active")
        .OrderBy(p => p.Id); // Consistent ordering for streaming
    
    await foreach (var person in query.StreamAsync())
    {
        await ProcessPersonAsync(person);
    }
}
```

## Error Handling Patterns

### 1. Comprehensive Exception Handling

```csharp
public class PlanningCenterService
{
    private readonly IPeopleService _peopleService;
    private readonly ILogger<PlanningCenterService> _logger;
    
    public async Task<Result<Person>> GetPersonSafelyAsync(string personId)
    {
        try
        {
            var person = await _peopleService.GetAsync(personId);
            return Result<Person>.Success(person);
        }
        catch (PlanningCenterApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogWarning("Person {PersonId} not found. CorrelationId: {CorrelationId}", 
                personId, ex.CorrelationId);
            return Result<Person>.NotFound($"Person {personId} not found");
        }
        catch (PlanningCenterApiException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogError(ex, "Authentication failed. CorrelationId: {CorrelationId}", ex.CorrelationId);
            return Result<Person>.Unauthorized("Authentication failed");
        }
        catch (PlanningCenterApiException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
        {
            _logger.LogWarning("Rate limit exceeded. CorrelationId: {CorrelationId}", ex.CorrelationId);
            return Result<Person>.RateLimited("Rate limit exceeded, please try again later");
        }
        catch (PlanningCenterApiException ex)
        {
            _logger.LogError(ex, "API error occurred. Status: {StatusCode}, CorrelationId: {CorrelationId}", 
                ex.StatusCode, ex.CorrelationId);
            return Result<Person>.Error($"API error: {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error occurred while fetching person {PersonId}", personId);
            return Result<Person>.Error("Network error occurred");
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "Request timeout while fetching person {PersonId}", personId);
            return Result<Person>.Timeout("Request timed out");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching person {PersonId}", personId);
            return Result<Person>.Error("An unexpected error occurred");
        }
    }
}

// Result pattern for clean error handling
public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T Data { get; private set; }
    public string ErrorMessage { get; private set; }
    public ResultType Type { get; private set; }
    
    public static Result<T> Success(T data) => new() { IsSuccess = true, Data = data, Type = ResultType.Success };
    public static Result<T> NotFound(string message) => new() { IsSuccess = false, ErrorMessage = message, Type = ResultType.NotFound };
    public static Result<T> Unauthorized(string message) => new() { IsSuccess = false, ErrorMessage = message, Type = ResultType.Unauthorized };
    public static Result<T> RateLimited(string message) => new() { IsSuccess = false, ErrorMessage = message, Type = ResultType.RateLimited };
    public static Result<T> Error(string message) => new() { IsSuccess = false, ErrorMessage = message, Type = ResultType.Error };
    public static Result<T> Timeout(string message) => new() { IsSuccess = false, ErrorMessage = message, Type = ResultType.Timeout };
}

public enum ResultType
{
    Success,
    NotFound,
    Unauthorized,
    RateLimited,
    Error,
    Timeout
}
```

### 2. Retry Patterns with Circuit Breaker

```csharp
public class ResilientPlanningCenterService
{
    private readonly IPeopleService _peopleService;
    private readonly IAsyncPolicy _retryPolicy;
    private readonly IAsyncPolicy _circuitBreakerPolicy;
    private readonly IAsyncPolicy _combinedPolicy;
    
    public ResilientPlanningCenterService(IPeopleService peopleService)
    {
        _peopleService = peopleService;
        
        // Retry policy for transient failures
        _retryPolicy = Policy
            .Handle<HttpRequestException>()
            .Or<TaskCanceledException>()
            .Or<PlanningCenterApiException>(ex => ex.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    Console.WriteLine($"Retry {retryCount} after {timespan} seconds");
                });
        
        // Circuit breaker for cascading failures
        _circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromMinutes(1),
                onBreak: (exception, duration) =>
                {
                    Console.WriteLine($"Circuit breaker opened for {duration}");
                },
                onReset: () =>
                {
                    Console.WriteLine("Circuit breaker reset");
                });
        
        // Combine policies
        _combinedPolicy = Policy.WrapAsync(_retryPolicy, _circuitBreakerPolicy);
    }
    
    public async Task<Person> GetPersonWithResilienceAsync(string personId)
    {
        return await _combinedPolicy.ExecuteAsync(async () =>
        {
            return await _peopleService.GetAsync(personId);
        });
    }
}
```

## Logging and Monitoring

### 1. Structured Logging

```csharp
public class PeopleServiceWrapper
{
    private readonly IPeopleService _peopleService;
    private readonly ILogger<PeopleServiceWrapper> _logger;
    
    public async Task<PagedResult<Person>> ListPeopleAsync(QueryParameters parameters)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["Operation"] = "ListPeople",
            ["PerPage"] = parameters.PerPage,
            ["Offset"] = parameters.Offset
        });
        
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            _logger.LogInformation("Starting to list people with {PerPage} per page, offset {Offset}", 
                parameters.PerPage, parameters.Offset);
            
            var result = await _peopleService.ListAsync(parameters);
            
            stopwatch.Stop();
            
            _logger.LogInformation("Successfully listed {Count} people in {ElapsedMs}ms. Total: {TotalCount}",
                result.Data.Count, stopwatch.ElapsedMilliseconds, result.Meta.TotalCount);
            
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            _logger.LogError(ex, "Failed to list people after {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
```

### 2. Performance Monitoring

```csharp
public class PerformanceMonitoringService
{
    private readonly IMetrics _metrics;
    private readonly ILogger<PerformanceMonitoringService> _logger;
    
    public async Task<T> MonitorAsync<T>(string operationName, Func<Task<T>> operation)
    {
        using var timer = _metrics.Measure.Timer.Time("planning_center_operation_duration", 
            new MetricTags("operation", operationName));
        
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var result = await operation();
            
            _metrics.Measure.Counter.Increment("planning_center_operation_success", 
                new MetricTags("operation", operationName));
            
            return result;
        }
        catch (Exception ex)
        {
            _metrics.Measure.Counter.Increment("planning_center_operation_error", 
                new MetricTags("operation", operationName, "error_type", ex.GetType().Name));
            
            throw;
        }
        finally
        {
            stopwatch.Stop();
            
            if (stopwatch.ElapsedMilliseconds > 5000) // Log slow operations
            {
                _logger.LogWarning("Slow operation detected: {Operation} took {ElapsedMs}ms", 
                    operationName, stopwatch.ElapsedMilliseconds);
            }
        }
    }
}
```

## Caching Strategies

### 1. Smart Caching Patterns

```csharp
public class CachedPeopleService
{
    private readonly IPeopleService _peopleService;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachedPeopleService> _logger;
    
    public async Task<Person> GetPersonAsync(string personId, bool useCache = true)
    {
        var cacheKey = $"person:{personId}";
        
        if (useCache && _cache.TryGetValue(cacheKey, out Person cachedPerson))
        {
            _logger.LogDebug("Cache hit for person {PersonId}", personId);
            return cachedPerson;
        }
        
        _logger.LogDebug("Cache miss for person {PersonId}, fetching from API", personId);
        
        var person = await _peopleService.GetAsync(personId);
        
        // Cache with sliding expiration
        var cacheOptions = new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(15),
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
            Priority = CacheItemPriority.Normal
        };
        
        _cache.Set(cacheKey, person, cacheOptions);
        
        return person;
    }
    
    public async Task<Person> UpdatePersonAsync(string personId, PersonUpdateRequest request)
    {
        var updatedPerson = await _peopleService.UpdateAsync(personId, request);
        
        // Invalidate cache after update
        var cacheKey = $"person:{personId}";
        _cache.Remove(cacheKey);
        
        _logger.LogDebug("Invalidated cache for person {PersonId} after update", personId);
        
        return updatedPerson;
    }
}
```

### 2. Distributed Caching

```csharp
public class DistributedCachedService
{
    private readonly IPeopleService _peopleService;
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<DistributedCachedService> _logger;
    
    public async Task<List<Person>> GetActivePersonsAsync()
    {
        const string cacheKey = "active_persons";
        
        var cachedData = await _distributedCache.GetStringAsync(cacheKey);
        if (cachedData != null)
        {
            _logger.LogDebug("Distributed cache hit for active persons");
            return JsonSerializer.Deserialize<List<Person>>(cachedData);
        }
        
        _logger.LogDebug("Distributed cache miss for active persons, fetching from API");
        
        var persons = await _peopleService.GetAllAsync(new QueryParameters
        {
            Where = new Dictionary<string, object> { ["status"] = "active" }
        });
        
        var serializedData = JsonSerializer.Serialize(persons);
        var cacheOptions = new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(30),
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
        };
        
        await _distributedCache.SetStringAsync(cacheKey, serializedData, cacheOptions);
        
        return persons;
    }
}
```

## Data Processing Patterns

### 1. Batch Processing

```csharp
public class BatchProcessor
{
    private readonly IPeopleService _peopleService;
    private readonly ILogger<BatchProcessor> _logger;
    
    public async Task ProcessPeopleInBatchesAsync(int batchSize = 100)
    {
        var processedCount = 0;
        var batch = new List<Person>();
        
        await foreach (var person in _peopleService.StreamAsync())
        {
            batch.Add(person);
            
            if (batch.Count >= batchSize)
            {
                await ProcessBatchAsync(batch);
                processedCount += batch.Count;
                
                _logger.LogInformation("Processed batch of {BatchSize} people. Total processed: {TotalProcessed}", 
                    batch.Count, processedCount);
                
                batch.Clear();
                
                // Small delay to avoid overwhelming the system
                await Task.Delay(100);
            }
        }
        
        // Process remaining items
        if (batch.Count > 0)
        {
            await ProcessBatchAsync(batch);
            processedCount += batch.Count;
        }
        
        _logger.LogInformation("Completed processing {TotalProcessed} people", processedCount);
    }
    
    private async Task ProcessBatchAsync(List<Person> batch)
    {
        var tasks = batch.Select(ProcessPersonAsync);
        await Task.WhenAll(tasks);
    }
    
    private async Task ProcessPersonAsync(Person person)
    {
        // Your processing logic here
        await Task.Delay(10); // Simulate work
    }
}
```

### 2. Data Synchronization

```csharp
public class DataSynchronizer
{
    private readonly IPeopleService _peopleService;
    private readonly ILocalDataService _localDataService;
    private readonly ILogger<DataSynchronizer> _logger;
    
    public async Task SynchronizePersonsAsync(DateTime? lastSyncTime = null)
    {
        var syncStartTime = DateTime.UtcNow;
        
        try
        {
            var parameters = new QueryParameters
            {
                Where = new Dictionary<string, object>(),
                OrderBy = "updated_at"
            };
            
            if (lastSyncTime.HasValue)
            {
                parameters.Where["updated_at"] = lastSyncTime.Value.ToString("yyyy-MM-ddTHH:mm:ssZ");
            }
            
            var syncedCount = 0;
            
            await foreach (var person in _peopleService.StreamAsync(parameters))
            {
                await _localDataService.UpsertPersonAsync(person);
                syncedCount++;
                
                if (syncedCount % 100 == 0)
                {
                    _logger.LogInformation("Synchronized {Count} people so far", syncedCount);
                }
            }
            
            await _localDataService.UpdateLastSyncTimeAsync(syncStartTime);
            
            _logger.LogInformation("Synchronization completed. Synced {Count} people", syncedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Synchronization failed");
            throw;
        }
    }
}
```

## Testing Strategies

### 1. Unit Testing with Mocks

```csharp
public class PeopleServiceTests
{
    private readonly Mock<IPeopleService> _mockPeopleService;
    private readonly PeopleManager _peopleManager;
    
    public PeopleServiceTests()
    {
        _mockPeopleService = new Mock<IPeopleService>();
        _peopleManager = new PeopleManager(_mockPeopleService.Object);
    }
    
    [Fact]
    public async Task GetActivePersonsAsync_ReturnsOnlyActivePersons()
    {
        // Arrange
        var mockData = new PagedResult<Person>
        {
            Data = new List<Person>
            {
                new() { Id = "1", FirstName = "John", LastName = "Doe", Status = "active" },
                new() { Id = "2", FirstName = "Jane", LastName = "Smith", Status = "active" }
            },
            Meta = new PageMeta { TotalCount = 2, CurrentPage = 1, TotalPages = 1 }
        };
        
        _mockPeopleService
            .Setup(x => x.ListAsync(It.Is<QueryParameters>(p => 
                p.Where.ContainsKey("status") && p.Where["status"].ToString() == "active")))
            .ReturnsAsync(mockData);
        
        // Act
        var result = await _peopleManager.GetActivePersonsAsync();
        
        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, person => Assert.Equal("active", person.Status));
        
        _mockPeopleService.Verify(x => x.ListAsync(It.IsAny<QueryParameters>()), Times.Once);
    }
}
```

### 2. Integration Testing

```csharp
public class PlanningCenterIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    
    public PlanningCenterIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }
    
    [Fact]
    public async Task GetPeople_WithValidToken_ReturnsSuccess()
    {
        // Arrange
        var testToken = Environment.GetEnvironmentVariable("TEST_PLANNING_CENTER_PAT");
        
        if (string.IsNullOrEmpty(testToken))
        {
            Skip.If(true, "Test token not available");
        }
        
        // Act
        var response = await _client.GetAsync("/api/people?limit=5");
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        var people = JsonSerializer.Deserialize<List<Person>>(content);
        
        Assert.NotNull(people);
        Assert.True(people.Count <= 5);
    }
}
```

## Production Deployment

### 1. Configuration Management

```csharp
// Program.cs
public static void Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);
    
    // Environment-specific configuration
    builder.Configuration
        .AddJsonFile("appsettings.json", optional: false)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
        .AddEnvironmentVariables()
        .AddUserSecrets<Program>(optional: true);
    
    // Validate configuration
    var planningCenterSection = builder.Configuration.GetSection("PlanningCenter");
    builder.Services.Configure<PlanningCenterOptions>(planningCenterSection);
    builder.Services.AddOptions<PlanningCenterOptions>()
        .Bind(planningCenterSection)
        .ValidateDataAnnotations()
        .ValidateOnStart();
    
    // Add services
    ConfigureServices(builder.Services, builder.Configuration);
    
    var app = builder.Build();
    
    // Validate configuration on startup
    var options = app.Services.GetRequiredService<IOptions<PlanningCenterOptions>>();
    ValidateConfiguration(options.Value);
    
    app.Run();
}

private static void ValidateConfiguration(PlanningCenterOptions options)
{
    if (string.IsNullOrEmpty(options.PersonalAccessToken) && 
        string.IsNullOrEmpty(options.AccessToken))
    {
        throw new InvalidOperationException("Planning Center authentication token is required");
    }
    
    if (options.RequestTimeout <= TimeSpan.Zero)
    {
        throw new InvalidOperationException("Request timeout must be positive");
    }
}
```

### 2. Health Checks and Monitoring

```csharp
// Configure comprehensive health checks
services.AddHealthChecks()
    .AddCheck<PlanningCenterHealthCheck>("planning-center")
    .AddCheck("memory", () =>
    {
        var allocated = GC.GetTotalMemory(false);
        var threshold = 1024 * 1024 * 1024; // 1GB
        
        return allocated < threshold 
            ? HealthCheckResult.Healthy($"Memory usage: {allocated / 1024 / 1024}MB")
            : HealthCheckResult.Degraded($"High memory usage: {allocated / 1024 / 1024}MB");
    })
    .AddCheck("disk-space", () =>
    {
        var drive = new DriveInfo("C:");
        var freeSpaceGB = drive.AvailableFreeSpace / 1024 / 1024 / 1024;
        
        return freeSpaceGB > 1 
            ? HealthCheckResult.Healthy($"Free disk space: {freeSpaceGB}GB")
            : HealthCheckResult.Unhealthy($"Low disk space: {freeSpaceGB}GB");
    });

// Add metrics
services.AddSingleton<IMetrics, MetricsRoot>();

// Configure logging
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.AddApplicationInsights(); // For Azure
    builder.AddSerilog(); // For structured logging
});
```

## Security Considerations

### 1. Secure Configuration

```csharp
// Use Azure Key Vault in production
if (builder.Environment.IsProduction())
{
    var keyVaultUrl = builder.Configuration["KeyVault:Url"];
    if (!string.IsNullOrEmpty(keyVaultUrl))
    {
        builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUrl), new DefaultAzureCredential());
    }
}

// Validate sensitive configuration
builder.Services.AddOptions<PlanningCenterOptions>()
    .Configure<IConfiguration>((options, config) =>
    {
        config.GetSection("PlanningCenter").Bind(options);
        
        // Ensure token is not logged
        if (!string.IsNullOrEmpty(options.PersonalAccessToken))
        {
            options.PersonalAccessToken = options.PersonalAccessToken.Trim();
        }
    })
    .Validate(options => !string.IsNullOrEmpty(options.PersonalAccessToken), 
        "Personal Access Token is required")
    .ValidateOnStart();
```

### 2. Request Sanitization

```csharp
public class SecurePeopleService
{
    private readonly IPeopleService _peopleService;
    
    public async Task<Person> GetPersonAsync(string personId)
    {
        // Validate and sanitize input
        if (string.IsNullOrWhiteSpace(personId))
        {
            throw new ArgumentException("Person ID cannot be null or empty", nameof(personId));
        }
        
        // Ensure it's a valid ID format (numeric)
        if (!long.TryParse(personId, out _))
        {
            throw new ArgumentException("Person ID must be numeric", nameof(personId));
        }
        
        return await _peopleService.GetAsync(personId);
    }
    
    public async Task<PagedResult<Person>> SearchPeopleAsync(string searchTerm)
    {
        // Sanitize search input
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            throw new ArgumentException("Search term cannot be null or empty", nameof(searchTerm));
        }
        
        // Limit search term length
        if (searchTerm.Length > 100)
        {
            throw new ArgumentException("Search term too long", nameof(searchTerm));
        }
        
        // Remove potentially dangerous characters
        var sanitizedTerm = Regex.Replace(searchTerm, @"[<>""'%;()&+]", "");
        
        var parameters = new QueryParameters
        {
            Where = new Dictionary<string, object>
            {
                ["search_name"] = sanitizedTerm
            }
        };
        
        return await _peopleService.ListAsync(parameters);
    }
}
```

## Code Organization

### 1. Service Layer Pattern

```csharp
// Domain layer
public interface IPeopleRepository
{
    Task<Person> GetAsync(string id);
    Task<List<Person>> GetActiveAsync();
    Task<Person> CreateAsync(CreatePersonRequest request);
}

// Infrastructure layer
public class PlanningCenterPeopleRepository : IPeopleRepository
{
    private readonly IPeopleService _peopleService;
    
    public async Task<Person> GetAsync(string id)
    {
        return await _peopleService.GetAsync(id);
    }
    
    public async Task<List<Person>> GetActiveAsync()
    {
        return await _peopleService.GetAllAsync(new QueryParameters
        {
            Where = new Dictionary<string, object> { ["status"] = "active" }
        });
    }
}

// Application layer
public class PeopleApplicationService
{
    private readonly IPeopleRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<PeopleApplicationService> _logger;
    
    public async Task<PersonDto> GetPersonAsync(string id)
    {
        var person = await _repository.GetAsync(id);
        return _mapper.Map<PersonDto>(person);
    }
}
```

### 2. Dependency Injection Setup

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPlanningCenterServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Core SDK
        services.AddPlanningCenterApiClient(configuration.GetSection("PlanningCenter"));
        
        // Repositories
        services.AddScoped<IPeopleRepository, PlanningCenterPeopleRepository>();
        services.AddScoped<IGivingRepository, PlanningCenterGivingRepository>();
        
        // Application services
        services.AddScoped<PeopleApplicationService>();
        services.AddScoped<GivingApplicationService>();
        
        // Decorators
        services.Decorate<IPeopleRepository, CachedPeopleRepository>();
        services.Decorate<IPeopleRepository, LoggingPeopleRepository>();
        
        // Background services
        services.AddHostedService<DataSynchronizationService>();
        
        return services;
    }
}
```

This comprehensive guide covers the essential patterns and practices for building robust, maintainable applications with the Planning Center API SDK. Follow these guidelines to ensure your application is secure, performant, and production-ready.