using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PlanningCenter.Api.Client;
using PlanningCenter.Api.Client.CLI.Commands;
using PlanningCenter.Api.Client.CLI.Configuration;
using PlanningCenter.Api.Client.CLI.Services;
using PlanningCenter.Api.Client.CLI.Formatters;

namespace PlanningCenter.Api.Client.CLI;

class Program
{
    static async Task<int> Main(string[] args)
    {
        // Build configuration
        var userConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".planningcenter", "config.json");
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile(userConfigPath, optional: true)
            .AddEnvironmentVariables()
            .Build();

        // Setup dependency injection
        var services = new ServiceCollection();
        ConfigureServices(services, configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Create root command
        var rootCommand = new RootCommand("Planning Center CLI - Command line interface for Planning Center API")
        {
            Name = "pc-cli"
        };

        // Add global options
        var tokenOption = new Option<string?>(
            aliases: new[] { "--token", "-t" },
            description: "Personal Access Token (format: app-id:secret)");
        
        var formatOption = new Option<OutputFormat>(
            aliases: new[] { "--format", "-f" },
            getDefaultValue: () => OutputFormat.Json,
            description: "Output format (table, json, csv, xml)");
        
        var verboseOption = new Option<bool>(
            aliases: new[] { "--verbose", "-v" },
            description: "Enable verbose logging");
        
        var detailedLoggingOption = new Option<bool>(
            aliases: new[] { "--detailed-logging", "-d" },
            description: "Enable detailed API request/response logging");

        rootCommand.AddGlobalOption(tokenOption);
        rootCommand.AddGlobalOption(formatOption);
        rootCommand.AddGlobalOption(verboseOption);
        rootCommand.AddGlobalOption(detailedLoggingOption);

        // Add module commands
        var peopleCommand = serviceProvider.GetRequiredService<PeopleCommand>();
        rootCommand.AddCommand(peopleCommand);
        
        var servicesCommand = serviceProvider.GetRequiredService<ServicesCommand>();
        rootCommand.AddCommand(servicesCommand);
        
        var registrationsCommand = serviceProvider.GetRequiredService<RegistrationsCommand>();
        rootCommand.AddCommand(registrationsCommand);
        
        var calendarCommand = serviceProvider.GetRequiredService<CalendarCommand>();
        rootCommand.AddCommand(calendarCommand);
        
        var checkInsCommand = serviceProvider.GetRequiredService<CheckInsCommand>();
        rootCommand.AddCommand(checkInsCommand);
        
        var givingCommand = serviceProvider.GetRequiredService<GivingCommand>();
        rootCommand.AddCommand(givingCommand);
        
        var groupsCommand = serviceProvider.GetRequiredService<GroupsCommand>();
        rootCommand.AddCommand(groupsCommand);
        
        var publishingCommand = serviceProvider.GetRequiredService<PublishingCommand>();
        rootCommand.AddCommand(publishingCommand);
        
        var webhooksCommand = serviceProvider.GetRequiredService<WebhooksCommand>();
        rootCommand.AddCommand(webhooksCommand);

        // Add configuration commands
        var configCommand = serviceProvider.GetRequiredService<ConfigCommand>();
        rootCommand.AddCommand(configCommand);

        // Execute command
        return await rootCommand.InvokeAsync(args);
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Configuration
        services.AddSingleton(configuration);
        services.AddSingleton<CliConfiguration>();

        // Logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // Planning Center API Client - Use SDK's built-in DI registration
        // We'll configure this with a temporary token that will be overridden at runtime
        services.AddPlanningCenterApiClient(options =>
        {
            options.PersonalAccessToken = "temp:temp";
            options.EnableDetailedLogging = configuration.GetValue<bool>("PlanningCenter:EnableDetailedLogging", false);
        });

        // Formatters
        services.AddSingleton<IOutputFormatter, TableFormatter>();
        services.AddSingleton<IOutputFormatter, JsonFormatter>();
        services.AddSingleton<IOutputFormatter, CsvFormatter>();
        services.AddSingleton<IOutputFormatter, XmlFormatter>();

        // Commands
        services.AddTransient<PeopleCommand>();
        services.AddTransient<ServicesCommand>();
        services.AddTransient<RegistrationsCommand>();
        services.AddTransient<CalendarCommand>();
        services.AddTransient<CheckInsCommand>();
        services.AddTransient<GivingCommand>();
        services.AddTransient<GroupsCommand>();
        services.AddTransient<PublishingCommand>();
        services.AddTransient<WebhooksCommand>();
        services.AddTransient<ConfigCommand>(provider => new ConfigCommand(
            provider.GetRequiredService<ILogger<ConfigCommand>>(),
            provider.GetRequiredService<IAuthenticationService>(),
            provider.GetRequiredService<IPlanningCenterClientFactory>(),
            provider,
            provider.GetRequiredService<IEnumerable<IOutputFormatter>>(),
            provider.GetRequiredService<CliConfiguration>()));

        // Services
        services.AddTransient<IAuthenticationService, AuthenticationService>();
        services.AddTransient<IPlanningCenterClientFactory, PlanningCenterClientFactory>();
    }
}