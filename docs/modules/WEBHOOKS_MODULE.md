# Webhooks Module Documentation

The Webhooks module provides comprehensive webhook management capabilities for Planning Center. This module handles all webhook-related functionality including webhook subscriptions, event handling, and notification management.

## 📋 Module Overview

### Key Entities
- **Webhook Subscriptions**: Configured endpoints to receive event notifications
- **Available Events**: Types of events that can trigger webhooks
- **Webhook Events**: Individual webhook delivery records
- **Event Deliveries**: Tracking of webhook delivery attempts and status

### Authentication
Requires Planning Center Webhooks app access with appropriate permissions.

## 🔧 Traditional Service API

### Webhook Subscription Management

#### Basic Subscription Operations
```csharp
public class WebhooksService
{
    private readonly IWebhooksService _webhooksService;
    
    public WebhooksService(IWebhooksService webhooksService)
    {
        _webhooksService = webhooksService;
    }
    
    // Get a webhook subscription by ID
    public async Task<WebhookSubscription?> GetSubscriptionAsync(string id)
    {
        return await _webhooksService.GetWebhookSubscriptionAsync(id);
    }
    
    // List webhook subscriptions
    public async Task<IPagedResponse<WebhookSubscription>> GetSubscriptionsAsync()
    {
        return await _webhooksService.ListWebhookSubscriptionsAsync(new QueryParameters
        {
            PerPage = 25,
            OrderBy = "name"
        });
    }
}
```

#### Create and Update Subscriptions
```csharp
// Create a new webhook subscription
var newSubscription = await _webhooksService.CreateWebhookSubscriptionAsync(new WebhookSubscriptionCreateRequest
{
    Name = "Person Updates",
    Url = "https://myapp.com/webhooks/people",
    EventTypes = new[] { "person.created", "person.updated", "person.deleted" },
    Active = true,
    IncludeBody = true
});

// Update a webhook subscription
var updatedSubscription = await _webhooksService.UpdateWebhookSubscriptionAsync("sub123", new WebhookSubscriptionUpdateRequest
{
    Name = "Person & Household Updates",
    EventTypes = new[] { "person.created", "person.updated", "person.deleted", "household.created" },
    Active = true
});

// Delete a webhook subscription
await _webhooksService.DeleteWebhookSubscriptionAsync("sub123");
```

### Available Events Management

#### Available Events Operations
```csharp
// Get available event types
var availableEvents = await _webhooksService.ListAvailableEventsAsync(new QueryParameters
{
    OrderBy = "name"
});

// Get specific available event
var availableEvent = await _webhooksService.GetAvailableEventAsync("event123");
```

### Webhook Events Management

#### Webhook Events Operations
```csharp
// Get a webhook event by ID
var webhookEvent = await _webhooksService.GetWebhookEventAsync("event123");

// List webhook events for a subscription
var subscriptionEvents = await _webhooksService.ListWebhookEventsAsync(new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["webhook_subscription_id"] = "sub123"
    },
    OrderBy = "-created_at"
});

// List recent webhook events
var recentEvents = await _webhooksService.ListWebhookEventsAsync(new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["created_at"] = $"{DateTime.Today.AddDays(-7):yyyy-MM-dd}.."
    },
    OrderBy = "-created_at"
});
```

## 🚀 Fluent API Usage

### Basic Queries
```csharp
public class WebhooksFluentService
{
    private readonly IPlanningCenterClient _client;
    
    public WebhooksFluentService(IPlanningCenterClient client)
    {
        _client = client;
    }
    
    // Get active webhook subscriptions
    public async Task<IReadOnlyList<WebhookSubscription>> GetActiveSubscriptionsAsync()
    {
        return await _client.Webhooks()
            .Where(w => w.Active == true)
            .OrderBy(w => w.Name)
            .GetAllAsync();
    }
}
```

### Advanced Webhook Filtering
```csharp
// Get subscriptions for specific event types
var personWebhooks = await _client.Webhooks()
    .Where(w => w.EventTypes.Contains("person.created"))
    .Where(w => w.Active == true)
    .OrderBy(w => w.Name)
    .GetAllAsync();

// Get failed webhook events
var failedEvents = await _client.Webhooks()
    .Events()
    .Where(e => e.DeliveryStatus == "failed")
    .Where(e => e.CreatedAt >= DateTime.Today.AddDays(-7))
    .OrderByDescending(e => e.CreatedAt)
    .GetPagedAsync(pageSize: 50);

// Get webhook events by type
var personEvents = await _client.Webhooks()
    .Events()
    .Where(e => e.EventType.StartsWith("person."))
    .OrderByDescending(e => e.CreatedAt)
    .GetAllAsync();
```

