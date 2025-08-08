using FluentAssertions;
using PlanningCenter.Api.Client.IntegrationTests.Infrastructure;
using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Requests;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PlanningCenter.Api.Client.IntegrationTests.Services;

[Collection(nameof(TestCollection))]
public class RegistrationsServiceIntegrationTests : RegistrationsServiceIntegrationTestBase
{
    public RegistrationsServiceIntegrationTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task SignupCrudLifecycle_ShouldWorkCorrectly()
    {
        SkipIfNoAuthentication();

        // Create
        var createRequest = new SignupCreateRequest
        {
            Name = $"Integration Test Signup {Guid.NewGuid():N}"[0..30],
            Description = "Test signup created during integration testing",
            OpenAt = DateTime.UtcNow.AddDays(1),
            CloseAt = DateTime.UtcNow.AddDays(29),
            RegistrationLimit = 50
        };

        var createdSignup = await _registrationsService.CreateSignupAsync(createRequest);
        createdSignup.Should().NotBeNull();
        createdSignup.Id.Should().NotBeNullOrEmpty();
        createdSignup.Name.Should().Be(createRequest.Name);
        createdSignup.Description.Should().Be(createRequest.Description);

        // Read
        var retrievedSignup = await _registrationsService.GetSignupAsync(createdSignup.Id);
        retrievedSignup.Should().NotBeNull();
        retrievedSignup!.Id.Should().Be(createdSignup.Id);
        retrievedSignup.Name.Should().Be(createdSignup.Name);
        retrievedSignup.Description.Should().Be(createdSignup.Description);

        // Update
        var updateRequest = new SignupUpdateRequest
        {
            Name = $"Updated Signup {Guid.NewGuid():N}"[0..30],
            Description = "Updated description for integration testing",
            RegistrationLimit = 75
        };

        var updatedSignup = await _registrationsService.UpdateSignupAsync(createdSignup.Id, updateRequest);
        updatedSignup.Should().NotBeNull();
        updatedSignup.Id.Should().Be(createdSignup.Id);
        updatedSignup.Name.Should().Be(updateRequest.Name);
        updatedSignup.Description.Should().Be(updateRequest.Description);
        updatedSignup.RegistrationLimit.Should().Be(updateRequest.RegistrationLimit);

        // Delete
        await _registrationsService.DeleteSignupAsync(createdSignup.Id);
        var deletedSignup = await _registrationsService.GetSignupAsync(createdSignup.Id);
        deletedSignup.Should().BeNull();
    }

    [Fact]
    public async Task RegistrationCrudLifecycle_ShouldWorkCorrectly()
    {
        SkipIfNoAuthentication();

        // First create a test signup
        var testSignup = await CreateTestSignupAsync();
        testSignup.Should().NotBeNull();

        // Create
        var createRequest = new RegistrationCreateRequest
        {
            FirstName = $"Test{Guid.NewGuid():N}"[0..10],
            LastName = $"User{Guid.NewGuid():N}"[0..10],
            Email = $"test{Guid.NewGuid():N}"[0..10] + "@example.com"
        };

        var createdRegistration = await _registrationsService.SubmitRegistrationAsync(testSignup.Id, createRequest);
        createdRegistration.Should().NotBeNull();
        createdRegistration.Id.Should().NotBeNullOrEmpty();
        createdRegistration.FirstName.Should().Be(createRequest.FirstName);
        createdRegistration.LastName.Should().Be(createRequest.LastName);
        createdRegistration.Email.Should().Be(createRequest.Email);

        // Read
        var retrievedRegistration = await _registrationsService.GetRegistrationAsync(createdRegistration.Id);
        retrievedRegistration.Should().NotBeNull();
        retrievedRegistration!.Id.Should().Be(createdRegistration.Id);
        retrievedRegistration.FirstName.Should().Be(createdRegistration.FirstName);
        retrievedRegistration.LastName.Should().Be(createdRegistration.LastName);
        retrievedRegistration.Email.Should().Be(createdRegistration.Email);

        // Note: Registration update and delete operations are not available in the current API
        // Registrations are typically managed through the signup lifecycle
    }

