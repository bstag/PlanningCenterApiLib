using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;
using System.Linq.Expressions;

namespace PlanningCenter.Api.Client.Fluent;

/// <summary>
/// Interface for fluent query building capabilities.
/// </summary>
/// <typeparam name="T">The resource type being queried</typeparam>
public interface IFluentQueryBuilder<T>
{
    /// <summary>
    /// Adds a filter condition to the query.
    /// </summary>
    /// <param name="field">The field to filter by</param>
    /// <param name="value">The value to filter for</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> Where(string field, object value);
    
    /// <summary>
    /// Adds multiple filter conditions to the query.
    /// </summary>
    /// <param name="filters">Dictionary of field-value pairs to filter by</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> Where(Dictionary<string, object> filters);
    
    /// <summary>
    /// Adds a filter condition using a LINQ expression.
    /// </summary>
    /// <param name="predicate">The LINQ expression to filter by</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> Where(Expression<Func<T, bool>> predicate);
    
    /// <summary>
    /// Adds an IN filter condition to the query.
    /// </summary>
    /// <param name="field">The field to filter by</param>
    /// <param name="values">The collection of values to match</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> WhereIn(string field, IEnumerable<object> values);
    
    /// <summary>
    /// Adds a NOT IN filter condition to the query.
    /// </summary>
    /// <param name="field">The field to filter by</param>
    /// <param name="values">The collection of values to exclude</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> WhereNotIn(string field, IEnumerable<object> values);
    
    /// <summary>
    /// Adds a NULL filter condition to the query.
    /// </summary>
    /// <param name="field">The field to check for null</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> WhereNull(string field);
    
    /// <summary>
    /// Adds a NOT NULL filter condition to the query.
    /// </summary>
    /// <param name="field">The field to check for not null</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> WhereNotNull(string field);
    
    /// <summary>
    /// Adds a date range filter condition to the query.
    /// </summary>
    /// <param name="field">The date field to filter by</param>
    /// <param name="startDate">The start date (inclusive)</param>
    /// <param name="endDate">The end date (inclusive)</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> WhereDateRange(string field, DateTime startDate, DateTime endDate);
    
    /// <summary>
    /// Adds a date range filter condition to the query for nullable DateTime fields.
    /// </summary>
    /// <param name="field">The nullable date field to filter by</param>
    /// <param name="startDate">The start date (inclusive)</param>
    /// <param name="endDate">The end date (inclusive)</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> WhereDateRange(string field, DateTime? startDate, DateTime? endDate);
    
    /// <summary>
    /// Adds a BETWEEN filter condition to the query for numeric values.
    /// </summary>
    /// <param name="field">The numeric field to filter by</param>
    /// <param name="minValue">The minimum value (inclusive)</param>
    /// <param name="maxValue">The maximum value (inclusive)</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> WhereBetween<TValue>(string field, TValue minValue, TValue maxValue) where TValue : IComparable<TValue>;
    
    /// <summary>
    /// Combines multiple filter conditions with AND logic.
    /// </summary>
    /// <param name="conditions">The conditions to combine with AND</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> WhereAnd(params Expression<Func<T, bool>>[] conditions);
    
    /// <summary>
    /// Combines multiple filter conditions with OR logic.
    /// </summary>
    /// <param name="conditions">The conditions to combine with OR</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> WhereOr(params Expression<Func<T, bool>>[] conditions);
    
    /// <summary>
    /// Includes related resources in the response.
    /// </summary>
    /// <param name="relationships">The relationships to include</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> Include(params string[] relationships);
    
    /// <summary>
    /// Includes related resources using a LINQ expression.
    /// </summary>
    /// <param name="include">The LINQ expression for the relationship to include</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> Include<TProperty>(Expression<Func<T, TProperty>> include);
    
    /// <summary>
    /// Includes deep nested relationships (e.g., person.households.members).
    /// </summary>
    /// <param name="relationshipPath">The dot-separated path to the nested relationship</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> IncludeDeep(string relationshipPath);
    
    /// <summary>
    /// Includes multiple deep nested relationships.
    /// </summary>
    /// <param name="relationshipPaths">The dot-separated paths to the nested relationships</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> IncludeDeep(params string[] relationshipPaths);
    
