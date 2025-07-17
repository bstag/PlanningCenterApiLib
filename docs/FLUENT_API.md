# Planning Center Fluent API

The Planning Center SDK provides a fluent API that offers LINQ-like syntax for querying and manipulating data. This API is built on top of the standard API calls and provides a more developer-friendly interface without affecting the underlying functionality.

## Overview

The fluent API provides:
- **LINQ-like syntax** for familiar .NET development patterns
- **Method chaining** for readable and expressive code
- **Type-safe queries** using C# expressions
- **Built-in pagination** handling for large datasets
- **Memory-efficient streaming** for processing large amounts of data
- **Fluent creation** of entities with related data
- **Performance monitoring** and query optimization insights

## Getting Started

### Basic Usage

```csharp
using PlanningCenter.Api.Client.Extensions;

// Get the fluent API interface
var fluentClient = client.Fluent();

// Access module-specific fluent contexts
var people = fluentClient.People;          // ✅ Full implementation
var giving = fluentClient.Giving;          // ✅ Full implementation  
var calendar = fluentClient.Calendar;      // ✅ Full implementation
var groups = fluentClient.Groups;          // ✅ Full implementation
var services = fluentClient.Services;      // ✅ Full implementation
var checkIns = fluentClient.CheckIns;      // ✅ Full implementation
var registrations = fluentClient.Registrations; // ✅ Full implementation
var publishing = fluentClient.Publishing;  // ✅ Full implementation
var webhooks = fluentClient.Webhooks;      // ✅ Full implementation
```

### Simple Queries

```csharp
// Get first 10 people
var people = await client.Fluent().People
    .GetPagedAsync(pageSize: 10);

// Get a specific person by ID
var person = await client.Fluent().People
    .GetAsync("123");

// Get all people (handles pagination automatically)
var allPeople = await client.Fluent().People
    .GetAllAsync();
```

## Query Building

### Filtering with Where

```csharp
// Single condition
var johns = await client.Fluent().People
    .Where(p => p.FirstName == "John")
    .GetPagedAsync();

// Multiple conditions (AND logic)
var johnSmiths = await client.Fluent().People
    .Where(p => p.FirstName == "John")
    .Where(p => p.LastName.Contains("Smith"))
    .GetPagedAsync();
```

### Including Related Data

```csharp
// Include related entities
var peopleWithAddresses = await client.Fluent().People
    .Include(p => p.Addresses)
    .Include(p => p.Emails)
    .GetPagedAsync();
```

### Sorting

```csharp
// Primary sort
var sortedPeople = await client.Fluent().People
    .OrderBy(p => p.LastName)
    .GetPagedAsync();

// Multiple sort levels
var complexSort = await client.Fluent().People
    .OrderBy(p => p.LastName)
    .ThenBy(p => p.FirstName)
    .ThenByDescending(p => p.CreatedAt)
    .GetPagedAsync();
```

### Complex Queries

```csharp
// Combine filtering, including, and sorting
var complexQuery = await client.Fluent().People
    .Where(p => p.FirstName == "John")
    .Where(p => p.CreatedAt > DateTime.Now.AddDays(-30))
    .Include(p => p.Addresses)
    .Include(p => p.PhoneNumbers)
    .OrderBy(p => p.LastName)
    .ThenBy(p => p.FirstName)
    .GetPagedAsync(pageSize: 20);
```

## LINQ-like Operations

### Terminal Operations

```csharp
// Get first person
var firstPerson = await client.Fluent().People
    .OrderBy(p => p.CreatedAt)
    .FirstAsync();

// Get first person or null
var firstOrNull = await client.Fluent().People
    .FirstOrDefaultAsync();

// Get first person matching additional criteria
var firstJohn = await client.Fluent().People
    .FirstAsync(p => p.FirstName == "John");

// Get single person (throws if 0 or >1 found)
var singlePerson = await client.Fluent().People
    .Where(p => p.Email == "unique@example.com")
    .SingleAsync();

// Count all people
var totalCount = await client.Fluent().People
    .CountAsync();

// Check if any people exist
var anyPeople = await client.Fluent().People
    .AnyAsync();

// Check if any people match criteria
var anyJohns = await client.Fluent().People
    .AnyAsync(p => p.FirstName == "John");
```

## Pagination

### Manual Pagination

