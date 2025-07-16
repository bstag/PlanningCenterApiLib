using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.Publishing;

/// <summary>
/// Represents the association between a speaker and an episode in Planning Center Publishing.
/// </summary>
public class Speakership : PlanningCenterResource
{
    /// <summary>
    /// Gets or sets the associated episode ID.
    /// </summary>
    public string EpisodeId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the associated speaker ID.
    /// </summary>
    public string SpeakerId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the speaker's role in this episode.
    /// </summary>
    public string? Role { get; set; }
    
    /// <summary>
    /// Gets or sets whether this is the primary speaker.
    /// </summary>
    public bool IsPrimary { get; set; }
    
    /// <summary>
    /// Gets or sets the display order for multiple speakers.
    /// </summary>
    public int SortOrder { get; set; }
    
    /// <summary>
    /// Gets or sets the speaker's contribution or topic.
    /// </summary>
    public string? Contribution { get; set; }
    
    /// <summary>
    /// Gets or sets the start time for this speaker's portion (in seconds).
    /// </summary>
    public int? StartTimeSeconds { get; set; }
    
    /// <summary>
    /// Gets or sets the end time for this speaker's portion (in seconds).
    /// </summary>
    public int? EndTimeSeconds { get; set; }
    
    /// <summary>
    /// Gets or sets additional notes about this speakership.
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Gets or sets additional metadata.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}