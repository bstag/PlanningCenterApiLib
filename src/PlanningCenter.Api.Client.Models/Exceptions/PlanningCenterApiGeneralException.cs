namespace PlanningCenter.Api.Client.Models.Exceptions;

/// <summary>
/// General exception for Planning Center API errors that don't fit into specific categories.
/// </summary>
public class PlanningCenterApiGeneralException : PlanningCenterApiException
{
    public PlanningCenterApiGeneralException(
        string message, 
        int? statusCode = null, 
        string? errorCode = null,
        string? requestId = null,
        string? requestUrl = null,
        string? requestMethod = null,
        string? rawResponse = null,
        Exception? innerException = null) 
        : base(message, statusCode, errorCode, requestId, requestUrl, requestMethod, rawResponse, innerException)
    {
    }
}