```csharp
// Get specific page
var page2 = await client.Fluent().People
    .OrderBy(p => p.LastName)
    .GetPageAsync(page: 2, pageSize: 25);

// Navigate through pages
var firstPage = await client.Fluent().People.GetPagedAsync(25);
if (firstPage.HasNextPage)
{
    var nextPage = await firstPage.GetNextPageAsync();
}
```

### Automatic Pagination

```csharp
// Get all data (handles pagination automatically)
var allPeople = await client.Fluent().People
    .Where(p => p.LastName.StartsWith("S"))
    .GetAllAsync();

// Configure pagination behavior
var options = new PaginationOptions
{
    PageSize = 100,
    MaxPages = 10
};

var limitedResults = await client.Fluent().People
    .GetAllAsync(options);
```

### Memory-Efficient Streaming

```csharp
// Stream large datasets
await foreach (var person in client.Fluent().People
    .OrderBy(p => p.CreatedAt)
    .AsAsyncEnumerable())
{
    // Process each person individually
    // Memory usage stays constant regardless of total count
    await ProcessPersonAsync(person);
}

// Stream with pagination options
var streamOptions = new PaginationOptions { PageSize = 50 };
await foreach (var person in client.Fluent().People
    .AsAsyncEnumerable(streamOptions))
{
    await ProcessPersonAsync(person);
}
```

## Fluent Creation

### Simple Creation

```csharp
// Create a person
var newPerson = await client.Fluent().People
    .Create(new PersonCreateRequest
    {
        FirstName = "Jane",
        LastName = "Doe",
        Birthdate = DateTime.Parse("1990-01-01")
    })
    .ExecuteAsync();
```

### Creation with Related Data

```csharp
// Create person with all contact information in one operation
var personWithContacts = await client.Fluent().People
    .Create(new PersonCreateRequest
    {
        FirstName = "John",
        LastName = "Smith",
        Birthdate = DateTime.Parse("1985-05-15")
    })
    .WithEmail(new EmailCreateRequest
    {
        Address = "john.smith@example.com",
        Location = "Home",
        Primary = true
    })
    .WithPhoneNumber(new PhoneNumberCreateRequest
    {
        Number = "555-123-4567",
        Location = "Mobile",
        Primary = true
    })
    .WithAddress(new AddressCreateRequest
    {
        Street = "123 Main St",
        City = "Anytown",
        State = "CA",
        Zip = "12345",
        Location = "Home",
        Primary = true
    })
    .ExecuteAsync();
```

### Multiple Related Items

```csharp
// Add multiple addresses, emails, etc.
var personWithMultipleContacts = await client.Fluent().People
    .Create(new PersonCreateRequest
    {
        FirstName = "Jane",
        LastName = "Doe"
    })
    .WithAddress(new AddressCreateRequest
    {
        Street = "123 Home St",
        City = "Hometown",
        State = "CA",
        Zip = "12345",
        Location = "Home",
        Primary = true
    })
    .WithAddress(new AddressCreateRequest
    {
        Street = "456 Work Ave",
        City = "Worktown",
        State = "CA",
        Zip = "54321",
        Location = "Work",
        Primary = false
    })
    .WithEmail(new EmailCreateRequest
    {
        Address = "jane.personal@example.com",
        Location = "Home",
        Primary = true
    })
    .WithEmail(new EmailCreateRequest
    {
        Address = "jane.work@company.com",
        Location = "Work",
        Primary = false
    })
    .ExecuteAsync();
```

## Error Handling

The fluent API uses the same error handling as the standard API:

```csharp
try
{
    var person = await client.Fluent().People
        .Where(p => p.Email == "notfound@example.com")
        .SingleAsync();
}
catch (InvalidOperationException ex)
{
    // Thrown when SingleAsync finds 0 or >1 results
    Console.WriteLine($"Single operation failed: {ex.Message}");
}
catch (PlanningCenterApiException ex)
{
    // Standard API exceptions
    Console.WriteLine($"API error: {ex.Message}");
}
```

## Performance Considerations

### Memory Usage

```csharp
// ❌ Not recommended for large datasets
var allPeople = await client.Fluent().People.GetAllAsync();
foreach (var person in allPeople)
{
    await ProcessPersonAsync(person);
}

// ✅ Recommended for large datasets
await foreach (var person in client.Fluent().People.AsAsyncEnumerable())
{
    await ProcessPersonAsync(person);
}
```

