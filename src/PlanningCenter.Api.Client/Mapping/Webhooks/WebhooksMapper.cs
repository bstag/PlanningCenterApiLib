using PlanningCenter.Api.Client.Models.JsonApi.Core;
using PlanningCenter.Api.Client.Models.JsonApi.Webhooks;
using PlanningCenter.Api.Client.Models.Webhooks;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Models.JsonApi;

namespace PlanningCenter.Api.Client.Mapping.Webhooks;

/// <summary>
/// Mapper for Webhooks entities between domain models and DTOs.
/// </summary>
public static class WebhooksMapper
{
    #region WebhookSubscription Mapping

    /// <summary>
    /// Maps a WebhookSubscriptionDto to a WebhookSubscription domain model.
    /// </summary>
    public static WebhookSubscription MapToDomain(WebhookSubscriptionDto dto)
    {
        return new WebhookSubscription
        {
            Id = dto.Id,
            Url = dto.Attributes.Url,
            Secret = dto.Attributes.Secret,
            Active = dto.Attributes.Active,
            Name = dto.Attributes.Name,
            Description = dto.Attributes.Description,
            AvailableEventId = dto.Relationships?.AvailableEvent?.Id ?? string.Empty,
            OrganizationId = dto.Relationships?.Organization?.Id,
            LastDeliveryAt = dto.Attributes.LastDeliveryAt,
            LastDeliveryStatus = dto.Attributes.LastDeliveryStatus,
            LastDeliveryStatusCode = dto.Attributes.LastDeliveryStatusCode,
            LastDeliveryResponseTimeMs = dto.Attributes.LastDeliveryResponseTimeMs,
            LastDeliveryError = dto.Attributes.LastDeliveryError,
            TotalDeliveries = dto.Attributes.TotalDeliveries,
            SuccessfulDeliveries = dto.Attributes.SuccessfulDeliveries,
            FailedDeliveries = dto.Attributes.FailedDeliveries,
            RetryPolicy = dto.Attributes.RetryPolicy,
            MaxRetries = dto.Attributes.MaxRetries,
            TimeoutSeconds = dto.Attributes.TimeoutSeconds,
            CustomHeaders = dto.Attributes.CustomHeaders,
            Metadata = dto.Attributes.Metadata,
            CreatedAt = dto.Attributes.CreatedAt,
            UpdatedAt = dto.Attributes.UpdatedAt,
            DataSource = "Webhooks"
        };
    }

    /// <summary>
    /// Maps a WebhookSubscriptionCreateRequest to a JSON:API request.
    /// </summary>
    public static JsonApiRequest<WebhookSubscriptionCreateDto> MapCreateRequestToJsonApi(WebhookSubscriptionCreateRequest request)
    {
        var dto = new WebhookSubscriptionCreateDto
        {
            Type = "WebhookSubscription",
            Attributes = new WebhookSubscriptionCreateAttributesDto
            {
                Url = request.Url,
                Secret = request.Secret,
                Name = request.Name,
                Description = request.Description,
                Active = request.Active,
                MaxRetries = request.MaxRetries,
                TimeoutSeconds = request.TimeoutSeconds,
                CustomHeaders = request.CustomHeaders
            }
        };

        if (!string.IsNullOrEmpty(request.AvailableEventId))
        {
            dto.Relationships = new WebhookSubscriptionCreateRelationshipsDto
            {
                AvailableEvent = new RelationshipData { Type = "AvailableEvent", Id = request.AvailableEventId }
            };
        }

        return new JsonApiRequest<WebhookSubscriptionCreateDto> { Data = dto };
    }

    /// <summary>
    /// Maps a WebhookSubscriptionUpdateRequest to a JSON:API request.
    /// </summary>
    public static JsonApiRequest<WebhookSubscriptionUpdateDto> MapUpdateRequestToJsonApi(string id, WebhookSubscriptionUpdateRequest request)
    {
        var dto = new WebhookSubscriptionUpdateDto
        {
            Type = "WebhookSubscription",
            Id = id,
            Attributes = new WebhookSubscriptionUpdateAttributesDto
            {
                Url = request.Url,
                Secret = request.Secret,
                Name = request.Name,
                Description = request.Description,
                Active = request.Active,
                MaxRetries = request.MaxRetries,
                TimeoutSeconds = request.TimeoutSeconds,
                CustomHeaders = request.CustomHeaders
            }
        };

        return new JsonApiRequest<WebhookSubscriptionUpdateDto> { Data = dto };
    }

    #endregion

    #region AvailableEvent Mapping (Placeholder)

    /// <summary>
    /// Maps an AvailableEventDto to an AvailableEvent domain model.
    /// </summary>
    public static AvailableEvent MapToDomain(dynamic dto)
    {
        // This would be implemented with proper AvailableEventDto in a complete implementation
        return new AvailableEvent
        {
            Id = dto.Id?.ToString() ?? string.Empty,
            Name = dto.Attributes?.Name?.ToString() ?? "unknown.event",
            Module = dto.Attributes?.Module?.ToString() ?? "unknown",
            Description = dto.Attributes?.Description?.ToString(),
            DataSource = "Webhooks"
        };
    }

    #endregion

    #region Event Mapping (Placeholder)

    /// <summary>
    /// Maps an EventDto to an Event domain model.
    /// </summary>
    public static Event MapToDomain(dynamic dto)
    {
        // This would be implemented with proper EventDto in a complete implementation
        return new Event
        {
            Id = dto.Id?.ToString() ?? string.Empty,
            WebhookSubscriptionId = dto.Relationships?.WebhookSubscription?.Id?.ToString() ?? string.Empty,
            AvailableEventId = dto.Relationships?.AvailableEvent?.Id?.ToString() ?? string.Empty,
            EventName = dto.Attributes?.EventName?.ToString() ?? "unknown.event",
            DeliveryStatus = dto.Attributes?.DeliveryStatus?.ToString() ?? "pending",
            DataSource = "Webhooks"
        };
    }

    #endregion
}