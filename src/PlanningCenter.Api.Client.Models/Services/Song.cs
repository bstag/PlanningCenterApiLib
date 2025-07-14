using System;
using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.Services;

/// <summary>
/// Represents a song in Planning Center Services.
/// Follows Single Responsibility Principle - handles only song data.
/// </summary>
public class Song : PlanningCenterResource
{
    /// <summary>
    /// Gets or sets the data source for the song.
    /// </summary>
    public string DataSource { get; set; } = "Services";

    /// <summary>
    /// Gets or sets the song title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the song author.
    /// </summary>
    public string? Author { get; set; }

    /// <summary>
    /// Gets or sets the copyright information.
    /// </summary>
    public string? Copyright { get; set; }

    /// <summary>
    /// Gets or sets the CCLI number.
    /// </summary>
    public string? CcliNumber { get; set; }

    /// <summary>
    /// Gets or sets whether the song is hidden.
    /// </summary>
    public bool Hidden { get; set; }

    /// <summary>
    /// Gets or sets the notes about the song.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the themes associated with the song.
    /// </summary>
    public string? Themes { get; set; }

    /// <summary>
    /// Gets or sets the last scheduled date.
    /// </summary>
    public DateTime? LastScheduledAt { get; set; }

    /// <summary>
    /// Gets or sets the creation date.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update date.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}