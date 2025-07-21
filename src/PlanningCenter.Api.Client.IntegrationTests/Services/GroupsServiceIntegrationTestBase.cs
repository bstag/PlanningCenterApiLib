using Microsoft.Extensions.DependencyInjection;
using PlanningCenter.Api.Client.IntegrationTests.Infrastructure;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Groups;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PlanningCenter.Api.Client.IntegrationTests.Services;

/// <summary>
/// Base class for GroupsService integration tests.
/// Provides common functionality and test data management.
/// </summary>
public abstract class GroupsServiceIntegrationTestBase : IClassFixture<TestFixture>
{
    protected readonly TestFixture _fixture;
    protected readonly IGroupsService _groupsService;
    protected readonly Random _random = new Random();

    protected GroupsServiceIntegrationTestBase(TestFixture fixture)
    {
        _fixture = fixture;
        _groupsService = _fixture.ServiceProvider.GetRequiredService<IGroupsService>();
    }

    /// <summary>
    /// Checks if the test should be skipped due to missing authentication.
    /// </summary>
    protected void SkipIfNoAuthentication()
    {
        if (!_fixture.HasRealAuthentication)
        {
            return; // Skip test
        }
    }

    /// <summary>
    /// Gets a valid group type ID for testing.
    /// </summary>
    protected async Task<string> GetGroupTypeIdAsync()
    {
        var groupTypes = await _groupsService.ListGroupTypesAsync(new QueryParameters { PerPage = 1 });
        if (groupTypes.Data.Count == 0)
        {
            throw new InvalidOperationException("No group types found. Tests cannot proceed.");
        }
        return groupTypes.Data[0].Id;
    }

    /// <summary>
    /// Creates a test group with random data.
    /// </summary>
    protected async Task<Group> CreateTestGroupAsync()
    {
        var randomSuffix = _random.Next(10000, 99999);
        var groupTypeId = await GetGroupTypeIdAsync();
        
        var request = new GroupCreateRequest
        {
            Name = $"Test Group {randomSuffix}",
            GroupTypeId = groupTypeId,
            Description = $"Test Group Description {randomSuffix}"
        };

        return await _groupsService.CreateGroupAsync(request);
    }

    /// <summary>
    /// Cleans up a test group after tests.
    /// </summary>
    protected async Task CleanupTestGroupAsync(string groupId)
    {
        if (!string.IsNullOrEmpty(groupId))
        {
            try
            {
                await _groupsService.DeleteGroupAsync(groupId);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }
}
