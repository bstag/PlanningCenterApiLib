namespace PlanningCenter.Api.Client.Models.People;

/// <summary>
/// Data Transfer Object for Campus from the Planning Center People API.
/// </summary>
public class CampusDto
{
    /// <summary>
    /// Unique identifier for the campus
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// The type of resource (always "Campus")
    /// </summary>
    public string Type { get; set; } = "Campus";
    
    /// <summary>
    /// All campus attributes from the API response
    /// </summary>
    public CampusAttributesDto Attributes { get; set; } = new();
    
    /// <summary>
    /// Related resource relationships
    /// </summary>
    public CampusRelationshipsDto Relationships { get; set; } = new();
    
    /// <summary>
    /// API links for this resource
    /// </summary>
    public CampusLinksDto Links { get; set; } = new();
}

/// <summary>
/// Campus attributes from the Planning Center People API.
/// </summary>
public class CampusAttributesDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Zip { get; set; }
    public string? Country { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Website { get; set; }
    public string? TimeZone { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Campus relationships from the Planning Center People API.
/// </summary>
public class CampusRelationshipsDto
{
    public RelationshipDto? Lists { get; set; }
    public RelationshipDto? ServiceTimes { get; set; }
}

/// <summary>
/// Campus links from the Planning Center People API.
/// </summary>
public class CampusLinksDto
{
    public string? Self { get; set; }
    public string? Lists { get; set; }
    public string? ServiceTimes { get; set; }
}