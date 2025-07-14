using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using PlanningCenter.Api.Client.Models.JsonApi;
using PlanningCenter.Api.Client.Models.JsonApi.Groups;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Services;
using PlanningCenter.Api.Client.Tests.Utilities;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Services;

/// <summary>
/// Unit tests for GroupsService.
/// Follows the same patterns as PeopleServiceTests.
/// </summary>
public class GroupsServiceTests
{
    private readonly MockApiConnection _mockApiConnection = new();
    private readonly GroupsService _groupsService;
    private readonly ExtendedTestDataBuilder _builder = new();

    public GroupsServiceTests()
    {
        _groupsService = new GroupsService(_mockApiConnection, NullLogger<GroupsService>.Instance);
    }

    #region Group Tests

    [Fact]
    public async Task GetGroupAsync_ShouldReturnGroup_WhenApiReturnsData()
    {
        // Arrange
        var groupDto = _builder.CreateGroupDto(g => g.Id = "group-123");
        var response = new JsonApiSingleResponse<GroupDto> { Data = groupDto };
        _mockApiConnection.SetupGetResponse("/groups/v2/groups/group-123", response);

        // Act
        var result = await _groupsService.GetGroupAsync("group-123");

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("group-123");
        result.Name.Should().Be(groupDto.Attributes.Name);
        result.DataSource.Should().Be("Groups");
    }

