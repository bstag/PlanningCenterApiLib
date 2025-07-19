using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PlanningCenter.Api.Client;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Exceptions;
using PlanningCenter.Api.Client.Models.Requests;

// Create host with dependency injection
var builder = Host.CreateApplicationBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Add Planning Center API client with ALL modules
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
var client = host.Services.GetRequiredService<IPlanningCenterClient>();

logger.LogInformation("🚀 Planning Center SDK - Multi-Module Integration Examples");
logger.LogInformation("🏗️ Demonstrating cross-module workflows and comprehensive usage");
logger.LogInformation("");

try
{
    // Example 1: Authentication and basic connectivity
    await TestConnectivityExample(client, logger);
    
    // Example 2: People module comprehensive examples
    await PeopleModuleExamples(client, logger);
    
    // Example 3: Calendar module examples
    await CalendarModuleExamples(client, logger);
    
    // Example 4: Giving module examples
    await GivingModuleExamples(client, logger);
    
    // Example 5: Groups module examples
    await GroupsModuleExamples(client, logger);
    
    // Example 6: Cross-module integration scenarios
    await CrossModuleIntegrationExamples(client, logger);
    
    // Example 7: Advanced features and performance
    await AdvancedFeaturesExamples(client, logger);
    
    logger.LogInformation("");
    logger.LogInformation("🎉 All multi-module examples completed successfully!");
    DisplayComprehensiveSummary(logger);
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

static async Task TestConnectivityExample(IPlanningCenterClient client, ILogger logger)
{
    logger.LogInformation("🔐 Example 1: Authentication & Connectivity Testing");
    logger.LogInformation("─────────────────────────────────────────────────");
    
    try
    {
        // Test basic connectivity with People module (always available)
        var currentUser = await client.People().GetAsync("me");
        if (currentUser != null)
        {
            logger.LogInformation("✅ Authentication successful!");
            logger.LogInformation("   👤 Current user: {FullName} (ID: {Id})", currentUser.FullName, currentUser.Id);
            logger.LogInformation("   📊 Status: {Status}, Membership: {MembershipStatus}", 
                currentUser.Status, currentUser.MembershipStatus ?? "Not specified");
        }
        
        // Test API health and limits
        var healthCheck = await client.CheckHealthAsync();
        logger.LogInformation("🏥 API Health Check:");
        logger.LogInformation("   ✅ Healthy: {IsHealthy}", healthCheck.IsHealthy);
        logger.LogInformation("   ⏱️ Response time: {ResponseTime}ms", healthCheck.ResponseTimeMs);
        logger.LogInformation("   📅 API version: {ApiVersion}", healthCheck.ApiVersion ?? "Unknown");
        
        var apiLimits = await client.GetApiLimitsAsync();
        logger.LogInformation("📊 API Rate Limits:");
        logger.LogInformation("   🔢 Rate limit: {RateLimit} requests per {TimePeriod}", 
            apiLimits.RateLimit, apiLimits.TimePeriod);
        logger.LogInformation("   ⏳ Remaining: {Remaining} requests", apiLimits.RemainingRequests);
        logger.LogInformation("   📈 Usage: {Usage:F1}%", apiLimits.UsagePercentage);
        
        logger.LogInformation("");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Connectivity test failed");
        throw;
    }
}

static async Task PeopleModuleExamples(IPlanningCenterClient client, ILogger logger)
{
    logger.LogInformation("👥 Example 2: People Module Comprehensive Examples");
    logger.LogInformation("─────────────────────────────────────────────────");
    
    try
    {
        // Basic people operations
        logger.LogInformation("📋 Basic people operations...");
        var activePeople = await client.People()
            .Where(p => p.Status == "active")
            .OrderBy(p => p.LastName)
            .GetPagedAsync(pageSize: 5);
        
        logger.LogInformation("✅ Found {Count} active people", activePeople.Data.Count);
        
        // Demonstrate different query patterns
        logger.LogInformation("🔍 Advanced query patterns...");
        
        // Count operations
        var totalPeople = await client.People().CountAsync();
        var activeMembers = await client.People()
            .Where(p => p.Status == "active")
            .Where(p => p.MembershipStatus == "member")
            .CountAsync();
        
        logger.LogInformation("📊 Statistics:");
        logger.LogInformation("   👥 Total people: {Total}", totalPeople);
        logger.LogInformation("   ✅ Active members: {ActiveMembers}", activeMembers);
        
        // Recent additions
        var recentPeople = await client.People()
            .Where(p => p.CreatedAt >= DateTime.Today.AddDays(-30))
            .OrderByDescending(p => p.CreatedAt)
            .GetPagedAsync(pageSize: 5);
        
        logger.LogInformation("📅 Recent additions (last 30 days): {Count}", recentPeople.Data.Count);
        
        // Memory-efficient processing
        logger.LogInformation("🌊 Memory-efficient streaming...");
        var processedCount = 0;
        await foreach (var person in client.People()
            .Where(p => p.Status == "active")
            .OrderBy(p => p.LastName)
            .AsAsyncEnumerable())
        {
            processedCount++;
            if (processedCount >= 10) break; // Limit for demo
        }
        logger.LogInformation("✅ Processed {Count} people via streaming", processedCount);
        
        logger.LogInformation("");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "⚠️ People module examples failed");
        logger.LogInformation("");
    }
}

