# Pagination Strategy for Planning Center SDK

> **Implementation Status**: This document describes the planned pagination architecture. Current implementation only includes basic project structure. See [CURRENT_STATUS.md](../CURRENT_STATUS.md) for actual implementation status.

## Overview

The Planning Center SDK implements a comprehensive pagination strategy that provides built-in helpers for handling paginated API responses. This eliminates the need for developers to manually implement pagination logic, offering multiple patterns for different use cases while maintaining performance and memory efficiency.

## Core Pagination Concepts

### 1. **IPagedResponse Interface**
The foundation of all paginated operations in the SDK.

```csharp
namespace PlanningCenter.Api.Client.Models.Responses
{
    public interface IPagedResponse<T>
    {
        /// <summary>
        /// The data items for the current page
        /// </summary>
        IReadOnlyList<T> Data { get; }
        
        /// <summary>
        /// Pagination metadata
        /// </summary>
        PagedResponseMeta Meta { get; }
        
        /// <summary>
        /// Navigation links for pagination
        /// </summary>
        PagedResponseLinks Links { get; }
        
        /// <summary>
        /// Indicates if there are more pages available
        /// </summary>
        bool HasNextPage { get; }
        
        /// <summary>
        /// Indicates if there are previous pages available
        /// </summary>
        bool HasPreviousPage { get; }
        
        /// <summary>
        /// Gets the next page of results
        /// </summary>
        Task<IPagedResponse<T>> GetNextPageAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Gets the previous page of results
        /// </summary>
        Task<IPagedResponse<T>> GetPreviousPageAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Gets all remaining pages and returns a flattened collection
        /// </summary>
        Task<IReadOnlyList<T>> GetAllRemainingAsync(CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Streams all remaining items as an async enumerable
        /// </summary>
        IAsyncEnumerable<T> GetAllRemainingAsyncEnumerable(CancellationToken cancellationToken = default);
    }
}
```

### 2. **Pagination Metadata**
Rich metadata about the current pagination state.

```csharp
public class PagedResponseMeta
{
    /// <summary>
    /// Total number of items across all pages
    /// </summary>
    public int TotalCount { get; set; }
    
    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PerPage { get; set; }
    
    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    public int CurrentPage { get; set; }
    
    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; set; }
    
    /// <summary>
    /// Zero-based offset of the first item on current page
    /// </summary>
    public int Offset { get; set; }
    
    /// <summary>
    /// Number of items on the current page
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
    /// Calculates the percentage of completion through all pages
    /// </summary>
    public double ProgressPercentage => TotalPages > 0 ? (double)CurrentPage / TotalPages * 100 : 0;
}
```

### 3. **Navigation Links**
URLs for navigating between pages.

```csharp
public class PagedResponseLinks
{
    /// <summary>
    /// URL for the first page
    /// </summary>
    public string First { get; set; }
    
    /// <summary>
    /// URL for the previous page (null if on first page)
    /// </summary>
    public string Previous { get; set; }
    
    /// <summary>
    /// URL for the current page
    /// </summary>
    public string Self { get; set; }
    
    /// <summary>
    /// URL for the next page (null if on last page)
    /// </summary>
    public string Next { get; set; }
    
    /// <summary>
    /// URL for the last page
    /// </summary>
    public string Last { get; set; }
}
```

## Built-in Pagination Helpers

### 1. **PagedResponseExtensions**
Extension methods that provide powerful pagination utilities.

