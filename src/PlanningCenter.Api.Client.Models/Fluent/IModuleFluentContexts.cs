using System.Linq.Expressions;

namespace PlanningCenter.Api.Client.Models.Fluent;

// Placeholder interfaces for other modules - will be fully implemented in future phases

/// <summary>
/// Fluent API context for the Giving module.
/// Provides LINQ-like syntax for querying and manipulating giving data with built-in pagination support.
/// </summary>
public interface IGivingFluentContext
{
    // Query building methods
    
    /// <summary>
    /// Adds a where condition to filter donations.
    /// Multiple where clauses are combined with AND logic.
    /// </summary>
    /// <param name="predicate">The filter condition</param>
    /// <returns>The fluent context for method chaining</returns>
    IGivingFluentContext Where(Expression<Func<Giving.Donation, bool>> predicate);
    
    /// <summary>
    /// Specifies related data to include in the response.
    /// </summary>
    /// <param name="include">The related data to include</param>
    /// <returns>The fluent context for method chaining</returns>
    IGivingFluentContext Include(Expression<Func<Giving.Donation, object>> include);
    
    /// <summary>
    /// Specifies the primary sort order.
    /// </summary>
    /// <param name="orderBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    IGivingFluentContext OrderBy(Expression<Func<Giving.Donation, object>> orderBy);
    
    /// <summary>
    /// Specifies the primary sort order in descending order.
    /// </summary>
    /// <param name="orderBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    IGivingFluentContext OrderByDescending(Expression<Func<Giving.Donation, object>> orderBy);
    
    /// <summary>
    /// Specifies a secondary sort order.
    /// </summary>
    /// <param name="thenBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    IGivingFluentContext ThenBy(Expression<Func<Giving.Donation, object>> thenBy);
    
    /// <summary>
    /// Specifies a secondary sort order in descending order.
    /// </summary>
    /// <param name="thenBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    IGivingFluentContext ThenByDescending(Expression<Func<Giving.Donation, object>> thenBy);
    
    // Execution methods with built-in pagination support
    
