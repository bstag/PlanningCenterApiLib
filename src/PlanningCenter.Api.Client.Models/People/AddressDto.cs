using System.Text.Json.Serialization;

namespace PlanningCenter.Api.Client.Models.People;

/// <summary>
/// Data Transfer Object for Address from the Planning Center People API.
/// </summary>
public class AddressDto
{
    /// <summary>
    /// Unique identifier for the address
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// The type of resource (always "Address")
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = "Address";
    
    /// <summary>
    /// All address attributes from the API response
    /// </summary>
    [JsonPropertyName("attributes")]
    public AddressAttributesDto Attributes { get; set; } = new();
    
    /// <summary>
    /// Related resource relationships
    /// </summary>
    [JsonPropertyName("relationships")]
    public AddressRelationshipsDto Relationships { get; set; } = new();
    
    /// <summary>
    /// API links for this resource
    /// </summary>
    [JsonPropertyName("links")]
    public AddressLinksDto Links { get; set; } = new();
}

/// <summary>
/// Address attributes from the Planning Center People API.
/// </summary>
public class AddressAttributesDto
{
    /// <summary>
    /// First line of street address
    /// </summary>
    [JsonPropertyName("street_line_1")]
    public string? StreetLine1 { get; set; }

    /// <summary>
    /// Second line of street address
    /// </summary>
    [JsonPropertyName("street_line_2")]
    public string? StreetLine2 { get; set; }

    [JsonPropertyName("city")]
    public string? City { get; set; }
    
    [JsonPropertyName("state")]
    public string? State { get; set; }
    
    [JsonPropertyName("zip")]
    public string? Zip { get; set; }

    /// <summary>
    /// Country code (e.g., US, CA)
    /// </summary>
    [JsonPropertyName("country_code")]
    public string? CountryCode { get; set; }

    /// <summary>
    /// Full country name
    /// </summary>
    [JsonPropertyName("country_name")]
    public string? CountryName { get; set; }

    [JsonPropertyName("location")]
    public string? Location { get; set; }
    
    [JsonPropertyName("primary")]
    public bool Primary { get; set; }
    
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Address relationships from the Planning Center People API.
/// </summary>
public class AddressRelationshipsDto
{
    [JsonPropertyName("person")]
    public RelationshipDto? Person { get; set; }
}

/// <summary>
/// Address links from the Planning Center People API.
/// </summary>
public class AddressLinksDto
{
    [JsonPropertyName("self")]
    public string? Self { get; set; }
    
    [JsonPropertyName("person")]
    public string? Person { get; set; }
}