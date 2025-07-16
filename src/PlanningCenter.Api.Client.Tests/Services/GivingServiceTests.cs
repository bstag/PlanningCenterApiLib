using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using PlanningCenter.Api.Client.Models.JsonApi;
using PlanningCenter.Api.Client.Models.JsonApi.Giving;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Services;
using PlanningCenter.Api.Client.Tests.Utilities;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Services;

public class GivingServiceTests
{
    private readonly MockApiConnection _mockApiConnection = new();
    private readonly GivingService _givingService;
    private readonly ExtendedTestDataBuilder _builder = new();

    public GivingServiceTests()
    {
        _givingService = new GivingService(_mockApiConnection, NullLogger<GivingService>.Instance);
    }

    #region Donation Tests

    [Fact]
    public async Task GetDonationAsync_ShouldReturnDonation_WhenApiReturnsData()
    {
        // Arrange
        var donationDto = _builder.CreateDonationDto(d => d.Id = "123");
        var response = new JsonApiSingleResponse<DonationDto> { Data = donationDto };
        _mockApiConnection.SetupGetResponse("/giving/v2/donations/123", response);

        // Act
        var result = await _givingService.GetDonationAsync("123");

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("123");
        result.AmountCents.Should().Be(donationDto.Attributes.AmountCents);
        result.AmountCurrency.Should().Be(donationDto.Attributes.AmountCurrency);
        result.DataSource.Should().Be("Giving");
    }

    [Fact]
    public async Task GetDonationAsync_ShouldReturnNull_WhenApiReturnsNull()
    {
        // Arrange
        _mockApiConnection.SetupGetResponse("/giving/v2/donations/999", (JsonApiSingleResponse<DonationDto>?)null);

        // Act
        var result = await _givingService.GetDonationAsync("999");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetDonationAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        // Act & Assert
        await _givingService.Invoking(s => s.GetDonationAsync(""))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("Donation ID cannot be null or empty*");
    }

    [Fact]
    public async Task ListDonationsAsync_ShouldReturnPagedDonations_WhenApiReturnsData()
    {
        // Arrange
        var donationsResponse = _builder.BuildDonationCollectionResponse(3);
        _mockApiConnection.SetupGetResponse("/giving/v2/donations", donationsResponse);

        // Act
        var result = await _givingService.ListDonationsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(3);
        result.Meta.Should().NotBeNull();
        result.Meta.TotalCount.Should().Be(3);
    }

    [Fact]
    public async Task CreateDonationAsync_ShouldReturnCreatedDonation_WhenRequestIsValid()
    {
        // Arrange
        var request = new DonationCreateRequest
        {
            AmountCents = 5000,
            AmountCurrency = "USD",
            PaymentMethod = "Credit Card",
            PersonId = "person123"
        };

        var donationDto = _builder.CreateDonationDto(d => 
        {
            d.Id = "new123";
            d.Attributes.AmountCents = 5000;
            d.Attributes.AmountCurrency = "USD";
            d.Attributes.PaymentMethod = "Credit Card";
        });

        var response = new JsonApiSingleResponse<DonationDto> { Data = donationDto };
        _mockApiConnection.SetupMutationResponse("POST", "/giving/v2/donations", response);

        // Act
        var result = await _givingService.CreateDonationAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("new123");
        result.AmountCents.Should().Be(5000);
        result.AmountCurrency.Should().Be("USD");
        result.PaymentMethod.Should().Be("Credit Card");
    }

