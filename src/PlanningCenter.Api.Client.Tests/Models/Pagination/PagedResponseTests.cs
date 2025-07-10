using FluentAssertions;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Tests.Utilities;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Models.Pagination;

/// <summary>
/// Unit tests for the PagedResponse model and its pagination functionality.
/// </summary>
public class PagedResponseTests
{
    private readonly TestDataBuilder _testDataBuilder = new();
    private readonly MockApiConnection _mockApiConnection = new();

    [Fact]
    public void PagedResponse_ShouldHaveCorrectDefaultValues()
    {
        // Arrange & Act
        var response = new PagedResponse<Person>();

        // Assert
        response.Data.Should().NotBeNull();
        response.Data.Should().BeEmpty();
        response.Meta.Should().BeNull();
        response.Links.Should().BeNull();
        response.ApiConnection.Should().BeNull();
        response.OriginalParameters.Should().BeNull();
        response.OriginalEndpoint.Should().BeNull();
    }

    [Fact]
    public void HasNextPage_ShouldReturnTrue_WhenNextLinkExists()
    {
        // Arrange
        var response = _testDataBuilder.CreatePagedResponse(
            new List<Person>(),
            currentPage: 1,
            totalPages: 3);

        // Act
        var hasNextPage = response.HasNextPage;

        // Assert
        hasNextPage.Should().BeTrue();
        response.Links!.Next.Should().NotBeNull();
    }

    [Fact]
    public void HasNextPage_ShouldReturnFalse_WhenNextLinkDoesNotExist()
    {
        // Arrange
        var response = _testDataBuilder.CreatePagedResponse(
            new List<Person>(),
            currentPage: 3,
            totalPages: 3);

        // Act
        var hasNextPage = response.HasNextPage;

        // Assert
        hasNextPage.Should().BeFalse();
        response.Links!.Next.Should().BeNull();
    }

    [Fact]
    public void HasPreviousPage_ShouldReturnTrue_WhenPrevLinkExists()
    {
        // Arrange
        var response = _testDataBuilder.CreatePagedResponse(
            new List<Person>(),
            currentPage: 2,
            totalPages: 3);

        // Act
        var hasPreviousPage = response.HasPreviousPage;

        // Assert
        hasPreviousPage.Should().BeTrue();
        response.Links!.Prev.Should().NotBeNull();
    }

    [Fact]
    public void HasPreviousPage_ShouldReturnFalse_WhenPrevLinkDoesNotExist()
    {
        // Arrange
        var response = _testDataBuilder.CreatePagedResponse(
            new List<Person>(),
            currentPage: 1,
            totalPages: 3);

        // Act
        var hasPreviousPage = response.HasPreviousPage;

        // Assert
        hasPreviousPage.Should().BeFalse();
        response.Links!.Prev.Should().BeNull();
    }

    [Fact]
    public void IsFirstPage_ShouldReturnTrue_WhenCurrentPageIsOne()
    {
        // Arrange
        var response = _testDataBuilder.CreatePagedResponse(
            new List<Person>(),
            currentPage: 1,
            totalPages: 3);

        // Act
        var isFirstPage = response.IsFirstPage;

        // Assert
        isFirstPage.Should().BeTrue();
    }

    [Fact]
    public void IsFirstPage_ShouldReturnFalse_WhenCurrentPageIsNotOne()
    {
        // Arrange
        var response = _testDataBuilder.CreatePagedResponse(
            new List<Person>(),
            currentPage: 2,
            totalPages: 3);

        // Act
        var isFirstPage = response.IsFirstPage;

        // Assert
        isFirstPage.Should().BeFalse();
    }

    [Fact]
    public void IsLastPage_ShouldReturnTrue_WhenCurrentPageEqualsTotal()
    {
        // Arrange
        var response = _testDataBuilder.CreatePagedResponse(
            new List<Person>(),
            currentPage: 3,
            totalPages: 3);

        // Act
        var isLastPage = response.IsLastPage;

        // Assert
        isLastPage.Should().BeTrue();
    }

    [Fact]
    public void IsLastPage_ShouldReturnFalse_WhenCurrentPageIsLessThanTotal()
    {
        // Arrange
        var response = _testDataBuilder.CreatePagedResponse(
            new List<Person>(),
            currentPage: 2,
            totalPages: 3);

        // Act
        var isLastPage = response.IsLastPage;

        // Assert
        isLastPage.Should().BeFalse();
    }

