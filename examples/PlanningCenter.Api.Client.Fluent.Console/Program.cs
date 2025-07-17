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

    // Example 10: Giving fluent API
    await GivingFluentApiExample(client, logger);

    // Example 11: Calendar fluent API
    await CalendarFluentApiExample(client, logger);

    // Example 12: Groups fluent API
    await GroupsFluentApiExample(client, logger);

    // Example 13: Services fluent API
    await ServicesFluentApiExample(client, logger);

    // Example 14: CheckIns fluent API
    await CheckInsFluentApiExample(client, logger);

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

static async Task GivingFluentApiExample(IPlanningCenterClient client, ILogger logger)
{
    logger.LogInformation("=== Giving Fluent API Example ===");

    try
    {
        // Example 1: Find donations by person
        var personDonations = await client.Fluent().Giving
            .ByPerson("12345")
            .ByDateRange(DateTime.Now.AddMonths(-3), DateTime.Now)
            .OrderByDescending(d => d.ReceivedAt)
            .GetPagedAsync(25);

        logger.LogInformation("Found {Count} donations for person in last 3 months", personDonations.Data.Count);

        // Example 2: Find donations by fund
        var fundDonations = await client.Fluent().Giving
            .ByFund("general-fund")
            .WithMinimumAmount(10000) // $100.00 in cents
            .GetAllAsync();

        logger.LogInformation("Found {Count} donations to general fund over $100", fundDonations.Count);

        // Example 3: Calculate total donations
        var totalAmount = await client.Fluent().Giving
            .ByDateRange(DateTime.Now.AddYears(-1), DateTime.Now)
            .TotalAmountAsync();

        logger.LogInformation("Total donations in last year: ${Amount:N2}", totalAmount / 100.0m);

        // Example 4: Check if any donations exist
        var hasRecentDonations = await client.Fluent().Giving
            .ByDateRange(DateTime.Now.AddDays(-30), DateTime.Now)
            .AnyAsync();

        logger.LogInformation("Has donations in last 30 days: {HasDonations}", hasRecentDonations);
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Giving fluent API example failed (this is expected if not authenticated)");
    }
}

static async Task CalendarFluentApiExample(IPlanningCenterClient client, ILogger logger)
{
    logger.LogInformation("=== Calendar Fluent API Example ===");

    try
    {
        // Example 1: Get today's events
        var todayEvents = await client.Fluent().Calendar
            .Today()
            .OrderBy(e => e.StartsAt)
            .GetAllAsync();

        logger.LogInformation("Found {Count} events today", todayEvents.Count);

        // Example 2: Get this week's events
        var weekEvents = await client.Fluent().Calendar
            .ThisWeek()
            .OrderBy(e => e.StartsAt)
            .GetPagedAsync(50);

        logger.LogInformation("Found {Count} events this week", weekEvents.Data.Count);

        // Example 3: Get upcoming events
        var upcomingEvents = await client.Fluent().Calendar
            .Upcoming()
            .OrderBy(e => e.StartsAt)
            .GetPagedAsync(25);

        logger.LogInformation("Found {Count} upcoming events", upcomingEvents.Data.Count);

        // Example 4: Get events in a specific date range
        var dateRangeEvents = await client.Fluent().Calendar
            .ByDateRange(DateTime.Now.AddDays(7), DateTime.Now.AddDays(14))
            .OrderBy(e => e.StartsAt)
            .GetAllAsync();

        logger.LogInformation("Found {Count} events in next week", dateRangeEvents.Count);

        // Example 5: Get events for this month
        var monthEvents = await client.Fluent().Calendar
            .ThisMonth()
            .OrderBy(e => e.StartsAt)
            .CountAsync();

        logger.LogInformation("Total events this month: {Count}", monthEvents);
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Calendar fluent API example failed (this is expected if not authenticated)");
    }
}

