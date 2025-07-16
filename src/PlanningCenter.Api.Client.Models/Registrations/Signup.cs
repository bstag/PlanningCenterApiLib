using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.Registrations;

/// <summary>
/// Represents a registration event or opportunity in Planning Center Registrations.
/// </summary>
public class Signup : PlanningCenterResource
{
    /// <summary>
    /// Gets or sets the signup name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the signup description.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets whether the signup is archived.
    /// </summary>
    public bool Archived { get; set; }
    
    /// <summary>
    /// Gets or sets when registration opens.
    /// </summary>
    public DateTime? OpenAt { get; set; }
    
    /// <summary>
    /// Gets or sets when registration closes.
    /// </summary>
    public DateTime? CloseAt { get; set; }
    
    /// <summary>
    /// Gets or sets the signup logo/image URL.
    /// </summary>
    public string? LogoUrl { get; set; }
    
    /// <summary>
    /// Gets or sets the public registration URL.
    /// </summary>
    public string? NewRegistrationUrl { get; set; }
    
    /// <summary>
    /// Gets or sets the maximum number of registrations allowed.
    /// </summary>
    public int? RegistrationLimit { get; set; }
    
    /// <summary>
    /// Gets or sets whether waitlist is enabled.
    /// </summary>
    public bool WaitlistEnabled { get; set; }
    
    /// <summary>
    /// Gets or sets the signup category ID.
    /// </summary>
    public string? CategoryId { get; set; }
    
    /// <summary>
    /// Gets or sets the associated campus ID.
    /// </summary>
    public string? CampusId { get; set; }
    
    /// <summary>
    /// Gets or sets the signup location ID.
    /// </summary>
    public string? SignupLocationId { get; set; }
    
    /// <summary>
    /// Gets or sets the current registration count.
    /// </summary>
    public int RegistrationCount { get; set; }
    
    /// <summary>
    /// Gets or sets the current waitlist count.
    /// </summary>
    public int WaitlistCount { get; set; }
    
    /// <summary>
    /// Gets or sets whether the signup requires approval.
    /// </summary>
    public bool RequiresApproval { get; set; }
    
    /// <summary>
    /// Gets or sets the signup status.
    /// </summary>
    public string Status { get; set; } = "active";
}