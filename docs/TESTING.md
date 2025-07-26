# Testing Guide

## Overview
This guide provides comprehensive testing strategies and best practices for applications using the Planning Center API SDK for .NET.

## Testing Architecture

### Test Project Structure
```
YourProject.Tests/
├── Unit/
│   ├── Services/
│   ├── Models/
│   └── Helpers/
├── Integration/
│   ├── PeopleService/
│   ├── GivingService/
│   └── CalendarService/
├── Fixtures/
│   ├── TestData/
│   └── MockData/
└── Helpers/
    ├── TestBase.cs
    └── MockServiceFactory.cs
```

## Unit Testing

### 1. Service Mocking

#### Basic Service Mocking
```csharp
public class PersonServiceTests
{
    private readonly Mock<IPeopleService> _mockPeopleService;
    private readonly PersonController _controller;
    
    public PersonServiceTests()
    {
        _mockPeopleService = new Mock<IPeopleService>();
        _controller = new PersonController(_mockPeopleService.Object);
    }
    
    [Fact]
    public async Task GetPerson_WithValidId_ReturnsPersonDto()
    {
        // Arrange
        var personId = "123";
        var expectedPerson = new Person
        {
            Id = personId,
            FirstName = "John",
            LastName = "Doe"
        };
        
        _mockPeopleService
            .Setup(x => x.GetAsync(personId))
            .ReturnsAsync(expectedPerson);
        
        // Act
        var result = await _controller.GetPersonAsync(personId);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedPerson.FirstName, result.FirstName);
        Assert.Equal(expectedPerson.LastName, result.LastName);
        
        _mockPeopleService.Verify(x => x.GetAsync(personId), Times.Once);
    }
    
    [Fact]
    public async Task GetPerson_WithInvalidId_ThrowsNotFoundException()
    {
        // Arrange
        var invalidId = "invalid";
        
        _mockPeopleService
            .Setup(x => x.GetAsync(invalidId))
            .ThrowsAsync(new PlanningCenterApiNotFoundException("Person not found"));
        
        // Act & Assert
        await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(
            () => _controller.GetPersonAsync(invalidId));
    }
}
```

#### Advanced Mocking with Callbacks
```csharp
[Fact]
public async Task CreatePerson_WithValidData_CallsServiceWithCorrectParameters()
{
    // Arrange
    Person capturedPerson = null;
    
    _mockPeopleService
        .Setup(x => x.CreateAsync(It.IsAny<Person>()))
        .Callback<Person>(person => capturedPerson = person)
        .ReturnsAsync((Person p) => new Person { Id = "new-id", FirstName = p.FirstName, LastName = p.LastName });
    
    var createRequest = new CreatePersonRequest
    {
        FirstName = "Jane",
        LastName = "Smith"
    };
    
    // Act
    await _controller.CreatePersonAsync(createRequest);
    
    // Assert
    Assert.NotNull(capturedPerson);
    Assert.Equal(createRequest.FirstName, capturedPerson.FirstName);
    Assert.Equal(createRequest.LastName, capturedPerson.LastName);
}
```

### 2. Fluent API Testing

```csharp
public class FluentApiTests
{
    private readonly Mock<IPlanningCenterClient> _mockClient;
    private readonly Mock<IFluentQueryBuilder<Person>> _mockQueryBuilder;
    
    public FluentApiTests()
    {
        _mockClient = new Mock<IPlanningCenterClient>();
        _mockQueryBuilder = new Mock<IFluentQueryBuilder<Person>>();
        
        _mockClient.Setup(x => x.People).Returns(_mockQueryBuilder.Object);
    }
    
    [Fact]
    public async Task FluentQuery_WithFilters_BuildsCorrectQuery()
    {
        // Arrange
        var expectedPeople = new List<Person>
        {
            new Person { Id = "1", FirstName = "John", LastName = "Doe" }
        };
        
        _mockQueryBuilder
            .Setup(x => x.Where(It.IsAny<Expression<Func<Person, bool>>>()))
            .Returns(_mockQueryBuilder.Object);
            
        _mockQueryBuilder
            .Setup(x => x.OrderBy(It.IsAny<Expression<Func<Person, object>>>()))
            .Returns(_mockQueryBuilder.Object);
            
        _mockQueryBuilder
            .Setup(x => x.Take(It.IsAny<int>()))
            .Returns(_mockQueryBuilder.Object);
            
        _mockQueryBuilder
            .Setup(x => x.ToListAsync())
            .ReturnsAsync(expectedPeople);
        
        // Act
        var result = await _mockClient.Object.People
            .Where(p => p.FirstName == "John")
            .OrderBy(p => p.LastName)
            .Take(10)
            .ToListAsync();
        
        // Assert
        Assert.Single(result);
        Assert.Equal("John", result[0].FirstName);
        
        // Verify method chain was called
        _mockQueryBuilder.Verify(x => x.Where(It.IsAny<Expression<Func<Person, bool>>>()), Times.Once);
        _mockQueryBuilder.Verify(x => x.OrderBy(It.IsAny<Expression<Func<Person, object>>>()), Times.Once);
        _mockQueryBuilder.Verify(x => x.Take(10), Times.Once);
        _mockQueryBuilder.Verify(x => x.ToListAsync(), Times.Once);
    }
}
```

