using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using PlanningCenter.Api.Client.Models.JsonApi;
using PlanningCenter.Api.Client.Models.JsonApi.Calendar;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Services;
using PlanningCenter.Api.Client.Tests.Utilities;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Services;

/// <summary>
/// Unit tests for CalendarService.
/// Follows the same patterns as PeopleServiceTests.
/// </summary>
public class CalendarServiceTests
{
    private readonly MockApiConnection _mockApiConnection = new();
    private readonly CalendarService _calendarService;
    private readonly ExtendedTestDataBuilder _builder = new();

    public CalendarServiceTests()
    {
        _calendarService = new CalendarService(_mockApiConnection, NullLogger<CalendarService>.Instance);
    }

    #region Event Tests

    [Fact]
    public async Task GetEventAsync_ShouldReturnEvent_WhenApiReturnsData()
    {
        // Arrange
        var eventDto = _builder.CreateCalendarEventDto(e => e.Id = "event-123");
        var response = new JsonApiSingleResponse<Models.JsonApi.Calendar.EventDto> { Data = eventDto };
        _mockApiConnection.SetupGetResponse("/calendar/v2/events/event-123", response);

        // Act
        var result = await _calendarService.GetEventAsync("event-123");

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("event-123");
        result.Name.Should().Be(eventDto.Attributes.Name);
        result.DataSource.Should().Be("Calendar");
    }

