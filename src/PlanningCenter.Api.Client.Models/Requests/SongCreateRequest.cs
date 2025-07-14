namespace PlanningCenter.Api.Client.Models.Requests;

/// <summary>
/// Request model for creating a song.
/// Follows Single Responsibility Principle - handles only song creation data.
/// </summary>
public class SongCreateRequest
{
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
    public bool Hidden { get; set; } = false;

    /// <summary>
    /// Gets or sets the notes about the song.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the themes associated with the song.
    /// </summary>
    public string? Themes { get; set; }
}

/// <summary>
/// Request model for updating a song.
/// </summary>
public class SongUpdateRequest
{
    /// <summary>
    /// Gets or sets the song title.
    /// </summary>
    public string? Title { get; set; }

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
    public bool? Hidden { get; set; }

    /// <summary>
    /// Gets or sets the notes about the song.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the themes associated with the song.
    /// </summary>
    public string? Themes { get; set; }
}