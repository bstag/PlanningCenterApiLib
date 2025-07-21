using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Publishing;

public class EpisodeAnalyticsDto : IResourceObject
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "EpisodeAnalytics";

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("attributes")]
    public EpisodeAnalyticsAttributesDto? Attributes { get; set; }

    [JsonPropertyName("links")]
    public ResourceLinks? Links { get; set; }
}

public class EpisodeAnalyticsAttributesDto
{
    [JsonPropertyName("episode_id")]
    public string? EpisodeId { get; set; }

    [JsonPropertyName("view_count")]
    public int? ViewCount { get; set; }

    [JsonPropertyName("download_count")]
    public int? DownloadCount { get; set; }

    [JsonPropertyName("average_watch_time_seconds")]
    public double? AverageWatchTimeSeconds { get; set; }

    [JsonPropertyName("period_start")]
    public DateTimeOffset? PeriodStart { get; set; }

    [JsonPropertyName("period_end")]
    public DateTimeOffset? PeriodEnd { get; set; }

    [JsonPropertyName("additional_data")]
    public Dictionary<string, object>? AdditionalData { get; set; }
}
