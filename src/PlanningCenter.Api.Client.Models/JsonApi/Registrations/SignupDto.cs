using System.Text.Json.Serialization;
using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Registrations;

/// <summary>
/// JSON API DTO for Signup entity.
/// </summary>
public class SignupDto
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "Signup";
    
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("attributes")]
    public SignupAttributesDto Attributes { get; set; } = new();
    
    [JsonPropertyName("relationships")]
    public SignupRelationshipsDto? Relationships { get; set; }
}

/// <summary>
/// Attributes for Signup JSON API DTO.
/// </summary>
public class SignupAttributesDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    [JsonPropertyName("archived")]
    public bool Archived { get; set; }
    
    [JsonPropertyName("open_at")]
    public DateTime? OpenAt { get; set; }
    
    [JsonPropertyName("close_at")]
    public DateTime? CloseAt { get; set; }
    
    [JsonPropertyName("logo_url")]
    public string? LogoUrl { get; set; }
    
    [JsonPropertyName("new_registration_url")]
    public string? NewRegistrationUrl { get; set; }
    
    [JsonPropertyName("registration_limit")]
    public int? RegistrationLimit { get; set; }
    
    [JsonPropertyName("waitlist_enabled")]
    public bool WaitlistEnabled { get; set; }
    
    [JsonPropertyName("registration_count")]
    public int RegistrationCount { get; set; }
    
    [JsonPropertyName("waitlist_count")]
    public int WaitlistCount { get; set; }
    
    [JsonPropertyName("requires_approval")]
    public bool RequiresApproval { get; set; }
    
    [JsonPropertyName("status")]
    public string Status { get; set; } = "active";
    
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Relationships for Signup JSON API DTO.
/// </summary>
public class SignupRelationshipsDto
{
    [JsonPropertyName("category")]
    public RelationshipData? Category { get; set; }
    
    [JsonPropertyName("campus")]
    public RelationshipData? Campus { get; set; }
    
    [JsonPropertyName("signup_location")]
    public RelationshipData? SignupLocation { get; set; }
}

/// <summary>
/// Create DTO for Signup.
/// </summary>
public class SignupCreateDto
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "Signup";
    
    [JsonPropertyName("attributes")]
    public SignupCreateAttributesDto Attributes { get; set; } = new();
    
    [JsonPropertyName("relationships")]
    public SignupCreateRelationshipsDto? Relationships { get; set; }
}

/// <summary>
/// Create attributes for Signup.
/// </summary>
public class SignupCreateAttributesDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    [JsonPropertyName("open_at")]
    public DateTime? OpenAt { get; set; }
    
    [JsonPropertyName("close_at")]
    public DateTime? CloseAt { get; set; }
    
    [JsonPropertyName("logo_url")]
    public string? LogoUrl { get; set; }
    
    [JsonPropertyName("registration_limit")]
    public int? RegistrationLimit { get; set; }
    
    [JsonPropertyName("waitlist_enabled")]
    public bool WaitlistEnabled { get; set; }
    
    [JsonPropertyName("requires_approval")]
    public bool RequiresApproval { get; set; }
}

/// <summary>
/// Create relationships for Signup.
/// </summary>
public class SignupCreateRelationshipsDto
{
    [JsonPropertyName("category")]
    public RelationshipData? Category { get; set; }
    
    [JsonPropertyName("campus")]
    public RelationshipData? Campus { get; set; }
}

/// <summary>
/// Update DTO for Signup.
/// </summary>
public class SignupUpdateDto
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "Signup";
    
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("attributes")]
    public SignupUpdateAttributesDto Attributes { get; set; } = new();
}

/// <summary>
/// Update attributes for Signup.
/// </summary>
public class SignupUpdateAttributesDto
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    [JsonPropertyName("open_at")]
    public DateTime? OpenAt { get; set; }
    
    [JsonPropertyName("close_at")]
    public DateTime? CloseAt { get; set; }
    
    [JsonPropertyName("logo_url")]
    public string? LogoUrl { get; set; }
    
    [JsonPropertyName("registration_limit")]
    public int? RegistrationLimit { get; set; }
    
    [JsonPropertyName("waitlist_enabled")]
    public bool? WaitlistEnabled { get; set; }
    
    [JsonPropertyName("requires_approval")]
    public bool? RequiresApproval { get; set; }
    
    [JsonPropertyName("archived")]
    public bool? Archived { get; set; }
}