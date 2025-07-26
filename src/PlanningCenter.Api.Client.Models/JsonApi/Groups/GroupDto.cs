using System;
using System.Text.Json.Serialization;
using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Groups;

/// <summary>
/// JSON:API DTO for Planning Center Groups Group.
/// Follows Interface Segregation Principle - focused on JSON:API serialization.
/// </summary>
public class GroupDto
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
    public string Type { get; set; } = "Group";

    /// <summary>
    /// Gets or sets the group attributes.
    /// </summary>
    [JsonPropertyName("attributes")]
    public GroupAttributes Attributes { get; set; } = new();

    /// <summary>
    /// Gets or sets the group relationships.
    /// </summary>
    [JsonPropertyName("relationships")]
    public GroupRelationships? Relationships { get; set; }
}

/// <summary>
/// Group attributes for JSON:API.
/// </summary>
public class GroupAttributes
{
    /// <summary>
    /// Gets or sets the group name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the group description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the archive timestamp.
    /// </summary>
    [JsonPropertyName("archived_at")]
    public DateTime? ArchivedAt { get; set; }

    /// <summary>
    /// Gets or sets the group contact email.
    /// </summary>
    [JsonPropertyName("contact_email")]
    public string? ContactEmail { get; set; }

    /// <summary>
    /// Gets or sets the meeting schedule description.
    /// </summary>
    [JsonPropertyName("schedule")]
    public string? Schedule { get; set; }

    /// <summary>
    /// Gets or sets the preferred location type.
    /// </summary>
    [JsonPropertyName("location_type_preference")]
    public string? LocationTypePreference { get; set; }

    /// <summary>
    /// Gets or sets the virtual meeting URL.
    /// </summary>
    [JsonPropertyName("virtual_location_url")]
    public string? VirtualLocationUrl { get; set; }

    /// <summary>
    /// Gets or sets the event visibility settings.
    /// </summary>
    [JsonPropertyName("events_visibility")]
    public string? EventsVisibility { get; set; }

    /// <summary>
    /// Gets or sets the current member count.
    /// </summary>
    [JsonPropertyName("memberships_count")]
    public int MembershipsCount { get; set; }

    /// <summary>
    /// Gets or sets whether members are confidential.
    /// </summary>
    [JsonPropertyName("members_are_confidential")]
    public bool MembersAreConfidential { get; set; }

    /// <summary>
    /// Gets or sets whether leaders can search people database.
    /// </summary>
    [JsonPropertyName("leaders_can_search_people_database")]
    public bool LeadersCanSearchPeopleDatabase { get; set; }

    /// <summary>
    /// Gets or sets whether the group can create conversations.
    /// </summary>
    [JsonPropertyName("can_create_conversation")]
    public bool CanCreateConversation { get; set; }

    /// <summary>
    /// Gets or sets whether chat is enabled.
    /// </summary>
    [JsonPropertyName("chat_enabled")]
    public bool ChatEnabled { get; set; }

    /// <summary>
    /// Gets or sets the public Church Center web URL.
    /// </summary>
    [JsonPropertyName("public_church_center_web_url")]
    public string? PublicChurchCenterWebUrl { get; set; }

    /// <summary>
    /// Gets or sets the group header image object.
    /// </summary>
    [JsonPropertyName("header_image")]
    public HeaderImageDto? HeaderImage { get; set; }

    /// <summary>
    /// Gets or sets the widget status.
    /// </summary>
    [JsonPropertyName("widget_status")]
    public string? WidgetStatus { get; set; }

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
/// Group relationships for JSON:API.
/// </summary>
public class GroupRelationships
{
    /// <summary>
    /// Gets or sets the group type relationship.
    /// </summary>
    [JsonPropertyName("group_type")]
    public RelationshipData? GroupType { get; set; }

    /// <summary>
    /// Gets or sets the location relationship.
    /// </summary>
    [JsonPropertyName("location")]
    public RelationshipData? Location { get; set; }
}