using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.Exceptions;
using PlanningCenter.Api.Client.Models.People;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Services;
using PlanningCenter.Api.Client.Tests.Utilities;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Services;

/// <summary>
/// Comprehensive unit tests for the PeopleService class.
/// Tests all CRUD operations, error handling, and integration with IApiConnection.
/// </summary>
public class PeopleServiceTests
{
    private readonly MockApiConnection _mockApiConnection;
    private readonly ILogger<PeopleService> _logger;
    private readonly PeopleService _peopleService;
    private readonly TestDataBuilder _testDataBuilder;

    public PeopleServiceTests()
    {
        _mockApiConnection = new MockApiConnection();
        _logger = NullLogger<PeopleService>.Instance;
        _peopleService = new PeopleService(_mockApiConnection.Object, _logger);
        _testDataBuilder = new TestDataBuilder();
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenApiConnectionIsNull()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => 
            new PeopleService(null!, _logger));
        
        exception.ParamName.Should().Be("apiConnection");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenLoggerIsNull()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => 
            new PeopleService(_mockApiConnection.Object, null!));
        
        exception.ParamName.Should().Be("logger");
    }

    [Fact]
    public void Constructor_ShouldCreateInstance_WhenParametersAreValid()
    {
        // Arrange & Act
        var service = new PeopleService(_mockApiConnection.Object, _logger);

        // Assert
        service.Should().NotBeNull();
    }

    #endregion

    #region GetMeAsync Tests

    [Fact]
    public async Task GetMeAsync_ShouldReturnCurrentUser_WhenApiReturnsValidData()
    {
        // Arrange
        var personDto = _testDataBuilder.CreatePersonDto();
        var response = new JsonApiSingleResponse<PersonDto> { Data = personDto };
        
        _mockApiConnection.SetupGetResponse("/people/v2/me", response);

        // Act
        var result = await _peopleService.GetMeAsync();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(personDto.Id);
        result.FirstName.Should().Be(personDto.Attributes.FirstName);
        result.LastName.Should().Be(personDto.Attributes.LastName);
        
        _mockApiConnection.VerifyGetRequest("/people/v2/me", Times.Once());
    }

    [Fact]
    public async Task GetMeAsync_ShouldThrowException_WhenApiReturnsNullResponse()
    {
        // Arrange
        _mockApiConnection.SetupGetResponse<JsonApiSingleResponse<PersonDto>>("/people/v2/me", null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PlanningCenterApiGeneralException>(
            () => _peopleService.GetMeAsync());
        
        exception.Message.Should().Contain("Failed to get current user - no data returned");
    }

    [Fact]
    public async Task GetMeAsync_ShouldThrowException_WhenApiReturnsNullData()
    {
        // Arrange
        var response = new JsonApiSingleResponse<PersonDto> { Data = null! };
        _mockApiConnection.SetupGetResponse("/people/v2/me", response);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PlanningCenterApiGeneralException>(
            () => _peopleService.GetMeAsync());
        
        exception.Message.Should().Contain("Failed to get current user - no data returned");
    }

    [Fact]
    public async Task GetMeAsync_ShouldPassCancellationToken_WhenProvided()
    {
        // Arrange
        var personDto = _testDataBuilder.CreatePersonDto();
        var response = new JsonApiSingleResponse<PersonDto> { Data = personDto };
        var cancellationToken = new CancellationToken(true);
        
        _mockApiConnection.SetupGetResponse("/people/v2/me", response);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _peopleService.GetMeAsync(cancellationToken));
    }

    #endregion

    #region GetAsync Tests

    [Fact]
    public async Task GetAsync_ShouldReturnPerson_WhenPersonExists()
    {
        // Arrange
        var personId = "123";
        var personDto = _testDataBuilder.CreatePersonDto(p => p.Id = personId);
        var response = new JsonApiSingleResponse<PersonDto> { Data = personDto };
        
        _mockApiConnection.SetupGetResponse($"/people/v2/people/{personId}", response);

        // Act
        var result = await _peopleService.GetAsync(personId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(personId);
        result.FirstName.Should().Be(personDto.Attributes.FirstName);
        
        _mockApiConnection.VerifyGetRequest($"/people/v2/people/{personId}", Times.Once());
    }

    [Fact]
    public async Task GetAsync_ShouldReturnNull_WhenPersonNotFound()
    {
        // Arrange
        var personId = "999";
        _mockApiConnection.SetupException($"/people/v2/people/{personId}", 
            new PlanningCenterApiNotFoundException("Person not found"));

        // Act
        var result = await _peopleService.GetAsync(personId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_ShouldReturnNull_WhenApiReturnsNullData()
    {
        // Arrange
        var personId = "123";
        var response = new JsonApiSingleResponse<PersonDto> { Data = null! };
        _mockApiConnection.SetupGetResponse($"/people/v2/people/{personId}", response);

        // Act
        var result = await _peopleService.GetAsync(personId);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetAsync_ShouldThrowArgumentException_WhenIdIsInvalid(string invalidId)
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _peopleService.GetAsync(invalidId));
        
        exception.ParamName.Should().Be("id");
        exception.Message.Should().Contain("Person ID cannot be null or empty");
    }

    #endregion

    #region ListAsync Tests

    [Fact]
    public async Task ListAsync_ShouldReturnPagedResponse_WhenApiReturnsData()
    {
        // Arrange
        var people = _testDataBuilder.CreatePeople(3);
        var peopleDto = people.Select(p => _testDataBuilder.CreatePersonDto(dto => 
        {
            dto.Id = p.Id;
            dto.Attributes.FirstName = p.FirstName;
            dto.Attributes.LastName = p.LastName;
        })).ToList();
        
        var pagedResponse = _testDataBuilder.CreatePagedResponse(peopleDto, 1, 25, 3);
        _mockApiConnection.SetupPagedResponse("/people/v2/people", pagedResponse);

        // Act
        var result = await _peopleService.ListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(3);
        result.Data.First().Id.Should().Be(people.First().Id);
        
        _mockApiConnection.VerifyGetRequest("/people/v2/people", Times.Once());
    }

    [Fact]
    public async Task ListAsync_ShouldPassQueryParameters_WhenProvided()
    {
        // Arrange
        var parameters = new QueryParameters();
        parameters.Where["status"] = "active";
        parameters.Include = new[] { "addresses", "emails" };
        parameters.PerPage = 50;
        
        var pagedResponse = _testDataBuilder.CreatePagedResponse<PersonDto>(new List<PersonDto>(), 1, 50, 0);
        _mockApiConnection.SetupPagedResponse("/people/v2/people", pagedResponse);

        // Act
        var result = await _peopleService.ListAsync(parameters);

        // Assert
        result.Should().NotBeNull();
        _mockApiConnection.VerifyGetRequest("/people/v2/people", Times.Once());
    }

    [Fact]
    public async Task ListAsync_ShouldHandleEmptyResults_WhenNoDataReturned()
    {
        // Arrange
        var pagedResponse = _testDataBuilder.CreatePagedResponse<PersonDto>(new List<PersonDto>(), 1, 25, 0);
        _mockApiConnection.SetupPagedResponse("/people/v2/people", pagedResponse);

        // Act
        var result = await _peopleService.ListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().BeEmpty();
        result.Meta!.TotalCount.Should().Be(0);
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_ShouldReturnCreatedPerson_WhenRequestIsValid()
    {
        // Arrange
        var request = new PersonCreateRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };
        
        var createdPersonDto = _testDataBuilder.CreatePersonDto(p =>
        {
            p.Id = "new-person-id";
            p.Attributes.FirstName = request.FirstName;
            p.Attributes.LastName = request.LastName;
        });
        
        var response = new JsonApiSingleResponse<PersonDto> { Data = createdPersonDto };
        _mockApiConnection.SetupPostResponse("/people/v2/people", response);

        // Act
        var result = await _peopleService.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("new-person-id");
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
        
        _mockApiConnection.VerifyPostRequest("/people/v2/people", Times.Once());
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => _peopleService.CreateAsync(null!));
        
        exception.ParamName.Should().Be("request");
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowException_WhenApiReturnsNullData()
    {
        // Arrange
        var request = new PersonCreateRequest { FirstName = "John", LastName = "Doe" };
        var response = new JsonApiSingleResponse<PersonDto> { Data = null! };
        _mockApiConnection.SetupPostResponse("/people/v2/people", response);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PlanningCenterApiGeneralException>(
            () => _peopleService.CreateAsync(request));
        
        exception.Message.Should().Contain("Failed to create person - no data returned");
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ShouldReturnUpdatedPerson_WhenRequestIsValid()
    {
        // Arrange
        var personId = "123";
        var request = new PersonUpdateRequest
        {
            FirstName = "Jane",
            LastName = "Smith"
        };
        
        var updatedPersonDto = _testDataBuilder.CreatePersonDto(p =>
        {
            p.Id = personId;
            p.Attributes.FirstName = request.FirstName;
            p.Attributes.LastName = request.LastName;
        });
        
        var response = new JsonApiSingleResponse<PersonDto> { Data = updatedPersonDto };
        _mockApiConnection.SetupPatchResponse($"/people/v2/people/{personId}", response);

        // Act
        var result = await _peopleService.UpdateAsync(personId, request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(personId);
        result.FirstName.Should().Be("Jane");
        result.LastName.Should().Be("Smith");
        
        _mockApiConnection.VerifyPatchRequest($"/people/v2/people/{personId}", Times.Once());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task UpdateAsync_ShouldThrowArgumentException_WhenIdIsInvalid(string invalidId)
    {
        // Arrange
        var request = new PersonUpdateRequest { FirstName = "Jane" };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _peopleService.UpdateAsync(invalidId, request));
        
        exception.ParamName.Should().Be("id");
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => _peopleService.UpdateAsync("123", null!));
        
        exception.ParamName.Should().Be("request");
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_ShouldCompleteSuccessfully_WhenPersonExists()
    {
        // Arrange
        var personId = "123";
        _mockApiConnection.SetupDeleteResponse($"/people/v2/people/{personId}");

        // Act
        await _peopleService.DeleteAsync(personId);

        // Assert
        _mockApiConnection.VerifyDeleteRequest($"/people/v2/people/{personId}", Times.Once());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task DeleteAsync_ShouldThrowArgumentException_WhenIdIsInvalid(string invalidId)
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _peopleService.DeleteAsync(invalidId));
        
        exception.ParamName.Should().Be("id");
    }

    [Fact]
    public async Task DeleteAsync_ShouldPropagateException_WhenApiThrowsException()
    {
        // Arrange
        var personId = "123";
        var apiException = new PlanningCenterApiNotFoundException("Person not found");
        _mockApiConnection.SetupException($"/people/v2/people/{personId}", apiException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(
            () => _peopleService.DeleteAsync(personId));
        
        exception.Should().BeSameAs(apiException);
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPeople_WhenMultiplePagesExist()
    {
        // Arrange
        var firstPagePeople = _testDataBuilder.CreatePeople(2);
        var secondPagePeople = _testDataBuilder.CreatePeople(2);
        var allPeople = firstPagePeople.Concat(secondPagePeople).ToList();
        
        // Convert to DTOs
        var firstPageDto = firstPagePeople.Select(p => _testDataBuilder.CreatePersonDto(dto => 
        {
            dto.Id = p.Id;
            dto.Attributes.FirstName = p.FirstName;
            dto.Attributes.LastName = p.LastName;
        })).ToList();
        
        var secondPageDto = secondPagePeople.Select(p => _testDataBuilder.CreatePersonDto(dto => 
        {
            dto.Id = p.Id;
            dto.Attributes.FirstName = p.FirstName;
            dto.Attributes.LastName = p.LastName;
        })).ToList();
        
        // Create first page response with next page link
        var firstPageResponse = _testDataBuilder.CreatePagedResponse(firstPageDto, 1, 2, 4);
        firstPageResponse.Links!.Next = "/people/v2/people?page=2";
        
        // Create second page response (last page)
        var secondPageResponse = _testDataBuilder.CreatePagedResponse(secondPageDto, 2, 2, 4);
        secondPageResponse.Links!.Next = null;
        
        // Set up the mock to return different responses for different calls
        var callCount = 0;
        _mockApiConnection.Object
            .Setup(x => x.GetPagedAsync<PersonDto>("/people/v2/people", It.IsAny<QueryParameters?>(), It.IsAny<CancellationToken>()))
            .Returns(() =>
            {
                callCount++;
                return callCount == 1 ? Task.FromResult(firstPageResponse) : Task.FromResult(secondPageResponse);
            });

        // Act
        var result = await _peopleService.GetAllAsync();

        // Assert
        result.Should().HaveCount(4);
        result.Should().Contain(p => firstPagePeople.Any(fp => fp.Id == p.Id));
        result.Should().Contain(p => secondPagePeople.Any(sp => sp.Id == p.Id));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnSinglePage_WhenOnlyOnePageExists()
    {
        // Arrange
        var people = _testDataBuilder.CreatePeople(3);
        var peopleDto = people.Select(p => _testDataBuilder.CreatePersonDto(dto => 
        {
            dto.Id = p.Id;
            dto.Attributes.FirstName = p.FirstName;
            dto.Attributes.LastName = p.LastName;
        })).ToList();
        
        var pagedResponse = _testDataBuilder.CreatePagedResponse(peopleDto, 1, 25, 3);
        pagedResponse.Links!.Next = null; // No next page
        
        _mockApiConnection.SetupPagedResponse("/people/v2/people", pagedResponse);

        // Act
        var result = await _peopleService.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(p => people.Any(pp => pp.Id == p.Id));
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public async Task Service_ShouldPropagateAuthenticationException_WhenApiThrowsAuthError()
    {
        // Arrange
        var authException = new PlanningCenterApiAuthenticationException("Invalid credentials");
        _mockApiConnection.SetupException("/people/v2/me", authException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PlanningCenterApiAuthenticationException>(
            () => _peopleService.GetMeAsync());
        
        exception.Should().BeSameAs(authException);
    }

    [Fact]
    public async Task Service_ShouldPropagateRateLimitException_WhenApiThrowsRateLimit()
    {
        // Arrange
        var rateLimitException = new PlanningCenterApiRateLimitException("Rate limit exceeded");
        _mockApiConnection.SetupException("/people/v2/people", rateLimitException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PlanningCenterApiRateLimitException>(
            () => _peopleService.ListAsync());
        
        exception.Should().BeSameAs(rateLimitException);
    }

    [Fact]
    public async Task Service_ShouldPropagateServerException_WhenApiThrowsServerError()
    {
        // Arrange
        var serverException = new PlanningCenterApiServerException("Internal server error");
        _mockApiConnection.SetupException("/people/v2/people/123", serverException);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PlanningCenterApiServerException>(
            () => _peopleService.GetAsync("123"));
        
        exception.Should().BeSameAs(serverException);
    }

    #endregion
}