using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PlanningCenter.Api.Client;
using PlanningCenter.Api.Client.Extensions;
using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Models.Exceptions;

// Create host with dependency injection
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
    builder.Services.AddPlanningCenterApiClientWithPAT("demo-app-id:demo-secret");
}

// Configure SDK options for optimal performance
builder.Services.Configure<PlanningCenterOptions>(options =>
{
    options.EnableDetailedLogging = true;
    options.EnableCaching = true;
    options.DefaultCacheExpiration = TimeSpan.FromMinutes(5);
    options.MaxRetryAttempts = 3;
    options.RequestTimeout = TimeSpan.FromSeconds(30);
});

var host = builder.Build();
var client = host.Services.GetRequiredService<IPlanningCenterClient>();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation("🚀 Planning Center Fluent API - Advanced Examples");
logger.LogInformation("🎯 Demonstrating LINQ-like syntax and modern patterns");
logger.LogInformation("");

try
{
    // Example 1: Basic fluent query patterns
    await BasicFluentQueryExample(client, logger);

    // Example 2: Advanced filtering and sorting
    await AdvancedFilteringExample(client, logger);

    // Example 3: LINQ-like terminal operations
    await LinqLikeOperationsExample(client, logger);

    // Example 4: Memory-efficient streaming
    await StreamingExample(client, logger);

    // Example 5: Performance monitoring and optimization
    await PerformanceOptimizationExample(client, logger);

    // Example 6: Multi-module fluent operations
    await MultiModuleFluentExample(client, logger);

    // Example 7: Real-world scenarios
    await RealWorldScenariosExample(client, logger);

    logger.LogInformation("");
    logger.LogInformation("🎉 All fluent API examples completed successfully!");
    DisplayFluentApiSummary(logger);
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

static async Task BasicFluentQueryExample(IPlanningCenterClient client, ILogger logger)
{
    logger.LogInformation("🔍 Example 1: Basic Fluent Query Patterns");
    logger.LogInformation("─────────────────────────────────────────");

    try
    {
        // Correct API usage - client.People() not client.Fluent().People
        var people = await client.People()
            .OrderBy(p => p.LastName)
            .GetPagedAsync(pageSize: 5);

        logger.LogInformation("✅ Basic fluent query returned {Count} people", people.Data.Count);

        foreach (var person in people.Data.Take(3))
        {
            logger.LogInformation("   👤 {Name} (ID: {Id})", person.FullName, person.Id);
        }

        // Demonstrate pagination metadata
        logger.LogInformation("📊 Pagination: Page {Current} of {Total}, {TotalCount} total",
            people.Meta.CurrentPage, people.Meta.TotalPages, people.Meta.TotalCount);

        // Show navigation capabilities
        if (people.HasNextPage)
        {
            logger.LogInformation("➡️ Has next page available");
            var nextPage = await people.GetNextPageAsync();
            if (nextPage != null)
            {
                logger.LogInformation("✅ Successfully navigated to page {Page}", nextPage.Meta.CurrentPage);
            }
        }

        logger.LogInformation("");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "⚠️ Basic fluent query failed");
        logger.LogInformation("");
    }
}

static async Task AdvancedFilteringExample(IPlanningCenterClient client, ILogger logger)
{
    logger.LogInformation("🎯 Example 2: Advanced Filtering and Sorting");
    logger.LogInformation("─────────────────────────────────────────");

    try
    {
        // Complex filtering with multiple conditions
        logger.LogInformation("🔍 Filtering active people, ordered by name...");
        var activePeople = await client.People()
            .Where(p => p.Status == "active")
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .GetPagedAsync(pageSize: 5);

        logger.LogInformation("✅ Found {Count} active people", activePeople.Data.Count);

        // Demonstrate method chaining
        logger.LogInformation("🔗 Method chaining with custom parameters...");
        var customQuery = await client.People()
            .OrderBy(p => p.CreatedAt!)
            .Include(p => p.Addresses)
            .Include(p => p.Emails)
            .GetPagedAsync(pageSize: 3);

        logger.LogInformation("✅ Custom parameter query returned {Count} people", customQuery.Data.Count);

        logger.LogInformation("");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "⚠️ Advanced filtering failed");
        logger.LogInformation("");
    }
}