    [Fact]
    public async Task SelectionTypeCrudLifecycle_ShouldWorkCorrectly()
    {
        SkipIfNoAuthentication();

        // First create a test signup
        var testSignup = await CreateTestSignupAsync();
        testSignup.Should().NotBeNull();

        // Create
        var createRequest = new SelectionTypeCreateRequest
        {
            Name = $"Integration Test Selection {Guid.NewGuid():N}"[0..30],
            Description = "Test selection type created during integration testing",
            Required = false,
            AllowMultiple = true,
            MaxSelections = 3
        };

        var createdSelectionType = await _registrationsService.CreateSelectionTypeAsync(testSignup.Id, createRequest);
        createdSelectionType.Should().NotBeNull();
        createdSelectionType.Id.Should().NotBeNullOrEmpty();
        createdSelectionType.Name.Should().Be(createRequest.Name);
        createdSelectionType.Description.Should().Be(createRequest.Description);
        createdSelectionType.Required.Should().Be(createRequest.Required);

        // Read
        var retrievedSelectionType = await _registrationsService.GetSelectionTypeAsync(createdSelectionType.Id);
        retrievedSelectionType.Should().NotBeNull();
        retrievedSelectionType!.Id.Should().Be(createdSelectionType.Id);
        retrievedSelectionType.Name.Should().Be(createdSelectionType.Name);
        retrievedSelectionType.Description.Should().Be(createdSelectionType.Description);

        // Update
        var updateRequest = new SelectionTypeUpdateRequest
        {
            Name = $"Updated Selection {Guid.NewGuid():N}"[0..30],
            Description = "Updated description for integration testing",
            Required = true
        };

        var updatedSelectionType = await _registrationsService.UpdateSelectionTypeAsync(createdSelectionType.Id, updateRequest);
        updatedSelectionType.Should().NotBeNull();
        updatedSelectionType.Id.Should().Be(createdSelectionType.Id);
        updatedSelectionType.Name.Should().Be(updateRequest.Name);
        updatedSelectionType.Description.Should().Be(updateRequest.Description);
        updatedSelectionType.Required.Should().Be(updateRequest.Required.GetValueOrDefault());

        // Delete
        await _registrationsService.DeleteSelectionTypeAsync(createdSelectionType.Id);
        var deletedSelectionType = await _registrationsService.GetSelectionTypeAsync(createdSelectionType.Id);
        deletedSelectionType.Should().BeNull();
    }

    [Fact]
    public async Task ListSignupsAsync_ShouldReturnPaginatedResults()
    {
        SkipIfNoAuthentication();

        // Create a few test signups
        var signup1 = await CreateTestSignupAsync();
        var signup2 = await CreateTestSignupAsync();
        
        signup1.Should().NotBeNull();
        signup2.Should().NotBeNull();

        // Test pagination
        var firstPage = await _registrationsService.ListSignupsAsync(new QueryParameters { PerPage = 1 });
        firstPage.Should().NotBeNull();
        firstPage.Data.Should().NotBeEmpty();
        firstPage.Data.Should().HaveCountLessThanOrEqualTo(1);

        if (firstPage.HasNextPage)
        {
            var secondPage = await _registrationsService.ListSignupsAsync(new QueryParameters { PerPage = 1, Offset = 1 });
            secondPage.Should().NotBeNull();
            secondPage.Data.Should().NotBeEmpty();
            secondPage.Data.Should().HaveCountLessThanOrEqualTo(1);
        }
    }

