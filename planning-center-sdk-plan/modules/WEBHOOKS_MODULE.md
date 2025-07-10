# Webhooks Module Specification

## Overview

The Webhooks module manages real-time event notifications and integrations in Planning Center. It provides comprehensive functionality for creating webhook subscriptions, managing event delivery, validating signatures, and handling webhook events across all Planning Center modules.

## Core Entities

### WebhookSubscription
The primary entity representing a webhook subscription.

**Key Attributes:**
- `id` - Unique identifier
- `url` - Webhook endpoint URL
- `secret` - Webhook secret for signature validation
- `active` - Subscription status
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp
- `last_delivery_at` - Last successful delivery timestamp
- `last_delivery_status` - Status of last delivery attempt

**Relationships:**
- `available_event` - Subscribed event type
- `organization` - Associated organization
- `events` - Delivered webhook events

### AvailableEvent
Represents an event type that can be subscribed to.

**Key Attributes:**
- `id` - Unique identifier
- `name` - Event name (e.g., "person.created", "donation.updated")
- `description` - Event description
- `module` - Source module (people, giving, calendar, etc.)
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `webhook_subscriptions` - Subscriptions for this event type

### Event
Represents a delivered webhook event.

**Key Attributes:**
- `id` - Unique identifier
- `name` - Event name
- `delivery_attempt` - Delivery attempt number
- `delivery_status` - Delivery status (success, failed, pending)
- `delivered_at` - Delivery timestamp
- `response_code` - HTTP response code
- `response_body` - Response body from webhook endpoint
- `payload` - Event payload data
- `created_at` - Event creation timestamp

**Relationships:**
- `webhook_subscription` - Associated subscription

## API Endpoints

### Subscription Management
- `GET /webhooks/v2/webhook_subscriptions` - List all subscriptions
- `GET /webhooks/v2/webhook_subscriptions/{id}` - Get specific subscription
- `POST /webhooks/v2/webhook_subscriptions` - Create new subscription
- `PATCH /webhooks/v2/webhook_subscriptions/{id}` - Update subscription
- `DELETE /webhooks/v2/webhook_subscriptions/{id}` - Delete subscription

### Available Events
- `GET /webhooks/v2/available_events` - List available event types
- `GET /webhooks/v2/available_events/{id}` - Get specific event type

### Event History
- `GET /webhooks/v2/webhook_subscriptions/{subscription_id}/events` - List events for subscription
- `GET /webhooks/v2/events/{id}` - Get specific event
- `POST /webhooks/v2/events/{id}/redeliver` - Redeliver failed event

### Event Types by Module

#### People Module Events
- `person.created` - New person added
- `person.updated` - Person information changed
- `person.deleted` - Person removed
- `household.created` - New household created
- `household.updated` - Household information changed
- `workflow_card.created` - New workflow card
- `workflow_card.updated` - Workflow card status changed
- `form_submission.created` - New form submission

#### Giving Module Events
- `donation.created` - New donation received
- `donation.updated` - Donation information changed
- `donation.deleted` - Donation removed
- `batch.created` - New donation batch
- `batch.committed` - Batch committed for processing
- `refund.created` - Refund issued
- `pledge.created` - New pledge made
- `recurring_donation.created` - New recurring donation setup

#### Calendar Module Events
- `event.created` - New calendar event
- `event.updated` - Event information changed
- `event.deleted` - Event removed
- `resource_booking.created` - Resource booked
- `resource_booking.updated` - Booking changed
- `conflict.created` - Scheduling conflict detected
- `conflict.resolved` - Conflict resolved

#### Check-Ins Module Events
- `check_in.created` - Person checked in
- `check_in.updated` - Check-in information changed
- `check_in.checked_out` - Person checked out

#### Groups Module Events
- `group.created` - New group created
- `group.updated` - Group information changed
- `membership.created` - New group member
- `membership.updated` - Membership status changed
- `enrollment.created` - New enrollment request
- `enrollment.approved` - Enrollment approved

#### Registrations Module Events
- `signup.created` - New signup created
- `signup.updated` - Signup information changed
- `registration.created` - New registration submitted
- `attendee.created` - New attendee registered
- `attendee.waitlisted` - Attendee added to waitlist

#### Publishing Module Events
- `episode.created` - New episode created
- `episode.published` - Episode published
- `series.created` - New series created
- `series.published` - Series published

#### Services Module Events
- `plan.created` - New service plan created
- `plan.updated` - Plan information changed
- `plan_person.created` - Team member assigned
- `plan_person.updated` - Assignment status changed
- `song.created` - New song added to library

## Query Parameters

