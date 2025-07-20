using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Publishing;

/// <summary>
/// JSON API DTO for Media entity.
/// </summary>
public class MediaDto
{
    public string Type { get; set; } = "Media";
    public string Id { get; set; } = string.Empty;
    public MediaAttributesDto Attributes { get; set; } = new();
}

/// <summary>
/// Attributes for Media JSON API DTO.
/// </summary>
public class MediaAttributesDto
{
    public string FileName { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public long? FileSizeInBytes { get; set; }
    public string? MediaType { get; set; }
    public string? Quality { get; set; }
    public string? Url { get; set; }
    public string? DownloadUrl { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Create DTO for Media.
/// </summary>
public class MediaCreateDto
{
    public string Type { get; set; } = "Media";
    public MediaCreateAttributesDto Attributes { get; set; } = new();
    public MediaCreateRelationshipsDto? Relationships { get; set; }
}

/// <summary>
/// Create attributes for Media.
/// </summary>
public class MediaCreateAttributesDto
{
    public string FileName { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public long? FileSizeInBytes { get; set; }
    public string? MediaType { get; set; }
    public string? Quality { get; set; }
    public bool IsPrimary { get; set; }
}

/// <summary>
/// Create relationships for Media.
/// </summary>
public class MediaCreateRelationshipsDto
{
    public RelationshipData Episode { get; set; } = new();
}

/// <summary>
/// Update DTO for Media.
/// </summary>
public class MediaUpdateDto
{
    public string Type { get; set; } = "Media";
    public string Id { get; set; } = string.Empty;
    public MediaUpdateAttributesDto Attributes { get; set; } = new();
}

/// <summary>
/// Update attributes for Media.
/// </summary>
public class MediaUpdateAttributesDto
{
    public string? FileName { get; set; }
    public string? Quality { get; set; }
    public bool? IsPrimary { get; set; }
}