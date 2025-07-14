using PlanningCenter.Api.Client.Models.Calendar;
using PlanningCenter.Api.Client.Models.Requests;

namespace PlanningCenter.Api.Client.Models;

/// <summary>
/// Service interface for the Planning Center Calendar module.
/// Follows Interface Segregation Principle - focused on calendar-related operations.
/// Provides comprehensive calendar management with built-in pagination support.
/// </summary>
public interface ICalendarService
{
    // Event management
    
    /// <summary>
    /// Gets a single event by ID.
    /// </summary>
    /// <param name="id">The event's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The event, or null if not found</returns>
    Task<Event?> GetEventAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists events with optional filtering, sorting, and pagination.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with events</returns>
    Task<IPagedResponse<Event>> ListEventsAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new event.
    /// </summary>
    /// <param name="request">The event creation request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created event</returns>
    Task<Event> CreateEventAsync(EventCreateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing event.
    /// </summary>
    /// <param name="id">The event's unique identifier</param>
    /// <param name="request">The event update request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The updated event</returns>
    Task<Event> UpdateEventAsync(string id, EventUpdateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes an event.
    /// </summary>
    /// <param name="id">The event's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task DeleteEventAsync(string id, CancellationToken cancellationToken = default);
    
    // Resource management
    
    /// <summary>
    /// Lists resources with optional filtering, sorting, and pagination.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with resources</returns>
    Task<IPagedResponse<Resource>> ListResourcesAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a single resource by ID.
    /// </summary>
    /// <param name="id">The resource's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The resource, or null if not found</returns>
    Task<Resource?> GetResourceAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new resource.
    /// </summary>
    /// <param name="request">The resource creation request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created resource</returns>
    Task<Resource> CreateResourceAsync(ResourceCreateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing resource.
    /// </summary>
    /// <param name="id">The resource's unique identifier</param>
    /// <param name="request">The resource update request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The updated resource</returns>
    Task<Resource> UpdateResourceAsync(string id, ResourceUpdateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a resource.
    /// </summary>
    /// <param name="id">The resource's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task DeleteResourceAsync(string id, CancellationToken cancellationToken = default);
    
    // Event filtering by date range
    
    /// <summary>
    /// Lists events within a specific date range.
    /// </summary>
    /// <param name="startDate">The start date for the range</param>
    /// <param name="endDate">The end date for the range</param>
    /// <param name="parameters">Additional query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with events in the date range</returns>
    Task<IPagedResponse<Event>> ListEventsByDateRangeAsync(DateTime startDate, DateTime endDate, QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    // Pagination helpers that eliminate manual pagination logic
    
    /// <summary>
    /// Gets all events matching the specified criteria.
    /// This method automatically handles pagination behind the scenes.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>All events matching the criteria</returns>
    Task<IReadOnlyList<Event>> GetAllEventsAsync(
        QueryParameters? parameters = null,
        PaginationOptions? options = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Streams events matching the specified criteria for memory-efficient processing.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>An async enumerable that yields events from all pages</returns>
    IAsyncEnumerable<Event> StreamEventsAsync(
        QueryParameters? parameters = null,
        PaginationOptions? options = null,
        CancellationToken cancellationToken = default);
}