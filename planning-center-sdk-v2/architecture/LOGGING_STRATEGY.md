# Logging Strategy for Planning Center SDK

## Overview

The Planning Center SDK implements a comprehensive logging strategy that provides valuable insights for debugging, monitoring, and troubleshooting while giving consumers full control over logging levels and output. The SDK uses Microsoft.Extensions.Logging for consistency with .NET ecosystem standards.

## Logging Levels and Usage

### 1. **Trace** - Ultra-detailed execution flow
**When to use:** Extremely detailed information for deep debugging
**Performance Impact:** High - only for development/troubleshooting
**Examples:**
- Method entry/exit with parameters
- Loop iterations and detailed state changes
- Internal algorithm steps

```csharp
_logger.LogTrace("Entering {Method} with parameters: {@Parameters}", 
    nameof(GetPersonAsync), new { personId, includeRelated });

_logger.LogTrace("Processing page {PageNumber} of {TotalPages} for query: {Query}", 
    currentPage, totalPages, queryString);

_logger.LogTrace("Mapping {SourceType} to {TargetType} with {PropertyCount} properties", 
    sourceType.Name, targetType.Name, propertyCount);
```

### 2. **Debug** - Development and troubleshooting information
**When to use:** Information useful during development and debugging
**Performance Impact:** Medium - acceptable for development environments
**Examples:**
- API request/response details
- Cache hits/misses
- Authentication token refresh
- Query parameter construction

```csharp
_logger.LogDebug("Making {Method} request to {Endpoint} with query: {Query}", 
    httpMethod, endpoint, queryParameters);

_logger.LogDebug("Cache {CacheResult} for key: {CacheKey}", 
    cacheHit ? "HIT" : "MISS", cacheKey);

_logger.LogDebug("Token expires at {ExpiryTime}, refresh needed: {RefreshNeeded}", 
    tokenExpiry, needsRefresh);

_logger.LogDebug("Constructed query with {ParameterCount} parameters: {QueryString}", 
    parameters.Count, queryString);
```

### 3. **Information** - General operational information
**When to use:** Important application flow and business events
**Performance Impact:** Low - suitable for production
**Examples:**
- Successful API operations
- Authentication events
- Webhook deliveries
- Configuration changes

```csharp
_logger.LogInformation("Successfully retrieved {Count} people from Planning Center", 
    people.Count);

_logger.LogInformation("User authenticated successfully with client ID: {ClientId}", 
    clientId);

_logger.LogInformation("Webhook delivered successfully to {Url} with status {StatusCode}", 
    webhookUrl, responseStatusCode);

_logger.LogInformation("SDK initialized with base URL: {BaseUrl} and {ModuleCount} modules enabled", 
    baseUrl, enabledModules.Count);
```

### 4. **Warning** - Potentially problematic situations
**When to use:** Issues that don't prevent operation but may indicate problems
**Performance Impact:** Very Low - always acceptable
**Examples:**
- Rate limit approaching
- Deprecated API usage
- Fallback mechanisms triggered
- Configuration issues

```csharp
_logger.LogWarning("Rate limit at {Percentage}% capacity. Remaining requests: {Remaining}", 
    (usedRequests / totalRequests) * 100, remainingRequests);

_logger.LogWarning("Using deprecated API endpoint {Endpoint}. Consider upgrading to {NewEndpoint}", 
    oldEndpoint, newEndpoint);

_logger.LogWarning("Primary cache unavailable, falling back to in-memory cache");

_logger.LogWarning("Webhook delivery failed with status {StatusCode}, will retry in {RetryDelay}s", 
    statusCode, retryDelay.TotalSeconds);
```

### 5. **Error** - Error conditions that prevent normal operation
**When to use:** Exceptions and errors that affect functionality
**Performance Impact:** Very Low - always acceptable
**Examples:**
- API errors
- Authentication failures
- Network connectivity issues
- Data validation errors

```csharp
_logger.LogError(ex, "Failed to retrieve person {PersonId} from Planning Center: {ErrorCode}", 
    personId, ex.ErrorCode);

_logger.LogError("Authentication failed for client {ClientId}: {ErrorMessage}", 
    clientId, errorMessage);

_logger.LogError(ex, "Network error occurred while calling {Endpoint}", endpoint);

_logger.LogError("Webhook signature validation failed for URL {WebhookUrl}", webhookUrl);
```

