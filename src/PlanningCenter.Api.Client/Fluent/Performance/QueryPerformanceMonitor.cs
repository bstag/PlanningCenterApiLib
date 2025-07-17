using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using PlanningCenter.Api.Client.Fluent.QueryBuilder;

namespace PlanningCenter.Api.Client.Fluent.Performance;

/// <summary>
/// Monitors and tracks query performance metrics for optimization insights.
/// </summary>
public class QueryPerformanceMonitor
{
    private static readonly ConcurrentDictionary<string, QueryPerformanceMetrics> _queryMetrics = new();
    private static readonly ConcurrentDictionary<string, List<QueryExecutionInfo>> _recentExecutions = new();
    private static readonly object _lock = new();

    /// <summary>
    /// Tracks the execution of a query and measures performance.
    /// </summary>
    public static async Task<T> TrackQueryExecution<T>(
        string queryName,
        Func<Task<T>> queryExecution,
        QueryOptimizationInfo? optimizationInfo = null)
    {
        var stopwatch = Stopwatch.StartNew();
        var startTime = DateTime.UtcNow;
        Exception? exception = null;

        try
        {
            var result = await queryExecution();
            stopwatch.Stop();
            
            RecordExecution(queryName, stopwatch.ElapsedMilliseconds, true, optimizationInfo, exception);
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            exception = ex;
            RecordExecution(queryName, stopwatch.ElapsedMilliseconds, false, optimizationInfo, exception);
            throw;
        }
    }

    /// <summary>
    /// Records query execution metrics.
    /// </summary>
    private static void RecordExecution(
        string queryName,
        long executionTimeMs,
        bool success,
        QueryOptimizationInfo? optimizationInfo,
        Exception? exception)
    {
        var executionInfo = new QueryExecutionInfo
        {
            QueryName = queryName,
            ExecutionTimeMs = executionTimeMs,
            Success = success,
            Timestamp = DateTime.UtcNow,
            OptimizationInfo = optimizationInfo,
            Exception = exception
        };

        // Update aggregated metrics
        _queryMetrics.AddOrUpdate(queryName,
            new QueryPerformanceMetrics
            {
                QueryName = queryName,
                TotalExecutions = 1,
                SuccessfulExecutions = success ? 1 : 0,
                TotalExecutionTimeMs = executionTimeMs,
                MinExecutionTimeMs = executionTimeMs,
                MaxExecutionTimeMs = executionTimeMs,
                LastExecutionTime = DateTime.UtcNow,
                AverageExecutionTimeMs = executionTimeMs
            },
            (key, existing) =>
            {
                existing.TotalExecutions++;
                if (success) existing.SuccessfulExecutions++;
                existing.TotalExecutionTimeMs += executionTimeMs;
                existing.MinExecutionTimeMs = Math.Min(existing.MinExecutionTimeMs, executionTimeMs);
                existing.MaxExecutionTimeMs = Math.Max(existing.MaxExecutionTimeMs, executionTimeMs);
                existing.AverageExecutionTimeMs = existing.TotalExecutionTimeMs / existing.TotalExecutions;
                existing.LastExecutionTime = DateTime.UtcNow;
                return existing;
            });

        // Store recent executions (keep last 100 per query)
        _recentExecutions.AddOrUpdate(queryName,
            new List<QueryExecutionInfo> { executionInfo },
            (key, existing) =>
            {
                lock (_lock)
                {
                    existing.Add(executionInfo);
                    if (existing.Count > 100)
                    {
                        existing.RemoveAt(0);
                    }
                }
                return existing;
            });
    }

    /// <summary>
    /// Gets performance metrics for a specific query.
    /// </summary>
    public static QueryPerformanceMetrics? GetQueryMetrics(string queryName)
    {
        return _queryMetrics.TryGetValue(queryName, out var metrics) ? metrics : null;
    }

