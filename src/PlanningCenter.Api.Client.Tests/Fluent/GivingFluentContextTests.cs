using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using PlanningCenter.Api.Client.Fluent;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.Giving;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Tests.Utilities;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Fluent;

/// <summary>
/// Unit tests for GivingFluentContext.
/// Tests the fluent API functionality for the Giving module.
/// </summary>
public class GivingFluentContextTests
{
    private readonly Mock<IGivingService> _mockGivingService;
    private readonly GivingFluentContext _fluentContext;
    private readonly TestDataBuilder _builder = new();

    public GivingFluentContextTests()
    {
        _mockGivingService = new Mock<IGivingService>();
        _fluentContext = new GivingFluentContext(_mockGivingService.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenGivingServiceIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new GivingFluentContext(null!));
    }

    #endregion

    #region Query Building Tests

    [Fact]
    public void Where_ShouldReturnSameContext_WhenPredicateIsProvided()
    {
        // Act
        var result = _fluentContext.Where(d => d.AmountCents > 1000);

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
        var result = _fluentContext.Include(d => d.PersonId!);

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void OrderBy_ShouldReturnSameContext_WhenOrderByExpressionIsProvided()
    {
        // Act
        var result = _fluentContext.OrderBy(d => d.ReceivedAt!);

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void OrderByDescending_ShouldReturnSameContext_WhenOrderByExpressionIsProvided()
    {
        // Act
        var result = _fluentContext.OrderByDescending(d => d.AmountCents);

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    #endregion

    #region Execution Method Tests

    [Fact]
    public async Task GetAsync_ShouldCallGivingService_WhenIdIsProvided()
    {
        // Arrange
        var donationId = "123";
        var expectedDonation = BuildDonation(donationId);
        _mockGivingService.Setup(s => s.GetDonationAsync(donationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDonation);

        // Act
        var result = await _fluentContext.GetAsync(donationId);

        // Assert
        result.Should().BeSameAs(expectedDonation);
        _mockGivingService.Verify(s => s.GetDonationAsync(donationId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _fluentContext.GetAsync(""));
    }

    [Fact]
    public async Task GetPagedAsync_ShouldCallGivingServiceWithCorrectPageSize()
    {
        // Arrange
        var pageSize = 10;
        var pagedResponse = BuildDonationPagedResponse(pageSize);

        _mockGivingService.Setup(s => s.ListDonationsAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        // Act
        var result = await _fluentContext.GetPagedAsync(pageSize);

        // Assert
        result.Should().BeSameAs(pagedResponse);
        _mockGivingService.Verify(s => s.ListDonationsAsync(
            It.Is<QueryParameters>(p => p.PerPage == pageSize), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldCallGivingServiceGetAllAsync()
    {
        // Arrange
        var expectedDonations = new List<Donation> { BuildDonation("1"), BuildDonation("2") };
        _mockGivingService.Setup(s => s.GetAllDonationsAsync(It.IsAny<QueryParameters>(), It.IsAny<PaginationOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDonations);

        // Act
        var result = await _fluentContext.GetAllAsync();

        // Assert
        result.Should().BeSameAs(expectedDonations);
        _mockGivingService.Verify(s => s.GetAllDonationsAsync(It.IsAny<QueryParameters>(), It.IsAny<PaginationOptions>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region LINQ-like Terminal Operations Tests

    [Fact]
    public async Task FirstAsync_ShouldReturnFirstDonation_WhenDonationsExist()
    {
        // Arrange
        var donations = new List<Donation> { BuildDonation("1"), BuildDonation("2") };
        var pagedResponse = BuildDonationPagedResponse(donations);

        _mockGivingService.Setup(s => s.ListDonationsAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        // Act
        var result = await _fluentContext.FirstAsync();

        // Assert
        result.Should().BeSameAs(donations[0]);
        _mockGivingService.Verify(s => s.ListDonationsAsync(
            It.Is<QueryParameters>(p => p.PerPage == 1), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task FirstAsync_ShouldThrowInvalidOperationException_WhenNoDonationsExist()
    {
        // Arrange
        var pagedResponse = BuildDonationPagedResponse(new List<Donation>());

        _mockGivingService.Setup(s => s.ListDonationsAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _fluentContext.FirstAsync());
    }

    [Fact]
    public async Task CountAsync_ShouldReturnTotalCount_WhenMetaHasTotalCount()
    {
        // Arrange
        var expectedCount = 42;
        var pagedResponse = new PagedResponse<Donation>
        {
            Data = new List<Donation> { BuildDonation("1") },
            Meta = new PagedResponseMeta { TotalCount = expectedCount },
            Links = new PagedResponseLinks()
        };

        _mockGivingService.Setup(s => s.ListDonationsAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        // Act
        var result = await _fluentContext.CountAsync();

        // Assert
        result.Should().Be(expectedCount);
    }

    [Fact]
    public async Task AnyAsync_ShouldReturnTrue_WhenDonationsExist()
    {
        // Arrange
        var pagedResponse = BuildDonationPagedResponse(new List<Donation> { BuildDonation("1") });

        _mockGivingService.Setup(s => s.ListDonationsAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        // Act
        var result = await _fluentContext.AnyAsync();

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region Specialized Giving Operations Tests

    [Fact]
    public void ByFund_ShouldReturnSameContext_WhenFundIdIsProvided()
    {
        // Act
        var result = _fluentContext.ByFund("fund-123");

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void ByFund_ShouldThrowArgumentException_WhenFundIdIsEmpty()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _fluentContext.ByFund(""));
    }

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
    public void ByDateRange_ShouldReturnSameContext_WhenValidDateRangeIsProvided()
    {
        // Arrange
        var startDate = DateTime.Now.AddMonths(-1);
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
        var endDate = DateTime.Now.AddMonths(-1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _fluentContext.ByDateRange(startDate, endDate));
    }

    [Fact]
    public void WithMinimumAmount_ShouldReturnSameContext_WhenValidAmountIsProvided()
    {
        // Act
        var result = _fluentContext.WithMinimumAmount(1000);

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void WithMinimumAmount_ShouldThrowArgumentException_WhenAmountIsNegative()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _fluentContext.WithMinimumAmount(-100));
    }

    [Fact]
    public async Task TotalAmountAsync_ShouldReturnSumOfAmountCents_WhenDonationsExist()
    {
        // Arrange
        var donations = new List<Donation> 
        { 
            BuildDonation("1", 1000), 
            BuildDonation("2", 2000), 
            BuildDonation("3", 3000) 
        };
        var expectedTotal = 6000L;

        _mockGivingService.Setup(s => s.GetAllDonationsAsync(It.IsAny<QueryParameters>(), It.IsAny<PaginationOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(donations);

        // Act
        var result = await _fluentContext.TotalAmountAsync();

        // Assert
        result.Should().Be(expectedTotal);
    }

    [Fact]
    public async Task TotalAmountAsync_ShouldReturnZero_WhenNoDonationsExist()
    {
        // Arrange
        _mockGivingService.Setup(s => s.GetAllDonationsAsync(It.IsAny<QueryParameters>(), It.IsAny<PaginationOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Donation>());

        // Act
        var result = await _fluentContext.TotalAmountAsync();

        // Assert
        result.Should().Be(0);
    }

    #endregion

    #region Chaining Tests

    [Fact]
    public void FluentChaining_ShouldAllowMethodChaining()
    {
        // Act & Assert (should not throw)
        var result = _fluentContext
            .Where(d => d.AmountCents > 1000)
            .ByFund("fund-123")
            .ByDateRange(DateTime.Now.AddMonths(-1), DateTime.Now)
            .WithMinimumAmount(500)
            .OrderByDescending(d => d.ReceivedAt!);

        result.Should().BeSameAs(_fluentContext);
    }

    #endregion

    #region Helper Methods

    private Donation BuildDonation(string id, long amountCents = 1000)
    {
        return new Donation
        {
            Id = id,
            AmountCents = amountCents,
            ReceivedAt = DateTime.Now.AddDays(-new Random().Next(30)),
            CreatedAt = DateTime.Now.AddDays(-new Random().Next(30)),
            UpdatedAt = DateTime.Now.AddDays(-new Random().Next(10))
        };
    }

    private PagedResponse<Donation> BuildDonationPagedResponse(int count)
    {
        var donations = Enumerable.Range(1, count).Select(i => BuildDonation(i.ToString())).ToList();
        return BuildDonationPagedResponse(donations);
    }

    private PagedResponse<Donation> BuildDonationPagedResponse(List<Donation> donations)
    {
        return new PagedResponse<Donation>
        {
            Data = donations,
            Meta = new PagedResponseMeta { TotalCount = donations.Count },
            Links = new PagedResponseLinks()
        };
    }

    #endregion
}