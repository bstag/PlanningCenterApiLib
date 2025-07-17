using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using PlanningCenter.Api.Client.Fluent;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Calendar;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Tests.Utilities;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Fluent;

/// <summary>
/// Unit tests for CalendarFluentContext.
/// Tests the fluent API functionality for the Calendar module.
/// </summary>
public class CalendarFluentContextTests
{
    private readonly Mock<ICalendarService> _mockCalendarService;
    private readonly CalendarFluentContext _fluentContext;
    private readonly TestDataBuilder _builder = new();

    public CalendarFluentContextTests()
    {
        _mockCalendarService = new Mock<ICalendarService>();
        _fluentContext = new CalendarFluentContext(_mockCalendarService.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenCalendarServiceIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CalendarFluentContext(null!));
    }

    #endregion

    #region Query Building Tests

    [Fact]
    public void Where_ShouldReturnSameContext_WhenPredicateIsProvided()
    {
        // Act
        var result = _fluentContext.Where(e => e.Name == "Sunday Service");

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
        var result = _fluentContext.Include(e => e.LocationName);

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void OrderBy_ShouldReturnSameContext_WhenOrderByExpressionIsProvided()
    {
        // Act
        var result = _fluentContext.OrderBy(e => e.StartsAt);

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void OrderByDescending_ShouldReturnSameContext_WhenOrderByExpressionIsProvided()
    {
        // Act
        var result = _fluentContext.OrderByDescending(e => e.CreatedAt);

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    #endregion

    #region Execution Method Tests

    [Fact]
    public async Task GetAsync_ShouldCallCalendarService_WhenIdIsProvided()
    {
        // Arrange
        var eventId = "123";
        var expectedEvent = BuildEvent(eventId);
        _mockCalendarService.Setup(s => s.GetEventAsync(eventId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEvent);

        // Act
        var result = await _fluentContext.GetAsync(eventId);

        // Assert
        result.Should().BeSameAs(expectedEvent);
        _mockCalendarService.Verify(s => s.GetEventAsync(eventId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _fluentContext.GetAsync(""));
    }

    [Fact]
    public async Task GetPagedAsync_ShouldCallCalendarServiceWithCorrectPageSize()
    {
        // Arrange
        var pageSize = 10;
        var pagedResponse = BuildEventPagedResponse(pageSize);

        _mockCalendarService.Setup(s => s.ListEventsAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        // Act
        var result = await _fluentContext.GetPagedAsync(pageSize);

        // Assert
        result.Should().BeSameAs(pagedResponse);
        _mockCalendarService.Verify(s => s.ListEventsAsync(
            It.Is<QueryParameters>(p => p.PerPage == pageSize), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldCallCalendarServiceGetAllAsync()
    {
        // Arrange
        var expectedEvents = new List<Event> { BuildEvent("1"), BuildEvent("2") };
        _mockCalendarService.Setup(s => s.GetAllEventsAsync(It.IsAny<QueryParameters>(), It.IsAny<PaginationOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedEvents);

        // Act
        var result = await _fluentContext.GetAllAsync();

        // Assert
        result.Should().BeSameAs(expectedEvents);
        _mockCalendarService.Verify(s => s.GetAllEventsAsync(It.IsAny<QueryParameters>(), It.IsAny<PaginationOptions>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region LINQ-like Terminal Operations Tests

    [Fact]
    public async Task FirstAsync_ShouldReturnFirstEvent_WhenEventsExist()
    {
        // Arrange
        var events = new List<Event> { BuildEvent("1"), BuildEvent("2") };
        var pagedResponse = BuildEventPagedResponse(events);

        _mockCalendarService.Setup(s => s.ListEventsAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        // Act
        var result = await _fluentContext.FirstAsync();

        // Assert
        result.Should().BeSameAs(events[0]);
        _mockCalendarService.Verify(s => s.ListEventsAsync(
            It.Is<QueryParameters>(p => p.PerPage == 1), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CountAsync_ShouldReturnTotalCount_WhenMetaHasTotalCount()
    {
        // Arrange
        var expectedCount = 42;
        var pagedResponse = new PagedResponse<Event>
        {
            Data = new List<Event> { BuildEvent("1") },
            Meta = new PagedResponseMeta { TotalCount = expectedCount },
            Links = new PagedResponseLinks()
        };

        _mockCalendarService.Setup(s => s.ListEventsAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        // Act
        var result = await _fluentContext.CountAsync();

        // Assert
        result.Should().Be(expectedCount);
    }

    [Fact]
    public async Task AnyAsync_ShouldReturnTrue_WhenEventsExist()
    {
        // Arrange
        var pagedResponse = BuildEventPagedResponse(new List<Event> { BuildEvent("1") });

        _mockCalendarService.Setup(s => s.ListEventsAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        // Act
        var result = await _fluentContext.AnyAsync();

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region Specialized Calendar Operations Tests

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
    public void ThisMonth_ShouldReturnSameContext()
    {
        // Act
        var result = _fluentContext.ThisMonth();

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void Upcoming_ShouldReturnSameContext()
    {
        // Act
        var result = _fluentContext.Upcoming();

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    #endregion

    #region Chaining Tests

    [Fact]
    public void FluentChaining_ShouldAllowMethodChaining()
    {
        // Act & Assert (should not throw)
        var result = _fluentContext
            .Where(e => e.Name.Contains("Service"))
            .Today()
            .Upcoming()
            .OrderBy(e => e.StartsAt);

        result.Should().BeSameAs(_fluentContext);
    }

    #endregion

    #region Helper Methods

    private Event BuildEvent(string id)
    {
        var random = new Random();
        return new Event
        {
            Id = id,
            Name = $"Event {id}",
            StartsAt = DateTime.Now.AddDays(random.Next(-30, 30)),
            EndsAt = DateTime.Now.AddDays(random.Next(-30, 30)).AddHours(2),
            CreatedAt = DateTime.Now.AddDays(-random.Next(30)),
            UpdatedAt = DateTime.Now.AddDays(-random.Next(10))
        };
    }

    private PagedResponse<Event> BuildEventPagedResponse(int count)
    {
        var events = Enumerable.Range(1, count).Select(i => BuildEvent(i.ToString())).ToList();
        return BuildEventPagedResponse(events);
    }

    private PagedResponse<Event> BuildEventPagedResponse(List<Event> events)
    {
        return new PagedResponse<Event>
        {
            Data = events,
            Meta = new PagedResponseMeta { TotalCount = events.Count },
            Links = new PagedResponseLinks()
        };
    }

    #endregion
}