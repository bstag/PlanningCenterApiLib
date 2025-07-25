namespace PlanningCenter.Api.Client.Models.Core;

/// <summary>
/// Represents the result of an API health check.
/// </summary>
public class HealthCheckResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the API is healthy.
    /// </summary>
    public bool IsHealthy { get; set; }

    /// <summary>
    /// Gets or sets the overall status of the health check (e.g., "healthy", "unhealthy").
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Gets or sets the response time in milliseconds.
    /// </summary>
    public double ResponseTimeMs { get; set; }

    /// <summary>
    /// Gets or sets the API version.
    /// </summary>
    public string? ApiVersion { get; set; }

    /// <summary>
    /// Gets or sets when the health check was performed.
    /// </summary>
    public DateTime CheckedAt { get; set; }

    /// <summary>
    /// Gets or sets additional health check details.
    /// </summary>
    public Dictionary<string, object> Details { get; set; } = new();
}