using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using PlanningCenter.Api.Client.Models.JsonApi;
using PlanningCenter.Api.Client.Models.JsonApi.People;
using PersonDto = PlanningCenter.Api.Client.Models.People.PersonDto;
using PhoneNumberDto = PlanningCenter.Api.Client.Models.People.PhoneNumberDto;
using AddressDto = PlanningCenter.Api.Client.Models.People.AddressDto;
using EmailDto = PlanningCenter.Api.Client.Models.People.EmailDto;
using PlanningCenter.Api.Client.Models.People;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Services;
using PlanningCenter.Api.Client.Tests.Utilities;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Services;

/// <summary>
/// Unit tests for PeopleService.
/// Follows the same comprehensive patterns as other service tests.
/// </summary>
public class PeopleServiceTests
{
    private readonly MockApiConnection _mockApiConnection = new();
    private readonly PeopleService _peopleService;
    private readonly TestDataBuilder _builder = new();
    private readonly ExtendedTestDataBuilder _extendedBuilder = new();

    public PeopleServiceTests()
    {
        _peopleService = new PeopleService(_mockApiConnection, NullLogger<PeopleService>.Instance);
    }

    #region Basic CRUD Tests

    [Fact]
    public async Task GetMeAsync_ShouldReturnCurrentUser_WhenApiReturnsData()
    {
        // Arrange
        var personDto = _builder.CreatePersonDto(p => p.Id = "42");
        var response = new JsonApiSingleResponse<PersonDto> { Data = personDto };
        _mockApiConnection.SetupGetResponse("/people/v2/me", response);

        // Act
        var result = await _peopleService.GetMeAsync();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("42");
        result.FirstName.Should().Be(personDto.Attributes.FirstName);
        result.LastName.Should().Be(personDto.Attributes.LastName);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnPerson_WhenApiReturnsData()
    {
        // Arrange
        var personDto = _builder.CreatePersonDto(p => p.Id = "123");
        var response = new JsonApiSingleResponse<PersonDto> { Data = personDto };
        _mockApiConnection.SetupGetResponse("/people/v2/people/123", response);

        // Act
        var result = await _peopleService.GetAsync("123");

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("123");
        result.FirstName.Should().Be(personDto.Attributes.FirstName);
        result.LastName.Should().Be(personDto.Attributes.LastName);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnNull_WhenApiReturnsNull()
    {
        // Arrange
        var response = new JsonApiSingleResponse<PersonDto> { Data = null };
        _mockApiConnection.SetupGetResponse("/people/v2/people/nonexistent", response);

        // Act
        var result = await _peopleService.GetAsync("nonexistent");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _peopleService.GetAsync(""));
    }

    [Fact]
    public async Task ListAsync_ShouldReturnPagedPeople_WhenApiReturnsData()
    {
        // Arrange
        var peopleResponse = _builder.BuildPersonCollectionResponse(3);
        _mockApiConnection.SetupGetResponse("/people/v2/people", peopleResponse);

        // Act
        var result = await _peopleService.ListAsync();

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(3);
        result.Meta.Should().NotBeNull();
        result.Meta.TotalCount.Should().Be(3);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnCreatedPerson_WhenRequestIsValid()
    {
        // Arrange
        var request = new PersonCreateRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Birthdate = DateTime.Parse("1990-01-01")
        };

        var personDto = _builder.CreatePersonDto(p =>
        {
            p.Id = "new123";
            p.Attributes.FirstName = "John";
            p.Attributes.LastName = "Doe";
            p.Attributes.Birthdate = DateTime.Parse("1990-01-01");
        });

        var response = new JsonApiSingleResponse<PersonDto> { Data = personDto };
        _mockApiConnection.SetupMutationResponse("POST", "/people/v2/people", response);

        // Act
        var result = await _peopleService.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("new123");
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
        result.Birthdate.Should().Be(DateTime.Parse("1990-01-01"));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _peopleService.CreateAsync(null!));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowArgumentException_WhenFirstNameIsEmpty()
    {
        // Arrange
        var request = new PersonCreateRequest
        {
            FirstName = "",
            LastName = "Doe"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _peopleService.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowArgumentException_WhenLastNameIsEmpty()
    {
        // Arrange
        var request = new PersonCreateRequest
        {
            FirstName = "John",
            LastName = ""
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _peopleService.CreateAsync(request));
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnUpdatedPerson_WhenRequestIsValid()
    {
        // Arrange
        var request = new PersonUpdateRequest
        {
            FirstName = "Jane",
            LastName = "Smith"
        };

        var personDto = _builder.CreatePersonDto(p =>
        {
            p.Id = "123";
            p.Attributes.FirstName = "Jane";
            p.Attributes.LastName = "Smith";
        });

        var response = new JsonApiSingleResponse<PersonDto> { Data = personDto };
        _mockApiConnection.SetupMutationResponse("PATCH", "/people/v2/people/123", response);

        // Act
        var result = await _peopleService.UpdateAsync("123", request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("123");
        result.FirstName.Should().Be("Jane");
        result.LastName.Should().Be("Smith");
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        // Arrange
        var request = new PersonUpdateRequest { FirstName = "Jane" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _peopleService.UpdateAsync("", request));
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _peopleService.UpdateAsync("123", null!));
    }

    [Fact]
    public async Task DeleteAsync_ShouldCompleteSuccessfully_WhenIdIsValid()
    {
        // Act & Assert (should not throw)
        await _peopleService.DeleteAsync("123");
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _peopleService.DeleteAsync(""));
    }

    #endregion

    #region Pagination Helper Tests

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllPeople_WhenMultiplePagesExist()
    {
        // Arrange
        var peopleResponse = _builder.BuildPersonCollectionResponse(3);
        _mockApiConnection.SetupGetResponse("/people/v2/people", peopleResponse);

        // Act
        var result = await _peopleService.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task StreamAsync_ShouldYieldAllPeople_WhenDataExists()
    {
        // Arrange
        var peopleResponse = _builder.BuildPersonCollectionResponse(2);
        _mockApiConnection.SetupGetResponse("/people/v2/people", peopleResponse);

        // Act
        var people = new List<Models.Core.Person>();
        await foreach (var person in _peopleService.StreamAsync())
        {
            people.Add(person);
        }

        // Assert
        people.Should().HaveCount(2);
    }

    #endregion

    #region Address Management Tests

    [Fact]
    public async Task AddAddressAsync_ShouldReturnCreatedAddress_WhenRequestIsValid()
    {
        // Arrange
        var request = new AddressCreateRequest
        {
            Street = "123 Main St",
            City = "Anytown",
            State = "CA",
            Zip = "12345"
        };

        var addressDto = CreateAddressDto(a =>
        {
            a.Id = "addr123";
            a.Attributes.Street = "123 Main St";
            a.Attributes.City = "Anytown";
            a.Attributes.State = "CA";
            a.Attributes.Zip = "12345";
        });

        var response = new JsonApiSingleResponse<AddressDto> { Data = addressDto };
        _mockApiConnection.SetupMutationResponse("POST", "/people/v2/people/person123/addresses", response);

        // Act
        var result = await _peopleService.AddAddressAsync("person123", request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("addr123");
        result.Street.Should().Be("123 Main St");
        result.City.Should().Be("Anytown");
        result.State.Should().Be("CA");
        result.Zip.Should().Be("12345");
    }

    [Fact]
    public async Task AddAddressAsync_ShouldThrowArgumentException_WhenPersonIdIsEmpty()
    {
        // Arrange
        var request = new AddressCreateRequest { Street = "123 Main St", City = "Anytown", State = "CA", Zip = "12345" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _peopleService.AddAddressAsync("", request));
    }

    [Fact]
    public async Task AddAddressAsync_ShouldThrowArgumentException_WhenStreetIsEmpty()
    {
        // Arrange
        var request = new AddressCreateRequest { Street = "", City = "Anytown", State = "CA", Zip = "12345" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _peopleService.AddAddressAsync("person123", request));
    }

    [Fact]
    public async Task UpdateAddressAsync_ShouldReturnUpdatedAddress_WhenRequestIsValid()
    {
        // Arrange
        var request = new AddressUpdateRequest
        {
            Street = "456 Oak Ave",
            City = "Newtown"
        };

        var addressDto = CreateAddressDto(a =>
        {
            a.Id = "addr123";
            a.Attributes.Street = "456 Oak Ave";
            a.Attributes.City = "Newtown";
        });

        var response = new JsonApiSingleResponse<AddressDto> { Data = addressDto };
        _mockApiConnection.SetupMutationResponse("PATCH", "/people/v2/people/person123/addresses/addr123", response);

        // Act
        var result = await _peopleService.UpdateAddressAsync("person123", "addr123", request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("addr123");
        result.Street.Should().Be("456 Oak Ave");
        result.City.Should().Be("Newtown");
    }

    [Fact]
    public async Task DeleteAddressAsync_ShouldCompleteSuccessfully_WhenIdsAreValid()
    {
        // Act & Assert (should not throw)
        await _peopleService.DeleteAddressAsync("person123", "addr123");
    }

    #endregion

    #region Email Management Tests

    [Fact]
    public async Task AddEmailAsync_ShouldReturnCreatedEmail_WhenRequestIsValid()
    {
        // Arrange
        var request = new EmailCreateRequest
        {
            Address = "test@example.com",
            Location = "Home",
            Primary = true
        };

        var emailDto = CreateEmailDto(e =>
        {
            e.Id = "email123";
            e.Attributes.Address = "test@example.com";
            e.Attributes.Location = "Home";
            e.Attributes.Primary = true;
        });

        var response = new JsonApiSingleResponse<EmailDto> { Data = emailDto };
        _mockApiConnection.SetupMutationResponse("POST", "/people/v2/people/person123/emails", response);

        // Act
        var result = await _peopleService.AddEmailAsync("person123", request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("email123");
        result.Address.Should().Be("test@example.com");
        result.Location.Should().Be("Home");
        result.Primary.Should().BeTrue();
    }

    [Fact]
    public async Task AddEmailAsync_ShouldThrowArgumentException_WhenAddressIsEmpty()
    {
        // Arrange
        var request = new EmailCreateRequest { Address = "" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _peopleService.AddEmailAsync("person123", request));
    }

    [Fact]
    public async Task UpdateEmailAsync_ShouldReturnUpdatedEmail_WhenRequestIsValid()
    {
        // Arrange
        var request = new EmailUpdateRequest
        {
            Address = "updated@example.com",
            Primary = false
        };

        var emailDto = CreateEmailDto(e =>
        {
            e.Id = "email123";
            e.Attributes.Address = "updated@example.com";
            e.Attributes.Primary = false;
        });

        var response = new JsonApiSingleResponse<EmailDto> { Data = emailDto };
        _mockApiConnection.SetupMutationResponse("PATCH", "/people/v2/people/person123/emails/email123", response);

        // Act
        var result = await _peopleService.UpdateEmailAsync("person123", "email123", request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("email123");
        result.Address.Should().Be("updated@example.com");
        result.Primary.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteEmailAsync_ShouldCompleteSuccessfully_WhenIdsAreValid()
    {
        // Act & Assert (should not throw)
        await _peopleService.DeleteEmailAsync("person123", "email123");
    }

    #endregion

    #region Phone Number Management Tests

    [Fact]
    public async Task AddPhoneNumberAsync_ShouldReturnCreatedPhoneNumber_WhenRequestIsValid()
    {
        // Arrange
        var request = new PhoneNumberCreateRequest
        {
            Number = "555-123-4567",
            Location = "Mobile",
            Primary = true
        };

        var phoneDto = _builder.CreatePhoneNumberDto(p =>
        {
            p.Id = "phone123";
            p.Attributes.Number = "555-123-4567";
            p.Attributes.Location = "Mobile";
            p.Attributes.Primary = true;
        });

        var response = new JsonApiSingleResponse<PhoneNumberDto> { Data = phoneDto };
        _mockApiConnection.SetupMutationResponse("POST", "/people/v2/people/person123/phone_numbers", response);

        // Act
        var result = await _peopleService.AddPhoneNumberAsync("person123", request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("phone123");
        result.Number.Should().Be("555-123-4567");
        result.Location.Should().Be("Mobile");
        result.Primary.Should().BeTrue();
    }

    [Fact]
    public async Task AddPhoneNumberAsync_ShouldThrowArgumentException_WhenNumberIsEmpty()
    {
        // Arrange
        var request = new PhoneNumberCreateRequest { Number = "" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _peopleService.AddPhoneNumberAsync("person123", request));
    }

    [Fact]
    public async Task UpdatePhoneNumberAsync_ShouldReturnUpdatedPhoneNumber_WhenRequestIsValid()
    {
        // Arrange
        var request = new PhoneNumberUpdateRequest
        {
            Number = "555-987-6543",
            Primary = false
        };

        var phoneDto = _builder.CreatePhoneNumberDto(p =>
        {
            p.Id = "phone123";
            p.Attributes.Number = "555-987-6543";
            p.Attributes.Primary = false;
        });

        var response = new JsonApiSingleResponse<PhoneNumberDto> { Data = phoneDto };
        _mockApiConnection.SetupMutationResponse("PATCH", "/people/v2/people/person123/phone_numbers/phone123", response);

        // Act
        var result = await _peopleService.UpdatePhoneNumberAsync("person123", "phone123", request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("phone123");
        result.Number.Should().Be("555-987-6543");
        result.Primary.Should().BeFalse();
    }

    [Fact]
    public async Task DeletePhoneNumberAsync_ShouldCompleteSuccessfully_WhenIdsAreValid()
    {
        // Act & Assert (should not throw)
        await _peopleService.DeletePhoneNumberAsync("person123", "phone123");
    }

    #endregion

    #region Household Management Tests

    [Fact]
    public async Task GetHouseholdAsync_ShouldReturnHousehold_WhenApiReturnsData()
    {
        // Arrange
        var householdDto = CreateHouseholdDto(h => h.Id = "household123");
        var response = new JsonApiSingleResponse<HouseholdDto> { Data = householdDto };
        _mockApiConnection.SetupGetResponse("/people/v2/households/household123", response);

        // Act
        var result = await _peopleService.GetHouseholdAsync("household123");

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("household123");
        result.Name.Should().Be(householdDto.Attributes.Name);
    }

    [Fact]
    public async Task ListHouseholdsAsync_ShouldReturnPagedHouseholds_WhenApiReturnsData()
    {
        // Arrange
        var householdsResponse = CreateHouseholdCollectionResponse(2);
        _mockApiConnection.SetupGetResponse("/people/v2/households", householdsResponse);

        // Act
        var result = await _peopleService.ListHouseholdsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Meta.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateHouseholdAsync_ShouldReturnCreatedHousehold_WhenRequestIsValid()
    {
        // Arrange
        var request = new HouseholdCreateRequest
        {
            Name = "Smith Family",
            PrimaryContactId = "person123",
            PersonIds = new[] { "person123", "person456" }
        };

        var householdDto = CreateHouseholdDto(h =>
        {
            h.Id = "newhousehold123";
            h.Attributes.Name = "Smith Family";
            h.Attributes.PrimaryContactId = "person123";
        });

        var response = new JsonApiSingleResponse<HouseholdDto> { Data = householdDto };
        _mockApiConnection.SetupMutationResponse("POST", "/people/v2/households", response);

        // Act
        var result = await _peopleService.CreateHouseholdAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("newhousehold123");
        result.Name.Should().Be("Smith Family");
        result.PrimaryContactId.Should().Be("person123");
    }

    [Fact]
    public async Task CreateHouseholdAsync_ShouldThrowArgumentException_WhenNameIsEmpty()
    {
        // Arrange
        var request = new HouseholdCreateRequest
        {
            Name = "",
            PrimaryContactId = "person123",
            PersonIds = new[] { "person123" }
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _peopleService.CreateHouseholdAsync(request));
    }

    [Fact]
    public async Task UpdateHouseholdAsync_ShouldReturnUpdatedHousehold_WhenRequestIsValid()
    {
        // Arrange
        var request = new HouseholdUpdateRequest
        {
            Name = "Updated Family Name"
        };

        var householdDto = CreateHouseholdDto(h =>
        {
            h.Id = "household123";
            h.Attributes.Name = "Updated Family Name";
        });

        var response = new JsonApiSingleResponse<HouseholdDto> { Data = householdDto };
        _mockApiConnection.SetupMutationResponse("PATCH", "/people/v2/households/household123", response);

        // Act
        var result = await _peopleService.UpdateHouseholdAsync("household123", request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("household123");
        result.Name.Should().Be("Updated Family Name");
    }

    [Fact]
    public async Task DeleteHouseholdAsync_ShouldCompleteSuccessfully_WhenIdIsValid()
    {
        // Act & Assert (should not throw)
        await _peopleService.DeleteHouseholdAsync("household123");
    }

    [Fact]
    public async Task ListPeopleInHouseholdAsync_ShouldReturnPagedPeople_WhenApiReturnsData()
    {
        // Arrange
        var peopleResponse = _builder.BuildPersonCollectionResponse(2);
        _mockApiConnection.SetupGetResponse("/people/v2/households/household123/people", peopleResponse);

        // Act
        var result = await _peopleService.ListPeopleInHouseholdAsync("household123");

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Meta.Should().NotBeNull();
    }

    #endregion

    #region Workflow Management Tests

    [Fact]
    public async Task GetWorkflowAsync_ShouldReturnWorkflow_WhenApiReturnsData()
    {
        // Arrange
        var workflowDto = CreateWorkflowDto(w => w.Id = "workflow123");
        var response = new JsonApiSingleResponse<WorkflowDto> { Data = workflowDto };
        _mockApiConnection.SetupGetResponse("/people/v2/workflows/workflow123", response);

        // Act
        var result = await _peopleService.GetWorkflowAsync("workflow123");

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("workflow123");
        result.Name.Should().Be(workflowDto.Attributes.Name);
    }

    [Fact]
    public async Task ListWorkflowsAsync_ShouldReturnPagedWorkflows_WhenApiReturnsData()
    {
        // Arrange
        var workflowsResponse = CreateWorkflowCollectionResponse(2);
        _mockApiConnection.SetupGetResponse("/people/v2/workflows", workflowsResponse);

        // Act
        var result = await _peopleService.ListWorkflowsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Meta.Should().NotBeNull();
    }

    [Fact]
    public async Task GetWorkflowCardAsync_ShouldReturnWorkflowCard_WhenApiReturnsData()
    {
        // Arrange
        var cardDto = CreateWorkflowCardDto(c => c.Id = "card123");
        var response = new JsonApiSingleResponse<WorkflowCardDto> { Data = cardDto };
        _mockApiConnection.SetupGetResponse("/people/v2/workflows/workflow123/cards/card123", response);

        // Act
        var result = await _peopleService.GetWorkflowCardAsync("workflow123", "card123");

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("card123");
        result.PersonId.Should().Be(cardDto.Attributes.PersonId);
    }

    [Fact]
    public async Task ListWorkflowCardsAsync_ShouldReturnPagedCards_WhenApiReturnsData()
    {
        // Arrange
        var cardsResponse = CreateWorkflowCardCollectionResponse(2);
        _mockApiConnection.SetupGetResponse("/people/v2/workflows/workflow123/cards", cardsResponse);

        // Act
        var result = await _peopleService.ListWorkflowCardsAsync("workflow123");

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Meta.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateWorkflowCardAsync_ShouldReturnCreatedCard_WhenRequestIsValid()
    {
        // Arrange
        var request = new WorkflowCardCreateRequest
        {
            PersonId = "person123",
            Note = "Test workflow card"
        };

        var cardDto = CreateWorkflowCardDto(c =>
        {
            c.Id = "newcard123";
            c.Attributes.PersonId = "person123";
            c.Attributes.Note = "Test workflow card";
        });

        var response = new JsonApiSingleResponse<WorkflowCardDto> { Data = cardDto };
        _mockApiConnection.SetupMutationResponse("POST", "/people/v2/workflows/workflow123/cards", response);

        // Act
        var result = await _peopleService.CreateWorkflowCardAsync("workflow123", request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("newcard123");
        result.PersonId.Should().Be("person123");
        result.Note.Should().Be("Test workflow card");
    }

    [Fact]
    public async Task UpdateWorkflowCardAsync_ShouldReturnUpdatedCard_WhenRequestIsValid()
    {
        // Arrange
        var request = new WorkflowCardUpdateRequest
        {
            Note = "Updated note"
        };

        var cardDto = CreateWorkflowCardDto(c =>
        {
            c.Id = "card123";
            c.Attributes.Note = "Updated note";
        });

        var response = new JsonApiSingleResponse<WorkflowCardDto> { Data = cardDto };
        _mockApiConnection.SetupMutationResponse("PATCH", "/people/v2/workflows/workflow123/cards/card123", response);

        // Act
        var result = await _peopleService.UpdateWorkflowCardAsync("workflow123", "card123", request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("card123");
        result.Note.Should().Be("Updated note");
    }

    [Fact]
    public async Task DeleteWorkflowCardAsync_ShouldCompleteSuccessfully_WhenIdsAreValid()
    {
        // Act & Assert (should not throw)
        await _peopleService.DeleteWorkflowCardAsync("workflow123", "card123");
    }

    #endregion

    #region Form Management Tests

    [Fact]
    public async Task GetFormAsync_ShouldReturnForm_WhenApiReturnsData()
    {
        // Arrange
        var formDto = CreateFormDto(f => f.Id = "form123");
        var response = new JsonApiSingleResponse<FormDto> { Data = formDto };
        _mockApiConnection.SetupGetResponse("/people/v2/forms/form123", response);

        // Act
        var result = await _peopleService.GetFormAsync("form123");

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("form123");
        result.Name.Should().Be(formDto.Attributes.Name);
    }

    [Fact]
    public async Task ListFormsAsync_ShouldReturnPagedForms_WhenApiReturnsData()
    {
        // Arrange
        var formsResponse = CreateFormCollectionResponse(2);
        _mockApiConnection.SetupGetResponse("/people/v2/forms", formsResponse);

        // Act
        var result = await _peopleService.ListFormsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Meta.Should().NotBeNull();
    }

    [Fact]
    public async Task GetFormSubmissionAsync_ShouldReturnSubmission_WhenApiReturnsData()
    {
        // Arrange
        var submissionDto = CreateFormSubmissionDto(s => s.Id = "submission123");
        var response = new JsonApiSingleResponse<FormSubmissionDto> { Data = submissionDto };
        _mockApiConnection.SetupGetResponse("/people/v2/forms/form123/form_submissions/submission123", response);

        // Act
        var result = await _peopleService.GetFormSubmissionAsync("form123", "submission123");

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("submission123");
        result.PersonId.Should().Be(submissionDto.Attributes.PersonId);
    }

    [Fact]
    public async Task ListFormSubmissionsAsync_ShouldReturnPagedSubmissions_WhenApiReturnsData()
    {
        // Arrange
        var submissionsResponse = CreateFormSubmissionCollectionResponse(2);
        _mockApiConnection.SetupGetResponse("/people/v2/forms/form123/form_submissions", submissionsResponse);

        // Act
        var result = await _peopleService.ListFormSubmissionsAsync("form123");

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Meta.Should().NotBeNull();
    }

    [Fact]
    public async Task SubmitFormAsync_ShouldReturnSubmission_WhenRequestIsValid()
    {
        // Arrange
        var request = new FormSubmitRequest
        {
            PersonId = "person123",
            FieldData = new Dictionary<string, object>
            {
                { "field1", "value1" },
                { "field2", "value2" }
            }
        };

        var submissionDto = CreateFormSubmissionDto(s =>
        {
            s.Id = "newsubmission123";
            s.Attributes.PersonId = "person123";
        });

        var response = new JsonApiSingleResponse<FormSubmissionDto> { Data = submissionDto };
        _mockApiConnection.SetupMutationResponse("POST", "/people/v2/forms/form123/form_submissions", response);

        // Act
        var result = await _peopleService.SubmitFormAsync("form123", request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("newsubmission123");
        result.PersonId.Should().Be("person123");
    }

    [Fact]
    public async Task SubmitFormAsync_ShouldThrowArgumentException_WhenFieldDataIsEmpty()
    {
        // Arrange
        var request = new FormSubmitRequest
        {
            PersonId = "person123",
            FieldData = new Dictionary<string, object>()
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _peopleService.SubmitFormAsync("form123", request));
    }

    #endregion

    #region List Management Tests

    [Fact]
    public async Task GetPeopleListAsync_ShouldReturnList_WhenApiReturnsData()
    {
        // Arrange
        var listDto = CreatePeopleListDto(l => l.Id = "list123");
        var response = new JsonApiSingleResponse<PeopleListDto> { Data = listDto };
        _mockApiConnection.SetupGetResponse("/people/v2/lists/list123", response);

        // Act
        var result = await _peopleService.GetPeopleListAsync("list123");

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("list123");
        result.Name.Should().Be(listDto.Attributes.Name);
    }

    [Fact]
    public async Task ListPeopleListsAsync_ShouldReturnPagedLists_WhenApiReturnsData()
    {
        // Arrange
        var listsResponse = CreatePeopleListCollectionResponse(2);
        _mockApiConnection.SetupGetResponse("/people/v2/lists", listsResponse);

        // Act
        var result = await _peopleService.ListPeopleListsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Meta.Should().NotBeNull();
    }

    [Fact]
    public async Task CreatePeopleListAsync_ShouldReturnCreatedList_WhenRequestIsValid()
    {
        // Arrange
        var request = new PeopleListCreateRequest
        {
            Name = "Test List",
            Description = "A test list"
        };

        var listDto = CreatePeopleListDto(l =>
        {
            l.Id = "newlist123";
            l.Attributes.Name = "Test List";
            l.Attributes.Description = "A test list";
        });

        var response = new JsonApiSingleResponse<PeopleListDto> { Data = listDto };
        _mockApiConnection.SetupMutationResponse("POST", "/people/v2/lists", response);

        // Act
        var result = await _peopleService.CreatePeopleListAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("newlist123");
        result.Name.Should().Be("Test List");
        result.Description.Should().Be("A test list");
    }

    [Fact]
    public async Task UpdatePeopleListAsync_ShouldReturnUpdatedList_WhenRequestIsValid()
    {
        // Arrange
        var request = new PeopleListUpdateRequest
        {
            Name = "Updated List Name"
        };

        var listDto = CreatePeopleListDto(l =>
        {
            l.Id = "list123";
            l.Attributes.Name = "Updated List Name";
        });

        var response = new JsonApiSingleResponse<PeopleListDto> { Data = listDto };
        _mockApiConnection.SetupMutationResponse("PATCH", "/people/v2/lists/list123", response);

        // Act
        var result = await _peopleService.UpdatePeopleListAsync("list123", request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("list123");
        result.Name.Should().Be("Updated List Name");
    }

    [Fact]
    public async Task DeletePeopleListAsync_ShouldCompleteSuccessfully_WhenIdIsValid()
    {
        // Act & Assert (should not throw)
        await _peopleService.DeletePeopleListAsync("list123");
    }

    [Fact]
    public async Task ListPeopleInListAsync_ShouldReturnPagedPeople_WhenApiReturnsData()
    {
        // Arrange
        var peopleResponse = _builder.BuildPersonCollectionResponse(2);
        _mockApiConnection.SetupGetResponse("/people/v2/lists/list123/people", peopleResponse);

        // Act
        var result = await _peopleService.ListPeopleInListAsync("list123");

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Meta.Should().NotBeNull();
    }

    [Fact]
    public async Task AddPersonToListAsync_ShouldReturnListMember_WhenRequestIsValid()
    {
        // Arrange
        var request = new ListMemberCreateRequest
        {
            PersonId = "person123"
        };

        var memberDto = CreateListMemberDto(m =>
        {
            m.Id = "member123";
            m.Attributes.PersonId = "person123";
        });

        var response = new JsonApiSingleResponse<ListMemberDto> { Data = memberDto };
        _mockApiConnection.SetupMutationResponse("POST", "/people/v2/lists/list123/people", response);

        // Act
        var result = await _peopleService.AddPersonToListAsync("list123", request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("member123");
        result.PersonId.Should().Be("person123");
    }

    [Fact]
    public async Task RemovePersonFromListAsync_ShouldCompleteSuccessfully_WhenIdsAreValid()
    {
        // Act & Assert (should not throw)
        await _peopleService.RemovePersonFromListAsync("list123", "person123");
    }

    #endregion

    // Helper methods for creating test DTOs that aren't in the builders
    private AddressDto CreateAddressDto(Action<AddressDto> customize)
    {
        var dto = new AddressDto
        {
            Id = "addr123",
            Type = "Address",
            Attributes = new AddressAttributesDto
            {
                Street = "123 Test St",
                City = "Test City",
                State = "TS",
                Zip = "12345",
                Location = "Home",
                Primary = true,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize(dto);
        return dto;
    }

    private EmailDto CreateEmailDto(Action<EmailDto> customize)
    {
        var dto = new EmailDto
        {
            Id = "email123",
            Type = "Email",
            Attributes = new EmailAttributesDto
            {
                Address = "test@example.com",
                Location = "Home",
                Primary = true,
                Blocked = false,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize(dto);
        return dto;
    }

    private HouseholdDto CreateHouseholdDto(Action<HouseholdDto> customize)
    {
        var dto = new HouseholdDto
        {
            Id = "household123",
            Type = "Household",
            Attributes = new HouseholdAttributesDto
            {
                Name = "Test Household",
                PrimaryContactId = "person123",
                MemberCount = 2,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize(dto);
        return dto;
    }

    private PagedResponse<HouseholdDto> CreateHouseholdCollectionResponse(int count)
    {
        var households = new List<HouseholdDto>();
        for (int i = 0; i < count; i++)
        {
            households.Add(CreateHouseholdDto(h => h.Id = $"household{i + 1}"));
        }

        return new PagedResponse<HouseholdDto>
        {
            Data = households,
            Meta = new PagedResponseMeta { Count = count, TotalCount = count },
            Links = new PagedResponseLinks { Self = "/people/v2/households" }
        };
    }

    private WorkflowDto CreateWorkflowDto(Action<WorkflowDto> customize)
    {
        var dto = new WorkflowDto
        {
            Id = "workflow123",
            Type = "Workflow",
            Attributes = new WorkflowAttributesDto
            {
                Name = "Test Workflow",
                MyReadyCardCount = 5,
                TotalReadyCardCount = 10,
                TotalCardsCount = 15,
                CompletedCardCount = 3,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize(dto);
        return dto;
    }

    private PagedResponse<WorkflowDto> CreateWorkflowCollectionResponse(int count)
    {
        var workflows = new List<WorkflowDto>();
        for (int i = 0; i < count; i++)
        {
            workflows.Add(CreateWorkflowDto(w => w.Id = $"workflow{i + 1}"));
        }

        return new PagedResponse<WorkflowDto>
        {
            Data = workflows,
            Meta = new PagedResponseMeta { Count = count, TotalCount = count },
            Links = new PagedResponseLinks { Self = "/people/v2/workflows" }
        };
    }

    private WorkflowCardDto CreateWorkflowCardDto(Action<WorkflowCardDto> customize)
    {
        var dto = new WorkflowCardDto
        {
            Id = "card123",
            Type = "WorkflowCard",
            Attributes = new WorkflowCardAttributesDto
            {
                PersonId = "person123",
                WorkflowId = "workflow123",
                CurrentStep = "step1",
                CompletedAt = null,
                Note = "Test note",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize(dto);
        return dto;
    }

    private PagedResponse<WorkflowCardDto> CreateWorkflowCardCollectionResponse(int count)
    {
        var cards = new List<WorkflowCardDto>();
        for (int i = 0; i < count; i++)
        {
            cards.Add(CreateWorkflowCardDto(c => c.Id = $"card{i + 1}"));
        }

        return new PagedResponse<WorkflowCardDto>
        {
            Data = cards,
            Meta = new PagedResponseMeta { Count = count, TotalCount = count },
            Links = new PagedResponseLinks { Self = "/people/v2/workflows/workflow123/cards" }
        };
    }

    private FormDto CreateFormDto(Action<FormDto> customize)
    {
        var dto = new FormDto
        {
            Id = "form123",
            Type = "Form",
            Attributes = new FormAttributesDto
            {
                Name = "Test Form",
                Description = "A test form",
                Active = true,
                Archived = false,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize(dto);
        return dto;
    }

    private PagedResponse<FormDto> CreateFormCollectionResponse(int count)
    {
        var forms = new List<FormDto>();
        for (int i = 0; i < count; i++)
        {
            forms.Add(CreateFormDto(f => f.Id = $"form{i + 1}"));
        }

        return new PagedResponse<FormDto>
        {
            Data = forms,
            Meta = new PagedResponseMeta { Count = count, TotalCount = count },
            Links = new PagedResponseLinks { Self = "/people/v2/forms" }
        };
    }

    private FormSubmissionDto CreateFormSubmissionDto(Action<FormSubmissionDto> customize)
    {
        var dto = new FormSubmissionDto
        {
            Id = "submission123",
            Type = "FormSubmission",
            Attributes = new FormSubmissionAttributesDto
            {
                PersonId = "person123",
                FormId = "form123",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize(dto);
        return dto;
    }

    private PagedResponse<FormSubmissionDto> CreateFormSubmissionCollectionResponse(int count)
    {
        var submissions = new List<FormSubmissionDto>();
        for (int i = 0; i < count; i++)
        {
            submissions.Add(CreateFormSubmissionDto(s => s.Id = $"submission{i + 1}"));
        }

        return new PagedResponse<FormSubmissionDto>
        {
            Data = submissions,
            Meta = new PagedResponseMeta { Count = count, TotalCount = count },
            Links = new PagedResponseLinks { Self = "/people/v2/forms/form123/form_submissions" }
        };
    }

    private PeopleListDto CreatePeopleListDto(Action<PeopleListDto> customize)
    {
        var dto = new PeopleListDto
        {
            Id = "list123",
            Type = "List",
            Attributes = new PeopleListAttributesDto
            {
                Name = "Test List",
                Description = "A test list",
                AutoRefresh = false,
                Status = "active",
                HasInactiveResults = false,
                IncludeInactive = false,
                Returns = "Person",
                TotalPeople = 10,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize(dto);
        return dto;
    }

    private PagedResponse<PeopleListDto> CreatePeopleListCollectionResponse(int count)
    {
        var lists = new List<PeopleListDto>();
        for (int i = 0; i < count; i++)
        {
            lists.Add(CreatePeopleListDto(l => l.Id = $"list{i + 1}"));
        }

        return new PagedResponse<PeopleListDto>
        {
            Data = lists,
            Meta = new PagedResponseMeta { Count = count, TotalCount = count },
            Links = new PagedResponseLinks { Self = "/people/v2/lists" }
        };
    }

    private ListMemberDto CreateListMemberDto(Action<ListMemberDto> customize)
    {
        var dto = new ListMemberDto
        {
            Id = "member123",
            Type = "ListMember",
            Attributes = new ListMemberAttributesDto
            {
                PersonId = "person123",
                ListId = "list123",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow
            }
        };
        customize(dto);
        return dto;
    }
}
