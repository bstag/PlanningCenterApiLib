using FluentAssertions;
using PlanningCenter.Api.Client.IntegrationTests.Infrastructure;
using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Calendar;
using PlanningCenter.Api.Client.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PlanningCenter.Api.Client.IntegrationTests.Services;

/// <summary>
/// Integration tests for CalendarService.
/// Tests the basic CRUD operations against the real Planning Center API.
/// </summary>
[Collection("Integration Tests")]
public class CalendarServiceIntegrationTests : CalendarServiceIntegrationTestBase
{
    public CalendarServiceIntegrationTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task CreateGetUpdateDelete_Event_ShouldPerformFullLifecycle()
    {
        // Arrange
        var randomSuffix = _random.Next(10000, 99999);
        var createRequest = new EventCreateRequest
        {
            Name = $"Test Event {randomSuffix}",
            Description = $"Test Description {randomSuffix}",
            StartsAt = DateTime.Now.AddDays(1),
            EndsAt = DateTime.Now.AddDays(1).AddHours(2)
        };

        try
        {
            // Act - Create
            var createdEvent = await _calendarService.CreateEventAsync(createRequest);

            // Assert - Create
            createdEvent.Should().NotBeNull();
            createdEvent.Id.Should().NotBeNullOrEmpty();
            createdEvent.Name.Should().Be(createRequest.Name);
            createdEvent.Description.Should().Be(createRequest.Description);
            createdEvent.StartsAt.Should().BeCloseTo(createRequest.StartsAt, TimeSpan.FromSeconds(1));
            createdEvent.EndsAt.Should().BeCloseTo(createRequest.EndsAt.Value, TimeSpan.FromSeconds(1));

            // Act - Get
            var retrievedEvent = await _calendarService.GetEventAsync(createdEvent.Id);

            // Assert - Get
            retrievedEvent.Should().NotBeNull();
            retrievedEvent!.Id.Should().Be(createdEvent.Id);
            retrievedEvent.Name.Should().Be(createRequest.Name);
            retrievedEvent.Description.Should().Be(createRequest.Description);

            // Act - Update
            var updateRequest = new EventUpdateRequest
            {
                Name = $"Updated Event {randomSuffix}",
                Description = $"Updated Description {randomSuffix}"
            };

            var updatedEvent = await _calendarService.UpdateEventAsync(createdEvent.Id, updateRequest);

            // Assert - Update
            updatedEvent.Should().NotBeNull();
            updatedEvent.Id.Should().Be(createdEvent.Id);
            updatedEvent.Name.Should().Be(updateRequest.Name);
            updatedEvent.Description.Should().Be(updateRequest.Description);
            // Times should remain unchanged
            updatedEvent.StartsAt.Should().NotBeNull();
            createdEvent.StartsAt.Should().NotBeNull();
            updatedEvent.StartsAt!.Value.Should().BeCloseTo(createdEvent.StartsAt!.Value, TimeSpan.FromSeconds(1));
            
            updatedEvent.EndsAt.Should().NotBeNull();
            createdEvent.EndsAt.Should().NotBeNull();
            updatedEvent.EndsAt!.Value.Should().BeCloseTo(createdEvent.EndsAt!.Value, TimeSpan.FromSeconds(1));

            // Act - Delete
            await _calendarService.DeleteEventAsync(createdEvent.Id);

            // Assert - Delete
            var deletedEvent = await _calendarService.GetEventAsync(createdEvent.Id);
            deletedEvent.Should().BeNull();
        }
        catch (Exception ex)
        {
            Assert.Fail($"Test failed with exception: {ex.Message}");
        }
    }

