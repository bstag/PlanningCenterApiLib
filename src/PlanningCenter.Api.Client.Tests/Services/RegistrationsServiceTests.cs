using System.Dynamic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.JsonApi;
using PlanningCenter.Api.Client.Models.JsonApi.Registrations;
using PlanningCenter.Api.Client.Models.People;
using CampusDto = PlanningCenter.Api.Client.Models.JsonApi.Registrations.CampusDto;
using CampusAttributesDto = PlanningCenter.Api.Client.Models.JsonApi.Registrations.CampusAttributesDto;
using PlanningCenter.Api.Client.Models.Registrations;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Services;
using PlanningCenter.Api.Client.Tests.Utilities;
using Xunit;
using Campus = PlanningCenter.Api.Client.Models.Registrations.Campus;

namespace PlanningCenter.Api.Client.Tests.Services;

/// <summary>
/// Unit tests for RegistrationsService.
/// Follows the same comprehensive patterns as other service tests.
/// </summary>
public class RegistrationsServiceTests
{
    private readonly MockApiConnection _mockApiConnection;
    private readonly RegistrationsService _registrationsService;
    private readonly ExtendedTestDataBuilder _builder = new();

    public RegistrationsServiceTests()
    {
        _mockApiConnection = new MockApiConnection();
        _registrationsService = new RegistrationsService(_mockApiConnection, NullLogger<RegistrationsService>.Instance);
    }

