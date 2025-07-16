using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Registrations;

/// <summary>
/// JSON API DTO for Attendee entity.
/// </summary>
public class AttendeeDto
{
    public string Type { get; set; } = "Attendee";
    public string Id { get; set; } = string.Empty;
    public AttendeeAttributesDto Attributes { get; set; } = new();
    public AttendeeRelationshipsDto? Relationships { get; set; }
}

/// <summary>
/// Attributes for Attendee JSON API DTO.
/// </summary>
public class AttendeeAttributesDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? Birthdate { get; set; }
    public string? Gender { get; set; }
    public string? Grade { get; set; }
    public string? School { get; set; }
    public string Status { get; set; } = "confirmed";
    public bool OnWaitlist { get; set; }
    public int? WaitlistPosition { get; set; }
    public bool CheckedIn { get; set; }
    public DateTime? CheckedInAt { get; set; }
    public string? MedicalNotes { get; set; }
    public string? DietaryRestrictions { get; set; }
    public string? SpecialNeeds { get; set; }
    public string? Notes { get; set; }
    public Dictionary<string, object> CustomFields { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Relationships for Attendee JSON API DTO.
/// </summary>
public class AttendeeRelationshipsDto
{
    public RelationshipData? Signup { get; set; }
    public RelationshipData? Registration { get; set; }
    public RelationshipData? Person { get; set; }
    public RelationshipData? EmergencyContact { get; set; }
}

/// <summary>
/// Create DTO for Attendee.
/// </summary>
public class AttendeeCreateDto
{
    public string Type { get; set; } = "Attendee";
    public AttendeeCreateAttributesDto Attributes { get; set; } = new();
    public AttendeeCreateRelationshipsDto? Relationships { get; set; }
}

/// <summary>
/// Create attributes for Attendee.
/// </summary>
public class AttendeeCreateAttributesDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? Birthdate { get; set; }
    public string? Gender { get; set; }
    public string? Grade { get; set; }
    public string? School { get; set; }
    public string? MedicalNotes { get; set; }
    public string? DietaryRestrictions { get; set; }
    public string? SpecialNeeds { get; set; }
    public string? Notes { get; set; }
    public Dictionary<string, object> CustomFields { get; set; } = new();
}

/// <summary>
/// Create relationships for Attendee.
/// </summary>
public class AttendeeCreateRelationshipsDto
{
    public RelationshipData? Signup { get; set; }
    public RelationshipData? Registration { get; set; }
    public RelationshipData? Person { get; set; }
}

/// <summary>
/// Update DTO for Attendee.
/// </summary>
public class AttendeeUpdateDto
{
    public string Type { get; set; } = "Attendee";
    public string Id { get; set; } = string.Empty;
    public AttendeeUpdateAttributesDto Attributes { get; set; } = new();
}

/// <summary>
/// Update attributes for Attendee.
/// </summary>
public class AttendeeUpdateAttributesDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? MedicalNotes { get; set; }
    public string? DietaryRestrictions { get; set; }
    public string? SpecialNeeds { get; set; }
    public string? Notes { get; set; }
    public string? Status { get; set; }
    public Dictionary<string, object>? CustomFields { get; set; }
}