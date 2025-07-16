using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PlanningCenter.Api.Client;
using PlanningCenter.Api.Client.Extensions;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Requests;

// Create host with dependency injection
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Add Planning Center API client
        services.AddPlanningCenterApi(options =>
        {
            options.ClientId = Environment.GetEnvironmentVariable("PLANNING_CENTER_APP_ID") ?? "your-app-id";
            options.ClientSecret = Environment.GetEnvironmentVariable("PLANNING_CENTER_SECRET") ?? "your-secret";
        });
    })
    .Build();

// Get the Planning Center client
var client = host.Services.GetRequiredService<IPlanningCenterClient>();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

try
{
    logger.LogInformation("Starting Planning Center Fluent API Examples");

    // Example 1: Basic fluent query
    await BasicFluentQueryExample(client, logger);

    // Example 2: Complex filtering and sorting
    await ComplexQueryExample(client, logger);

    // Example 3: LINQ-like operations
    await LinqLikeOperationsExample(client, logger);

    // Example 4: Fluent person creation with related data
    await FluentCreationExample(client, logger);

    // Example 5: Pagination with fluent API
    await PaginationExample(client, logger);

    // Example 6: Streaming large datasets
    await StreamingExample(client, logger);

    // Example 7: Advanced query optimization
    await QueryOptimizationExample(client, logger);

    // Example 8: Batch operations
    await BatchOperationsExample(client, logger);

    // Example 9: Query debugging and performance analysis
    await QueryDebuggingExample(client, logger);

    logger.LogInformation("All examples completed successfully!");
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred while running examples");
}

static async Task BasicFluentQueryExample(IPlanningCenterClient client, ILogger logger)
{
    logger.LogInformation("=== Basic Fluent Query Example ===");

    try
    {
        // Get first 10 people using fluent API
        var people = await client.Fluent().People
            .GetPagedAsync(pageSize: 10);

        logger.LogInformation("Found {Count} people on first page", people.Data.Count);

        foreach (var person in people.Data.Take(3)) // Show first 3
        {
            logger.LogInformation("Person: {Name} (ID: {Id})", person.FullName, person.Id);
        }
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Basic fluent query example failed (this is expected if not authenticated)");
    }
}

static async Task ComplexQueryExample(IPlanningCenterClient client, ILogger logger)
{
    logger.LogInformation("=== Complex Query Example ===");

    try
    {
        // Note: This demonstrates the fluent syntax. The actual filtering
        // will be implemented when expression parsing is completed.
        var people = await client.Fluent().People
            .Where(p => p.FirstName == "John")
            .Where(p => p.LastName.Contains("Smith"))
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .GetPagedAsync(pageSize: 5);

        logger.LogInformation("Complex query returned {Count} people", people.Data.Count);
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Complex query example failed (this is expected if not authenticated)");
    }
}

static async Task LinqLikeOperationsExample(IPlanningCenterClient client, ILogger logger)
{
    logger.LogInformation("=== LINQ-like Operations Example ===");

    try
    {
        var peopleContext = client.Fluent().People;

        // Check if any people exist
        var anyPeople = await peopleContext.AnyAsync();
        logger.LogInformation("Any people exist: {AnyPeople}", anyPeople);

        // Get count of all people
        var totalCount = await peopleContext.CountAsync();
        logger.LogInformation("Total people count: {Count}", totalCount);

        // Get first person (if any)
        var firstPerson = await peopleContext.FirstOrDefaultAsync();
        if (firstPerson != null)
        {
            logger.LogInformation("First person: {Name}", firstPerson.FullName);
        }
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "LINQ-like operations example failed (this is expected if not authenticated)");
    }
}

static async Task FluentCreationExample(IPlanningCenterClient client, ILogger logger)
{
    logger.LogInformation("=== Fluent Creation Example ===");

    try
    {
        // Create a person with related data in one fluent operation
        var newPerson = await client.Fluent().People
            .Create(new PersonCreateRequest
            {
                FirstName = "Jane",
                LastName = "Doe",
                Birthdate = DateTime.Parse("1990-01-01")
            })
            .WithEmail(new EmailCreateRequest
            {
                Address = "jane.doe@example.com",
                Location = "Home",
                Primary = true
            })
            .WithPhoneNumber(new PhoneNumberCreateRequest
            {
                Number = "555-123-4567",
                Location = "Mobile",
                Primary = true
            })
            .WithAddress(new AddressCreateRequest
            {
                Street = "123 Main St",
                City = "Anytown",
                State = "CA",
                Zip = "12345",
                Location = "Home",
                Primary = true
            })
            .ExecuteAsync();

        logger.LogInformation("Created person with related data: {Name} (ID: {Id})", 
            newPerson.FullName, newPerson.Id);
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Fluent creation example failed (this is expected if not authenticated)");
    }
}

static async Task PaginationExample(IPlanningCenterClient client, ILogger logger)
{
    logger.LogInformation("=== Pagination Example ===");

    try
    {
        var peopleContext = client.Fluent().People
            .OrderBy(p => p.LastName);

        // Get specific page
        var page2 = await peopleContext.GetPageAsync(page: 2, pageSize: 5);
        logger.LogInformation("Page 2 contains {Count} people", page2.Data.Count);

        // Get all people (handles pagination automatically)
        var allPeople = await peopleContext.GetAllAsync();
        logger.LogInformation("Total people retrieved: {Count}", allPeople.Count);
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Pagination example failed (this is expected if not authenticated)");
    }
}

