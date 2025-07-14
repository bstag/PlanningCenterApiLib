namespace PlanningCenter.Api.Client.Models.Requests;

/// <summary>
/// Request model for creating a new address.
/// </summary>
public class AddressCreateRequest
{
    /// <summary>
    /// Street address line 1 (required)
    /// </summary>
    public string Street { get; set; } = string.Empty;
    
    /// <summary>
    /// Street address line 2
    /// </summary>
    public string? Street2 { get; set; }
    
    /// <summary>
    /// City name (required)
    /// </summary>
    public string City { get; set; } = string.Empty;
    
    /// <summary>
    /// State or province (required)
    /// </summary>
    public string State { get; set; } = string.Empty;
    
    /// <summary>
    /// ZIP or postal code (required)
    /// </summary>
    public string Zip { get; set; } = string.Empty;
    
    /// <summary>
    /// Country (defaults to US)
    /// </summary>
    public string Country { get; set; } = "US";
    
    /// <summary>
    /// Address type or location (defaults to Home)
    /// </summary>
    public string Location { get; set; } = "Home";
    
    /// <summary>
    /// Whether this should be the primary address
    /// </summary>
    public bool IsPrimary { get; set; }
    
    /// <summary>
    /// Alias for IsPrimary expected by legacy tests.
    /// </summary>
    public bool Primary { get => IsPrimary; set => IsPrimary = value; }
}

/// <summary>
/// Request model for updating an existing address.
/// </summary>
public class AddressUpdateRequest
{
    /// <summary>
    /// Street address line 1
    /// </summary>
    public string? Street { get; set; }
    
    /// <summary>
    /// Street address line 2
    /// </summary>
    public string? Street2 { get; set; }
    
    /// <summary>
    /// City name
    /// </summary>
    public string? City { get; set; }
    
    /// <summary>
    /// State or province
    /// </summary>
    public string? State { get; set; }
    
    /// <summary>
    /// ZIP or postal code
    /// </summary>
    public string? Zip { get; set; }
    
    /// <summary>
    /// Country
    /// </summary>
    public string? Country { get; set; }
    
    /// <summary>
    /// Address type or location
    /// </summary>
    public string? Location { get; set; }
    
    /// <summary>
    /// Whether this should be the primary address
    /// </summary>
    public bool? IsPrimary { get; set; }
    
    /// <summary>
    /// Alias for IsPrimary expected by legacy tests.
    /// </summary>
    public bool? Primary { get => IsPrimary; set => IsPrimary = value; }
}