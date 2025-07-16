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