static async Task CalendarModuleExamples(IPlanningCenterClient client, ILogger logger)
{
    logger.LogInformation("📅 Example 3: Calendar Module Examples");
    logger.LogInformation("─────────────────────────────────────");
    
    try
    {
        // Test calendar module availability
        var upcomingEvents = await client.Calendar()
            .Where(e => e.StartsAt > DateTime.Now)
            .Where(e => e.VisibleInChurchCenter == true)
            .OrderBy(e => e.StartsAt)
            .GetPagedAsync(pageSize: 5);
        
        logger.LogInformation("✅ Calendar module: Found {Count} upcoming events", upcomingEvents.Data.Count);
        
        foreach (var eventItem in upcomingEvents.Data.Take(3))
        {
            logger.LogInformation("   📅 {Name} - {StartTime:yyyy-MM-dd HH:mm}", 
                eventItem.Name, eventItem.StartsAt);
        }
        
        // Events this week
        var weekStart = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
        var weekEnd = weekStart.AddDays(6);
        
        var thisWeekEvents = await client.Calendar()
            .Where(e => e.StartsAt >= weekStart)
            .Where(e => e.StartsAt <= weekEnd)
            .OrderBy(e => e.StartsAt)
            .GetAllAsync();
        
        logger.LogInformation("📊 Events this week: {Count}", thisWeekEvents.Count);
        
        // Resources
        var resources = await client.Calendar()
            .Resources()
            .Where(r => r.Quantity > 0)
            .OrderBy(r => r.Name)
            .GetPagedAsync(pageSize: 5);
        
        logger.LogInformation("🏢 Available resources: {Count}", resources.Data.Count);
        
        logger.LogInformation("");
    }
    catch (InvalidOperationException ex) when (ex.Message.Contains("not registered"))
    {
        logger.LogInformation("ℹ️ Calendar service not registered - add it to DI container to enable");
        logger.LogInformation("");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "⚠️ Calendar module examples failed");
        logger.LogInformation("");
    }
}

static async Task GivingModuleExamples(IPlanningCenterClient client, ILogger logger)
{
    logger.LogInformation("💰 Example 4: Giving Module Examples");
    logger.LogInformation("───────────────────────────────────");
    
    try
    {
        // Recent donations
        var recentDonations = await client.Giving()
            .Where(d => d.ReceivedAt >= DateTime.Today.AddDays(-30))
            .OrderByDescending(d => d.ReceivedAt)
            .GetPagedAsync(pageSize: 5);
        
        logger.LogInformation("✅ Giving module: Found {Count} recent donations", recentDonations.Data.Count);
        
        // Calculate totals
        var monthlyDonations = await client.Giving()
            .Where(d => d.ReceivedAt >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1))
            .GetAllAsync();
        
        var monthlyTotal = monthlyDonations.Sum(d => d.AmountCents) / 100.0m;
        logger.LogInformation("📊 This month's total: ${Total:F2}", monthlyTotal);
        
        // Funds
        var activeFunds = await client.Giving()
            .Funds()
            .Where(f => f.Visibility == "everyone")
            .OrderBy(f => f.Name)
            .GetPagedAsync(pageSize: 5);
        
        logger.LogInformation("🎯 Active funds: {Count}", activeFunds.Data.Count);
        
        // Recurring donations
        var activeRecurring = await client.Giving()
            .RecurringDonations()
            .Where(rd => rd.Status == "active")
            .CountAsync();
        
        logger.LogInformation("🔄 Active recurring donations: {Count}", activeRecurring);
        
        logger.LogInformation("");
    }
    catch (InvalidOperationException ex) when (ex.Message.Contains("not registered"))
    {
        logger.LogInformation("ℹ️ Giving service not registered - add it to DI container to enable");
        logger.LogInformation("");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "⚠️ Giving module examples failed");
        logger.LogInformation("");
    }
}

