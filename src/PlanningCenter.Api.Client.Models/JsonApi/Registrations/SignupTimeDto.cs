using System;
using System.Text.Json.Serialization;
using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Registrations;

public class SignupTimeDto : IResourceObject
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "SignupTime";

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("attributes")]
    public SignupTimeAttributes? Attributes { get; set; }

    [JsonPropertyName("links")]
    public ResourceLinks? Links { get; set; }
}

public class SignupTimeAttributes
{
    [JsonPropertyName("starts_at")]
    public DateTimeOffset StartsAt { get; set; }

    [JsonPropertyName("ends_at")]
    public DateTimeOffset? EndsAt { get; set; }

    [JsonPropertyName("all_day")]
    public bool AllDay { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }
}
