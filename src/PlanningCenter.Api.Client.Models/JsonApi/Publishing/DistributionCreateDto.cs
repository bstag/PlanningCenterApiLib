using System.Text.Json.Serialization;

namespace PlanningCenter.Api.Client.Models.JsonApi.Publishing;

public class DistributionCreateDto
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "Distribution";

    [JsonPropertyName("attributes")]
    public DistributionCreateAttributesDto Attributes { get; set; } = new();
}

public class DistributionCreateAttributesDto
{
    [JsonPropertyName("episode_id")]
    public string? EpisodeId { get; set; }

    [JsonPropertyName("series_id")]
    public string? SeriesId { get; set; }

    [JsonPropertyName("channel_id")]
    public string? ChannelId { get; set; }
}
