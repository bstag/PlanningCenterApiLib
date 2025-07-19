using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using PlanningCenter.Api.Client.Models.JsonApi;
using PlanningCenter.Api.Client.Models.JsonApi.CheckIns;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Services;
using PlanningCenter.Api.Client.Tests.Utilities;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Services;

/// <summary>
/// Unit tests for CheckInsService.
/// Follows the same patterns as PeopleServiceTests.
/// </summary>
public class CheckInsServiceTests
{
    private readonly MockApiConnection _mockApiConnection = new();
    private readonly CheckInsService _checkInsService;
    private readonly ExtendedTestDataBuilder _builder = new();

    public CheckInsServiceTests()
    {
        _checkInsService = new CheckInsService(_mockApiConnection, NullLogger<CheckInsService>.Instance);
    }

    #region Check-In Tests

    [Fact]
    public async Task GetCheckInAsync_ShouldReturnCheckIn_WhenApiReturnsData()
    {
        // Arrange
        var checkInDto = _builder.CreateCheckInDto(c => c.Id = "checkin-123");
        var response = new JsonApiSingleResponse<CheckInDto> { Data = checkInDto };
        _mockApiConnection.SetupGetResponse("/check_ins/v2/check_ins/checkin-123", response);

        // Act
        var result = await _checkInsService.GetCheckInAsync("checkin-123");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("checkin-123");
        result!.FirstName.Should().Be(checkInDto.Attributes.FirstName);
        result!.LastName.Should().Be(checkInDto.Attributes.LastName);
        result!.DataSource.Should().Be("CheckIns");
    }

