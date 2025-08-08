using FluentAssertions;
using PlanningCenter.Api.Client.IntegrationTests.Infrastructure;
using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Services;
using PlanningCenter.Api.Client.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PlanningCenter.Api.Client.IntegrationTests.Services;

/// <summary>
/// Integration tests for ServicesService.
/// Tests the basic CRUD operations against the real Planning Center API.
/// </summary>
[Collection("Integration Tests")]
public class ServicesServiceIntegrationTests : ServicesServiceIntegrationTestBase
{
    public ServicesServiceIntegrationTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task CreateGetUpdateDelete_Plan_ShouldPerformFullLifecycle()
    {
        // Arrange
        var randomSuffix = _random.Next(10000, 99999);
        var serviceTypeId = await GetServiceTypeIdAsync();
        var createRequest = new PlanCreateRequest
        {
            Title = $"Test Plan {randomSuffix}",
            ServiceTypeId = serviceTypeId,
            SortDate = DateTime.Now.AddDays(7).Date
        };

        try
        {
            // Act - Create
            var createdPlan = await _servicesService.CreatePlanAsync(createRequest);

            // Assert - Create
            createdPlan.Should().NotBeNull();
            createdPlan.Id.Should().NotBeNullOrEmpty();
            createdPlan.Title.Should().Be(createRequest.Title);
            createdPlan.ServiceTypeId.Should().Be(createRequest.ServiceTypeId);
            createdPlan.SortDate.Should().Be(createRequest.SortDate);

            // Act - Get
            var retrievedPlan = await _servicesService.GetPlanAsync(createdPlan.Id);

            // Assert - Get
            retrievedPlan.Should().NotBeNull();
            retrievedPlan!.Id.Should().Be(createdPlan.Id);
            retrievedPlan.Title.Should().Be(createRequest.Title);
            retrievedPlan.ServiceTypeId.Should().Be(createRequest.ServiceTypeId);

            // Act - Update
            var updateRequest = new PlanUpdateRequest
            {
                Title = $"Updated Plan {randomSuffix}",
                Notes = "Updated notes for testing"
            };

            var updatedPlan = await _servicesService.UpdatePlanAsync(createdPlan.Id, updateRequest);

            // Assert - Update
            updatedPlan.Should().NotBeNull();
            updatedPlan.Id.Should().Be(createdPlan.Id);
            updatedPlan.Title.Should().Be(updateRequest.Title);
            updatedPlan.Notes.Should().Be(updateRequest.Notes);
            updatedPlan.ServiceTypeId.Should().Be(createdPlan.ServiceTypeId); // Unchanged

            // Act - Delete
            await _servicesService.DeletePlanAsync(createdPlan.Id);

            // Assert - Delete
            var deletedPlan = await _servicesService.GetPlanAsync(createdPlan.Id);
            deletedPlan.Should().BeNull();
        }
        catch (Exception ex)
        {
            Assert.Fail($"Test failed with exception: {ex.Message}");
        }
    }

