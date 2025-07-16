namespace PlanningCenter.Api.Client.Models.Requests;

/// <summary>
/// Request model for creating a speaker.
/// </summary>
public class SpeakerCreateRequest
{
    /// <summary>
    /// Gets or sets the speaker's first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the speaker's last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the speaker's display name or stage name.
    /// </summary>
    public string? DisplayName { get; set; }
    
    /// <summary>
    /// Gets or sets the speaker's title or position.
    /// </summary>
    public string? Title { get; set; }
    
    /// <summary>
    /// Gets or sets the speaker's biography.
    /// </summary>
    public string? Biography { get; set; }
    
    /// <summary>
    /// Gets or sets the speaker's email address.
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// Gets or sets the speaker's phone number.
    /// </summary>
    public string? PhoneNumber { get; set; }
    
    /// <summary>
    /// Gets or sets the speaker's website URL.
    /// </summary>
    public string? WebsiteUrl { get; set; }
    
    /// <summary>
    /// Gets or sets the speaker's photo URL.
    /// </summary>
    public string? PhotoUrl { get; set; }
    
    /// <summary>
    /// Gets or sets the speaker's organization or church.
    /// </summary>
    public string? Organization { get; set; }
    
    /// <summary>
    /// Gets or sets the speaker's location.
    /// </summary>
    public string? Location { get; set; }
    
    /// <summary>
    /// Gets or sets the speaker's specialties or topics.
    /// </summary>
    public List<string> Specialties { get; set; } = new();
    
    /// <summary>
    /// Gets or sets whether the speaker is active.
    /// </summary>
    public bool Active { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the speaker's social media profiles.
    /// </summary>
    public Dictionary<string, string> SocialProfiles { get; set; } = new();
}

/// <summary>
/// Request model for updating a speaker.
/// </summary>
public class SpeakerUpdateRequest
{
    /// <summary>
    /// Gets or sets the speaker's first name.
    /// </summary>
    public string? FirstName { get; set; }
    
    /// <summary>
    /// Gets or sets the speaker's last name.
    /// </summary>
    public string? LastName { get; set; }
    
    /// <summary>
    /// Gets or sets the speaker's display name or stage name.
    /// </summary>
    public string? DisplayName { get; set; }
    
    /// <summary>
    /// Gets or sets the speaker's title or position.
    /// </summary>
    public string? Title { get; set; }
    
    /// <summary>
    /// Gets or sets the speaker's biography.
    /// </summary>
    public string? Biography { get; set; }
    
    /// <summary>
    /// Gets or sets the speaker's email address.
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// Gets or sets the speaker's phone number.
    /// </summary>
    public string? PhoneNumber { get; set; }
    
    /// <summary>
    /// Gets or sets the speaker's website URL.
    /// </summary>
    public string? WebsiteUrl { get; set; }
    
    /// <summary>
    /// Gets or sets the speaker's photo URL.
    /// </summary>
    public string? PhotoUrl { get; set; }
    
    /// <summary>
    /// Gets or sets the speaker's organization or church.
    /// </summary>
    public string? Organization { get; set; }
    
    /// <summary>
    /// Gets or sets the speaker's location.
    /// </summary>
    public string? Location { get; set; }
    
    /// <summary>
    /// Gets or sets the speaker's specialties or topics.
    /// </summary>
    public List<string>? Specialties { get; set; }
    
    /// <summary>
    /// Gets or sets whether the speaker is active.
    /// </summary>
    public bool? Active { get; set; }
    
    /// <summary>
    /// Gets or sets the speaker's social media profiles.
    /// </summary>
    public Dictionary<string, string>? SocialProfiles { get; set; }
}