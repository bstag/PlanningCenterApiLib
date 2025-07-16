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
    public string? ArtworkUrl { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}