    /// <summary>
    /// Gets performance metrics for all queries.
    /// </summary>
    public static IReadOnlyDictionary<string, QueryPerformanceMetrics> GetAllMetrics()
    {
        return _queryMetrics.ToImmutableDictionary();
    }

    /// <summary>
    /// Gets recent execution history for a specific query.
    /// </summary>
    public static List<QueryExecutionInfo> GetRecentExecutions(string queryName, int limit = 10)
    {
        if (!_recentExecutions.TryGetValue(queryName, out var executions))
            return new List<QueryExecutionInfo>();

        lock (_lock)
        {
            return executions.TakeLast(limit).ToList();
        }
    }

    /// <summary>
    /// Gets queries that may need optimization based on performance metrics.
    /// </summary>
    public static List<QueryOptimizationRecommendation> GetOptimizationRecommendations()
    {
        var recommendations = new List<QueryOptimizationRecommendation>();

        foreach (var (queryName, metrics) in _queryMetrics)
        {
            var recommendation = new QueryOptimizationRecommendation
            {
                QueryName = queryName,
                Priority = OptimizationPriority.Low,
                Issues = new List<string>(),
                Recommendations = new List<string>()
            };

            // Check for slow queries
            if (metrics.AverageExecutionTimeMs > 5000) // 5 seconds
            {
                recommendation.Priority = OptimizationPriority.High;
                recommendation.Issues.Add($"High average execution time: {metrics.AverageExecutionTimeMs}ms");
                recommendation.Recommendations.Add("Consider adding database indexes or optimizing query conditions");
            }
            else if (metrics.AverageExecutionTimeMs > 2000) // 2 seconds
            {
                recommendation.Priority = OptimizationPriority.Medium;
                recommendation.Issues.Add($"Moderate execution time: {metrics.AverageExecutionTimeMs}ms");
                recommendation.Recommendations.Add("Review query complexity and consider pagination");
            }

            // Check for high failure rates
            var failureRate = (double)(metrics.TotalExecutions - metrics.SuccessfulExecutions) / metrics.TotalExecutions;
            if (failureRate > 0.1) // 10% failure rate
            {
                recommendation.Priority = OptimizationPriority.High;
                recommendation.Issues.Add($"High failure rate: {failureRate:P}");
                recommendation.Recommendations.Add("Review error handling and query robustness");
            }

            // Check for queries with high variance in execution time
            if (metrics.MaxExecutionTimeMs > metrics.MinExecutionTimeMs * 5)
            {
                recommendation.Priority = (OptimizationPriority)Math.Max((int)recommendation.Priority, (int)OptimizationPriority.Medium);
                recommendation.Issues.Add($"High execution time variance: {metrics.MinExecutionTimeMs}ms - {metrics.MaxExecutionTimeMs}ms");
                recommendation.Recommendations.Add("Investigate query consistency and potential caching opportunities");
            }

            // Check for frequently executed queries
            if (metrics.TotalExecutions > 1000 && metrics.AverageExecutionTimeMs > 1000)
            {
                recommendation.Priority = (OptimizationPriority)Math.Max((int)recommendation.Priority, (int)OptimizationPriority.Medium);
                recommendation.Issues.Add($"Frequently executed slow query: {metrics.TotalExecutions} executions");
                recommendation.Recommendations.Add("Consider implementing caching or query result optimization");
            }

            if (recommendation.Issues.Any())
            {
                recommendations.Add(recommendation);
            }
        }

        return recommendations.OrderByDescending(r => r.Priority).ThenByDescending(r => r.Issues.Count).ToList();
    }

