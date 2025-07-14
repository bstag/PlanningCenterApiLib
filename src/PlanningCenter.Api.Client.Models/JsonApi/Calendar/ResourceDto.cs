using System;
using System.Text.Json.Serialization;

namespace PlanningCenter.Api.Client.Models.JsonApi.Calendar;

/// <summary>
/// JSON:API DTO for Planning Center Calendar Resource.
/// Follows Interface Segregation Principle - focused on JSON:API serialization.
/// </summary>
public class ResourceDto
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
    public string Type { get; set; } = "Resource";

    /// <summary>
    /// Gets or sets the resource attributes.
    /// </summary>
    [JsonPropertyName("attributes")]
    public ResourceAttributes Attributes { get; set; } = new();
}

/// <summary>
/// Resource attributes for JSON:API.
/// </summary>
public class ResourceAttributes
{
    /// <summary>
    /// Gets or sets the resource name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the resource description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the resource kind.
    /// </summary>
    [JsonPropertyName("kind")]
    public string? Kind { get; set; }

    /// <summary>
    /// Gets or sets the home location.
    /// </summary>
    [JsonPropertyName("home_location")]
    public string? HomeLocation { get; set; }

    /// <summary>
    /// Gets or sets the serial number.
    /// </summary>
    [JsonPropertyName("serial_number")]
    public string? SerialNumber { get; set; }

    /// <summary>
    /// Gets or sets whether the resource expires.
    /// </summary>
    [JsonPropertyName("expires")]
    public bool Expires { get; set; }

    /// <summary>
    /// Gets or sets the expiration date.
    /// </summary>
    [JsonPropertyName("expires_at")]
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets the renewal date.
    /// </summary>
    [JsonPropertyName("renewal_at")]
    public DateTime? RenewalAt { get; set; }

    /// <summary>
    /// Gets or sets the quantity available.
    /// </summary>
    [JsonPropertyName("quantity")]
    public int? Quantity { get; set; }

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