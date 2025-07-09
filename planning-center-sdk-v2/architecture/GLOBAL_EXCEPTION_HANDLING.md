# Global Exception Handling Strategy

## Overview

The Planning Center SDK implements a comprehensive global exception handling strategy that provides consistent error handling across all modules, proper error classification, and detailed error information for debugging and monitoring.

## Exception Hierarchy

### Base Exception Classes
```csharp
namespace PlanningCenter.Api.Client.Models.Exceptions
{
    /// <summary>
    /// Base exception for all Planning Center API-related errors
    /// </summary>
    public abstract class PlanningCenterApiException : Exception
    {
        public int? StatusCode { get; }
        public string RequestId { get; }
        public string ErrorCode { get; }
        public string RequestUrl { get; }
        public string RequestMethod { get; }
        public DateTime Timestamp { get; }
        public Dictionary<string, object> AdditionalData { get; }

        protected PlanningCenterApiException(
            string message, 
            int? statusCode = null, 
            string errorCode = null,
            string requestId = null,
            string requestUrl = null,
            string requestMethod = null,
            Exception innerException = null) 
            : base(message, innerException)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
            RequestId = requestId;
            RequestUrl = requestUrl;
            RequestMethod = requestMethod;
            Timestamp = DateTime.UtcNow;
            AdditionalData = new Dictionary<string, object>();
        }
    }
}
```

### Specific Exception Types
```csharp
/// <summary>
/// Thrown when a resource is not found (404)
/// </summary>
public class PlanningCenterApiNotFoundException : PlanningCenterApiException
{
    public string ResourceType { get; }
    public string ResourceId { get; }

    public PlanningCenterApiNotFoundException(
        string resourceType, 
        string resourceId, 
        string requestId = null,
        string requestUrl = null) 
        : base($"{resourceType} with ID '{resourceId}' was not found", 404, "not_found", requestId, requestUrl, "GET")
    {
        ResourceType = resourceType;
        ResourceId = resourceId;
    }
}

/// <summary>
/// Thrown when authentication fails (401)
/// </summary>
public class PlanningCenterApiAuthenticationException : PlanningCenterApiException
{
    public PlanningCenterApiAuthenticationException(
        string message = "Authentication failed", 
        string requestId = null,
        string requestUrl = null,
        string requestMethod = null) 
        : base(message, 401, "authentication_failed", requestId, requestUrl, requestMethod)
    {
    }
}

/// <summary>
/// Thrown when authorization fails (403)
/// </summary>
public class PlanningCenterApiAuthorizationException : PlanningCenterApiException
{
    public string RequiredPermission { get; }

    public PlanningCenterApiAuthorizationException(
        string message = "Access denied", 
        string requiredPermission = null,
        string requestId = null,
        string requestUrl = null,
        string requestMethod = null) 
        : base(message, 403, "access_denied", requestId, requestUrl, requestMethod)
    {
        RequiredPermission = requiredPermission;
    }
}

/// <summary>
/// Thrown when validation fails (422)
/// </summary>
public class PlanningCenterApiValidationException : PlanningCenterApiException
{
    public List<ValidationError> Errors { get; }

    public PlanningCenterApiValidationException(
        List<ValidationError> errors, 
        string requestId = null,
        string requestUrl = null,
        string requestMethod = null) 
        : base("Validation failed", 422, "validation_failed", requestId, requestUrl, requestMethod)
    {
        Errors = errors ?? new List<ValidationError>();
    }
}

/// <summary>
/// Thrown when rate limit is exceeded (429)
/// </summary>
public class PlanningCenterApiRateLimitException : PlanningCenterApiException
{
    public TimeSpan RetryAfter { get; }
    public int? RemainingRequests { get; }
    public DateTime? ResetTime { get; }

    public PlanningCenterApiRateLimitException(
        TimeSpan retryAfter, 
        int? remainingRequests = null,
        DateTime? resetTime = null,
        string requestId = null,
        string requestUrl = null,
        string requestMethod = null) 
        : base($"Rate limit exceeded. Retry after {retryAfter.TotalSeconds} seconds", 429, "rate_limit_exceeded", requestId, requestUrl, requestMethod)
    {
        RetryAfter = retryAfter;
        RemainingRequests = remainingRequests;
        ResetTime = resetTime;
    }
}

/// <summary>
/// Thrown when server error occurs (5xx)
/// </summary>
public class PlanningCenterApiServerException : PlanningCenterApiException
{
    public PlanningCenterApiServerException(
        string message, 
        int statusCode,
        string requestId = null,
        string requestUrl = null,
        string requestMethod = null,
        Exception innerException = null) 
        : base(message, statusCode, "server_error", requestId, requestUrl, requestMethod, innerException)
    {
    }
}

/// <summary>
/// Thrown when network or connection errors occur
/// </summary>
public class PlanningCenterApiConnectionException : PlanningCenterApiException
{
    public PlanningCenterApiConnectionException(
        string message, 
        string requestUrl = null,
        string requestMethod = null,
        Exception innerException = null) 
        : base(message, null, "connection_error", null, requestUrl, requestMethod, innerException)
    {
    }
}

/// <summary>
/// Thrown when request timeout occurs
/// </summary>
public class PlanningCenterApiTimeoutException : PlanningCenterApiException
{
    public TimeSpan Timeout { get; }

    public PlanningCenterApiTimeoutException(
        TimeSpan timeout,
        string requestUrl = null,
        string requestMethod = null) 
        : base($"Request timed out after {timeout.TotalSeconds} seconds", 408, "timeout", null, requestUrl, requestMethod)
    {
        Timeout = timeout;
    }
}

/// <summary>
/// Thrown when webhook signature validation fails
/// </summary>
public class PlanningCenterWebhookValidationException : PlanningCenterApiException
{
    public string ProvidedSignature { get; }
    public string ExpectedSignature { get; }

    public PlanningCenterWebhookValidationException(
        string providedSignature, 
        string expectedSignature) 
        : base("Webhook signature validation failed", null, "webhook_validation_failed")
    {
        ProvidedSignature = providedSignature;
        ExpectedSignature = expectedSignature;
    }
}
```

