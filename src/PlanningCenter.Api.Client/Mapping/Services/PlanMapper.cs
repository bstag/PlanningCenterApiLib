using PlanningCenter.Api.Client.Models.JsonApi.Services;
using PlanningCenter.Api.Client.Models.Services;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Models.JsonApi;
using PlanningCenter.Api.Client.Models.JsonApi.Core;
using System;

namespace PlanningCenter.Api.Client.Mapping.Services;

/// <summary>
/// Mapper for converting between Plan DTOs and domain models.
/// Follows DRY principle by centralizing all plan mapping logic.
/// Follows Single Responsibility Principle - handles only plan mapping.
/// </summary>
public static class PlanMapper
{
    /// <summary>
    /// Maps a PlanDto to a Plan domain model.
    /// </summary>
    /// <param name="dto">The DTO to map from</param>
    /// <returns>The mapped domain model</returns>
    /// <exception cref="ArgumentNullException">Thrown when dto is null</exception>
    public static Plan MapToDomain(PlanDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        return new Plan
        {
            Id = dto.Id,
            Title = dto.Attributes.Title ?? string.Empty,
            Dates = dto.Attributes.Dates,
            ShortDates = dto.Attributes.ShortDates,
            PlanningCenterUrl = dto.Attributes.PlanningCenterUrl,
            SortDate = dto.Attributes.SortDate,
            IsPublic = dto.Attributes.IsPublic,
            RehearsalTimes = dto.Attributes.RehearsalTimes,
            ServiceTimes = dto.Attributes.ServiceTimes,
            OtherTimes = dto.Attributes.OtherTimes,
            Length = dto.Attributes.Length,
            Notes = dto.Attributes.Notes,
            ServiceTypeId = dto.Relationships?.ServiceType?.Id,
            CreatedAt = dto.Attributes.CreatedAt,
            UpdatedAt = dto.Attributes.UpdatedAt,
            DataSource = "Services"
        };
    }

    /// <summary>
    /// Maps a ServiceTypeDto to a ServiceType domain model.
    /// </summary>
    /// <param name="dto">The DTO to map from</param>
    /// <returns>The mapped domain model</returns>
    /// <exception cref="ArgumentNullException">Thrown when dto is null</exception>
    public static ServiceType MapServiceTypeToDomain(ServiceTypeDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        return new ServiceType
        {
            Id = dto.Id,
            Name = dto.Attributes.Name ?? string.Empty,
            Sequence = dto.Attributes.Sequence,
            CreatedAt = dto.Attributes.CreatedAt,
            UpdatedAt = dto.Attributes.UpdatedAt,
            DataSource = "Services"
        };
    }

    /// <summary>
    /// Maps an ItemDto to an Item domain model.
    /// </summary>
    /// <param name="dto">The DTO to map from</param>
    /// <returns>The mapped domain model</returns>
    /// <exception cref="ArgumentNullException">Thrown when dto is null</exception>
    public static Item MapItemToDomain(ItemDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        return new Item
        {
            Id = dto.Id,
            Title = dto.Attributes.Title ?? string.Empty,
            Sequence = dto.Attributes.Sequence,
            ItemType = dto.Attributes.ItemType,
            Description = dto.Attributes.Description,
            KeyName = dto.Attributes.KeyName,
            Length = dto.Attributes.Length,
            ServicePosition = dto.Attributes.ServicePosition,
            PlanId = dto.Relationships?.Plan?.Id ?? string.Empty,
            SongId = dto.Relationships?.Song?.Id,
            ArrangementId = dto.Relationships?.Arrangement?.Id,
            CreatedAt = dto.Attributes.CreatedAt,
            UpdatedAt = dto.Attributes.UpdatedAt,
            DataSource = "Services"
        };
    }

    /// <summary>
    /// Maps a SongDto to a Song domain model.
    /// </summary>
    /// <param name="dto">The DTO to map from</param>
    /// <returns>The mapped domain model</returns>
    /// <exception cref="ArgumentNullException">Thrown when dto is null</exception>
    public static Song MapSongToDomain(SongDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        return new Song
        {
            Id = dto.Id,
            Title = dto.Attributes.Title ?? string.Empty,
            Author = dto.Attributes.Author,
            Copyright = dto.Attributes.Copyright,
            CcliNumber = dto.Attributes.CcliNumber,
            Hidden = dto.Attributes.Hidden,
            Notes = dto.Attributes.Notes,
            Themes = dto.Attributes.Themes,
            LastScheduledAt = dto.Attributes.LastScheduledAt,
            CreatedAt = dto.Attributes.CreatedAt,
            UpdatedAt = dto.Attributes.UpdatedAt,
            DataSource = "Services"
        };
    }

