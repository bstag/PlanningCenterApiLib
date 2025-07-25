using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.JsonApi;
using PlanningCenter.Api.Client.Models.JsonApi.Webhooks;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Services;
using PlanningCenter.Api.Client.Tests.Utilities;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Services;

public class WebhooksServiceTests
{
    private readonly MockApiConnection _mockApiConnection = new();
    private readonly WebhooksService _webhooksService;
    private readonly ExtendedTestDataBuilder _builder = new();

    public WebhooksServiceTests()
    {
        _webhooksService = new WebhooksService(_mockApiConnection, NullLogger<WebhooksService>.Instance);
    }

    #region Subscription Management Tests

    [Fact]
    public async Task GetSubscriptionAsync_ShouldReturnSubscription_WhenApiReturnsData()
    {
        // Arrange
        var subscriptionDto = _builder.CreateWebhookSubscriptionDto(s => s.Id = "sub123");
        var response = new JsonApiSingleResponse<WebhookSubscriptionDto> { Data = subscriptionDto };
        _mockApiConnection.SetupGetResponse("/webhooks/v2/subscriptions/sub123", response);

        // Act
        var result = await _webhooksService.GetSubscriptionAsync("sub123");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("sub123");
        result!.Url.Should().Be(subscriptionDto.Attributes.Url);
        result!.DataSource.Should().Be("Webhooks");
    }

