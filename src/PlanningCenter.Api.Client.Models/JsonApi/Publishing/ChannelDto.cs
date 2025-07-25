using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Publishing;

public class ChannelDto : IResourceObject
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "Channel";

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("attributes")]
    public ChannelAttributes? Attributes { get; set; }

    [JsonPropertyName("links")]
    public ResourceLinks? Links { get; set; }
}

public class ChannelAttributes
{
    [JsonPropertyName("art")]
    public Dictionary<string, object>? Art { get; set; }

    [JsonPropertyName("podcast_art")]
    public Dictionary<string, object>? PodcastArt { get; set; }

    [JsonPropertyName("podcast_settings")]
    public Dictionary<string, object>? PodcastSettings { get; set; }

    [JsonPropertyName("activate_episode_minutes_before")]
    public int? ActivateEpisodeMinutesBefore { get; set; }

    [JsonPropertyName("can_enable_chat")]
    public bool? CanEnableChat { get; set; }

    [JsonPropertyName("church_center_url")]
    public string? ChurchCenterUrl { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }

    [JsonPropertyName("default_video_embed_code")]
    public string? DefaultVideoEmbedCode { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("default_video_duration")]
    public int? DefaultVideoDuration { get; set; }

    [JsonPropertyName("default_video_url")]
    public string? DefaultVideoUrl { get; set; }

    [JsonPropertyName("enable_audio")]
    public bool? EnableAudio { get; set; }

    [JsonPropertyName("enable_on_demand_video")]
    public bool? EnableOnDemandVideo { get; set; }

    [JsonPropertyName("enable_watch_live")]
    public bool? EnableWatchLive { get; set; }

    [JsonPropertyName("general_chat_enabled")]
    public bool? GeneralChatEnabled { get; set; }

    [JsonPropertyName("group_chat_enabled")]
    public bool? GroupChatEnabled { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("podcast_feed_url")]
    public string? PodcastFeedUrl { get; set; }

    [JsonPropertyName("position")]
    public int? Position { get; set; }

    [JsonPropertyName("published")]
    public bool? Published { get; set; }

    [JsonPropertyName("sermon_notes_enabled")]
    public bool? SermonNotesEnabled { get; set; }

    [JsonPropertyName("services_service_type_remote_identifier")]
    public string? ServicesServiceTypeRemoteIdentifier { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }
}
