using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

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
        HttpStatusCode? statusCode = null, 
        string? requestId = null,
        string? errorCode = null,
        string? requestUrl = null,
        string? requestMethod = null,
        Exception? innerException = null) 
        : base(message, innerException)
    {
        StatusCode = (int?)statusCode;
        ErrorCode = errorCode;
        RequestId = requestId;
        RequestUrl = requestUrl;
        RequestMethod = requestMethod;
        RawResponse = null;
        Timestamp = DateTime.UtcNow;
        AdditionalData = new Dictionary<string, object>();
    }
    
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
    public virtual string DetailedMessage
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
    /// Creates a specific exception type based on the HTTP status code.
    /// </summary>
    /// <param name="statusCode">The HTTP status code</param>
    /// <param name="message">The error message</param>
    /// <param name="requestId">The request ID</param>
    /// <param name="errorCode">The error code</param>
    /// <param name="requestUrl">The request URL</param>
    /// <param name="requestMethod">The request method</param>
    /// <param name="rawResponse">The raw response content</param>
    /// <param name="innerException">The inner exception</param>
    /// <returns>A specific exception type based on the status code</returns>
    public static PlanningCenterApiException CreateFromHttpStatusCode(
        System.Net.HttpStatusCode statusCode,
        string message,
        string? requestId = null,
        string? errorCode = null,
        string? requestUrl = null,
        string? requestMethod = null,
        string? rawResponse = null,
        Exception? innerException = null)
    {
        return statusCode switch
        {
            System.Net.HttpStatusCode.TooManyRequests => new PlanningCenterApiRateLimitException(message, requestId, requestUrl, requestMethod, innerException: innerException),
            System.Net.HttpStatusCode.RequestTimeout => new PlanningCenterApiTimeoutException(message, TimeSpan.FromSeconds(30), TimeoutType.Request, requestId, requestUrl, requestMethod, innerException),
            System.Net.HttpStatusCode.ServiceUnavailable => new PlanningCenterApiNetworkException(message, NetworkErrorType.Http, null, requestId, requestUrl, requestMethod, innerException),
            _ => new PlanningCenterApiException(message, statusCode, requestId, errorCode, requestUrl, requestMethod, innerException)
        };
    }
    
    /// <summary>
    /// Creates an exception from an HTTP response message.
    /// </summary>
    /// <param name="response">The HTTP response message</param>
    /// <param name="content">The response content</param>
    /// <param name="innerException">The inner exception</param>
    /// <returns>A specific exception type based on the response</returns>
    public static async Task<PlanningCenterApiException> CreateFromHttpResponseAsync(
        System.Net.Http.HttpResponseMessage response,
        string? content = null,
        Exception? innerException = null)
    {
        content ??= await response.Content.ReadAsStringAsync();
        
        var requestId = response.Headers.TryGetValues("X-Request-Id", out var requestIdValues)
            ? requestIdValues.FirstOrDefault()
            : null;
            
        var message = $"API request failed with status {(int)response.StatusCode} ({response.StatusCode})";
        
        // Handle rate limiting with headers
        if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        {
            var headers = response.Headers.ToDictionary(
                h => h.Key,
                h => h.Value,
                StringComparer.OrdinalIgnoreCase);
                
            return PlanningCenterApiRateLimitException.FromHeaders(
                message,
                headers,
                requestId,
                response.RequestMessage?.RequestUri?.ToString(),
                response.RequestMessage?.Method?.Method,
                innerException);
        }
        
        return CreateFromHttpStatusCode(
            response.StatusCode,
            message,
            requestId,
            null,
            response.RequestMessage?.RequestUri?.ToString(),
            response.RequestMessage?.Method?.Method,
            content,
            innerException);
    }
    
    /// <summary>
    /// Creates a network exception from a network-related exception.
    /// </summary>
    /// <param name="exception">The network exception</param>
    /// <param name="requestUrl">The request URL</param>
    /// <param name="requestMethod">The request method</param>
    /// <returns>A PlanningCenterApiNetworkException</returns>
    public static PlanningCenterApiNetworkException CreateNetworkException(
        Exception exception,
        string? requestUrl = null,
        string? requestMethod = null)
    {
        return PlanningCenterApiNetworkException.FromNetworkException(exception, requestUrl, requestMethod);
    }
    
    /// <summary>
    /// Creates a timeout exception.
    /// </summary>
    /// <param name="timeout">The timeout duration</param>
    /// <param name="timeoutType">The type of timeout</param>
    /// <param name="requestUrl">The request URL</param>
    /// <param name="requestMethod">The request method</param>
    /// <param name="innerException">The inner exception</param>
    /// <returns>A PlanningCenterApiTimeoutException</returns>
    public static PlanningCenterApiTimeoutException CreateTimeoutException(
        TimeSpan timeout,
        TimeoutType timeoutType = TimeoutType.Request,
        string? requestUrl = null,
        string? requestMethod = null,
        Exception? innerException = null)
    {
        var message = $"Request timed out after {timeout.TotalSeconds:F1} seconds ({timeoutType})";
        return new PlanningCenterApiTimeoutException(message, timeout, timeoutType, null, requestUrl, requestMethod, innerException);
    }
}