using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.Webhooks;

/// <summary>
/// Represents an event type that can be subscribed to in Planning Center Webhooks.
/// </summary>
public class AvailableEvent : PlanningCenterResource
{
    /// <summary>
    /// Gets or sets the event name (e.g., "person.created", "donation.updated").
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the event description.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the source module (people, giving, calendar, etc.).
    /// </summary>
    public string Module { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the event category or group.
    /// </summary>
    public string? Category { get; set; }
    
    /// <summary>
    /// Gets or sets the event action (created, updated, deleted, etc.).
    /// </summary>
    public string? Action { get; set; }
    
    /// <summary>
    /// Gets or sets the resource type affected by this event.
    /// </summary>
    public string? ResourceType { get; set; }
    
    /// <summary>
    /// Gets or sets whether this event is active and available for subscription.
    /// </summary>
    public bool Active { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the event version or schema version.
    /// </summary>
    public string? Version { get; set; }
    
    /// <summary>
    /// Gets or sets the event payload schema or structure.
    /// </summary>
    public Dictionary<string, object>? PayloadSchema { get; set; }
    
    /// <summary>
    /// Gets or sets example payload data for this event.
    /// </summary>
    public Dictionary<string, object>? ExamplePayload { get; set; }
    
    /// <summary>
    /// Gets or sets the number of active subscriptions for this event.
    /// </summary>
    public int SubscriptionCount { get; set; }
    
    /// <summary>
    /// Gets or sets the total number of times this event has been triggered.
    /// </summary>
    public long TotalTriggerCount { get; set; }
    
    /// <summary>
    /// Gets or sets when this event was last triggered.
    /// </summary>
    public DateTime? LastTriggeredAt { get; set; }
    
    /// <summary>
    /// Gets or sets the average frequency of this event (events per day).
    /// </summary>
    public double? AverageFrequency { get; set; }
    
    /// <summary>
    /// Gets or sets additional documentation or help text.
    /// </summary>
    public string? Documentation { get; set; }
    
    /// <summary>
    /// Gets or sets related event types.
    /// </summary>
    public List<string> RelatedEvents { get; set; } = new();
    
    /// <summary>
    /// Gets or sets additional metadata.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}