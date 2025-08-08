

using PlanningCenter.Api.Client.Models;

namespace PlanningCenter.Api.Client.Abstractions;

/// <summary>
/// Interface for caching API responses to improve performance and reduce API calls.
/// Supports both in-memory and distributed caching scenarios.
/// </summary>
public interface ICacheProvider
{
    /// <summary>
    /// Gets a cached value by key.
    /// </summary>
    /// <typeparam name="T">The type of the cached value</typeparam>
    /// <param name="key">The cache key</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The cached value, or default(T) if not found</returns>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Sets a value in the cache with an optional expiration time.
    /// </summary>
    /// <typeparam name="T">The type of the value to cache</typeparam>
    /// <param name="key">The cache key</param>
    /// <param name="value">The value to cache</param>
    /// <param name="expiry">Optional expiration time. If null, uses default expiration.</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Removes a value from the cache.
    /// </summary>
    /// <param name="key">The cache key to remove</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Removes all cache entries that match a pattern.
    /// Useful for invalidating related cache entries (e.g., all person-related cache entries).
    /// </summary>
    /// <param name="pattern">The pattern to match (implementation-specific format)</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a paginated response from the cache.
    /// Specialized method for caching paginated API responses.
    /// </summary>
    /// <typeparam name="T">The type of items in the paginated response</typeparam>
    /// <param name="key">The cache key</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The cached paginated response, or null if not found</returns>
    Task<IPagedResponse<T>?> GetPagedAsync<T>(string key, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Sets a paginated response in the cache.
    /// Specialized method for caching paginated API responses.
    /// </summary>
    /// <typeparam name="T">The type of items in the paginated response</typeparam>
    /// <param name="key">The cache key</param>
    /// <param name="value">The paginated response to cache</param>
    /// <param name="expiry">Optional expiration time. If null, uses default expiration.</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task SetPagedAsync<T>(string key, IPagedResponse<T> value, TimeSpan? expiry = null, CancellationToken cancellationToken = default);
}