```csharp
namespace PlanningCenter.Api.Client.Extensions
{
    public static class PagedResponseExtensions
    {
        /// <summary>
        /// Automatically fetches all pages and returns a flattened collection
        /// </summary>
        public static async Task<IReadOnlyList<T>> GetAllPagesAsync<T>(
            this IPagedResponse<T> firstPage,
            CancellationToken cancellationToken = default)
        {
            var allItems = new List<T>(firstPage.Data);
            var currentPage = firstPage;
            
            while (currentPage.HasNextPage)
            {
                cancellationToken.ThrowIfCancellationRequested();
                currentPage = await currentPage.GetNextPageAsync(cancellationToken);
                allItems.AddRange(currentPage.Data);
            }
            
            return allItems.AsReadOnly();
        }
        
        /// <summary>
        /// Streams all pages as an async enumerable for memory-efficient processing
        /// </summary>
        public static async IAsyncEnumerable<T> StreamAllPagesAsync<T>(
            this IPagedResponse<T> firstPage,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var currentPage = firstPage;
            
            do
            {
                foreach (var item in currentPage.Data)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    yield return item;
                }
                
                if (currentPage.HasNextPage)
                {
                    currentPage = await currentPage.GetNextPageAsync(cancellationToken);
                }
                else
                {
                    break;
                }
            } while (true);
        }
        
        /// <summary>
        /// Processes all pages with a custom action, useful for batch processing
        /// </summary>
        public static async Task ProcessAllPagesAsync<T>(
            this IPagedResponse<T> firstPage,
            Func<IReadOnlyList<T>, Task> pageProcessor,
            CancellationToken cancellationToken = default)
        {
            var currentPage = firstPage;
            
            do
            {
                cancellationToken.ThrowIfCancellationRequested();
                await pageProcessor(currentPage.Data);
                
                if (currentPage.HasNextPage)
                {
                    currentPage = await currentPage.GetNextPageAsync(cancellationToken);
                }
                else
                {
                    break;
                }
            } while (true);
        }
        
        /// <summary>
        /// Finds the first item across all pages that matches a predicate
        /// </summary>
        public static async Task<T> FindFirstAcrossPagesAsync<T>(
            this IPagedResponse<T> firstPage,
            Func<T, bool> predicate,
            CancellationToken cancellationToken = default)
        {
            var currentPage = firstPage;
            
            do
            {
                var found = currentPage.Data.FirstOrDefault(predicate);
                if (found != null)
                    return found;
                
                if (currentPage.HasNextPage)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    currentPage = await currentPage.GetNextPageAsync(cancellationToken);
                }
                else
                {
                    break;
                }
            } while (true);
            
            return default(T);
        }
        
        /// <summary>
        /// Counts all items across all pages that match a predicate
        /// </summary>
        public static async Task<int> CountAcrossPagesAsync<T>(
            this IPagedResponse<T> firstPage,
            Func<T, bool> predicate = null,
            CancellationToken cancellationToken = default)
        {
            var count = 0;
            var currentPage = firstPage;
            
            do
            {
                count += predicate == null 
                    ? currentPage.Data.Count 
                    : currentPage.Data.Count(predicate);
                
                if (currentPage.HasNextPage)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    currentPage = await currentPage.GetNextPageAsync(cancellationToken);
                }
                else
                {
                    break;
                }
            } while (true);
            
            return count;
        }
        
        /// <summary>
        /// Converts a paged response to a paginated view model for UI binding
        /// </summary>
        public static PaginatedViewModel<TViewModel> ToViewModel<T, TViewModel>(
            this IPagedResponse<T> pagedResponse,
            Func<T, TViewModel> mapper)
        {
            return new PaginatedViewModel<TViewModel>
            {
                Items = pagedResponse.Data.Select(mapper).ToList(),
                CurrentPage = pagedResponse.Meta.CurrentPage,
                TotalPages = pagedResponse.Meta.TotalPages,
                TotalCount = pagedResponse.Meta.TotalCount,
                PerPage = pagedResponse.Meta.PerPage,
                HasNextPage = pagedResponse.HasNextPage,
                HasPreviousPage = pagedResponse.HasPreviousPage
            };
        }
    }
}
```

### 2. **PaginationHelper**
Utility class for common pagination calculations and operations.

