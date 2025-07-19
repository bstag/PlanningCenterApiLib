# Planning Center SDK Examples

This directory contains comprehensive examples demonstrating how to use the Planning Center SDK effectively. Each example showcases different aspects of the SDK, from basic usage to advanced integration patterns.

## 📁 Available Examples

### 1. **Traditional Service API Example**
**Location**: `PlanningCenter.Api.Client.Console/`
**Focus**: Traditional service-based API patterns

**Features Demonstrated**:
- ✅ Proper authentication setup (PAT and OAuth)
- ✅ Basic CRUD operations using service interfaces
- ✅ Built-in pagination helpers and navigation
- ✅ Error handling with specific exception types
- ✅ Performance monitoring and correlation tracking
- ✅ Memory-efficient streaming operations

**Best For**: Developers who prefer traditional service injection patterns and explicit API calls.

### 2. **Fluent API Example**
**Location**: `PlanningCenter.Api.Client.Fluent.Console/`
**Focus**: LINQ-like fluent API patterns

**Features Demonstrated**:
- ✅ LINQ-like query syntax with method chaining
- ✅ Advanced filtering and sorting operations
- ✅ Terminal operations (Count, Any, First, etc.)
- ✅ Memory-efficient streaming with AsAsyncEnumerable()
- ✅ Performance optimization and query analysis
- ✅ Real-world scenarios and practical examples

**Best For**: Developers who prefer LINQ-like syntax and want expressive, readable query code.

### 3. **Multi-Module Integration Example**
**Location**: `PlanningCenter.Api.Client.MultiModule.Console/`
**Focus**: Comprehensive multi-module usage and integration

**Features Demonstrated**:
- ✅ All Planning Center modules (People, Calendar, Giving, Groups, etc.)
- ✅ Cross-module data correlation and analysis
- ✅ Advanced features and performance monitoring
- ✅ Graceful handling of missing module registrations
- ✅ Real-world integration scenarios
- ✅ Production-ready patterns and best practices

**Best For**: Developers building comprehensive Planning Center integrations that span multiple modules.

### 4. **Worker Service Example**
**Location**: `PlanningCenter.Api.Client.Worker/`
**Focus**: Background processing and scheduled tasks

**Features Demonstrated**:
- ✅ Background service implementation
- ✅ Scheduled data synchronization
- ✅ Long-running operations with proper cancellation
- ✅ Configuration management
- ✅ Logging and monitoring in production scenarios

**Best For**: Developers building background services, data sync applications, or scheduled tasks.

## 🚀 Getting Started

### Prerequisites

1. **.NET 9.0 SDK** or later
2. **Planning Center API credentials** (Personal Access Token or OAuth)

### Setting Up Credentials

#### Option 1: Personal Access Token (Recommended for server applications)

