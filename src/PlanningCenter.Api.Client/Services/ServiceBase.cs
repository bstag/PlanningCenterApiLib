using Microsoft.Extensions.Logging;
using PlanningCenter.Api.Client.Correlation;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Exceptions;
using PlanningCenter.Api.Client.Monitoring;

namespace PlanningCenter.Api.Client.Services;

/// <summary>
/// Base class for Planning Center API services with standardized logging and exception handling.
/// </summary>
public abstract class ServiceBase
{
    protected readonly ILogger Logger;
    protected readonly IApiConnection ApiConnection;

    protected ServiceBase(ILogger logger, IApiConnection apiConnection)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        ApiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
    }

    /// <summary>
    /// Executes an operation with standardized exception handling and performance monitoring.
    /// </summary>
    /// <typeparam name="T">The return type of the operation</typeparam>
    /// <param name="operation">The operation to execute</param>
    /// <param name="operationName">The name of the operation for logging</param>
    /// <param name="resourceId">Optional resource identifier</param>
    /// <param name="allowNotFound">Whether to return null for NotFound exceptions instead of throwing</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of the operation</returns>
    protected async Task<T> ExecuteAsync<T>(
        Func<Task<T>> operation,
        string operationName,
        string? resourceId = null,
        bool allowNotFound = false,
        CancellationToken cancellationToken = default)
    {
        return await PerformanceMonitor.TrackAsync(
            Logger,
            operationName,
            async () =>
            {
                try
                {
                    return await operation();
                }
                catch (PlanningCenterApiNotFoundException) when (allowNotFound)
                {
                    var correlationId = CorrelationContext.Current;
                    Logger.LogWarning("Resource not found: {ResourceId} [CorrelationId: {CorrelationId}]", 
                        resourceId ?? "unknown", correlationId);
                    return default(T)!;
                }
                catch (PlanningCenterApiException ex)
                {
                    var correlationId = CorrelationContext.Current;
                    Logger.LogError(ex, "API error in {OperationName} for resource {ResourceId}: {ErrorMessage} [CorrelationId: {CorrelationId}]",
                        operationName, resourceId ?? "unknown", ex.Message, correlationId);
                    throw;
                }
                catch (Exception ex)
                {
                    var correlationId = CorrelationContext.Current;
                    Logger.LogError(ex, "Unexpected error in {OperationName} for resource {ResourceId} [CorrelationId: {CorrelationId}]",
                        operationName, resourceId ?? "unknown", correlationId);
                    throw;
                }
            },
            resourceId,
            cancellationToken);
    }

    /// <summary>
    /// Executes an operation without a return value with standardized exception handling and performance monitoring.
    /// </summary>
    /// <param name="operation">The operation to execute</param>
    /// <param name="operationName">The name of the operation for logging</param>
    /// <param name="resourceId">Optional resource identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    protected async Task ExecuteAsync(
        Func<Task> operation,
        string operationName,
        string? resourceId = null,
        CancellationToken cancellationToken = default)
    {
        await PerformanceMonitor.TrackAsync(
            Logger,
            operationName,
            operation,
            resourceId,
            cancellationToken);
    }

    /// <summary>
    /// Executes a get operation that may return null for not found resources.
    /// </summary>
    /// <typeparam name="T">The return type of the operation</typeparam>
    /// <param name="operation">The operation to execute</param>
    /// <param name="operationName">The name of the operation for logging</param>
    /// <param name="resourceId">The resource identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of the operation or null if not found</returns>
    protected async Task<T?> ExecuteGetAsync<T>(
        Func<Task<T>> operation,
        string operationName,
        string resourceId,
        CancellationToken cancellationToken = default)
        where T : class
    {
        return await ExecuteAsync(operation, operationName, resourceId, allowNotFound: true, cancellationToken);
    }

    /// <summary>
    /// Validates that the provided parameter is not null or empty.
    /// </summary>
    /// <param name="value">The value to validate</param>
    /// <param name="paramName">The parameter name</param>
    /// <exception cref="ArgumentException">Thrown when the value is null or empty</exception>
    protected static void ValidateNotNullOrEmpty(string? value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{paramName} cannot be null or empty", paramName);
        }
    }

    /// <summary>
    /// Validates that the provided parameter is not null.
    /// </summary>
    /// <param name="value">The value to validate</param>
    /// <param name="paramName">The parameter name</param>
    /// <exception cref="ArgumentNullException">Thrown when the value is null</exception>
    protected static void ValidateNotNull<T>(T? value, string paramName) where T : class
    {
        if (value == null)
        {
            throw new ArgumentNullException(paramName);
        }
    }
}