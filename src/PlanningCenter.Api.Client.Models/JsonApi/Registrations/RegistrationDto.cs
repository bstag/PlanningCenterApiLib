using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Registrations;

/// <summary>
/// JSON API DTO for Registration entity.
/// </summary>
public class RegistrationDto
{
    public string Type { get; set; } = "Registration";
    public string Id { get; set; } = string.Empty;
    public RegistrationAttributesDto Attributes { get; set; } = new();
    public RegistrationRelationshipsDto? Relationships { get; set; }
}

/// <summary>
/// Attributes for Registration JSON API DTO.
/// </summary>
public class RegistrationAttributesDto
{
    public string Status { get; set; } = "pending";
    public decimal? TotalCost { get; set; }
    public decimal? AmountPaid { get; set; }
    public string? PaymentStatus { get; set; }
    public string? PaymentMethod { get; set; }
    public string? ConfirmationCode { get; set; }
    public string? Notes { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int AttendeeCount { get; set; }
    public bool RequiresApproval { get; set; }
    public string? ApprovalStatus { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Relationships for Registration JSON API DTO.
/// </summary>
public class RegistrationRelationshipsDto
{
    public RelationshipData? Signup { get; set; }
}

/// <summary>
/// Create DTO for Registration.
/// </summary>
public class RegistrationCreateDto
{
    public string Type { get; set; } = "Registration";
    public RegistrationCreateAttributesDto Attributes { get; set; } = new();
    public RegistrationCreateRelationshipsDto? Relationships { get; set; }
}

/// <summary>
/// Create attributes for Registration.
/// </summary>
public class RegistrationCreateAttributesDto
{
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Create relationships for Registration.
/// </summary>
public class RegistrationCreateRelationshipsDto
{
    public RelationshipData? Signup { get; set; }
}