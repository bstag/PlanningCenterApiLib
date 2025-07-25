using System.Text.Json.Serialization;
using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Publishing;

public class PublishingReportDto : IResourceObject
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "PublishingReport";

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("attributes")]
    public PublishingReportAttributesDto? Attributes { get; set; }

    [JsonPropertyName("links")]
    public ResourceLinks? Links { get; set; }
}

public class PublishingReportAttributesDto
{
    [JsonPropertyName("summary")]
    public string? Summary { get; set; }

    [JsonPropertyName("total_views")]
    public int? TotalViews { get; set; }

    [JsonPropertyName("unique_viewers")]
    public int? UniqueViewers { get; set; }

    [JsonPropertyName("average_watch_time")]
    public int? AverageWatchTime { get; set; }

    [JsonPropertyName("total_view_count")]
    public int? TotalViewCount { get; set; }

    [JsonPropertyName("total_download_count")]
    public int? TotalDownloadCount { get; set; }

    [JsonPropertyName("episode_count")]
    public int? EpisodeCount { get; set; }
}
