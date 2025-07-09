namespace PlanningCenter.Api.Client.Models.Requests;

/// <summary>
/// Request model for creating a new phone number.
/// </summary>
public class PhoneNumberCreateRequest
{
    /// <summary>
    /// The phone number (required)
    /// </summary>
    public string Number { get; set; } = string.Empty;
    
    /// <summary>
    /// Phone number type or location (defaults to Mobile)
    /// </summary>
    public string Location { get; set; } = "Mobile";
    
    /// <summary>
    /// Whether this should be the primary phone number
    /// </summary>
    public bool IsPrimary { get; set; }
    
    /// <summary>
    /// Whether this phone number can receive SMS messages
    /// </summary>
    public bool CanReceiveSms { get; set; } = true;
    
    /// <summary>
    /// Country code for international numbers
    /// </summary>
    public string? CountryCode { get; set; }
}

/// <summary>
/// Request model for updating an existing phone number.
/// </summary>
public class PhoneNumberUpdateRequest
{
    /// <summary>
    /// The phone number
    /// </summary>
    public string? Number { get; set; }
    
    /// <summary>
    /// Phone number type or location
    /// </summary>
    public string? Location { get; set; }
    
    /// <summary>
    /// Whether this should be the primary phone number
    /// </summary>
    public bool? IsPrimary { get; set; }
    
    /// <summary>
    /// Whether this phone number can receive SMS messages
    /// </summary>
    public bool? CanReceiveSms { get; set; }
    
    /// <summary>
    /// Country code for international numbers
    /// </summary>
    public string? CountryCode { get; set; }
}