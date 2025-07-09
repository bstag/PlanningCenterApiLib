
namespace PlanningCenter.Api.Client.Models;

/// <summary>
/// Implementation of IPagedResponse that provides built-in pagination helpers.
/// This class eliminates the need for developers to implement pagination logic manually.
/// </summary>
/// <typeparam name="T">The type of items in the paginated response</typeparam>
public class PagedResponse<T> : IPagedResponse<T>
{
    /// <summary>
    /// The data items for the current page
    /// </summary>
    public IReadOnlyList<T> Data { get; set; } = new List<T>();
    
    /// <summary>
    /// Pagination metadata including total count, current page, etc.
    /// </summary>
    public PagedResponseMeta Meta { get; set; } = new();
    
    /// <summary>
    /// Navigation links for pagination (next, previous, first, last)
    /// </summary>
    public PagedResponseLinks Links { get; set; } = new();
    
    /// <summary>
    /// Indicates if there are more pages available after the current page
    /// </summary>
    public bool HasNextPage => Links.CanNavigateNext;
    
    /// <summary>
    /// Indicates if there are previous pages available before the current page
    /// </summary>
    public bool HasPreviousPage => Links.CanNavigatePrevious;
    
    // Internal properties for navigation (will be set by the implementation)
    internal IApiConnection? ApiConnection { get; set; }
    internal QueryParameters? OriginalParameters { get; set; }
    internal string? OriginalEndpoint { get; set; }
    
    /// <summary>
    /// Gets the next page of results using the same query parameters.
    /// This is a built-in helper that eliminates manual pagination logic.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The next page of results, or null if no next page exists</returns>
    public async Task<IPagedResponse<T>?> GetNextPageAsync(CancellationToken cancellationToken = default)
    {
        if (!HasNextPage || ApiConnection == null || OriginalParameters == null || OriginalEndpoint == null)
        {
            return null;
        }
        
        // Create parameters for the next page
        var nextPageParameters = OriginalParameters.Clone();
        nextPageParameters.Offset = Meta.Offset + Meta.PerPage;
        
        return await ApiConnection.GetPagedAsync<T>(OriginalEndpoint, nextPageParameters, cancellationToken);
    }
    
    /// <summary>
    /// Gets the previous page of results using the same query parameters.
    /// This is a built-in helper that eliminates manual pagination logic.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The previous page of results, or null if no previous page exists</returns>
    public async Task<IPagedResponse<T>?> GetPreviousPageAsync(CancellationToken cancellationToken = default)
    {
        if (!HasPreviousPage || ApiConnection == null || OriginalParameters == null || OriginalEndpoint == null)
        {
            return null;
        }
        
        // Create parameters for the previous page
        var previousPageParameters = OriginalParameters.Clone();
        var previousOffset = Math.Max(0, Meta.Offset - Meta.PerPage);
        previousPageParameters.Offset = previousOffset;
        
        return await ApiConnection.GetPagedAsync<T>(OriginalEndpoint, previousPageParameters, cancellationToken);
    }
    
    /// <summary>
    /// Automatically fetches all remaining pages and returns a flattened collection.
    /// This is a powerful helper that handles all pagination logic automatically.
    /// Use with caution for large datasets - consider using GetAllRemainingAsyncEnumerable for memory efficiency.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>All remaining items from this page forward</returns>
    public async Task<IReadOnlyList<T>> GetAllRemainingAsync(CancellationToken cancellationToken = default)
    {
        var allItems = new List<T>(Data);
        var currentPage = this;
        
        while (currentPage.HasNextPage)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var nextPage = await currentPage.GetNextPageAsync(cancellationToken);
            if (nextPage == null) break;
            
            allItems.AddRange(nextPage.Data);
            currentPage = (PagedResponse<T>)nextPage;
        }
        
        return allItems.AsReadOnly();
    }
    
    /// <summary>
    /// Streams all remaining items as an async enumerable for memory-efficient processing of large datasets.
    /// This helper automatically handles pagination while using minimal memory.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>An async enumerable that yields items from all remaining pages</returns>
    public async IAsyncEnumerable<T> GetAllRemainingAsyncEnumerable([System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // Yield items from current page
        foreach (var item in Data)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return item;
        }
        
        // Yield items from remaining pages
        var currentPage = this;
        while (currentPage.HasNextPage)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var nextPage = await currentPage.GetNextPageAsync(cancellationToken);
            if (nextPage == null) yield break;
            
            foreach (var item in nextPage.Data)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return item;
            }
            
            currentPage = (PagedResponse<T>)nextPage;
        }
    }
}