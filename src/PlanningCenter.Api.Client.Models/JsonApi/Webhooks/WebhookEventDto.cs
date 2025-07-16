using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Webhooks;

/// <summary>
/// JSON API DTO for WebhookEvent entity.
/// </summary>
public class WebhookEventDto
{
    public string Type { get; set; } = "Event";
    public string Id { get; set; } = string.Empty;
    public WebhookEventAttributesDto Attributes { get; set; } = new();
}

/// <summary>
/// Attributes for WebhookEvent JSON API DTO.
/// </summary>
public class WebhookEventAttributesDto
{
    public string EventType { get; set; } = string.Empty;
    public string DeliveryStatus { get; set; } = string.Empty;
    public DateTime? DeliveredAt { get; set; }
    public int? ResponseCode { get; set; }
    public double? ResponseTimeMs { get; set; }
    public string? Payload { get; set; }
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }
    public DateTime? NextRetryAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}