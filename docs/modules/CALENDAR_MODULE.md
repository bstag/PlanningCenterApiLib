# Calendar Module Documentation

The Calendar module provides comprehensive event and resource management capabilities for Planning Center. This module handles all calendar-related functionality including events, resources, scheduling, and calendar management.

## 📋 Module Overview

### Key Entities
- **Events**: Calendar events with scheduling, location, and registration information
- **Resources**: Bookable resources like rooms, equipment, and facilities
- **Event Instances**: Recurring event occurrences
- **Resource Bookings**: Resource reservations for events
- **Event Connections**: Relationships between events and other Planning Center data

### Authentication
Requires Planning Center Calendar app access with appropriate permissions.

## 🔧 Traditional Service API

### Event Management

#### Basic Event Operations
```csharp
public class CalendarService
{
    private readonly ICalendarService _calendarService;
    
    public CalendarService(ICalendarService calendarService)
    {
        _calendarService = calendarService;
    }
    
    // Get an event by ID
    public async Task<Event?> GetEventAsync(string id)
    {
        return await _calendarService.GetEventAsync(id);
    }
    
    // List events with pagination
    public async Task<IPagedResponse<Event>> GetEventsAsync()
    {
        return await _calendarService.ListEventsAsync(new QueryParameters
        {
            PerPage = 25,
            OrderBy = "starts_at"
        });
    }
}
```

#### Create and Update Events
```csharp
// Create a new event
var newEvent = await _calendarService.CreateEventAsync(new EventCreateRequest
{
    Name = "Sunday Service",
    Summary = "Weekly worship service",
    Description = "Join us for worship, teaching, and fellowship",
    AllDayEvent = false,
    StartsAt = DateTime.Now.AddDays(7).Date.AddHours(10), // Next Sunday at 10 AM
    EndsAt = DateTime.Now.AddDays(7).Date.AddHours(11.5), // 1.5 hours later
    LocationName = "Main Sanctuary",
    VisibleInChurchCenter = true,
    RegistrationRequired = false
});

// Update an event
var updatedEvent = await _calendarService.UpdateEventAsync("event123", new EventUpdateRequest
{
    Name = "Sunday Worship Service",
    Description = "Updated description with more details",
    LocationName = "Sanctuary - Main Campus"
});

// Delete an event
await _calendarService.DeleteEventAsync("event123");
```

#### Date Range Queries
```csharp
// Get events in a specific date range
var startDate = DateTime.Today;
var endDate = DateTime.Today.AddDays(30);

var upcomingEvents = await _calendarService.ListEventsByDateRangeAsync(
    startDate, 
    endDate, 
    new QueryParameters
    {
        PerPage = 50,
        OrderBy = "starts_at"
    });

// Get events for this week
var weekStart = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
var weekEnd = weekStart.AddDays(6);

var thisWeekEvents = await _calendarService.ListEventsByDateRangeAsync(weekStart, weekEnd);
```

### Resource Management

#### Basic Resource Operations
```csharp
// Get a resource by ID
var resource = await _calendarService.GetResourceAsync("resource123");

// List all resources
var resources = await _calendarService.ListResourcesAsync(new QueryParameters
{
    PerPage = 50,
    OrderBy = "name"
});

// Create a new resource
var newResource = await _calendarService.CreateResourceAsync(new ResourceCreateRequest
{
    Name = "Main Sanctuary",
    Description = "Primary worship space with 500 seat capacity",
    Kind = "Room",
    HomeLocation = "Main Campus",
    Quantity = 1
});

// Update a resource
var updatedResource = await _calendarService.UpdateResourceAsync("resource123", new ResourceUpdateRequest
{
    Name = "Main Sanctuary - Renovated",
    Description = "Newly renovated worship space with 600 seat capacity",
    Quantity = 1
});

// Delete a resource
await _calendarService.DeleteResourceAsync("resource123");
```

#### Resource with Expiration
```csharp
// Create a resource that expires (like equipment)
var equipment = await _calendarService.CreateResourceAsync(new ResourceCreateRequest
{
    Name = "Portable Sound System",
    Description = "Wireless microphone system for outdoor events",
    Kind = "Equipment",
    SerialNumber = "PSS-2024-001",
    Expires = true,
    ExpiresAt = DateTime.Now.AddYears(1), // Expires in 1 year
    RenewalAt = DateTime.Now.AddMonths(11), // Renewal reminder 1 month before
    Quantity = 2
});
```