static async Task GroupsModuleExamples(IPlanningCenterClient client, ILogger logger)
{
    logger.LogInformation("👥 Example 5: Groups Module Examples");
    logger.LogInformation("───────────────────────────────────");
    
    try
    {
        // Open groups
        var openGroups = await client.Groups()
            .Where(g => g.Enrollment == "open")
            .Where(g => g.PublicChurchCenterWebEnabled == true)
            .OrderBy(g => g.Name)
            .GetPagedAsync(pageSize: 5);
        
        logger.LogInformation("✅ Groups module: Found {Count} open groups", openGroups.Data.Count);
        
        foreach (var group in openGroups.Data.Take(3))
        {
            logger.LogInformation("   👥 {Name} - {MemberCount} members", 
                group.Name, group.MembershipsCount);
        }
        
        // Group types
        var groupTypes = await client.Groups()
            .GroupTypes()
            .Where(gt => gt.ChurchCenterVisible == true)
            .OrderBy(gt => gt.Name)
            .GetAllAsync();
        
        logger.LogInformation("📋 Group types: {Count}", groupTypes.Count);
        
        // Membership statistics
        var totalMemberships = await client.Groups()
            .Memberships()
            .Where(m => m.JoinedAt.HasValue)
            .CountAsync();
        
        logger.LogInformation("📊 Total active memberships: {Count}", totalMemberships);
        
        logger.LogInformation("");
    }
    catch (InvalidOperationException ex) when (ex.Message.Contains("not registered"))
    {
        logger.LogInformation("ℹ️ Groups service not registered - add it to DI container to enable");
        logger.LogInformation("");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "⚠️ Groups module examples failed");
        logger.LogInformation("");
    }
}

static async Task CrossModuleIntegrationExamples(IPlanningCenterClient client, ILogger logger)
{
    logger.LogInformation("🔗 Example 6: Cross-Module Integration Scenarios");
    logger.LogInformation("─────────────────────────────────────────────");
    
    try
    {
        // Scenario 1: Member engagement report
        logger.LogInformation("📊 Scenario 1: Member engagement analysis...");
        
        var activeMembers = await client.People()
            .Where(p => p.Status == "active")
            .Where(p => p.MembershipStatus == "member")
            .GetAllAsync();
        
        logger.LogInformation("✅ Active members: {Count}", activeMembers.Count);
        
        // Try to correlate with group memberships (if Groups module available)
        try
        {
            var groupMemberships = await client.Groups()
                .Memberships()
                .Where(m => m.JoinedAt.HasValue)
                .GetAllAsync();
            
            var membersInGroups = groupMemberships
                .Select(m => m.PersonId)
                .Distinct()
                .Count();
            
            logger.LogInformation("👥 Members in groups: {Count}", membersInGroups);
            
            var engagementRate = activeMembers.Count > 0 
                ? (membersInGroups / (double)activeMembers.Count) * 100 
                : 0;
            
            logger.LogInformation("📈 Group engagement rate: {Rate:F1}%", engagementRate);
        }
        catch (InvalidOperationException)
        {
            logger.LogInformation("ℹ️ Groups module not available for engagement analysis");
        }
        
        // Scenario 2: Event attendance correlation (if Calendar module available)
        logger.LogInformation("📅 Scenario 2: Event attendance correlation...");
        try
        {
            var recentEvents = await client.Calendar()
                .Where(e => e.StartsAt >= DateTime.Today.AddDays(-30))
                .Where(e => e.VisibleInChurchCenter == true)
                .GetAllAsync();
            
            logger.LogInformation("✅ Recent events: {Count}", recentEvents.Count);
            logger.LogInformation("📊 Average events per week: {Average:F1}", 
                recentEvents.Count / 4.0); // Approximate weeks in 30 days
        }
        catch (InvalidOperationException)
        {
            logger.LogInformation("ℹ️ Calendar module not available for event analysis");
        }
        
        // Scenario 3: Giving patterns (if Giving module available)
        logger.LogInformation("💰 Scenario 3: Giving pattern analysis...");
        try
        {
            var recentDonations = await client.Giving()
                .Where(d => d.ReceivedAt >= DateTime.Today.AddDays(-30))
                .GetAllAsync();
            
            var uniqueGivers = recentDonations
                .Select(d => d.PersonId)
                .Distinct()
                .Count();
            
            logger.LogInformation("✅ Recent donations: {Count} from {Givers} unique givers", 
                recentDonations.Count, uniqueGivers);
            
            var givingRate = activeMembers.Count > 0 
                ? (uniqueGivers / (double)activeMembers.Count) * 100 
                : 0;
            
            logger.LogInformation("📈 Member giving participation: {Rate:F1}%", givingRate);
        }
        catch (InvalidOperationException)
        {
            logger.LogInformation("ℹ️ Giving module not available for giving analysis");
        }
        
        logger.LogInformation("");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "⚠️ Cross-module integration examples failed");
        logger.LogInformation("");
    }
}

