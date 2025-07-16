namespace PlanningCenter.Api.Client.Models.Requests;

/// <summary>
/// Request model for creating a webhook subscription.
/// </summary>
public class WebhookSubscriptionCreateRequest
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
    /// Gets or sets the subscription name or description.
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Gets or sets the subscription description.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the available event ID to subscribe to.
    /// </summary>
    public string AvailableEventId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets whether the subscription should be active immediately.
    /// </summary>
    public bool Active { get; set; } = true;
    
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
}

/// <summary>
/// Request model for updating a webhook subscription.
/// </summary>
public class WebhookSubscriptionUpdateRequest
{
    /// <summary>
    /// Gets or sets the webhook endpoint URL.
    /// </summary>
    public string? Url { get; set; }
    
    /// <summary>
    /// Gets or sets the webhook secret for signature validation.
    /// </summary>
    public string? Secret { get; set; }
    
    /// <summary>
    /// Gets or sets the subscription name or description.
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Gets or sets the subscription description.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets whether the subscription is active.
    /// </summary>
    public bool? Active { get; set; }
    
    /// <summary>
    /// Gets or sets the maximum number of retry attempts.
    /// </summary>
    public int? MaxRetries { get; set; }
    
    /// <summary>
    /// Gets or sets the timeout for webhook delivery (in seconds).
    /// </summary>
    public int? TimeoutSeconds { get; set; }
    
    /// <summary>
    /// Gets or sets custom headers to include in webhook requests.
    /// </summary>
    public Dictionary<string, string>? CustomHeaders { get; set; }
}