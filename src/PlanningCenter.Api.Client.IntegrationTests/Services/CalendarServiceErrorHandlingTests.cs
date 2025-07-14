using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PlanningCenter.Api.Client.IntegrationTests.Infrastructure;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Exceptions;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PlanningCenter.Api.Client.IntegrationTests.Services;

/// <summary>
/// Integration tests for CalendarService error handling.
/// Tests how the client library handles API errors and edge cases.
/// </summary>
[Collection("Integration Tests")]
public class CalendarServiceErrorHandlingTests : IClassFixture<TestFixture>
{
    private readonly TestFixture _fixture;
    private readonly ICalendarService _calendarService;
    
    public CalendarServiceErrorHandlingTests(TestFixture fixture)
    {
        _fixture = fixture;
        _calendarService = _fixture.ServiceProvider.GetRequiredService<ICalendarService>();
    }

    [Fact]
    public async Task GetEventAsync_ShouldReturnNull_WhenEventDoesNotExist()
    {
        // Arrange - Use a non-existent ID
        var nonExistentId = "non-existent-id";

        // Act
        var result = await _calendarService.GetEventAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetResourceAsync_ShouldReturnNull_WhenResourceDoesNotExist()
    {
        // Arrange - Use a non-existent ID
        var nonExistentId = "non-existent-id";

        // Act
        var result = await _calendarService.GetResourceAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateEventAsync_ShouldThrowValidationException_WhenRequiredFieldsAreMissing()
    {
        // Arrange - Create an invalid request with missing required fields
        var invalidRequest = new EventCreateRequest
        {
            // Missing Name which is required
            Description = "Test Description"
        };

        // Act & Assert
        await Assert.ThrowsAsync<PlanningCenterApiValidationException>(() => 
            _calendarService.CreateEventAsync(invalidRequest));
    }

    [Fact]
    public async Task CreateResourceAsync_ShouldThrowValidationException_WhenRequiredFieldsAreMissing()
    {
        // Arrange - Create an invalid request with missing required fields
        var invalidRequest = new ResourceCreateRequest
        {
            // Missing Name which is required
            Description = "Test Resource Description"
        };

        // Act & Assert
        await Assert.ThrowsAsync<PlanningCenterApiValidationException>(() => 
            _calendarService.CreateResourceAsync(invalidRequest));
    }

    [Fact]
    public async Task UpdateEventAsync_ShouldThrowNotFoundException_WhenEventDoesNotExist()
    {
        // Arrange
        var nonExistentId = "non-existent-id";
        var updateRequest = new EventUpdateRequest
        {
            Name = "Updated Event"
        };

        // Act & Assert
        await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(() => 
            _calendarService.UpdateEventAsync(nonExistentId, updateRequest));
    }

    [Fact]
    public async Task UpdateResourceAsync_ShouldThrowNotFoundException_WhenResourceDoesNotExist()
    {
        // Arrange
        var nonExistentId = "non-existent-id";
        var updateRequest = new ResourceUpdateRequest
        {
            Name = "Updated Resource"
        };

        // Act & Assert
        await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(() => 
            _calendarService.UpdateResourceAsync(nonExistentId, updateRequest));
    }

    [Fact]
    public async Task DeleteEventAsync_ShouldThrowNotFoundException_WhenEventDoesNotExist()
    {
        // Arrange
        var nonExistentId = "non-existent-id";

        // Act & Assert
        await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(() => 
            _calendarService.DeleteEventAsync(nonExistentId));
    }

    [Fact]
    public async Task DeleteResourceAsync_ShouldThrowNotFoundException_WhenResourceDoesNotExist()
    {
        // Arrange
        var nonExistentId = "non-existent-id";

        // Act & Assert
        await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(() => 
            _calendarService.DeleteResourceAsync(nonExistentId));
    }

    [Fact]
    public async Task ListEventsByDateRangeAsync_ShouldHandleInvalidDateRange()
    {
        // Arrange - End date before start date
        var startDate = DateTime.Now.AddDays(10);
        var endDate = DateTime.Now.AddDays(-10);
        var parameters = new PlanningCenter.Api.Client.Models.QueryParameters
        {
            PerPage = 5
        };

        // Act
        var result = await _calendarService.ListEventsByDateRangeAsync(startDate, endDate, parameters);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().BeEmpty(); // No events should match this invalid range
    }

    [Fact]
    public async Task ArgumentValidation_ShouldThrowArgumentException_WhenInvalidArgumentsProvided()
    {
        // Arrange
        var validRequest = new EventCreateRequest
        {
            Name = "Test Event",
            Description = "Test Description"
        };

        // Act & Assert - Test null or empty ID
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _calendarService.GetEventAsync(string.Empty));
            
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _calendarService.GetResourceAsync(string.Empty));
            
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _calendarService.UpdateEventAsync(string.Empty, new EventUpdateRequest()));
            
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _calendarService.UpdateResourceAsync(string.Empty, new ResourceUpdateRequest()));
            
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _calendarService.DeleteEventAsync(string.Empty));
            
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _calendarService.DeleteResourceAsync(string.Empty));

        // Act & Assert - Test null request
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _calendarService.CreateEventAsync(null));
            
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _calendarService.CreateResourceAsync(null));
            
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _calendarService.UpdateEventAsync("valid-id", null));
            
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _calendarService.UpdateResourceAsync("valid-id", null));
    }
}
