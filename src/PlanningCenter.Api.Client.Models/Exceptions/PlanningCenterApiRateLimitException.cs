using System.Net;

namespace PlanningCenter.Api.Client.Models.Exceptions;

/// <summary>
/// Exception thrown when the API rate limit is exceeded.
/// </summary>
public class PlanningCenterApiRateLimitException : PlanningCenterApiException
{
    /// <summary>
    /// The number of seconds to wait before retrying the request.
    /// </summary>
    public int? RetryAfterSeconds { get; }
    
    /// <summary>
    /// The current rate limit for the API.
    /// </summary>
    public int? RateLimit { get; }
    
    /// <summary>
    /// The number of requests remaining in the current rate limit window.
    /// </summary>
    public int? RateLimitRemaining { get; }
    
    /// <summary>
    /// The time when the rate limit window resets.
    /// </summary>
    public DateTimeOffset? RateLimitReset { get; }
    
    /// <summary>
    /// Initializes a new instance of the PlanningCenterApiRateLimitException class.
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="requestId">The request ID from the API response</param>
    /// <param name="requestUrl">The URL that was requested</param>
    /// <param name="requestMethod">The HTTP method used</param>
    /// <param name="retryAfterSeconds">The number of seconds to wait before retrying</param>
    /// <param name="rateLimit">The current rate limit</param>
    /// <param name="rateLimitRemaining">The number of requests remaining</param>
    /// <param name="rateLimitReset">The time when the rate limit resets</param>
    /// <param name="innerException">The inner exception</param>
    public PlanningCenterApiRateLimitException(
        string message,
        string? requestId = null,
        string? requestUrl = null,
        string? requestMethod = null,
        int? retryAfterSeconds = null,
        int? rateLimit = null,
        int? rateLimitRemaining = null,
        DateTimeOffset? rateLimitReset = null,
        Exception? innerException = null)
        : base(message, HttpStatusCode.TooManyRequests, requestId, null, requestUrl, requestMethod, innerException)
    {
        RetryAfterSeconds = retryAfterSeconds;
        RateLimit = rateLimit;
        RateLimitRemaining = rateLimitRemaining;
        RateLimitReset = rateLimitReset;
    }
    
    /// <summary>
    /// Gets a detailed message including rate limit information.
    /// </summary>
    public override string DetailedMessage
    {
        get
        {
            var details = new List<string> { base.DetailedMessage };
            
            if (RetryAfterSeconds.HasValue)
                details.Add($"Retry after: {RetryAfterSeconds.Value} seconds");
                
            if (RateLimit.HasValue)
                details.Add($"Rate limit: {RateLimit.Value} requests");
                
            if (RateLimitRemaining.HasValue)
                details.Add($"Remaining: {RateLimitRemaining.Value} requests");
                
            if (RateLimitReset.HasValue)
                details.Add($"Reset time: {RateLimitReset.Value:yyyy-MM-dd HH:mm:ss UTC}");
                
            return string.Join(Environment.NewLine, details);
        }
    }
    
    /// <summary>
    /// Creates a PlanningCenterApiRateLimitException from HTTP response headers.
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="headers">The HTTP response headers</param>
    /// <param name="requestId">The request ID</param>
    /// <param name="requestUrl">The request URL</param>
    /// <param name="requestMethod">The request method</param>
    /// <param name="innerException">The inner exception</param>
    /// <returns>A new PlanningCenterApiRateLimitException instance</returns>
    public static PlanningCenterApiRateLimitException FromHeaders(
        string message,
        IDictionary<string, IEnumerable<string>> headers,
        string? requestId = null,
        string? requestUrl = null,
        string? requestMethod = null,
        Exception? innerException = null)
    {
        int? retryAfter = null;
        int? rateLimit = null;
        int? rateLimitRemaining = null;
        DateTimeOffset? rateLimitReset = null;
        
        // Parse Retry-After header
        if (headers.TryGetValue("Retry-After", out var retryAfterValues))
        {
            var retryAfterValue = retryAfterValues.FirstOrDefault();
            if (int.TryParse(retryAfterValue, out var retryAfterInt))
            {
                retryAfter = retryAfterInt;
            }
        }
        
        // Parse X-RateLimit-Limit header
        if (headers.TryGetValue("X-RateLimit-Limit", out var rateLimitValues))
        {
            var rateLimitValue = rateLimitValues.FirstOrDefault();
            if (int.TryParse(rateLimitValue, out var rateLimitInt))
            {
                rateLimit = rateLimitInt;
            }
        }
        
        // Parse X-RateLimit-Remaining header
        if (headers.TryGetValue("X-RateLimit-Remaining", out var rateLimitRemainingValues))
        {
            var rateLimitRemainingValue = rateLimitRemainingValues.FirstOrDefault();
            if (int.TryParse(rateLimitRemainingValue, out var rateLimitRemainingInt))
            {
                rateLimitRemaining = rateLimitRemainingInt;
            }
        }
        
        // Parse X-RateLimit-Reset header
        if (headers.TryGetValue("X-RateLimit-Reset", out var rateLimitResetValues))
        {
            var rateLimitResetValue = rateLimitResetValues.FirstOrDefault();
            if (long.TryParse(rateLimitResetValue, out var rateLimitResetLong))
            {
                rateLimitReset = DateTimeOffset.FromUnixTimeSeconds(rateLimitResetLong);
            }
        }
        
        return new PlanningCenterApiRateLimitException(
            message,
            requestId,
            requestUrl,
            requestMethod,
            retryAfter,
            rateLimit,
            rateLimitRemaining,
            rateLimitReset,
            innerException);
    }
}