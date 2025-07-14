using PlanningCenter.Api.Client.Models.JsonApi.Calendar;
using PlanningCenter.Api.Client.Models.Calendar;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Models.JsonApi;
using System;

namespace PlanningCenter.Api.Client.Mapping.Calendar;

/// <summary>
/// Mapper for converting between Calendar DTOs and domain models.
/// Follows DRY principle by centralizing all calendar mapping logic.
/// Follows Single Responsibility Principle - handles only calendar mapping.
/// </summary>
public static class CalendarMapper
{
    /// <summary>
    /// Maps an EventDto to an Event domain model.
    /// </summary>
    /// <param name="dto">The DTO to map from</param>
    /// <returns>The mapped domain model</returns>
    /// <exception cref="ArgumentNullException">Thrown when dto is null</exception>
    public static Event MapToDomain(EventDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        return new Event
        {
            Id = dto.Id,
            Name = dto.Attributes.Name ?? string.Empty,
            Summary = dto.Attributes.Summary,
            Description = dto.Attributes.Description,
            AllDayEvent = dto.Attributes.AllDayEvent,
            StartsAt = dto.Attributes.StartsAt,
            EndsAt = dto.Attributes.EndsAt,
            LocationName = dto.Attributes.LocationName,
            DetailsUrl = dto.Attributes.DetailsUrl,
            VisibleInChurchCenter = dto.Attributes.VisibleInChurchCenter,
            RegistrationRequired = dto.Attributes.RegistrationRequired,
            RegistrationUrl = dto.Attributes.RegistrationUrl,
            OwnerName = dto.Attributes.OwnerName,
            ApprovalStatus = dto.Attributes.ApprovalStatus,
            CreatedAt = dto.Attributes.CreatedAt,
            UpdatedAt = dto.Attributes.UpdatedAt,
            DataSource = "Calendar"
        };
    }

    /// <summary>
    /// Maps a ResourceDto to a Resource domain model.
    /// </summary>
    /// <param name="dto">The DTO to map from</param>
    /// <returns>The mapped domain model</returns>
    /// <exception cref="ArgumentNullException">Thrown when dto is null</exception>
    public static Resource MapResourceToDomain(ResourceDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        return new Resource
        {
            Id = dto.Id,
            Name = dto.Attributes.Name ?? string.Empty,
            Description = dto.Attributes.Description,
            Kind = dto.Attributes.Kind,
            HomeLocation = dto.Attributes.HomeLocation,
            SerialNumber = dto.Attributes.SerialNumber,
            Expires = dto.Attributes.Expires,
            ExpiresAt = dto.Attributes.ExpiresAt,
            RenewalAt = dto.Attributes.RenewalAt,
            Quantity = dto.Attributes.Quantity,
            CreatedAt = dto.Attributes.CreatedAt,
            UpdatedAt = dto.Attributes.UpdatedAt,
            DataSource = "Calendar"
        };
    }

    /// <summary>
    /// Maps an EventCreateRequest to a JSON:API request.
    /// </summary>
    /// <param name="request">The request to map from</param>
    /// <returns>The mapped JSON:API request</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null</exception>
    public static JsonApiRequest<EventCreateDto> MapCreateRequestToJsonApi(EventCreateRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        return new JsonApiRequest<EventCreateDto>
        {
            Data = new EventCreateDto
            {
                Type = "Event",
                Attributes = new EventCreateAttributes
                {
                    Name = request.Name,
                    Summary = request.Summary,
                    Description = request.Description,
                    AllDayEvent = request.AllDayEvent,
                    StartsAt = request.StartsAt,
                    EndsAt = request.EndsAt,
                    LocationName = request.LocationName,
                    VisibleInChurchCenter = request.VisibleInChurchCenter,
                    RegistrationRequired = request.RegistrationRequired,
                    RegistrationUrl = request.RegistrationUrl
                }
            }
        };
    }

