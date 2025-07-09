namespace PlanningCenter.Api.Client.Models.Exceptions;

/// <summary>
/// Exception thrown when request validation fails (HTTP 400, 422).
/// </summary>
public class PlanningCenterApiValidationException : PlanningCenterApiException
{
    /// <summary>
    /// Validation errors by field name
    /// </summary>
    public Dictionary<string, List<string>> ValidationErrors { get; }
    
    public PlanningCenterApiValidationException(
        string message = "Validation failed", 
        Dictionary<string, List<string>>? validationErrors = null,
        int statusCode = 400,
        string? requestId = null,
        string? requestUrl = null,
        string? requestMethod = null,
        string? rawResponse = null)
        : base(message, statusCode, "validation_failed", requestId, requestUrl, requestMethod, rawResponse)
    {
        ValidationErrors = validationErrors ?? new Dictionary<string, List<string>>();
    }
    
    /// <summary>
    /// Gets all validation error messages as a flat list
    /// </summary>
    public List<string> AllErrorMessages
    {
        get
        {
            var messages = new List<string>();
            foreach (var (field, errors) in ValidationErrors)
            {
                foreach (var error in errors)
                {
                    messages.Add($"{field}: {error}");
                }
            }
            return messages;
        }
    }
    
    /// <summary>
    /// Gets a formatted string of all validation errors
    /// </summary>
    public string FormattedErrors => string.Join("; ", AllErrorMessages);
    
    /// <summary>
    /// Adds a validation error for a specific field
    /// </summary>
    public void AddError(string field, string error)
    {
        if (!ValidationErrors.ContainsKey(field))
        {
            ValidationErrors[field] = new List<string>();
        }
        ValidationErrors[field].Add(error);
    }
    
    /// <summary>
    /// Checks if there are validation errors for a specific field
    /// </summary>
    public bool HasErrorsForField(string field)
    {
        return ValidationErrors.ContainsKey(field) && ValidationErrors[field].Any();
    }
    
    /// <summary>
    /// Gets validation errors for a specific field
    /// </summary>
    public List<string> GetErrorsForField(string field)
    {
        return ValidationErrors.TryGetValue(field, out var errors) ? errors : new List<string>();
    }
}