namespace PlanningCenter.Api.Client.Models.Exceptions;

/// <summary>
/// Base exception for all Planning Center API-related errors.
/// Provides comprehensive error information for debugging and monitoring.
/// </summary>
public abstract class PlanningCenterApiException : Exception
{
    /// <summary>
    /// HTTP status code from the API response
    /// </summary>
    public int? StatusCode { get; }
    
    /// <summary>
    /// Unique request identifier for tracking and debugging
    /// </summary>
    public string? RequestId { get; }
    
    /// <summary>
    /// Planning Center specific error code
    /// </summary>
    public string? ErrorCode { get; }
    
    /// <summary>
    /// The URL that was requested when the error occurred
    /// </summary>
    public string? RequestUrl { get; }
    
    /// <summary>
    /// The HTTP method that was used when the error occurred
    /// </summary>
    public string? RequestMethod { get; }
    
    /// <summary>
    /// When the error occurred
    /// </summary>
    public DateTime Timestamp { get; }
    
    /// <summary>
    /// Additional error data from the API response
    /// </summary>
    public Dictionary<string, object> AdditionalData { get; }
    
    /// <summary>
    /// Raw response content for debugging
    /// </summary>
    public string? RawResponse { get; }
    
    protected PlanningCenterApiException(
        string message, 
        int? statusCode = null, 
        string? errorCode = null,
        string? requestId = null,
        string? requestUrl = null,
        string? requestMethod = null,
        string? rawResponse = null,
        Exception? innerException = null) 
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
        RequestId = requestId;
        RequestUrl = requestUrl;
        RequestMethod = requestMethod;
        RawResponse = rawResponse;
        Timestamp = DateTime.UtcNow;
        AdditionalData = new Dictionary<string, object>();
    }
    
    /// <summary>
    /// Gets a detailed error message including all available context
    /// </summary>
    public string DetailedMessage
    {
        get
        {
            var details = new List<string> { Message };
            
            if (StatusCode.HasValue)
                details.Add($"Status Code: {StatusCode}");
            
            if (!string.IsNullOrEmpty(ErrorCode))
                details.Add($"Error Code: {ErrorCode}");
            
            if (!string.IsNullOrEmpty(RequestId))
                details.Add($"Request ID: {RequestId}");
            
            if (!string.IsNullOrEmpty(RequestUrl))
                details.Add($"URL: {RequestMethod} {RequestUrl}");
            
            details.Add($"Timestamp: {Timestamp:yyyy-MM-dd HH:mm:ss} UTC");
            
            return string.Join(" | ", details);
        }
    }
}