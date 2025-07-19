using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.JsonApi;
using PlanningCenter.Api.Client.Models.JsonApi.Registrations;
using PlanningCenter.Api.Client.Models.Registrations;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Services;
using PlanningCenter.Api.Client.Tests.Utilities;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Services;

/// <summary>
/// Unit tests for RegistrationsService.
/// Follows the same comprehensive patterns as other service tests.
/// </summary>
public class RegistrationsServiceTests
{
    private readonly MockApiConnection _mockApiConnection = new();
    private readonly RegistrationsService _registrationsService;
    private readonly ExtendedTestDataBuilder _builder = new();

    public RegistrationsServiceTests()
    {
        _registrationsService = new RegistrationsService(_mockApiConnection, NullLogger<RegistrationsService>.Instance);
    }

    #region Signup Management Tests

    [Fact]
    public async Task GetSignupAsync_ShouldReturnSignup_WhenApiReturnsData()
    {
        // Arrange
        var signupDto = CreateSignupDto(s => s.Id = "signup123");
        var response = new JsonApiSingleResponse<SignupDto> { Data = signupDto };
        _mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123", response);

        // Act
        var result = await _registrationsService.GetSignupAsync("signup123");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("signup123");
        result!.Name.Should().Be(signupDto.Attributes.Name);
        result!.DataSource.Should().Be("Registrations");
    }

