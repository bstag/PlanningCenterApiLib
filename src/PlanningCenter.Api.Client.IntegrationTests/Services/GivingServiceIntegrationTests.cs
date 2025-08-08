using FluentAssertions;
using PlanningCenter.Api.Client.IntegrationTests.Infrastructure;
using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Giving;
using PlanningCenter.Api.Client.Models.Requests;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PlanningCenter.Api.Client.IntegrationTests.Services;

[Collection(nameof(TestCollection))]
public class GivingServiceIntegrationTests : GivingServiceIntegrationTestBase
{
    public GivingServiceIntegrationTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Fund_CreateGetUpdateDelete_ShouldWorkCorrectly()
    {
        SkipIfNoAuthentication();

        // Arrange & Act - Create
        var createdFund = await CreateTestFundAsync();

        try
        {
            // Assert - Create
            createdFund.Should().NotBeNull();
            createdFund.Id.Should().NotBeNullOrEmpty();
            createdFund.Name.Should().StartWith("Test Fund");
            createdFund.Visibility.Should().Be(false);

            // Act - Get
            var retrievedFund = await _givingService.GetFundAsync(createdFund.Id);

            // Assert - Get
            retrievedFund.Should().NotBeNull();
            retrievedFund!.Id.Should().Be(createdFund.Id);
            retrievedFund.Name.Should().Be(createdFund.Name);
            retrievedFund.Description.Should().Be(createdFund.Description);

            // Act - Update
            var updateRequest = new FundUpdateRequest
            {
                Name = $"Updated {createdFund.Name}",
                Description = "Updated description"
            };
            var updatedFund = await _givingService.UpdateFundAsync(createdFund.Id, updateRequest);

            // Assert - Update
            updatedFund.Should().NotBeNull();
            updatedFund.Id.Should().Be(createdFund.Id);
            updatedFund.Name.Should().Be(updateRequest.Name);
            updatedFund.Description.Should().Be(updateRequest.Description);
        }
        finally
        {
            // Cleanup
            await CleanupTestFundAsync(createdFund.Id);
        }
    }

