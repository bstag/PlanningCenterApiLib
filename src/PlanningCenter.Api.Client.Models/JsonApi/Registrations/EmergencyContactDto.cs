using System.Text.Json.Serialization;
using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Registrations;

public class EmergencyContactDto : IResourceObject
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "EmergencyContact";

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("attributes")]
    public EmergencyContactAttributes? Attributes { get; set; }

    [JsonPropertyName("links")]
    public ResourceLinks? Links { get; set; }
}

public class EmergencyContactAttributes
{
    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("relationship")]
    public string? Relationship { get; set; }

    [JsonPropertyName("primary_phone")]
    public string? PrimaryPhone { get; set; }

    [JsonPropertyName("secondary_phone")]
    public string? SecondaryPhone { get; set; }

    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("street_address")]
    public string? StreetAddress { get; set; }

    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("state")]
    public string? State { get; set; }

    [JsonPropertyName("postal_code")]
    public string? PostalCode { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [JsonPropertyName("is_primary")]
    public bool IsPrimary { get; set; }

    [JsonPropertyName("priority")]
    public int Priority { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    [JsonPropertyName("preferred_contact_method")]
    public string? PreferredContactMethod { get; set; }

    [JsonPropertyName("best_time_to_contact")]
    public string? BestTimeToContact { get; set; }

    [JsonPropertyName("can_authorize_medical_treatment")]
    public bool CanAuthorizeMedicalTreatment { get; set; }
}
