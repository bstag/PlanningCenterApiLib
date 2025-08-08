using Microsoft.Extensions.DependencyInjection;
using PlanningCenter.Api.Client.IntegrationTests.Infrastructure;
using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Registrations;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Services;
using Xunit;

namespace PlanningCenter.Api.Client.IntegrationTests;

public abstract class RegistrationsServiceIntegrationTestBase : IClassFixture<TestFixture>
{
    protected readonly TestFixture _fixture;
    protected readonly IRegistrationsService _registrationsService;
    protected PlanningCenterClient Client => _fixture.Client;

    protected RegistrationsServiceIntegrationTestBase(TestFixture testFixture)
    {
        _fixture = testFixture;
        _registrationsService = _fixture.ServiceProvider.GetRequiredService<IRegistrationsService>();
    }

    protected async Task<Signup> CreateTestSignupAsync(string name = "Test Signup")
    {
        var signupRequest = new SignupCreateRequest
        {
            Name = name,
            Description = "Test signup for integration testing",
            OpenAt = DateTime.UtcNow.AddDays(-1),
            CloseAt = DateTime.UtcNow.AddDays(30),
            RegistrationLimit = 100
        };

        return await _registrationsService.CreateSignupAsync(signupRequest);
    }

    protected async Task<Registration> CreateTestRegistrationAsync(string signupId, string email = "test@example.com")
    {
        var registrationRequest = new RegistrationCreateRequest
        {
            Email = email,
            FirstName = "Test",
            LastName = "User"
        };

        return await _registrationsService.SubmitRegistrationAsync(signupId, registrationRequest);
    }

    protected async Task<SelectionType> CreateTestSelectionTypeAsync(string signupId, string name = "Test Selection Type")
    {
        var selectionTypeRequest = new SelectionTypeCreateRequest
        {
            Name = name,
            Description = "Test selection type for integration testing",
            Cost = 10.00m // $10.00
        };

        return await _registrationsService.CreateSelectionTypeAsync(signupId, selectionTypeRequest);
    }

    protected async Task CleanupTestSignupAsync(string signupId)
    {
        try
        {
            await _registrationsService.DeleteSignupAsync(signupId);
        }
        catch
        {
            // Ignore cleanup errors in tests
        }
    }

    protected async Task CleanupTestRegistrationAsync(string registrationId)
    {
        try
        {
            // Note: Registration deletion may not be supported in the API
            // await _registrationsService.DeleteRegistrationAsync(registrationId);
            await Task.CompletedTask;
        }
        catch
        {
            // Ignore cleanup errors in tests
        }
    }

    protected async Task CleanupTestSelectionTypeAsync(string selectionTypeId)
    {
        try
        {
            await _registrationsService.DeleteSelectionTypeAsync(selectionTypeId);
        }
        catch
        {
            // Ignore cleanup errors in tests
        }
    }
}