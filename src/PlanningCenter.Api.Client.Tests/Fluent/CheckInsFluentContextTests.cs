using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using PlanningCenter.Api.Client.Fluent;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.CheckIns;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Tests.Utilities;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Fluent;

/// <summary>
/// Unit tests for CheckInsFluentContext.
/// Tests the fluent API functionality for the CheckIns module.
/// </summary>
public class CheckInsFluentContextTests
{
    private readonly Mock<ICheckInsService> _mockCheckInsService;
    private readonly CheckInsFluentContext _fluentContext;
    private readonly TestDataBuilder _builder = new();

    public CheckInsFluentContextTests()
    {
        _mockCheckInsService = new Mock<ICheckInsService>();
        _fluentContext = new CheckInsFluentContext(_mockCheckInsService.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenCheckInsServiceIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CheckInsFluentContext(null!));
    }

    #endregion

    #region Query Building Tests

    [Fact]
    public void Where_ShouldReturnSameContext_WhenPredicateIsProvided()
    {
        // Act
        var result = _fluentContext.Where(c => c.FirstName == "John");

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
        var result = _fluentContext.Include(c => c.PersonId!);

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void OrderBy_ShouldReturnSameContext_WhenOrderByExpressionIsProvided()
    {
        // Act
        var result = _fluentContext.OrderBy(c => c.CreatedAt!);

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void OrderByDescending_ShouldReturnSameContext_WhenOrderByExpressionIsProvided()
    {
        // Act
        var result = _fluentContext.OrderByDescending(c => c.FirstName!);

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    #endregion

    #region Execution Method Tests

    [Fact]
    public async Task GetAsync_ShouldCallCheckInsService_WhenIdIsProvided()
    {
        // Arrange
        var checkInId = "123";
        var expectedCheckIn = BuildCheckIn(checkInId);
        _mockCheckInsService.Setup(s => s.GetCheckInAsync(checkInId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCheckIn);

        // Act
        var result = await _fluentContext.GetAsync(checkInId);

        // Assert
        result.Should().BeSameAs(expectedCheckIn);
        _mockCheckInsService.Verify(s => s.GetCheckInAsync(checkInId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _fluentContext.GetAsync(""));
    }

    [Fact]
    public async Task GetPagedAsync_ShouldCallCheckInsServiceWithCorrectPageSize()
    {
        // Arrange
        var pageSize = 10;
        var pagedResponse = BuildCheckInPagedResponse(pageSize);

        _mockCheckInsService.Setup(s => s.ListCheckInsAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        // Act
        var result = await _fluentContext.GetPagedAsync(pageSize);

        // Assert
        result.Should().BeSameAs(pagedResponse);
        _mockCheckInsService.Verify(s => s.ListCheckInsAsync(
            It.Is<QueryParameters>(p => p.PerPage == pageSize), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldCallCheckInsServiceGetAllAsync()
    {
        // Arrange
        var expectedCheckIns = new List<CheckIn> { BuildCheckIn("1"), BuildCheckIn("2") };
        _mockCheckInsService.Setup(s => s.GetAllCheckInsAsync(It.IsAny<QueryParameters>(), It.IsAny<PaginationOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCheckIns);

        // Act
        var result = await _fluentContext.GetAllAsync();

        // Assert
        result.Should().BeSameAs(expectedCheckIns);
        _mockCheckInsService.Verify(s => s.GetAllCheckInsAsync(It.IsAny<QueryParameters>(), It.IsAny<PaginationOptions>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region LINQ-like Terminal Operations Tests

    [Fact]
    public async Task FirstAsync_ShouldReturnFirstCheckIn_WhenCheckInsExist()
    {
        // Arrange
        var checkIns = new List<CheckIn> { BuildCheckIn("1"), BuildCheckIn("2") };
        var pagedResponse = BuildCheckInPagedResponse(checkIns);

        _mockCheckInsService.Setup(s => s.ListCheckInsAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        // Act
        var result = await _fluentContext.FirstAsync();

        // Assert
        result.Should().BeSameAs(checkIns[0]);
        _mockCheckInsService.Verify(s => s.ListCheckInsAsync(
            It.Is<QueryParameters>(p => p.PerPage == 1), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FirstAsync_ShouldThrowInvalidOperationException_WhenNoCheckInsExist()
    {
        // Arrange
        var pagedResponse = BuildCheckInPagedResponse(new List<CheckIn>());

        _mockCheckInsService.Setup(s => s.ListCheckInsAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _fluentContext.FirstAsync());
    }

    [Fact]
    public async Task CountAsync_ShouldReturnTotalCount_WhenMetaHasTotalCount()
    {
        // Arrange
        var expectedCount = 42;
        var pagedResponse = new PagedResponse<CheckIn>
        {
            Data = new List<CheckIn> { BuildCheckIn("1") },
            Meta = new PagedResponseMeta { TotalCount = expectedCount },
            Links = new PagedResponseLinks()
        };

        _mockCheckInsService.Setup(s => s.ListCheckInsAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        // Act
        var result = await _fluentContext.CountAsync();

        // Assert
        result.Should().Be(expectedCount);
    }

    [Fact]
    public async Task AnyAsync_ShouldReturnTrue_WhenCheckInsExist()
    {
        // Arrange
        var pagedResponse = BuildCheckInPagedResponse(new List<CheckIn> { BuildCheckIn("1") });

        _mockCheckInsService.Setup(s => s.ListCheckInsAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        // Act
        var result = await _fluentContext.AnyAsync();

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region Specialized CheckIns Operations Tests

    [Fact]
    public void ByPerson_ShouldReturnSameContext_WhenPersonIdIsProvided()
    {
        // Act
        var result = _fluentContext.ByPerson("person-123");

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void ByPerson_ShouldThrowArgumentException_WhenPersonIdIsEmpty()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _fluentContext.ByPerson(""));
    }

    [Fact]
    public void ByEvent_ShouldReturnSameContext_WhenEventIdIsProvided()
    {
        // Act
        var result = _fluentContext.ByEvent("event-123");

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void ByEvent_ShouldThrowArgumentException_WhenEventIdIsEmpty()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _fluentContext.ByEvent(""));
    }

    [Fact]
    public void ByEventTime_ShouldReturnSameContext_WhenEventTimeIdIsProvided()
    {
        // Act
        var result = _fluentContext.ByEventTime("event-time-123");

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void ByEventTime_ShouldThrowArgumentException_WhenEventTimeIdIsEmpty()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _fluentContext.ByEventTime(""));
    }

    [Fact]
    public void ByLocation_ShouldReturnSameContext_WhenLocationIdIsProvided()
    {
        // Act
        var result = _fluentContext.ByLocation("location-123");

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void ByLocation_ShouldThrowArgumentException_WhenLocationIdIsEmpty()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _fluentContext.ByLocation(""));
    }

    [Fact]
    public void ByDateRange_ShouldReturnSameContext_WhenValidDateRangeIsProvided()
    {
        // Arrange
        var startDate = DateTime.Now.AddDays(-7);
        var endDate = DateTime.Now;

        // Act
        var result = _fluentContext.ByDateRange(startDate, endDate);

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void ByDateRange_ShouldThrowArgumentException_WhenStartDateIsAfterEndDate()
    {
        // Arrange
        var startDate = DateTime.Now;
        var endDate = DateTime.Now.AddDays(-7);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _fluentContext.ByDateRange(startDate, endDate));
    }

    [Fact]
    public void Today_ShouldReturnSameContext()
    {
        // Act
        var result = _fluentContext.Today();

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void ThisWeek_ShouldReturnSameContext()
    {
        // Act
        var result = _fluentContext.ThisWeek();

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void CheckedIn_ShouldReturnSameContext()
    {
        // Act
        var result = _fluentContext.CheckedIn();

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void CheckedOut_ShouldReturnSameContext()
    {
        // Act
        var result = _fluentContext.CheckedOut();

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void Confirmed_ShouldReturnSameContext()
    {
        // Act
        var result = _fluentContext.Confirmed();

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void Unconfirmed_ShouldReturnSameContext()
    {
        // Act
        var result = _fluentContext.Unconfirmed();

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void Guests_ShouldReturnSameContext()
    {
        // Act
        var result = _fluentContext.Guests();

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void Members_ShouldReturnSameContext()
    {
        // Act
        var result = _fluentContext.Members();

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void WithMedicalNotes_ShouldReturnSameContext()
    {
        // Act
        var result = _fluentContext.WithMedicalNotes();

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void ByNameContains_ShouldReturnSameContext_WhenNameFragmentIsProvided()
    {
        // Act
        var result = _fluentContext.ByNameContains("Smith");

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void ByNameContains_ShouldThrowArgumentException_WhenNameFragmentIsEmpty()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _fluentContext.ByNameContains(""));
    }

    [Fact]
    public void ByKind_ShouldReturnSameContext_WhenKindIsProvided()
    {
        // Act
        var result = _fluentContext.ByKind("volunteer");

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void ByKind_ShouldThrowArgumentException_WhenKindIsEmpty()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _fluentContext.ByKind(""));
    }

    #endregion

    #region Aggregation Methods Tests

    [Fact]
    public async Task CountByEventAsync_ShouldReturnCount_WhenEventIdIsProvided()
    {
        // Arrange
        var eventId = "event-123";
        var expectedCount = 5;
        var response = BuildCheckInPagedResponse(expectedCount);
        _mockCheckInsService.Setup(s => s.ListCheckInsAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _fluentContext.CountByEventAsync(eventId);

        // Assert
        result.Should().Be(expectedCount);
        _mockCheckInsService.Verify(s => s.ListCheckInsAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CountByEventAsync_ShouldThrowArgumentException_WhenEventIdIsEmpty()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _fluentContext.CountByEventAsync(""));
    }

    [Fact]
    public async Task CountByLocationAsync_ShouldReturnCount_WhenLocationIdIsProvided()
    {
        // Arrange
        var locationId = "location-456";
        var expectedCount = 3;
        var response = BuildCheckInPagedResponse(expectedCount);
        _mockCheckInsService.Setup(s => s.ListCheckInsAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _fluentContext.CountByLocationAsync(locationId);

        // Assert
        result.Should().Be(expectedCount);
        _mockCheckInsService.Verify(s => s.ListCheckInsAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CountByLocationAsync_ShouldThrowArgumentException_WhenLocationIdIsEmpty()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _fluentContext.CountByLocationAsync(""));
    }

    [Fact]
    public async Task CountCheckedInAsync_ShouldReturnCount()
    {
        // Arrange
        var expectedCount = 7;
        var response = BuildCheckInPagedResponse(expectedCount);
        _mockCheckInsService.Setup(s => s.ListCheckInsAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _fluentContext.CountCheckedInAsync();

        // Assert
        result.Should().Be(expectedCount);
        _mockCheckInsService.Verify(s => s.ListCheckInsAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CountCheckedOutAsync_ShouldReturnCount()
    {
        // Arrange
        var expectedCount = 4;
        var response = BuildCheckInPagedResponse(expectedCount);
        _mockCheckInsService.Setup(s => s.ListCheckInsAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _fluentContext.CountCheckedOutAsync();

        // Assert
        result.Should().Be(expectedCount);
        _mockCheckInsService.Verify(s => s.ListCheckInsAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CountGuestsAsync_ShouldReturnCount()
    {
        // Arrange
        var expectedCount = 2;
        var response = BuildCheckInPagedResponse(expectedCount);
        _mockCheckInsService.Setup(s => s.ListCheckInsAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _fluentContext.CountGuestsAsync();

        // Assert
        result.Should().Be(expectedCount);
        _mockCheckInsService.Verify(s => s.ListCheckInsAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CountMembersAsync_ShouldReturnCount()
    {
        // Arrange
        var expectedCount = 8;
        var response = BuildCheckInPagedResponse(expectedCount);
        _mockCheckInsService.Setup(s => s.ListCheckInsAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _fluentContext.CountMembersAsync();

        // Assert
        result.Should().Be(expectedCount);
        _mockCheckInsService.Verify(s => s.ListCheckInsAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CountWithMedicalNotesAsync_ShouldReturnCount()
    {
        // Arrange
        var expectedCount = 1;
        var response = BuildCheckInPagedResponse(expectedCount);
        _mockCheckInsService.Setup(s => s.ListCheckInsAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _fluentContext.CountWithMedicalNotesAsync();

        // Assert
        result.Should().Be(expectedCount);
        _mockCheckInsService.Verify(s => s.ListCheckInsAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Chaining Tests

    [Fact]
    public void FluentChaining_ShouldAllowMethodChaining()
    {
        // Act & Assert (should not throw)
        var result = _fluentContext
            .Where(c => c.FirstName == "John")
            .Today()
            .CheckedIn()
            .Confirmed()
            .Members()
            .OrderBy(c => c.CreatedAt!);

        result.Should().BeSameAs(_fluentContext);
    }

    #endregion

    #region Helper Methods

    private CheckIn BuildCheckIn(string id)
    {
        var random = new Random();
        var now = DateTime.Now;
        return new CheckIn
        {
            Id = id,
            FirstName = $"First{id}",
            LastName = $"Last{id}",
            Kind = "attendee",
            OneTimeGuest = random.Next(0, 2) == 1,
            CreatedAt = now.AddDays(-random.Next(7)),
            CheckedOutAt = random.Next(0, 3) == 0 ? now.AddHours(-random.Next(5)) : null,
            ConfirmedAt = random.Next(0, 2) == 1 ? now.AddMinutes(-random.Next(60)) : null,
            MedicalNotes = random.Next(0, 4) == 0 ? "Has allergies" : null,
            UpdatedAt = now.AddDays(-random.Next(5))
        };
    }

    private PagedResponse<CheckIn> BuildCheckInPagedResponse(int count)
    {
        var checkIns = Enumerable.Range(1, count).Select(i => BuildCheckIn(i.ToString())).ToList();
        return BuildCheckInPagedResponse(checkIns);
    }

    private PagedResponse<CheckIn> BuildCheckInPagedResponse(List<CheckIn> checkIns)
    {
        return new PagedResponse<CheckIn>
        {
            Data = checkIns,
            Meta = new PagedResponseMeta { TotalCount = checkIns.Count },
            Links = new PagedResponseLinks()
        };
    }

    #endregion
}