### Include Parameters
- `available_event` - Include event type information
- `events` - Include recent events
- `organization` - Include organization information

### Filtering
- `where[url]` - Filter by webhook URL
- `where[active]` - Filter by active status
- `where[last_delivery_status]` - Filter by delivery status
- `where[created_at]` - Filter by creation date

### Sorting
- `order=created_at` - Sort by creation date
- `order=last_delivery_at` - Sort by last delivery
- `order=url` - Sort by URL
- `order=-created_at` - Sort by creation date (descending)

## Service Interface

```csharp
public interface IWebhooksService
{
    // Subscription management
    Task<WebhookSubscription> GetSubscriptionAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<WebhookSubscription>> ListSubscriptionsAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<WebhookSubscription> CreateSubscriptionAsync(WebhookSubscriptionCreateRequest request, CancellationToken cancellationToken = default);
    Task<WebhookSubscription> UpdateSubscriptionAsync(string id, WebhookSubscriptionUpdateRequest request, CancellationToken cancellationToken = default);
    Task DeleteSubscriptionAsync(string id, CancellationToken cancellationToken = default);
    
    // Subscription activation
    Task<WebhookSubscription> ActivateSubscriptionAsync(string id, CancellationToken cancellationToken = default);
    Task<WebhookSubscription> DeactivateSubscriptionAsync(string id, CancellationToken cancellationToken = default);
    
    // Available events
    Task<AvailableEvent> GetAvailableEventAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<AvailableEvent>> ListAvailableEventsAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<IPagedResponse<AvailableEvent>> ListAvailableEventsByModuleAsync(string module, QueryParameters parameters = null, CancellationToken cancellationToken = default);
    
    // Event history
    Task<Event> GetEventAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Event>> ListEventsAsync(string subscriptionId, QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Event> RedeliverEventAsync(string eventId, CancellationToken cancellationToken = default);
    
    // Event validation
    Task<bool> ValidateEventSignatureAsync(string payload, string signature, string secret);
    Task<T> DeserializeEventAsync<T>(string payload) where T : class;
    
    // Subscription testing
    Task<WebhookTestResult> TestSubscriptionAsync(string id, CancellationToken cancellationToken = default);
    Task<WebhookTestResult> TestWebhookUrlAsync(string url, string secret, CancellationToken cancellationToken = default);
    
    // Bulk operations
    Task<IPagedResponse<WebhookSubscription>> CreateBulkSubscriptionsAsync(BulkWebhookSubscriptionCreateRequest request, CancellationToken cancellationToken = default);
    Task DeleteBulkSubscriptionsAsync(BulkWebhookSubscriptionDeleteRequest request, CancellationToken cancellationToken = default);
    
    // Analytics and monitoring
    Task<WebhookAnalytics> GetSubscriptionAnalyticsAsync(string subscriptionId, AnalyticsRequest request, CancellationToken cancellationToken = default);
    Task<WebhookDeliveryReport> GenerateDeliveryReportAsync(WebhookReportRequest request, CancellationToken cancellationToken = default);
}
```

## Fluent API Interface

```csharp
public interface IWebhooksFluentContext
{
    // Subscription queries
    IWebhookSubscriptionFluentContext Subscriptions();
    IWebhookSubscriptionFluentContext Subscription(string subscriptionId);
    
    // Available event queries
    IAvailableEventFluentContext AvailableEvents();
    IAvailableEventFluentContext AvailableEvent(string eventId);
    
    // Event queries
    IWebhookEventFluentContext Events();
    IWebhookEventFluentContext Event(string eventId);
    
    // Module-specific events
    IModuleEventFluentContext Module(string moduleName);
    
    // Analytics and reporting
    IWebhookAnalyticsFluentContext Analytics();
}

public interface IWebhookSubscriptionFluentContext
{
    IWebhookSubscriptionFluentContext Where(Expression<Func<WebhookSubscription, bool>> predicate);
    IWebhookSubscriptionFluentContext Include(Expression<Func<WebhookSubscription, object>> include);
    IWebhookSubscriptionFluentContext OrderBy(Expression<Func<WebhookSubscription, object>> orderBy);
    IWebhookSubscriptionFluentContext Active();
    IWebhookSubscriptionFluentContext Inactive();
    IWebhookSubscriptionFluentContext ForUrl(string url);
    IWebhookSubscriptionFluentContext ForEvent(string eventName);
    IWebhookSubscriptionFluentContext WithSuccessfulDeliveries();
    IWebhookSubscriptionFluentContext WithFailedDeliveries();
    
    // Subscription-specific operations
    IWebhookEventFluentContext Events();
    
    Task<WebhookSubscription> GetAsync(CancellationToken cancellationToken = default);
    Task<IPagedResponse<WebhookSubscription>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default);
    Task<List<WebhookSubscription>> GetAllAsync(CancellationToken cancellationToken = default);
}

public interface IAvailableEventFluentContext
{
    IAvailableEventFluentContext Where(Expression<Func<AvailableEvent, bool>> predicate);
    IAvailableEventFluentContext ForModule(string module);
    IAvailableEventFluentContext WithName(string name);
    
    Task<IPagedResponse<AvailableEvent>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default);
    Task<List<AvailableEvent>> GetAllAsync(CancellationToken cancellationToken = default);
}

public interface IWebhookEventFluentContext
{
    IWebhookEventFluentContext Where(Expression<Func<Event, bool>> predicate);
    IWebhookEventFluentContext Include(Expression<Func<Event, object>> include);
    IWebhookEventFluentContext OrderBy(Expression<Func<Event, object>> orderBy);
    IWebhookEventFluentContext Successful();
    IWebhookEventFluentContext Failed();
    IWebhookEventFluentContext Pending();
    IWebhookEventFluentContext DeliveredAfter(DateTime date);
    IWebhookEventFluentContext DeliveredBefore(DateTime date);
    IWebhookEventFluentContext WithEventName(string eventName);
    
    Task<IPagedResponse<Event>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default);
    Task<List<Event>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
}
```

