namespace PlanningCenter.Api.Client.Models.Requests;

/// <summary>
/// Request model for creating a signup location.
/// </summary>
public class SignupLocationCreateRequest
{
    /// <summary>
    /// Gets or sets the location name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the location description.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the street address.
    /// </summary>
    public string? StreetAddress { get; set; }
    
    /// <summary>
    /// Gets or sets the city.
    /// </summary>
    public string? City { get; set; }
    
    /// <summary>
    /// Gets or sets the state or province.
    /// </summary>
    public string? State { get; set; }
    
    /// <summary>
    /// Gets or sets the postal code.
    /// </summary>
    public string? PostalCode { get; set; }
    
    /// <summary>
    /// Gets or sets the country.
    /// </summary>
    public string? Country { get; set; }
    
    /// <summary>
    /// Gets or sets the latitude coordinate.
    /// </summary>
    public double? Latitude { get; set; }
    
    /// <summary>
    /// Gets or sets the longitude coordinate.
    /// </summary>
    public double? Longitude { get; set; }
    
    /// <summary>
    /// Gets or sets the location phone number.
    /// </summary>
    public string? PhoneNumber { get; set; }
    
    /// <summary>
    /// Gets or sets the location website URL.
    /// </summary>
    public string? WebsiteUrl { get; set; }
    
    /// <summary>
    /// Gets or sets directions to the location.
    /// </summary>
    public string? Directions { get; set; }
    
    /// <summary>
    /// Gets or sets parking information.
    /// </summary>
    public string? ParkingInfo { get; set; }
    
    /// <summary>
    /// Gets or sets accessibility information.
    /// </summary>
    public string? AccessibilityInfo { get; set; }
    
    /// <summary>
    /// Gets or sets the location capacity.
    /// </summary>
    public int? Capacity { get; set; }
    
    /// <summary>
    /// Gets or sets additional location notes.
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Gets or sets the location timezone.
    /// </summary>
    public string? Timezone { get; set; }
}

/// <summary>
/// Request model for updating a signup location.
/// </summary>
public class SignupLocationUpdateRequest
{
    /// <summary>
    /// Gets or sets the location name.
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Gets or sets the location description.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the street address.
    /// </summary>
    public string? StreetAddress { get; set; }
    
    /// <summary>
    /// Gets or sets the city.
    /// </summary>
    public string? City { get; set; }
    
    /// <summary>
    /// Gets or sets the state or province.
    /// </summary>
    public string? State { get; set; }
    
    /// <summary>
    /// Gets or sets the postal code.
    /// </summary>
    public string? PostalCode { get; set; }
    
    /// <summary>
    /// Gets or sets the country.
    /// </summary>
    public string? Country { get; set; }
    
    /// <summary>
    /// Gets or sets the latitude coordinate.
    /// </summary>
    public double? Latitude { get; set; }
    
    /// <summary>
    /// Gets or sets the longitude coordinate.
    /// </summary>
    public double? Longitude { get; set; }
    
    /// <summary>
    /// Gets or sets the location phone number.
    /// </summary>
    public string? PhoneNumber { get; set; }
    
    /// <summary>
    /// Gets or sets the location website URL.
    /// </summary>
    public string? WebsiteUrl { get; set; }
    
    /// <summary>
    /// Gets or sets directions to the location.
    /// </summary>
    public string? Directions { get; set; }
    
    /// <summary>
    /// Gets or sets parking information.
    /// </summary>
    public string? ParkingInfo { get; set; }
    
    /// <summary>
    /// Gets or sets accessibility information.
    /// </summary>
    public string? AccessibilityInfo { get; set; }
    
    /// <summary>
    /// Gets or sets the location capacity.
    /// </summary>
    public int? Capacity { get; set; }
    
    /// <summary>
    /// Gets or sets additional location notes.
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Gets or sets the location timezone.
    /// </summary>
    public string? Timezone { get; set; }
}