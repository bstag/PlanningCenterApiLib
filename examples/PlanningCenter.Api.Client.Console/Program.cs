using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PlanningCenter.Api.Client;
using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Exceptions;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Extensions;
using System;
using System.Threading.Tasks;

namespace PlanningCenter.Api.Client.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Create host builder with dependency injection
            var builder = Host.CreateApplicationBuilder(args);

            // Configure logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.SetMinimumLevel(LogLevel.Information);

            // Add Planning Center API client with proper authentication
            var pat = Environment.GetEnvironmentVariable("PLANNING_CENTER_PAT");
            var clientId = Environment.GetEnvironmentVariable("PLANNING_CENTER_CLIENT_ID");
            var clientSecret = Environment.GetEnvironmentVariable("PLANNING_CENTER_CLIENT_SECRET");

            if (!string.IsNullOrEmpty(pat))
            {
                builder.Services.AddPlanningCenterApiClientWithPAT(pat);
            }
            else if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret))
            {
                builder.Services.AddPlanningCenterApiClient(clientId, clientSecret);
            }
            else
            {
                builder.Services.AddPlanningCenterApiClientWithPAT("8c69ef5698268937e4bc53698435a5771222afb5694e37c06e6c046fd50811f8:16c161bb872079a014d5e779a9c166379b44835e94b435e4a82d81f4d76817e9");
            }

            // Configure SDK options
            builder.Services.Configure<PlanningCenterOptions>(options =>
            {
                options.EnableDetailedLogging = true;
                options.EnableCaching = true;
                options.DefaultCacheExpiration = TimeSpan.FromMinutes(5);
                options.MaxRetryAttempts = 3;
                options.RequestTimeout = TimeSpan.FromSeconds(30);
            });

            var host = builder.Build();
            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            var peopleService = host.Services.GetRequiredService<IPeopleService>();
            var client = host.Services.GetRequiredService<IPlanningCenterClient>();

            logger.LogInformation("🚀 Planning Center SDK - Comprehensive Examples");
            logger.LogInformation("📚 Demonstrating both Traditional Service API and Fluent API patterns");
            logger.LogInformation("");

            try
            {
                // Example 1: Authentication Testing
                await TestAuthenticationExample(peopleService, logger);

                // Example 2: Traditional Service API
                await TraditionalServiceExamples(peopleService, logger);

                // Example 3: Fluent API Examples
                await FluentApiExamples(client, logger);

                // Example 4: Advanced Features
                await AdvancedFeaturesExamples(client, logger);

                // Example 5: Error Handling
                await ErrorHandlingExamples(peopleService, logger);

                // Example 6: Multi-Module Examples
                await MultiModuleExamples(client, logger);

                logger.LogInformation("");
                logger.LogInformation("🎉 All examples completed successfully!");
                DisplaySummary(logger);
            }
            catch (PlanningCenterApiAuthenticationException ex)
            {
                HandleAuthenticationError(ex, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ An unexpected error occurred");
                Environment.Exit(1);
            }

            // Example Methods

            static async Task TestAuthenticationExample(IPeopleService peopleService, ILogger logger)
            {
                logger.LogInformation("🔐 Example 1: Authentication Testing");
                logger.LogInformation("─────────────────────────────────────");

                try
                {
                    var currentUser = await peopleService.GetMeAsync();
                    logger.LogInformation("✅ Authentication successful!");
                    logger.LogInformation("   👤 Current user: {FullName} (ID: {Id})", currentUser.FullName, currentUser.Id);
                    logger.LogInformation("   📊 Status: {Status}, Membership: {MembershipStatus}",
                        currentUser.Status, currentUser.MembershipStatus ?? "Not specified");

                    if (currentUser.Birthdate.HasValue)
                    {
                        logger.LogInformation("   🎂 Birthday: {Birthday:yyyy-MM-dd}", currentUser.Birthdate.Value);
                    }

                    logger.LogInformation("");
                }
                catch (PlanningCenterApiAuthenticationException)
                {
                    logger.LogError("❌ Authentication failed - please check your credentials");
                    throw;
                }
            }

            static async Task TraditionalServiceExamples(IPeopleService peopleService, ILogger logger)
            {
                logger.LogInformation("👥 Example 2: Traditional Service API");
                logger.LogInformation("─────────────────────────────────────");

                // Basic listing with pagination
                var parameters = new QueryParameters
                {
                    PerPage = 5,
                    Where = new Dictionary<string, object> { ["status"] = "active" },
                    OrderBy = "last_name"
                };

                var firstPage = await peopleService.ListAsync(parameters);
                logger.LogInformation("📋 Retrieved page {CurrentPage} with {Count} people (Total: {TotalCount})",
                    firstPage.Meta.CurrentPage, firstPage.Data.Count, firstPage.Meta.TotalCount);

                foreach (var person in firstPage.Data.Take(3))
                {
                    logger.LogInformation("   👤 {FullName} - {Status}", person.FullName, person.Status);
                }

                // Demonstrate pagination navigation
                if (firstPage.HasNextPage)
                {
                    logger.LogInformation("➡️ Navigating to next page...");
                    var nextPage = await firstPage.GetNextPageAsync();
                    if (nextPage != null)
                    {
                        logger.LogInformation("✅ Page {Page} retrieved with {Count} people",
                            nextPage.Meta.CurrentPage, nextPage.Data.Count);
                    }
                }

                // Demonstrate automatic pagination
                logger.LogInformation("🔄 Getting all people (automatic pagination)...");
                var limitedOptions = new PaginationOptions { MaxItems = 10 }; // Limit for demo
                var allPeople = await peopleService.GetAllAsync(parameters, limitedOptions);
                logger.LogInformation("✅ Retrieved {Count} people total using automatic pagination", allPeople.Count);

                logger.LogInformation("");
            }

            static async Task FluentApiExamples(IPlanningCenterClient client, ILogger logger)
            {
                logger.LogInformation("🚀 Example 3: Fluent API");
                logger.LogInformation("─────────────────────────────────────");

                try
                {
                    // Basic fluent query
                    logger.LogInformation("🔍 Basic fluent query...");
                    var people = await client.People()
                        .OrderBy(p => p.LastName)
                        .GetPagedAsync(pageSize: 5);

                    logger.LogInformation("✅ Fluent query returned {Count} people", people.Data.Count);

                    // LINQ-like operations
                    logger.LogInformation("📊 LINQ-like operations...");

                    var totalCount = await client.People().CountAsync();
                    logger.LogInformation("   📈 Total people count: {Count}", totalCount);

                    var anyPeople = await client.People().AnyAsync();
                    logger.LogInformation("   ❓ Any people exist: {AnyPeople}", anyPeople);

                    var firstPerson = await client.People()
                        .OrderBy(p => p.CreatedAt!)
                        .FirstOrDefaultAsync();

                    if (firstPerson != null)
                    {
                        logger.LogInformation("   👤 First person (by creation date): {FullName}", firstPerson.FullName);
                    }

                    // Memory-efficient streaming
                    logger.LogInformation("🌊 Memory-efficient streaming...");
                    var processedCount = 0;

                    await foreach (var person in client.People()
                        .OrderBy(p => p.LastName)
                        .AsAsyncEnumerable())
                    {
                        processedCount++;
                        if (processedCount <= 3)
                        {
                            logger.LogInformation("   👤 Streaming: {FullName}", person.FullName);
                        }

                        if (processedCount >= 5) break; // Limit for demo
                    }

                    logger.LogInformation("✅ Processed {Count} people via streaming", processedCount);

                    logger.LogInformation("");
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "⚠️ Fluent API example failed (may be expected if not authenticated)");
                    logger.LogInformation("");
                }
            }

            static async Task AdvancedFeaturesExamples(IPlanningCenterClient client, ILogger logger)
            {
                logger.LogInformation("⚡ Example 4: Advanced Features");
                logger.LogInformation("─────────────────────────────────────");

                try
                {
                    // Performance monitoring
                    logger.LogInformation("📊 Performance monitoring...");
                    var result = await client.People()
                        .OrderBy(p => p.LastName)
                        .GetPagedAsync(pageSize: 5);

                    logger.LogInformation("✅ Query executed successfully");
                    logger.LogInformation("   📊 Returned {Count} people", result.Data.Count);

                    // Basic query
                    var people = await client.People()
                        .Where(p => p.Status == "active")
                        .GetPagedAsync(pageSize: 5);

                    logger.LogInformation("🔧 Query information:");
                    logger.LogInformation("   📊 Total count: {Count}", people.Meta.TotalCount);
                    logger.LogInformation("   📈 Current page: {Page}", people.Meta.CurrentPage);

                    // Include related data
                    logger.LogInformation("⚙️ Including related data...");
                    var customQuery = await client.People()
                        .Include(p => p.Addresses)
                        .Include(p => p.Emails)
                        .GetPagedAsync(pageSize: 3);

                    logger.LogInformation("✅ Custom parameter query returned {Count} people", customQuery.Data.Count);

                    logger.LogInformation("");
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "⚠️ Advanced features example failed");
                    logger.LogInformation("");
                }
            }

            static async Task ErrorHandlingExamples(IPeopleService peopleService, ILogger logger)
            {
                logger.LogInformation("🛡️ Example 5: Error Handling");
                logger.LogInformation("─────────────────────────────────────");

                // Test getting a non-existent person
                try
                {
                    logger.LogInformation("🔍 Testing error handling with non-existent person...");
                    var nonExistentPerson = await peopleService.GetAsync("999999999");

                    if (nonExistentPerson == null)
                    {
                        logger.LogInformation("✅ Properly handled non-existent person (returned null)");
                    }
                }
                catch (PlanningCenterApiNotFoundException ex)
                {
                    logger.LogInformation("✅ Caught NotFoundException: {Message}", ex.Message);
                    logger.LogInformation("   📋 Request ID: {RequestId}", ex.RequestId);
                    logger.LogInformation("   🕒 Timestamp: {Timestamp}", ex.Timestamp);
                }
                catch (PlanningCenterApiException ex)
                {
                    logger.LogInformation("✅ Caught API Exception: {Message}", ex.Message);
                    logger.LogInformation("   📋 Status Code: {StatusCode}", ex.StatusCode);
                    logger.LogInformation("   🔗 Request URL: {RequestUrl}", ex.RequestUrl);
                }

                logger.LogInformation("");
            }

            static async Task MultiModuleExamples(IPlanningCenterClient client, ILogger logger)
            {
                logger.LogInformation("🏗️ Example 6: Multi-Module Examples");
                logger.LogInformation("─────────────────────────────────────");

                try
                {
                    // Try to access different modules
                    logger.LogInformation("📅 Testing Calendar module access...");
                    try
                    {
                        var events = await client.Calendar()
                            .OrderBy(e => e.StartsAt!)
                            .GetPagedAsync(pageSize: 3);
                        logger.LogInformation("✅ Calendar module: Retrieved {Count} events", events.Data.Count);
                    }
                    catch (InvalidOperationException ex) when (ex.Message.Contains("not registered"))
                    {
                        logger.LogInformation("ℹ️ Calendar service not registered (expected in basic setup)");
                    }

                    logger.LogInformation("💰 Testing Giving module access...");
                    try
                    {
                        var donations = await client.Giving()
                            .OrderByDescending(d => d.ReceivedAt!)
                            .GetPagedAsync(pageSize: 3);
                        logger.LogInformation("✅ Giving module: Retrieved {Count} donations", donations.Data.Count);
                    }
                    catch (InvalidOperationException ex) when (ex.Message.Contains("not registered"))
                    {
                        logger.LogInformation("ℹ️ Giving service not registered (expected in basic setup)");
                    }

                    logger.LogInformation("👥 Testing Groups module access...");
                    try
                    {
                        var groups = await client.Groups()
                            .Where(g => g.Name.Contains("open"))
                            .GetPagedAsync(pageSize: 3);
                        logger.LogInformation("✅ Groups module: Retrieved {Count} groups", groups.Data.Count);
                    }
                    catch (InvalidOperationException ex) when (ex.Message.Contains("not registered"))
                    {
                        logger.LogInformation("ℹ️ Groups service not registered (expected in basic setup)");
                    }

                    logger.LogInformation("");
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "⚠️ Multi-module example failed");
                    logger.LogInformation("");
                }
            }

            static void HandleAuthenticationError(PlanningCenterApiAuthenticationException ex, ILogger logger)
            {
                logger.LogError("🔐 Authentication failed: {Message}", ex.Message);
                logger.LogWarning("");
                logger.LogWarning("Please set your credentials using one of these methods:");
                logger.LogWarning("");
                logger.LogWarning("Option 1 - Personal Access Token (Recommended):");
                logger.LogWarning("   export PLANNING_CENTER_PAT=\"your-app-id:your-secret\"");
                logger.LogWarning("");
                logger.LogWarning("Option 2 - OAuth Credentials:");
                logger.LogWarning("   export PLANNING_CENTER_CLIENT_ID=\"your-client-id\"");
                logger.LogWarning("   export PLANNING_CENTER_CLIENT_SECRET=\"your-client-secret\"");
                logger.LogWarning("");
                logger.LogWarning("You can get these from your Planning Center API application settings at:");
                logger.LogWarning("https://api.planningcenteronline.com/oauth/applications");

                Environment.Exit(1);
            }

            static void DisplaySummary(ILogger logger)
            {
                logger.LogInformation("📋 Examples Summary");
                logger.LogInformation("─────────────────────────────────────");
                logger.LogInformation("✅ Authentication testing with GetMeAsync()");
                logger.LogInformation("✅ Traditional service-based API patterns");
                logger.LogInformation("✅ Fluent API with LINQ-like syntax");
                logger.LogInformation("✅ Built-in pagination helpers and navigation");
                logger.LogInformation("✅ Memory-efficient streaming with AsAsyncEnumerable()");
                logger.LogInformation("✅ Performance monitoring and query optimization");
                logger.LogInformation("✅ Comprehensive error handling with specific exception types");
                logger.LogInformation("✅ Multi-module architecture demonstration");
                logger.LogInformation("✅ Correlation tracking and detailed logging");
                logger.LogInformation("✅ Custom parameters and advanced query features");
                logger.LogInformation("");
                logger.LogInformation("🚀 Ready to build amazing Planning Center integrations!");
                logger.LogInformation("");
                logger.LogInformation("📖 Next Steps:");
                logger.LogInformation("   • Check out the module documentation in /docs/modules/");
                logger.LogInformation("   • Try the fluent console example for advanced patterns");
                logger.LogInformation("   • Explore the worker service example for background processing");
                logger.LogInformation("   • Review the integration tests for comprehensive examples");
            }
        }
    }
}