    [Fact]
    public async Task GetGroupAsync_ShouldReturnNull_WhenApiReturnsNull()
    {
        // Arrange
        var response = new JsonApiSingleResponse<GroupDto> { Data = null };
        _mockApiConnection.SetupGetResponse("/groups/v2/groups/nonexistent", response);

        // Act
        var result = await _groupsService.GetGroupAsync("nonexistent");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetGroupAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _groupsService.GetGroupAsync(""));
    }

    [Fact]
    public async Task ListGroupsAsync_ShouldReturnPagedGroups_WhenApiReturnsData()
    {
        // Arrange
        var groupDtos = new[]
        {
            _builder.CreateGroupDto(g => g.Id = "1"),
            _builder.CreateGroupDto(g => g.Id = "2")
        };
        _mockApiConnection.SetupGetResponse("/groups/v2/groups", groupDtos);

        // Act
        var result = await _groupsService.ListGroupsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Data[0].Id.Should().Be("1");
        result.Data[1].Id.Should().Be("2");
        result.Meta.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateGroupAsync_ShouldReturnCreatedGroup_WhenValidRequest()
    {
        // Arrange
        var request = new GroupCreateRequest
        {
            Name = "Small Group Alpha",
            GroupTypeId = "group-type-123",
            Description = "A small group for fellowship",
            ContactEmail = "leader@example.com",
            ChatEnabled = true
        };

        var createdGroupDto = _builder.CreateGroupDto(g =>
        {
            g.Id = "new-group-123";
            g.Attributes.Name = request.Name;
            g.Attributes.Description = request.Description;
        });

        var response = new JsonApiSingleResponse<GroupDto> { Data = createdGroupDto };
        _mockApiConnection.SetupMutationResponse("POST", "/groups/v2/groups", response);

        // Act
        var result = await _groupsService.CreateGroupAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("new-group-123");
        result.Name.Should().Be("Small Group Alpha");
        result.Description.Should().Be("A small group for fellowship");
        result.DataSource.Should().Be("Groups");
    }

    [Fact]
    public async Task CreateGroupAsync_ShouldThrowArgumentException_WhenNameIsEmpty()
    {
        // Arrange
        var request = new GroupCreateRequest
        {
            Name = "",
            GroupTypeId = "group-type-123"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _groupsService.CreateGroupAsync(request));
    }

    [Fact]
    public async Task CreateGroupAsync_ShouldThrowArgumentException_WhenGroupTypeIdIsEmpty()
    {
        // Arrange
        var request = new GroupCreateRequest
        {
            Name = "Test Group",
            GroupTypeId = ""
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _groupsService.CreateGroupAsync(request));
    }

    [Fact]
    public async Task UpdateGroupAsync_ShouldReturnUpdatedGroup_WhenValidRequest()
    {
        // Arrange
        var request = new GroupUpdateRequest
        {
            Name = "Updated Group Name",
            Description = "Updated description",
            ChatEnabled = false
        };

        var updatedGroupDto = _builder.CreateGroupDto(g =>
        {
            g.Id = "group-123";
            g.Attributes.Name = "Updated Group Name";
            g.Attributes.Description = "Updated description";
            g.Attributes.ChatEnabled = false;
        });

        var response = new JsonApiSingleResponse<GroupDto> { Data = updatedGroupDto };
        _mockApiConnection.SetupMutationResponse("PATCH", "/groups/v2/groups/group-123", response);

        // Act
        var result = await _groupsService.UpdateGroupAsync("group-123", request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("group-123");
        result.Name.Should().Be("Updated Group Name");
        result.Description.Should().Be("Updated description");
        result.ChatEnabled.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteGroupAsync_ShouldCompleteSuccessfully_WhenValidId()
    {
        // Act & Assert (should not throw)
        await _groupsService.DeleteGroupAsync("group-123");
    }

    #endregion

    #region Group Type Tests

    [Fact]
    public async Task GetGroupTypeAsync_ShouldReturnGroupType_WhenApiReturnsData()
    {
        // Arrange
        var groupTypeDto = _builder.CreateGroupTypeDto(gt => gt.Id = "group-type-123");
        var response = new JsonApiSingleResponse<GroupTypeDto> { Data = groupTypeDto };
        _mockApiConnection.SetupGetResponse("/groups/v2/group_types/group-type-123", response);

        // Act
        var result = await _groupsService.GetGroupTypeAsync("group-type-123");

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("group-type-123");
        result.Name.Should().Be(groupTypeDto.Attributes.Name);
        result.DataSource.Should().Be("Groups");
    }

    [Fact]
    public async Task ListGroupTypesAsync_ShouldReturnPagedGroupTypes_WhenApiReturnsData()
    {
        // Arrange
        var groupTypeDtos = new[]
        {
            _builder.CreateGroupTypeDto(gt => gt.Id = "1"),
            _builder.CreateGroupTypeDto(gt => gt.Id = "2")
        };
        _mockApiConnection.SetupGetResponse("/groups/v2/group_types", groupTypeDtos);

        // Act
        var result = await _groupsService.ListGroupTypesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Data[0].Id.Should().Be("1");
        result.Data[1].Id.Should().Be("2");
    }

    #endregion

    #region Membership Tests

    [Fact]
    public async Task GetGroupMembershipAsync_ShouldReturnMembership_WhenApiReturnsData()
    {
        // Arrange
        var membershipDto = _builder.CreateMembershipDto(m => m.Id = "membership-123");
        var response = new JsonApiSingleResponse<MembershipDto> { Data = membershipDto };
        _mockApiConnection.SetupGetResponse("/groups/v2/groups/group-123/memberships/membership-123", response);

        // Act
        var result = await _groupsService.GetGroupMembershipAsync("group-123", "membership-123");

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("membership-123");
        result.Role.Should().Be(membershipDto.Attributes.Role);
        result.DataSource.Should().Be("Groups");
    }

    [Fact]
    public async Task ListGroupMembershipsAsync_ShouldReturnPagedMemberships_WhenApiReturnsData()
    {
        // Arrange
        var membershipDtos = new[]
        {
            _builder.CreateMembershipDto(m => m.Id = "1"),
            _builder.CreateMembershipDto(m => m.Id = "2")
        };
        _mockApiConnection.SetupGetResponse("/groups/v2/groups/group-123/memberships", membershipDtos);

        // Act
        var result = await _groupsService.ListGroupMembershipsAsync("group-123");

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Data[0].Id.Should().Be("1");
        result.Data[1].Id.Should().Be("2");
    }

    [Fact]
    public async Task CreateGroupMembershipAsync_ShouldReturnCreatedMembership_WhenValidRequest()
    {
        // Arrange
        var request = new MembershipCreateRequest
        {
            PersonId = "person-123",
            Role = "Leader",
            CanViewMembers = true,
            CanManageMembers = true
        };

        var createdMembershipDto = _builder.CreateMembershipDto(m =>
        {
            m.Id = "new-membership-123";
            m.Attributes.Role = request.Role;
            m.Attributes.CanViewMembers = request.CanViewMembers;
            m.Attributes.CanManageMembers = request.CanManageMembers;
        });

        var response = new JsonApiSingleResponse<MembershipDto> { Data = createdMembershipDto };
        _mockApiConnection.SetupMutationResponse("POST", "/groups/v2/groups/group-123/memberships", response);

        // Act
        var result = await _groupsService.CreateGroupMembershipAsync("group-123", request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("new-membership-123");
        result.Role.Should().Be("Leader");
        result.CanViewMembers.Should().BeTrue();
        result.CanManageMembers.Should().BeTrue();
    }

    [Fact]
    public async Task CreateGroupMembershipAsync_ShouldThrowArgumentException_WhenPersonIdIsEmpty()
    {
        // Arrange
        var request = new MembershipCreateRequest
        {
            PersonId = "",
            Role = "Member"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _groupsService.CreateGroupMembershipAsync("group-123", request));
    }

    [Fact]
    public async Task UpdateGroupMembershipAsync_ShouldReturnUpdatedMembership_WhenValidRequest()
    {
        // Arrange
        var request = new MembershipUpdateRequest
        {
            Role = "Co-Leader",
            CanCreateEvents = true
        };

        var updatedMembershipDto = _builder.CreateMembershipDto(m =>
        {
            m.Id = "membership-123";
            m.Attributes.Role = "Co-Leader";
            m.Attributes.CanCreateEvents = true;
        });

        var response = new JsonApiSingleResponse<MembershipDto> { Data = updatedMembershipDto };
        _mockApiConnection.SetupMutationResponse("PATCH", "/groups/v2/groups/group-123/memberships/membership-123", response);

        // Act
        var result = await _groupsService.UpdateGroupMembershipAsync("group-123", "membership-123", request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("membership-123");
        result.Role.Should().Be("Co-Leader");
        result.CanCreateEvents.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteGroupMembershipAsync_ShouldCompleteSuccessfully_WhenValidIds()
    {
        // Act & Assert (should not throw)
        await _groupsService.DeleteGroupMembershipAsync("group-123", "membership-123");
    }

    #endregion

    #region Pagination Tests

    [Fact]
    public async Task GetAllGroupsAsync_ShouldReturnAllGroups_WhenMultiplePagesExist()
    {
        // Arrange
        var groupDtos = new[]
        {
            _builder.CreateGroupDto(g => g.Id = "1"),
            _builder.CreateGroupDto(g => g.Id = "2"),
            _builder.CreateGroupDto(g => g.Id = "3")
        };
        _mockApiConnection.SetupGetResponse("/groups/v2/groups", groupDtos);

        // Act
        var result = await _groupsService.GetAllGroupsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result[0].Id.Should().Be("1");
        result[1].Id.Should().Be("2");
        result[2].Id.Should().Be("3");
    }

    [Fact]
    public async Task StreamGroupsAsync_ShouldYieldGroups_WhenDataExists()
    {
        // Arrange
        var groupDtos = new[]
        {
            _builder.CreateGroupDto(g => g.Id = "1"),
            _builder.CreateGroupDto(g => g.Id = "2")
        };
        _mockApiConnection.SetupGetResponse("/groups/v2/groups", groupDtos);

        // Act
        var groups = new List<Models.Groups.Group>();
        await foreach (var group in _groupsService.StreamGroupsAsync())
        {
            groups.Add(group);
        }

        // Assert
        groups.Should().HaveCount(2);
        groups[0].Id.Should().Be("1");
        groups[1].Id.Should().Be("2");
    }

    #endregion
}