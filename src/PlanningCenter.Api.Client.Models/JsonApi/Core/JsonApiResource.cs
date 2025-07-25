using System.Text.Json.Serialization;

namespace PlanningCenter.Api.Client.Models.JsonApi.Core;

/// <summary>
/// Base class for all JSON:API resource objects.
/// Implements the JSON:API 1.0 specification for resource objects.
/// </summary>
public abstract class JsonApiResource : IResourceObject
{
    /// <summary>
    /// Gets or sets the resource type identifier.
    /// This MUST be the same as the type used in the API endpoint.
    /// </summary>
    [JsonPropertyName("type")]
    public virtual string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the resource identifier.
    /// This MUST be unique within the resource type.
    /// </summary>
    [JsonPropertyName("id")]
    public virtual string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the resource links.
    /// Contains URLs related to the resource.
    /// </summary>
    [JsonPropertyName("links")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public virtual ResourceLinks? Links { get; set; }

    /// <summary>
    /// Gets or sets the resource meta information.
    /// Contains non-standard meta-information about the resource.
    /// </summary>
    [JsonPropertyName("meta")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public virtual Dictionary<string, object>? Meta { get; set; }
}

/// <summary>
/// Base class for JSON:API resource objects with typed attributes.
/// </summary>
/// <typeparam name="TAttributes">The type of the attributes object</typeparam>
public abstract class JsonApiResource<TAttributes> : JsonApiResource
    where TAttributes : class, new()
{
    /// <summary>
    /// Gets or sets the resource attributes.
    /// Contains the resource's data attributes.
    /// </summary>
    [JsonPropertyName("attributes")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public virtual TAttributes? Attributes { get; set; }
}

/// <summary>
/// Base class for JSON:API resource objects with typed attributes and relationships.
/// </summary>
/// <typeparam name="TAttributes">The type of the attributes object</typeparam>
/// <typeparam name="TRelationships">The type of the relationships object</typeparam>
public abstract class JsonApiResource<TAttributes, TRelationships> : JsonApiResource<TAttributes>
    where TAttributes : class, new()
    where TRelationships : class, new()
{
    /// <summary>
    /// Gets or sets the resource relationships.
    /// Contains references to related resources.
    /// </summary>
    [JsonPropertyName("relationships")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public virtual TRelationships? Relationships { get; set; }
}