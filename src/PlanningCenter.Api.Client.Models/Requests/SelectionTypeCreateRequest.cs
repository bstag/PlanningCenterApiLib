namespace PlanningCenter.Api.Client.Models.Requests;

/// <summary>
/// Request model for creating a selection type.
/// </summary>
public class SelectionTypeCreateRequest
{
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
    /// Gets or sets the display order.
    /// </summary>
    public int SortOrder { get; set; }
    
    /// <summary>
    /// Gets or sets whether this selection type is active.
    /// </summary>
    public bool Active { get; set; } = true;
}

/// <summary>
/// Request model for updating a selection type.
/// </summary>
public class SelectionTypeUpdateRequest
{
    /// <summary>
    /// Gets or sets the selection type name.
    /// </summary>
    public string? Name { get; set; }
    
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
    public string? Currency { get; set; }
    
    /// <summary>
    /// Gets or sets whether this selection is required.
    /// </summary>
    public bool? Required { get; set; }
    
    /// <summary>
    /// Gets or sets whether multiple selections are allowed.
    /// </summary>
    public bool? AllowMultiple { get; set; }
    
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
    /// Gets or sets the display order.
    /// </summary>
    public int? SortOrder { get; set; }
    
    /// <summary>
    /// Gets or sets whether this selection type is active.
    /// </summary>
    public bool? Active { get; set; }
}