namespace PlanningCenter.Api.Client.Models.Exceptions;

/// <summary>
/// Base exception for all Planning Center API-related errors.
/// Provides comprehensive error information for debugging and monitoring.
/// </summary>
public class PlanningCenterApiException : Exception
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
    
    public PlanningCenterApiException() : this("An error occurred while communicating with the Planning Center API") { }
    
    public PlanningCenterApiException(string message) : this(message, null, null, null, null, null, null, null) { }
    
    public PlanningCenterApiException(string message, Exception? innerException) : this(message, null, null, null, null, null, null, innerException) { }
    
    public PlanningCenterApiException(
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
    
    /// <summary>
    /// Creates an appropriate exception instance based on the HTTP status code
    /// </summary>
    /// <param name="statusCode">The HTTP status code</param>
    /// <param name="message">The error message</param>
    /// <param name="endpoint">The API endpoint that was called</param>
    /// <param name="retryAfter">Optional retry after timespan for rate limit exceptions</param>
    /// <returns>An appropriate exception instance</returns>
    public static PlanningCenterApiException CreateFromHttpStatusCode(
        System.Net.HttpStatusCode statusCode,
        string message,
        string endpoint,
        TimeSpan? retryAfter = null)
    {
        var fullMessage = $"{message} (Endpoint: {endpoint})";
        
        return statusCode switch
        {
            System.Net.HttpStatusCode.Unauthorized => new PlanningCenterApiAuthenticationException(fullMessage),
            System.Net.HttpStatusCode.Forbidden => new PlanningCenterApiAuthorizationException(fullMessage),
            System.Net.HttpStatusCode.NotFound => new PlanningCenterApiNotFoundException(fullMessage),
            System.Net.HttpStatusCode.BadRequest => new PlanningCenterApiValidationException(fullMessage),
            System.Net.HttpStatusCode.UnprocessableEntity => new PlanningCenterApiValidationException(fullMessage),
            System.Net.HttpStatusCode.TooManyRequests => new PlanningCenterApiRateLimitException(fullMessage, null, retryAfter),
            System.Net.HttpStatusCode.InternalServerError => new PlanningCenterApiServerException(fullMessage),
            System.Net.HttpStatusCode.BadGateway => new PlanningCenterApiServerException(fullMessage),
            System.Net.HttpStatusCode.ServiceUnavailable => new PlanningCenterApiServerException(fullMessage),
            System.Net.HttpStatusCode.GatewayTimeout => new PlanningCenterApiServerException(fullMessage),
            _ => new PlanningCenterApiGeneralException($"{fullMessage} (Status: {statusCode})")
        };
    }
}