### Supporting Classes
```csharp
public class ValidationError
{
    public string Field { get; set; }
    public string Code { get; set; }
    public string Message { get; set; }
    public object AttemptedValue { get; set; }
}
```

## Global Exception Handler Implementation

### ApiExceptionFactory
```csharp
namespace PlanningCenter.Api.Client.Http
{
    public static class ApiExceptionFactory
    {
        public static PlanningCenterApiException CreateException(
            HttpResponseMessage response, 
            string content,
            string requestUrl,
            string requestMethod)
        {
            var requestId = ExtractRequestId(response);
            
            return response.StatusCode switch
            {
                HttpStatusCode.NotFound => CreateNotFoundException(content, requestId, requestUrl),
                HttpStatusCode.Unauthorized => new PlanningCenterApiAuthenticationException(
                    ExtractErrorMessage(content, "Authentication failed"), 
                    requestId, requestUrl, requestMethod),
                HttpStatusCode.Forbidden => new PlanningCenterApiAuthorizationException(
                    ExtractErrorMessage(content, "Access denied"), 
                    ExtractRequiredPermission(content),
                    requestId, requestUrl, requestMethod),
                HttpStatusCode.UnprocessableEntity => CreateValidationException(content, requestId, requestUrl, requestMethod),
                HttpStatusCode.TooManyRequests => CreateRateLimitException(response, content, requestId, requestUrl, requestMethod),
                HttpStatusCode.RequestTimeout => new PlanningCenterApiTimeoutException(
                    TimeSpan.FromSeconds(30), requestUrl, requestMethod),
                _ when ((int)response.StatusCode >= 500) => new PlanningCenterApiServerException(
                    ExtractErrorMessage(content, "Server error occurred"), 
                    (int)response.StatusCode, requestId, requestUrl, requestMethod),
                _ => new PlanningCenterApiException(
                    ExtractErrorMessage(content, $"API error occurred: {response.StatusCode}"), 
                    (int)response.StatusCode, "api_error", requestId, requestUrl, requestMethod)
            };
        }

        public static PlanningCenterApiConnectionException CreateConnectionException(
            Exception innerException,
            string requestUrl,
            string requestMethod)
        {
            return innerException switch
            {
                TaskCanceledException when innerException.InnerException is TimeoutException => 
                    new PlanningCenterApiTimeoutException(TimeSpan.FromSeconds(30), requestUrl, requestMethod),
                TaskCanceledException => 
                    new PlanningCenterApiConnectionException("Request was cancelled", requestUrl, requestMethod, innerException),
                HttpRequestException => 
                    new PlanningCenterApiConnectionException("Network error occurred", requestUrl, requestMethod, innerException),
                _ => 
                    new PlanningCenterApiConnectionException("Connection error occurred", requestUrl, requestMethod, innerException)
            };
        }

        private static string ExtractRequestId(HttpResponseMessage response)
        {
            return response.Headers.TryGetValues("X-Request-Id", out var values) 
                ? values.FirstOrDefault() 
                : null;
        }

        private static string ExtractErrorMessage(string content, string defaultMessage)
        {
            try
            {
                var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content);
                return errorResponse?.Error?.Message ?? defaultMessage;
            }
            catch
            {
                return defaultMessage;
            }
        }

        private static PlanningCenterApiNotFoundException CreateNotFoundException(
            string content, 
            string requestId, 
            string requestUrl)
        {
            try
            {
                var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content);
                var resourceType = errorResponse?.Error?.ResourceType ?? "Resource";
                var resourceId = errorResponse?.Error?.ResourceId ?? "unknown";
                return new PlanningCenterApiNotFoundException(resourceType, resourceId, requestId, requestUrl);
            }
            catch
            {
                return new PlanningCenterApiNotFoundException("Resource", "unknown", requestId, requestUrl);
            }
        }

        private static PlanningCenterApiValidationException CreateValidationException(
            string content, 
            string requestId, 
            string requestUrl, 
            string requestMethod)
        {
            try
            {
                var errorResponse = JsonSerializer.Deserialize<ValidationErrorResponse>(content);
                var errors = errorResponse?.Errors?.Select(e => new ValidationError
                {
                    Field = e.Field,
                    Code = e.Code,
                    Message = e.Message,
                    AttemptedValue = e.AttemptedValue
                }).ToList() ?? new List<ValidationError>();

                return new PlanningCenterApiValidationException(errors, requestId, requestUrl, requestMethod);
            }
            catch
            {
                return new PlanningCenterApiValidationException(
                    new List<ValidationError>(), requestId, requestUrl, requestMethod);
            }
        }

        private static PlanningCenterApiRateLimitException CreateRateLimitException(
            HttpResponseMessage response, 
            string content, 
            string requestId, 
            string requestUrl, 
            string requestMethod)
        {
            var retryAfter = TimeSpan.FromSeconds(60); // Default
            
            if (response.Headers.RetryAfter?.Delta.HasValue == true)
            {
                retryAfter = response.Headers.RetryAfter.Delta.Value;
            }
            else if (response.Headers.TryGetValues("Retry-After", out var values))
            {
                if (int.TryParse(values.FirstOrDefault(), out var seconds))
                {
                    retryAfter = TimeSpan.FromSeconds(seconds);
                }
            }

            var remainingRequests = ExtractRemainingRequests(response);
            var resetTime = ExtractResetTime(response);

            return new PlanningCenterApiRateLimitException(
                retryAfter, remainingRequests, resetTime, requestId, requestUrl, requestMethod);
        }

        private static int? ExtractRemainingRequests(HttpResponseMessage response)
        {
            if (response.Headers.TryGetValues("X-Rate-Limit-Remaining", out var values))
            {
                if (int.TryParse(values.FirstOrDefault(), out var remaining))
                {
                    return remaining;
                }
            }
            return null;
        }

        private static DateTime? ExtractResetTime(HttpResponseMessage response)
        {
            if (response.Headers.TryGetValues("X-Rate-Limit-Reset", out var values))
            {
                if (long.TryParse(values.FirstOrDefault(), out var unixTimestamp))
                {
                    return DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).DateTime;
                }
            }
            return null;
        }

        private static string ExtractRequiredPermission(string content)
        {
            try
            {
                var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content);
                return errorResponse?.Error?.RequiredPermission;
            }
            catch
            {
                return null;
            }
        }
    }

    // Supporting classes for error response parsing
    internal class ErrorResponse
    {
        public ErrorDetail Error { get; set; }
    }

    internal class ErrorDetail
    {
        public string Message { get; set; }
        public string Code { get; set; }
        public string ResourceType { get; set; }
        public string ResourceId { get; set; }
        public string RequiredPermission { get; set; }
    }

    internal class ValidationErrorResponse
    {
        public List<ValidationErrorDetail> Errors { get; set; }
    }

    internal class ValidationErrorDetail
    {
        public string Field { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public object AttemptedValue { get; set; }
    }
}
```

