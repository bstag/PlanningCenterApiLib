namespace PlanningCenter.Api.Client.Models.Requests;

/// <summary>
/// Request model for uploading media.
/// </summary>
public class MediaUploadRequest
{
    /// <summary>
    /// Gets or sets the media file name.
    /// </summary>
    public string FileName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the media content type (MIME type).
    /// </summary>
    public string ContentType { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the media file size in bytes.
    /// </summary>
    public long FileSizeInBytes { get; set; }
    
    /// <summary>
    /// Gets or sets the media type (e.g., "video", "audio", "image", "document").
    /// </summary>
    public string MediaType { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the media quality or resolution.
    /// </summary>
    public string? Quality { get; set; }
    
    /// <summary>
    /// Gets or sets whether this is the primary media file.
    /// </summary>
    public bool IsPrimary { get; set; }
    
    /// <summary>
    /// Gets or sets the file data (base64 encoded or binary).
    /// </summary>
    public byte[]? FileData { get; set; }
    
    /// <summary>
    /// Gets or sets the upload URL (for direct uploads).
    /// </summary>
    public string? UploadUrl { get; set; }
}

/// <summary>
/// Request model for updating media.
/// </summary>
public class MediaUpdateRequest
{
    /// <summary>
    /// Gets or sets the media file name.
    /// </summary>
    public string? FileName { get; set; }
    
    /// <summary>
    /// Gets or sets the media quality or resolution.
    /// </summary>
    public string? Quality { get; set; }
    
    /// <summary>
    /// Gets or sets whether this is the primary media file.
    /// </summary>
    public bool? IsPrimary { get; set; }
}