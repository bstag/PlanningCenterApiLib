using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using PlanningCenter.Api.Client.Fluent;
using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Models.Services;
using PlanningCenter.Api.Client.Tests.Utilities;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Fluent;

/// <summary>
/// Unit tests for ServicesFluentContext.
/// Tests the fluent API functionality for the Services module.
/// </summary>
public class ServicesFluentContextTests
{
    private readonly Mock<IServicesService> _mockServicesService;
    private readonly ServicesFluentContext _fluentContext;
    private readonly TestDataBuilder _builder = new();

    public ServicesFluentContextTests()
    {
        _mockServicesService = new Mock<IServicesService>();
        _fluentContext = new ServicesFluentContext(_mockServicesService.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenServicesServiceIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ServicesFluentContext(null!));
    }

    #endregion

    #region Query Building Tests

    [Fact]
    public void Where_ShouldReturnSameContext_WhenPredicateIsProvided()
    {
        // Act
        var result = _fluentContext.Where(p => p.Title == "Sunday Service");

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
        var result = _fluentContext.Include(p => p.ServiceTypeId!);

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void OrderBy_ShouldReturnSameContext_WhenOrderByExpressionIsProvided()
    {
        // Act
        var result = _fluentContext.OrderBy(p => p.SortDate!);

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void OrderByDescending_ShouldReturnSameContext_WhenOrderByExpressionIsProvided()
    {
        // Act
        var result = _fluentContext.OrderByDescending(p => p.CreatedAt!);

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    #endregion

    #region Execution Method Tests

    [Fact]
    public async Task GetAsync_ShouldCallServicesService_WhenIdIsProvided()
    {
        // Arrange
        var planId = "123";
        var expectedPlan = BuildPlan(planId);
        _mockServicesService.Setup(s => s.GetPlanAsync(planId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPlan);

        // Act
        var result = await _fluentContext.GetAsync(planId);

        // Assert
        result.Should().BeSameAs(expectedPlan);
        _mockServicesService.Verify(s => s.GetPlanAsync(planId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _fluentContext.GetAsync(""));
    }

    [Fact]
    public async Task GetPagedAsync_ShouldCallServicesServiceWithCorrectPageSize()
    {
        // Arrange
        var pageSize = 10;
        var pagedResponse = BuildPlanPagedResponse(pageSize);

        _mockServicesService.Setup(s => s.ListPlansAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        // Act
        var result = await _fluentContext.GetPagedAsync(pageSize);

        // Assert
        result.Should().BeSameAs(pagedResponse);
        _mockServicesService.Verify(s => s.ListPlansAsync(
            It.Is<QueryParameters>(p => p.PerPage == pageSize), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldCallServicesServiceGetAllAsync()
    {
        // Arrange
        var expectedPlans = new List<Plan> { BuildPlan("1"), BuildPlan("2") };
        _mockServicesService.Setup(s => s.GetAllPlansAsync(It.IsAny<QueryParameters>(), It.IsAny<PaginationOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPlans);

        // Act
        var result = await _fluentContext.GetAllAsync();

        // Assert
        result.Should().BeSameAs(expectedPlans);
        _mockServicesService.Verify(s => s.GetAllPlansAsync(It.IsAny<QueryParameters>(), It.IsAny<PaginationOptions>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region LINQ-like Terminal Operations Tests

    [Fact]
    public async Task FirstAsync_ShouldReturnFirstPlan_WhenPlansExist()
    {
        // Arrange
        var plans = new List<Plan> { BuildPlan("1"), BuildPlan("2") };
        var pagedResponse = BuildPlanPagedResponse(plans);

        _mockServicesService.Setup(s => s.ListPlansAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        // Act
        var result = await _fluentContext.FirstAsync();

        // Assert
        result.Should().BeSameAs(plans[0]);
        _mockServicesService.Verify(s => s.ListPlansAsync(
            It.Is<QueryParameters>(p => p.PerPage == 1), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CountAsync_ShouldReturnTotalCount_WhenMetaHasTotalCount()
    {
        // Arrange
        var expectedCount = 42;
        var pagedResponse = new PagedResponse<Plan>
        {
            Data = new List<Plan> { BuildPlan("1") },
            Meta = new PagedResponseMeta { TotalCount = expectedCount },
            Links = new PagedResponseLinks()
        };

        _mockServicesService.Setup(s => s.ListPlansAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        // Act
        var result = await _fluentContext.CountAsync();

        // Assert
        result.Should().Be(expectedCount);
    }

    [Fact]
    public async Task AnyAsync_ShouldReturnTrue_WhenPlansExist()
    {
        // Arrange
        var pagedResponse = BuildPlanPagedResponse(new List<Plan> { BuildPlan("1") });

        _mockServicesService.Setup(s => s.ListPlansAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        // Act
        var result = await _fluentContext.AnyAsync();

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region Specialized Services Operations Tests

    [Fact]
    public void ByServiceType_ShouldReturnSameContext_WhenServiceTypeIdIsProvided()
    {
        // Act
        var result = _fluentContext.ByServiceType("sunday-morning");

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void ByServiceType_ShouldThrowArgumentException_WhenServiceTypeIdIsEmpty()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _fluentContext.ByServiceType(""));
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
    public void Upcoming_ShouldReturnSameContext()
    {
        // Act
        var result = _fluentContext.Upcoming();

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void Past_ShouldReturnSameContext()
    {
        // Act
        var result = _fluentContext.Past();

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
    public void Public_ShouldReturnSameContext()
    {
        // Act
        var result = _fluentContext.Public();

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void Private_ShouldReturnSameContext()
    {
        // Act
        var result = _fluentContext.Private();

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void WithMinimumLength_ShouldReturnSameContext_WhenValidMinutesProvided()
    {
        // Act
        var result = _fluentContext.WithMinimumLength(60);

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void WithMinimumLength_ShouldThrowArgumentException_WhenMinutesIsNegative()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _fluentContext.WithMinimumLength(-10));
    }

    [Fact]
    public void ByTitleContains_ShouldReturnSameContext_WhenTitleFragmentIsProvided()
    {
        // Act
        var result = _fluentContext.ByTitleContains("Christmas");

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void ByTitleContains_ShouldThrowArgumentException_WhenTitleFragmentIsEmpty()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _fluentContext.ByTitleContains(""));
    }

    #endregion

    #region Chaining Tests

    [Fact]
    public void FluentChaining_ShouldAllowMethodChaining()
    {
        // Act & Assert (should not throw)
        var result = _fluentContext
            .Where(p => p.Title.Contains("Service"))
            .Upcoming()
            .Public()
            .WithMinimumLength(60)
            .OrderBy(p => p.SortDate!);

        result.Should().BeSameAs(_fluentContext);
    }

    #endregion

    #region Helper Methods

    private Plan BuildPlan(string id)
    {
        var random = new Random();
        return new Plan
        {
            Id = id,
            Title = $"Service Plan {id}",
            SortDate = DateTime.Now.AddDays(random.Next(-30, 30)),
            IsPublic = random.Next(0, 2) == 1,
            Length = random.Next(30, 180),
            CreatedAt = DateTime.Now.AddDays(-random.Next(30)),
            UpdatedAt = DateTime.Now.AddDays(-random.Next(10))
        };
    }

    private PagedResponse<Plan> BuildPlanPagedResponse(int count)
    {
        var plans = Enumerable.Range(1, count).Select(i => BuildPlan(i.ToString())).ToList();
        return BuildPlanPagedResponse(plans);
    }

    private PagedResponse<Plan> BuildPlanPagedResponse(List<Plan> plans)
    {
        return new PagedResponse<Plan>
        {
            Data = plans,
            Meta = new PagedResponseMeta { TotalCount = plans.Count },
            Links = new PagedResponseLinks()
        };
    }

    #endregion
}