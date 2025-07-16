namespace PlanningCenter.Api.Client.Models.Requests;

/// <summary>
/// Request model for creating a category.
/// </summary>
public class CategoryCreateRequest
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
}

/// <summary>
/// Request model for updating a category.
/// </summary>
public class CategoryUpdateRequest
{
    /// <summary>
    /// Gets or sets the category name.
    /// </summary>
    public string? Name { get; set; }
    
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
    public int? SortOrder { get; set; }
    
    /// <summary>
    /// Gets or sets whether this category is active.
    /// </summary>
    public bool? Active { get; set; }
    
    /// <summary>
    /// Gets or sets the category icon or image URL.
    /// </summary>
    public string? IconUrl { get; set; }
}