using FluentAssertions;
using PlanningCenter.Api.Client.IntegrationTests.Infrastructure;
using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Groups;
using PlanningCenter.Api.Client.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PlanningCenter.Api.Client.IntegrationTests.Services;

/// <summary>
/// Integration tests for GroupsService.
/// Tests the basic CRUD operations against the real Planning Center API.
/// </summary>
[Collection("Integration Tests")]
public class GroupsServiceIntegrationTests : GroupsServiceIntegrationTestBase
{
    public GroupsServiceIntegrationTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task CreateGetUpdateDelete_Group_ShouldPerformFullLifecycle()
    {
        // Arrange
        var randomSuffix = _random.Next(10000, 99999);
        var groupTypeId = await GetGroupTypeIdAsync();
        var createRequest = new GroupCreateRequest
        {
            Name = $"Test Group {randomSuffix}",
            GroupTypeId = groupTypeId,
            Description = $"Test Group Description {randomSuffix}"
        };

        try
        {
            // Act - Create
            var createdGroup = await _groupsService.CreateGroupAsync(createRequest);

            // Assert - Create
            createdGroup.Should().NotBeNull();
            createdGroup.Id.Should().NotBeNullOrEmpty();
            createdGroup.Name.Should().Be(createRequest.Name);
            createdGroup.GroupTypeId.Should().Be(createRequest.GroupTypeId);
            createdGroup.Description.Should().Be(createRequest.Description);

            // Act - Get
            var retrievedGroup = await _groupsService.GetGroupAsync(createdGroup.Id);

            // Assert - Get
            retrievedGroup.Should().NotBeNull();
            retrievedGroup!.Id.Should().Be(createdGroup.Id);
            retrievedGroup.Name.Should().Be(createRequest.Name);
            retrievedGroup.GroupTypeId.Should().Be(createRequest.GroupTypeId);
            retrievedGroup.Description.Should().Be(createRequest.Description);

            // Act - Update
            var updateRequest = new GroupUpdateRequest
            {
                Name = $"Updated Group {randomSuffix}",
                Description = $"Updated Group Description {randomSuffix}"
            };

            var updatedGroup = await _groupsService.UpdateGroupAsync(createdGroup.Id, updateRequest);

            // Assert - Update
            updatedGroup.Should().NotBeNull();
            updatedGroup.Id.Should().Be(createdGroup.Id);
            updatedGroup.Name.Should().Be(updateRequest.Name);
            updatedGroup.Description.Should().Be(updateRequest.Description);
            updatedGroup.GroupTypeId.Should().Be(createdGroup.GroupTypeId); // Unchanged

            // Act - Delete
            await _groupsService.DeleteGroupAsync(createdGroup.Id);

            // Assert - Delete
            var deletedGroup = await _groupsService.GetGroupAsync(createdGroup.Id);
            deletedGroup.Should().BeNull();
        }
        catch (Exception ex)
        {
            Assert.Fail($"Test failed with exception: {ex.Message}");
        }
    }

    [Fact]
    public async Task GetGroupType_ShouldReturnGroupType()
    {
        // Arrange
        var groupTypeId = await GetGroupTypeIdAsync();

        // Act
        var groupType = await _groupsService.GetGroupTypeAsync(groupTypeId);

        // Assert
        groupType.Should().NotBeNull();
        groupType!.Id.Should().Be(groupTypeId);
        groupType.Name.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ListGroupTypes_ShouldReturnPaginatedResults()
    {
        // Arrange
        var parameters = new QueryParameters
        {
            PerPage = 5
        };

        // Act
        var result = await _groupsService.ListGroupTypesAsync(parameters);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Meta.Should().NotBeNull();
        result.Links.Should().NotBeNull();
        
        result.Data.Count.Should().BeLessThanOrEqualTo(5);
        result.Meta.PerPage.Should().Be(5);
    }

    [Fact]
    public async Task ListGroupMemberships_ShouldReturnPaginatedResults()
    {
        // Arrange - Create a test group first
        Group testGroup = null!;
        try
        {
            testGroup = await CreateTestGroupAsync();
            
            var parameters = new QueryParameters
            {
                PerPage = 5
            };

            // Act
            var result = await _groupsService.ListGroupMembershipsAsync(testGroup.Id, parameters);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().NotBeNull();
            result.Meta.Should().NotBeNull();
            result.Links.Should().NotBeNull();
            
            // Note: A newly created group might not have any memberships yet
            result.Data.Count.Should().BeLessThanOrEqualTo(5);
            result.Meta.PerPage.Should().Be(5);
        }
        finally
        {
            // Cleanup
            if (testGroup != null)
            {
                await CleanupTestGroupAsync(testGroup.Id);
            }
        }
    }

    [Fact]
    public async Task ListGroups_ShouldReturnPaginatedResults()
    {
        // Arrange
        var parameters = new QueryParameters
        {
            PerPage = 5
        };

        // Act
        var result = await _groupsService.ListGroupsAsync(parameters);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Meta.Should().NotBeNull();
        result.Links.Should().NotBeNull();
        
        result.Data.Count.Should().BeLessThanOrEqualTo(5);
        result.Meta.PerPage.Should().Be(5);
    }

    [Fact]
    public async Task GetAllGroupsAsync_ShouldReturnAllResults()
    {
        // Arrange
        var parameters = new QueryParameters
        {
            PerPage = 5
        };

        // Act
        var result = await _groupsService.GetAllGroupsAsync(parameters);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task StreamGroupsAsync_ShouldStreamResults()
    {
        // Arrange
        var parameters = new QueryParameters
        {
            PerPage = 5
        };

        // Act
        var groups = new List<Group>();
        await foreach (var group in _groupsService.StreamGroupsAsync(parameters))
        {
            groups.Add(group);
            
            // Limit to 10 groups to avoid long-running test
            if (groups.Count >= 10)
                break;
        }

        // Assert
        groups.Should().NotBeEmpty();
    }
}
