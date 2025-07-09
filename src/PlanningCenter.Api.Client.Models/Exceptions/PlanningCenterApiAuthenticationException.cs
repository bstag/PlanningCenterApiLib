namespace PlanningCenter.Api.Client.Models.Exceptions;

/// <summary>
/// Exception thrown when authentication fails (HTTP 401).
/// </summary>
public class PlanningCenterApiAuthenticationException : PlanningCenterApiException
{
    /// <summary>
    /// The authentication scheme that failed (e.g., "Bearer", "OAuth")
    /// </summary>
    public string? AuthenticationScheme { get; }
    
    /// <summary>
    /// Whether the token has expired
    /// </summary>
    public bool IsTokenExpired { get; }
    
    public PlanningCenterApiAuthenticationException(
        string message = "Authentication failed", 
        string? authenticationScheme = null,
        bool isTokenExpired = false,
        string? requestId = null,
        string? requestUrl = null,
        string? requestMethod = null,
        string? rawResponse = null,
        Exception? innerException = null)
        : base(message, 401, "authentication_failed", requestId, requestUrl, requestMethod, rawResponse, innerException)
    {
        AuthenticationScheme = authenticationScheme;
        IsTokenExpired = isTokenExpired;
    }
    
    /// <summary>
    /// Creates an exception for an expired token
    /// </summary>
    public static PlanningCenterApiAuthenticationException TokenExpired(
        string? requestId = null,
        string? requestUrl = null,
        string? requestMethod = null)
    {
        return new PlanningCenterApiAuthenticationException(
            "Access token has expired",
            "Bearer",
            true,
            requestId,
            requestUrl,
            requestMethod);
    }
    
    /// <summary>
    /// Creates an exception for invalid credentials
    /// </summary>
    public static PlanningCenterApiAuthenticationException InvalidCredentials(
        string? requestId = null,
        string? requestUrl = null,
        string? requestMethod = null)
    {
        return new PlanningCenterApiAuthenticationException(
            "Invalid credentials provided",
            "OAuth",
            false,
            requestId,
            requestUrl,
            requestMethod);
    }
}