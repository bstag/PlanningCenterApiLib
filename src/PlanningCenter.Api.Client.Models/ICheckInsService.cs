using PlanningCenter.Api.Client.Models.CheckIns;
using PlanningCenter.Api.Client.Models.Requests;

namespace PlanningCenter.Api.Client.Models;

/// <summary>
/// Service interface for the Planning Center Check-Ins module.
/// Follows Interface Segregation Principle - focused on check-ins-related operations.
/// Provides comprehensive check-in management with built-in pagination support.
/// </summary>
public interface ICheckInsService
{
    // Check-in management
    
    /// <summary>
    /// Gets a single check-in by ID.
    /// </summary>
    /// <param name="id">The check-in's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The check-in, or null if not found</returns>
    Task<CheckIn?> GetCheckInAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists check-ins with optional filtering, sorting, and pagination.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with check-ins</returns>
    Task<IPagedResponse<CheckIn>> ListCheckInsAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new check-in.
    /// </summary>
    /// <param name="request">The check-in creation request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created check-in</returns>
    Task<CheckIn> CreateCheckInAsync(CheckInCreateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing check-in.
    /// </summary>
    /// <param name="id">The check-in's unique identifier</param>
    /// <param name="request">The check-in update request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The updated check-in</returns>
    Task<CheckIn> UpdateCheckInAsync(string id, CheckInUpdateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks out a person.
    /// </summary>
    /// <param name="id">The check-in's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The updated check-in with checkout time</returns>
    Task<CheckIn> CheckOutAsync(string id, CancellationToken cancellationToken = default);
    
    // Event management
    
    /// <summary>
    /// Lists events with optional filtering, sorting, and pagination.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with events</returns>
    Task<IPagedResponse<Event>> ListEventsAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a single event by ID.
    /// </summary>
    /// <param name="id">The event's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The event, or null if not found</returns>
    Task<Event?> GetEventAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists check-ins for a specific event.
    /// </summary>
    /// <param name="eventId">The event's unique identifier</param>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with event check-ins</returns>
    Task<IPagedResponse<CheckIn>> ListEventCheckInsAsync(string eventId, QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    // Pagination helpers that eliminate manual pagination logic
    
    /// <summary>
    /// Gets all check-ins matching the specified criteria.
    /// This method automatically handles pagination behind the scenes.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>All check-ins matching the criteria</returns>
    Task<IReadOnlyList<CheckIn>> GetAllCheckInsAsync(
        QueryParameters? parameters = null,
        PaginationOptions? options = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Streams check-ins matching the specified criteria for memory-efficient processing.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>An async enumerable that yields check-ins from all pages</returns>
    IAsyncEnumerable<CheckIn> StreamCheckInsAsync(
        QueryParameters? parameters = null,
        PaginationOptions? options = null,
        CancellationToken cancellationToken = default);
}