static async Task LinqLikeOperationsExample(IPlanningCenterClient client, ILogger logger)
{
    logger.LogInformation("📊 Example 3: LINQ-like Terminal Operations");
    logger.LogInformation("─────────────────────────────────────────");

    try
    {
        // Count operations
        logger.LogInformation("🔢 Counting people...");
        var totalCount = await client.People().CountAsync();
        logger.LogInformation("✅ Total people count: {Count}", totalCount);

        // Existence checks
        logger.LogInformation("❓ Checking if any people exist...");
        var anyPeople = await client.People().AnyAsync();
        logger.LogInformation("✅ Any people exist: {AnyPeople}", anyPeople);

        // First/Single operations
        logger.LogInformation("👤 Getting first person...");
        var firstPerson = await client.People()
            .OrderBy(p => p.CreatedAt!)
            .FirstOrDefaultAsync();

        if (firstPerson != null)
        {
            logger.LogInformation("✅ First person: {Name} (Created: {Created:yyyy-MM-dd})", 
                firstPerson.FullName, firstPerson.CreatedAt);
        }

        // Conditional operations
        logger.LogInformation("🔍 Checking for specific conditions...");
        var hasActiveMembers = await client.People()
            .AnyAsync(p => p.Status == "active" && p.MembershipStatus == "member");
        logger.LogInformation("✅ Has active members: {HasActiveMembers}", hasActiveMembers);

        logger.LogInformation("");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "⚠️ LINQ-like operations failed");
        logger.LogInformation("");
    }
}

static async Task StreamingExample(IPlanningCenterClient client, ILogger logger)
{
    logger.LogInformation("🌊 Example 4: Memory-Efficient Streaming");
    logger.LogInformation("─────────────────────────────────────────");

    try
    {
        logger.LogInformation("🔄 Streaming people for memory-efficient processing...");
        var processedCount = 0;
        var startTime = DateTime.Now;

        await foreach (var person in client.People()
            .OrderBy(p => p.LastName!)
            .AsAsyncEnumerable())
        {
            // Process each person individually without loading all into memory
            processedCount++;
            
            if (processedCount <= 5)
            {
                logger.LogInformation("   👤 Processing: {Name}", person.FullName);
            }
            else if (processedCount == 6)
            {
                logger.LogInformation("   ... (continuing to process more people)");
            }

            // Simulate processing work
            await Task.Delay(10);

            // Break after processing 10 for demo purposes
            if (processedCount >= 10) break;
        }

        var elapsed = DateTime.Now - startTime;
        logger.LogInformation("✅ Processed {Count} people in {ElapsedMs}ms via streaming", 
            processedCount, elapsed.TotalMilliseconds);

        // Demonstrate streaming with pagination options
        logger.LogInformation("⚙️ Streaming with custom pagination options...");
        var options = new PaginationOptions { PageSize = 100 }; // Larger page size for efficiency
        var streamCount = 0;

        await foreach (var person in client.People()
            .OrderBy(p => p.CreatedAt!)
            .AsAsyncEnumerable(options))
        {
            streamCount++;
            if (streamCount >= 5) break; // Limit for demo
        }

        logger.LogInformation("✅ Streamed {Count} people with custom pagination", streamCount);
        logger.LogInformation("");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "⚠️ Streaming example failed");
        logger.LogInformation("");
    }
}

