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

// Add Planning Center API client
// NOTE: You need to provide actual credentials for this to work
// You can get these from your Planning Center API application settings

// Option 1: OAuth Authentication (for user-facing applications)
builder.Services.AddPlanningCenterApiClient(options =>
{
    // For demo purposes - in real applications, use configuration or environment variables
    options.ClientId = Environment.GetEnvironmentVariable("PLANNING_CENTER_CLIENT_ID") ?? "your-client-id";
    options.ClientSecret = Environment.GetEnvironmentVariable("PLANNING_CENTER_CLIENT_SECRET") ?? "your-client-secret";
    
    // Optional: Enable detailed logging for debugging (be careful in production)
    options.EnableDetailedLogging = true;
    
    // Optional: Configure caching
    options.EnableCaching = true;
    options.DefaultCacheExpiration = TimeSpan.FromMinutes(5);
});

// Option 2: Personal Access Token (PAT) Authentication (for server-side applications)
// Uncomment the lines below and comment out the OAuth configuration above to use PAT
/*
builder.Services.AddPlanningCenterApiClientWithPAT(
    Environment.GetEnvironmentVariable("PLANNING_CENTER_PAT") ?? "your-app-id:your-secret");

builder.Services.Configure<PlanningCenterOptions>(options =>
{
    options.EnableDetailedLogging = true;
    options.EnableCaching = true;
    options.DefaultCacheExpiration = TimeSpan.FromMinutes(5);
});
*/

// Build the host
var host = builder.Build();

// Get the logger
var logger = host.Services.GetRequiredService<ILogger<Program>>();