    [Fact]
    public async Task GetServiceType_ShouldReturnServiceType()
    {
        // Arrange
        var serviceTypeId = await GetServiceTypeIdAsync();

        // Act
        var serviceType = await _servicesService.GetServiceTypeAsync(serviceTypeId);

        // Assert
        serviceType.Should().NotBeNull();
        serviceType!.Id.Should().Be(serviceTypeId);
        serviceType.Name.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ListServiceTypes_ShouldReturnPaginatedResults()
    {
        // Arrange
        var parameters = new QueryParameters
        {
            PerPage = 5
        };

        // Act
        var result = await _servicesService.ListServiceTypesAsync(parameters);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Meta.Should().NotBeNull();
        result.Links.Should().NotBeNull();
        
        result.Data.Count.Should().BeLessThanOrEqualTo(5);
        result.Meta.PerPage.Should().Be(5);
    }

    [Fact]
    public async Task CreateGetUpdateDelete_PlanItem_ShouldPerformFullLifecycle()
    {
        // Arrange - Create a test plan first
        Plan testPlan = null!;
        try
        {
            testPlan = await CreateTestPlanAsync();
            
            // Arrange - Create item request
            var randomSuffix = _random.Next(10000, 99999);
            var createRequest = new ItemCreateRequest
            {
                Title = $"Test Item {randomSuffix}",
                Description = $"Test Item Description {randomSuffix}",
                Length = 300 // 5 minutes in seconds
            };

            // Act - Create
            var createdItem = await _servicesService.CreatePlanItemAsync(testPlan.Id, createRequest);

            // Assert - Create
            createdItem.Should().NotBeNull();
            createdItem.Id.Should().NotBeNullOrEmpty();
            createdItem.Title.Should().Be(createRequest.Title);
            createdItem.Description.Should().Be(createRequest.Description);
            createdItem.Length.Should().Be(createRequest.Length);

            // Act - Get
            var retrievedItem = await _servicesService.GetPlanItemAsync(testPlan.Id, createdItem.Id);

            // Assert - Get
            retrievedItem.Should().NotBeNull();
            retrievedItem!.Id.Should().Be(createdItem.Id);
            retrievedItem.Title.Should().Be(createRequest.Title);
            retrievedItem.Description.Should().Be(createRequest.Description);

            // Act - Update
            var updateRequest = new ItemUpdateRequest
            {
                Title = $"Updated Item {randomSuffix}",
                Description = $"Updated Item Description {randomSuffix}"
            };

            var updatedItem = await _servicesService.UpdatePlanItemAsync(testPlan.Id, createdItem.Id, updateRequest);

            // Assert - Update
            updatedItem.Should().NotBeNull();
            updatedItem.Id.Should().Be(createdItem.Id);
            updatedItem.Title.Should().Be(updateRequest.Title);
            updatedItem.Description.Should().Be(updateRequest.Description);
            updatedItem.Length.Should().Be(createdItem.Length); // Unchanged

            // Act - Delete
            await _servicesService.DeletePlanItemAsync(testPlan.Id, createdItem.Id);

            // Assert - Delete
            var deletedItem = await _servicesService.GetPlanItemAsync(testPlan.Id, createdItem.Id);
            deletedItem.Should().BeNull();
        }
        finally
        {
            // Cleanup
            if (testPlan != null)
            {
                await CleanupTestPlanAsync(testPlan.Id);
            }
        }
    }

    [Fact]
    public async Task ListPlanItems_ShouldReturnPaginatedResults()
    {
        // Arrange - Create a test plan first
        Plan testPlan = null!;
        try
        {
            testPlan = await CreateTestPlanAsync();
            
            // Create a few items
            await CreateTestPlanItemAsync(testPlan.Id);
            await CreateTestPlanItemAsync(testPlan.Id);
            
            var parameters = new QueryParameters
            {
                PerPage = 5
            };

            // Act
            var result = await _servicesService.ListPlanItemsAsync(testPlan.Id, parameters);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().NotBeNull();
            result.Meta.Should().NotBeNull();
            result.Links.Should().NotBeNull();
            
            result.Data.Count.Should().BeGreaterThan(0);
            result.Data.Count.Should().BeLessThanOrEqualTo(5);
            result.Meta.PerPage.Should().Be(5);
        }
        finally
        {
            // Cleanup
            if (testPlan != null)
            {
                await CleanupTestPlanAsync(testPlan.Id);
            }
        }
    }

    [Fact]
    public async Task CreateGetUpdateDelete_Song_ShouldPerformFullLifecycle()
    {
        // Arrange
        var randomSuffix = _random.Next(10000, 99999);
        var createRequest = new SongCreateRequest
        {
            Title = $"Test Song {randomSuffix}",
            Author = $"Test Author {randomSuffix}"
        };

        try
        {
            // Act - Create
            var createdSong = await _servicesService.CreateSongAsync(createRequest);

            // Assert - Create
            createdSong.Should().NotBeNull();
            createdSong.Id.Should().NotBeNullOrEmpty();
            createdSong.Title.Should().Be(createRequest.Title);
            createdSong.Author.Should().Be(createRequest.Author);

            // Act - Get
            var retrievedSong = await _servicesService.GetSongAsync(createdSong.Id);

            // Assert - Get
            retrievedSong.Should().NotBeNull();
            retrievedSong!.Id.Should().Be(createdSong.Id);
            retrievedSong.Title.Should().Be(createRequest.Title);
            retrievedSong.Author.Should().Be(createRequest.Author);

            // Act - Update
            var updateRequest = new SongUpdateRequest
            {
                Title = $"Updated Song {randomSuffix}",
                Author = $"Updated Author {randomSuffix}"
            };

            var updatedSong = await _servicesService.UpdateSongAsync(createdSong.Id, updateRequest);

            // Assert - Update
            updatedSong.Should().NotBeNull();
            updatedSong.Id.Should().Be(createdSong.Id);
            updatedSong.Title.Should().Be(updateRequest.Title);
            updatedSong.Author.Should().Be(updateRequest.Author);

            // Act - Delete
            await _servicesService.DeleteSongAsync(createdSong.Id);

            // Assert - Delete
            var deletedSong = await _servicesService.GetSongAsync(createdSong.Id);
            deletedSong.Should().BeNull();
        }
        catch (Exception ex)
        {
            Assert.Fail($"Test failed with exception: {ex.Message}");
        }
    }

    [Fact]
    public async Task ListSongs_ShouldReturnPaginatedResults()
    {
        // Arrange
        var parameters = new QueryParameters
        {
            PerPage = 5
        };

        // Act
        var result = await _servicesService.ListSongsAsync(parameters);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Meta.Should().NotBeNull();
        result.Links.Should().NotBeNull();
        
        result.Data.Count.Should().BeLessThanOrEqualTo(5);
        result.Meta.PerPage.Should().Be(5);
    }

    [Fact]
    public async Task GetAllPlansAsync_ShouldReturnAllResults()
    {
        // Arrange
        var parameters = new QueryParameters
        {
            PerPage = 5
        };

        // Act
        var result = await _servicesService.GetAllPlansAsync(parameters);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task StreamPlansAsync_ShouldStreamResults()
    {
        // Arrange
        var parameters = new QueryParameters
        {
            PerPage = 5
        };

        // Act
        var plans = new List<Plan>();
        await foreach (var plan in _servicesService.StreamPlansAsync(parameters))
        {
            plans.Add(plan);
            
            // Limit to 10 plans to avoid long-running test
            if (plans.Count >= 10)
                break;
        }

        // Assert
        plans.Should().NotBeEmpty();
    }
}
