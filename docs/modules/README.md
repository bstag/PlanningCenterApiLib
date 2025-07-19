# Planning Center SDK - Module Documentation

This directory contains comprehensive documentation for all Planning Center SDK modules, including both traditional service-based API and fluent API usage patterns.

## Available Modules

| Module | Status | Description |
|--------|--------|-------------|
| [People](PEOPLE_MODULE.md) | ✅ Complete | Manage people, households, contact information, workflows, and lists |
| [Calendar](CALENDAR_MODULE.md) | ✅ Complete | Manage events, resources, and calendar scheduling |
| [Giving](GIVING_MODULE.md) | ✅ Complete | Handle donations, pledges, recurring gifts, and financial data |
| [Groups](GROUPS_MODULE.md) | ✅ Complete | Manage groups, memberships, and group types |
| [Check-Ins](CHECKINS_MODULE.md) | ✅ Complete | Handle event check-ins, attendance, and location management |
| [Services](SERVICES_MODULE.md) | ✅ Complete | Manage service plans, items, songs, and service types |
| [Registrations](REGISTRATIONS_MODULE.md) | ✅ Complete | Handle event registrations, attendees, and signup management |
| [Publishing](PUBLISHING_MODULE.md) | ✅ Complete | Manage media, series, episodes, and speakers |
| [Webhooks](WEBHOOKS_MODULE.md) | ✅ Complete | Configure webhook subscriptions and event handling |

## Documentation Structure

Each module documentation includes:

### 📋 **Service Overview**
- Module purpose and capabilities
- Key entities and relationships
- Authentication requirements

### 🔧 **Traditional API Usage**
- Service interface methods
- CRUD operations
- Pagination patterns
- Error handling

### 🚀 **Fluent API Usage**
- LINQ-like query syntax
- Method chaining patterns
- Advanced filtering and sorting
- Memory-efficient streaming

### 💡 **Common Use Cases**
- Real-world scenarios
- Best practices
- Performance considerations
- Code examples

### 📊 **Advanced Features**
- Batch operations
- Performance monitoring
- Caching strategies
- Custom parameters

## Quick Start

### Traditional Service API
```csharp
// Dependency injection setup
services.AddPlanningCenterApiClientWithPAT("your-app-id:your-secret");

// Usage
public class MyService
{
    private readonly IPeopleService _peopleService;
    
    public MyService(IPeopleService peopleService)
    {
        _peopleService = peopleService;
    }
    
    public async Task<Person?> GetPersonAsync(string id)
    {
        return await _peopleService.GetAsync(id);
    }
}
```

### Fluent API
```csharp
// Dependency injection setup
services.AddPlanningCenterApiClientWithPAT("your-app-id:your-secret");

// Usage
public class MyService
{
    private readonly IPlanningCenterClient _client;
    
    public MyService(IPlanningCenterClient client)
    {
        _client = client;
    }
    
    public async Task<IReadOnlyList<Person>> GetActiveAdultsAsync()
    {
        return await _client.People()
            .Where(p => p.Status == "active")
            .Where(p => p.Birthdate < DateTime.Now.AddYears(-18))
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .GetAllAsync();
    }
}
```

## Common Patterns

### Pagination
All modules support consistent pagination patterns:

```csharp
// Manual pagination
var page = await service.ListAsync(new QueryParameters { PerPage = 25 });

// Automatic pagination (loads all data)
var allItems = await service.GetAllAsync();

// Memory-efficient streaming
await foreach (var item in service.StreamAsync())
{
    // Process one item at a time
}

// Fluent pagination
var page = await client.Module().GetPagedAsync(pageSize: 25);
var allItems = await client.Module().GetAllAsync();
await foreach (var item in client.Module().AsAsyncEnumerable())
{
    // Process one item at a time
}
```

### Error Handling
All modules use consistent exception types:

```csharp
try
{
    var person = await peopleService.GetAsync("123");
}
catch (PlanningCenterApiNotFoundException ex)
{
    // Handle not found
}
catch (PlanningCenterApiValidationException ex)
{
    // Handle validation errors
}
catch (PlanningCenterApiAuthenticationException ex)
{
    // Handle authentication issues
}
catch (PlanningCenterApiException ex)
{
    // Handle general API errors
}
```

### Performance Monitoring
All operations include built-in performance tracking:

```csharp
// Automatic correlation tracking and performance monitoring
var result = await client.People()
    .Where(p => p.Status == "active")
    .ExecuteWithDebugInfoAsync();

Console.WriteLine($"Query took {result.ExecutionTime.TotalMilliseconds}ms");
Console.WriteLine($"Optimization score: {result.OptimizationInfo.Score}");
```

## Best Practices

1. **Use Fluent API for Complex Queries**: The fluent API provides better readability and type safety for complex filtering and sorting operations.

2. **Prefer Streaming for Large Datasets**: Use `StreamAsync()` or `AsAsyncEnumerable()` when processing large amounts of data to minimize memory usage.

3. **Implement Proper Error Handling**: Always handle specific exception types to provide better user experience.

4. **Use Correlation IDs**: The SDK automatically generates correlation IDs for tracking requests across service boundaries.

5. **Monitor Performance**: Use the built-in performance monitoring to identify slow operations and optimize queries.

6. **Cache When Appropriate**: Use the built-in caching for frequently accessed, relatively static data.

## Support and Resources

- [API Reference](../API_REFERENCE.md) - Complete API documentation
- [Fluent API Guide](../FLUENT_API.md) - Detailed fluent API documentation
- [Examples Repository](../../examples/) - Working code examples
- [GitHub Issues](https://github.com/planningcenter/planning-center-sdk/issues) - Report bugs and request features