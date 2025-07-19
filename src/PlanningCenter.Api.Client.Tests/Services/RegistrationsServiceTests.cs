using System.Dynamic;
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

    #region SelectionType Management Tests

    [Fact]
    public async Task GetSelectionTypeAsync_ShouldReturnSelectionType_WhenApiReturnsData()
    {
        // Arrange
        var response = new JsonApiSingleResponse<dynamic>
        {
            Data = new
            {
                id = "selection123",
                attributes = new
                {
                    name = "Early Bird Pricing",
                    description = "Discounted pricing for early registration",
                    cost = 25.00m,
                    required = true
                },
                relationships = new
                {
                    signup = new { data = new { id = "signup123" } }
                }
            }
        };
        _mockApiConnection.SetupGetResponse("/registrations/v2/selection_types/selection123", response);

        // Act
        var result = await _registrationsService.GetSelectionTypeAsync("selection123");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("selection123");
        result!.Name.Should().Be("Early Bird Pricing");
        result!.DataSource.Should().Be("Registrations");
    }

    [Fact]
    public async Task CreateSelectionTypeAsync_ShouldReturnCreatedSelectionType_WhenRequestIsValid()
    {
        // Arrange
        var request = new SelectionTypeCreateRequest
        {
            Name = "T-Shirt Size",
            Description = "Select your t-shirt size",
            Required = true,
            AllowMultiple = false
        };

        dynamic data = new ExpandoObject();
        data.id = "newselection123";

        var response = new JsonApiSingleResponse<dynamic>
        {
            Data = data
        };
        _mockApiConnection.SetupMutationResponse("POST", "/registrations/v2/signups/signup123/selection_types", response);

        // Act
        var result = await _registrationsService.CreateSelectionTypeAsync("signup123", request);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("newselection123");
        result!.Name.Should().Be("T-Shirt Size");
        result!.Required.Should().BeTrue();
    }

    [Fact]
    public async Task SetSignupLocationAsync_ShouldReturnCreatedLocation_WhenRequestIsValid()
    {
        // Arrange
        var request = new SignupLocationCreateRequest
        {
            Name = "New Venue",
            StreetAddress = "456 Oak Ave",
            City = "Springfield",
            State = "IL"
        };

        dynamic data = new ExpandoObject();
        data.id = "newlocation123";

        var response = new JsonApiSingleResponse<dynamic>
        {
            Data = data
        };
        _mockApiConnection.SetupMutationResponse("POST", "/registrations/v2/signups/signup123/location", response);

        // Act
        var result = await _registrationsService.SetSignupLocationAsync("signup123", request);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("newlocation123");
        result!.Name.Should().Be("New Venue");
        result!.City.Should().Be("Springfield");
    }

    [Fact]
    public async Task AddSignupTimeAsync_ShouldReturnCreatedTime_WhenRequestIsValid()
    {
        // Arrange
        var request = new SignupTimeCreateRequest
        {
            Name = "Evening Session",
            StartTime = DateTime.Parse("2024-06-01T18:00:00Z"),
            EndTime = DateTime.Parse("2024-06-01T21:00:00Z"),
            Capacity = 20
        };

        dynamic data = new ExpandoObject();
        data.id = "newtime123";

        var response = new JsonApiSingleResponse<dynamic>
        {
            Data = data
        };
        _mockApiConnection.SetupMutationResponse("POST", "/registrations/v2/signups/signup123/signup_times", response);

        // Act
        var result = await _registrationsService.AddSignupTimeAsync("signup123", request);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("newtime123");
        result!.Name.Should().Be("Evening Session");
        result!.Capacity.Should().Be(20);
    }

    [Fact]
    public async Task UpdateSignupTimeAsync_ShouldReturnUpdatedTime_WhenRequestIsValid()
    {
        // Arrange
        var request = new SignupTimeUpdateRequest
        {
            Name = "Updated Session",
            Capacity = 35
        };

        dynamic data = new ExpandoObject();
        data.id = "time123";

        var response = new JsonApiSingleResponse<dynamic>
        {
            Data = data
        };
        _mockApiConnection.SetupMutationResponse("PATCH", "/registrations/v2/signup_times/time123", response);

        // Act
        var result = await _registrationsService.UpdateSignupTimeAsync("time123", request);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("time123");
        result!.Name.Should().Be("Updated Session");
        result!.Capacity.Should().Be(35);
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
    public async Task GetEmergencyContactAsync_ShouldReturnContact_WhenApiReturnsData()
    {
        // Arrange
        var response = new JsonApiSingleResponse<dynamic>
        {
            Data = new
            {
                id = "contact123",
                attributes = new
                {
                    first_name = "Jane",
                    last_name = "Doe",
                    relationship = "Parent",
                    primary_phone = "555-123-4567",
                    email = "jane.doe@example.com"
                }
            }
        };
        _mockApiConnection.SetupGetResponse("/registrations/v2/attendees/attendee123/emergency_contact", response);

        // Act
        var result = await _registrationsService.GetEmergencyContactAsync("attendee123");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("contact123");
        result!.FirstName.Should().Be("Jane");
        result!.LastName.Should().Be("Doe");
        result!.Relationship.Should().Be("Parent");
    }

    [Fact]
    public async Task SetEmergencyContactAsync_ShouldReturnCreatedContact_WhenRequestIsValid()
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

        dynamic data = new ExpandoObject();
        data.id = "newcontact123";

        var response = new JsonApiSingleResponse<dynamic>
        {
            Data = data
        };
        _mockApiConnection.SetupMutationResponse("POST", "/registrations/v2/attendees/attendee123/emergency_contact", response);

        // Act
        var result = await _registrationsService.SetEmergencyContactAsync("attendee123", request);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("newcontact123");
        result!.FirstName.Should().Be("John");
        result!.LastName.Should().Be("Smith");
        result!.Relationship.Should().Be("Guardian");
    }

    [Fact]
    public async Task UpdateEmergencyContactAsync_ShouldReturnUpdatedContact_WhenRequestIsValid()
    {
        // Arrange
        var request = new EmergencyContactUpdateRequest
        {
            FirstName = "Updated",
            LastName = "Contact",
            PrimaryPhone = "555-111-2222"
        };

        dynamic data = new ExpandoObject();
        data.id = "contact123";

        var response = new JsonApiSingleResponse<dynamic>
        {
            Data = data
        };
        _mockApiConnection.SetupMutationResponse("PATCH", "/registrations/v2/attendees/attendee123/emergency_contact", response);

        // Act
        var result = await _registrationsService.UpdateEmergencyContactAsync("attendee123", request);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("contact123");
        result!.FirstName.Should().Be("Updated");
        result!.LastName.Should().Be("Contact");
    }

    #endregion

    #region Category Management Tests

    [Fact]
    public async Task GetCategoryAsync_ShouldReturnCategory_WhenApiReturnsData()
    {
        // Arrange
        var response = new JsonApiSingleResponse<dynamic>
        {
            Data = new
            {
                id = "category123",
                attributes = new
                {
                    name = "Youth Events",
                    description = "Events for young people",
                    color = "#FF5733",
                    active = true,
                    sort_order = 1
                }
            }
        };
        _mockApiConnection.SetupGetResponse("/registrations/v2/categories/category123", response);

        // Act
        var result = await _registrationsService.GetCategoryAsync("category123");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("category123");
        result!.Name.Should().Be("Youth Events");
        result!.Color.Should().Be("#FF5733");
    }

    [Fact]
    public async Task ListCategoriesAsync_ShouldReturnPagedCategories_WhenApiReturnsData()
    {
        // Arrange
        var response = new PagedResponse<dynamic>
        {
            Data = new List<dynamic>
            {
                new
                {
                    id = "cat1",
                    attributes = new
                    {
                        name = "Youth Events",
                        description = "Events for young people",
                        active = true
                    }
                },
                new
                {
                    id = "cat2",
                    attributes = new
                    {
                        name = "Adult Events",
                        description = "Events for adults",
                        active = true
                    }
                }
            },
            Meta = new() { Count = 2, TotalCount = 2 },
            Links = new() { Self = "/registrations/v2/categories" }
        };
        _mockApiConnection.SetupGetResponse("/registrations/v2/categories", response);

        // Act
        var result = await _registrationsService.ListCategoriesAsync();

        // Assert
        result.Should().NotBeNull();
        result!.Data.Should().HaveCount(2);
        result!.Data.First().Name.Should().Be("Youth Events");
    }

    [Fact]
    public async Task CreateCategoryAsync_ShouldReturnCreatedCategory_WhenRequestIsValid()
    {
        // Arrange
        var request = new CategoryCreateRequest
        {
            Name = "Special Events",
            Description = "One-time special events",
            Color = "#00FF00",
            Active = true
        };

        dynamic data = new ExpandoObject();
        data.id = "newcategory123";

        var response = new JsonApiSingleResponse<dynamic>
        {
            Data = data
        };
        _mockApiConnection.SetupMutationResponse("POST", "/registrations/v2/categories", response);

        // Act
        var result = await _registrationsService.CreateCategoryAsync(request);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("newcategory123");
        result!.Name.Should().Be("Special Events");
        result!.Color.Should().Be("#00FF00");
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
    public async Task GetCampusAsync_ShouldReturnCampus_WhenApiReturnsData()
    {
        // Arrange
        var response = new JsonApiSingleResponse<dynamic>
        {
            Data = new ExpandoObject()
        };
        var campusData = (dynamic)response.Data;
        campusData.id = "campus123";
        campusData.attributes = new ExpandoObject();
        ((dynamic)campusData.attributes).name = "Main Campus";
        ((dynamic)campusData.attributes).description = "Our main campus location";
        ((dynamic)campusData.attributes).city = "Springfield";
        ((dynamic)campusData.attributes).state = "IL";
        ((dynamic)campusData.attributes).active = true;
        _mockApiConnection.SetupGetResponse("/registrations/v2/campuses/campus123", response);

        // Act
        var result = await _registrationsService.GetCampusAsync("campus123");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("campus123");
        result!.Name.Should().Be("Main Campus");
        result!.City.Should().Be("Springfield");
    }

    [Fact]
    public async Task ListCampusesAsync_ShouldReturnPagedCampuses_WhenApiReturnsData()
    {
        // Arrange
        dynamic campus1 = new ExpandoObject();
        campus1.id = "campus1";
        campus1.attributes = new ExpandoObject();
        ((dynamic)campus1.attributes).name = "Main Campus";
        ((dynamic)campus1.attributes).city = "Springfield";
        ((dynamic)campus1.attributes).active = true;

        dynamic campus2 = new ExpandoObject();
        campus2.id = "campus2";
        campus2.attributes = new ExpandoObject();
        ((dynamic)campus2.attributes).name = "North Campus";
        ((dynamic)campus2.attributes).city = "Northfield";
        ((dynamic)campus2.attributes).active = true;

        var response = new PagedResponse<dynamic>
        {
            Data = new List<dynamic> { campus1, campus2 },
            Meta = new() { Count = 2, TotalCount = 2 },
            Links = new() { Self = "/registrations/v2/campuses" }
        };
        _mockApiConnection.SetupGetResponse("/registrations/v2/campuses", response);

        // Act
        var result = await _registrationsService.ListCampusesAsync();

        // Assert
        result.Should().NotBeNull();
        result!.Data.Should().HaveCount(2);
        result!.Data.First().Name.Should().Be("Main Campus");
    }

    #endregion

    #region Person Management Tests

    [Fact]
    public async Task GetPersonAsync_ShouldReturnPerson_WhenApiReturnsData()
    {
        // Arrange
        var response = new JsonApiSingleResponse<dynamic>
        {
            Data = new ExpandoObject()
        };
        var personData = (dynamic)response.Data;
        personData.id = "person123";
        personData.attributes = new ExpandoObject();
        ((dynamic)personData.attributes).first_name = "John";
        ((dynamic)personData.attributes).last_name = "Doe";
        ((dynamic)personData.attributes).email = "john.doe@example.com";
        ((dynamic)personData.attributes).phone_number = "555-123-4567";
        ((dynamic)personData.attributes).created_at = DateTime.Parse("2024-01-01T00:00:00Z");
        ((dynamic)personData.attributes).updated_at = DateTime.Parse("2024-01-15T00:00:00Z");
        _mockApiConnection.SetupGetResponse("/registrations/v2/people/person123", response);

        // Act
        var result = await _registrationsService.GetPersonAsync("person123");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("person123");
        result!.FirstName.Should().Be("John");
        result!.LastName.Should().Be("Doe");
        result!.PrimaryEmail.Should().Be("john.doe@example.com");
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
        _mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123/registrations?filter[status]=confirmed", registrationResponse);

        // Setup waitlist count response
        var waitlistResponse = CreateAttendeeCollectionResponse(5);
        _mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123/attendees?filter[on_waitlist]=true", waitlistResponse);

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
        _mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123/registrations?filter[status]=confirmed", registrationResponse);

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
        _mockApiConnection.SetupGetResponse("/registrations/v2/signups/signup123/attendees?filter[on_waitlist]=true", waitlistResponse);

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
            Links = new() { Self = "/registrations/v2/signups", Next = "/registrations/v2/signups?offset=1" }
        };

        var page2Response = new PagedResponse<SignupDto>
        {
            Data = new List<SignupDto> { CreateSignupDto(s => s.Id = "signup2") },
            Meta = new() { Count = 1, TotalCount = 2 },
            Links = new() { Self = "/registrations/v2/signups?offset=1" }
        };

        _mockApiConnection.SetupGetResponse("/registrations/v2/signups?per_page=100", page1Response);
        _mockApiConnection.SetupGetResponse("/registrations/v2/signups?per_page=100&offset=100", page2Response);

        // Act
        var result = await _registrationsService.GetAllSignupsAsync();

        // Assert
        result.Should().NotBeNull();
        result!.Should().HaveCount(2);
        result![0].Id.Should().Be("signup1");
        result![1].Id.Should().Be("signup2");
    }

    [Fact]
    public async Task StreamSignupsAsync_ShouldYieldAllSignups_WhenMultiplePagesExist()
    {
        // Arrange
        var page1Response = new PagedResponse<SignupDto>
        {
            Data = new List<SignupDto> { CreateSignupDto(s => s.Id = "signup1") },
            Meta = new() { Count = 1, TotalCount = 2 },
            Links = new() { Self = "/registrations/v2/signups", Next = "/registrations/v2/signups?offset=1" }
        };

        var page2Response = new PagedResponse<SignupDto>
        {
            Data = new List<SignupDto> { CreateSignupDto(s => s.Id = "signup2") },
            Meta = new() { Count = 1, TotalCount = 2 },
            Links = new() { Self = "/registrations/v2/signups?offset=1" }
        };

        _mockApiConnection.SetupGetResponse("/registrations/v2/signups?per_page=100", page1Response);
        _mockApiConnection.SetupGetResponse("/registrations/v2/signups?per_page=100&offset=100", page2Response);

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