namespace PlanningCenter.Api.Client.Models;

/// <summary>
/// Navigation links for paginated responses.
/// These URLs can be used to navigate between pages without manually constructing query parameters.
/// </summary>
public class PagedResponseLinks
{
    /// <summary>
    /// URL for the first page of results
    /// </summary>
    public string? First { get; set; }
    
    /// <summary>
    /// URL for the previous page (null if on first page)
    /// </summary>
    public string? Previous { get; set; }
    
    /// <summary>
    /// URL for the current page
    /// </summary>
    public string? Self { get; set; }
    
    /// <summary>
    /// URL for the next page (null if on last page)
    /// </summary>
    public string? Next { get; set; }
    
    /// <summary>
    /// URL for the last page of results
    /// </summary>
    public string? Last { get; set; }
    
    /// <summary>
    /// Indicates if navigation to the next page is possible
    /// </summary>
    public bool CanNavigateNext => !string.IsNullOrEmpty(Next);
    
    /// <summary>
    /// Indicates if navigation to the previous page is possible
    /// </summary>
    public bool CanNavigatePrevious => !string.IsNullOrEmpty(Previous);
}