    /// <summary>
    /// Maps an EventUpdateRequest to a JSON:API request.
    /// </summary>
    /// <param name="request">The request to map from</param>
    /// <returns>The mapped JSON:API request</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null</exception>
    public static JsonApiRequest<EventUpdateDto> MapUpdateRequestToJsonApi(EventUpdateRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        return new JsonApiRequest<EventUpdateDto>
        {
            Data = new EventUpdateDto
            {
                Type = "Event",
                Attributes = new EventUpdateAttributes
                {
                    Name = request.Name,
                    Summary = request.Summary,
                    Description = request.Description,
                    AllDayEvent = request.AllDayEvent,
                    StartsAt = request.StartsAt,
                    EndsAt = request.EndsAt,
                    LocationName = request.LocationName,
                    VisibleInChurchCenter = request.VisibleInChurchCenter,
                    RegistrationRequired = request.RegistrationRequired,
                    RegistrationUrl = request.RegistrationUrl
                }
            }
        };
    }

    /// <summary>
    /// Maps a ResourceCreateRequest to a JSON:API request.
    /// </summary>
    /// <param name="request">The request to map from</param>
    /// <returns>The mapped JSON:API request</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null</exception>
    public static JsonApiRequest<ResourceCreateDto> MapResourceCreateRequestToJsonApi(ResourceCreateRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        return new JsonApiRequest<ResourceCreateDto>
        {
            Data = new ResourceCreateDto
            {
                Type = "Resource",
                Attributes = new ResourceCreateAttributes
                {
                    Name = request.Name,
                    Description = request.Description,
                    Kind = request.Kind,
                    HomeLocation = request.HomeLocation,
                    SerialNumber = request.SerialNumber,
                    Expires = request.Expires,
                    ExpiresAt = request.ExpiresAt,
                    RenewalAt = request.RenewalAt,
                    Quantity = request.Quantity
                }
            }
        };
    }

    /// <summary>
    /// Maps a ResourceUpdateRequest to a JSON:API request.
    /// </summary>
    /// <param name="request">The request to map from</param>
    /// <returns>The mapped JSON:API request</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null</exception>
    public static JsonApiRequest<ResourceUpdateDto> MapResourceUpdateRequestToJsonApi(ResourceUpdateRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        return new JsonApiRequest<ResourceUpdateDto>
        {
            Data = new ResourceUpdateDto
            {
                Type = "Resource",
                Attributes = new ResourceUpdateAttributes
                {
                    Name = request.Name,
                    Description = request.Description,
                    Kind = request.Kind,
                    HomeLocation = request.HomeLocation,
                    SerialNumber = request.SerialNumber,
                    Expires = request.Expires,
                    ExpiresAt = request.ExpiresAt,
                    RenewalAt = request.RenewalAt,
                    Quantity = request.Quantity
                }
            }
        };
    }
}

// Supporting DTOs for create/update operations
public class EventCreateDto
{
    public string Type { get; set; } = "Event";
    public EventCreateAttributes Attributes { get; set; } = new();
}

public class EventCreateAttributes
{
    public string Name { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? Description { get; set; }
    public bool AllDayEvent { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }
    public string? LocationName { get; set; }
    public bool VisibleInChurchCenter { get; set; }
    public bool RegistrationRequired { get; set; }
    public string? RegistrationUrl { get; set; }
}

public class EventUpdateDto
{
    public string Type { get; set; } = "Event";
    public EventUpdateAttributes Attributes { get; set; } = new();
}

public class EventUpdateAttributes
{
    public string? Name { get; set; }
    public string? Summary { get; set; }
    public string? Description { get; set; }
    public bool? AllDayEvent { get; set; }
    public DateTime? StartsAt { get; set; }
    public DateTime? EndsAt { get; set; }
    public string? LocationName { get; set; }
    public bool? VisibleInChurchCenter { get; set; }
    public bool? RegistrationRequired { get; set; }
    public string? RegistrationUrl { get; set; }
}

// Resource DTOs
public class ResourceCreateDto
{
    public string Type { get; set; } = "Resource";
    public ResourceCreateAttributes Attributes { get; set; } = new();
}

public class ResourceCreateAttributes
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Kind { get; set; }
    public string? HomeLocation { get; set; }
    public string? SerialNumber { get; set; }
    public bool Expires { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? RenewalAt { get; set; }
    public int? Quantity { get; set; }
}

public class ResourceUpdateDto
{
    public string Type { get; set; } = "Resource";
    public ResourceUpdateAttributes Attributes { get; set; } = new();
}

public class ResourceUpdateAttributes
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Kind { get; set; }
    public string? HomeLocation { get; set; }
    public string? SerialNumber { get; set; }
    public bool? Expires { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? RenewalAt { get; set; }
    public int? Quantity { get; set; }
}