using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PlanningCenter.Api.Client.Fluent;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.Giving;

namespace PlanningCenter.Api.Client.Fluent;

/// <summary>
/// Interface for fluent Giving API service providing comprehensive query building capabilities.
/// </summary>
public interface IFluentGivingService
{
    /// <summary>
    /// Creates a new fluent query builder for donations.
    /// </summary>
    /// <returns>A fluent query builder instance</returns>
    IFluentQueryExecutor<Donation> Query();
    
    /// <summary>
    /// Filter donations by person ID.
    /// </summary>
    IFluentQueryBuilder<Donation> ByPerson(string personId);
    
    /// <summary>
    /// Filter donations by batch ID.
    /// </summary>
    IFluentQueryBuilder<Donation> ByBatch(string batchId);
    
    /// <summary>
    /// Filter donations by campus ID.
    /// </summary>
    IFluentQueryBuilder<Donation> ByCampus(string campusId);
    
    /// <summary>
    /// Filter donations by payment method.
    /// </summary>
    IFluentQueryBuilder<Donation> ByPaymentMethod(string paymentMethod);
    
    /// <summary>
    /// Filter donations by payment status.
    /// </summary>
    IFluentQueryBuilder<Donation> ByPaymentStatus(string paymentStatus);
    
    /// <summary>
    /// Filter donations by date range.
    /// </summary>
    IFluentQueryBuilder<Donation> ByDateRange(DateTime startDate, DateTime endDate);
    
    /// <summary>
    /// Filter donations by minimum amount.
    /// </summary>
    IFluentQueryBuilder<Donation> ByMinAmount(decimal minAmount);
    
    /// <summary>
    /// Filter donations by maximum amount.
    /// </summary>
    IFluentQueryBuilder<Donation> ByMaxAmount(decimal maxAmount);
    
    /// <summary>
    /// Filter donations by amount range.
    /// </summary>
    IFluentQueryBuilder<Donation> ByAmountRange(decimal minAmount, decimal maxAmount);
    
    /// <summary>
    /// Filter donations by refund status.
    /// </summary>
    IFluentQueryBuilder<Donation> ByRefundStatus(bool isRefunded);
    
    /// <summary>
    /// Include person information in the response.
    /// </summary>
    IFluentQueryBuilder<Donation> WithPerson();
    
    /// <summary>
    /// Include batch information in the response.
    /// </summary>
    IFluentQueryBuilder<Donation> WithBatch();
    
    /// <summary>
    /// Include campus information in the response.
    /// </summary>
    IFluentQueryBuilder<Donation> WithCampus();
    
    /// <summary>
    /// Include payment source information in the response.
    /// </summary>
    IFluentQueryBuilder<Donation> WithPaymentSource();
    
    /// <summary>
    /// Include designations in the response.
    /// </summary>
    IFluentQueryBuilder<Donation> WithDesignations();
    
    /// <summary>
    /// Include refund information in the response.
    /// </summary>
    IFluentQueryBuilder<Donation> WithRefund();
    
    /// <summary>
    /// Include all common relationships.
    /// </summary>
    IFluentQueryBuilder<Donation> WithAllRelationships();
    
    /// <summary>
    /// Order donations by amount in ascending order.
    /// </summary>
    IFluentQueryBuilder<Donation> OrderByAmount();
    
    /// <summary>
    /// Order donations by amount in descending order.
    /// </summary>
    IFluentQueryBuilder<Donation> OrderByAmountDescending();
    
    /// <summary>
    /// Order donations by received date in ascending order.
    /// </summary>
    IFluentQueryBuilder<Donation> OrderByReceivedDate();
    
    /// <summary>
    /// Order donations by received date in descending order.
    /// </summary>
    IFluentQueryBuilder<Donation> OrderByReceivedDateDescending();
    
    /// <summary>
    /// Creates a fluent query builder with an initial filter condition.
    /// </summary>
    /// <param name="field">The field to filter by</param>
    /// <param name="value">The value to filter for</param>
    /// <returns>A fluent query builder instance with the filter applied</returns>
    IFluentQueryBuilder<Donation> Where(string field, object value);
    
    /// <summary>
    /// Creates a fluent query builder with multiple filter conditions.
    /// </summary>
    /// <param name="filters">Dictionary of field-value pairs to filter by</param>
    /// <returns>A fluent query builder instance with the filters applied</returns>
    IFluentQueryBuilder<Donation> Where(Dictionary<string, object> filters);
    
    /// <summary>
    /// Creates a fluent query builder with included relationships.
    /// </summary>
    /// <param name="relationships">The relationships to include</param>
    /// <returns>A fluent query builder instance with the includes applied</returns>
    IFluentQueryBuilder<Donation> Include(params string[] relationships);
    
    /// <summary>
    /// Creates a fluent query builder with ordering.
    /// </summary>
    /// <param name="field">The field to order by</param>
    /// <param name="descending">Whether to order in descending order</param>
    /// <returns>A fluent query builder instance with the ordering applied</returns>
    IFluentQueryBuilder<Donation> OrderBy(string field, bool descending = false);
    
    /// <summary>
    /// Creates a fluent query builder with descending ordering.
    /// </summary>
    /// <param name="field">The field to order by</param>
    /// <returns>A fluent query builder instance with the descending ordering applied</returns>
    IFluentQueryBuilder<Donation> OrderByDescending(string field);
    
    /// <summary>
    /// Creates a fluent query builder with a limit on the number of results.
    /// </summary>
    /// <param name="count">The maximum number of results to return</param>
    /// <returns>A fluent query builder instance with the limit applied</returns>
    IFluentQueryBuilder<Donation> Take(int count);
    
    /// <summary>
    /// Creates a fluent query builder with pagination.
    /// </summary>
    /// <param name="page">The page number (1-based)</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <returns>A fluent query builder instance with the pagination applied</returns>
    IFluentQueryBuilder<Donation> Page(int page, int pageSize);
    
    /// <summary>
    /// Gets all donations without any filters.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A paged response containing all donations</returns>
    Task<IPagedResponse<Donation>> AllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the first donation or null if none exist.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The first donation or null</returns>
    Task<Donation?> FirstOrDefaultAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a single donation, throwing an exception if zero or more than one exist.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The single donation</returns>
    /// <exception cref="InvalidOperationException">Thrown when zero or more than one result is found</exception>
    Task<Donation> SingleAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a single donation or null if none exist, throwing an exception if more than one exists.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The single donation or null</returns>
    /// <exception cref="InvalidOperationException">Thrown when more than one result is found</exception>
    Task<Donation?> SingleOrDefaultAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the count of all donations.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The count of donations</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if any donations exist.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if any donations exist, false otherwise</returns>
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
}