try
{
    logger.LogInformation("🚀 Planning Center SDK Console Example Starting...");
    
    // Get the People service
    var peopleService = host.Services.GetRequiredService<IPeopleService>();
    
    // Example 1: List people with pagination
    logger.LogInformation("📋 Example 1: Listing people with built-in pagination...");
    
    var parameters = new QueryParameters
    {
        Where = new Dictionary<string, object> { ["status"] = "active" },
        PerPage = 10  // Small page size for demo
    };
    
    var firstPage = await peopleService.ListAsync(parameters);
    
    logger.LogInformation("✅ Retrieved page {CurrentPage} of {TotalPages} with {Count} people (Total: {TotalCount})",
        firstPage.Meta.CurrentPage, 
        firstPage.Meta.TotalPages, 
        firstPage.Data.Count, 
        firstPage.Meta.TotalCount);
    
    // Display first few people
    foreach (var person in firstPage.Data.Take(3))
    {
        logger.LogInformation("👤 {FullName} (ID: {Id}, Status: {Status})", 
            person.FullName, person.Id, person.Status);
    }
    
    // Example 2: Navigate to next page using built-in helper
    if (firstPage.HasNextPage)
    {
        logger.LogInformation("➡️ Example 2: Getting next page using built-in navigation...");
        
        var nextPage = await firstPage.GetNextPageAsync();
        if (nextPage != null)
        {
            logger.LogInformation("✅ Retrieved next page with {Count} people", nextPage.Data.Count);
            
            foreach (var person in nextPage.Data.Take(2))
            {
                logger.LogInformation("👤 {FullName} (ID: {Id})", person.FullName, person.Id);
            }
        }
    }
    
    // Example 3: Get all people automatically (handles pagination behind the scenes)
    logger.LogInformation("🔄 Example 3: Getting ALL people automatically (this may take a while for large datasets)...");
    
    var limitedParameters = new QueryParameters
    {
        Where = new Dictionary<string, object> { ["status"] = "active" },
        PerPage = 5  // Small page size for demo
    };
    
    // For demo purposes, let's limit to first 20 people to avoid long waits
    var paginationOptions = new PaginationOptions
    {
        MaxItems = 20  // Limit to 20 people for demo
    };
    
    var allPeople = await peopleService.GetAllAsync(limitedParameters, paginationOptions);
    
    logger.LogInformation("✅ Retrieved ALL {Count} people automatically (pagination handled behind the scenes)", 
        allPeople.Count);
    
    // Example 4: Stream people memory-efficiently
    logger.LogInformation("🌊 Example 4: Streaming people memory-efficiently...");
    
    var streamCount = 0;
    await foreach (var person in peopleService.StreamAsync(limitedParameters, paginationOptions))
    {
        streamCount++;
        if (streamCount <= 5)  // Only log first 5 for demo
        {
            logger.LogInformation("🌊 Streamed: {FullName} (Age: {Age})", 
                person.FullName, person.Age?.ToString() ?? "Unknown");
        }
    }
    
    logger.LogInformation("✅ Streamed {Count} people total", streamCount);
    
    // Example 5: Create a new person (commented out to avoid creating test data)
    /*
    logger.LogInformation("➕ Example 5: Creating a new person...");
    
    var createRequest = new PersonCreateRequest
    {
        FirstName = "Test",
        LastName = "Person",
        Email = "test@example.com",
        Status = "active"
    };
    
    var newPerson = await peopleService.CreateAsync(createRequest);
    logger.LogInformation("✅ Created person: {FullName} (ID: {Id})", newPerson.FullName, newPerson.Id);
    
    // Clean up - delete the test person
    await peopleService.DeleteAsync(newPerson.Id);
    logger.LogInformation("🗑️ Cleaned up test person");
    */
    
    logger.LogInformation("🎉 All examples completed successfully!");
    logger.LogInformation("");
    logger.LogInformation("🔑 Key Features Demonstrated:");
    logger.LogInformation("   ✅ Built-in pagination helpers eliminate manual pagination logic");
    logger.LogInformation("   ✅ Automatic page fetching with GetAllAsync()");
    logger.LogInformation("   ✅ Memory-efficient streaming with StreamAsync()");
    logger.LogInformation("   ✅ Rich pagination metadata and navigation");
    logger.LogInformation("   ✅ Comprehensive error handling and logging");
    logger.LogInformation("");
    logger.LogInformation("📖 To run this example with real data:");
    logger.LogInformation("   Option 1 - OAuth:");
    logger.LogInformation("   1. Set PLANNING_CENTER_CLIENT_ID environment variable");
    logger.LogInformation("   2. Set PLANNING_CENTER_CLIENT_SECRET environment variable");
    logger.LogInformation("   3. Run: dotnet run");
    logger.LogInformation("");
    logger.LogInformation("   Option 2 - Personal Access Token (recommended for scripts):");
    logger.LogInformation("   1. Set PLANNING_CENTER_PAT environment variable (format: app_id:secret)");
    logger.LogInformation("   2. Uncomment PAT configuration in Program.cs");
    logger.LogInformation("   3. Comment out OAuth configuration");
    logger.LogInformation("   4. Run: dotnet run");
}
catch (Exception ex)
{
    logger.LogError(ex, "❌ An error occurred during execution");
    
    if (ex.Message.Contains("your-client-id") || ex.Message.Contains("your-app-id") || ex.Message.Contains("Authentication"))
    {
        logger.LogWarning("");
        logger.LogWarning("🔑 Authentication Error - Please set your credentials:");
        logger.LogWarning("");
        logger.LogWarning("   Option 1 - OAuth (for user-facing apps):");
        logger.LogWarning("   export PLANNING_CENTER_CLIENT_ID=\"your-actual-client-id\"");
        logger.LogWarning("   export PLANNING_CENTER_CLIENT_SECRET=\"your-actual-client-secret\"");
        logger.LogWarning("");
        logger.LogWarning("   Option 2 - Personal Access Token (for server apps):");
        logger.LogWarning("   export PLANNING_CENTER_PAT=\"your-app-id:your-secret\"");
        logger.LogWarning("");
        logger.LogWarning("   You can get these from your Planning Center API application settings.");
    }
    
    Environment.Exit(1);
}