## Usage Examples

### Service-Based API
```csharp
// Create webhook subscriptions for different events
var personSubscription = await webhooksService.CreateSubscriptionAsync(new WebhookSubscriptionCreateRequest
{
    Url = "https://myapp.com/webhooks/planning-center",
    Events = new[] { "person.created", "person.updated", "person.deleted" },
    Secret = "my-webhook-secret-key"
});

var donationSubscription = await webhooksService.CreateSubscriptionAsync(new WebhookSubscriptionCreateRequest
{
    Url = "https://myapp.com/webhooks/donations",
    Events = new[] { "donation.created", "donation.updated", "refund.created" },
    Secret = "my-donation-webhook-secret"
});

// List available events for a specific module
var peopleEvents = await webhooksService.ListAvailableEventsByModuleAsync("people");
var givingEvents = await webhooksService.ListAvailableEventsByModuleAsync("giving");

// Get subscription delivery history
var subscriptionEvents = await webhooksService.ListEventsAsync(personSubscription.Id, new QueryParameters
{
    OrderBy = "-delivered_at",
    PerPage = 50
});

// Redeliver a failed event
var failedEvent = subscriptionEvents.Data.FirstOrDefault(e => e.DeliveryStatus == "failed");
if (failedEvent != null)
{
    await webhooksService.RedeliverEventAsync(failedEvent.Id);
}

// Test a webhook subscription
var testResult = await webhooksService.TestSubscriptionAsync(personSubscription.Id);
if (!testResult.Success)
{
    Console.WriteLine($"Webhook test failed: {testResult.ErrorMessage}");
}

// Validate webhook signature (in your webhook endpoint)
public async Task<IActionResult> HandleWebhook()
{
    var payload = await new StreamReader(Request.Body).ReadToEndAsync();
    var signature = Request.Headers["X-PCO-Webhook-Signature"].FirstOrDefault();
    
    var isValid = await webhooksService.ValidateEventSignatureAsync(payload, signature, "my-webhook-secret");
    if (!isValid)
    {
        return Unauthorized();
    }
    
    var eventType = Request.Headers["X-PCO-Webhook-Event"].FirstOrDefault();
    
    switch (eventType)
    {
        case "person.created":
            var personCreated = await webhooksService.DeserializeEventAsync<PersonCreatedEvent>(payload);
            await HandlePersonCreated(personCreated);
            break;
            
        case "donation.created":
            var donationCreated = await webhooksService.DeserializeEventAsync<DonationCreatedEvent>(payload);
            await HandleDonationCreated(donationCreated);
            break;
    }
    
    return Ok();
}
```