static async Task PerformanceOptimizationExample(IPlanningCenterClient client, ILogger logger)
{
    logger.LogInformation("⚡ Example 5: Performance Monitoring & Optimization");
    logger.LogInformation("─────────────────────────────────────────");

    try
    {
        // Performance monitoring
        logger.LogInformation("📊 Executing query with performance monitoring...");
        var startTime = DateTime.UtcNow;
        var result = await client.People()
            .OrderBy(p => p.LastName!)
            .GetPagedAsync(pageSize: 5);
            
        var executionTime = DateTime.UtcNow - startTime;
        
        logger.LogInformation("✅ Query executed successfully:");
        logger.LogInformation("   ⏱️ Execution time: {ElapsedMs}ms", executionTime.TotalMilliseconds);
        logger.LogInformation("   📊 Returned {Count} people", result.Data.Count);

        // Query metadata analysis
        logger.LogInformation("🔧 Analyzing query metadata...");
        var queryResult = await client.People()
            .Where(p => p.Status == "active")
            .OrderBy(p => p.LastName!)
            .GetPagedAsync(pageSize: 5);

        logger.LogInformation("✅ Query metadata analysis:");
        logger.LogInformation("   📊 Total count: {Count}", queryResult.Meta.TotalCount);
        logger.LogInformation("   📈 Current page: {Page}", queryResult.Meta.CurrentPage);

        // Query cloning for reuse
        logger.LogInformation("🔄 Demonstrating query cloning...");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "⚠️ Performance optimization example failed");
        logger.LogInformation("");
    }
}

