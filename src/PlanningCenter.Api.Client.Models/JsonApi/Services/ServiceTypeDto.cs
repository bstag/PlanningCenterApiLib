using System;
using System.Text.Json.Serialization;

namespace PlanningCenter.Api.Client.Models.JsonApi.Services;

/// <summary>
/// JSON:API DTO for Planning Center Services ServiceType.
/// Follows Interface Segregation Principle - focused on JSON:API serialization.
/// </summary>
public class ServiceTypeDto
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
    public string Type { get; set; } = "ServiceType";

    /// <summary>
    /// Gets or sets the service type attributes.
    /// </summary>
    [JsonPropertyName("attributes")]
    public ServiceTypeAttributes Attributes { get; set; } = new();
}

/// <summary>
/// ServiceType attributes for JSON:API.
/// </summary>
public class ServiceTypeAttributes
{
    /// <summary>
    /// Gets or sets the service type name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the sequence/sort order.
    /// </summary>
    [JsonPropertyName("sequence")]
    public int Sequence { get; set; }

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