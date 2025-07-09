using System.Linq.Expressions;
using PlanningCenter.Api.Client.Models.Requests;

namespace PlanningCenter.Api.Client.Models.Fluent;

/// <summary>
/// Fluent API context for the People module.
/// Provides LINQ-like syntax for querying and manipulating people data with built-in pagination support.
/// </summary>
public interface IPeopleFluentContext
{
    // Query building methods
    
    /// <summary>
    /// Adds a where condition to filter people.
    /// Multiple where clauses are combined with AND logic.
    /// </summary>
    /// <param name="predicate">The filter condition</param>
    /// <returns>The fluent context for method chaining</returns>
    IPeopleFluentContext Where(Expression<Func<Core.Person, bool>> predicate);
    
    /// <summary>
    /// Specifies related data to include in the response.
    /// </summary>
    /// <param name="include">The related data to include</param>
    /// <returns>The fluent context for method chaining</returns>
    IPeopleFluentContext Include(Expression<Func<Core.Person, object>> include);
    
    /// <summary>
    /// Specifies the primary sort order.
    /// </summary>
    /// <param name="orderBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    IPeopleFluentContext OrderBy(Expression<Func<Core.Person, object>> orderBy);
    
    /// <summary>
    /// Specifies the primary sort order in descending order.
    /// </summary>
    /// <param name="orderBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    IPeopleFluentContext OrderByDescending(Expression<Func<Core.Person, object>> orderBy);
    
    /// <summary>
    /// Specifies a secondary sort order.
    /// </summary>
    /// <param name="thenBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    IPeopleFluentContext ThenBy(Expression<Func<Core.Person, object>> thenBy);
    
    /// <summary>
    /// Specifies a secondary sort order in descending order.
    /// </summary>
    /// <param name="thenBy">The field to sort by</param>
    /// <returns>The fluent context for method chaining</returns>
    IPeopleFluentContext ThenByDescending(Expression<Func<Core.Person, object>> thenBy);
    
    // Execution methods with built-in pagination support
    
    /// <summary>
    /// Gets a single person by ID.
    /// </summary>
    /// <param name="id">The person's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The person, or null if not found</returns>
    Task<Core.Person?> GetAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a paginated response with the specified page size.
    /// Returns the first page with built-in navigation helpers.
    /// </summary>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with built-in navigation helpers</returns>
    Task<IPagedResponse<Core.Person>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a specific page of results.
    /// </summary>
    /// <param name="page">The page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with built-in navigation helpers</returns>
    Task<IPagedResponse<Core.Person>> GetPageAsync(int page, int pageSize = 25, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all people matching the query criteria.
    /// This method automatically handles pagination behind the scenes.
    /// Use with caution for large datasets - consider using AsAsyncEnumerable for memory efficiency.
    /// </summary>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>All people matching the criteria</returns>
    Task<IReadOnlyList<Core.Person>> GetAllAsync(PaginationOptions? options = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Streams people matching the query criteria for memory-efficient processing.
    /// This method automatically handles pagination while using minimal memory.
    /// </summary>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>An async enumerable that yields people from all pages</returns>
    IAsyncEnumerable<Core.Person> AsAsyncEnumerable(PaginationOptions? options = null, CancellationToken cancellationToken = default);
    
    // LINQ-like terminal operations
    
    /// <summary>
    /// Gets the first person matching the query criteria.
    /// Throws an exception if no person is found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first person matching the criteria</returns>
    Task<Core.Person> FirstAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the first person matching the query criteria, or null if none found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first person matching the criteria, or null</returns>
    Task<Core.Person?> FirstOrDefaultAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the first person matching the additional predicate.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first person matching all criteria</returns>
    Task<Core.Person> FirstAsync(Expression<Func<Core.Person, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the first person matching the additional predicate, or null if none found.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The first person matching all criteria, or null</returns>
    Task<Core.Person?> FirstOrDefaultAsync(Expression<Func<Core.Person, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the single person matching the query criteria.
    /// Throws an exception if zero or more than one person is found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The single person matching the criteria</returns>
    Task<Core.Person> SingleAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the single person matching the query criteria, or null if none found.
    /// Throws an exception if more than one person is found.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The single person matching the criteria, or null</returns>
    Task<Core.Person?> SingleOrDefaultAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Counts all people matching the query criteria across all pages.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The total count of people matching the criteria</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if any people exist matching the query criteria.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>True if any people match the criteria, false otherwise</returns>
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if any people exist matching the additional predicate.
    /// </summary>
    /// <param name="predicate">Additional filter condition</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>True if any people match all criteria, false otherwise</returns>
    Task<bool> AnyAsync(Expression<Func<Core.Person, bool>> predicate, CancellationToken cancellationToken = default);
    
    // Creation context
    
    /// <summary>
    /// Creates a new person creation context.
    /// </summary>
    /// <param name="request">The person creation request</param>
    /// <returns>A creation context for fluent person creation</returns>
    IPeopleCreateContext Create(PersonCreateRequest request);
}