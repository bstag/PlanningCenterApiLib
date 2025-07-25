using System.Text.Json.Serialization;

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
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// The type of resource (always "Person")
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = "Person";
    
    /// <summary>
    /// All person attributes from the API response
    /// </summary>
    [JsonPropertyName("attributes")]
    public PersonAttributesDto Attributes { get; set; } = new();
    
    /// <summary>
    /// Related resource relationships
    /// </summary>
    [JsonPropertyName("relationships")]
    public PersonRelationshipsDto Relationships { get; set; } = new();
    
    /// <summary>
    /// API links for this resource
    /// </summary>
    [JsonPropertyName("links")]
    public PersonLinksDto Links { get; set; } = new();
    
    /// <summary>
    /// Raw metadata from the API response
    /// </summary>
    [JsonPropertyName("meta")]
    public Dictionary<string, object> Meta { get; set; } = new();
}

/// <summary>
/// Person attributes from the Planning Center People API.
/// </summary>
public class PersonAttributesDto
{
    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }
    
    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }
    
    [JsonPropertyName("middle_name")]
    public string? MiddleName { get; set; }
    
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }
    
    [JsonPropertyName("gender")]
    public string? Gender { get; set; }
    
    [JsonPropertyName("birthdate")]
    public DateTime? Birthdate { get; set; }
    
    [JsonPropertyName("anniversary")]
    public DateTime? Anniversary { get; set; }
    
    [JsonPropertyName("status")]
    public string? Status { get; set; }
    
    [JsonPropertyName("membership_status")]
    public string? MembershipStatus { get; set; }
    
    [JsonPropertyName("marital_status")]
    public string? MaritalStatus { get; set; }
    
    [JsonPropertyName("school")]
    public string? School { get; set; }
    
    [JsonPropertyName("grade")]
    [JsonConverter(typeof(PlanningCenter.Api.Client.Models.JsonApi.Core.FlexibleStringJsonConverter))]
    public string? Grade { get; set; }
    
    [JsonPropertyName("graduation_year")]
    public int? GraduationYear { get; set; }
    
    [JsonPropertyName("medical_notes")]
    public string? MedicalNotes { get; set; }
    
    [JsonPropertyName("emergency_contact_name")]
    public string? EmergencyContactName { get; set; }
    
    [JsonPropertyName("emergency_contact_phone")]
    public string? EmergencyContactPhone { get; set; }
    
    [JsonPropertyName("avatar_url")]
    public string? AvatarUrl { get; set; }
    
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; } = new();

    /// <summary>
    /// Indicates whether this person is a child account.
    /// </summary>
    [JsonPropertyName("child")]
    public bool? Child { get; set; }

    /// <summary>
    /// Contact data convenience object returned by the API.
    /// </summary>
    [JsonPropertyName("contact_data")]
    public Dictionary<string, object>? ContactData { get; set; }

    // Missing fields from API specification
    /// <summary>
    /// File UUID for avatar image
    /// </summary>
    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }

    /// <summary>
    /// Demographic avatar URL
    /// </summary>
    [JsonPropertyName("demographic_avatar_url")]
    public string? DemographicAvatarUrl { get; set; }

    /// <summary>
    /// Computed full name
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Remote identifier
    /// </summary>
    [JsonPropertyName("remote_id")]
    public string? RemoteId { get; set; }

    /// <summary>
    /// Accounting administrator flag
    /// </summary>
    [JsonPropertyName("accounting_administrator")]
    public bool? AccountingAdministrator { get; set; }

    /// <summary>
    /// Given name
    /// </summary>
    [JsonPropertyName("given_name")]
    public string? GivenName { get; set; }

    /// <summary>
    /// People permissions
    /// </summary>
    [JsonPropertyName("people_permissions")]
    public string? PeoplePermissions { get; set; }

    /// <summary>
    /// Site administrator flag
    /// </summary>
    [JsonPropertyName("site_administrator")]
    public bool? SiteAdministrator { get; set; }

    /// <summary>
    /// Date when person was inactivated
    /// </summary>
    [JsonPropertyName("inactivated_at")]
    public DateTime? InactivatedAt { get; set; }

    /// <summary>
    /// Membership information
    /// </summary>
    [JsonPropertyName("membership")]
    public string? Membership { get; set; }

    /// <summary>
    /// Can create forms flag
    /// </summary>
    [JsonPropertyName("can_create_forms")]
    public bool? CanCreateForms { get; set; }

    /// <summary>
    /// Can email lists flag
    /// </summary>
    [JsonPropertyName("can_email_lists")]
    public bool? CanEmailLists { get; set; }

    /// <summary>
    /// Directory shared info
    /// </summary>
    [JsonPropertyName("directory_shared_info")]
    public string? DirectorySharedInfo { get; set; }

    /// <summary>
    /// Directory status
    /// </summary>
    [JsonPropertyName("directory_status")]
    public string? DirectoryStatus { get; set; }

    /// <summary>
    /// Passed background check flag
    /// </summary>
    [JsonPropertyName("passed_background_check")]
    public bool? PassedBackgroundCheck { get; set; }

    /// <summary>
    /// Resource permission flags
    /// </summary>
    [JsonPropertyName("resource_permission_flags")]
    public Dictionary<string, bool>? ResourcePermissionFlags { get; set; }

    /// <summary>
    /// School type
    /// </summary>
    [JsonPropertyName("school_type")]
    public string? SchoolType { get; set; }

    /// <summary>
    /// Login identifier
    /// </summary>
    [JsonPropertyName("login_identifier")]
    public string? LoginIdentifier { get; set; }

    /// <summary>
    /// MFA configured flag
    /// </summary>
    [JsonPropertyName("mfa_configured")]
    public bool? MfaConfigured { get; set; }

    /// <summary>
    /// Stripe customer identifier
    /// </summary>
    [JsonPropertyName("stripe_customer_identifier")]
    public string? StripeCustomerIdentifier { get; set; }

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