using AutoFixture;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using PlanningCenter.Api.Client.Models;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Utilities;

/// <summary>
/// Common test utilities and helpers for unit testing.
/// </summary>
public static class TestHelpers
{
    /// <summary>
    /// Creates a configured AutoFixture instance for generating test data.
    /// </summary>
    public static IFixture CreateFixture()
    {
        var fixture = new Fixture();
        
        // Configure AutoFixture to handle common scenarios
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        
        // Customize string generation to avoid null/empty values
        fixture.Customize<string>(composer => composer.FromFactory(() => 
            fixture.Create<Guid>().ToString()));
        
        return fixture;
    }
    
    /// <summary>
    /// Creates a test logger for the specified type.
    /// </summary>
    public static ILogger<T> CreateTestLogger<T>()
    {
        return NullLogger<T>.Instance;
    }
    
    /// <summary>
    /// Creates test options for PlanningCenterOptions.
    /// </summary>
    public static IOptions<PlanningCenterOptions> CreateTestOptions(
        Action<PlanningCenterOptions>? configure = null)
    {
        var options = new PlanningCenterOptions
        {
            BaseUrl = "https://api.test.planningcenteronline.com",
            PersonalAccessToken = "test-app-id:test-secret",
            RequestTimeout = TimeSpan.FromSeconds(10),
            MaxRetryAttempts = 2,
            RetryBaseDelay = TimeSpan.FromMilliseconds(100),
            EnableCaching = false, // Disable caching in tests by default
            EnableDetailedLogging = true
        };
        
        configure?.Invoke(options);
        
        return Options.Create(options);
    }
    
    /// <summary>
    /// Creates a test QueryParameters object with common test values.
    /// </summary>
    public static QueryParameters CreateTestQueryParameters()
    {
        return new QueryParameters
        {
            Where = new Dictionary<string, object>
            {
                ["status"] = "active",
                ["test_field"] = "test_value"
            },
            Include = new[] { "addresses", "emails" },
            Order = "created_at",
            PerPage = 25,
            Offset = 0
        };
    }
    
    /// <summary>
    /// Creates a test PaginationOptions object.
    /// </summary>
    public static PaginationOptions CreateTestPaginationOptions(int? maxItems = null)
    {
        return new PaginationOptions
        {
            MaxItems = maxItems ?? 100,
            PageSize = 25,
            MaxConcurrentRequests = 3
        };
    }
    
    /// <summary>
    /// Asserts that an action throws a specific exception type with a specific message.
    /// </summary>
    public static async Task AssertThrowsAsync<TException>(
        Func<Task> action, 
        string expectedMessage) 
        where TException : Exception
    {
        var exception = await Assert.ThrowsAsync<TException>(action);
        Assert.Contains(expectedMessage, exception.Message);
    }
    
    /// <summary>
    /// Asserts that an action throws a specific exception type.
    /// </summary>
    public static void AssertThrows<TException>(
        Action action, 
        string expectedMessage) 
        where TException : Exception
    {
        var exception = Assert.Throws<TException>(action);
        Assert.Contains(expectedMessage, exception.Message);
    }
    
    /// <summary>
    /// Creates a test DateTime that's deterministic for testing.
    /// </summary>
    public static DateTime CreateTestDateTime()
    {
        return new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);
    }
    
    /// <summary>
    /// Creates a test GUID that's deterministic for testing.
    /// </summary>
    public static Guid CreateTestGuid()
    {
        return new Guid("12345678-1234-1234-1234-123456789012");
    }
    
    /// <summary>
    /// Waits for a condition to be true with a timeout.
    /// Useful for testing async operations.
    /// </summary>
    public static async Task WaitForConditionAsync(
        Func<bool> condition, 
        TimeSpan timeout, 
        TimeSpan? interval = null)
    {
        var checkInterval = interval ?? TimeSpan.FromMilliseconds(10);
        var endTime = DateTime.UtcNow.Add(timeout);
        
        while (DateTime.UtcNow < endTime)
        {
            if (condition())
                return;
                
            await Task.Delay(checkInterval);
        }
        
        throw new TimeoutException($"Condition was not met within {timeout}");
    }
}