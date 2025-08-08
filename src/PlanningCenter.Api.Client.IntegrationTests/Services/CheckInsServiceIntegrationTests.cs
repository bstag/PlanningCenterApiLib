using FluentAssertions;
using PlanningCenter.Api.Client.IntegrationTests.Infrastructure;
using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.CheckIns;
using PlanningCenter.Api.Client.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PlanningCenter.Api.Client.IntegrationTests.Services;

/// <summary>
/// Integration tests for CheckInsService.
/// Tests the basic CRUD operations against the real Planning Center API.
/// </summary>
[Collection("Integration Tests")]
public class CheckInsServiceIntegrationTests : CheckInsServiceIntegrationTestBase
{
    public CheckInsServiceIntegrationTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task CreateGetUpdateCheckOut_CheckIn_ShouldPerformFullLifecycle()
    {
        // Arrange
        var randomSuffix = _random.Next(10000, 99999);
        var eventId = await GetEventIdAsync();
        var createRequest = new CheckInCreateRequest
        {
            FirstName = $"Test{randomSuffix}",
            LastName = $"User{randomSuffix}",
            EventId = eventId
        };

        try
        {
            // Act - Create
            var createdCheckIn = await _checkInsService.CreateCheckInAsync(createRequest);

            // Assert - Create
            createdCheckIn.Should().NotBeNull();
            createdCheckIn.Id.Should().NotBeNullOrEmpty();
            createdCheckIn.FirstName.Should().Be(createRequest.FirstName);
            createdCheckIn.LastName.Should().Be(createRequest.LastName);
            createdCheckIn.EventId.Should().Be(createRequest.EventId);
            createdCheckIn.CheckedOutAt.Should().BeNull();

            // Act - Get
            var retrievedCheckIn = await _checkInsService.GetCheckInAsync(createdCheckIn.Id);

            // Assert - Get
            retrievedCheckIn.Should().NotBeNull();
            retrievedCheckIn!.Id.Should().Be(createdCheckIn.Id);
            retrievedCheckIn.FirstName.Should().Be(createRequest.FirstName);
            retrievedCheckIn.LastName.Should().Be(createRequest.LastName);
            retrievedCheckIn.EventId.Should().Be(createRequest.EventId);

            // Act - Update
            var updateRequest = new CheckInUpdateRequest
            {
                MedicalNotes = $"Updated medical notes {randomSuffix}",
                EmergencyContactName = $"Emergency Contact {randomSuffix}"
            };

            var updatedCheckIn = await _checkInsService.UpdateCheckInAsync(createdCheckIn.Id, updateRequest);

            // Assert - Update
            updatedCheckIn.Should().NotBeNull();
            updatedCheckIn.Id.Should().Be(createdCheckIn.Id);
            updatedCheckIn.MedicalNotes.Should().Be(updateRequest.MedicalNotes);
            updatedCheckIn.EmergencyContactName.Should().Be(updateRequest.EmergencyContactName);
            updatedCheckIn.FirstName.Should().Be(createdCheckIn.FirstName); // Unchanged
            updatedCheckIn.LastName.Should().Be(createdCheckIn.LastName); // Unchanged
            updatedCheckIn.EventId.Should().Be(createdCheckIn.EventId); // Unchanged
            updatedCheckIn.CheckedOutAt.Should().BeNull();

            // Act - Check Out
            var checkedOutCheckIn = await _checkInsService.CheckOutAsync(createdCheckIn.Id);

            // Assert - Check Out
            checkedOutCheckIn.Should().NotBeNull();
            checkedOutCheckIn.Id.Should().Be(createdCheckIn.Id);
            checkedOutCheckIn.CheckedOutAt.Should().NotBeNull();
        }
        catch (Exception ex)
        {
            Assert.Fail($"Test failed with exception: {ex.Message}");
        }
    }

    [Fact]
    public async Task GetEvent_ShouldReturnEvent()
    {
        // Arrange
        var eventId = await GetEventIdAsync();

        // Act
        var eventItem = await _checkInsService.GetEventAsync(eventId);

        // Assert
        eventItem.Should().NotBeNull();
        eventItem!.Id.Should().Be(eventId);
        eventItem.Name.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ListEvents_ShouldReturnPaginatedResults()
    {
        // Arrange
        var parameters = new QueryParameters
        {
            PerPage = 5
        };

        // Act
        var result = await _checkInsService.ListEventsAsync(parameters);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Meta.Should().NotBeNull();
        result.Links.Should().NotBeNull();
        
        result.Data.Count.Should().BeLessThanOrEqualTo(5);
        result.Meta.PerPage.Should().Be(5);
    }

    [Fact]
    public async Task ListEventCheckIns_ShouldReturnPaginatedResults()
    {
        // Arrange
        var eventId = await GetEventIdAsync();
        var parameters = new QueryParameters
        {
            PerPage = 5
        };

        // Act
        var result = await _checkInsService.ListEventCheckInsAsync(eventId, parameters);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Meta.Should().NotBeNull();
        result.Links.Should().NotBeNull();
        
        result.Data.Count.Should().BeLessThanOrEqualTo(5);
        result.Meta.PerPage.Should().Be(5);
    }

    [Fact]
    public async Task ListCheckIns_ShouldReturnPaginatedResults()
    {
        // Arrange
        var parameters = new QueryParameters
        {
            PerPage = 5
        };

        // Act
        var result = await _checkInsService.ListCheckInsAsync(parameters);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Meta.Should().NotBeNull();
        result.Links.Should().NotBeNull();
        
        result.Data.Count.Should().BeLessThanOrEqualTo(5);
        result.Meta.PerPage.Should().Be(5);
    }

    [Fact]
    public async Task GetAllCheckInsAsync_ShouldReturnAllResults()
    {
        // Arrange
        var parameters = new QueryParameters
        {
            PerPage = 5
        };

        // Act
        var result = await _checkInsService.GetAllCheckInsAsync(parameters);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task StreamCheckInsAsync_ShouldStreamResults()
    {
        // Arrange
        var parameters = new QueryParameters
        {
            PerPage = 5
        };

        // Act
        var checkIns = new List<CheckIn>();
        await foreach (var checkIn in _checkInsService.StreamCheckInsAsync(parameters))
        {
            checkIns.Add(checkIn);
            
            // Limit to 10 check-ins to avoid long-running test
            if (checkIns.Count >= 10)
                break;
        }

        // Assert
        checkIns.Should().NotBeEmpty();
    }
}
