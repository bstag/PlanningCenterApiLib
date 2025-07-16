using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Registrations;

/// <summary>
/// JSON API DTO for Signup entity.
/// </summary>
public class SignupDto
{
    public string Type { get; set; } = "Signup";
    public string Id { get; set; } = string.Empty;
    public SignupAttributesDto Attributes { get; set; } = new();
    public SignupRelationshipsDto? Relationships { get; set; }
}

/// <summary>
/// Attributes for Signup JSON API DTO.
/// </summary>
public class SignupAttributesDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool Archived { get; set; }
    public DateTime? OpenAt { get; set; }
    public DateTime? CloseAt { get; set; }
    public string? LogoUrl { get; set; }
    public string? NewRegistrationUrl { get; set; }
    public int? RegistrationLimit { get; set; }
    public bool WaitlistEnabled { get; set; }
    public int RegistrationCount { get; set; }
    public int WaitlistCount { get; set; }
    public bool RequiresApproval { get; set; }
    public string Status { get; set; } = "active";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Relationships for Signup JSON API DTO.
/// </summary>
public class SignupRelationshipsDto
{
    public RelationshipData? Category { get; set; }
    public RelationshipData? Campus { get; set; }
    public RelationshipData? SignupLocation { get; set; }
}

/// <summary>
/// Create DTO for Signup.
/// </summary>
public class SignupCreateDto
{
    public string Type { get; set; } = "Signup";
    public SignupCreateAttributesDto Attributes { get; set; } = new();
    public SignupCreateRelationshipsDto? Relationships { get; set; }
}

/// <summary>
/// Create attributes for Signup.
/// </summary>
public class SignupCreateAttributesDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? OpenAt { get; set; }
    public DateTime? CloseAt { get; set; }
    public string? LogoUrl { get; set; }
    public int? RegistrationLimit { get; set; }
    public bool WaitlistEnabled { get; set; }
    public bool RequiresApproval { get; set; }
}

/// <summary>
/// Create relationships for Signup.
/// </summary>
public class SignupCreateRelationshipsDto
{
    public RelationshipData? Category { get; set; }
    public RelationshipData? Campus { get; set; }
}

/// <summary>
/// Update DTO for Signup.
/// </summary>
public class SignupUpdateDto
{
    public string Type { get; set; } = "Signup";
    public string Id { get; set; } = string.Empty;
    public SignupUpdateAttributesDto Attributes { get; set; } = new();
}

/// <summary>
/// Update attributes for Signup.
/// </summary>
public class SignupUpdateAttributesDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime? OpenAt { get; set; }
    public DateTime? CloseAt { get; set; }
    public string? LogoUrl { get; set; }
    public int? RegistrationLimit { get; set; }
    public bool? WaitlistEnabled { get; set; }
    public bool? RequiresApproval { get; set; }
    public bool? Archived { get; set; }
}