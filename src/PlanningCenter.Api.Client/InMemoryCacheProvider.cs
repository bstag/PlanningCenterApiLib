using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;
using System.Text.RegularExpressions;

namespace PlanningCenter.Api.Client;

/// <summary>
/// In-memory cache provider for API responses.
/// Provides caching with expiration and pattern-based invalidation.
/// </summary>
public class InMemoryCacheProvider : ICacheProvider, IDisposable
{
    private readonly IMemoryCache _cache;
    private readonly PlanningCenterOptions _options;
    private readonly ILogger<InMemoryCacheProvider> _logger;
    private readonly HashSet<string> _cacheKeys = new();
    private readonly object _keysLock = new();
    private bool _disposed;

    public InMemoryCacheProvider(
        IMemoryCache cache,
        IOptions<PlanningCenterOptions> options,
        ILogger<InMemoryCacheProvider> logger)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets a cached value by key.
    /// </summary>
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        if (key.Length == 0)
            throw new ArgumentException("Cache key cannot be empty", nameof(key));

        if (!_options.EnableCaching)
        {
            return Task.FromResult<T?>(default);
        }

        var result = _cache.Get<T>(key);
        
        if (result != null)
        {
            _logger.LogDebug("Cache hit for key: {Key}", key);
        }
        else
        {
            _logger.LogDebug("Cache miss for key: {Key}", key);
        }

