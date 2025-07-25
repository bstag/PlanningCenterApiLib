using System.Text.Json.Serialization;

namespace PlanningCenter.Api.Client.Models.JsonApi.Core;

/// <summary>
/// Base class for JSON:API resource attributes.
/// Provides common functionality for all attribute objects.
/// </summary>
public abstract class JsonApiAttributes
{
    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    [JsonPropertyName("created_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public virtual DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update timestamp.
    /// </summary>
    [JsonPropertyName("updated_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public virtual DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Base class for JSON:API resource attributes that support archiving.
/// </summary>
public abstract class ArchivableJsonApiAttributes : JsonApiAttributes
{
    /// <summary>
    /// Gets or sets the archive timestamp.
    /// When set, indicates the resource has been archived.
    /// </summary>
    [JsonPropertyName("archived_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public virtual DateTime? ArchivedAt { get; set; }
}

/// <summary>
/// Base class for JSON:API resource attributes that support soft deletion.
/// </summary>
public abstract class SoftDeletableJsonApiAttributes : JsonApiAttributes
{
    /// <summary>
    /// Gets or sets the deletion timestamp.
    /// When set, indicates the resource has been soft deleted.
    /// </summary>
    [JsonPropertyName("deleted_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public virtual DateTime? DeletedAt { get; set; }
}

/// <summary>
/// Base class for JSON:API resource attributes that support both archiving and soft deletion.
/// </summary>
public abstract class ArchivableSoftDeletableJsonApiAttributes : JsonApiAttributes
{
    /// <summary>
    /// Gets or sets the archive timestamp.
    /// When set, indicates the resource has been archived.
    /// </summary>
    [JsonPropertyName("archived_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public virtual DateTime? ArchivedAt { get; set; }

    /// <summary>
    /// Gets or sets the deletion timestamp.
    /// When set, indicates the resource has been soft deleted.
    /// </summary>
    [JsonPropertyName("deleted_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public virtual DateTime? DeletedAt { get; set; }
}