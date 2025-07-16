using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PlanningCenter.Api.Client.Fluent;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Tests.Utilities;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Fluent;

/// <summary>
/// Unit tests for PeopleFluentContext.
/// Tests the fluent API functionality without affecting standard API calls.
/// </summary>
public class PeopleFluentContextTests
{
    private readonly Mock<IPeopleService> _mockPeopleService;
    private readonly PeopleFluentContext _fluentContext;
    private readonly TestDataBuilder _builder = new();

    public PeopleFluentContextTests()
    {
        _mockPeopleService = new Mock<IPeopleService>();
        _fluentContext = new PeopleFluentContext(_mockPeopleService.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenPeopleServiceIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new PeopleFluentContext(null!));
    }

    #endregion

    #region Query Building Tests

    [Fact]
    public void Where_ShouldReturnSameContext_WhenPredicateIsProvided()
    {
        // Act
        var result = _fluentContext.Where(p => p.FirstName == "John");

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void Where_ShouldThrowArgumentNullException_WhenPredicateIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _fluentContext.Where(null!));
    }

    [Fact]
    public void Include_ShouldReturnSameContext_WhenIncludeExpressionIsProvided()
    {
        // Act
        var result = _fluentContext.Include(p => p.Addresses);

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void Include_ShouldThrowArgumentNullException_WhenIncludeExpressionIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _fluentContext.Include(null!));
    }

    [Fact]
    public void OrderBy_ShouldReturnSameContext_WhenOrderByExpressionIsProvided()
    {
        // Act
        var result = _fluentContext.OrderBy(p => p.LastName);

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void OrderBy_ShouldThrowArgumentNullException_WhenOrderByExpressionIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _fluentContext.OrderBy(null!));
    }

    [Fact]
    public void OrderByDescending_ShouldReturnSameContext_WhenOrderByExpressionIsProvided()
    {
        // Act
        var result = _fluentContext.OrderByDescending(p => p.CreatedAt);

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void ThenBy_ShouldReturnSameContext_WhenThenByExpressionIsProvided()
    {
        // Act
        var result = _fluentContext.OrderBy(p => p.LastName).ThenBy(p => p.FirstName);

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void ThenByDescending_ShouldReturnSameContext_WhenThenByExpressionIsProvided()
    {
        // Act
        var result = _fluentContext.OrderBy(p => p.LastName).ThenByDescending(p => p.CreatedAt);

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    #endregion

    #region Execution Method Tests

    [Fact]
    public async Task GetAsync_ShouldCallPeopleService_WhenIdIsProvided()
    {
        // Arrange
        var personId = "123";
        var expectedPerson = _builder.BuildPerson(personId);
        _mockPeopleService.Setup(s => s.GetAsync(personId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPerson);

        // Act
        var result = await _fluentContext.GetAsync(personId);

        // Assert
        result.Should().BeSameAs(expectedPerson);
        _mockPeopleService.Verify(s => s.GetAsync(personId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _fluentContext.GetAsync(""));
    }

    [Fact]
    public async Task GetPagedAsync_ShouldCallPeopleServiceWithCorrectPageSize()
    {
        // Arrange
        var pageSize = 10;
        var expectedResponse = _builder.BuildPersonCollectionResponse(pageSize);
        var pagedResponse = new PagedResponse<Person>
        {
            Data = expectedResponse.Data!.Select(dto => _builder.BuildPerson(dto.Id)).ToList(),
            Meta = expectedResponse.Meta,
            Links = expectedResponse.Links
        };

        _mockPeopleService.Setup(s => s.ListAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        // Act
        var result = await _fluentContext.GetPagedAsync(pageSize);

        // Assert
        result.Should().BeSameAs(pagedResponse);
        _mockPeopleService.Verify(s => s.ListAsync(
            It.Is<QueryParameters>(p => p.PerPage == pageSize), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPageAsync_ShouldCallPeopleServiceWithCorrectOffsetAndPageSize()
    {
        // Arrange
        var page = 2;
        var pageSize = 5;
        var expectedOffset = (page - 1) * pageSize; // Should be 5
        var pagedResponse = new PagedResponse<Person>
        {
            Data = new List<Person> { _builder.BuildPerson("1") },
            Meta = new PagedResponseMeta { TotalCount = 1 },
            Links = new PagedResponseLinks()
        };

        _mockPeopleService.Setup(s => s.ListAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        // Act
        var result = await _fluentContext.GetPageAsync(page, pageSize);

        // Assert
        result.Should().BeSameAs(pagedResponse);
        _mockPeopleService.Verify(s => s.ListAsync(
            It.Is<QueryParameters>(p => p.PerPage == pageSize && p.Offset == expectedOffset), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPageAsync_ShouldThrowArgumentException_WhenPageIsLessThanOne()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _fluentContext.GetPageAsync(0, 10));
    }

    [Fact]
    public async Task GetAllAsync_ShouldCallPeopleServiceGetAllAsync()
    {
        // Arrange
        var expectedPeople = new List<Person> { _builder.BuildPerson("1"), _builder.BuildPerson("2") };
        _mockPeopleService.Setup(s => s.GetAllAsync(It.IsAny<QueryParameters>(), It.IsAny<PaginationOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPeople);

        // Act
        var result = await _fluentContext.GetAllAsync();

        // Assert
        result.Should().BeSameAs(expectedPeople);
        _mockPeopleService.Verify(s => s.GetAllAsync(It.IsAny<QueryParameters>(), It.IsAny<PaginationOptions>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region LINQ-like Terminal Operations Tests

    [Fact]
    public async Task FirstAsync_ShouldReturnFirstPerson_WhenPeopleExist()
    {
        // Arrange
        var people = new List<Person> { _builder.BuildPerson("1"), _builder.BuildPerson("2") };
        var pagedResponse = new PagedResponse<Person>
        {
            Data = people,
            Meta = new PagedResponseMeta { TotalCount = 2 },
            Links = new PagedResponseLinks()
        };

        _mockPeopleService.Setup(s => s.ListAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        // Act
        var result = await _fluentContext.FirstAsync();

        // Assert
        result.Should().BeSameAs(people[0]);
        _mockPeopleService.Verify(s => s.ListAsync(
            It.Is<QueryParameters>(p => p.PerPage == 1), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FirstAsync_ShouldThrowInvalidOperationException_WhenNoPeopleExist()
    {
        // Arrange
        var pagedResponse = new PagedResponse<Person>
        {
            Data = new List<Person>(),
            Meta = new PagedResponseMeta { TotalCount = 0 },
            Links = new PagedResponseLinks()
        };

        _mockPeopleService.Setup(s => s.ListAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _fluentContext.FirstAsync());
    }

    [Fact]
    public async Task FirstOrDefaultAsync_ShouldReturnNull_WhenNoPeopleExist()
    {
        // Arrange
        var pagedResponse = new PagedResponse<Person>
        {
            Data = new List<Person>(),
            Meta = new PagedResponseMeta { TotalCount = 0 },
            Links = new PagedResponseLinks()
        };

        _mockPeopleService.Setup(s => s.ListAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        // Act
        var result = await _fluentContext.FirstOrDefaultAsync();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CountAsync_ShouldReturnTotalCount_WhenMetaHasTotalCount()
    {
        // Arrange
        var expectedCount = 42;
        var pagedResponse = new PagedResponse<Person>
        {
            Data = new List<Person> { _builder.BuildPerson("1") },
            Meta = new PagedResponseMeta { TotalCount = expectedCount },
            Links = new PagedResponseLinks()
        };

        _mockPeopleService.Setup(s => s.ListAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        // Act
        var result = await _fluentContext.CountAsync();

        // Assert
        result.Should().Be(expectedCount);
    }

    [Fact]
    public async Task AnyAsync_ShouldReturnTrue_WhenPeopleExist()
    {
        // Arrange
        var pagedResponse = new PagedResponse<Person>
        {
            Data = new List<Person> { _builder.BuildPerson("1") },
            Meta = new PagedResponseMeta { TotalCount = 1 },
            Links = new PagedResponseLinks()
        };

        _mockPeopleService.Setup(s => s.ListAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        // Act
        var result = await _fluentContext.AnyAsync();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task AnyAsync_ShouldReturnFalse_WhenNoPeopleExist()
    {
        // Arrange
        var pagedResponse = new PagedResponse<Person>
        {
            Data = new List<Person>(),
            Meta = new PagedResponseMeta { TotalCount = 0 },
            Links = new PagedResponseLinks()
        };

        _mockPeopleService.Setup(s => s.ListAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        // Act
        var result = await _fluentContext.AnyAsync();

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region Creation Context Tests

    [Fact]
    public void Create_ShouldReturnPeopleCreateContext_WhenRequestIsProvided()
    {
        // Arrange
        var request = new PersonCreateRequest { FirstName = "John", LastName = "Doe" };

        // Act
        var result = _fluentContext.Create(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<PeopleCreateContext>();
    }

    [Fact]
    public void Create_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _fluentContext.Create(null!));
    }

    #endregion

    #region Chaining Tests

    [Fact]
    public void FluentChaining_ShouldAllowMethodChaining()
    {
        // Act & Assert (should not throw)
        var result = _fluentContext
            .Where(p => p.FirstName == "John")
            .Where(p => p.LastName.Contains("Smith"))
            .Include(p => p.Addresses)
            .Include(p => p.Emails)
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ThenByDescending(p => p.CreatedAt);

        result.Should().BeSameAs(_fluentContext);
    }

    #endregion
}