    [Fact]
    public void IsEmpty_ShouldReturnTrue_WhenDataIsEmpty()
    {
        // Arrange
        var response = _testDataBuilder.CreatePagedResponse(new List<Person>());

        // Act
        var isEmpty = response.IsEmpty;

        // Assert
        isEmpty.Should().BeTrue();
    }

    [Fact]
    public void IsEmpty_ShouldReturnFalse_WhenDataIsNotEmpty()
    {
        // Arrange
        var people = _testDataBuilder.CreatePersons(3);
        var response = _testDataBuilder.CreatePagedResponse(people);

        // Act
        var isEmpty = response.IsEmpty;

        // Assert
        isEmpty.Should().BeFalse();
    }

    [Fact]
    public async Task GetNextPageAsync_ShouldReturnNextPage_WhenNextPageExists()
    {
        // Arrange
        var firstPagePeople = _testDataBuilder.CreatePersons(2);
        var secondPagePeople = _testDataBuilder.CreatePersons(2);
        
        var firstPageResponse = _testDataBuilder.CreatePagedResponse(
            firstPagePeople,
            currentPage: 1,
            totalPages: 2);
        
        var secondPageResponse = _testDataBuilder.CreatePagedResponse(
            secondPagePeople,
            currentPage: 2,
            totalPages: 2);

        // Set up the response with API connection
        firstPageResponse.ApiConnection = _mockApiConnection.Object;
        firstPageResponse.OriginalEndpoint = "/people/v2/people";
        firstPageResponse.OriginalParameters = _testDataBuilder.CreateQueryParameters();

        _mockApiConnection.SetupPagedResponse("/people/v2/people", secondPageResponse);

        // Act
        var nextPage = await firstPageResponse.GetNextPageAsync();

        // Assert
        nextPage.Should().NotBeNull();
        nextPage!.Data.Should().HaveCount(2);
        nextPage.Meta!.CurrentPage.Should().Be(2);
        _mockApiConnection.VerifyGetRequest("/people/v2/people");
    }

    [Fact]
    public async Task GetNextPageAsync_ShouldReturnNull_WhenNoNextPageExists()
    {
        // Arrange
        var people = _testDataBuilder.CreatePersons(2);
        var response = _testDataBuilder.CreatePagedResponse(
            people,
            currentPage: 2,
            totalPages: 2);

        response.ApiConnection = _mockApiConnection.Object;

        // Act
        var nextPage = await response.GetNextPageAsync();

        // Assert
        nextPage.Should().BeNull();
        _mockApiConnection.VerifyNoRequests();
    }

    [Fact]
    public async Task GetPreviousPageAsync_ShouldReturnPreviousPage_WhenPreviousPageExists()
    {
        // Arrange
        var firstPagePeople = _testDataBuilder.CreatePersons(2);
        var secondPagePeople = _testDataBuilder.CreatePersons(2);
        
        var firstPageResponse = _testDataBuilder.CreatePagedResponse(
            firstPagePeople,
            currentPage: 1,
            totalPages: 2);
        
        var secondPageResponse = _testDataBuilder.CreatePagedResponse(
            secondPagePeople,
            currentPage: 2,
            totalPages: 2);

        // Set up the response with API connection
        secondPageResponse.ApiConnection = _mockApiConnection.Object;
        secondPageResponse.OriginalEndpoint = "/people/v2/people";
        secondPageResponse.OriginalParameters = _testDataBuilder.CreateQueryParameters();

        _mockApiConnection.SetupPagedResponse("/people/v2/people", firstPageResponse);

        // Act
        var previousPage = await secondPageResponse.GetPreviousPageAsync();

        // Assert
        previousPage.Should().NotBeNull();
        previousPage!.Data.Should().HaveCount(2);
        previousPage.Meta!.CurrentPage.Should().Be(1);
        _mockApiConnection.VerifyGetRequest("/people/v2/people");
    }

