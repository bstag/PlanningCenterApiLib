using System.Linq.Expressions;
using PlanningCenter.Api.Client.Fluent.ExpressionParsing;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Fluent.QueryBuilder;

/// <summary>
/// Advanced query builder that optimizes and caches expression parsing for better performance.
/// Provides intelligent query optimization and parameter building.
/// </summary>
public class FluentQueryBuilder<T> where T : class
{
    private readonly List<Expression<Func<T, bool>>> _whereConditions = new();
    private readonly List<Expression<Func<T, object>>> _includeExpressions = new();
    private readonly List<(Expression<Func<T, object>> Expression, bool Descending)> _orderByExpressions = new();
    private readonly Dictionary<string, object> _customParameters = new();
    
    // Caching for parsed expressions to improve performance
    private static readonly Dictionary<string, FilterResult> _filterCache = new();
    private static readonly Dictionary<string, string> _includeCache = new();
    private static readonly Dictionary<string, string> _sortCache = new();

    public FluentQueryBuilder<T> Where(Expression<Func<T, bool>> predicate)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        _whereConditions.Add(predicate);
        return this;
    }

    public FluentQueryBuilder<T> Include(Expression<Func<T, object>> include)
    {
        if (include == null) throw new ArgumentNullException(nameof(include));
        
        _includeExpressions.Add(include);
        return this;
    }

    public FluentQueryBuilder<T> OrderBy(Expression<Func<T, object>> orderBy)
    {
        if (orderBy == null) throw new ArgumentNullException(nameof(orderBy));
        
        _orderByExpressions.Clear();
        _orderByExpressions.Add((orderBy, false));
        return this;
    }

    public FluentQueryBuilder<T> OrderByDescending(Expression<Func<T, object>> orderBy)
    {
        if (orderBy == null) throw new ArgumentNullException(nameof(orderBy));
        
        _orderByExpressions.Clear();
        _orderByExpressions.Add((orderBy, true));
        return this;
    }

    public FluentQueryBuilder<T> ThenBy(Expression<Func<T, object>> thenBy)
    {
        if (thenBy == null) throw new ArgumentNullException(nameof(thenBy));
        
        _orderByExpressions.Add((thenBy, false));
        return this;
    }

    public FluentQueryBuilder<T> ThenByDescending(Expression<Func<T, object>> thenBy)
    {
        if (thenBy == null) throw new ArgumentNullException(nameof(thenBy));
        
        _orderByExpressions.Add((thenBy, true));
        return this;
    }

    public FluentQueryBuilder<T> WithParameter(string key, object value)
    {
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Key cannot be null or empty", nameof(key));
        
        _customParameters[key] = value;
        return this;
    }

    public QueryParameters Build()
    {
        var parameters = new QueryParameters();

        // Add custom parameters first
        foreach (var (key, value) in _customParameters)
        {
            parameters.Add(key, value.ToString()!);
        }

        // Process where conditions with caching
        foreach (var condition in _whereConditions)
        {
            var expressionKey = GetExpressionKey(condition);
            
            if (!_filterCache.TryGetValue(expressionKey, out var filterResult))
            {
                filterResult = ParseFilterExpression(condition);
                _filterCache[expressionKey] = filterResult;
            }

            if (!filterResult.IsEmpty)
            {
                parameters.AddFilter(filterResult.Field, filterResult.ToApiFilterString());
            }
        }

        // Process include expressions with caching
        foreach (var include in _includeExpressions)
        {
            var expressionKey = GetExpressionKey(include);
            
            if (!_includeCache.TryGetValue(expressionKey, out var includeField))
            {
                includeField = ParseIncludeExpression(include);
                _includeCache[expressionKey] = includeField;
            }

            if (!string.IsNullOrEmpty(includeField))
            {
                parameters.AddInclude(includeField);
            }
        }

        // Process order by expressions with caching
        foreach (var (expression, descending) in _orderByExpressions)
        {
            var expressionKey = GetExpressionKey(expression);
            
            if (!_sortCache.TryGetValue(expressionKey, out var sortField))
            {
                sortField = ParseSortExpression(expression);
                _sortCache[expressionKey] = sortField;
            }

            if (!string.IsNullOrEmpty(sortField))
            {
                var sortValue = descending ? $"-{sortField}" : sortField;
                parameters.AddSort(sortValue);
            }
        }

        return parameters;
    }

    public QueryOptimizationInfo GetOptimizationInfo()
    {
        return new QueryOptimizationInfo
        {
            WhereConditionsCount = _whereConditions.Count,
            IncludeExpressionsCount = _includeExpressions.Count,
            OrderByExpressionsCount = _orderByExpressions.Count,
            CustomParametersCount = _customParameters.Count,
            EstimatedComplexity = CalculateQueryComplexity(),
            CacheHitRate = CalculateCacheHitRate()
        };
    }

    public FluentQueryBuilder<T> Clone()
    {
        var clone = new FluentQueryBuilder<T>();
        
        clone._whereConditions.AddRange(_whereConditions);
        clone._includeExpressions.AddRange(_includeExpressions);
        clone._orderByExpressions.AddRange(_orderByExpressions);
        
        foreach (var (key, value) in _customParameters)
        {
            clone._customParameters[key] = value;
        }

        return clone;
    }

    private static FilterResult ParseFilterExpression(Expression<Func<T, bool>> expression)
    {
        // For Person type, use the ExpressionParser
        if (typeof(T) == typeof(Person))
        {
            var personExpression = expression as Expression<Func<Person, bool>>;
            return ExpressionParser.ParseFilter(personExpression!);
        }

        // For other types, return empty for now
        // TODO: Implement generic expression parsing for other entity types
        return new FilterResult();
    }

    private static string ParseIncludeExpression(Expression<Func<T, object>> expression)
    {
        // For Person type, use the ExpressionParser
        if (typeof(T) == typeof(Person))
        {
            var personExpression = expression as Expression<Func<Person, object>>;
            return ExpressionParser.ParseInclude(personExpression!);
        }

        // For other types, return empty for now
        return string.Empty;
    }

    private static string ParseSortExpression(Expression<Func<T, object>> expression)
    {
        // For Person type, use the ExpressionParser
        if (typeof(T) == typeof(Person))
        {
            var personExpression = expression as Expression<Func<Person, object>>;
            return ExpressionParser.ParseSort(personExpression!);
        }

        // For other types, return empty for now
        return string.Empty;
    }

    private static string GetExpressionKey(Expression expression)
    {
        // Create a unique key for the expression for caching purposes
        // This is a simplified implementation - in production, you might want a more robust approach
        return expression.ToString();
    }

    private QueryComplexity CalculateQueryComplexity()
    {
        var totalConditions = _whereConditions.Count + _includeExpressions.Count + _orderByExpressions.Count;
        
        return totalConditions switch
        {
            0 => QueryComplexity.Simple,
            <= 3 => QueryComplexity.Moderate,
            <= 7 => QueryComplexity.Complex,
            _ => QueryComplexity.VeryComplex
        };
    }

    private double CalculateCacheHitRate()
    {
        var totalExpressions = _whereConditions.Count + _includeExpressions.Count + _orderByExpressions.Count;
        if (totalExpressions == 0) return 1.0;

        var cacheHits = 0;
        
        foreach (var condition in _whereConditions)
        {
            if (_filterCache.ContainsKey(GetExpressionKey(condition)))
                cacheHits++;
        }
        
        foreach (var include in _includeExpressions)
        {
            if (_includeCache.ContainsKey(GetExpressionKey(include)))
                cacheHits++;
        }
        
        foreach (var (expression, _) in _orderByExpressions)
        {
            if (_sortCache.ContainsKey(GetExpressionKey(expression)))
                cacheHits++;
        }

        return (double)cacheHits / totalExpressions;
    }

    public static void ClearCache()
    {
        _filterCache.Clear();
        _includeCache.Clear();
        _sortCache.Clear();
    }
}

