using System.Linq.Expressions;
using PlanningCenter.Api.Client.Fluent.ExpressionParsing;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Fluent.QueryBuilder;

/// <summary>
/// Advanced query builder that optimizes and caches expression parsing for better performance.
/// Provides intelligent query optimization and parameter building.
/// </summary>
public class FluentQueryBuilder<T> : IFluentQueryBuilder<T> where T : class
{
    private readonly List<Expression<Func<T, bool>>> _whereConditions = new();
    private readonly List<Expression<Func<T, object>>> _includeExpressions = new();
    private readonly List<string> _includeRelationships = new();
    private readonly List<(Expression<Func<T, object>> Expression, bool Descending)> _orderByExpressions = new();
    private readonly Dictionary<string, object> _customParameters = new();
    private readonly Dictionary<string, object> _whereFilters = new();
    
    // Caching for parsed expressions to improve performance
    private static readonly Dictionary<string, FilterResult> _filterCache = new();
    private static readonly Dictionary<string, string> _includeCache = new();
    private static readonly Dictionary<string, string> _sortCache = new();

    public IFluentQueryBuilder<T> Where(Expression<Func<T, bool>> predicate)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        _whereConditions.Add(predicate);
        return this;
    }

    public IFluentQueryBuilder<T> Include(Expression<Func<T, object>> include)
    {
        if (include == null) throw new ArgumentNullException(nameof(include));
        
        _includeExpressions.Add(include);
        return this;
    }

    public IFluentQueryBuilder<T> Include(params string[] relationships)
    {
        if (relationships == null) throw new ArgumentNullException(nameof(relationships));
        
        _includeRelationships.AddRange(relationships);
        return this;
    }

    public IFluentQueryBuilder<T> Where(string field, object value)
    {
        if (string.IsNullOrWhiteSpace(field)) throw new ArgumentException("Field cannot be null or empty", nameof(field));
        
        _whereFilters[field] = value;
        return this;
    }

    public IFluentQueryBuilder<T> Where(Dictionary<string, object> filters)
    {
        if (filters == null) throw new ArgumentNullException(nameof(filters));
        
        foreach (var filter in filters)
        {
            _whereFilters[filter.Key] = filter.Value;
        }
        return this;
    }

    public IFluentQueryBuilder<T> OrderBy(string field, bool descending = false)
    {
        if (string.IsNullOrWhiteSpace(field)) throw new ArgumentException("Field cannot be null or empty", nameof(field));
        
        _customParameters["sort"] = descending ? $"-{field}" : field;
        return this;
    }

    public IFluentQueryBuilder<T> OrderByDescending(string field)
    {
        return OrderBy(field, true);
    }

    public IFluentQueryBuilder<T> Take(int count)
    {
        if (count <= 0) throw new ArgumentException("Count must be greater than zero", nameof(count));
        
        _customParameters["per_page"] = count;
        return this;
    }

    public IFluentQueryBuilder<T> Skip(int count)
    {
        if (count < 0) throw new ArgumentException("Count cannot be negative", nameof(count));
        
        _customParameters["offset"] = count;
        return this;
    }

    public IFluentQueryBuilder<T> Page(int page, int pageSize)
    {
        if (page <= 0) throw new ArgumentException("Page must be greater than zero", nameof(page));
        if (pageSize <= 0) throw new ArgumentException("Page size must be greater than zero", nameof(pageSize));
        
        _customParameters["page"] = page;
        _customParameters["per_page"] = pageSize;
        return this;
    }

    public IFluentQueryBuilder<T> OrderBy(string field)
    {
        if (string.IsNullOrWhiteSpace(field)) throw new ArgumentException("Field cannot be null or empty", nameof(field));
        
        _customParameters["order"] = field;
        return this;
    }

    public IFluentQueryBuilder<T> OrderBy(Expression<Func<T, object>> orderBy)
    {
        if (orderBy == null) throw new ArgumentNullException(nameof(orderBy));
        
        _orderByExpressions.Clear();
        _orderByExpressions.Add((orderBy, false));
        return this;
    }

    public IFluentQueryBuilder<T> OrderByDescending(Expression<Func<T, object>> orderBy)
    {
        if (orderBy == null) throw new ArgumentNullException(nameof(orderBy));
        
        _orderByExpressions.Clear();
        _orderByExpressions.Add((orderBy, true));
        return this;
    }

    public IFluentQueryBuilder<T> ThenBy(Expression<Func<T, object>> thenBy)
    {
        if (thenBy == null) throw new ArgumentNullException(nameof(thenBy));
        
        _orderByExpressions.Add((thenBy, false));
        return this;
    }

    public IFluentQueryBuilder<T> ThenByDescending(Expression<Func<T, object>> thenBy)
    {
        if (thenBy == null) throw new ArgumentNullException(nameof(thenBy));
        
        _orderByExpressions.Add((thenBy, true));
        return this;
    }

    public IFluentQueryBuilder<T> WithParameter(string key, object value)
    {
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Key cannot be null or empty", nameof(key));
        
        _customParameters[key] = value;
        return this;
    }

    IFluentQueryBuilder<T> IFluentQueryBuilder<T>.WithParameter(string key, object value)
    {
        return WithParameter(key, value);
    }

    /// <summary>
    /// Groups the results by the specified field.
    /// </summary>
    public IFluentQueryBuilder<T> GroupBy(string field)
    {
        if (string.IsNullOrWhiteSpace(field)) throw new ArgumentException("Field cannot be null or empty", nameof(field));
        
        _customParameters["group_by"] = field;
        return this;
    }

    /// <summary>
    /// Groups the results by multiple fields.
    /// </summary>
    public IFluentQueryBuilder<T> GroupBy(params string[] fields)
    {
        if (fields == null) throw new ArgumentNullException(nameof(fields));
        if (fields.Length == 0) throw new ArgumentException("At least one field must be specified", nameof(fields));
        
        _customParameters["group_by"] = string.Join(",", fields);
        return this;
    }

    /// <summary>
    /// Groups the results using a LINQ expression.
    /// </summary>
    public IFluentQueryBuilder<T> GroupBy<TKey>(Expression<Func<T, TKey>> keySelector)
    {
        if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
        
        var field = ParseSortExpression(keySelector as Expression<Func<T, object>> ?? 
                                       Expression.Lambda<Func<T, object>>(Expression.Convert(keySelector.Body, typeof(object)), keySelector.Parameters));
        
        if (!string.IsNullOrEmpty(field))
        {
            _customParameters["group_by"] = field;
        }
        
        return this;
    }

    /// <summary>
    /// Adds a HAVING clause condition to filter grouped results.
    /// </summary>
    public IFluentQueryBuilder<T> Having(string field, object value)
    {
        if (string.IsNullOrWhiteSpace(field)) throw new ArgumentException("Field cannot be null or empty", nameof(field));
        
        _customParameters[$"having[{field}]"] = value;
        return this;
    }

    /// <summary>
    /// Adds multiple HAVING clause conditions to filter grouped results.
    /// </summary>
    public IFluentQueryBuilder<T> Having(Dictionary<string, object> conditions)
    {
        if (conditions == null) throw new ArgumentNullException(nameof(conditions));
        
        foreach (var condition in conditions)
        {
            _customParameters[$"having[{condition.Key}]"] = condition.Value;
        }
        
        return this;
    }

    /// <summary>
    /// Adds a HAVING clause condition using a LINQ expression.
    /// </summary>
    public IFluentQueryBuilder<T> Having(Expression<Func<IGrouping<object, T>, bool>> predicate)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        // For now, we'll store the expression as a string representation
        // In a full implementation, this would need proper expression parsing
        _customParameters["having_expression"] = predicate.ToString();
        
        return this;
    }

    /// <summary>
    /// Adds a filter for values within a collection (IN operation).
    /// </summary>
    public IFluentQueryBuilder<T> WhereIn<TProperty>(Expression<Func<T, TProperty>> property, IEnumerable<TProperty> values)
    {
        if (property == null) throw new ArgumentNullException(nameof(property));
        if (values == null) throw new ArgumentNullException(nameof(values));
        
        var valuesList = values.ToList();
        if (!valuesList.Any()) return this;
        
        // Create a Contains expression: values.Contains(property)
        var valuesConstant = Expression.Constant(valuesList);
        var containsMethod = typeof(Enumerable).GetMethods()
            .First(m => m.Name == "Contains" && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(TProperty));
        var containsCall = Expression.Call(containsMethod, valuesConstant, property.Body);
        var lambda = Expression.Lambda<Func<T, bool>>(containsCall, property.Parameters[0]);
        
        return Where(lambda);
    }

    /// <summary>
    /// Adds a filter for values within a collection (IN operation) using string field name.
    /// </summary>
    public IFluentQueryBuilder<T> WhereIn(string field, IEnumerable<object> values)
    {
        if (string.IsNullOrWhiteSpace(field)) throw new ArgumentException("Field cannot be null or empty", nameof(field));
        if (values == null) throw new ArgumentNullException(nameof(values));
        
        var valuesList = values.ToList();
        if (!valuesList.Any()) return this;
        
        var valuesString = string.Join(",", valuesList);
        _whereFilters[field] = valuesString;
        
        return this;
    }

    /// <summary>
    /// Adds a filter for values not within a collection (NOT IN operation).
    /// </summary>
    public IFluentQueryBuilder<T> WhereNotIn<TProperty>(Expression<Func<T, TProperty>> property, IEnumerable<TProperty> values)
    {
        if (property == null) throw new ArgumentNullException(nameof(property));
        if (values == null) throw new ArgumentNullException(nameof(values));
        
        var valuesList = values.ToList();
        if (!valuesList.Any()) return this;
        
        // Create a !Contains expression: !values.Contains(property)
        var valuesConstant = Expression.Constant(valuesList);
        var containsMethod = typeof(Enumerable).GetMethods()
            .First(m => m.Name == "Contains" && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(TProperty));
        var containsCall = Expression.Call(containsMethod, valuesConstant, property.Body);
        var notContains = Expression.Not(containsCall);
        var lambda = Expression.Lambda<Func<T, bool>>(notContains, property.Parameters[0]);
        
        return Where(lambda);
    }

    /// <summary>
    /// Adds a filter for values not within a collection (NOT IN operation) using string field name.
    /// </summary>
    public IFluentQueryBuilder<T> WhereNotIn(string field, IEnumerable<object> values)
    {
        if (string.IsNullOrWhiteSpace(field)) throw new ArgumentException("Field cannot be null or empty", nameof(field));
        if (values == null) throw new ArgumentNullException(nameof(values));
        
        var valuesList = values.ToList();
        if (!valuesList.Any()) return this;
        
        var valuesString = "!" + string.Join(",", valuesList);
        _whereFilters[field] = valuesString;
        
        return this;
    }

    /// <summary>
    /// Adds a filter for null values.
    /// </summary>
    public IFluentQueryBuilder<T> WhereNull<TProperty>(Expression<Func<T, TProperty>> property)
    {
        if (property == null) throw new ArgumentNullException(nameof(property));
        
        var nullConstant = Expression.Constant(null, typeof(TProperty));
        var equalExpression = Expression.Equal(property.Body, nullConstant);
        var lambda = Expression.Lambda<Func<T, bool>>(equalExpression, property.Parameters[0]);
        
        return Where(lambda);
    }

    /// <summary>
    /// Adds a filter for null values using string field name.
    /// </summary>
    public IFluentQueryBuilder<T> WhereNull(string field)
    {
        if (string.IsNullOrWhiteSpace(field)) throw new ArgumentException("Field cannot be null or empty", nameof(field));
        
        _whereFilters[field] = "null";
        
        return this;
    }

    /// <summary>
    /// Adds a filter for non-null values.
    /// </summary>
    public IFluentQueryBuilder<T> WhereNotNull<TProperty>(Expression<Func<T, TProperty>> property)
    {
        if (property == null) throw new ArgumentNullException(nameof(property));
        
        var nullConstant = Expression.Constant(null, typeof(TProperty));
        var notEqualExpression = Expression.NotEqual(property.Body, nullConstant);
        var lambda = Expression.Lambda<Func<T, bool>>(notEqualExpression, property.Parameters[0]);
        
        return Where(lambda);
    }

    /// <summary>
    /// Adds a filter for non-null values using string field name.
    /// </summary>
    public IFluentQueryBuilder<T> WhereNotNull(string field)
    {
        if (string.IsNullOrWhiteSpace(field)) throw new ArgumentException("Field cannot be null or empty", nameof(field));
        
        _whereFilters[field] = "!null";
        
        return this;
    }

    /// <summary>
    /// Adds a date range filter.
    /// </summary>
    public IFluentQueryBuilder<T> WhereDateRange(Expression<Func<T, DateTime>> property, DateTime startDate, DateTime endDate)
    {
        if (property == null) throw new ArgumentNullException(nameof(property));
        if (startDate > endDate) throw new ArgumentException("Start date must be before end date");
        
        var startConstant = Expression.Constant(startDate);
        var endConstant = Expression.Constant(endDate);
        
        var greaterThanOrEqual = Expression.GreaterThanOrEqual(property.Body, startConstant);
        var lessThanOrEqual = Expression.LessThanOrEqual(property.Body, endConstant);
        var andExpression = Expression.AndAlso(greaterThanOrEqual, lessThanOrEqual);
        
        var lambda = Expression.Lambda<Func<T, bool>>(andExpression, property.Parameters[0]);
        
        return Where(lambda);
    }

    /// <summary>
    /// Adds a date range filter using string field name.
    /// </summary>
    public IFluentQueryBuilder<T> WhereDateRange(string field, DateTime startDate, DateTime endDate)
    {
        if (string.IsNullOrWhiteSpace(field)) throw new ArgumentException("Field cannot be null or empty", nameof(field));
        if (startDate > endDate) throw new ArgumentException("Start date must be before end date");
        
        var startString = startDate.ToString("yyyy-MM-dd");
        var endString = endDate.ToString("yyyy-MM-dd");
        _whereFilters[field] = $"{startString}..{endString}";
        
        return this;
    }

    /// <summary>
    /// Adds a date range filter for nullable DateTime properties.
    /// </summary>
    public IFluentQueryBuilder<T> WhereDateRange(Expression<Func<T, DateTime?>> property, DateTime startDate, DateTime endDate)
    {
        if (property == null) throw new ArgumentNullException(nameof(property));
        if (startDate > endDate) throw new ArgumentException("Start date must be before end date");
        
        var startConstant = Expression.Constant(startDate, typeof(DateTime?));
        var endConstant = Expression.Constant(endDate, typeof(DateTime?));
        
        var greaterThanOrEqual = Expression.GreaterThanOrEqual(property.Body, startConstant);
        var lessThanOrEqual = Expression.LessThanOrEqual(property.Body, endConstant);
        var andExpression = Expression.AndAlso(greaterThanOrEqual, lessThanOrEqual);
        
        var lambda = Expression.Lambda<Func<T, bool>>(andExpression, property.Parameters[0]);
        
        return Where(lambda);
    }

    /// <summary>
    /// Adds a date range filter for nullable DateTime using string field name.
    /// </summary>
    public IFluentQueryBuilder<T> WhereDateRange(string field, DateTime? startDate, DateTime? endDate)
    {
        if (string.IsNullOrWhiteSpace(field)) throw new ArgumentException("Field cannot be null or empty", nameof(field));
        
        if (startDate.HasValue && endDate.HasValue)
        {
            if (startDate > endDate) throw new ArgumentException("Start date must be before end date");
            var startString = startDate.Value.ToString("yyyy-MM-dd");
            var endString = endDate.Value.ToString("yyyy-MM-dd");
            _whereFilters[field] = $"{startString}..{endString}";
        }
        else if (startDate.HasValue)
        {
            var startString = startDate.Value.ToString("yyyy-MM-dd");
            _whereFilters[field] = $">={startString}";
        }
        else if (endDate.HasValue)
        {
            var endString = endDate.Value.ToString("yyyy-MM-dd");
            _whereFilters[field] = $"<={endString}";
        }
        
        return this;
    }

    /// <summary>
    /// Adds a between filter for numeric values.
    /// </summary>
    public IFluentQueryBuilder<T> WhereBetween<TProperty>(Expression<Func<T, TProperty>> property, TProperty minValue, TProperty maxValue)
        where TProperty : IComparable<TProperty>
    {
        if (property == null) throw new ArgumentNullException(nameof(property));
        if (minValue.CompareTo(maxValue) > 0) throw new ArgumentException("Min value must be less than or equal to max value");
        
        var minConstant = Expression.Constant(minValue);
        var maxConstant = Expression.Constant(maxValue);
        
        var greaterThanOrEqual = Expression.GreaterThanOrEqual(property.Body, minConstant);
        var lessThanOrEqual = Expression.LessThanOrEqual(property.Body, maxConstant);
        var andExpression = Expression.AndAlso(greaterThanOrEqual, lessThanOrEqual);
        
        var lambda = Expression.Lambda<Func<T, bool>>(andExpression, property.Parameters[0]);
        
        return Where(lambda);
    }

    /// <summary>
    /// Adds a between filter for numeric values using string field name.
    /// </summary>
    public IFluentQueryBuilder<T> WhereBetween<TValue>(string field, TValue minValue, TValue maxValue) where TValue : IComparable<TValue>
    {
        if (string.IsNullOrWhiteSpace(field)) throw new ArgumentException("Field cannot be null or empty", nameof(field));
        if (minValue.CompareTo(maxValue) > 0) throw new ArgumentException("Min value must be less than or equal to max value");
        
        _whereFilters[field] = $"{minValue}..{maxValue}";
        
        return this;
    }

    /// <summary>
    /// Combines multiple conditions with AND logic.
    /// </summary>
    public IFluentQueryBuilder<T> WhereAnd(params Expression<Func<T, bool>>[] conditions)
    {
        if (conditions == null || !conditions.Any()) return this;
        
        foreach (var condition in conditions)
        {
            Where(condition);
        }
        
        return this;
    }

    /// <summary>
    /// Combines multiple conditions with OR logic.
    /// </summary>
    public IFluentQueryBuilder<T> WhereOr(params Expression<Func<T, bool>>[] conditions)
    {
        if (conditions == null || !conditions.Any()) return this;
        
        if (conditions.Length == 1)
        {
            return Where(conditions[0]);
        }
        
        // Combine all conditions with OR
        var parameter = conditions[0].Parameters[0];
        Expression combinedExpression = conditions[0].Body;
        
        for (int i = 1; i < conditions.Length; i++)
        {
            var condition = conditions[i];
            var visitor = new ParameterReplacerVisitor(condition.Parameters[0], parameter);
            var replacedBody = visitor.Visit(condition.Body);
            combinedExpression = Expression.OrElse(combinedExpression, replacedBody);
        }
        
        var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
        return Where(lambda);
    }

    /// <summary>
    /// Includes related resources using a LINQ expression.
    /// </summary>
    public IFluentQueryBuilder<T> Include<TProperty>(Expression<Func<T, TProperty>> include)
    {
        if (include == null) throw new ArgumentNullException(nameof(include));
        
        // Convert to object expression for consistency with existing implementation
        var objectExpression = Expression.Lambda<Func<T, object>>(
            Expression.Convert(include.Body, typeof(object)), 
            include.Parameters[0]);
        
        _includeExpressions.Add(objectExpression);
        return this;
    }

    /// <summary>
    /// Includes deep nested relationships (e.g., person.households.members).
    /// </summary>
    public IFluentQueryBuilder<T> IncludeDeep(string relationshipPath)
    {
        if (string.IsNullOrWhiteSpace(relationshipPath)) 
            throw new ArgumentException("Relationship path cannot be null or empty", nameof(relationshipPath));
        
        _includeRelationships.Add(relationshipPath);
        return this;
    }

    /// <summary>
    /// Includes multiple deep nested relationships.
    /// </summary>
    public IFluentQueryBuilder<T> IncludeDeep(params string[] relationshipPaths)
    {
        if (relationshipPaths == null) throw new ArgumentNullException(nameof(relationshipPaths));
        
        foreach (var path in relationshipPaths)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                _includeRelationships.Add(path);
            }
        }
        return this;
    }

    /// <summary>
    /// Filters results based on the existence of a relationship.
    /// </summary>
    public IFluentQueryBuilder<T> WhereHasRelationship(string relationshipName)
    {
        if (string.IsNullOrWhiteSpace(relationshipName)) 
            throw new ArgumentException("Relationship name cannot be null or empty", nameof(relationshipName));
        
        _whereFilters[$"has_{relationshipName}"] = "true";
        return this;
    }

    /// <summary>
    /// Filters results based on the absence of a relationship.
    /// </summary>
    public IFluentQueryBuilder<T> WhereDoesntHaveRelationship(string relationshipName)
    {
        if (string.IsNullOrWhiteSpace(relationshipName)) 
            throw new ArgumentException("Relationship name cannot be null or empty", nameof(relationshipName));
        
        _whereFilters[$"has_{relationshipName}"] = "false";
        return this;
    }

    /// <summary>
    /// Filters results based on a condition within a related entity.
    /// </summary>
    public IFluentQueryBuilder<T> WhereHas(string relationshipName, string field, object value)
    {
        if (string.IsNullOrWhiteSpace(relationshipName)) 
            throw new ArgumentException("Relationship name cannot be null or empty", nameof(relationshipName));
        if (string.IsNullOrWhiteSpace(field)) 
            throw new ArgumentException("Field cannot be null or empty", nameof(field));
        
        _whereFilters[$"{relationshipName}.{field}"] = value;
        return this;
    }

    /// <summary>
    /// Filters results based on multiple conditions within a related entity.
    /// </summary>
    public IFluentQueryBuilder<T> WhereHas(string relationshipName, Dictionary<string, object> filters)
    {
        if (string.IsNullOrWhiteSpace(relationshipName)) 
            throw new ArgumentException("Relationship name cannot be null or empty", nameof(relationshipName));
        if (filters == null) throw new ArgumentNullException(nameof(filters));
        
        foreach (var filter in filters)
        {
            _whereFilters[$"{relationshipName}.{filter.Key}"] = filter.Value;
        }
        return this;
    }

    /// <summary>
    /// Filters results based on the count of related entities.
    /// </summary>
    public IFluentQueryBuilder<T> WhereRelationshipCount(string relationshipName, int count)
    {
        if (string.IsNullOrWhiteSpace(relationshipName)) 
            throw new ArgumentException("Relationship name cannot be null or empty", nameof(relationshipName));
        if (count < 0) throw new ArgumentException("Count cannot be negative", nameof(count));
        
        _whereFilters[$"{relationshipName}_count"] = count;
        return this;
    }

    /// <summary>
    /// Filters results based on the minimum count of related entities.
    /// </summary>
    public IFluentQueryBuilder<T> WhereRelationshipCountGreaterThan(string relationshipName, int minCount)
    {
        if (string.IsNullOrWhiteSpace(relationshipName)) 
            throw new ArgumentException("Relationship name cannot be null or empty", nameof(relationshipName));
        if (minCount < 0) throw new ArgumentException("Min count cannot be negative", nameof(minCount));
        
        _whereFilters[$"{relationshipName}_count"] = $">{minCount}";
        return this;
    }

    /// <summary>
    /// Filters results based on the maximum count of related entities.
    /// </summary>
    public IFluentQueryBuilder<T> WhereRelationshipCountLessThan(string relationshipName, int maxCount)
    {
        if (string.IsNullOrWhiteSpace(relationshipName)) 
            throw new ArgumentException("Relationship name cannot be null or empty", nameof(relationshipName));
        if (maxCount < 0) throw new ArgumentException("Max count cannot be negative", nameof(maxCount));
        
        _whereFilters[$"{relationshipName}_count"] = $"<{maxCount}";
        return this;
    }

    /// <summary>
    /// Filters results based on a count range of related entities.
    /// </summary>
    public IFluentQueryBuilder<T> WhereRelationshipCountBetween(string relationshipName, int minCount, int maxCount)
    {
        if (string.IsNullOrWhiteSpace(relationshipName)) 
            throw new ArgumentException("Relationship name cannot be null or empty", nameof(relationshipName));
        if (minCount < 0) throw new ArgumentException("Min count cannot be negative", nameof(minCount));
        if (maxCount < 0) throw new ArgumentException("Max count cannot be negative", nameof(maxCount));
        if (minCount > maxCount) throw new ArgumentException("Min count must be less than or equal to max count");
        
        _whereFilters[$"{relationshipName}_count"] = $"{minCount}..{maxCount}";
        return this;
    }

    /// <summary>
    /// Helper class to replace parameters in expressions for combining OR conditions.
    /// </summary>
    private class ParameterReplacerVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParameter;
        private readonly ParameterExpression _newParameter;
        
        public ParameterReplacerVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
        {
            _oldParameter = oldParameter;
            _newParameter = newParameter;
        }
        
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _oldParameter ? _newParameter : base.VisitParameter(node);
        }
    }

    public QueryParameters Build()
    {
        var parameters = new QueryParameters();

        // Add custom parameters first, handling special cases
        foreach (var (key, value) in _customParameters)
        {
            if (key == "order")
            {
                parameters.OrderBy = value.ToString();
            }
            else if (key == "per_page")
            {
                parameters.PerPage = Convert.ToInt32(value);
            }
            else if (key == "offset")
            {
                parameters.Offset = Convert.ToInt32(value);
            }
            else
            {
                parameters.Add(key, value.ToString()!);
            }
        }

        // Process string-based where filters
        foreach (var (field, value) in _whereFilters)
        {
            parameters.AddFilter(field, value.ToString()!);
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

        // Process string-based includes
        foreach (var relationship in _includeRelationships)
        {
            if (!string.IsNullOrEmpty(relationship))
            {
                parameters.AddInclude(relationship);
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
            WhereConditionsCount = _whereConditions.Count + _whereFilters.Count,
            IncludeExpressionsCount = _includeExpressions.Count + _includeRelationships.Count,
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
        clone._includeRelationships.AddRange(_includeRelationships);
        clone._orderByExpressions.AddRange(_orderByExpressions);
        
        foreach (var (key, value) in _whereFilters)
        {
            clone._whereFilters[key] = value;
        }
        
        foreach (var (key, value) in _customParameters)
        {
            clone._customParameters[key] = value;
        }

        return clone;
    }

    private static FilterResult ParseFilterExpression(Expression<Func<T, bool>> expression)
    {
        // Use the generic expression parser for all entity types
        return ExpressionParser.ParseFilter(expression);
    }

    private static string ParseIncludeExpression(Expression<Func<T, object>> expression)
    {
        // Use the generic expression parser for all entity types
        return ExpressionParser.ParseInclude(expression);
    }

    private static string ParseSortExpression(Expression<Func<T, object>> expression)
    {
        // Use the generic expression parser for all entity types
        return ExpressionParser.ParseSort(expression);
    }

    private static string GetExpressionKey(Expression expression)
    {
        // Create a unique key for the expression for caching purposes
        // This is a simplified implementation - in production, you might want a more robust approach
        return expression.ToString();
    }

    private QueryComplexity CalculateQueryComplexity()
    {
        var totalConditions = _whereConditions.Count + _whereFilters.Count + _includeExpressions.Count + _includeRelationships.Count + _orderByExpressions.Count;
        
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