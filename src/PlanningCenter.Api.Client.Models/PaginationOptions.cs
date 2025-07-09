namespace PlanningCenter.Api.Client.Models;

/// <summary>
/// Configuration options for pagination behavior.
/// Allows fine-tuning of pagination performance, memory usage, and rate limiting.
/// </summary>
public class PaginationOptions
{
    /// <summary>
    /// Page size to use for pagination requests.
    /// If null, the SDK will calculate an optimal page size based on the estimated total count.
    /// Larger page sizes reduce the number of HTTP requests but use more memory.
    /// </summary>
    public int? PageSize { get; set; }
    
    /// <summary>
    /// Maximum number of items to fetch across all pages.
    /// If null, all available items will be fetched.
    /// Useful for limiting memory usage or processing time for large datasets.
    /// </summary>
    public int? MaxItems { get; set; }
    
    /// <summary>
    /// Estimated total count of items for optimization purposes.
    /// Used to calculate optimal page sizes and memory allocation.
    /// If not provided, defaults to 1000.
    /// </summary>
    public int EstimatedTotalCount { get; set; } = 1000;
    
    /// <summary>
    /// Delay to add between page requests to avoid rate limiting.
    /// Useful when processing large datasets to be respectful of API rate limits.
    /// </summary>
    public TimeSpan? DelayBetweenPages { get; set; }
    
    /// <summary>
    /// Whether to prefetch the next page in the background for better performance.
    /// This can improve perceived performance but uses more memory and API calls.
    /// </summary>
    public bool PrefetchNextPage { get; set; } = false;
    
    /// <summary>
    /// Creates pagination options optimized for memory efficiency.
    /// Uses smaller page sizes and includes rate limiting delays.
    /// </summary>
    /// <param name="maxMemoryMB">Maximum memory to use in megabytes</param>
    /// <returns>Pagination options optimized for memory efficiency</returns>
    public static PaginationOptions ForMemoryEfficiency(int maxMemoryMB = 50)
    {
        return new PaginationOptions
        {
            PageSize = 25, // Small page size
            DelayBetweenPages = TimeSpan.FromMilliseconds(100),
            PrefetchNextPage = false
        };
    }
    
    /// <summary>
    /// Creates pagination options optimized for speed.
    /// Uses larger page sizes and prefetching for faster data retrieval.
    /// </summary>
    /// <returns>Pagination options optimized for speed</returns>
    public static PaginationOptions ForSpeed()
    {
        return new PaginationOptions
        {
            PageSize = 100, // Larger page size
            DelayBetweenPages = null,
            PrefetchNextPage = true
        };
    }
    
    /// <summary>
    /// Creates pagination options optimized for large datasets.
    /// Balances memory usage and performance for processing large amounts of data.
    /// </summary>
    /// <param name="estimatedTotal">Estimated total number of items</param>
    /// <returns>Pagination options optimized for large datasets</returns>
    public static PaginationOptions ForLargeDataset(int estimatedTotal)
    {
        return new PaginationOptions
        {
            PageSize = estimatedTotal > 10000 ? 50 : 100,
            EstimatedTotalCount = estimatedTotal,
            DelayBetweenPages = TimeSpan.FromMilliseconds(50),
            PrefetchNextPage = false
        };
    }
}