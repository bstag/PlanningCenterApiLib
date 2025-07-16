namespace PlanningCenter.Api.Client.Models.Requests;

/// <summary>
/// Request model for creating a registration.
/// </summary>
public class RegistrationCreateRequest
{
    /// <summary>
    /// Gets or sets the registrant's email address.
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// Gets or sets the registrant's first name.
    /// </summary>
    public string? FirstName { get; set; }
    
    /// <summary>
    /// Gets or sets the registrant's last name.
    /// </summary>
    public string? LastName { get; set; }
    
    /// <summary>
    /// Gets or sets the registrant's phone number.
    /// </summary>
    public string? PhoneNumber { get; set; }
    
    /// <summary>
    /// Gets or sets the registration notes.
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Gets or sets the attendees for this registration.
    /// </summary>
    public List<AttendeeCreateRequest> Attendees { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the selected options for this registration.
    /// </summary>
    public Dictionary<string, object> Selections { get; set; } = new();
}