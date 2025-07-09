namespace PlanningCenter.Api.Client.Models.People;

/// <summary>
/// Data Transfer Object for PhoneNumber from the Planning Center People API.
/// </summary>
public class PhoneNumberDto
{
    /// <summary>
    /// Unique identifier for the phone number
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// The type of resource (always "PhoneNumber")
    /// </summary>
    public string Type { get; set; } = "PhoneNumber";
    
    /// <summary>
    /// All phone number attributes from the API response
    /// </summary>
    public PhoneNumberAttributesDto Attributes { get; set; } = new();
    
    /// <summary>
    /// Related resource relationships
    /// </summary>
    public PhoneNumberRelationshipsDto Relationships { get; set; } = new();
    
    /// <summary>
    /// API links for this resource
    /// </summary>
    public PhoneNumberLinksDto Links { get; set; } = new();
}

/// <summary>
/// PhoneNumber attributes from the Planning Center People API.
/// </summary>
public class PhoneNumberAttributesDto
{
    public string? Number { get; set; }
    public string? Location { get; set; }
    public bool Primary { get; set; }
    public bool Carrier { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// PhoneNumber relationships from the Planning Center People API.
/// </summary>
public class PhoneNumberRelationshipsDto
{
    public RelationshipDto? Person { get; set; }
}

/// <summary>
/// PhoneNumber links from the Planning Center People API.
/// </summary>
public class PhoneNumberLinksDto
{
    public string? Self { get; set; }
    public string? Person { get; set; }
}