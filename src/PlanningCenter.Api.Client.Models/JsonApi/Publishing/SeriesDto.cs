using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Publishing;

/// <summary>
/// JSON API DTO for Series entity.
/// </summary>
public class SeriesDto
{
    public string Type { get; set; } = "Series";
    public string Id { get; set; } = string.Empty;
    public SeriesAttributesDto Attributes { get; set; } = new();
}

/// <summary>
/// Attributes for Series JSON API DTO.
/// </summary>
public class SeriesAttributesDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Summary { get; set; }
    public string? ArtworkUrl { get; set; }
    public long? ArtworkFileSizeInBytes { get; set; }
    public string? ArtworkContentType { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsPublished { get; set; }
    public string Status { get; set; } = "draft";
    public string? SeriesType { get; set; }
    public string? Language { get; set; } = "en";
    public List<string> Tags { get; set; } = new();
    public List<string> Categories { get; set; } = new();
    public int EpisodeCount { get; set; }
    public long TotalViewCount { get; set; }
    public long TotalDownloadCount { get; set; }
    public long SubscriberCount { get; set; }
    public string? Author { get; set; }
    public string? Copyright { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? RssFeedUrl { get; set; }
    public bool IsExplicit { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Create DTO for Series.
/// </summary>
public class SeriesCreateDto
{
    public string Type { get; set; } = "Series";
    public SeriesCreateAttributesDto Attributes { get; set; } = new();
}

/// <summary>
/// Create attributes for Series.
/// </summary>
public class SeriesCreateAttributesDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Summary { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? SeriesType { get; set; }
    public string? Language { get; set; } = "en";
    public List<string> Tags { get; set; } = new();
    public List<string> Categories { get; set; } = new();
    public string? Author { get; set; }
    public string? Copyright { get; set; }
    public string? WebsiteUrl { get; set; }
    public bool IsExplicit { get; set; }
}

/// <summary>
/// Update DTO for Series.
/// </summary>
public class SeriesUpdateDto
{
    public string Type { get; set; } = "Series";
    public string Id { get; set; } = string.Empty;
    public SeriesUpdateAttributesDto Attributes { get; set; } = new();
}

/// <summary>
/// Update attributes for Series.
/// </summary>
public class SeriesUpdateAttributesDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Summary { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? SeriesType { get; set; }
    public string? Language { get; set; }
    public List<string>? Tags { get; set; }
    public List<string>? Categories { get; set; }
    public string? Author { get; set; }
    public string? Copyright { get; set; }
    public string? WebsiteUrl { get; set; }
    public bool? IsExplicit { get; set; }
    public bool? IsPublished { get; set; }
}