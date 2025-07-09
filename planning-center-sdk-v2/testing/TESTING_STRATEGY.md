# Planning Center SDK Testing Strategy

## Overview

This document outlines the comprehensive testing strategy for the Planning Center .NET SDK, ensuring high quality, reliability, and maintainability across all modules and features.

## Testing Pyramid

### Unit Tests (90% coverage target)
- **Scope:** Individual classes and methods
- **Focus:** Business logic, mapping, validation
- **Tools:** xUnit, Moq, FluentAssertions
- **Location:** `PlanningCenter.Api.Client.Tests`

### Integration Tests (Key scenarios)
- **Scope:** API communication, authentication, caching
- **Focus:** Real API interactions, end-to-end workflows
- **Tools:** xUnit, TestContainers, WebApplicationFactory
- **Location:** `PlanningCenter.Api.Client.IntegrationTests`

### Performance Tests (Benchmarks)
- **Scope:** Response times, memory usage, throughput
- **Focus:** Scalability and efficiency
- **Tools:** BenchmarkDotNet, NBomber
- **Location:** `PlanningCenter.Api.Client.PerformanceTests`

## Unit Testing Strategy

### Test Organization
```
PlanningCenter.Api.Client.Tests/
├── Services/
│   ├── PeopleServiceTests.cs
│   ├── GivingServiceTests.cs
│   ├── CalendarServiceTests.cs
│   └── ...
├── Mapping/
│   ├── PersonMapperTests.cs
│   ├── DonationMapperTests.cs
│   └── ...
├── Http/
│   ├── ApiConnectionTests.cs
│   ├── ApiExceptionFactoryTests.cs
│   └── ...
├── Auth/
│   ├── OAuthAuthenticatorTests.cs
│   ├── AuthHandlerTests.cs
│   └── ...
├── Fluent/
│   ├── PeopleFluentContextTests.cs
│   ├── GivingFluentContextTests.cs
│   └── ...
├── Caching/
│   ├── InMemoryCacheProviderTests.cs
│   ├── DistributedCacheProviderTests.cs
│   └── ...
└── Webhooks/
    ├── WebhookValidatorTests.cs
    └── WebhookEventHandlerTests.cs
```