    [Fact]
    public async Task ListRegistrationsAsync_ShouldReturnPaginatedResults()
    {
        SkipIfNoAuthentication();

        // Create a test signup and registrations
        var testSignup = await CreateTestSignupAsync();
        var registration1 = await CreateTestRegistrationAsync(testSignup.Id);
        var registration2 = await CreateTestRegistrationAsync(testSignup.Id);
        
        testSignup.Should().NotBeNull();
        registration1.Should().NotBeNull();
        registration2.Should().NotBeNull();

        // Test pagination
        var firstPage = await _registrationsService.ListRegistrationsAsync(testSignup.Id, new QueryParameters { PerPage = 1 });
        firstPage.Should().NotBeNull();
        firstPage.Data.Should().NotBeEmpty();
        firstPage.Data.Should().HaveCountLessThanOrEqualTo(1);

        if (firstPage.HasNextPage)
        {
            var secondPage = await _registrationsService.ListRegistrationsAsync(testSignup.Id, new QueryParameters { PerPage = 1, Offset = 1 });
            secondPage.Should().NotBeNull();
            secondPage.Data.Should().NotBeEmpty();
            secondPage.Data.Should().HaveCountLessThanOrEqualTo(1);
        }
    }

    [Fact]
    public async Task ListSelectionTypesAsync_ShouldReturnPaginatedResults()
    {
        SkipIfNoAuthentication();

        // Create a test signup and selection types
        var testSignup = await CreateTestSignupAsync();
        var selectionType1 = await CreateTestSelectionTypeAsync(testSignup.Id);
        var selectionType2 = await CreateTestSelectionTypeAsync(testSignup.Id);
        
        testSignup.Should().NotBeNull();
        selectionType1.Should().NotBeNull();
        selectionType2.Should().NotBeNull();

        // Test pagination
        var firstPage = await _registrationsService.ListSelectionTypesAsync(testSignup.Id, new QueryParameters { PerPage = 1 });
        firstPage.Should().NotBeNull();
        firstPage.Data.Should().NotBeEmpty();
        firstPage.Data.Should().HaveCountLessThanOrEqualTo(1);

        if (firstPage.HasNextPage)
        {
            var secondPage = await _registrationsService.ListSelectionTypesAsync(testSignup.Id, new QueryParameters { PerPage = 1, Offset = 1 });
            secondPage.Should().NotBeNull();
            secondPage.Data.Should().NotBeEmpty();
            secondPage.Data.Should().HaveCountLessThanOrEqualTo(1);
        }
    }

    [Fact]
    public async Task ListRegistrationsAsync_ShouldReturnSignupSpecificRegistrations()
    {
        SkipIfNoAuthentication();

        // Create a test signup and registration
        var testSignup = await CreateTestSignupAsync();
        var testRegistration = await CreateTestRegistrationAsync(testSignup.Id);
        
        testSignup.Should().NotBeNull();
        testRegistration.Should().NotBeNull();

        // Get registrations for the specific signup
        var signupRegistrations = await _registrationsService.ListRegistrationsAsync(testSignup.Id, new QueryParameters { PerPage = 10 });
        signupRegistrations.Should().NotBeNull();
        signupRegistrations.Data.Should().NotBeEmpty();
        signupRegistrations.Data.Should().Contain(r => r.Id == testRegistration.Id);
        // Note: Registration model doesn't contain relationship data - this is expected behavior
    }

    [Fact]
    public async Task ListSelectionTypesAsync_ShouldReturnSignupSpecificSelectionTypes()
    {
        SkipIfNoAuthentication();

        // Create a test signup and selection type
        var testSignup = await CreateTestSignupAsync();
        var testSelectionType = await CreateTestSelectionTypeAsync(testSignup.Id);
        
        testSignup.Should().NotBeNull();
        testSelectionType.Should().NotBeNull();

        // Get selection types for the specific signup
        var signupSelectionTypes = await _registrationsService.ListSelectionTypesAsync(testSignup.Id, new QueryParameters { PerPage = 10 });
        signupSelectionTypes.Should().NotBeNull();
        signupSelectionTypes.Data.Should().NotBeEmpty();
        signupSelectionTypes.Data.Should().Contain(st => st.Id == testSelectionType.Id);
        // Note: SelectionType model doesn't contain relationship data - this is expected behavior
    }
}