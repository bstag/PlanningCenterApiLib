
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Publishing;
using PlanningCenter.Api.Client.Models.Requests;

namespace PlanningCenter.Api.Client.Abstractions;

/// <summary>
/// Service interface for the Planning Center Publishing module.
/// Provides comprehensive media content and sermon management with built-in pagination support.
/// </summary>
public interface IPublishingService
{
    // Episode management
    
    /// <summary>
    /// Gets a single episode by ID.
    /// </summary>
    /// <param name="id">The episode's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The episode, or null if not found</returns>
    Task<Episode?> GetEpisodeAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists episodes with optional filtering, sorting, and pagination.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with episodes</returns>
    Task<IPagedResponse<Episode>> ListEpisodesAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new episode.
    /// </summary>
    /// <param name="request">The episode creation request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created episode</returns>
    Task<Episode> CreateEpisodeAsync(EpisodeCreateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing episode.
    /// </summary>
    /// <param name="id">The episode's unique identifier</param>
    /// <param name="request">The episode update request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The updated episode</returns>
    Task<Episode> UpdateEpisodeAsync(string id, EpisodeUpdateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes an episode.
    /// </summary>
    /// <param name="id">The episode's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task DeleteEpisodeAsync(string id, CancellationToken cancellationToken = default);
    
    // Series management
    
    /// <summary>
    /// Gets a single series by ID.
    /// </summary>
    /// <param name="id">The series's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The series, or null if not found</returns>
    Task<Series?> GetSeriesAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists series with optional filtering, sorting, and pagination.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with series</returns>
    Task<IPagedResponse<Series>> ListSeriesAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new series.
    /// </summary>
    /// <param name="request">The series creation request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created series</returns>
    Task<Series> CreateSeriesAsync(SeriesCreateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing series.
    /// </summary>
    /// <param name="id">The series's unique identifier</param>
    /// <param name="request">The series update request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The updated series</returns>
    Task<Series> UpdateSeriesAsync(string id, SeriesUpdateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a series.
    /// </summary>
    /// <param name="id">The series's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task DeleteSeriesAsync(string id, CancellationToken cancellationToken = default);
    
    // Speaker management
    
    /// <summary>
    /// Gets a single speaker by ID.
    /// </summary>
    /// <param name="id">The speaker's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The speaker, or null if not found</returns>
    Task<Speaker?> GetSpeakerAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists speakers with optional filtering, sorting, and pagination.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with speakers</returns>
    Task<IPagedResponse<Speaker>> ListSpeakersAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new speaker.
    /// </summary>
    /// <param name="request">The speaker creation request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created speaker</returns>
    Task<Speaker> CreateSpeakerAsync(SpeakerCreateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing speaker.
    /// </summary>
    /// <param name="id">The speaker's unique identifier</param>
    /// <param name="request">The speaker update request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The updated speaker</returns>
    Task<Speaker> UpdateSpeakerAsync(string id, SpeakerUpdateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a speaker.
    /// </summary>
    /// <param name="id">The speaker's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task DeleteSpeakerAsync(string id, CancellationToken cancellationToken = default);
    
    // Speakership management
    
    /// <summary>
    /// Lists speakerships for a specific episode.
    /// </summary>
    /// <param name="episodeId">The episode's unique identifier</param>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with speakerships</returns>
    Task<IPagedResponse<Speakership>> ListSpeakershipsAsync(string episodeId, QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists episodes for a specific speaker.
    /// </summary>
    /// <param name="speakerId">The speaker's unique identifier</param>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with speakerships</returns>
    Task<IPagedResponse<Speakership>> ListSpeakerEpisodesAsync(string speakerId, QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Adds a speaker to an episode.
    /// </summary>
    /// <param name="episodeId">The episode's unique identifier</param>
    /// <param name="speakerId">The speaker's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created speakership</returns>
    Task<Speakership> AddSpeakerToEpisodeAsync(string episodeId, string speakerId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Removes a speaker from an episode.
    /// </summary>
    /// <param name="id">The speakership's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task DeleteSpeakershipAsync(string id, CancellationToken cancellationToken = default);
    
    // Media management
    
    /// <summary>
    /// Gets a single media file by ID.
    /// </summary>
    /// <param name="id">The media's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The media, or null if not found</returns>
    Task<Media?> GetMediaAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists media files for a specific episode.
    /// </summary>
    /// <param name="episodeId">The episode's unique identifier</param>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with media files</returns>
    Task<IPagedResponse<Media>> ListMediaAsync(string episodeId, QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Uploads a new media file to an episode.
    /// </summary>
    /// <param name="episodeId">The episode's unique identifier</param>
    /// <param name="request">The media upload request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The uploaded media</returns>
    Task<Media> UploadMediaAsync(string episodeId, MediaUploadRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing media file.
    /// </summary>
    /// <param name="id">The media's unique identifier</param>
    /// <param name="request">The media update request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The updated media</returns>
    Task<Media> UpdateMediaAsync(string id, MediaUpdateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a media file.
    /// </summary>
    /// <param name="id">The media's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task DeleteMediaAsync(string id, CancellationToken cancellationToken = default);
    
    // Content publishing
    
    /// <summary>
    /// Publishes an episode, making it publicly available.
    /// </summary>
    /// <param name="id">The episode's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The published episode</returns>
    Task<Episode> PublishEpisodeAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Unpublishes an episode, making it private.
    /// </summary>
    /// <param name="id">The episode's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The unpublished episode</returns>
    Task<Episode> UnpublishEpisodeAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Publishes a series, making it publicly available.
    /// </summary>
    /// <param name="id">The series's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The published series</returns>
    Task<Series> PublishSeriesAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Unpublishes a series, making it private.
    /// </summary>
    /// <param name="id">The series's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The unpublished series</returns>
    Task<Series> UnpublishSeriesAsync(string id, CancellationToken cancellationToken = default);
    
    // Content distribution
    
    /// <summary>
    /// Lists available distribution channels.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with distribution channels</returns>
    Task<IPagedResponse<DistributionChannel>> ListDistributionChannelsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Distributes an episode to a specific channel.
    /// </summary>
    /// <param name="episodeId">The episode's unique identifier</param>
    /// <param name="channelId">The distribution channel's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The distribution result</returns>
    Task<DistributionResult> DistributeEpisodeAsync(string episodeId, string channelId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Distributes a series to a specific channel.
    /// </summary>
    /// <param name="seriesId">The series's unique identifier</param>
    /// <param name="channelId">The distribution channel's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The distribution result</returns>
    Task<DistributionResult> DistributeSeriesAsync(string seriesId, string channelId, CancellationToken cancellationToken = default);
    
    // Analytics and reporting
    
    /// <summary>
    /// Gets analytics data for an episode.
    /// </summary>
    /// <param name="episodeId">The episode's unique identifier</param>
    /// <param name="request">The analytics request parameters</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The episode analytics data</returns>
    Task<EpisodeAnalytics> GetEpisodeAnalyticsAsync(string episodeId, AnalyticsRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets analytics data for a series.
    /// </summary>
    /// <param name="seriesId">The series's unique identifier</param>
    /// <param name="request">The analytics request parameters</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The series analytics data</returns>
    Task<SeriesAnalytics> GetSeriesAnalyticsAsync(string seriesId, AnalyticsRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Generates a publishing report.
    /// </summary>
    /// <param name="request">The publishing report request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The generated publishing report</returns>
    Task<PublishingReport> GeneratePublishingReportAsync(PublishingReportRequest request, CancellationToken cancellationToken = default);
    
    // Pagination helpers that eliminate manual pagination logic
    
    /// <summary>
    /// Gets all episodes matching the specified criteria.
    /// This method automatically handles pagination behind the scenes.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>All episodes matching the criteria</returns>
    Task<IReadOnlyList<Episode>> GetAllEpisodesAsync(
        QueryParameters? parameters = null,
        PaginationOptions? options = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Streams episodes matching the specified criteria for memory-efficient processing.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>An async enumerable that yields episodes from all pages</returns>
    IAsyncEnumerable<Episode> StreamEpisodesAsync(
        QueryParameters? parameters = null,
        PaginationOptions? options = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Distribution result information.
/// </summary>
public class DistributionResult
{
    /// <summary>
    /// Gets or sets whether the distribution was successful.
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Gets or sets the distribution status message.
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the distribution timestamp.
    /// </summary>
    public DateTime DistributedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Gets or sets the external URL if applicable.
    /// </summary>
    public string? ExternalUrl { get; set; }
    
    /// <summary>
    /// Gets or sets additional distribution metadata.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Episode analytics data.
/// </summary>
public class EpisodeAnalytics
{
    /// <summary>
    /// Gets or sets the episode ID.
    /// </summary>
    public string EpisodeId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the total view count.
    /// </summary>
    public long ViewCount { get; set; }
    
    /// <summary>
    /// Gets or sets the total download count.
    /// </summary>
    public long DownloadCount { get; set; }
    
    /// <summary>
    /// Gets or sets the average watch time in seconds.
    /// </summary>
    public double AverageWatchTimeSeconds { get; set; }
    
    /// <summary>
    /// Gets or sets the analytics period start date.
    /// </summary>
    public DateTime PeriodStart { get; set; }
    
    /// <summary>
    /// Gets or sets the analytics period end date.
    /// </summary>
    public DateTime PeriodEnd { get; set; }
    
    /// <summary>
    /// Gets or sets additional analytics data.
    /// </summary>
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

/// <summary>
/// Series analytics data.
/// </summary>
public class SeriesAnalytics
{
    /// <summary>
    /// Gets or sets the series ID.
    /// </summary>
    public string SeriesId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the total view count across all episodes.
    /// </summary>
    public long TotalViewCount { get; set; }
    
    /// <summary>
    /// Gets or sets the total download count across all episodes.
    /// </summary>
    public long TotalDownloadCount { get; set; }
    
    /// <summary>
    /// Gets or sets the number of episodes in the series.
    /// </summary>
    public int EpisodeCount { get; set; }
    
    /// <summary>
    /// Gets or sets the analytics period start date.
    /// </summary>
    public DateTime PeriodStart { get; set; }
    
    /// <summary>
    /// Gets or sets the analytics period end date.
    /// </summary>
    public DateTime PeriodEnd { get; set; }
    
    /// <summary>
    /// Gets or sets additional analytics data.
    /// </summary>
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

/// <summary>
/// Publishing report data.
/// </summary>
public class PublishingReport
{
    /// <summary>
    /// Gets or sets the total number of episodes.
    /// </summary>
    public int TotalEpisodes { get; set; }
    
    /// <summary>
    /// Gets or sets the total number of series.
    /// </summary>
    public int TotalSeries { get; set; }
    
    /// <summary>
    /// Gets or sets the total view count.
    /// </summary>
    public long TotalViews { get; set; }
    
    /// <summary>
    /// Gets or sets the total download count.
    /// </summary>
    public long TotalDownloads { get; set; }
    
    /// <summary>
    /// Gets or sets the report generation timestamp.
    /// </summary>
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Gets or sets the report period start date.
    /// </summary>
    public DateTime PeriodStart { get; set; }
    
    /// <summary>
    /// Gets or sets the report period end date.
    /// </summary>
    public DateTime PeriodEnd { get; set; }
    
    /// <summary>
    /// Gets or sets additional report data.
    /// </summary>
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

/// <summary>
/// Request for analytics data.
/// </summary>
public class AnalyticsRequest
{
    /// <summary>
    /// Gets or sets the start date for the analytics period.
    /// </summary>
    public DateTime StartDate { get; set; }
    
    /// <summary>
    /// Gets or sets the end date for the analytics period.
    /// </summary>
    public DateTime EndDate { get; set; }
    
    /// <summary>
    /// Gets or sets the metrics to include in the analytics.
    /// </summary>
    public List<string> Metrics { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the grouping for the analytics data.
    /// </summary>
    public string? GroupBy { get; set; }
}

/// <summary>
/// Request for generating publishing reports.
/// </summary>
public class PublishingReportRequest
{
    /// <summary>
    /// Gets or sets the start date for the report period.
    /// </summary>
    public DateTime StartDate { get; set; }
    
    /// <summary>
    /// Gets or sets the end date for the report period.
    /// </summary>
    public DateTime EndDate { get; set; }
    
    /// <summary>
    /// Gets or sets whether to include episode details.
    /// </summary>
    public bool IncludeEpisodeDetails { get; set; } = false;
    
    /// <summary>
    /// Gets or sets whether to include series details.
    /// </summary>
    public bool IncludeSeriesDetails { get; set; } = false;
    
    /// <summary>
    /// Gets or sets the report format.
    /// </summary>
    public string Format { get; set; } = "json";
}