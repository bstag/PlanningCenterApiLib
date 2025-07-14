using System;
using System.Text.Json.Serialization;
using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Groups;

/// <summary>
/// JSON:API DTO for Planning Center Groups Membership.
/// Follows Interface Segregation Principle - focused on JSON:API serialization.
/// </summary>
public class MembershipDto
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
    public string Type { get; set; } = "Membership";

    /// <summary>
    /// Gets or sets the membership attributes.
    /// </summary>
    [JsonPropertyName("attributes")]
    public MembershipAttributes Attributes { get; set; } = new();

    /// <summary>
    /// Gets or sets the membership relationships.
    /// </summary>
    [JsonPropertyName("relationships")]
    public MembershipRelationships? Relationships { get; set; }
}

/// <summary>
/// Membership attributes for JSON:API.
/// </summary>
public class MembershipAttributes
{
    /// <summary>
    /// Gets or sets the membership role.
    /// </summary>
    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the join date.
    /// </summary>
    [JsonPropertyName("joined_at")]
    public DateTime? JoinedAt { get; set; }

    /// <summary>
    /// Gets or sets whether the member can create events.
    /// </summary>
    [JsonPropertyName("can_create_events")]
    public bool CanCreateEvents { get; set; }

    /// <summary>
    /// Gets or sets whether the member can edit group.
    /// </summary>
    [JsonPropertyName("can_edit_group")]
    public bool CanEditGroup { get; set; }

    /// <summary>
    /// Gets or sets whether the member can manage members.
    /// </summary>
    [JsonPropertyName("can_manage_members")]
    public bool CanManageMembers { get; set; }

    /// <summary>
    /// Gets or sets whether the member can view members.
    /// </summary>
    [JsonPropertyName("can_view_members")]
    public bool CanViewMembers { get; set; }

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
/// Membership relationships for JSON:API.
/// </summary>
public class MembershipRelationships
{
    /// <summary>
    /// Gets or sets the person relationship.
    /// </summary>
    [JsonPropertyName("person")]
    public RelationshipData? Person { get; set; }

    /// <summary>
    /// Gets or sets the group relationship.
    /// </summary>
    [JsonPropertyName("group")]
    public RelationshipData? Group { get; set; }
}