### Pagination Helpers
```csharp
// Get all events (handles pagination automatically)
var allEvents = await _calendarService.GetAllEventsAsync(new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["visible_in_church_center"] = true
    }
});

// Stream events for memory-efficient processing
await foreach (var eventItem in _calendarService.StreamEventsAsync())
{
    Console.WriteLine($"{eventItem.Name} - {eventItem.StartsAt:yyyy-MM-dd HH:mm}");
    // Process one event at a time without loading all into memory
}
```

## 🚀 Fluent API Usage

### Basic Queries
```csharp
public class CalendarFluentService
{
    private readonly IPlanningCenterClient _client;
    
    public CalendarFluentService(IPlanningCenterClient client)
    {
        _client = client;
    }
    
    // Get an event by ID
    public async Task<Event?> GetEventAsync(string id)
    {
        return await _client.Calendar().GetAsync(id);
    }
    
    // Get first page of events
    public async Task<IPagedResponse<Event>> GetEventsPageAsync()
    {
        return await _client.Calendar().GetPagedAsync(pageSize: 25);
    }
}
```

### Advanced Event Filtering
```csharp
// Get upcoming public events
var upcomingPublicEvents = await _client.Calendar()
    .Where(e => e.StartsAt > DateTime.Now)
    .Where(e => e.VisibleInChurchCenter == true)
    .OrderBy(e => e.StartsAt)
    .GetAllAsync();

// Get events requiring registration
var registrationEvents = await _client.Calendar()
    .Where(e => e.RegistrationRequired == true)
    .Where(e => e.StartsAt > DateTime.Now)
    .OrderBy(e => e.StartsAt)
    .GetPagedAsync(pageSize: 50);

// Get all-day events
var allDayEvents = await _client.Calendar()
    .Where(e => e.AllDayEvent == true)
    .Where(e => e.StartsAt >= DateTime.Today)
    .OrderBy(e => e.StartsAt)
    .GetAllAsync();

// Get events by location
var sanctuaryEvents = await _client.Calendar()
    .Where(e => e.LocationName.Contains("Sanctuary"))
    .Where(e => e.StartsAt > DateTime.Now)
    .OrderBy(e => e.StartsAt)
    .GetAllAsync();
```

### Resource Filtering
```csharp
// Get available rooms
var rooms = await _client.Calendar()
    .Resources()
    .Where(r => r.Kind == "Room")
    .Where(r => r.Quantity > 0)
    .OrderBy(r => r.Name)
    .GetAllAsync();

// Get equipment that expires soon
var expiringEquipment = await _client.Calendar()
    .Resources()
    .Where(r => r.Expires == true)
    .Where(r => r.ExpiresAt < DateTime.Now.AddMonths(3))
    .OrderBy(r => r.ExpiresAt)
    .GetAllAsync();
```

### LINQ-like Terminal Operations
```csharp
// Get next upcoming event
var nextEvent = await _client.Calendar()
    .Where(e => e.StartsAt > DateTime.Now)
    .Where(e => e.VisibleInChurchCenter == true)
    .OrderBy(e => e.StartsAt)
    .FirstAsync();

// Get event by name (should be unique)
var sundayService = await _client.Calendar()
    .Where(e => e.Name == "Sunday Service")
    .Where(e => e.StartsAt > DateTime.Today)
    .SingleOrDefaultAsync();

// Count upcoming events
var upcomingCount = await _client.Calendar()
    .Where(e => e.StartsAt > DateTime.Now)
    .CountAsync();

// Check if any events need registration
var hasRegistrationEvents = await _client.Calendar()
    .Where(e => e.RegistrationRequired == true)
    .Where(e => e.StartsAt > DateTime.Now)
    .AnyAsync();
```

### Memory-Efficient Streaming
```csharp
// Stream all upcoming events
await foreach (var eventItem in _client.Calendar()
    .Where(e => e.StartsAt > DateTime.Now)
    .OrderBy(e => e.StartsAt)
    .AsAsyncEnumerable())
{
    // Process one event at a time
    await ProcessEventAsync(eventItem);
}

// Stream with custom pagination
var options = new PaginationOptions { PageSize = 100 };
await foreach (var eventItem in _client.Calendar()
    .Where(e => e.VisibleInChurchCenter == true)
    .AsAsyncEnumerable(options))
{
    await ProcessEventAsync(eventItem);
}
```