### Available Events Filtering
```csharp
// Get available events by application
var peopleEvents = await _client.Webhooks()
    .AvailableEvents()
    .Where(e => e.Application == "people")
    .OrderBy(e => e.Name)
    .GetAllAsync();

// Get all available event types
var allEventTypes = await _client.Webhooks()
    .AvailableEvents()
    .OrderBy(e => e.Application)
    .ThenBy(e => e.Name)
    .GetAllAsync();
```

## 💡 Common Use Cases

### 1. Webhook Health Dashboard
```csharp
public async Task<WebhookHealthDashboard> GetWebhookHealthDashboardAsync()
{
    var subscriptions = await _client.Webhooks()
        .GetAllAsync();
    
    var recentEvents = await _client.Webhooks()
        .Events()
        .Where(e => e.CreatedAt >= DateTime.Today.AddDays(-7))
        .GetAllAsync();
    
    return new WebhookHealthDashboard
    {
        TotalSubscriptions = subscriptions.Count,
        ActiveSubscriptions = subscriptions.Count(s => s.Active),
        InactiveSubscriptions = subscriptions.Count(s => !s.Active),
        RecentEvents = recentEvents.Count,
        FailedEvents = recentEvents.Count(e => e.DeliveryStatus == "failed"),
        SuccessfulEvents = recentEvents.Count(e => e.DeliveryStatus == "successful"),
        SuccessRate = recentEvents.Any() ? 
            (recentEvents.Count(e => e.DeliveryStatus == "successful") / (double)recentEvents.Count) * 100 : 0
    };
}

public class WebhookHealthDashboard
{
    public int TotalSubscriptions { get; set; }
    public int ActiveSubscriptions { get; set; }
    public int InactiveSubscriptions { get; set; }
    public int RecentEvents { get; set; }
    public int FailedEvents { get; set; }
    public int SuccessfulEvents { get; set; }
    public double SuccessRate { get; set; }
}
```

### 2. Event Type Analysis
```csharp
public async Task<EventTypeAnalysis> AnalyzeEventTypesAsync(DateTime startDate, DateTime endDate)
{
    var events = await _client.Webhooks()
        .Events()
        .Where(e => e.CreatedAt >= startDate)
        .Where(e => e.CreatedAt <= endDate)
        .GetAllAsync();
    
    var eventTypeStats = events
        .GroupBy(e => e.EventType)
        .Select(g => new EventTypeStats
        {
            EventType = g.Key,
            TotalEvents = g.Count(),
            SuccessfulEvents = g.Count(e => e.DeliveryStatus == "successful"),
            FailedEvents = g.Count(e => e.DeliveryStatus == "failed"),
            SuccessRate = g.Any() ? (g.Count(e => e.DeliveryStatus == "successful") / (double)g.Count()) * 100 : 0
        })
        .OrderByDescending(s => s.TotalEvents)
        .ToList();
    
    return new EventTypeAnalysis
    {
        Period = $"{startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}",
        TotalEvents = events.Count,
        EventTypeStats = eventTypeStats
    };
}

public class EventTypeAnalysis
{
    public string Period { get; set; } = "";
    public int TotalEvents { get; set; }
    public List<EventTypeStats> EventTypeStats { get; set; } = new();
}

public class EventTypeStats
{
    public string EventType { get; set; } = "";
    public int TotalEvents { get; set; }
    public int SuccessfulEvents { get; set; }
    public int FailedEvents { get; set; }
    public double SuccessRate { get; set; }
}
```