    [Fact]
    public async Task GetPreviousPageAsync_ShouldReturnNull_WhenNoPreviousPageExists()
    {
        // Arrange
        var people = _testDataBuilder.CreatePersons(2);
        var response = _testDataBuilder.CreatePagedResponse(
            people,
            currentPage: 1,
            totalPages: 2);

        response.ApiConnection = _mockApiConnection.Object;

        // Act
        var previousPage = await response.GetPreviousPageAsync();

        // Assert
        previousPage.Should().BeNull();
        _mockApiConnection.VerifyNoRequests();
    }

    [Fact]
    public async Task GetAllRemainingAsync_ShouldReturnAllItems_WhenMultiplePagesExist()
    {
        // Arrange
        var firstPagePeople = _testDataBuilder.CreatePersons(2, (p, i) => p.Id = $"person-{i + 1}");
        var secondPagePeople = _testDataBuilder.CreatePersons(2, (p, i) => p.Id = $"person-{i + 3}");
        var thirdPagePeople = _testDataBuilder.CreatePersons(1, (p, i) => p.Id = $"person-{i + 5}");

        var firstPageResponse = _testDataBuilder.CreatePagedResponse(
            firstPagePeople,
            currentPage: 1,
            totalPages: 3,
            totalCount: 5);

        var secondPageResponse = _testDataBuilder.CreatePagedResponse(
            secondPagePeople,
            currentPage: 2,
            totalPages: 3,
            totalCount: 5);

        var thirdPageResponse = _testDataBuilder.CreatePagedResponse(
            thirdPagePeople,
            currentPage: 3,
            totalPages: 3,
            totalCount: 5);

        // Set up the first page response with API connection
        firstPageResponse.ApiConnection = _mockApiConnection.Object;
        firstPageResponse.OriginalEndpoint = "/people/v2/people";
        firstPageResponse.OriginalParameters = _testDataBuilder.CreateQueryParameters();

        // Set up mock responses for subsequent pages
        _mockApiConnection
            .SetupPagedResponse("/people/v2/people", secondPageResponse)
            .SetupPagedResponse("/people/v2/people", thirdPageResponse);

        // Act
        var allPeople = await firstPageResponse.GetAllRemainingAsync();

        // Assert
        allPeople.Should().HaveCount(5);
        allPeople.Select(p => p.Id).Should().BeEquivalentTo(new[] 
        { 
            "person-1", "person-2", "person-3", "person-4", "person-5" 
        });
    }

    [Fact]
    public async Task GetAllRemainingAsyncEnumerable_ShouldYieldAllItems_WhenMultiplePagesExist()
    {
        // Arrange
        var firstPagePeople = _testDataBuilder.CreatePersons(2, (p, i) => p.Id = $"person-{i + 1}");
        var secondPagePeople = _testDataBuilder.CreatePersons(2, (p, i) => p.Id = $"person-{i + 3}");

        var firstPageResponse = _testDataBuilder.CreatePagedResponse(
            firstPagePeople,
            currentPage: 1,
            totalPages: 2,
            totalCount: 4);

        var secondPageResponse = _testDataBuilder.CreatePagedResponse(
            secondPagePeople,
            currentPage: 2,
            totalPages: 2,
            totalCount: 4);

        // Set up the first page response with API connection
        firstPageResponse.ApiConnection = _mockApiConnection.Object;
        firstPageResponse.OriginalEndpoint = "/people/v2/people";
        firstPageResponse.OriginalParameters = _testDataBuilder.CreateQueryParameters();

        _mockApiConnection.SetupPagedResponse("/people/v2/people", secondPageResponse);

        // Act
        var allPeople = new List<Person>();
        await foreach (var person in firstPageResponse.GetAllRemainingAsyncEnumerable())
        {
            allPeople.Add(person);
        }

        // Assert
        allPeople.Should().HaveCount(4);
        allPeople.Select(p => p.Id).Should().BeEquivalentTo(new[] 
        { 
            "person-1", "person-2", "person-3", "person-4" 
        });
    }

    [Fact]
    public void PagedResponse_ShouldThrowException_WhenApiConnectionIsNull()
    {
        // Arrange
        var people = _testDataBuilder.CreatePersons(2);
        var response = _testDataBuilder.CreatePagedResponse(people, currentPage: 1, totalPages: 2);
        
        // ApiConnection is null by default

        // Act & Assert
        var act = async () => await response.GetNextPageAsync();
        act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*ApiConnection*");
    }
}