using PlanningCenter.Api.Client.Fluent.QueryBuilder;

namespace PlanningCenter.Api.Client.Fluent.Performance;

/// <summary>
/// Performance monitoring extensions for fluent API contexts.
/// </summary>
public static class FluentPerformanceExtensions
{
    /// <summary>
    /// Tracks the execution of a fluent query operation with performance monitoring.
    /// </summary>
    public static async Task<T> TrackFluentExecution<T>(
        this FluentQueryBuilder<object> queryBuilder,
        string operationName,
        Func<Task<T>> operation,
        CancellationToken cancellationToken = default)
    {
        var optimizationInfo = queryBuilder.GetOptimizationInfo();
        
        return await QueryPerformanceMonitor.TrackQueryExecution(
            operationName,
            operation,
            optimizationInfo);
    }

    /// <summary>
    /// Gets performance metrics for a specific operation pattern.
    /// </summary>
    public static QueryPerformanceMetrics? GetPerformanceMetrics(string operationName)
    {
        return QueryPerformanceMonitor.GetQueryMetrics(operationName);
    }

    /// <summary>
    /// Gets all performance metrics for fluent operations.
    /// </summary>
    public static IReadOnlyDictionary<string, QueryPerformanceMetrics> GetAllFluentMetrics()
    {
        return QueryPerformanceMonitor.GetAllMetrics();
    }

    /// <summary>
    /// Gets performance optimization recommendations for fluent operations.
    /// </summary>
    public static List<QueryOptimizationRecommendation> GetFluentOptimizationRecommendations()
    {
        return QueryPerformanceMonitor.GetOptimizationRecommendations();
    }

    /// <summary>
    /// Generates a comprehensive performance report for all fluent operations.
    /// </summary>
    public static QueryPerformanceReport GenerateFluentPerformanceReport()
    {
        return QueryPerformanceMonitor.GenerateReport();
    }

    /// <summary>
    /// Clears all performance metrics for fluent operations.
    /// </summary>
    public static void ClearFluentPerformanceMetrics()
    {
        QueryPerformanceMonitor.ClearMetrics();
    }
}