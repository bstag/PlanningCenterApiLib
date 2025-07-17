using System.Diagnostics;
using Microsoft.Extensions.Logging;
using PlanningCenter.Api.Client.Correlation;

namespace PlanningCenter.Api.Client.Monitoring;

/// <summary>
/// Provides performance monitoring capabilities for API operations.
/// </summary>
public static class PerformanceMonitor
{
    /// <summary>
    /// Tracks the execution time of an operation and logs performance metrics.
    /// </summary>
    /// <typeparam name="T">The return type of the operation</typeparam>
    /// <param name="logger">The logger instance</param>
    /// <param name="operationName">The name of the operation being tracked</param>
    /// <param name="operation">The operation to execute</param>
    /// <param name="resourceId">Optional resource identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of the operation</returns>
    public static async Task<T> TrackAsync<T>(
        ILogger logger,
        string operationName,
        Func<Task<T>> operation,
        string? resourceId = null,
        CancellationToken cancellationToken = default)
    {
        var correlationId = CorrelationContext.GetOrGenerate();
        var stopwatch = Stopwatch.StartNew();
        
        using var scope = logger.BeginScope(new Dictionary<string, object?>
        {
            ["OperationName"] = operationName,
            ["CorrelationId"] = correlationId,
            ["ResourceId"] = resourceId
        });

        logger.LogDebug("Starting operation {OperationName} [CorrelationId: {CorrelationId}]", 
            operationName, correlationId);

        try
        {
            var result = await operation();
            stopwatch.Stop();

            logger.LogInformation("Operation {OperationName} completed successfully in {ElapsedMs}ms [CorrelationId: {CorrelationId}]",
                operationName, stopwatch.ElapsedMilliseconds, correlationId);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            logger.LogError(ex, "Operation {OperationName} failed after {ElapsedMs}ms [CorrelationId: {CorrelationId}]",
                operationName, stopwatch.ElapsedMilliseconds, correlationId);
            
            throw;
        }
    }

    /// <summary>
    /// Tracks the execution time of an operation without a return value.
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="operationName">The name of the operation being tracked</param>
    /// <param name="operation">The operation to execute</param>
    /// <param name="resourceId">Optional resource identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public static async Task TrackAsync(
        ILogger logger,
        string operationName,
        Func<Task> operation,
        string? resourceId = null,
        CancellationToken cancellationToken = default)
    {
        await TrackAsync(logger, operationName, async () =>
        {
            await operation();
            return Task.CompletedTask;
        }, resourceId, cancellationToken);
    }

    /// <summary>
    /// Creates a performance tracking scope that logs the operation timing when disposed.
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="operationName">The name of the operation being tracked</param>
    /// <param name="resourceId">Optional resource identifier</param>
    /// <returns>A disposable performance tracking scope</returns>
    public static PerformanceTrackingScope CreateScope(
        ILogger logger,
        string operationName,
        string? resourceId = null)
    {
        return new PerformanceTrackingScope(logger, operationName, resourceId);
    }
}

/// <summary>
/// A disposable scope for tracking operation performance.
/// </summary>
public sealed class PerformanceTrackingScope : IDisposable
{
    private readonly ILogger _logger;
    private readonly string _operationName;
    private readonly string? _resourceId;
    private readonly string _correlationId;
    private readonly Stopwatch _stopwatch;
    private readonly IDisposable? _loggerScope;
    private bool _disposed;

    internal PerformanceTrackingScope(ILogger logger, string operationName, string? resourceId)
    {
        _logger = logger;
        _operationName = operationName;
        _resourceId = resourceId;
        _correlationId = CorrelationContext.GetOrGenerate();
        _stopwatch = Stopwatch.StartNew();
        
        _loggerScope = _logger.BeginScope(new Dictionary<string, object?>
        {
            ["OperationName"] = operationName,
            ["CorrelationId"] = _correlationId,
            ["ResourceId"] = resourceId
        });

        _logger.LogDebug("Starting operation {OperationName} [CorrelationId: {CorrelationId}]", 
            _operationName, _correlationId);
    }

    /// <summary>
    /// Marks the operation as completed successfully.
    /// </summary>
    public void Success()
    {
        if (_disposed) return;

        _stopwatch.Stop();
        _logger.LogInformation("Operation {OperationName} completed successfully in {ElapsedMs}ms [CorrelationId: {CorrelationId}]",
            _operationName, _stopwatch.ElapsedMilliseconds, _correlationId);
    }

    /// <summary>
    /// Marks the operation as failed with the specified exception.
    /// </summary>
    /// <param name="exception">The exception that caused the failure</param>
    public void Failure(Exception exception)
    {
        if (_disposed) return;

        _stopwatch.Stop();
        _logger.LogError(exception, "Operation {OperationName} failed after {ElapsedMs}ms [CorrelationId: {CorrelationId}]",
            _operationName, _stopwatch.ElapsedMilliseconds, _correlationId);
    }

    /// <summary>
    /// Disposes the tracking scope and logs completion if not already logged.
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;

        if (_stopwatch.IsRunning)
        {
            _stopwatch.Stop();
            _logger.LogDebug("Operation {OperationName} completed in {ElapsedMs}ms [CorrelationId: {CorrelationId}]",
                _operationName, _stopwatch.ElapsedMilliseconds, _correlationId);
        }

        _loggerScope?.Dispose();
        _disposed = true;
    }
}