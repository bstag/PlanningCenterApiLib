
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Webhooks;
using PlanningCenter.Api.Client.Models.Requests;

namespace PlanningCenter.Api.Client.Abstractions;

/// <summary>
/// Service interface for the Planning Center Webhooks module.
/// Provides comprehensive webhook subscription and event management with built-in pagination support.
/// </summary>
public interface IWebhooksService
{
    // Subscription management
    
    /// <summary>
    /// Gets a single webhook subscription by ID.
    /// </summary>
    /// <param name="id">The subscription's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The webhook subscription, or null if not found</returns>
    Task<WebhookSubscription?> GetSubscriptionAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists webhook subscriptions with optional filtering, sorting, and pagination.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with webhook subscriptions</returns>
    Task<IPagedResponse<WebhookSubscription>> ListSubscriptionsAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new webhook subscription.
    /// </summary>
    /// <param name="request">The webhook subscription creation request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created webhook subscription</returns>
    Task<WebhookSubscription> CreateSubscriptionAsync(WebhookSubscriptionCreateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing webhook subscription.
    /// </summary>
    /// <param name="id">The subscription's unique identifier</param>
    /// <param name="request">The webhook subscription update request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The updated webhook subscription</returns>
    Task<WebhookSubscription> UpdateSubscriptionAsync(string id, WebhookSubscriptionUpdateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a webhook subscription.
    /// </summary>
    /// <param name="id">The subscription's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task DeleteSubscriptionAsync(string id, CancellationToken cancellationToken = default);
    
    // Subscription activation
    
    /// <summary>
    /// Activates a webhook subscription.
    /// </summary>
    /// <param name="id">The subscription's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The activated webhook subscription</returns>
    Task<WebhookSubscription> ActivateSubscriptionAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deactivates a webhook subscription.
    /// </summary>
    /// <param name="id">The subscription's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The deactivated webhook subscription</returns>
    Task<WebhookSubscription> DeactivateSubscriptionAsync(string id, CancellationToken cancellationToken = default);
    
    // Available events
    
    /// <summary>
    /// Gets a single available event by ID.
    /// </summary>
    /// <param name="id">The available event's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The available event, or null if not found</returns>
    Task<AvailableEvent?> GetAvailableEventAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists all available events with optional filtering, sorting, and pagination.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with available events</returns>
    Task<IPagedResponse<AvailableEvent>> ListAvailableEventsAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists available events for a specific module.
    /// </summary>
    /// <param name="module">The module name (e.g., "people", "giving", "calendar")</param>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with available events for the module</returns>
    Task<IPagedResponse<AvailableEvent>> ListAvailableEventsByModuleAsync(string module, QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    // Event history
    
    /// <summary>
    /// Gets a single webhook event by ID.
    /// </summary>
    /// <param name="id">The event's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The webhook event, or null if not found</returns>
    Task<Event?> GetEventAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists webhook events for a specific subscription.
    /// </summary>
    /// <param name="subscriptionId">The subscription's unique identifier</param>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with webhook events</returns>
    Task<IPagedResponse<Event>> ListEventsAsync(string subscriptionId, QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Redelivers a webhook event.
    /// </summary>
    /// <param name="eventId">The event's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The redelivered webhook event</returns>
    Task<Event> RedeliverEventAsync(string eventId, CancellationToken cancellationToken = default);
    
    // Event validation
    
    /// <summary>
    /// Validates a webhook event signature.
    /// </summary>
    /// <param name="payload">The webhook payload</param>
    /// <param name="signature">The webhook signature</param>
    /// <param name="secret">The webhook secret</param>
    /// <returns>True if the signature is valid, false otherwise</returns>
    Task<bool> ValidateEventSignatureAsync(string payload, string signature, string secret);
    
    /// <summary>
    /// Deserializes a webhook event payload.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to</typeparam>
    /// <param name="payload">The webhook payload</param>
    /// <returns>The deserialized event data</returns>
    Task<T> DeserializeEventAsync<T>(string payload) where T : class;
    
    // Subscription testing
    
    /// <summary>
    /// Tests a webhook subscription by sending a test event.
    /// </summary>
    /// <param name="id">The subscription's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The webhook test result</returns>
    Task<WebhookTestResult> TestSubscriptionAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Tests a webhook URL by sending a test event.
    /// </summary>
    /// <param name="url">The webhook URL to test</param>
    /// <param name="secret">The webhook secret</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The webhook test result</returns>
    Task<WebhookTestResult> TestWebhookUrlAsync(string url, string secret, CancellationToken cancellationToken = default);
    
    // Bulk operations
    
    /// <summary>
    /// Creates multiple webhook subscriptions in bulk.
    /// </summary>
    /// <param name="request">The bulk webhook subscription creation request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with created webhook subscriptions</returns>
    Task<IPagedResponse<WebhookSubscription>> CreateBulkSubscriptionsAsync(BulkWebhookSubscriptionCreateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes multiple webhook subscriptions in bulk.
    /// </summary>
    /// <param name="request">The bulk webhook subscription deletion request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task DeleteBulkSubscriptionsAsync(BulkWebhookSubscriptionDeleteRequest request, CancellationToken cancellationToken = default);
    
    // Analytics and monitoring
    
    /// <summary>
    /// Gets analytics data for a webhook subscription.
    /// </summary>
    /// <param name="subscriptionId">The subscription's unique identifier</param>
    /// <param name="request">The analytics request parameters</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The webhook analytics data</returns>
    Task<WebhookAnalytics> GetSubscriptionAnalyticsAsync(string subscriptionId, AnalyticsRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Generates a webhook delivery report.
    /// </summary>
    /// <param name="request">The webhook report request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The generated webhook delivery report</returns>
    Task<WebhookDeliveryReport> GenerateDeliveryReportAsync(WebhookReportRequest request, CancellationToken cancellationToken = default);
    
    // Pagination helpers that eliminate manual pagination logic
    
    /// <summary>
    /// Gets all webhook subscriptions matching the specified criteria.
    /// This method automatically handles pagination behind the scenes.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>All webhook subscriptions matching the criteria</returns>
    Task<IReadOnlyList<WebhookSubscription>> GetAllSubscriptionsAsync(
        QueryParameters? parameters = null,
        PaginationOptions? options = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Streams webhook subscriptions matching the specified criteria for memory-efficient processing.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>An async enumerable that yields webhook subscriptions from all pages</returns>
    IAsyncEnumerable<WebhookSubscription> StreamSubscriptionsAsync(
        QueryParameters? parameters = null,
        PaginationOptions? options = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Webhook test result information.
/// </summary>
public class WebhookTestResult
{
    /// <summary>
    /// Gets or sets whether the test was successful.
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    /// Gets or sets the test result message.
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the HTTP status code returned by the webhook endpoint.
    /// </summary>
    public int? StatusCode { get; set; }
    
    /// <summary>
    /// Gets or sets the response time in milliseconds.
    /// </summary>
    public double ResponseTimeMs { get; set; }
    
    /// <summary>
    /// Gets or sets the test timestamp.
    /// </summary>
    public DateTime TestedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Gets or sets additional test metadata.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Webhook analytics data.
/// </summary>
public class WebhookAnalytics
{
    /// <summary>
    /// Gets or sets the subscription ID.
    /// </summary>
    public string SubscriptionId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the total number of events delivered.
    /// </summary>
    public long TotalEventsDelivered { get; set; }
    
    /// <summary>
    /// Gets or sets the number of successful deliveries.
    /// </summary>
    public long SuccessfulDeliveries { get; set; }
    
    /// <summary>
    /// Gets or sets the number of failed deliveries.
    /// </summary>
    public long FailedDeliveries { get; set; }
    
    /// <summary>
    /// Gets or sets the average response time in milliseconds.
    /// </summary>
    public double AverageResponseTimeMs { get; set; }
    
    /// <summary>
    /// Gets or sets the analytics period start date.
    /// </summary>
    public DateTime PeriodStart { get; set; }
    
    /// <summary>
    /// Gets or sets the analytics period end date.
    /// </summary>
    public DateTime PeriodEnd { get; set; }
    
    /// <summary>
    /// Gets or sets additional analytics data.
    /// </summary>
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

/// <summary>
/// Webhook delivery report data.
/// </summary>
public class WebhookDeliveryReport
{
    /// <summary>
    /// Gets or sets the total number of subscriptions.
    /// </summary>
    public int TotalSubscriptions { get; set; }
    
    /// <summary>
    /// Gets or sets the total number of events.
    /// </summary>
    public long TotalEvents { get; set; }
    
    /// <summary>
    /// Gets or sets the total number of successful deliveries.
    /// </summary>
    public long SuccessfulDeliveries { get; set; }
    
    /// <summary>
    /// Gets or sets the total number of failed deliveries.
    /// </summary>
    public long FailedDeliveries { get; set; }
    
    /// <summary>
    /// Gets or sets the overall delivery success rate as a percentage.
    /// </summary>
    public double SuccessRate { get; set; }
    
    /// <summary>
    /// Gets or sets the report generation timestamp.
    /// </summary>
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Gets or sets the report period start date.
    /// </summary>
    public DateTime PeriodStart { get; set; }
    
    /// <summary>
    /// Gets or sets the report period end date.
    /// </summary>
    public DateTime PeriodEnd { get; set; }
    
    /// <summary>
    /// Gets or sets additional report data.
    /// </summary>
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

/// <summary>
/// Request for bulk webhook subscription creation.
/// </summary>
public class BulkWebhookSubscriptionCreateRequest
{
    /// <summary>
    /// Gets or sets the webhook subscriptions to create.
    /// </summary>
    public List<WebhookSubscriptionCreateRequest> Subscriptions { get; set; } = new();
}

/// <summary>
/// Request for bulk webhook subscription deletion.
/// </summary>
public class BulkWebhookSubscriptionDeleteRequest
{
    /// <summary>
    /// Gets or sets the subscription IDs to delete.
    /// </summary>
    public List<string> SubscriptionIds { get; set; } = new();
}

/// <summary>
/// Request for generating webhook reports.
/// </summary>
public class WebhookReportRequest
{
    /// <summary>
    /// Gets or sets the start date for the report period.
    /// </summary>
    public DateTime StartDate { get; set; }
    
    /// <summary>
    /// Gets or sets the end date for the report period.
    /// </summary>
    public DateTime EndDate { get; set; }
    
    /// <summary>
    /// Gets or sets the subscription IDs to include in the report.
    /// </summary>
    public List<string>? SubscriptionIds { get; set; }
    
    /// <summary>
    /// Gets or sets whether to include detailed event information.
    /// </summary>
    public bool IncludeEventDetails { get; set; } = false;
    
    /// <summary>
    /// Gets or sets the report format.
    /// </summary>
    public string Format { get; set; } = "json";
}