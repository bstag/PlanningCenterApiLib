# Migration Guide

## Overview
This guide helps you migrate between versions of the Planning Center API SDK for .NET and provides guidance for upgrading from other Planning Center integrations.

## Version Migration

### From v1.0.x to v1.1.x

#### ServiceBase Pattern Migration ✅ Complete
All services have been migrated to the new ServiceBase pattern. This change is **backward compatible** - no code changes required.

**What Changed:**
- All services now inherit from `ServiceBase`
- Enhanced logging with correlation IDs
- Automatic performance monitoring
- Unified exception handling

**Benefits:**
- Better error messages with correlation tracking
- Automatic performance metrics
- Consistent logging across all operations
- Improved debugging capabilities

#### CLI Tool Addition
New CLI tool available for command-line operations.

**Installation:**
```bash
cd examples/PlanningCenter.Api.Client.CLI
dotnet run -- config set-token "your-app-id:your-secret"
```

### Breaking Changes

#### v1.1.x
- **None** - All changes are backward compatible

#### v1.0.x
- Initial release - no breaking changes

## Migration from Other SDKs

### From Planning Center API v1

If you're migrating from the older Planning Center API v1, here are the key differences:

#### Authentication
**Old (v1):**
```csharp
// Manual HTTP client setup
var client = new HttpClient();
client.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Basic", encodedCredentials);
```

**New (v2 SDK):**
```csharp
// Dependency injection with automatic authentication
builder.Services.AddPlanningCenterApiClientWithPAT("your-app-id:your-secret");
var peopleService = serviceProvider.GetRequiredService<IPeopleService>();
```

#### Data Access
**Old (v1):**
```csharp
// Manual JSON parsing
var response = await client.GetAsync("/people/v2/people");
var json = await response.Content.ReadAsStringAsync();
var data = JsonConvert.DeserializeObject<dynamic>(json);
```

**New (v2 SDK):**
```csharp
// Strongly typed with automatic pagination
var people = await peopleService.ListAsync();
foreach (var person in people.Data)
{
    Console.WriteLine(person.FullName);
}
```

#### Error Handling
**Old (v1):**
```csharp
// Manual status code checking
if (!response.IsSuccessStatusCode)
{
    // Handle errors manually
}
```

**New (v2 SDK):**
```csharp
// Automatic exception handling with specific types
try
{
    var person = await peopleService.GetAsync(id);
}
catch (PlanningCenterApiNotFoundException ex)
{
    // Person not found
}
catch (PlanningCenterApiValidationException ex)
{
    // Validation errors
}
```

### From Custom HTTP Implementations

#### Replace Manual HTTP Calls
**Before:**
```csharp
public async Task<Person> GetPersonAsync(string id)
{
    var url = $"https://api.planningcenteronline.com/people/v2/people/{id}";
    var response = await _httpClient.GetAsync(url);
    
    if (response.StatusCode == HttpStatusCode.NotFound)
        return null;
        
    var json = await response.Content.ReadAsStringAsync();
    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<PersonDto>>(json);
    
    return MapToDomain(apiResponse.Data);
}
```

**After:**
```csharp
public async Task<Person> GetPersonAsync(string id)
{
    try
    {
        return await _peopleService.GetAsync(id);
    }
    catch (PlanningCenterApiNotFoundException)
    {
        return null;
    }
}
```

#### Replace Manual Pagination
**Before:**
```csharp
public async Task<List<Person>> GetAllPeopleAsync()
{
    var allPeople = new List<Person>();
    var offset = 0;
    const int limit = 25;
    
    while (true)
    {
        var url = $"https://api.planningcenteronline.com/people/v2/people?offset={offset}&per_page={limit}";
        var response = await _httpClient.GetAsync(url);
        var json = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonConvert.DeserializeObject<PagedApiResponse<PersonDto>>(json);
        
        allPeople.AddRange(apiResponse.Data.Select(MapToDomain));
        
        if (apiResponse.Data.Count < limit)
            break;
            
        offset += limit;
    }
    
    return allPeople;
}
```

**After:**
```csharp
public async Task<List<Person>> GetAllPeopleAsync()
{
    return await _peopleService.GetAllAsync();
}

// Or for memory efficiency with large datasets:
public async IAsyncEnumerable<Person> StreamAllPeopleAsync()
{
    await foreach (var person in _peopleService.StreamAsync())
    {
        yield return person;
    }
}
```

## Configuration Migration

### Dependency Injection Setup

#### Basic Setup
```csharp
// Program.cs or Startup.cs
builder.Services.AddPlanningCenterApiClientWithPAT("your-app-id:your-secret");

// Or with OAuth
builder.Services.AddPlanningCenterApiClient(options =>
{
    options.ClientId = "your-oauth-client-id";
    options.ClientSecret = "your-oauth-client-secret";
    options.AccessToken = "user-access-token";
});
```

#### Advanced Configuration
```csharp
builder.Services.Configure<PlanningCenterOptions>(options =>
{
    options.BaseUrl = "https://api.planningcenteronline.com";
    options.RequestTimeout = TimeSpan.FromSeconds(30);
    options.MaxRetryAttempts = 3;
    options.EnableCaching = true;
    options.DefaultCacheExpiration = TimeSpan.FromMinutes(5);
});
```

### Environment Variables

#### Development
```bash
# .env or environment variables
PLANNING_CENTER_APP_ID=your-app-id
PLANNING_CENTER_SECRET=your-secret
```