    /// <summary>
    /// Filters results based on the existence of a relationship.
    /// </summary>
    /// <param name="relationshipName">The name of the relationship to check</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> WhereHasRelationship(string relationshipName);
    
    /// <summary>
    /// Filters results based on the absence of a relationship.
    /// </summary>
    /// <param name="relationshipName">The name of the relationship to check</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> WhereDoesntHaveRelationship(string relationshipName);
    
    /// <summary>
    /// Filters results based on a condition within a related entity.
    /// </summary>
    /// <param name="relationshipName">The name of the relationship</param>
    /// <param name="field">The field in the related entity to filter by</param>
    /// <param name="value">The value to filter for</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> WhereHas(string relationshipName, string field, object value);
    
    /// <summary>
    /// Filters results based on multiple conditions within a related entity.
    /// </summary>
    /// <param name="relationshipName">The name of the relationship</param>
    /// <param name="filters">Dictionary of field-value pairs to filter by in the related entity</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> WhereHas(string relationshipName, Dictionary<string, object> filters);
    
    /// <summary>
    /// Filters results based on the count of related entities.
    /// </summary>
    /// <param name="relationshipName">The name of the relationship</param>
    /// <param name="count">The exact count to match</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> WhereRelationshipCount(string relationshipName, int count);
    
    /// <summary>
    /// Filters results based on the minimum count of related entities.
    /// </summary>
    /// <param name="relationshipName">The name of the relationship</param>
    /// <param name="minCount">The minimum count required</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> WhereRelationshipCountGreaterThan(string relationshipName, int minCount);
    
    /// <summary>
    /// Filters results based on the maximum count of related entities.
    /// </summary>
    /// <param name="relationshipName">The name of the relationship</param>
    /// <param name="maxCount">The maximum count allowed</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> WhereRelationshipCountLessThan(string relationshipName, int maxCount);
    
    /// <summary>
    /// Filters results based on a count range of related entities.
    /// </summary>
    /// <param name="relationshipName">The name of the relationship</param>
    /// <param name="minCount">The minimum count (inclusive)</param>
    /// <param name="maxCount">The maximum count (inclusive)</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> WhereRelationshipCountBetween(string relationshipName, int minCount, int maxCount);
    
    /// <summary>
    /// Orders the results by the specified field.
    /// </summary>
    /// <param name="field">The field to order by</param>
    /// <param name="descending">Whether to order in descending order</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> OrderBy(string field, bool descending = false);
    
    /// <summary>
    /// Orders the results by the specified field in ascending order.
    /// </summary>
    /// <param name="field">The field name to order by</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> OrderBy(string field);

    /// <summary>
    /// Orders the results by the specified field in ascending order.
    /// </summary>
    /// <param name="orderBy">The field to order by</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> OrderBy(Expression<Func<T, object>> orderBy);
    
    /// <summary>
    /// Orders the results by the specified field in descending order.
    /// </summary>
    /// <param name="field">The field to order by</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> OrderByDescending(string field);
    
    /// <summary>
    /// Limits the number of results returned.
    /// </summary>
    /// <param name="count">The maximum number of results to return</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> Take(int count);
    
    /// <summary>
    /// Skips the specified number of results.
    /// </summary>
    /// <param name="count">The number of results to skip</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> Skip(int count);
    
    /// <summary>
    /// Sets pagination parameters.
    /// </summary>
    /// <param name="page">The page number (1-based)</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> Page(int page, int pageSize);
    
    /// <summary>
    /// Adds a custom parameter to the query.
    /// </summary>
    /// <param name="key">The parameter name</param>
    /// <param name="value">The parameter value</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> WithParameter(string key, object value);
    
    /// <summary>
    /// Groups the results by the specified field.
    /// </summary>
    /// <param name="field">The field to group by</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> GroupBy(string field);
    
    /// <summary>
    /// Groups the results by multiple fields.
    /// </summary>
    /// <param name="fields">The fields to group by</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> GroupBy(params string[] fields);
    
    /// <summary>
    /// Groups the results using a LINQ expression.
    /// </summary>
    /// <param name="keySelector">The LINQ expression for grouping</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> GroupBy<TKey>(Expression<Func<T, TKey>> keySelector);
    