### Fluent API
```csharp
// Complex subscription queries
var activeSubscriptions = await client
    .Webhooks()
    .Subscriptions()
    .Active()
    .WithSuccessfulDeliveries()
    .Include(s => s.AvailableEvent)
    .Include(s => s.Events)
    .OrderByDescending(s => s.LastDeliveryAt)
    .GetPagedAsync(pageSize: 50);

// Get all failed events for troubleshooting
var failedEvents = await client
    .Webhooks()
    .Events()
    .Failed()
    .DeliveredAfter(DateTime.Now.AddDays(-7))
    .Include(e => e.WebhookSubscription)
    .OrderByDescending(e => e.DeliveredAt)
    .GetAllAsync();

// Module-specific event discovery
var peopleEvents = await client
    .Webhooks()
    .Module("people")
    .AvailableEvents()
    .GetAllAsync();

var calendarEvents = await client
    .Webhooks()
    .Module("calendar")
    .AvailableEvents()
    .GetAllAsync();

// Subscription-specific event history
var subscriptionHistory = await client
    .Webhooks()
    .Subscription("subscription123")
    .Events()
    .DeliveredAfter(DateTime.Now.AddDays(-30))
    .OrderByDescending(e => e.DeliveredAt)
    .GetPagedAsync();

// Event type analysis
var donationEvents = await client
    .Webhooks()
    .Events()
    .WithEventName("donation.created")
    .Successful()
    .DeliveredAfter(DateTime.Now.AddDays(-7))
    .GetCountAsync();

// URL-based subscription management
var urlSubscriptions = await client
    .Webhooks()
    .Subscriptions()
    .ForUrl("https://myapp.com/webhooks/planning-center")
    .Active()
    .Include(s => s.AvailableEvent)
    .GetAllAsync();

// Analytics queries
var subscriptionAnalytics = await client
    .Webhooks()
    .Subscription("subscription123")
    .Analytics()
    .ForPeriod(DateTime.Now.AddDays(-30), DateTime.Now)
    .GetAsync();
```

## Webhook Event Handling Framework

### ASP.NET Core Integration
```csharp
// Startup.cs or Program.cs
services.AddPlanningCenterWebhooks(options =>
{
    options.Secret = "your-webhook-secret";
    options.AddHandler<PersonCreatedHandler>("person.created");
    options.AddHandler<PersonUpdatedHandler>("person.updated");
    options.AddHandler<DonationCreatedHandler>("donation.created");
    options.AddHandler<WorkflowCardUpdatedHandler>("workflow_card.updated");
});

// Event handlers
public class PersonCreatedHandler : IWebhookEventHandler<PersonCreatedEvent>
{
    private readonly IEmailService _emailService;
    private readonly IPeopleService _peopleService;
    
    public PersonCreatedHandler(IEmailService emailService, IPeopleService peopleService)
    {
        _emailService = emailService;
        _peopleService = peopleService;
    }
    
    public async Task HandleAsync(PersonCreatedEvent eventData, CancellationToken cancellationToken = default)
    {
        // Send welcome email
        await _emailService.SendWelcomeEmailAsync(eventData.Data.Id);
        
        // Add to new member workflow
        await _peopleService.CreateWorkflowCardAsync(new WorkflowCardCreateRequest
        {
            PersonId = eventData.Data.Id,
            WorkflowId = "new-member-workflow-id"
        });
    }
}

public class DonationCreatedHandler : IWebhookEventHandler<DonationCreatedEvent>
{
    private readonly IEmailService _emailService;
    private readonly IGivingService _givingService;
    
    public async Task HandleAsync(DonationCreatedEvent eventData, CancellationToken cancellationToken = default)
    {
        // Send thank you email
        await _emailService.SendThankYouEmailAsync(eventData.Data.PersonId, eventData.Data.AmountCents);
        
        // Check if first-time donor
        var previousDonations = await _givingService.GetDonationsForPersonAsync(eventData.Data.PersonId);
        if (previousDonations.Data.Count == 1)
        {
            // Trigger first-time donor workflow
            await TriggerFirstTimeDonorWorkflow(eventData.Data.PersonId);
        }
    }
}
```

## Implementation Notes

### Security Considerations
- Implement HMAC-SHA256 signature validation for all webhook events
- Use secure, randomly generated webhook secrets
- Validate webhook URLs and implement HTTPS requirements
- Implement replay attack prevention with timestamp validation

### Reliability Features
- Automatic retry logic with exponential backoff
- Dead letter queue for permanently failed events
- Event deduplication to prevent duplicate processing
- Comprehensive delivery status tracking and reporting

### Performance Considerations
- Asynchronous event processing to prevent blocking
- Batch event delivery for high-volume scenarios
- Rate limiting for webhook endpoints
- Efficient event storage and retrieval

### Monitoring and Observability
- Comprehensive delivery metrics and analytics
- Real-time monitoring of webhook health
- Alerting for failed deliveries and subscription issues
- Detailed logging for troubleshooting

### Event Schema Validation
- JSON schema validation for event payloads
- Versioned event schemas for backward compatibility
- Type-safe event deserialization
- Event payload documentation and examples

This module provides comprehensive webhook management capabilities while maintaining security, reliability, and performance standards for real-time event processing systems.