    [Fact]
    public async Task GetSubscriptionAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        // Act & Assert
        await _webhooksService.Invoking(s => s.GetSubscriptionAsync(""))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("Subscription ID cannot be null or empty*");
    }

    [Fact]
    public async Task ListSubscriptionsAsync_ShouldReturnPagedSubscriptions_WhenApiReturnsData()
    {
        // Arrange
        var subscriptionsResponse = _builder.BuildWebhookSubscriptionCollectionResponse(3);
        _mockApiConnection.SetupGetResponse("/webhooks/v2/subscriptions", subscriptionsResponse);

        // Act
        var result = await _webhooksService.ListSubscriptionsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(3);
        result.Meta.Should().NotBeNull();
        result.Meta.TotalCount.Should().Be(3);
    }

    [Fact]
    public async Task CreateSubscriptionAsync_ShouldReturnCreatedSubscription_WhenRequestIsValid()
    {
        // Arrange
        var request = new WebhookSubscriptionCreateRequest
        {
            Url = "https://example.com/webhook",
            Secret = "secret123",
            AvailableEventId = "event123"
        };

        var subscriptionDto = _builder.CreateWebhookSubscriptionDto(s => 
        {
            s.Id = "newsub123";
            s.Attributes.Url = "https://example.com/webhook";
            s.Attributes.Secret = "secret123";
            s.Attributes.Active = true;
        });

        var response = new JsonApiSingleResponse<WebhookSubscriptionDto> { Data = subscriptionDto };
        _mockApiConnection.SetupMutationResponse("POST", "/webhooks/v2/subscriptions", response);

        // Act
        var result = await _webhooksService.CreateSubscriptionAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("newsub123");
        result.Url.Should().Be("https://example.com/webhook");
        result.Active.Should().BeTrue();
    }

    [Fact]
    public async Task CreateSubscriptionAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Act & Assert
        await _webhooksService.Invoking(s => s.CreateSubscriptionAsync(null!))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdateSubscriptionAsync_ShouldReturnUpdatedSubscription_WhenRequestIsValid()
    {
        // Arrange
        var request = new WebhookSubscriptionUpdateRequest
        {
            Url = "https://updated.example.com/webhook",
            Secret = "newsecret123"
        };

        var subscriptionDto = _builder.CreateWebhookSubscriptionDto(s => 
        {
            s.Id = "sub123";
            s.Attributes.Url = "https://updated.example.com/webhook";
            s.Attributes.Secret = "newsecret123";
        });

        var response = new JsonApiSingleResponse<WebhookSubscriptionDto> { Data = subscriptionDto };
        _mockApiConnection.SetupMutationResponse("PATCH", "/webhooks/v2/subscriptions/sub123", response);

        // Act
        var result = await _webhooksService.UpdateSubscriptionAsync("sub123", request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("sub123");
        result.Url.Should().Be("https://updated.example.com/webhook");
    }

    [Fact]
    public async Task DeleteSubscriptionAsync_ShouldCompleteSuccessfully_WhenIdIsValid()
    {
        // Act & Assert
        await _webhooksService.Invoking(s => s.DeleteSubscriptionAsync("sub123"))
            .Should().NotThrowAsync();
    }

    #endregion

    #region Subscription Activation Tests

    [Fact]
    public async Task ActivateSubscriptionAsync_ShouldReturnActivatedSubscription_WhenIdIsValid()
    {
        // Arrange
        var response = new JsonApiSingleResponse<dynamic> 
        { 
            Data = new { id = "sub123", type = "WebhookSubscription" } 
        };
        _mockApiConnection.SetupMutationResponse("POST", "/webhooks/v2/subscriptions/sub123/activate", response);

        // Act
        var result = await _webhooksService.ActivateSubscriptionAsync("sub123");

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("sub123");
        result.Active.Should().BeTrue();
    }

    [Fact]
    public async Task DeactivateSubscriptionAsync_ShouldReturnDeactivatedSubscription_WhenIdIsValid()
    {
        // Arrange
        var response = new JsonApiSingleResponse<dynamic> 
        { 
            Data = new { id = "sub123", type = "WebhookSubscription" } 
        };
        _mockApiConnection.SetupMutationResponse("POST", "/webhooks/v2/subscriptions/sub123/deactivate", response);

        // Act
        var result = await _webhooksService.DeactivateSubscriptionAsync("sub123");

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("sub123");
        result.Active.Should().BeFalse();
    }

    #endregion

    #region Event Validation Tests


    [Fact]
    public async Task ValidateEventSignatureAsync_ShouldReturnFalse_WhenSignatureIsInvalid()
    {
        // Arrange
        var payload = "{\"test\":\"data\"}";
        var secret = "test_secret";
        var signature = "sha256=invalid_signature";

        // Act
        var result = await _webhooksService.ValidateEventSignatureAsync(payload, signature, secret);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateEventSignatureAsync_ShouldThrowArgumentException_WhenPayloadIsEmpty()
    {
        // Act & Assert
        await _webhooksService.Invoking(s => s.ValidateEventSignatureAsync("", "signature", "secret"))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("Payload cannot be null or empty*");
    }

    [Fact]
    public async Task DeserializeEventAsync_ShouldReturnDeserializedObject_WhenPayloadIsValid()
    {
        // Arrange
        var payload = "{\"id\":\"123\",\"name\":\"test\"}";

        // Act
        var result = await _webhooksService.DeserializeEventAsync<TestEventData>(payload);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("123");
        result.Name.Should().Be("test");
    }

    [Fact]
    public async Task DeserializeEventAsync_ShouldThrowArgumentException_WhenPayloadIsEmpty()
    {
        // Act & Assert
        await _webhooksService.Invoking(s => s.DeserializeEventAsync<TestEventData>(""))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("Payload cannot be null or empty*");
    }

    #endregion

    #region Subscription Testing Tests

    [Fact]
    public async Task TestSubscriptionAsync_ShouldReturnSuccessResult_WhenTestSucceeds()
    {
        // Arrange
        var response = new { success = true, message = "Test successful" };
        _mockApiConnection.SetupMutationResponse("POST", "/webhooks/v2/subscriptions/sub123/test", response);

        // Act
        var result = await _webhooksService.TestSubscriptionAsync("sub123");

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Test webhook delivered successfully");
        result.StatusCode.Should().Be(200);
        result.ResponseTimeMs.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task TestWebhookUrlAsync_ShouldReturnSuccessResult_WhenTestSucceeds()
    {
        // Arrange
        var url = "https://example.com/webhook";
        var secret = "test_secret";
        var response = new { success = true, message = "Test successful" };
        _mockApiConnection.SetupMutationResponse("POST", "/webhooks/v2/test", response);

        // Act
        var result = await _webhooksService.TestWebhookUrlAsync(url, secret);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Test webhook delivered successfully");
        result.StatusCode.Should().Be(200);
        result.ResponseTimeMs.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task TestWebhookUrlAsync_ShouldThrowArgumentException_WhenUrlIsEmpty()
    {
        // Act & Assert
        await _webhooksService.Invoking(s => s.TestWebhookUrlAsync("", "secret"))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("URL cannot be null or empty*");
    }

    #endregion

    #region Available Events Tests

    [Fact]
    public async Task GetAvailableEventAsync_ShouldReturnAvailableEvent_WhenApiReturnsData()
    {
        // Arrange
        var eventDto = _builder.CreateAvailableEventDto(e => e.Id = "event123");
        var response = new JsonApiSingleResponse<dynamic> { Data = eventDto };
        _mockApiConnection.SetupGetResponse("/webhooks/v2/available_events/event123", response);

        // Act
        var result = await _webhooksService.GetAvailableEventAsync("event123");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("event123");
        result!.DataSource.Should().Be("Webhooks");
    }

    [Fact]
    public async Task ListAvailableEventsByModuleAsync_ShouldThrowArgumentException_WhenModuleIsEmpty()
    {
        // Act & Assert
        await _webhooksService.Invoking(s => s.ListAvailableEventsByModuleAsync(""))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("Module cannot be null or empty*");
    }

    #endregion

    #region Event History Tests

    [Fact]
    public async Task GetEventAsync_ShouldReturnEvent_WhenApiReturnsData()
    {
        // Arrange
        var eventDto = _builder.CreateWebhookEventDto(e => e.Id = "event123");
        var response = new JsonApiSingleResponse<WebhookEventDto> { Data = eventDto };
        _mockApiConnection.SetupGetResponse("/webhooks/v2/events/event123", response);

        // Act
        var result = await _webhooksService.GetEventAsync("event123");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("event123");
        result!.DataSource.Should().Be("Webhooks");
    }

    [Fact]
    public async Task RedeliverEventAsync_ShouldReturnRedeliveredEvent_WhenIdIsValid()
    {
        // Arrange
        var eventDto = _builder.CreateWebhookEventDto(e => e.Id = "event123");
        var response = new JsonApiSingleResponse<WebhookEventDto> { Data = eventDto };
        _mockApiConnection.SetupMutationResponse("POST", "/webhooks/v2/events/event123/redeliver", response);

        // Act
        var result = await _webhooksService.RedeliverEventAsync("event123");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("event123");
        result!.DataSource.Should().Be("Webhooks");
    }

    #endregion

    #region Bulk Operations Tests

    [Fact]
    public async Task CreateBulkSubscriptionsAsync_ShouldReturnCreatedSubscriptions_WhenRequestIsValid()
    {
        // Arrange
        var request = new BulkWebhookSubscriptionCreateRequest
        {
            Subscriptions = new List<WebhookSubscriptionCreateRequest>
            {
                new() { Url = "https://example1.com/webhook", Secret = "secret1", AvailableEventId = "event1" },
                new() { Url = "https://example2.com/webhook", Secret = "secret2", AvailableEventId = "event2" }
            }
        };

        // Setup individual subscription creation responses
        var sub1Response = new JsonApiSingleResponse<WebhookSubscriptionDto> 
        { 
            Data = _builder.CreateWebhookSubscriptionDto(s => s.Id = "sub1") 
        };
        var sub2Response = new JsonApiSingleResponse<WebhookSubscriptionDto> 
        { 
            Data = _builder.CreateWebhookSubscriptionDto(s => s.Id = "sub2") 
        };

        _mockApiConnection.SetupMutationResponse("POST", "/webhooks/v2/subscriptions", sub1Response);
        _mockApiConnection.SetupMutationResponse("POST", "/webhooks/v2/subscriptions", sub2Response);

        // Act
        var result = await _webhooksService.CreateBulkSubscriptionsAsync(request);

        // Assert
        result.Should().NotBeNull();
        result!.Data.Should().HaveCount(2);
        result.Meta.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task CreateBulkSubscriptionsAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Act & Assert
        await _webhooksService.Invoking(s => s.CreateBulkSubscriptionsAsync(null!))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task DeleteBulkSubscriptionsAsync_ShouldCompleteSuccessfully_WhenRequestIsValid()
    {
        // Arrange
        var request = new BulkWebhookSubscriptionDeleteRequest
        {
            SubscriptionIds = new List<string> { "sub1", "sub2", "sub3" }
        };

        // Act & Assert
        await _webhooksService.Invoking(s => s.DeleteBulkSubscriptionsAsync(request))
            .Should().NotThrowAsync();
    }

    #endregion

    #region Analytics Tests

    [Fact]
    public async Task GenerateDeliveryReportAsync_ShouldReturnReport_WhenRequestIsValid()
    {
        // Arrange
        var request = new WebhookReportRequest
        {
            StartDate = new DateTime(2024, 1, 1),
            EndDate = new DateTime(2024, 1, 31),
            IncludeEventDetails = false
        };

        var subscriptionsResponse = _builder.BuildWebhookSubscriptionCollectionResponse(2);
        _mockApiConnection.SetupGetResponse("/webhooks/v2/subscriptions", subscriptionsResponse);

        // Act
        var result = await _webhooksService.GenerateDeliveryReportAsync(request);

        // Assert
        result.Should().NotBeNull();
        result!.TotalSubscriptions.Should().Be(2);
        result.PeriodStart.Should().Be(request.StartDate);
        result.PeriodEnd.Should().Be(request.EndDate);
        result.GeneratedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    #endregion

    #region Pagination Helper Tests

    #endregion

    // Helper class for testing event deserialization
    private class TestEventData
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}