    /// <summary>
    /// Adds a HAVING clause condition to filter grouped results.
    /// </summary>
    /// <param name="field">The field to filter by in the grouped results</param>
    /// <param name="value">The value to filter for</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> Having(string field, object value);
    
    /// <summary>
    /// Adds multiple HAVING clause conditions to filter grouped results.
    /// </summary>
    /// <param name="conditions">Dictionary of field-value pairs for HAVING conditions</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> Having(Dictionary<string, object> conditions);
    
    /// <summary>
    /// Adds a HAVING clause condition using a LINQ expression.
    /// </summary>
    /// <param name="predicate">The LINQ expression for the HAVING condition</param>
    /// <returns>The query builder for method chaining</returns>
    IFluentQueryBuilder<T> Having(Expression<Func<IGrouping<object, T>, bool>> predicate);
    
    /// <summary>
    /// Builds the QueryParameters object from the fluent configuration.
    /// </summary>
    /// <returns>The configured QueryParameters</returns>
    QueryParameters Build();
}

/// <summary>
/// Interface for executing fluent queries.
/// </summary>
/// <typeparam name="T">The resource type being queried</typeparam>
public interface IFluentQueryExecutor<T> : IFluentQueryBuilder<T>
{
    /// <summary>
    /// Executes the query and returns all matching results.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A paged response containing the results</returns>
    Task<IPagedResponse<T>> ExecuteAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes the query and returns the first matching result.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The first matching result or null if none found</returns>
    Task<T?> FirstOrDefaultAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes the query and returns a single matching result.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The single matching result</returns>
    /// <exception cref="InvalidOperationException">Thrown when zero or more than one result is found</exception>
    Task<T> SingleAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes the query and returns a single matching result or null.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The single matching result or null if none found</returns>
    /// <exception cref="InvalidOperationException">Thrown when more than one result is found</exception>
    Task<T?> SingleOrDefaultAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes the query and returns the count of matching results.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The count of matching results</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes the query and returns the count of results matching the specified condition.
    /// </summary>
    /// <param name="predicate">The condition to filter by before counting</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The count of matching results</returns>
    Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes the query and returns the distinct count of the specified field.
    /// </summary>
    /// <param name="field">The field to count distinct values for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The distinct count of the specified field</returns>
    Task<int> CountDistinctAsync(string field, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes the query and returns the distinct count using a LINQ expression.
    /// </summary>
    /// <param name="selector">The LINQ expression for the field to count distinct values for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The distinct count of the selected field</returns>
    Task<int> CountDistinctAsync<TProperty>(Expression<Func<T, TProperty>> selector, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes the query and returns the sum of the specified numeric field.
    /// </summary>
    /// <param name="field">The numeric field to sum</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sum of the specified field</returns>
    Task<decimal> SumAsync(string field, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes the query and returns the sum using a LINQ expression.
    /// </summary>
    /// <param name="selector">The LINQ expression for the field to sum</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sum of the selected field</returns>
    Task<decimal> SumAsync<TProperty>(Expression<Func<T, TProperty>> selector, CancellationToken cancellationToken = default) where TProperty : struct;
    
    /// <summary>
    /// Executes the query and returns the sum of the specified field for results matching the condition.
    /// </summary>
    /// <param name="field">The numeric field to sum</param>
    /// <param name="predicate">The condition to filter by before summing</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sum of the specified field</returns>
    Task<decimal> SumAsync(string field, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes the query and returns the sum using a LINQ expression for results matching the condition.
    /// </summary>
    /// <param name="selector">The LINQ expression for the field to sum</param>
    /// <param name="predicate">The condition to filter by before summing</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sum of the selected field</returns>
    Task<decimal> SumAsync<TProperty>(Expression<Func<T, TProperty>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) where TProperty : struct;
    
    /// <summary>
    /// Executes the query and returns the distinct sum of the specified field.
    /// </summary>
    /// <param name="field">The numeric field to sum distinct values for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The distinct sum of the specified field</returns>
    Task<decimal> SumDistinctAsync(string field, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes the query and returns the distinct sum using a LINQ expression.
    /// </summary>
    /// <param name="selector">The LINQ expression for the field to sum distinct values for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The distinct sum of the selected field</returns>
    Task<decimal> SumDistinctAsync<TProperty>(Expression<Func<T, TProperty>> selector, CancellationToken cancellationToken = default) where TProperty : struct;
    
    /// <summary>
    /// Executes the query and returns the average of the specified numeric field.
    /// </summary>
    /// <param name="field">The numeric field to average</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The average of the specified field</returns>
    Task<decimal> AverageAsync(string field, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes the query and returns the average using a LINQ expression.
    /// </summary>
    /// <param name="selector">The LINQ expression for the field to average</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The average of the selected field</returns>
    Task<decimal> AverageAsync<TProperty>(Expression<Func<T, TProperty>> selector, CancellationToken cancellationToken = default) where TProperty : struct;
    
    /// <summary>
    /// Executes the query and returns the average of the specified field for results matching the condition.
    /// </summary>
    /// <param name="field">The numeric field to average</param>
    /// <param name="predicate">The condition to filter by before averaging</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The average of the specified field</returns>
    Task<decimal> AverageAsync(string field, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes the query and returns the average using a LINQ expression for results matching the condition.
    /// </summary>
    /// <param name="selector">The LINQ expression for the field to average</param>
    /// <param name="predicate">The condition to filter by before averaging</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The average of the selected field</returns>
    Task<decimal> AverageAsync<TProperty>(Expression<Func<T, TProperty>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) where TProperty : struct;
    
    /// <summary>
    /// Executes the query and returns the minimum value of the specified field.
    /// </summary>
    /// <param name="field">The field to find the minimum value for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The minimum value of the specified field</returns>
    Task<TResult> MinAsync<TResult>(string field, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes the query and returns the minimum value using a LINQ expression.
    /// </summary>
    /// <param name="selector">The LINQ expression for the field to find the minimum value for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The minimum value of the selected field</returns>
    Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes the query and returns the minimum value of the specified field for results matching the condition.
    /// </summary>
    /// <param name="field">The field to find the minimum value for</param>
    /// <param name="predicate">The condition to filter by before finding minimum</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The minimum value of the specified field</returns>
    Task<TResult> MinAsync<TResult>(string field, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes the query and returns the minimum value using a LINQ expression for results matching the condition.
    /// </summary>
    /// <param name="selector">The LINQ expression for the field to find the minimum value for</param>
    /// <param name="predicate">The condition to filter by before finding minimum</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The minimum value of the selected field</returns>
    Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes the query and returns the maximum value of the specified field.
    /// </summary>
    /// <param name="field">The field to find the maximum value for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The maximum value of the specified field</returns>
    Task<TResult> MaxAsync<TResult>(string field, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes the query and returns the maximum value using a LINQ expression.
    /// </summary>
    /// <param name="selector">The LINQ expression for the field to find the maximum value for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The maximum value of the selected field</returns>
    Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> selector, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes the query and returns the maximum value of the specified field for results matching the condition.
    /// </summary>
    /// <param name="field">The field to find the maximum value for</param>
    /// <param name="predicate">The condition to filter by before finding maximum</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The maximum value of the specified field</returns>
    Task<TResult> MaxAsync<TResult>(string field, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes the query and returns the maximum value using a LINQ expression for results matching the condition.
    /// </summary>
    /// <param name="selector">The LINQ expression for the field to find the maximum value for</param>
    /// <param name="predicate">The condition to filter by before finding maximum</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The maximum value of the selected field</returns>
    Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes the query and returns whether any results exist.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if any results exist, false otherwise</returns>
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes the query and returns all matching results.
    /// This is an alias for ExecuteAsync for backward compatibility.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A paged response containing all results</returns>
    Task<IPagedResponse<T>> AllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes the query with grouping and returns grouped results.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of grouped results</returns>
    Task<IEnumerable<IGrouping<object, T>>> GroupedAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Executes the query with grouping and returns grouped results with a specific key type.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of grouped results with the specified key type</returns>
    Task<IEnumerable<IGrouping<TKey, T>>> GroupedAsync<TKey>(CancellationToken cancellationToken = default);
}