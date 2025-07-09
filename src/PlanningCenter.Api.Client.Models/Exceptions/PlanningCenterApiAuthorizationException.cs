namespace PlanningCenter.Api.Client.Models.Exceptions;

/// <summary>
/// Exception thrown when the user is authenticated but not authorized to access a resource (HTTP 403).
/// </summary>
public class PlanningCenterApiAuthorizationException : PlanningCenterApiException
{
    /// <summary>
    /// The required permission or scope that is missing
    /// </summary>
    public string? RequiredPermission { get; }
    
    /// <summary>
    /// The resource that access was denied to
    /// </summary>
    public string? Resource { get; }
    
    public PlanningCenterApiAuthorizationException(
        string message = "Access denied", 
        string? requiredPermission = null,
        string? resource = null,
        string? requestId = null,
        string? requestUrl = null,
        string? requestMethod = null,
        string? rawResponse = null)
        : base(message, 403, "access_denied", requestId, requestUrl, requestMethod, rawResponse)
    {
        RequiredPermission = requiredPermission;
        Resource = resource;
    }
    
    /// <summary>
    /// Creates an exception for insufficient permissions
    /// </summary>
    public static PlanningCenterApiAuthorizationException InsufficientPermissions(
        string resource,
        string requiredPermission,
        string? requestId = null,
        string? requestUrl = null,
        string? requestMethod = null)
    {
        return new PlanningCenterApiAuthorizationException(
            $"Insufficient permissions to access {resource}. Required permission: {requiredPermission}",
            requiredPermission,
            resource,
            requestId,
            requestUrl,
            requestMethod);
    }
}