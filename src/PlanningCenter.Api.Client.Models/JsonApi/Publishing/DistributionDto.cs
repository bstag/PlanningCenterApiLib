using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Publishing;

public class DistributionDto : IResourceObject
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "Distribution";

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("attributes")]
    public DistributionAttributesDto? Attributes { get; set; }

    [JsonPropertyName("links")]
    public ResourceLinks? Links { get; set; }
}

public class DistributionAttributesDto
{
    [JsonPropertyName("success")]
    public bool? Success { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("distributed_at")]
    public DateTimeOffset? DistributedAt { get; set; }

    [JsonPropertyName("external_url")]
    public string? ExternalUrl { get; set; }

    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }
}
