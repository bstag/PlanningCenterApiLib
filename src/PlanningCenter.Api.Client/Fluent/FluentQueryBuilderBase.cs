using System.Collections;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Exceptions;
using PlanningCenter.Api.Client.Services;

namespace PlanningCenter.Api.Client.Fluent;

/// <summary>
/// Base implementation for fluent query builders.
/// </summary>
/// <typeparam name="T">The resource type being queried</typeparam>
/// <typeparam name="TDto">The DTO type returned by the API</typeparam>
public abstract class FluentQueryBuilderBase<T, TDto> : IFluentQueryExecutor<T>
    where TDto : class
    where T : class
{
    protected readonly ServiceBase Service;
    protected readonly ILogger Logger;
    protected readonly string BaseEndpoint;
    protected readonly Func<TDto, T> Mapper;
    protected readonly QueryParameters Parameters;
    
    protected FluentQueryBuilderBase(
        ServiceBase service,
        ILogger logger,
        string baseEndpoint,
        Func<TDto, T> mapper)
    {
        Service = service ?? throw new ArgumentNullException(nameof(service));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        BaseEndpoint = baseEndpoint ?? throw new ArgumentNullException(nameof(baseEndpoint));
        Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        Parameters = new QueryParameters();
    }
    
    /// <summary>
    /// Creates a new instance with the same configuration but fresh parameters.
    /// </summary>
    protected abstract FluentQueryBuilderBase<T, TDto> CreateNew();
    
    public virtual IFluentQueryBuilder<T> Where(string field, object value)
    {
        if (string.IsNullOrWhiteSpace(field))
            throw new ArgumentException("Field cannot be null or empty", nameof(field));
        
        var newBuilder = CreateNew();
        newBuilder.Parameters.Where.Clear();
        foreach (var kvp in Parameters.Where)
        {
            newBuilder.Parameters.Where[kvp.Key] = kvp.Value;
        }
        newBuilder.Parameters.Where[field] = value;
        
        // Copy other parameters
        newBuilder.Parameters.Include = Parameters.Include?.ToArray();
        newBuilder.Parameters.OrderBy = Parameters.OrderBy;
        newBuilder.Parameters.PerPage = Parameters.PerPage;
        newBuilder.Parameters.Offset = Parameters.Offset;
        
        return newBuilder;
    }
    
    public virtual IFluentQueryBuilder<T> Where(Dictionary<string, object> filters)
    {
        if (filters == null)
            throw new ArgumentNullException(nameof(filters));
        
        var newBuilder = CreateNew();
        newBuilder.Parameters.Where.Clear();
        
        // Copy existing filters
        foreach (var kvp in Parameters.Where)
        {
            newBuilder.Parameters.Where[kvp.Key] = kvp.Value;
        }
        
        // Add new filters
        foreach (var kvp in filters)
        {
            newBuilder.Parameters.Where[kvp.Key] = kvp.Value;
        }
        
        // Copy other parameters
        newBuilder.Parameters.Include = Parameters.Include?.ToArray();
        newBuilder.Parameters.OrderBy = Parameters.OrderBy;
        newBuilder.Parameters.PerPage = Parameters.PerPage;
        newBuilder.Parameters.Offset = Parameters.Offset;
        
        return newBuilder;
    }

    public virtual IFluentQueryBuilder<T> Where(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
    {
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));
        
        // This is a basic implementation - derived classes can override for more sophisticated handling
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        
        // For the base implementation, we'll just return the builder as-is
        // Derived classes like FluentQueryBuilder<T> will provide the actual implementation
        return newBuilder;
    }
    
    public virtual IFluentQueryBuilder<T> Include(params string[] relationships)
    {
        if (relationships == null)
            throw new ArgumentNullException(nameof(relationships));
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        
        var currentIncludes = newBuilder.Parameters.Include?.ToList() ?? new List<string>();
        foreach (var relationship in relationships)
        {
            if (!string.IsNullOrWhiteSpace(relationship) && !currentIncludes.Contains(relationship))
            {
                currentIncludes.Add(relationship);
            }
        }
        newBuilder.Parameters.Include = currentIncludes.ToArray();
        
        return newBuilder;
    }
    
    public virtual IFluentQueryBuilder<T> OrderBy(string field, bool descending = false)
    {
        if (string.IsNullOrWhiteSpace(field))
            throw new ArgumentException("Field cannot be null or empty", nameof(field));
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        newBuilder.Parameters.OrderBy = descending ? $"-{field}" : field;
        
        return newBuilder;
    }
    
    public virtual IFluentQueryBuilder<T> OrderBy(string field)
    {
        return OrderBy(field, false);
    }
    
    public virtual IFluentQueryBuilder<T> OrderByDescending(string field)
    {
        return OrderBy(field, true);
    }
    
    public virtual IFluentQueryBuilder<T> OrderBy(System.Linq.Expressions.Expression<Func<T, object>> keySelector)
    {
        if (keySelector == null)
            throw new ArgumentNullException(nameof(keySelector));
        
        // This is a basic implementation - derived classes can override for more sophisticated handling
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        
        // For the base implementation, we'll just return the builder as-is
        // Derived classes like FluentQueryBuilder<T> will provide the actual implementation
        return newBuilder;
    }
    
    public virtual IFluentQueryBuilder<T> Take(int count)
    {
        if (count <= 0)
            throw new ArgumentException("Count must be greater than 0", nameof(count));
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        newBuilder.Parameters.PerPage = count;
        
        return newBuilder;
    }
    
    public virtual IFluentQueryBuilder<T> Skip(int count)
    {
        if (count < 0)
            throw new ArgumentException("Count cannot be negative", nameof(count));
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        newBuilder.Parameters.Offset = count;
        
        return newBuilder;
    }
    
    public virtual IFluentQueryBuilder<T> Page(int page, int pageSize)
    {
        if (page <= 0)
            throw new ArgumentException("Page must be greater than 0", nameof(page));
        if (pageSize <= 0)
            throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        newBuilder.Parameters.PerPage = pageSize;
        newBuilder.Parameters.Offset = (page - 1) * pageSize;
        
        return newBuilder;
    }
    
    public virtual IFluentQueryBuilder<T> WithParameter(string key, object value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or empty", nameof(key));
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        newBuilder.Parameters.Where[key] = value;
        
        return newBuilder;
    }

    public virtual IFluentQueryBuilder<T> WhereAnd(params System.Linq.Expressions.Expression<Func<T, bool>>[] predicates)
    {
        // This is a basic implementation - derived classes can override for more sophisticated handling
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        
        // For the base implementation, we'll just return the builder as-is
        // Derived classes like FluentQueryBuilder<T> will provide the actual implementation
        return newBuilder;
    }

    public virtual IFluentQueryBuilder<T> WhereOr(params System.Linq.Expressions.Expression<Func<T, bool>>[] predicates)
    {
        // This is a basic implementation - derived classes can override for more sophisticated handling
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        
        // For the base implementation, we'll just return the builder as-is
        // Derived classes like FluentQueryBuilder<T> will provide the actual implementation
        return newBuilder;
    }

    public virtual IFluentQueryBuilder<T> WhereIn(string field, IEnumerable<object> values)
    {
        if (string.IsNullOrWhiteSpace(field))
            throw new ArgumentException("Field cannot be null or empty", nameof(field));
        if (values == null)
            throw new ArgumentNullException(nameof(values));
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        
        var valuesList = values.ToList();
        if (valuesList.Any())
        {
            var valuesString = string.Join(",", valuesList);
            newBuilder.Parameters.Where[field] = valuesString;
        }
        
        return newBuilder;
    }

    public virtual IFluentQueryBuilder<T> WhereNotIn(string field, IEnumerable<object> values)
    {
        if (string.IsNullOrWhiteSpace(field))
            throw new ArgumentException("Field cannot be null or empty", nameof(field));
        if (values == null)
            throw new ArgumentNullException(nameof(values));
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        
        var valuesList = values.ToList();
        if (valuesList.Any())
        {
            var valuesString = "!" + string.Join(",", valuesList);
            newBuilder.Parameters.Where[field] = valuesString;
        }
        
        return newBuilder;
    }

    public virtual IFluentQueryBuilder<T> WhereNull(string field)
    {
        if (string.IsNullOrWhiteSpace(field))
            throw new ArgumentException("Field cannot be null or empty", nameof(field));
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        newBuilder.Parameters.Where[field] = "null";
        
        return newBuilder;
    }

    public virtual IFluentQueryBuilder<T> WhereNotNull(string field)
    {
        if (string.IsNullOrWhiteSpace(field))
            throw new ArgumentException("Field cannot be null or empty", nameof(field));
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        newBuilder.Parameters.Where[field] = "!null";
        
        return newBuilder;
    }

    public virtual IFluentQueryBuilder<T> WhereDateRange(string field, DateTime startDate, DateTime endDate)
    {
        if (string.IsNullOrWhiteSpace(field))
            throw new ArgumentException("Field cannot be null or empty", nameof(field));
        if (startDate > endDate)
            throw new ArgumentException("Start date cannot be greater than end date");
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        newBuilder.Parameters.Where[field] = $"{startDate:yyyy-MM-dd}..{endDate:yyyy-MM-dd}";
        
        return newBuilder;
    }

    public virtual IFluentQueryBuilder<T> WhereDateRange(string field, DateTime? startDate, DateTime? endDate)
    {
        if (string.IsNullOrWhiteSpace(field))
            throw new ArgumentException("Field cannot be null or empty", nameof(field));
        if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            throw new ArgumentException("Start date cannot be greater than end date");
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        
        if (startDate.HasValue && endDate.HasValue)
        {
            newBuilder.Parameters.Where[field] = $"{startDate.Value:yyyy-MM-dd}..{endDate.Value:yyyy-MM-dd}";
        }
        else if (startDate.HasValue)
        {
            newBuilder.Parameters.Where[field] = $"{startDate.Value:yyyy-MM-dd}..";
        }
        else if (endDate.HasValue)
        {
            newBuilder.Parameters.Where[field] = $"..{endDate.Value:yyyy-MM-dd}";
        }
        
        return newBuilder;
    }

    public virtual IFluentQueryBuilder<T> WhereBetween<TValue>(string field, TValue minValue, TValue maxValue)
        where TValue : IComparable<TValue>
    {
        if (string.IsNullOrWhiteSpace(field))
            throw new ArgumentException("Field cannot be null or empty", nameof(field));
        if (minValue == null)
            throw new ArgumentNullException(nameof(minValue));
        if (maxValue == null)
            throw new ArgumentNullException(nameof(maxValue));
        if (minValue.CompareTo(maxValue) > 0)
            throw new ArgumentException("Min value cannot be greater than max value");
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        newBuilder.Parameters.Where[field] = $"{minValue}..{maxValue}";
        
        return newBuilder;
    }

    public virtual IFluentQueryBuilder<T> Include<TProperty>(System.Linq.Expressions.Expression<Func<T, TProperty>> include)
    {
        // This is a basic implementation - derived classes can override for more sophisticated handling
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        
        // For the base implementation, we'll just return the builder as-is
        // Derived classes like FluentQueryBuilder<T> will provide the actual implementation
        return newBuilder;
    }

    public virtual IFluentQueryBuilder<T> IncludeDeep(string relationshipPath)
    {
        if (string.IsNullOrWhiteSpace(relationshipPath))
            throw new ArgumentException("Relationship path cannot be null or empty", nameof(relationshipPath));
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        
        // Add the deep relationship to includes
        var currentIncludes = newBuilder.Parameters.Include?.ToList() ?? new List<string>();
        currentIncludes.Add(relationshipPath);
        newBuilder.Parameters.Include = currentIncludes.ToArray();
        
        return newBuilder;
    }

    public virtual IFluentQueryBuilder<T> IncludeDeep(params string[] relationshipPaths)
    {
        if (relationshipPaths == null) throw new ArgumentNullException(nameof(relationshipPaths));
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        
        var currentIncludes = newBuilder.Parameters.Include?.ToList() ?? new List<string>();
        foreach (var path in relationshipPaths)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                currentIncludes.Add(path);
            }
        }
        newBuilder.Parameters.Include = currentIncludes.ToArray();
        
        return newBuilder;
    }

    public virtual IFluentQueryBuilder<T> WhereHasRelationship(string relationshipName)
    {
        if (string.IsNullOrWhiteSpace(relationshipName))
            throw new ArgumentException("Relationship name cannot be null or empty", nameof(relationshipName));
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        newBuilder.Parameters.Where[$"has_{relationshipName}"] = "true";
        
        return newBuilder;
    }

    public virtual IFluentQueryBuilder<T> WhereDoesntHaveRelationship(string relationshipName)
    {
        if (string.IsNullOrWhiteSpace(relationshipName))
            throw new ArgumentException("Relationship name cannot be null or empty", nameof(relationshipName));
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        newBuilder.Parameters.Where[$"has_{relationshipName}"] = "false";
        
        return newBuilder;
    }

    public virtual IFluentQueryBuilder<T> WhereHas(string relationshipName, string field, object value)
    {
        if (string.IsNullOrWhiteSpace(relationshipName))
            throw new ArgumentException("Relationship name cannot be null or empty", nameof(relationshipName));
        if (string.IsNullOrWhiteSpace(field))
            throw new ArgumentException("Field cannot be null or empty", nameof(field));
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        newBuilder.Parameters.Where[$"{relationshipName}.{field}"] = value?.ToString() ?? "null";
        
        return newBuilder;
    }

    public virtual IFluentQueryBuilder<T> WhereHas(string relationshipName, Dictionary<string, object> filters)
    {
        if (string.IsNullOrWhiteSpace(relationshipName))
            throw new ArgumentException("Relationship name cannot be null or empty", nameof(relationshipName));
        if (filters == null) throw new ArgumentNullException(nameof(filters));
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        
        foreach (var filter in filters)
        {
            newBuilder.Parameters.Where[$"{relationshipName}.{filter.Key}"] = filter.Value?.ToString() ?? "null";
        }
        
        return newBuilder;
    }

    public virtual IFluentQueryBuilder<T> WhereRelationshipCount(string relationshipName, int count)
    {
        if (string.IsNullOrWhiteSpace(relationshipName))
            throw new ArgumentException("Relationship name cannot be null or empty", nameof(relationshipName));
        if (count < 0) throw new ArgumentException("Count cannot be negative", nameof(count));
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        newBuilder.Parameters.Where[$"{relationshipName}_count"] = count.ToString();
        
        return newBuilder;
    }

    public virtual IFluentQueryBuilder<T> WhereRelationshipCountGreaterThan(string relationshipName, int minCount)
    {
        if (string.IsNullOrWhiteSpace(relationshipName))
            throw new ArgumentException("Relationship name cannot be null or empty", nameof(relationshipName));
        if (minCount < 0) throw new ArgumentException("Min count cannot be negative", nameof(minCount));
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        newBuilder.Parameters.Where[$"{relationshipName}_count"] = $">{minCount}";
        
        return newBuilder;
    }

    public virtual IFluentQueryBuilder<T> WhereRelationshipCountLessThan(string relationshipName, int maxCount)
    {
        if (string.IsNullOrWhiteSpace(relationshipName))
            throw new ArgumentException("Relationship name cannot be null or empty", nameof(relationshipName));
        if (maxCount < 0) throw new ArgumentException("Max count cannot be negative", nameof(maxCount));
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        newBuilder.Parameters.Where[$"{relationshipName}_count"] = $"<{maxCount}";
        
        return newBuilder;
    }

    public virtual IFluentQueryBuilder<T> WhereRelationshipCountBetween(string relationshipName, int minCount, int maxCount)
    {
        if (string.IsNullOrWhiteSpace(relationshipName))
            throw new ArgumentException("Relationship name cannot be null or empty", nameof(relationshipName));
        if (minCount < 0) throw new ArgumentException("Min count cannot be negative", nameof(minCount));
        if (maxCount < 0) throw new ArgumentException("Max count cannot be negative", nameof(maxCount));
        if (minCount > maxCount) throw new ArgumentException("Min count must be less than or equal to max count");
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        newBuilder.Parameters.Where[$"{relationshipName}_count"] = $"{minCount}..{maxCount}";
        
        return newBuilder;
    }
    
    public virtual QueryParameters Build()
    {
        return Parameters.Clone();
    }
    
    public abstract Task<IPagedResponse<T>> ExecuteAsync(CancellationToken cancellationToken = default);
    
    public virtual async Task<T?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        newBuilder.Parameters.PerPage = 1;
        
        var result = await newBuilder.ExecuteAsync(cancellationToken);
        return result.Data.FirstOrDefault();
    }
    
    public virtual async Task<T> SingleAsync(CancellationToken cancellationToken = default)
    {
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        newBuilder.Parameters.PerPage = 2; // Get 2 to detect multiple results
        
        var result = await newBuilder.ExecuteAsync(cancellationToken);
        
        if (result.Data.Count == 0)
            throw new InvalidOperationException("Sequence contains no elements");
        if (result.Data.Count > 1)
            throw new InvalidOperationException("Sequence contains more than one element");
        
        return result.Data.First();
    }
    
    public virtual async Task<T?> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        newBuilder.Parameters.PerPage = 2; // Get 2 to detect multiple results
        
        var result = await newBuilder.ExecuteAsync(cancellationToken);
        
        if (result.Data.Count > 1)
            throw new InvalidOperationException("Sequence contains more than one element");
        
        return result.Data.FirstOrDefault();
    }
    
    public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        var result = await ExecuteAsync(cancellationToken);
        return result.Meta?.TotalCount ?? result.Data.Count;
    }
    
    public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        
        // Add the predicate as a where condition
        var filterResult = ParseFilterExpression(predicate);
        newBuilder.Parameters.Where[filterResult.Field] = filterResult.Value;
        
        var result = await newBuilder.ExecuteAsync(cancellationToken);
        return result.Meta?.TotalCount ?? result.Data.Count;
    }
    
    public virtual async Task<int> CountDistinctAsync(string field, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(field)) throw new ArgumentException("Field cannot be null or empty", nameof(field));
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        newBuilder.Parameters.Where["aggregate"] = $"count_distinct({field})";
        
        var result = await newBuilder.ExecuteAsync(cancellationToken);
        
        // In a real implementation, this would parse the distinct count from the API response
        return result.Data.Select(item => GetPropertyValue(item, field)).Distinct().Count();
    }
    
    public virtual async Task<int> CountDistinctAsync<TProperty>(Expression<Func<T, TProperty>> selector, CancellationToken cancellationToken = default)
    {
        if (selector == null) throw new ArgumentNullException(nameof(selector));
        
        var fieldName = ParseExpressionToFieldName(selector);
        return await CountDistinctAsync(fieldName, cancellationToken);
    }
    
    public virtual async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        newBuilder.Parameters.PerPage = 1;
        
        var result = await newBuilder.ExecuteAsync(cancellationToken);
        return result.Data.Any();
    }
    
    public virtual async Task<IPagedResponse<T>> AllAsync(CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(cancellationToken);
    }
    
    public virtual async Task<decimal> SumAsync(string field, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(field)) throw new ArgumentException("Field cannot be null or empty", nameof(field));
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        newBuilder.Parameters.Where["aggregate"] = $"sum({field})";
        
        var result = await newBuilder.ExecuteAsync(cancellationToken);
        
        // In a real implementation, this would parse the aggregation result from the API response
        // For now, we'll return a placeholder value
        return result.Meta?.TotalCount ?? 0;
    }
    
    public virtual async Task<decimal> SumAsync<TProperty>(Expression<Func<T, TProperty>> selector, CancellationToken cancellationToken = default) where TProperty : struct
    {
        if (selector == null) throw new ArgumentNullException(nameof(selector));
        
        // Parse the expression to get the field name
        var fieldName = ParseExpressionToFieldName(selector);
        return await SumAsync(fieldName, cancellationToken);
    }
    
    public virtual async Task<decimal> SumAsync(string field, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(field)) throw new ArgumentException("Field cannot be null or empty", nameof(field));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        
        // Add the predicate as a where condition
        var filterResult = ParseFilterExpression(predicate);
        newBuilder.Parameters.Where[filterResult.Field] = filterResult.Value;
        newBuilder.Parameters.Where["aggregate"] = $"sum({field})";
        
        var result = await newBuilder.ExecuteAsync(cancellationToken);
        
        // In a real implementation, this would parse the sum from the API response
        return result.Data.Where(predicate.Compile()).Sum(item => Convert.ToDecimal(GetPropertyValue(item, field) ?? 0));
    }
    
    public virtual async Task<decimal> SumAsync<TProperty>(Expression<Func<T, TProperty>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) where TProperty : struct
    {
        if (selector == null) throw new ArgumentNullException(nameof(selector));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var fieldName = ParseExpressionToFieldName(selector);
        return await SumAsync(fieldName, predicate, cancellationToken);
    }
    
    public virtual async Task<decimal> SumDistinctAsync(string field, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(field)) throw new ArgumentException("Field cannot be null or empty", nameof(field));
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        newBuilder.Parameters.Where["aggregate"] = $"sum_distinct({field})";
        
        var result = await newBuilder.ExecuteAsync(cancellationToken);
        
        // In a real implementation, this would parse the distinct sum from the API response
        return result.Data.Select(item => Convert.ToDecimal(GetPropertyValue(item, field) ?? 0)).Distinct().Sum();
    }
    
    public virtual async Task<decimal> SumDistinctAsync<TProperty>(Expression<Func<T, TProperty>> selector, CancellationToken cancellationToken = default) where TProperty : struct
    {
        if (selector == null) throw new ArgumentNullException(nameof(selector));
        
        var fieldName = ParseExpressionToFieldName(selector);
        return await SumDistinctAsync(fieldName, cancellationToken);
    }
    
    public virtual async Task<decimal> AverageAsync(string field, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(field)) throw new ArgumentException("Field cannot be null or empty", nameof(field));
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        newBuilder.Parameters.Where["aggregate"] = $"avg({field})";
        
        var result = await newBuilder.ExecuteAsync(cancellationToken);
        
        // In a real implementation, this would parse the aggregation result from the API response
        return result.Meta?.TotalCount ?? 0;
    }
    
    public virtual async Task<decimal> AverageAsync<TProperty>(Expression<Func<T, TProperty>> selector, CancellationToken cancellationToken = default) where TProperty : struct
    {
        if (selector == null) throw new ArgumentNullException(nameof(selector));
        
        var fieldName = ParseExpressionToFieldName(selector);
        return await AverageAsync(fieldName, cancellationToken);
    }
    
    public virtual async Task<decimal> AverageAsync(string field, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(field)) throw new ArgumentException("Field cannot be null or empty", nameof(field));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        
        // Add the predicate as a where condition
        var filterResult = ParseFilterExpression(predicate);
        newBuilder.Parameters.Where[filterResult.Field] = filterResult.Value;
        newBuilder.Parameters.Where["aggregate"] = $"avg({field})";
        
        var result = await newBuilder.ExecuteAsync(cancellationToken);
        
        // In a real implementation, this would parse the average from the API response
        var filteredData = result.Data.Where(predicate.Compile());
        return filteredData.Any() ? filteredData.Average(item => Convert.ToDecimal(GetPropertyValue(item, field) ?? 0)) : 0;
    }
    
    public virtual async Task<decimal> AverageAsync<TProperty>(Expression<Func<T, TProperty>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) where TProperty : struct
    {
        if (selector == null) throw new ArgumentNullException(nameof(selector));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var fieldName = ParseExpressionToFieldName(selector);
        return await AverageAsync(fieldName, predicate, cancellationToken);
    }
    
    public virtual async Task<TResult> MinAsync<TResult>(string field, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(field)) throw new ArgumentException("Field cannot be null or empty", nameof(field));
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        newBuilder.Parameters.Where["aggregate"] = $"min({field})";
        
        var result = await newBuilder.ExecuteAsync(cancellationToken);
        
        // In a real implementation, this would parse the aggregation result from the API response
        return default(TResult)!;
    }
    
    public virtual async Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default)
    {
        if (selector == null) throw new ArgumentNullException(nameof(selector));
        
        var fieldName = ParseExpressionToFieldName(selector);
        return await MinAsync<TResult>(fieldName, cancellationToken);
    }
    
    public virtual async Task<TResult> MinAsync<TResult>(string field, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(field)) throw new ArgumentException("Field cannot be null or empty", nameof(field));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        
        // Add the predicate as a where condition
        var filterResult = ParseFilterExpression(predicate);
        newBuilder.Parameters.Where[filterResult.Field] = filterResult.Value;
        newBuilder.Parameters.Where["aggregate"] = $"min({field})";
        
        var result = await newBuilder.ExecuteAsync(cancellationToken);
        
        // In a real implementation, this would parse the min from the API response
        var filteredData = result.Data.Where(predicate.Compile());
        if (!filteredData.Any()) return default(TResult)!;
        
        var values = filteredData.Select(item => GetPropertyValue(item, field)).Where(v => v != null);
        return values.Any() ? (TResult)values.Min()! : default(TResult)!;
    }
    
    public virtual async Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (selector == null) throw new ArgumentNullException(nameof(selector));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var fieldName = ParseExpressionToFieldName(selector);
        return await MinAsync<TResult>(fieldName, predicate, cancellationToken);
    }
    
    public virtual async Task<TResult> MaxAsync<TResult>(string field, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(field)) throw new ArgumentException("Field cannot be null or empty", nameof(field));
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        newBuilder.Parameters.Where["aggregate"] = $"max({field})";
        
        var result = await newBuilder.ExecuteAsync(cancellationToken);
        
        // In a real implementation, this would parse the aggregation result from the API response
        return default(TResult)!;
    }
    
    public virtual async Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default)
    {
        if (selector == null) throw new ArgumentNullException(nameof(selector));
        
        var fieldName = ParseExpressionToFieldName(selector);
        return await MaxAsync<TResult>(fieldName, cancellationToken);
    }
    
    public virtual async Task<TResult> MaxAsync<TResult>(string field, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(field)) throw new ArgumentException("Field cannot be null or empty", nameof(field));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        
        // Add the predicate as a where condition
        var filterResult = ParseFilterExpression(predicate);
        newBuilder.Parameters.Where[filterResult.Field] = filterResult.Value;
        newBuilder.Parameters.Where["aggregate"] = $"max({field})";
        
        var result = await newBuilder.ExecuteAsync(cancellationToken);
        
        // In a real implementation, this would parse the max from the API response
        var filteredData = result.Data.Where(predicate.Compile());
        if (!filteredData.Any()) return default(TResult)!;
        
        var values = filteredData.Select(item => GetPropertyValue(item, field)).Where(v => v != null);
        return values.Any() ? (TResult)values.Max()! : default(TResult)!;
    }
    
    public virtual async Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (selector == null) throw new ArgumentNullException(nameof(selector));
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var fieldName = ParseExpressionToFieldName(selector);
        return await MaxAsync<TResult>(fieldName, predicate, cancellationToken);
    }
    
    public virtual async Task<IEnumerable<IGrouping<object, T>>> GroupedAsync(CancellationToken cancellationToken = default)
    {
        var result = await ExecuteAsync(cancellationToken);
        
        // In a real implementation, this would return properly grouped results from the API
        // For now, we'll return a single group containing all results
        return new[] { new Grouping<object, T>("default", result.Data) };
    }
    
    public virtual async Task<IEnumerable<IGrouping<TKey, T>>> GroupedAsync<TKey>(CancellationToken cancellationToken = default)
    {
        var result = await ExecuteAsync(cancellationToken);
        
        // In a real implementation, this would return properly grouped results from the API
        // For now, we'll return a single group containing all results
        return new[] { new Grouping<TKey, T>(default(TKey)!, result.Data) };
    }
    
    public virtual IFluentQueryBuilder<T> GroupBy(string field)
    {
        if (string.IsNullOrWhiteSpace(field)) throw new ArgumentException("Field cannot be null or empty", nameof(field));
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        newBuilder.Parameters.Where["group_by"] = field;
        
        return newBuilder;
    }
    
    public virtual IFluentQueryBuilder<T> GroupBy(params string[] fields)
    {
        if (fields == null) throw new ArgumentNullException(nameof(fields));
        if (fields.Length == 0) throw new ArgumentException("At least one field must be specified", nameof(fields));
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        newBuilder.Parameters.Where["group_by"] = string.Join(",", fields);
        
        return newBuilder;
    }
    
    public virtual IFluentQueryBuilder<T> GroupBy<TKey>(Expression<Func<T, TKey>> keySelector)
    {
        if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
        
        var fieldName = ParseExpressionToFieldName(keySelector);
        return GroupBy(fieldName);
    }
    
    public virtual IFluentQueryBuilder<T> Having(string field, object value)
    {
        if (string.IsNullOrWhiteSpace(field)) throw new ArgumentException("Field cannot be null or empty", nameof(field));
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        newBuilder.Parameters.Where[$"having[{field}]"] = value?.ToString() ?? "null";
        
        return newBuilder;
    }
    
    public virtual IFluentQueryBuilder<T> Having(Dictionary<string, object> conditions)
    {
        if (conditions == null) throw new ArgumentNullException(nameof(conditions));
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        
        foreach (var condition in conditions)
        {
            newBuilder.Parameters.Where[$"having[{condition.Key}]"] = condition.Value?.ToString() ?? "null";
        }
        
        return newBuilder;
    }
    
    public virtual IFluentQueryBuilder<T> Having(Expression<Func<IGrouping<object, T>, bool>> predicate)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var newBuilder = CreateNew();
        CopyParameters(newBuilder);
        newBuilder.Parameters.Where["having_expression"] = predicate.ToString();
        
        return newBuilder;
    }
    
    /// <summary>
    /// Parses an expression to extract the field name.
    /// </summary>
    protected virtual string ParseExpressionToFieldName<TProperty>(Expression<Func<T, TProperty>> expression)
    {
        if (expression.Body is MemberExpression memberExpression)
        {
            return memberExpression.Member.Name.ToLowerInvariant();
        }
        
        if (expression.Body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression memberExpr)
        {
            return memberExpr.Member.Name.ToLowerInvariant();
        }
        
        throw new ArgumentException($"Unable to parse expression: {expression}");
    }
    
    /// <summary>
    /// Parses a filter expression to extract field name and value.
    /// </summary>
    protected virtual (string Field, object Value) ParseFilterExpression(Expression<Func<T, bool>> predicate)
    {
        // Basic implementation - derived classes can override for more sophisticated parsing
        if (predicate.Body is BinaryExpression binaryExpression)
        {
            if (binaryExpression.Left is MemberExpression memberExpression)
            {
                var fieldName = memberExpression.Member.Name.ToLowerInvariant();
                var value = ExtractValue(binaryExpression.Right);
                return (fieldName, value);
            }
        }
        
        throw new ArgumentException($"Unable to parse filter expression: {predicate}");
    }
    
    /// <summary>
    /// Extracts the value from an expression.
    /// </summary>
    protected virtual object ExtractValue(Expression expression)
    {
        if (expression is ConstantExpression constantExpression)
        {
            return constantExpression.Value ?? throw new InvalidOperationException("Constant expression value is null");
        }
        
        if (expression is MemberExpression memberExpression)
        {
            var objectMember = Expression.Convert(memberExpression, typeof(object));
            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
            var getter = getterLambda.Compile();
            return getter();
        }
        
        // For more complex expressions, compile and execute
        var lambda = Expression.Lambda(expression);
        return lambda.Compile().DynamicInvoke() ?? throw new InvalidOperationException("Expression evaluation resulted in null");
    }
    
    /// <summary>
    /// Gets the value of a property from an object using reflection.
    /// </summary>
    protected virtual object GetPropertyValue(T item, string propertyName)
    {
        var property = typeof(T).GetProperty(propertyName, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        return property?.GetValue(item) ?? throw new InvalidOperationException($"Property '{propertyName}' not found or returned null");
    }
    
    /// <summary>
    /// Copies all parameters from the current instance to the new builder.
    /// </summary>
    protected virtual void CopyParameters(FluentQueryBuilderBase<T, TDto> newBuilder)
    {
        newBuilder.Parameters.Where.Clear();
        foreach (var kvp in Parameters.Where)
        {
            newBuilder.Parameters.Where[kvp.Key] = kvp.Value;
        }
        
        newBuilder.Parameters.Include = Parameters.Include?.ToArray();
        newBuilder.Parameters.OrderBy = Parameters.OrderBy;
        newBuilder.Parameters.PerPage = Parameters.PerPage;
        newBuilder.Parameters.Offset = Parameters.Offset;
    }
}

/// <summary>
/// Helper class to implement IGrouping for aggregation results.
/// </summary>
/// <typeparam name="TKey">The type of the grouping key</typeparam>
/// <typeparam name="TElement">The type of the grouped elements</typeparam>
public class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
{
    private readonly IEnumerable<TElement> _elements;
    
    public Grouping(TKey key, IEnumerable<TElement> elements)
    {
        Key = key;
        _elements = elements ?? throw new ArgumentNullException(nameof(elements));
    }
    
    public TKey Key { get; }
    
    public IEnumerator<TElement> GetEnumerator()
    {
        return _elements.GetEnumerator();
    }
    
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}