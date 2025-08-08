

using PlanningCenter.Api.Client.Models;

namespace PlanningCenter.Api.Client.Abstractions;

/// <summary>
/// Represents a paginated response from the Planning Center API with built-in navigation helpers.
/// This interface eliminates the need for manual pagination logic by providing automatic page fetching capabilities.
/// </summary>
/// <typeparam name="T">The type of items in the paginated response</typeparam>
public interface IPagedResponse<T>
{
    /// <summary>
    /// The data items for the current page
    /// </summary>
    IReadOnlyList<T> Data { get; }
    
    /// <summary>
    /// Pagination metadata including total count, current page, etc.
    /// </summary>
    PagedResponseMeta Meta { get; }
    
    /// <summary>
    /// Navigation links for pagination (next, previous, first, last)
    /// </summary>
    PagedResponseLinks Links { get; }
    
    /// <summary>
    /// Indicates if there are more pages available after the current page
    /// </summary>
    bool HasNextPage { get; }
    
    /// <summary>
    /// Indicates if there are previous pages available before the current page
    /// </summary>
    bool HasPreviousPage { get; }
    
    /// <summary>
    /// Gets the next page of results using the same query parameters.
    /// This is a built-in helper that eliminates manual pagination logic.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The next page of results, or null if no next page exists</returns>
    Task<IPagedResponse<T>?> GetNextPageAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the previous page of results using the same query parameters.
    /// This is a built-in helper that eliminates manual pagination logic.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The previous page of results, or null if no previous page exists</returns>
    Task<IPagedResponse<T>?> GetPreviousPageAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Automatically fetches all remaining pages and returns a flattened collection.
    /// This is a powerful helper that handles all pagination logic automatically.
    /// Use with caution for large datasets - consider using GetAllRemainingAsyncEnumerable for memory efficiency.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>All remaining items from this page forward</returns>
    Task<IReadOnlyList<T>> GetAllRemainingAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Streams all remaining items as an async enumerable for memory-efficient processing of large datasets.
    /// This helper automatically handles pagination while using minimal memory.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>An async enumerable that yields items from all remaining pages</returns>
    IAsyncEnumerable<T> GetAllRemainingAsyncEnumerable(CancellationToken cancellationToken = default);
}