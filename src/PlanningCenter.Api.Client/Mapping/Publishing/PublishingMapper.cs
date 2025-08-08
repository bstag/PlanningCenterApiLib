using PlanningCenter.Api.Client.Models.JsonApi.Core;
using PlanningCenter.Api.Client.Models.JsonApi.Publishing;
using PlanningCenter.Api.Client.Models.Publishing;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Models.JsonApi;
using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;

namespace PlanningCenter.Api.Client.Mapping.Publishing;

/// <summary>
/// Mapper for Publishing entities between domain models and DTOs.
/// </summary>
public static class PublishingMapper
{
    #region Episode Mapping

    /// <summary>
    /// Maps an EpisodeDto to an Episode domain model.
    /// </summary>
    public static Episode MapToDomain(EpisodeDto dto)
    {
        return new Episode
        {
            Id = dto.Id,
            Title = dto.Attributes.Title,
            Description = dto.Attributes.Description,
            PublishedAt = dto.Attributes.PublishedAt,
            LengthInSeconds = dto.Attributes.LengthInSeconds,
            VideoUrl = dto.Attributes.VideoUrl,
            AudioUrl = dto.Attributes.AudioUrl,
            VideoDownloadUrl = dto.Attributes.VideoDownloadUrl,
            AudioDownloadUrl = dto.Attributes.AudioDownloadUrl,
            VideoFileSizeInBytes = dto.Attributes.VideoFileSizeInBytes,
            AudioFileSizeInBytes = dto.Attributes.AudioFileSizeInBytes,
            VideoContentType = dto.Attributes.VideoContentType,
            AudioContentType = dto.Attributes.AudioContentType,
            ArtworkUrl = dto.Attributes.ArtworkUrl,
            ArtworkFileSizeInBytes = dto.Attributes.ArtworkFileSizeInBytes,
            ArtworkContentType = dto.Attributes.ArtworkContentType,
            Notes = dto.Attributes.Notes,
            SeriesId = dto.Relationships?.Series?.Id,
            IsPublished = dto.Attributes.IsPublished,
            Status = dto.Attributes.Status,
            EpisodeNumber = dto.Attributes.EpisodeNumber,
            SeasonNumber = dto.Attributes.SeasonNumber,
            Tags = dto.Attributes.Tags,
            Categories = dto.Attributes.Categories,
            ViewCount = dto.Attributes.ViewCount,
            DownloadCount = dto.Attributes.DownloadCount,
            LikeCount = dto.Attributes.LikeCount,
            Metadata = dto.Attributes.Metadata,
            CreatedAt = dto.Attributes.CreatedAt,
            UpdatedAt = dto.Attributes.UpdatedAt,
            DataSource = "Publishing"
        };
    }

    /// <summary>
    /// Maps an EpisodeCreateRequest to a JSON:API request.
    /// </summary>
    public static JsonApiRequest<EpisodeCreateDto> MapCreateRequestToJsonApi(EpisodeCreateRequest request)
    {
        var dto = new EpisodeCreateDto
        {
            Type = "Episode",
            Attributes = new EpisodeCreateAttributesDto
            {
                Title = request.Title,
                Description = request.Description,
                PublishedAt = request.PublishedAt,
                LengthInSeconds = request.LengthInSeconds,
                Notes = request.Notes,
                EpisodeNumber = request.EpisodeNumber,
                SeasonNumber = request.SeasonNumber,
                Tags = request.Tags,
                Categories = request.Categories
            }
        };

        if (!string.IsNullOrEmpty(request.SeriesId))
        {
            dto.Relationships = new EpisodeCreateRelationshipsDto
            {
                Series = new RelationshipData { Type = "Series", Id = request.SeriesId }
            };
        }

        return new JsonApiRequest<EpisodeCreateDto> { Data = dto };
    }

    /// <summary>
    /// Maps an EpisodeUpdateRequest to a JSON:API request.
    /// </summary>
    public static JsonApiRequest<EpisodeUpdateDto> MapUpdateRequestToJsonApi(string id, EpisodeUpdateRequest request)
    {
        var dto = new EpisodeUpdateDto
        {
            Type = "Episode",
            Id = id,
            Attributes = new EpisodeUpdateAttributesDto
            {
                Title = request.Title,
                Description = request.Description,
                PublishedAt = request.PublishedAt,
                LengthInSeconds = request.LengthInSeconds,
                Notes = request.Notes,
                EpisodeNumber = request.EpisodeNumber,
                SeasonNumber = request.SeasonNumber,
                Tags = request.Tags,
                Categories = request.Categories,
                IsPublished = request.IsPublished
            }
        };

        return new JsonApiRequest<EpisodeUpdateDto> { Data = dto };
    }