/// <summary>
/// Information about query optimization and performance.
/// </summary>
public class QueryOptimizationInfo
{
    public int WhereConditionsCount { get; set; }
    public int IncludeExpressionsCount { get; set; }
    public int OrderByExpressionsCount { get; set; }
    public int CustomParametersCount { get; set; }
    public QueryComplexity EstimatedComplexity { get; set; }
    public double CacheHitRate { get; set; }

    public bool ShouldOptimize => EstimatedComplexity >= QueryComplexity.Complex || CacheHitRate < 0.5;
    
    public string GetRecommendations()
    {
        var recommendations = new List<string>();

        if (WhereConditionsCount > 5)
        {
            recommendations.Add("Consider combining multiple Where conditions into fewer, more complex expressions");
        }

        if (IncludeExpressionsCount > 3)
        {
            recommendations.Add("Too many includes may impact performance - consider if all related data is needed");
        }

        if (OrderByExpressionsCount > 3)
        {
            recommendations.Add("Complex sorting with many levels may be slow - consider simplifying");
        }

        if (CacheHitRate < 0.5)
        {
            recommendations.Add("Low cache hit rate detected - consider reusing query patterns");
        }

        return recommendations.Any() 
            ? string.Join("; ", recommendations)
            : "Query is well optimized";
    }
}

/// <summary>
/// Represents the complexity level of a query.
/// </summary>
public enum QueryComplexity
{
    Simple,
    Moderate,
    Complex,
    VeryComplex
}