static async Task StreamingExample(IPlanningCenterClient client, ILogger logger)
{
    logger.LogInformation("=== Streaming Example ===");

    try
    {
        var peopleContext = client.Fluent().People
            .OrderBy(p => p.CreatedAt);

        // Stream people for memory-efficient processing
        var processedCount = 0;
        await foreach (var person in peopleContext.AsAsyncEnumerable())
        {
            // Process each person individually
            logger.LogDebug("Processing person: {Name}", person.FullName);
            processedCount++;

            // Break after processing 10 for demo purposes
            if (processedCount >= 10) break;
        }

        logger.LogInformation("Processed {Count} people via streaming", processedCount);
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Streaming example failed (this is expected if not authenticated)");
    }
}

static async Task QueryOptimizationExample(IPlanningCenterClient client, ILogger logger)
{
    logger.LogInformation("=== Query Optimization Example ===");

    try
    {
        var peopleContext = client.Fluent().People
            .Where(p => p.FirstName == "John")
            .Where(p => p.LastName.Contains("Smith"))
            .Include(p => p.Addresses)
            .Include(p => p.Emails)
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName);

        // Get optimization information before executing
        // var optimizationInfo = peopleContext.GetOptimizationInfo();
        // logger.LogInformation("Query complexity: {Complexity}", optimizationInfo.EstimatedComplexity);
        // logger.LogInformation("Cache hit rate: {CacheHitRate:P1}", optimizationInfo.CacheHitRate);
        // logger.LogInformation("Recommendations: {Recommendations}", optimizationInfo.GetRecommendations());

        // Clone the context for reuse
        // var clonedContext = peopleContext.Clone();
        // logger.LogInformation("Created cloned context for reuse");

        // Add custom parameters
        // var customContext = peopleContext
        //     .WithParameter("custom_filter", "active_members")
        //     .WithParameter("include_inactive", "false");

        logger.LogInformation("Query optimization and custom parameters examples not yet implemented");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Query optimization example failed (this is expected if not authenticated)");
    }
}

static async Task BatchOperationsExample(IPlanningCenterClient client, ILogger logger)
{
    logger.LogInformation("=== Batch Operations Example ===");

    try
    {
        // Create multiple people in a batch
        // var batchResult = await client.Fluent().People
        //     .Batch()
        //     .CreatePerson(new PersonCreateRequest
        //     {
        //         FirstName = "John",
        //         LastName = "Doe",
        //         Birthdate = DateTime.Parse("1990-01-01")
        //     })
        //     .CreatePerson(new PersonCreateRequest
        //     {
        //         FirstName = "Jane",
        //         LastName = "Smith",
        //         Birthdate = DateTime.Parse("1985-05-15")
        //     })
        //     .CreatePerson(new PersonCreateRequest
        //     {
        //         FirstName = "Bob",
        //         LastName = "Johnson",
        //         Birthdate = DateTime.Parse("1992-12-25")
        //     })
        //     .WithBatchSize(2)
        //     .WithParallelism(2)
        //     .ContinueOnError(true)
        //     .ExecuteAsync();

        // logger.LogInformation("Batch execution summary: {Summary}", batchResult.GetSummary());
        // logger.LogInformation("Created {Count} people successfully", batchResult.CreatedPeople.Count);
        
        // if (batchResult.Errors.Any())
        // {
        //     logger.LogWarning("Batch had {ErrorCount} errors", batchResult.Errors.Count);
        //     foreach (var error in batchResult.Errors.Take(3)) // Show first 3 errors
        //     {
        //         logger.LogWarning("Error in {Operation}: {Error}", error.Operation, error.Error);
        //     }
        // }
        
        logger.LogInformation("Batch operations example not yet implemented");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Batch operations example failed (this is expected if not authenticated)");
    }
}

static async Task QueryDebuggingExample(IPlanningCenterClient client, ILogger logger)
{
    logger.LogInformation("=== Query Debugging Example ===");

    try
    {
        var peopleContext = client.Fluent().People
            .Where(p => p.FirstName == "John")
            .Include(p => p.Addresses)
            .OrderBy(p => p.LastName);

        // Execute with debug information
        // var debugResult = await peopleContext.ExecuteWithDebugInfoAsync(pageSize: 10);

        // logger.LogInformation("Query execution summary: {Summary}", debugResult.GetExecutionSummary());
        // logger.LogInformation("Performance recommendations: {Recommendations}", debugResult.GetPerformanceRecommendations());

        // Get detailed performance report
        // var detailedReport = debugResult.GetDetailedReport();
        // logger.LogInformation("Detailed performance report:");
        // logger.LogInformation(detailedReport.ToString());

        // if (debugResult.Success && debugResult.Data != null)
        // {
        //     logger.LogInformation("Query returned {Count} results", debugResult.Data.Data.Count);
        // }
        // else if (!debugResult.Success)
        // {
        //     logger.LogWarning("Query failed: {Error}", debugResult.Error?.Message);
        // }
        
        logger.LogInformation("Query debugging and performance analysis examples not yet implemented");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Query debugging example failed (this is expected if not authenticated)");
    }
}