### Pagination Optimization

```csharp
// Configure optimal page sizes
var options = new PaginationOptions
{
    PageSize = 100, // Larger pages = fewer API calls
    MaxPages = 50   // Prevent runaway queries
};

var results = await client.Fluent().People
    .GetAllAsync(options);
```

## Module Support

### Fully Implemented ✅
- ✅ **People Module**: Complete fluent API with creation contexts
- ✅ **Giving Module**: Complete fluent API with specialized operations
- ✅ **Calendar Module**: Complete fluent API with date-based filtering
- ✅ **Groups Module**: Complete fluent API with membership and type filtering
- ✅ **Services Module**: Complete fluent API with service planning operations
- ✅ **Check-Ins Module**: Complete fluent API with check-in status and filtering
- ✅ **Registrations Module**: Complete fluent API with event registration filtering
- ✅ **Publishing Module**: Complete fluent API with episode and media management
- ✅ **Webhooks Module**: Complete fluent API with subscription monitoring

## Performance Monitoring

The fluent API includes comprehensive performance monitoring capabilities to help optimize your queries and identify potential bottlenecks:

### Query Performance Tracking

All fluent operations are automatically tracked for performance metrics:

```csharp
// All fluent operations are monitored automatically
var people = await client.Fluent().People
    .Where(p => p.FirstName == "John")
    .OrderBy(p => p.LastName)
    .GetPagedAsync(25);
```

### Getting Performance Metrics

```csharp
using PlanningCenter.Api.Client.Fluent.Performance;

// Get metrics for a specific operation
var metrics = FluentPerformanceExtensions.GetPerformanceMetrics("People.GetPaged[25]");
Console.WriteLine($"Average execution time: {metrics?.AverageExecutionTimeMs}ms");
Console.WriteLine($"Total executions: {metrics?.TotalExecutions}");
Console.WriteLine($"Success rate: {metrics?.SuccessRate:P}");

// Get all performance metrics
var allMetrics = FluentPerformanceExtensions.GetAllFluentMetrics();
foreach (var (operationName, metric) in allMetrics)
{
    Console.WriteLine($"{operationName}: {metric.AverageExecutionTimeMs}ms avg");
}
```

### Performance Optimization Recommendations

The system provides automated recommendations for query optimization:

```csharp
// Get optimization recommendations
var recommendations = FluentPerformanceExtensions.GetFluentOptimizationRecommendations();

foreach (var recommendation in recommendations)
{
    Console.WriteLine($"Query: {recommendation.QueryName}");
    Console.WriteLine($"Priority: {recommendation.Priority}");
    
    foreach (var issue in recommendation.Issues)
    {
        Console.WriteLine($"  Issue: {issue}");
    }
    
    foreach (var rec in recommendation.Recommendations)
    {
        Console.WriteLine($"  Recommendation: {rec}");
    }
}
```

### Performance Reports

Generate comprehensive performance reports:

```csharp
// Generate a full performance report
var report = FluentPerformanceExtensions.GenerateFluentPerformanceReport();

Console.WriteLine($"Generated: {report.GeneratedAt}");
Console.WriteLine($"Total queries: {report.TotalQueries}");
Console.WriteLine($"Total executions: {report.TotalExecutions}");
Console.WriteLine($"Overall success rate: {report.OverallSuccessRate:P}");
Console.WriteLine($"Average execution time: {report.AverageExecutionTime}ms");

// Show slowest queries
Console.WriteLine("\nSlowest queries:");
foreach (var query in report.SlowestQueries.Take(5))
{
    Console.WriteLine($"  {query.QueryName}: {query.AverageExecutionTimeMs}ms");
}

// Show most frequent queries
Console.WriteLine("\nMost frequent queries:");
foreach (var query in report.MostFrequentQueries.Take(5))
{
    Console.WriteLine($"  {query.QueryName}: {query.TotalExecutions} executions");
}
```

### Performance Thresholds

The monitoring system automatically identifies performance issues based on configurable thresholds:

