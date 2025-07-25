using Microsoft.Extensions.DependencyInjection;
using PlanningCenter.Api.Client.IntegrationTests.Infrastructure;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Calendar;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PlanningCenter.Api.Client.IntegrationTests.Services;

/// <summary>
/// Base class for CalendarService integration tests.
/// Provides common functionality and test data management.
/// </summary>
public abstract class CalendarServiceIntegrationTestBase : IClassFixture<TestFixture>
{
    protected readonly TestFixture _fixture;
    protected readonly ICalendarService _calendarService;
    protected readonly Random _random = new Random();

    protected CalendarServiceIntegrationTestBase(TestFixture fixture)
    {
        _fixture = fixture;
        _calendarService = _fixture.ServiceProvider.GetRequiredService<ICalendarService>();
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
    /// Creates a test event with random data.
    /// </summary>
    protected async Task<Event> CreateTestEventAsync()
    {
        var randomSuffix = _random.Next(10000, 99999);
        var request = new EventCreateRequest
        {
            Name = $"Test Event {randomSuffix}",
            Description = $"Test Description {randomSuffix}",
            StartsAt = DateTime.Now.AddDays(1),
            EndsAt = DateTime.Now.AddDays(1).AddHours(2)
        };

        return await _calendarService.CreateEventAsync(request);
    }

    /// <summary>
    /// Cleans up a test event after tests.
    /// </summary>
    protected async Task CleanupTestEventAsync(string eventId)
    {
        if (!string.IsNullOrEmpty(eventId))
        {
            try
            {
                await _calendarService.DeleteEventAsync(eventId);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }

    /// <summary>
    /// Creates a test resource with random data.
    /// </summary>
    protected async Task<Resource> CreateTestResourceAsync()
    {
        var randomSuffix = _random.Next(10000, 99999);
        var request = new ResourceCreateRequest
        {
            Name = $"Test Resource {randomSuffix}",
            Description = $"Test Resource Description {randomSuffix}"
        };

        return await _calendarService.CreateResourceAsync(request);
    }

    /// <summary>
    /// Cleans up a test resource after tests.
    /// </summary>
    protected async Task CleanupTestResourceAsync(string resourceId)
    {
        if (!string.IsNullOrEmpty(resourceId))
        {
            try
            {
                await _calendarService.DeleteResourceAsync(resourceId);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }
}
