# Publishing Module Specification

## Overview

The Publishing module manages media content and sermon management in Planning Center. It provides comprehensive functionality for organizing episodes, managing series, handling speaker information, and distributing media content across various platforms.

## Core Entities

### Episode
The primary entity representing an individual piece of content (sermon, teaching, etc.).

**Key Attributes:**
- `id` - Unique identifier
- `title` - Episode title
- `description` - Episode description
- `published_at` - Publication timestamp
- `length_in_seconds` - Duration in seconds
- `video_url` - Video content URL
- `audio_url` - Audio content URL
- `video_download_url` - Video download link
- `audio_download_url` - Audio download link
- `video_file_size_in_bytes` - Video file size
- `audio_file_size_in_bytes` - Audio file size
- `video_content_type` - Video MIME type
- `audio_content_type` - Audio MIME type
- `artwork_url` - Episode artwork URL
- `artwork_file_size_in_bytes` - Artwork file size
- `artwork_content_type` - Artwork MIME type
- `notes` - Episode notes/transcript
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `series` - Associated series
- `speakerships` - Speaker associations
- `media` - Media files

### Series
Represents a collection of related episodes.

**Key Attributes:**
- `id` - Unique identifier
- `title` - Series title
- `description` - Series description
- `artwork_url` - Series artwork URL
- `artwork_file_size_in_bytes` - Artwork file size
- `artwork_content_type` - Artwork MIME type
- `published_at` - Publication timestamp
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `episodes` - Episodes in this series

### Speaker
Represents a content presenter or speaker.

**Key Attributes:**
- `id` - Unique identifier
- `name` - Speaker name
- `bio` - Speaker biography
- `photo_url` - Speaker photo URL
- `photo_file_size_in_bytes` - Photo file size
- `photo_content_type` - Photo MIME type
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `speakerships` - Speaker-episode associations

### Speakership
Represents the relationship between a speaker and an episode.

**Key Attributes:**
- `id` - Unique identifier
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `episode` - Associated episode
- `speaker` - Associated speaker

### Media
Represents media files associated with episodes.

**Key Attributes:**
- `id` - Unique identifier
- `filename` - Original filename
- `content_type` - MIME type
- `file_size_in_bytes` - File size
- `downloadable` - Download permission flag
- `streamable` - Streaming permission flag
- `duration_in_seconds` - Media duration
- `created_at` - Upload timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `episode` - Associated episode

## API Endpoints

### Episode Management
- `GET /publishing/v2/episodes` - List all episodes
- `GET /publishing/v2/episodes/{id}` - Get specific episode
- `POST /publishing/v2/episodes` - Create new episode
- `PATCH /publishing/v2/episodes/{id}` - Update episode
- `DELETE /publishing/v2/episodes/{id}` - Delete episode

### Series Management
- `GET /publishing/v2/series` - List all series
- `GET /publishing/v2/series/{id}` - Get specific series
- `POST /publishing/v2/series` - Create new series
- `PATCH /publishing/v2/series/{id}` - Update series
- `DELETE /publishing/v2/series/{id}` - Delete series

### Speaker Management
- `GET /publishing/v2/speakers` - List all speakers
- `GET /publishing/v2/speakers/{id}` - Get specific speaker
- `POST /publishing/v2/speakers` - Create new speaker
- `PATCH /publishing/v2/speakers/{id}` - Update speaker
- `DELETE /publishing/v2/speakers/{id}` - Delete speaker

### Speakership Management
- `GET /publishing/v2/episodes/{episode_id}/speakerships` - List episode speakers
- `GET /publishing/v2/speakers/{speaker_id}/speakerships` - List speaker episodes
- `POST /publishing/v2/episodes/{episode_id}/speakerships` - Add speaker to episode
- `DELETE /publishing/v2/speakerships/{id}` - Remove speaker from episode

### Media Management
- `GET /publishing/v2/episodes/{episode_id}/media` - List episode media
- `POST /publishing/v2/episodes/{episode_id}/media` - Upload media
- `PATCH /publishing/v2/media/{id}` - Update media
- `DELETE /publishing/v2/media/{id}` - Delete media

## Query Parameters