    /// <summary>
    /// Maps a PlanCreateRequest to a JSON:API request.
    /// </summary>
    /// <param name="request">The request to map from</param>
    /// <returns>The mapped JSON:API request</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null</exception>
    public static JsonApiRequest<PlanCreateDto> MapCreateRequestToJsonApi(PlanCreateRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        return new JsonApiRequest<PlanCreateDto>
        {
            Data = new PlanCreateDto
            {
                Type = "Plan",
                Attributes = new PlanCreateAttributes
                {
                    Title = request.Title,
                    Dates = request.Dates,
                    SortDate = request.SortDate,
                    IsPublic = request.IsPublic,
                    Notes = request.Notes,
                    RehearsalTimes = request.RehearsalTimes,
                    ServiceTimes = request.ServiceTimes,
                    OtherTimes = request.OtherTimes
                },
                Relationships = new PlanCreateRelationships
                {
                    ServiceType = new RelationshipData
                    {
                        Type = "ServiceType",
                        Id = request.ServiceTypeId
                    }
                }
            }
        };
    }

    /// <summary>
    /// Maps a PlanUpdateRequest to a JSON:API request.
    /// </summary>
    /// <param name="request">The request to map from</param>
    /// <returns>The mapped JSON:API request</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null</exception>
    public static JsonApiRequest<PlanUpdateDto> MapUpdateRequestToJsonApi(PlanUpdateRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        return new JsonApiRequest<PlanUpdateDto>
        {
            Data = new PlanUpdateDto
            {
                Type = "Plan",
                Attributes = new PlanUpdateAttributes
                {
                    Title = request.Title,
                    Dates = request.Dates,
                    SortDate = request.SortDate,
                    IsPublic = request.IsPublic,
                    Notes = request.Notes,
                    RehearsalTimes = request.RehearsalTimes,
                    ServiceTimes = request.ServiceTimes,
                    OtherTimes = request.OtherTimes
                }
            }
        };
    }

    /// <summary>
    /// Maps an ItemCreateRequest to a JSON:API request.
    /// </summary>
    /// <param name="request">The request to map from</param>
    /// <returns>The mapped JSON:API request</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null</exception>
    public static JsonApiRequest<ItemCreateDto> MapItemCreateRequestToJsonApi(ItemCreateRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        return new JsonApiRequest<ItemCreateDto>
        {
            Data = new ItemCreateDto
            {
                Type = "Item",
                Attributes = new ItemCreateAttributes
                {
                    Title = request.Title,
                    Sequence = request.Sequence,
                    ItemType = request.ItemType,
                    Description = request.Description,
                    KeyName = request.KeyName,
                    Length = request.Length,
                    ServicePosition = request.ServicePosition
                },
                Relationships = new ItemCreateRelationships
                {
                    Song = !string.IsNullOrEmpty(request.SongId) ? new RelationshipData { Type = "Song", Id = request.SongId } : null,
                    Arrangement = !string.IsNullOrEmpty(request.ArrangementId) ? new RelationshipData { Type = "Arrangement", Id = request.ArrangementId } : null
                }
            }
        };
    }

    /// <summary>
    /// Maps an ItemUpdateRequest to a JSON:API request.
    /// </summary>
    /// <param name="request">The request to map from</param>
    /// <returns>The mapped JSON:API request</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null</exception>
    public static JsonApiRequest<ItemUpdateDto> MapItemUpdateRequestToJsonApi(ItemUpdateRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        return new JsonApiRequest<ItemUpdateDto>
        {
            Data = new ItemUpdateDto
            {
                Type = "Item",
                Attributes = new ItemUpdateAttributes
                {
                    Title = request.Title,
                    Sequence = request.Sequence,
                    Description = request.Description,
                    KeyName = request.KeyName,
                    Length = request.Length,
                    ServicePosition = request.ServicePosition
                }
            }
        };
    }