### Pagination Testing Example
```csharp
public class PaginationTests
{
    private readonly Mock<IApiConnection> _mockApiConnection;
    private readonly PeopleService _peopleService;
    
    public PaginationTests()
    {
        _mockApiConnection = new Mock<IApiConnection>();
        _peopleService = new PeopleService(_mockApiConnection.Object, new PersonMapper(), Mock.Of<ILogger<PeopleService>>());
    }
    
    [Fact]
    public async Task GetAllAsync_ShouldFetchAllPages_WhenMultiplePagesExist()
    {
        // Arrange
        var page1Response = CreatePagedResponse(1, 2, 100, CreatePeople(50));
        var page2Response = CreatePagedResponse(2, 2, 100, CreatePeople(50));
        
        _mockApiConnection.SetupSequence(x => x.GetPagedAsync<People.PersonDto>(
                It.IsAny<string>(), It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(page1Response)
            .ReturnsAsync(page2Response);
        
        // Act
        var result = await _peopleService.GetAllAsync();
        
        // Assert
        result.Should().HaveCount(100);
        _mockApiConnection.Verify(x => x.GetPagedAsync<People.PersonDto>(
            It.IsAny<string>(), It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()), 
            Times.Exactly(2));
    }
    
    [Fact]
    public async Task StreamAsync_ShouldYieldItemsFromAllPages()
    {
        // Arrange
        var page1Response = CreatePagedResponse(1, 2, 4, CreatePeople(2));
        var page2Response = CreatePagedResponse(2, 2, 4, CreatePeople(2));
        
        _mockApiConnection.SetupSequence(x => x.GetPagedAsync<People.PersonDto>(
                It.IsAny<string>(), It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(page1Response)
            .ReturnsAsync(page2Response);
        
        // Act
        var people = new List<Core.Person>();
        await foreach (var person in _peopleService.StreamAsync())
        {
            people.Add(person);
        }
        
        // Assert
        people.Should().HaveCount(4);
    }
    
    [Fact]
    public async Task GetPagedAsync_ShouldRespectPaginationOptions()
    {
        // Arrange
        var options = new PaginationOptions
        {
            PageSize = 25,
            MaxItems = 50
        };
        
        var response = CreatePagedResponse(1, 1, 25, CreatePeople(25));
        _mockApiConnection.Setup(x => x.GetPagedAsync<People.PersonDto>(
                It.IsAny<string>(), 
                It.Is<QueryParameters>(q => q.PerPage == 25), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);
        
        // Act
        var result = await _peopleService.GetAllAsync(options: options);
        
        // Assert
        result.Should().HaveCount(25);
        _mockApiConnection.Verify(x => x.GetPagedAsync<People.PersonDto>(
            It.IsAny<string>(), 
            It.Is<QueryParameters>(q => q.PerPage == 25), 
            It.IsAny<CancellationToken>()), 
            Times.Once);
    }
    
    private static IPagedResponse<People.PersonDto> CreatePagedResponse(int currentPage, int totalPages, int totalCount, IReadOnlyList<People.PersonDto> data)
    {
        return new PagedResponse<People.PersonDto>
        {
            Data = data,
            Meta = new PagedResponseMeta
            {
                CurrentPage = currentPage,
                TotalPages = totalPages,
                TotalCount = totalCount,
                PerPage = data.Count,
                Count = data.Count
            },
            Links = new PagedResponseLinks
            {
                Self = $"/people/v2/people?page={currentPage}",
                Next = currentPage < totalPages ? $"/people/v2/people?page={currentPage + 1}" : null,
                Previous = currentPage > 1 ? $"/people/v2/people?page={currentPage - 1}" : null
            }
        };
    }
    
    private static IReadOnlyList<People.PersonDto> CreatePeople(int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => new People.PersonDto
            {
                Id = i.ToString(),
                FirstName = $"Person{i}",
                LastName = "Test",
                Status = "active"
            })
            .ToList()
            .AsReadOnly();
    }
}
```