    [Fact]
    public async Task CreateDonationAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Act & Assert
        await _givingService.Invoking(s => s.CreateDonationAsync(null!))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdateDonationAsync_ShouldReturnUpdatedDonation_WhenRequestIsValid()
    {
        // Arrange
        var request = new DonationUpdateRequest
        {
            AmountCents = 6000,
            PaymentMethod = "Check"
        };

        var donationDto = _builder.CreateDonationDto(d => 
        {
            d.Id = "123";
            d.Attributes.AmountCents = 6000;
            d.Attributes.PaymentMethod = "Check";
        });

        var response = new JsonApiSingleResponse<DonationDto> { Data = donationDto };
        _mockApiConnection.SetupMutationResponse("PATCH", "/giving/v2/donations/123", response);

        // Act
        var result = await _givingService.UpdateDonationAsync("123", request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("123");
        result.AmountCents.Should().Be(6000);
        result.PaymentMethod.Should().Be("Check");
    }

    [Fact]
    public async Task DeleteDonationAsync_ShouldCompleteSuccessfully_WhenIdIsValid()
    {
        // Act & Assert
        await _givingService.Invoking(s => s.DeleteDonationAsync("123"))
            .Should().NotThrowAsync();
    }

    #endregion

    #region Fund Tests

    [Fact]
    public async Task GetFundAsync_ShouldReturnFund_WhenApiReturnsData()
    {
        // Arrange
        var fundDto = _builder.CreateFundDto(f => f.Id = "fund123");
        var response = new JsonApiSingleResponse<FundDto> { Data = fundDto };
        _mockApiConnection.SetupGetResponse("/giving/v2/funds/fund123", response);

        // Act
        var result = await _givingService.GetFundAsync("fund123");

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("fund123");
        result.Name.Should().Be(fundDto.Attributes.Name);
        result.DataSource.Should().Be("Giving");
    }

    [Fact]
    public async Task ListFundsAsync_ShouldReturnPagedFunds_WhenApiReturnsData()
    {
        // Arrange
        var fundsResponse = _builder.BuildFundCollectionResponse(2);
        _mockApiConnection.SetupGetResponse("/giving/v2/funds", fundsResponse);

        // Act
        var result = await _givingService.ListFundsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Meta.Should().NotBeNull();
        result.Meta.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task CreateFundAsync_ShouldReturnCreatedFund_WhenRequestIsValid()
    {
        // Arrange
        var request = new FundCreateRequest
        {
            Name = "Building Fund",
            Description = "For building improvements",
            Code = "BUILD"
        };

        var fundDto = _builder.CreateFundDto(f => 
        {
            f.Id = "newfund123";
            f.Attributes.Name = "Building Fund";
            f.Attributes.Description = "For building improvements";
            f.Attributes.Code = "BUILD";
        });

        var response = new JsonApiSingleResponse<FundDto> { Data = fundDto };
        _mockApiConnection.SetupMutationResponse("POST", "/giving/v2/funds", response);

        // Act
        var result = await _givingService.CreateFundAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("newfund123");
        result.Name.Should().Be("Building Fund");
        result.Description.Should().Be("For building improvements");
        result.Code.Should().Be("BUILD");
    }

    #endregion

    #region Batch Tests

    [Fact]
    public async Task GetBatchAsync_ShouldReturnBatch_WhenApiReturnsData()
    {
        // Arrange
        var batchDto = _builder.CreateBatchDto(b => b.Id = "batch123");
        var response = new JsonApiSingleResponse<BatchDto> { Data = batchDto };
        _mockApiConnection.SetupGetResponse("/giving/v2/batches/batch123", response);

        // Act
        var result = await _givingService.GetBatchAsync("batch123");

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("batch123");
        result.Description.Should().Be(batchDto.Attributes.Description);
        result.DataSource.Should().Be("Giving");
    }

    [Fact]
    public async Task CommitBatchAsync_ShouldReturnCommittedBatch_WhenIdIsValid()
    {
        // Arrange
        var batchDto = _builder.CreateBatchDto(b => 
        {
            b.Id = "batch123";
            b.Attributes.Status = "committed";
            b.Attributes.CommittedAt = DateTime.UtcNow;
        });

        var response = new JsonApiSingleResponse<BatchDto> { Data = batchDto };
        _mockApiConnection.SetupMutationResponse("POST", "/giving/v2/batches/batch123/commit", response);

        // Act
        var result = await _givingService.CommitBatchAsync("batch123");

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("batch123");
        result.Status.Should().Be("committed");
        result.CommittedAt.Should().NotBeNull();
    }

    #endregion

    #region Pledge Tests

    [Fact]
    public async Task GetPledgeAsync_ShouldReturnPledge_WhenApiReturnsData()
    {
        // Arrange
        var pledgeDto = _builder.CreatePledgeDto(p => p.Id = "pledge123");
        var response = new JsonApiSingleResponse<PledgeDto> { Data = pledgeDto };
        _mockApiConnection.SetupGetResponse("/giving/v2/pledges/pledge123", response);

        // Act
        var result = await _givingService.GetPledgeAsync("pledge123");

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("pledge123");
        result.AmountCents.Should().Be(pledgeDto.Attributes.AmountCents);
        result.DataSource.Should().Be("Giving");
    }

    [Fact]
    public async Task CreatePledgeAsync_ShouldReturnCreatedPledge_WhenRequestIsValid()
    {
        // Arrange
        var request = new PledgeCreateRequest
        {
            AmountCents = 100000,
            AmountCurrency = "USD",
            PersonId = "person123",
            FundId = "fund123"
        };

        var pledgeDto = _builder.CreatePledgeDto(p => 
        {
            p.Id = "newpledge123";
            p.Attributes.AmountCents = 100000;
            p.Attributes.AmountCurrency = "USD";
        });

        var response = new JsonApiSingleResponse<PledgeDto> { Data = pledgeDto };
        _mockApiConnection.SetupMutationResponse("POST", "/giving/v2/pledges", response);

        // Act
        var result = await _givingService.CreatePledgeAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("newpledge123");
        result.AmountCents.Should().Be(100000);
        result.AmountCurrency.Should().Be("USD");
    }

    #endregion

    #region Recurring Donation Tests

    [Fact]
    public async Task GetRecurringDonationAsync_ShouldReturnRecurringDonation_WhenApiReturnsData()
    {
        // Arrange
        var recurringDto = _builder.CreateRecurringDonationDto(r => r.Id = "recurring123");
        var response = new JsonApiSingleResponse<RecurringDonationDto> { Data = recurringDto };
        _mockApiConnection.SetupGetResponse("/giving/v2/recurring_donations/recurring123", response);

        // Act
        var result = await _givingService.GetRecurringDonationAsync("recurring123");

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("recurring123");
        result.AmountCents.Should().Be(recurringDto.Attributes.AmountCents);
        result.DataSource.Should().Be("Giving");
    }

    [Fact]
    public async Task CreateRecurringDonationAsync_ShouldReturnCreatedRecurringDonation_WhenRequestIsValid()
    {
        // Arrange
        var request = new RecurringDonationCreateRequest
        {
            AmountCents = 5000,
            AmountCurrency = "USD",
            Schedule = "monthly",
            PersonId = "person123"
        };

        var recurringDto = _builder.CreateRecurringDonationDto(r => 
        {
            r.Id = "newrecurring123";
            r.Attributes.AmountCents = 5000;
            r.Attributes.AmountCurrency = "USD";
            r.Attributes.Schedule = "monthly";
        });

        var response = new JsonApiSingleResponse<RecurringDonationDto> { Data = recurringDto };
        _mockApiConnection.SetupMutationResponse("POST", "/giving/v2/recurring_donations", response);

        // Act
        var result = await _givingService.CreateRecurringDonationAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("newrecurring123");
        result.AmountCents.Should().Be(5000);
        result.Schedule.Should().Be("monthly");
    }

    #endregion

    #region Person-specific Operations Tests

    [Fact]
    public async Task GetDonationsForPersonAsync_ShouldReturnPersonDonations_WhenApiReturnsData()
    {
        // Arrange
        var donationsResponse = _builder.BuildDonationCollectionResponse(2);
        _mockApiConnection.SetupGetResponse("/giving/v2/people/person123/donations", donationsResponse);

        // Act
        var result = await _givingService.GetDonationsForPersonAsync("person123");

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Meta.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPledgesForPersonAsync_ShouldReturnPersonPledges_WhenApiReturnsData()
    {
        // Arrange
        var pledgesResponse = _builder.BuildPledgeCollectionResponse(1);
        _mockApiConnection.SetupGetResponse("/giving/v2/people/person123/pledges", pledgesResponse);

        // Act
        var result = await _givingService.GetPledgesForPersonAsync("person123");

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
        result.Meta.Should().NotBeNull();
    }

    [Fact]
    public async Task GetRecurringDonationsForPersonAsync_ShouldReturnPersonRecurringDonations_WhenApiReturnsData()
    {
        // Arrange
        var recurringResponse = _builder.BuildRecurringDonationCollectionResponse(1);
        _mockApiConnection.SetupGetResponse("/giving/v2/people/person123/recurring_donations", recurringResponse);

        // Act
        var result = await _givingService.GetRecurringDonationsForPersonAsync("person123");

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
        result.Meta.Should().NotBeNull();
    }

    #endregion

    #region Pagination Helper Tests

    [Fact]
    public async Task GetAllDonationsAsync_ShouldReturnAllDonations_WhenMultiplePagesExist()
    {
        // Arrange
        var firstPageResponse = _builder.BuildDonationCollectionResponse(2);
        firstPageResponse.Links = new() { Next = "/giving/v2/donations?offset=2" };
        
        var secondPageResponse = _builder.BuildDonationCollectionResponse(1);
        secondPageResponse.Links = new() { Next = null };

        _mockApiConnection.SetupGetResponse("/giving/v2/donations", firstPageResponse);
        _mockApiConnection.SetupGetResponse("/giving/v2/donations?offset=2", secondPageResponse);

        // Act
        var result = await _givingService.GetAllDonationsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3); // 2 from first page + 1 from second page
    }

    [Fact]
    public async Task StreamDonationsAsync_ShouldYieldAllDonations_WhenMultiplePagesExist()
    {
        // Arrange
        var firstPageResponse = _builder.BuildDonationCollectionResponse(2);
        firstPageResponse.Links = new() { Next = "/giving/v2/donations?offset=2" };
        
        var secondPageResponse = _builder.BuildDonationCollectionResponse(1);
        secondPageResponse.Links = new() { Next = null };

        _mockApiConnection.SetupGetResponse("/giving/v2/donations", firstPageResponse);
        _mockApiConnection.SetupGetResponse("/giving/v2/donations?offset=2", secondPageResponse);

        // Act
        var donations = new List<Models.Giving.Donation>();
        await foreach (var donation in _givingService.StreamDonationsAsync())
        {
            donations.Add(donation);
        }

        // Assert
        donations.Should().HaveCount(3); // 2 from first page + 1 from second page
    }

    #endregion

    #region Analytics Tests

    [Fact]
    public async Task GetTotalGivingAsync_ShouldReturnTotalAmount_WhenApiReturnsData()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;
        
        var donationsResponse = _builder.BuildDonationCollectionResponse(2);
        // Modify the donations to have specific amounts
        donationsResponse.Data![0].Attributes.AmountCents = 5000;
        donationsResponse.Data![1].Attributes.AmountCents = 7500;
        
        _mockApiConnection.SetupGetResponse($"/giving/v2/donations?filter[received_at]={startDate:yyyy-MM-dd}..{endDate:yyyy-MM-dd}", donationsResponse);

        // Act
        var result = await _givingService.GetTotalGivingAsync(startDate, endDate);

        // Assert
        result.Should().Be(12500); // 5000 + 7500
    }

    #endregion
}