### Fluent Event Creation
```csharp
// Create an event with fluent syntax
var newEvent = await _client.Calendar()
    .CreateEvent(new EventCreateRequest
    {
        Name = "Youth Group Meeting",
        StartsAt = DateTime.Now.AddDays(7).Date.AddHours(19), // Next week at 7 PM
        EndsAt = DateTime.Now.AddDays(7).Date.AddHours(21),   // Until 9 PM
        LocationName = "Youth Room"
    })
    .WithDescription("Weekly youth group gathering with games and teaching")
    .WithRegistration(required: true, url: "https://example.com/register")
    .MakeVisible()
    .ExecuteAsync();
```

## 💡 Common Use Cases

### 1. Weekly Schedule Display
```csharp
public async Task<IReadOnlyList<Event>> GetWeeklyScheduleAsync(DateTime weekStart)
{
    var weekEnd = weekStart.AddDays(6).Date.AddHours(23).AddMinutes(59);
    
    return await _client.Calendar()
        .Where(e => e.StartsAt >= weekStart)
        .Where(e => e.StartsAt <= weekEnd)
        .Where(e => e.VisibleInChurchCenter == true)
        .OrderBy(e => e.StartsAt)
        .ThenBy(e => e.Name)
        .GetAllAsync();
}
```

### 2. Event Registration Report
```csharp
public async Task<IReadOnlyList<Event>> GetEventsRequiringRegistrationAsync()
{
    return await _client.Calendar()
        .Where(e => e.RegistrationRequired == true)
        .Where(e => e.StartsAt > DateTime.Now)
        .Where(e => e.RegistrationUrl != null)
        .OrderBy(e => e.StartsAt)
        .GetAllAsync();
}
```

### 3. Resource Availability Check
```csharp
public async Task<bool> IsResourceAvailableAsync(string resourceName, DateTime startTime, DateTime endTime)
{
    // This would require additional API calls to check bookings
    // For now, we'll check if the resource exists and is active
    var resource = await _client.Calendar()
        .Resources()
        .Where(r => r.Name == resourceName)
        .Where(r => r.Quantity > 0)
        .FirstOrDefaultAsync();
    
    return resource != null;
}

public async Task<IReadOnlyList<Resource>> GetAvailableResourcesAsync(string resourceType)
{
    return await _client.Calendar()
        .Resources()
        .Where(r => r.Kind == resourceType)
        .Where(r => r.Quantity > 0)
        .Where(r => !r.Expires || r.ExpiresAt > DateTime.Now)
        .OrderBy(r => r.Name)
        .GetAllAsync();
}
```

### 4. Event Calendar Export
```csharp
public async Task ExportCalendarAsync(string filePath, DateTime startDate, DateTime endDate)
{
    using var writer = new StreamWriter(filePath);
    await writer.WriteLineAsync("Name,Start,End,Location,Description,Registration Required");
    
    await foreach (var eventItem in _client.Calendar()
        .Where(e => e.StartsAt >= startDate)
        .Where(e => e.StartsAt <= endDate)
        .Where(e => e.VisibleInChurchCenter == true)
        .OrderBy(e => e.StartsAt)
        .AsAsyncEnumerable())
    {
        var line = $"\"{eventItem.Name}\"," +
                   $"{eventItem.StartsAt:yyyy-MM-dd HH:mm}," +
                   $"{eventItem.EndsAt:yyyy-MM-dd HH:mm}," +
                   $"\"{eventItem.LocationName}\"," +
                   $"\"{eventItem.Description?.Replace("\"", "\"\"")}\"," +
                   $"{eventItem.RegistrationRequired}";
        
        await writer.WriteLineAsync(line);
    }
}
```

### 5. Recurring Event Management
```csharp
public async Task CreateRecurringEventAsync(EventCreateRequest template, int weeks)
{
    var events = new List<EventCreateRequest>();
    
    for (int i = 0; i < weeks; i++)
    {
        var eventRequest = new EventCreateRequest
        {
            Name = template.Name,
            Summary = template.Summary,
            Description = template.Description,
            AllDayEvent = template.AllDayEvent,
            StartsAt = template.StartsAt.AddDays(i * 7),
            EndsAt = template.EndsAt?.AddDays(i * 7),
            LocationName = template.LocationName,
            VisibleInChurchCenter = template.VisibleInChurchCenter,
            RegistrationRequired = template.RegistrationRequired,
            RegistrationUrl = template.RegistrationUrl
        };
        
        events.Add(eventRequest);
    }
    
    // Create events in batch
    var batch = _client.Calendar().Batch();
    foreach (var eventRequest in events)
    {
        batch.CreateEvent(eventRequest);
    }
    
    await batch.ExecuteAsync();
}
```

