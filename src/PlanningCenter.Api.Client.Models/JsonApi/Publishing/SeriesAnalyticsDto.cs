using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Publishing;

/// <summary>
/// JSON API DTO for Series Analytics entity.
/// </summary>
public class SeriesAnalyticsDto
{
    public string Type { get; set; } = "SeriesAnalytics";
    public string Id { get; set; } = string.Empty;
    public SeriesAnalyticsAttributesDto Attributes { get; set; } = new();
}

/// <summary>
/// Attributes for Series Analytics JSON API DTO.
/// </summary>
public class SeriesAnalyticsAttributesDto
{
    public string SeriesId { get; set; } = string.Empty;
    public long TotalViewCount { get; set; }
    public long TotalDownloadCount { get; set; }
    public int EpisodeCount { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

/// <summary>
/// JSON API DTO for Episode Analytics entity.
/// </summary>
public class EpisodeAnalyticsDto
{
    public string Type { get; set; } = "EpisodeAnalytics";
    public string Id { get; set; } = string.Empty;
    public EpisodeAnalyticsAttributesDto Attributes { get; set; } = new();
}

/// <summary>
/// Attributes for Episode Analytics JSON API DTO.
/// </summary>
public class EpisodeAnalyticsAttributesDto
{
    public string EpisodeId { get; set; } = string.Empty;
    public long ViewCount { get; set; }
    public long DownloadCount { get; set; }
    public double AverageWatchTimeSeconds { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

/// <summary>
/// JSON API DTO for Distribution entity.
/// </summary>
public class DistributionDto
{
    public string Type { get; set; } = "Distribution";
    public string Id { get; set; } = string.Empty;
    public DistributionAttributesDto Attributes { get; set; } = new();
}

/// <summary>
/// Attributes for Distribution JSON API DTO.
/// </summary>
public class DistributionAttributesDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime DistributedAt { get; set; }
    public string? ExternalUrl { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Create DTO for Distribution.
/// </summary>
public class DistributionCreateDto
{
    public string Type { get; set; } = "Distribution";
    public DistributionCreateAttributesDto Attributes { get; set; } = new();
}

/// <summary>
/// Create attributes for Distribution.
/// </summary>
public class DistributionCreateAttributesDto
{
    public string? EpisodeId { get; set; }
    public string? SeriesId { get; set; }
    public string ChannelId { get; set; } = string.Empty;
}
