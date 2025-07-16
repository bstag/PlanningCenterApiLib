using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.Registrations;

/// <summary>
/// Represents a time slot for a signup in Planning Center Registrations.
/// </summary>
public class SignupTime : PlanningCenterResource
{
    /// <summary>
    /// Gets or sets the associated signup ID.
    /// </summary>
    public string SignupId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the time slot name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the time slot description.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the start date and time.
    /// </summary>
    public DateTime StartTime { get; set; }
    
    /// <summary>
    /// Gets or sets the end date and time.
    /// </summary>
    public DateTime? EndTime { get; set; }
    
    /// <summary>
    /// Gets or sets whether this is an all-day event.
    /// </summary>
    public bool AllDay { get; set; }
    
    /// <summary>
    /// Gets or sets the timezone for this time slot.
    /// </summary>
    public string? Timezone { get; set; }
    
    /// <summary>
    /// Gets or sets the time slot type (e.g., "session", "meal", "activity").
    /// </summary>
    public string? TimeType { get; set; }
    
    /// <summary>
    /// Gets or sets the capacity for this time slot.
    /// </summary>
    public int? Capacity { get; set; }
    
    /// <summary>
    /// Gets or sets the current registration count for this time slot.
    /// </summary>
    public int RegistrationCount { get; set; }
    
    /// <summary>
    /// Gets or sets whether this time slot is required.
    /// </summary>
    public bool Required { get; set; }
    
    /// <summary>
    /// Gets or sets the display order.
    /// </summary>
    public int SortOrder { get; set; }
    
    /// <summary>
    /// Gets or sets whether this time slot is active.
    /// </summary>
    public bool Active { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the location for this time slot.
    /// </summary>
    public string? Location { get; set; }
    
    /// <summary>
    /// Gets or sets the room or specific location.
    /// </summary>
    public string? Room { get; set; }
    
    /// <summary>
    /// Gets or sets the instructor or leader.
    /// </summary>
    public string? Instructor { get; set; }
    
    /// <summary>
    /// Gets or sets the cost for this time slot.
    /// </summary>
    public decimal? Cost { get; set; }
    
    /// <summary>
    /// Gets or sets additional notes for this time slot.
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Gets or sets additional metadata.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}