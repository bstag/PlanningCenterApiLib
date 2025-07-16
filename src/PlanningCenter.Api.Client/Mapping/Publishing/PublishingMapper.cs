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

    #region Series Mapping (Placeholder)

    /// <summary>
    /// Maps a SeriesDto to a Series domain model.
    /// </summary>
    public static Series MapToDomain(SeriesDto dto)
    {
        return new Series
        {
            Id = dto.Id,
            Title = dto.Attributes.Title,
            DataSource = "Publishing"
        };
    }

    #endregion

    #region Speaker Mapping (Placeholder)

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
            DataSource = "Publishing"
        };
    }

    #endregion

    #region Media Mapping (Placeholder)

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
            ContentType = dto.Attributes.ContentType ?? "application/octet-stream",
            MediaType = dto.Attributes.MediaType ?? "unknown",
            DataSource = "Publishing"
        };
    }

    #endregion
}