namespace PlanningCenter.Api.Client.Correlation;

/// <summary>
/// Provides a disposable scope for correlation context.
/// </summary>
public sealed class CorrelationScope : IDisposable
{
    private readonly string? _previousCorrelationId;
    private bool _disposed;

    /// <summary>
    /// Creates a new correlation scope with the specified correlation ID.
    /// </summary>
    /// <param name="correlationId">The correlation ID for this scope</param>
    public CorrelationScope(string correlationId)
    {
        _previousCorrelationId = CorrelationContext.Current;
        CorrelationContext.Set(correlationId);
        CorrelationId = correlationId;
    }

    /// <summary>
    /// Gets the correlation ID for this scope.
    /// </summary>
    public string CorrelationId { get; }

    /// <summary>
    /// Creates a new correlation scope with a generated correlation ID.
    /// </summary>
    /// <returns>A new correlation scope</returns>
    public static CorrelationScope Create()
    {
        return new CorrelationScope(CorrelationContext.GenerateNew());
    }

    /// <summary>
    /// Creates a new correlation scope with the specified correlation ID.
    /// </summary>
    /// <param name="correlationId">The correlation ID for this scope</param>
    /// <returns>A new correlation scope</returns>
    public static CorrelationScope Create(string correlationId)
    {
        return new CorrelationScope(correlationId);
    }

    /// <summary>
    /// Disposes the correlation scope and restores the previous correlation ID.
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;

        if (_previousCorrelationId != null)
        {
            CorrelationContext.Set(_previousCorrelationId);
        }
        else
        {
            CorrelationContext.Clear();
        }

        _disposed = true;
    }
}