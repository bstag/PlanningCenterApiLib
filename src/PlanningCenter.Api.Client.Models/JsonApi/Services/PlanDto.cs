using System;
using System.Text.Json.Serialization;
using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Services;

/// <summary>
/// JSON:API DTO for Planning Center Services Plan.
/// Follows Interface Segregation Principle - focused on JSON:API serialization.
/// </summary>
public class PlanDto
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
    public string Type { get; set; } = "Plan";

    /// <summary>
    /// Gets or sets the plan attributes.
    /// </summary>
    [JsonPropertyName("attributes")]
    public PlanAttributes Attributes { get; set; } = new();

    /// <summary>
    /// Gets or sets the plan relationships.
    /// </summary>
    [JsonPropertyName("relationships")]
    public PlanRelationships? Relationships { get; set; }
}

/// <summary>
/// Plan attributes for JSON:API.
/// </summary>
public class PlanAttributes
{
    /// <summary>
    /// Gets or sets the plan title.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the service dates.
    /// </summary>
    [JsonPropertyName("dates")]
    public string? Dates { get; set; }

    /// <summary>
    /// Gets or sets the abbreviated date display.
    /// </summary>
    [JsonPropertyName("short_dates")]
    public string? ShortDates { get; set; }

    /// <summary>
    /// Gets or sets the Planning Center URL.
    /// </summary>
    [JsonPropertyName("planning_center_url")]
    public string? PlanningCenterUrl { get; set; }

    /// <summary>
    /// Gets or sets the sort date.
    /// </summary>
    [JsonPropertyName("sort_date")]
    public DateTime? SortDate { get; set; }

    /// <summary>
    /// Gets or sets whether the plan is public.
    /// </summary>
    [JsonPropertyName("public")]
    public bool IsPublic { get; set; }

    /// <summary>
    /// Gets or sets the rehearsal times.
    /// </summary>
    [JsonPropertyName("rehearsal_times")]
    public string? RehearsalTimes { get; set; }

    /// <summary>
    /// Gets or sets the service times.
    /// </summary>
    [JsonPropertyName("service_times")]
    public string? ServiceTimes { get; set; }

    /// <summary>
    /// Gets or sets other times.
    /// </summary>
    [JsonPropertyName("other_times")]
    public string? OtherTimes { get; set; }

    /// <summary>
    /// Gets or sets the service length.
    /// </summary>
    [JsonPropertyName("length")]
    public int? Length { get; set; }

    /// <summary>
    /// Gets or sets the plan notes.
    /// </summary>
    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

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
/// Plan relationships for JSON:API.
/// </summary>
public class PlanRelationships
{
    /// <summary>
    /// Gets or sets the service type relationship.
    /// </summary>
    [JsonPropertyName("service_type")]
    public RelationshipData? ServiceType { get; set; }

    /// <summary>
    /// Gets or sets the items relationship.
    /// </summary>
    [JsonPropertyName("items")]
    public RelationshipDataCollection? Items { get; set; }
}