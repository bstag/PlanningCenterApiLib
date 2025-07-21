using System;
using System.Text.Json.Serialization;

namespace PlanningCenter.Api.Client.Models.JsonApi.Registrations;

public class EmergencyContactAttributesDto
{
    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    [JsonPropertyName("relationship")]
    public string? Relationship { get; set; }

    [JsonPropertyName("primary_phone")]
    public string? PrimaryPhone { get; set; }

    [JsonPropertyName("secondary_phone")]
    public string? SecondaryPhone { get; set; }

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

    [JsonPropertyName("is_primary")]
    public bool? IsPrimary { get; set; }

    [JsonPropertyName("priority")]
    public int? Priority { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    [JsonPropertyName("preferred_contact_method")]
    public string? PreferredContactMethod { get; set; }

    [JsonPropertyName("best_time_to_contact")]
    public string? BestTimeToContact { get; set; }

    [JsonPropertyName("can_authorize_medical_treatment")]
    public bool? CanAuthorizeMedicalTreatment { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }
}