```csharp
namespace PlanningCenter.Api.Client.Helpers
{
    public static class PaginationHelper
    {
        /// <summary>
        /// Calculates the optimal page size based on expected total count and memory constraints
        /// </summary>
        public static int CalculateOptimalPageSize(int estimatedTotalCount, int maxMemoryMB = 50)
        {
            // Estimate memory per item (rough calculation)
            const int estimatedBytesPerItem = 2048; // 2KB per item estimate
            var maxItemsInMemory = (maxMemoryMB * 1024 * 1024) / estimatedBytesPerItem;
            
            // Use smaller page sizes for large datasets
            if (estimatedTotalCount > 10000)
                return Math.Min(50, maxItemsInMemory);
            if (estimatedTotalCount > 1000)
                return Math.Min(100, maxItemsInMemory);
            
            return Math.Min(250, maxItemsInMemory);
        }
        
        /// <summary>
        /// Creates query parameters for a specific page
        /// </summary>
        public static QueryParameters CreatePageParameters(int page, int pageSize, QueryParameters baseParameters = null)
        {
            var parameters = baseParameters?.Clone() ?? new QueryParameters();
            parameters.PerPage = pageSize;
            parameters.Offset = (page - 1) * pageSize;
            return parameters;
        }
        
        /// <summary>
        /// Validates pagination parameters
        /// </summary>
        public static (int page, int pageSize) ValidatePaginationParameters(int page, int pageSize)
        {
            const int maxPageSize = 1000;
            const int defaultPageSize = 25;
            
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = defaultPageSize;
            if (pageSize > maxPageSize) pageSize = maxPageSize;
            
            return (page, pageSize);
        }
        
        /// <summary>
        /// Estimates the time to fetch all pages based on current performance
        /// </summary>
        public static TimeSpan EstimateTimeToFetchAllPages(
            int totalPages, 
            TimeSpan averagePageFetchTime)
        {
            // Add 10% buffer for network variability
            var estimatedTime = TimeSpan.FromMilliseconds(
                totalPages * averagePageFetchTime.TotalMilliseconds * 1.1);
            
            return estimatedTime;
        }
    }
}
```

### 3. **PaginatedEnumerator**
Advanced enumerator for complex pagination scenarios.

```csharp
namespace PlanningCenter.Api.Client.Pagination
{
    public class PaginatedEnumerator<T> : IAsyncEnumerable<T>, IAsyncDisposable
    {
        private readonly Func<QueryParameters, CancellationToken, Task<IPagedResponse<T>>> _pageProvider;
        private readonly QueryParameters _baseParameters;
        private readonly PaginationOptions _options;
        
        public PaginatedEnumerator(
            Func<QueryParameters, CancellationToken, Task<IPagedResponse<T>>> pageProvider,
            QueryParameters baseParameters = null,
            PaginationOptions options = null)
        {
            _pageProvider = pageProvider;
            _baseParameters = baseParameters ?? new QueryParameters();
            _options = options ?? new PaginationOptions();
        }
        
        public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            var pageSize = _options.PageSize ?? PaginationHelper.CalculateOptimalPageSize(_options.EstimatedTotalCount);
            var page = 1;
            var totalFetched = 0;
            
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                // Check if we've reached the maximum items limit
                if (_options.MaxItems.HasValue && totalFetched >= _options.MaxItems.Value)
                    yield break;
                
                var parameters = PaginationHelper.CreatePageParameters(page, pageSize, _baseParameters);
                var response = await _pageProvider(parameters, cancellationToken);
                
                if (!response.Data.Any())
                    yield break;
                
                foreach (var item in response.Data)
                {
                    if (_options.MaxItems.HasValue && totalFetched >= _options.MaxItems.Value)
                        yield break;
                    
                    cancellationToken.ThrowIfCancellationRequested();
                    yield return item;
                    totalFetched++;
                }
                
                if (!response.HasNextPage)
                    yield break;
                
                page++;
                
                // Add delay between pages if configured
                if (_options.DelayBetweenPages.HasValue)
                {
                    await Task.Delay(_options.DelayBetweenPages.Value, cancellationToken);
                }
            }
        }
        
        public ValueTask DisposeAsync()
        {
            // Cleanup if needed
            return ValueTask.CompletedTask;
        }
    }
    
    public class PaginationOptions
    {
        /// <summary>
        /// Page size to use (if null, will be calculated automatically)
        /// </summary>
        public int? PageSize { get; set; }
        
        /// <summary>
        /// Maximum number of items to fetch (if null, fetches all)
        /// </summary>
        public int? MaxItems { get; set; }
        
        /// <summary>
        /// Estimated total count for optimization
        /// </summary>
        public int EstimatedTotalCount { get; set; } = 1000;
        
        /// <summary>
        /// Delay between page requests to avoid rate limiting
        /// </summary>
        public TimeSpan? DelayBetweenPages { get; set; }
        
        /// <summary>
        /// Whether to prefetch the next page for better performance
        /// </summary>
        public bool PrefetchNextPage { get; set; } = false;
    }
}
```