### Include Parameters
- `series` - Include series information
- `speakerships` - Include speaker associations
- `speakers` - Include speaker details
- `media` - Include media files

### Filtering
- `where[title]` - Filter by title
- `where[series_id]` - Filter by series
- `where[published_at]` - Filter by publication date
- `where[created_at]` - Filter by creation date
- `where[speaker_id]` - Filter by speaker

### Sorting
- `order=title` - Sort by title
- `order=published_at` - Sort by publication date
- `order=created_at` - Sort by creation date
- `order=-published_at` - Sort by publication date (descending)

## Service Interface

```csharp
public interface IPublishingService
{
    // Episode management
    Task<Episode> GetEpisodeAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Episode>> ListEpisodesAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Episode> CreateEpisodeAsync(EpisodeCreateRequest request, CancellationToken cancellationToken = default);
    Task<Episode> UpdateEpisodeAsync(string id, EpisodeUpdateRequest request, CancellationToken cancellationToken = default);
    Task DeleteEpisodeAsync(string id, CancellationToken cancellationToken = default);
    
    // Series management
    Task<Series> GetSeriesAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Series>> ListSeriesAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Series> CreateSeriesAsync(SeriesCreateRequest request, CancellationToken cancellationToken = default);
    Task<Series> UpdateSeriesAsync(string id, SeriesUpdateRequest request, CancellationToken cancellationToken = default);
    Task DeleteSeriesAsync(string id, CancellationToken cancellationToken = default);
    
    // Speaker management
    Task<Speaker> GetSpeakerAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Speaker>> ListSpeakersAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Speaker> CreateSpeakerAsync(SpeakerCreateRequest request, CancellationToken cancellationToken = default);
    Task<Speaker> UpdateSpeakerAsync(string id, SpeakerUpdateRequest request, CancellationToken cancellationToken = default);
    Task DeleteSpeakerAsync(string id, CancellationToken cancellationToken = default);
    
    // Speakership management
    Task<IPagedResponse<Speakership>> ListSpeakershipsAsync(string episodeId, QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Speakership>> ListSpeakerEpisodesAsync(string speakerId, QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Speakership> AddSpeakerToEpisodeAsync(string episodeId, string speakerId, CancellationToken cancellationToken = default);
    Task DeleteSpeakershipAsync(string id, CancellationToken cancellationToken = default);
    
    // Media management
    Task<Media> GetMediaAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Media>> ListMediaAsync(string episodeId, QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Media> UploadMediaAsync(string episodeId, MediaUploadRequest request, CancellationToken cancellationToken = default);
    Task<Media> UpdateMediaAsync(string id, MediaUpdateRequest request, CancellationToken cancellationToken = default);
    Task DeleteMediaAsync(string id, CancellationToken cancellationToken = default);
    
    // Content publishing
    Task<Episode> PublishEpisodeAsync(string id, CancellationToken cancellationToken = default);
    Task<Episode> UnpublishEpisodeAsync(string id, CancellationToken cancellationToken = default);
    Task<Series> PublishSeriesAsync(string id, CancellationToken cancellationToken = default);
    Task<Series> UnpublishSeriesAsync(string id, CancellationToken cancellationToken = default);
    
    // Content distribution
    Task<IPagedResponse<DistributionChannel>> ListDistributionChannelsAsync(CancellationToken cancellationToken = default);
    Task<DistributionResult> DistributeEpisodeAsync(string episodeId, string channelId, CancellationToken cancellationToken = default);
    Task<DistributionResult> DistributeSeriesAsync(string seriesId, string channelId, CancellationToken cancellationToken = default);
    
    // Analytics and reporting
    Task<EpisodeAnalytics> GetEpisodeAnalyticsAsync(string episodeId, AnalyticsRequest request, CancellationToken cancellationToken = default);
    Task<SeriesAnalytics> GetSeriesAnalyticsAsync(string seriesId, AnalyticsRequest request, CancellationToken cancellationToken = default);
    Task<PublishingReport> GeneratePublishingReportAsync(PublishingReportRequest request, CancellationToken cancellationToken = default);
}
```

## Fluent API Interface

