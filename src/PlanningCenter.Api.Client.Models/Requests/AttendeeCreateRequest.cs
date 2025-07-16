namespace PlanningCenter.Api.Client.Models.Requests;

/// <summary>
/// Request model for creating an attendee.
/// </summary>
public class AttendeeCreateRequest
{
    /// <summary>
    /// Gets or sets the associated person ID from People module.
    /// </summary>
    public string? PersonId { get; set; }
    
    /// <summary>
    /// Gets or sets the attendee's first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the attendee's last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the attendee's email address.
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// Gets or sets the attendee's phone number.
    /// </summary>
    public string? PhoneNumber { get; set; }
    
    /// <summary>
    /// Gets or sets the attendee's date of birth.
    /// </summary>
    public DateTime? Birthdate { get; set; }
    
    /// <summary>
    /// Gets or sets the attendee's gender.
    /// </summary>
    public string? Gender { get; set; }
    
    /// <summary>
    /// Gets or sets the attendee's grade (for students).
    /// </summary>
    public string? Grade { get; set; }
    
    /// <summary>
    /// Gets or sets the attendee's school.
    /// </summary>
    public string? School { get; set; }
    
    /// <summary>
    /// Gets or sets the attendee's medical notes.
    /// </summary>
    public string? MedicalNotes { get; set; }
    
    /// <summary>
    /// Gets or sets the attendee's dietary restrictions.
    /// </summary>
    public string? DietaryRestrictions { get; set; }
    
    /// <summary>
    /// Gets or sets special needs or accommodations.
    /// </summary>
    public string? SpecialNeeds { get; set; }
    
    /// <summary>
    /// Gets or sets additional notes about the attendee.
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Gets or sets custom field data.
    /// </summary>
    public Dictionary<string, object> CustomFields { get; set; } = new();
}

/// <summary>
/// Request model for updating an attendee.
/// </summary>
public class AttendeeUpdateRequest
{
    /// <summary>
    /// Gets or sets the attendee's first name.
    /// </summary>
    public string? FirstName { get; set; }
    
    /// <summary>
    /// Gets or sets the attendee's last name.
    /// </summary>
    public string? LastName { get; set; }
    
    /// <summary>
    /// Gets or sets the attendee's email address.
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// Gets or sets the attendee's phone number.
    /// </summary>
    public string? PhoneNumber { get; set; }
    
    /// <summary>
    /// Gets or sets the attendee's medical notes.
    /// </summary>
    public string? MedicalNotes { get; set; }
    
    /// <summary>
    /// Gets or sets the attendee's dietary restrictions.
    /// </summary>
    public string? DietaryRestrictions { get; set; }
    
    /// <summary>
    /// Gets or sets special needs or accommodations.
    /// </summary>
    public string? SpecialNeeds { get; set; }
    
    /// <summary>
    /// Gets or sets additional notes about the attendee.
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Gets or sets the attendee status.
    /// </summary>
    public string? Status { get; set; }
    
    /// <summary>
    /// Gets or sets custom field data.
    /// </summary>
    public Dictionary<string, object>? CustomFields { get; set; }
}