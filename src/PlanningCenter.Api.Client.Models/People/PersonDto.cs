namespace PlanningCenter.Api.Client.Models.People;

/// <summary>
/// Data Transfer Object for Person from the Planning Center People API.
/// This represents the raw API response structure.
/// </summary>
public class PersonDto
{
    /// <summary>
    /// Unique identifier for the person
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// The type of resource (always "Person")
    /// </summary>
    public string Type { get; set; } = "Person";
    
    /// <summary>
    /// All person attributes from the API response
    /// </summary>
    public PersonAttributesDto Attributes { get; set; } = new();
    
    /// <summary>
    /// Related resource relationships
    /// </summary>
    public PersonRelationshipsDto Relationships { get; set; } = new();
    
    /// <summary>
    /// API links for this resource
    /// </summary>
    public PersonLinksDto Links { get; set; } = new();
    
    /// <summary>
    /// Raw metadata from the API response
    /// </summary>
    public Dictionary<string, object> Meta { get; set; } = new();
}

/// <summary>
/// Person attributes from the Planning Center People API.
/// </summary>
public class PersonAttributesDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
    public string? Nickname { get; set; }
    public string? Gender { get; set; }
    public DateTime? Birthdate { get; set; }
    public DateTime? Anniversary { get; set; }
    public string? Status { get; set; }
    public string? MembershipStatus { get; set; }
    public string? MaritalStatus { get; set; }
    public string? School { get; set; }
    public string? Grade { get; set; }
    public int? GraduationYear { get; set; }
    public string? MedicalNotes { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; } = new();

    /// <summary>
    /// Indicates whether this person is a child account.
    /// </summary>
    public bool? Child { get; set; }

    /// <summary>
    /// Contact data convenience object returned by the API.
    /// </summary>
    public Dictionary<string, object>? ContactData { get; set; }

    /// <summary>
    /// Additional custom attributes that may be present
    /// </summary>
    public Dictionary<string, object> AdditionalAttributes { get; set; } = new();
}

/// <summary>
/// Person relationships from the Planning Center People API.
/// </summary>
public class PersonRelationshipsDto
{
    public RelationshipDto? PrimaryCampus { get; set; }
    public RelationshipDto? Addresses { get; set; }
    public RelationshipDto? Emails { get; set; }
    public RelationshipDto? PhoneNumbers { get; set; }
    public RelationshipDto? Households { get; set; }
    public RelationshipDto? FieldData { get; set; }
    public RelationshipDto? WorkflowCards { get; set; }
    
    /// <summary>
    /// Additional relationships that may be present
    /// </summary>
    public Dictionary<string, RelationshipDto> AdditionalRelationships { get; set; } = new();
}

/// <summary>
/// Generic relationship structure from the Planning Center API.
/// </summary>
public class RelationshipDto
{
    public RelationshipDataDto? Data { get; set; }
    public RelationshipLinksDto? Links { get; set; }
}

/// <summary>
/// Relationship data structure.
/// </summary>
public class RelationshipDataDto
{
    public string? Type { get; set; }
    public string? Id { get; set; }
    
    /// <summary>
    /// For array relationships
    /// </summary>
    public List<RelationshipDataDto>? Items { get; set; }
}

/// <summary>
/// Relationship links structure.
/// </summary>
public class RelationshipLinksDto
{
    public string? Self { get; set; }
    public string? Related { get; set; }
}

/// <summary>
/// Person links from the Planning Center People API.
/// </summary>
public class PersonLinksDto
{
    public string? Self { get; set; }
    public string? Addresses { get; set; }
    public string? Emails { get; set; }
    public string? PhoneNumbers { get; set; }
    public string? Households { get; set; }
    public string? FieldData { get; set; }
    
    /// <summary>
    /// Additional links that may be present
    /// </summary>
    public Dictionary<string, string> AdditionalLinks { get; set; } = new();
}