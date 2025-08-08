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
/// Integration tests for CheckInsService error handling.
/// Tests how the client library handles API errors and edge cases.
/// </summary>
[Collection("Integration Tests")]
public class CheckInsServiceErrorHandlingTests : CheckInsServiceIntegrationTestBase
{
    public CheckInsServiceErrorHandlingTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task GetCheckInAsync_ShouldReturnNull_WhenCheckInDoesNotExist()
    {
        // Arrange - Use a non-existent ID
        var nonExistentId = "non-existent-id";

        // Act
        var result = await _checkInsService.GetCheckInAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetEventAsync_ShouldReturnNull_WhenEventDoesNotExist()
    {
        // Arrange - Use a non-existent ID
        var nonExistentId = "non-existent-id";

        // Act
        var result = await _checkInsService.GetEventAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateCheckInAsync_ShouldThrowValidationException_WhenRequiredFieldsAreMissing()
    {
        // Arrange - Create an invalid request with missing required fields
        var invalidRequest = new CheckInCreateRequest
        {
            // Missing FirstName which is required
            LastName = "Test",
            EventId = "1" // This might not be a valid ID, but validation should fail on FirstName first
        };

        // Act & Assert
        await Assert.ThrowsAsync<PlanningCenterApiValidationException>(() => 
            _checkInsService.CreateCheckInAsync(invalidRequest));
    }

    [Fact]
    public async Task CreateCheckInAsync_ShouldThrowValidationException_WhenLastNameIsMissing()
    {
        // Arrange - Create an invalid request with missing required fields
        var invalidRequest = new CheckInCreateRequest
        {
            FirstName = "Test",
            // Missing LastName which is required
            EventId = "1" // This might not be a valid ID, but validation should fail on LastName first
        };

        // Act & Assert
        await Assert.ThrowsAsync<PlanningCenterApiValidationException>(() => 
            _checkInsService.CreateCheckInAsync(invalidRequest));
    }

    [Fact]
    public async Task CreateCheckInAsync_ShouldThrowValidationException_WhenEventIdIsMissing()
    {
        // Arrange - Create an invalid request with missing required fields
        var invalidRequest = new CheckInCreateRequest
        {
            FirstName = "Test",
            LastName = "User",
            // Missing EventId which is required
        };

        // Act & Assert
        await Assert.ThrowsAsync<PlanningCenterApiValidationException>(() => 
            _checkInsService.CreateCheckInAsync(invalidRequest));
    }

    [Fact]
    public async Task UpdateCheckInAsync_ShouldThrowNotFoundException_WhenCheckInDoesNotExist()
    {
        // Arrange
        var nonExistentId = "non-existent-id";
        var updateRequest = new CheckInUpdateRequest
        {
            MedicalNotes = "Updated medical notes",
            EmergencyContactName = "Updated emergency contact"
        };

        // Act & Assert
        await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(() => 
            _checkInsService.UpdateCheckInAsync(nonExistentId, updateRequest));
    }

    [Fact]
    public async Task CheckOutAsync_ShouldThrowNotFoundException_WhenCheckInDoesNotExist()
    {
        // Arrange
        var nonExistentId = "non-existent-id";

        // Act & Assert
        await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(() => 
            _checkInsService.CheckOutAsync(nonExistentId));
    }

    [Fact]
    public async Task ListEventCheckInsAsync_ShouldThrowNotFoundException_WhenEventDoesNotExist()
    {
        // Arrange
        var nonExistentId = "non-existent-id";
        var parameters = new QueryParameters { PerPage = 5 };

        // Act & Assert
        await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(() => 
            _checkInsService.ListEventCheckInsAsync(nonExistentId, parameters));
    }

    [Fact]
    public async Task ArgumentValidation_ShouldThrowArgumentException_WhenInvalidArgumentsProvided()
    {
        // Act & Assert - Test null or empty ID
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _checkInsService.GetCheckInAsync(string.Empty));
            
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _checkInsService.GetEventAsync(string.Empty));
            
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _checkInsService.ListEventCheckInsAsync(string.Empty, new QueryParameters()));

        // Act & Assert - Test null request
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _checkInsService.CreateCheckInAsync(null!));
            
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _checkInsService.UpdateCheckInAsync("valid-id", null!));
    }
}