    /// <summary>
    /// Maps a SongCreateRequest to a JSON:API request.
    /// </summary>
    /// <param name="request">The request to map from</param>
    /// <returns>The mapped JSON:API request</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null</exception>
    public static JsonApiRequest<SongCreateDto> MapSongCreateRequestToJsonApi(SongCreateRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        return new JsonApiRequest<SongCreateDto>
        {
            Data = new SongCreateDto
            {
                Type = "Song",
                Attributes = new SongCreateAttributes
                {
                    Title = request.Title,
                    Author = request.Author,
                    Copyright = request.Copyright,
                    CcliNumber = request.CcliNumber,
                    Hidden = request.Hidden,
                    Notes = request.Notes,
                    Themes = request.Themes
                }
            }
        };
    }

    /// <summary>
    /// Maps a SongUpdateRequest to a JSON:API request.
    /// </summary>
    /// <param name="request">The request to map from</param>
    /// <returns>The mapped JSON:API request</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null</exception>
    public static JsonApiRequest<SongUpdateDto> MapSongUpdateRequestToJsonApi(SongUpdateRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        return new JsonApiRequest<SongUpdateDto>
        {
            Data = new SongUpdateDto
            {
                Type = "Song",
                Attributes = new SongUpdateAttributes
                {
                    Title = request.Title,
                    Author = request.Author,
                    Copyright = request.Copyright,
                    CcliNumber = request.CcliNumber,
                    Hidden = request.Hidden,
                    Notes = request.Notes,
                    Themes = request.Themes
                }
            }
        };
    }
}

// Supporting DTOs for create/update operations
public class PlanCreateDto
{
    public string Type { get; set; } = "Plan";
    public PlanCreateAttributes Attributes { get; set; } = new();
    public PlanCreateRelationships? Relationships { get; set; }
}

public class PlanCreateAttributes
{
    public string Title { get; set; } = string.Empty;
    public string? Dates { get; set; }
    public DateTime? SortDate { get; set; }
    public bool IsPublic { get; set; }
    public string? Notes { get; set; }
    public string? RehearsalTimes { get; set; }
    public string? ServiceTimes { get; set; }
    public string? OtherTimes { get; set; }
}

public class PlanCreateRelationships
{
    public RelationshipData? ServiceType { get; set; }
}

public class PlanUpdateDto
{
    public string Type { get; set; } = "Plan";
    public PlanUpdateAttributes Attributes { get; set; } = new();
}

public class PlanUpdateAttributes
{
    public string? Title { get; set; }
    public string? Dates { get; set; }
    public DateTime? SortDate { get; set; }
    public bool? IsPublic { get; set; }
    public string? Notes { get; set; }
    public string? RehearsalTimes { get; set; }
    public string? ServiceTimes { get; set; }
    public string? OtherTimes { get; set; }
}

// Item DTOs
public class ItemCreateDto
{
    public string Type { get; set; } = "Item";
    public ItemCreateAttributes Attributes { get; set; } = new();
    public ItemCreateRelationships? Relationships { get; set; }
}

public class ItemCreateAttributes
{
    public string Title { get; set; } = string.Empty;
    public int Sequence { get; set; }
    public string? ItemType { get; set; }
    public string? Description { get; set; }
    public string? KeyName { get; set; }
    public int? Length { get; set; }
    public string? ServicePosition { get; set; }
}

public class ItemCreateRelationships
{
    public RelationshipData? Song { get; set; }
    public RelationshipData? Arrangement { get; set; }
}

public class ItemUpdateDto
{
    public string Type { get; set; } = "Item";
    public ItemUpdateAttributes Attributes { get; set; } = new();
}

public class ItemUpdateAttributes
{
    public string? Title { get; set; }
    public int? Sequence { get; set; }
    public string? Description { get; set; }
    public string? KeyName { get; set; }
    public int? Length { get; set; }
    public string? ServicePosition { get; set; }
}

// Song DTOs
public class SongCreateDto
{
    public string Type { get; set; } = "Song";
    public SongCreateAttributes Attributes { get; set; } = new();
}

public class SongCreateAttributes
{
    public string Title { get; set; } = string.Empty;
    public string? Author { get; set; }
    public string? Copyright { get; set; }
    public string? CcliNumber { get; set; }
    public bool Hidden { get; set; }
    public string? Notes { get; set; }
    public string? Themes { get; set; }
}

public class SongUpdateDto
{
    public string Type { get; set; } = "Song";
    public SongUpdateAttributes Attributes { get; set; } = new();
}

public class SongUpdateAttributes
{
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Copyright { get; set; }
    public string? CcliNumber { get; set; }
    public bool? Hidden { get; set; }
    public string? Notes { get; set; }
    public string? Themes { get; set; }
}