## Global Exception Handling in ApiConnection

### Enhanced ApiConnection with Global Exception Handling
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

        public async Task<T> PostAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default)
        {
            return await ExecuteRequestAsync<T>(HttpMethod.Post, endpoint, data, cancellationToken);
        }

        // ... other HTTP methods

        private async Task<T> ExecuteRequestAsync<T>(
            HttpMethod method, 
            string endpoint, 
            object data, 
            CancellationToken cancellationToken)
        {
            var requestUrl = $"{_httpClient.BaseAddress?.ToString().TrimEnd('/')}/{endpoint.TrimStart('/')}";
            
            try
            {
                using var request = new HttpRequestMessage(method, endpoint);
                
                if (data != null)
                {
                    var json = JsonSerializer.Serialize(data);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                _logger.LogDebug("Making {Method} request to {Url}", method, requestUrl);

                using var response = await _httpClient.SendAsync(request, cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("API request failed: {StatusCode} {ReasonPhrase} - {Content}", 
                        response.StatusCode, response.ReasonPhrase, content);
                    
                    var exception = ApiExceptionFactory.CreateException(
                        response, content, requestUrl, method.Method);
                    
                    throw exception;
                }

                _logger.LogDebug("API request successful: {StatusCode}", response.StatusCode);

                if (typeof(T) == typeof(string))
                {
                    return (T)(object)content;
                }

                return JsonSerializer.Deserialize<T>(content);
            }
            catch (PlanningCenterApiException)
            {
                // Re-throw our custom exceptions
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during API request to {Url}", requestUrl);
                
                var connectionException = ApiExceptionFactory.CreateConnectionException(
                    ex, requestUrl, method.Method);
                
                throw connectionException;
            }
        }
    }
}
```

## ASP.NET Core Global Exception Middleware

### Exception Handling Middleware
```csharp
namespace PlanningCenter.Api.Client.AspNetCore
{
    public class PlanningCenterExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PlanningCenterExceptionMiddleware> _logger;

