# Planning Center API SDK for .NET

A comprehensive, production-ready .NET SDK for the [Planning Center API](https://developer.planning.center/docs/#/overview/). Built with modern .NET practices, this SDK provides a clean, intuitive interface for interacting with Planning Center's services.

## Features

- **Multiple Authentication Methods**: Personal Access Tokens (PAT), OAuth 2.0, and Access Tokens
- **Automatic Pagination**: Built-in pagination helpers eliminate manual pagination logic
- **Memory-Efficient Streaming**: Stream large datasets without loading everything into memory
- **Comprehensive Error Handling**: Detailed exceptions with proper error context
- **Built-in Caching**: Configurable response caching for improved performance
- **Retry Logic**: Automatic retry with exponential backoff for transient failures
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

var builder = Host.CreateApplicationBuilder(args);

// Add Planning Center API client with Personal Access Token
builder.Services.AddPlanningCenterApiClientWithPAT("your-app-id:your-secret");

var host = builder.Build();

// Use the People service
var peopleService = host.Services.GetRequiredService<IPeopleService>();
var people = await peopleService.ListAsync();

foreach (var person in people.Data)
{
    Console.WriteLine($"{person.FullName} - {person.Status}");
}
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

## Documentation

- **[Authentication Guide](docs/AUTHENTICATION.md)** - Complete guide to all authentication methods
- **[Examples](examples/)** - Working examples for different scenarios
- **[API Reference](planning-center-sdk-v2/api-reference/)** - Detailed API documentation
- **[Architecture](planning-center-sdk-v2/architecture/)** - SDK architecture and design decisions

## Examples

See the `examples/` directory for complete working examples:

- `PlanningCenter.Api.Client.Console/` - General usage with OAuth and PAT options
- `PlanningCenter.Api.Client.PAT.Console/` - Dedicated Personal Access Token example
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
| **People** | ✅ Complete | Manage people, households, and contact information |
| **Services** | 🚧 In Progress | Worship services, plans, and scheduling |
| **Groups** | 📋 Planned | Small groups and group management |
| **Giving** | 📋 Planned | Donations and financial management |
| **Check-Ins** | 📋 Planned | Event check-ins and attendance |
| **Registrations** | 📋 Planned | Event registrations and signups |
| **Calendar** | 📋 Planned | Calendar events and resources |
| **Publishing** | 📋 Planned | Church website and content management |

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.