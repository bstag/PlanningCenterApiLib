using FluentAssertions;
using Moq;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.Exceptions;
using PlanningCenter.Api.Client.Models.Fluent;
using PlanningCenter.Api.Client.Services;
using Xunit;

namespace PlanningCenter.Api.Client.Tests;

/// <summary>
/// Unit tests for PlanningCenterClient - the main client orchestration component.
/// This is critical infrastructure that coordinates access to all modules.
/// </summary>
public class PlanningCenterClientTests
{
    private readonly Mock<IPeopleService> _mockPeopleService;
    private readonly Mock<IApiConnection> _mockApiConnection;
    private readonly Mock<IGivingService> _mockGivingService;
    private readonly Mock<ICalendarService> _mockCalendarService;
    private readonly Mock<ICheckInsService> _mockCheckInsService;
    private readonly Mock<IGroupsService> _mockGroupsService;
    private readonly Mock<IRegistrationsService> _mockRegistrationsService;
    private readonly Mock<IPublishingService> _mockPublishingService;
    private readonly Mock<IServicesService> _mockServicesService;
    private readonly Mock<IWebhooksService> _mockWebhooksService;

    public PlanningCenterClientTests()
    {
        _mockPeopleService = new Mock<IPeopleService>();
        _mockApiConnection = new Mock<IApiConnection>();
        _mockGivingService = new Mock<IGivingService>();
        _mockCalendarService = new Mock<ICalendarService>();
        _mockCheckInsService = new Mock<ICheckInsService>();
        _mockGroupsService = new Mock<IGroupsService>();
        _mockRegistrationsService = new Mock<IRegistrationsService>();
        _mockPublishingService = new Mock<IPublishingService>();
        _mockServicesService = new Mock<IServicesService>();
        _mockWebhooksService = new Mock<IWebhooksService>();
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithRequiredServices_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var client = new PlanningCenterClient(_mockPeopleService.Object, _mockApiConnection.Object);

        // Assert
        client.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithAllServices_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var client = new PlanningCenterClient(
            _mockPeopleService.Object,
            _mockApiConnection.Object,
            _mockGivingService.Object,
            _mockCalendarService.Object,
            _mockCheckInsService.Object,
            _mockGroupsService.Object,
            _mockRegistrationsService.Object,
            _mockPublishingService.Object,
            _mockServicesService.Object,
            _mockWebhooksService.Object);

        // Assert
        client.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullPeopleService_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var act = () => new PlanningCenterClient(null!, _mockApiConnection.Object);
        act.Should().Throw<ArgumentNullException>().WithParameterName("peopleService");
    }

    [Fact]
    public void Constructor_WithNullApiConnection_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var act = () => new PlanningCenterClient(_mockPeopleService.Object, null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("apiConnection");
    }

    #endregion

    #region Fluent Context Tests

    [Fact]
    public void People_ShouldReturnPeopleFluentContext()
    {
        // Arrange
        var client = new PlanningCenterClient(_mockPeopleService.Object, _mockApiConnection.Object);

        // Act
        var result = client.People();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IPeopleFluentContext>();
    }

    [Fact]
    public void Giving_WithRegisteredService_ShouldReturnGivingFluentContext()
    {
        // Arrange
        var client = new PlanningCenterClient(
            _mockPeopleService.Object,
            _mockApiConnection.Object,
            givingService: _mockGivingService.Object);

        // Act
        var result = client.Giving();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IGivingFluentContext>();
    }

    [Fact]
    public void Giving_WithoutRegisteredService_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var client = new PlanningCenterClient(_mockPeopleService.Object, _mockApiConnection.Object);

