using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.Webhooks;

/// <summary>
/// Represents a webhook subscription in Planning Center Webhooks.
/// </summary>
public class WebhookSubscription : PlanningCenterResource
{
    /// <summary>
    /// Gets or sets the webhook endpoint URL.
    /// </summary>
    public string Url { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the webhook secret for signature validation.
    /// </summary>
    public string? Secret { get; set; }
    
    /// <summary>
    /// Gets or sets whether the subscription is active.
    /// </summary>
    public bool Active { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the subscription name or description.
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Gets or sets the subscription description.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the associated available event ID.
    /// </summary>
    public string AvailableEventId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the associated organization ID.
    /// </summary>
    public string? OrganizationId { get; set; }
    
    /// <summary>
    /// Gets or sets when the last delivery was attempted.
    /// </summary>
    public DateTime? LastDeliveryAt { get; set; }
    
    /// <summary>
    /// Gets or sets the status of the last delivery attempt.
    /// </summary>
    public string? LastDeliveryStatus { get; set; }
    
    /// <summary>
    /// Gets or sets the HTTP status code of the last delivery.
    /// </summary>
    public int? LastDeliveryStatusCode { get; set; }
    
    /// <summary>
    /// Gets or sets the response time of the last delivery (in milliseconds).
    /// </summary>
    public double? LastDeliveryResponseTimeMs { get; set; }
    
    /// <summary>
    /// Gets or sets the error message from the last failed delivery.
    /// </summary>
    public string? LastDeliveryError { get; set; }
    
    /// <summary>
    /// Gets or sets the total number of events delivered.
    /// </summary>
    public long TotalDeliveries { get; set; }
    
    /// <summary>
    /// Gets or sets the number of successful deliveries.
    /// </summary>
    public long SuccessfulDeliveries { get; set; }
    
    /// <summary>
    /// Gets or sets the number of failed deliveries.
    /// </summary>
    public long FailedDeliveries { get; set; }
    
    /// <summary>
    /// Gets or sets the delivery success rate as a percentage.
    /// </summary>
    public double SuccessRate => TotalDeliveries > 0 ? (double)SuccessfulDeliveries / TotalDeliveries * 100 : 0;
    
    /// <summary>
    /// Gets or sets the retry policy for failed deliveries.
    /// </summary>
    public string? RetryPolicy { get; set; }
    
    /// <summary>
    /// Gets or sets the maximum number of retry attempts.
    /// </summary>
    public int MaxRetries { get; set; } = 3;
    
    /// <summary>
    /// Gets or sets the timeout for webhook delivery (in seconds).
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;
    
    /// <summary>
    /// Gets or sets custom headers to include in webhook requests.
    /// </summary>
    public Dictionary<string, string> CustomHeaders { get; set; } = new();
    
    /// <summary>
    /// Gets or sets additional metadata.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}