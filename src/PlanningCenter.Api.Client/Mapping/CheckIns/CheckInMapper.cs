using PlanningCenter.Api.Client.Models.JsonApi.CheckIns;
using PlanningCenter.Api.Client.Models.CheckIns;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Models.JsonApi;
using PlanningCenter.Api.Client.Models.JsonApi.Core;
using System;

namespace PlanningCenter.Api.Client.Mapping.CheckIns;

/// <summary>
/// Mapper for converting between CheckIn DTOs and domain models.
/// Follows DRY principle by centralizing all check-in mapping logic.
/// Follows Single Responsibility Principle - handles only check-in mapping.
/// </summary>
public static class CheckInMapper
{
    /// <summary>
    /// Maps a CheckInDto to a CheckIn domain model.
    /// </summary>
    /// <param name="dto">The DTO to map from</param>
    /// <returns>The mapped domain model</returns>
    /// <exception cref="ArgumentNullException">Thrown when dto is null</exception>
    public static CheckIn MapToDomain(CheckInDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        return new CheckIn
        {
            Id = dto.Id,
            FirstName = dto.Attributes.FirstName ?? string.Empty,
            LastName = dto.Attributes.LastName ?? string.Empty,
            Number = dto.Attributes.Number,
            SecurityCode = dto.Attributes.SecurityCode,
            Kind = dto.Attributes.Kind ?? string.Empty,
            CreatedAt = dto.Attributes.CreatedAt,
            CheckedOutAt = dto.Attributes.CheckedOutAt,
            ConfirmedAt = dto.Attributes.ConfirmedAt,
            MedicalNotes = dto.Attributes.MedicalNotes,
            EmergencyContactName = dto.Attributes.EmergencyContactName,
            EmergencyContactPhoneNumber = dto.Attributes.EmergencyContactPhoneNumber,
            OneTimeGuest = dto.Attributes.OneTimeGuest,
            PersonId = dto.Relationships?.Person?.Id,
            EventId = dto.Relationships?.Event?.Id,
            EventTimeId = dto.Relationships?.EventTime?.Id,
            LocationId = dto.Relationships?.Location?.Id,
            UpdatedAt = dto.Attributes.UpdatedAt,
            DataSource = "CheckIns"
        };
    }

    /// <summary>
    /// Maps an EventDto to an Event domain model.
    /// </summary>
    /// <param name="dto">The DTO to map from</param>
    /// <returns>The mapped domain model</returns>
    /// <exception cref="ArgumentNullException">Thrown when dto is null</exception>
    public static Event MapEventToDomain(EventDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        return new Event
        {
            Id = dto.Id,
            Name = dto.Attributes.Name ?? string.Empty,
            Frequency = dto.Attributes.Frequency,
            EnableServicesIntegration = dto.Attributes.EnableServicesIntegration,
            Archived = dto.Attributes.Archived,
            CreatedAt = dto.Attributes.CreatedAt,
            UpdatedAt = dto.Attributes.UpdatedAt,
            DataSource = "CheckIns"
        };
    }

    /// <summary>
    /// Maps a CheckInCreateRequest to a JSON:API request.
    /// </summary>
    /// <param name="request">The request to map from</param>
    /// <returns>The mapped JSON:API request</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null</exception>
    public static JsonApiRequest<CheckInCreateDto> MapCreateRequestToJsonApi(CheckInCreateRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        return new JsonApiRequest<CheckInCreateDto>
        {
            Data = new CheckInCreateDto
            {
                Type = "CheckIn",
                Attributes = new CheckInCreateAttributes
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Kind = request.Kind,
                    MedicalNotes = request.MedicalNotes,
                    EmergencyContactName = request.EmergencyContactName,
                    EmergencyContactPhoneNumber = request.EmergencyContactPhoneNumber,
                    OneTimeGuest = request.OneTimeGuest
                },
                Relationships = new CheckInCreateRelationships
                {
                    Person = !string.IsNullOrEmpty(request.PersonId) ? new RelationshipData { Type = "Person", Id = request.PersonId } : null,
                    Event = new RelationshipData { Type = "Event", Id = request.EventId },
                    EventTime = !string.IsNullOrEmpty(request.EventTimeId) ? new RelationshipData { Type = "EventTime", Id = request.EventTimeId } : null,
                    Location = !string.IsNullOrEmpty(request.LocationId) ? new RelationshipData { Type = "Location", Id = request.LocationId } : null
                }
            }
        };
    }

    /// <summary>
    /// Maps a CheckInUpdateRequest to a JSON:API request.
    /// </summary>
    /// <param name="request">The request to map from</param>
    /// <returns>The mapped JSON:API request</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null</exception>
    public static JsonApiRequest<CheckInUpdateDto> MapUpdateRequestToJsonApi(CheckInUpdateRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        return new JsonApiRequest<CheckInUpdateDto>
        {
            Data = new CheckInUpdateDto
            {
                Type = "CheckIn",
                Attributes = new CheckInUpdateAttributes
                {
                    MedicalNotes = request.MedicalNotes,
                    EmergencyContactName = request.EmergencyContactName,
                    EmergencyContactPhoneNumber = request.EmergencyContactPhoneNumber
                }
            }
        };
    }
}

// Supporting DTOs for create/update operations
public class CheckInCreateDto
{
    public string Type { get; set; } = "CheckIn";
    public CheckInCreateAttributes Attributes { get; set; } = new();
    public CheckInCreateRelationships? Relationships { get; set; }
}

public class CheckInCreateAttributes
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Kind { get; set; } = string.Empty;
    public string? MedicalNotes { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhoneNumber { get; set; }
    public bool OneTimeGuest { get; set; }
}

public class CheckInCreateRelationships
{
    public RelationshipData? Person { get; set; }
    public RelationshipData? Event { get; set; }
    public RelationshipData? EventTime { get; set; }
    public RelationshipData? Location { get; set; }
}

public class CheckInUpdateDto
{
    public string Type { get; set; } = "CheckIn";
    public CheckInUpdateAttributes Attributes { get; set; } = new();
}

public class CheckInUpdateAttributes
{
    public string? MedicalNotes { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhoneNumber { get; set; }
}