### 3. Exception Testing

```csharp
public class ExceptionHandlingTests
{
    [Theory]
    [InlineData(typeof(PlanningCenterApiNotFoundException), "Resource not found")]
    [InlineData(typeof(PlanningCenterApiValidationException), "Validation failed")]
    [InlineData(typeof(PlanningCenterApiAuthenticationException), "Authentication failed")]
    public async Task ServiceMethod_WithSpecificException_HandlesCorrectly(Type exceptionType, string message)
    {
        // Arrange
        var mockService = new Mock<IPeopleService>();
        var exception = (Exception)Activator.CreateInstance(exceptionType, message);
        
        mockService
            .Setup(x => x.GetAsync(It.IsAny<string>()))
            .ThrowsAsync(exception);
        
        var handler = new PersonExceptionHandler(mockService.Object);
        
        // Act & Assert
        var thrownException = await Assert.ThrowsAsync(exceptionType, 
            () => handler.GetPersonSafelyAsync("123"));
        
        Assert.Equal(message, thrownException.Message);
    }
}
```

## Integration Testing

### 1. Test Base Setup

```csharp
public abstract class IntegrationTestBase : IDisposable
{
    protected readonly IServiceProvider ServiceProvider;
    protected readonly IPeopleService PeopleService;
    protected readonly IGivingService GivingService;
    protected readonly ICalendarService CalendarService;
    protected readonly ITestOutputHelper Output;
    
    protected IntegrationTestBase(ITestOutputHelper output)
    {
        Output = output;
        
        var services = new ServiceCollection();
        
        // Use test token from environment
        var testToken = Environment.GetEnvironmentVariable("PLANNING_CENTER_TEST_TOKEN")
            ?? throw new InvalidOperationException("PLANNING_CENTER_TEST_TOKEN environment variable is required");
        
        services.AddPlanningCenterApiClientWithPAT(testToken);
        
        // Add test-specific configuration
        services.Configure<PlanningCenterOptions>(options =>
        {
            options.RequestTimeout = TimeSpan.FromSeconds(30);
            options.MaxRetryAttempts = 3;
            options.EnableCaching = false; // Disable caching for tests
        });
        
        // Add logging
        services.AddLogging(builder =>
        {
            builder.AddXUnit(output);
            builder.SetMinimumLevel(LogLevel.Debug);
        });
        
        ServiceProvider = services.BuildServiceProvider();
        
        PeopleService = ServiceProvider.GetRequiredService<IPeopleService>();
        GivingService = ServiceProvider.GetRequiredService<IGivingService>();
        CalendarService = ServiceProvider.GetRequiredService<ICalendarService>();
    }
    
    public void Dispose()
    {
        ServiceProvider?.Dispose();
    }
}
```

### 2. People Service Integration Tests