```csharp
public interface IPublishingFluentContext
{
    // Episode queries
    IEpisodeFluentContext Episodes();
    IEpisodeFluentContext Episode(string episodeId);
    
    // Series queries
    ISeriesFluentContext Series();
    ISeriesFluentContext Series(string seriesId);
    
    // Speaker queries
    ISpeakerFluentContext Speakers();
    ISpeakerFluentContext Speaker(string speakerId);
    
    // Media queries
    IMediaFluentContext Media();
    IMediaFluentContext Media(string mediaId);
    
    // Analytics and reporting
    IPublishingAnalyticsFluentContext Analytics();
    IPublishingReportingFluentContext Reports();
}

public interface IEpisodeFluentContext
{
    IEpisodeFluentContext Where(Expression<Func<Episode, bool>> predicate);
    IEpisodeFluentContext Include(Expression<Func<Episode, object>> include);
    IEpisodeFluentContext OrderBy(Expression<Func<Episode, object>> orderBy);
    IEpisodeFluentContext OrderByDescending(Expression<Func<Episode, object>> orderBy);
    IEpisodeFluentContext InSeries(string seriesId);
    IEpisodeFluentContext BySpeaker(string speakerId);
    IEpisodeFluentContext Published();
    IEpisodeFluentContext Unpublished();
    IEpisodeFluentContext PublishedAfter(DateTime date);
    IEpisodeFluentContext PublishedBefore(DateTime date);
    IEpisodeFluentContext WithVideo();
    IEpisodeFluentContext WithAudio();
    
    // Episode-specific operations
    ISpeakerFluentContext Speakers();
    IMediaFluentContext Media();
    
    Task<Episode> GetAsync(CancellationToken cancellationToken = default);
    Task<IPagedResponse<Episode>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default);
    Task<List<Episode>> GetAllAsync(CancellationToken cancellationToken = default);
}

public interface ISeriesFluentContext
{
    ISeriesFluentContext Where(Expression<Func<Series, bool>> predicate);
    ISeriesFluentContext Include(Expression<Func<Series, object>> include);
    ISeriesFluentContext OrderBy(Expression<Func<Series, object>> orderBy);
    ISeriesFluentContext OrderByDescending(Expression<Func<Series, object>> orderBy);
    ISeriesFluentContext Published();
    ISeriesFluentContext Unpublished();
    ISeriesFluentContext PublishedAfter(DateTime date);
    ISeriesFluentContext PublishedBefore(DateTime date);
    
    // Series-specific operations
    IEpisodeFluentContext Episodes();
    
    Task<Series> GetAsync(CancellationToken cancellationToken = default);
    Task<IPagedResponse<Series>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default);
    Task<List<Series>> GetAllAsync(CancellationToken cancellationToken = default);
}

public interface ISpeakerFluentContext
{
    ISpeakerFluentContext Where(Expression<Func<Speaker, bool>> predicate);
    ISpeakerFluentContext Include(Expression<Func<Speaker, object>> include);
    ISpeakerFluentContext OrderBy(Expression<Func<Speaker, object>> orderBy);
    
    // Speaker-specific operations
    IEpisodeFluentContext Episodes();
    
    Task<Speaker> GetAsync(CancellationToken cancellationToken = default);
    Task<IPagedResponse<Speaker>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default);
    Task<List<Speaker>> GetAllAsync(CancellationToken cancellationToken = default);
}
```

## Usage Examples

