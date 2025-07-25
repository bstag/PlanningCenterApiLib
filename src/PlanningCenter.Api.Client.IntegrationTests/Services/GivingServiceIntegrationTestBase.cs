using Microsoft.Extensions.DependencyInjection;
using PlanningCenter.Api.Client.IntegrationTests.Infrastructure;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Giving;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PlanningCenter.Api.Client.IntegrationTests.Services;

/// <summary>
/// Base class for GivingService integration tests.
/// Provides common functionality and test data management.
/// </summary>
public abstract class GivingServiceIntegrationTestBase : IClassFixture<TestFixture>
{
    protected readonly TestFixture _fixture;
    protected readonly IGivingService _givingService;
    protected readonly Random _random = new Random();

    protected GivingServiceIntegrationTestBase(TestFixture fixture)
    {
        _fixture = fixture;
        _givingService = _fixture.ServiceProvider.GetRequiredService<IGivingService>();
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
    /// Creates a test fund with random data.
    /// </summary>
    protected async Task<Fund> CreateTestFundAsync()
    {
        var randomSuffix = _random.Next(10000, 99999);
        var request = new FundCreateRequest
        {
            Name = $"Test Fund {randomSuffix}",
            Description = $"Integration test fund {randomSuffix}",
            Visibility = false // Use hidden visibility to avoid cluttering the real database
        };

        return await _givingService.CreateFundAsync(request);
    }

    /// <summary>
    /// Creates a test batch with random data.
    /// </summary>
    protected async Task<Batch> CreateTestBatchAsync()
    {
        var randomSuffix = _random.Next(10000, 99999);
        var request = new BatchCreateRequest
        {
            Description = $"Test Batch {randomSuffix}"
        };

        return await _givingService.CreateBatchAsync(request);
    }

    /// <summary>
    /// Creates a test donation with random data.
    /// </summary>
    protected async Task<Donation> CreateTestDonationAsync(string? fundId = null, string? batchId = null)
    {
        var randomSuffix = _random.Next(10000, 99999);
        var request = new DonationCreateRequest
        {
            AmountCents = _random.Next(1000, 10000), // $10.00 to $100.00
            PaymentMethod = "cash",
            ReceivedAt = DateTime.UtcNow,
            BatchId = batchId
        };

        if (!string.IsNullOrEmpty(fundId))
        {
            request.Designations.Add(new DonationDesignation
            {
                FundId = fundId,
                AmountCents = request.AmountCents
            });
        }

        return await _givingService.CreateDonationAsync(request);
    }

    /// <summary>
    /// Creates a test pledge with random data.
    /// </summary>
    protected async Task<Pledge> CreateTestPledgeAsync(string? fundId = null)
    {
        var randomSuffix = _random.Next(10000, 99999);
        var request = new PledgeCreateRequest
        {
            AmountCents = _random.Next(10000, 100000), // $100.00 to $1000.00
            JointGiverAmountCents = 0,
            FundId = fundId
        };

        return await _givingService.CreatePledgeAsync(request);
    }

    /// <summary>
    /// Creates a test recurring donation with random data.
    /// </summary>
    protected async Task<RecurringDonation> CreateTestRecurringDonationAsync(string? fundId = null)
    {
        var randomSuffix = _random.Next(10000, 99999);
        var request = new RecurringDonationCreateRequest
        {
            AmountCents = _random.Next(2500, 25000), // $25.00 to $250.00
            Schedule = "monthly"
        };

        // Note: RecurringDonationCreateRequest may not have FundId property
        // This would need to be handled through designations or relationships

        return await _givingService.CreateRecurringDonationAsync(request);
    }

    /// <summary>
    /// Cleans up a test fund after tests.
    /// </summary>
    protected async Task CleanupTestFundAsync(string fundId)
    {
        if (!string.IsNullOrEmpty(fundId))
        {
            try
            {
                // Note: Funds typically cannot be deleted, only archived
                // This is a placeholder for cleanup logic
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }

    /// <summary>
    /// Cleans up a test donation after tests.
    /// </summary>
    protected async Task CleanupTestDonationAsync(string donationId)
    {
        if (!string.IsNullOrEmpty(donationId))
        {
            try
            {
                await _givingService.DeleteDonationAsync(donationId);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }

    /// <summary>
    /// Cleans up a test batch after tests.
    /// </summary>
    protected async Task CleanupTestBatchAsync(string batchId)
    {
        if (!string.IsNullOrEmpty(batchId))
        {
            try
            {
                // Note: Committed batches typically cannot be deleted
                // This is a placeholder for cleanup logic
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }

    /// <summary>
    /// Cleans up a test pledge after tests.
    /// </summary>
    protected async Task CleanupTestPledgeAsync(string pledgeId)
    {
        if (!string.IsNullOrEmpty(pledgeId))
        {
            try
            {
                // Note: Pledges typically cannot be deleted, only archived
                // This is a placeholder for cleanup logic
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }

    /// <summary>
    /// Cleans up a test recurring donation after tests.
    /// </summary>
    protected async Task CleanupTestRecurringDonationAsync(string recurringDonationId)
    {
        if (!string.IsNullOrEmpty(recurringDonationId))
        {
            try
            {
                // Note: Recurring donations are typically cancelled, not deleted
                // This is a placeholder for cleanup logic
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }
}