```csharp
public class PeopleServiceIntegrationTests : IntegrationTestBase
{
    public PeopleServiceIntegrationTests(ITestOutputHelper output) : base(output) { }
    
    [Fact]
    public async Task ListPeople_WithDefaultParameters_ReturnsResults()
    {
        // Act
        var result = await PeopleService.ListAsync();
        
        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.True(result.Data.Count > 0, "Should return at least one person");
        
        var firstPerson = result.Data[0];
        Assert.NotNull(firstPerson.Id);
        Assert.NotEmpty(firstPerson.Id);
        
        Output.WriteLine($"Retrieved {result.Data.Count} people");
        Output.WriteLine($"First person: {firstPerson.FirstName} {firstPerson.LastName}");
    }
    
    [Fact]
    public async Task GetPerson_WithValidId_ReturnsPersonDetails()
    {
        // Arrange - Get a person ID from the list
        var people = await PeopleService.ListAsync(new QueryParameters { PerPage = 1 });
        Assert.True(people.Data.Count > 0, "Need at least one person for this test");
        
        var personId = people.Data[0].Id;
        
        // Act
        var person = await PeopleService.GetAsync(personId);
        
        // Assert
        Assert.NotNull(person);
        Assert.Equal(personId, person.Id);
        Assert.NotNull(person.FirstName);
        
        Output.WriteLine($"Retrieved person: {person.FirstName} {person.LastName} (ID: {person.Id})");
    }
    
    [Fact]
    public async Task GetPerson_WithInvalidId_ThrowsNotFoundException()
    {
        // Arrange
        var invalidId = "invalid-person-id-12345";
        
        // Act & Assert
        var exception = await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(
            () => PeopleService.GetAsync(invalidId));
        
        Assert.Contains("not found", exception.Message.ToLower());
        Output.WriteLine($"Correctly threw exception: {exception.Message}");
    }
    
    [Fact]
    public async Task ListPeople_WithFiltering_ReturnsFilteredResults()
    {
        // Act
        var queryParams = new QueryParameters
        {
            Where = "status='active'",
            PerPage = 10,
            OrderBy = "last_name"
        };
        
        var result = await PeopleService.ListAsync(queryParams);
        
        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        
        if (result.Data.Count > 1)
        {
            // Verify ordering
            for (int i = 1; i < result.Data.Count; i++)
            {
                var current = result.Data[i].LastName ?? "";
                var previous = result.Data[i - 1].LastName ?? "";
                Assert.True(string.Compare(current, previous, StringComparison.OrdinalIgnoreCase) >= 0,
                    "Results should be ordered by last name");
            }
        }
        
        Output.WriteLine($"Retrieved {result.Data.Count} filtered people");
    }
    
    [Fact]
    public async Task StreamPeople_WithLargeDataset_ProcessesEfficiently()
    {
        // Arrange
        var processedCount = 0;
        var maxToProcess = 100; // Limit for test performance
        
        // Act
        await foreach (var person in PeopleService.StreamAsync())
        {
            Assert.NotNull(person.Id);
            processedCount++;
            
            if (processedCount >= maxToProcess)
                break;
        }
        
        // Assert
        Assert.True(processedCount > 0, "Should process at least one person");
        Output.WriteLine($"Streamed {processedCount} people efficiently");
    }
}
```

### 3. Fluent API Integration Tests

```csharp
public class FluentApiIntegrationTests : IntegrationTestBase
{
    public FluentApiIntegrationTests(ITestOutputHelper output) : base(output) { }
    
    [Fact]
    public async Task FluentQuery_WithComplexFiltering_ReturnsCorrectResults()
    {
        // Arrange
        var client = ServiceProvider.GetRequiredService<IPlanningCenterClient>();
        
        // Act
        var results = await client.People
            .Where(p => p.Status == "active")
            .OrderBy(p => p.LastName)
            .Take(5)
            .ToListAsync();
        
        // Assert
        Assert.NotNull(results);
        Assert.True(results.Count <= 5, "Should not exceed Take limit");
        
        if (results.Count > 1)
        {
            // Verify ordering
            for (int i = 1; i < results.Count; i++)
            {
                var current = results[i].LastName ?? "";
                var previous = results[i - 1].LastName ?? "";
                Assert.True(string.Compare(current, previous, StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }
        
        Output.WriteLine($"Fluent query returned {results.Count} results");
    }
    
    [Fact]
    public async Task FluentQuery_WithIncludes_LoadsRelatedData()
    {
        // Arrange
        var client = ServiceProvider.GetRequiredService<IPlanningCenterClient>();
        
        // Act
        var results = await client.People
            .Include(p => p.Emails)
            .Take(3)
            .ToListAsync();
        
        // Assert
        Assert.NotNull(results);
        Assert.True(results.Count > 0, "Should return at least one person");
        
        // Note: The actual loading of related data depends on the API response
        // This test verifies the query executes without error
        Output.WriteLine($"Query with includes returned {results.Count} results");
    }
}
```

