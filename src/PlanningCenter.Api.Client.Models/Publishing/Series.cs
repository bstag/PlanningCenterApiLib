using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.Publishing;

/// <summary>
/// Represents a series or collection of episodes in Planning Center Publishing.
/// </summary>
public class Series : PlanningCenterResource
{
    /// <summary>
    /// Gets or sets the series title.
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the series description.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the series summary.
    /// </summary>
    public string? Summary { get; set; }
    
    /// <summary>
    /// Gets or sets when the series was published.
    /// </summary>
    public DateTime? PublishedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the series start date.
    /// </summary>
    public DateTime? StartDate { get; set; }
    
    /// <summary>
    /// Gets or sets the series end date.
    /// </summary>
    public DateTime? EndDate { get; set; }
    
    /// <summary>
    /// Gets or sets the series artwork URL.
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
    /// Gets or sets whether the series is published.
    /// </summary>
    public bool IsPublished { get; set; }
    
    /// <summary>
    /// Gets or sets the series status.
    /// </summary>
    public string Status { get; set; } = "draft";
    
    /// <summary>
    /// Gets or sets the series type (e.g., "sermon", "teaching", "podcast").
    /// </summary>
    public string? SeriesType { get; set; }
    
    /// <summary>
    /// Gets or sets the series language.
    /// </summary>
    public string? Language { get; set; } = "en";
    
    /// <summary>
    /// Gets or sets the series tags.
    /// </summary>
    public List<string> Tags { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the series categories.
    /// </summary>
    public List<string> Categories { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the number of episodes in the series.
    /// </summary>
    public int EpisodeCount { get; set; }
    
    /// <summary>
    /// Gets or sets the total view count across all episodes.
    /// </summary>
    public long TotalViewCount { get; set; }
    
    /// <summary>
    /// Gets or sets the total download count across all episodes.
    /// </summary>
    public long TotalDownloadCount { get; set; }
    
    /// <summary>
    /// Gets or sets the subscriber count.
    /// </summary>
    public long SubscriberCount { get; set; }
    
    /// <summary>
    /// Gets or sets the series author or creator.
    /// </summary>
    public string? Author { get; set; }
    
    /// <summary>
    /// Gets or sets the series copyright information.
    /// </summary>
    public string? Copyright { get; set; }
    
    /// <summary>
    /// Gets or sets the series website URL.
    /// </summary>
    public string? WebsiteUrl { get; set; }
    
    /// <summary>
    /// Gets or sets the RSS feed URL.
    /// </summary>
    public string? RssFeedUrl { get; set; }
    
    /// <summary>
    /// Gets or sets whether the series is explicit content.
    /// </summary>
    public bool IsExplicit { get; set; }
    
    /// <summary>
    /// Gets or sets additional metadata.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}