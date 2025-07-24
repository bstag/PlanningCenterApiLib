using System.Net.Http;
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
    public static Dictionary<string, object> Handle(
        ILogger logger,
        Exception exception,
        string operationName,
        string? resourceId = null,
        Dictionary<string, object>? additionalContext = null)
    {
        if (exception == null)
            throw new ArgumentNullException(nameof(exception));
        if (string.IsNullOrWhiteSpace(operationName))
            throw new ArgumentNullException(nameof(operationName));

        if (logger == null)
        {
            throw new ArgumentNullException(nameof(logger));
        }
        
        var correlationId = CorrelationContext.Current;
        
        var scopeContext = new Dictionary<string, object?>
        {
            ["Operation"] = operationName,
            ["OperationName"] = operationName,
            ["CorrelationId"] = correlationId,
            ["ResourceId"] = resourceId,
            ["ExceptionType"] = exception.GetType().Name
        };

        if (additionalContext != null)
        {
            foreach (var (key, value) in additionalContext)
            {
                scopeContext[key] = value;
            }
        }

        using var scope = logger.BeginScope(scopeContext);
        
        // Helper method to format additional context for inclusion in log messages
        string FormatAdditionalContext(Dictionary<string, object>? context)
        {
            if (context == null || context.Count == 0)
                return string.Empty;
            
            var contextPairs = context.Select(kvp => $"{kvp.Key}: {kvp.Value}");
            return $" [Context: {string.Join(", ", contextPairs)}]";
        }
        
        var contextSuffix = FormatAdditionalContext(additionalContext);

        switch (exception)
        {
            case PlanningCenterApiNotFoundException notFoundEx:
                logger.LogWarning("Resource not found: {ResourceId} - {Message} [CorrelationId: {CorrelationId}]{ContextSuffix}",
                    resourceId ?? "unknown", notFoundEx.Message, correlationId, contextSuffix);
                break;

            case PlanningCenterApiValidationException validationEx:
                var formattedErrors = string.Join("; ", validationEx.ValidationErrors.SelectMany(kvp => 
                    kvp.Value.Select(msg => $"{kvp.Key}: {msg}")));
                logger.LogWarning("Validation error for {OperationName}: {ValidationErrors} [CorrelationId: {CorrelationId}]{ContextSuffix}",
                    operationName, formattedErrors, correlationId, contextSuffix);
                break;

            case PlanningCenterApiAuthenticationException authEx:
                logger.LogError("Authentication error for {OperationName}: {Message} [CorrelationId: {CorrelationId}]{ContextSuffix}",
                    operationName, authEx.Message, correlationId, contextSuffix);
                break;

            case PlanningCenterApiAuthorizationException authzEx:
                logger.LogError("Authorization error for {OperationName}: {Message} [CorrelationId: {CorrelationId}]{ContextSuffix}",
                    operationName, authzEx.Message, correlationId, contextSuffix);
                break;

            case PlanningCenterApiRateLimitException rateLimitEx:
                logger.LogWarning("Rate limit exceeded for {OperationName}: {Message}, Retry after: {RetryAfter} seconds [CorrelationId: {CorrelationId}]{ContextSuffix}",
                    operationName, rateLimitEx.Message, rateLimitEx.RetryAfterSeconds, correlationId, contextSuffix);
                break;

            case PlanningCenterApiServerException serverEx:
                logger.LogError("Server error for {OperationName}: {Message} [CorrelationId: {CorrelationId}]{ContextSuffix}",
                    operationName, serverEx.Message, correlationId, contextSuffix);
                break;

            case PlanningCenterApiGeneralException apiEx:
                logger.LogError("API error for {OperationName}: {Message} [CorrelationId: {CorrelationId}]{ContextSuffix}",
                    operationName, apiEx.Message, correlationId, contextSuffix);
                break;

            case OperationCanceledException:
                logger.LogWarning("Operation cancelled: {OperationName} was cancelled [CorrelationId: {CorrelationId}]{ContextSuffix}",
                    operationName, correlationId, contextSuffix);
                break;

            case HttpRequestException httpEx:
                logger.LogError(httpEx, "Network error in {OperationName} for resource {ResourceId} [CorrelationId: {CorrelationId}]{ContextSuffix}",
                    operationName, resourceId ?? "unknown", correlationId, contextSuffix);
                break;

            case TimeoutException timeoutEx:
                logger.LogError(timeoutEx, "Operation {OperationName} timed out [CorrelationId: {CorrelationId}]{ContextSuffix}",
                    operationName, correlationId, contextSuffix);
                break;

            default:
                logger.LogError("General error: Unexpected error in {OperationName} for resource {ResourceId} [CorrelationId: {CorrelationId}]{ContextSuffix}",
                    operationName, resourceId, correlationId, contextSuffix);
                break;
        }

        return CreateErrorContext(exception, operationName, resourceId, additionalContext);
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
        if (exception == null)
            throw new ArgumentNullException(nameof(exception));
        var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
        {
            ["Operation"] = operationName,
            ["OperationName"] = operationName,
            ["ExceptionType"] = exception.GetType().Name,
            ["Message"] = exception.Message,
            ["CorrelationId"] = CorrelationContext.Current ?? "unknown"
        };
        
        if (resourceId != null)
        {
            result["ResourceId"] = resourceId;
            result["ResourceId"] = resourceId;
        }
        
        if (exception is PlanningCenterApiException apiEx)
        {
            if (apiEx.RequestId != null) result["RequestId"] = apiEx.RequestId;
            if (apiEx.RequestUrl != null) result["RequestUrl"] = apiEx.RequestUrl;
            if (apiEx.StatusCode.HasValue) result["StatusCode"] = apiEx.StatusCode.Value;
            if (apiEx.ErrorCode != null) result["ErrorCode"] = apiEx.ErrorCode;
        }
        
        if (additionalContext != null)
        {
            foreach (var (key, value) in additionalContext)
            {
                result[key] = value;
            }
        }
        
        return result;
    }
}