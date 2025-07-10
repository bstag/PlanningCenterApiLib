using Microsoft.Extensions.DependencyInjection;
using PlanningCenter.Api.Client.IntegrationTests.Infrastructure;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.People;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PlanningCenter.Api.Client.IntegrationTests.Services;

/// <summary>
/// Base class for PeopleService integration tests.
/// Provides common functionality and test data management.
/// </summary>
public abstract class PeopleServiceIntegrationTestBase : IClassFixture<TestFixture>
{
    protected readonly TestFixture _fixture;
    protected readonly IPeopleService _peopleService;
    protected readonly Random _random = new Random();

    protected PeopleServiceIntegrationTestBase(TestFixture fixture)
    {
        _fixture = fixture;
        _peopleService = _fixture.ServiceProvider.GetRequiredService<IPeopleService>();
    }

    /// <summary>
    /// Creates a test person with random data.
    /// </summary>
    protected async Task<Person> CreateTestPersonAsync()
    {
        var randomSuffix = _random.Next(10000, 99999);
        var request = new PersonCreateRequest
        {
            FirstName = $"Test{randomSuffix}",
            LastName = $"Integration{randomSuffix}",
            Email = $"test.integration.{randomSuffix}@example.com",
            Status = "inactive" // Use inactive status to avoid cluttering the real database
        };

        return await _peopleService.CreateAsync(request);
    }

    /// <summary>
    /// Cleans up a test person after tests.
    /// </summary>
    protected async Task CleanupTestPersonAsync(string personId)
    {
        if (!string.IsNullOrEmpty(personId))
        {
            try
            {
                await _peopleService.DeleteAsync(personId);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }
}