## Service Integration Patterns

### 1. **Service Interface with Pagination Support**
```csharp
public interface IPeopleService
{
    // Standard paginated list
    Task<IPagedResponse<Core.Person>> ListAsync(
        QueryParameters parameters = null, 
        CancellationToken cancellationToken = default);
    
    // Convenience method for getting all items
    Task<IReadOnlyList<Core.Person>> GetAllAsync(
        QueryParameters parameters = null,
        PaginationOptions options = null,
        CancellationToken cancellationToken = default);
    
    // Streaming method for large datasets
    IAsyncEnumerable<Core.Person> StreamAsync(
        QueryParameters parameters = null,
        PaginationOptions options = null,
        CancellationToken cancellationToken = default);
    
    // Paginated search with custom pagination
    Task<IPagedResponse<Core.Person>> SearchAsync(
        string query,
        int page = 1,
        int pageSize = 25,
        CancellationToken cancellationToken = default);
}
```

### 2. **Service Implementation with Pagination**
```csharp
public class PeopleService : IPeopleService
{
    private readonly IApiConnection _apiConnection;
    private readonly PersonMapper _personMapper;
    private readonly ILogger<PeopleService> _logger;
    
    public async Task<IPagedResponse<Core.Person>> ListAsync(
        QueryParameters parameters = null, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Listing people with parameters: {@Parameters}", parameters);
        
        var response = await _apiConnection.GetPagedAsync<People.PersonDto>(
            "/people/v2/people", parameters, cancellationToken);
        
        var people = response.Data.Select(_personMapper.Map).ToList();
        
        _logger.LogInformation("Successfully retrieved page {Page} with {Count} people", 
            response.Meta.CurrentPage, people.Count);
        
        return new PagedResponse<Core.Person>
        {
            Data = people,
            Meta = response.Meta,
            Links = response.Links,
            ApiConnection = _apiConnection, // For navigation methods
            Mapper = _personMapper
        };
    }
    
    public async Task<IReadOnlyList<Core.Person>> GetAllAsync(
        QueryParameters parameters = null,
        PaginationOptions options = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching all people with options: {@Options}", options);
        
        var firstPage = await ListAsync(parameters, cancellationToken);
        
        if (options?.MaxItems == firstPage.Data.Count && !firstPage.HasNextPage)
        {
            return firstPage.Data;
        }
        
        return await firstPage.GetAllPagesAsync(cancellationToken);
    }
    
    public async IAsyncEnumerable<Core.Person> StreamAsync(
        QueryParameters parameters = null,
        PaginationOptions options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var enumerator = new PaginatedEnumerator<Core.Person>(
            async (queryParams, ct) =>
            {
                var response = await _apiConnection.GetPagedAsync<People.PersonDto>(
                    "/people/v2/people", queryParams, ct);
                
                var people = response.Data.Select(_personMapper.Map).ToList();
                
                return new PagedResponse<Core.Person>
                {
                    Data = people,
                    Meta = response.Meta,
                    Links = response.Links,
                    ApiConnection = _apiConnection,
                    Mapper = _personMapper
                };
            },
            parameters,
            options);
        
        await foreach (var person in enumerator.WithCancellation(cancellationToken))
        {
            yield return person;
        }
    }
}
```

## Fluent API Pagination

