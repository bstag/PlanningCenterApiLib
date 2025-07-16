using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Webhooks;

/// <summary>
/// JSON API DTO for WebhookSubscription entity.
/// </summary>
public class WebhookSubscriptionDto
{
    public string Type { get; set; } = "WebhookSubscription";
    public string Id { get; set; } = string.Empty;
    public WebhookSubscriptionAttributesDto Attributes { get; set; } = new();
    public WebhookSubscriptionRelationshipsDto? Relationships { get; set; }
}

/// <summary>
/// Attributes for WebhookSubscription JSON API DTO.
/// </summary>
public class WebhookSubscriptionAttributesDto
{
    public string Url { get; set; } = string.Empty;
    public string? Secret { get; set; }
    public bool Active { get; set; } = true;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime? LastDeliveryAt { get; set; }
    public string? LastDeliveryStatus { get; set; }
    public int? LastDeliveryStatusCode { get; set; }
    public double? LastDeliveryResponseTimeMs { get; set; }
    public string? LastDeliveryError { get; set; }
    public long TotalDeliveries { get; set; }
    public long SuccessfulDeliveries { get; set; }
    public long FailedDeliveries { get; set; }
    public string? RetryPolicy { get; set; }
    public int MaxRetries { get; set; } = 3;
    public int TimeoutSeconds { get; set; } = 30;
    public Dictionary<string, string> CustomHeaders { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Relationships for WebhookSubscription JSON API DTO.
/// </summary>
public class WebhookSubscriptionRelationshipsDto
{
    public RelationshipData? AvailableEvent { get; set; }
    public RelationshipData? Organization { get; set; }
}

/// <summary>
/// Create DTO for WebhookSubscription.
/// </summary>
public class WebhookSubscriptionCreateDto
{
    public string Type { get; set; } = "WebhookSubscription";
    public WebhookSubscriptionCreateAttributesDto Attributes { get; set; } = new();
    public WebhookSubscriptionCreateRelationshipsDto? Relationships { get; set; }
}

/// <summary>
/// Create attributes for WebhookSubscription.
/// </summary>
public class WebhookSubscriptionCreateAttributesDto
{
    public string Url { get; set; } = string.Empty;
    public string? Secret { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool Active { get; set; } = true;
    public int MaxRetries { get; set; } = 3;
    public int TimeoutSeconds { get; set; } = 30;
    public Dictionary<string, string> CustomHeaders { get; set; } = new();
}

/// <summary>
/// Create relationships for WebhookSubscription.
/// </summary>
public class WebhookSubscriptionCreateRelationshipsDto
{
    public RelationshipData? AvailableEvent { get; set; }
}

/// <summary>
/// Update DTO for WebhookSubscription.
/// </summary>
public class WebhookSubscriptionUpdateDto
{
    public string Type { get; set; } = "WebhookSubscription";
    public string Id { get; set; } = string.Empty;
    public WebhookSubscriptionUpdateAttributesDto Attributes { get; set; } = new();
}

/// <summary>
/// Update attributes for WebhookSubscription.
/// </summary>
public class WebhookSubscriptionUpdateAttributesDto
{
    public string? Url { get; set; }
    public string? Secret { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool? Active { get; set; }
    public int? MaxRetries { get; set; }
    public int? TimeoutSeconds { get; set; }
    public Dictionary<string, string>? CustomHeaders { get; set; }
}