        public PlanningCenterExceptionMiddleware(RequestDelegate next, ILogger<PlanningCenterExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (PlanningCenterApiException ex)
            {
                _logger.LogError(ex, "Planning Center API error occurred");
                await HandlePlanningCenterExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred");
                await HandleGenericExceptionAsync(context, ex);
            }
        }

        private static async Task HandlePlanningCenterExceptionAsync(HttpContext context, PlanningCenterApiException ex)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new
            {
                error = new
                {
                    message = ex.Message,
                    code = ex.ErrorCode,
                    statusCode = ex.StatusCode,
                    requestId = ex.RequestId,
                    timestamp = ex.Timestamp,
                    details = CreateErrorDetails(ex)
                }
            };

            response.StatusCode = ex.StatusCode ?? 500;
            
            var jsonResponse = JsonSerializer.Serialize(errorResponse);
            await response.WriteAsync(jsonResponse);
        }

        private static async Task HandleGenericExceptionAsync(HttpContext context, Exception ex)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = 500;

            var errorResponse = new
            {
                error = new
                {
                    message = "An unexpected error occurred",
                    code = "internal_error",
                    statusCode = 500,
                    timestamp = DateTime.UtcNow
                }
            };

            var jsonResponse = JsonSerializer.Serialize(errorResponse);
            await response.WriteAsync(jsonResponse);
        }

        private static object CreateErrorDetails(PlanningCenterApiException ex)
        {
            return ex switch
            {
                PlanningCenterApiValidationException validationEx => new
                {
                    validationErrors = validationEx.Errors.Select(e => new
                    {
                        field = e.Field,
                        code = e.Code,
                        message = e.Message,
                        attemptedValue = e.AttemptedValue
                    })
                },
                PlanningCenterApiRateLimitException rateLimitEx => new
                {
                    retryAfter = rateLimitEx.RetryAfter.TotalSeconds,
                    remainingRequests = rateLimitEx.RemainingRequests,
                    resetTime = rateLimitEx.ResetTime
                },
                PlanningCenterApiNotFoundException notFoundEx => new
                {
                    resourceType = notFoundEx.ResourceType,
                    resourceId = notFoundEx.ResourceId
                },
                PlanningCenterApiAuthorizationException authEx => new
                {
                    requiredPermission = authEx.RequiredPermission
                },
                _ => null
            };
        }
    }

    // Extension method for easy registration
    public static class PlanningCenterExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UsePlanningCenterExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<PlanningCenterExceptionMiddleware>();
        }
    }
}
```

## Exception Handling in Service Layer

### Service Base Class with Exception Handling
```csharp
namespace PlanningCenter.Api.Client.Services
{
    public abstract class ServiceBase
    {
        protected readonly IApiConnection _apiConnection;
        protected readonly ILogger _logger;

