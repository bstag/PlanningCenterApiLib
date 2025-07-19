using Microsoft.Extensions.Logging;
using PlanningCenter.Api.Client.Correlation;
using PlanningCenter.Api.Client.Models.Exceptions;

namespace PlanningCenter.Api.Client.Exceptions;

/// <summary>
/// Global exception handler for Planning Center API operations.
/// </summary>
public static class GlobalExceptionHandler
{
    /// <summary>
    /// Handles exceptions globally with consistent logging and correlation tracking.
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="exception">The exception to handle</param>
    /// <param name="operationName">The name of the operation that failed</param>
    /// <param name="resourceId">Optional resource identifier</param>
    /// <param name="additionalContext">Additional context for logging</param>
    public static void Handle(
        ILogger logger,
        Exception exception,
        string operationName,
        string? resourceId = null,
        Dictionary<string, object>? additionalContext = null)
    {
        var correlationId = CorrelationContext.Current;
        
        using var scope = logger.BeginScope(new Dictionary<string, object?>
        {
            ["OperationName"] = operationName,
            ["CorrelationId"] = correlationId,
            ["ResourceId"] = resourceId,
            ["ExceptionType"] = exception.GetType().Name
        });

        // Add additional context if provided
        if (additionalContext != null)
        {
            foreach (var (key, value) in additionalContext)
            {
                scope?.GetType().GetProperty(key)?.SetValue(scope, value);
            }
        }

        switch (exception)
        {
            case PlanningCenterApiNotFoundException notFoundEx:
                logger.LogWarning("Resource not found: {ResourceId} - {Message} [CorrelationId: {CorrelationId}]",
                    resourceId ?? "unknown", notFoundEx.Message, correlationId);
                break;

            case PlanningCenterApiValidationException validationEx:
                logger.LogWarning("Validation failed for {OperationName}: {ValidationErrors} [CorrelationId: {CorrelationId}]",
                    operationName, 
                    validationEx.FormattedErrors, 
                    correlationId);
                break;

            case PlanningCenterApiAuthenticationException authEx:
                logger.LogError("Authentication failed for {OperationName}: {Message} [CorrelationId: {CorrelationId}]",
                    operationName, authEx.Message, correlationId);
                break;

            case PlanningCenterApiAuthorizationException authzEx:
                logger.LogError("Authorization failed for {OperationName}: {Message} [CorrelationId: {CorrelationId}]",
                    operationName, authzEx.Message, correlationId);
                break;

            case PlanningCenterApiRateLimitException rateLimitEx:
                logger.LogWarning("Rate limit exceeded for {OperationName}: {Message}, Retry after: {RetryAfter} [CorrelationId: {CorrelationId}]",
                    operationName, rateLimitEx.Message, rateLimitEx.RetryAfter, correlationId);
                break;

            case PlanningCenterApiServerException serverEx:
                logger.LogError(serverEx, "Server error for {OperationName}: {Message} [CorrelationId: {CorrelationId}]",
                    operationName, serverEx.Message, correlationId);
                break;

            case PlanningCenterApiException apiEx:
                logger.LogError(apiEx, "API error for {OperationName}: {Message} [CorrelationId: {CorrelationId}]",
                    operationName, apiEx.Message, correlationId);
                break;

            case TaskCanceledException cancelEx when cancelEx.CancellationToken.IsCancellationRequested:
                logger.LogInformation("Operation {OperationName} was cancelled [CorrelationId: {CorrelationId}]",
                    operationName, correlationId);
                break;

            case TimeoutException timeoutEx:
                logger.LogError(timeoutEx, "Operation {OperationName} timed out [CorrelationId: {CorrelationId}]",
                    operationName, correlationId);
                break;

            default:
                logger.LogError(exception, "Unexpected error in {OperationName} for resource {ResourceId} [CorrelationId: {CorrelationId}]",
                    operationName, resourceId ?? "unknown", correlationId);
                break;
        }
    }

    /// <summary>
    /// Wraps an operation with global exception handling.
    /// </summary>
    /// <typeparam name="T">The return type of the operation</typeparam>
    /// <param name="logger">The logger instance</param>
    /// <param name="operation">The operation to execute</param>
    /// <param name="operationName">The name of the operation</param>
    /// <param name="resourceId">Optional resource identifier</param>
    /// <param name="additionalContext">Additional context for logging</param>
    /// <returns>The result of the operation</returns>
    public static async Task<T> WrapAsync<T>(
        ILogger logger,
        Func<Task<T>> operation,
        string operationName,
        string? resourceId = null,
        Dictionary<string, object>? additionalContext = null)
    {
        try
        {
            return await operation();
        }
        catch (Exception ex)
        {
            Handle(logger, ex, operationName, resourceId, additionalContext);
            throw;
        }
    }

    /// <summary>
    /// Wraps an operation without return value with global exception handling.
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="operation">The operation to execute</param>
    /// <param name="operationName">The name of the operation</param>
    /// <param name="resourceId">Optional resource identifier</param>
    /// <param name="additionalContext">Additional context for logging</param>
    public static async Task WrapAsync(
        ILogger logger,
        Func<Task> operation,
        string operationName,
        string? resourceId = null,
        Dictionary<string, object>? additionalContext = null)
    {
        try
        {
            await operation();
        }
        catch (Exception ex)
        {
            Handle(logger, ex, operationName, resourceId, additionalContext);
            throw;
        }
    }

    /// <summary>
    /// Creates a structured error context dictionary for logging and telemetry.
    /// </summary>
    /// <param name="exception">The exception that occurred</param>
    /// <param name="operationName">The name of the operation that failed</param>
    /// <param name="resourceId">Optional resource identifier</param>
    /// <param name="additionalContext">Additional context to include</param>
    /// <returns>A dictionary containing structured error context</returns>
    public static Dictionary<string, object> CreateErrorContext(
        Exception exception,
        string operationName,
        string? resourceId = null,
        Dictionary<string, object>? additionalContext = null)
    {
        var context = new Dictionary<string, object>
        {
            ["OperationName"] = operationName,
            ["ExceptionType"] = exception.GetType().Name,
            ["CorrelationId"] = CorrelationContext.Current ?? "unknown"
        };
        
        if (resourceId != null)
        {
            context["ResourceId"] = resourceId;
        }
        
        if (exception is PlanningCenterApiException apiEx)
        {
            if (apiEx.RequestId != null) context["RequestId"] = apiEx.RequestId;
            if (apiEx.RequestUrl != null) context["RequestUrl"] = apiEx.RequestUrl;
            if (apiEx.StatusCode.HasValue) context["StatusCode"] = apiEx.StatusCode.Value;
            if (apiEx.ErrorCode != null) context["ErrorCode"] = apiEx.ErrorCode;
        }
        
        if (additionalContext != null)
        {
            foreach (var (key, value) in additionalContext)
            {
                context[key] = value;
            }
        }
        
        return context;
    }
}