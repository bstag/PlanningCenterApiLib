using PlanningCenter.Api.Client.Fluent;
using PlanningCenter.Api.Client.Models;

namespace PlanningCenter.Api.Client.Extensions;

/// <summary>
/// Extension methods to add fluent API support to the Planning Center client.
/// </summary>
public static class PlanningCenterClientExtensions
{
    /// <summary>
    /// Gets the fluent API interface for the Planning Center client.
    /// Provides LINQ-like syntax for querying and manipulating data.
    /// </summary>
    /// <param name="client">The Planning Center client</param>
    /// <returns>A fluent API client with LINQ-like syntax</returns>
    public static IPlanningCenterFluentClient Fluent(this IPlanningCenterClient client)
    {
        if (client == null) throw new ArgumentNullException(nameof(client));
        
        return new PlanningCenterFluentClient(client);
    }
}