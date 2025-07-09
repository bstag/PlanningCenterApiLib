namespace PlanningCenter.Api.Client.Models.Exceptions;

/// <summary>
/// Exception thrown when a requested resource is not found (HTTP 404).
/// </summary>
public class PlanningCenterApiNotFoundException : PlanningCenterApiException
{
    /// <summary>
    /// The type of resource that was not found (e.g., "Person", "Event")
    /// </summary>
    public string ResourceType { get; }
    
    /// <summary>
    /// The ID of the resource that was not found
    /// </summary>
    public string ResourceId { get; }
    
    public PlanningCenterApiNotFoundException(
        string resourceType, 
        string resourceId, 
        string? requestId = null,
        string? requestUrl = null,
        string? rawResponse = null) 
        : base($"{resourceType} with ID '{resourceId}' was not found", 404, "not_found", requestId, requestUrl, "GET", rawResponse)
    {
        ResourceType = resourceType;
        ResourceId = resourceId;
    }
    
    public PlanningCenterApiNotFoundException(
        string message,
        string resourceType,
        string resourceId,
        string? requestId = null,
        string? requestUrl = null,
        string? rawResponse = null)
        : base(message, 404, "not_found", requestId, requestUrl, "GET", rawResponse)
    {
        ResourceType = resourceType;
        ResourceId = resourceId;
    }
}