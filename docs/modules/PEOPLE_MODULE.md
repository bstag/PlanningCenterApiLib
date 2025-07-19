# People Module Documentation

The People module is the core module of Planning Center, providing comprehensive management of people, households, contact information, workflows, and lists. This module handles all person-related data and relationships within your organization.

## 📋 Module Overview

### Key Entities
- **People**: Individual person records with demographics, contact info, and relationships
- **Households**: Family units containing multiple people
- **Addresses**: Physical addresses associated with people or households
- **Emails**: Email addresses with types (home, work, etc.)
- **Phone Numbers**: Phone numbers with types (mobile, home, work, etc.)
- **Workflows**: Process management for people (new member, follow-up, etc.)
- **Workflow Cards**: Individual instances of people in workflows
- **Lists**: Custom groupings of people for various purposes
- **Forms**: Data collection forms and their submissions

### Authentication
Requires Planning Center People app access with appropriate permissions.

## 🔧 Traditional Service API

### Basic CRUD Operations

#### Get Current User
```csharp
public class PeopleService
{
    private readonly IPeopleService _peopleService;
    
    public PeopleService(IPeopleService peopleService)
    {
        _peopleService = peopleService;
    }
    
    // Get the current authenticated user
    public async Task<Person> GetCurrentUserAsync()
    {
        return await _peopleService.GetMeAsync();
    }
}
```

#### Person Management
```csharp
// Get a person by ID
var person = await _peopleService.GetAsync("12345");

// List people with pagination
var people = await _peopleService.ListAsync(new QueryParameters 
{ 
    PerPage = 25,
    Where = new Dictionary<string, object> 
    {
        ["status"] = "active"
    }
});

// Create a new person
var newPerson = await _peopleService.CreateAsync(new PersonCreateRequest
{
    FirstName = "John",
    LastName = "Doe",
    Gender = "Male",
    Birthdate = new DateTime(1990, 1, 1),
    Status = "active"
});

// Update a person
var updatedPerson = await _peopleService.UpdateAsync("12345", new PersonUpdateRequest
{
    FirstName = "Jonathan",
    MembershipStatus = "member"
});

// Delete a person
await _peopleService.DeleteAsync("12345");
```

#### Contact Information Management
```csharp
// Add an address
var address = await _peopleService.AddAddressAsync("12345", new AddressCreateRequest
{
    Street = "123 Main St",
    City = "Anytown",
    State = "CA",
    Zip = "12345",
    Location = "Home"
});

// Add an email
var email = await _peopleService.AddEmailAsync("12345", new EmailCreateRequest
{
    Address = "john.doe@example.com",
    Location = "Home",
    Primary = true
});

// Add a phone number
var phone = await _peopleService.AddPhoneNumberAsync("12345", new PhoneNumberCreateRequest
{
    Number = "+1-555-123-4567",
    Location = "Mobile",
    Primary = true
});

// Update contact information
var updatedEmail = await _peopleService.UpdateEmailAsync("12345", "email123", new EmailUpdateRequest
{
    Address = "john.doe@newdomain.com"
});

// Delete contact information
await _peopleService.DeleteAddressAsync("12345", "address123");
await _peopleService.DeleteEmailAsync("12345", "email123");
await _peopleService.DeletePhoneNumberAsync("12345", "phone123");
```

### Household Management
```csharp
// Get a household
var household = await _peopleService.GetHouseholdAsync("household123");

// List households
var households = await _peopleService.ListHouseholdsAsync(new QueryParameters
{
    PerPage = 25,
    OrderBy = "name"
});

// Create a household
var newHousehold = await _peopleService.CreateHouseholdAsync(new HouseholdCreateRequest
{
    Name = "The Doe Family",
    PrimaryContactId = "12345",
    PersonIds = new[] { "12345", "67890" }
});

// Update a household
var updatedHousehold = await _peopleService.UpdateHouseholdAsync("household123", new HouseholdUpdateRequest
{
    Name = "The Smith Family"
});

// List people in a household
var householdMembers = await _peopleService.ListPeopleInHouseholdAsync("household123");

// Delete a household
await _peopleService.DeleteHouseholdAsync("household123");
```

