using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Fluent;

namespace PlanningCenter.Api.Client.Fluent;

/// <summary>
/// Main fluent API client for Planning Center.
/// Provides access to all module-specific fluent contexts.
/// </summary>
public class PlanningCenterFluentClient : IPlanningCenterFluentClient
{
    private readonly IPlanningCenterClient _client;

    public PlanningCenterFluentClient(IPlanningCenterClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    /// <summary>
    /// Gets the fluent API context for the People module.
    /// </summary>
    public IPeopleFluentContext People => _client.People();

    /// <summary>
    /// Gets the fluent API context for the Giving module.
    /// </summary>
    public IGivingFluentContext Giving => _client.Giving();

    /// <summary>
    /// Gets the fluent API context for the Calendar module.
    /// </summary>
    public ICalendarFluentContext Calendar => _client.Calendar();

    /// <summary>
    /// Gets the fluent API context for the Check-Ins module.
    /// </summary>
    public ICheckInsFluentContext CheckIns => _client.CheckIns();

    /// <summary>
    /// Gets the fluent API context for the Groups module.
    /// </summary>
    public IGroupsFluentContext Groups => _client.Groups();

    /// <summary>
    /// Gets the fluent API context for the Registrations module.
    /// </summary>
    public IRegistrationsFluentContext Registrations => _client.Registrations();

    /// <summary>
    /// Gets the fluent API context for the Publishing module.
    /// </summary>
    public IPublishingFluentContext Publishing => _client.Publishing();

    /// <summary>
    /// Gets the fluent API context for the Services module.
    /// </summary>
    public IServicesFluentContext Services => _client.Services();

    /// <summary>
    /// Gets the fluent API context for the Webhooks module.
    /// </summary>
    public IWebhooksFluentContext Webhooks => _client.Webhooks();
}

/// <summary>
/// Interface for the main fluent API client.
/// </summary>
public interface IPlanningCenterFluentClient
{
    /// <summary>
    /// Gets the fluent API context for the People module.
    /// </summary>
    IPeopleFluentContext People { get; }

    /// <summary>
    /// Gets the fluent API context for the Giving module.
    /// </summary>
    IGivingFluentContext Giving { get; }

    /// <summary>
    /// Gets the fluent API context for the Calendar module.
    /// </summary>
    ICalendarFluentContext Calendar { get; }

    /// <summary>
    /// Gets the fluent API context for the Check-Ins module.
    /// </summary>
    ICheckInsFluentContext CheckIns { get; }

    /// <summary>
    /// Gets the fluent API context for the Groups module.
    /// </summary>
    IGroupsFluentContext Groups { get; }

    /// <summary>
    /// Gets the fluent API context for the Registrations module.
    /// </summary>
    IRegistrationsFluentContext Registrations { get; }

    /// <summary>
    /// Gets the fluent API context for the Publishing module.
    /// </summary>
    IPublishingFluentContext Publishing { get; }

    /// <summary>
    /// Gets the fluent API context for the Services module.
    /// </summary>
    IServicesFluentContext Services { get; }

    /// <summary>
    /// Gets the fluent API context for the Webhooks module.
    /// </summary>
    IWebhooksFluentContext Webhooks { get; }
}

// Placeholder implementations for other modules
// These will be implemented as we expand the fluent API

internal class GivingFluentContext : IGivingFluentContext
{
    private readonly IGivingService _givingService;

    public GivingFluentContext(IGivingService givingService)
    {
        _givingService = givingService ?? throw new ArgumentNullException(nameof(givingService));
    }

    // TODO: Implement fluent API for Giving module
}

internal class CalendarFluentContext : ICalendarFluentContext
{
    private readonly ICalendarService _calendarService;

    public CalendarFluentContext(ICalendarService calendarService)
    {
        _calendarService = calendarService ?? throw new ArgumentNullException(nameof(calendarService));
    }

    // TODO: Implement fluent API for Calendar module
}

internal class CheckInsFluentContext : ICheckInsFluentContext
{
    private readonly ICheckInsService _checkInsService;

    public CheckInsFluentContext(ICheckInsService checkInsService)
    {
        _checkInsService = checkInsService ?? throw new ArgumentNullException(nameof(checkInsService));
    }

    // TODO: Implement fluent API for CheckIns module
}

internal class GroupsFluentContext : IGroupsFluentContext
{
    private readonly IGroupsService _groupsService;

    public GroupsFluentContext(IGroupsService groupsService)
    {
        _groupsService = groupsService ?? throw new ArgumentNullException(nameof(groupsService));
    }

    // TODO: Implement fluent API for Groups module
}

internal class RegistrationsFluentContext : IRegistrationsFluentContext
{
    private readonly IRegistrationsService _registrationsService;

    public RegistrationsFluentContext(IRegistrationsService registrationsService)
    {
        _registrationsService = registrationsService ?? throw new ArgumentNullException(nameof(registrationsService));
    }

    // TODO: Implement fluent API for Registrations module
}

internal class PublishingFluentContext : IPublishingFluentContext
{
    private readonly IPublishingService _publishingService;

    public PublishingFluentContext(IPublishingService publishingService)
    {
        _publishingService = publishingService ?? throw new ArgumentNullException(nameof(publishingService));
    }

    // TODO: Implement fluent API for Publishing module
}

internal class ServicesFluentContext : IServicesFluentContext
{
    private readonly IServicesService _servicesService;

    public ServicesFluentContext(IServicesService servicesService)
    {
        _servicesService = servicesService ?? throw new ArgumentNullException(nameof(servicesService));
    }

    // TODO: Implement fluent API for Services module
}

internal class WebhooksFluentContext : IWebhooksFluentContext
{
    private readonly IWebhooksService _webhooksService;

    public WebhooksFluentContext(IWebhooksService webhooksService)
    {
        _webhooksService = webhooksService ?? throw new ArgumentNullException(nameof(webhooksService));
    }

    // TODO: Implement fluent API for Webhooks module
}