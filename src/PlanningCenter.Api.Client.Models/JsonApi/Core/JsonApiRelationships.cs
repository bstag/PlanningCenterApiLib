using System.Text.Json.Serialization;

namespace PlanningCenter.Api.Client.Models.JsonApi.Core;

/// <summary>
/// Base class for JSON:API resource relationships.
/// Provides common functionality for all relationship objects.
/// </summary>
public abstract class JsonApiRelationships
{
    // This class serves as a base for all relationship objects
    // Individual relationship properties are defined in derived classes
}

/// <summary>
/// Represents a JSON:API relationship object.
/// Contains data, links, and meta information for a relationship.
/// </summary>
public class JsonApiRelationship
{
    /// <summary>
    /// Gets or sets the relationship data.
    /// Can be null, a single RelationshipData object, or an array of RelationshipData objects.
    /// </summary>
    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Data { get; set; }

    /// <summary>
    /// Gets or sets the relationship links.
    /// Contains URLs related to the relationship.
    /// </summary>
    [JsonPropertyName("links")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public RelationshipLinks? Links { get; set; }

    /// <summary>
    /// Gets or sets the relationship meta information.
    /// Contains non-standard meta-information about the relationship.
    /// </summary>
    [JsonPropertyName("meta")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, object>? Meta { get; set; }
}

/// <summary>
/// Represents a JSON:API relationship with a single related resource.
/// </summary>
public class JsonApiToOneRelationship : JsonApiRelationship
{
    /// <summary>
    /// Gets or sets the related resource data.
    /// </summary>
    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public new RelationshipData? Data { get; set; }
}

/// <summary>
/// Represents a JSON:API relationship with multiple related resources.
/// </summary>
public class JsonApiToManyRelationship : JsonApiRelationship
{
    /// <summary>
    /// Gets or sets the related resources data.
    /// </summary>
    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public new List<RelationshipData>? Data { get; set; }
}

/// <summary>
/// Represents links for a JSON:API relationship.
/// </summary>
public class RelationshipLinks
{
    /// <summary>
    /// Gets or sets the self link for the relationship.
    /// </summary>
    [JsonPropertyName("self")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Self { get; set; }

    /// <summary>
    /// Gets or sets the related link for the relationship.
    /// </summary>
    [JsonPropertyName("related")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Related { get; set; }
}