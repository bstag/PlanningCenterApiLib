using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PlanningCenter.Api.Client;
using PlanningCenter.Api.Client.Models;
using System;
using System.IO;

namespace PlanningCenter.Api.Client.IntegrationTests.Infrastructure;

/// <summary>
/// Test fixture for integration tests that provides configured services.
/// </summary>
public class TestFixture : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    public IServiceProvider ServiceProvider => _serviceProvider;

    public TestFixture()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.local.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var services = new ServiceCollection();

        // Add logging
        services.AddLogging(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Information);
            builder.AddConsole();
        });

        // Configure PlanningCenter client
        services.AddPlanningCenterApiClient(options =>
        {
            configuration.GetSection("PlanningCenter").Bind(options);
            
            // Override authentication if provided via environment variables
            var appId = Environment.GetEnvironmentVariable("PLANNING_CENTER_APP_ID");
            var secret = Environment.GetEnvironmentVariable("PLANNING_CENTER_SECRET");
            
            if (!string.IsNullOrEmpty(appId) && !string.IsNullOrEmpty(secret))
            {
                // Set personal access token (format: "app_id:secret")
                options.PersonalAccessToken = $"{appId}:{secret}";
            }
        });

        _serviceProvider = services.BuildServiceProvider();
    }

    public void Dispose()
    {
        _serviceProvider?.Dispose();
    }
}
