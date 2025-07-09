namespace PlanningCenter.Api.Client.Models.Core;

/// <summary>
/// Represents a physical address for a person or organization.
/// </summary>
public class Address
{
    /// <summary>
    /// Unique identifier for the address
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Street address line 1
    /// </summary>
    public string Street { get; set; } = string.Empty;
    
    /// <summary>
    /// Street address line 2 (apartment, suite, etc.)
    /// </summary>
    public string? Street2 { get; set; }
    
    /// <summary>
    /// City name
    /// </summary>
    public string City { get; set; } = string.Empty;
    
    /// <summary>
    /// State or province
    /// </summary>
    public string State { get; set; } = string.Empty;
    
    /// <summary>
    /// ZIP or postal code
    /// </summary>
    public string Zip { get; set; } = string.Empty;
    
    /// <summary>
    /// Country (defaults to US)
    /// </summary>
    public string Country { get; set; } = "US";
    
    /// <summary>
    /// Address type or location (Home, Work, etc.)
    /// </summary>
    public string Location { get; set; } = "Home";
    
    /// <summary>
    /// Whether this is the primary address
    /// </summary>
    public bool IsPrimary { get; set; }
    
    /// <summary>
    /// When the address was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// When the address was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
    
    /// <summary>
    /// Gets the full address as a formatted string
    /// </summary>
    public string FullAddress
    {
        get
        {
            var parts = new List<string>();
            
            if (!string.IsNullOrWhiteSpace(Street))
                parts.Add(Street);
            
            if (!string.IsNullOrWhiteSpace(Street2))
                parts.Add(Street2);
            
            var cityStateZip = new List<string>();
            if (!string.IsNullOrWhiteSpace(City))
                cityStateZip.Add(City);
            if (!string.IsNullOrWhiteSpace(State))
                cityStateZip.Add(State);
            if (!string.IsNullOrWhiteSpace(Zip))
                cityStateZip.Add(Zip);
            
            if (cityStateZip.Any())
                parts.Add(string.Join(", ", cityStateZip.Take(2)) + (cityStateZip.Count > 2 ? " " + cityStateZip.Last() : ""));
            
            if (!string.IsNullOrWhiteSpace(Country) && Country != "US")
                parts.Add(Country);
            
            return string.Join("\n", parts);
        }
    }
    
    /// <summary>
    /// Gets a single-line version of the address
    /// </summary>
    public string SingleLineAddress
    {
        get
        {
            var parts = new List<string>();
            
            if (!string.IsNullOrWhiteSpace(Street))
                parts.Add(Street);
            
            if (!string.IsNullOrWhiteSpace(Street2))
                parts.Add(Street2);
            
            if (!string.IsNullOrWhiteSpace(City))
                parts.Add(City);
            
            if (!string.IsNullOrWhiteSpace(State))
                parts.Add(State);
            
            if (!string.IsNullOrWhiteSpace(Zip))
                parts.Add(Zip);
            
            return string.Join(", ", parts);
        }
    }
}