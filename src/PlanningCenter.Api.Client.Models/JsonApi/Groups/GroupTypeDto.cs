using System;
using System.Text.Json.Serialization;

namespace PlanningCenter.Api.Client.Models.JsonApi.Groups;

/// <summary>
/// JSON:API DTO for Planning Center Groups GroupType.
/// Follows Interface Segregation Principle - focused on JSON:API serialization.
/// </summary>
public class GroupTypeDto
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
    public string Type { get; set; } = "GroupType";

    /// <summary>
    /// Gets or sets the group type attributes.
    /// </summary>
    [JsonPropertyName("attributes")]
    public GroupTypeAttributes Attributes { get; set; } = new();
}

/// <summary>
/// GroupType attributes for JSON:API.
/// </summary>
public class GroupTypeAttributes
{
    /// <summary>
    /// Gets or sets the type name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets whether the type is visible in Church Center.
    /// </summary>
    [JsonPropertyName("church_center_visible")]
    public bool ChurchCenterVisible { get; set; }

    /// <summary>
    /// Gets or sets the display color.
    /// </summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }

    /// <summary>
    /// Gets or sets the sort order position.
    /// </summary>
    [JsonPropertyName("position")]
    public int Position { get; set; }

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