## Test Data Management

### 1. Test Data Factory

```csharp
public static class TestDataFactory
{
    public static Person CreateTestPerson(string id = null)
    {
        return new Person
        {
            Id = id ?? Guid.NewGuid().ToString(),
            FirstName = "Test",
            LastName = "Person",
            Status = "active",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
    
    public static List<Person> CreateTestPeople(int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => CreateTestPerson($"test-person-{i}"))
            .ToList();
    }
    
    public static QueryParameters CreateTestQueryParameters()
    {
        return new QueryParameters
        {
            PerPage = 25,
            OrderBy = "last_name",
            Include = ["emails"]
        };
    }
}
```

### 2. Mock Data Builder

```csharp
public class MockDataBuilder
{
    private readonly List<Person> _people = new();
    
    public MockDataBuilder WithPerson(string firstName, string lastName)
    {
        _people.Add(new Person
        {
            Id = Guid.NewGuid().ToString(),
            FirstName = firstName,
            LastName = lastName,
            Status = "active"
        });
        return this;
    }
    
    public MockDataBuilder WithPeople(int count)
    {
        for (int i = 0; i < count; i++)
        {
            WithPerson($"FirstName{i}", $"LastName{i}");
        }
        return this;
    }
    
    public List<Person> Build() => _people.ToList();
    
    public PagedApiResponse<Person> BuildPagedResponse(int page = 1, int perPage = 25)
    {
        var skip = (page - 1) * perPage;
        var data = _people.Skip(skip).Take(perPage).ToList();
        
        return new PagedApiResponse<Person>
        {
            Data = data,
            Meta = new ApiResponseMeta
            {
                TotalCount = _people.Count,
                Count = data.Count,
                Prev = page > 1 ? $"/people?page={page - 1}&per_page={perPage}" : null,
                Next = skip + perPage < _people.Count ? $"/people?page={page + 1}&per_page={perPage}" : null
            }
        };
    }
}
```

## Performance Testing

### 1. Load Testing

```csharp
public class PerformanceTests : IntegrationTestBase
{
    public PerformanceTests(ITestOutputHelper output) : base(output) { }
    
    [Fact]
    public async Task ConcurrentRequests_ShouldHandleLoad()
    {
        // Arrange
        const int concurrentRequests = 10;
        const int requestsPerThread = 5;
        
        // Act
        var stopwatch = Stopwatch.StartNew();
        
        var tasks = Enumerable.Range(0, concurrentRequests)
            .Select(async _ =>
            {
                for (int i = 0; i < requestsPerThread; i++)
                {
                    await PeopleService.ListAsync(new QueryParameters { PerPage = 10 });
                }
            });
        
        await Task.WhenAll(tasks);
        stopwatch.Stop();
        
        // Assert
        var totalRequests = concurrentRequests * requestsPerThread;
        var requestsPerSecond = totalRequests / stopwatch.Elapsed.TotalSeconds;
        
        Output.WriteLine($"Completed {totalRequests} requests in {stopwatch.Elapsed.TotalSeconds:F2}s");
        Output.WriteLine($"Requests per second: {requestsPerSecond:F2}");
        
        Assert.True(requestsPerSecond > 1, "Should handle at least 1 request per second");
    }
    
    [Fact]
    public async Task StreamingVsBatch_MemoryComparison()
    {
        // Measure streaming memory usage
        var initialMemory = GC.GetTotalMemory(true);
        
        var streamCount = 0;
        await foreach (var person in PeopleService.StreamAsync())
        {
            streamCount++;
            if (streamCount >= 100) break;
        }
        
        var streamingMemory = GC.GetTotalMemory(true);
        var streamingIncrease = streamingMemory - initialMemory;
        
        Output.WriteLine($"Streaming processed {streamCount} items");
        Output.WriteLine($"Memory increase with streaming: {streamingIncrease / 1024:F2} KB");
        
        // Streaming should use minimal memory
        Assert.True(streamingIncrease < 5 * 1024 * 1024, "Streaming should use less than 5MB");
    }
}
```

## Test Configuration

### 1. Test Settings