    /// <summary>
    /// Gets a single donation by ID.
    /// </summary>
    /// <param name="id">The donation's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The donation, or null if not found</returns>
    Task<Giving.Donation?> GetAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a paginated response with the specified page size.
    /// Returns the first page with built-in navigation helpers.
    /// </summary>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with built-in navigation helpers</returns>
    Task<IPagedResponse<Giving.Donation>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a specific page of results.
    /// </summary>
    /// <param name="page">The page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with built-in navigation helpers</returns>
    Task<IPagedResponse<Giving.Donation>> GetPageAsync(int page, int pageSize = 25, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all donations matching the query criteria.
    /// This method automatically handles pagination behind the scenes.
    /// Use with caution for large datasets - consider using AsAsyncEnumerable for memory efficiency.
    /// </summary>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>All donations matching the criteria</returns>
    Task<IReadOnlyList<Giving.Donation>> GetAllAsync(PaginationOptions? options = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Streams donations matching the query criteria for memory-efficient processing.
    /// This method automatically handles pagination while using minimal memory.
    /// </summary>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>An async enumerable that yields donations from all pages</returns>
    IAsyncEnumerable<Giving.Donation> AsAsyncEnumerable(PaginationOptions? options = null, CancellationToken cancellationToken = default);
    
    // LINQ-like terminal operations
    
    /// <summary>
    /// Gets the first donation matching the query criteria.
    /// Throws an exception if no donation is found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first donation matching the criteria</returns>
    Task<Giving.Donation> FirstAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the first donation matching the query criteria, or null if none found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first donation matching the criteria, or null</returns>
    Task<Giving.Donation?> FirstOrDefaultAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the first donation matching the additional predicate.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first donation matching all criteria</returns>
    Task<Giving.Donation> FirstAsync(Expression<Func<Giving.Donation, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the first donation matching the additional predicate, or null if none found.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first donation matching all criteria, or null</returns>
    Task<Giving.Donation?> FirstOrDefaultAsync(Expression<Func<Giving.Donation, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the single donation matching the query criteria.
    /// Throws an exception if zero or more than one donation is found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The single donation matching the criteria</returns>
    Task<Giving.Donation> SingleAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the single donation matching the query criteria, or null if none found.
    /// Throws an exception if more than one donation is found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The single donation matching the criteria, or null</returns>
    Task<Giving.Donation?> SingleOrDefaultAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the single donation matching the additional predicate.
    /// Throws an exception if zero or more than one donation is found.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The single donation matching all criteria</returns>
    Task<Giving.Donation> SingleAsync(Expression<Func<Giving.Donation, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the single donation matching the additional predicate, or null if none found.
    /// Throws an exception if more than one donation is found.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The single donation matching all criteria, or null</returns>
    Task<Giving.Donation?> SingleOrDefaultAsync(Expression<Func<Giving.Donation, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Counts all donations matching the query criteria across all pages.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The total count of donations matching the criteria</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if any donations exist matching the query criteria.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>True if any donations match the criteria, false otherwise</returns>
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if any donations exist matching the additional predicate.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>True if any donations match all criteria, false otherwise</returns>
    Task<bool> AnyAsync(Expression<Func<Giving.Donation, bool>> predicate, CancellationToken cancellationToken = default);
    
    // Specialized giving operations
    
    /// <summary>
    /// Filters donations by fund.
    /// </summary>
    /// <param name="fundId">The fund identifier</param>
    /// <returns>The fluent context for method chaining</returns>
    IGivingFluentContext ByFund(string fundId);
    
    /// <summary>
    /// Filters donations by person.
    /// </summary>
    /// <param name="personId">The person identifier</param>
    /// <returns>The fluent context for method chaining</returns>
    IGivingFluentContext ByPerson(string personId);
    
    /// <summary>
    /// Filters donations by date range.
    /// </summary>
    /// <param name="startDate">The start date</param>
    /// <param name="endDate">The end date</param>
    /// <returns>The fluent context for method chaining</returns>
    IGivingFluentContext ByDateRange(DateTime startDate, DateTime endDate);
    
    /// <summary>
    /// Filters donations by minimum amount.
    /// </summary>
    /// <param name="minimumAmount">The minimum amount in cents</param>
    /// <returns>The fluent context for method chaining</returns>
    IGivingFluentContext WithMinimumAmount(long minimumAmount);
    
    /// <summary>
    /// Filters donations by maximum amount.
    /// </summary>
    /// <param name="maximumAmount">The maximum amount in cents</param>
    /// <returns>The fluent context for method chaining</returns>
    IGivingFluentContext WithMaximumAmount(long maximumAmount);
    
    /// <summary>
    /// Filters donations by amount range.
    /// </summary>
    /// <param name="minimumAmount">The minimum amount in cents</param>
    /// <param name="maximumAmount">The maximum amount in cents</param>
    /// <returns>The fluent context for method chaining</returns>
    IGivingFluentContext ByAmountRange(long minimumAmount, long maximumAmount);
    
    /// <summary>
    /// Filters donations by status.
    /// </summary>
    /// <param name="status">The donation status</param>
    /// <returns>The fluent context for method chaining</returns>
    IGivingFluentContext ByStatus(string status);
    
    /// <summary>
    /// Filters donations by batch.
    /// </summary>
    /// <param name="batchId">The batch identifier</param>
    /// <returns>The fluent context for method chaining</returns>
    IGivingFluentContext ByBatch(string batchId);
    
    // Designation relationship querying methods
    
    /// <summary>
    /// Filters donations to only include those with designations.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IGivingFluentContext WithDesignations();
    
    /// <summary>
    /// Filters donations by specific designation.
    /// </summary>
    /// <param name="designationId">The designation identifier</param>
    /// <returns>The fluent context for method chaining</returns>
    IGivingFluentContext ByDesignation(string designationId);
    
    /// <summary>
    /// Filters donations to only include those with multiple designations.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IGivingFluentContext WithMultipleDesignations();
    
    /// <summary>
    /// Filters donations by designation count.
    /// </summary>
    /// <param name="count">The exact designation count</param>
    /// <returns>The fluent context for method chaining</returns>
    IGivingFluentContext ByDesignationCount(int count);
    
    // Payment method filtering methods
    
    /// <summary>
    /// Filters donations by payment method.
    /// </summary>
    /// <param name="paymentMethod">The payment method</param>
    /// <returns>The fluent context for method chaining</returns>
    IGivingFluentContext ByPaymentMethod(string paymentMethod);
    
    /// <summary>
    /// Filters donations by transaction ID.
    /// </summary>
    /// <param name="transactionId">The transaction identifier</param>
    /// <returns>The fluent context for method chaining</returns>
    IGivingFluentContext ByTransactionId(string transactionId);
    
    /// <summary>
    /// Filters donations to only include cash donations.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IGivingFluentContext CashOnly();
    
    /// <summary>
    /// Filters donations to only include check donations.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IGivingFluentContext CheckOnly();
    
    /// <summary>
    /// Filters donations to only include credit card donations.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IGivingFluentContext CreditCardOnly();
    
    /// <summary>
    /// Filters donations to only include ACH/bank transfer donations.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IGivingFluentContext AchOnly();
    
    // Advanced aggregation methods
    
    /// <summary>
    /// Calculates the total amount of donations matching the current criteria.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The total amount in cents</returns>
    Task<long> TotalAmountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Calculates the average donation amount matching the current criteria.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The average amount in cents</returns>
    Task<double> AverageAmountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the largest donation amount matching the current criteria.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The maximum amount in cents</returns>
    Task<long> MaxAmountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the smallest donation amount matching the current criteria.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The minimum amount in cents</returns>
    Task<long> MinAmountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the total count of donations by fund.
    /// </summary>
    /// <param name="fundId">The fund identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The count of donations for the specified fund</returns>
    Task<int> CountByFundAsync(string fundId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the total count of donations by payment method.
    /// </summary>
    /// <param name="paymentMethod">The payment method</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The count of donations for the specified payment method</returns>
    Task<int> CountByPaymentMethodAsync(string paymentMethod, CancellationToken cancellationToken = default);
}

/// <summary>
/// Fluent API context for the Calendar module.
/// Provides LINQ-like syntax for querying and manipulating calendar data with built-in pagination support.
/// </summary>
public interface ICalendarFluentContext
{
    // Query building methods
    
    /// <summary>
    /// Adds a where condition to filter calendar events.
    /// Multiple where clauses are combined with AND logic.
    /// </summary>
    /// <param name="predicate">The filter condition</param>
    /// <returns>The fluent context for method chaining</returns>
    ICalendarFluentContext Where(Expression<Func<Calendar.Event, bool>> predicate);
    
    /// <summary>
    /// Specifies related data to include in the response.
    /// </summary>
    /// <param name="include">The related data to include</param>
    /// <returns>The fluent context for method chaining</returns>
    ICalendarFluentContext Include(Expression<Func<Calendar.Event, object>> include);
    
    /// <summary>
    /// Specifies the primary sort order.
    /// </summary>
    /// <param name="orderBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    ICalendarFluentContext OrderBy(Expression<Func<Calendar.Event, object>> orderBy);
    
    /// <summary>
    /// Specifies the primary sort order in descending order.
    /// </summary>
    /// <param name="orderBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    ICalendarFluentContext OrderByDescending(Expression<Func<Calendar.Event, object>> orderBy);
    
    /// <summary>
    /// Specifies a secondary sort order.
    /// </summary>
    /// <param name="thenBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    ICalendarFluentContext ThenBy(Expression<Func<Calendar.Event, object>> thenBy);
    
    /// <summary>
    /// Specifies a secondary sort order in descending order.
    /// </summary>
    /// <param name="thenBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    ICalendarFluentContext ThenByDescending(Expression<Func<Calendar.Event, object>> thenBy);
    
    // Execution methods with built-in pagination support
    
    /// <summary>
    /// Gets a single calendar event by ID.
    /// </summary>
    /// <param name="id">The event's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The calendar event, or null if not found</returns>
    Task<Calendar.Event?> GetAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a paginated response with the specified page size.
    /// Returns the first page with built-in navigation helpers.
    /// </summary>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with built-in navigation helpers</returns>
    Task<IPagedResponse<Calendar.Event>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a specific page of results.
    /// </summary>
    /// <param name="page">The page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with built-in navigation helpers</returns>
    Task<IPagedResponse<Calendar.Event>> GetPageAsync(int page, int pageSize = 25, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all calendar events matching the query criteria.
    /// This method automatically handles pagination behind the scenes.
    /// Use with caution for large datasets - consider using AsAsyncEnumerable for memory efficiency.
    /// </summary>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>All calendar events matching the criteria</returns>
    Task<IReadOnlyList<Calendar.Event>> GetAllAsync(PaginationOptions? options = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Streams calendar events matching the query criteria for memory-efficient processing.
    /// This method automatically handles pagination while using minimal memory.
    /// </summary>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>An async enumerable that yields calendar events from all pages</returns>
    IAsyncEnumerable<Calendar.Event> AsAsyncEnumerable(PaginationOptions? options = null, CancellationToken cancellationToken = default);
    
    // LINQ-like terminal operations
    
    /// <summary>
    /// Gets the first calendar event matching the query criteria.
    /// Throws an exception if no event is found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first calendar event matching the criteria</returns>
    Task<Calendar.Event> FirstAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the first calendar event matching the query criteria, or null if none found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first calendar event matching the criteria, or null</returns>
    Task<Calendar.Event?> FirstOrDefaultAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the first calendar event matching the additional predicate.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first calendar event matching all criteria</returns>
    Task<Calendar.Event> FirstAsync(Expression<Func<Calendar.Event, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the first calendar event matching the additional predicate, or null if none found.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first calendar event matching all criteria, or null</returns>
    Task<Calendar.Event?> FirstOrDefaultAsync(Expression<Func<Calendar.Event, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the single calendar event matching the query criteria.
    /// Throws an exception if zero or more than one event is found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The single calendar event matching the criteria</returns>
    Task<Calendar.Event> SingleAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the single calendar event matching the query criteria, or null if none found.
    /// Throws an exception if more than one event is found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The single calendar event matching the criteria, or null</returns>
    Task<Calendar.Event?> SingleOrDefaultAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the single calendar event matching the additional predicate.
    /// Throws an exception if zero or more than one event is found.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The single calendar event matching all criteria</returns>
    Task<Calendar.Event> SingleAsync(Expression<Func<Calendar.Event, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the single calendar event matching the additional predicate, or null if none found.
    /// Throws an exception if more than one event is found.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The single calendar event matching all criteria, or null</returns>
    Task<Calendar.Event?> SingleOrDefaultAsync(Expression<Func<Calendar.Event, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Counts all calendar events matching the query criteria across all pages.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The total count of calendar events matching the criteria</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if any calendar events exist matching the query criteria.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>True if any calendar events match the criteria, false otherwise</returns>
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if any calendar events exist matching the additional predicate.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>True if any calendar events match all criteria, false otherwise</returns>
    Task<bool> AnyAsync(Expression<Func<Calendar.Event, bool>> predicate, CancellationToken cancellationToken = default);
    
    // Specialized calendar operations
    
    /// <summary>
    /// Filters events by date range.
    /// </summary>
    /// <param name="startDate">The start date</param>
    /// <param name="endDate">The end date</param>
    /// <returns>The fluent context for method chaining</returns>
    ICalendarFluentContext ByDateRange(DateTime startDate, DateTime endDate);
    
    /// <summary>
    /// Filters events occurring today.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    ICalendarFluentContext Today();
    
    /// <summary>
    /// Filters events occurring this week.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    ICalendarFluentContext ThisWeek();
    
    /// <summary>
    /// Filters events occurring this month.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    ICalendarFluentContext ThisMonth();
    
    /// <summary>
    /// Filters events that are upcoming (in the future).
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    ICalendarFluentContext Upcoming();
    
    // Aggregation methods
    
    /// <summary>
    /// Counts events by approval status.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A dictionary with approval status as key and count as value</returns>
    Task<Dictionary<string, int>> CountByApprovalStatusAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the total count of events that require registration.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The count of events requiring registration</returns>
    Task<int> CountRegistrationRequiredAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the total count of all-day events.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The count of all-day events</returns>
    Task<int> CountAllDayEventsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Calculates the average duration of events in hours.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The average duration in hours</returns>
    Task<double> AverageDurationHoursAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Groups events by the specified key selector.
    /// </summary>
    /// <typeparam name="TKey">The type of the grouping key</typeparam>
    /// <param name="keySelector">Function to extract the grouping key from each event</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A dictionary where keys are the grouping values and values are lists of events</returns>
    Task<Dictionary<TKey, List<Calendar.Event>>> GroupByAsync<TKey>(Expression<Func<Calendar.Event, TKey>> keySelector, CancellationToken cancellationToken = default);
}

/// <summary>
/// Fluent API context for the Check-Ins module.
/// Provides LINQ-like syntax for querying and manipulating check-in data with built-in pagination support.
/// </summary>
public interface ICheckInsFluentContext
{
    // Query building methods
    
    /// <summary>
    /// Adds a where condition to filter check-ins.
    /// Multiple where clauses are combined with AND logic.
    /// </summary>
    /// <param name="predicate">The filter condition</param>
    /// <returns>The fluent context for method chaining</returns>
    ICheckInsFluentContext Where(Expression<Func<CheckIns.CheckIn, bool>> predicate);
    
    /// <summary>
    /// Specifies related data to include in the response.
    /// </summary>
    /// <param name="include">The related data to include</param>
    /// <returns>The fluent context for method chaining</returns>
    ICheckInsFluentContext Include(Expression<Func<CheckIns.CheckIn, object>> include);
    
    /// <summary>
    /// Specifies the primary sort order.
    /// </summary>
    /// <param name="orderBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    ICheckInsFluentContext OrderBy(Expression<Func<CheckIns.CheckIn, object>> orderBy);
    
    /// <summary>
    /// Specifies the primary sort order in descending order.
    /// </summary>
    /// <param name="orderBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    ICheckInsFluentContext OrderByDescending(Expression<Func<CheckIns.CheckIn, object>> orderBy);
    
    /// <summary>
    /// Specifies a secondary sort order.
    /// </summary>
    /// <param name="thenBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    ICheckInsFluentContext ThenBy(Expression<Func<CheckIns.CheckIn, object>> thenBy);
    
    /// <summary>
    /// Specifies a secondary sort order in descending order.
    /// </summary>
    /// <param name="thenBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    ICheckInsFluentContext ThenByDescending(Expression<Func<CheckIns.CheckIn, object>> thenBy);
    
    // Execution methods with built-in pagination support
    
    /// <summary>
    /// Gets a single check-in by ID.
    /// </summary>
    /// <param name="id">The check-in's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The check-in, or null if not found</returns>
    Task<CheckIns.CheckIn?> GetAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a paginated response with the specified page size.
    /// Returns the first page with built-in navigation helpers.
    /// </summary>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with built-in navigation helpers</returns>
    Task<IPagedResponse<CheckIns.CheckIn>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a specific page of results.
    /// </summary>
    /// <param name="page">The page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with built-in navigation helpers</returns>
    Task<IPagedResponse<CheckIns.CheckIn>> GetPageAsync(int page, int pageSize = 25, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all check-ins matching the query criteria.
    /// This method automatically handles pagination behind the scenes.
    /// Use with caution for large datasets - consider using AsAsyncEnumerable for memory efficiency.
    /// </summary>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>All check-ins matching the criteria</returns>
    Task<IReadOnlyList<CheckIns.CheckIn>> GetAllAsync(PaginationOptions? options = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Streams check-ins matching the query criteria for memory-efficient processing.
    /// This method automatically handles pagination while using minimal memory.
    /// </summary>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>An async enumerable that yields check-ins from all pages</returns>
    IAsyncEnumerable<CheckIns.CheckIn> AsAsyncEnumerable(PaginationOptions? options = null, CancellationToken cancellationToken = default);
    
    // LINQ-like terminal operations
    
    /// <summary>
    /// Gets the first check-in matching the query criteria.
    /// Throws an exception if no check-in is found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first check-in matching the criteria</returns>
    Task<CheckIns.CheckIn> FirstAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the first check-in matching the query criteria, or null if none found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first check-in matching the criteria, or null</returns>
    Task<CheckIns.CheckIn?> FirstOrDefaultAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the first check-in matching the additional predicate.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first check-in matching all criteria</returns>
    Task<CheckIns.CheckIn> FirstAsync(Expression<Func<CheckIns.CheckIn, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the first check-in matching the additional predicate, or null if none found.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first check-in matching all criteria, or null</returns>
    Task<CheckIns.CheckIn?> FirstOrDefaultAsync(Expression<Func<CheckIns.CheckIn, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the single check-in matching the query criteria.
    /// Throws an exception if zero or more than one check-in is found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The single check-in matching the criteria</returns>
    Task<CheckIns.CheckIn> SingleAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the single check-in matching the query criteria, or null if none found.
    /// Throws an exception if more than one check-in is found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The single check-in matching the criteria, or null</returns>
    Task<CheckIns.CheckIn?> SingleOrDefaultAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Counts all check-ins matching the query criteria across all pages.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The total count of check-ins matching the criteria</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if any check-ins exist matching the query criteria.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>True if any check-ins match the criteria, false otherwise</returns>
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if any check-ins exist matching the additional predicate.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>True if any check-ins match all criteria, false otherwise</returns>
    Task<bool> AnyAsync(Expression<Func<CheckIns.CheckIn, bool>> predicate, CancellationToken cancellationToken = default);
    
    // Specialized check-ins operations
    
    /// <summary>
    /// Filters check-ins by person.
    /// </summary>
    /// <param name="personId">The person identifier</param>
    /// <returns>The fluent context for method chaining</returns>
    ICheckInsFluentContext ByPerson(string personId);
    
    /// <summary>
    /// Filters check-ins by event.
    /// </summary>
    /// <param name="eventId">The event identifier</param>
    /// <returns>The fluent context for method chaining</returns>
    ICheckInsFluentContext ByEvent(string eventId);
    
    /// <summary>
    /// Filters check-ins by event time.
    /// </summary>
    /// <param name="eventTimeId">The event time identifier</param>
    /// <returns>The fluent context for method chaining</returns>
    ICheckInsFluentContext ByEventTime(string eventTimeId);
    
    /// <summary>
    /// Filters check-ins by location.
    /// </summary>
    /// <param name="locationId">The location identifier</param>
    /// <returns>The fluent context for method chaining</returns>
    ICheckInsFluentContext ByLocation(string locationId);
    
    /// <summary>
    /// Filters check-ins by date range.
    /// </summary>
    /// <param name="startDate">The start date</param>
    /// <param name="endDate">The end date</param>
    /// <returns>The fluent context for method chaining</returns>
    ICheckInsFluentContext ByDateRange(DateTime startDate, DateTime endDate);
    
    /// <summary>
    /// Filters check-ins occurring today.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    ICheckInsFluentContext Today();
    
    /// <summary>
    /// Filters check-ins occurring this week.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    ICheckInsFluentContext ThisWeek();
    
    /// <summary>
    /// Filters to check-ins that are currently checked in (not checked out).
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    ICheckInsFluentContext CheckedIn();
    
    /// <summary>
    /// Filters to check-ins that have been checked out.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    ICheckInsFluentContext CheckedOut();
    
    /// <summary>
    /// Filters to confirmed check-ins.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    ICheckInsFluentContext Confirmed();
    
    /// <summary>
    /// Filters to unconfirmed check-ins.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    ICheckInsFluentContext Unconfirmed();
    
    /// <summary>
    /// Filters to one-time guests.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    ICheckInsFluentContext Guests();
    
    /// <summary>
    /// Filters to regular members (non-guests).
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    ICheckInsFluentContext Members();
    
    /// <summary>
    /// Filters to check-ins with medical notes.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    ICheckInsFluentContext WithMedicalNotes();
    
    /// <summary>
    /// Filters check-ins by name containing the specified text.
    /// </summary>
    /// <param name="nameFragment">The text to search for in names</param>
    /// <returns>The fluent context for method chaining</returns>
    ICheckInsFluentContext ByNameContains(string nameFragment);
    
    /// <summary>
    /// Filters check-ins by kind/type.
    /// </summary>
    /// <param name="kind">The check-in kind</param>
    /// <returns>The fluent context for method chaining</returns>
    ICheckInsFluentContext ByKind(string kind);
    
    // Advanced aggregation methods
    
    /// <summary>
    /// Gets the total count of check-ins by event.
    /// </summary>
    /// <param name="eventId">The event identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The count of check-ins for the specified event</returns>
    Task<int> CountByEventAsync(string eventId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the total count of check-ins by location.
    /// </summary>
    /// <param name="locationId">The location identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The count of check-ins for the specified location</returns>
    Task<int> CountByLocationAsync(string locationId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the total count of currently checked-in attendees.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The count of currently checked-in attendees</returns>
    Task<int> CountCheckedInAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the total count of checked-out attendees.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The count of checked-out attendees</returns>
    Task<int> CountCheckedOutAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the total count of guest check-ins.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The count of guest check-ins</returns>
    Task<int> CountGuestsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the total count of member check-ins.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The count of member check-ins</returns>
    Task<int> CountMembersAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the total count of check-ins with medical notes.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The count of check-ins with medical notes</returns>
    Task<int> CountWithMedicalNotesAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Fluent API context for the Groups module.
/// Provides LINQ-like syntax for querying and manipulating groups data with built-in pagination support.
/// </summary>
public interface IGroupsFluentContext
{
    // Query building methods
    
    /// <summary>
    /// Adds a where condition to filter groups.
    /// Multiple where clauses are combined with AND logic.
    /// </summary>
    /// <param name="predicate">The filter condition</param>
    /// <returns>The fluent context for method chaining</returns>
    IGroupsFluentContext Where(Expression<Func<Groups.Group, bool>> predicate);
    
    /// <summary>
    /// Specifies related data to include in the response.
    /// </summary>
    /// <param name="include">The related data to include</param>
    /// <returns>The fluent context for method chaining</returns>
    IGroupsFluentContext Include(Expression<Func<Groups.Group, object>> include);
    
    /// <summary>
    /// Specifies the primary sort order.
    /// </summary>
    /// <param name="orderBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    IGroupsFluentContext OrderBy(Expression<Func<Groups.Group, object>> orderBy);
    
    /// <summary>
    /// Specifies the primary sort order in descending order.
    /// </summary>
    /// <param name="orderBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    IGroupsFluentContext OrderByDescending(Expression<Func<Groups.Group, object>> orderBy);
    
    /// <summary>
    /// Specifies a secondary sort order.
    /// </summary>
    /// <param name="thenBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    IGroupsFluentContext ThenBy(Expression<Func<Groups.Group, object>> thenBy);
    
    /// <summary>
    /// Specifies a secondary sort order in descending order.
    /// </summary>
    /// <param name="thenBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    IGroupsFluentContext ThenByDescending(Expression<Func<Groups.Group, object>> thenBy);
    
    // Execution methods with built-in pagination support
    
    /// <summary>
    /// Gets a single group by ID.
    /// </summary>
    /// <param name="id">The group's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The group, or null if not found</returns>
    Task<Groups.Group?> GetAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a paginated response with the specified page size.
    /// Returns the first page with built-in navigation helpers.
    /// </summary>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with built-in navigation helpers</returns>
    Task<IPagedResponse<Groups.Group>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a specific page of results.
    /// </summary>
    /// <param name="page">The page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with built-in navigation helpers</returns>
    Task<IPagedResponse<Groups.Group>> GetPageAsync(int page, int pageSize = 25, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all groups matching the query criteria.
    /// This method automatically handles pagination behind the scenes.
    /// Use with caution for large datasets - consider using AsAsyncEnumerable for memory efficiency.
    /// </summary>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>All groups matching the criteria</returns>
    Task<IReadOnlyList<Groups.Group>> GetAllAsync(PaginationOptions? options = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Streams groups matching the query criteria for memory-efficient processing.
    /// This method automatically handles pagination while using minimal memory.
    /// </summary>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>An async enumerable that yields groups from all pages</returns>
    IAsyncEnumerable<Groups.Group> AsAsyncEnumerable(PaginationOptions? options = null, CancellationToken cancellationToken = default);
    
    // LINQ-like terminal operations
    
    /// <summary>
    /// Gets the first group matching the query criteria.
    /// Throws an exception if no group is found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first group matching the criteria</returns>
    Task<Groups.Group> FirstAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the first group matching the query criteria, or null if none found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first group matching the criteria, or null</returns>
    Task<Groups.Group?> FirstOrDefaultAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the first group matching the additional predicate.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first group matching all criteria</returns>
    Task<Groups.Group> FirstAsync(Expression<Func<Groups.Group, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the first group matching the additional predicate, or null if none found.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first group matching all criteria, or null</returns>
    Task<Groups.Group?> FirstOrDefaultAsync(Expression<Func<Groups.Group, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the single group matching the query criteria.
    /// Throws an exception if zero or more than one group is found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The single group matching the criteria</returns>
    Task<Groups.Group> SingleAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the single group matching the query criteria, or null if none found.
    /// Throws an exception if more than one group is found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The single group matching the criteria, or null</returns>
    Task<Groups.Group?> SingleOrDefaultAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Counts all groups matching the query criteria across all pages.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The total count of groups matching the criteria</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if any groups exist matching the query criteria.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>True if any groups match the criteria, false otherwise</returns>
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if any groups exist matching the additional predicate.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>True if any groups match all criteria, false otherwise</returns>
    Task<bool> AnyAsync(Expression<Func<Groups.Group, bool>> predicate, CancellationToken cancellationToken = default);
    
    // Specialized groups operations
    
    /// <summary>
    /// Filters groups by group type.
    /// </summary>
    /// <param name="groupTypeId">The group type identifier</param>
    /// <returns>The fluent context for method chaining</returns>
    IGroupsFluentContext ByGroupType(string groupTypeId);
    
    /// <summary>
    /// Filters groups by location.
    /// </summary>
    /// <param name="locationId">The location identifier</param>
    /// <returns>The fluent context for method chaining</returns>
    IGroupsFluentContext ByLocation(string locationId);
    
    /// <summary>
    /// Filters to only active (non-archived) groups.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IGroupsFluentContext Active();
    
    /// <summary>
    /// Filters to only archived groups.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IGroupsFluentContext Archived();
    
    /// <summary>
    /// Filters groups with at least the specified number of members.
    /// </summary>
    /// <param name="minimumCount">The minimum member count</param>
    /// <returns>The fluent context for method chaining</returns>
    IGroupsFluentContext WithMinimumMembers(int minimumCount);
    
    /// <summary>
    /// Filters groups with at most the specified number of members.
    /// </summary>
    /// <param name="maximumCount">The maximum member count</param>
    /// <returns>The fluent context for method chaining</returns>
    IGroupsFluentContext WithMaximumMembers(int maximumCount);
    
    /// <summary>
    /// Filters to groups with chat enabled.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IGroupsFluentContext WithChatEnabled();
    
    /// <summary>
    /// Filters to groups with virtual meeting capabilities.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IGroupsFluentContext WithVirtualMeeting();
    
    /// <summary>
    /// Filters groups by name containing the specified text.
    /// </summary>
    /// <param name="nameFragment">The text to search for in group names</param>
    /// <returns>The fluent context for method chaining</returns>
    IGroupsFluentContext ByNameContains(string nameFragment);
    
    // Member Relationship Querying
    
    /// <summary>
    /// Include members in the response.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IGroupsFluentContext WithMembers();
    
    /// <summary>
    /// Include member roles in the response.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IGroupsFluentContext WithMemberRoles();
    
    /// <summary>
    /// Filter groups by member count range.
    /// </summary>
    /// <param name="minCount">Minimum member count</param>
    /// <param name="maxCount">Maximum member count</param>
    /// <returns>The fluent context for method chaining</returns>
    IGroupsFluentContext ByMemberCount(int minCount, int maxCount);
    
    /// <summary>
    /// Filter groups by membership type.
    /// </summary>
    /// <param name="membershipType">The membership type to filter by</param>
    /// <returns>The fluent context for method chaining</returns>
    IGroupsFluentContext ByMembershipType(string membershipType);
    
    /// <summary>
    /// Filter groups that have active members.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IGroupsFluentContext WithActiveMembers();
    
    /// <summary>
    /// Filter groups by member status.
    /// </summary>
    /// <param name="status">The member status to filter by</param>
    /// <returns>The fluent context for method chaining</returns>
    IGroupsFluentContext ByMemberStatus(string status);
    
    // Group Hierarchy Navigation
    
    /// <summary>
    /// Include parent group in the response.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IGroupsFluentContext WithParentGroup();
    
    /// <summary>
    /// Include child groups in the response.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IGroupsFluentContext WithChildGroups();
    
    /// <summary>
    /// Filter groups that have a parent group.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IGroupsFluentContext WithParent();
    
    /// <summary>
    /// Filter groups that are top-level (no parent).
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IGroupsFluentContext TopLevel();
    
    /// <summary>
    /// Filter groups by specific parent group.
    /// </summary>
    /// <param name="parentGroupId">The parent group identifier</param>
    /// <returns>The fluent context for method chaining</returns>
    IGroupsFluentContext ByParentGroup(string parentGroupId);
    
    /// <summary>
    /// Filter groups that have child groups.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IGroupsFluentContext WithChildren();
    
    /// <summary>
    /// Filter groups by hierarchy level.
    /// </summary>
    /// <param name="level">The hierarchy level</param>
    /// <returns>The fluent context for method chaining</returns>
    IGroupsFluentContext ByHierarchyLevel(int level);
    
    // Advanced Group Features
    
    /// <summary>
    /// Include group events in the response.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IGroupsFluentContext WithEvents();
    
    /// <summary>
    /// Include group resources in the response.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IGroupsFluentContext WithResources();
    
    /// <summary>
    /// Include group tags in the response.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IGroupsFluentContext WithTags();
    
    /// <summary>
    /// Filter groups by enrollment status.
    /// </summary>
    /// <param name="enrollmentStatus">The enrollment status to filter by</param>
    /// <returns>The fluent context for method chaining</returns>
    IGroupsFluentContext ByEnrollmentStatus(string enrollmentStatus);
    
    /// <summary>
    /// Filter groups that allow public enrollment.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IGroupsFluentContext PublicEnrollment();
    
    /// <summary>
    /// Filter groups by schedule.
    /// </summary>
    /// <param name="schedule">The schedule to filter by</param>
    /// <returns>The fluent context for method chaining</returns>
    IGroupsFluentContext BySchedule(string schedule);
    
    /// <summary>
    /// Include all common group relationships.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IGroupsFluentContext WithAllRelationships();
}

/// <summary>
/// Fluent API context for the Registrations module.
/// Provides LINQ-like syntax for querying and manipulating signup data with built-in pagination support.
/// </summary>
public interface IRegistrationsFluentContext
{
    // Query building methods
    
    /// <summary>
    /// Adds a where condition to filter signups.
    /// Multiple where clauses are combined with AND logic.
    /// </summary>
    /// <param name="predicate">The filter condition</param>
    /// <returns>The fluent context for method chaining</returns>
    IRegistrationsFluentContext Where(Expression<Func<Registrations.Signup, bool>> predicate);
    
    /// <summary>
    /// Specifies related data to include in the response.
    /// </summary>
    /// <param name="include">The related data to include</param>
    /// <returns>The fluent context for method chaining</returns>
    IRegistrationsFluentContext Include(Expression<Func<Registrations.Signup, object>> include);
    
    /// <summary>
    /// Specifies the primary sort order.
    /// </summary>
    /// <param name="orderBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    IRegistrationsFluentContext OrderBy(Expression<Func<Registrations.Signup, object>> orderBy);
    
    /// <summary>
    /// Specifies the primary sort order in descending order.
    /// </summary>
    /// <param name="orderBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    IRegistrationsFluentContext OrderByDescending(Expression<Func<Registrations.Signup, object>> orderBy);
    
    /// <summary>
    /// Specifies a secondary sort order.
    /// </summary>
    /// <param name="thenBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    IRegistrationsFluentContext ThenBy(Expression<Func<Registrations.Signup, object>> thenBy);
    
    /// <summary>
    /// Specifies a secondary sort order in descending order.
    /// </summary>
    /// <param name="thenBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    IRegistrationsFluentContext ThenByDescending(Expression<Func<Registrations.Signup, object>> thenBy);
    
    // Execution methods with built-in pagination support
    
    /// <summary>
    /// Gets a single signup by ID.
    /// </summary>
    /// <param name="id">The signup's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The signup, or null if not found</returns>
    Task<Registrations.Signup?> GetAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a paginated response with the specified page size.
    /// Returns the first page with built-in navigation helpers.
    /// </summary>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with built-in navigation helpers</returns>
    Task<IPagedResponse<Registrations.Signup>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a specific page of results.
    /// </summary>
    /// <param name="page">The page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with built-in navigation helpers</returns>
    Task<IPagedResponse<Registrations.Signup>> GetPageAsync(int page, int pageSize = 25, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all signups matching the query criteria.
    /// This method automatically handles pagination behind the scenes.
    /// Use with caution for large datasets - consider using AsAsyncEnumerable for memory efficiency.
    /// </summary>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>All signups matching the criteria</returns>
    Task<IReadOnlyList<Registrations.Signup>> GetAllAsync(PaginationOptions? options = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Streams signups matching the query criteria for memory-efficient processing.
    /// This method automatically handles pagination while using minimal memory.
    /// </summary>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>An async enumerable that yields signups from all pages</returns>
    IAsyncEnumerable<Registrations.Signup> AsAsyncEnumerable(PaginationOptions? options = null, CancellationToken cancellationToken = default);
    
    // LINQ-like terminal operations
    
    /// <summary>
    /// Gets the first signup matching the query criteria.
    /// Throws an exception if no signup is found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first signup matching the criteria</returns>
    Task<Registrations.Signup> FirstAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the first signup matching the query criteria, or null if none found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first signup matching the criteria, or null</returns>
    Task<Registrations.Signup?> FirstOrDefaultAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the first signup matching the additional predicate.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first signup matching all criteria</returns>
    Task<Registrations.Signup> FirstAsync(Expression<Func<Registrations.Signup, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the first signup matching the additional predicate, or null if none found.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first signup matching all criteria, or null</returns>
    Task<Registrations.Signup?> FirstOrDefaultAsync(Expression<Func<Registrations.Signup, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the single signup matching the query criteria.
    /// Throws an exception if zero or more than one signup is found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The single signup matching the criteria</returns>
    Task<Registrations.Signup> SingleAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the single signup matching the query criteria, or null if none found.
    /// Throws an exception if more than one signup is found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The single signup matching the criteria, or null</returns>
    Task<Registrations.Signup?> SingleOrDefaultAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the single signup matching the additional predicate.
    /// Throws an exception if zero or more than one signup is found.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The single signup matching all criteria</returns>
    Task<Registrations.Signup> SingleAsync(Expression<Func<Registrations.Signup, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the single signup matching the additional predicate, or null if none found.
    /// Throws an exception if more than one signup is found.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The single signup matching all criteria, or null</returns>
    Task<Registrations.Signup?> SingleOrDefaultAsync(Expression<Func<Registrations.Signup, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Counts all signups matching the query criteria across all pages.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The total count of signups matching the criteria</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if any signups exist matching the query criteria.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>True if any signups match the criteria, false otherwise</returns>
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if any signups exist matching the additional predicate.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>True if any signups match all criteria, false otherwise</returns>
    Task<bool> AnyAsync(Expression<Func<Registrations.Signup, bool>> predicate, CancellationToken cancellationToken = default);
    
    // Specialized registration operations
    
    /// <summary>
    /// Filters signups to only include active (non-archived) signups.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IRegistrationsFluentContext Active();
    
    /// <summary>
    /// Filters signups to only include archived signups.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IRegistrationsFluentContext Archived();
    
    /// <summary>
    /// Filters signups by status.
    /// </summary>
    /// <param name="status">The signup status</param>
    /// <returns>The fluent context for method chaining</returns>
    IRegistrationsFluentContext ByStatus(string status);
    
    /// <summary>
    /// Filters signups to only include those currently open for registration.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IRegistrationsFluentContext OpenForRegistration();
    
    /// <summary>
    /// Filters signups to only include those that are closed for registration.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IRegistrationsFluentContext ClosedForRegistration();
    
    /// <summary>
    /// Filters signups to only include those opening after the specified date.
    /// </summary>
    /// <param name="date">The date to filter by</param>
    /// <returns>The fluent context for method chaining</returns>
    IRegistrationsFluentContext OpeningAfter(DateTime date);
    
    /// <summary>
    /// Filters signups to only include those closing before the specified date.
    /// </summary>
    /// <param name="date">The date to filter by</param>
    /// <returns>The fluent context for method chaining</returns>
    IRegistrationsFluentContext ClosingBefore(DateTime date);
    
    /// <summary>
    /// Filters signups to only include those within the specified date range.
    /// </summary>
    /// <param name="startDate">The start date</param>
    /// <param name="endDate">The end date</param>
    /// <returns>The fluent context for method chaining</returns>
    IRegistrationsFluentContext ByDateRange(DateTime startDate, DateTime endDate);
    
    /// <summary>
    /// Filters signups to only include those with waitlist enabled.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IRegistrationsFluentContext WithWaitlistEnabled();
    
    /// <summary>
    /// Filters signups to only include those without waitlist.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IRegistrationsFluentContext WithoutWaitlist();
    
    /// <summary>
    /// Filters signups to only include those with a registration limit.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IRegistrationsFluentContext WithRegistrationLimit();
    
    /// <summary>
    /// Filters signups to only include those without a registration limit (unlimited).
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IRegistrationsFluentContext Unlimited();
    
    /// <summary>
    /// Filters signups to only include those with registrations.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IRegistrationsFluentContext WithRegistrations();
    
    /// <summary>
    /// Filters signups to only include those with people on the waitlist.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IRegistrationsFluentContext WithWaitlist();
    
    /// <summary>
    /// Filters signups to only include those with minimum registration count.
    /// </summary>
    /// <param name="minimumCount">The minimum registration count</param>
    /// <returns>The fluent context for method chaining</returns>
    IRegistrationsFluentContext WithMinimumRegistrations(int minimumCount);
    
    /// <summary>
    /// Filters signups to only include those that are full (at registration limit).
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IRegistrationsFluentContext Full();
    
    /// <summary>
    /// Filters signups to only include those that have available spots.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IRegistrationsFluentContext HasAvailableSpots();
    
    /// <summary>
    /// Filters signups to only include those requiring approval.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IRegistrationsFluentContext RequiringApproval();
    
    /// <summary>
    /// Filters signups to only include those not requiring approval.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IRegistrationsFluentContext NotRequiringApproval();
    
    /// <summary>
    /// Filters signups by category ID.
    /// </summary>
    /// <param name="categoryId">The category identifier</param>
    /// <returns>The fluent context for method chaining</returns>
    IRegistrationsFluentContext ByCategory(string categoryId);
    
    /// <summary>
    /// Filters signups by campus ID.
    /// </summary>
    /// <param name="campusId">The campus identifier</param>
    /// <returns>The fluent context for method chaining</returns>
    IRegistrationsFluentContext ByCampus(string campusId);
    
    /// <summary>
    /// Filters signups by location ID.
    /// </summary>
    /// <param name="locationId">The location identifier</param>
    /// <returns>The fluent context for method chaining</returns>
    IRegistrationsFluentContext ByLocation(string locationId);
    
    /// <summary>
    /// Filters signups to only include those containing the specified text in the name.
    /// </summary>
    /// <param name="nameFragment">The text to search for in signup names</param>
    /// <returns>The fluent context for method chaining</returns>
    IRegistrationsFluentContext ByNameContains(string nameFragment);
    
    /// <summary>
    /// Filters signups to only include those containing the specified text in the description.
    /// </summary>
    /// <param name="descriptionFragment">The text to search for in signup descriptions</param>
    /// <returns>The fluent context for method chaining</returns>
    IRegistrationsFluentContext ByDescriptionContains(string descriptionFragment);
    
    /// <summary>
    /// Gets the total registration count across all signups in the current query.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The total registration count</returns>
    Task<int> TotalRegistrationsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the total waitlist count across all signups in the current query.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The total waitlist count</returns>
    Task<int> TotalWaitlistAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the average registration count per signup in the current query.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The average registration count</returns>
    Task<double> AverageRegistrationsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Filters signups by person ID.
    /// </summary>
    /// <param name="personId">The person identifier</param>
    /// <returns>The fluent context for method chaining</returns>
    IRegistrationsFluentContext ByPerson(string personId);
    
    /// <summary>
    /// Filters signups by event ID.
    /// </summary>
    /// <param name="eventId">The event identifier</param>
    /// <returns>The fluent context for method chaining</returns>
    IRegistrationsFluentContext ByEvent(string eventId);
    
    /// <summary>
    /// Filters signups to include only those with registration limit set.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IRegistrationsFluentContext WithRegistrationLimitSet();
    
    /// <summary>
    /// Filters signups to include only those without registration limit (unlimited).
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IRegistrationsFluentContext WithoutRegistrationLimit();
    
    /// <summary>
    /// Filters signups by name matching the specified value.
    /// </summary>
    /// <param name="name">The name to match</param>
    /// <returns>The fluent context for method chaining</returns>
    IRegistrationsFluentContext ByName(string name);
}

/// <summary>
/// Fluent API context for the Publishing module.
/// Provides LINQ-like syntax for querying and manipulating episode data with built-in pagination support.
/// </summary>
public interface IPublishingFluentContext
{
    // Query building methods
    
    /// <summary>
    /// Adds a where condition to filter episodes.
    /// Multiple where clauses are combined with AND logic.
    /// </summary>
    /// <param name="predicate">The filter condition</param>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext Where(Expression<Func<Publishing.Episode, bool>> predicate);
    
    /// <summary>
    /// Specifies related data to include in the response.
    /// </summary>
    /// <param name="include">The related data to include</param>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext Include(Expression<Func<Publishing.Episode, object>> include);
    
    /// <summary>
    /// Specifies the primary sort order.
    /// </summary>
    /// <param name="orderBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext OrderBy(Expression<Func<Publishing.Episode, object>> orderBy);
    
    /// <summary>
    /// Specifies the primary sort order in descending order.
    /// </summary>
    /// <param name="orderBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext OrderByDescending(Expression<Func<Publishing.Episode, object>> orderBy);
    
    /// <summary>
    /// Specifies a secondary sort order.
    /// </summary>
    /// <param name="thenBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext ThenBy(Expression<Func<Publishing.Episode, object>> thenBy);
    
    /// <summary>
    /// Specifies a secondary sort order in descending order.
    /// </summary>
    /// <param name="thenBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext ThenByDescending(Expression<Func<Publishing.Episode, object>> thenBy);
    
    // Execution methods with built-in pagination support
    
    /// <summary>
    /// Gets a single episode by ID.
    /// </summary>
    /// <param name="id">The episode's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The episode, or null if not found</returns>
    Task<Publishing.Episode?> GetAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a paginated response with the specified page size.
    /// Returns the first page with built-in navigation helpers.
    /// </summary>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with built-in navigation helpers</returns>
    Task<IPagedResponse<Publishing.Episode>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a specific page of results.
    /// </summary>
    /// <param name="page">The page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with built-in navigation helpers</returns>
    Task<IPagedResponse<Publishing.Episode>> GetPageAsync(int page, int pageSize = 25, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all episodes matching the query criteria.
    /// This method automatically handles pagination behind the scenes.
    /// Use with caution for large datasets - consider using AsAsyncEnumerable for memory efficiency.
    /// </summary>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>All episodes matching the criteria</returns>
    Task<IReadOnlyList<Publishing.Episode>> GetAllAsync(PaginationOptions? options = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Streams episodes matching the query criteria for memory-efficient processing.
    /// This method automatically handles pagination while using minimal memory.
    /// </summary>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>An async enumerable that yields episodes from all pages</returns>
    IAsyncEnumerable<Publishing.Episode> AsAsyncEnumerable(PaginationOptions? options = null, CancellationToken cancellationToken = default);
    
    // LINQ-like terminal operations
    
    /// <summary>
    /// Gets the first episode matching the query criteria.
    /// Throws an exception if no episodes are found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first episode matching the criteria</returns>
    Task<Publishing.Episode> FirstAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the first episode matching the query criteria, or null if none found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first episode matching the criteria, or null</returns>
    Task<Publishing.Episode?> FirstOrDefaultAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the first episode matching the additional predicate.
    /// Throws an exception if no episodes are found.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first episode matching all criteria</returns>
    Task<Publishing.Episode> FirstAsync(Expression<Func<Publishing.Episode, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the first episode matching the additional predicate, or null if none found.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first episode matching all criteria, or null</returns>
    Task<Publishing.Episode?> FirstOrDefaultAsync(Expression<Func<Publishing.Episode, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the single episode matching the query criteria.
    /// Throws an exception if no episodes are found or more than one is found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The single episode matching the criteria</returns>
    Task<Publishing.Episode> SingleAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the single episode matching the query criteria, or null if none found.
    /// Throws an exception if more than one episode is found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The single episode matching the criteria, or null</returns>
    Task<Publishing.Episode?> SingleOrDefaultAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the single episode matching the additional predicate.
    /// Throws an exception if no episodes are found or more than one is found.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The single episode matching all criteria</returns>
    Task<Publishing.Episode> SingleAsync(Expression<Func<Publishing.Episode, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the single episode matching the additional predicate, or null if none found.
    /// Throws an exception if more than one episode is found.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The single episode matching all criteria, or null</returns>
    Task<Publishing.Episode?> SingleOrDefaultAsync(Expression<Func<Publishing.Episode, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Counts all episodes matching the query criteria across all pages.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The total count of episodes matching the criteria</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if any episodes exist matching the query criteria.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>True if any episodes match the criteria, false otherwise</returns>
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if any episodes exist matching the additional predicate.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>True if any episodes match all criteria, false otherwise</returns>
    Task<bool> AnyAsync(Expression<Func<Publishing.Episode, bool>> predicate, CancellationToken cancellationToken = default);
    
    // Specialized publishing operations
    
    /// <summary>
    /// Filters episodes to only include published episodes.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext Published();
    
    /// <summary>
    /// Filters episodes to only include unpublished/draft episodes.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext Unpublished();
    
    /// <summary>
    /// Filters episodes by status.
    /// </summary>
    /// <param name="status">The episode status to filter by</param>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext ByStatus(string status);
    
    /// <summary>
    /// Filters episodes published after the specified date.
    /// </summary>
    /// <param name="date">The date to filter after</param>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext PublishedAfter(DateTime date);
    
    /// <summary>
    /// Filters episodes published before the specified date.
    /// </summary>
    /// <param name="date">The date to filter before</param>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext PublishedBefore(DateTime date);
    
    /// <summary>
    /// Filters episodes published within the specified date range.
    /// </summary>
    /// <param name="startDate">The start date</param>
    /// <param name="endDate">The end date</param>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext PublishedBetween(DateTime startDate, DateTime endDate);
    
    /// <summary>
    /// Filters episodes published today.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext PublishedToday();
    
    /// <summary>
    /// Filters episodes published this week.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext PublishedThisWeek();
    
    /// <summary>
    /// Filters episodes published this month.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext PublishedThisMonth();
    
    /// <summary>
    /// Filters episodes that have video content.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext WithVideo();
    
    /// <summary>
    /// Filters episodes that have audio content.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext WithAudio();
    
    /// <summary>
    /// Filters episodes that have artwork.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext WithArtwork();
    
    /// <summary>
    /// Filters episodes that have downloadable video.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext WithVideoDownload();
    
    /// <summary>
    /// Filters episodes that have downloadable audio.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext WithAudioDownload();
    
    /// <summary>
    /// Filters episodes by minimum duration in seconds.
    /// </summary>
    /// <param name="durationInSeconds">The minimum duration in seconds</param>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext WithMinimumDuration(int durationInSeconds);
    
    /// <summary>
    /// Filters episodes by maximum duration in seconds.
    /// </summary>
    /// <param name="durationInSeconds">The maximum duration in seconds</param>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext WithMaximumDuration(int durationInSeconds);
    
    /// <summary>
    /// Filters episodes by series ID.
    /// </summary>
    /// <param name="seriesId">The series identifier</param>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext BySeries(string seriesId);
    
    /// <summary>
    /// Filters episodes by episode number.
    /// </summary>
    /// <param name="episodeNumber">The episode number</param>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext ByEpisodeNumber(int episodeNumber);
    
    /// <summary>
    /// Filters episodes by season number.
    /// </summary>
    /// <param name="seasonNumber">The season number</param>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext BySeason(int seasonNumber);
    
    /// <summary>
    /// Filters episodes containing the specified text in the title.
    /// </summary>
    /// <param name="titleFragment">The text to search for in titles</param>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext ByTitleContains(string titleFragment);
    
    /// <summary>
    /// Filters episodes containing the specified text in the description.
    /// </summary>
    /// <param name="descriptionFragment">The text to search for in descriptions</param>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext ByDescriptionContains(string descriptionFragment);
    
    /// <summary>
    /// Filters episodes that have any of the specified tags.
    /// </summary>
    /// <param name="tags">The tags to filter by</param>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext WithTags(params string[] tags);
    
    /// <summary>
    /// Filters episodes that have all of the specified tags.
    /// </summary>
    /// <param name="tags">The tags that must all be present</param>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext WithAllTags(params string[] tags);
    
    /// <summary>
    /// Filters episodes that belong to any of the specified categories.
    /// </summary>
    /// <param name="categories">The categories to filter by</param>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext InCategories(params string[] categories);
    
    /// <summary>
    /// Filters episodes with minimum view count.
    /// </summary>
    /// <param name="minimumViews">The minimum number of views</param>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext WithMinimumViews(long minimumViews);
    
    /// <summary>
    /// Filters episodes with minimum download count.
    /// </summary>
    /// <param name="minimumDownloads">The minimum number of downloads</param>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext WithMinimumDownloads(long minimumDownloads);
    
    /// <summary>
    /// Filters episodes with minimum like count.
    /// </summary>
    /// <param name="minimumLikes">The minimum number of likes</param>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext WithMinimumLikes(long minimumLikes);
    
    /// <summary>
    /// Filters episodes that have any engagement (views, downloads, or likes).
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext WithEngagement();
    
    /// <summary>
    /// Gets the total view count across all episodes in the current query.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The total view count</returns>
    Task<long> TotalViewsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the total download count across all episodes in the current query.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The total download count</returns>
    Task<long> TotalDownloadsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the total like count across all episodes in the current query.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The total like count</returns>
    Task<long> TotalLikesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the average duration in seconds across all episodes in the current query.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The average duration in seconds</returns>
    Task<double> AverageDurationAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the total duration in seconds across all episodes in the current query.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The total duration in seconds</returns>
    Task<long> TotalDurationAsync(CancellationToken cancellationToken = default);
    
    // Publishing-Specific Filters
    
    /// <summary>
    /// Filters episodes that are in draft status (not published).
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext Draft();
    
    /// <summary>
    /// Filters episodes by speaker ID.
    /// </summary>
    /// <param name="speakerId">The speaker identifier</param>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext BySpeaker(string speakerId);
    
    /// <summary>
    /// Filters episodes by series ID (alias for BySeries for consistency).
    /// </summary>
    /// <param name="seriesId">The series identifier</param>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext BySeriesId(string seriesId);
    
    /// <summary>
    /// Filters episodes that do not belong to any series.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext WithoutSeries();
    
    // Series and Episode Relationship Querying
    
    /// <summary>
    /// Filters episodes that belong to the specified series (alias for BySeries).
    /// </summary>
    /// <param name="seriesId">The series identifier</param>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext InSeries(string seriesId);
    
    /// <summary>
    /// Filters episodes that belong to any of the specified series.
    /// </summary>
    /// <param name="seriesIds">The series identifiers</param>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext InSeries(params string[] seriesIds);
    
    // Speaker Filtering Methods
    
    /// <summary>
    /// Filters episodes that have any of the specified speakers.
    /// </summary>
    /// <param name="speakerIds">The speaker identifiers</param>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext WithSpeakers(params string[] speakerIds);
    
    /// <summary>
    /// Filters episodes that do not have any speakers assigned.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext WithoutSpeakers();
    
    // Media Management Methods
    
    /// <summary>
    /// Filters episodes that have any media files attached.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext WithMedia();
    
    /// <summary>
    /// Filters episodes that do not have any media files attached.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext WithoutMedia();
    
    /// <summary>
    /// Filters episodes by media type.
    /// </summary>
    /// <param name="mediaType">The media type (video, audio, artwork)</param>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext ByMediaType(string mediaType);
    
    /// <summary>
    /// Filters episodes that have downloadable media.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext WithDownloadableMedia();
    
    /// <summary>
    /// Filters episodes that have streamable media.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IPublishingFluentContext WithStreamableMedia();
    
    // Advanced Aggregation Methods
    
    /// <summary>
    /// Gets the total number of unique series across all episodes in the current query.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The total number of unique series</returns>
    Task<int> TotalSeriesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the total number of unique speakers across all episodes in the current query.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The total number of unique speakers</returns>
    Task<int> TotalSpeakersAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the average number of episodes per series in the current query.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The average number of episodes per series</returns>
    Task<double> AverageEpisodeCountAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Fluent API context for the Services module.
/// Provides LINQ-like syntax for querying and manipulating service plans data with built-in pagination support.
/// </summary>
public interface IServicesFluentContext
{
    // Query building methods
    
    /// <summary>
    /// Adds a where condition to filter service plans.
    /// Multiple where clauses are combined with AND logic.
    /// </summary>
    /// <param name="predicate">The filter condition</param>
    /// <returns>The fluent context for method chaining</returns>
    IServicesFluentContext Where(Expression<Func<Services.Plan, bool>> predicate);
    
    /// <summary>
    /// Specifies related data to include in the response.
    /// </summary>
    /// <param name="include">The related data to include</param>
    /// <returns>The fluent context for method chaining</returns>
    IServicesFluentContext Include(Expression<Func<Services.Plan, object>> include);
    
    /// <summary>
    /// Specifies the primary sort order.
    /// </summary>
    /// <param name="orderBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    IServicesFluentContext OrderBy(Expression<Func<Services.Plan, object>> orderBy);
    
    /// <summary>
    /// Specifies the primary sort order in descending order.
    /// </summary>
    /// <param name="orderBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    IServicesFluentContext OrderByDescending(Expression<Func<Services.Plan, object>> orderBy);
    
    /// <summary>
    /// Specifies a secondary sort order.
    /// </summary>
    /// <param name="thenBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    IServicesFluentContext ThenBy(Expression<Func<Services.Plan, object>> thenBy);
    
    /// <summary>
    /// Specifies a secondary sort order in descending order.
    /// </summary>
    /// <param name="thenBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    IServicesFluentContext ThenByDescending(Expression<Func<Services.Plan, object>> thenBy);
    
    // Execution methods with built-in pagination support
    
    /// <summary>
    /// Gets a single service plan by ID.
    /// </summary>
    /// <param name="id">The plan's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The service plan, or null if not found</returns>
    Task<Services.Plan?> GetAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a paginated response with the specified page size.
    /// Returns the first page with built-in navigation helpers.
    /// </summary>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with built-in navigation helpers</returns>
    Task<IPagedResponse<Services.Plan>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a specific page of results.
    /// </summary>
    /// <param name="page">The page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with built-in navigation helpers</returns>
    Task<IPagedResponse<Services.Plan>> GetPageAsync(int page, int pageSize = 25, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all service plans matching the query criteria.
    /// This method automatically handles pagination behind the scenes.
    /// Use with caution for large datasets - consider using AsAsyncEnumerable for memory efficiency.
    /// </summary>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>All service plans matching the criteria</returns>
    Task<IReadOnlyList<Services.Plan>> GetAllAsync(PaginationOptions? options = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Streams service plans matching the query criteria for memory-efficient processing.
    /// This method automatically handles pagination while using minimal memory.
    /// </summary>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>An async enumerable that yields service plans from all pages</returns>
    IAsyncEnumerable<Services.Plan> AsAsyncEnumerable(PaginationOptions? options = null, CancellationToken cancellationToken = default);
    
    // LINQ-like terminal operations
    
    /// <summary>
    /// Gets the first service plan matching the query criteria.
    /// Throws an exception if no plan is found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first service plan matching the criteria</returns>
    Task<Services.Plan> FirstAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the first service plan matching the query criteria, or null if none found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first service plan matching the criteria, or null</returns>
    Task<Services.Plan?> FirstOrDefaultAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the first service plan matching the additional predicate.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first service plan matching all criteria</returns>
    Task<Services.Plan> FirstAsync(Expression<Func<Services.Plan, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the first service plan matching the additional predicate, or null if none found.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first service plan matching all criteria, or null</returns>
    Task<Services.Plan?> FirstOrDefaultAsync(Expression<Func<Services.Plan, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the single service plan matching the query criteria.
    /// Throws an exception if zero or more than one plan is found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The single service plan matching the criteria</returns>
    Task<Services.Plan> SingleAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the single service plan matching the query criteria, or null if none found.
    /// Throws an exception if more than one plan is found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The single service plan matching the criteria, or null</returns>
    Task<Services.Plan?> SingleOrDefaultAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Counts all service plans matching the query criteria across all pages.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The total count of service plans matching the criteria</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if any service plans exist matching the query criteria.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>True if any service plans match the criteria, false otherwise</returns>
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if any service plans exist matching the additional predicate.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>True if any service plans match all criteria, false otherwise</returns>
    Task<bool> AnyAsync(Expression<Func<Services.Plan, bool>> predicate, CancellationToken cancellationToken = default);
    
    // Specialized services operations
    
    /// <summary>
    /// Filters service plans by service type.
    /// </summary>
    /// <param name="serviceTypeId">The service type identifier</param>
    /// <returns>The fluent context for method chaining</returns>
    IServicesFluentContext ByServiceType(string serviceTypeId);
    
    /// <summary>
    /// Filters service plans by date range.
    /// </summary>
    /// <param name="startDate">The start date</param>
    /// <param name="endDate">The end date</param>
    /// <returns>The fluent context for method chaining</returns>
    IServicesFluentContext ByDateRange(DateTime startDate, DateTime endDate);
    
    /// <summary>
    /// Filters to upcoming service plans (in the future).
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IServicesFluentContext Upcoming();
    
    /// <summary>
    /// Filters to past service plans.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IServicesFluentContext Past();
    
    /// <summary>
    /// Filters service plans occurring this week.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IServicesFluentContext ThisWeek();
    
    /// <summary>
    /// Filters service plans occurring this month.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IServicesFluentContext ThisMonth();
    
    /// <summary>
    /// Filters to public service plans.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IServicesFluentContext Public();
    
    /// <summary>
    /// Filters to private service plans.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IServicesFluentContext Private();
    
    /// <summary>
    /// Filters service plans with minimum length in minutes.
    /// </summary>
    /// <param name="minimumMinutes">The minimum length in minutes</param>
    /// <returns>The fluent context for method chaining</returns>
    IServicesFluentContext WithMinimumLength(int minimumMinutes);
    
    /// <summary>
    /// Filters service plans by title containing the specified text.
    /// </summary>
    /// <param name="titleFragment">The text to search for in plan titles</param>
    /// <returns>The fluent context for method chaining</returns>
    IServicesFluentContext ByTitleContains(string titleFragment);
    
    // Additional service-specific filtering methods
    
    /// <summary>
    /// Filters service plans by date.
    /// </summary>
    /// <param name="date">The specific date</param>
    /// <returns>The fluent context for method chaining</returns>
    IServicesFluentContext ByDate(DateTime date);
    
    /// <summary>
    /// Filters service plans by status.
    /// </summary>
    /// <param name="status">The plan status</param>
    /// <returns>The fluent context for method chaining</returns>
    IServicesFluentContext ByStatus(string status);
    
    /// <summary>
    /// Filters service plans to only include those with a series.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IServicesFluentContext WithSeries();
    
    /// <summary>
    /// Filters service plans to only include those without a series.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IServicesFluentContext WithoutSeries();
    
    // Plan and item relationship querying methods
    
    /// <summary>
    /// Filters service plans to only include those with plan items.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IServicesFluentContext WithPlans();
    
    /// <summary>
    /// Filters service plans to only include those with plan items.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IServicesFluentContext WithPlanItems();
    
    /// <summary>
    /// Filters service plans by plan type.
    /// </summary>
    /// <param name="planType">The plan type</param>
    /// <returns>The fluent context for method chaining</returns>
    IServicesFluentContext ByPlanType(string planType);
    
    /// <summary>
    /// Filters service plans to only include those with songs.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IServicesFluentContext WithSongs();
    
    /// <summary>
    /// Filters service plans to only include those with media items.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IServicesFluentContext WithMedia();
    
    // Team member filtering methods
    
    /// <summary>
    /// Filters service plans to only include those with team members.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IServicesFluentContext WithTeamMembers();
    
    /// <summary>
    /// Filters service plans by team role.
    /// </summary>
    /// <param name="teamRole">The team role</param>
    /// <returns>The fluent context for method chaining</returns>
    IServicesFluentContext ByTeamRole(string teamRole);
    
    /// <summary>
    /// Filters service plans by team position.
    /// </summary>
    /// <param name="teamPosition">The team position</param>
    /// <returns>The fluent context for method chaining</returns>
    IServicesFluentContext ByTeamPosition(string teamPosition);
    
    /// <summary>
    /// Filters service plans to only include those with confirmed team members.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IServicesFluentContext WithConfirmedTeamMembers();
    
    /// <summary>
    /// Filters service plans to only include those with declined team members.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IServicesFluentContext WithDeclinedTeamMembers();
    
    /// <summary>
    /// Filters service plans by minimum team member count.
    /// </summary>
    /// <param name="minimumCount">The minimum team member count</param>
    /// <returns>The fluent context for method chaining</returns>
    IServicesFluentContext WithMinimumTeamMembers(int minimumCount);
    
    // Advanced aggregation methods
    
    /// <summary>
    /// Gets the total count of plans by service type.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The count of plans by service type</returns>
    Task<int> CountByServiceTypeAsync(string serviceTypeId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the total count of public plans.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The count of public plans</returns>
    Task<int> CountPublicPlansAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the total count of private plans.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The count of private plans</returns>
    Task<int> CountPrivatePlansAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the average plan length in minutes.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The average plan length</returns>
    Task<double> AveragePlanLengthAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Fluent API context for the Webhooks module.
/// Provides LINQ-like syntax for querying and managing webhook subscriptions with built-in pagination support.
/// </summary>
public interface IWebhooksFluentContext
{
    // Query building methods
    
    /// <summary>
    /// Adds a where condition to filter webhook subscriptions.
    /// Multiple where clauses are combined with AND logic.
    /// </summary>
    /// <param name="predicate">The filter condition</param>
    /// <returns>The fluent context for method chaining</returns>
    IWebhooksFluentContext Where(Expression<Func<Webhooks.WebhookSubscription, bool>> predicate);
    
    /// <summary>
    /// Specifies related data to include in the response.
    /// </summary>
    /// <param name="include">The related data to include</param>
    /// <returns>The fluent context for method chaining</returns>
    IWebhooksFluentContext Include(Expression<Func<Webhooks.WebhookSubscription, object>> include);
    
    /// <summary>
    /// Specifies the primary sort order.
    /// </summary>
    /// <param name="orderBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    IWebhooksFluentContext OrderBy(Expression<Func<Webhooks.WebhookSubscription, object>> orderBy);
    
    /// <summary>
    /// Specifies the primary sort order in descending order.
    /// </summary>
    /// <param name="orderBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    IWebhooksFluentContext OrderByDescending(Expression<Func<Webhooks.WebhookSubscription, object>> orderBy);
    
    /// <summary>
    /// Specifies a secondary sort order.
    /// </summary>
    /// <param name="thenBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    IWebhooksFluentContext ThenBy(Expression<Func<Webhooks.WebhookSubscription, object>> thenBy);
    
    /// <summary>
    /// Specifies a secondary sort order in descending order.
    /// </summary>
    /// <param name="thenBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    IWebhooksFluentContext ThenByDescending(Expression<Func<Webhooks.WebhookSubscription, object>> thenBy);
    
    // Execution methods with built-in pagination support
    
    /// <summary>
    /// Gets a single webhook subscription by ID.
    /// </summary>
    /// <param name="id">The subscription's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The webhook subscription, or null if not found</returns>
    Task<Webhooks.WebhookSubscription?> GetAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a paginated response with the specified page size.
    /// Returns the first page with built-in navigation helpers.
    /// </summary>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with built-in navigation helpers</returns>
    Task<IPagedResponse<Webhooks.WebhookSubscription>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a specific page of results.
    /// </summary>
    /// <param name="page">The page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with built-in navigation helpers</returns>
    Task<IPagedResponse<Webhooks.WebhookSubscription>> GetPageAsync(int page, int pageSize = 25, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all webhook subscriptions matching the query criteria.
    /// This method automatically handles pagination behind the scenes.
    /// Use with caution for large datasets - consider using AsAsyncEnumerable for memory efficiency.
    /// </summary>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>All webhook subscriptions matching the criteria</returns>
    Task<IReadOnlyList<Webhooks.WebhookSubscription>> GetAllAsync(PaginationOptions? options = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Streams webhook subscriptions matching the query criteria for memory-efficient processing.
    /// This method automatically handles pagination while using minimal memory.
    /// </summary>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>An async enumerable that yields webhook subscriptions from all pages</returns>
    IAsyncEnumerable<Webhooks.WebhookSubscription> AsAsyncEnumerable(PaginationOptions? options = null, CancellationToken cancellationToken = default);
    
    // LINQ-like terminal operations
    
    /// <summary>
    /// Gets the first webhook subscription matching the query criteria.
    /// Throws an exception if no subscriptions are found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first webhook subscription matching the criteria</returns>
    Task<Webhooks.WebhookSubscription> FirstAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the first webhook subscription matching the query criteria, or null if none found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first webhook subscription matching the criteria, or null</returns>
    Task<Webhooks.WebhookSubscription?> FirstOrDefaultAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the first webhook subscription matching the additional predicate.
    /// Throws an exception if no subscriptions are found.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first webhook subscription matching all criteria</returns>
    Task<Webhooks.WebhookSubscription> FirstAsync(Expression<Func<Webhooks.WebhookSubscription, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the first webhook subscription matching the additional predicate, or null if none found.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first webhook subscription matching all criteria, or null</returns>
    Task<Webhooks.WebhookSubscription?> FirstOrDefaultAsync(Expression<Func<Webhooks.WebhookSubscription, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the single webhook subscription matching the query criteria.
    /// Throws an exception if no subscriptions are found or more than one is found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The single webhook subscription matching the criteria</returns>
    Task<Webhooks.WebhookSubscription> SingleAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the single webhook subscription matching the query criteria, or null if none found.
    /// Throws an exception if more than one subscription is found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The single webhook subscription matching the criteria, or null</returns>
    Task<Webhooks.WebhookSubscription?> SingleOrDefaultAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the single webhook subscription matching the additional predicate.
    /// Throws an exception if no subscriptions are found or more than one is found.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The single webhook subscription matching all criteria</returns>
    Task<Webhooks.WebhookSubscription> SingleAsync(Expression<Func<Webhooks.WebhookSubscription, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the single webhook subscription matching the additional predicate, or null if none found.
    /// Throws an exception if more than one subscription is found.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The single webhook subscription matching all criteria, or null</returns>
    Task<Webhooks.WebhookSubscription?> SingleOrDefaultAsync(Expression<Func<Webhooks.WebhookSubscription, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Counts all webhook subscriptions matching the query criteria across all pages.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The total count of webhook subscriptions matching the criteria</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if any webhook subscriptions exist matching the query criteria.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>True if any webhook subscriptions match the criteria, false otherwise</returns>
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if any webhook subscriptions exist matching the additional predicate.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>True if any webhook subscriptions match all criteria, false otherwise</returns>
    Task<bool> AnyAsync(Expression<Func<Webhooks.WebhookSubscription, bool>> predicate, CancellationToken cancellationToken = default);
    
    // Specialized webhook operations
    
    /// <summary>
    /// Filters webhook subscriptions to only include active subscriptions.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IWebhooksFluentContext Active();
    
    /// <summary>
    /// Filters webhook subscriptions to only include inactive subscriptions.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IWebhooksFluentContext Inactive();
    
    /// <summary>
    /// Filters webhook subscriptions by URL pattern.
    /// </summary>
    /// <param name="urlFragment">The URL fragment to search for</param>
    /// <returns>The fluent context for method chaining</returns>
    IWebhooksFluentContext ByUrlContains(string urlFragment);
    
    /// <summary>
    /// Filters webhook subscriptions by exact URL.
    /// </summary>
    /// <param name="url">The exact URL to match</param>
    /// <returns>The fluent context for method chaining</returns>
    IWebhooksFluentContext ByExactUrl(string url);
    
    /// <summary>
    /// Filters webhook subscriptions by URL starting with the specified prefix.
    /// </summary>
    /// <param name="urlPrefix">The URL prefix to match</param>
    /// <returns>The fluent context for method chaining</returns>
    IWebhooksFluentContext ByUrlStartsWith(string urlPrefix);
    
    /// <summary>
    /// Filters webhook subscriptions by available event ID.
    /// </summary>
    /// <param name="availableEventId">The available event ID</param>
    /// <returns>The fluent context for method chaining</returns>
    IWebhooksFluentContext ByEventType(string availableEventId);
    
    /// <summary>
    /// Filters webhook subscriptions by organization ID.
    /// </summary>
    /// <param name="organizationId">The organization ID</param>
    /// <returns>The fluent context for method chaining</returns>
    IWebhooksFluentContext ByOrganization(string organizationId);
    
    /// <summary>
    /// Filters webhook subscriptions that have had recent delivery attempts.
    /// </summary>
    /// <param name="timeSpan">The time span to check for recent deliveries</param>
    /// <returns>The fluent context for method chaining</returns>
    IWebhooksFluentContext WithRecentDelivery(TimeSpan timeSpan);
    
    /// <summary>
    /// Filters webhook subscriptions by last delivery status.
    /// </summary>
    /// <param name="status">The delivery status to filter by</param>
    /// <returns>The fluent context for method chaining</returns>
    IWebhooksFluentContext ByLastDeliveryStatus(string status);
    
    /// <summary>
    /// Filters webhook subscriptions that had successful last delivery.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IWebhooksFluentContext WithSuccessfulLastDelivery();
    
    /// <summary>
    /// Filters webhook subscriptions that had failed last delivery.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IWebhooksFluentContext WithFailedLastDelivery();
    
    /// <summary>
    /// Filters webhook subscriptions by minimum success rate percentage.
    /// </summary>
    /// <param name="minimumSuccessRate">The minimum success rate (0-100)</param>
    /// <returns>The fluent context for method chaining</returns>
    IWebhooksFluentContext WithMinimumSuccessRate(double minimumSuccessRate);
    
    /// <summary>
    /// Filters webhook subscriptions with poor delivery success rate (below threshold).
    /// </summary>
    /// <param name="threshold">The success rate threshold (default: 50.0)</param>
    /// <returns>The fluent context for method chaining</returns>
    IWebhooksFluentContext WithPoorSuccessRate(double threshold = 50.0);
    
    /// <summary>
    /// Filters webhook subscriptions by maximum response time.
    /// </summary>
    /// <param name="maxResponseTimeMs">The maximum response time in milliseconds</param>
    /// <returns>The fluent context for method chaining</returns>
    IWebhooksFluentContext WithMaxResponseTime(double maxResponseTimeMs);
    
    /// <summary>
    /// Filters webhook subscriptions that are responding slowly.
    /// </summary>
    /// <param name="thresholdMs">The response time threshold in milliseconds (default: 5000)</param>
    /// <returns>The fluent context for method chaining</returns>
    IWebhooksFluentContext SlowResponding(double thresholdMs = 5000);
    
    /// <summary>
    /// Filters webhook subscriptions that are responding quickly.
    /// </summary>
    /// <param name="thresholdMs">The response time threshold in milliseconds (default: 1000)</param>
    /// <returns>The fluent context for method chaining</returns>
    IWebhooksFluentContext FastResponding(double thresholdMs = 1000);
    
    /// <summary>
    /// Filters webhook subscriptions with minimum total deliveries.
    /// </summary>
    /// <param name="minimumDeliveries">The minimum number of deliveries</param>
    /// <returns>The fluent context for method chaining</returns>
    IWebhooksFluentContext WithMinimumDeliveries(long minimumDeliveries);
    
    /// <summary>
    /// Filters webhook subscriptions with minimum failed deliveries.
    /// </summary>
    /// <param name="minimumFailures">The minimum number of failures</param>
    /// <returns>The fluent context for method chaining</returns>
    IWebhooksFluentContext WithMinimumFailures(long minimumFailures);
    
    /// <summary>
    /// Filters webhook subscriptions that have had no deliveries.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IWebhooksFluentContext WithoutDeliveries();
    
    /// <summary>
    /// Filters webhook subscriptions that have had deliveries.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IWebhooksFluentContext WithDeliveries();
    
    /// <summary>
    /// Filters webhook subscriptions by maximum retry attempts.
    /// </summary>
    /// <param name="maxRetries">The maximum number of retry attempts</param>
    /// <returns>The fluent context for method chaining</returns>
    IWebhooksFluentContext WithMaxRetries(int maxRetries);
    
    /// <summary>
    /// Filters webhook subscriptions by timeout setting.
    /// </summary>
    /// <param name="timeoutSeconds">The timeout in seconds</param>
    /// <returns>The fluent context for method chaining</returns>
    IWebhooksFluentContext WithTimeout(int timeoutSeconds);
    
    /// <summary>
    /// Filters webhook subscriptions that have custom headers configured.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IWebhooksFluentContext WithCustomHeaders();
    
    /// <summary>
    /// Filters webhook subscriptions that have a secret configured.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IWebhooksFluentContext WithSecret();
    
    /// <summary>
    /// Filters webhook subscriptions that do not have a secret configured.
    /// </summary>
    /// <returns>The fluent context for method chaining</returns>
    IWebhooksFluentContext WithoutSecret();
    
    /// <summary>
    /// Filters webhook subscriptions containing the specified text in the name.
    /// </summary>
    /// <param name="nameFragment">The text to search for in names</param>
    /// <returns>The fluent context for method chaining</returns>
    IWebhooksFluentContext ByNameContains(string nameFragment);
    
    /// <summary>
    /// Filters webhook subscriptions containing the specified text in the description.
    /// </summary>
    /// <param name="descriptionFragment">The text to search for in descriptions</param>
    /// <returns>The fluent context for method chaining</returns>
    IWebhooksFluentContext ByDescriptionContains(string descriptionFragment);
    
    /// <summary>
    /// Gets the total number of deliveries across all webhook subscriptions in the current query.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The total number of deliveries</returns>
    Task<long> TotalDeliveriesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the total number of successful deliveries across all webhook subscriptions in the current query.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The total number of successful deliveries</returns>
    Task<long> TotalSuccessfulDeliveriesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the total number of failed deliveries across all webhook subscriptions in the current query.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The total number of failed deliveries</returns>
    Task<long> TotalFailedDeliveriesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the overall success rate across all webhook subscriptions in the current query.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The overall success rate as a percentage</returns>
    Task<double> OverallSuccessRateAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the average response time across all webhook subscriptions in the current query.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The average response time in milliseconds</returns>
    Task<double> AverageResponseTimeAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the count of active webhook subscriptions in the current query.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The count of active subscriptions</returns>
    Task<int> ActiveCountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the count of inactive webhook subscriptions in the current query.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The count of inactive subscriptions</returns>
    Task<int> InactiveCountAsync(CancellationToken cancellationToken = default);
}