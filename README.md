# Planning Center API SDK for .NET

A comprehensive, production-ready .NET SDK for the [Planning Center API](https://developer.planning.center/docs/#/overview/). Built with modern .NET practices, this SDK provides a clean, intuitive interface for interacting with all of Planning Center's services.

## Features

- **Complete API Coverage**: Full implementation of all 9 Planning Center modules
- **ServiceBase Architecture**: Unified service pattern with correlation ID management and performance monitoring
- **Multiple Authentication Methods**: Personal Access Tokens (PAT), OAuth 2.0, and Access Tokens
- **Automatic Pagination**: Built-in pagination helpers eliminate manual pagination logic
- **Memory-Efficient Streaming**: Stream large datasets without loading everything into memory
- **Fluent API**: LINQ-like interface for intuitive query building across all modules
- **Comprehensive Error Handling**: Detailed exceptions with proper error context and correlation tracking
- **Built-in Caching**: Configurable response caching for improved performance
- **Dependency Injection**: Full support for .NET dependency injection
- **Async/Await**: Modern async patterns throughout
- **Strongly Typed**: Rich type system with comprehensive models
- **Production Ready**: Logging, monitoring, correlation tracking, and configuration support
- **CLI Tool**: Complete command-line interface for all 9 modules with advanced features

## Quick Start

### Personal Access Token (Recommended for Server Apps)

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlanningCenter.Api.Client;
using PlanningCenter.Api.Client.Models;

var builder = Host.CreateApplicationBuilder(args);

// Add Planning Center API client with Personal Access Token
builder.Services.AddPlanningCenterApiClientWithPAT("your-app-id:your-secret");

var host = builder.Build();

// Use any of the 9 available services
var peopleService = host.Services.GetRequiredService<IPeopleService>();
var givingService = host.Services.GetRequiredService<IGivingService>();
var calendarService = host.Services.GetRequiredService<ICalendarService>();

// List people with automatic pagination
var people = await peopleService.ListAsync();
foreach (var person in people.Data)
{
    Console.WriteLine($"{person.FullName} - {person.Status}");
}

// Use the fluent API for LINQ-like queries
var client = host.Services.GetRequiredService<IPlanningCenterClient>();
var johnSmiths = await client.Fluent().People
    .Where(p => p.FirstName == "John")
    .Where(p => p.LastName.Contains("Smith"))
    .GetAllAsync();
```

### OAuth 2.0 (For User-Facing Apps)

```csharp
builder.Services.AddPlanningCenterApiClient(options =>
{
    options.ClientId = "your-oauth-client-id";
    options.ClientSecret = "your-oauth-client-secret";
    options.AccessToken = "user-access-token";
    options.RefreshToken = "user-refresh-token";
});
```

## Authentication Methods

The SDK supports three authentication methods:

1. **Personal Access Token (PAT)** - Recommended for server-side applications
2. **OAuth 2.0** - For user-facing applications requiring user consent
3. **Access Token** - For simple token-based authentication

See the [Authentication Guide](docs/AUTHENTICATION.md) for detailed information on each method.

## ServiceBase Architecture

All services in the SDK are built on a unified **ServiceBase** pattern that provides enterprise-grade reliability and observability:

### Core Features
- **Correlation ID Management**: Every request gets a unique correlation ID for end-to-end tracking
- **Performance Monitoring**: Built-in timing and performance metrics for all operations
- **Unified Exception Handling**: Consistent error handling with detailed context and correlation tracking
- **Automatic Retry Logic**: Configurable retry policies for transient failures with exponential backoff
- **Request/Response Logging**: Structured logging with correlation IDs for debugging and monitoring
- **Rate Limit Handling**: Automatic rate limit detection and intelligent backoff strategies
- **Circuit Breaker Pattern**: Prevents cascading failures during API outages
- **Request Deduplication**: Prevents duplicate requests within configurable time windows

### Observability & Monitoring
```csharp
// All services inherit from ServiceBase and provide consistent behavior
var peopleService = host.Services.GetRequiredService<IPeopleService>();

// Every operation includes comprehensive tracking
var people = await peopleService.ListAsync();
// Logs: [CorrelationId: abc123] PeopleService.ListAsync started
// Logs: [CorrelationId: abc123] HTTP GET /people/v2/people completed in 245ms (Status: 200)
// Logs: [CorrelationId: abc123] PeopleService.ListAsync completed successfully (Duration: 267ms)

// Performance metrics are automatically collected
// - Request duration
// - Success/failure rates
// - Rate limit status
// - Cache hit/miss ratios
```

### Error Handling & Context
```csharp
// Errors include full context and correlation tracking
try 
{
    var person = await peopleService.GetAsync("invalid-id");
}
catch (PlanningCenterApiNotFoundException ex)
{
    // Rich exception context
    // ex.CorrelationId = "abc123"
    // ex.RequestId = "req-xyz789"
    // ex.StatusCode = 404
    // ex.ErrorDetails contains full API response
    // ex.RequestUrl = "https://api.planningcenteronline.com/people/v2/people/invalid-id"
    // ex.RequestDuration = TimeSpan.FromMilliseconds(156)
    
    _logger.LogWarning("Person not found: {PersonId} [CorrelationId: {CorrelationId}]", 
        "invalid-id", ex.CorrelationId);
}
catch (PlanningCenterApiRateLimitException ex)
{
    // Automatic retry will be attempted based on configuration
    _logger.LogInformation("Rate limit hit, retrying in {RetryAfter}s [CorrelationId: {CorrelationId}]", 
        ex.RetryAfter.TotalSeconds, ex.CorrelationId);
}
```

### Configuration & Customization
```csharp
// Configure ServiceBase behavior
builder.Services.Configure<PlanningCenterOptions>(options =>
{
    // Performance monitoring
    options.EnablePerformanceLogging = true;
    options.SlowRequestThreshold = TimeSpan.FromSeconds(2);
    
    // Retry configuration
    options.MaxRetryAttempts = 3;
    options.RetryBaseDelay = TimeSpan.FromSeconds(1);
    options.RetryMaxDelay = TimeSpan.FromSeconds(30);
    
    // Circuit breaker
    options.CircuitBreakerFailureThreshold = 5;
    options.CircuitBreakerRecoveryTime = TimeSpan.FromMinutes(1);
    
    // Request deduplication
    options.EnableRequestDeduplication = true;
    options.DeduplicationWindow = TimeSpan.FromSeconds(10);
});
```

The ServiceBase architecture ensures that all 9 Planning Center modules provide consistent, reliable, and observable behavior. See the [Performance Guide](docs/PERFORMANCE.md) for optimization strategies and the [Best Practices](docs/BEST_PRACTICES.md) for recommended usage patterns.

## CLI Tool

🚀 **NEW: Command-Line Interface** - A powerful CLI tool for interacting with Planning Center from the terminal!

```bash
# Navigate to the CLI
cd examples/PlanningCenter.Api.Client.CLI

# Set your token and start using
dotnet run -- config set-token "your-app-id:your-secret"

# People module
dotnet run -- people list --format table --limit 10
dotnet run -- people get 12345 --include emails,phone-numbers
dotnet run -- people search --name "John Smith" --status active

# Services module
dotnet run -- services list-plans --service-type-id 1 --future
dotnet run -- services list-songs --arrangement "Key of C"

# Calendar module
dotnet run -- calendar list-events --start-date 2024-01-01 --end-date 2024-12-31

# Giving module
dotnet run -- giving list-donations --fund-id 1 --date-range "2024-01-01,2024-12-31"

# Advanced usage with filtering and export
dotnet run -- people list --where "status=active" --sort "last_name" --format csv --output people.csv
dotnet run -- services list-plans --include "plan_times,contributors" --format json --output plans.json
```

**Features:**
- **9 Complete Modules**: All Planning Center modules (People, Services, Registrations, Calendar, Check-Ins, Giving, Groups, Publishing, Webhooks)
- **Multiple Output Formats**: JSON (default), CSV, XML, Table with customizable columns
- **Advanced Filtering**: Complex queries with WHERE clauses, sorting, and pagination
- **Secure Authentication**: Encrypted token storage with multiple authentication methods
- **Export Capabilities**: Save results to files with various formats
- **Include Related Data**: Fetch related resources in single requests
- **Batch Operations**: Process multiple items efficiently
- **Production Ready**: Comprehensive error handling, logging, and retry logic
- **Interactive Mode**: Step-by-step guided operations
- **Configuration Management**: Multiple environment support

**Quick Examples:**
```bash
# Get person with all related data
dotnet run -- people get 12345 --include "emails,phone-numbers,addresses,field-data"

# Export active people to CSV
dotnet run -- people list --where "status=active" --format csv --output active-people.csv

# List upcoming service plans
dotnet run -- services list-plans --future --include "plan-times,contributors" --format table

# Search for events in date range
dotnet run -- calendar list-events --start-date 2024-06-01 --end-date 2024-06-30 --format json
```

See the [CLI Documentation](examples/PlanningCenter.Api.Client.CLI/README.md) for complete usage guide and advanced scenarios.

## Documentation

### Getting Started
- **[Getting Started Guide](docs/GETTING_STARTED.md)** - Step-by-step guide to get up and running quickly
- **[Authentication Guide](docs/AUTHENTICATION.md)** - Complete guide to all authentication methods
- **[Best Practices](docs/BEST_PRACTICES.md)** - SDK usage best practices and patterns
- **[Troubleshooting](docs/TROUBLESHOOTING.md)** - Common issues and solutions
- **[Migration Guide](docs/MIGRATION_GUIDE.md)** - Upgrade from older versions or other SDKs

### API Documentation
- **[Fluent API Guide](docs/FLUENT_API.md)** - Complete guide to the LINQ-like fluent interface
- **[API Reference](planning-center-sdk-plan/api-reference/)** - Detailed API documentation
- **[ServiceBase Architecture](planning-center-sdk-plan/architecture/)** - SDK architecture and design decisions

### Advanced Topics
- **[Performance Guide](docs/PERFORMANCE.md)** - Optimization strategies and best practices
- **[Testing Guide](docs/TESTING.md)** - Unit testing, integration testing, and mocking
- **[Issues & Status](docs/ISSUES.md)** - Known issues and current development status

### Tools and Examples
- **[CLI Tool Guide](examples/PlanningCenter.Api.Client.CLI/README.md)** - Complete command-line interface documentation
- **[Examples](examples/)** - Working examples for different scenarios
- **[Console Examples](examples/PlanningCenter.Api.Client.Console/README.md)** - Basic API usage examples
- **[Fluent Examples](examples/PlanningCenter.Api.Client.Fluent.Console/README.md)** - Fluent API examples
- **[Worker Service Examples](examples/PlanningCenter.Api.Client.Worker/README.md)** - Background service patterns

## Examples

See the `examples/` directory for complete working examples:

- **`PlanningCenter.Api.Client.CLI/`** - 🆕 **Command-line interface with all 9 Planning Center modules**
- `PlanningCenter.Api.Client.Console/` - General usage with standard API
- `PlanningCenter.Api.Client.Fluent.Console/` - Fluent API usage examples
- `PlanningCenter.Api.Client.Worker/` - Background service implementation

### Automatic Pagination

```csharp
// Get all people automatically (handles pagination behind the scenes)
var allPeople = await peopleService.GetAllAsync();
Console.WriteLine($"Total people: {allPeople.Count}");

// Or stream for memory efficiency
await foreach (var person in peopleService.StreamAsync())
{
    Console.WriteLine($"Processing: {person.FullName}");
}
```

### Manual Pagination with Rich Metadata

```csharp
var parameters = new QueryParameters
{
    Where = new Dictionary<string, object> { ["status"] = "active" },
    PerPage = 25
};

var page = await peopleService.ListAsync(parameters);

Console.WriteLine($"Page {page.Meta.CurrentPage} of {page.Meta.TotalPages}");
Console.WriteLine($"Showing {page.Data.Count} of {page.Meta.TotalCount} total");

// Navigate to next page
if (page.HasNextPage)
{
    var nextPage = await page.GetNextPageAsync();
}
```

### Multiple Service Examples

```csharp
// People management
var person = await peopleService.CreateAsync(new PersonCreateRequest
{
    FirstName = "John",
    LastName = "Smith",
    Birthdate = DateTime.Parse("1990-01-01")
});

// Giving management
var donations = await givingService.ListDonationsAsync();
var totalGiving = await givingService.GetTotalGivingAsync(
    DateTime.Now.AddYears(-1), DateTime.Now);

// Calendar management
var events = await calendarService.ListEventsAsync();
var newEvent = await calendarService.CreateEventAsync(new EventCreateRequest
{
    Name = "Sunday Service",
    StartsAt = DateTime.Now.AddDays(7),
    EndsAt = DateTime.Now.AddDays(7).AddHours(2)
});

// Webhook management
var webhooks = await webhooksService.ListSubscriptionsAsync();
var newWebhook = await webhooksService.CreateSubscriptionAsync(new WebhookSubscriptionCreateRequest
{
    Url = "https://your-app.com/webhook",
    EventTypes = new[] { "person.created", "person.updated" }
});
```

## Configuration

```csharp
builder.Services.Configure<PlanningCenterOptions>(options =>
{
    // API Configuration
    options.BaseUrl = "https://api.planningcenteronline.com";
    options.RequestTimeout = TimeSpan.FromSeconds(30);
    
    // Retry Configuration
    options.MaxRetryAttempts = 3;
    options.RetryBaseDelay = TimeSpan.FromSeconds(1);
    
    // Caching Configuration
    options.EnableCaching = true;
    options.DefaultCacheExpiration = TimeSpan.FromMinutes(5);
    
    // Logging (be careful in production)
    options.EnableDetailedLogging = false;
});
```

## Supported Modules

| Module | Status | Description |
|--------|--------|-------------|
| **People** | ✅ Complete | Manage people, households, contact information, workflows, and forms |
| **Giving** | ✅ Complete | Donations, pledges, funds, batches, and payment management |
| **Calendar** | ✅ Complete | Calendar events, resources, and scheduling |
| **Check-Ins** | ✅ Complete | Event check-ins, locations, and attendance tracking |
| **Groups** | ✅ Complete | Small groups, memberships, and group management |
| **Registrations** | ✅ Complete | Event registrations, attendees, and signups |
| **Services** | ✅ Complete | Worship services, plans, items, and scheduling |
| **Publishing** | ✅ Complete | Media content, episodes, series, and speakers |
| **Webhooks** | ✅ Complete | Webhook subscriptions, events, and delivery management |

All modules support:
- **Full CRUD operations** with comprehensive validation
- **Automatic pagination** with helpers for large datasets
- **Memory-efficient streaming** for processing large amounts of data
- **Fluent API** for LINQ-like querying (People module complete, others in progress)
- **Robust error handling** with detailed exception types
- **Comprehensive logging** with structured data

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.