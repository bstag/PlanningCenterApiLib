# Troubleshooting Guide

This guide helps you diagnose and resolve common issues when using the Planning Center API SDK for .NET.

## Table of Contents

- [Authentication Issues](#authentication-issues)
- [API Request Problems](#api-request-problems)
- [Performance Issues](#performance-issues)
- [Configuration Problems](#configuration-problems)
- [Dependency Injection Issues](#dependency-injection-issues)
- [Fluent API Problems](#fluent-api-problems)
- [CLI Tool Issues](#cli-tool-issues)
- [Network and Connectivity](#network-and-connectivity)
- [Memory and Resource Issues](#memory-and-resource-issues)
- [Debugging Techniques](#debugging-techniques)

## Authentication Issues

### Issue: "Invalid authentication credentials"

**Symptoms:**
```
PlanningCenterApiException: Invalid authentication credentials
Status Code: 401 Unauthorized
```

**Common Causes & Solutions:**

#### 1. Incorrect Token Format

```csharp
// ❌ Wrong format
var token = "app-id";
var token = "secret";
var token = "app-id secret"; // Space instead of colon

// ✅ Correct format
var token = "app-id:secret";
```

#### 2. Token Not Set

```csharp
// Check if token is properly configured
public void ValidateConfiguration()
{
    var token = Environment.GetEnvironmentVariable("PLANNING_CENTER_PAT");
    if (string.IsNullOrEmpty(token))
    {
        throw new InvalidOperationException("PLANNING_CENTER_PAT environment variable is not set");
    }
    
    if (!token.Contains(':'))
    {
        throw new InvalidOperationException("Token must be in format 'app-id:secret'");
    }
    
    Console.WriteLine($"Token configured: {token.Substring(0, 8)}...");
}
```

#### 3. Expired or Revoked Token

**Solution:** Generate a new Personal Access Token:

1. Log in to Planning Center
2. Go to Settings → Developer → Personal Access Tokens
3. Delete the old token
4. Create a new token with required permissions
5. Update your configuration

#### 4. Insufficient Permissions

```csharp
// Test token permissions
public async Task TestTokenPermissionsAsync()
{
    try
    {
        var peopleService = serviceProvider.GetRequiredService<IPeopleService>();
        var people = await peopleService.ListAsync(new QueryParameters { PerPage = 1 });
        Console.WriteLine("✅ People access: OK");
    }
    catch (UnauthorizedAccessException)
    {
        Console.WriteLine("❌ People access: DENIED");
    }
    
    try
    {
        var givingService = serviceProvider.GetRequiredService<IGivingService>();
        var donations = await givingService.ListDonationsAsync(new QueryParameters { PerPage = 1 });
        Console.WriteLine("✅ Giving access: OK");
    }
    catch (UnauthorizedAccessException)
    {
        Console.WriteLine("❌ Giving access: DENIED");
    }
}
```

### Issue: OAuth Token Refresh Failures

**Symptoms:**
```
PlanningCenterApiException: Token has expired
Status Code: 401 Unauthorized
```

**Solution:**

```csharp
public class TokenRefreshService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TokenRefreshService> _logger;
    
    public async Task<bool> RefreshTokenAsync()
    {
        try
        {
            var refreshToken = _configuration["PlanningCenter:RefreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogError("Refresh token not found in configuration");
                return false;
            }
            
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.planningcenteronline.com/oauth/token")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["grant_type"] = "refresh_token",
                    ["client_id"] = _configuration["PlanningCenter:ClientId"],
                    ["client_secret"] = _configuration["PlanningCenter:ClientSecret"],
                    ["refresh_token"] = refreshToken
                })
            };
            
            var response = await _httpClient.SendAsync(request);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Token refresh failed: {StatusCode} - {Content}", 
                    response.StatusCode, errorContent);
                return false;
            }
            
            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
            
            // Update configuration with new tokens
            // Note: In production, store these securely
            _configuration["PlanningCenter:AccessToken"] = tokenResponse.AccessToken;
            _configuration["PlanningCenter:RefreshToken"] = tokenResponse.RefreshToken;
            
            _logger.LogInformation("Token refreshed successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return false;
        }
    }
}
```

## API Request Problems

### Issue: "Rate limit exceeded"

**Symptoms:**
```
PlanningCenterApiException: Rate limit exceeded
Status Code: 429 Too Many Requests
```

**Solutions:**

#### 1. Configure Retry Policy

```csharp
builder.Services.Configure<PlanningCenterOptions>(options =>
{
    options.MaxRetryAttempts = 5;
    options.RetryBaseDelay = TimeSpan.FromSeconds(2);
    options.RetryMaxDelay = TimeSpan.FromMinutes(1);
});
```

#### 2. Implement Custom Backoff

```csharp
public class RateLimitHandler
{
    private readonly IPeopleService _peopleService;
    private readonly ILogger<RateLimitHandler> _logger;
    
    public async Task<T> ExecuteWithBackoffAsync<T>(Func<Task<T>> operation, int maxAttempts = 5)
    {
        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                return await operation();
            }
            catch (PlanningCenterApiException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
            {
                if (attempt == maxAttempts)
                    throw;
                
                var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
                _logger.LogWarning("Rate limit hit, waiting {Delay} seconds before retry {Attempt}/{MaxAttempts}", 
                    delay.TotalSeconds, attempt, maxAttempts);
                
                await Task.Delay(delay);
            }
        }
        
        throw new InvalidOperationException("Should not reach here");
    }
}
```

#### 3. Monitor Rate Limits

```csharp
public class RateLimitMonitor
{
    public void LogRateLimitInfo(HttpResponseMessage response)
    {
        if (response.Headers.TryGetValues("X-PCO-API-Request-Rate-Count", out var countValues) &&
            response.Headers.TryGetValues("X-PCO-API-Request-Rate-Period", out var periodValues))
        {
            var count = countValues.FirstOrDefault();
            var period = periodValues.FirstOrDefault();
            
            Console.WriteLine($"Rate limit: {count} requests in {period} seconds");
            
            if (int.TryParse(count, out var requestCount) && requestCount > 80)
            {
                Console.WriteLine("⚠️ Approaching rate limit!");
            }
        }
    }
}
```

### Issue: "Resource not found"

**Symptoms:**
```
PlanningCenterApiException: Resource not found
Status Code: 404 Not Found
```

**Solutions:**

#### 1. Validate IDs Before Requests

```csharp
public async Task<Person> GetPersonSafelyAsync(string personId)
{
    // Validate ID format
    if (string.IsNullOrWhiteSpace(personId))
    {
        throw new ArgumentException("Person ID cannot be null or empty");
    }
    
    if (!long.TryParse(personId, out _))
    {
        throw new ArgumentException("Person ID must be numeric");
    }
    
    try
    {
        return await _peopleService.GetAsync(personId);
    }
    catch (PlanningCenterApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
    {
        _logger.LogWarning("Person {PersonId} not found", personId);
        return null; // or throw a more specific exception
    }
}
```

#### 2. Check Resource Existence

```csharp
public async Task<bool> PersonExistsAsync(string personId)
{
    try
    {
        await _peopleService.GetAsync(personId);
        return true;
    }
    catch (PlanningCenterApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
    {
        return false;
    }
}
```

### Issue: Request Timeout

**Symptoms:**
```
TaskCanceledException: The operation was canceled.
HttpRequestException: The request timed out.
```

**Solutions:**

#### 1. Increase Timeout

```csharp
builder.Services.Configure<PlanningCenterOptions>(options =>
{
    options.RequestTimeout = TimeSpan.FromMinutes(2); // Increase from default 30 seconds
});
```

#### 2. Use Cancellation Tokens

```csharp
public async Task<List<Person>> GetPeopleWithTimeoutAsync()
{
    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
    
    try
    {
        return await _peopleService.GetAllAsync(cancellationToken: cts.Token);
    }
    catch (OperationCanceledException) when (cts.Token.IsCancellationRequested)
    {
        _logger.LogWarning("Request timed out after 30 seconds");
        throw new TimeoutException("Request timed out");
    }
}
```

## Performance Issues

### Issue: Slow API Responses

**Symptoms:**
- Requests taking longer than expected
- Application feels sluggish
- High memory usage

**Solutions:**

#### 1. Use Streaming for Large Datasets

```csharp
// ❌ Slow: Loading everything into memory
public async Task ProcessAllPeopleBadAsync()
{
    var allPeople = await _peopleService.GetAllAsync(); // Could be 50,000+ records
    foreach (var person in allPeople)
    {
        await ProcessPersonAsync(person);
    }
}

// ✅ Fast: Streaming
public async Task ProcessAllPeopleGoodAsync()
{
    await foreach (var person in _peopleService.StreamAsync())
    {
        await ProcessPersonAsync(person);
    }
}
```

#### 2. Optimize Queries

```csharp
// ❌ Inefficient: Multiple requests
public async Task<List<PersonWithEmails>> GetPeopleWithEmailsBadAsync()
{
    var people = await _peopleService.GetAllAsync();
    var result = new List<PersonWithEmails>();
    
    foreach (var person in people)
    {
        var emails = await _peopleService.GetEmailsAsync(person.Id); // N+1 problem!
        result.Add(new PersonWithEmails { Person = person, Emails = emails });
    }
    
    return result;
}

// ✅ Efficient: Single request with includes
public async Task<List<Person>> GetPeopleWithEmailsGoodAsync()
{
    return await _client.Fluent().People
        .Include("emails")
        .GetAllAsync();
}
```

#### 3. Enable Caching

```csharp
builder.Services.Configure<PlanningCenterOptions>(options =>
{
    options.EnableCaching = true;
    options.DefaultCacheExpiration = TimeSpan.FromMinutes(5);
});
```

### Issue: Memory Leaks

**Symptoms:**
- Increasing memory usage over time
- OutOfMemoryException
- Application crashes

**Solutions:**

#### 1. Proper Disposal

```csharp
// ✅ Use using statements
public async Task ProcessDataAsync()
{
    using var scope = _serviceProvider.CreateScope();
    var peopleService = scope.ServiceProvider.GetRequiredService<IPeopleService>();
    
    await foreach (var person in peopleService.StreamAsync())
    {
        await ProcessPersonAsync(person);
    }
} // Scope is disposed here
```

#### 2. Monitor Memory Usage

```csharp
public class MemoryMonitor
{
    private readonly ILogger<MemoryMonitor> _logger;
    
    public void LogMemoryUsage(string operation)
    {
        var beforeGC = GC.GetTotalMemory(false);
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        var afterGC = GC.GetTotalMemory(false);
        
        _logger.LogInformation("{Operation} - Memory before GC: {BeforeGC:N0} bytes, after GC: {AfterGC:N0} bytes",
            operation, beforeGC, afterGC);
    }
}
```

## Configuration Problems

### Issue: Configuration Not Loading

**Symptoms:**
```
InvalidOperationException: Planning Center configuration is missing
ArgumentNullException: Value cannot be null. (Parameter 'token')
```

**Solutions:**

#### 1. Verify Configuration Files

```csharp
public static void ValidateConfiguration(IConfiguration configuration)
{
    var section = configuration.GetSection("PlanningCenter");
    
    if (!section.Exists())
    {
        throw new InvalidOperationException("PlanningCenter configuration section not found");
    }
    
    var token = section["PersonalAccessToken"];
    if (string.IsNullOrEmpty(token))
    {
        throw new InvalidOperationException("PersonalAccessToken not configured");
    }
    
    Console.WriteLine("✅ Configuration validated successfully");
}
```

#### 2. Debug Configuration Loading

```csharp
public static void DebugConfiguration(IConfiguration configuration)
{
    Console.WriteLine("Configuration Sources:");
    foreach (var source in ((IConfigurationRoot)configuration).Providers)
    {
        Console.WriteLine($"- {source.GetType().Name}");
    }
    
    Console.WriteLine("\nPlanningCenter Configuration:");
    var section = configuration.GetSection("PlanningCenter");
    foreach (var child in section.GetChildren())
    {
        var value = child.Key == "PersonalAccessToken" 
            ? "***HIDDEN***" 
            : child.Value;
        Console.WriteLine($"- {child.Key}: {value}");
    }
}
```

### Issue: Environment Variables Not Working

**Solutions:**

#### 1. Check Environment Variable Names

```bash
# Correct format for nested configuration
PlanningCenter__PersonalAccessToken=app-id:secret
PlanningCenter__RequestTimeout=00:01:00
PlanningCenter__EnableCaching=true

# Or use the simple format
PLANNING_CENTER_PAT=app-id:secret
```

#### 2. Verify Environment Variable Loading

```csharp
public static void CheckEnvironmentVariables()
{
    var envVars = new[]
    {
        "PlanningCenter__PersonalAccessToken",
        "PLANNING_CENTER_PAT",
        "PlanningCenter__RequestTimeout"
    };
    
    foreach (var envVar in envVars)
    {
        var value = Environment.GetEnvironmentVariable(envVar);
        Console.WriteLine($"{envVar}: {(string.IsNullOrEmpty(value) ? "NOT SET" : "SET")}");
    }
}
```

## Dependency Injection Issues

### Issue: Service Not Registered

**Symptoms:**
```
InvalidOperationException: Unable to resolve service for type 'IPeopleService'
```

**Solutions:**

#### 1. Verify Service Registration

```csharp
// Make sure this is called
builder.Services.AddPlanningCenterApiClientWithPAT("your-token");

// Or
builder.Services.AddPlanningCenterApiClient(options =>
{
    options.PersonalAccessToken = "your-token";
});
```

#### 2. Check Service Lifetime

```csharp
public static void DebugServiceRegistration(IServiceCollection services)
{
    var planningCenterServices = services
        .Where(s => s.ServiceType.Namespace?.Contains("PlanningCenter") == true)
        .ToList();
    
    Console.WriteLine("Registered Planning Center Services:");
    foreach (var service in planningCenterServices)
    {
        Console.WriteLine($"- {service.ServiceType.Name} ({service.Lifetime})");
    }
}
```

### Issue: Circular Dependencies

**Symptoms:**
```
InvalidOperationException: A circular dependency was detected
```

**Solutions:**

#### 1. Use Factory Pattern

```csharp
public interface IPeopleServiceFactory
{
    IPeopleService CreatePeopleService();
}

public class PeopleServiceFactory : IPeopleServiceFactory
{
    private readonly IServiceProvider _serviceProvider;
    
    public PeopleServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public IPeopleService CreatePeopleService()
    {
        return _serviceProvider.GetRequiredService<IPeopleService>();
    }
}
```

## Fluent API Problems

### Issue: Fluent Query Not Working

**Symptoms:**
```
NotSupportedException: This query operation is not supported
InvalidOperationException: Query could not be translated
```

**Solutions:**

#### 1. Check Supported Operations

```csharp
// ✅ Supported operations
var people = await _client.Fluent().People
    .Where(p => p.Status == "active")
    .Where(p => p.FirstName.Contains("John"))
    .OrderBy(p => p.LastName)
    .Take(10)
    .GetAllAsync();

// ❌ Unsupported operations
var people = await _client.Fluent().People
    .Where(p => p.CreatedAt.Year == 2024) // Complex date operations not supported
    .Where(p => p.FirstName.ToLower() == "john") // String methods not supported
    .GetAllAsync();
```

#### 2. Use Raw Queries for Complex Scenarios

```csharp
// When fluent API doesn't support your query
var parameters = new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["created_at"] = "2024-01-01..2024-12-31",
        ["first_name"] = "John"
    },
    OrderBy = "last_name"
};

var people = await _peopleService.ListAsync(parameters);
```

## CLI Tool Issues

### Issue: CLI Commands Not Working

**Symptoms:**
```
Error: Command 'people' not found
Error: Invalid token format
```

**Solutions:**

#### 1. Verify CLI Setup

```bash
# Navigate to CLI directory
cd examples/PlanningCenter.Api.Client.CLI

# Build the project
dotnet build

# Check if it runs
dotnet run -- --help
```

#### 2. Configure Authentication

```bash
# Set token
dotnet run -- config set-token "your-app-id:your-secret"

# Verify configuration
dotnet run -- config show

# Test connection
dotnet run -- people list --limit 1
```

#### 3. Debug CLI Issues

```bash
# Enable verbose logging
dotnet run -- people list --verbose

# Check specific command help
dotnet run -- people list --help
```

## Network and Connectivity

### Issue: SSL/TLS Errors

**Symptoms:**
```
HttpRequestException: The SSL connection could not be established
AuthenticationException: The remote certificate is invalid
```

**Solutions:**

#### 1. Update .NET Runtime

```bash
# Check .NET version
dotnet --version

# Update to latest
dotnet --list-sdks
```

#### 2. Configure HTTP Client

```csharp
builder.Services.Configure<PlanningCenterOptions>(options =>
{
    options.HttpClientConfigureAction = httpClient =>
    {
        httpClient.Timeout = TimeSpan.FromMinutes(2);
    };
});
```

### Issue: Proxy Configuration

**Solutions:**

```csharp
builder.Services.Configure<PlanningCenterOptions>(options =>
{
    options.HttpClientConfigureAction = httpClient =>
    {
        var proxy = new WebProxy("http://proxy.company.com:8080")
        {
            Credentials = new NetworkCredential("username", "password")
        };
        
        var handler = new HttpClientHandler()
        {
            Proxy = proxy,
            UseProxy = true
        };
        
        // Note: This requires custom HttpClient configuration
    };
});
```

## Memory and Resource Issues

### Issue: OutOfMemoryException

**Solutions:**

#### 1. Use Streaming

```csharp
// ❌ Memory intensive
var allPeople = await _peopleService.GetAllAsync();

// ✅ Memory efficient
await foreach (var person in _peopleService.StreamAsync())
{
    // Process one at a time
}
```

#### 2. Implement Batching

```csharp
public async Task ProcessInBatchesAsync(int batchSize = 100)
{
    var batch = new List<Person>();
    
    await foreach (var person in _peopleService.StreamAsync())
    {
        batch.Add(person);
        
        if (batch.Count >= batchSize)
        {
            await ProcessBatchAsync(batch);
            batch.Clear();
            
            // Force garbage collection
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
    
    if (batch.Count > 0)
    {
        await ProcessBatchAsync(batch);
    }
}
```

## Debugging Techniques

### 1. Enable Detailed Logging

```csharp
builder.Services.Configure<PlanningCenterOptions>(options =>
{
    options.EnableDetailedLogging = true; // Only in development!
});

builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
```

### 2. Correlation ID Tracking

```csharp
public async Task DebugWithCorrelationAsync()
{
    try
    {
        var people = await _peopleService.ListAsync();
        Console.WriteLine($"Success! Retrieved {people.Data.Count} people");
    }
    catch (PlanningCenterApiException ex)
    {
        Console.WriteLine($"Error occurred:");
        Console.WriteLine($"- Status Code: {ex.StatusCode}");
        Console.WriteLine($"- Message: {ex.Message}");
        Console.WriteLine($"- Correlation ID: {ex.CorrelationId}");
        Console.WriteLine($"- Request ID: {ex.RequestId}");
        
        if (ex.ErrorDetails != null)
        {
            Console.WriteLine($"- Error Details: {ex.ErrorDetails}");
        }
    }
}
```

### 3. Network Debugging

```csharp
public class NetworkDebugger
{
    public static void LogHttpTraffic()
    {
        // Enable HTTP logging (development only)
        Environment.SetEnvironmentVariable("DOTNET_SYSTEM_NET_HTTP_SOCKETSHTTPHANDLER_HTTP2SUPPORT", "false");
        Environment.SetEnvironmentVariable("DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER", "false");
    }
}
```

### 4. Performance Profiling

```csharp
public class PerformanceProfiler
{
    public static async Task<T> ProfileAsync<T>(string operationName, Func<Task<T>> operation)
    {
        var stopwatch = Stopwatch.StartNew();
        var memoryBefore = GC.GetTotalMemory(false);
        
        try
        {
            var result = await operation();
            
            stopwatch.Stop();
            var memoryAfter = GC.GetTotalMemory(false);
            
            Console.WriteLine($"Operation: {operationName}");
            Console.WriteLine($"Duration: {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"Memory delta: {(memoryAfter - memoryBefore):N0} bytes");
            
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Console.WriteLine($"Operation failed: {operationName} after {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"Error: {ex.Message}");
            throw;
        }
    }
}
```

## Getting Help

If you're still experiencing issues after trying these solutions:

### 1. Check Documentation
- [Getting Started Guide](GETTING_STARTED.md)
- [Authentication Guide](AUTHENTICATION.md)
- [Best Practices](BEST_PRACTICES.md)
- [Planning Center API Docs](https://developer.planning.center/docs/)

### 2. Search Existing Issues
- Check [GitHub Issues](https://github.com/your-repo/issues)
- Search for similar problems
- Look at closed issues for solutions

### 3. Create a New Issue

When creating an issue, include:

```
**Environment:**
- .NET Version: [e.g., .NET 8.0]
- SDK Version: [e.g., 1.1.0]
- Operating System: [e.g., Windows 11, Ubuntu 22.04]

**Problem Description:**
[Clear description of the issue]

**Steps to Reproduce:**
1. [First step]
2. [Second step]
3. [Third step]

**Expected Behavior:**
[What you expected to happen]

**Actual Behavior:**
[What actually happened]

**Code Sample:**
```csharp
// Minimal code that reproduces the issue
```

**Error Messages:**
```
[Full error message and stack trace]
```

**Additional Context:**
[Any other relevant information]
```

### 4. Enable Debug Mode

Before reporting issues, enable debug mode to get more information:

```csharp
builder.Services.Configure<PlanningCenterOptions>(options =>
{
    options.EnableDetailedLogging = true;
});

builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
```

This will provide detailed logs that can help diagnose the issue.

---

**Remember:** Most issues are configuration-related. Double-check your authentication setup, configuration files, and environment variables before diving into complex debugging.