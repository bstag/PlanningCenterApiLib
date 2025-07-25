using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PlanningCenter.Api.Client.Fluent;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.Calendar;

namespace PlanningCenter.Api.Client.Fluent;

/// <summary>
/// Interface for fluent Calendar API service providing comprehensive query building capabilities.
/// </summary>
public interface IFluentCalendarService
{
    /// <summary>
    /// Creates a new fluent query builder for events.
    /// </summary>
    /// <returns>A fluent query builder instance</returns>
    IFluentQueryExecutor<Event> Query();
    
    #region Event Filtering Methods
    
    /// <summary>
    /// Filter events by name.
    /// </summary>
    IFluentQueryBuilder<Event> ByName(string name);
    
    /// <summary>
    /// Filter events by date range.
    /// </summary>
    IFluentQueryBuilder<Event> ByDateRange(DateTime startDate, DateTime endDate);
    
    /// <summary>
    /// Filter events by location.
    /// </summary>
    IFluentQueryBuilder<Event> ByLocation(string location);
    
    /// <summary>
    /// Filter events by approval status.
    /// </summary>
    IFluentQueryBuilder<Event> ByApprovalStatus(string approvalStatus);
    
    /// <summary>
    /// Filter events by visibility in Church Center.
    /// </summary>
    IFluentQueryBuilder<Event> ByChurchCenterVisibility(bool visible);
    
    /// <summary>
    /// Filter events by registration requirement.
    /// </summary>
    IFluentQueryBuilder<Event> ByRegistrationRequired(bool required);
    
    /// <summary>
    /// Filter events by owner.
    /// </summary>
    IFluentQueryBuilder<Event> ByOwner(string ownerName);
    
    /// <summary>
    /// Filter all-day events.
    /// </summary>
    IFluentQueryBuilder<Event> ByAllDay(bool allDay);
    
    #endregion
    
    #region Event Include Methods
    
    /// <summary>
    /// Include event instances in the response.
    /// </summary>
    IFluentQueryBuilder<Event> WithEventInstances();
    
    /// <summary>
    /// Include event resource requests in the response.
    /// </summary>
    IFluentQueryBuilder<Event> WithResourceRequests();
    
    /// <summary>
    /// Include owner information in the response.
    /// </summary>
    IFluentQueryBuilder<Event> WithOwner();
    
    /// <summary>
    /// Include tags in the response.
    /// </summary>
    IFluentQueryBuilder<Event> WithTags();
    
    /// <summary>
    /// Include all common relationships.
    /// </summary>
    IFluentQueryBuilder<Event> WithAllRelationships();
    
    #endregion
    
    #region Event Ordering Methods
    
    /// <summary>
    /// Order events by start date in ascending order.
    /// </summary>
    IFluentQueryBuilder<Event> OrderByStartDate();
    
    /// <summary>
    /// Order events by start date in descending order.
    /// </summary>
    IFluentQueryBuilder<Event> OrderByStartDateDescending();
    
    /// <summary>
    /// Order events by name in ascending order.
    /// </summary>
    IFluentQueryBuilder<Event> OrderByName();
    
    /// <summary>
    /// Order events by name in descending order.
    /// </summary>
    IFluentQueryBuilder<Event> OrderByNameDescending();
    
    /// <summary>
    /// Order events by creation date in ascending order.
    /// </summary>
    IFluentQueryBuilder<Event> OrderByCreatedDate();
    
    /// <summary>
    /// Order events by creation date in descending order.
    /// </summary>
    IFluentQueryBuilder<Event> OrderByCreatedDateDescending();
    
    #endregion
    
    #region Resource Query Methods
    
    /// <summary>
    /// Creates a new fluent query builder for resources.
    /// </summary>
    IFluentQueryExecutor<Resource> QueryResources();
    
    /// <summary>
    /// Filter resources by name.
    /// </summary>
    IFluentQueryBuilder<Resource> ResourcesByName(string name);
    
    /// <summary>
    /// Filter resources by kind.
    /// </summary>
    IFluentQueryBuilder<Resource> ResourcesByKind(string kind);
    
    /// <summary>
    /// Filter resources by home location.
    /// </summary>
    IFluentQueryBuilder<Resource> ResourcesByLocation(string location);
    
    /// <summary>
    /// Filter resources by expiration status.
    /// </summary>
    IFluentQueryBuilder<Resource> ResourcesByExpiration(bool expires);
    
    /// <summary>
    /// Filter resources by minimum quantity.
    /// </summary>
    IFluentQueryBuilder<Resource> ResourcesByMinQuantity(int minQuantity);
    
    #endregion
    
    #region Base Query Methods
    
    /// <summary>
    /// Creates a fluent query builder with an initial filter condition.
    /// </summary>
    /// <param name="field">The field to filter by</param>
    /// <param name="value">The value to filter for</param>
    /// <returns>A fluent query builder instance with the filter applied</returns>
    IFluentQueryBuilder<Event> Where(string field, object value);
    
    /// <summary>
    /// Creates a fluent query builder with multiple filter conditions.
    /// </summary>
    /// <param name="filters">Dictionary of field-value pairs to filter by</param>
    /// <returns>A fluent query builder instance with the filters applied</returns>
    IFluentQueryBuilder<Event> Where(Dictionary<string, object> filters);
    
    /// <summary>
    /// Creates a fluent query builder with included relationships.
    /// </summary>
    /// <param name="relationships">The relationships to include</param>
    /// <returns>A fluent query builder instance with the includes applied</returns>
    IFluentQueryBuilder<Event> Include(params string[] relationships);
    
    /// <summary>
    /// Creates a fluent query builder with ordering.
    /// </summary>
    /// <param name="field">The field to order by</param>
    /// <param name="descending">Whether to order in descending order</param>
    /// <returns>A fluent query builder instance with the ordering applied</returns>
    IFluentQueryBuilder<Event> OrderBy(string field, bool descending = false);
    
    /// <summary>
    /// Creates a fluent query builder with descending ordering.
    /// </summary>
    /// <param name="field">The field to order by</param>
    /// <returns>A fluent query builder instance with the descending ordering applied</returns>
    IFluentQueryBuilder<Event> OrderByDescending(string field);
    
    /// <summary>
    /// Creates a fluent query builder with a limit on the number of results.
    /// </summary>
    /// <param name="count">The maximum number of results to return</param>
    /// <returns>A fluent query builder instance with the limit applied</returns>
    IFluentQueryBuilder<Event> Take(int count);
    
    /// <summary>
    /// Creates a fluent query builder with pagination.
    /// </summary>
    /// <param name="page">The page number (1-based)</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <returns>A fluent query builder instance with the pagination applied</returns>
    IFluentQueryBuilder<Event> Page(int page, int pageSize);
    
    /// <summary>
    /// Gets all events without any filters.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A paged response containing all events</returns>
    Task<IPagedResponse<Event>> AllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the first event or null if none exist.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The first event or null</returns>
    Task<Event?> FirstOrDefaultAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a single event, throwing an exception if zero or more than one exist.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The single event</returns>
    /// <exception cref="InvalidOperationException">Thrown when zero or more than one result is found</exception>
    Task<Event> SingleAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a single event or null if none exist, throwing an exception if more than one exists.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The single event or null</returns>
    /// <exception cref="InvalidOperationException">Thrown when more than one result is found</exception>
    Task<Event?> SingleOrDefaultAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the count of all events.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The count of events</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if any events exist.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if any events exist, false otherwise</returns>
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
    
    #endregion
}