#### Production
```csharp
// Use secure configuration providers
builder.Configuration.AddAzureKeyVault(keyVaultUrl);
// or
builder.Configuration.AddUserSecrets<Program>();
```

## Common Migration Patterns

### 1. Service Registration

**Old Pattern:**
```csharp
services.AddSingleton<IHttpClientFactory, HttpClientFactory>();
services.AddScoped<IPeopleRepository, PeopleRepository>();
```

**New Pattern:**
```csharp
services.AddPlanningCenterApiClientWithPAT(token);
// All services automatically registered:
// - IPeopleService, IGivingService, ICalendarService, etc.
// - IPlanningCenterClient for fluent API
```

### 2. Error Handling

**Old Pattern:**
```csharp
if (response.StatusCode == HttpStatusCode.Unauthorized)
{
    // Handle auth error
}
else if (response.StatusCode == HttpStatusCode.NotFound)
{
    // Handle not found
}
```

**New Pattern:**
```csharp
try
{
    var result = await service.GetAsync(id);
}
catch (PlanningCenterApiAuthenticationException ex)
{
    // Handle auth error with detailed context
    _logger.LogError(ex, "Authentication failed: {Message}", ex.Message);
}
catch (PlanningCenterApiNotFoundException ex)
{
    // Handle not found with correlation ID
    _logger.LogWarning("Resource not found: {CorrelationId}", ex.CorrelationId);
}
```

### 3. Pagination

**Old Pattern:**
```csharp
var page = 1;
var allResults = new List<T>();

while (true)
{
    var response = await GetPageAsync(page);
    allResults.AddRange(response.Data);
    
    if (response.Data.Count == 0 || !response.HasMore)
        break;
        
    page++;
}
```

**New Pattern:**
```csharp
// Automatic pagination
var allResults = await service.GetAllAsync();

// Or manual pagination with rich metadata
var page = await service.ListAsync(new QueryParameters { PerPage = 50 });
while (page.HasNextPage)
{
    page = await page.GetNextPageAsync();
    // Process page.Data
}

// Or streaming for memory efficiency
await foreach (var item in service.StreamAsync())
{
    // Process item
}
```

## Testing Migration

### Unit Testing

**Old Pattern:**
```csharp
// Mock HTTP client
var mockHandler = new Mock<HttpMessageHandler>();
mockHandler.Setup(/* complex HTTP setup */);
var httpClient = new HttpClient(mockHandler.Object);
```

**New Pattern:**
```csharp
// Mock service interfaces
var mockPeopleService = new Mock<IPeopleService>();
mockPeopleService.Setup(x => x.GetAsync("123"))
    .ReturnsAsync(new Person { Id = "123", FirstName = "John" });
```

### Integration Testing

**Setup:**
```csharp
public class IntegrationTestBase
{
    protected readonly IServiceProvider ServiceProvider;
    
    public IntegrationTestBase()
    {
        var services = new ServiceCollection();
        services.AddPlanningCenterApiClientWithPAT(
            Environment.GetEnvironmentVariable("PLANNING_CENTER_TEST_TOKEN"));
        
        ServiceProvider = services.BuildServiceProvider();
    }
}
```

## Performance Considerations

### Memory Usage

**Before (Loading All Data):**
```csharp
var allPeople = await GetAllPeopleAsync(); // Loads everything into memory
foreach (var person in allPeople)
{
    await ProcessPersonAsync(person);
}
```

**After (Streaming):**
```csharp
await foreach (var person in peopleService.StreamAsync())
{
    await ProcessPersonAsync(person); // Process one at a time
}
```

### Caching

**Enable Caching:**
```csharp
builder.Services.Configure<PlanningCenterOptions>(options =>
{
    options.EnableCaching = true;
    options.DefaultCacheExpiration = TimeSpan.FromMinutes(5);
});
```

### Parallel Processing

**Safe Parallel Operations:**
```csharp
// Services are thread-safe
var tasks = ids.Select(async id => await peopleService.GetAsync(id));
var people = await Task.WhenAll(tasks);
```

## Troubleshooting Migration

### Common Issues

#### 1. Authentication Errors
**Problem:** `PlanningCenterApiAuthenticationException`
**Solution:** Verify token format and permissions
```csharp
// Correct format: "app-id:secret"
var token = "your-app-id:your-secret";
```

#### 2. Dependency Injection Issues
**Problem:** Service not registered
**Solution:** Ensure proper service registration
```csharp
// Add this line in Program.cs
builder.Services.AddPlanningCenterApiClientWithPAT(token);
```

#### 3. Configuration Issues
**Problem:** Settings not applied
**Solution:** Configure before service registration
```csharp
builder.Services.Configure<PlanningCenterOptions>(/* options */);
builder.Services.AddPlanningCenterApiClientWithPAT(token);
```

### Migration Checklist

- [ ] Update authentication to use SDK methods
- [ ] Replace manual HTTP calls with service methods
- [ ] Update error handling to use specific exceptions
- [ ] Replace manual pagination with SDK helpers
- [ ] Update dependency injection registration
- [ ] Update unit tests to mock service interfaces
- [ ] Configure logging and monitoring
- [ ] Test all critical paths
- [ ] Update documentation and examples

## Support

If you encounter issues during migration:

1. Check the [Troubleshooting Guide](TROUBLESHOOTING.md)
2. Review the [API Reference](API_REFERENCE.md)
3. Examine the [Examples](../examples/)
4. Check the [Issues](ISSUES.md) document

---

*Last Updated: 2024-11-20*