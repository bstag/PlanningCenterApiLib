namespace PlanningCenter.Api.Client.Models;

/// <summary>
/// Rich metadata about the current pagination state.
/// Provides comprehensive information about the current page, total items, and progress through the dataset.
/// </summary>
public class PagedResponseMeta
{
    /// <summary>
    /// Total number of items across all pages
    /// </summary>
    public int TotalCount { get; set; }
    
    /// <summary>
    /// Number of items per page (page size)
    /// </summary>
    public int PerPage { get; set; }
    
    /// <summary>
    /// Current page number (1-based indexing)
    /// </summary>
    public int CurrentPage { get; set; }
    
    /// <summary>
    /// Total number of pages available
    /// </summary>
    public int TotalPages { get; set; }
    
    /// <summary>
    /// Zero-based offset of the first item on the current page
    /// </summary>
    public int Offset { get; set; }
    
    /// <summary>
    /// Number of items on the current page (may be less than PerPage on the last page)
    /// </summary>
    public int Count { get; set; }
    
    /// <summary>
    /// Indicates if this is the first page
    /// </summary>
    public bool IsFirstPage => CurrentPage == 1;
    
    /// <summary>
    /// Indicates if this is the last page
    /// </summary>
    public bool IsLastPage => CurrentPage == TotalPages;
    
    /// <summary>
    /// Calculates the percentage of completion through all pages (0-100)
    /// </summary>
    public double ProgressPercentage => TotalPages > 0 ? (double)CurrentPage / TotalPages * 100 : 0;
    
    /// <summary>
    /// Gets a human-readable status string for progress reporting
    /// </summary>
    public string StatusDescription => $"Page {CurrentPage} of {TotalPages} ({Count} items, {TotalCount} total)";
    
    /// <summary>
    /// Fields that can be used for ordering results
    /// </summary>
    public string[] CanOrderBy { get; set; } = Array.Empty<string>();
    
    /// <summary>
    /// Fields that can be used for querying/filtering results
    /// </summary>
    public string[] CanQueryBy { get; set; } = Array.Empty<string>();
    
    /// <summary>
    /// Related resources that can be included in the response
    /// </summary>
    public string[] CanInclude { get; set; } = Array.Empty<string>();
    
    /// <summary>
    /// Fields that can be used for filtering results
    /// </summary>
    public string[] CanFilter { get; set; } = Array.Empty<string>();
    
    /// <summary>
    /// Information about the parent resource (if applicable)
    /// </summary>
    public Dictionary<string, object> Parent { get; set; } = new();
}