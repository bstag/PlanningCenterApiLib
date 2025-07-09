namespace PlanningCenter.Api.Client.Models.People;

/// <summary>
/// Data Transfer Object for Email from the Planning Center People API.
/// </summary>
public class EmailDto
{
    /// <summary>
    /// Unique identifier for the email
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// The type of resource (always "Email")
    /// </summary>
    public string Type { get; set; } = "Email";
    
    /// <summary>
    /// All email attributes from the API response
    /// </summary>
    public EmailAttributesDto Attributes { get; set; } = new();
    
    /// <summary>
    /// Related resource relationships
    /// </summary>
    public EmailRelationshipsDto Relationships { get; set; } = new();
    
    /// <summary>
    /// API links for this resource
    /// </summary>
    public EmailLinksDto Links { get; set; } = new();
}

/// <summary>
/// Email attributes from the Planning Center People API.
/// </summary>
public class EmailAttributesDto
{
    public string? Address { get; set; }
    public string? Location { get; set; }
    public bool Primary { get; set; }
    public bool Blocked { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Email relationships from the Planning Center People API.
/// </summary>
public class EmailRelationshipsDto
{
    public RelationshipDto? Person { get; set; }
}

/// <summary>
/// Email links from the Planning Center People API.
/// </summary>
public class EmailLinksDto
{
    public string? Self { get; set; }
    public string? Person { get; set; }
}