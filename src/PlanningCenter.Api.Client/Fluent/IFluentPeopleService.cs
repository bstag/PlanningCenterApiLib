using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PlanningCenter.Api.Client.Fluent;
using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Fluent;

/// <summary>
/// Interface for fluent People API service providing comprehensive query building capabilities.
/// </summary>
public interface IFluentPeopleService
{
    /// <summary>
    /// Creates a new fluent query builder for people.
    /// </summary>
    /// <returns>A fluent query builder instance</returns>
    IFluentQueryExecutor<Person> Query();
    
    /// <summary>
    /// Filter people by first name.
    /// </summary>
    IFluentQueryBuilder<Person> ByFirstName(string firstName);
    
    /// <summary>
    /// Filter people by last name.
    /// </summary>
    IFluentQueryBuilder<Person> ByLastName(string lastName);
    
    /// <summary>
    /// Filter people by status.
    /// </summary>
    IFluentQueryBuilder<Person> ByStatus(string status);
    
    /// <summary>
    /// Filter people by membership status.
    /// </summary>
    IFluentQueryBuilder<Person> ByMembershipStatus(string membershipStatus);
    
    /// <summary>
    /// Filter people by campus.
    /// </summary>
    IFluentQueryBuilder<Person> ByCampus(string campusId);
    
    /// <summary>
    /// Include addresses in the response.
    /// </summary>
    IFluentQueryBuilder<Person> WithAddresses();
    
    /// <summary>
    /// Include emails in the response.
    /// </summary>
    IFluentQueryBuilder<Person> WithEmails();
    
    /// <summary>
    /// Include phone numbers in the response.
    /// </summary>
    IFluentQueryBuilder<Person> WithPhoneNumbers();
    
    /// <summary>
    /// Include households in the response.
    /// </summary>
    IFluentQueryBuilder<Person> WithHouseholds();
    
    /// <summary>
    /// Include all common relationships.
    /// </summary>
    IFluentQueryBuilder<Person> WithAllRelationships();
    
    /// <summary>
    /// Creates a fluent query builder with an initial filter condition.
    /// </summary>
    /// <param name="field">The field to filter by</param>
    /// <param name="value">The value to filter for</param>
    /// <returns>A fluent query builder instance with the filter applied</returns>
    IFluentQueryBuilder<Person> Where(string field, object value);
    
    /// <summary>
    /// Creates a fluent query builder with multiple filter conditions.
    /// </summary>
    /// <param name="filters">Dictionary of field-value pairs to filter by</param>
    /// <returns>A fluent query builder instance with the filters applied</returns>
    IFluentQueryBuilder<Person> Where(Dictionary<string, object> filters);
    
    /// <summary>
    /// Creates a fluent query builder with included relationships.
    /// </summary>
    /// <param name="relationships">The relationships to include</param>
    /// <returns>A fluent query builder instance with the includes applied</returns>
    IFluentQueryBuilder<Person> Include(params string[] relationships);
    
    /// <summary>
    /// Creates a fluent query builder with ordering.
    /// </summary>
    /// <param name="field">The field to order by</param>
    /// <param name="descending">Whether to order in descending order</param>
    /// <returns>A fluent query builder instance with the ordering applied</returns>
    IFluentQueryBuilder<Person> OrderBy(string field, bool descending = false);
    
    /// <summary>
    /// Creates a fluent query builder with descending ordering.
    /// </summary>
    /// <param name="field">The field to order by</param>
    /// <returns>A fluent query builder instance with the descending ordering applied</returns>
    IFluentQueryBuilder<Person> OrderByDescending(string field);
    
    /// <summary>
    /// Creates a fluent query builder with a limit on the number of results.
    /// </summary>
    /// <param name="count">The maximum number of results to return</param>
    /// <returns>A fluent query builder instance with the limit applied</returns>
    IFluentQueryBuilder<Person> Take(int count);
    
    /// <summary>
    /// Creates a fluent query builder with pagination.
    /// </summary>
    /// <param name="page">The page number (1-based)</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <returns>A fluent query builder instance with the pagination applied</returns>
    IFluentQueryBuilder<Person> Page(int page, int pageSize);
    
    /// <summary>
    /// Gets all people without any filters.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A paged response containing all people</returns>
    Task<IPagedResponse<Person>> AllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the first person or null if none exist.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The first person or null</returns>
    Task<Person?> FirstOrDefaultAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a single person, throwing an exception if zero or more than one exist.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The single person</returns>
    /// <exception cref="InvalidOperationException">Thrown when zero or more than one result is found</exception>
    Task<Person> SingleAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a single person or null if none exist, throwing an exception if more than one exists.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The single person or null</returns>
    /// <exception cref="InvalidOperationException">Thrown when more than one result is found</exception>
    Task<Person?> SingleOrDefaultAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the count of all people.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The count of people</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if any people exist.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if any people exist, false otherwise</returns>
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
}