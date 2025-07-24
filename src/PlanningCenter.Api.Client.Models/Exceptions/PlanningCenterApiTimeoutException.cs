using System.Net;

namespace PlanningCenter.Api.Client.Models.Exceptions;

/// <summary>
/// Exception thrown when an API request times out.
/// </summary>
public class PlanningCenterApiTimeoutException : PlanningCenterApiException
{
    /// <summary>
    /// The timeout duration that was exceeded.
    /// </summary>
    public TimeSpan Timeout { get; }
    
    /// <summary>
    /// The type of timeout that occurred.
    /// </summary>
    public TimeoutType TimeoutType { get; }
    
    /// <summary>
    /// Initializes a new instance of the PlanningCenterApiTimeoutException class.
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="timeout">The timeout duration that was exceeded</param>
    /// <param name="timeoutType">The type of timeout</param>
    /// <param name="requestId">The request ID from the API response</param>
    /// <param name="requestUrl">The URL that was requested</param>
    /// <param name="requestMethod">The HTTP method used</param>
    /// <param name="innerException">The inner exception</param>
    public PlanningCenterApiTimeoutException(
        string message,
        TimeSpan timeout,
        TimeoutType timeoutType = TimeoutType.Request,
        string? requestId = null,
        string? requestUrl = null,
        string? requestMethod = null,
        Exception? innerException = null)
        : base(message, HttpStatusCode.RequestTimeout, requestId, null, requestUrl, requestMethod, innerException)
    {
        Timeout = timeout;
        TimeoutType = timeoutType;
    }
    
    /// <summary>
    /// Gets a detailed message including timeout information.
    /// </summary>
    public override string DetailedMessage
    {
        get
        {
            var details = new List<string> { base.DetailedMessage };
            
            details.Add($"Timeout: {Timeout.TotalSeconds:F1} seconds");
            details.Add($"Timeout type: {TimeoutType}");
            
            return string.Join(Environment.NewLine, details);
        }
    }
}

/// <summary>
/// Represents the type of timeout that occurred.
/// </summary>
public enum TimeoutType
{
    /// <summary>
    /// The entire request timed out.
    /// </summary>
    Request,
    
    /// <summary>
    /// The connection attempt timed out.
    /// </summary>
    Connection,
    
    /// <summary>
    /// Reading the response timed out.
    /// </summary>
    Read,
    
    /// <summary>
    /// Writing the request timed out.
    /// </summary>
    Write
}