### 6. Event Conflict Detection
```csharp
public async Task<IReadOnlyList<Event>> FindConflictingEventsAsync(
    DateTime startTime, 
    DateTime endTime, 
    string location)
{
    return await _client.Calendar()
        .Where(e => e.LocationName == location)
        .Where(e => e.StartsAt < endTime)
        .Where(e => e.EndsAt > startTime)
        .OrderBy(e => e.StartsAt)
        .GetAllAsync();
}
```

## 📊 Advanced Features

### Date Range Queries
```csharp
// Get events for the next month
var nextMonth = await _client.Calendar()
    .Where(e => e.StartsAt >= DateTime.Today)
    .Where(e => e.StartsAt < DateTime.Today.AddMonths(1))
    .OrderBy(e => e.StartsAt)
    .GetAllAsync();

// Get events for a specific day
var today = DateTime.Today;
var tomorrow = today.AddDays(1);

var todayEvents = await _client.Calendar()
    .Where(e => e.StartsAt >= today)
    .Where(e => e.StartsAt < tomorrow)
    .OrderBy(e => e.StartsAt)
    .GetAllAsync();
```

### Performance Monitoring
```csharp
// Monitor query performance
var result = await _client.Calendar()
    .Where(e => e.StartsAt > DateTime.Now)
    .Where(e => e.VisibleInChurchCenter == true)
    .ExecuteWithDebugInfoAsync();

Console.WriteLine($"Query took {result.ExecutionTime.TotalMilliseconds}ms");
Console.WriteLine($"Returned {result.Data?.Data.Count ?? 0} events");
```

### Custom Parameters
```csharp
// Add custom parameters for advanced filtering
var customQuery = await _client.Calendar()
    .Where(e => e.VisibleInChurchCenter == true)
    .WithParameter("include", "event_instances,resource_bookings")
    .WithParameter("filter", "future")
    .GetPagedAsync();
```

### Batch Operations
```csharp
// Create multiple events in a batch
var batch = _client.Calendar().Batch();

batch.CreateEvent(new EventCreateRequest
{
    Name = "Sunday Service",
    StartsAt = DateTime.Now.AddDays(7).Date.AddHours(10)
});

batch.CreateEvent(new EventCreateRequest
{
    Name = "Wednesday Prayer",
    StartsAt = DateTime.Now.AddDays(10).Date.AddHours(19)
});

batch.CreateResource(new ResourceCreateRequest
{
    Name = "New Meeting Room",
    Kind = "Room",
    Quantity = 1
});

var results = await batch.ExecuteAsync();
```

## 🎯 Best Practices

1. **Use Date Range Queries**: Always filter by date ranges to avoid loading unnecessary historical data.

2. **Filter by Visibility**: Use `VisibleInChurchCenter` to show only public events when appropriate.

3. **Order by Start Time**: Most calendar queries should be ordered by `StartsAt` for logical display.

4. **Handle Time Zones**: Be aware of time zone considerations when working with event times.

5. **Check Registration Requirements**: Always check `RegistrationRequired` and `RegistrationUrl` for events.

6. **Stream Large Date Ranges**: Use streaming for queries that might return many events.

7. **Validate Resource Availability**: Check resource quantity and expiration before booking.

8. **Use Batch Operations**: Create multiple related events or resources in batches for better performance.

9. **Monitor Performance**: Use built-in performance monitoring for optimization.

10. **Handle Conflicts**: Always check for scheduling conflicts when creating events.

### Error Handling
```csharp
public async Task<Event?> SafeGetEventAsync(string id)
{
    try
    {
        return await _client.Calendar().GetAsync(id);
    }
    catch (PlanningCenterApiNotFoundException)
    {
        // Event doesn't exist
        return null;
    }
    catch (PlanningCenterApiValidationException ex)
    {
        // Handle validation errors (e.g., invalid date ranges)
        _logger.LogWarning(ex, "Validation error when getting event {EventId}: {Errors}", 
            id, ex.FormattedErrors);
        throw;
    }
    catch (PlanningCenterApiException ex)
    {
        // Log API error with correlation ID
        _logger.LogError(ex, "API error when getting event {EventId}: {ErrorMessage} [RequestId: {RequestId}]", 
            id, ex.Message, ex.RequestId);
        throw;
    }
}
```

This Calendar module documentation provides comprehensive coverage of both traditional and fluent API usage patterns, with practical examples for common calendar management scenarios.