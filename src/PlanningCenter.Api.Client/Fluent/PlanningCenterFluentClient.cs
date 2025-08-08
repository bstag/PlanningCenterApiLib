using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;


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


// RegistrationsFluentContext, PublishingFluentContext, and WebhooksFluentContext are now implemented as separate class files