### 1. **Fluent Context with Pagination**
```csharp
public interface IPeopleFluentContext
{
    // Query building
    IPeopleFluentContext Where(Expression<Func<Core.Person, bool>> predicate);
    IPeopleFluentContext Include(Expression<Func<Core.Person, object>> include);
    IPeopleFluentContext OrderBy(Expression<Func<Core.Person, object>> orderBy);
    
    // Pagination execution methods
    Task<IPagedResponse<Core.Person>> GetPagedAsync(
        int pageSize = 25, 
        CancellationToken cancellationToken = default);
    
    Task<IPagedResponse<Core.Person>> GetPageAsync(
        int page, 
        int pageSize = 25, 
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<Core.Person>> GetAllAsync(
        PaginationOptions options = null,
        CancellationToken cancellationToken = default);
    
    IAsyncEnumerable<Core.Person> AsAsyncEnumerable(
        PaginationOptions options = null,
        CancellationToken cancellationToken = default);
    
    // Convenience methods
    Task<Core.Person> FirstAsync(CancellationToken cancellationToken = default);
    Task<Core.Person> FirstOrDefaultAsync(CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
}
```

### 2. **Fluent Implementation Example**
```csharp
public class PeopleFluentContext : IPeopleFluentContext
{
    private readonly IPeopleService _peopleService;
    private readonly QueryBuilder _queryBuilder;
    
    public async Task<IPagedResponse<Core.Person>> GetPagedAsync(
        int pageSize = 25, 
        CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = pageSize;
        
        return await _peopleService.ListAsync(parameters, cancellationToken);
    }
    
    public async Task<IReadOnlyList<Core.Person>> GetAllAsync(
        PaginationOptions options = null,
        CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        return await _peopleService.GetAllAsync(parameters, options, cancellationToken);
    }
    
    public async IAsyncEnumerable<Core.Person> AsAsyncEnumerable(
        PaginationOptions options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        await foreach (var person in _peopleService.StreamAsync(parameters, options, cancellationToken))
        {
            yield return person;
        }
    }
}
```

## Performance Optimization

### 1. **Memory Management**
```csharp
public class MemoryEfficientPaginationService
{
    private readonly IPeopleService _peopleService;
    private readonly IMemoryCache _cache;
    
    /// <summary>
    /// Processes large datasets without loading everything into memory
    /// </summary>
    public async Task ProcessLargeDatasetAsync(
        Func<Core.Person, Task> processor,
        CancellationToken cancellationToken = default)
    {
        var options = new PaginationOptions
        {
            PageSize = 50, // Small page size for memory efficiency
            DelayBetweenPages = TimeSpan.FromMilliseconds(100) // Rate limiting
        };
        
        await foreach (var person in _peopleService.StreamAsync(options: options, cancellationToken: cancellationToken))
        {
            await processor(person);
            
            // Force garbage collection periodically for long-running operations
            if (GC.GetTotalMemory(false) > 100 * 1024 * 1024) // 100MB threshold
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}
```

### 2. **Caching Strategy**
```csharp
public class CachedPaginationService
{
    private readonly IPeopleService _peopleService;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(5);
    
    public async Task<IPagedResponse<Core.Person>> GetCachedPageAsync(
        QueryParameters parameters,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"people_page_{parameters.GetHashCode()}_{page}_{pageSize}";
        
        if (_cache.TryGetValue(cacheKey, out IPagedResponse<Core.Person> cachedPage))
        {
            return cachedPage;
        }
        
        var pageParams = PaginationHelper.CreatePageParameters(page, pageSize, parameters);
        var result = await _peopleService.ListAsync(pageParams, cancellationToken);
        
        _cache.Set(cacheKey, result, _cacheExpiry);
        return result;
    }
}
```

## Error Handling and Resilience

