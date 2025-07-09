namespace PlanningCenter.Api.Client.Models.Core;

/// <summary>
/// Represents a campus or location in the Planning Center system.
/// </summary>
public class Campus
{
    /// <summary>
    /// Unique identifier for the campus
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Campus name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Campus description
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Physical address of the campus
    /// </summary>
    public Address? Address { get; set; }
    
    /// <summary>
    /// Campus phone number
    /// </summary>
    public string? PhoneNumber { get; set; }
    
    /// <summary>
    /// Campus website URL
    /// </summary>
    public string? Website { get; set; }
    
    /// <summary>
    /// Campus email address
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// Time zone for the campus
    /// </summary>
    public string? TimeZone { get; set; }
    
    /// <summary>
    /// Whether this campus is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// When the campus was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// When the campus was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
    
    /// <summary>
    /// Additional campus metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}