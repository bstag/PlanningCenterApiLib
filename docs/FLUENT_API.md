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

## Getting Started

### Basic Usage

```csharp
using PlanningCenter.Api.Client.Extensions;

// Get the fluent API interface
var fluentClient = client.Fluent();

// Access module-specific fluent contexts
var people = fluentClient.People;
var giving = fluentClient.Giving;
var calendar = fluentClient.Calendar;
// ... etc
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

### Currently Implemented
- ✅ **People Module**: Full fluent API implementation

### Planned Implementation
- 🚧 **Giving Module**: Coming soon
- 🚧 **Calendar Module**: Coming soon
- 🚧 **Check-Ins Module**: Coming soon
- 🚧 **Groups Module**: Coming soon
- 🚧 **Registrations Module**: Coming soon
- 🚧 **Publishing Module**: Coming soon
- 🚧 **Services Module**: Coming soon
- 🚧 **Webhooks Module**: Coming soon

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

## Examples Repository

See the `examples/PlanningCenter.Api.Client.Fluent.Console` project for comprehensive examples of fluent API usage.