### 1. **Retry Logic for Pagination**
```csharp
public class ResilientPaginationService
{
    private readonly IPeopleService _peopleService;
    private readonly ILogger<ResilientPaginationService> _logger;
    
    public async Task<IReadOnlyList<Core.Person>> GetAllWithRetryAsync(
        QueryParameters parameters = null,
        int maxRetries = 3,
        CancellationToken cancellationToken = default)
    {
        var allPeople = new List<Core.Person>();
        var page = 1;
        const int pageSize = 100;
        
        while (true)
        {
            var pageParams = PaginationHelper.CreatePageParameters(page, pageSize, parameters);
            IPagedResponse<Core.Person> response = null;
            
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    response = await _peopleService.ListAsync(pageParams, cancellationToken);
                    break; // Success, exit retry loop
                }
                catch (Exception ex) when (attempt < maxRetries && IsRetryableException(ex))
                {
                    _logger.LogWarning(ex, "Attempt {Attempt} failed for page {Page}, retrying...", attempt, page);
                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)), cancellationToken);
                }
            }
            
            if (response == null)
            {
                throw new InvalidOperationException($"Failed to fetch page {page} after {maxRetries} attempts");
            }
            
            allPeople.AddRange(response.Data);
            
            if (!response.HasNextPage)
                break;
            
            page++;
        }
        
        return allPeople.AsReadOnly();
    }
    
    private static bool IsRetryableException(Exception ex)
    {
        return ex is HttpRequestException ||
               ex is TaskCanceledException ||
               (ex is PlanningCenterApiException apiEx && apiEx.StatusCode >= 500);
    }
}
```

## Usage Examples

### 1. **Basic Pagination**
```csharp
// Get first page
var firstPage = await peopleService.ListAsync(new QueryParameters { PerPage = 50 });

// Navigate to next page
if (firstPage.HasNextPage)
{
    var nextPage = await firstPage.GetNextPageAsync();
}

// Get all items automatically
var allPeople = await firstPage.GetAllPagesAsync();
```

### 2. **Memory-Efficient Streaming**
```csharp
// Process large datasets without loading everything into memory
await foreach (var person in peopleService.StreamAsync())
{
    await ProcessPersonAsync(person);
}
```

### 3. **Fluent API Pagination**
```csharp
// Get specific page with fluent API
var page2 = await client
    .People()
    .Where(p => p.Status == "active")
    .GetPageAsync(page: 2, pageSize: 100);

// Stream with custom options
var options = new PaginationOptions
{
    PageSize = 50,
    MaxItems = 1000,
    DelayBetweenPages = TimeSpan.FromMilliseconds(100)
};

await foreach (var person in client
    .People()
    .Where(p => p.Status == "active")
    .AsAsyncEnumerable(options))
{
    // Process each person
}
```

### 4. **Advanced Pagination Scenarios**
```csharp
// Find first person matching criteria across all pages
var targetPerson = await firstPage.FindFirstAcrossPagesAsync(
    p => p.Email.Contains("@example.com"));

// Count all active people across all pages
var activeCount = await firstPage.CountAcrossPagesAsync(
    p => p.Status == "active");

// Process pages in batches
await firstPage.ProcessAllPagesAsync(async page =>
{
    // Process each page as a batch
    await ProcessBatchAsync(page);
});
```

## Best Practices

### 1. **Page Size Selection**
- **Small datasets (< 1,000 items)**: Use page sizes of 100-250
- **Medium datasets (1,000-10,000 items)**: Use page sizes of 50-100
- **Large datasets (> 10,000 items)**: Use page sizes of 25-50
- **Memory-constrained environments**: Use page sizes of 10-25

### 2. **Performance Optimization**
- Use `StreamAsync()` for large datasets to avoid memory issues
- Implement caching for frequently accessed pages
- Add delays between requests to respect rate limits
- Use cancellation tokens for long-running operations

### 3. **Error Handling**
- Always handle `OperationCanceledException` for cancellable operations
- Implement retry logic for transient failures
- Log pagination progress for monitoring
- Validate pagination parameters before making requests

### 4. **Memory Management**
- Dispose of large collections when no longer needed
- Use streaming for processing large datasets
- Monitor memory usage in long-running pagination operations
- Consider implementing backpressure for high-throughput scenarios

This comprehensive pagination strategy ensures that developers can efficiently work with large datasets from the Planning Center API without having to implement complex pagination logic themselves.