```json
// appsettings.Test.json
{
  "PlanningCenter": {
    "BaseUrl": "https://api.planningcenteronline.com",
    "RequestTimeout": "00:00:30",
    "MaxRetryAttempts": 3,
    "EnableCaching": false
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "PlanningCenter": "Trace"
    }
  }
}
```

### 2. Environment Setup

```csharp
// TestEnvironment.cs
public static class TestEnvironment
{
    public static string GetTestToken()
    {
        return Environment.GetEnvironmentVariable("PLANNING_CENTER_TEST_TOKEN")
            ?? throw new InvalidOperationException(
                "PLANNING_CENTER_TEST_TOKEN environment variable is required for integration tests");
    }
    
    public static bool IsIntegrationTestEnabled()
    {
        return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PLANNING_CENTER_TEST_TOKEN"));
    }
    
    public static void SkipIfIntegrationTestsDisabled()
    {
        if (!IsIntegrationTestEnabled())
        {
            throw new SkipException("Integration tests are disabled. Set PLANNING_CENTER_TEST_TOKEN to enable.");
        }
    }
}
```

## Test Utilities

### 1. Assertion Helpers

```csharp
public static class PlanningCenterAssertions
{
    public static void AssertValidPerson(Person person)
    {
        Assert.NotNull(person);
        Assert.NotNull(person.Id);
        Assert.NotEmpty(person.Id);
        Assert.True(person.CreatedAt <= DateTime.UtcNow);
        Assert.True(person.UpdatedAt <= DateTime.UtcNow);
    }
    
    public static void AssertValidPagedResponse<T>(PagedApiResponse<T> response)
    {
        Assert.NotNull(response);
        Assert.NotNull(response.Data);
        Assert.NotNull(response.Meta);
        Assert.True(response.Meta.Count >= 0);
        Assert.Equal(response.Data.Count, response.Meta.Count);
    }
    
    public static void AssertOrderedBy<T, TKey>(IEnumerable<T> items, Func<T, TKey> keySelector)
        where TKey : IComparable<TKey>
    {
        var list = items.ToList();
        for (int i = 1; i < list.Count; i++)
        {
            var current = keySelector(list[i]);
            var previous = keySelector(list[i - 1]);
            Assert.True(current.CompareTo(previous) >= 0, 
                $"Items should be ordered. Item at index {i} is out of order.");
        }
    }
}
```

### 2. Test Categories

```csharp
// Use traits to categorize tests
[Trait("Category", "Unit")]
public class UnitTests { }

[Trait("Category", "Integration")]
public class IntegrationTests { }

[Trait("Category", "Performance")]
public class PerformanceTests { }

// Run specific categories:
// dotnet test --filter "Category=Unit"
// dotnet test --filter "Category=Integration"
```

## CI/CD Testing

### 1. GitHub Actions Example

```yaml
# .github/workflows/test.yml
name: Tests

on: [push, pull_request]

jobs:
  unit-tests:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    - name: Run Unit Tests
      run: dotnet test --filter "Category=Unit" --logger trx --results-directory TestResults
    - name: Upload Test Results
      uses: actions/upload-artifact@v3
      with:
        name: unit-test-results
        path: TestResults

  integration-tests:
    runs-on: ubuntu-latest
    if: github.event_name == 'push' && github.ref == 'refs/heads/main'
    steps:
    - uses: actions/checkout@v3
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    - name: Run Integration Tests
      run: dotnet test --filter "Category=Integration" --logger trx
      env:
        PLANNING_CENTER_TEST_TOKEN: ${{ secrets.PLANNING_CENTER_TEST_TOKEN }}
```

## Best Practices

### 1. Test Organization
- **Separate unit and integration tests**
- **Use descriptive test names**
- **Group related tests in classes**
- **Use test categories/traits**
- **Keep tests independent**

### 2. Mocking Guidelines
- **Mock external dependencies**
- **Verify important interactions**
- **Use realistic test data**
- **Don't over-mock**
- **Test error scenarios**

### 3. Integration Test Guidelines
- **Use test-specific tokens**
- **Clean up test data**
- **Handle rate limiting**
- **Test real scenarios**
- **Monitor test performance**

### 4. Performance Test Guidelines
- **Set realistic expectations**
- **Test under load**
- **Monitor memory usage**
- **Test timeout scenarios**
- **Measure key metrics**

---

*Last Updated: 2024-11-20*