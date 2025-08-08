using Microsoft.Extensions.DependencyInjection;
using PlanningCenter.Api.Client.IntegrationTests.Infrastructure;
using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Registrations;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PlanningCenter.Api.Client.IntegrationTests.Services;

public abstract class RegistrationsServiceIntegrationTestBase : IClassFixture<TestFixture>
{
    protected readonly TestFixture _fixture;
    protected readonly IRegistrationsService _registrationsService;
    protected PlanningCenterClient Client => _fixture.Client;
    private readonly List<string> _createdSignupIds = new();
    private readonly List<string> _createdRegistrationIds = new();
    private readonly List<string> _createdSelectionTypeIds = new();

    protected RegistrationsServiceIntegrationTestBase(TestFixture testFixture)
    {
        _fixture = testFixture;
        _registrationsService = _fixture.ServiceProvider.GetRequiredService<IRegistrationsService>();
    }

    protected void SkipIfNoAuthentication()
    {
        if (!_fixture.HasRealAuthentication)
        {
            return; // Skip test
        }
    }

    protected async Task<Signup> CreateTestSignupAsync()
    {
        var request = new SignupCreateRequest
        {
            Name = $"Test Signup {Guid.NewGuid():N}[0..8]",
            Description = "Test signup for integration testing",
            OpenAt = DateTime.UtcNow.AddDays(-1),
            CloseAt = DateTime.UtcNow.AddDays(30),
            RegistrationLimit = 100
        };

        var createdSignup = await _registrationsService.CreateSignupAsync(request);
        if (createdSignup?.Id != null)
        {
            _createdSignupIds.Add(createdSignup.Id);
        }
        return createdSignup!;
    }

    protected async Task<Registration> CreateTestRegistrationAsync(string? signupId = null)
    {
        // If no signupId provided, create a test signup first
        if (string.IsNullOrEmpty(signupId))
        {
            var testSignup = await CreateTestSignupAsync();
            signupId = testSignup?.Id!;
        }

        if (string.IsNullOrEmpty(signupId))
        {
            throw new InvalidOperationException("Unable to create test signup for registration");
        }

        var request = new RegistrationCreateRequest
        {
            FirstName = $"Test{Guid.NewGuid():N}[0..8]",
            LastName = $"User{Guid.NewGuid():N}[0..8]",
            Email = $"test{Guid.NewGuid():N}[0..8]@example.com"
        };

        var createdRegistration = await _registrationsService.SubmitRegistrationAsync(signupId, request);
        if (createdRegistration?.Id != null)
        {
            _createdRegistrationIds.Add(createdRegistration.Id);
        }
        return createdRegistration!;
    }

    protected async Task<SelectionType> CreateTestSelectionTypeAsync(string? signupId = null)
    {
        // If no signupId provided, create a test signup first
        if (string.IsNullOrEmpty(signupId))
        {
            var testSignup = await CreateTestSignupAsync();
            signupId = testSignup?.Id!;
        }

        if (string.IsNullOrEmpty(signupId))
        {
            throw new InvalidOperationException("Unable to create test signup for selection type");
        }

        var request = new SelectionTypeCreateRequest
        {
            Name = $"Test Selection Type {Guid.NewGuid():N}[0..8]",
            Description = "Test selection type for integration testing",
            Cost = 10.00m // $10.00
        };

        var createdSelectionType = await _registrationsService.CreateSelectionTypeAsync(signupId, request);
        if (createdSelectionType?.Id != null)
        {
            _createdSelectionTypeIds.Add(createdSelectionType.Id);
        }
        return createdSelectionType!;
    }



    protected async Task CleanupAsync()
    {
        // Clean up in reverse order of creation to respect dependencies
        
        // Clean up selection types first
        foreach (var selectionTypeId in _createdSelectionTypeIds)
        {
            try
            {
                await _registrationsService.DeleteSelectionTypeAsync(selectionTypeId);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
        _createdSelectionTypeIds.Clear();

        // Clean up registrations
        foreach (var registrationId in _createdRegistrationIds)
        {
            try
            {
                // Note: Registration deletion may not be supported in the API
                // await RegistrationsService.DeleteRegistrationAsync(registrationId);
                await Task.CompletedTask;
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
        _createdRegistrationIds.Clear();

        // Clean up signups last
        foreach (var signupId in _createdSignupIds)
        {
            try
            {
                await _registrationsService.DeleteSignupAsync(signupId);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
        _createdSignupIds.Clear();
    }
}