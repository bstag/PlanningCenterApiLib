using System;
using System.Text.Json.Serialization;

namespace PlanningCenter.Api.Client.Models.JsonApi.CheckIns;

/// <summary>
/// JSON:API DTO for Planning Center Check-Ins Event.
/// Follows Interface Segregation Principle - focused on JSON:API serialization.
/// </summary>
public class EventDto
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the resource type.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = "Event";

    /// <summary>
    /// Gets or sets the event attributes.
    /// </summary>
    [JsonPropertyName("attributes")]
    public EventAttributes Attributes { get; set; } = new();
}

/// <summary>
/// Event attributes for JSON:API.
/// </summary>
public class EventAttributes
{
    /// <summary>
    /// Gets or sets the event name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the event frequency.
    /// </summary>
    [JsonPropertyName("frequency")]
    public string? Frequency { get; set; }

    /// <summary>
    /// Gets or sets whether the event enables services integration.
    /// </summary>
    [JsonPropertyName("enable_services_integration")]
    public bool EnableServicesIntegration { get; set; }

    /// <summary>
    /// Gets or sets whether the event is archived.
    /// </summary>
    [JsonPropertyName("archived")]
    public bool Archived { get; set; }

    /// <summary>
    /// Gets or sets the creation date.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update date.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}