    /// <summary>
    /// Generates a performance report for all queries.
    /// </summary>
    public static QueryPerformanceReport GenerateReport()
    {
        var allMetrics = GetAllMetrics();
        var totalQueries = allMetrics.Count;
        var totalExecutions = allMetrics.Values.Sum(m => m.TotalExecutions);
        var totalSuccessful = allMetrics.Values.Sum(m => m.SuccessfulExecutions);
        var averageExecutionTime = allMetrics.Values.Average(m => m.AverageExecutionTimeMs);

        var slowestQueries = allMetrics.Values
            .OrderByDescending(m => m.AverageExecutionTimeMs)
            .Take(10)
            .ToList();

        var mostFrequentQueries = allMetrics.Values
            .OrderByDescending(m => m.TotalExecutions)
            .Take(10)
            .ToList();

        var recommendations = GetOptimizationRecommendations();

        return new QueryPerformanceReport
        {
            GeneratedAt = DateTime.UtcNow,
            TotalQueries = totalQueries,
            TotalExecutions = totalExecutions,
            OverallSuccessRate = totalExecutions > 0 ? (double)totalSuccessful / totalExecutions : 1.0,
            AverageExecutionTime = averageExecutionTime,
            SlowestQueries = slowestQueries,
            MostFrequentQueries = mostFrequentQueries,
            OptimizationRecommendations = recommendations
        };
    }

    /// <summary>
    /// Clears all performance metrics and history.
    /// </summary>
    public static void ClearMetrics()
    {
        _queryMetrics.Clear();
        _recentExecutions.Clear();
    }

    /// <summary>
    /// Clears metrics older than the specified time span.
    /// </summary>
    public static void ClearOldMetrics(TimeSpan maxAge)
    {
        var cutoff = DateTime.UtcNow - maxAge;
        
        var keysToRemove = _queryMetrics
            .Where(kvp => kvp.Value.LastExecutionTime < cutoff)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in keysToRemove)
        {
            _queryMetrics.TryRemove(key, out _);
            _recentExecutions.TryRemove(key, out _);
        }
    }
}

/// <summary>
/// Represents performance metrics for a specific query.
/// </summary>
public class QueryPerformanceMetrics
{
    public string QueryName { get; set; } = string.Empty;
    public long TotalExecutions { get; set; }
    public long SuccessfulExecutions { get; set; }
    public long TotalExecutionTimeMs { get; set; }
    public double AverageExecutionTimeMs { get; set; }
    public long MinExecutionTimeMs { get; set; }
    public long MaxExecutionTimeMs { get; set; }
    public DateTime LastExecutionTime { get; set; }
    
    public double SuccessRate => TotalExecutions > 0 ? (double)SuccessfulExecutions / TotalExecutions : 1.0;
    public long FailedExecutions => TotalExecutions - SuccessfulExecutions;
}

/// <summary>
/// Represents detailed information about a specific query execution.
/// </summary>
public class QueryExecutionInfo
{
    public string QueryName { get; set; } = string.Empty;
    public long ExecutionTimeMs { get; set; }
    public bool Success { get; set; }
    public DateTime Timestamp { get; set; }
    public QueryOptimizationInfo? OptimizationInfo { get; set; }
    public Exception? Exception { get; set; }
}

/// <summary>
/// Represents optimization recommendations for a query.
/// </summary>
public class QueryOptimizationRecommendation
{
    public string QueryName { get; set; } = string.Empty;
    public OptimizationPriority Priority { get; set; }
    public List<string> Issues { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
}

/// <summary>
/// Represents a comprehensive performance report.
/// </summary>
public class QueryPerformanceReport
{
    public DateTime GeneratedAt { get; set; }
    public int TotalQueries { get; set; }
    public long TotalExecutions { get; set; }
    public double OverallSuccessRate { get; set; }
    public double AverageExecutionTime { get; set; }
    public List<QueryPerformanceMetrics> SlowestQueries { get; set; } = new();
    public List<QueryPerformanceMetrics> MostFrequentQueries { get; set; } = new();
    public List<QueryOptimizationRecommendation> OptimizationRecommendations { get; set; } = new();
}

/// <summary>
/// Priority levels for optimization recommendations.
/// </summary>
public enum OptimizationPriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}