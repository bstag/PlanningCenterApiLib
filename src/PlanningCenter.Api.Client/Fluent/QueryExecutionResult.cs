using PlanningCenter.Api.Client.Fluent.QueryBuilder;
using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;

namespace PlanningCenter.Api.Client.Fluent;

/// <summary>
/// Represents the result of executing a fluent query with debug and performance information.
/// </summary>
/// <typeparam name="T">The type of data returned by the query</typeparam>
public class QueryExecutionResult<T>
{
    /// <summary>
    /// The data returned by the query (null if the query failed).
    /// </summary>
    public IPagedResponse<T>? Data { get; set; }

    /// <summary>
    /// The time it took to execute the query.
    /// </summary>
    public TimeSpan ExecutionTime { get; set; }

    /// <summary>
    /// Information about query optimization and performance.
    /// </summary>
    public QueryOptimizationInfo OptimizationInfo { get; set; } = new();

    /// <summary>
    /// Whether the query executed successfully.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The error that occurred during query execution (null if successful).
    /// </summary>
    public Exception? Error { get; set; }

    /// <summary>
    /// Gets a human-readable summary of the query execution.
    /// </summary>
    public string GetExecutionSummary()
    {
        if (!Success)
        {
            return $"Query failed after {ExecutionTime.TotalMilliseconds:F2}ms: {Error?.Message}";
        }

        var resultCount = Data?.Data.Count ?? 0;
        var totalCount = Data?.Meta?.TotalCount ?? 0;
        var complexity = OptimizationInfo.EstimatedComplexity;
        var cacheHitRate = OptimizationInfo.CacheHitRate;

        return $"Query executed successfully in {ExecutionTime.TotalMilliseconds:F2}ms. " +
               $"Returned {resultCount} of {totalCount} total results. " +
               $"Complexity: {complexity}, Cache hit rate: {cacheHitRate:P1}";
    }

    /// <summary>
    /// Gets performance recommendations based on the execution results.
    /// </summary>
    public string GetPerformanceRecommendations()
    {
        var recommendations = new List<string>();

        if (ExecutionTime.TotalMilliseconds > 5000) // 5 seconds
        {
            recommendations.Add("Query took longer than 5 seconds - consider adding more specific filters");
        }

        if (ExecutionTime.TotalMilliseconds > 1000) // 1 second
        {
            recommendations.Add("Query took longer than 1 second - consider optimizing filters or reducing data");
        }

        if (OptimizationInfo.CacheHitRate < 0.5)
        {
            recommendations.Add("Low cache hit rate - consider reusing similar query patterns");
        }

        if (OptimizationInfo.EstimatedComplexity >= QueryComplexity.Complex)
        {
            recommendations.Add("Complex query detected - " + OptimizationInfo.GetRecommendations());
        }

        var totalResults = Data?.Meta?.TotalCount ?? 0;
        var returnedResults = Data?.Data.Count ?? 0;

        if (totalResults > 1000 && returnedResults == totalResults)
        {
            recommendations.Add("Large result set returned - consider using pagination or streaming");
        }

        return recommendations.Any() 
            ? string.Join("; ", recommendations)
            : "Query performance is optimal";
    }

    /// <summary>
    /// Gets a detailed performance report.
    /// </summary>
    public QueryPerformanceReport GetDetailedReport()
    {
        return new QueryPerformanceReport
        {
            ExecutionTime = ExecutionTime,
            Success = Success,
            Error = Error?.Message,
            ResultCount = Data?.Data.Count ?? 0,
            TotalCount = Data?.Meta?.TotalCount ?? 0,
            QueryComplexity = OptimizationInfo.EstimatedComplexity,
            CacheHitRate = OptimizationInfo.CacheHitRate,
            WhereConditionsCount = OptimizationInfo.WhereConditionsCount,
            IncludeExpressionsCount = OptimizationInfo.IncludeExpressionsCount,
            OrderByExpressionsCount = OptimizationInfo.OrderByExpressionsCount,
            CustomParametersCount = OptimizationInfo.CustomParametersCount,
            ExecutionSummary = GetExecutionSummary(),
            PerformanceRecommendations = GetPerformanceRecommendations()
        };
    }
}

/// <summary>
/// Detailed performance report for a query execution.
/// </summary>
public class QueryPerformanceReport
{
    public TimeSpan ExecutionTime { get; set; }
    public bool Success { get; set; }
    public string? Error { get; set; }
    public int ResultCount { get; set; }
    public int TotalCount { get; set; }
    public QueryComplexity QueryComplexity { get; set; }
    public double CacheHitRate { get; set; }
    public int WhereConditionsCount { get; set; }
    public int IncludeExpressionsCount { get; set; }
    public int OrderByExpressionsCount { get; set; }
    public int CustomParametersCount { get; set; }
    public string ExecutionSummary { get; set; } = string.Empty;
    public string PerformanceRecommendations { get; set; } = string.Empty;

    /// <summary>
    /// Converts the report to a JSON-like string for logging or debugging.
    /// </summary>
    public override string ToString()
    {
        return $@"{{
  ""ExecutionTime"": ""{ExecutionTime.TotalMilliseconds:F2}ms"",
  ""Success"": {Success.ToString().ToLower()},
  ""Error"": ""{Error}"",
  ""ResultCount"": {ResultCount},
  ""TotalCount"": {TotalCount},
  ""QueryComplexity"": ""{QueryComplexity}"",
  ""CacheHitRate"": ""{CacheHitRate:P1}"",
  ""WhereConditionsCount"": {WhereConditionsCount},
  ""IncludeExpressionsCount"": {IncludeExpressionsCount},
  ""OrderByExpressionsCount"": {OrderByExpressionsCount},
  ""CustomParametersCount"": {CustomParametersCount},
  ""ExecutionSummary"": ""{ExecutionSummary}"",
  ""PerformanceRecommendations"": ""{PerformanceRecommendations}""
}}";
    }
}