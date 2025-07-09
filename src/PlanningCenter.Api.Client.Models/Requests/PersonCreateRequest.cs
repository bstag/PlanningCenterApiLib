namespace PlanningCenter.Api.Client.Models.Requests;

/// <summary>
/// Request model for creating a new person.
/// </summary>
public class PersonCreateRequest
{
    /// <summary>
    /// Person's first name (required)
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Person's last name (required)
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Person's middle name
    /// </summary>
    public string? MiddleName { get; set; }
    
    /// <summary>
    /// Person's nickname or preferred name
    /// </summary>
    public string? Nickname { get; set; }
    
    /// <summary>
    /// Primary email address
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// Person's date of birth
    /// </summary>
    public DateTime? Birthdate { get; set; }
    
    /// <summary>
    /// Person's gender
    /// </summary>
    public string? Gender { get; set; }
    
    /// <summary>
    /// Person's marital status
    /// </summary>
    public string? MaritalStatus { get; set; }
    
    /// <summary>
    /// Anniversary date
    /// </summary>
    public DateTime? Anniversary { get; set; }
    
    /// <summary>
    /// Person's status (defaults to "active")
    /// </summary>
    public string Status { get; set; } = "active";
    
    /// <summary>
    /// ID of the person's primary campus
    /// </summary>
    public string? PrimaryCampusId { get; set; }
    
    /// <summary>
    /// Person's membership status
    /// </summary>
    public string? MembershipStatus { get; set; }
    
    /// <summary>
    /// School information
    /// </summary>
    public string? School { get; set; }
    
    /// <summary>
    /// Grade level
    /// </summary>
    public string? Grade { get; set; }
    
    /// <summary>
    /// Graduation year
    /// </summary>
    public int? GraduationYear { get; set; }
    
    /// <summary>
    /// Medical notes or allergies
    /// </summary>
    public string? MedicalNotes { get; set; }
    
    /// <summary>
    /// Emergency contact name
    /// </summary>
    public string? EmergencyContactName { get; set; }
    
    /// <summary>
    /// Emergency contact phone number
    /// </summary>
    public string? EmergencyContactPhone { get; set; }
    
    /// <summary>
    /// Custom fields for the person
    /// </summary>
    public Dictionary<string, object> CustomFields { get; set; } = new();
}