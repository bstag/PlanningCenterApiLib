using System;
using System.Text.Json.Serialization;
using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Services;

/// <summary>
/// JSON:API DTO for Planning Center Services Item.
/// Follows Interface Segregation Principle - focused on JSON:API serialization.
/// </summary>
public class ItemDto
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
    public string Type { get; set; } = "Item";

    /// <summary>
    /// Gets or sets the item attributes.
    /// </summary>
    [JsonPropertyName("attributes")]
    public ItemAttributes Attributes { get; set; } = new();

    /// <summary>
    /// Gets or sets the item relationships.
    /// </summary>
    [JsonPropertyName("relationships")]
    public ItemRelationships? Relationships { get; set; }
}

/// <summary>
/// Item attributes for JSON:API.
/// </summary>
public class ItemAttributes
{
    /// <summary>
    /// Gets or sets the item title.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the sequence/order in the plan.
    /// </summary>
    [JsonPropertyName("sequence")]
    public int Sequence { get; set; }

    /// <summary>
    /// Gets or sets the item type.
    /// </summary>
    [JsonPropertyName("item_type")]
    public string? ItemType { get; set; }

    /// <summary>
    /// Gets or sets the item description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the key signature.
    /// </summary>
    [JsonPropertyName("key_name")]
    public string? KeyName { get; set; }

    /// <summary>
    /// Gets or sets the length in minutes.
    /// </summary>
    [JsonPropertyName("length")]
    public int? Length { get; set; }

    /// <summary>
    /// Gets or sets the service position.
    /// </summary>
    [JsonPropertyName("service_position")]
    public string? ServicePosition { get; set; }

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
/// Item relationships for JSON:API.
/// </summary>
public class ItemRelationships
{
    /// <summary>
    /// Gets or sets the plan relationship.
    /// </summary>
    [JsonPropertyName("plan")]
    public RelationshipData? Plan { get; set; }

    /// <summary>
    /// Gets or sets the song relationship.
    /// </summary>
    [JsonPropertyName("song")]
    public RelationshipData? Song { get; set; }

    /// <summary>
    /// Gets or sets the arrangement relationship.
    /// </summary>
    [JsonPropertyName("arrangement")]
    public RelationshipData? Arrangement { get; set; }
}