static async Task GroupsFluentApiExample(IPlanningCenterClient client, ILogger logger)
{
    logger.LogInformation("=== Groups Fluent API Example ===");

    try
    {
        // Example 1: Get active groups with members
        var activeGroups = await client.Fluent().Groups
            .Active()
            .WithMinimumMembers(5)
            .OrderBy(g => g.Name)
            .GetPagedAsync(25);

        logger.LogInformation("Found {Count} active groups with 5+ members", activeGroups.Data.Count);

        // Example 2: Find groups by type and location
        var smallGroupsByType = await client.Fluent().Groups
            .ByGroupType("small-group")
            .ByLocation("main-campus")
            .Active()
            .GetAllAsync();

        logger.LogInformation("Found {Count} small groups at main campus", smallGroupsByType.Count);

        // Example 3: Find groups with chat enabled
        var chatEnabledGroups = await client.Fluent().Groups
            .WithChatEnabled()
            .Active()
            .OrderByDescending(g => g.MembershipsCount)
            .GetPagedAsync(50);

        logger.LogInformation("Found {Count} groups with chat enabled", chatEnabledGroups.Data.Count);

        // Example 4: Search groups by name
        var youthGroups = await client.Fluent().Groups
            .ByNameContains("Youth")
            .Active()
            .GetAllAsync();

        logger.LogInformation("Found {Count} youth groups", youthGroups.Count);

        // Example 5: Find groups with virtual meeting capabilities
        var virtualGroups = await client.Fluent().Groups
            .WithVirtualMeeting()
            .Active()
            .CountAsync();

        logger.LogInformation("Total groups with virtual meeting: {Count}", virtualGroups);

        // Example 6: Find medium-sized groups (between 10-30 members)
        var mediumGroups = await client.Fluent().Groups
            .WithMinimumMembers(10)
            .WithMaximumMembers(30)
            .Active()
            .OrderBy(g => g.MembershipsCount)
            .GetAllAsync();

        logger.LogInformation("Found {Count} medium-sized groups (10-30 members)", mediumGroups.Count);
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Groups fluent API example failed (this is expected if not authenticated)");
    }
}

static async Task ServicesFluentApiExample(IPlanningCenterClient client, ILogger logger)
{
    logger.LogInformation("=== Services Fluent API Example ===");

    try
    {
        // Example 1: Get upcoming service plans
        var upcomingServices = await client.Fluent().Services
            .Upcoming()
            .OrderBy(p => p.SortDate)
            .GetPagedAsync(25);

        logger.LogInformation("Found {Count} upcoming service plans", upcomingServices.Data.Count);

        // Example 2: Find service plans by type
        var sundayServices = await client.Fluent().Services
            .ByServiceType("sunday-morning")
            .ThisMonth()
            .GetAllAsync();

        logger.LogInformation("Found {Count} Sunday morning services this month", sundayServices.Count);

        // Example 3: Get public service plans this week
        var publicServices = await client.Fluent().Services
            .Public()
            .ThisWeek()
            .OrderBy(p => p.SortDate)
            .GetAllAsync();

        logger.LogInformation("Found {Count} public services this week", publicServices.Count);

        // Example 4: Find longer service plans
        var longServices = await client.Fluent().Services
            .WithMinimumLength(90) // 90+ minutes
            .Upcoming()
            .GetAllAsync();

        logger.LogInformation("Found {Count} upcoming services 90+ minutes long", longServices.Count);

        // Example 5: Search service plans by title
        var christmasServices = await client.Fluent().Services
            .ByTitleContains("Christmas")
            .ByDateRange(DateTime.Now.AddMonths(-1), DateTime.Now.AddMonths(1))
            .GetAllAsync();

        logger.LogInformation("Found {Count} Christmas-related services", christmasServices.Count);

        // Example 6: Get past service plans count
        var pastServicesCount = await client.Fluent().Services
            .Past()
            .CountAsync();

        logger.LogInformation("Total past service plans: {Count}", pastServicesCount);

        // Example 7: Check if any private services exist this week
        var hasPrivateServices = await client.Fluent().Services
            .Private()
            .ThisWeek()
            .AnyAsync();

        logger.LogInformation("Has private services this week: {HasPrivate}", hasPrivateServices);

        // Example 8: Get the next upcoming service
        var nextService = await client.Fluent().Services
            .Upcoming()
            .OrderBy(p => p.SortDate)
            .FirstOrDefaultAsync();

        if (nextService != null)
        {
            logger.LogInformation("Next service: {Title} on {Date}", 
                nextService.Title, nextService.SortDate?.ToString("yyyy-MM-dd"));
        }
        else
        {
            logger.LogInformation("No upcoming services found");
        }
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Services fluent API example failed (this is expected if not authenticated)");
    }
}