    [Fact]
    public async Task GetCheckInAsync_ShouldReturnNull_WhenApiReturnsNull()
    {
        // Arrange
        var response = new JsonApiSingleResponse<CheckInDto> { Data = null };
        _mockApiConnection.SetupGetResponse("/check_ins/v2/check_ins/nonexistent", response);

        // Act
        var result = await _checkInsService.GetCheckInAsync("nonexistent");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetCheckInAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _checkInsService.GetCheckInAsync(""));
    }

    [Fact]
    public async Task ListCheckInsAsync_ShouldReturnPagedCheckIns_WhenApiReturnsData()
    {
        // Arrange
        var checkInDtos = new[]
        {
            _builder.CreateCheckInDto(c => c.Id = "1"),
            _builder.CreateCheckInDto(c => c.Id = "2")
        };
        _mockApiConnection.SetupGetResponse("/check_ins/v2/check_ins", checkInDtos);

        // Act
        var result = await _checkInsService.ListCheckInsAsync();

        // Assert
        result.Should().NotBeNull();
        result!.Data.Should().HaveCount(2);
        result!.Data[0].Id.Should().Be("1");
        result!.Data[1].Id.Should().Be("2");
        result!.Meta.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateCheckInAsync_ShouldReturnCreatedCheckIn_WhenValidRequest()
    {
        // Arrange
        var request = new CheckInCreateRequest
        {
            FirstName = "John",
            LastName = "Doe",
            EventId = "event-123",
            Kind = "regular",
            PersonId = "person-123"
        };

        var createdCheckInDto = _builder.CreateCheckInDto(c =>
        {
            c.Id = "new-checkin-123";
            c.Attributes.FirstName = request.FirstName;
            c.Attributes.LastName = request.LastName;
            c.Attributes.Kind = request.Kind;
        });

        var response = new JsonApiSingleResponse<CheckInDto> { Data = createdCheckInDto };
        _mockApiConnection.SetupMutationResponse("POST", "/check_ins/v2/check_ins", response);

        // Act
        var result = await _checkInsService.CreateCheckInAsync(request);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("new-checkin-123");
        result!.FirstName.Should().Be("John");
        result!.LastName.Should().Be("Doe");
        result!.Kind.Should().Be("regular");
        result!.DataSource.Should().Be("CheckIns");
    }

    [Fact]
    public async Task CreateCheckInAsync_ShouldThrowArgumentException_WhenFirstNameIsEmpty()
    {
        // Arrange
        var request = new CheckInCreateRequest
        {
            FirstName = "",
            LastName = "Doe",
            EventId = "event-123"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _checkInsService.CreateCheckInAsync(request));
    }

    [Fact]
    public async Task CreateCheckInAsync_ShouldThrowArgumentException_WhenLastNameIsEmpty()
    {
        // Arrange
        var request = new CheckInCreateRequest
        {
            FirstName = "John",
            LastName = "",
            EventId = "event-123"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _checkInsService.CreateCheckInAsync(request));
    }

    [Fact]
    public async Task CreateCheckInAsync_ShouldThrowArgumentException_WhenEventIdIsEmpty()
    {
        // Arrange
        var request = new CheckInCreateRequest
        {
            FirstName = "John",
            LastName = "Doe",
            EventId = ""
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _checkInsService.CreateCheckInAsync(request));
    }

    [Fact]
    public async Task UpdateCheckInAsync_ShouldReturnUpdatedCheckIn_WhenValidRequest()
    {
        // Arrange
        var request = new CheckInUpdateRequest
        {
            MedicalNotes = "Allergic to peanuts",
            EmergencyContactName = "Jane Doe",
            EmergencyContactPhoneNumber = "555-0123"
        };

        var updatedCheckInDto = _builder.CreateCheckInDto(c =>
        {
            c.Id = "checkin-123";
            c.Attributes.MedicalNotes = request.MedicalNotes;
            c.Attributes.EmergencyContactName = request.EmergencyContactName;
            c.Attributes.EmergencyContactPhoneNumber = request.EmergencyContactPhoneNumber;
        });

        var response = new JsonApiSingleResponse<CheckInDto> { Data = updatedCheckInDto };
        _mockApiConnection.SetupMutationResponse("PATCH", "/check_ins/v2/check_ins/checkin-123", response);

        // Act
        var result = await _checkInsService.UpdateCheckInAsync("checkin-123", request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("checkin-123");
        result.MedicalNotes.Should().Be("Allergic to peanuts");
        result.EmergencyContactName.Should().Be("Jane Doe");
        result.EmergencyContactPhoneNumber.Should().Be("555-0123");
    }

    [Fact]
    public async Task CheckOutAsync_ShouldReturnCheckedOutCheckIn_WhenValidId()
    {
        // Arrange
        var checkedOutDto = _builder.CreateCheckInDto(c =>
        {
            c.Id = "checkin-123";
            c.Attributes.CheckedOutAt = DateTime.UtcNow;
        });

        var response = new JsonApiSingleResponse<CheckInDto> { Data = checkedOutDto };
        _mockApiConnection.SetupMutationResponse("PATCH", "/check_ins/v2/check_ins/checkin-123/check_out", response);

        // Act
        var result = await _checkInsService.CheckOutAsync("checkin-123");

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("checkin-123");
        result.CheckedOutAt.Should().NotBeNull();
    }

    #endregion

    #region Event Tests

    [Fact]
    public async Task GetEventAsync_ShouldReturnEvent_WhenApiReturnsData()
    {
        // Arrange
        var eventDto = _builder.CreateCheckInEventDto(e => e.Id = "event-123");
        var response = new JsonApiSingleResponse<Models.JsonApi.CheckIns.EventDto> { Data = eventDto };
        _mockApiConnection.SetupGetResponse("/check_ins/v2/events/event-123", response);

        // Act
        var result = await _checkInsService.GetEventAsync("event-123");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("event-123");
        result!.Name.Should().Be(eventDto.Attributes.Name);
        result!.DataSource.Should().Be("CheckIns");
    }

    [Fact]
    public async Task ListEventsAsync_ShouldReturnPagedEvents_WhenApiReturnsData()
    {
        // Arrange
        var eventDtos = new[]
        {
            _builder.CreateCheckInEventDto(e => e.Id = "1"),
            _builder.CreateCheckInEventDto(e => e.Id = "2")
        };
        _mockApiConnection.SetupGetResponse("/check_ins/v2/events", eventDtos);

        // Act
        var result = await _checkInsService.ListEventsAsync();

        // Assert
        result.Should().NotBeNull();
        result!.Data.Should().HaveCount(2);
        result!.Data[0].Id.Should().Be("1");
        result!.Data[1].Id.Should().Be("2");
    }

    [Fact]
    public async Task ListEventCheckInsAsync_ShouldReturnPagedCheckIns_WhenApiReturnsData()
    {
        // Arrange
        var checkInDtos = new[]
        {
            _builder.CreateCheckInDto(c => c.Id = "1"),
            _builder.CreateCheckInDto(c => c.Id = "2")
        };
        _mockApiConnection.SetupGetResponse("/check_ins/v2/events/event-123/check_ins", checkInDtos);

        // Act
        var result = await _checkInsService.ListEventCheckInsAsync("event-123");

        // Assert
        result.Should().NotBeNull();
        result!.Data.Should().HaveCount(2);
        result!.Data[0].Id.Should().Be("1");
        result!.Data[1].Id.Should().Be("2");
    }

    [Fact]
    public async Task ListEventCheckInsAsync_ShouldThrowArgumentException_WhenEventIdIsEmpty()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _checkInsService.ListEventCheckInsAsync(""));
    }

    #endregion

    #region Pagination Tests

    [Fact]
    public async Task GetAllCheckInsAsync_ShouldReturnAllCheckIns_WhenMultiplePagesExist()
    {
        // Arrange
        var checkInDtos = new[]
        {
            _builder.CreateCheckInDto(c => c.Id = "1"),
            _builder.CreateCheckInDto(c => c.Id = "2"),
            _builder.CreateCheckInDto(c => c.Id = "3")
        };
        _mockApiConnection.SetupGetResponse("/check_ins/v2/check_ins", checkInDtos);

        // Act
        var result = await _checkInsService.GetAllCheckInsAsync();

        // Assert
        result.Should().NotBeNull();
        result!.Should().HaveCount(3);
        result![0].Id.Should().Be("1");
        result![1].Id.Should().Be("2");
        result![2].Id.Should().Be("3");
    }

    [Fact]
    public async Task StreamCheckInsAsync_ShouldYieldCheckIns_WhenDataExists()
    {
        // Arrange
        var checkInDtos = new[]
        {
            _builder.CreateCheckInDto(c => c.Id = "1"),
            _builder.CreateCheckInDto(c => c.Id = "2")
        };
        _mockApiConnection.SetupGetResponse("/check_ins/v2/check_ins", checkInDtos);

        // Act
        var checkIns = new List<Models.CheckIns.CheckIn>();
        await foreach (var checkIn in _checkInsService.StreamCheckInsAsync())
        {
            checkIns.Add(checkIn);
        }

        // Assert
        checkIns.Should().HaveCount(2);
        checkIns![0].Id.Should().Be("1");
        checkIns![1].Id.Should().Be("2");
    }

    #endregion
}