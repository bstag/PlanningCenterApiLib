using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.Registrations;

/// <summary>
/// Represents a campus in Planning Center Registrations.
/// </summary>
public class Campus : PlanningCenterResource
{
    /// <summary>
    /// Gets or sets the campus name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the campus description.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the campus timezone.
    /// </summary>
    public string? Timezone { get; set; }
    
    /// <summary>
    /// Gets or sets the campus address.
    /// </summary>
    public string? Address { get; set; }
    
    /// <summary>
    /// Gets or sets the campus city.
    /// </summary>
    public string? City { get; set; }
    
    /// <summary>
    /// Gets or sets the campus state.
    /// </summary>
    public string? State { get; set; }
    
    /// <summary>
    /// Gets or sets the campus postal code.
    /// </summary>
    public string? PostalCode { get; set; }
    
    /// <summary>
    /// Gets or sets the campus country.
    /// </summary>
    public string? Country { get; set; }
    
    /// <summary>
    /// Gets or sets the campus phone number.
    /// </summary>
    public string? PhoneNumber { get; set; }
    
    /// <summary>
    /// Gets or sets the campus website URL.
    /// </summary>
    public string? WebsiteUrl { get; set; }
    
    /// <summary>
    /// Gets or sets whether this campus is active.
    /// </summary>
    public bool Active { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the display order.
    /// </summary>
    public int SortOrder { get; set; }
    
    /// <summary>
    /// Gets or sets the number of signups for this campus.
    /// </summary>
    public int SignupCount { get; set; }
}