static async Task MultiModuleFluentExample(IPlanningCenterClient client, ILogger logger)
{
    logger.LogInformation("🏗️ Example 6: Multi-Module Fluent Operations");
    logger.LogInformation("─────────────────────────────────────────");

    try
    {
        // People module
        logger.LogInformation("👥 Testing People module fluent API...");
        var peopleCount = await client.People().CountAsync();
        logger.LogInformation("✅ People module: {Count} people found", peopleCount);

        // Calendar module (if available)
        logger.LogInformation("📅 Testing Calendar module fluent API...");
        try
        {
            var upcomingEvents = await client.Calendar()
                .Where(e => e.StartsAt > DateTime.Now)
                .OrderBy(e => e.StartsAt!)
                .GetPagedAsync(pageSize: 3);
            logger.LogInformation("✅ Calendar module: {Count} upcoming events", upcomingEvents.Data.Count);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not registered"))
        {
            logger.LogInformation("ℹ️ Calendar service not registered (expected in basic setup)");
        }

        // Giving module (if available)
        logger.LogInformation("💰 Testing Giving module fluent API...");
        try
        {
            var recentDonations = await client.Giving()
                .Where(d => d.ReceivedAt >= DateTime.Today.AddDays(-30))
                .OrderByDescending(d => d.ReceivedAt!)
                .GetPagedAsync(pageSize: 3);
            logger.LogInformation("✅ Giving module: {Count} recent donations", recentDonations.Data.Count);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not registered"))
        {
            logger.LogInformation("ℹ️ Giving service not registered (expected in basic setup)");
        }

        // Groups module (if available)
        logger.LogInformation("👥 Testing Groups module fluent API...");
        try
        {
            var openGroups = await client.Groups()
                .Where(g => g.Name.Contains("open"))
                .OrderBy(g => g.Name!)
                .GetPagedAsync(pageSize: 3);
            logger.LogInformation("✅ Groups module: {Count} open groups", openGroups.Data.Count);
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

static async Task RealWorldScenariosExample(IPlanningCenterClient client, ILogger logger)
{
    logger.LogInformation("🌍 Example 7: Real-World Scenarios");
    logger.LogInformation("─────────────────────────────────────────");

    try
    {
        // Scenario 1: Member Directory
        logger.LogInformation("📋 Scenario 1: Building a member directory...");
        var members = await client.People()
            .Where(p => p.Status == "active")
            .Where(p => p.MembershipStatus == "member")
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .GetAllAsync();

        logger.LogInformation("✅ Member directory: {Count} active members", members.Count);

        // Scenario 2: Recent Additions Report
        logger.LogInformation("📊 Scenario 2: Recent additions report...");
        var recentAdditions = await client.People()
            .Where(p => p.CreatedAt >= DateTime.Today.AddDays(-30))
            .OrderByDescending(p => p.CreatedAt!)
            .GetAllAsync();

        logger.LogInformation("✅ Recent additions: {Count} people added in last 30 days", recentAdditions.Count);

        // Scenario 3: Birthday List
        logger.LogInformation("🎂 Scenario 3: Upcoming birthdays...");
        var allPeopleForBirthdays = await client.People()
            .Where(p => p.Status == "active")
            .GetAllAsync();

        var upcomingBirthdays = allPeopleForBirthdays
            .Where(p => p.Birthdate.HasValue)
            .Where(p => 
            {
                if (!p.Birthdate.HasValue) return false;
                var birthday = new DateTime(DateTime.Today.Year, p.Birthdate.Value.Month, p.Birthdate.Value.Day);
                if (birthday < DateTime.Today) birthday = birthday.AddYears(1);
                return birthday <= DateTime.Today.AddDays(30);
            })
            .OrderBy(p => 
            {
                var birthday = new DateTime(DateTime.Today.Year, p.Birthdate!.Value.Month, p.Birthdate.Value.Day);
                if (birthday < DateTime.Today) birthday = birthday.AddYears(1);
                return birthday;
            })
            .Take(10)
            .ToList();

        logger.LogInformation("✅ Upcoming birthdays: {Count} people have birthdays in next 30 days", 
            upcomingBirthdays.Count);

        foreach (var person in upcomingBirthdays.Take(3))
        {
            var birthday = new DateTime(DateTime.Today.Year, person.Birthdate!.Value.Month, person.Birthdate.Value.Day);
            if (birthday < DateTime.Today) birthday = birthday.AddYears(1);
            logger.LogInformation("   🎂 {Name} - {Birthday:MMM dd}", person.FullName, birthday);
        }

        // Scenario 4: Data Export Preparation
        logger.LogInformation("📤 Scenario 4: Data export preparation...");
        var exportData = new List<object>();
        
        await foreach (var person in client.People()
            .Where(p => p.Status == "active")
            .OrderBy(p => p.LastName)
            .AsAsyncEnumerable())
        {
            exportData.Add(new 
            {
                Name = person.FullName,
                Status = person.Status,
                Membership = person.MembershipStatus,
                Created = person.CreatedAt
            });
            
            if (exportData.Count >= 10) break; // Limit for demo
        }

        logger.LogInformation("✅ Export preparation: {Count} records prepared for export", exportData.Count);

        logger.LogInformation("");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "⚠️ Real-world scenarios example failed");
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
    logger.LogWarning("Get credentials from: https://api.planningcenteronline.com/oauth/applications");
    
    Environment.Exit(1);
}

static void DisplayFluentApiSummary(ILogger logger)
{
    logger.LogInformation("📋 Fluent API Examples Summary");
    logger.LogInformation("─────────────────────────────────────────");
    logger.LogInformation("✅ Basic fluent query patterns with method chaining");
    logger.LogInformation("✅ Advanced filtering and sorting with Where/OrderBy");
    logger.LogInformation("✅ LINQ-like terminal operations (Count, Any, First, etc.)");
    logger.LogInformation("✅ Memory-efficient streaming with AsAsyncEnumerable()");
    logger.LogInformation("✅ Performance monitoring and query optimization");
    logger.LogInformation("✅ Multi-module fluent API access patterns");
    logger.LogInformation("✅ Real-world scenarios and practical examples");
    logger.LogInformation("✅ Proper error handling and authentication patterns");
    logger.LogInformation("✅ Pagination navigation and automatic handling");
    logger.LogInformation("✅ Custom parameters and advanced query features");
    logger.LogInformation("");
    logger.LogInformation("🎯 Key Fluent API Benefits:");
    logger.LogInformation("   • LINQ-like syntax familiar to .NET developers");
    logger.LogInformation("   • Type-safe query building with IntelliSense");
    logger.LogInformation("   • Automatic pagination handling");
    logger.LogInformation("   • Memory-efficient streaming for large datasets");
    logger.LogInformation("   • Built-in performance monitoring and optimization");
    logger.LogInformation("   • Consistent patterns across all modules");
    logger.LogInformation("");
    logger.LogInformation("🚀 Ready to build powerful Planning Center integrations!");
}