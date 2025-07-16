namespace PlanningCenter.Api.Client.Models.Requests;

/// <summary>
/// Request model for creating an episode.
/// </summary>
public class EpisodeCreateRequest
{
    /// <summary>
    /// Gets or sets the episode title.
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the episode description.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the associated series ID.
    /// </summary>
    public string? SeriesId { get; set; }
    
    /// <summary>
    /// Gets or sets when the episode was published.
    /// </summary>
    public DateTime? PublishedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the episode duration in seconds.
    /// </summary>
    public int? LengthInSeconds { get; set; }
    
    /// <summary>
    /// Gets or sets the episode notes or transcript.
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Gets or sets the episode number within the series.
    /// </summary>
    public int? EpisodeNumber { get; set; }
    
    /// <summary>
    /// Gets or sets the season number.
    /// </summary>
    public int? SeasonNumber { get; set; }
    
    /// <summary>
    /// Gets or sets the episode tags.
    /// </summary>
    public List<string> Tags { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the episode categories.
    /// </summary>
    public List<string> Categories { get; set; } = new();
}

/// <summary>
/// Request model for updating an episode.
/// </summary>
public class EpisodeUpdateRequest
{
    /// <summary>
    /// Gets or sets the episode title.
    /// </summary>
    public string? Title { get; set; }
    
    /// <summary>
    /// Gets or sets the episode description.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets when the episode was published.
    /// </summary>
    public DateTime? PublishedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the episode duration in seconds.
    /// </summary>
    public int? LengthInSeconds { get; set; }
    
    /// <summary>
    /// Gets or sets the episode notes or transcript.
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Gets or sets the episode number within the series.
    /// </summary>
    public int? EpisodeNumber { get; set; }
    
    /// <summary>
    /// Gets or sets the season number.
    /// </summary>
    public int? SeasonNumber { get; set; }
    
    /// <summary>
    /// Gets or sets the episode tags.
    /// </summary>
    public List<string>? Tags { get; set; }
    
    /// <summary>
    /// Gets or sets the episode categories.
    /// </summary>
    public List<string>? Categories { get; set; }
    
    /// <summary>
    /// Gets or sets whether the episode is published.
    /// </summary>
    public bool? IsPublished { get; set; }
}