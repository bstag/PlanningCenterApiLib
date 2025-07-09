namespace PlanningCenter.Api.Client.Models.Exceptions;

/// <summary>
/// Exception thrown when the Planning Center API returns a server error (HTTP 5xx).
/// </summary>
public class PlanningCenterApiServerException : PlanningCenterApiException
{
    /// <summary>
    /// Whether this error is likely to be transient and worth retrying
    /// </summary>
    public bool IsTransient { get; }
    
    public PlanningCenterApiServerException(
        string message = "Server error occurred", 
        int statusCode = 500,
        bool isTransient = true,
        string? requestId = null,
        string? requestUrl = null,
        string? requestMethod = null,
        string? rawResponse = null,
        Exception? innerException = null)
        : base(message, statusCode, "server_error", requestId, requestUrl, requestMethod, rawResponse, innerException)
    {
        IsTransient = isTransient;
    }
    
    /// <summary>
    /// Creates a server exception for a 500 Internal Server Error
    /// </summary>
    public static PlanningCenterApiServerException InternalServerError(
        string? requestId = null,
        string? requestUrl = null,
        string? requestMethod = null,
        string? rawResponse = null)
    {
        return new PlanningCenterApiServerException(
            "Internal server error occurred",
            500,
            true,
            requestId,
            requestUrl,
            requestMethod,
            rawResponse);
    }
    
    /// <summary>
    /// Creates a server exception for a 502 Bad Gateway
    /// </summary>
    public static PlanningCenterApiServerException BadGateway(
        string? requestId = null,
        string? requestUrl = null,
        string? requestMethod = null,
        string? rawResponse = null)
    {
        return new PlanningCenterApiServerException(
            "Bad gateway error",
            502,
            true,
            requestId,
            requestUrl,
            requestMethod,
            rawResponse);
    }
    
    /// <summary>
    /// Creates a server exception for a 503 Service Unavailable
    /// </summary>
    public static PlanningCenterApiServerException ServiceUnavailable(
        string? requestId = null,
        string? requestUrl = null,
        string? requestMethod = null,
        string? rawResponse = null)
    {
        return new PlanningCenterApiServerException(
            "Service temporarily unavailable",
            503,
            true,
            requestId,
            requestUrl,
            requestMethod,
            rawResponse);
    }
    
    /// <summary>
    /// Creates a server exception for a 504 Gateway Timeout
    /// </summary>
    public static PlanningCenterApiServerException GatewayTimeout(
        string? requestId = null,
        string? requestUrl = null,
        string? requestMethod = null,
        string? rawResponse = null)
    {
        return new PlanningCenterApiServerException(
            "Gateway timeout",
            504,
            true,
            requestId,
            requestUrl,
            requestMethod,
            rawResponse);
    }
}