    #endregion

    #region Series Mapping

    /// <summary>
    /// Maps a SeriesDto to a Series domain model.
    /// </summary>
    public static Series MapToDomain(SeriesDto dto)
    {
        return new Series
        {
            Id = dto.Id,
            Title = dto.Attributes.Title,
            Description = dto.Attributes.Description,
            Summary = dto.Attributes.Summary,
            PublishedAt = dto.Attributes.PublishedAt,
            StartDate = dto.Attributes.StartDate,
            EndDate = dto.Attributes.EndDate,
            ArtworkUrl = dto.Attributes.ArtworkUrl,
            ArtworkFileSizeInBytes = dto.Attributes.ArtworkFileSizeInBytes,
            ArtworkContentType = dto.Attributes.ArtworkContentType,
            IsPublished = dto.Attributes.IsPublished,
            Status = dto.Attributes.Status,
            SeriesType = dto.Attributes.SeriesType,
            Language = dto.Attributes.Language,
            Tags = dto.Attributes.Tags,
            Categories = dto.Attributes.Categories,
            EpisodeCount = dto.Attributes.EpisodeCount,
            TotalViewCount = dto.Attributes.TotalViewCount,
            TotalDownloadCount = dto.Attributes.TotalDownloadCount,
            SubscriberCount = dto.Attributes.SubscriberCount,
            Author = dto.Attributes.Author,
            Copyright = dto.Attributes.Copyright,
            WebsiteUrl = dto.Attributes.WebsiteUrl,
            RssFeedUrl = dto.Attributes.RssFeedUrl,
            IsExplicit = dto.Attributes.IsExplicit,
            Metadata = dto.Attributes.Metadata,
            CreatedAt = dto.Attributes.CreatedAt,
            UpdatedAt = dto.Attributes.UpdatedAt,
            DataSource = "Publishing"
        };
    }

    /// <summary>
    /// Maps a SeriesCreateRequest to a JSON:API request.
    /// </summary>
    public static JsonApiRequest<SeriesCreateDto> MapCreateRequestToJsonApi(SeriesCreateRequest request)
    {
        var dto = new SeriesCreateDto
        {
            Type = "Series",
            Attributes = new SeriesCreateAttributesDto
            {
                Title = request.Title,
                Description = request.Description,
                Summary = request.Summary,
                PublishedAt = request.PublishedAt,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                SeriesType = request.SeriesType,
                Language = request.Language,
                Tags = request.Tags,
                Categories = request.Categories,
                Author = request.Author,
                Copyright = request.Copyright,
                WebsiteUrl = request.WebsiteUrl,
                IsExplicit = request.IsExplicit
            }
        };

        return new JsonApiRequest<SeriesCreateDto> { Data = dto };
    }

    /// <summary>
    /// Maps a SeriesUpdateRequest to a JSON:API request.
    /// </summary>
    public static JsonApiRequest<SeriesUpdateDto> MapUpdateRequestToJsonApi(string id, SeriesUpdateRequest request)
    {
        var dto = new SeriesUpdateDto
        {
            Type = "Series",
            Id = id,
            Attributes = new SeriesUpdateAttributesDto
            {
                Title = request.Title,
                Description = request.Description,
                Summary = request.Summary,
                PublishedAt = request.PublishedAt,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                SeriesType = request.SeriesType,
                Language = request.Language,
                Tags = request.Tags,
                Categories = request.Categories,
                Author = request.Author,
                Copyright = request.Copyright,
                WebsiteUrl = request.WebsiteUrl,
                IsExplicit = request.IsExplicit,
                IsPublished = request.IsPublished
            }
        };

        return new JsonApiRequest<SeriesUpdateDto> { Data = dto };
    }

    #endregion

    #region Speaker Mapping

    /// <summary>
    /// Maps a SpeakerDto to a Speaker domain model.
    /// </summary>
    public static Speaker MapToDomain(SpeakerDto dto)
    {
        return new Speaker
        {
            Id = dto.Id,
            FirstName = dto.Attributes.FirstName,
            LastName = dto.Attributes.LastName,
            DisplayName = dto.Attributes.DisplayName,
            Title = dto.Attributes.Title,
            Biography = dto.Attributes.Biography,
            Email = dto.Attributes.Email,
            PhoneNumber = dto.Attributes.PhoneNumber,
            WebsiteUrl = dto.Attributes.WebsiteUrl,
            PhotoUrl = dto.Attributes.PhotoUrl,
            Organization = dto.Attributes.Organization,
            Location = dto.Attributes.Location,
            Active = dto.Attributes.Active,
            CreatedAt = dto.Attributes.CreatedAt,
            UpdatedAt = dto.Attributes.UpdatedAt,
            DataSource = "Publishing"
        };
    }