### 6. **Critical** - Critical errors that may cause application failure
**When to use:** Severe errors that require immediate attention
**Performance Impact:** Very Low - always acceptable
**Examples:**
- SDK initialization failures
- Security violations
- Data corruption
- System resource exhaustion

```csharp
_logger.LogCritical("SDK initialization failed: Unable to connect to Planning Center API");

_logger.LogCritical("Security violation detected: Invalid webhook signature from {IpAddress}", 
    clientIpAddress);

_logger.LogCritical(ex, "Critical error in authentication system: {ErrorMessage}", 
    ex.Message);
```

## Logging Implementation

### Core Logging Infrastructure

```csharp
namespace PlanningCenter.Api.Client.Logging
{
    public static class LoggerExtensions
    {
        // High-performance logging with compile-time optimization
        private static readonly Action<ILogger, string, string, Exception> _apiRequestStarted =
            LoggerMessage.Define<string, string>(
                LogLevel.Debug,
                new EventId(1001, "ApiRequestStarted"),
                "Starting {Method} request to {Endpoint}");

        private static readonly Action<ILogger, string, int, double, Exception> _apiRequestCompleted =
            LoggerMessage.Define<string, int, double>(
                LogLevel.Debug,
                new EventId(1002, "ApiRequestCompleted"),
                "Completed {Method} request with status {StatusCode} in {ElapsedMs}ms");

        private static readonly Action<ILogger, string, string, string, Exception> _apiRequestFailed =
            LoggerMessage.Define<string, string, string>(
                LogLevel.Error,
                new EventId(1003, "ApiRequestFailed"),
                "Failed {Method} request to {Endpoint}: {ErrorMessage}");

        private static readonly Action<ILogger, string, string, Exception> _cacheOperation =
            LoggerMessage.Define<string, string>(
                LogLevel.Debug,
                new EventId(2001, "CacheOperation"),
                "Cache {Operation} for key: {CacheKey}");

        private static readonly Action<ILogger, string, int, Exception> _authenticationEvent =
            LoggerMessage.Define<string, int>(
                LogLevel.Information,
                new EventId(3001, "AuthenticationEvent"),
                "Authentication {Event} for client ending in {ClientIdSuffix}");

        private static readonly Action<ILogger, string, int, int, Exception> _rateLimitWarning =
            LoggerMessage.Define<string, int, int>(
                LogLevel.Warning,
                new EventId(4001, "RateLimitWarning"),
                "Rate limit warning for {Endpoint}: {Used}/{Total} requests used");

        private static readonly Action<ILogger, string, string, Exception> _webhookEvent =
            LoggerMessage.Define<string, string>(
                LogLevel.Information,
                new EventId(5001, "WebhookEvent"),
                "Webhook {Event} for URL: {WebhookUrl}");

        // Extension methods for consistent logging
        public static void ApiRequestStarted(this ILogger logger, string method, string endpoint)
            => _apiRequestStarted(logger, method, endpoint, null);

        public static void ApiRequestCompleted(this ILogger logger, string method, int statusCode, double elapsedMs)
            => _apiRequestCompleted(logger, method, statusCode, elapsedMs, null);

        public static void ApiRequestFailed(this ILogger logger, string method, string endpoint, string errorMessage, Exception exception = null)
            => _apiRequestFailed(logger, method, endpoint, errorMessage, exception);

        public static void CacheHit(this ILogger logger, string cacheKey)
            => _cacheOperation(logger, "HIT", cacheKey, null);

        public static void CacheMiss(this ILogger logger, string cacheKey)
            => _cacheOperation(logger, "MISS", cacheKey, null);

        public static void AuthenticationSucceeded(this ILogger logger, string clientId)
            => _authenticationEvent(logger, "succeeded", GetClientIdSuffix(clientId), null);

        public static void AuthenticationFailed(this ILogger logger, string clientId)
            => _authenticationEvent(logger, "failed", GetClientIdSuffix(clientId), null);

        public static void RateLimitWarning(this ILogger logger, string endpoint, int used, int total)
            => _rateLimitWarning(logger, endpoint, used, total, null);

        public static void WebhookDelivered(this ILogger logger, string webhookUrl)
            => _webhookEvent(logger, "delivered", webhookUrl, null);

        public static void WebhookFailed(this ILogger logger, string webhookUrl)
            => _webhookEvent(logger, "failed", webhookUrl, null);

        private static int GetClientIdSuffix(string clientId)
        {
            return clientId?.Length >= 4 ? 
                int.Parse(clientId.Substring(clientId.Length - 4)) : 
                0;
        }
    }
}
```

