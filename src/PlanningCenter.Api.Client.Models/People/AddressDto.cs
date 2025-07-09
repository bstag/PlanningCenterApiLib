namespace PlanningCenter.Api.Client.Models.People;

/// <summary>
/// Data Transfer Object for Address from the Planning Center People API.
/// </summary>
public class AddressDto
{
    /// <summary>
    /// Unique identifier for the address
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// The type of resource (always "Address")
    /// </summary>
    public string Type { get; set; } = "Address";
    
    /// <summary>
    /// All address attributes from the API response
    /// </summary>
    public AddressAttributesDto Attributes { get; set; } = new();
    
    /// <summary>
    /// Related resource relationships
    /// </summary>
    public AddressRelationshipsDto Relationships { get; set; } = new();
    
    /// <summary>
    /// API links for this resource
    /// </summary>
    public AddressLinksDto Links { get; set; } = new();
}

/// <summary>
/// Address attributes from the Planning Center People API.
/// </summary>
public class AddressAttributesDto
{
    public string? Street { get; set; }
    public string? Street2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Zip { get; set; }
    public string? Country { get; set; }
    public string? Location { get; set; }
    public bool Primary { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Address relationships from the Planning Center People API.
/// </summary>
public class AddressRelationshipsDto
{
    public RelationshipDto? Person { get; set; }
}

/// <summary>
/// Address links from the Planning Center People API.
/// </summary>
public class AddressLinksDto
{
    public string? Self { get; set; }
    public string? Person { get; set; }
}