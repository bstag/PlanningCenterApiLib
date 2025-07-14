using FluentAssertions;
using PlanningCenter.Api.Client.IntegrationTests.Infrastructure;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Exceptions;
using PlanningCenter.Api.Client.Models.Requests;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PlanningCenter.Api.Client.IntegrationTests.Services;

/// <summary>
/// Integration tests for ServicesService error handling.
/// Tests how the client library handles API errors and edge cases.
/// </summary>
[Collection("Integration Tests")]
public class ServicesServiceErrorHandlingTests : ServicesServiceIntegrationTestBase
{
    public ServicesServiceErrorHandlingTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task GetPlanAsync_ShouldReturnNull_WhenPlanDoesNotExist()
    {
        // Arrange - Use a non-existent ID
        var nonExistentId = "non-existent-id";

        // Act
        var result = await _servicesService.GetPlanAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetServiceTypeAsync_ShouldReturnNull_WhenServiceTypeDoesNotExist()
    {
        // Arrange - Use a non-existent ID
        var nonExistentId = "non-existent-id";

        // Act
        var result = await _servicesService.GetServiceTypeAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPlanItemAsync_ShouldReturnNull_WhenItemDoesNotExist()
    {
        // Arrange - Create a test plan first
        var testPlan = await CreateTestPlanAsync();
        try
        {
            // Act
            var result = await _servicesService.GetPlanItemAsync(testPlan.Id, "non-existent-id");

            // Assert
            result.Should().BeNull();
        }
        finally
        {
            await CleanupTestPlanAsync(testPlan.Id);
        }
    }

    [Fact]
    public async Task GetSongAsync_ShouldReturnNull_WhenSongDoesNotExist()
    {
        // Arrange - Use a non-existent ID
        var nonExistentId = "non-existent-id";

        // Act
        var result = await _servicesService.GetSongAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreatePlanAsync_ShouldThrowValidationException_WhenRequiredFieldsAreMissing()
    {
        // Arrange - Create an invalid request with missing required fields
        var invalidRequest = new PlanCreateRequest
        {
            // Missing Title which is required
            ServiceTypeId = "1" // This might not be a valid ID, but validation should fail on Title first
        };

        // Act & Assert
        await Assert.ThrowsAsync<PlanningCenterApiValidationException>(() => 
            _servicesService.CreatePlanAsync(invalidRequest));
    }

    [Fact]
    public async Task CreatePlanItemAsync_ShouldThrowValidationException_WhenRequiredFieldsAreMissing()
    {
        // Arrange - Create a test plan first
        var testPlan = await CreateTestPlanAsync();
        try
        {
            // Arrange - Create an invalid item request
            var invalidRequest = new ItemCreateRequest
            {
                // Missing Title which is required
                Description = "Test Description"
            };

            // Act & Assert
            await Assert.ThrowsAsync<PlanningCenterApiValidationException>(() => 
                _servicesService.CreatePlanItemAsync(testPlan.Id, invalidRequest));
        }
        finally
        {
            await CleanupTestPlanAsync(testPlan.Id);
        }
    }

    [Fact]
    public async Task CreateSongAsync_ShouldThrowValidationException_WhenRequiredFieldsAreMissing()
    {
        // Arrange - Create an invalid request with missing required fields
        var invalidRequest = new SongCreateRequest
        {
            // Missing Title which is required
            Author = "Test Author"
        };

        // Act & Assert
        await Assert.ThrowsAsync<PlanningCenterApiValidationException>(() => 
            _servicesService.CreateSongAsync(invalidRequest));
    }

    [Fact]
    public async Task UpdatePlanAsync_ShouldThrowNotFoundException_WhenPlanDoesNotExist()
    {
        // Arrange
        var nonExistentId = "non-existent-id";
        var updateRequest = new PlanUpdateRequest
        {
            Title = "Updated Plan"
        };

        // Act & Assert
        await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(() => 
            _servicesService.UpdatePlanAsync(nonExistentId, updateRequest));
    }

    [Fact]
    public async Task UpdatePlanItemAsync_ShouldThrowNotFoundException_WhenItemDoesNotExist()
    {
        // Arrange - Create a test plan first
        var testPlan = await CreateTestPlanAsync();
        try
        {
            var updateRequest = new ItemUpdateRequest
            {
                Title = "Updated Item"
            };

            // Act & Assert
            await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(() => 
                _servicesService.UpdatePlanItemAsync(testPlan.Id, "non-existent-id", updateRequest));
        }
        finally
        {
            await CleanupTestPlanAsync(testPlan.Id);
        }
    }

    [Fact]
    public async Task UpdateSongAsync_ShouldThrowNotFoundException_WhenSongDoesNotExist()
    {
        // Arrange
        var nonExistentId = "non-existent-id";
        var updateRequest = new SongUpdateRequest
        {
            Title = "Updated Song"
        };

        // Act & Assert
        await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(() => 
            _servicesService.UpdateSongAsync(nonExistentId, updateRequest));
    }

    [Fact]
    public async Task DeletePlanAsync_ShouldThrowNotFoundException_WhenPlanDoesNotExist()
    {
        // Arrange
        var nonExistentId = "non-existent-id";

        // Act & Assert
        await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(() => 
            _servicesService.DeletePlanAsync(nonExistentId));
    }

    [Fact]
    public async Task DeletePlanItemAsync_ShouldThrowNotFoundException_WhenItemDoesNotExist()
    {
        // Arrange - Create a test plan first
        var testPlan = await CreateTestPlanAsync();
        try
        {
            // Act & Assert
            await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(() => 
                _servicesService.DeletePlanItemAsync(testPlan.Id, "non-existent-id"));
        }
        finally
        {
            await CleanupTestPlanAsync(testPlan.Id);
        }
    }

    [Fact]
    public async Task DeleteSongAsync_ShouldThrowNotFoundException_WhenSongDoesNotExist()
    {
        // Arrange
        var nonExistentId = "non-existent-id";

        // Act & Assert
        await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(() => 
            _servicesService.DeleteSongAsync(nonExistentId));
    }

    [Fact]
    public async Task ArgumentValidation_ShouldThrowArgumentException_WhenInvalidArgumentsProvided()
    {
        // Act & Assert - Test null or empty ID
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _servicesService.GetPlanAsync(string.Empty));
            
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _servicesService.GetServiceTypeAsync(string.Empty));
            
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _servicesService.GetPlanItemAsync(string.Empty, "valid-id"));
            
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _servicesService.GetPlanItemAsync("valid-id", string.Empty));
            
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _servicesService.GetSongAsync(string.Empty));

        // Act & Assert - Test null request
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _servicesService.CreatePlanAsync(null!));
            
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _servicesService.CreatePlanItemAsync("valid-id", null!));
            
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _servicesService.CreateSongAsync(null!));
            
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _servicesService.UpdatePlanAsync("valid-id", null!));
            
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _servicesService.UpdatePlanItemAsync("valid-id", "valid-id", null!));
            
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _servicesService.UpdateSongAsync("valid-id", null!));
    }

    [Fact]
    public async Task ListPlanItemsAsync_ShouldThrowNotFoundException_WhenPlanDoesNotExist()
    {
        // Arrange
        var nonExistentId = "non-existent-id";
        var parameters = new QueryParameters { PerPage = 5 };

        // Act & Assert
        await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(() => 
            _servicesService.ListPlanItemsAsync(nonExistentId, parameters));
    }
}