### Service Testing Example
```csharp
public class PeopleServiceTests
{
    private readonly Mock<IApiConnection> _mockApiConnection;
    private readonly Mock<PersonMapper> _mockPersonMapper;
    private readonly Mock<ICacheProvider> _mockCacheProvider;
    private readonly PeopleService _peopleService;
    
    public PeopleServiceTests()
    {
        _mockApiConnection = new Mock<IApiConnection>();
        _mockPersonMapper = new Mock<PersonMapper>();
        _mockCacheProvider = new Mock<ICacheProvider>();
        _peopleService = new PeopleService(
            _mockApiConnection.Object,
            _mockPersonMapper.Object,
            _mockCacheProvider.Object);
    }
    
    [Fact]
    public async Task GetAsync_WithValidId_ReturnsPerson()
    {
        // Arrange
        var personId = "123";
        var personDto = new People.PersonDto { Id = personId, FirstName = "John", LastName = "Doe" };
        var expectedPerson = new Core.Person { Id = personId, FirstName = "John", LastName = "Doe" };
        
        _mockApiConnection
            .Setup(x => x.GetAsync<People.PersonDto>($"/people/v2/people/{personId}", It.IsAny<CancellationToken>()))
            .ReturnsAsync(personDto);
            
        _mockPersonMapper
            .Setup(x => x.Map(personDto))
            .Returns(expectedPerson);
        
        // Act
        var result = await _peopleService.GetAsync(personId);
        
        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(personId);
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
        
        _mockApiConnection.Verify(x => x.GetAsync<People.PersonDto>($"/people/v2/people/{personId}", It.IsAny<CancellationToken>()), Times.Once);
        _mockPersonMapper.Verify(x => x.Map(personDto), Times.Once);
    }
    
    [Fact]
    public async Task GetAsync_WithInvalidId_ThrowsNotFoundException()
    {
        // Arrange
        var personId = "invalid";
        
        _mockApiConnection
            .Setup(x => x.GetAsync<People.PersonDto>($"/people/v2/people/{personId}", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new PlanningCenterApiNotFoundException("Person not found"));
        
        // Act & Assert
        await _peopleService.Invoking(x => x.GetAsync(personId))
            .Should().ThrowAsync<PlanningCenterApiNotFoundException>()
            .WithMessage("Person not found");
    }
    
    [Fact]
    public async Task ListAsync_WithParameters_ReturnsPagedResponse()
    {
        // Arrange
        var parameters = new QueryParameters
        {
            Where = new Dictionary<string, object> { ["status"] = "active" },
            OrderBy = "last_name",
            PerPage = 25
        };
        
        var dtoResponse = new PagedResponse<People.PersonDto>
        {
            Data = new List<People.PersonDto>
            {
                new() { Id = "1", FirstName = "John", LastName = "Doe" },
                new() { Id = "2", FirstName = "Jane", LastName = "Smith" }
            },
            Meta = new PageMeta { TotalCount = 2, Count = 2 }
        };
        
        var expectedPeople = new List<Core.Person>
        {
            new() { Id = "1", FirstName = "John", LastName = "Doe" },
            new() { Id = "2", FirstName = "Jane", LastName = "Smith" }
        };
        
        _mockApiConnection
            .Setup(x => x.GetPagedAsync<People.PersonDto>("/people/v2/people", parameters, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dtoResponse);
            
        _mockPersonMapper
            .Setup(x => x.Map(It.IsAny<List<People.PersonDto>>()))
            .Returns(expectedPeople);
        
        // Act
        var result = await _peopleService.ListAsync(parameters);
        
        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Meta.TotalCount.Should().Be(2);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public async Task GetAsync_WithInvalidId_ThrowsArgumentException(string invalidId)
    {
        // Act & Assert
        await _peopleService.Invoking(x => x.GetAsync(invalidId))
            .Should().ThrowAsync<ArgumentException>();
    }
}
```

### Mapping Testing Example
```csharp
public class PersonMapperTests
{
    private readonly PersonMapper _mapper;
    
    public PersonMapperTests()
    {
        _mapper = new PersonMapper();
    }
    
    [Fact]
    public void Map_PeoplePersonDto_MapsAllProperties()
    {
        // Arrange
        var dto = new People.PersonDto
        {
            Id = "123",
            FirstName = "John",
            LastName = "Doe",
            Birthdate = new DateTime(1990, 1, 1),
            Gender = "Male",
            Status = "active",
            EmailAddresses = new List<People.EmailDto>
            {
                new() { Address = "john@example.com", Location = "Home", Primary = true }
            },
            Addresses = new List<People.AddressDto>
            {
                new() { Street = "123 Main St", City = "Anytown", State = "CA", Zip = "12345", Primary = true }
            },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        // Act
        var result = _mapper.Map(dto);
        
        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("123");
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
        result.FullName.Should().Be("John Doe");
        result.Birthdate.Should().Be(new DateTime(1990, 1, 1));
        result.Gender.Should().Be("Male");
        result.Status.Should().Be("active");
        result.PrimaryEmail.Should().Be("john@example.com");
        result.Emails.Should().HaveCount(1);
        result.Addresses.Should().HaveCount(1);
        result.SourceModule.Should().Be("People");
    }
    
    [Fact]
    public void Map_GivingPersonDto_MapsGivingSpecificProperties()
    {
        // Arrange
        var dto = new Giving.PersonDto
        {
            Id = "123",
            FirstName = "John",
            LastName = "Doe",
            TotalGiven = 150000, // $1,500.00 in cents
            LastGiftDate = new DateTime(2024, 1, 15),
            AverageGiftAmount = 5000 // $50.00 in cents
        };
        
        // Act
        var result = _mapper.Map(dto);
        
        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("123");
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
        result.TotalGiven.Should().Be(1500.00m);
        result.LastGiftDate.Should().Be(new DateTime(2024, 1, 15));
        result.AverageGiftAmount.Should().Be(50.00m);
        result.SourceModule.Should().Be("Giving");
    }
    
    [Fact]
    public void Map_NullDto_ThrowsArgumentNullException()
    {
        // Act & Assert
        _mapper.Invoking(x => x.Map((People.PersonDto)null))
            .Should().Throw<ArgumentNullException>();
    }
}
```

