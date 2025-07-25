# Planning Center API SDK for .NET

A comprehensive, production-ready .NET SDK for the [Planning Center API](https://developer.planning.center/docs/#/overview/). Built with modern .NET practices, this SDK provides a clean, intuitive interface for interacting with all of Planning Center's services.

## Features

- **Complete API Coverage**: Full implementation of all 9 Planning Center modules
- **Multiple Authentication Methods**: Personal Access Tokens (PAT), OAuth 2.0, and Access Tokens
- **Automatic Pagination**: Built-in pagination helpers eliminate manual pagination logic
- **Memory-Efficient Streaming**: Stream large datasets without loading everything into memory
- **Fluent API**: LINQ-like interface for intuitive query building
- **Comprehensive Error Handling**: Detailed exceptions with proper error context
- **Built-in Caching**: Configurable response caching for improved performance
- **Dependency Injection**: Full support for .NET dependency injection
- **Async/Await**: Modern async patterns throughout
- **Strongly Typed**: Rich type system with comprehensive models
- **Production Ready**: Logging, monitoring, and configuration support

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

## CLI Tool

🚀 **NEW: Command-Line Interface** - A powerful CLI tool for interacting with Planning Center from the terminal!

```bash
# Navigate to the CLI
cd examples/PlanningCenter.Api.Client.CLI

# Set your token and start using
dotnet run -- config set-token "your-app-id:your-secret"
dotnet run -- people list
dotnet run -- services list-plans
dotnet run -- calendar list-events
dotnet run -- giving list-donations
dotnet run -- groups list
dotnet run -- publishing list-episodes
dotnet run -- webhooks list-subscriptions
```

**Features:**
- **9 Complete Modules**: All Planning Center modules (People, Services, Registrations, Calendar, Check-Ins, Giving, Groups, Publishing, Webhooks)
- **Multiple Output Formats**: JSON (default), CSV, XML, Table
- **Advanced Filtering**: Complex queries with sorting and pagination
- **Secure Authentication**: Encrypted token storage
- **Export Capabilities**: Save results to files
- **Production Ready**: Comprehensive error handling and logging

See the [CLI Documentation](examples/PlanningCenter.Api.Client.CLI/README.md) for complete usage guide.

## Documentation

- **[Fluent API Guide](docs/FLUENT_API.md)** - Complete guide to the LINQ-like fluent interface
- **[Authentication Guide](planning-center-sdk-plan/architecture/AUTHENTICATION.md)** - Complete guide to all authentication methods
- **[CLI Tool Guide](examples/PlanningCenter.Api.Client.CLI/README.md)** - Complete command-line interface documentation
- **[Examples](examples/)** - Working examples for different scenarios
- **[API Reference](planning-center-sdk-plan/api-reference/)** - Detailed API documentation
- **[Architecture](planning-center-sdk-plan/architecture/)** - SDK architecture and design decisions

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