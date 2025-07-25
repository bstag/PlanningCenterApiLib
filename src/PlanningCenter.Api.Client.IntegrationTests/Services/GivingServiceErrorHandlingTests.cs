using FluentAssertions;
using PlanningCenter.Api.Client.IntegrationTests.Infrastructure;
using PlanningCenter.Api.Client.Models.Exceptions;
using PlanningCenter.Api.Client.Models.Requests;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PlanningCenter.Api.Client.IntegrationTests.Services;

[Collection(nameof(TestCollection))]
public class GivingServiceErrorHandlingTests : GivingServiceIntegrationTestBase
{
    public GivingServiceErrorHandlingTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task GetDonationAsync_ShouldReturnNull_WhenDonationDoesNotExist()
    {
        SkipIfNoAuthentication();

        // Arrange
        var nonExistentId = "999999999";

        // Act
        var result = await _givingService.GetDonationAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetFundAsync_ShouldReturnNull_WhenFundDoesNotExist()
    {
        SkipIfNoAuthentication();

        // Arrange
        var nonExistentId = "999999999";

        // Act
        var result = await _givingService.GetFundAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetBatchAsync_ShouldReturnNull_WhenBatchDoesNotExist()
    {
        SkipIfNoAuthentication();

        // Arrange
        var nonExistentId = "999999999";

        // Act
        var result = await _givingService.GetBatchAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPledgeAsync_ShouldReturnNull_WhenPledgeDoesNotExist()
    {
        SkipIfNoAuthentication();

        // Arrange
        var nonExistentId = "999999999";

        // Act
        var result = await _givingService.GetPledgeAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetRecurringDonationAsync_ShouldReturnNull_WhenRecurringDonationDoesNotExist()
    {
        SkipIfNoAuthentication();

        // Arrange
        var nonExistentId = "999999999";

        // Act
        var result = await _givingService.GetRecurringDonationAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPaymentSourceAsync_ShouldReturnNull_WhenPaymentSourceDoesNotExist()
    {
        SkipIfNoAuthentication();

        // Arrange
        var nonExistentId = "999999999";

        // Act
        var result = await _givingService.GetPaymentSourceAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateDonationAsync_ShouldThrowValidationException_WhenRequiredFieldsAreMissing()
    {
        SkipIfNoAuthentication();

        // Arrange - Create an invalid donation request
        var invalidRequest = new DonationCreateRequest
        {
            // Missing AmountCents which is required
            PaymentMethod = "cash"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PlanningCenterApiValidationException>(
            () => _givingService.CreateDonationAsync(invalidRequest));

        exception.Should().NotBeNull();
        exception.Message.Should().ContainEquivalentOf("validation");
    }

    [Fact]
    public async Task CreateFundAsync_ShouldThrowValidationException_WhenRequiredFieldsAreMissing()
    {
        SkipIfNoAuthentication();

        // Arrange - Create an invalid fund request
        var invalidRequest = new FundCreateRequest
        {
            // Missing Name which is required
            Description = "Test Description"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PlanningCenterApiValidationException>(
            () => _givingService.CreateFundAsync(invalidRequest));

        exception.Should().NotBeNull();
        exception.Message.Should().ContainEquivalentOf("validation");
    }

    [Fact]
    public async Task CreateBatchAsync_ShouldThrowValidationException_WhenRequiredFieldsAreMissing()
    {
        SkipIfNoAuthentication();

        // Arrange - Create an invalid batch request
        var invalidRequest = new BatchCreateRequest
        {
            // Missing Description which is required
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PlanningCenterApiValidationException>(
            () => _givingService.CreateBatchAsync(invalidRequest));

        exception.Should().NotBeNull();
        exception.Message.Should().ContainEquivalentOf("validation");
    }

    [Fact]
    public async Task CreatePledgeAsync_ShouldThrowValidationException_WhenRequiredFieldsAreMissing()
    {
        SkipIfNoAuthentication();

        // Arrange - Create an invalid pledge request
        var invalidRequest = new PledgeCreateRequest
        {
            // Missing AmountCents which is required
            JointGiverAmountCents = 0
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PlanningCenterApiValidationException>(
            () => _givingService.CreatePledgeAsync(invalidRequest));

        exception.Should().NotBeNull();
        exception.Message.Should().ContainEquivalentOf("validation");
    }

    [Fact]
    public async Task CreateRecurringDonationAsync_ShouldThrowValidationException_WhenRequiredFieldsAreMissing()
    {
        SkipIfNoAuthentication();

        // Arrange - Create an invalid recurring donation request
        var invalidRequest = new RecurringDonationCreateRequest
        {
            // Missing AmountCents which is required
            Schedule = "monthly"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PlanningCenterApiValidationException>(
            () => _givingService.CreateRecurringDonationAsync(invalidRequest));

        exception.Should().NotBeNull();
        exception.Message.Should().ContainEquivalentOf("validation");
    }

    [Fact]
    public async Task UpdateDonationAsync_ShouldThrowNotFoundException_WhenDonationDoesNotExist()
    {
        SkipIfNoAuthentication();

        // Arrange
        var nonExistentId = "999999999";
        var updateRequest = new DonationUpdateRequest
        {
            AmountCents = 5000
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(
            () => _givingService.UpdateDonationAsync(nonExistentId, updateRequest));

        exception.Should().NotBeNull();
        exception.Message.Should().ContainEquivalentOf("not found");
    }

    [Fact]
    public async Task UpdateFundAsync_ShouldThrowNotFoundException_WhenFundDoesNotExist()
    {
        SkipIfNoAuthentication();

        // Arrange
        var nonExistentId = "999999999";
        var updateRequest = new FundUpdateRequest
        {
            Name = "Updated Fund Name"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(
            () => _givingService.UpdateFundAsync(nonExistentId, updateRequest));

        exception.Should().NotBeNull();
        exception.Message.Should().ContainEquivalentOf("not found");
    }

    [Fact]
    public async Task UpdateBatchAsync_ShouldThrowNotFoundException_WhenBatchDoesNotExist()
    {
        SkipIfNoAuthentication();

        // Arrange
        var nonExistentId = "999999999";
        var updateRequest = new BatchUpdateRequest
        {
            Description = "Updated Batch Description"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(
            () => _givingService.UpdateBatchAsync(nonExistentId, updateRequest));

        exception.Should().NotBeNull();
        exception.Message.Should().ContainEquivalentOf("not found");
    }

    [Fact]
    public async Task UpdatePledgeAsync_ShouldThrowNotFoundException_WhenPledgeDoesNotExist()
    {
        SkipIfNoAuthentication();

        // Arrange
        var nonExistentId = "999999999";
        var updateRequest = new PledgeUpdateRequest
        {
            AmountCents = 10000
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(
            () => _givingService.UpdatePledgeAsync(nonExistentId, updateRequest));

        exception.Should().NotBeNull();
        exception.Message.Should().ContainEquivalentOf("not found");
    }

    [Fact]
    public async Task UpdateRecurringDonationAsync_ShouldThrowNotFoundException_WhenRecurringDonationDoesNotExist()
    {
        SkipIfNoAuthentication();

        // Arrange
        var nonExistentId = "999999999";
        var updateRequest = new RecurringDonationUpdateRequest
        {
            AmountCents = 5000,
            Schedule = "weekly"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(
            () => _givingService.UpdateRecurringDonationAsync(nonExistentId, updateRequest));

        exception.Should().NotBeNull();
        exception.Message.Should().ContainEquivalentOf("not found");
    }

    [Fact]
    public async Task DeleteDonationAsync_ShouldThrowNotFoundException_WhenDonationDoesNotExist()
    {
        SkipIfNoAuthentication();

        // Arrange
        var nonExistentId = "999999999";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(
            () => _givingService.DeleteDonationAsync(nonExistentId));

        exception.Should().NotBeNull();
        exception.Message.Should().ContainEquivalentOf("not found");
    }

    [Fact]
    public async Task CommitBatchAsync_ShouldThrowNotFoundException_WhenBatchDoesNotExist()
    {
        SkipIfNoAuthentication();

        // Arrange
        var nonExistentId = "999999999";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(
            () => _givingService.CommitBatchAsync(nonExistentId));

        exception.Should().NotBeNull();
        exception.Message.Should().ContainEquivalentOf("not found");
    }

    [Fact]
    public async Task IssueRefundAsync_ShouldThrowNotFoundException_WhenDonationDoesNotExist()
    {
        SkipIfNoAuthentication();

        // Arrange
        var nonExistentDonationId = "999999999";
        var refundRequest = new RefundCreateRequest
        {
            AmountCents = 1000
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(
            () => _givingService.IssueRefundAsync(nonExistentDonationId, refundRequest));

        exception.Should().NotBeNull();
        exception.Message.Should().ContainEquivalentOf("not found");
    }
}