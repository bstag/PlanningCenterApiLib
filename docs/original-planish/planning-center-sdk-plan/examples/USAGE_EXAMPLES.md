# Planning Center SDK Usage Examples

> **Implementation Status**: These examples represent the current fully implemented API. All modules and fluent API functionality are production-ready. See [CURRENT_STATUS.md](../CURRENT_STATUS.md) for complete implementation details.

## Getting Started

### Installation
```bash
dotnet add package PlanningCenter.Api.Client
```

### Basic Configuration
```csharp
// Program.cs or Startup.cs
services.AddPlanningCenterApiClient(options =>
{
    options.ClientId = "your-client-id";
    options.ClientSecret = "your-client-secret";
    options.BaseUrl = "https://api.planningcenteronline.com";
});
```

## Service-Based API Examples

### People Management
```csharp
public class PeopleController : ControllerBase
{
    private readonly IPeopleService _peopleService;
    
    public PeopleController(IPeopleService peopleService)
    {
        _peopleService = peopleService;
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Core.Person>> GetPerson(string id)
    {
        try
        {
            var person = await _peopleService.GetAsync(id);
            return Ok(person);
        }
        catch (PlanningCenterApiNotFoundException)
        {
            return NotFound();
        }
    }
    
    [HttpGet]
    public async Task<ActionResult<IPagedResponse<Core.Person>>> GetPeople(
        [FromQuery] string status = "active",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25)
    {
        var parameters = new QueryParameters
        {
            Where = new Dictionary<string, object> { ["status"] = status },
            Include = new[] { "addresses", "emails", "phone_numbers" },
            OrderBy = "last_name",
            Offset = (page - 1) * pageSize,
            PerPage = pageSize
        };
        
        var people = await _peopleService.ListAsync(parameters);
        return Ok(people);
    }
    
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<Core.Person>>> GetAllPeople(
        [FromQuery] string status = "active")
    {
        // Built-in pagination helper automatically fetches all pages
        var parameters = new QueryParameters
        {
            Where = new Dictionary<string, object> { ["status"] = status },
            Include = new[] { "addresses", "emails", "phone_numbers" },
            OrderBy = "last_name"
        };
        
        var allPeople = await _peopleService.GetAllAsync(parameters);
        return Ok(allPeople);
    }
    
    [HttpGet("stream")]
    public async IAsyncEnumerable<Core.Person> StreamPeople(
        [FromQuery] string status = "active",
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // Memory-efficient streaming of large datasets
        var parameters = new QueryParameters
        {
            Where = new Dictionary<string, object> { ["status"] = status },
            OrderBy = "last_name"
        };
        
        await foreach (var person in _peopleService.StreamAsync(parameters, cancellationToken: cancellationToken))
        {
            yield return person;
        }
    }
    
    [HttpPost]
    public async Task<ActionResult<Core.Person>> CreatePerson([FromBody] PersonCreateRequest request)
    {
        var person = await _peopleService.CreateAsync(request);
        return CreatedAtAction(nameof(GetPerson), new { id = person.Id }, person);
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<Core.Person>> UpdatePerson(string id, [FromBody] PersonUpdateRequest request)
    {
        var person = await _peopleService.UpdateAsync(id, request);
        return Ok(person);
    }
    
    [HttpPost("{id}/addresses")]
    public async Task<ActionResult<Address>> AddAddress(string id, [FromBody] AddressCreateRequest request)
    {
        var address = await _peopleService.AddAddressAsync(id, request);
        return Ok(address);
    }
}
```

> **Note**: The above examples show the current fully implemented API design. All features demonstrated are production-ready and available now.

## Built-in Pagination Support Examples

The SDK provides comprehensive pagination support with automatic handling:

### Manual Pagination
```csharp
// Get specific page
var page2 = await _peopleService.ListAsync(new QueryParameters
{
    Offset = 25, // Skip first 25 records
    PerPage = 25, // Get next 25 records
    OrderBy = "last_name"
});

// Navigate through pages using fluent API
var firstPage = await client.Fluent().People
    .OrderBy(p => p.LastName)
    .GetPagedAsync(pageSize: 25);

if (firstPage.HasNextPage)
{
    var nextPage = await firstPage.GetNextPageAsync();
}
```

### Automatic Pagination
```csharp
// Get all data automatically (handles pagination internally)
var allPeople = await _peopleService.GetAllAsync(new QueryParameters
{
    Where = new Dictionary<string, object> { ["status"] = "active" },
    OrderBy = "last_name"
});

// Using fluent API
var allActivePeople = await client.Fluent().People
    .Where(p => p.Status == "Active")
    .OrderBy(p => p.LastName)
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
// Stream large datasets without loading everything into memory
await foreach (var person in _peopleService.StreamAsync(new QueryParameters
{
    Where = new Dictionary<string, object> { ["status"] = "active" },
    OrderBy = "created_at"
}))
{
    // Process each person individually
    await ProcessPersonAsync(person);
}

// Using fluent API streaming
await foreach (var person in client.Fluent().People
    .Where(p => p.Status == "Active")
    .OrderBy(p => p.CreatedAt)
    .AsAsyncEnumerable())
{
    // Memory usage stays constant regardless of total count
    await ProcessPersonAsync(person);
}
```

## Fluent API Examples

The fluent API provides LINQ-like syntax for intuitive querying across all modules:

### People Module
```csharp
// Basic querying
var activePeople = await client.Fluent().People
    .Where(p => p.Status == "Active")
    .OrderBy(p => p.LastName)
    .GetPagedAsync();

// Complex queries with includes
var peopleWithDetails = await client.Fluent().People
    .Where(p => p.FirstName == "John")
    .Where(p => p.CreatedAt > DateTime.Now.AddDays(-30))
    .Include(p => p.Addresses)
    .Include(p => p.Emails)
    .OrderBy(p => p.LastName)
    .ThenBy(p => p.FirstName)
    .GetAllAsync();

// LINQ-like terminal operations
var firstPerson = await client.Fluent().People
    .OrderBy(p => p.CreatedAt)
    .FirstAsync();

var personCount = await client.Fluent().People
    .Where(p => p.Status == "Active")
    .CountAsync();

var hasActiveMembers = await client.Fluent().People
    .AnyAsync(p => p.Status == "Active");
```

### Giving Module
```csharp
// Fund-based queries
var recentDonations = await client.Fluent().Giving
    .ByFund("123")
    .InDateRange(DateTime.Now.AddDays(-30), DateTime.Now)
    .OrderByDescending(d => d.ReceivedAt)
    .GetPagedAsync();

// Financial aggregations
var totalAmount = await client.Fluent().Giving
    .ByFund("123")
    .TotalAmountAsync();

var averageGift = await client.Fluent().Giving
    .InDateRange(startDate, endDate)
    .AverageAmountAsync();

// Payment method filtering
var creditCardGifts = await client.Fluent().Giving
    .CreditCardOnly()
    .MinimumAmount(100)
    .GetAllAsync();
```

### Calendar Module
```csharp
// Date-based filtering
var upcomingEvents = await client.Fluent().Calendar
    .Upcoming()
    .OrderBy(e => e.StartsAt)
    .GetPagedAsync();

var thisWeekEvents = await client.Fluent().Calendar
    .ThisWeek()
    .Include(e => e.EventInstances)
    .GetAllAsync();

// Event aggregations
var eventCount = await client.Fluent().Calendar
    .ByDateRange(startDate, endDate)
    .CountAsync();

var avgDuration = await client.Fluent().Calendar
    .ThisMonth()
    .AverageDurationHoursAsync();
```

### Services Module
```csharp
// Service planning queries
var recentPlans = await client.Fluent().Services
    .ByServiceType("456")
    .InDateRange(DateTime.Now.AddDays(-7), DateTime.Now.AddDays(7))
    .OrderBy(p => p.Date)
    .GetPagedAsync();

// Plan item management
var plansWithSongs = await client.Fluent().Services
    .Include(p => p.Items)
    .Where(p => p.Date >= DateTime.Today)
    .GetAllAsync();
```

