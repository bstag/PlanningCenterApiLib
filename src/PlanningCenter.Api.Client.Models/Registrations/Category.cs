using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.Registrations;

/// <summary>
/// Represents a category for organizing signups in Planning Center Registrations.
/// </summary>
public class Category : PlanningCenterResource
{
    /// <summary>
    /// Gets or sets the category name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the category description.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the category color (hex code).
    /// </summary>
    public string? Color { get; set; }
    
    /// <summary>
    /// Gets or sets the display order.
    /// </summary>
    public int SortOrder { get; set; }
    
    /// <summary>
    /// Gets or sets whether this category is active.
    /// </summary>
    public bool Active { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the parent category ID (for hierarchical categories).
    /// </summary>
    public string? ParentCategoryId { get; set; }
    
    /// <summary>
    /// Gets or sets the category icon or image URL.
    /// </summary>
    public string? IconUrl { get; set; }
    
    /// <summary>
    /// Gets or sets the number of signups in this category.
    /// </summary>
    public int SignupCount { get; set; }
    
    /// <summary>
    /// Gets or sets additional metadata.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}