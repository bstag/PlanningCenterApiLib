namespace PlanningCenter.Api.Client.Models.Core;

/// <summary>
/// Unified person model that combines data from all Planning Center modules.
/// This model provides a consistent interface regardless of which module the person data came from.
/// </summary>
public class Person
{
    /// <summary>
    /// Unique identifier for the person
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Person's first name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Person's last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Person's middle name
    /// </summary>
    public string? MiddleName { get; set; }
    
    /// <summary>
    /// Computed full name (first + middle + last)
    /// </summary>
    public string FullName => string.Join(" ", new[] { FirstName, MiddleName, LastName }.Where(n => !string.IsNullOrWhiteSpace(n)));
    
    /// <summary>
    /// Person's nickname or preferred name
    /// </summary>
    public string? Nickname { get; set; }
    
    // Contact Information
    
    /// <summary>
    /// Primary email address
    /// </summary>
    public string? PrimaryEmail { get; set; }
    
    /// <summary>
    /// All email addresses associated with this person
    /// </summary>
    public List<Email> Emails { get; set; } = new();
    
    /// <summary>
    /// All phone numbers associated with this person
    /// </summary>
    public List<PhoneNumber> PhoneNumbers { get; set; } = new();
    
    /// <summary>
    /// All addresses associated with this person
    /// </summary>
    public List<Address> Addresses { get; set; } = new();
    
    // Demographics
    
    /// <summary>
    /// Person's date of birth
    /// </summary>
    public DateTime? Birthdate { get; set; }
    
    /// <summary>
    /// Person's age (calculated from birthdate)
    /// </summary>
    public int? Age => Birthdate?.Date.CalculateAge();

    /// <summary>
    /// Indicates whether the person is classified as a child in the Planning Center system.
    /// Mirrors the "child" boolean attribute returned by the API.
    /// </summary>
    public bool IsChild { get; set; }

    /// <summary>
    /// Arbitrary contact data blob returned by the API (e.g., email/phone convenience object).
    /// Represented as an untyped dictionary for forward-compatibility with new keys.
    /// </summary>
    public Dictionary<string, object>? ContactData { get; set; }
    
    /// <summary>
    /// Person's gender
    /// </summary>
    public string? Gender { get; set; }
    
    /// <summary>
    /// Person's marital status
    /// </summary>
    public string? MaritalStatus { get; set; }
    
    /// <summary>
    /// Anniversary date (typically wedding anniversary)
    /// </summary>
    public DateTime? Anniversary { get; set; }
    
    /// <summary>
    /// Person's status (active, inactive, etc.)
    /// </summary>
    public string Status { get; set; } = "active";
    
    // Organizational Information
    
    /// <summary>
    /// ID of the person's primary campus
    /// </summary>
    public string? PrimaryCampusId { get; set; }
    
    /// <summary>
    /// The person's primary campus (loaded via include)
    /// </summary>
    public Campus? PrimaryCampus { get; set; }
    
    /// <summary>
    /// Person's membership status
    /// </summary>
    public string? MembershipStatus { get; set; }
    
    /// <summary>
    /// School information
    /// </summary>
    public string? School { get; set; }
    
    /// <summary>
    /// Grade level (for students)
    /// </summary>
    public string? Grade { get; set; }
    
    /// <summary>
    /// Graduation year
    /// </summary>
    public int? GraduationYear { get; set; }
    
    // Medical and Emergency Information
    
    /// <summary>
    /// Medical notes or allergies
    /// </summary>
    public string? MedicalNotes { get; set; }
    
    /// <summary>
    /// Emergency contact information
    /// </summary>
    public string? EmergencyContactName { get; set; }
    
    /// <summary>
    /// Emergency contact phone number
    /// </summary>
    public string? EmergencyContactPhone { get; set; }
    
    // Metadata
    
    /// <summary>
    /// When the person record was created
    /// </summary>
    public DateTime? CreatedAt { get; set; }
    
    /// <summary>
    /// When the person record was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// Avatar or profile picture URL
    /// </summary>
    public string? AvatarUrl { get; set; }
    
    /// <summary>
    /// Avatar image data (base64 encoded or URL)
    /// </summary>
    public string? Avatar { get; set; }
    
    /// <summary>
    /// Demographic avatar URL
    /// </summary>
    public string? DemographicAvatarUrl { get; set; }
    
    /// <summary>
    /// Full name as returned by the API
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Remote identifier for external system integration
    /// </summary>
    public string? RemoteId { get; set; }
    
    /// <summary>
    /// Whether the person is an accounting administrator
    /// </summary>
    public bool? AccountingAdministrator { get; set; }
    
    /// <summary>
    /// Whether the person is classified as a child
    /// </summary>
    public bool? Child { get; set; }
    
    /// <summary>
    /// Given name (first name)
    /// </summary>
    public string? GivenName { get; set; }
    
    /// <summary>
    /// Whether the person has passed background check
    /// </summary>
    public bool? PassedBackgroundCheck { get; set; }
    
    /// <summary>
    /// Whether the person can create forms
    /// </summary>
    public bool? CanCreateForms { get; set; }
    
    /// <summary>
    /// Whether the person can email lists
    /// </summary>
    public bool? CanEmailLists { get; set; }
    
    /// <summary>
    /// Whether the person is a site administrator
    /// </summary>
    public bool? SiteAdministrator { get; set; }
    
    /// <summary>
    /// People module permissions
    /// </summary>
    public string? PeoplePermissions { get; set; }
    
    /// <summary>
    /// Custom fields and additional data
    /// Key-value pairs for module-specific or custom field data
    /// </summary>
    public Dictionary<string, object> CustomFields { get; set; } = new();
    
    /// <summary>
    /// Source tracking - which API module this person data came from
    /// Useful for debugging and data provenance
    /// </summary>
    public string DataSource { get; set; } = "People";
    
    /// <summary>
    /// Additional metadata about the person record
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
    
    /// <summary>
    /// Gets the primary address for this person
    /// </summary>
    public Address? PrimaryAddress => Addresses.FirstOrDefault(a => a.IsPrimary) ?? Addresses.FirstOrDefault();
    
    /// <summary>
    /// Gets the primary phone number for this person
    /// </summary>
    public PhoneNumber? PrimaryPhoneNumber => PhoneNumbers.FirstOrDefault(p => p.IsPrimary) ?? PhoneNumbers.FirstOrDefault();
    
    /// <summary>
    /// Gets the primary email for this person
    /// </summary>
    public Email? PrimaryEmailObject => Emails.FirstOrDefault(e => e.IsPrimary) ?? Emails.FirstOrDefault();
    
    /// <summary>
    /// Gets the display name for this person (nickname if available, otherwise full name)
    /// </summary>
    public string DisplayName => !string.IsNullOrWhiteSpace(Nickname) ? Nickname : FullName;
    
    /// <summary>
    /// Gets whether this person is active
    /// </summary>
    public bool IsActive => string.Equals(Status, "active", StringComparison.OrdinalIgnoreCase);
    
    /// <summary>
    /// Gets whether this person has an avatar/profile picture
    /// </summary>
    public bool HasAvatar => !string.IsNullOrWhiteSpace(AvatarUrl);
}

/// <summary>
/// Extension methods for date calculations
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Calculates age from a birthdate
    /// </summary>
    public static int CalculateAge(this DateTime birthDate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age)) age--;
        return age;
    }
}