    /// <summary>
    /// Maps a SpeakerCreateRequest to a JSON:API request.
    /// </summary>
    public static JsonApiRequest<SpeakerCreateDto> MapCreateRequestToJsonApi(SpeakerCreateRequest request)
    {
        var dto = new SpeakerCreateDto
        {
            Type = "Speaker",
            Attributes = new SpeakerCreateAttributesDto
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                DisplayName = request.DisplayName,
                Title = request.Title,
                Biography = request.Biography,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                WebsiteUrl = request.WebsiteUrl,
                PhotoUrl = request.PhotoUrl,
                Organization = request.Organization,
                Location = request.Location,
                Active = request.Active
            }
        };

        return new JsonApiRequest<SpeakerCreateDto> { Data = dto };
    }

    /// <summary>
    /// Maps a SpeakerUpdateRequest to a JSON:API request.
    /// </summary>
    public static JsonApiRequest<SpeakerUpdateDto> MapUpdateRequestToJsonApi(string id, SpeakerUpdateRequest request)
    {
        var dto = new SpeakerUpdateDto
        {
            Type = "Speaker",
            Id = id,
            Attributes = new SpeakerUpdateAttributesDto
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                DisplayName = request.DisplayName,
                Title = request.Title,
                Biography = request.Biography,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                WebsiteUrl = request.WebsiteUrl,
                PhotoUrl = request.PhotoUrl,
                Organization = request.Organization,
                Location = request.Location,
                Active = request.Active
            }
        };

        return new JsonApiRequest<SpeakerUpdateDto> { Data = dto };
    }

    #endregion

    #region Media Mapping

    /// <summary>
    /// Maps a MediaDto to a Media domain model.
    /// </summary>
    public static Media MapToDomain(MediaDto dto)
    {
        return new Media
        {
            Id = dto.Id,
            FileName = dto.Attributes.FileName,
            FileUrl = dto.Attributes.Url ?? string.Empty,
            DownloadUrl = dto.Attributes.DownloadUrl,
            FileSizeInBytes = dto.Attributes.FileSizeInBytes ?? 0,
            ContentType = dto.Attributes.ContentType ?? "application/octet-stream",
            MediaType = dto.Attributes.MediaType ?? "unknown",
            Quality = dto.Attributes.Quality,
            IsPrimary = dto.Attributes?.IsPrimary ?? false,
            CreatedAt = dto.Attributes?.CreatedAt ?? DateTime.MinValue,
            UpdatedAt = dto.Attributes?.UpdatedAt ?? DateTime.MinValue,
            DataSource = "Publishing"
        };
    }

    /// <summary>
    /// Maps a MediaUploadRequest to a JSON:API request.
    /// </summary>
    public static JsonApiRequest<MediaCreateDto> MapMediaUploadToJsonApi(string episodeId, MediaUploadRequest request)
    {
        var dto = new MediaCreateDto
        {
            Type = "Media",
            Attributes = new MediaCreateAttributesDto
            {
                FileName = request.FileName,
                ContentType = request.ContentType,
                FileSizeInBytes = request.FileSizeInBytes,
                MediaType = request.MediaType,
                Quality = request.Quality,
                IsPrimary = request.IsPrimary
            },
            Relationships = new MediaCreateRelationshipsDto
            {
                Episode = new RelationshipData { Type = "Episode", Id = episodeId }
            }
        };

        return new JsonApiRequest<MediaCreateDto> { Data = dto };
    }

    /// <summary>
    /// Maps a MediaUpdateRequest to a JSON:API request.
    /// </summary>
    public static JsonApiRequest<MediaUpdateDto> MapMediaUpdateToJsonApi(string id, MediaUpdateRequest request)
    {
        var dto = new MediaUpdateDto
        {
            Type = "Media",
            Id = id,
            Attributes = new MediaUpdateAttributesDto
            {
                FileName = request.FileName,
                Quality = request.Quality,
                IsPrimary = request.IsPrimary
            }
        };

        return new JsonApiRequest<MediaUpdateDto> { Data = dto };
    }

    #endregion

    #region Speakership Mapping

    /// <summary>
    /// Maps a SpeakershipDto to a Speakership domain model.
    /// </summary>
    public static Speakership MapToDomain(SpeakershipDto dto)
    {
        return new Speakership
        {
            Id = dto.Id,
            EpisodeId = dto.Relationships?.Episode?.Id ?? string.Empty,
            SpeakerId = dto.Relationships?.Speaker?.Id ?? string.Empty,
            Role = dto.Attributes?.Role,
            Notes = dto.Attributes?.Notes,
            SortOrder = dto.Attributes?.Order ?? 0,
            IsPrimary = dto.Attributes?.IsPrimary ?? false,
            CreatedAt = dto.Attributes?.CreatedAt ?? DateTime.MinValue,
            UpdatedAt = dto.Attributes?.UpdatedAt ?? DateTime.MinValue,
            DataSource = "Publishing"
        };
    }

    /// <summary>
    /// Maps speakership creation parameters to a JSON:API request.
    /// </summary>
    public static JsonApiRequest<SpeakershipCreateDto> MapSpeakershipCreateToJsonApi(string episodeId, string speakerId, string? role = null)
    {
        var dto = new SpeakershipCreateDto
        {
            Type = "Speakership",
            Attributes = new SpeakershipCreateAttributesDto
            {
                Role = role,
                IsPrimary = false
            },
            Relationships = new SpeakershipCreateRelationshipsDto
            {
                Episode = new RelationshipData { Type = "Episode", Id = episodeId },
                Speaker = new RelationshipData { Type = "Speaker", Id = speakerId }
            }
        };

        return new JsonApiRequest<SpeakershipCreateDto> { Data = dto };
    }

    #endregion

    #region Analytics Mapping

    /// <summary>
    /// Maps an EpisodeAnalyticsDto to an EpisodeAnalytics domain model.
    /// </summary>
    public static EpisodeAnalytics MapToDomain(EpisodeAnalyticsDto dto)
    {
        return new EpisodeAnalytics
        {
            EpisodeId = dto.Attributes?.EpisodeId ?? string.Empty,
            ViewCount = dto.Attributes?.ViewCount ?? 0,
            DownloadCount = dto.Attributes?.DownloadCount ?? 0,
            AverageWatchTimeSeconds = dto.Attributes?.AverageWatchTimeSeconds ?? 0.0,
            PeriodStart = dto.Attributes?.PeriodStart?.DateTime ?? DateTime.MinValue,
            PeriodEnd = dto.Attributes?.PeriodEnd?.DateTime ?? DateTime.MinValue,
            AdditionalData = dto.Attributes?.AdditionalData ?? new Dictionary<string, object>()
        };
    }

    /// <summary>
    /// Maps a SeriesAnalyticsDto to a SeriesAnalytics domain model.
    /// </summary>
    public static SeriesAnalytics MapToDomain(SeriesAnalyticsDto dto)
    {
        return new SeriesAnalytics
        {
            SeriesId = dto.Id ?? string.Empty,
            TotalViewCount = dto.Attributes?.TotalViews ?? 0,
            TotalDownloadCount = dto.Attributes?.TotalDownloads ?? 0,
            EpisodeCount = 0, // Placeholder
            PeriodStart = DateTime.MinValue, // Placeholder
            PeriodEnd = DateTime.MinValue, // Placeholder
            AdditionalData = new Dictionary<string, object>() // Placeholder
        };
    }

    #endregion

    #region Distribution Mapping

    /// <summary>
    /// Maps a DistributionDto to a DistributionResult domain model.
    /// </summary>
    public static DistributionResult MapToDomain(DistributionDto dto)
    {
        return new DistributionResult
        {
            Success = dto.Attributes?.Success ?? false,
            Message = dto.Attributes?.Message ?? string.Empty,
            DistributedAt = dto.Attributes?.DistributedAt?.DateTime ?? DateTime.MinValue,
            ExternalUrl = dto.Attributes?.ExternalUrl,
            Metadata = dto.Attributes?.Metadata ?? new Dictionary<string, object>()
        };
    }

    /// <summary>
    /// Maps distribution parameters to a JSON:API request.
    /// </summary>
    public static JsonApiRequest<DistributionCreateDto> MapDistributionCreateToJsonApi(string? episodeId, string? seriesId, string channelId)
    {
        var dto = new DistributionCreateDto
        {
            Type = "Distribution",
            Attributes = new DistributionCreateAttributesDto
            {
                EpisodeId = episodeId,
                SeriesId = seriesId,
                ChannelId = channelId
            }
        };

        return new JsonApiRequest<DistributionCreateDto> { Data = dto };
    }

    #endregion

    #region Channel Mapping

    /// <summary>
    /// Maps a ChannelDto to a DistributionChannel domain model.
    /// </summary>
    public static DistributionChannel MapToDomain(ChannelDto dto)
    {
        return new DistributionChannel
        {
            Id = dto.Id,
            Name = dto.Attributes?.Name ?? "Unknown Channel",
            ChannelType = "unknown", // Would need to be determined from attributes
            Active = dto.Attributes?.Published ?? false,
            Description = dto.Attributes?.Description,
            ChannelUrl = dto.Attributes?.Url,
            DataSource = "Publishing"
        };
    }

    #endregion
}