    [Fact]
    public async Task GetEventAsync_ShouldReturnNull_WhenApiReturnsNull()
    {
        // Arrange
        var response = new JsonApiSingleResponse<Models.JsonApi.Calendar.EventDto> { Data = null };
        _mockApiConnection.SetupGetResponse("/calendar/v2/events/nonexistent", response);

        // Act
        var result = await _calendarService.GetEventAsync("nonexistent");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetEventAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _calendarService.GetEventAsync(""));
    }

    [Fact]
    public async Task ListEventsAsync_ShouldReturnPagedEvents_WhenApiReturnsData()
    {
        // Arrange
        var eventDtos = new[]
        {
            _builder.CreateCalendarEventDto(e => e.Id = "1"),
            _builder.CreateCalendarEventDto(e => e.Id = "2")
        };
        _mockApiConnection.SetupGetResponse("/calendar/v2/events", eventDtos);

        // Act
        var result = await _calendarService.ListEventsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Data[0].Id.Should().Be("1");
        result.Data[1].Id.Should().Be("2");
        result.Meta.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateEventAsync_ShouldReturnCreatedEvent_WhenValidRequest()
    {
        // Arrange
        var request = new EventCreateRequest
        {
            Name = "Christmas Service",
            Summary = "Special Christmas celebration",
            StartsAt = DateTime.UtcNow.AddDays(30),
            EndsAt = DateTime.UtcNow.AddDays(30).AddHours(2),
            LocationName = "Main Sanctuary",
            VisibleInChurchCenter = true,
            RegistrationRequired = false
        };

        var createdEventDto = _builder.CreateCalendarEventDto(e =>
        {
            e.Id = "new-event-123";
            e.Attributes.Name = request.Name;
            e.Attributes.Summary = request.Summary;
            e.Attributes.StartsAt = request.StartsAt;
            e.Attributes.EndsAt = request.EndsAt;
        });

        var response = new JsonApiSingleResponse<Models.JsonApi.Calendar.EventDto> { Data = createdEventDto };
        _mockApiConnection.SetupMutationResponse("POST", "/calendar/v2/events", response);

        // Act
        var result = await _calendarService.CreateEventAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("new-event-123");
        result.Name.Should().Be("Christmas Service");
        result.Summary.Should().Be("Special Christmas celebration");
        result.DataSource.Should().Be("Calendar");
    }

    [Fact]
    public async Task CreateEventAsync_ShouldThrowArgumentException_WhenNameIsEmpty()
    {
        // Arrange
        var request = new EventCreateRequest
        {
            Name = "",
            StartsAt = DateTime.UtcNow.AddDays(1)
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _calendarService.CreateEventAsync(request));
    }

    [Fact]
    public async Task UpdateEventAsync_ShouldReturnUpdatedEvent_WhenValidRequest()
    {
        // Arrange
        var request = new EventUpdateRequest
        {
            Name = "Updated Event Name",
            Summary = "Updated summary",
            VisibleInChurchCenter = false
        };

        var updatedEventDto = _builder.CreateCalendarEventDto(e =>
        {
            e.Id = "event-123";
            e.Attributes.Name = "Updated Event Name";
            e.Attributes.Summary = "Updated summary";
            e.Attributes.VisibleInChurchCenter = false;
        });

        var response = new JsonApiSingleResponse<Models.JsonApi.Calendar.EventDto> { Data = updatedEventDto };
        _mockApiConnection.SetupMutationResponse("PATCH", "/calendar/v2/events/event-123", response);

        // Act
        var result = await _calendarService.UpdateEventAsync("event-123", request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("event-123");
        result.Name.Should().Be("Updated Event Name");
        result.Summary.Should().Be("Updated summary");
        result.VisibleInChurchCenter.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteEventAsync_ShouldCompleteSuccessfully_WhenValidId()
    {
        // Act & Assert (should not throw)
        await _calendarService.DeleteEventAsync("event-123");
    }

    [Fact]
    public async Task ListEventsByDateRangeAsync_ShouldReturnFilteredEvents_WhenValidDateRange()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(1);
        var endDate = DateTime.UtcNow.AddDays(7);
        var eventDtos = new[]
        {
            _builder.CreateCalendarEventDto(e => e.Id = "1"),
            _builder.CreateCalendarEventDto(e => e.Id = "2")
        };
        _mockApiConnection.SetupGetResponse("/calendar/v2/events", eventDtos);

        // Act
        var result = await _calendarService.ListEventsByDateRangeAsync(startDate, endDate);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Data[0].Id.Should().Be("1");
        result.Data[1].Id.Should().Be("2");
    }

    #endregion

    #region Resource Tests

    [Fact]
    public async Task GetResourceAsync_ShouldReturnResource_WhenApiReturnsData()
    {
        // Arrange
        var resourceDto = _builder.CreateResourceDto(r => r.Id = "resource-123");
        var response = new JsonApiSingleResponse<ResourceDto> { Data = resourceDto };
        _mockApiConnection.SetupGetResponse("/calendar/v2/resources/resource-123", response);

        // Act
        var result = await _calendarService.GetResourceAsync("resource-123");

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("resource-123");
        result.Name.Should().Be(resourceDto.Attributes.Name);
        result.DataSource.Should().Be("Calendar");
    }

    [Fact]
    public async Task ListResourcesAsync_ShouldReturnPagedResources_WhenApiReturnsData()
    {
        // Arrange
        var resourceDtos = new[]
        {
            _builder.CreateResourceDto(r => r.Id = "1"),
            _builder.CreateResourceDto(r => r.Id = "2")
        };
        _mockApiConnection.SetupGetResponse("/calendar/v2/resources", resourceDtos);

        // Act
        var result = await _calendarService.ListResourcesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Data[0].Id.Should().Be("1");
        result.Data[1].Id.Should().Be("2");
    }

    [Fact]
    public async Task CreateResourceAsync_ShouldReturnCreatedResource_WhenValidRequest()
    {
        // Arrange
        var request = new ResourceCreateRequest
        {
            Name = "Sound System",
            Description = "Main sanctuary sound system",
            Kind = "Equipment",
            Quantity = 1,
            Expires = false
        };

        var createdResourceDto = _builder.CreateResourceDto(r =>
        {
            r.Id = "new-resource-123";
            r.Attributes.Name = request.Name;
            r.Attributes.Description = request.Description;
            r.Attributes.Kind = request.Kind;
        });

        var response = new JsonApiSingleResponse<ResourceDto> { Data = createdResourceDto };
        _mockApiConnection.SetupMutationResponse("POST", "/calendar/v2/resources", response);

        // Act
        var result = await _calendarService.CreateResourceAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("new-resource-123");
        result.Name.Should().Be("Sound System");
        result.Description.Should().Be("Main sanctuary sound system");
        result.Kind.Should().Be("Equipment");
    }

    [Fact]
    public async Task CreateResourceAsync_ShouldThrowArgumentException_WhenNameIsEmpty()
    {
        // Arrange
        var request = new ResourceCreateRequest
        {
            Name = "",
            Kind = "Equipment"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _calendarService.CreateResourceAsync(request));
    }

    [Fact]
    public async Task UpdateResourceAsync_ShouldReturnUpdatedResource_WhenValidRequest()
    {
        // Arrange
        var request = new ResourceUpdateRequest
        {
            Name = "Updated Resource Name",
            Description = "Updated description",
            Quantity = 2
        };

        var updatedResourceDto = _builder.CreateResourceDto(r =>
        {
            r.Id = "resource-123";
            r.Attributes.Name = "Updated Resource Name";
            r.Attributes.Description = "Updated description";
            r.Attributes.Quantity = 2;
        });

        var response = new JsonApiSingleResponse<ResourceDto> { Data = updatedResourceDto };
        _mockApiConnection.SetupMutationResponse("PATCH", "/calendar/v2/resources/resource-123", response);

        // Act
        var result = await _calendarService.UpdateResourceAsync("resource-123", request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("resource-123");
        result.Name.Should().Be("Updated Resource Name");
        result.Description.Should().Be("Updated description");
        result.Quantity.Should().Be(2);
    }

    [Fact]
    public async Task DeleteResourceAsync_ShouldCompleteSuccessfully_WhenValidId()
    {
        // Act & Assert (should not throw)
        await _calendarService.DeleteResourceAsync("resource-123");
    }

    #endregion

    #region Pagination Tests

    [Fact]
    public async Task GetAllEventsAsync_ShouldReturnAllEvents_WhenMultiplePagesExist()
    {
        // Arrange
        var eventDtos = new[]
        {
            _builder.CreateCalendarEventDto(e => e.Id = "1"),
            _builder.CreateCalendarEventDto(e => e.Id = "2"),
            _builder.CreateCalendarEventDto(e => e.Id = "3")
        };
        _mockApiConnection.SetupGetResponse("/calendar/v2/events", eventDtos);

        // Act
        var result = await _calendarService.GetAllEventsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result[0].Id.Should().Be("1");
        result[1].Id.Should().Be("2");
        result[2].Id.Should().Be("3");
    }

    [Fact]
    public async Task StreamEventsAsync_ShouldYieldEvents_WhenDataExists()
    {
        // Arrange
        var eventDtos = new[]
        {
            _builder.CreateCalendarEventDto(e => e.Id = "1"),
            _builder.CreateCalendarEventDto(e => e.Id = "2")
        };
        _mockApiConnection.SetupGetResponse("/calendar/v2/events", eventDtos);

        // Act
        var events = new List<Models.Calendar.Event>();
        await foreach (var eventItem in _calendarService.StreamEventsAsync())
        {
            events.Add(eventItem);
        }

        // Assert
        events.Should().HaveCount(2);
        events[0].Id.Should().Be("1");
        events[1].Id.Should().Be("2");
    }

    #endregion
}