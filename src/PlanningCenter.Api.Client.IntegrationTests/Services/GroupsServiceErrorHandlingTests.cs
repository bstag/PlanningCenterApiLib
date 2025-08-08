using FluentAssertions;
using PlanningCenter.Api.Client.IntegrationTests.Infrastructure;
using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Exceptions;
using PlanningCenter.Api.Client.Models.Requests;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PlanningCenter.Api.Client.IntegrationTests.Services;

/// <summary>
/// Integration tests for GroupsService error handling.
/// Tests how the client library handles API errors and edge cases.
/// </summary>
[Collection("Integration Tests")]
public class GroupsServiceErrorHandlingTests : GroupsServiceIntegrationTestBase
{
    public GroupsServiceErrorHandlingTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task GetGroupAsync_ShouldReturnNull_WhenGroupDoesNotExist()
    {
        // Arrange - Use a non-existent ID
        var nonExistentId = "non-existent-id";

        // Act
        var result = await _groupsService.GetGroupAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetGroupTypeAsync_ShouldReturnNull_WhenGroupTypeDoesNotExist()
    {
        // Arrange - Use a non-existent ID
        var nonExistentId = "non-existent-id";

        // Act
        var result = await _groupsService.GetGroupTypeAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetGroupMembershipAsync_ShouldReturnNull_WhenMembershipDoesNotExist()
    {
        // Arrange - Create a test group first
        var testGroup = await CreateTestGroupAsync();
        try
        {
            // Act
            var result = await _groupsService.GetGroupMembershipAsync(testGroup.Id, "non-existent-id");

            // Assert
            result.Should().BeNull();
        }
        finally
        {
            await CleanupTestGroupAsync(testGroup.Id);
        }
    }

    [Fact]
    public async Task CreateGroupAsync_ShouldThrowValidationException_WhenRequiredFieldsAreMissing()
    {
        // Arrange - Create an invalid request with missing required fields
        var invalidRequest = new GroupCreateRequest
        {
            // Missing Name which is required
            GroupTypeId = "1" // This might not be a valid ID, but validation should fail on Name first
        };

        // Act & Assert
        await Assert.ThrowsAsync<PlanningCenterApiValidationException>(() => 
            _groupsService.CreateGroupAsync(invalidRequest));
    }

    [Fact]
    public async Task CreateGroupAsync_ShouldThrowValidationException_WhenGroupTypeIdIsMissing()
    {
        // Arrange - Create an invalid request with missing required fields
        var invalidRequest = new GroupCreateRequest
        {
            Name = "Test Group",
            // Missing GroupTypeId which is required
        };

        // Act & Assert
        await Assert.ThrowsAsync<PlanningCenterApiValidationException>(() => 
            _groupsService.CreateGroupAsync(invalidRequest));
    }

    [Fact]
    public async Task UpdateGroupAsync_ShouldThrowNotFoundException_WhenGroupDoesNotExist()
    {
        // Arrange
        var nonExistentId = "non-existent-id";
        var updateRequest = new GroupUpdateRequest
        {
            Name = "Updated Group"
        };

        // Act & Assert
        await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(() => 
            _groupsService.UpdateGroupAsync(nonExistentId, updateRequest));
    }

    [Fact]
    public async Task DeleteGroupAsync_ShouldThrowNotFoundException_WhenGroupDoesNotExist()
    {
        // Arrange
        var nonExistentId = "non-existent-id";

        // Act & Assert
        await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(() => 
            _groupsService.DeleteGroupAsync(nonExistentId));
    }

    [Fact]
    public async Task ListGroupMembershipsAsync_ShouldThrowNotFoundException_WhenGroupDoesNotExist()
    {
        // Arrange
        var nonExistentId = "non-existent-id";
        var parameters = new QueryParameters { PerPage = 5 };

        // Act & Assert
        await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(() => 
            _groupsService.ListGroupMembershipsAsync(nonExistentId, parameters));
    }

    [Fact]
    public async Task ArgumentValidation_ShouldThrowArgumentException_WhenInvalidArgumentsProvided()
    {
        // Act & Assert - Test null or empty ID
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _groupsService.GetGroupAsync(string.Empty));
            
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _groupsService.GetGroupTypeAsync(string.Empty));
            
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _groupsService.GetGroupMembershipAsync(string.Empty, "valid-id"));
            
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _groupsService.GetGroupMembershipAsync("valid-id", string.Empty));

        // Act & Assert - Test null request
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _groupsService.CreateGroupAsync(null!));
            
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _groupsService.UpdateGroupAsync("valid-id", null!));
    }
}
