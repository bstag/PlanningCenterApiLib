using System.Text.Json.Serialization;
using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Publishing;

/// <summary>
/// JSON API DTO for Speaker entity.
/// </summary>
public class SpeakerDto
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "Speaker";
    
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("attributes")]
    public SpeakerAttributesDto Attributes { get; set; } = new();
}

/// <summary>
/// Attributes for Speaker JSON API DTO.
/// </summary>
public class SpeakerAttributesDto
{
    [JsonPropertyName("first_name")]
    public string FirstName { get; set; } = string.Empty;
    
    [JsonPropertyName("last_name")]
    public string LastName { get; set; } = string.Empty;
    
    [JsonPropertyName("display_name")]
    public string? DisplayName { get; set; }
    
    [JsonPropertyName("title")]
    public string? Title { get; set; }
    
    [JsonPropertyName("biography")]
    public string? Biography { get; set; }
    
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }
    
    [JsonPropertyName("website_url")]
    public string? WebsiteUrl { get; set; }
    
    [JsonPropertyName("photo_url")]
    public string? PhotoUrl { get; set; }
    
    [JsonPropertyName("organization")]
    public string? Organization { get; set; }
    
    [JsonPropertyName("location")]
    public string? Location { get; set; }
    
    [JsonPropertyName("active")]
    public bool Active { get; set; } = true;
    
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Create DTO for Speaker.
/// </summary>
public class SpeakerCreateDto
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "Speaker";
    
    [JsonPropertyName("attributes")]
    public SpeakerCreateAttributesDto Attributes { get; set; } = new();
}

/// <summary>
/// Create attributes for Speaker.
/// </summary>
public class SpeakerCreateAttributesDto
{
    [JsonPropertyName("first_name")]
    public string FirstName { get; set; } = string.Empty;
    
    [JsonPropertyName("last_name")]
    public string LastName { get; set; } = string.Empty;
    
    [JsonPropertyName("display_name")]
    public string? DisplayName { get; set; }
    
    [JsonPropertyName("title")]
    public string? Title { get; set; }
    
    [JsonPropertyName("biography")]
    public string? Biography { get; set; }
    
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }
    
    [JsonPropertyName("website_url")]
    public string? WebsiteUrl { get; set; }
    
    [JsonPropertyName("photo_url")]
    public string? PhotoUrl { get; set; }
    
    [JsonPropertyName("organization")]
    public string? Organization { get; set; }
    
    [JsonPropertyName("location")]
    public string? Location { get; set; }
    
    [JsonPropertyName("active")]
    public bool Active { get; set; } = true;
}

/// <summary>
/// Update DTO for Speaker.
/// </summary>
public class SpeakerUpdateDto
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "Speaker";
    
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("attributes")]
    public SpeakerUpdateAttributesDto Attributes { get; set; } = new();
}

/// <summary>
/// Update attributes for Speaker.
/// </summary>
public class SpeakerUpdateAttributesDto
{
    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }
    
    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }
    
    [JsonPropertyName("display_name")]
    public string? DisplayName { get; set; }
    
    [JsonPropertyName("title")]
    public string? Title { get; set; }
    
    [JsonPropertyName("biography")]
    public string? Biography { get; set; }
    
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }
    
    [JsonPropertyName("website_url")]
    public string? WebsiteUrl { get; set; }
    
    [JsonPropertyName("photo_url")]
    public string? PhotoUrl { get; set; }
    
    [JsonPropertyName("organization")]
    public string? Organization { get; set; }
    
    [JsonPropertyName("location")]
    public string? Location { get; set; }
    
    [JsonPropertyName("active")]
    public bool? Active { get; set; }
}