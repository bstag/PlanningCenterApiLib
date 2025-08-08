using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Services;
using PlanningCenter.Api.Client.Fluent;

namespace PlanningCenter.Api.Client;

/// <summary>
/// Extension methods for configuring Planning Center API client services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Planning Center API client services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <param name="configureOptions">Action to configure the Planning Center options</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddPlanningCenterApiClient(
        this IServiceCollection services,
        Action<PlanningCenterOptions> configureOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));
        
        if (configureOptions == null)
            throw new ArgumentNullException(nameof(configureOptions));

        // Configure options
        services.Configure(configureOptions);

        // Add core infrastructure services
        services.AddHttpClient<ApiConnection>();
        services.AddHttpClient<OAuthAuthenticator>();
        
        // Add memory cache if not already registered
        services.TryAddSingleton<IMemoryCache, MemoryCache>();
        
        // Register the appropriate authenticator based on configuration
        services.AddSingleton<IAuthenticator>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<PlanningCenterOptions>>();
            var logger = serviceProvider.GetRequiredService<ILogger<IAuthenticator>>();
            
            // Priority: PersonalAccessToken > AccessToken (treated as PAT) > OAuth
            if (!string.IsNullOrWhiteSpace(options.Value.PersonalAccessToken))
            {
                var patLogger = serviceProvider.GetRequiredService<ILogger<PersonalAccessTokenAuthenticator>>();
                return new PersonalAccessTokenAuthenticator(options, patLogger);
            }
            else if (!string.IsNullOrWhiteSpace(options.Value.AccessToken))
            {
                // Treat access token like PAT even if OAuth credentials are present
                var accessTokenOptions = Microsoft.Extensions.Options.Options.Create(new PlanningCenterOptions
                {
                    PersonalAccessToken = options.Value.AccessToken,
                    BaseUrl = options.Value.BaseUrl
                });
                var patLogger = serviceProvider.GetRequiredService<ILogger<PersonalAccessTokenAuthenticator>>();
                return new PersonalAccessTokenAuthenticator(accessTokenOptions, patLogger);
            }
            else if (!string.IsNullOrWhiteSpace(options.Value.ClientId) && !string.IsNullOrWhiteSpace(options.Value.ClientSecret))
            {
                var httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(OAuthAuthenticator));
                var oauthLogger = serviceProvider.GetRequiredService<ILogger<OAuthAuthenticator>>();
                return new OAuthAuthenticator(httpClient, options, oauthLogger);
            }
            else
            {
                throw new InvalidOperationException("No valid authentication method configured. Please provide PersonalAccessToken, OAuth credentials, or AccessToken.");
            }
        });
        services.AddSingleton<ICacheProvider, InMemoryCacheProvider>();
        services.AddScoped<IApiConnection, ApiConnection>();
        
        // Register module services
        services.AddScoped<IPeopleService, PeopleService>();
        services.AddScoped<IGivingService, GivingService>();
        services.AddScoped<ICalendarService, CalendarService>();
        services.AddScoped<ICheckInsService, CheckInsService>();
        services.AddScoped<IGroupsService, GroupsService>();
        services.AddScoped<IRegistrationsService, RegistrationsService>();
        services.AddScoped<IServicesService, ServicesService>();
        services.AddScoped<IPublishingService, PublishingService>();
        services.AddScoped<IWebhooksService, WebhooksService>();
        
        // Register main client
        services.AddScoped<IPlanningCenterClient, PlanningCenterClient>();
        
        // Register fluent client
        services.AddScoped<IPlanningCenterFluentClient, PlanningCenterFluentClient>();

        return services;
    }

    /// <summary>
    /// Adds Planning Center API client services with default configuration.
    /// You must still configure authentication credentials separately.
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddPlanningCenterApiClient(this IServiceCollection services)
    {
        return services.AddPlanningCenterApiClient(options =>
        {
            // Default configuration - authentication must be configured separately
        });
    }

    /// <summary>
    /// Adds Planning Center API client services with OAuth client credentials.
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <param name="clientId">OAuth client ID</param>
    /// <param name="clientSecret">OAuth client secret</param>
    /// <param name="baseUrl">Optional custom base URL (defaults to official API)</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddPlanningCenterApiClient(
        this IServiceCollection services,
        string clientId,
        string clientSecret,
        string? baseUrl = null)
    {
        if (string.IsNullOrWhiteSpace(clientId))
            throw new ArgumentException("Client ID cannot be null or empty", nameof(clientId));
        
        if (string.IsNullOrWhiteSpace(clientSecret))
            throw new ArgumentException("Client secret cannot be null or empty", nameof(clientSecret));

        return services.AddPlanningCenterApiClient(options =>
        {
            options.ClientId = clientId;
            options.ClientSecret = clientSecret;
            
            if (!string.IsNullOrWhiteSpace(baseUrl))
            {
                options.BaseUrl = baseUrl;
            }
        });
    }

    /// <summary>
    /// Adds Planning Center API client services with an access token.
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <param name="accessToken">Access token for API requests</param>
    /// <param name="baseUrl">Optional custom base URL (defaults to official API)</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddPlanningCenterApiClientWithToken(
        this IServiceCollection services,
        string accessToken,
        string? baseUrl = null)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
            throw new ArgumentException("Access token cannot be null or empty", nameof(accessToken));

        return services.AddPlanningCenterApiClient(options =>
        {
            options.AccessToken = accessToken;
            
            if (!string.IsNullOrWhiteSpace(baseUrl))
            {
                options.BaseUrl = baseUrl;
            }
        });
    }

    /// <summary>
    /// Adds Planning Center API client services with a Personal Access Token (PAT).
    /// This is the recommended method for personal scripts and server-side applications.
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <param name="personalAccessToken">Personal Access Token in the format "app_id:secret"</param>
    /// <param name="baseUrl">Optional custom base URL (defaults to official API)</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddPlanningCenterApiClientWithPAT(
        this IServiceCollection services,
        string personalAccessToken,
        string? baseUrl = null)
    {
        if (string.IsNullOrWhiteSpace(personalAccessToken))
            throw new ArgumentException("Personal Access Token cannot be null or empty", nameof(personalAccessToken));

        return services.AddPlanningCenterApiClient(options =>
        {
            options.PersonalAccessToken = personalAccessToken;
            
            if (!string.IsNullOrWhiteSpace(baseUrl))
            {
                options.BaseUrl = baseUrl;
            }
        });
    }

    /// <summary>
    /// Adds Planning Center API client services. This is an alias for AddPlanningCenterApiClient.
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <param name="configureOptions">Action to configure the Planning Center options</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddPlanningCenterApi(
        this IServiceCollection services,
        Action<PlanningCenterOptions> configureOptions)
    {
        return services.AddPlanningCenterApiClient(configureOptions);
    }
}