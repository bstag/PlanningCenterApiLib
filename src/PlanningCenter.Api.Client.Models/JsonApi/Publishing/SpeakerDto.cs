using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Publishing;

/// <summary>
/// JSON API DTO for Speaker entity.
/// </summary>
public class SpeakerDto
{
    public string Type { get; set; } = "Speaker";
    public string Id { get; set; } = string.Empty;
    public SpeakerAttributesDto Attributes { get; set; } = new();
}

/// <summary>
/// Attributes for Speaker JSON API DTO.
/// </summary>
public class SpeakerAttributesDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? Title { get; set; }
    public string? Biography { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Organization { get; set; }
    public string? Location { get; set; }
    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}