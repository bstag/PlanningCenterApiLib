using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.Publishing;

/// <summary>
/// Represents an individual piece of content (sermon, teaching, etc.) in Planning Center Publishing.
/// </summary>
public class Episode : PlanningCenterResource
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
    /// Gets or sets when the episode was published.
    /// </summary>
    public DateTime? PublishedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the episode duration in seconds.
    /// </summary>
    public int? LengthInSeconds { get; set; }
    
    /// <summary>
    /// Gets or sets the video content URL.
    /// </summary>
    public string? VideoUrl { get; set; }
    
    /// <summary>
    /// Gets or sets the audio content URL.
    /// </summary>
    public string? AudioUrl { get; set; }
    
    /// <summary>
    /// Gets or sets the video download URL.
    /// </summary>
    public string? VideoDownloadUrl { get; set; }
    
    /// <summary>
    /// Gets or sets the audio download URL.
    /// </summary>
    public string? AudioDownloadUrl { get; set; }
    
    /// <summary>
    /// Gets or sets the video file size in bytes.
    /// </summary>
    public long? VideoFileSizeInBytes { get; set; }
    
    /// <summary>
    /// Gets or sets the audio file size in bytes.
    /// </summary>
    public long? AudioFileSizeInBytes { get; set; }
    
    /// <summary>
    /// Gets or sets the video content type (MIME type).
    /// </summary>
    public string? VideoContentType { get; set; }
    
    /// <summary>
    /// Gets or sets the audio content type (MIME type).
    /// </summary>
    public string? AudioContentType { get; set; }
    
    /// <summary>
    /// Gets or sets the episode artwork URL.
    /// </summary>
    public string? ArtworkUrl { get; set; }
    
    /// <summary>
    /// Gets or sets the artwork file size in bytes.
    /// </summary>
    public long? ArtworkFileSizeInBytes { get; set; }
    
    /// <summary>
    /// Gets or sets the artwork content type (MIME type).
    /// </summary>
    public string? ArtworkContentType { get; set; }
    
    /// <summary>
    /// Gets or sets the episode notes or transcript.
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Gets or sets the associated series ID.
    /// </summary>
    public string? SeriesId { get; set; }
    
    /// <summary>
    /// Gets or sets whether the episode is published.
    /// </summary>
    public bool IsPublished { get; set; }
    
    /// <summary>
    /// Gets or sets the episode status.
    /// </summary>
    public string Status { get; set; } = "draft";
    
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
    
    /// <summary>
    /// Gets or sets the view count.
    /// </summary>
    public long ViewCount { get; set; }
    
    /// <summary>
    /// Gets or sets the download count.
    /// </summary>
    public long DownloadCount { get; set; }
    
    /// <summary>
    /// Gets or sets the like count.
    /// </summary>
    public long LikeCount { get; set; }
    
    /// <summary>
    /// Gets or sets additional metadata.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}