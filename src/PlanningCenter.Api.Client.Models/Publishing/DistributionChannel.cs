using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.Publishing;

/// <summary>
/// Represents a distribution channel for publishing content in Planning Center Publishing.
/// </summary>
public class DistributionChannel : PlanningCenterResource
{
    /// <summary>
    /// Gets or sets the channel name.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the channel description.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Gets or sets the channel type (e.g., "podcast", "youtube", "facebook", "website").
    /// </summary>
    public string ChannelType { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the channel provider or platform.
    /// </summary>
    public string? Provider { get; set; }
    
    /// <summary>
    /// Gets or sets the channel URL or endpoint.
    /// </summary>
    public string? ChannelUrl { get; set; }
    
    /// <summary>
    /// Gets or sets the API endpoint for this channel.
    /// </summary>
    public string? ApiEndpoint { get; set; }
    
    /// <summary>
    /// Gets or sets whether the channel is active.
    /// </summary>
    public bool Active { get; set; } = true;
    
    /// <summary>
    /// Gets or sets whether automatic distribution is enabled.
    /// </summary>
    public bool AutoDistribute { get; set; }
    
    /// <summary>
    /// Gets or sets the distribution schedule or frequency.
    /// </summary>
    public string? DistributionSchedule { get; set; }
    
    /// <summary>
    /// Gets or sets the supported content types for this channel.
    /// </summary>
    public List<string> SupportedContentTypes { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the supported media formats for this channel.
    /// </summary>
    public List<string> SupportedFormats { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the maximum file size allowed (in bytes).
    /// </summary>
    public long? MaxFileSizeBytes { get; set; }
    
    /// <summary>
    /// Gets or sets the maximum duration allowed (in seconds).
    /// </summary>
    public int? MaxDurationSeconds { get; set; }
    
    /// <summary>
    /// Gets or sets the channel configuration settings.
    /// </summary>
    public Dictionary<string, object> Configuration { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the authentication credentials (encrypted).
    /// </summary>
    public Dictionary<string, string> Credentials { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the number of items distributed to this channel.
    /// </summary>
    public int DistributionCount { get; set; }
    
    /// <summary>
    /// Gets or sets the last distribution timestamp.
    /// </summary>
    public DateTime? LastDistributedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the last distribution status.
    /// </summary>
    public string? LastDistributionStatus { get; set; }
    
    /// <summary>
    /// Gets or sets additional metadata.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}