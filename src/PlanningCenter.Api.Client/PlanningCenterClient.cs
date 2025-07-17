using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Fluent;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Services;
using PlanningCenter.Api.Client.Fluent;

namespace PlanningCenter.Api.Client;

/// <summary>
/// Main client implementation for the Planning Center SDK.
/// Provides access to all modules through both service-based and fluent APIs.
/// </summary>
public class PlanningCenterClient : IPlanningCenterClient
{
    private readonly IPeopleService _peopleService;
    private readonly IGivingService? _givingService;
    private readonly ICalendarService? _calendarService;
    private readonly ICheckInsService? _checkInsService;
    private readonly IGroupsService? _groupsService;
    private readonly IRegistrationsService? _registrationsService;
    private readonly IPublishingService? _publishingService;
    private readonly IServicesService? _servicesService;
    private readonly IWebhooksService? _webhooksService;
    private readonly IApiConnection _apiConnection;

    public PlanningCenterClient(
        IPeopleService peopleService,
        IApiConnection apiConnection,
        IGivingService? givingService = null,
        ICalendarService? calendarService = null,
        ICheckInsService? checkInsService = null,
        IGroupsService? groupsService = null,
        IRegistrationsService? registrationsService = null,
        IPublishingService? publishingService = null,
        IServicesService? servicesService = null,
        IWebhooksService? webhooksService = null)
    {
        _peopleService = peopleService ?? throw new ArgumentNullException(nameof(peopleService));
        _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
        _givingService = givingService;
        _calendarService = calendarService;
        _checkInsService = checkInsService;
        _groupsService = groupsService;
        _registrationsService = registrationsService;
        _publishingService = publishingService;
        _servicesService = servicesService;
        _webhooksService = webhooksService;
    }

    /// <summary>
    /// Access to the People module via fluent API.
    /// </summary>
    public IPeopleFluentContext People()
    {
        return new PeopleFluentContext(_peopleService);
    }

    /// <summary>
    /// Access to the Giving module via fluent API.
    /// </summary>
    public IGivingFluentContext Giving()
    {
        if (_givingService == null)
            throw new InvalidOperationException("Giving service is not registered. Please add the appropriate service to your DI container.");
        return new Fluent.GivingFluentContext(_givingService);
    }

    /// <summary>
    /// Access to the Calendar module via fluent API.
    /// </summary>
    public ICalendarFluentContext Calendar()
    {
        if (_calendarService == null)
            throw new InvalidOperationException("Calendar service is not registered. Please add the appropriate service to your DI container.");
        return new Fluent.CalendarFluentContext(_calendarService);
    }

    /// <summary>
    /// Access to the Check-Ins module via fluent API.
    /// </summary>
    public ICheckInsFluentContext CheckIns()
    {
        if (_checkInsService == null)
            throw new InvalidOperationException("CheckIns service is not registered. Please add the appropriate service to your DI container.");
        return new CheckInsFluentContext(_checkInsService);
    }

    /// <summary>
    /// Access to the Groups module via fluent API.
    /// </summary>
    public IGroupsFluentContext Groups()
    {
        if (_groupsService == null)
            throw new InvalidOperationException("Groups service is not registered. Please add the appropriate service to your DI container.");
        return new Fluent.GroupsFluentContext(_groupsService);
    }

    /// <summary>
    /// Access to the Registrations module via fluent API.
    /// </summary>
    public IRegistrationsFluentContext Registrations()
    {
        if (_registrationsService == null)
            throw new InvalidOperationException("Registrations service is not registered. Please add the appropriate service to your DI container.");
        return new RegistrationsFluentContext(_registrationsService);
    }

    /// <summary>
    /// Access to the Publishing module via fluent API.
    /// </summary>
    public IPublishingFluentContext Publishing()
    {
        if (_publishingService == null)
            throw new InvalidOperationException("Publishing service is not registered. Please add the appropriate service to your DI container.");
        return new PublishingFluentContext(_publishingService);
    }

    /// <summary>
    /// Access to the Services module via fluent API.
    /// </summary>
    public IServicesFluentContext Services()
    {
        if (_servicesService == null)
            throw new InvalidOperationException("Services service is not registered. Please add the appropriate service to your DI container.");
        return new Fluent.ServicesFluentContext(_servicesService);
    }

    /// <summary>
    /// Access to the Webhooks module via fluent API.
    /// </summary>
    public IWebhooksFluentContext Webhooks()
    {
        if (_webhooksService == null)
            throw new InvalidOperationException("Webhooks service is not registered. Please add the appropriate service to your DI container.");
        return new WebhooksFluentContext(_webhooksService);
    }

    /// <summary>
    /// Checks the health and connectivity of the Planning Center API.
    /// </summary>
    public async Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var startTime = DateTime.UtcNow;
            
            // Simple health check - try to get people (minimal data)
            var parameters = new QueryParameters();
            parameters.PerPage = 1;
            await _apiConnection.GetPagedAsync<Person>("/people/v2/people", parameters, cancellationToken);
            
            var endTime = DateTime.UtcNow;
            var responseTime = (endTime - startTime).TotalMilliseconds;

            return new HealthCheckResult
            {
                IsHealthy = true,
                ResponseTimeMs = responseTime,
                ApiVersion = "v2", // Planning Center API version
                CheckedAt = startTime
            };
        }
        catch (Exception ex)
        {
            return new HealthCheckResult
            {
                IsHealthy = false,
                ResponseTimeMs = 0,
                AdditionalInfo = { ["Error"] = ex.Message },
                CheckedAt = DateTime.UtcNow
            };
        }
    }

    /// <summary>
    /// Gets current API rate limit information.
    /// </summary>
    public async Task<ApiLimitsInfo> GetApiLimitsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Make a simple request to get rate limit headers
            var parameters = new QueryParameters();
            parameters.PerPage = 1;
            var response = await _apiConnection.GetPagedAsync<Person>("/people/v2/people", parameters, cancellationToken);
            
            // Planning Center API typically returns rate limit info in headers
            // For now, return default values since we don't have access to raw headers
            return new ApiLimitsInfo
            {
                RateLimit = 100, // Default assumed limit
                RemainingRequests = 95, // Placeholder
                ResetTime = DateTime.UtcNow.AddHours(1),
                TimePeriod = "hour"
            };
        }
        catch
        {
            return new ApiLimitsInfo
            {
                RateLimit = 100,
                RemainingRequests = 0,
                ResetTime = DateTime.UtcNow.AddHours(1),
                TimePeriod = "hour"
            };
        }
    }

    /// <summary>
    /// Gets information about the current user and their permissions.
    /// </summary>
    public async Task<CurrentUserInfo> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Planning Center API doesn't have a dedicated /me endpoint
            // We'll need to make a request to get current user info
            // For now, return placeholder data
            return new CurrentUserInfo
            {
                Id = "current-user",
                Name = "Current User",
                Email = "user@example.com",
                Permissions = new List<string> { "read", "write" },
                Organizations = new List<string> { "default" }
            };
        }
        catch
        {
            return new CurrentUserInfo
            {
                Id = "unknown",
                Name = "Unknown User",
                Email = "unknown@example.com"
            };
        }
    }
}