### Memory-Efficient Streaming
```csharp
// Process large datasets efficiently
await foreach (var person in client.Fluent().People
    .Where(p => p.Status == "Active")
    .OrderBy(p => p.CreatedAt)
    .AsAsyncEnumerable())
{
    // Process each person individually
    // Memory usage stays constant regardless of total count
    await ProcessPersonAsync(person);
}

// Stream with custom pagination
var streamOptions = new PaginationOptions { PageSize = 50 };
await foreach (var donation in client.Fluent().Giving
    .InDateRange(startDate, endDate)
    .AsAsyncEnumerable(streamOptions))
{
    await ProcessDonationAsync(donation);
}
```

## Working Example Projects

The SDK includes several complete example projects demonstrating real-world usage:

### Console Application
```csharp
// examples/PlanningCenter.Api.Client.Console/Program.cs
// Complete working example with real API integration
var serviceCollection = new ServiceCollection();
serviceCollection.AddPlanningCenterApiClient(options =>
{
    options.ClientId = Environment.GetEnvironmentVariable("PC_CLIENT_ID");
    options.ClientSecret = Environment.GetEnvironmentVariable("PC_CLIENT_SECRET");
});

var serviceProvider = serviceCollection.BuildServiceProvider();
var client = serviceProvider.GetRequiredService<IPlanningCenterApiClient>();

// Demonstrate pagination
var people = await client.People.ListAsync(new QueryParameters
{
    Include = new[] { "addresses", "emails" },
    PerPage = 10
});

Console.WriteLine($"Found {people.Data.Count} people");
foreach (var person in people.Data)
{
    Console.WriteLine($"{person.Attributes.FirstName} {person.Attributes.LastName}");
}
```

### Fluent Console Example
```csharp
// examples/PlanningCenter.Api.Client.Fluent.Console/Program.cs
// Complete fluent API demonstration
var activePeople = await client.Fluent().People
    .Where(p => p.Status == "Active")
    .Include(p => p.Addresses)
    .Include(p => p.Emails)
    .OrderBy(p => p.LastName)
    .GetPagedAsync(pageSize: 10);

Console.WriteLine($"Found {activePeople.Data.Count} active people");

// Demonstrate streaming for large datasets
await foreach (var person in client.Fluent().People
    .Where(p => p.Status == "Active")
    .AsAsyncEnumerable())
{
    Console.WriteLine($"Processing: {person.Attributes.FirstName} {person.Attributes.LastName}");
}
```

### Background Worker Service
```csharp
// examples/PlanningCenter.Api.Client.Worker/Worker.cs
// Production-ready background service with Planning Center integration
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IPlanningCenterApiClient _client;

    public Worker(ILogger<Worker> logger, IPlanningCenterApiClient client)
    {
        _logger = logger;
        _client = client;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Sync people data every hour
                await SyncPeopleDataAsync(stoppingToken);
                _logger.LogInformation("People data sync completed at: {time}", DateTimeOffset.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during people data sync");
            }
            
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
    
    private async Task SyncPeopleDataAsync(CancellationToken cancellationToken)
    {
        await foreach (var person in _client.Fluent().People
            .Where(p => p.UpdatedAt > DateTime.Now.AddHours(-1))
            .AsAsyncEnumerable()
            .WithCancellation(cancellationToken))
        {
            // Process recently updated people
            await ProcessPersonUpdateAsync(person);
        }
    }
}
```

## Additional Resources

- **Complete Examples**: See the `examples/` directory for fully functional projects
- **API Documentation**: Comprehensive XML documentation on all public APIs
- **Performance Guide**: Best practices for optimal performance
- **Migration Guide**: Upgrading from previous versions

All examples are production-ready and demonstrate real-world usage patterns with proper error handling, logging, and configuration management.