### Workflow Management
```csharp
// Get a workflow
var workflow = await _peopleService.GetWorkflowAsync("workflow123");

// List workflows
var workflows = await _peopleService.ListWorkflowsAsync();

// List workflow cards
var cards = await _peopleService.ListWorkflowCardsAsync("workflow123", new QueryParameters
{
    Where = new Dictionary<string, object> { ["stage"] = "new" }
});

// Create a workflow card (add person to workflow)
var card = await _peopleService.CreateWorkflowCardAsync("workflow123", new WorkflowCardCreateRequest
{
    PersonId = "12345",
    Note = "New member follow-up needed"
});

// Update a workflow card
var updatedCard = await _peopleService.UpdateWorkflowCardAsync("workflow123", "card123", new WorkflowCardUpdateRequest
{
    Note = "Follow-up completed",
    Stage = "completed"
});

// Get a specific workflow card
var specificCard = await _peopleService.GetWorkflowCardAsync("workflow123", "card123");

// Delete a workflow card
await _peopleService.DeleteWorkflowCardAsync("workflow123", "card123");
```

### List Management
```csharp
// Get a people list
var list = await _peopleService.GetPeopleListAsync("list123");

// List all people lists
var lists = await _peopleService.ListPeopleListsAsync(new QueryParameters
{
    Where = new Dictionary<string, object> { ["batch_completed_at"] = null }
});

// Create a people list
var newList = await _peopleService.CreatePeopleListAsync(new PeopleListCreateRequest
{
    Name = "New Members 2024",
    Description = "People who joined in 2024",
    AutoRefresh = true
});

// Update a people list
var updatedList = await _peopleService.UpdatePeopleListAsync("list123", new PeopleListUpdateRequest
{
    Name = "Updated List Name",
    Description = "Updated description"
});

// Get list members
var members = await _peopleService.GetListMembersAsync("list123");

// List people in a list
var peopleInList = await _peopleService.ListPeopleInListAsync("list123");

// Add person to list
var member = await _peopleService.AddPersonToListAsync("list123", new ListMemberCreateRequest
{
    PersonId = "12345"
});

// Remove person from list
await _peopleService.RemovePersonFromListAsync("list123", "12345");

// Delete a people list
await _peopleService.DeletePeopleListAsync("list123");
```

### Form Management
```csharp
// Get a form
var form = await _peopleService.GetFormAsync("form123");

// List forms
var forms = await _peopleService.ListFormsAsync(new QueryParameters
{
    Where = new Dictionary<string, object> { ["active"] = true }
});

// List form submissions
var submissions = await _peopleService.ListFormSubmissionsAsync("form123");

// Submit a form
var submission = await _peopleService.SubmitFormAsync("form123", new FormSubmitRequest
{
    FieldData = new Dictionary<string, object>
    {
        ["first_name"] = "John",
        ["last_name"] = "Doe",
        ["email"] = "john.doe@example.com"
    }
});

// Get a specific form submission
var specificSubmission = await _peopleService.GetFormSubmissionAsync("form123", "submission123");
```

### Pagination Helpers
```csharp
// Get all people (handles pagination automatically)
var allPeople = await _peopleService.GetAllAsync(new QueryParameters
{
    Where = new Dictionary<string, object> { ["status"] = "active" }
});

// Stream people for memory-efficient processing
await foreach (var person in _peopleService.StreamAsync())
{
    Console.WriteLine($"{person.FirstName} {person.LastName}");
    // Process one person at a time without loading all into memory
}
```

## 🚀 Fluent API Usage

### Basic Queries
```csharp
public class PeopleFluentService
{
    private readonly IPlanningCenterClient _client;
    
    public PeopleFluentService(IPlanningCenterClient client)
    {
        _client = client;
    }
    
    // Get a person by ID
    public async Task<Person?> GetPersonAsync(string id)
    {
        return await _client.People().GetAsync(id);
    }
    
    // Get first page of people
    public async Task<IPagedResponse<Person>> GetPeoplePageAsync()
    {
        return await _client.People().GetPagedAsync(pageSize: 25);
    }
}
```

### Advanced Filtering and Sorting
```csharp
// Complex filtering with multiple conditions
var activeAdults = await _client.People()
    .Where(p => p.Status == "active")
    .Where(p => p.Birthdate < DateTime.Now.AddYears(-18))
    .Where(p => p.MembershipStatus == "member")
    .OrderBy(p => p.LastName)
    .ThenBy(p => p.FirstName)
    .GetAllAsync();

// Find people by name pattern
var johnsAndJanes = await _client.People()
    .Where(p => p.FirstName.StartsWith("John") || p.FirstName.StartsWith("Jane"))
    .OrderBy(p => p.FirstName)
    .GetPagedAsync(pageSize: 50);

// Get people with recent birthdays
var recentBirthdays = await _client.People()
    .Where(p => p.Birthdate.HasValue)
    .Where(p => p.Status == "active")
    .OrderBy(p => p.Birthdate)
    .GetAllAsync();
```

