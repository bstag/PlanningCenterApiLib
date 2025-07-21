using System.Text.Json.Serialization;
using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Publishing;

public class SeriesAnalyticsDto : IResourceObject
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "SeriesAnalytics";

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("attributes")]
    public SeriesAnalyticsAttributesDto? Attributes { get; set; }

    [JsonPropertyName("links")]
    public ResourceLinks? Links { get; set; }
}

public class SeriesAnalyticsAttributesDto
{
    [JsonPropertyName("total_views")]
    public int? TotalViews { get; set; }

    [JsonPropertyName("total_downloads")]
    public int? TotalDownloads { get; set; }

    [JsonPropertyName("total_plays")]
    public int? TotalPlays { get; set; }
}