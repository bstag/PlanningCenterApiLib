using System.Text.Json.Serialization;
using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Registrations;

/// <summary>
/// JSON API DTO for Count entity.
/// </summary>
public class CountDto : IResourceObject
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "Count";

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("attributes")]
    public CountAttributesDto Attributes { get; set; } = new();

    [JsonPropertyName("links")]
    public ResourceLinks? Links { get; set; }
}

/// <summary>
/// Attributes for Count JSON API DTO.
/// </summary>
public class CountAttributesDto
{
    [JsonPropertyName("count")]
    public int Count { get; set; }
}