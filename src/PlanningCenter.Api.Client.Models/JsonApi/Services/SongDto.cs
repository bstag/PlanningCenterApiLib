using System;
using System.Text.Json.Serialization;

namespace PlanningCenter.Api.Client.Models.JsonApi.Services;

/// <summary>
/// JSON:API DTO for Planning Center Services Song.
/// Follows Interface Segregation Principle - focused on JSON:API serialization.
/// </summary>
public class SongDto
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
    public string Type { get; set; } = "Song";

    /// <summary>
    /// Gets or sets the song attributes.
    /// </summary>
    [JsonPropertyName("attributes")]
    public SongAttributes Attributes { get; set; } = new();
}

/// <summary>
/// Song attributes for JSON:API.
/// </summary>
public class SongAttributes
{
    /// <summary>
    /// Gets or sets the song title.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the song author.
    /// </summary>
    [JsonPropertyName("author")]
    public string? Author { get; set; }

    /// <summary>
    /// Gets or sets the copyright information.
    /// </summary>
    [JsonPropertyName("copyright")]
    public string? Copyright { get; set; }

    /// <summary>
    /// Gets or sets the CCLI number.
    /// </summary>
    [JsonPropertyName("ccli_number")]
    public string? CcliNumber { get; set; }

    /// <summary>
    /// Gets or sets whether the song is hidden.
    /// </summary>
    [JsonPropertyName("hidden")]
    public bool Hidden { get; set; }

    /// <summary>
    /// Gets or sets the notes about the song.
    /// </summary>
    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the themes associated with the song.
    /// </summary>
    [JsonPropertyName("themes")]
    public string? Themes { get; set; }

    /// <summary>
    /// Gets or sets the last scheduled date.
    /// </summary>
    [JsonPropertyName("last_scheduled_at")]
    public DateTime? LastScheduledAt { get; set; }

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