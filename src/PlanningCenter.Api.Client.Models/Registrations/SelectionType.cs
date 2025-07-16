using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.Registrations;

/// <summary>
/// Represents a registration option or selection type in Planning Center Registrations.
/// </summary>
public class SelectionType : PlanningCenterResource
{
    /// <summary>
    /// Gets or sets the associated signup ID.
    /// </summary>
    public string SignupId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the selection type name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the selection type description.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the selection type category.
    /// </summary>
    public string? Category { get; set; }
    
    /// <summary>
    /// Gets or sets the cost for this selection.
    /// </summary>
    public decimal? Cost { get; set; }
    
    /// <summary>
    /// Gets or sets the currency for the cost.
    /// </summary>
    public string Currency { get; set; } = "USD";
    
    /// <summary>
    /// Gets or sets whether this selection is required.
    /// </summary>
    public bool Required { get; set; }
    
    /// <summary>
    /// Gets or sets whether multiple selections are allowed.
    /// </summary>
    public bool AllowMultiple { get; set; }
    
    /// <summary>
    /// Gets or sets the maximum number of selections allowed.
    /// </summary>
    public int? MaxSelections { get; set; }
    
    /// <summary>
    /// Gets or sets the minimum number of selections required.
    /// </summary>
    public int? MinSelections { get; set; }
    
    /// <summary>
    /// Gets or sets the selection limit (capacity).
    /// </summary>
    public int? SelectionLimit { get; set; }
    
    /// <summary>
    /// Gets or sets the current selection count.
    /// </summary>
    public int SelectionCount { get; set; }
    
    /// <summary>
    /// Gets or sets the display order.
    /// </summary>
    public int SortOrder { get; set; }
    
    /// <summary>
    /// Gets or sets whether this selection type is active.
    /// </summary>
    public bool Active { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the selection type options.
    /// </summary>
    public List<SelectionOption> Options { get; set; } = new();
    
    /// <summary>
    /// Gets or sets additional metadata.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Represents an option within a selection type.
/// </summary>
public class SelectionOption
{
    /// <summary>
    /// Gets or sets the option ID.
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the option label.
    /// </summary>
    public string Label { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the option value.
    /// </summary>
    public string Value { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the additional cost for this option.
    /// </summary>
    public decimal? AdditionalCost { get; set; }
    
    /// <summary>
    /// Gets or sets the display order.
    /// </summary>
    public int SortOrder { get; set; }
    
    /// <summary>
    /// Gets or sets whether this option is active.
    /// </summary>
    public bool Active { get; set; } = true;
}