### Service-Based API
```csharp
// Get recent episodes
var recentEpisodes = await publishingService.ListEpisodesAsync(new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["published_at"] = $">={DateTime.Now.AddDays(-30):yyyy-MM-dd}"
    },
    Include = new[] { "series", "speakerships", "speakers" },
    OrderBy = "-published_at"
});

// Create a new series
var newSeries = await publishingService.CreateSeriesAsync(new SeriesCreateRequest
{
    Title = "Faith in Action",
    Description = "A series exploring practical faith in daily life",
    ArtworkUrl = "https://example.com/series-artwork.jpg"
});

// Create a new episode
var newEpisode = await publishingService.CreateEpisodeAsync(new EpisodeCreateRequest
{
    Title = "Living with Purpose",
    Description = "Discovering God's purpose for your life",
    SeriesId = newSeries.Id,
    Notes = "Episode transcript and study notes...",
    VideoUrl = "https://example.com/video.mp4",
    AudioUrl = "https://example.com/audio.mp3"
});

// Add a speaker to an episode
var speaker = await publishingService.CreateSpeakerAsync(new SpeakerCreateRequest
{
    Name = "Pastor John Smith",
    Bio = "Lead Pastor with 15 years of ministry experience",
    PhotoUrl = "https://example.com/pastor-photo.jpg"
});

var speakership = await publishingService.AddSpeakerToEpisodeAsync(newEpisode.Id, speaker.Id);

// Upload media file
var media = await publishingService.UploadMediaAsync(newEpisode.Id, new MediaUploadRequest
{
    Filename = "sermon-video.mp4",
    ContentType = "video/mp4",
    FileData = videoFileBytes,
    Downloadable = true,
    Streamable = true
});

// Publish the episode
var publishedEpisode = await publishingService.PublishEpisodeAsync(newEpisode.Id);
```

### Fluent API
```csharp
// Complex episode query
var sermonEpisodes = await client
    .Publishing()
    .Episodes()
    .Published()
    .PublishedAfter(DateTime.Now.AddMonths(-6))
    .InSeries("sermon-series-id")
    .WithVideo()
    .Include(e => e.Series)
    .Include(e => e.Speakers)
    .OrderByDescending(e => e.PublishedAt)
    .GetPagedAsync(pageSize: 50);

// Get all episodes by a specific speaker
var speakerEpisodes = await client
    .Publishing()
    .Speaker("speaker123")
    .Episodes()
    .Published()
    .Include(e => e.Series)
    .OrderByDescending(e => e.PublishedAt)
    .GetAllAsync();

// Series-specific operations
var seriesEpisodes = await client
    .Publishing()
    .Series("series123")
    .Episodes()
    .Published()
    .OrderBy(e => e.PublishedAt)
    .GetAllAsync();

// Media management
var episodeMedia = await client
    .Publishing()
    .Episode("episode123")
    .Media()
    .Where(m => m.ContentType.StartsWith("video/"))
    .GetAllAsync();

// Content discovery
var recentSeries = await client
    .Publishing()
    .Series()
    .Published()
    .PublishedAfter(DateTime.Now.AddMonths(-3))
    .Include(s => s.Episodes)
    .OrderByDescending(s => s.PublishedAt)
    .GetAllAsync();

// Speaker management
var activeSpeakers = await client
    .Publishing()
    .Speakers()
    .Where(s => s.Episodes.Any(e => e.PublishedAt > DateTime.Now.AddYears(-1)))
    .OrderBy(s => s.Name)
    .GetAllAsync();

// Analytics queries
var popularEpisodes = await client
    .Publishing()
    .Episodes()
    .Published()
    .PublishedAfter(DateTime.Now.AddMonths(-1))
    .OrderByDescending(e => e.ViewCount)
    .Take(10)
    .GetAllAsync();
```

## Implementation Notes

### Data Mapping
- Handle media URLs and file metadata properly
- Preserve content hierarchy (series -> episodes -> media)
- Map speaker relationships and biographical information

### Security Considerations
- Implement proper authorization for content management
- Secure media file uploads and storage
- Protect unpublished content from unauthorized access
- Handle media streaming permissions

### Caching Strategy
- Cache published content for performance
- Avoid caching unpublished or draft content
- Use CDN for media file delivery
- Cache speaker and series metadata

### Error Handling
- Handle media upload failures gracefully
- Validate media file types and sizes
- Handle publishing workflow errors
- Manage content distribution failures

### Performance Considerations
- Optimize media file handling and streaming
- Implement efficient content queries
- Use pagination for large content libraries
- Consider read replicas for content delivery

### Media Management
- Support for multiple media formats (video, audio, images)
- Automatic transcoding and optimization
- Media file versioning and backup
- Streaming optimization and CDN integration

### Content Distribution
- Integration with podcast platforms
- Social media sharing capabilities
- RSS feed generation
- Content syndication and API access

### Analytics and Reporting
- View and download tracking
- Engagement metrics and analytics
- Content performance reporting
- Audience insights and demographics

This module provides comprehensive content management capabilities while maintaining security and performance standards for media publishing systems.