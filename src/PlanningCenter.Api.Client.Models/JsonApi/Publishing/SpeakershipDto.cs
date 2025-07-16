using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Publishing;

/// <summary>
/// JSON API DTO for Speakership entity.
/// </summary>
public class SpeakershipDto
{
    public string Type { get; set; } = "Speakership";
    public string Id { get; set; } = string.Empty;
    public SpeakershipAttributesDto Attributes { get; set; } = new();
}

/// <summary>
/// Attributes for Speakership JSON API DTO.
/// </summary>
public class SpeakershipAttributesDto
{
    public string? Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}