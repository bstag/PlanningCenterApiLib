using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.Webhooks;

/// <summary>
/// Represents a delivered webhook event in Planning Center Webhooks.
/// </summary>
public class Event : PlanningCenterResource
{
    /// <summary>
    /// Gets or sets the associated webhook subscription ID.
    /// </summary>
    public string WebhookSubscriptionId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the associated available event ID.
    /// </summary>
    public string AvailableEventId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the event name.
    /// </summary>
    public string EventName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the event payload data.
    /// </summary>
    public Dictionary<string, object> Payload { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the delivery status.
    /// </summary>
    public string DeliveryStatus { get; set; } = "pending";
    
    /// <summary>
    /// Gets or sets when the event was delivered.
    /// </summary>
    public DateTime? DeliveredAt { get; set; }
    
    /// <summary>
    /// Gets or sets the HTTP status code returned by the webhook endpoint.
    /// </summary>
    public int? ResponseStatusCode { get; set; }
    
    /// <summary>
    /// Gets or sets the response body from the webhook endpoint.
    /// </summary>
    public string? ResponseBody { get; set; }
    
    /// <summary>
    /// Gets or sets the response headers from the webhook endpoint.
    /// </summary>
    public Dictionary<string, string> ResponseHeaders { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the delivery response time in milliseconds.
    /// </summary>
    public double? ResponseTimeMs { get; set; }
    
    /// <summary>
    /// Gets or sets the error message if delivery failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// Gets or sets the number of delivery attempts.
    /// </summary>
    public int DeliveryAttempts { get; set; } = 0;
    
    /// <summary>
    /// Gets or sets the maximum number of retry attempts allowed.
    /// </summary>
    public int MaxRetries { get; set; } = 3;
    
    /// <summary>
    /// Gets or sets when the next retry attempt is scheduled.
    /// </summary>
    public DateTime? NextRetryAt { get; set; }
    
    /// <summary>
    /// Gets or sets when the event will expire and no longer be retried.
    /// </summary>
    public DateTime? ExpiresAt { get; set; }
    
    /// <summary>
    /// Gets or sets the webhook signature for payload verification.
    /// </summary>
    public string? Signature { get; set; }
    
    /// <summary>
    /// Gets or sets the event version or schema version.
    /// </summary>
    public string? Version { get; set; }
    
    /// <summary>
    /// Gets or sets the source module that triggered this event.
    /// </summary>
    public string? SourceModule { get; set; }
    
    /// <summary>
    /// Gets or sets the resource ID that triggered this event.
    /// </summary>
    public string? ResourceId { get; set; }
    
    /// <summary>
    /// Gets or sets the resource type that triggered this event.
    /// </summary>
    public string? ResourceType { get; set; }
    
    /// <summary>
    /// Gets or sets the action that triggered this event.
    /// </summary>
    public string? Action { get; set; }
    
    /// <summary>
    /// Gets or sets the user or system that triggered this event.
    /// </summary>
    public string? TriggeredBy { get; set; }
    
    /// <summary>
    /// Gets or sets when the original event occurred.
    /// </summary>
    public DateTime? OccurredAt { get; set; }
    
    /// <summary>
    /// Gets or sets additional metadata.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}