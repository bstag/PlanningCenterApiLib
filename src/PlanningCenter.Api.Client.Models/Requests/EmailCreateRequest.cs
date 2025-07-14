namespace PlanningCenter.Api.Client.Models.Requests;

/// <summary>
/// Request model for creating a new email address.
/// </summary>
public class EmailCreateRequest
{
    /// <summary>
    /// The email address (required)
    /// </summary>
    public string Address { get; set; } = string.Empty;
    
    /// <summary>
    /// Email type or location (defaults to Home)
    /// </summary>
    public string Location { get; set; } = "Home";
    
    /// <summary>
    /// Whether this should be the primary email address
    /// </summary>
    public bool IsPrimary { get; set; }
    
    /// <summary>
    /// Alias for IsPrimary expected by legacy tests.
    /// </summary>
    public bool Primary { get => IsPrimary; set => IsPrimary = value; }
}

/// <summary>
/// Request model for updating an existing email address.
/// </summary>
public class EmailUpdateRequest
{
    /// <summary>
    /// The email address
    /// </summary>
    public string? Address { get; set; }
    
    /// <summary>
    /// Email type or location
    /// </summary>
    public string? Location { get; set; }
    
    /// <summary>
    /// Whether this should be the primary email address
    /// </summary>
    public bool? IsPrimary { get; set; }
    /// <summary>
    /// Alias for IsPrimary expected by legacy tests.
    /// </summary>
    public bool? Primary { get => IsPrimary; set => IsPrimary = value; }
}