### Service-Level Logging

```csharp
namespace PlanningCenter.Api.Client.Services
{
    public class PeopleService : IPeopleService
    {
        private readonly IApiConnection _apiConnection;
        private readonly ILogger<PeopleService> _logger;
        private readonly PersonMapper _personMapper;

        public PeopleService(
            IApiConnection apiConnection, 
            ILogger<PeopleService> logger,
            PersonMapper personMapper)
        {
            _apiConnection = apiConnection;
            _logger = logger;
            _personMapper = personMapper;
        }

        public async Task<Core.Person> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["PersonId"] = id,
                ["Operation"] = "GetPerson"
            });

            _logger.LogDebug("Retrieving person with ID: {PersonId}", id);

            try
            {
                var dto = await _apiConnection.GetAsync<People.PersonDto>($"/people/v2/people/{id}", cancellationToken);
                var person = _personMapper.Map(dto);

                _logger.LogInformation("Successfully retrieved person {PersonId}: {PersonName}", 
                    id, person.FullName);

                return person;
            }
            catch (PlanningCenterApiNotFoundException ex)
            {
                _logger.LogWarning("Person {PersonId} not found", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve person {PersonId}", id);
                throw;
            }
        }

        public async Task<IPagedResponse<Core.Person>> ListAsync(
            QueryParameters parameters = null, 
            CancellationToken cancellationToken = default)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["Operation"] = "ListPeople",
                ["PageSize"] = parameters?.PerPage ?? 25,
                ["Filters"] = parameters?.Where?.Keys.ToArray() ?? Array.Empty<string>()
            });

            _logger.LogDebug("Listing people with parameters: {@Parameters}", parameters);

            try
            {
                var response = await _apiConnection.GetPagedAsync<People.PersonDto>("/people/v2/people", parameters, cancellationToken);
                var people = response.Data.Select(_personMapper.Map).ToList();

                _logger.LogInformation("Successfully retrieved {Count} people (page {Page} of {TotalPages})", 
                    people.Count, response.Meta.Page, response.Meta.TotalPages);

                return new PagedResponse<Core.Person>
                {
                    Data = people,
                    Meta = response.Meta,
                    Links = response.Links
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list people");
                throw;
            }
        }
    }
}
```

### HTTP Client Logging

```csharp
namespace PlanningCenter.Api.Client.Http
{
    public class ApiConnection : IApiConnection
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiConnection> _logger;

        public async Task<T> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
        {
            return await ExecuteRequestAsync<T>(HttpMethod.Get, endpoint, null, cancellationToken);
        }

        private async Task<T> ExecuteRequestAsync<T>(
            HttpMethod method, 
            string endpoint, 
            object data, 
            CancellationToken cancellationToken)
        {
            var requestUrl = $"{_httpClient.BaseAddress?.ToString().TrimEnd('/')}/{endpoint.TrimStart('/')}";
            var stopwatch = Stopwatch.StartNew();

            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["RequestId"] = Guid.NewGuid().ToString("N")[..8],
                ["Method"] = method.Method,
                ["Endpoint"] = endpoint
            });

            _logger.ApiRequestStarted(method.Method, endpoint);

            try
            {
                using var request = new HttpRequestMessage(method, endpoint);
                
                if (data != null)
                {
                    var json = JsonSerializer.Serialize(data);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                    
                    _logger.LogTrace("Request body: {RequestBody}", json);
                }

                using var response = await _httpClient.SendAsync(request, cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                stopwatch.Stop();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.ApiRequestFailed(method.Method, endpoint, $"{response.StatusCode}: {response.ReasonPhrase}");
                    
                    var exception = ApiExceptionFactory.CreateException(response, content, requestUrl, method.Method);
                    throw exception;
                }

                _logger.ApiRequestCompleted(method.Method, (int)response.StatusCode, stopwatch.Elapsed.TotalMilliseconds);
                
                if (_logger.IsEnabled(LogLevel.Trace))
                {
                    _logger.LogTrace("Response body: {ResponseBody}", content);
                }

                return JsonSerializer.Deserialize<T>(content);
            }
            catch (PlanningCenterApiException)
            {
                throw; // Re-throw our custom exceptions
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Unexpected error during {Method} request to {Endpoint} after {ElapsedMs}ms", 
                    method.Method, endpoint, stopwatch.Elapsed.TotalMilliseconds);
                
                var connectionException = ApiExceptionFactory.CreateConnectionException(ex, requestUrl, method.Method);
                throw connectionException;
            }
        }
    }
}
```