static async Task CheckInsFluentApiExample(IPlanningCenterClient client, ILogger logger)
{
    logger.LogInformation("=== CheckIns Fluent API Example ===");

    try
    {
        // Example 1: Get today's check-ins
        var todayCheckIns = await client.Fluent().CheckIns
            .Today()
            .OrderBy(c => c.CreatedAt)
            .GetPagedAsync(25);

        logger.LogInformation("Found {Count} check-ins today", todayCheckIns.Data.Count);

        // Example 2: Find check-ins for a specific person
        var personCheckIns = await client.Fluent().CheckIns
            .ByPerson("12345")
            .OrderByDescending(c => c.CreatedAt)
            .GetPagedAsync(10);

        logger.LogInformation("Found {Count} check-ins for person", personCheckIns.Data.Count);

        // Example 3: Get check-ins for a specific event
        var eventCheckIns = await client.Fluent().CheckIns
            .ByEvent("event-123")
            .CheckedIn() // Only currently checked in
            .OrderBy(c => c.FirstName)
            .ThenBy(c => c.LastName)
            .GetAllAsync();

        logger.LogInformation("Found {Count} currently checked in for event", eventCheckIns.Count);

        // Example 4: Find check-ins with medical notes
        var medicalCheckIns = await client.Fluent().CheckIns
            .WithMedicalNotes()
            .ThisWeek()
            .GetAllAsync();

        logger.LogInformation("Found {Count} check-ins with medical notes this week", medicalCheckIns.Count);

        // Example 5: Get guests vs members for today
        var guestsToday = await client.Fluent().CheckIns
            .Today()
            .Guests()
            .CountAsync();

        var membersToday = await client.Fluent().CheckIns
            .Today()
            .Members()
            .CountAsync();

        logger.LogInformation("Today: {Guests} guests, {Members} members", guestsToday, membersToday);

        // Example 6: Find check-ins by name
        var smithCheckIns = await client.Fluent().CheckIns
            .ByNameContains("Smith")
            .ByDateRange(DateTime.Now.AddDays(-7), DateTime.Now)
            .OrderByDescending(c => c.CreatedAt)
            .GetPagedAsync(20);

        logger.LogInformation("Found {Count} check-ins for people with 'Smith' in name", smithCheckIns.Data.Count);

        // Example 7: Get confirmed vs unconfirmed check-ins
        var confirmedCount = await client.Fluent().CheckIns
            .Today()
            .Confirmed()
            .CountAsync();

        var unconfirmedCount = await client.Fluent().CheckIns
            .Today()
            .Unconfirmed()
            .CountAsync();

        logger.LogInformation("Today: {Confirmed} confirmed, {Unconfirmed} unconfirmed", 
            confirmedCount, unconfirmedCount);

        // Example 8: Find check-ins by location and kind
        var locationCheckIns = await client.Fluent().CheckIns
            .ByLocation("main-room")
            .ByKind("volunteer")
            .ThisWeek()
            .GetAllAsync();

        logger.LogInformation("Found {Count} volunteer check-ins at main room this week", locationCheckIns.Count);

        // Example 9: Get checked out people
        var checkedOutToday = await client.Fluent().CheckIns
            .Today()
            .CheckedOut()
            .OrderByDescending(c => c.CheckedOutAt)
            .GetPagedAsync(15);

        logger.LogInformation("Found {Count} people checked out today", checkedOutToday.Data.Count);

        // Example 10: Check if any check-ins exist for an event time
        var hasEventTimeCheckIns = await client.Fluent().CheckIns
            .ByEventTime("event-time-456")
            .AnyAsync();

        logger.LogInformation("Event time has check-ins: {HasCheckIns}", hasEventTimeCheckIns);

        // Example 11: Get the most recent check-in
        var mostRecentCheckIn = await client.Fluent().CheckIns
            .OrderByDescending(c => c.CreatedAt)
            .FirstOrDefaultAsync();

        if (mostRecentCheckIn != null)
        {
            logger.LogInformation("Most recent check-in: {Name} at {Time}", 
                $"{mostRecentCheckIn.FirstName} {mostRecentCheckIn.LastName}",
                mostRecentCheckIn.CreatedAt?.ToString("yyyy-MM-dd HH:mm"));
        }
        else
        {
            logger.LogInformation("No check-ins found");
        }

        // Example 12: Stream check-ins for memory-efficient processing
        var processedCount = 0;
        await foreach (var checkIn in client.Fluent().CheckIns
            .Today()
            .AsAsyncEnumerable())
        {
            // Process each check-in individually
            logger.LogDebug("Processing check-in: {Name}", $"{checkIn.FirstName} {checkIn.LastName}");
            processedCount++;

            // Break after processing 5 for demo purposes
            if (processedCount >= 5) break;
        }

        logger.LogInformation("Processed {Count} check-ins via streaming", processedCount);
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "CheckIns fluent API example failed (this is expected if not authenticated)");
    }
}
