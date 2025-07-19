# Publishing Module Documentation

The Publishing module provides comprehensive media and content management capabilities for Planning Center. This module handles all publishing-related functionality including media, series, episodes, and speakers.

## 📋 Module Overview

### Key Entities
- **Media**: Audio, video, and other media content
- **Series**: Collections of related episodes or content
- **Episodes**: Individual pieces of content within series
- **Speakers**: People who present or teach content
- **Speakerships**: Relationships between speakers and episodes

### Authentication
Requires Planning Center Publishing app access with appropriate permissions.

## 🔧 Traditional Service API

### Media Management

#### Basic Media Operations
```csharp
public class PublishingService
{
    private readonly IPublishingService _publishingService;
    
    public PublishingService(IPublishingService publishingService)
    {
        _publishingService = publishingService;
    }
    
    // Get media by ID
    public async Task<Media?> GetMediaAsync(string id)
    {
        return await _publishingService.GetMediaAsync(id);
    }
    
    // List media with pagination
    public async Task<IPagedResponse<Media>> GetMediaAsync()
    {
        return await _publishingService.ListMediaAsync(new QueryParameters
        {
            PerPage = 25,
            OrderBy = "-created_at"
        });
    }
}
```

#### Create and Update Media
```csharp
// Create new media
var newMedia = await _publishingService.CreateMediaAsync(new MediaCreateRequest
{
    Title = "Sunday Morning Message",
    Description = "Weekly teaching from our pastor",
    MediaType = "audio",
    PublishedAt = DateTime.Now
});

// Update media
var updatedMedia = await _publishingService.UpdateMediaAsync("media123", new MediaUpdateRequest
{
    Title = "Sunday Morning Teaching",
    Description = "Updated description with more details"
});
```

### Series Management

#### Series Operations
```csharp
// Get a series by ID
var series = await _publishingService.GetSeriesAsync("series123");

// List series
var seriesList = await _publishingService.ListSeriesAsync(new QueryParameters
{
    OrderBy = "-created_at"
});

// Create a new series
var newSeries = await _publishingService.CreateSeriesAsync(new SeriesCreateRequest
{
    Title = "Faith Foundations",
    Description = "A series exploring the basics of Christian faith",
    PublishedAt = DateTime.Now
});
```

### Episode Management

#### Episode Operations
```csharp
// Get an episode by ID
var episode = await _publishingService.GetEpisodeAsync("episode123");

// List episodes for a series
var seriesEpisodes = await _publishingService.ListEpisodesAsync(new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["series_id"] = "series123"
    },
    OrderBy = "position"
});

// Create a new episode
var newEpisode = await _publishingService.CreateEpisodeAsync(new EpisodeCreateRequest
{
    SeriesId = "series123",
    Title = "The Foundation of Faith",
    Description = "Exploring what it means to have faith",
    Position = 1,
    PublishedAt = DateTime.Now
});
```

## 🚀 Fluent API Usage

### Basic Queries
```csharp
public class PublishingFluentService
{
    private readonly IPlanningCenterClient _client;
    
    public PublishingFluentService(IPlanningCenterClient client)
    {
        _client = client;
    }
    
    // Get published media
    public async Task<IReadOnlyList<Media>> GetPublishedMediaAsync()
    {
        return await _client.Publishing()
            .Where(m => m.PublishedAt.HasValue)
            .OrderByDescending(m => m.PublishedAt)
            .GetAllAsync();
    }
}
```

### Advanced Media Filtering
```csharp
// Get audio content
var audioMedia = await _client.Publishing()
    .Where(m => m.MediaType == "audio")
    .Where(m => m.PublishedAt.HasValue)
    .OrderByDescending(m => m.PublishedAt)
    .GetAllAsync();

// Get recent episodes
var recentEpisodes = await _client.Publishing()
    .Episodes()
    .Where(e => e.PublishedAt >= DateTime.Today.AddDays(-30))
    .OrderByDescending(e => e.PublishedAt)
    .GetPagedAsync(pageSize: 50);
```

## 💡 Common Use Cases

### 1. Content Library
```csharp
public async Task<ContentLibrary> GetContentLibraryAsync()
{
    var series = await _client.Publishing()
        .Series()
        .Where(s => s.PublishedAt.HasValue)
        .OrderByDescending(s => s.PublishedAt)
        .GetAllAsync();
    
    var episodes = await _client.Publishing()
        .Episodes()
        .Where(e => e.PublishedAt.HasValue)
        .OrderByDescending(e => e.PublishedAt)
        .GetAllAsync();
    
    return new ContentLibrary
    {
        TotalSeries = series.Count,
        TotalEpisodes = episodes.Count,
        RecentSeries = series.Take(5).ToList(),
        RecentEpisodes = episodes.Take(10).ToList()
    };
}
```

### 2. Speaker Directory
```csharp
public async Task<SpeakerDirectory> GetSpeakerDirectoryAsync()
{
    var speakers = await _client.Publishing()
        .Speakers()
        .OrderBy(s => s.Name)
        .GetAllAsync();
    
    return new SpeakerDirectory
    {
        TotalSpeakers = speakers.Count,
        Speakers = speakers.Select(s => new SpeakerInfo
        {
            Id = s.Id,
            Name = s.Name,
            Bio = s.Bio
        }).ToList()
    };
}
```

## 🎯 Best Practices

1. **Organize with Series**: Group related content into series
2. **Track Publication Status**: Monitor published vs draft content
3. **Manage Speaker Information**: Maintain accurate speaker details
4. **Order Content Properly**: Use position fields for episode ordering
5. **Handle Media Types**: Properly categorize audio, video, and other media

This Publishing module provides essential media and content management capabilities.