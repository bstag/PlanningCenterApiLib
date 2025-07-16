using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.Registrations;

/// <summary>
/// Represents a person registered for a signup in Planning Center Registrations.
/// </summary>
public class Attendee : PlanningCenterResource
{
    /// <summary>
    /// Gets or sets the associated signup ID.
    /// </summary>
    public string SignupId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the associated registration ID.
    /// </summary>
    public string? RegistrationId { get; set; }
    
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
    /// Gets or sets the attendee's full name.
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();
    
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
    /// Gets or sets the attendee status.
    /// </summary>
    public string Status { get; set; } = "confirmed";
    
    /// <summary>
    /// Gets or sets whether the attendee is on the waitlist.
    /// </summary>
    public bool OnWaitlist { get; set; }
    
    /// <summary>
    /// Gets or sets the waitlist position (if on waitlist).
    /// </summary>
    public int? WaitlistPosition { get; set; }
    
    /// <summary>
    /// Gets or sets whether the attendee has checked in.
    /// </summary>
    public bool CheckedIn { get; set; }
    
    /// <summary>
    /// Gets or sets when the attendee checked in.
    /// </summary>
    public DateTime? CheckedInAt { get; set; }
    
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
    /// Gets or sets the emergency contact ID.
    /// </summary>
    public string? EmergencyContactId { get; set; }
    
    /// <summary>
    /// Gets or sets custom field data.
    /// </summary>
    public Dictionary<string, object> CustomFields { get; set; } = new();
}