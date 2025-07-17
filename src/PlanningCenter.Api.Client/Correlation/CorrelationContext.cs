namespace PlanningCenter.Api.Client.Correlation;

/// <summary>
/// Provides correlation context for tracking requests across service calls.
/// </summary>
public static class CorrelationContext
{
    private static readonly AsyncLocal<string?> _correlationId = new();

    /// <summary>
    /// Gets the current correlation ID for the request.
    /// </summary>
    public static string? Current => _correlationId.Value;

    /// <summary>
    /// Sets the correlation ID for the current request.
    /// </summary>
    /// <param name="correlationId">The correlation ID to set</param>
    public static void Set(string correlationId)
    {
        _correlationId.Value = correlationId;
    }

    /// <summary>
    /// Generates a new correlation ID and sets it as current.
    /// </summary>
    /// <returns>The generated correlation ID</returns>
    public static string GenerateNew()
    {
        var correlationId = Guid.NewGuid().ToString("N")[..8]; // Short 8-character ID
        Set(correlationId);
        return correlationId;
    }

    /// <summary>
    /// Gets the current correlation ID or generates a new one if none exists.
    /// </summary>
    /// <returns>The correlation ID</returns>
    public static string GetOrGenerate()
    {
        return Current ?? GenerateNew();
    }

    /// <summary>
    /// Clears the current correlation ID.
    /// </summary>
    public static void Clear()
    {
        _correlationId.Value = null;
    }
}