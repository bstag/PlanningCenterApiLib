using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PlanningCenter.Api.Client;
using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Services;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.CLI.Utilities;
using System.Text.RegularExpressions;

namespace PlanningCenter.Api.Client.CLI.Services;

/// <summary>
/// Interface for Planning Center client factory
/// </summary>
public interface IPlanningCenterClientFactory
{
    /// <summary>
    /// Creates an authenticated Planning Center client
    /// </summary>
    IPlanningCenterClient CreateClient(string token);

    /// <summary>
    /// Validates the token format
    /// </summary>
    bool ValidateTokenFormat(string token);
}

/// <summary>
/// Factory for creating Planning Center clients with authentication
/// </summary>
public class PlanningCenterClientFactory : IPlanningCenterClientFactory
{
    /// <summary>
    /// Creates a Planning Center client with Personal Access Token authentication
    /// </summary>
    /// <param name="token">The Personal Access Token (format: app-id:secret)</param>
    /// <returns>Configured Planning Center client</returns>
    public IPlanningCenterClient CreateClient(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            throw new ArgumentException("Personal Access Token cannot be null or empty", nameof(token));
        }

        if (!ValidateTokenFormat(token))
        {
            throw new ArgumentException("Invalid Personal Access Token format. Expected format: app-id:secret", nameof(token));
        }

        var parts = token.Split(':');
        var appId = parts[0];
        var secret = parts[1];

        // Create the required dependencies
        var services = new ServiceCollection();
        services.AddPlanningCenterApiClientWithPAT(token);
        var serviceProvider = services.BuildServiceProvider();
        
        var peopleService = serviceProvider.GetRequiredService<IPeopleService>();
        var apiConnection = serviceProvider.GetRequiredService<IApiConnection>();
        
        return new PlanningCenterClient(peopleService, apiConnection);
    }

    /// <summary>
    /// Validates the token format
    /// </summary>
    public bool ValidateTokenFormat(string token)
    {
        if (string.IsNullOrEmpty(token))
            return false;

        var parts = token.Split(':');
        return parts.Length == 2 && !string.IsNullOrEmpty(parts[0]) && !string.IsNullOrEmpty(parts[1]);
    }
}

/// <summary>
/// Extension methods for the Planning Center client factory
/// </summary>
public static class PlanningCenterClientFactoryExtensions
{
    /// <summary>
    /// Gets the People service from the client
    /// </summary>
    public static IPeopleService GetPeopleService(this IPlanningCenterClient client)
    {
        // For now, we'll need to access the service through dependency injection
        // This is a temporary solution until we have direct access to services from the client
        var services = new ServiceCollection();
        services.AddPlanningCenterApiClientWithPAT("temp:temp"); // This will be replaced
        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<IPeopleService>();
    }

    /// <summary>
    /// Gets the Services service from the client
    /// </summary>
    public static IServicesService GetServicesService(this IPlanningCenterClient client)
    {
        var services = new ServiceCollection();
        services.AddPlanningCenterApiClientWithPAT("temp:temp");
        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<IServicesService>();
    }

    /// <summary>
    /// Gets the Registrations service from the client
    /// </summary>
    public static IRegistrationsService GetRegistrationsService(this IPlanningCenterClient client)
    {
        var services = new ServiceCollection();
        services.AddPlanningCenterApiClientWithPAT("temp:temp");
        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<IRegistrationsService>();
    }

    /// <summary>
    /// Gets the Calendar service from the client
    /// </summary>
    public static ICalendarService GetCalendarService(this IPlanningCenterClient client)
    {
        var services = new ServiceCollection();
        services.AddPlanningCenterApiClientWithPAT("temp:temp");
        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<ICalendarService>();
    }

    /// <summary>
    /// Gets the CheckIns service from the client
    /// </summary>
    public static ICheckInsService GetCheckInsService(this IPlanningCenterClient client)
    {
        var services = new ServiceCollection();
        services.AddPlanningCenterApiClientWithPAT("temp:temp");
        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<ICheckInsService>();
    }

    /// <summary>
    /// Gets the Giving service from the client
    /// </summary>
    public static IGivingService GetGivingService(this IPlanningCenterClient client)
    {
        var services = new ServiceCollection();
        services.AddPlanningCenterApiClientWithPAT("temp:temp");
        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<IGivingService>();
    }

    /// <summary>
    /// Gets the Groups service from the client
    /// </summary>
    public static IGroupsService GetGroupsService(this IPlanningCenterClient client)
    {
        var services = new ServiceCollection();
        services.AddPlanningCenterApiClientWithPAT("temp:temp");
        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<IGroupsService>();
    }

    /// <summary>
    /// Gets the Publishing service from the client
    /// </summary>
    public static IPublishingService GetPublishingService(this IPlanningCenterClient client)
    {
        var services = new ServiceCollection();
        services.AddPlanningCenterApiClientWithPAT("temp:temp");
        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<IPublishingService>();
    }

    /// <summary>
    /// Gets the Webhooks service from the client
    /// </summary>
    public static IWebhooksService GetWebhooksService(this IPlanningCenterClient client)
    {
        var services = new ServiceCollection();
        services.AddPlanningCenterApiClientWithPAT("temp:temp");
        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<IWebhooksService>();
    }
}