        protected ServiceBase(IApiConnection apiConnection, ILogger logger)
        {
            _apiConnection = apiConnection;
            _logger = logger;
        }

        protected async Task<T> ExecuteWithExceptionHandlingAsync<T>(
            Func<Task<T>> operation,
            string operationName,
            object parameters = null)
        {
            try
            {
                _logger.LogDebug("Executing {OperationName} with parameters: {@Parameters}", 
                    operationName, parameters);

                var result = await operation();

                _logger.LogDebug("Successfully executed {OperationName}", operationName);
                return result;
            }
            catch (PlanningCenterApiException ex)
            {
                _logger.LogWarning(ex, "Planning Center API error in {OperationName}: {ErrorCode} - {Message}", 
                    operationName, ex.ErrorCode, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in {OperationName}", operationName);
                throw new PlanningCenterApiException(
                    $"Unexpected error in {operationName}: {ex.Message}", 
                    innerException: ex);
            }
        }

        protected async Task ExecuteWithExceptionHandlingAsync(
            Func<Task> operation,
            string operationName,
            object parameters = null)
        {
            await ExecuteWithExceptionHandlingAsync(async () =>
            {
                await operation();
                return true;
            }, operationName, parameters);
        }
    }
}
```

## Usage Examples

### Service Implementation with Exception Handling
```csharp
public class PeopleService : ServiceBase, IPeopleService
{
    public PeopleService(IApiConnection apiConnection, ILogger<PeopleService> logger) 
        : base(apiConnection, logger)
    {
    }

    public async Task<Core.Person> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithExceptionHandlingAsync(
            async () =>
            {
                var dto = await _apiConnection.GetAsync<People.PersonDto>($"/people/v2/people/{id}", cancellationToken);
                return _personMapper.Map(dto);
            },
            "GetPerson",
            new { PersonId = id });
    }
}
```

### Application Startup Configuration
```csharp
// Program.cs or Startup.cs
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    // Add global exception handling middleware
    app.UsePlanningCenterExceptionHandling();
    
    // Other middleware...
    app.UseRouting();
    app.UseEndpoints(endpoints => endpoints.MapControllers());
}
```

### Client Exception Handling
```csharp
public class PersonController : ControllerBase
{
    private readonly IPeopleService _peopleService;

    public async Task<ActionResult<Core.Person>> GetPerson(string id)
    {
        try
        {
            var person = await _peopleService.GetAsync(id);
            return Ok(person);
        }
        catch (PlanningCenterApiNotFoundException)
        {
            return NotFound();
        }
        catch (PlanningCenterApiAuthorizationException)
        {
            return Forbid();
        }
        catch (PlanningCenterApiValidationException ex)
        {
            return BadRequest(new { errors = ex.Errors });
        }
        catch (PlanningCenterApiRateLimitException ex)
        {
            Response.Headers.Add("Retry-After", ex.RetryAfter.TotalSeconds.ToString());
            return StatusCode(429, new { message = ex.Message, retryAfter = ex.RetryAfter.TotalSeconds });
        }
        // Global exception middleware will handle other PlanningCenterApiExceptions
    }
}
```

## Monitoring and Observability

### Exception Metrics and Logging
```csharp
public class ExceptionMetricsService
{
    private readonly ILogger<ExceptionMetricsService> _logger;
    private readonly IMetrics _metrics;

    public void RecordException(PlanningCenterApiException exception)
    {
        _metrics.Counter("planning_center_api_exceptions")
            .WithTag("error_code", exception.ErrorCode)
            .WithTag("status_code", exception.StatusCode?.ToString() ?? "unknown")
            .WithTag("request_method", exception.RequestMethod ?? "unknown")
            .Increment();

        _logger.LogError(exception, 
            "Planning Center API exception: {ErrorCode} - {Message} - Request: {Method} {Url}",
            exception.ErrorCode, exception.Message, exception.RequestMethod, exception.RequestUrl);
    }
}
```

This comprehensive global exception handling strategy provides:

1. **Consistent Error Classification** across all modules
2. **Detailed Error Information** for debugging and monitoring
3. **Proper HTTP Status Code Mapping** for web applications
4. **Structured Error Responses** for API consumers
5. **Comprehensive Logging** for observability
6. **Retry Logic Support** for transient errors
7. **Security-Conscious Error Messages** that don't leak sensitive information

The global exception handler ensures that all errors from the Planning Center API are properly categorized, logged, and presented to consumers in a consistent format.