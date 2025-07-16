using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.Registrations;

/// <summary>
/// Represents an emergency contact for an attendee in Planning Center Registrations.
/// </summary>
public class EmergencyContact : PlanningCenterResource
{
    /// <summary>
    /// Gets or sets the associated attendee ID.
    /// </summary>
    public string AttendeeId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the emergency contact's first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the emergency contact's last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the emergency contact's full name.
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();
    
    /// <summary>
    /// Gets or sets the relationship to the attendee.
    /// </summary>
    public string? Relationship { get; set; }
    
    /// <summary>
    /// Gets or sets the primary phone number.
    /// </summary>
    public string? PrimaryPhone { get; set; }
    
    /// <summary>
    /// Gets or sets the secondary phone number.
    /// </summary>
    public string? SecondaryPhone { get; set; }
    
    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// Gets or sets the street address.
    /// </summary>
    public string? StreetAddress { get; set; }
    
    /// <summary>
    /// Gets or sets the city.
    /// </summary>
    public string? City { get; set; }
    
    /// <summary>
    /// Gets or sets the state or province.
    /// </summary>
    public string? State { get; set; }
    
    /// <summary>
    /// Gets or sets the postal code.
    /// </summary>
    public string? PostalCode { get; set; }
    
    /// <summary>
    /// Gets or sets the country.
    /// </summary>
    public string? Country { get; set; }
    
    /// <summary>
    /// Gets or sets whether this is the primary emergency contact.
    /// </summary>
    public bool IsPrimary { get; set; }
    
    /// <summary>
    /// Gets or sets the priority order for contacting.
    /// </summary>
    public int Priority { get; set; } = 1;
    
    /// <summary>
    /// Gets or sets additional notes about the emergency contact.
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Gets or sets the preferred contact method.
    /// </summary>
    public string? PreferredContactMethod { get; set; }
    
    /// <summary>
    /// Gets or sets the best time to contact.
    /// </summary>
    public string? BestTimeToContact { get; set; }
    
    /// <summary>
    /// Gets or sets whether this contact can authorize medical treatment.
    /// </summary>
    public bool CanAuthorizeMedicalTreatment { get; set; }
    
    /// <summary>
    /// Gets or sets additional metadata.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}