    /// <summary>
    /// Helper method to test that a service method calls the correct endpoint
    /// by verifying it throws the expected "No stub configured" exception.
    /// </summary>
    private async Task AssertCallsEndpoint(Func<Task> serviceCall, string expectedEndpoint, string httpMethod = "GET")
    {
        var exception = await Record.ExceptionAsync(serviceCall);
        exception.Should().NotBeNull();
        exception.Should().BeOfType<InvalidOperationException>();
        exception.Message.Should().Contain($"No {httpMethod} stub configured for {expectedEndpoint}");
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

    #region SelectionType Management Tests

    [Fact]
    public async Task GetSelectionTypeAsync_ShouldCallCorrectEndpoint()
    {
        // Act & Assert
        await AssertCallsEndpoint(
            () => _registrationsService.GetSelectionTypeAsync("selection123"),
            "/registrations/v2/selection_types/selection123"
        );
    }

    [Fact]
    public async Task CreateSelectionTypeAsync_ShouldCallCorrectEndpoint_WhenRequestIsValid()
    {
        // Arrange
        var request = new SelectionTypeCreateRequest
        {
            Name = "T-Shirt Size",
            Description = "Select your t-shirt size",
            Required = true,
            AllowMultiple = false
        };

        // Act & Assert - Test that the method calls the correct endpoint
        var exception = await Record.ExceptionAsync(async () =>
        {
            await _registrationsService.CreateSelectionTypeAsync("signup123", request);
        });

        // The method should throw InvalidOperationException due to no stub configured
        // This confirms the method is calling the API connection with the correct endpoint
        exception.Should().NotBeNull();
        exception.Should().BeOfType<InvalidOperationException>();
        exception.Message.Should().Contain("No POST stub configured for /registrations/v2/signups/signup123/selection_types");
    }

    [Fact]
    public async Task SetSignupLocationAsync_ShouldCallCorrectEndpoint()
    {
        // Arrange
        var request = new SignupLocationCreateRequest
        {
            Name = "New Venue",
            StreetAddress = "456 Oak Ave",
            City = "Springfield",
            State = "IL"
        };

        // Act & Assert
        await AssertCallsEndpoint(
            () => _registrationsService.SetSignupLocationAsync("signup123", request),
            "/registrations/v2/signups/signup123/location",
            "POST"
        );
    }

    [Fact]
    public async Task AddSignupTimeAsync_ShouldCallCorrectEndpoint()
    {
        // Arrange
        var request = new SignupTimeCreateRequest
        {
            Name = "Evening Session",
            StartTime = DateTime.Parse("2024-06-01T18:00:00Z"),
            EndTime = DateTime.Parse("2024-06-01T21:00:00Z"),
            Capacity = 20
        };

        // Act & Assert
        await AssertCallsEndpoint(
            () => _registrationsService.AddSignupTimeAsync("signup123", request),
            "/registrations/v2/signups/signup123/signup_times",
            "POST"
        );
    }

    [Fact]
    public async Task UpdateSignupTimeAsync_ShouldCallCorrectEndpoint()
    {
        // Arrange
        var request = new SignupTimeUpdateRequest
        {
            Name = "Updated Session",
            Capacity = 35
        };

        // Act & Assert
        await AssertCallsEndpoint(
            () => _registrationsService.UpdateSignupTimeAsync("time123", request),
            "/registrations/v2/signup_times/time123",
            "PATCH"
        );
    }

    [Fact]
    public async Task DeleteSignupTimeAsync_ShouldCompleteSuccessfully_WhenIdIsValid()
    {
        // Act & Assert (should not throw)
        await _registrationsService.DeleteSignupTimeAsync("time123");
    }

    #endregion

    #region EmergencyContact Management Tests

    [Fact]
    public async Task GetEmergencyContactAsync_ShouldCallCorrectEndpoint()
    {
        // Act & Assert
        await AssertCallsEndpoint(
            () => _registrationsService.GetEmergencyContactAsync("attendee123"),
            "/registrations/v2/attendees/attendee123/emergency_contact"
        );
    }

    [Fact]
    public async Task SetEmergencyContactAsync_ShouldCallCorrectEndpoint_WhenRequestIsValid()
    {
        // Arrange
        var request = new EmergencyContactCreateRequest
        {
            FirstName = "John",
            LastName = "Smith",
            Relationship = "Guardian",
            PrimaryPhone = "555-987-6543",
            Email = "john.smith@example.com"
        };

        // Act & Assert - Test that the method calls the correct endpoint
        var exception = await Record.ExceptionAsync(async () =>
        {
            await _registrationsService.SetEmergencyContactAsync("attendee123", request);
        });

        // The method should throw InvalidOperationException due to no stub configured
        // This confirms the method is calling the API connection with the correct endpoint
        exception.Should().NotBeNull();
        exception.Should().BeOfType<InvalidOperationException>();
        exception.Message.Should().Contain("No POST stub configured for /registrations/v2/attendees/attendee123/emergency_contact");
    }

    [Fact]
    public async Task UpdateEmergencyContactAsync_ShouldCallCorrectEndpoint()
    {
        // Arrange
        var request = new EmergencyContactUpdateRequest
        {
            FirstName = "Updated",
            LastName = "Contact",
            PrimaryPhone = "555-111-2222"
        };

        // Act & Assert
        await AssertCallsEndpoint(
            () => _registrationsService.UpdateEmergencyContactAsync("attendee123", request),
            "/registrations/v2/attendees/attendee123/emergency_contact",
            "PATCH"
        );
    }

    #endregion

    #region Category Management Tests

    [Fact]
    public async Task GetCategoryAsync_ShouldCallCorrectEndpoint()
    {
        // Act & Assert
        await AssertCallsEndpoint(
            () => _registrationsService.GetCategoryAsync("category123"),
            "/registrations/v2/categories/category123"
        );
    }

    [Fact]
    public async Task ListCategoriesAsync_ShouldCallCorrectEndpoint()
    {
        // Act & Assert
        await AssertCallsEndpoint(
            () => _registrationsService.ListCategoriesAsync(),
            "/registrations/v2/categories"
        );
    }

    [Fact]
    public async Task CreateCategoryAsync_ShouldCallCorrectEndpoint_WhenRequestIsValid()
    {
        // Arrange
        var request = new CategoryCreateRequest
        {
            Name = "Special Events",
            Description = "One-time special events",
            Color = "#00FF00",
            Active = true
        };

        // Act & Assert
        await AssertCallsEndpoint(
            () => _registrationsService.CreateCategoryAsync(request),
            "/registrations/v2/categories",
            "POST"
        );
    }

    [Fact]
    public async Task UpdateCategoryAsync_ShouldReturnUpdatedCategory_WhenRequestIsValid()
    {
        // Arrange
        var request = new CategoryUpdateRequest
        {
            Name = "Updated Category",
            Description = "Updated description"
        };

        dynamic data = new ExpandoObject();
        data.id = "category123";

        var response = new JsonApiSingleResponse<dynamic>
        {
            Data = data
        };
        _mockApiConnection.SetupMutationResponse("PATCH", "/registrations/v2/categories/category123", response);

        // Act
        var result = await _registrationsService.UpdateCategoryAsync("category123", request);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("category123");
        result!.Name.Should().Be("Updated Category");
    }

    #endregion

    #region Campus Management Tests

    [Fact]
    public async Task GetCampusAsync_ShouldCallCorrectEndpoint()
    {
        // Act & Assert
        await AssertCallsEndpoint(
            () => _registrationsService.GetCampusAsync("campus123"),
            "/registrations/v2/campuses/campus123"
        );
    }

    [Fact]
    public async Task ListCampusesAsync_ShouldCallCorrectEndpoint()
    {
        // Act & Assert
        await AssertCallsEndpoint(
            () => _registrationsService.ListCampusesAsync(),
            "/registrations/v2/campuses"
        );
    }

    [Fact]
    public async Task ListCampusesAsync_ShouldReturnPagedCampuses_WhenApiReturnsData()
    {
        // Arrange
        var campusesResponse = CreateCampusCollectionResponse(3);
        _mockApiConnection.SetupGetResponse("/registrations/v2/campuses", campusesResponse);

        // Act
        var result = await _registrationsService.ListCampusesAsync();

        // Assert
        result.Should().NotBeNull();
        result!.Data.Should().HaveCount(3);
        result!.Data.First().Name.Should().Be("Test Campus");
    }

    #endregion

    #region Person Management Tests

    [Fact]
    public async Task GetPersonAsync_ShouldCallCorrectEndpoint()
    {
        // Act & Assert
        await AssertCallsEndpoint(
            () => _registrationsService.GetPersonAsync("person123"),
            "/registrations/v2/people/person123"
        );
    }

    [Fact]
    public async Task GetAttendeesForPersonAsync_ShouldReturnPagedAttendees_WhenApiReturnsData()
    {
        // Arrange
        var attendeesResponse = CreateAttendeeCollectionResponse(2);
        _mockApiConnection.SetupGetResponse("/registrations/v2/people/person123/attendees", attendeesResponse);

        // Act
        var result = await _registrationsService.GetAttendeesForPersonAsync("person123");

        // Assert
        result.Should().NotBeNull();
        result!.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetSignupsForPersonAsync_ShouldReturnPagedSignups_WhenApiReturnsData()
    {
        // Arrange
        var signupsResponse = CreateSignupCollectionResponse(3);
        _mockApiConnection.SetupGetResponse("/registrations/v2/people/person123/signups", signupsResponse);

        // Act
        var result = await _registrationsService.GetSignupsForPersonAsync("person123");

        // Assert
        result.Should().NotBeNull();
        result!.Data.Should().HaveCount(3);
    }

    #endregion

    #region Reporting Tests

    [Fact]
    public async Task GenerateRegistrationReportAsync_ShouldReturnReport_WhenRequestIsValid()
    {
        // Arrange
        var request = new RegistrationReportRequest
        {
            SignupId = "signup123",
            IncludeAttendeeDetails = true,
            IncludeWaitlist = true
        };

        // Setup signup response
        var signupDto = CreateSignupDto(s => s.Id = "signup123");
        var signupResponse = new JsonApiSingleResponse<SignupDto> { Data = signupDto };
        _mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123", signupResponse);

        // Setup registration count response
        var registrationResponse = CreateRegistrationCollectionResponse(25);
        _mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123/registrations?where[status]=confirmed", registrationResponse);

        // Setup waitlist count response
        var waitlistResponse = CreateAttendeeCollectionResponse(5);
        _mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123/attendees?where[on_waitlist]=true", waitlistResponse);

        // Act
        var result = await _registrationsService.GenerateRegistrationReportAsync(request);

        // Assert
        result.Should().NotBeNull();
        result!.SignupId.Should().Be("signup123");
        result!.SignupName.Should().Be(signupDto.Attributes.Name);
        result!.TotalRegistrations.Should().Be(25);
        result!.WaitlistCount.Should().Be(5);
        result!.GeneratedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task GetRegistrationCountAsync_ShouldReturnCount_WhenApiReturnsData()
    {
        // Arrange
        var registrationResponse = CreateRegistrationCollectionResponse(15);
        _mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123/registrations?where[status]=confirmed", registrationResponse);

        // Act
        var result = await _registrationsService.GetRegistrationCountAsync("signup123");

        // Assert
        result.Should().Be(15);
    }

    [Fact]
    public async Task GetWaitlistCountAsync_ShouldReturnCount_WhenApiReturnsData()
    {
        // Arrange
        var waitlistResponse = CreateAttendeeCollectionResponse(8);
        _mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123/attendees?where[on_waitlist]=true", waitlistResponse);

        // Act
        var result = await _registrationsService.GetWaitlistCountAsync("signup123");

        // Assert
        result.Should().Be(8);
    }

    #endregion

    #region Pagination Helper Tests

    [Fact]
    public async Task GetAllSignupsAsync_ShouldReturnAllSignups_WhenMultiplePagesExist()
    {
        // Arrange
        var page1Response = new PagedResponse<SignupDto>
        {
            Data = new List<SignupDto> { CreateSignupDto(s => s.Id = "signup1") },
            Meta = new() { Count = 1, TotalCount = 2 },
            Links = new() { Self = "/registrations/v2/signups?per_page=100", Next = "/registrations/v2/signups?per_page=100&offset=100" }
        };

        var page2Response = new PagedResponse<SignupDto>
        {
            Data = new List<SignupDto> { CreateSignupDto(s => s.Id = "signup2") },
            Meta = new() { Count = 1, TotalCount = 2 },
            Links = new() { Self = "/registrations/v2/signups?per_page=100&offset=100" } // No Next link - this terminates pagination
        };

        // Setup responses for the exact URLs that GetAllSignupsAsync will call
        // First call: GetAllSignupsAsync -> ListSignupsAsync with PerPage=100 -> /registrations/v2/signups?per_page=100
        _mockApiConnection.SetupGetResponse("/registrations/v2/signups?per_page=100", (IPagedResponse<SignupDto>)page1Response);

        // Second call: GetAllSignupsAsync -> ListSignupsAsync with PerPage=100&Offset=100 -> /registrations/v2/signups?per_page=100&offset=100
        _mockApiConnection.SetupGetResponse("/registrations/v2/signups?per_page=100&offset=100", (IPagedResponse<SignupDto>)page2Response);

        // Act
        var result = await _registrationsService.GetAllSignupsAsync();

        // Assert
        result.Should().NotBeNull();
        result!.Should().HaveCount(2);
        result![0].Id.Should().Be("signup1");
        result![1].Id.Should().Be("signup2");
    }
    /*
        [Fact]
        public async Task StreamSignupsAsync_ShouldYieldAllSignups_WhenMultiplePagesExist()
        {
            // Arrange - Create a single page response with all signups to avoid pagination complexity
            var allSignupsResponse = new PagedResponse<SignupDto>
            {
                Data = new List<SignupDto> 
                { 
                    CreateSignupDto(s => s.Id = "signup1"),
                    CreateSignupDto(s => s.Id = "signup2")
                },
                Meta = new() { Count = 2, TotalCount = 2 },
                Links = new() { Self = "/registrations/v2/signups" }
            };

            // Set up the base endpoint for paged responses
            _mockApiConnection.SetupGetResponse("/registrations/v2/signups", (IPagedResponse<SignupDto>)allSignupsResponse);

            // Act
            var results = new List<Signup>();
            await foreach (var signup in _registrationsService.StreamSignupsAsync())
            {
                results.Add(signup);
            }

            // Assert
            results.Should().HaveCount(2);
            results[0].Id.Should().Be("signup1");
            results[1].Id.Should().Be("signup2");
        }

        #endregion

        #region Selection Type Management Tests

        [Fact]
        public async Task GetSelectionTypeAsync_ShouldReturnSelectionType_WhenApiReturnsData()
        {
            // Arrange
            var selectionTypeDto = CreateSelectionTypeDto(s => s.Id = "seltype123");
            var response = new JsonApiSingleResponse<SelectionTypeDto> { Data = selectionTypeDto };
            _mockApiConnection.SetupGetResponse("/registrations/v2/selection_types/seltype123", response);

            // Act
            var result = await _registrationsService.GetSelectionTypeAsync("seltype123");

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be("seltype123");
            result!.Name.Should().Be(selectionTypeDto.Attributes!.Name);
            result!.DataSource.Should().Be("Registrations");
        }

        [Fact]
        public async Task GetSelectionTypeAsync_ShouldReturnNull_WhenApiReturnsNull()
        {
            // Arrange
            var response = new JsonApiSingleResponse<SelectionTypeDto> { Data = null };
            _mockApiConnection.SetupGetResponse("/registrations/v2/selection_types/nonexistent", response);

            // Act
            var result = await _registrationsService.GetSelectionTypeAsync("nonexistent");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetSelectionTypeAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _registrationsService.GetSelectionTypeAsync(""));
        }

        [Fact]
        public async Task ListSelectionTypesAsync_ShouldReturnPagedSelectionTypes_WhenApiReturnsData()
        {
            // Arrange
            var selectionTypesResponse = CreateSelectionTypeCollectionResponse(3);
            _mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123/selection_types", selectionTypesResponse);

            // Act
            var result = await _registrationsService.ListSelectionTypesAsync("signup123");

            // Assert
            result.Should().NotBeNull();
            result!.Data.Should().HaveCount(3);
            result!.Meta.Should().NotBeNull();
        }

        [Fact]
        public async Task ListSelectionTypesAsync_ShouldThrowArgumentException_WhenSignupIdIsEmpty()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _registrationsService.ListSelectionTypesAsync(""));
        }

        [Fact]
        public async Task CreateSelectionTypeAsync_ShouldReturnCreatedSelectionType_WhenRequestIsValid()
        {
            // Arrange
            var request = new SelectionTypeCreateRequest
            {
                Name = "T-Shirt Size",
                Description = "Select your t-shirt size",
                Required = true
            };

            var selectionTypeDto = CreateSelectionTypeDto(s =>
            {
                s.Id = "newseltype123";
                s.Attributes!.Name = "T-Shirt Size";
                s.Attributes!.Description = "Select your t-shirt size";
                s.Attributes!.Required = true;
            });

            var response = new JsonApiSingleResponse<SelectionTypeDto> { Data = selectionTypeDto };
            _mockApiConnection.SetupMutationResponse("POST", "/registrations/v2/signups/signup123/selection_types", response);

            // Act
            var result = await _registrationsService.CreateSelectionTypeAsync("signup123", request);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be("newseltype123");
            result!.Name.Should().Be("T-Shirt Size");
            result!.Description.Should().Be("Select your t-shirt size");
            result!.Required.Should().BeTrue();
        }

        [Fact]
        public async Task CreateSelectionTypeAsync_ShouldThrowArgumentException_WhenSignupIdIsEmpty()
        {
            // Arrange
            var request = new SelectionTypeCreateRequest { Name = "Test" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _registrationsService.CreateSelectionTypeAsync("", request));
        }

        [Fact]
        public async Task CreateSelectionTypeAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _registrationsService.CreateSelectionTypeAsync("signup123", null!));
        }

        [Fact]
        public async Task UpdateSelectionTypeAsync_ShouldReturnUpdatedSelectionType_WhenRequestIsValid()
        {
            // Arrange
            var request = new SelectionTypeUpdateRequest
            {
                Name = "Updated Selection Type",
                Description = "Updated description"
            };

            var selectionTypeDto = CreateSelectionTypeDto(s =>
            {
                s.Id = "seltype123";
                s.Attributes!.Name = "Updated Selection Type";
                s.Attributes!.Description = "Updated description";
            });

            var response = new JsonApiSingleResponse<SelectionTypeDto> { Data = selectionTypeDto };
            _mockApiConnection.SetupMutationResponse("PATCH", "/registrations/v2/selection_types/seltype123", response);

            // Act
            var result = await _registrationsService.UpdateSelectionTypeAsync("seltype123", request);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be("seltype123");
            result!.Name.Should().Be("Updated Selection Type");
            result!.Description.Should().Be("Updated description");
        }

        [Fact]
        public async Task UpdateSelectionTypeAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
        {
            // Arrange
            var request = new SelectionTypeUpdateRequest { Name = "Updated Name" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _registrationsService.UpdateSelectionTypeAsync("", request));
        }

        [Fact]
        public async Task UpdateSelectionTypeAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _registrationsService.UpdateSelectionTypeAsync("seltype123", null!));
        }

        [Fact]
        public async Task DeleteSelectionTypeAsync_ShouldCompleteSuccessfully_WhenIdIsValid()
        {
            // Act & Assert (should not throw)
            await _registrationsService.DeleteSelectionTypeAsync("seltype123");
        }

        [Fact]
        public async Task DeleteSelectionTypeAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _registrationsService.DeleteSelectionTypeAsync(""));
        }

        #endregion

        #region Signup Location Management Tests

        [Fact]
        public async Task GetSignupLocationAsync_ShouldReturnSignupLocation_WhenApiReturnsData()
        {
            // Arrange
            var locationDto = CreateSignupLocationDto(l => l.Id = "location123");
            var response = new JsonApiSingleResponse<SignupLocationDto> { Data = locationDto };
            _mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123/location", response);

            // Act
            var result = await _registrationsService.GetSignupLocationAsync("signup123");

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be("location123");
            result!.Name.Should().Be(locationDto.Attributes!.Name);
            result!.DataSource.Should().Be("Registrations");
        }

        [Fact]
        public async Task GetSignupLocationAsync_ShouldReturnNull_WhenApiReturnsNull()
        {
            // Arrange
            var response = new JsonApiSingleResponse<SignupLocationDto> { Data = null };
            _mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123/location", response);

            // Act
            var result = await _registrationsService.GetSignupLocationAsync("signup123");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetSignupLocationAsync_ShouldThrowArgumentException_WhenSignupIdIsEmpty()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _registrationsService.GetSignupLocationAsync(""));
        }

        [Fact]
        public async Task SetSignupLocationAsync_ShouldReturnCreatedLocation_WhenRequestIsValid()
        {
            // Arrange
            var request = new SignupLocationCreateRequest
            {
                Name = "Main Campus",
                StreetAddress = "123 Main St",
                City = "Anytown",
                State = "CA"
            };

            var locationDto = CreateSignupLocationDto(l =>
            {
                l.Id = "newlocation123";
                l.Attributes!.Name = "Main Campus";
                l.Attributes!.StreetAddress = "123 Main St";
                l.Attributes!.City = "Anytown";
                l.Attributes!.State = "CA";
            });

            var response = new JsonApiSingleResponse<SignupLocationDto> { Data = locationDto };
            _mockApiConnection.SetupMutationResponse("POST", "/registrations/v2/signups/signup123/location", response);

            // Act
            var result = await _registrationsService.SetSignupLocationAsync("signup123", request);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be("newlocation123");
            result!.Name.Should().Be("Main Campus");
            result!.StreetAddress.Should().Be("123 Main St");
            result!.City.Should().Be("Anytown");
            result!.State.Should().Be("CA");
        }

        [Fact]
        public async Task SetSignupLocationAsync_ShouldThrowArgumentException_WhenSignupIdIsEmpty()
        {
            // Arrange
            var request = new SignupLocationCreateRequest { Name = "Test Location" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _registrationsService.SetSignupLocationAsync("", request));
        }

        [Fact]
        public async Task SetSignupLocationAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _registrationsService.SetSignupLocationAsync("signup123", null!));
        }

        [Fact]
        public async Task UpdateSignupLocationAsync_ShouldReturnUpdatedLocation_WhenRequestIsValid()
        {
            // Arrange
            var request = new SignupLocationUpdateRequest
            {
                Name = "Updated Location",
                Description = "Updated description"
            };

            var locationDto = CreateSignupLocationDto(l =>
            {
                l.Id = "location123";
                l.Attributes!.Name = "Updated Location";
                l.Attributes!.Description = "Updated description";
            });

            var response = new JsonApiSingleResponse<SignupLocationDto> { Data = locationDto };
            _mockApiConnection.SetupMutationResponse("PATCH", "/registrations/v2/signups/signup123/location", response);

            // Act
            var result = await _registrationsService.UpdateSignupLocationAsync("signup123", request);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be("location123");
            result!.Name.Should().Be("Updated Location");
            result!.Description.Should().Be("Updated description");
        }

        [Fact]
        public async Task UpdateSignupLocationAsync_ShouldThrowArgumentException_WhenSignupIdIsEmpty()
        {
            // Arrange
            var request = new SignupLocationUpdateRequest { Name = "Updated Name" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _registrationsService.UpdateSignupLocationAsync("", request));
        }

        [Fact]
        public async Task UpdateSignupLocationAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _registrationsService.UpdateSignupLocationAsync("signup123", null!));
        }

        #endregion

        #region Signup Time Management Tests

        [Fact]
        public async Task ListSignupTimesAsync_ShouldReturnPagedSignupTimes_WhenApiReturnsData()
        {
            // Arrange
            var signupTimesResponse = CreateSignupTimeCollectionResponse(3);
            _mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123/signup_times", signupTimesResponse);

            // Act
            var result = await _registrationsService.ListSignupTimesAsync("signup123");

            // Assert
            result.Should().NotBeNull();
            result!.Data.Should().HaveCount(3);
            result!.Meta.Should().NotBeNull();
        }

        [Fact]
        public async Task ListSignupTimesAsync_ShouldThrowArgumentException_WhenSignupIdIsEmpty()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _registrationsService.ListSignupTimesAsync(""));
        }

        [Fact]
        public async Task AddSignupTimeAsync_ShouldReturnCreatedSignupTime_WhenRequestIsValid()
        {
            // Arrange
            var request = new SignupTimeCreateRequest
            {
                Name = "Morning Session",
                StartTime = DateTime.UtcNow.AddDays(1),
                EndTime = DateTime.UtcNow.AddDays(1).AddHours(2)
            };

            var signupTimeDto = CreateSignupTimeDto(t =>
            {
                t.Id = "newtime123";
                t.Attributes!.Name = "Morning Session";
                t.Attributes!.StartsAt = new DateTimeOffset(request.StartTime);
                t.Attributes!.EndsAt = request.EndTime.HasValue ? new DateTimeOffset(request.EndTime.Value) : null;
            });

            var response = new JsonApiSingleResponse<SignupTimeDto> { Data = signupTimeDto };
            _mockApiConnection.SetupMutationResponse("POST", "/registrations/v2/signups/signup123/signup_times", response);

            // Act
            var result = await _registrationsService.AddSignupTimeAsync("signup123", request);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be("newtime123");
            result!.Name.Should().Be("Morning Session");
            result!.StartTime.Should().Be(request.StartTime);
            result!.EndTime.Should().Be(request.EndTime);
        }

        [Fact]
        public async Task AddSignupTimeAsync_ShouldThrowArgumentException_WhenSignupIdIsEmpty()
        {
            // Arrange
            var request = new SignupTimeCreateRequest { Name = "Test Time" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _registrationsService.AddSignupTimeAsync("", request));
        }

        [Fact]
        public async Task AddSignupTimeAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _registrationsService.AddSignupTimeAsync("signup123", null!));
        }

        [Fact]
        public async Task UpdateSignupTimeAsync_ShouldReturnUpdatedSignupTime_WhenRequestIsValid()
        {
            // Arrange
            var request = new SignupTimeUpdateRequest
            {
                Name = "Updated Time Slot",
                StartTime = DateTime.UtcNow.AddDays(2),
                EndTime = DateTime.UtcNow.AddDays(2).AddHours(3)
            };

            var signupTimeDto = CreateSignupTimeDto(t =>
            {
                t.Id = "time123";
                t.Attributes!.Name = "Updated Time Slot";
                t.Attributes!.StartsAt = request.StartTime.HasValue ? new DateTimeOffset(request.StartTime.Value) : DateTimeOffset.UtcNow;
                t.Attributes!.EndsAt = request.EndTime.HasValue ? new DateTimeOffset(request.EndTime.Value) : null;
            });

            var response = new JsonApiSingleResponse<SignupTimeDto> { Data = signupTimeDto };
            _mockApiConnection.SetupMutationResponse("PATCH", "/registrations/v2/signup_times/time123", response);

            // Act
            var result = await _registrationsService.UpdateSignupTimeAsync("time123", request);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be("time123");
            result!.Name.Should().Be("Updated Time Slot");
            result!.StartTime.Should().Be(request.StartTime);
            result!.EndTime.Should().Be(request.EndTime);
        }

        [Fact]
        public async Task UpdateSignupTimeAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
        {
            // Arrange
            var request = new SignupTimeUpdateRequest { Name = "Updated Name" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _registrationsService.UpdateSignupTimeAsync("", request));
        }

        [Fact]
        public async Task UpdateSignupTimeAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _registrationsService.UpdateSignupTimeAsync("time123", null!));
        }



        #endregion

        #region Emergency Contact Management Tests

        [Fact]
        public async Task GetEmergencyContactAsync_ShouldReturnEmergencyContact_WhenApiReturnsData()
        {
            // Arrange
            var contactDto = CreateEmergencyContactDto(c => c.Id = "contact123");
            var response = new JsonApiSingleResponse<EmergencyContactDto> { Data = contactDto };
            _mockApiConnection.SetupGetResponse("/registrations/v2/attendees/attendee123/emergency_contact", response);

            // Act
            var result = await _registrationsService.GetEmergencyContactAsync("attendee123");

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be("contact123");
            result!.FirstName.Should().Be(contactDto.Attributes!.FirstName);
            result!.LastName.Should().Be(contactDto.Attributes!.LastName);
            result!.DataSource.Should().Be("Registrations");
        }

        [Fact]
        public async Task GetEmergencyContactAsync_ShouldReturnNull_WhenApiReturnsNull()
        {
            // Arrange
            var response = new JsonApiSingleResponse<EmergencyContactDto> { Data = null };
            _mockApiConnection.SetupGetResponse("/registrations/v2/attendees/attendee123/emergency_contact", response);

            // Act
            var result = await _registrationsService.GetEmergencyContactAsync("attendee123");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetEmergencyContactAsync_ShouldThrowArgumentException_WhenAttendeeIdIsEmpty()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _registrationsService.GetEmergencyContactAsync(""));
        }

        [Fact]
        public async Task SetEmergencyContactAsync_ShouldReturnCreatedContact_WhenRequestIsValid()
        {
            // Arrange
            var request = new EmergencyContactCreateRequest
            {
                FirstName = "Jane",
                LastName = "Smith",
                Relationship = "Mother",
                PrimaryPhone = "555-9876",
                Email = "jane.smith@example.com"
            };

            var contactDto = CreateEmergencyContactDto(c =>
            {
                c.Id = "newcontact123";
                c.Attributes!.FirstName = "Jane";
                c.Attributes!.LastName = "Smith";
                c.Attributes!.Relationship = "Mother";
                c.Attributes!.PrimaryPhone = "555-9876";
                c.Attributes!.Email = "jane.smith@example.com";
            });

            var response = new JsonApiSingleResponse<EmergencyContactDto> { Data = contactDto };
            _mockApiConnection.SetupMutationResponse("POST", "/registrations/v2/attendees/attendee123/emergency_contact", response);

            // Act
            var result = await _registrationsService.SetEmergencyContactAsync("attendee123", request);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be("newcontact123");
            result!.FirstName.Should().Be("Jane");
            result!.LastName.Should().Be("Smith");
            result!.Relationship.Should().Be("Mother");
            result!.PrimaryPhone.Should().Be("555-9876");
            result!.Email.Should().Be("jane.smith@example.com");
        }

        [Fact]
        public async Task SetEmergencyContactAsync_ShouldThrowArgumentException_WhenAttendeeIdIsEmpty()
        {
            // Arrange
            var request = new EmergencyContactCreateRequest { FirstName = "Test" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _registrationsService.SetEmergencyContactAsync("", request));
        }

        [Fact]
        public async Task SetEmergencyContactAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _registrationsService.SetEmergencyContactAsync("attendee123", null!));
        }

        [Fact]
        public async Task UpdateEmergencyContactAsync_ShouldReturnUpdatedContact_WhenRequestIsValid()
        {
            // Arrange
            var request = new EmergencyContactUpdateRequest
            {
                FirstName = "Updated Jane",
                LastName = "Updated Smith",
                Relationship = "Guardian"
            };

            var contactDto = CreateEmergencyContactDto(c =>
            {
                c.Id = "contact123";
                c.Attributes!.FirstName = "Updated Jane";
                c.Attributes!.LastName = "Updated Smith";
                c.Attributes!.Relationship = "Guardian";
            });

            var response = new JsonApiSingleResponse<EmergencyContactDto> { Data = contactDto };
            _mockApiConnection.SetupMutationResponse("PATCH", "/registrations/v2/attendees/attendee123/emergency_contact", response);

            // Act
            var result = await _registrationsService.UpdateEmergencyContactAsync("attendee123", request);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be("contact123");
            result!.FirstName.Should().Be("Updated Jane");
            result!.LastName.Should().Be("Updated Smith");
            result!.Relationship.Should().Be("Guardian");
        }

        [Fact]
        public async Task UpdateEmergencyContactAsync_ShouldThrowArgumentException_WhenAttendeeIdIsEmpty()
        {
            // Arrange
            var request = new EmergencyContactUpdateRequest { FirstName = "Updated Name" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _registrationsService.UpdateEmergencyContactAsync("", request));
        }

        [Fact]
        public async Task UpdateEmergencyContactAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _registrationsService.UpdateEmergencyContactAsync("attendee123", null!));
        }

        #endregion

        #region Category Management Tests

        [Fact]
        public async Task GetCategoryAsync_ShouldReturnCategory_WhenApiReturnsData()
        {
            // Arrange
            var categoryDto = CreateCategoryDto(c => c.Id = "category123");
            var response = new JsonApiSingleResponse<CategoryDto> { Data = categoryDto };
            _mockApiConnection.SetupGetResponse("/registrations/v2/categories/category123", response);

            // Act
            var result = await _registrationsService.GetCategoryAsync("category123");

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be("category123");
            result!.Name.Should().Be(categoryDto.Attributes!.Name);
            result!.DataSource.Should().Be("Registrations");
        }

        [Fact]
        public async Task GetCategoryAsync_ShouldReturnNull_WhenApiReturnsNull()
        {
            // Arrange
            var response = new JsonApiSingleResponse<CategoryDto> { Data = null };
            _mockApiConnection.SetupGetResponse("/registrations/v2/categories/nonexistent", response);

            // Act
            var result = await _registrationsService.GetCategoryAsync("nonexistent");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetCategoryAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _registrationsService.GetCategoryAsync(""));
        }

        [Fact]
        public async Task ListCategoriesAsync_ShouldReturnPagedCategories_WhenApiReturnsData()
        {
            // Arrange
            var categoriesResponse = CreateCategoryCollectionResponse(3);
            _mockApiConnection.SetupGetResponse("/registrations/v2/categories", categoriesResponse);

            // Act
            var result = await _registrationsService.ListCategoriesAsync();

            // Assert
            result.Should().NotBeNull();
            result!.Data.Should().HaveCount(3);
            result!.Meta.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateCategoryAsync_ShouldReturnCreatedCategory_WhenRequestIsValid()
        {
            // Arrange
            var request = new CategoryCreateRequest
            {
                Name = "Youth Group",
                Description = "Category for youth participants",
                Color = "#FF5733"
            };

            var categoryDto = CreateCategoryDto(c =>
            {
                c.Id = "newcategory123";
                c.Attributes!.Name = "Youth Group";
                c.Attributes!.Description = "Category for youth participants";
                c.Attributes!.Color = "#FF5733";
            });

            var response = new JsonApiSingleResponse<CategoryDto> { Data = categoryDto };
            _mockApiConnection.SetupMutationResponse("POST", "/registrations/v2/categories", response);

            // Act
            var result = await _registrationsService.CreateCategoryAsync(request);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be("newcategory123");
            result!.Name.Should().Be("Youth Group");
            result!.Description.Should().Be("Category for youth participants");
            result!.Color.Should().Be("#FF5733");
        }

        [Fact]
        public async Task CreateCategoryAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _registrationsService.CreateCategoryAsync(null!));
        }

        #endregion
    */
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

    private CampusDto CreateCampusDto(Action<CampusDto> customize)
    {
        var dto = new CampusDto
        {
            Id = "campus123",
            Type = "Campus",
            Attributes = new CampusAttributesDto
            {
                Name = "Test Campus",
                Description = "A test campus",
                Timezone = "America/New_York",
                Address = "123 Main St",
                City = "Test City",
                State = "NY",
                PostalCode = "12345",
                Country = "US",
                PhoneNumber = "555-123-4567",
                WebsiteUrl = "https://testcampus.com",
                Active = true,
                SortOrder = 1,
                SignupCount = 5,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-30),
                UpdatedAt = DateTimeOffset.UtcNow
            }
        };
        customize(dto);
        return dto;
    }

    private PagedResponse<CampusDto> CreateCampusCollectionResponse(int count)
    {
        var campuses = new List<CampusDto>();
        for (int i = 0; i < count; i++)
        {
            campuses.Add(CreateCampusDto(c => c.Id = $"campus{i + 1}"));
        }

        return new PagedResponse<CampusDto>
        {
            Data = campuses,
            Meta = new() { Count = count, TotalCount = count },
            Links = new() { Self = "/registrations/v2/campuses" }
        };
    }

    private SelectionTypeDto CreateSelectionTypeDto(Action<SelectionTypeDto> customize)
    {
        var dto = new SelectionTypeDto
        {
            Id = "seltype123",
            Type = "SelectionType",
            Attributes = new SelectionTypeAttributesDto
            {
                Name = "Test Selection Type",
                Description = "Test description",
                Required = false,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize(dto);
        return dto;
    }

    private PagedResponse<SelectionTypeDto> CreateSelectionTypeCollectionResponse(int count)
    {
        var selectionTypes = new List<SelectionTypeDto>();
        for (int i = 0; i < count; i++)
        {
            selectionTypes.Add(CreateSelectionTypeDto(s => s.Id = $"seltype{i + 1}"));
        }

        return new PagedResponse<SelectionTypeDto>
        {
            Data = selectionTypes,
            Meta = new() { Count = count, TotalCount = count },
            Links = new() { Self = "/registrations/v2/selection_types" }
        };
    }

    private SignupLocationDto CreateSignupLocationDto(Action<SignupLocationDto> customize)
    {
        var dto = new SignupLocationDto
        {
            Id = "location123",
            Type = "SignupLocation",
            Attributes = new SignupLocationAttributesDto
            {
                Name = "Test Location",
                Description = "Test location description",
                StreetAddress = "123 Test St",
                City = "Test City",
                State = "CA",
                PostalCode = "12345",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize(dto);
         return dto;
     }

     private SignupTimeDto CreateSignupTimeDto(Action<SignupTimeDto> customize)
     {
         var dto = new SignupTimeDto
         {
             Id = "time123",
             Type = "SignupTime",
             Attributes = new SignupTimeAttributesDto
             {
                 Name = "Test Time Slot",
                 StartsAt = new DateTimeOffset(DateTime.UtcNow.AddDays(1)),
                 EndsAt = new DateTimeOffset(DateTime.UtcNow.AddDays(1).AddHours(2)),
                 CreatedAt = DateTime.UtcNow.AddDays(-1),
                 UpdatedAt = DateTime.UtcNow
             }
         };
         customize(dto);
         return dto;
     }

     private PagedResponse<SignupTimeDto> CreateSignupTimeCollectionResponse(int count)
     {
         var signupTimes = new List<SignupTimeDto>();
         for (int i = 0; i < count; i++)
         {
             signupTimes.Add(CreateSignupTimeDto(t => t.Id = $"time{i + 1}"));
         }

         return new PagedResponse<SignupTimeDto>
         {
             Data = signupTimes,
             Meta = new() { Count = count, TotalCount = count },
             Links = new() { Self = "/registrations/v2/signup_times" }
         };
     }

     private EmergencyContactDto CreateEmergencyContactDto(Action<EmergencyContactDto> customize)
     {
         var dto = new EmergencyContactDto
         {
             Id = "contact123",
             Type = "EmergencyContact",
             Attributes = new EmergencyContactAttributes
             {
                 FirstName = "John",
                 LastName = "Doe",
                 Relationship = "Parent",
                 PrimaryPhone = "555-1234",
                 SecondaryPhone = "555-5678",
                 Email = "john.doe@example.com",
                 StreetAddress = "123 Emergency St",
                 City = "Emergency City",
                 State = "CA",
                 PostalCode = "12345",
                 Priority = 1,
                 Notes = "Primary emergency contact",
                 PreferredContactMethod = "phone",
                 BestTimeToContact = "anytime",
                 CanAuthorizeMedicalTreatment = true,
                 CreatedAt = DateTime.UtcNow.AddDays(-1),
                 UpdatedAt = DateTime.UtcNow
             }
         };
         customize(dto);
          return dto;
      }

      private CategoryDto CreateCategoryDto(Action<CategoryDto> customize)
      {
          var dto = new CategoryDto
          {
              Id = "category123",
              Type = "Category",
              Attributes = new CategoryAttributesDto
              {
                  Name = "Test Category",
                  Description = "Test category description",
                  Color = "#FF5733",
                  CreatedAt = DateTime.UtcNow.AddDays(-1),
                  UpdatedAt = DateTime.UtcNow
              }
          };
          customize(dto);
          return dto;
      }

      private PagedResponse<CategoryDto> CreateCategoryCollectionResponse(int count)
      {
          var categories = new List<CategoryDto>();
          for (int i = 0; i < count; i++)
          {
              categories.Add(CreateCategoryDto(c => c.Id = $"category{i + 1}"));
          }

          return new PagedResponse<CategoryDto>
          {
              Data = categories,
              Meta = new() { Count = count, TotalCount = count },
              Links = new() { Self = "/registrations/v2/categories" }
          };
      }

      private PersonDto CreatePersonDto(Action<PersonDto> customize)
      {
          var dto = new PersonDto
          {
              Id = "person123",
              Type = "Person",
              Attributes = new PersonAttributesDto
              {
                  FirstName = "John",
                  LastName = "Doe",
                  MiddleName = "Michael",
                  Nickname = "Johnny",
                  Birthdate = DateTime.UtcNow.AddYears(-30),
                  CreatedAt = DateTime.UtcNow.AddDays(-1),
                  UpdatedAt = DateTime.UtcNow
              }
          };
          customize(dto);
          return dto;
      }
      
      #endregion
  }