### LINQ-like Terminal Operations
```csharp
// Get first person matching criteria
var firstActive = await _client.People()
    .Where(p => p.Status == "active")
    .FirstAsync();

// Get first or default
var maybeInactive = await _client.People()
    .Where(p => p.Status == "inactive")
    .FirstOrDefaultAsync();

// Get single person (throws if more than one)
var uniqueEmail = await _client.People()
    .Where(p => p.Emails.Any(e => e.Address == "unique@example.com"))
    .SingleAsync();

// Count people
var activeCount = await _client.People()
    .Where(p => p.Status == "active")
    .CountAsync();

// Check if any exist
var hasInactive = await _client.People()
    .Where(p => p.Status == "inactive")
    .AnyAsync();

// Check with additional predicate
var hasAdults = await _client.People()
    .AnyAsync(p => p.Birthdate < DateTime.Now.AddYears(-18));
```

### Memory-Efficient Streaming
```csharp
// Stream all active people
await foreach (var person in _client.People()
    .Where(p => p.Status == "active")
    .AsAsyncEnumerable())
{
    // Process one person at a time
    await ProcessPersonAsync(person);
}

// Stream with pagination options
var options = new PaginationOptions { PageSize = 100 };
await foreach (var person in _client.People()
    .Where(p => p.Status == "active")
    .AsAsyncEnumerable(options))
{
    await ProcessPersonAsync(person);
}
```

### Fluent Person Creation
```csharp
// Create a person with fluent syntax
var newPerson = await _client.People()
    .Create(new PersonCreateRequest
    {
        FirstName = "John",
        LastName = "Doe",
        Gender = "Male",
        Birthdate = new DateTime(1990, 1, 1)
    })
    .WithAddress(new AddressCreateRequest
    {
        Street = "123 Main St",
        City = "Anytown",
        State = "CA",
        Zip = "12345"
    })
    .WithEmail(new EmailCreateRequest
    {
        Address = "john.doe@example.com",
        Primary = true
    })
    .WithPhoneNumber(new PhoneNumberCreateRequest
    {
        Number = "+1-555-123-4567",
        Location = "Mobile",
        Primary = true
    })
    .ExecuteAsync();
```

### Advanced Features
```csharp
// Get query optimization information
var optimizationInfo = _client.People()
    .Where(p => p.Status == "active")
    .GetOptimizationInfo();

Console.WriteLine($"Query complexity: {optimizationInfo.ComplexityScore}");
Console.WriteLine($"Estimated results: {optimizationInfo.EstimatedResultCount}");

// Execute with debug information
var result = await _client.People()
    .Where(p => p.Status == "active")
    .ExecuteWithDebugInfoAsync();

Console.WriteLine($"Execution time: {result.ExecutionTime.TotalMilliseconds}ms");
Console.WriteLine($"Success: {result.Success}");

// Clone a query context
var baseQuery = _client.People()
    .Where(p => p.Status == "active")
    .OrderBy(p => p.LastName);

var membersQuery = baseQuery.Clone()
    .Where(p => p.MembershipStatus == "member");

var visitorsQuery = baseQuery.Clone()
    .Where(p => p.MembershipStatus == "visitor");

// Add custom parameters
var customQuery = await _client.People()
    .Where(p => p.Status == "active")
    .WithParameter("include", "addresses,emails,phone_numbers")
    .GetPagedAsync();
```

## 💡 Common Use Cases

### 1. Member Directory
```csharp
public async Task<IReadOnlyList<Person>> GetMemberDirectoryAsync()
{
    return await _client.People()
        .Where(p => p.Status == "active")
        .Where(p => p.MembershipStatus == "member")
        .Include(p => p.Addresses)
        .Include(p => p.Emails)
        .Include(p => p.PhoneNumbers)
        .OrderBy(p => p.LastName)
        .ThenBy(p => p.FirstName)
        .GetAllAsync();
}
```

### 2. Birthday List
```csharp
public async Task<IReadOnlyList<Person>> GetUpcomingBirthdaysAsync(int days = 30)
{
    var today = DateTime.Today;
    var endDate = today.AddDays(days);
    
    return await _client.People()
        .Where(p => p.Status == "active")
        .Where(p => p.Birthdate.HasValue)
        .OrderBy(p => p.Birthdate)
        .GetAllAsync()
        .ContinueWith(task => 
        {
            var people = task.Result;
            return people.Where(p => 
            {
                if (!p.Birthdate.HasValue) return false;
                var birthday = new DateTime(today.Year, p.Birthdate.Value.Month, p.Birthdate.Value.Day);
                if (birthday < today) birthday = birthday.AddYears(1);
                return birthday <= endDate;
            }).ToList().AsReadOnly();
        });
}
```