### HTTP Client Testing Example
```csharp
public class ApiConnectionTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpHandler;
    private readonly HttpClient _httpClient;
    private readonly Mock<ILogger<ApiConnection>> _mockLogger;
    private readonly ApiConnection _apiConnection;
    
    public ApiConnectionTests()
    {
        _mockHttpHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpHandler.Object)
        {
            BaseAddress = new Uri("https://api.planningcenteronline.com")
        };
        _mockLogger = new Mock<ILogger<ApiConnection>>();
        _apiConnection = new ApiConnection(_httpClient, _mockLogger.Object);
    }
    
    [Fact]
    public async Task GetAsync_SuccessfulResponse_ReturnsDeserializedObject()
    {
        // Arrange
        var expectedPerson = new People.PersonDto { Id = "123", FirstName = "John" };
        var jsonResponse = JsonSerializer.Serialize(expectedPerson);
        
        _mockHttpHandler
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString().Contains("/people/v2/people/123")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            });
        
        // Act
        var result = await _apiConnection.GetAsync<People.PersonDto>("/people/v2/people/123");
        
        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("123");
        result.FirstName.Should().Be("John");
    }
    
    [Fact]
    public async Task GetAsync_NotFoundResponse_ThrowsNotFoundException()
    {
        // Arrange
        _mockHttpHandler
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent("{\"error\":\"Not found\"}", Encoding.UTF8, "application/json")
            });
        
        // Act & Assert
        await _apiConnection.Invoking(x => x.GetAsync<People.PersonDto>("/people/v2/people/invalid"))
            .Should().ThrowAsync<PlanningCenterApiNotFoundException>();
    }
    
    [Fact]
    public async Task GetAsync_RateLimitResponse_ThrowsRateLimitException()
    {
        // Arrange
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.TooManyRequests,
            Content = new StringContent("{\"error\":\"Rate limit exceeded\"}", Encoding.UTF8, "application/json")
        };
        response.Headers.Add("Retry-After", "60");
        
        _mockHttpHandler
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
        
        // Act & Assert
        var exception = await _apiConnection.Invoking(x => x.GetAsync<People.PersonDto>("/people/v2/people/123"))
            .Should().ThrowAsync<PlanningCenterApiRateLimitException>();
            
        exception.Which.RetryAfter.Should().Be(TimeSpan.FromSeconds(60));
    }
}
```

## Integration Testing Strategy

