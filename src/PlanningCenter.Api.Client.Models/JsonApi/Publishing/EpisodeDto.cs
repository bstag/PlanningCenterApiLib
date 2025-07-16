using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Publishing;

/// <summary>
/// JSON API DTO for Episode entity.
/// </summary>
public class EpisodeDto
{
    public string Type { get; set; } = "Episode";
    public string Id { get; set; } = string.Empty;
    public EpisodeAttributesDto Attributes { get; set; } = new();
    public EpisodeRelationshipsDto? Relationships { get; set; }
}

/// <summary>
/// Attributes for Episode JSON API DTO.
/// </summary>
public class EpisodeAttributesDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? PublishedAt { get; set; }
    public int? LengthInSeconds { get; set; }
    public string? VideoUrl { get; set; }
    public string? AudioUrl { get; set; }
    public string? VideoDownloadUrl { get; set; }
    public string? AudioDownloadUrl { get; set; }
    public long? VideoFileSizeInBytes { get; set; }
    public long? AudioFileSizeInBytes { get; set; }
    public string? VideoContentType { get; set; }
    public string? AudioContentType { get; set; }
    public string? ArtworkUrl { get; set; }
    public long? ArtworkFileSizeInBytes { get; set; }
    public string? ArtworkContentType { get; set; }
    public string? Notes { get; set; }
    public bool IsPublished { get; set; }
    public string Status { get; set; } = "draft";
    public int? EpisodeNumber { get; set; }
    public int? SeasonNumber { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<string> Categories { get; set; } = new();
    public long ViewCount { get; set; }
    public long DownloadCount { get; set; }
    public long LikeCount { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Relationships for Episode JSON API DTO.
/// </summary>
public class EpisodeRelationshipsDto
{
    public RelationshipData? Series { get; set; }
}

/// <summary>
/// Create DTO for Episode.
/// </summary>
public class EpisodeCreateDto
{
    public string Type { get; set; } = "Episode";
    public EpisodeCreateAttributesDto Attributes { get; set; } = new();
    public EpisodeCreateRelationshipsDto? Relationships { get; set; }
}

/// <summary>
/// Create attributes for Episode.
/// </summary>
public class EpisodeCreateAttributesDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? PublishedAt { get; set; }
    public int? LengthInSeconds { get; set; }
    public string? Notes { get; set; }
    public int? EpisodeNumber { get; set; }
    public int? SeasonNumber { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<string> Categories { get; set; } = new();
}

/// <summary>
/// Create relationships for Episode.
/// </summary>
public class EpisodeCreateRelationshipsDto
{
    public RelationshipData? Series { get; set; }
}

/// <summary>
/// Update DTO for Episode.
/// </summary>
public class EpisodeUpdateDto
{
    public string Type { get; set; } = "Episode";
    public string Id { get; set; } = string.Empty;
    public EpisodeUpdateAttributesDto Attributes { get; set; } = new();
}

/// <summary>
/// Update attributes for Episode.
/// </summary>
public class EpisodeUpdateAttributesDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? PublishedAt { get; set; }
    public int? LengthInSeconds { get; set; }
    public string? Notes { get; set; }
    public int? EpisodeNumber { get; set; }
    public int? SeasonNumber { get; set; }
    public List<string>? Tags { get; set; }
    public List<string>? Categories { get; set; }
    public bool? IsPublished { get; set; }
}