    [Fact]
    public async Task CreateGetUpdateDelete_Resource_ShouldPerformFullLifecycle()
    {
        // Arrange
        var randomSuffix = _random.Next(10000, 99999);
        var createRequest = new ResourceCreateRequest
        {
            Name = $"Test Resource {randomSuffix}",
            Description = $"Test Resource Description {randomSuffix}"
        };

        try
        {
            // Act - Create
            var createdResource = await _calendarService.CreateResourceAsync(createRequest);

            // Assert - Create
            createdResource.Should().NotBeNull();
            createdResource.Id.Should().NotBeNullOrEmpty();
            createdResource.Name.Should().Be(createRequest.Name);
            createdResource.Description.Should().Be(createRequest.Description);

            // Act - Get
            var retrievedResource = await _calendarService.GetResourceAsync(createdResource.Id);

            // Assert - Get
            retrievedResource.Should().NotBeNull();
            retrievedResource!.Id.Should().Be(createdResource.Id);
            retrievedResource.Name.Should().Be(createRequest.Name);
            retrievedResource.Description.Should().Be(createRequest.Description);

            // Act - Update
            var updateRequest = new ResourceUpdateRequest
            {
                Name = $"Updated Resource {randomSuffix}",
                Description = $"Updated Resource Description {randomSuffix}"
            };

            var updatedResource = await _calendarService.UpdateResourceAsync(createdResource.Id, updateRequest);

            // Assert - Update
            updatedResource.Should().NotBeNull();
            updatedResource.Id.Should().Be(createdResource.Id);
            updatedResource.Name.Should().Be(updateRequest.Name);
            updatedResource.Description.Should().Be(updateRequest.Description);

            // Act - Delete
            await _calendarService.DeleteResourceAsync(createdResource.Id);

            // Assert - Delete
            var deletedResource = await _calendarService.GetResourceAsync(createdResource.Id);
            deletedResource.Should().BeNull();
        }
        catch (Exception ex)
        {
            Assert.Fail($"Test failed with exception: {ex.Message}");
        }
    }

    [Fact]
    public async Task ListEventsAsync_ShouldReturnPaginatedResults()
    {
        // Arrange
        var parameters = new QueryParameters
        {
            PerPage = 5
        };

        // Act
        var result = await _calendarService.ListEventsAsync(parameters);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Meta.Should().NotBeNull();
        result.Links.Should().NotBeNull();
        
        result.Data.Count.Should().BeLessThanOrEqualTo(5);
        result.Meta.PerPage.Should().Be(5);
    }

    [Fact]
    public async Task ListResourcesAsync_ShouldReturnPaginatedResults()
    {
        // Arrange
        var parameters = new QueryParameters
        {
            PerPage = 5
        };

        // Act
        var result = await _calendarService.ListResourcesAsync(parameters);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Meta.Should().NotBeNull();
        result.Links.Should().NotBeNull();
        
        result.Data.Count.Should().BeLessThanOrEqualTo(5);
        result.Meta.PerPage.Should().Be(5);
    }

    [Fact]
    public async Task GetAllEventsAsync_ShouldReturnAllResults()
    {
        // Arrange
        var parameters = new QueryParameters
        {
            PerPage = 5
        };

        // Act
        var result = await _calendarService.GetAllEventsAsync(parameters);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task StreamEventsAsync_ShouldStreamResults()
    {
        // Arrange
        var parameters = new QueryParameters
        {
            PerPage = 5
        };

        // Act
        var events = new List<Event>();
        await foreach (var eventItem in _calendarService.StreamEventsAsync(parameters))
        {
            events.Add(eventItem);
            
            // Limit to 10 events to avoid long-running test
            if (events.Count >= 10)
                break;
        }

        // Assert
        events.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ListEventsByDateRangeAsync_ShouldReturnEventsInRange()
    {
        // Arrange
        var startDate = DateTime.Now.AddDays(-30);
        var endDate = DateTime.Now.AddDays(30);
        var parameters = new QueryParameters
        {
            PerPage = 5
        };

        // Act
        var result = await _calendarService.ListEventsByDateRangeAsync(startDate, endDate, parameters);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        
        // All events should be within the date range
        foreach (var eventItem in result.Data)
        {
            eventItem.StartsAt.Should().BeOnOrAfter(startDate.Date);
            eventItem.StartsAt.Should().BeOnOrBefore(endDate.Date.AddDays(1).AddSeconds(-1));
        }
    }
}
