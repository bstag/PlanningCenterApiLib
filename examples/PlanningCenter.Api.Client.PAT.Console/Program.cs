using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PlanningCenter.Api.Client;
using PlanningCenter.Api.Client.Models;

// Create host builder with dependency injection
var builder = Host.CreateApplicationBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Add Planning Center API client with Personal Access Token (PAT)
// NOTE: You need to provide an actual PAT for this to work
// You can get this from your Planning Center API application settings
builder.Services.AddPlanningCenterApiClientWithPAT(
    Environment.GetEnvironmentVariable("PLANNING_CENTER_PAT") ?? "your-app-id:your-secret");

// Configure additional options
builder.Services.Configure<PlanningCenterOptions>(options =>
{
    // Optional: Enable detailed logging for debugging (be careful in production)
    options.EnableDetailedLogging = true;
    
    // Optional: Configure caching
    options.EnableCaching = true;
    options.DefaultCacheExpiration = TimeSpan.FromMinutes(5);
    
    // Optional: Configure retry behavior
    options.MaxRetryAttempts = 3;
    options.RetryBaseDelay = TimeSpan.FromSeconds(1);
});

// Build the host
var host = builder.Build();

// Get the logger
var logger = host.Services.GetRequiredService<ILogger<Program>>();

try
{
    logger.LogInformation("🚀 Planning Center SDK PAT Authentication Example Starting...");
    logger.LogInformation("🔑 Using Personal Access Token authentication");
    
    // Get the People service
    var peopleService = host.Services.GetRequiredService<IPeopleService>();
    
    // Example 1: Test authentication by listing a few people
    logger.LogInformation("🔐 Example 1: Testing PAT authentication...");
    
    var parameters = new QueryParameters
    {
        Where = new Dictionary<string, object> { ["status"] = "active" },
        PerPage = 5  // Small page size for demo
    };
    
    var firstPage = await peopleService.ListAsync(parameters);
    
    logger.LogInformation("✅ PAT Authentication successful!");
    logger.LogInformation("📋 Retrieved {Count} people from page {CurrentPage} of {TotalPages} (Total: {TotalCount})",
        firstPage.Data.Count,
        firstPage.Meta.CurrentPage, 
        firstPage.Meta.TotalPages, 
        firstPage.Meta.TotalCount);
    
    // Display first few people
    foreach (var person in firstPage.Data.Take(3))
    {
        logger.LogInformation("👤 {FullName} (ID: {Id}, Status: {Status})", 
            person.FullName, person.Id, person.Status);
    }
    
    // Example 2: Demonstrate that PAT doesn't need token refresh
    logger.LogInformation("🔄 Example 2: Testing token validity (PATs are always valid)...");
    
    var authenticator = host.Services.GetRequiredService<IAuthenticator>();
    var isValid = await authenticator.IsTokenValidAsync();
    logger.LogInformation("✅ Token is valid: {IsValid}", isValid);
    
    // Example 3: Get access token (will be Basic Auth header)
    logger.LogInformation("🎫 Example 3: Getting access token...");
    
    var accessToken = await authenticator.GetAccessTokenAsync();
    logger.LogInformation("✅ Access token retrieved (Basic Auth format): {TokenPrefix}...", 
        accessToken.Length > 20 ? accessToken[..20] : accessToken);
    
    // Example 4: Demonstrate streaming with PAT
    logger.LogInformation("🌊 Example 4: Streaming people with PAT authentication...");
    
    var streamParameters = new QueryParameters
    {
        Where = new Dictionary<string, object> { ["status"] = "active" },
        PerPage = 3  // Small page size for demo
    };
    
    var paginationOptions = new PaginationOptions
    {
        MaxItems = 10  // Limit to 10 people for demo
    };
    
    var streamCount = 0;
    await foreach (var person in peopleService.StreamAsync(streamParameters, paginationOptions))
    {
        streamCount++;
        if (streamCount <= 5)  // Only log first 5 for demo
        {
            logger.LogInformation("🌊 Streamed: {FullName} (Age: {Age})", 
                person.FullName, person.Age?.ToString() ?? "Unknown");
        }
    }
    
    logger.LogInformation("✅ Streamed {Count} people total using PAT authentication", streamCount);
    
    logger.LogInformation("🎉 All PAT authentication examples completed successfully!");
    logger.LogInformation("");
    logger.LogInformation("🔑 Key PAT Authentication Features:");
    logger.LogInformation("   ✅ Simple Basic Authentication with app_id:secret format");
    logger.LogInformation("   ✅ No token refresh needed - PATs don't expire");
    logger.LogInformation("   ✅ Perfect for server-side applications and scripts");
    logger.LogInformation("   ✅ Automatic authentication header management");
    logger.LogInformation("   ✅ Same API surface as OAuth - just different auth method");
    logger.LogInformation("");
    logger.LogInformation("📖 To run this example with real data:");
    logger.LogInformation("   1. Get your PAT from Planning Center API settings");
    logger.LogInformation("   2. Set PLANNING_CENTER_PAT environment variable: export PLANNING_CENTER_PAT=\"your-app-id:your-secret\"");
    logger.LogInformation("   3. Run: dotnet run");
    logger.LogInformation("");
    logger.LogInformation("🆚 PAT vs OAuth:");
    logger.LogInformation("   • PAT: Simple, no expiration, perfect for server apps");
    logger.LogInformation("   • OAuth: More complex, token refresh, better for user-facing apps");
}
catch (Exception ex)
{
    logger.LogError(ex, "❌ An error occurred during execution");
    
    if (ex.Message.Contains("your-app-id") || ex.Message.Contains("Authentication") || ex.Message.Contains("401"))
    {
        logger.LogWarning("");
        logger.LogWarning("🔑 PAT Authentication Error - Please set your Personal Access Token:");
        logger.LogWarning("   export PLANNING_CENTER_PAT=\"your-app-id:your-secret\"");
        logger.LogWarning("");
        logger.LogWarning("   You can get your PAT from Planning Center:");
        logger.LogWarning("   1. Go to https://api.planningcenteronline.com/oauth/applications");
        logger.LogWarning("   2. Create a new application or use an existing one");
        logger.LogWarning("   3. Copy the Application ID and Secret");
        logger.LogWarning("   4. Format as: app_id:secret");
    }
    
    Environment.Exit(1);
}