### Test Configuration
```csharp
public class IntegrationTestFixture : IDisposable
{
    public IServiceProvider ServiceProvider { get; private set; }
    public IPlanningCenterClient Client { get; private set; }
    
    public IntegrationTestFixture()
    {
        var services = new ServiceCollection();
        
        services.AddPlanningCenterApiClient(options =>
        {
            options.ClientId = Environment.GetEnvironmentVariable("PCO_CLIENT_ID") ?? "test-client-id";
            options.ClientSecret = Environment.GetEnvironmentVariable("PCO_CLIENT_SECRET") ?? "test-client-secret";
            options.BaseUrl = Environment.GetEnvironmentVariable("PCO_BASE_URL") ?? "https://api.planningcenteronline.com";
        });
        
        services.AddLogging(builder => builder.AddConsole());
        
        ServiceProvider = services.BuildServiceProvider();
        Client = ServiceProvider.GetRequiredService<IPlanningCenterClient>();
    }
    
    public void Dispose()
    {
        ServiceProvider?.Dispose();
    }
}

[Collection("Integration Tests")]
public class PeopleIntegrationTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;
    private readonly IPeopleService _peopleService;
    
    public PeopleIntegrationTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _peopleService = _fixture.ServiceProvider.GetRequiredService<IPeopleService>();
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAsync_WithRealApi_ReturnsValidPerson()
    {
        // This test requires a real person ID from your test environment
        var testPersonId = Environment.GetEnvironmentVariable("TEST_PERSON_ID");
        
        if (string.IsNullOrEmpty(testPersonId))
        {
            // Skip test if no test person ID is configured
            return;
        }
        
        // Act
        var person = await _peopleService.GetAsync(testPersonId);
        
        // Assert
        person.Should().NotBeNull();
        person.Id.Should().Be(testPersonId);
        person.FirstName.Should().NotBeNullOrEmpty();
        person.LastName.Should().NotBeNullOrEmpty();
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public async Task ListAsync_WithRealApi_ReturnsPagedResults()
    {
        // Act
        var people = await _peopleService.ListAsync(new QueryParameters
        {
            PerPage = 5,
            OrderBy = "created_at"
        });
        
        // Assert
        people.Should().NotBeNull();
        people.Data.Should().NotBeEmpty();
        people.Data.Should().HaveCountLessOrEqualTo(5);
        people.Meta.Should().NotBeNull();
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public async Task CreateUpdateDelete_WithRealApi_WorksEndToEnd()
    {
        // Arrange
        var createRequest = new PersonCreateRequest
        {
            FirstName = "Test",
            LastName = $"Person-{Guid.NewGuid():N}",
            Email = $"test-{Guid.NewGuid():N}@example.com"
        };
        
        Core.Person createdPerson = null;
        
        try
        {
            // Act - Create
            createdPerson = await _peopleService.CreateAsync(createRequest);
            
            // Assert - Create
            createdPerson.Should().NotBeNull();
            createdPerson.Id.Should().NotBeNullOrEmpty();
            createdPerson.FirstName.Should().Be("Test");
            
            // Act - Update
            var updateRequest = new PersonUpdateRequest
            {
                FirstName = "Updated Test"
            };
            
            var updatedPerson = await _peopleService.UpdateAsync(createdPerson.Id, updateRequest);
            
            // Assert - Update
            updatedPerson.FirstName.Should().Be("Updated Test");
            updatedPerson.LastName.Should().Be(createdPerson.LastName);
        }
        finally
        {
            // Cleanup - Delete
            if (createdPerson != null)
            {
                await _peopleService.DeleteAsync(createdPerson.Id);
            }
        }
    }
}
```

### Authentication Integration Tests
```csharp
public class AuthenticationIntegrationTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IAuthenticator _authenticator;
    
    public AuthenticationIntegrationTests(IntegrationTestFixture fixture)
    {
        _authenticator = fixture.ServiceProvider.GetRequiredService<IAuthenticator>();
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAccessTokenAsync_WithValidCredentials_ReturnsToken()
    {
        // Act
        var token = await _authenticator.GetAccessTokenAsync();
        
        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Should().StartWith("Bearer ");
    }
    
    [Fact]
    [Trait("Category", "Integration")]
    public async Task ValidateTokenAsync_WithValidToken_ReturnsTrue()
    {
        // Arrange
        var token = await _authenticator.GetAccessTokenAsync();
        
        // Act
        var isValid = await _authenticator.ValidateTokenAsync(token);
        
        // Assert
        isValid.Should().BeTrue();
    }
}
```

## Performance Testing Strategy

### Benchmark Tests
```csharp
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net90)]
public class PeopleServiceBenchmarks
{
    private IPeopleService _peopleService;
    private string _testPersonId;
    
    [GlobalSetup]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddPlanningCenterApiClient(options =>
        {
            options.ClientId = Environment.GetEnvironmentVariable("PCO_CLIENT_ID");
            options.ClientSecret = Environment.GetEnvironmentVariable("PCO_CLIENT_SECRET");
        });
        
        var serviceProvider = services.BuildServiceProvider();
        _peopleService = serviceProvider.GetRequiredService<IPeopleService>();
        _testPersonId = Environment.GetEnvironmentVariable("TEST_PERSON_ID");
    }
    
    [Benchmark]
    public async Task<Core.Person> GetPersonAsync()
    {
        return await _peopleService.GetAsync(_testPersonId);
    }
    
    [Benchmark]
    public async Task<IPagedResponse<Core.Person>> ListPeopleAsync()
    {
        return await _peopleService.ListAsync(new QueryParameters { PerPage = 25 });
    }
    
    [Benchmark]
    public async Task<IPagedResponse<Core.Person>> ListPeopleWithIncludesAsync()
    {
        return await _peopleService.ListAsync(new QueryParameters
        {
            PerPage = 25,
            Include = new[] { "addresses", "emails", "phone_numbers" }
        });
    }
}
```