        // Act & Assert
        var act = () => client.Giving();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Giving service is not registered. Please add the appropriate service to your DI container.");
    }

    [Fact]
    public void Calendar_WithRegisteredService_ShouldReturnCalendarFluentContext()
    {
        // Arrange
        var client = new PlanningCenterClient(
            _mockPeopleService.Object,
            _mockApiConnection.Object,
            calendarService: _mockCalendarService.Object);

        // Act
        var result = client.Calendar();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<ICalendarFluentContext>();
    }

    [Fact]
    public void Calendar_WithoutRegisteredService_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var client = new PlanningCenterClient(_mockPeopleService.Object, _mockApiConnection.Object);

        // Act & Assert
        var act = () => client.Calendar();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Calendar service is not registered. Please add the appropriate service to your DI container.");
    }

    [Fact]
    public void CheckIns_WithRegisteredService_ShouldReturnCheckInsFluentContext()
    {
        // Arrange
        var client = new PlanningCenterClient(
            _mockPeopleService.Object,
            _mockApiConnection.Object,
            checkInsService: _mockCheckInsService.Object);

        // Act
        var result = client.CheckIns();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<ICheckInsFluentContext>();
    }

    [Fact]
    public void CheckIns_WithoutRegisteredService_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var client = new PlanningCenterClient(_mockPeopleService.Object, _mockApiConnection.Object);

        // Act & Assert
        var act = () => client.CheckIns();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("CheckIns service is not registered. Please add the appropriate service to your DI container.");
    }

    [Fact]
    public void Groups_WithRegisteredService_ShouldReturnGroupsFluentContext()
    {
        // Arrange
        var client = new PlanningCenterClient(
            _mockPeopleService.Object,
            _mockApiConnection.Object,
            groupsService: _mockGroupsService.Object);

        // Act
        var result = client.Groups();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IGroupsFluentContext>();
    }

    [Fact]
    public void Groups_WithoutRegisteredService_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var client = new PlanningCenterClient(_mockPeopleService.Object, _mockApiConnection.Object);

        // Act & Assert
        var act = () => client.Groups();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Groups service is not registered. Please add the appropriate service to your DI container.");
    }

    [Fact]
    public void Registrations_WithRegisteredService_ShouldReturnRegistrationsFluentContext()
    {
        // Arrange
        var client = new PlanningCenterClient(
            _mockPeopleService.Object,
            _mockApiConnection.Object,
            registrationsService: _mockRegistrationsService.Object);

        // Act
        var result = client.Registrations();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IRegistrationsFluentContext>();
    }

    [Fact]
    public void Registrations_WithoutRegisteredService_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var client = new PlanningCenterClient(_mockPeopleService.Object, _mockApiConnection.Object);

        // Act & Assert
        var act = () => client.Registrations();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Registrations service is not registered. Please add the appropriate service to your DI container.");
    }

    [Fact]
    public void Publishing_WithRegisteredService_ShouldReturnPublishingFluentContext()
    {
        // Arrange
        var client = new PlanningCenterClient(
            _mockPeopleService.Object,
            _mockApiConnection.Object,
            publishingService: _mockPublishingService.Object);

        // Act
        var result = client.Publishing();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IPublishingFluentContext>();
    }

    [Fact]
    public void Publishing_WithoutRegisteredService_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var client = new PlanningCenterClient(_mockPeopleService.Object, _mockApiConnection.Object);

        // Act & Assert
        var act = () => client.Publishing();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Publishing service is not registered. Please add the appropriate service to your DI container.");
    }

    [Fact]
    public void Services_WithRegisteredService_ShouldReturnServicesFluentContext()
    {
        // Arrange
        var client = new PlanningCenterClient(
            _mockPeopleService.Object,
            _mockApiConnection.Object,
            servicesService: _mockServicesService.Object);

        // Act
        var result = client.Services();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IServicesFluentContext>();
    }

    [Fact]
    public void Services_WithoutRegisteredService_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var client = new PlanningCenterClient(_mockPeopleService.Object, _mockApiConnection.Object);

        // Act & Assert
        var act = () => client.Services();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Services service is not registered. Please add the appropriate service to your DI container.");
    }

    [Fact]
    public void Webhooks_WithRegisteredService_ShouldReturnWebhooksFluentContext()
    {
        // Arrange
        var client = new PlanningCenterClient(
            _mockPeopleService.Object,
            _mockApiConnection.Object,
            webhooksService: _mockWebhooksService.Object);

        // Act
        var result = client.Webhooks();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IWebhooksFluentContext>();
    }

    [Fact]
    public void Webhooks_WithoutRegisteredService_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var client = new PlanningCenterClient(_mockPeopleService.Object, _mockApiConnection.Object);

        // Act & Assert
        var act = () => client.Webhooks();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Webhooks service is not registered. Please add the appropriate service to your DI container.");
    }

    #endregion

    #region Global Operations Tests

    [Fact]
    public async Task CheckHealthAsync_ShouldReturnHealthCheckResult()
    {
        // Arrange
        var client = new PlanningCenterClient(_mockPeopleService.Object, _mockApiConnection.Object);
        var expectedHealthCheck = new HealthCheckResult
        {
            IsHealthy = true,
            ResponseTimeMs = 0.4549,
            ApiVersion = "2.0",
            CheckedAt = DateTime.UtcNow
        };

        _mockApiConnection.Setup(a => a.GetAsync<HealthCheckResult>("/", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedHealthCheck);

        // Act
        var result = await client.CheckHealthAsync();

        // Assert
        result.Should().NotBeNull();
        result.IsHealthy.Should().BeTrue();
        result.ResponseTimeMs.Should().Be(0.4549);
        result.ApiVersion.Should().Be("2.0");
    }

    [Fact]
    public async Task GetApiLimitsAsync_ShouldReturnApiLimitsInfo()
    {
        // Arrange
        var client = new PlanningCenterClient(_mockPeopleService.Object, _mockApiConnection.Object);
        var expectedLimits = new ApiLimitsInfo
        {
            RateLimit = 100,
            RemainingRequests = 75,
            ResetTime = DateTime.UtcNow.AddHours(1),
            TimePeriod = "hour"
        };

        _mockApiConnection.Setup(a => a.GetAsync<ApiLimitsInfo>("/rate_limit", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedLimits);

        // Act
        var result = await client.GetApiLimitsAsync();

        // Assert
        result.Should().NotBeNull();
        result.RateLimit.Should().Be(100);
        result.RemainingRequests.Should().Be(75);
        result.TimePeriod.Should().Be("hour");
        result.UsagePercentage.Should().Be(25); // (100-75)/100 * 100
    }

    [Fact]
    public async Task GetCurrentUserAsync_ShouldReturnCurrentUserInfo()
    {
        // Arrange
        var client = new PlanningCenterClient(_mockPeopleService.Object, _mockApiConnection.Object);
        var currentPerson = new Person
        {
            Id = "12345",
            FirstName = "John",
            LastName = "Doe",
            PrimaryEmail = "john.doe@example.com",
            Nickname = "Johnny",
            Status = "active",
            MembershipStatus = "member",
            CreatedAt = DateTime.UtcNow.AddYears(-2),
            UpdatedAt = DateTime.UtcNow.AddDays(-1),
            DataSource = "People"
        };

        _mockPeopleService.Setup(s => s.GetMeAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentPerson);

        // Act
        var result = await client.GetCurrentUserAsync();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("12345");
        result.Name.Should().Be("John Doe");
        result.Email.Should().Be("john.doe@example.com");
        result.Permissions.Should().Contain("authenticated");
        result.Organizations.Should().Contain("People");
        result.Metadata.Should().ContainKey("first_name");
        result.Metadata["first_name"].Should().Be("John");
        result.Metadata.Should().ContainKey("last_name");
        result.Metadata["last_name"].Should().Be("Doe");
        result.Metadata.Should().ContainKey("nickname");
        result.Metadata["nickname"].Should().Be("Johnny");
        result.Metadata.Should().ContainKey("status");
        result.Metadata["status"].Should().Be("active");
        result.Metadata.Should().ContainKey("membership_status");
        result.Metadata["membership_status"].Should().Be("member");
        result.Metadata.Should().ContainKey("is_child");
        result.Metadata["is_child"].Should().Be(false);
        result.Metadata.Should().ContainKey("is_active");
        result.Metadata["is_active"].Should().Be(true);
        result.Metadata.Should().ContainKey("display_name");
        result.Metadata["display_name"].Should().Be("Johnny");
        result.Metadata.Should().ContainKey("avatar_url");
        result.Metadata["avatar_url"].Should().Be(string.Empty);
    }

    [Fact]
    public async Task GetCurrentUserAsync_WithPeopleServiceException_ShouldReturnFallbackUserInfo()
    {
        // Arrange
        var client = new PlanningCenterClient(_mockPeopleService.Object, _mockApiConnection.Object);
        var expectedException = new PlanningCenterApiAuthenticationException("Authentication failed");

        _mockPeopleService.Setup(s => s.GetMeAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act
        var result = await client.GetCurrentUserAsync();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("unknown");
        result.Name.Should().Be("Unknown User");
        result.Email.Should().Be("unknown@example.com");
        result.Metadata.Should().ContainKey("error");
        result.Metadata["error"].Should().Be("Authentication failed");
        result.Metadata.Should().ContainKey("error_type");
        result.Metadata["error_type"].Should().Be("PlanningCenterApiAuthenticationException");
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public async Task CheckHealthAsync_WithApiException_ShouldPropagateException()
    {
        // Arrange
        var client = new PlanningCenterClient(_mockPeopleService.Object, _mockApiConnection.Object);
        var expectedException = new PlanningCenterApiServerException("Server error");

        _mockApiConnection.Setup(a => a.GetAsync<HealthCheckResult>("/", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HealthCheckResult { IsHealthy = false, Details = { { "error", "Server error" } } });

        // Act & Assert
        var result = await client.CheckHealthAsync();
        result.IsHealthy.Should().BeFalse();
        result.Details["error"].Should().Be("Server error");
    }

    [Fact]
    public async Task GetApiLimitsAsync_WithApiException_ShouldPropagateException()
    {
        // Arrange
        var client = new PlanningCenterClient(_mockPeopleService.Object, _mockApiConnection.Object);
        var expectedException = new PlanningCenterApiAuthenticationException("Auth failed");

        _mockApiConnection.Setup(a => a.GetAsync<ApiLimitsInfo>("/rate_limit", It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var act = async () => await client.GetApiLimitsAsync();
        await act.Should().ThrowAsync<PlanningCenterApiAuthenticationException>();
    }

    #endregion

    #region Cancellation Tests

    [Fact]
    public async Task CheckHealthAsync_WithCancellation_ShouldRespectCancellationToken()
    {
        // Arrange
        var client = new PlanningCenterClient(_mockPeopleService.Object, _mockApiConnection.Object);
        var cts = new CancellationTokenSource();
        cts.Cancel();

        _mockApiConnection.Setup(a => a.GetAsync<HealthCheckResult>("/", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TaskCanceledException());

        // Act & Assert
        var act = async () => await client.CheckHealthAsync(cts.Token);
        await act.Should().ThrowAsync<TaskCanceledException>();
    }

    #endregion
}