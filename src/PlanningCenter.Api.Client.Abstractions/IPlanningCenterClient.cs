using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Abstractions;

/// <summary>
/// Main client interface for the Planning Center SDK.
/// Provides access to all modules through both service-based and fluent APIs.
/// </summary>
public interface IPlanningCenterClient
{
    // Fluent API access to all modules
    
    /// <summary>
    /// Access to the People module via fluent API.
    /// Provides LINQ-like syntax for querying and manipulating people data.
    /// </summary>
    IPeopleFluentContext People();
    
    /// <summary>
    /// Access to the Giving module via fluent API.
    /// Provides LINQ-like syntax for querying donation and financial data.
    /// </summary>
    IGivingFluentContext Giving();
    
    /// <summary>
    /// Access to the Calendar module via fluent API.
    /// Provides LINQ-like syntax for querying events and calendar data.
    /// </summary>
    ICalendarFluentContext Calendar();
    
    /// <summary>
    /// Access to the Check-Ins module via fluent API.
    /// Provides LINQ-like syntax for querying attendance and check-in data.
    /// </summary>
    ICheckInsFluentContext CheckIns();
    
    /// <summary>
    /// Access to the Groups module via fluent API.
    /// Provides LINQ-like syntax for querying group and membership data.
    /// </summary>
    IGroupsFluentContext Groups();
    
    /// <summary>
    /// Access to the Registrations module via fluent API.
    /// Provides LINQ-like syntax for querying registration and event data.
    /// </summary>
    IRegistrationsFluentContext Registrations();
    
    /// <summary>
    /// Access to the Publishing module via fluent API.
    /// Provides LINQ-like syntax for querying media and content data.
    /// </summary>
    IPublishingFluentContext Publishing();
    
    /// <summary>
    /// Access to the Services module via fluent API.
    /// Provides LINQ-like syntax for querying service planning data.
    /// </summary>
    IServicesFluentContext Services();
    
    /// <summary>
    /// Access to the Webhooks module via fluent API.
    /// Provides LINQ-like syntax for managing webhook subscriptions.
    /// </summary>
    IWebhooksFluentContext Webhooks();
    
    // Global operations
    
    /// <summary>
    /// Checks the health and connectivity of the Planning Center API.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Health check results</returns>
    Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets current API rate limit information.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>API rate limit information</returns>
    Task<ApiLimitsInfo> GetApiLimitsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets information about the current user and their permissions.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Current user information</returns>
    Task<CurrentUserInfo> GetCurrentUserAsync(CancellationToken cancellationToken = default);
}


/// <summary>
/// API rate limit information.
/// </summary>
public class ApiLimitsInfo
{
    /// <summary>
    /// Maximum requests allowed per time period
    /// </summary>
    public int RateLimit { get; set; }
    
    /// <summary>
    /// Remaining requests in current time period
    /// </summary>
    public int RemainingRequests { get; set; }
    
    /// <summary>
    /// When the rate limit resets
    /// </summary>
    public DateTime ResetTime { get; set; }
    
    /// <summary>
    /// Time period for the rate limit (e.g., "hour", "minute")
    /// </summary>
    public string TimePeriod { get; set; } = "hour";
    
    /// <summary>
    /// Percentage of rate limit used
    /// </summary>
    public double UsagePercentage => RateLimit > 0 ? (double)(RateLimit - RemainingRequests) / RateLimit * 100 : 0;
}

/// <summary>
/// Information about the current authenticated user.
/// </summary>
public class CurrentUserInfo
{
    /// <summary>
    /// User's unique identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// User's display name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// User's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// User's permissions and scopes
    /// </summary>
    public List<string> Permissions { get; set; } = new();
    
    /// <summary>
    /// Organizations the user has access to
    /// </summary>
    public List<string> Organizations { get; set; } = new();
    
    /// <summary>
    /// Additional user metadata
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}