namespace PlanningCenter.Api.Client.Models.Exceptions;

/// <summary>
/// Exception thrown when API rate limits are exceeded (HTTP 429).
/// </summary>
public class PlanningCenterApiRateLimitException : PlanningCenterApiException
{
    /// <summary>
    /// When the rate limit will reset and requests can be made again
    /// </summary>
    public DateTime? ResetTime { get; }
    
    /// <summary>
    /// How long to wait before retrying
    /// </summary>
    public TimeSpan? RetryAfter { get; }
    
    /// <summary>
    /// The current rate limit (requests per time period)
    /// </summary>
    public int? RateLimit { get; }
    
    /// <summary>
    /// How many requests remain in the current time period
    /// </summary>
    public int? RemainingRequests { get; }
    
    public PlanningCenterApiRateLimitException(
        string message = "Rate limit exceeded", 
        DateTime? resetTime = null,
        TimeSpan? retryAfter = null,
        int? rateLimit = null,
        int? remainingRequests = null,
        string? requestId = null,
        string? requestUrl = null,
        string? requestMethod = null,
        string? rawResponse = null)
        : base(message, 429, "rate_limit_exceeded", requestId, requestUrl, requestMethod, rawResponse)
    {
        ResetTime = resetTime;
        RetryAfter = retryAfter;
        RateLimit = rateLimit;
        RemainingRequests = remainingRequests;
    }
    
    /// <summary>
    /// Creates a rate limit exception from HTTP headers
    /// </summary>
    public static PlanningCenterApiRateLimitException FromHeaders(
        Dictionary<string, string> headers,
        string? requestId = null,
        string? requestUrl = null,
        string? requestMethod = null)
    {
        DateTime? resetTime = null;
        TimeSpan? retryAfter = null;
        int? rateLimit = null;
        int? remainingRequests = null;
        
        // Parse common rate limit headers
        if (headers.TryGetValue("X-RateLimit-Reset", out var resetHeader) && 
            long.TryParse(resetHeader, out var resetUnix))
        {
            resetTime = DateTimeOffset.FromUnixTimeSeconds(resetUnix).DateTime;
        }
        
        if (headers.TryGetValue("Retry-After", out var retryHeader) && 
            int.TryParse(retryHeader, out var retrySeconds))
        {
            retryAfter = TimeSpan.FromSeconds(retrySeconds);
        }
        
        if (headers.TryGetValue("X-RateLimit-Limit", out var limitHeader) && 
            int.TryParse(limitHeader, out var limit))
        {
            rateLimit = limit;
        }
        
        if (headers.TryGetValue("X-RateLimit-Remaining", out var remainingHeader) && 
            int.TryParse(remainingHeader, out var remaining))
        {
            remainingRequests = remaining;
        }
        
        var message = retryAfter.HasValue 
            ? $"Rate limit exceeded. Retry after {retryAfter.Value.TotalSeconds} seconds."
            : "Rate limit exceeded";
        
        return new PlanningCenterApiRateLimitException(
            message, resetTime, retryAfter, rateLimit, remainingRequests, requestId, requestUrl, requestMethod);
    }
}