    [Fact]
    public async Task Fund_List_ShouldReturnPagedResults()
    {
        SkipIfNoAuthentication();

        // Act
        var response = await _givingService.ListFundsAsync(new QueryParameters { PerPage = 5 });

        // Assert
        response.Should().NotBeNull();
        response.Data.Should().NotBeNull();
        response.Data.Count.Should().BeGreaterThan(0);
        response.Data.Count.Should().BeLessThanOrEqualTo(5);
        response.Meta.Should().NotBeNull();
        response.Meta.TotalCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Batch_CreateGetUpdateCommit_ShouldWorkCorrectly()
    {
        SkipIfNoAuthentication();

        // Arrange & Act - Create
        var createdBatch = await CreateTestBatchAsync();

        try
        {
            // Assert - Create
            createdBatch.Should().NotBeNull();
            createdBatch.Id.Should().NotBeNullOrEmpty();
            createdBatch.Description.Should().StartWith("Test Batch");
            createdBatch.CommittedAt.Should().BeNull();

            // Act - Get
            var retrievedBatch = await _givingService.GetBatchAsync(createdBatch.Id);

            // Assert - Get
            retrievedBatch.Should().NotBeNull();
            retrievedBatch!.Id.Should().Be(createdBatch.Id);
            retrievedBatch.Description.Should().Be(createdBatch.Description);

            // Act - Update
            var updateRequest = new BatchUpdateRequest
            {
                Description = $"Updated {createdBatch.Description}"
            };
            var updatedBatch = await _givingService.UpdateBatchAsync(createdBatch.Id, updateRequest);

            // Assert - Update
            updatedBatch.Should().NotBeNull();
            updatedBatch.Id.Should().Be(createdBatch.Id);
            updatedBatch.Description.Should().Be(updateRequest.Description);

            // Act - Commit
            var committedBatch = await _givingService.CommitBatchAsync(createdBatch.Id);

            // Assert - Commit
            committedBatch.Should().NotBeNull();
            committedBatch.Id.Should().Be(createdBatch.Id);
            committedBatch.CommittedAt.Should().NotBeNull();
        }
        finally
        {
            // Cleanup
            await CleanupTestBatchAsync(createdBatch.Id);
        }
    }

    [Fact]
    public async Task Batch_List_ShouldReturnPagedResults()
    {
        SkipIfNoAuthentication();

        // Act
        var response = await _givingService.ListBatchesAsync(new QueryParameters { PerPage = 5 });

        // Assert
        response.Should().NotBeNull();
        response.Data.Should().NotBeNull();
        response.Data.Count.Should().BeGreaterThan(0);
        response.Data.Count.Should().BeLessThanOrEqualTo(5);
        response.Meta.Should().NotBeNull();
        response.Meta.TotalCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Donation_CreateGetUpdateDelete_ShouldWorkCorrectly()
    {
        SkipIfNoAuthentication();

        // Arrange - Create dependencies
        var testFund = await CreateTestFundAsync();
        var testBatch = await CreateTestBatchAsync();

        try
        {
            // Act - Create
            var createdDonation = await CreateTestDonationAsync(testFund.Id, testBatch.Id);

            try
            {
                // Assert - Create
                createdDonation.Should().NotBeNull();
                createdDonation.Id.Should().NotBeNullOrEmpty();
                createdDonation.AmountCents.Should().BeGreaterThan(0);
                createdDonation.PaymentLast4.Should().Be(null); // Cash donations don't have last 4 digits

                // Act - Get
                var retrievedDonation = await _givingService.GetDonationAsync(createdDonation.Id);

                // Assert - Get
                retrievedDonation.Should().NotBeNull();
                retrievedDonation!.Id.Should().Be(createdDonation.Id);
                retrievedDonation.AmountCents.Should().Be(createdDonation.AmountCents);
                retrievedDonation.PaymentLast4.Should().Be(createdDonation.PaymentLast4);

                // Act - Update
                var updateRequest = new DonationUpdateRequest
                {
                    AmountCents = createdDonation.AmountCents + 500 // Add $5.00
                };
                var updatedDonation = await _givingService.UpdateDonationAsync(createdDonation.Id, updateRequest);

                // Assert - Update
                updatedDonation.Should().NotBeNull();
                updatedDonation.Id.Should().Be(createdDonation.Id);
                updatedDonation.AmountCents.Should().Be(updateRequest.AmountCents);

                // Act - Delete
                await _givingService.DeleteDonationAsync(createdDonation.Id);

                // Assert - Delete (verify it's gone)
                var deletedDonation = await _givingService.GetDonationAsync(createdDonation.Id);
                deletedDonation.Should().BeNull();
            }
            finally
            {
                // Cleanup donation if it still exists
                await CleanupTestDonationAsync(createdDonation.Id);
            }
        }
        finally
        {
            // Cleanup dependencies
            await CleanupTestFundAsync(testFund.Id);
            await CleanupTestBatchAsync(testBatch.Id);
        }
    }

    [Fact]
    public async Task Donation_List_ShouldReturnPagedResults()
    {
        SkipIfNoAuthentication();

        // Act
        var response = await _givingService.ListDonationsAsync(new QueryParameters { PerPage = 5 });

        // Assert
        response.Should().NotBeNull();
        response.Data.Should().NotBeNull();
        response.Data.Count.Should().BeGreaterThan(0);
        response.Data.Count.Should().BeLessThanOrEqualTo(5);
        response.Meta.Should().NotBeNull();
        response.Meta.TotalCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Donation_GetAllAsync_ShouldReturnAllDonations()
    {
        SkipIfNoAuthentication();

        // Act
        var donations = await _givingService.GetAllDonationsAsync(new QueryParameters { PerPage = 2 });

        // Assert
        donations.Should().NotBeNull();
        donations.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Donation_StreamAsync_ShouldStreamDonations()
    {
        SkipIfNoAuthentication();

        // Act
        var donationCount = 0;
        await foreach (var donation in _givingService.StreamDonationsAsync(new QueryParameters { PerPage = 2 }))
        {
            donation.Should().NotBeNull();
            donation.Id.Should().NotBeNullOrEmpty();
            donationCount++;
            
            // Limit to avoid long-running test
            if (donationCount >= 5) break;
        }

        // Assert
        donationCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Pledge_CreateGetUpdate_ShouldWorkCorrectly()
    {
        SkipIfNoAuthentication();

        // Arrange - Create dependency
        var testFund = await CreateTestFundAsync();

        try
        {
            // Act - Create
            var createdPledge = await CreateTestPledgeAsync(testFund.Id);

            try
            {
                // Assert - Create
                createdPledge.Should().NotBeNull();
                createdPledge.Id.Should().NotBeNullOrEmpty();
                createdPledge.AmountCents.Should().BeGreaterThan(0);

                // Act - Get
                var retrievedPledge = await _givingService.GetPledgeAsync(createdPledge.Id);

                // Assert - Get
                retrievedPledge.Should().NotBeNull();
                retrievedPledge!.Id.Should().Be(createdPledge.Id);
                retrievedPledge.AmountCents.Should().Be(createdPledge.AmountCents);

                // Act - Update
                var updateRequest = new PledgeUpdateRequest
                {
                    AmountCents = createdPledge.AmountCents + 1000 // Add $10.00
                };
                var updatedPledge = await _givingService.UpdatePledgeAsync(createdPledge.Id, updateRequest);

                // Assert - Update
                updatedPledge.Should().NotBeNull();
                updatedPledge.Id.Should().Be(createdPledge.Id);
                updatedPledge.AmountCents.Should().Be(updateRequest.AmountCents);
            }
            finally
            {
                // Cleanup
                await CleanupTestPledgeAsync(createdPledge.Id);
            }
        }
        finally
        {
            // Cleanup dependency
            await CleanupTestFundAsync(testFund.Id);
        }
    }

    [Fact]
    public async Task Pledge_List_ShouldReturnPagedResults()
    {
        SkipIfNoAuthentication();

        // Act
        var response = await _givingService.ListPledgesAsync(new QueryParameters { PerPage = 5 });

        // Assert
        response.Should().NotBeNull();
        response.Data.Should().NotBeNull();
        response.Data.Count.Should().BeGreaterThan(0);
        response.Data.Count.Should().BeLessThanOrEqualTo(5);
        response.Meta.Should().NotBeNull();
        response.Meta.TotalCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task RecurringDonation_CreateGetUpdate_ShouldWorkCorrectly()
    {
        SkipIfNoAuthentication();

        // Arrange - Create dependency
        var testFund = await CreateTestFundAsync();

        try
        {
            // Act - Create
            var createdRecurringDonation = await CreateTestRecurringDonationAsync(testFund.Id);

            try
            {
                // Assert - Create
                createdRecurringDonation.Should().NotBeNull();
                createdRecurringDonation.Id.Should().NotBeNullOrEmpty();
                createdRecurringDonation.AmountCents.Should().BeGreaterThan(0);
                createdRecurringDonation.Schedule.Should().Be("monthly");

                // Act - Get
                var retrievedRecurringDonation = await _givingService.GetRecurringDonationAsync(createdRecurringDonation.Id);

                // Assert - Get
                retrievedRecurringDonation.Should().NotBeNull();
                retrievedRecurringDonation!.Id.Should().Be(createdRecurringDonation.Id);
                retrievedRecurringDonation.AmountCents.Should().Be(createdRecurringDonation.AmountCents);
                retrievedRecurringDonation.Schedule.Should().Be(createdRecurringDonation.Schedule);

                // Act - Update
                var updateRequest = new RecurringDonationUpdateRequest
                {
                    AmountCents = createdRecurringDonation.AmountCents + 500, // Add $5.00
                    Schedule = "weekly"
                };
                var updatedRecurringDonation = await _givingService.UpdateRecurringDonationAsync(createdRecurringDonation.Id, updateRequest);

                // Assert - Update
                updatedRecurringDonation.Should().NotBeNull();
                updatedRecurringDonation.Id.Should().Be(createdRecurringDonation.Id);
                updatedRecurringDonation.AmountCents.Should().Be(updateRequest.AmountCents);
                updatedRecurringDonation.Schedule.Should().Be(updateRequest.Schedule);
            }
            finally
            {
                // Cleanup
                await CleanupTestRecurringDonationAsync(createdRecurringDonation.Id);
            }
        }
        finally
        {
            // Cleanup dependency
            await CleanupTestFundAsync(testFund.Id);
        }
    }

    [Fact]
    public async Task RecurringDonation_List_ShouldReturnPagedResults()
    {
        SkipIfNoAuthentication();

        // Act
        var response = await _givingService.ListRecurringDonationsAsync(new QueryParameters { PerPage = 5 });

        // Assert
        response.Should().NotBeNull();
        response.Data.Should().NotBeNull();
        response.Data.Count.Should().BeGreaterThan(0);
        response.Data.Count.Should().BeLessThanOrEqualTo(5);
        response.Meta.Should().NotBeNull();
        response.Meta.TotalCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task PaymentSource_Get_ShouldReturnPaymentSource()
    {
        SkipIfNoAuthentication();

        // First get a list to find a valid payment source ID
        var paymentSources = await _givingService.ListPaymentSourcesAsync(new QueryParameters { PerPage = 1 });
        
        if (paymentSources.Data.Any())
        {
            var paymentSourceId = paymentSources.Data.First().Id;

            // Act
            var paymentSource = await _givingService.GetPaymentSourceAsync(paymentSourceId);

            // Assert
            paymentSource.Should().NotBeNull();
            paymentSource!.Id.Should().Be(paymentSourceId);
        }
    }

    [Fact]
    public async Task PaymentSource_List_ShouldReturnPagedResults()
    {
        SkipIfNoAuthentication();

        // Act
        var response = await _givingService.ListPaymentSourcesAsync(new QueryParameters { PerPage = 5 });

        // Assert
        response.Should().NotBeNull();
        response.Data.Should().NotBeNull();
        response.Meta.Should().NotBeNull();
    }

    [Fact]
    public async Task Refund_List_ShouldReturnPagedResults()
    {
        SkipIfNoAuthentication();

        // Act
        var response = await _givingService.ListRefundsAsync(new QueryParameters { PerPage = 5 });

        // Assert
        response.Should().NotBeNull();
        response.Data.Should().NotBeNull();
        response.Meta.Should().NotBeNull();
    }
}