### Load Testing
```csharp
public class LoadTests
{
    [Fact]
    [Trait("Category", "Load")]
    public async Task ConcurrentRequests_DoNotExceedRateLimit()
    {
        // Arrange
        var fixture = new IntegrationTestFixture();
        var peopleService = fixture.ServiceProvider.GetRequiredService<IPeopleService>();
        var testPersonId = Environment.GetEnvironmentVariable("TEST_PERSON_ID");
        
        // Act
        var tasks = Enumerable.Range(0, 50)
            .Select(_ => peopleService.GetAsync(testPersonId))
            .ToArray();
        
        var results = await Task.WhenAll(tasks);
        
        // Assert
        results.Should().AllSatisfy(person => person.Should().NotBeNull());
    }
}
```

## Test Data Management

### Test Data Factory
```csharp
public static class TestDataFactory
{
    public static People.PersonDto CreatePersonDto(string id = null)
    {
        return new People.PersonDto
        {
            Id = id ?? Guid.NewGuid().ToString(),
            FirstName = "Test",
            LastName = "Person",
            Status = "active",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            EmailAddresses = new List<People.EmailDto>
            {
                new() { Address = "test@example.com", Location = "Home", Primary = true }
            }
        };
    }
    
    public static Core.Person CreateCorePerson(string id = null)
    {
        return new Core.Person
        {
            Id = id ?? Guid.NewGuid().ToString(),
            FirstName = "Test",
            LastName = "Person",
            PrimaryEmail = "test@example.com",
            Status = "active",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
    
    public static PersonCreateRequest CreatePersonCreateRequest()
    {
        return new PersonCreateRequest
        {
            FirstName = "Test",
            LastName = $"Person-{Guid.NewGuid():N}",
            Email = $"test-{Guid.NewGuid():N}@example.com"
        };
    }
}
```

## Continuous Integration

### Test Execution Pipeline
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
        dotnet-version: '9.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Run unit tests
      run: dotnet test tests/PlanningCenter.Api.Client.Tests --configuration Release --logger trx --collect:"XPlat Code Coverage"
    
    - name: Upload coverage to Codecov
      uses: codecov/codecov-action@v3
  
  integration-tests:
    runs-on: ubuntu-latest
    if: github.event_name == 'push' && github.ref == 'refs/heads/main'
    steps:
    - uses: actions/checkout@v3
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '9.0.x'
    
    - name: Run integration tests
      run: dotnet test tests/PlanningCenter.Api.Client.IntegrationTests --configuration Release
      env:
        PCO_CLIENT_ID: ${{ secrets.PCO_CLIENT_ID }}
        PCO_CLIENT_SECRET: ${{ secrets.PCO_CLIENT_SECRET }}
        TEST_PERSON_ID: ${{ secrets.TEST_PERSON_ID }}
```

## Quality Gates

### Coverage Requirements
- **Unit Tests:** 90% line coverage minimum
- **Integration Tests:** All critical paths covered
- **Performance Tests:** Response time < 200ms average

### Test Categories
- **Unit:** Fast, isolated, no external dependencies
- **Integration:** Real API calls, requires credentials
- **Load:** Performance and scalability testing
- **Smoke:** Basic functionality verification

### Reporting
- Code coverage reports generated automatically
- Performance benchmarks tracked over time
- Test results published to CI/CD dashboard
- Failed tests trigger immediate notifications

This comprehensive testing strategy ensures the Planning Center SDK maintains high quality and reliability across all modules and usage scenarios.