using Microsoft.Extensions.DependencyInjection;
using PlanningCenter.Api.Client.IntegrationTests.Infrastructure;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.CheckIns;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PlanningCenter.Api.Client.IntegrationTests.Services;

/// <summary>
/// Base class for CheckInsService integration tests.
/// Provides common functionality and test data management.
/// </summary>
public abstract class CheckInsServiceIntegrationTestBase : IClassFixture<TestFixture>
{
    protected readonly TestFixture _fixture;
    protected readonly ICheckInsService _checkInsService;
    protected readonly Random _random = new Random();

    protected CheckInsServiceIntegrationTestBase(TestFixture fixture)
    {
        _fixture = fixture;
        _checkInsService = _fixture.ServiceProvider.GetRequiredService<ICheckInsService>();
    }

    /// <summary>
    /// Checks if the test should be skipped due to missing authentication.
    /// </summary>
    protected void SkipIfNoAuthentication()
    {
        if (!_fixture.HasRealAuthentication)
        {
            return; // Skip test
        }
    }

    /// <summary>
    /// Gets a valid event ID for testing.
    /// </summary>
    protected async Task<string> GetEventIdAsync()
    {
        var events = await _checkInsService.ListEventsAsync(new QueryParameters { PerPage = 1 });
        if (events.Data.Count == 0)
        {
            throw new InvalidOperationException("No events found. Tests cannot proceed.");
        }
        return events.Data[0].Id;
    }

    /// <summary>
    /// Creates a test check-in with random data.
    /// </summary>
    protected async Task<CheckIn> CreateTestCheckInAsync()
    {
        var randomSuffix = _random.Next(10000, 99999);
        var eventId = await GetEventIdAsync();
        
        var request = new CheckInCreateRequest
        {
            FirstName = $"Test{randomSuffix}",
            LastName = $"User{randomSuffix}",
            EventId = eventId
        };

        return await _checkInsService.CreateCheckInAsync(request);
    }

    /// <summary>
    /// Checks out a test check-in after tests.
    /// </summary>
    protected async Task CheckOutTestCheckInAsync(string checkInId)
    {
        if (!string.IsNullOrEmpty(checkInId))
        {
            try
            {
                await _checkInsService.CheckOutAsync(checkInId);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }
}