## Consumer Configuration

### Basic Configuration

```csharp
// Program.cs or Startup.cs
public void ConfigureServices(IServiceCollection services)
{
    // Configure logging first
    services.AddLogging(builder =>
    {
        builder.AddConsole();
        builder.AddDebug();
        
        // Set minimum levels
        builder.SetMinimumLevel(LogLevel.Information);
        
        // Configure specific loggers
        builder.AddFilter("PlanningCenter.Api.Client", LogLevel.Debug);
        builder.AddFilter("PlanningCenter.Api.Client.Http", LogLevel.Information);
    });

    // Add Planning Center SDK
    services.AddPlanningCenterApiClient(options =>
    {
        options.ClientId = "your-client-id";
        options.ClientSecret = "your-client-secret";
        options.LogLevel = LogLevel.Information; // SDK-specific log level
    });
}
```

### Advanced Configuration with appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information",
      "PlanningCenter.Api.Client": "Debug",
      "PlanningCenter.Api.Client.Http.ApiConnection": "Information",
      "PlanningCenter.Api.Client.Services": "Debug",
      "PlanningCenter.Api.Client.Auth": "Information",
      "PlanningCenter.Api.Client.Webhooks": "Debug"
    }
  },
  "PlanningCenter": {
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "BaseUrl": "https://api.planningcenteronline.com",
    "Logging": {
      "LogRequestBodies": false,
      "LogResponseBodies": false,
      "LogSensitiveData": false
    }
  }
}
```

### Structured Logging with Serilog

```csharp
// Program.cs
public static void Main(string[] args)
{
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .MinimumLevel.Override("PlanningCenter.Api.Client", LogEventLevel.Debug)
        .Enrich.FromLogContext()
        .WriteTo.Console(outputTemplate: 
            "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
        .WriteTo.File("logs/planning-center-sdk-.txt", 
            rollingInterval: RollingInterval.Day,
            restrictedToMinimumLevel: LogEventLevel.Information)
        .CreateLogger();

    try
    {
        CreateHostBuilder(args).Build().Run();
    }
    finally
    {
        Log.CloseAndFlush();
    }
}

public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureServices((context, services) =>
        {
            services.AddPlanningCenterApiClient(options =>
            {
                context.Configuration.GetSection("PlanningCenter").Bind(options);
            });
        });
```

## Security and Sensitive Data

### Sensitive Data Handling

```csharp
namespace PlanningCenter.Api.Client.Logging
{
    public class SensitiveDataFilter
    {
        private static readonly string[] SensitiveFields = 
        {
            "client_secret", "access_token", "refresh_token", "password", 
            "ssn", "social_security_number", "credit_card", "bank_account"
        };

        public static string FilterSensitiveData(string json)
        {
            if (string.IsNullOrEmpty(json)) return json;

            var document = JsonDocument.Parse(json);
            return FilterJsonElement(document.RootElement).ToString();
        }

        private static JsonElement FilterJsonElement(JsonElement element)
        {
            // Implementation to recursively filter sensitive fields
            // Replace sensitive values with "[REDACTED]"
        }
    }

    public class LoggingOptions
    {
        public bool LogRequestBodies { get; set; } = false;
        public bool LogResponseBodies { get; set; } = false;
        public bool LogSensitiveData { get; set; } = false;
        public string[] AdditionalSensitiveFields { get; set; } = Array.Empty<string>();
    }
}
```

### Conditional Logging

```csharp
public class ApiConnection : IApiConnection
{
    private readonly LoggingOptions _loggingOptions;

    private async Task<T> ExecuteRequestAsync<T>(...)
    {
        // Only log request bodies if explicitly enabled
        if (_loggingOptions.LogRequestBodies && data != null && _logger.IsEnabled(LogLevel.Trace))
        {
            var json = JsonSerializer.Serialize(data);
            var filteredJson = _loggingOptions.LogSensitiveData ? 
                json : SensitiveDataFilter.FilterSensitiveData(json);
            
            _logger.LogTrace("Request body: {RequestBody}", filteredJson);
        }

        // ... rest of implementation
    }
}
```

## Performance Considerations

### High-Performance Logging

```csharp
public static class HighPerformanceLogging
{
    // Use LoggerMessage.Define for high-performance logging
    private static readonly Action<ILogger, string, int, Exception> _personRetrieved =
        LoggerMessage.Define<string, int>(
            LogLevel.Information,
            new EventId(1, "PersonRetrieved"),
            "Retrieved person {PersonName} with {AddressCount} addresses");

    public static void PersonRetrieved(this ILogger logger, string personName, int addressCount)
        => _personRetrieved(logger, personName, addressCount, null);
}
```

### Conditional Compilation

```csharp
public class PeopleService
{
    public async Task<Core.Person> GetAsync(string id, CancellationToken cancellationToken = default)
    {
#if DEBUG
        _logger.LogTrace("Entering GetAsync with id: {PersonId}", id);
#endif

        // Implementation...

#if DEBUG
        _logger.LogTrace("Exiting GetAsync with result: {@Person}", person);
#endif
        return person;
    }
}
```

## Monitoring and Observability Integration

### Application Insights Integration

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddApplicationInsightsTelemetry();
    
    services.AddPlanningCenterApiClient(options =>
    {
        options.ClientId = Configuration["PlanningCenter:ClientId"];
        options.ClientSecret = Configuration["PlanningCenter:ClientSecret"];
    });

    // Custom telemetry processor for Planning Center events
    services.AddSingleton<ITelemetryProcessor, PlanningCenterTelemetryProcessor>();
}

public class PlanningCenterTelemetryProcessor : ITelemetryProcessor
{
    public void Process(ITelemetry item)
    {
        if (item is TraceTelemetry trace && 
            trace.Context.Properties.ContainsKey("SourceContext") &&
            trace.Context.Properties["SourceContext"].StartsWith("PlanningCenter.Api.Client"))
        {
            // Add custom properties for Planning Center SDK logs
            trace.Properties["SDKComponent"] = "PlanningCenterSDK";
            trace.Properties["APIProvider"] = "PlanningCenter";
        }
    }
}
```

## Recommended Logging Levels by Environment

### Development Environment
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "PlanningCenter.Api.Client": "Trace",
      "PlanningCenter.Api.Client.Http": "Debug"
    }
  },
  "PlanningCenter": {
    "Logging": {
      "LogRequestBodies": true,
      "LogResponseBodies": true,
      "LogSensitiveData": false
    }
  }
}
```

### Staging Environment
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "PlanningCenter.Api.Client": "Debug",
      "PlanningCenter.Api.Client.Http": "Information"
    }
  },
  "PlanningCenter": {
    "Logging": {
      "LogRequestBodies": false,
      "LogResponseBodies": false,
      "LogSensitiveData": false
    }
  }
}
```

### Production Environment
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "PlanningCenter.Api.Client": "Information",
      "PlanningCenter.Api.Client.Http": "Warning"
    }
  },
  "PlanningCenter": {
    "Logging": {
      "LogRequestBodies": false,
      "LogResponseBodies": false,
      "LogSensitiveData": false
    }
  }
}
```

## Best Practices Summary

### ✅ Do's
- Use structured logging with consistent property names
- Implement high-performance logging with LoggerMessage.Define
- Provide granular logger categories for different SDK components
- Filter sensitive data by default
- Use log scopes for request correlation
- Include relevant context (request IDs, user IDs, etc.)
- Log business events at Information level
- Log technical details at Debug/Trace levels

### ❌ Don'ts
- Don't log sensitive data (tokens, passwords, PII) unless explicitly enabled
- Don't log at Trace level in production
- Don't log large payloads without size limits
- Don't use string interpolation in log messages (use structured logging)
- Don't log the same information at multiple levels
- Don't ignore log level checks for expensive operations

This comprehensive logging strategy ensures that the Planning Center SDK provides valuable insights while maintaining performance and security standards across all environments.