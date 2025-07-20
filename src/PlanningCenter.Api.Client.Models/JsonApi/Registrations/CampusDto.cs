using System;
using System.Text.Json.Serialization;
using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Registrations;

public class CampusDto : IResourceObject
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "Campus";

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("attributes")]
    public CampusAttributes? Attributes { get; set; }

    [JsonPropertyName("links")]
    public ResourceLinks? Links { get; set; }
}

public class CampusAttributes
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("timezone")]
    public string? Timezone { get; set; }

    [JsonPropertyName("address")]
    public string? Address { get; set; }

    [JsonPropertyName("street")]
    public string? Street { get; set; }

    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("state")]
    public string? State { get; set; }

    [JsonPropertyName("postal_code")]
    public string? PostalCode { get; set; }

    [JsonPropertyName("zip")]
    public string? Zip { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("website_url")]
    public string? WebsiteUrl { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; } = true;

    [JsonPropertyName("sort_order")]
    public int SortOrder { get; set; }

    [JsonPropertyName("signup_count")]
    public int SignupCount { get; set; }

    [JsonPropertyName("full_formatted_address")]
    public string? FullFormattedAddress { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }
}