- **Slow queries**: Average execution time > 2 seconds (Medium priority) or > 5 seconds (High priority)
- **High failure rates**: Error rate > 10% (High priority)
- **Execution variance**: Max time > 5x Min time (Medium priority)
- **Frequently executed slow queries**: > 1000 executions with > 1 second average (Medium priority)

### Best Practices for Performance

1. **Monitor regularly**: Check performance metrics periodically
2. **Optimize slow queries**: Address high-priority recommendations first
3. **Use appropriate page sizes**: Balance between API calls and memory usage
4. **Consider caching**: For frequently accessed data
5. **Use streaming**: For large datasets to reduce memory pressure

### Clearing Performance Data

```csharp
// Clear all performance metrics (useful for testing)
FluentPerformanceExtensions.ClearFluentPerformanceMetrics();
```

## Expression Parsing (Future Enhancement)

Currently, the fluent API accepts LINQ expressions but doesn't fully parse them into API filters. This is planned for future enhancement:

```csharp
// Current: Syntax is accepted but filtering happens client-side
var people = await client.Fluent().People
    .Where(p => p.FirstName == "John")  // Expression accepted
    .GetPagedAsync();                   // But filtering not yet server-side

// Future: Full server-side filtering
var people = await client.Fluent().People
    .Where(p => p.FirstName == "John")           // → filter[first_name]=John
    .Where(p => p.CreatedAt > DateTime.Now.AddDays(-30))  // → filter[created_at]=>2024-01-01
    .OrderBy(p => p.LastName)                    // → sort=last_name
    .GetPagedAsync();
```

## Integration with Standard API

The fluent API is built on top of the standard API and doesn't replace it:

```csharp
// Standard API - still available
var person = await client.People.GetAsync("123");

// Fluent API - new option
var person = await client.Fluent().People.GetAsync("123");

// Both use the same underlying service
// Choose based on your preference and use case
```

## Best Practices

1. **Use streaming for large datasets** to maintain constant memory usage
2. **Configure pagination options** to optimize performance
3. **Combine fluent creation** when creating entities with related data
4. **Use LINQ-like operations** for familiar .NET patterns
5. **Handle exceptions** appropriately for your use case
6. **Choose the right method** based on your data size and processing needs

## Module-Specific Examples

### Giving Module

```csharp
// Find donations by person for the last 3 months
var personDonations = await client.Fluent().Giving
    .ByPerson("12345")
    .ByDateRange(DateTime.Now.AddMonths(-3), DateTime.Now)
    .OrderByDescending(d => d.ReceivedAt)
    .GetPagedAsync(25);

// Calculate total donations to a specific fund
var totalAmount = await client.Fluent().Giving
    .ByFund("general-fund")
    .ByDateRange(DateTime.Now.AddYears(-1), DateTime.Now)
    .TotalAmountAsync();

// Find large donations (over $100)
var largeDonations = await client.Fluent().Giving
    .WithMinimumAmount(10000) // $100.00 in cents
    .GetAllAsync();
```

### Calendar Module

```csharp
// Get today's events
var todayEvents = await client.Fluent().Calendar
    .Today()
    .OrderBy(e => e.StartsAt)
    .GetAllAsync();

// Find upcoming events this month
var upcomingEvents = await client.Fluent().Calendar
    .Upcoming()
    .ThisMonth()
    .OrderBy(e => e.StartsAt)
    .GetPagedAsync(50);

// Get events in a specific date range
var weekendEvents = await client.Fluent().Calendar
    .ByDateRange(DateTime.Today.AddDays(5), DateTime.Today.AddDays(7))
    .OrderBy(e => e.StartsAt)
    .GetAllAsync();
```

### Groups Module

```csharp
// Find active groups with minimum members
var activeGroups = await client.Fluent().Groups
    .Active()
    .WithMinimumMembers(5)
    .OrderBy(g => g.Name)
    .GetPagedAsync(25);

// Search for youth groups with chat enabled
var youthGroups = await client.Fluent().Groups
    .ByNameContains("Youth")
    .WithChatEnabled()
    .Active()
    .GetAllAsync();

// Find groups by type and location
var smallGroups = await client.Fluent().Groups
    .ByGroupType("small-group")
    .ByLocation("main-campus")
    .WithMinimumMembers(3)
    .WithMaximumMembers(12)
    .GetAllAsync();
```

### Services Module

