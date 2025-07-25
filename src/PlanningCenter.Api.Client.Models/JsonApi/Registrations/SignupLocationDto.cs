using System;
using System.Text.Json.Serialization;
using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Registrations;

public class SignupLocationDto : IResourceObject
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "SignupLocation";

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("attributes")]
    public SignupLocationAttributesDto? Attributes { get; set; }

    [JsonPropertyName("links")]
    public ResourceLinks? Links { get; set; }
}

public class SignupLocationAttributesDto
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

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

    [JsonPropertyName("latitude")]
    public string? Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public string? Longitude { get; set; }

    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("website_url")]
    public string? WebsiteUrl { get; set; }

    [JsonPropertyName("directions")]
    public string? Directions { get; set; }

    [JsonPropertyName("parking_info")]
    public string? ParkingInfo { get; set; }

    [JsonPropertyName("accessibility_info")]
    public string? AccessibilityInfo { get; set; }

    [JsonPropertyName("capacity")]
    public int? Capacity { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    [JsonPropertyName("timezone")]
    public string? Timezone { get; set; }

    [JsonPropertyName("formatted_address")]
    public string? FormattedAddress { get; set; }

    [JsonPropertyName("full_formatted_address")]
    public string? FullFormattedAddress { get; set; }

    [JsonPropertyName("location_type")]
    public string? LocationType { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("subpremise")]
    public string? Subpremise { get; set; }

    [JsonPropertyName("address_data")]
    public string? AddressData { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }
}