    [Fact]
    public async Task GetSignupAsync_ShouldReturnNull_WhenApiReturnsNull()
    {
        // Arrange
        var response = new JsonApiSingleResponse<SignupDto> { Data = null };
        _mockApiConnection.SetupGetResponse("/registrations/v2/signups/nonexistent", response);

        // Act
        var result = await _registrationsService.GetSignupAsync("nonexistent");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetSignupAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _registrationsService.GetSignupAsync(""));
    }

    [Fact]
    public async Task ListSignupsAsync_ShouldReturnPagedSignups_WhenApiReturnsData()
    {
        // Arrange
        var signupsResponse = CreateSignupCollectionResponse(3);
        _mockApiConnection.SetupGetResponse("/registrations/v2/signups", signupsResponse);

        // Act
        var result = await _registrationsService.ListSignupsAsync();

        // Assert
        result.Should().NotBeNull();
        result!.Data.Should().HaveCount(3);
        result!.Meta.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateSignupAsync_ShouldReturnCreatedSignup_WhenRequestIsValid()
    {
        // Arrange
        var request = new SignupCreateRequest
        {
            Name = "Summer Camp 2024",
            Description = "Annual summer camp for kids"
        };

        var signupDto = CreateSignupDto(s =>
        {
            s.Id = "newsignup123";
            s.Attributes.Name = "Summer Camp 2024";
            s.Attributes.Description = "Annual summer camp for kids";
        });

        var response = new JsonApiSingleResponse<SignupDto> { Data = signupDto };
        _mockApiConnection.SetupMutationResponse("POST", "/registrations/v2/signups", response);

        // Act
        var result = await _registrationsService.CreateSignupAsync(request);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("newsignup123");
        result!.Name.Should().Be("Summer Camp 2024");
        result!.Description.Should().Be("Annual summer camp for kids");
    }

    [Fact]
    public async Task CreateSignupAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _registrationsService.CreateSignupAsync(null!));
    }

    [Fact]
    public async Task UpdateSignupAsync_ShouldReturnUpdatedSignup_WhenRequestIsValid()
    {
        // Arrange
        var request = new SignupUpdateRequest
        {
            Name = "Updated Camp Name",
            Description = "Updated description"
        };

        var signupDto = CreateSignupDto(s =>
        {
            s.Id = "signup123";
            s.Attributes.Name = "Updated Camp Name";
            s.Attributes.Description = "Updated description";
        });

        var response = new JsonApiSingleResponse<SignupDto> { Data = signupDto };
        _mockApiConnection.SetupMutationResponse("PATCH", "/registrations/v2/signups/signup123", response);

        // Act
        var result = await _registrationsService.UpdateSignupAsync("signup123", request);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("signup123");
        result!.Name.Should().Be("Updated Camp Name");
        result!.Description.Should().Be("Updated description");
    }

    [Fact]
    public async Task UpdateSignupAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        // Arrange
        var request = new SignupUpdateRequest { Name = "Updated Name" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _registrationsService.UpdateSignupAsync("", request));
    }

    [Fact]
    public async Task DeleteSignupAsync_ShouldCompleteSuccessfully_WhenIdIsValid()
    {
        // Act & Assert (should not throw)
        await _registrationsService.DeleteSignupAsync("signup123");
    }

    [Fact]
    public async Task DeleteSignupAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _registrationsService.DeleteSignupAsync(""));
    }

    #endregion

    #region Registration Processing Tests

    [Fact]
    public async Task GetRegistrationAsync_ShouldReturnRegistration_WhenApiReturnsData()
    {
        // Arrange
        var registrationDto = CreateRegistrationDto(r => r.Id = "reg123");
        var response = new JsonApiSingleResponse<RegistrationDto> { Data = registrationDto };
        _mockApiConnection.SetupGetResponse("/registrations/v2/registrations/reg123", response);

        // Act
        var result = await _registrationsService.GetRegistrationAsync("reg123");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("reg123");
        result!.Status.Should().Be(registrationDto.Attributes.Status);
    }

    [Fact]
    public async Task ListRegistrationsAsync_ShouldReturnPagedRegistrations_WhenApiReturnsData()
    {
        // Arrange
        var registrationsResponse = CreateRegistrationCollectionResponse(2);
        _mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123/registrations", registrationsResponse);

        // Act
        var result = await _registrationsService.ListRegistrationsAsync("signup123");

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Meta.Should().NotBeNull();
    }

    [Fact]
    public async Task SubmitRegistrationAsync_ShouldReturnRegistration_WhenRequestIsValid()
    {
        // Arrange
        var request = new RegistrationCreateRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };

        var registrationDto = CreateRegistrationDto(r =>
        {
            r.Id = "newreg123";
            r.Attributes.FirstName = "John";
            r.Attributes.LastName = "Doe";
            r.Attributes.Email = "john.doe@example.com";
            r.Attributes.Status = "confirmed";
        });

        var response = new JsonApiSingleResponse<RegistrationDto> { Data = registrationDto };
        _mockApiConnection.SetupMutationResponse("POST", "/registrations/v2/signups/signup123/registrations", response);

        // Act
        var result = await _registrationsService.SubmitRegistrationAsync("signup123", request);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("newreg123");
        result!.FirstName.Should().Be("John");
        result!.LastName.Should().Be("Doe");
        result!.Email.Should().Be("john.doe@example.com");
        result!.Status.Should().Be("confirmed");
    }

    #endregion

    #region Attendee Management Tests

    [Fact]
    public async Task GetAttendeeAsync_ShouldReturnAttendee_WhenApiReturnsData()
    {
        // Arrange
        var attendeeDto = CreateAttendeeDto(a => a.Id = "attendee123");
        var response = new JsonApiSingleResponse<AttendeeDto> { Data = attendeeDto };
        _mockApiConnection.SetupGetResponse("/registrations/v2/attendees/attendee123", response);

        // Act
        var result = await _registrationsService.GetAttendeeAsync("attendee123");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("attendee123");
        result!.FirstName.Should().Be(attendeeDto.Attributes.FirstName);
        result!.LastName.Should().Be(attendeeDto.Attributes.LastName);
    }

    [Fact]
    public async Task ListAttendeesAsync_ShouldReturnPagedAttendees_WhenApiReturnsData()
    {
        // Arrange
        var attendeesResponse = CreateAttendeeCollectionResponse(3);
        _mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123/attendees", attendeesResponse);

        // Act
        var result = await _registrationsService.ListAttendeesAsync("signup123");

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(3);
        result.Meta.Should().NotBeNull();
    }

    [Fact]
    public async Task AddAttendeeAsync_ShouldReturnAttendee_WhenRequestIsValid()
    {
        // Arrange
        var request = new AttendeeCreateRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };

        var attendeeDto = CreateAttendeeDto(a =>
        {
            a.Id = "newattendee123";
            a.Attributes.FirstName = "John";
            a.Attributes.LastName = "Doe";
            a.Attributes.Email = "john.doe@example.com";
        });

        var response = new JsonApiSingleResponse<AttendeeDto> { Data = attendeeDto };
        _mockApiConnection.SetupMutationResponse("POST", "/registrations/v2/signups/signup123/attendees", response);

        // Act
        var result = await _registrationsService.AddAttendeeAsync("signup123", request);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("newattendee123");
        result!.FirstName.Should().Be("John");
        result!.LastName.Should().Be("Doe");
        result!.Email.Should().Be("john.doe@example.com");
    }

    [Fact]
    public async Task UpdateAttendeeAsync_ShouldReturnUpdatedAttendee_WhenRequestIsValid()
    {
        // Arrange
        var request = new AttendeeUpdateRequest
        {
            FirstName = "Jane",
            LastName = "Smith"
        };

        var attendeeDto = CreateAttendeeDto(a =>
        {
            a.Id = "attendee123";
            a.Attributes.FirstName = "Jane";
            a.Attributes.LastName = "Smith";
        });

        var response = new JsonApiSingleResponse<AttendeeDto> { Data = attendeeDto };
        _mockApiConnection.SetupMutationResponse("PATCH", "/registrations/v2/attendees/attendee123", response);

        // Act
        var result = await _registrationsService.UpdateAttendeeAsync("attendee123", request);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("attendee123");
        result!.FirstName.Should().Be("Jane");
        result!.LastName.Should().Be("Smith");
    }

    [Fact]
    public async Task DeleteAttendeeAsync_ShouldCompleteSuccessfully_WhenIdIsValid()
    {
        // Act & Assert (should not throw)
        await _registrationsService.DeleteAttendeeAsync("attendee123");
    }

    #endregion

    #region Waitlist Management Tests

    [Fact]
    public async Task AddToWaitlistAsync_ShouldReturnAttendee_WhenRequestIsValid()
    {
        // Arrange
        var request = new AttendeeCreateRequest
        {
            FirstName = "Waitlist",
            LastName = "Person",
            Email = "waitlist@example.com"
        };

        var attendeeDto = CreateAttendeeDto(a =>
        {
            a.Id = "waitlistattendee123";
            a.Attributes.FirstName = "Waitlist";
            a.Attributes.LastName = "Person";
            a.Attributes.Email = "waitlist@example.com";
        });

        var response = new JsonApiSingleResponse<AttendeeDto> { Data = attendeeDto };
        _mockApiConnection.SetupMutationResponse("POST", "/registrations/v2/signups/signup123/waitlist", response);

        // Act
        var result = await _registrationsService.AddToWaitlistAsync("signup123", request);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("waitlistattendee123");
        result!.FirstName.Should().Be("Waitlist");
        result!.LastName.Should().Be("Person");
    }

    [Fact]
    public async Task RemoveFromWaitlistAsync_ShouldReturnAttendee_WhenIdIsValid()
    {
        // Arrange
        var attendeeDto = CreateAttendeeDto(a => a.Id = "attendee123");
        var response = new JsonApiSingleResponse<AttendeeDto> { Data = attendeeDto };
        _mockApiConnection.SetupMutationResponse("POST", "/registrations/v2/attendees/attendee123/remove_from_waitlist", response);

        // Act
        var result = await _registrationsService.RemoveFromWaitlistAsync("attendee123");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("attendee123");
    }

    [Fact]
    public async Task PromoteFromWaitlistAsync_ShouldReturnAttendee_WhenIdIsValid()
    {
        // Arrange
        var attendeeDto = CreateAttendeeDto(a => a.Id = "attendee123");
        var response = new JsonApiSingleResponse<AttendeeDto> { Data = attendeeDto };
        _mockApiConnection.SetupMutationResponse("POST", "/registrations/v2/attendees/attendee123/promote_from_waitlist", response);

        // Act
        var result = await _registrationsService.PromoteFromWaitlistAsync("attendee123");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("attendee123");
    }

    #endregion

    #region Reporting Tests


    #endregion

    #region Pagination Helper Tests


    #endregion

    // Helper methods for creating test DTOs
    private SignupDto CreateSignupDto(Action<SignupDto> customize)
    {
        var dto = new SignupDto
        {
            Id = "signup123",
            Type = "Signup",
            Attributes = new SignupAttributesDto
            {
                Name = "Test Signup",
                Description = "A test signup",
                Status = "active",
                OpenAt = DateTime.UtcNow.AddDays(-7),
                CloseAt = DateTime.UtcNow.AddDays(7),
                RegistrationLimit = 100,
                RegistrationCount = 25,
                WaitlistCount = 5,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize(dto);
        return dto;
    }

    private PagedResponse<SignupDto> CreateSignupCollectionResponse(int count)
    {
        var signups = new List<SignupDto>();
        for (int i = 0; i < count; i++)
        {
            signups.Add(CreateSignupDto(s => s.Id = $"signup{i + 1}"));
        }

        return new PagedResponse<SignupDto>
        {
            Data = signups,
            Meta = new() { Count = count, TotalCount = count },
            Links = new() { Self = "/registrations/v2/signups" }
        };
    }

    private RegistrationDto CreateRegistrationDto(Action<RegistrationDto> customize)
    {
        var dto = new RegistrationDto
        {
            Id = "reg123",
            Type = "Registration",
            Attributes = new RegistrationAttributesDto
            {
                // Note: PersonId and SignupId are in relationships, not attributes
                Status = "confirmed",
                CompletedAt = DateTime.UtcNow.AddDays(-1),
                TotalCost = 50.00m,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize(dto);
        return dto;
    }

    private PagedResponse<RegistrationDto> CreateRegistrationCollectionResponse(int count)
    {
        var registrations = new List<RegistrationDto>();
        for (int i = 0; i < count; i++)
        {
            registrations.Add(CreateRegistrationDto(r => r.Id = $"reg{i + 1}"));
        }

        return new PagedResponse<RegistrationDto>
        {
            Data = registrations,
            Meta = new() { Count = count, TotalCount = count },
            Links = new() { Self = "/registrations/v2/registrations" }
        };
    }

    private AttendeeDto CreateAttendeeDto(Action<AttendeeDto> customize)
    {
        var dto = new AttendeeDto
        {
            Id = "attendee123",
            Type = "Attendee",
            Attributes = new AttendeeAttributesDto
            {
                FirstName = "Test",
                LastName = "Attendee",
                Email = "test.attendee@example.com",
                PhoneNumber = "555-123-4567",
                Birthdate = DateTime.Parse("1990-01-01"),
                Grade = "Adult",
                Gender = "Other",
                MedicalNotes = "No known allergies",
                Status = "confirmed",
                CheckedInAt = null,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize(dto);
        return dto;
    }

    private PagedResponse<AttendeeDto> CreateAttendeeCollectionResponse(int count)
    {
        var attendees = new List<AttendeeDto>();
        for (int i = 0; i < count; i++)
        {
            attendees.Add(CreateAttendeeDto(a => a.Id = $"attendee{i + 1}"));
        }

        return new PagedResponse<AttendeeDto>
        {
            Data = attendees,
            Meta = new() { Count = count, TotalCount = count },
            Links = new() { Self = "/registrations/v2/attendees" }
        };
    }
}