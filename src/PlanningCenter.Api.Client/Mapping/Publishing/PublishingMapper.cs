using PlanningCenter.Api.Client.Models.JsonApi.Core;
using PlanningCenter.Api.Client.Models.JsonApi.Publishing;
using PlanningCenter.Api.Client.Models.Publishing;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Models.JsonApi;

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
            IsPrimary = dto.Attributes.IsPrimary,
            CreatedAt = dto.Attributes.CreatedAt,
            UpdatedAt = dto.Attributes.UpdatedAt,
            DataSource = "Publishing"
        };
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
            Role = dto.Attributes.Role,
            Notes = dto.Attributes.Notes,
            SortOrder = dto.Attributes.Order ?? 0,
            IsPrimary = dto.Attributes.IsPrimary,
            CreatedAt = dto.Attributes.CreatedAt,
            UpdatedAt = dto.Attributes.UpdatedAt,
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
}