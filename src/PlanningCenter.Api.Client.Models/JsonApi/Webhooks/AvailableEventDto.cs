using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Webhooks;

/// <summary>
/// JSON API DTO for AvailableEvent entity.
/// </summary>
public class AvailableEventDto
{
    public string Type { get; set; } = "AvailableEvent";
    public string Id { get; set; } = string.Empty;
    public AvailableEventAttributesDto Attributes { get; set; } = new();
}

/// <summary>
/// Attributes for AvailableEvent JSON API DTO.
/// </summary>
public class AvailableEventAttributesDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Module { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}