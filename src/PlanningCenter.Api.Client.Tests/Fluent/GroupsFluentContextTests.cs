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
using PlanningCenter.Api.Client.Models.Groups;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Tests.Utilities;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Fluent;

/// <summary>
/// Unit tests for GroupsFluentContext.
/// Tests the fluent API functionality for the Groups module.
/// </summary>
public class GroupsFluentContextTests
{
    private readonly Mock<IGroupsService> _mockGroupsService;
    private readonly GroupsFluentContext _fluentContext;
    private readonly TestDataBuilder _builder = new();

    public GroupsFluentContextTests()
    {
        _mockGroupsService = new Mock<IGroupsService>();
        _fluentContext = new GroupsFluentContext(_mockGroupsService.Object);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenGroupsServiceIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new GroupsFluentContext(null!));
    }

    #endregion

    #region Query Building Tests

    [Fact]
    public void Where_ShouldReturnSameContext_WhenPredicateIsProvided()
    {
        // Act
        var result = _fluentContext.Where(g => g.Name == "Small Group");

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
        var result = _fluentContext.Include(g => g.GroupTypeId);

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void OrderBy_ShouldReturnSameContext_WhenOrderByExpressionIsProvided()
    {
        // Act
        var result = _fluentContext.OrderBy(g => g.Name);

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void OrderByDescending_ShouldReturnSameContext_WhenOrderByExpressionIsProvided()
    {
        // Act
        var result = _fluentContext.OrderByDescending(g => g.MembershipsCount);

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    #endregion

    #region Execution Method Tests

    [Fact]
    public async Task GetAsync_ShouldCallGroupsService_WhenIdIsProvided()
    {
        // Arrange
        var groupId = "123";
        var expectedGroup = BuildGroup(groupId);
        _mockGroupsService.Setup(s => s.GetGroupAsync(groupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedGroup);

        // Act
        var result = await _fluentContext.GetAsync(groupId);

        // Assert
        result.Should().BeSameAs(expectedGroup);
        _mockGroupsService.Verify(s => s.GetGroupAsync(groupId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _fluentContext.GetAsync(""));
    }

    [Fact]
    public async Task GetPagedAsync_ShouldCallGroupsServiceWithCorrectPageSize()
    {
        // Arrange
        var pageSize = 10;
        var pagedResponse = BuildGroupPagedResponse(pageSize);

        _mockGroupsService.Setup(s => s.ListGroupsAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        // Act
        var result = await _fluentContext.GetPagedAsync(pageSize);

        // Assert
        result.Should().BeSameAs(pagedResponse);
        _mockGroupsService.Verify(s => s.ListGroupsAsync(
            It.Is<QueryParameters>(p => p.PerPage == pageSize), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldCallGroupsServiceGetAllAsync()
    {
        // Arrange
        var expectedGroups = new List<Group> { BuildGroup("1"), BuildGroup("2") };
        _mockGroupsService.Setup(s => s.GetAllGroupsAsync(It.IsAny<QueryParameters>(), It.IsAny<PaginationOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedGroups);

        // Act
        var result = await _fluentContext.GetAllAsync();

        // Assert
        result.Should().BeSameAs(expectedGroups);
        _mockGroupsService.Verify(s => s.GetAllGroupsAsync(It.IsAny<QueryParameters>(), It.IsAny<PaginationOptions>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region LINQ-like Terminal Operations Tests

    [Fact]
    public async Task FirstAsync_ShouldReturnFirstGroup_WhenGroupsExist()
    {
        // Arrange
        var groups = new List<Group> { BuildGroup("1"), BuildGroup("2") };
        var pagedResponse = BuildGroupPagedResponse(groups);

        _mockGroupsService.Setup(s => s.ListGroupsAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        // Act
        var result = await _fluentContext.FirstAsync();

        // Assert
        result.Should().BeSameAs(groups[0]);
        _mockGroupsService.Verify(s => s.ListGroupsAsync(
            It.Is<QueryParameters>(p => p.PerPage == 1), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CountAsync_ShouldReturnTotalCount_WhenMetaHasTotalCount()
    {
        // Arrange
        var expectedCount = 42;
        var pagedResponse = new PagedResponse<Group>
        {
            Data = new List<Group> { BuildGroup("1") },
            Meta = new PagedResponseMeta { TotalCount = expectedCount },
            Links = new PagedResponseLinks()
        };

        _mockGroupsService.Setup(s => s.ListGroupsAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        // Act
        var result = await _fluentContext.CountAsync();

        // Assert
        result.Should().Be(expectedCount);
    }

    [Fact]
    public async Task AnyAsync_ShouldReturnTrue_WhenGroupsExist()
    {
        // Arrange
        var pagedResponse = BuildGroupPagedResponse(new List<Group> { BuildGroup("1") });

        _mockGroupsService.Setup(s => s.ListGroupsAsync(It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResponse);

        // Act
        var result = await _fluentContext.AnyAsync();

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region Specialized Groups Operations Tests

    [Fact]
    public void ByGroupType_ShouldReturnSameContext_WhenGroupTypeIdIsProvided()
    {
        // Act
        var result = _fluentContext.ByGroupType("small-group");

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void ByGroupType_ShouldThrowArgumentException_WhenGroupTypeIdIsEmpty()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _fluentContext.ByGroupType(""));
    }

    [Fact]
    public void ByLocation_ShouldReturnSameContext_WhenLocationIdIsProvided()
    {
        // Act
        var result = _fluentContext.ByLocation("main-campus");

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
    public void Active_ShouldReturnSameContext()
    {
        // Act
        var result = _fluentContext.Active();

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void Archived_ShouldReturnSameContext()
    {
        // Act
        var result = _fluentContext.Archived();

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void WithMinimumMembers_ShouldReturnSameContext_WhenValidCountIsProvided()
    {
        // Act
        var result = _fluentContext.WithMinimumMembers(5);

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void WithMinimumMembers_ShouldThrowArgumentException_WhenCountIsNegative()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _fluentContext.WithMinimumMembers(-1));
    }

    [Fact]
    public void WithMaximumMembers_ShouldReturnSameContext_WhenValidCountIsProvided()
    {
        // Act
        var result = _fluentContext.WithMaximumMembers(20);

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void WithMaximumMembers_ShouldThrowArgumentException_WhenCountIsNegative()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _fluentContext.WithMaximumMembers(-1));
    }

    [Fact]
    public void WithChatEnabled_ShouldReturnSameContext()
    {
        // Act
        var result = _fluentContext.WithChatEnabled();

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void WithVirtualMeeting_ShouldReturnSameContext()
    {
        // Act
        var result = _fluentContext.WithVirtualMeeting();

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void ByNameContains_ShouldReturnSameContext_WhenNameFragmentIsProvided()
    {
        // Act
        var result = _fluentContext.ByNameContains("Youth");

        // Assert
        result.Should().BeSameAs(_fluentContext);
    }

    [Fact]
    public void ByNameContains_ShouldThrowArgumentException_WhenNameFragmentIsEmpty()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _fluentContext.ByNameContains(""));
    }

    #endregion

    #region Chaining Tests

    [Fact]
    public void FluentChaining_ShouldAllowMethodChaining()
    {
        // Act & Assert (should not throw)
        var result = _fluentContext
            .Where(g => g.Name.Contains("Small"))
            .Active()
            .WithMinimumMembers(3)
            .WithMaximumMembers(12)
            .WithChatEnabled()
            .OrderBy(g => g.Name);

        result.Should().BeSameAs(_fluentContext);
    }

    #endregion

    #region Helper Methods

    private Group BuildGroup(string id)
    {
        var random = new Random();
        return new Group
        {
            Id = id,
            Name = $"Group {id}",
            Description = $"Description for group {id}",
            MembershipsCount = random.Next(1, 50),
            ArchivedAt = null,
            ChatEnabled = random.Next(0, 2) == 1,
            VirtualLocationUrl = random.Next(0, 2) == 1 ? "https://zoom.us/meeting" : null,
            CreatedAt = DateTime.Now.AddDays(-random.Next(30)),
            UpdatedAt = DateTime.Now.AddDays(-random.Next(10))
        };
    }

    private PagedResponse<Group> BuildGroupPagedResponse(int count)
    {
        var groups = Enumerable.Range(1, count).Select(i => BuildGroup(i.ToString())).ToList();
        return BuildGroupPagedResponse(groups);
    }

    private PagedResponse<Group> BuildGroupPagedResponse(List<Group> groups)
    {
        return new PagedResponse<Group>
        {
            Data = groups,
            Meta = new PagedResponseMeta { TotalCount = groups.Count },
            Links = new PagedResponseLinks()
        };
    }

    #endregion
}