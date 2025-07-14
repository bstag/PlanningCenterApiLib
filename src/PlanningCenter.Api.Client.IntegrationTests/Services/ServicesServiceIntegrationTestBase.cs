using Microsoft.Extensions.DependencyInjection;
using PlanningCenter.Api.Client.IntegrationTests.Infrastructure;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Services;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PlanningCenter.Api.Client.IntegrationTests.Services;

/// <summary>
/// Base class for ServicesService integration tests.
/// Provides common functionality and test data management.
/// </summary>
public abstract class ServicesServiceIntegrationTestBase : IClassFixture<TestFixture>
{
    protected readonly TestFixture _fixture;
    protected readonly IServicesService _servicesService;
    protected readonly Random _random = new Random();

    protected ServicesServiceIntegrationTestBase(TestFixture fixture)
    {
        _fixture = fixture;
        _servicesService = _fixture.ServiceProvider.GetRequiredService<IServicesService>();
    }

    /// <summary>
    /// Gets a valid service type ID for testing.
    /// </summary>
    protected async Task<string> GetServiceTypeIdAsync()
    {
        var serviceTypes = await _servicesService.ListServiceTypesAsync(new QueryParameters { PerPage = 1 });
        if (serviceTypes.Data.Count == 0)
        {
            throw new InvalidOperationException("No service types found. Tests cannot proceed.");
        }
        return serviceTypes.Data[0].Id;
    }

    /// <summary>
    /// Creates a test plan with random data.
    /// </summary>
    protected async Task<Plan> CreateTestPlanAsync()
    {
        var randomSuffix = _random.Next(10000, 99999);
        var serviceTypeId = await GetServiceTypeIdAsync();
        
        var request = new PlanCreateRequest
        {
            Title = $"Test Plan {randomSuffix}",
            ServiceTypeId = serviceTypeId,
            SortDate = DateTime.Now.AddDays(7).Date
        };

        return await _servicesService.CreatePlanAsync(request);
    }

    /// <summary>
    /// Cleans up a test plan after tests.
    /// </summary>
    protected async Task CleanupTestPlanAsync(string planId)
    {
        if (!string.IsNullOrEmpty(planId))
        {
            try
            {
                await _servicesService.DeletePlanAsync(planId);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }

    /// <summary>
    /// Creates a test item in a plan with random data.
    /// </summary>
    protected async Task<Item> CreateTestPlanItemAsync(string planId)
    {
        var randomSuffix = _random.Next(10000, 99999);
        var request = new ItemCreateRequest
        {
            Title = $"Test Item {randomSuffix}",
            Description = $"Test Item Description {randomSuffix}",
            Length = 300 // 5 minutes in seconds
        };

        return await _servicesService.CreatePlanItemAsync(planId, request);
    }

    /// <summary>
    /// Creates a test song with random data.
    /// </summary>
    protected async Task<Song> CreateTestSongAsync()
    {
        var randomSuffix = _random.Next(10000, 99999);
        var request = new SongCreateRequest
        {
            Title = $"Test Song {randomSuffix}",
            Author = $"Test Author {randomSuffix}"
        };

        return await _servicesService.CreateSongAsync(request);
    }

    /// <summary>
    /// Cleans up a test song after tests.
    /// </summary>
    protected async Task CleanupTestSongAsync(string songId)
    {
        if (!string.IsNullOrEmpty(songId))
        {
            try
            {
                await _servicesService.DeleteSongAsync(songId);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }
}
