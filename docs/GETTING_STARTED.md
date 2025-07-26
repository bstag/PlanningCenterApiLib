# Getting Started with Planning Center API SDK for .NET

This guide will help you get up and running with the Planning Center API SDK for .NET in under 15 minutes. We'll walk through installation, authentication setup, and your first API calls.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Quick Start (5 minutes)](#quick-start-5-minutes)
- [Authentication Setup](#authentication-setup)
- [Your First API Calls](#your-first-api-calls)
- [Using the Fluent API](#using-the-fluent-api)
- [CLI Tool Setup](#cli-tool-setup)
- [Next Steps](#next-steps)

## Prerequisites

- **.NET 8.0 or later** (SDK includes .NET 9.0 support)
- **Planning Center account** with API access
- **Personal Access Token** or **OAuth credentials** (we'll help you get these)
- **Visual Studio 2022**, **VS Code**, or **JetBrains Rider** (recommended)

## Installation

### Option 1: Package Manager Console

```powershell
Install-Package PlanningCenter.Api.Client
```

### Option 2: .NET CLI

```bash
dotnet add package PlanningCenter.Api.Client
```

### Option 3: PackageReference

```xml
<PackageReference Include="PlanningCenter.Api.Client" Version="1.1.0" />
```

## Quick Start (5 minutes)

### Step 1: Get Your Personal Access Token

1. Log in to [Planning Center](https://accounts.planningcenteronline.com/)
2. Go to **Settings** → **Developer** → **Personal Access Tokens**
3. Click **New Token**
4. Enter a description (e.g., "My .NET App")
5. Select required permissions (start with "People" for testing)
6. Copy the token (format: `app_id:secret`)

### Step 2: Create a Console Application

```bash
# Create new console app
dotnet new console -n MyPlanningCenterApp
cd MyPlanningCenterApp

# Add the SDK
dotnet add package PlanningCenter.Api.Client
dotnet add package Microsoft.Extensions.Hosting
dotnet add package Microsoft.Extensions.DependencyInjection
```

### Step 3: Write Your First Code

```csharp
// Program.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlanningCenter.Api.Client;

// Create host with dependency injection
var builder = Host.CreateApplicationBuilder(args);

// Add Planning Center API client with your token
builder.Services.AddPlanningCenterApiClientWithPAT("your-app-id:your-secret");

var host = builder.Build();

// Get the People service
var peopleService = host.Services.GetRequiredService<IPeopleService>();

try
{
    // Fetch first 10 people
    var people = await peopleService.ListAsync(new QueryParameters { PerPage = 10 });
    
    Console.WriteLine($"Found {people.Data.Count} people:");
    foreach (var person in people.Data)
    {
        Console.WriteLine($"- {person.FullName} ({person.Status})");
    }
    
    Console.WriteLine($"\nTotal people in organization: {people.Meta.TotalCount}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();
```

### Step 4: Run Your Application

```bash
dotnet run
```

**Expected Output:**
```
Found 10 people:
- John Smith (active)
- Jane Doe (active)
- Bob Johnson (inactive)
...

Total people in organization: 1,234

Press any key to exit...
```

🎉 **Congratulations!** You've successfully made your first Planning Center API call!

## Authentication Setup

### Personal Access Token (Recommended)

For server-side applications and background services:

```csharp
// Method 1: Direct configuration
builder.Services.AddPlanningCenterApiClientWithPAT("your-app-id:your-secret");

// Method 2: From configuration
builder.Services.AddPlanningCenterApiClient(options =>
{
    options.PersonalAccessToken = builder.Configuration["PlanningCenter:PAT"];
});

// Method 3: From environment variable
builder.Services.AddPlanningCenterApiClient(options =>
{
    options.PersonalAccessToken = Environment.GetEnvironmentVariable("PLANNING_CENTER_PAT");
});
```

### Configuration File Setup

```json
// appsettings.json
{
  "PlanningCenter": {
    "PersonalAccessToken": "your-app-id:your-secret",
    "RequestTimeout": "00:00:30",
    "EnableCaching": true,
    "DefaultCacheExpiration": "00:05:00"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "PlanningCenter.Api.Client": "Debug"
    }
  }
}
```

```csharp
// Program.cs
builder.Services.Configure<PlanningCenterOptions>(
    builder.Configuration.GetSection("PlanningCenter"));
```

### Environment Variables (Production)

```bash
# Set environment variable
export PLANNING_CENTER_PAT="your-app-id:your-secret"

# Or in Windows
set PLANNING_CENTER_PAT=your-app-id:your-secret
```

```csharp
// Use in application
builder.Services.AddPlanningCenterApiClient(options =>
{
    options.PersonalAccessToken = Environment.GetEnvironmentVariable("PLANNING_CENTER_PAT")
        ?? throw new InvalidOperationException("PLANNING_CENTER_PAT environment variable is required");
});
```

## Your First API Calls

### Working with People

```csharp
var peopleService = host.Services.GetRequiredService<IPeopleService>();

// List all people with pagination
var allPeople = await peopleService.GetAllAsync();
Console.WriteLine($"Total people: {allPeople.Count}");

// Get a specific person
var person = await peopleService.GetAsync("12345");
Console.WriteLine($"Person: {person.FullName}");

// Search for people
var searchResults = await peopleService.ListAsync(new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["first_name"] = "John",
        ["status"] = "active"
    }
});

// Create a new person
var newPerson = await peopleService.CreateAsync(new PersonCreateRequest
{
    FirstName = "Jane",
    LastName = "Smith",
    Birthdate = DateTime.Parse("1990-01-01")
});

Console.WriteLine($"Created person: {newPerson.FullName} (ID: {newPerson.Id})");
```

### Working with Multiple Services

```csharp
// Get all available services
var peopleService = host.Services.GetRequiredService<IPeopleService>();
var givingService = host.Services.GetRequiredService<IGivingService>();
var calendarService = host.Services.GetRequiredService<ICalendarService>();
var servicesService = host.Services.GetRequiredService<IServicesService>();

// Parallel operations
var tasks = new[]
{
    peopleService.ListAsync(new QueryParameters { PerPage = 5 }),
    givingService.ListDonationsAsync(new QueryParameters { PerPage = 5 }),
    calendarService.ListEventsAsync(new QueryParameters { PerPage = 5 }),
    servicesService.ListPlansAsync(new QueryParameters { PerPage = 5 })
};

var results = await Task.WhenAll(tasks);

Console.WriteLine($"People: {results[0].Data.Count}");
Console.WriteLine($"Donations: {results[1].Data.Count}");
Console.WriteLine($"Events: {results[2].Data.Count}");
Console.WriteLine($"Service Plans: {results[3].Data.Count}");
```

### Handling Pagination

```csharp
// Manual pagination
var parameters = new QueryParameters { PerPage = 25 };
var page = await peopleService.ListAsync(parameters);

Console.WriteLine($"Page {page.Meta.CurrentPage} of {page.Meta.TotalPages}");
Console.WriteLine($"Showing {page.Data.Count} of {page.Meta.TotalCount} total");

// Navigate through pages
while (page.HasNextPage)
{
    page = await page.GetNextPageAsync();
    Console.WriteLine($"Processing page {page.Meta.CurrentPage}...");
    
    foreach (var person in page.Data)
    {
        Console.WriteLine($"  - {person.FullName}");
    }
}

// Automatic pagination (gets ALL results)
var allPeople = await peopleService.GetAllAsync();
Console.WriteLine($"Retrieved all {allPeople.Count} people automatically");

// Memory-efficient streaming
var count = 0;
await foreach (var person in peopleService.StreamAsync())
{
    Console.WriteLine($"Processing: {person.FullName}");
    count++;
    
    if (count % 100 == 0)
    {
        Console.WriteLine($"Processed {count} people so far...");
    }
}
```

## Using the Fluent API

The Fluent API provides a LINQ-like interface for building complex queries:

```csharp
// Get the fluent client
var client = host.Services.GetRequiredService<IPlanningCenterClient>();
var fluent = client.Fluent();

// Simple queries
var activePeople = await fluent.People
    .Where(p => p.Status == "active")
    .GetAllAsync();

// Complex filtering
var johnSmiths = await fluent.People
    .Where(p => p.FirstName == "John")
    .Where(p => p.LastName.Contains("Smith"))
    .Where(p => p.Status == "active")
    .OrderBy(p => p.LastName)
    .ThenBy(p => p.FirstName)
    .GetAllAsync();

// Include related data
var peopleWithEmails = await fluent.People
    .Include("emails")
    .Include("phone_numbers")
    .Where(p => p.Status == "active")
    .Take(10)
    .GetAllAsync();

// Pagination with fluent API
var pagedResults = await fluent.People
    .Where(p => p.Status == "active")
    .OrderBy(p => p.LastName)
    .Skip(20)
    .Take(10)
    .GetPageAsync();

Console.WriteLine($"Found {johnSmiths.Count} John Smiths");
Console.WriteLine($"People with contact info: {peopleWithEmails.Count}");
```

### Advanced Fluent Queries

```csharp
// Date range queries
var recentDonations = await fluent.Giving.Donations
    .Where(d => d.ReceivedAt >= DateTime.Now.AddMonths(-3))
    .Where(d => d.Amount > 100)
    .OrderByDescending(d => d.ReceivedAt)
    .GetAllAsync();

// Complex joins and filtering
var upcomingEvents = await fluent.Calendar.Events
    .Where(e => e.StartsAt >= DateTime.Now)
    .Where(e => e.StartsAt <= DateTime.Now.AddDays(30))
    .Include("resource_bookings")
    .OrderBy(e => e.StartsAt)
    .GetAllAsync();

// Aggregation-style queries
var membershipStats = await fluent.Groups.Groups
    .Include("memberships")
    .Where(g => g.GroupType == "Small Group")
    .GetAllAsync();

var totalMembers = membershipStats.Sum(g => g.Memberships?.Count ?? 0);
Console.WriteLine($"Total small group members: {totalMembers}");
```

## CLI Tool Setup

The SDK includes a powerful CLI tool for terminal-based operations:

### Setup

```bash
# Navigate to CLI project
cd examples/PlanningCenter.Api.Client.CLI

# Build the CLI
dotnet build

# Set your authentication token
dotnet run -- config set-token "your-app-id:your-secret"

# Verify setup
dotnet run -- config show
```

### Basic CLI Usage

```bash
# List people
dotnet run -- people list --format table --limit 10

# Get specific person with related data
dotnet run -- people get 12345 --include "emails,phone-numbers"

# Search and export
dotnet run -- people list --where "status=active" --format csv --output active-people.csv

# Service plans
dotnet run -- services list-plans --future --format table

# Calendar events
dotnet run -- calendar list-events --start-date 2024-06-01 --end-date 2024-06-30

# Giving reports
dotnet run -- giving list-donations --fund-id 1 --format json --output donations.json
```

### CLI Help

```bash
# General help
dotnet run -- --help

# Module-specific help
dotnet run -- people --help
dotnet run -- services --help

# Command-specific help
dotnet run -- people list --help
dotnet run -- services list-plans --help
```

## Error Handling

```csharp
try
{
    var people = await peopleService.ListAsync();
    // Process results
}
catch (PlanningCenterApiException ex)
{
    Console.WriteLine($"API Error: {ex.Message}");
    Console.WriteLine($"Status Code: {ex.StatusCode}");
    Console.WriteLine($"Correlation ID: {ex.CorrelationId}");
    
    if (ex.ErrorDetails != null)
    {
        Console.WriteLine($"Details: {ex.ErrorDetails}");
    }
}
catch (UnauthorizedAccessException ex)
{
    Console.WriteLine($"Authentication failed: {ex.Message}");
    Console.WriteLine("Please check your Personal Access Token");
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Network error: {ex.Message}");
    Console.WriteLine("Please check your internet connection");
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
}
```

## Logging and Debugging

```csharp
// Enable detailed logging for development
builder.Services.Configure<PlanningCenterOptions>(options =>
{
    options.EnableDetailedLogging = true; // Only in development!
});

// Configure logging
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Custom logging
builder.Logging.AddFilter("PlanningCenter.Api.Client", LogLevel.Information);
```

## Performance Tips

### 1. Use Streaming for Large Datasets

```csharp
// ❌ Don't load everything into memory
var allPeople = await peopleService.GetAllAsync(); // Could be 10,000+ records

// ✅ Use streaming for large datasets
await foreach (var person in peopleService.StreamAsync())
{
    // Process one at a time
    await ProcessPersonAsync(person);
}
```

### 2. Enable Caching

```csharp
builder.Services.Configure<PlanningCenterOptions>(options =>
{
    options.EnableCaching = true;
    options.DefaultCacheExpiration = TimeSpan.FromMinutes(5);
});
```

### 3. Use Parallel Operations

```csharp
// Process multiple services in parallel
var tasks = new[]
{
    peopleService.ListAsync(),
    givingService.ListDonationsAsync(),
    calendarService.ListEventsAsync()
};

var results = await Task.WhenAll(tasks);
```

### 4. Optimize Queries

```csharp
// ✅ Include only what you need
var people = await fluent.People
    .Include("emails") // Only include emails, not all relationships
    .Where(p => p.Status == "active") // Filter server-side
    .Take(100) // Limit results
    .GetAllAsync();

// ❌ Don't fetch everything
var people = await fluent.People
    .Include("emails,phone_numbers,addresses,field_data,households") // Too much data
    .GetAllAsync(); // No filtering
```

## Next Steps

Now that you're up and running, explore these resources:

### Documentation
- **[Authentication Guide](AUTHENTICATION.md)** - Detailed authentication setup
- **[Best Practices](BEST_PRACTICES.md)** - Patterns and recommendations
- **[Fluent API Guide](FLUENT_API.md)** - Complete LINQ-like interface guide
- **[Troubleshooting](TROUBLESHOOTING.md)** - Common issues and solutions

### Examples
- **[Console Examples](../examples/PlanningCenter.Api.Client.Console/README.md)** - Basic usage patterns
- **[Fluent Examples](../examples/PlanningCenter.Api.Client.Fluent.Console/README.md)** - Advanced querying
- **[Worker Service](../examples/PlanningCenter.Api.Client.Worker/README.md)** - Background processing
- **[CLI Tool](../examples/PlanningCenter.Api.Client.CLI/README.md)** - Command-line interface

### Advanced Topics
- **ASP.NET Core Integration** - Web application patterns
- **Blazor Integration** - Client-side web apps
- **Background Services** - Scheduled operations
- **Multi-tenant Applications** - Enterprise scenarios

### API Modules
Explore all 9 Planning Center modules:
- **People** - Manage people, households, and contact information
- **Giving** - Donations, pledges, and financial management
- **Services** - Worship services, plans, and scheduling
- **Calendar** - Events, resources, and scheduling
- **Check-Ins** - Event check-ins and attendance
- **Groups** - Small groups and memberships
- **Registrations** - Event registrations and signups
- **Publishing** - Media content and episodes
- **Webhooks** - Real-time event notifications

---

**Questions or Issues?**

1. Check the [Troubleshooting Guide](TROUBLESHOOTING.md)
2. Review [Planning Center API Docs](https://developer.planning.center/docs/)
3. Search [GitHub Issues](https://github.com/your-repo/issues)
4. Create a new issue with your question

**Happy coding!** 🚀