1. Go to [Planning Center API Applications](https://api.planningcenteronline.com/oauth/applications)
2. Create a new application or use an existing one
3. Copy your App ID and Secret
4. Set the environment variable:

```bash
# Format: app_id:secret
export PLANNING_CENTER_PAT="your-app-id:your-secret"
```

#### Option 2: OAuth Credentials (For user-facing applications)

1. Get your OAuth Client ID and Secret from Planning Center
2. Set the environment variables:

```bash
export PLANNING_CENTER_CLIENT_ID="your-client-id"
export PLANNING_CENTER_CLIENT_SECRET="your-client-secret"
```

### Running the Examples

#### Traditional Service API Example
```bash
cd examples/PlanningCenter.Api.Client.Console
dotnet run
```

#### Fluent API Example
```bash
cd examples/PlanningCenter.Api.Client.Fluent.Console
dotnet run
```

#### Multi-Module Integration Example
```bash
cd examples/PlanningCenter.Api.Client.MultiModule.Console
dotnet run
```

#### Worker Service Example
```bash
cd examples/PlanningCenter.Api.Client.Worker
dotnet run
```

## 📚 Example Highlights

### Authentication Patterns

All examples demonstrate proper authentication setup:

```csharp
// Automatic credential detection
var pat = Environment.GetEnvironmentVariable("PLANNING_CENTER_PAT");
var clientId = Environment.GetEnvironmentVariable("PLANNING_CENTER_CLIENT_ID");
var clientSecret = Environment.GetEnvironmentVariable("PLANNING_CENTER_CLIENT_SECRET");

if (!string.IsNullOrEmpty(pat))
{
    builder.Services.AddPlanningCenterApiClientWithPAT(pat);
}
else if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret))
{
    builder.Services.AddPlanningCenterApiClient(clientId, clientSecret);
}
```

### Traditional Service API Usage

```csharp
// Dependency injection
var peopleService = host.Services.GetRequiredService<IPeopleService>();

// Basic operations
var currentUser = await peopleService.GetMeAsync();
var people = await peopleService.ListAsync(new QueryParameters 
{ 
    PerPage = 25,
    Where = new Dictionary<string, object> { ["status"] = "active" }
});

// Automatic pagination
var allPeople = await peopleService.GetAllAsync(parameters);

// Memory-efficient streaming
await foreach (var person in peopleService.StreamAsync())
{
    // Process one person at a time
}
```

### Fluent API Usage

```csharp
// LINQ-like syntax
var activeMembers = await client.People()
    .Where(p => p.Status == "active")
    .Where(p => p.MembershipStatus == "member")
    .OrderBy(p => p.LastName)
    .ThenBy(p => p.FirstName)
    .GetAllAsync();

// Terminal operations
var count = await client.People().CountAsync();
var anyActive = await client.People().AnyAsync(p => p.Status == "active");
var firstPerson = await client.People().FirstOrDefaultAsync();

// Memory-efficient streaming
await foreach (var person in client.People()
    .Where(p => p.Status == "active")
    .AsAsyncEnumerable())
{
    // Process one person at a time
}
```

### Multi-Module Integration

```csharp
// Cross-module operations
var activeMembers = await client.People()
    .Where(p => p.Status == "active")
    .GetAllAsync();

var groupMemberships = await client.Groups()
    .Memberships()
    .Where(m => m.JoinedAt.HasValue)
    .GetAllAsync();

var recentDonations = await client.Giving()
    .Where(d => d.ReceivedAt >= DateTime.Today.AddDays(-30))
    .GetAllAsync();

// Calculate engagement metrics
var engagementRate = CalculateEngagementRate(activeMembers, groupMemberships);
```

### Error Handling

```csharp
try
{
    var person = await peopleService.GetAsync("123");
}
catch (PlanningCenterApiNotFoundException ex)
{
    // Handle not found
    logger.LogWarning("Person not found: {PersonId}", "123");
}
catch (PlanningCenterApiAuthenticationException ex)
{
    // Handle authentication issues
    logger.LogError("Authentication failed: {Message}", ex.Message);
}
catch (PlanningCenterApiException ex)
{
    // Handle general API errors
    logger.LogError("API error: {Message} [RequestId: {RequestId}]", 
        ex.Message, ex.RequestId);
}
```

### Performance Monitoring

```csharp
// Built-in performance tracking
var result = await client.People()
    .Where(p => p.Status == "active")
    .ExecuteWithDebugInfoAsync();

logger.LogInformation("Query took {ElapsedMs}ms", result.ExecutionTime.TotalMilliseconds);
logger.LogInformation("Optimization score: {Score}", result.OptimizationInfo.Score);
```

## 🎯 Best Practices Demonstrated

### 1. **Authentication Security**
- Environment variables for credentials
- Automatic credential detection
- Fallback patterns for development

### 2. **Error Handling**
- Specific exception types
- Correlation ID tracking
- Graceful degradation

### 3. **Performance Optimization**
- Memory-efficient streaming
- Built-in caching
- Performance monitoring
- Query optimization

### 4. **Production Readiness**
- Comprehensive logging
- Configuration management
- Health checks and monitoring
- Proper resource disposal

### 5. **Code Organization**
- Dependency injection patterns
- Separation of concerns
- Testable architecture
- Clean code principles

## 📖 Additional Resources

- **[Module Documentation](../docs/modules/)** - Detailed documentation for each Planning Center module
- **[API Reference](../docs/API_REFERENCE.md)** - Complete API documentation
- **[Fluent API Guide](../docs/FLUENT_API.md)** - Comprehensive fluent API documentation
- **[Integration Tests](../src/PlanningCenter.Api.Client.IntegrationTests/)** - Real-world integration test examples

## 🤝 Contributing

When adding new examples:

1. Follow the established patterns for authentication and error handling
2. Include comprehensive logging and documentation
3. Demonstrate both success and error scenarios
4. Add appropriate project files and dependencies
5. Update this README with your new example

## 🆘 Troubleshooting

### Common Issues

1. **Authentication Errors**
   - Verify your credentials are correct
   - Check environment variable names
   - Ensure your Planning Center app has the necessary permissions

2. **Module Not Registered Errors**
   - Add the required services to your DI container
   - Check the service registration examples

3. **Rate Limiting**
   - The SDK includes automatic retry logic
   - Monitor your API usage with the built-in rate limit tracking

4. **Performance Issues**
   - Use streaming for large datasets
   - Enable caching for frequently accessed data
   - Monitor query performance with built-in tools

### Getting Help

- Check the [Integration Tests](../src/PlanningCenter.Api.Client.IntegrationTests/) for working examples
- Review the [Module Documentation](../docs/modules/) for specific module usage
- Look at the SDK source code for implementation details

## 🎉 Success!

These examples provide a comprehensive foundation for building robust Planning Center integrations. They demonstrate production-ready patterns, best practices, and advanced features that will help you build reliable, performant applications.

Happy coding! 🚀