        return Task.FromResult(result);
    }

    /// <summary>
    /// Sets a value in the cache with an optional expiration time.
    /// </summary>
    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        if (key.Length == 0)
            throw new ArgumentException("Cache key cannot be empty", nameof(key));

        if (!_options.EnableCaching)
        {
            return Task.CompletedTask;
        }

        var expirationTime = expiry ?? _options.DefaultCacheExpiration;
        if (expirationTime <= TimeSpan.Zero)
        {
            // Treat zero/negative expiration as immediate expiration (store, then remove)
            _cache.Set(key, value, TimeSpan.FromMilliseconds(1));
            lock (_keysLock)
            {
                _cacheKeys.Add(key);
            }
            _logger.LogDebug("Cache entry set with immediate expiration: {Key}", key);
            // Remove immediately
            _cache.Remove(key);
            lock (_keysLock)
            {
                _cacheKeys.Remove(key);
            }
            return Task.CompletedTask;
        }

        // Eviction: if max cache size is exceeded, evict oldest
        lock (_keysLock)
        {
            if (_options.MaxCacheSize > 0 && _cacheKeys.Count >= _options.MaxCacheSize)
            {
                var oldestKey = _cacheKeys.FirstOrDefault();
                if (oldestKey != null)
                {
                    _cache.Remove(oldestKey);
                    _cacheKeys.Remove(oldestKey);
                    _logger.LogDebug("Evicted oldest cache entry: {Key}", oldestKey);
                }
            }
        }
        
        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expirationTime,
            Priority = CacheItemPriority.Normal
        };

        // Add removal callback to track keys
        cacheEntryOptions.RegisterPostEvictionCallback((k, v, reason, state) =>
        {
            lock (_keysLock)
            {
                _cacheKeys.Remove(k.ToString()!);
            }
            
            _logger.LogDebug("Cache entry removed: {Key}, Reason: {Reason}", k, reason);
        });

        _cache.Set(key, value, cacheEntryOptions);
        
        lock (_keysLock)
        {
            _cacheKeys.Add(key);
        }

        _logger.LogDebug("Cache entry set: {Key}, Expires in: {Expiry}", key, expirationTime);
        
        return Task.CompletedTask;
    }

    /// <summary>
    /// Removes a value from the cache.
    /// </summary>
    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            throw new OperationCanceledException(cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        if (key.Length == 0)
            throw new ArgumentException("Cache key cannot be empty", nameof(key));

        _cache.Remove(key);
        
        lock (_keysLock)
        {
            _cacheKeys.Remove(key);
        }

        _logger.LogDebug("Cache entry removed: {Key}", key);
        
        return Task.CompletedTask;
    }

    /// <summary>
    /// Removes all cache entries that match a pattern.
    /// Useful for invalidating related cache entries (e.g., all person-related cache entries).
    /// </summary>
    public Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (pattern == null)
            throw new ArgumentNullException(nameof(pattern));
        if (pattern.Length == 0)
            throw new ArgumentException("Pattern cannot be empty", nameof(pattern));

        List<string> keysToRemove;
        
        lock (_keysLock)
        {
            try
            {
                var regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                keysToRemove = _cacheKeys.Where(key => regex.IsMatch(key)).ToList();
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid regex pattern: {Pattern}", pattern);
                throw new ArgumentException($"Invalid regex pattern: {pattern}", nameof(pattern), ex);
            }
        }

        foreach (var key in keysToRemove)
        {
            _cache.Remove(key);
            
            lock (_keysLock)
            {
                _cacheKeys.Remove(key);
            }
        }

        _logger.LogInformation("Removed {Count} cache entries matching pattern: {Pattern}", 
            keysToRemove.Count, pattern);
        
        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets a paginated response from the cache.
    /// Specialized method for caching paginated API responses.
    /// </summary>
    public async Task<IPagedResponse<T>?> GetPagedAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var result = await GetAsync<PagedResponse<T>>(key, cancellationToken);
        return result;
    }

    /// <summary>
    /// Sets a paginated response in the cache.
    /// Specialized method for caching paginated API responses.
    /// </summary>
    public async Task SetPagedAsync<T>(string key, IPagedResponse<T> value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        // Only cache concrete PagedResponse instances
        if (value is PagedResponse<T> concreteResponse)
        {
            // Create a copy without the navigation properties to avoid circular references
            var cacheableResponse = new PagedResponse<T>
            {
                Data = concreteResponse.Data,
                Meta = concreteResponse.Meta,
                Links = concreteResponse.Links
                // Don't copy ApiConnection, OriginalParameters, OriginalEndpoint
            };
            
            await SetAsync(key, cacheableResponse, expiry, cancellationToken);
        }
        else
        {
            _logger.LogWarning("Cannot cache non-concrete PagedResponse implementation for key: {Key}", key);
        }
    }

    /// <summary>
    /// Gets cache statistics for monitoring.
    /// </summary>
    public CacheStatistics GetStatistics()
    {
        lock (_keysLock)
        {
            return new CacheStatistics
            {
                TotalKeys = _cacheKeys.Count,
                IsEnabled = _options.EnableCaching,
                DefaultExpiration = _options.DefaultCacheExpiration
            };
        }
    }

    /// <summary>
    /// Clears all cache entries.
    /// </summary>
    public Task ClearAllAsync(CancellationToken cancellationToken = default)
    {
        return ClearAsync(cancellationToken);
    }

    /// <summary>
    /// Clears all cache entries.
    /// </summary>
    public Task ClearAsync(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            throw new OperationCanceledException(cancellationToken);
        List<string> allKeys;
        
        lock (_keysLock)
        {
            allKeys = _cacheKeys.ToList();
        }

        foreach (var key in allKeys)
        {
            _cache.Remove(key);
        }
        
        lock (_keysLock)
        {
            _cacheKeys.Clear();
        }

        _logger.LogInformation("Cleared all {Count} cache entries", allKeys.Count);
        
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            // IMemoryCache is typically managed by DI container, so we don't dispose it
            _disposed = true;
        }
    }
}

/// <summary>
/// Cache statistics for monitoring.
/// </summary>
public class CacheStatistics
{
    /// <summary>
    /// Total number of keys in the cache
    /// </summary>
    public int TotalKeys { get; set; }
    
    /// <summary>
    /// Whether caching is enabled
    /// </summary>
    public bool IsEnabled { get; set; }
    
    /// <summary>
    /// Default cache expiration time
    /// </summary>
    public TimeSpan DefaultExpiration { get; set; }
}