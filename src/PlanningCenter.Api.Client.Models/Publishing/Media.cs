using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.Publishing;

/// <summary>
/// Represents a media file associated with an episode in Planning Center Publishing.
/// </summary>
public class Media : PlanningCenterResource
{
    /// <summary>
    /// Gets or sets the associated episode ID.
    /// </summary>
    public string EpisodeId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the media file name.
    /// </summary>
    public string FileName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the media file URL.
    /// </summary>
    public string FileUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the media download URL.
    /// </summary>
    public string? DownloadUrl { get; set; }
    
    /// <summary>
    /// Gets or sets the media file size in bytes.
    /// </summary>
    public long FileSizeInBytes { get; set; }
    
    /// <summary>
    /// Gets or sets the media content type (MIME type).
    /// </summary>
    public string ContentType { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the media type (e.g., "video", "audio", "image", "document").
    /// </summary>
    public string MediaType { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the media quality or resolution.
    /// </summary>
    public string? Quality { get; set; }
    
    /// <summary>
    /// Gets or sets the media duration in seconds (for audio/video).
    /// </summary>
    public int? DurationSeconds { get; set; }
    
    /// <summary>
    /// Gets or sets the video width in pixels.
    /// </summary>
    public int? Width { get; set; }
    
    /// <summary>
    /// Gets or sets the video height in pixels.
    /// </summary>
    public int? Height { get; set; }
    
    /// <summary>
    /// Gets or sets the video frame rate.
    /// </summary>
    public decimal? FrameRate { get; set; }
    
    /// <summary>
    /// Gets or sets the audio bitrate.
    /// </summary>
    public int? AudioBitrate { get; set; }
    
    /// <summary>
    /// Gets or sets the video bitrate.
    /// </summary>
    public int? VideoBitrate { get; set; }
    
    /// <summary>
    /// Gets or sets the audio sample rate.
    /// </summary>
    public int? SampleRate { get; set; }
    
    /// <summary>
    /// Gets or sets the number of audio channels.
    /// </summary>
    public int? Channels { get; set; }
    
    /// <summary>
    /// Gets or sets the media format or codec.
    /// </summary>
    public string? Format { get; set; }
    
    /// <summary>
    /// Gets or sets whether this is the primary media file.
    /// </summary>
    public bool IsPrimary { get; set; }
    
    /// <summary>
    /// Gets or sets the upload status.
    /// </summary>
    public string UploadStatus { get; set; } = "pending";
    
    /// <summary>
    /// Gets or sets the processing status.
    /// </summary>
    public string? ProcessingStatus { get; set; }
    
    /// <summary>
    /// Gets or sets when the upload was completed.
    /// </summary>
    public DateTime? UploadedAt { get; set; }
    
    /// <summary>
    /// Gets or sets when processing was completed.
    /// </summary>
    public DateTime? ProcessedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the download count for this media file.
    /// </summary>
    public long DownloadCount { get; set; }
    
    /// <summary>
    /// Gets or sets the view count for this media file.
    /// </summary>
    public long ViewCount { get; set; }
    
    /// <summary>
    /// Gets or sets additional metadata.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}