```csharp
// Get upcoming service plans
var upcomingServices = await client.Fluent().Services
    .Upcoming()
    .OrderBy(p => p.SortDate)
    .GetPagedAsync(25);

// Find Sunday services this month
var sundayServices = await client.Fluent().Services
    .ByServiceType("sunday-morning")
    .ThisMonth()
    .Public()
    .GetAllAsync();

// Search for Christmas services
var christmasServices = await client.Fluent().Services
    .ByTitleContains("Christmas")
    .ByDateRange(DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1))
    .GetAllAsync();

// Find longer services (90+ minutes)
var longServices = await client.Fluent().Services
    .WithMinimumLength(90)
    .Upcoming()
    .GetAllAsync();
```

### Check-Ins Module

```csharp
// Get today's check-ins
var todayCheckIns = await client.Fluent().CheckIns
    .Today()
    .OrderBy(c => c.CreatedAt)
    .GetPagedAsync(25);

// Find check-ins for a specific person
var personCheckIns = await client.Fluent().CheckIns
    .ByPerson("12345")
    .OrderByDescending(c => c.CreatedAt)
    .GetPagedAsync(10);

// Get currently checked in people for an event
var currentlyCheckedIn = await client.Fluent().CheckIns
    .ByEvent("event-123")
    .CheckedIn()
    .OrderBy(c => c.FirstName)
    .GetAllAsync();

// Find guests vs members today
var guestsToday = await client.Fluent().CheckIns
    .Today()
    .Guests()
    .CountAsync();

var membersToday = await client.Fluent().CheckIns
    .Today()
    .Members()
    .CountAsync();

// Get check-ins with medical notes this week
var medicalCheckIns = await client.Fluent().CheckIns
    .WithMedicalNotes()
    .ThisWeek()
    .GetAllAsync();

// Find check-ins by location and kind
var volunteerCheckIns = await client.Fluent().CheckIns
    .ByLocation("main-room")
    .ByKind("volunteer")
    .Confirmed()
    .GetAllAsync();
```

## Specialized Operations

Each module provides specialized operations that make common tasks easier:

### Giving Specialized Methods
- `ByFund(fundId)` - Filter by fund
- `ByPerson(personId)` - Filter by donor
- `ByDateRange(start, end)` - Filter by date range
- `WithMinimumAmount(amount)` - Filter by minimum amount
- `TotalAmountAsync()` - Calculate total donation amount

### Calendar Specialized Methods
- `Today()` - Events occurring today
- `ThisWeek()` - Events this week
- `ThisMonth()` - Events this month
- `Upcoming()` - Future events
- `ByDateRange(start, end)` - Events in date range

### Groups Specialized Methods
- `Active()` / `Archived()` - Filter by status
- `ByGroupType(typeId)` - Filter by group type
- `ByLocation(locationId)` - Filter by location
- `WithMinimumMembers(count)` - Filter by member count
- `WithChatEnabled()` - Groups with chat
- `WithVirtualMeeting()` - Groups with virtual capabilities
- `ByNameContains(text)` - Search by name

### Services Specialized Methods
- `Upcoming()` / `Past()` - Filter by time
- `ThisWeek()` / `ThisMonth()` - Filter by period
- `ByServiceType(typeId)` - Filter by service type
- `Public()` / `Private()` - Filter by visibility
- `WithMinimumLength(minutes)` - Filter by duration
- `ByTitleContains(text)` - Search by title

### Check-Ins Specialized Methods
- `ByPerson(personId)` - Filter by person
- `ByEvent(eventId)` - Filter by event
- `ByEventTime(eventTimeId)` - Filter by event time
- `ByLocation(locationId)` - Filter by location
- `Today()` / `ThisWeek()` - Filter by time period
- `ByDateRange(start, end)` - Filter by date range
- `CheckedIn()` / `CheckedOut()` - Filter by check-in status
- `Confirmed()` / `Unconfirmed()` - Filter by confirmation status
- `Guests()` / `Members()` - Filter by guest status
- `WithMedicalNotes()` - Filter by medical notes presence
- `ByNameContains(text)` - Search by name
- `ByKind(kind)` - Filter by check-in kind/type

### Publishing Module

