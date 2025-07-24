using System;
using System.Text.Json.Serialization;
using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Calendar;

/// <summary>
/// JSON:API DTO for Planning Center Calendar Event.
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
    
    /// <summary>
    /// Gets or sets the event relationships.
    /// </summary>
    [JsonPropertyName("relationships")]
    public EventRelationships? Relationships { get; set; }
    
    /// <summary>
    /// Gets or sets the event links.
    /// </summary>
    [JsonPropertyName("links")]
    public EventLinks? Links { get; set; }
    
    /// <summary>
    /// Gets or sets the event meta information.
    /// </summary>
    [JsonPropertyName("meta")]
    public object? Meta { get; set; }
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
    /// Gets or sets the event summary.
    /// </summary>
    [JsonPropertyName("summary")]
    public string? Summary { get; set; }

    /// <summary>
    /// Gets or sets the event description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets whether this is an all-day event.
    /// </summary>
    [JsonPropertyName("all_day_event")]
    public bool AllDayEvent { get; set; }

    /// <summary>
    /// Gets or sets the event start time.
    /// </summary>
    [JsonPropertyName("starts_at")]
    public DateTime? StartsAt { get; set; }

    /// <summary>
    /// Gets or sets the event end time.
    /// </summary>
    [JsonPropertyName("ends_at")]
    public DateTime? EndsAt { get; set; }

    /// <summary>
    /// Gets or sets the location name.
    /// </summary>
    [JsonPropertyName("location_name")]
    public string? LocationName { get; set; }

    /// <summary>
    /// Gets or sets the details URL.
    /// </summary>
    [JsonPropertyName("details_url")]
    public string? DetailsUrl { get; set; }

    /// <summary>
    /// Gets or sets whether the event is visible in Church Center.
    /// </summary>
    [JsonPropertyName("visible_in_church_center")]
    public bool VisibleInChurchCenter { get; set; }

    /// <summary>
    /// Gets or sets whether registration is required.
    /// </summary>
    [JsonPropertyName("registration_required")]
    public bool RegistrationRequired { get; set; }

    /// <summary>
    /// Gets or sets the registration URL.
    /// </summary>
    [JsonPropertyName("registration_url")]
    public string? RegistrationUrl { get; set; }

    /// <summary>
    /// Gets or sets the owner name.
    /// </summary>
    [JsonPropertyName("owner_name")]
    public string? OwnerName { get; set; }

    /// <summary>
    /// Gets or sets the approval status.
    /// </summary>
    [JsonPropertyName("approval_status")]
    public string? ApprovalStatus { get; set; }

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

/// <summary>
/// Event relationships for JSON:API.
/// </summary>
 public class EventRelationships
{
    /// <summary>
    /// Gets or sets the event instances relationship.
    /// </summary>
    [JsonPropertyName("event_instances")]
    public RelationshipData? EventInstances { get; set; }
    
    /// <summary>
    /// Gets or sets the event resource requests relationship.
    /// </summary>
    [JsonPropertyName("event_resource_requests")]
    public RelationshipData? EventResourceRequests { get; set; }
    
    /// <summary>
    /// Gets or sets the owner relationship.
    /// </summary>
    [JsonPropertyName("owner")]
    public RelationshipData? Owner { get; set; }
    
    /// <summary>
    /// Gets or sets the tags relationship.
    /// </summary>
    [JsonPropertyName("tags")]
    public RelationshipData? Tags { get; set; }
}

/// <summary>
/// Event links for JSON:API.
/// </summary>
public class EventLinks
{
    /// <summary>
    /// Gets or sets the self link.
    /// </summary>
    [JsonPropertyName("self")]
    public string? Self { get; set; }
}