### 3. New Member Workflow
```csharp
public async Task ProcessNewMemberAsync(string personId)
{
    // Add to new member workflow
    var card = await _peopleService.CreateWorkflowCardAsync("new-member-workflow", 
        new WorkflowCardCreateRequest
        {
            PersonId = personId,
            Note = "New member - needs welcome packet"
        });
    
    // Add to new members list
    await _peopleService.AddPersonToListAsync("new-members-list", 
        new ListMemberCreateRequest { PersonId = personId });
    
    // Send welcome email (custom logic)
    var person = await _client.People().GetAsync(personId);
    if (person?.Emails.Any(e => e.Primary) == true)
    {
        await SendWelcomeEmailAsync(person);
    }
}
```

### 4. Data Export
```csharp
public async Task ExportPeopleDataAsync(string filePath)
{
    using var writer = new StreamWriter(filePath);
    await writer.WriteLineAsync("FirstName,LastName,Email,Phone,Status");
    
    await foreach (var person in _client.People()
        .Where(p => p.Status == "active")
        .Include(p => p.Emails)
        .Include(p => p.PhoneNumbers)
        .AsAsyncEnumerable())
    {
        var email = person.Emails.FirstOrDefault(e => e.Primary)?.Address ?? "";
        var phone = person.PhoneNumbers.FirstOrDefault(p => p.Primary)?.Number ?? "";
        
        await writer.WriteLineAsync($"{person.FirstName},{person.LastName},{email},{phone},{person.Status}");
    }
}
```

### 5. Duplicate Detection
```csharp
public async Task<IEnumerable<IGrouping<string, Person>>> FindPotentialDuplicatesAsync()
{
    var allPeople = await _client.People()
        .Where(p => p.Status == "active")
        .GetAllAsync();
    
    // Group by similar names
    return allPeople
        .GroupBy(p => $"{p.FirstName?.ToLower().Trim()}{p.LastName?.ToLower().Trim()}")
        .Where(g => g.Count() > 1);
}
```

## 📊 Advanced Features

### Batch Operations
```csharp
// Create a batch context for multiple operations
var batch = _client.People().Batch();

// Add multiple operations to the batch
batch.Create(new PersonCreateRequest { FirstName = "John", LastName = "Doe" });
batch.Create(new PersonCreateRequest { FirstName = "Jane", LastName = "Smith" });
batch.Update("12345", new PersonUpdateRequest { Status = "active" });

// Execute all operations
var results = await batch.ExecuteAsync();
```

### Performance Monitoring
```csharp
// Monitor query performance
var stopwatch = Stopwatch.StartNew();
var people = await _client.People()
    .Where(p => p.Status == "active")
    .GetAllAsync();
stopwatch.Stop();

Console.WriteLine($"Query returned {people.Count} people in {stopwatch.ElapsedMilliseconds}ms");

// Use built-in performance tracking
var result = await _client.People()
    .Where(p => p.Status == "active")
    .ExecuteWithDebugInfoAsync();

Console.WriteLine($"Execution time: {result.ExecutionTime}");
Console.WriteLine($"Optimization score: {result.OptimizationInfo.Score}");
```

### Error Handling Best Practices
```csharp
public async Task<Person?> SafeGetPersonAsync(string id)
{
    try
    {
        return await _client.People().GetAsync(id);
    }
    catch (PlanningCenterApiNotFoundException)
    {
        // Person doesn't exist
        return null;
    }
    catch (PlanningCenterApiAuthenticationException ex)
    {
        // Log authentication issue
        _logger.LogError(ex, "Authentication failed when getting person {PersonId}", id);
        throw;
    }
    catch (PlanningCenterApiException ex)
    {
        // Log API error with correlation ID
        _logger.LogError(ex, "API error when getting person {PersonId}: {ErrorMessage} [RequestId: {RequestId}]", 
            id, ex.Message, ex.RequestId);
        throw;
    }
}
```

## 🎯 Best Practices

1. **Use Fluent API for Complex Queries**: The fluent API provides better readability and type safety.

2. **Stream Large Datasets**: Use `AsAsyncEnumerable()` for processing large amounts of data.

3. **Handle Specific Exceptions**: Always catch and handle specific exception types.

4. **Use Correlation IDs**: The SDK automatically tracks correlation IDs for debugging.

5. **Monitor Performance**: Use built-in performance monitoring for optimization.

6. **Validate Input**: Always validate input parameters before making API calls.

7. **Use Pagination**: Don't load all data at once unless necessary.

8. **Cache Static Data**: Cache relatively static data like workflows and forms.

9. **Batch Operations**: Use batch operations for multiple related changes.

10. **Include Related Data**: Use `Include()` to fetch related data in a single request when needed.