```csharp
// Get recent published episodes
var recentEpisodes = await client.Fluent().Publishing
    .Published()
    .OrderByDescending(e => e.PublishedAt)
    .GetPagedAsync(25);

// Find episodes by title
var christmasEpisodes = await client.Fluent().Publishing
    .ByTitleContains("Christmas")
    .Published()
    .OrderByDescending(e => e.PublishedAt)
    .GetAllAsync();

// Get episodes from a specific speaker
var speakerEpisodes = await client.Fluent().Publishing
    .BySpeaker("john-doe")
    .Published()
    .OrderByDescending(e => e.PublishedAt)
    .GetAllAsync();

// Calculate total views for published episodes
var totalViews = await client.Fluent().Publishing
    .Published()
    .TotalViewsAsync();

// Find popular episodes (high view count)
var popularEpisodes = await client.Fluent().Publishing
    .WithMinimumViews(1000)
    .Published()
    .OrderByDescending(e => e.ViewCount)
    .GetPagedAsync(10);
```

### Webhooks Module

```csharp
// Get all active webhook subscriptions
var activeWebhooks = await client.Fluent().Webhooks
    .Active()
    .OrderBy(w => w.Url)
    .GetAllAsync();

// Find webhooks with poor success rates
var problematicWebhooks = await client.Fluent().Webhooks
    .WithPoorSuccessRate(80.0) // Below 80% success rate
    .Active()
    .OrderBy(w => w.SuccessRate)
    .GetAllAsync();

// Get webhooks that are responding slowly
var slowWebhooks = await client.Fluent().Webhooks
    .SlowResponding(5000) // Over 5 seconds
    .Active()
    .GetAllAsync();

// Find webhooks by URL pattern
var apiWebhooks = await client.Fluent().Webhooks
    .ByUrlContains("api.example.com")
    .Active()
    .GetAllAsync();

// Calculate overall webhook success rate
var overallSuccessRate = await client.Fluent().Webhooks
    .Active()
    .OverallSuccessRateAsync();

// Get webhook delivery statistics
var totalDeliveries = await client.Fluent().Webhooks
    .Active()
    .TotalDeliveriesAsync();

var successfulDeliveries = await client.Fluent().Webhooks
    .Active()
    .TotalSuccessfulDeliveriesAsync();
```

### Registrations Module

```csharp
// Get recent registrations
var recentRegistrations = await client.Fluent().Registrations
    .OrderByDescending(r => r.CreatedAt)
    .GetPagedAsync(25);

// Find registrations by person
var personRegistrations = await client.Fluent().Registrations
    .ByPerson("12345")
    .OrderByDescending(r => r.CreatedAt)
    .GetAllAsync();

// Get registrations for a specific event
var eventRegistrations = await client.Fluent().Registrations
    .ByEvent("event-123")
    .OrderBy(r => r.CreatedAt)
    .GetAllAsync();

// Find registrations by campus
var campusRegistrations = await client.Fluent().Registrations
    .ByCampus("main-campus")
    .OrderByDescending(r => r.CreatedAt)
    .GetAllAsync();
```

### Specialized Operations Summary

Each module provides specialized operations for common use cases:

#### Publishing Specialized Methods
- `Published()` / `Draft()` - Filter by publication status
- `ByTitleContains(text)` - Search by episode title
- `BySpeaker(speakerId)` - Filter by speaker
- `WithMinimumViews(count)` - Filter by view count
- `TotalViewsAsync()` - Calculate total views across episodes

#### Webhooks Specialized Methods
- `Active()` / `Inactive()` - Filter by subscription status
- `ByUrlContains(fragment)` - Filter by URL pattern
- `WithSuccessfulLastDelivery()` - Filter by delivery success
- `WithPoorSuccessRate(threshold)` - Filter by success rate
- `SlowResponding(thresholdMs)` - Filter by response time
- `WithMinimumDeliveries(count)` - Filter by delivery volume
- `OverallSuccessRateAsync()` - Calculate success rate across subscriptions
- `TotalDeliveriesAsync()` - Calculate total deliveries

#### Registrations Specialized Methods
- `ByPerson(personId)` - Filter by registered person
- `ByEvent(eventId)` - Filter by event
- `ByCampus(campusId)` - Filter by campus
- `ByCategory(categoryId)` - Filter by registration category

## Examples Repository

See the `examples/PlanningCenter.Api.Client.Fluent.Console` project for comprehensive examples of fluent API usage across all implemented modules.