### 3. Subscription Management
```csharp
public async Task<SubscriptionReport> GetSubscriptionReportAsync()
{
    var subscriptions = await _client.Webhooks()
        .GetAllAsync();
    
    var availableEvents = await _client.Webhooks()
        .AvailableEvents()
        .GetAllAsync();
    
    var subscriptionDetails = subscriptions.Select(s => new SubscriptionDetail
    {
        Id = s.Id,
        Name = s.Name,
        Url = s.Url,
        Active = s.Active,
        EventTypes = s.EventTypes.ToList(),
        EventTypeCount = s.EventTypes.Count(),
        IncludeBody = s.IncludeBody
    }).ToList();
    
    return new SubscriptionReport
    {
        TotalSubscriptions = subscriptions.Count,
        ActiveSubscriptions = subscriptions.Count(s => s.Active),
        TotalAvailableEvents = availableEvents.Count,
        Subscriptions = subscriptionDetails
    };
}

public class SubscriptionReport
{
    public int TotalSubscriptions { get; set; }
    public int ActiveSubscriptions { get; set; }
    public int TotalAvailableEvents { get; set; }
    public List<SubscriptionDetail> Subscriptions { get; set; } = new();
}

public class SubscriptionDetail
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Url { get; set; } = "";
    public bool Active { get; set; }
    public List<string> EventTypes { get; set; } = new();
    public int EventTypeCount { get; set; }
    public bool IncludeBody { get; set; }
}
```

### 4. Failed Event Retry Management
```csharp
public async Task<IReadOnlyList<WebhookEvent>> GetFailedEventsForRetryAsync(int maxAge = 24)
{
    var cutoffTime = DateTime.Now.AddHours(-maxAge);
    
    return await _client.Webhooks()
        .Events()
        .Where(e => e.DeliveryStatus == "failed")
        .Where(e => e.CreatedAt >= cutoffTime)
        .OrderBy(e => e.CreatedAt)
        .GetAllAsync();
}

public async Task<RetryReport> RetryFailedEventsAsync(IEnumerable<string> eventIds)
{
    var retryResults = new List<RetryResult>();
    
    foreach (var eventId in eventIds)
    {
        try
        {
            // Note: Actual retry logic would depend on Planning Center API capabilities
            // This is a conceptual example
            var result = new RetryResult
            {
                EventId = eventId,
                Success = true,
                Message = "Retry successful"
            };
            retryResults.Add(result);
        }
        catch (Exception ex)
        {
            var result = new RetryResult
            {
                EventId = eventId,
                Success = false,
                Message = ex.Message
            };
            retryResults.Add(result);
        }
    }
    
    return new RetryReport
    {
        TotalAttempts = retryResults.Count,
        SuccessfulRetries = retryResults.Count(r => r.Success),
        FailedRetries = retryResults.Count(r => !r.Success),
        Results = retryResults
    };
}

public class RetryReport
{
    public int TotalAttempts { get; set; }
    public int SuccessfulRetries { get; set; }
    public int FailedRetries { get; set; }
    public List<RetryResult> Results { get; set; } = new();
}

public class RetryResult
{
    public string EventId { get; set; } = "";
    public bool Success { get; set; }
    public string Message { get; set; } = "";
}
```

### 5. Webhook Configuration Backup
```csharp
public async Task<WebhookConfiguration> BackupWebhookConfigurationAsync()
{
    var subscriptions = await _client.Webhooks()
        .GetAllAsync();
    
    var availableEvents = await _client.Webhooks()
        .AvailableEvents()
        .GetAllAsync();
    
    return new WebhookConfiguration
    {
        BackupDate = DateTime.UtcNow,
        Subscriptions = subscriptions.Select(s => new WebhookSubscriptionBackup
        {
            Name = s.Name,
            Url = s.Url,
            EventTypes = s.EventTypes.ToList(),
            Active = s.Active,
            IncludeBody = s.IncludeBody
        }).ToList(),
        AvailableEventTypes = availableEvents.Select(e => e.Name).ToList()
    };
}

public class WebhookConfiguration
{
    public DateTime BackupDate { get; set; }
    public List<WebhookSubscriptionBackup> Subscriptions { get; set; } = new();
    public List<string> AvailableEventTypes { get; set; } = new();
}

public class WebhookSubscriptionBackup
{
    public string Name { get; set; } = "";
    public string Url { get; set; } = "";
    public List<string> EventTypes { get; set; } = new();
    public bool Active { get; set; }
    public bool IncludeBody { get; set; }
}
```

## 📊 Advanced Features

