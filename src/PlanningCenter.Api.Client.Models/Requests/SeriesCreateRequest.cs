namespace PlanningCenter.Api.Client.Models.Requests;

/// <summary>
/// Request model for creating a series.
/// </summary>
public class SeriesCreateRequest
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
    /// Gets or sets the series type (e.g., "sermon", "teaching", "podcast").
    /// </summary>
    public string? SeriesType { get; set; }
    
    /// <summary>
    /// Gets or sets the series language.
    /// </summary>
    public string Language { get; set; } = "en";
    
    /// <summary>
    /// Gets or sets the series tags.
    /// </summary>
    public List<string> Tags { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the series categories.
    /// </summary>
    public List<string> Categories { get; set; } = new();
    
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
    /// Gets or sets whether the series is explicit content.
    /// </summary>
    public bool IsExplicit { get; set; }
}

/// <summary>
/// Request model for updating a series.
/// </summary>
public class SeriesUpdateRequest
{
    /// <summary>
    /// Gets or sets the series title.
    /// </summary>
    public string? Title { get; set; }
    
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
    /// Gets or sets the series type (e.g., "sermon", "teaching", "podcast").
    /// </summary>
    public string? SeriesType { get; set; }
    
    /// <summary>
    /// Gets or sets the series language.
    /// </summary>
    public string? Language { get; set; }
    
    /// <summary>
    /// Gets or sets the series tags.
    /// </summary>
    public List<string>? Tags { get; set; }
    
    /// <summary>
    /// Gets or sets the series categories.
    /// </summary>
    public List<string>? Categories { get; set; }
    
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
    /// Gets or sets whether the series is explicit content.
    /// </summary>
    public bool? IsExplicit { get; set; }
    
    /// <summary>
    /// Gets or sets whether the series is published.
    /// </summary>
    public bool? IsPublished { get; set; }
}