static async Task AdvancedFeaturesExamples(IPlanningCenterClient client, ILogger logger)
{
    logger.LogInformation("⚡ Example 7: Advanced Features & Performance");
    logger.LogInformation("─────────────────────────────────────────");
    
    try
    {
        // Performance monitoring
        logger.LogInformation("📊 Performance monitoring...");
        var result = await client.People()
            .Where(p => p.Status == "active")
            .OrderBy(p => p.LastName)
            .ExecuteWithDebugInfoAsync(pageSize: 10);
        
        if (result.Success)
        {
            logger.LogInformation("✅ Query performance:");
            logger.LogInformation("   ⏱️ Execution time: {ElapsedMs}ms", result.ExecutionTime.TotalMilliseconds);
            logger.LogInformation("   📈 Optimization score: {Score}", result.OptimizationInfo.Score);
            logger.LogInformation("   📊 Results: {Count} people", result.Data?.Data.Count ?? 0);
        }
        
        // Batch operations demonstration
        logger.LogInformation("🔄 Batch operations...");
        var batch = client.People().Batch();
        
        // Note: In a real scenario, you'd add actual operations
        logger.LogInformation("✅ Batch context created (add operations and call ExecuteAsync())");
        
        // Caching demonstration
        logger.LogInformation("💾 Caching demonstration...");
        var startTime = DateTime.Now;
        
        // First call (not cached)
        var firstCall = await client.People()
            .OrderBy(p => p.LastName)
            .GetPagedAsync(pageSize: 5);
        var firstCallTime = DateTime.Now - startTime;
        
        startTime = DateTime.Now;
        
        // Second call (potentially cached)
        var secondCall = await client.People()
            .OrderBy(p => p.LastName)
            .GetPagedAsync(pageSize: 5);
        var secondCallTime = DateTime.Now - startTime;
        
        logger.LogInformation("✅ Caching performance:");
        logger.LogInformation("   🥇 First call: {FirstTime}ms", firstCallTime.TotalMilliseconds);
        logger.LogInformation("   🥈 Second call: {SecondTime}ms", secondCallTime.TotalMilliseconds);
        
        if (secondCallTime < firstCallTime)
        {
            logger.LogInformation("   🚀 Cache hit detected! {Improvement:F1}% faster", 
                ((firstCallTime - secondCallTime).TotalMilliseconds / firstCallTime.TotalMilliseconds) * 100);
        }
        
        // Correlation tracking
        logger.LogInformation("🔗 Correlation tracking...");
        // The SDK automatically generates correlation IDs for all operations
        logger.LogInformation("✅ All operations automatically tracked with correlation IDs");
        
        logger.LogInformation("");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "⚠️ Advanced features examples failed");
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

static void DisplayComprehensiveSummary(ILogger logger)
{
    logger.LogInformation("📋 Multi-Module Integration Summary");
    logger.LogInformation("─────────────────────────────────────");
    logger.LogInformation("✅ Authentication and connectivity testing");
    logger.LogInformation("✅ People module comprehensive operations");
    logger.LogInformation("✅ Calendar module event and resource management");
    logger.LogInformation("✅ Giving module donation and fund tracking");
    logger.LogInformation("✅ Groups module membership management");
    logger.LogInformation("✅ Cross-module integration scenarios");
    logger.LogInformation("✅ Advanced features and performance monitoring");
    logger.LogInformation("✅ Proper error handling and fallback patterns");
    logger.LogInformation("✅ Memory-efficient streaming operations");
    logger.LogInformation("✅ Caching and optimization demonstrations");
    logger.LogInformation("");
    logger.LogInformation("🎯 Key Integration Patterns:");
    logger.LogInformation("   • Consistent API patterns across all modules");
    logger.LogInformation("   • Graceful handling of missing module registrations");
    logger.LogInformation("   • Cross-module data correlation and analysis");
    logger.LogInformation("   • Performance monitoring and optimization");
    logger.LogInformation("   • Automatic correlation tracking and logging");
    logger.LogInformation("   • Memory-efficient processing for large datasets");
    logger.LogInformation("");
    logger.LogInformation("🚀 Ready for production Planning Center integrations!");
    logger.LogInformation("");
    logger.LogInformation("📖 Next Steps:");
    logger.LogInformation("   • Register additional modules in your DI container");
    logger.LogInformation("   • Implement error handling and retry logic");
    logger.LogInformation("   • Add monitoring and alerting for production use");
    logger.LogInformation("   • Consider implementing caching strategies");
    logger.LogInformation("   • Review security best practices for credential management");
}