### Webhook Event Monitoring
```csharp
// Monitor webhook delivery performance
public async Task<DeliveryPerformanceReport> GetDeliveryPerformanceAsync(DateTime startDate, DateTime endDate)
{
    var events = await _client.Webhooks()
        .Events()
        .Where(e => e.CreatedAt >= startDate)
        .Where(e => e.CreatedAt <= endDate)
        .GetAllAsync();
    
    var performanceBySubscription = events
        .GroupBy(e => e.WebhookSubscriptionId)
        .ToDictionary(g => g.Key, g => new SubscriptionPerformance
        {
            TotalEvents = g.Count(),
            SuccessfulEvents = g.Count(e => e.DeliveryStatus == "successful"),
            FailedEvents = g.Count(e => e.DeliveryStatus == "failed"),
            AverageResponseTime = g.Where(e => e.ResponseTimeMs.HasValue).Average(e => e.ResponseTimeMs ?? 0)
        });
    
    return new DeliveryPerformanceReport
    {
        Period = $"{startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}",
        TotalEvents = events.Count,
        OverallSuccessRate = events.Any() ? (events.Count(e => e.DeliveryStatus == "successful") / (double)events.Count) * 100 : 0,
        PerformanceBySubscription = performanceBySubscription
    };
}

public class DeliveryPerformanceReport
{
    public string Period { get; set; } = "";
    public int TotalEvents { get; set; }
    public double OverallSuccessRate { get; set; }
    public Dictionary<string, SubscriptionPerformance> PerformanceBySubscription { get; set; } = new();
}

public class SubscriptionPerformance
{
    public int TotalEvents { get; set; }
    public int SuccessfulEvents { get; set; }
    public int FailedEvents { get; set; }
    public double AverageResponseTime { get; set; }
    public double SuccessRate => TotalEvents > 0 ? (SuccessfulEvents / (double)TotalEvents) * 100 : 0;
}
```

### Batch Operations
```csharp
// Create multiple webhook subscriptions in a batch
var batch = _client.Webhooks().Batch();

batch.CreateSubscription(new WebhookSubscriptionCreateRequest
{
    Name = "People Events",
    Url = "https://myapp.com/webhooks/people",
    EventTypes = new[] { "person.created", "person.updated" }
});

batch.CreateSubscription(new WebhookSubscriptionCreateRequest
{
    Name = "Giving Events",
    Url = "https://myapp.com/webhooks/giving",
    EventTypes = new[] { "donation.created", "donation.updated" }
});

var results = await batch.ExecuteAsync();
```

## 🎯 Best Practices

1. **Monitor Delivery Status**: Regularly check webhook delivery success rates
2. **Handle Failures Gracefully**: Implement retry logic for failed deliveries
3. **Secure Webhook Endpoints**: Use HTTPS and validate webhook signatures
4. **Filter Event Types**: Subscribe only to events you actually need
5. **Test Webhook Endpoints**: Verify endpoints can handle the expected load
6. **Monitor Performance**: Track response times and delivery rates
7. **Backup Configurations**: Regularly backup webhook subscription settings
8. **Handle Duplicates**: Implement idempotency in webhook handlers
9. **Log Webhook Events**: Maintain logs for debugging and monitoring
10. **Update Subscriptions**: Keep event type subscriptions current with your needs

### Error Handling
```csharp
public async Task<WebhookSubscription?> SafeCreateSubscriptionAsync(WebhookSubscriptionCreateRequest request)
{
    try
    {
        return await _client.Webhooks().CreateSubscription(request).ExecuteAsync();
    }
    catch (PlanningCenterApiValidationException ex)
    {
        // Handle validation errors (e.g., invalid URL, unsupported event types)
        _logger.LogWarning(ex, "Validation error when creating webhook subscription: {Errors}", ex.FormattedErrors);
        throw;
    }
    catch (PlanningCenterApiAuthorizationException ex)
    {
        // Handle permission errors
        _logger.LogError(ex, "Authorization error when creating webhook subscription: {Message}", ex.Message);
        throw;
    }
    catch (PlanningCenterApiException ex)
    {
        // Log API error with correlation ID
        _logger.LogError(ex, "API error when creating webhook subscription: {ErrorMessage} [RequestId: {RequestId}]", 
            ex.Message, ex.RequestId);
        throw;
    }
}
```

This Webhooks module documentation provides comprehensive coverage of webhook management capabilities with both traditional and fluent API usage patterns, including practical examples for common webhook scenarios.