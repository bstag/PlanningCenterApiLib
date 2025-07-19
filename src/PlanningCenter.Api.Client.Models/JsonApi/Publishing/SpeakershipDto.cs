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
    public SpeakershipRelationshipsDto? Relationships { get; set; }
}

/// <summary>
/// Attributes for Speakership JSON API DTO.
/// </summary>
public class SpeakershipAttributesDto
{
    public string? Role { get; set; }
    public string? Notes { get; set; }
    public int? Order { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Relationships for Speakership JSON API DTO.
/// </summary>
public class SpeakershipRelationshipsDto
{
    public RelationshipData? Episode { get; set; }
    public RelationshipData? Speaker { get; set; }
}

/// <summary>
/// Create DTO for Speakership.
/// </summary>
public class SpeakershipCreateDto
{
    public string Type { get; set; } = "Speakership";
    public SpeakershipCreateAttributesDto Attributes { get; set; } = new();
    public SpeakershipCreateRelationshipsDto Relationships { get; set; } = new();
}

/// <summary>
/// Create attributes for Speakership.
/// </summary>
public class SpeakershipCreateAttributesDto
{
    public string? Role { get; set; }
    public string? Notes { get; set; }
    public int? Order { get; set; }
    public bool IsPrimary { get; set; }
}

/// <summary>
/// Create relationships for Speakership.
/// </summary>
public class SpeakershipCreateRelationshipsDto
{
    public RelationshipData Episode { get; set; } = new();
    public RelationshipData Speaker { get; set; } = new();
}