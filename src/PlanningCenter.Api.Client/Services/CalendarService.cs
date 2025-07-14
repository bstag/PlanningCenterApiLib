using Microsoft.Extensions.Logging;
using PlanningCenter.Api.Client.Mapping.Calendar;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Exceptions;
using PlanningCenter.Api.Client.Models.JsonApi;
using PlanningCenter.Api.Client.Models.JsonApi.Calendar;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Models.Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace PlanningCenter.Api.Client.Services;

/// <summary>
/// Service implementation for the Planning Center Calendar module.
/// Follows Single Responsibility Principle - handles only Calendar API operations.
/// Follows Dependency Inversion Principle - depends on abstractions.
/// </summary>
public class CalendarService(IApiConnection apiConnection, ILogger<CalendarService> logger) : ICalendarService
{
    private readonly IApiConnection _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
    private readonly ILogger<CalendarService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private const string BaseEndpoint = "/calendar/v2";

    // Event management

    /// <summary>
    /// Gets a single event by ID.
    /// </summary>
    public async Task<Event?> GetEventAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Event ID cannot be null or empty.", nameof(id));

        _logger.LogDebug("Getting event with ID: {EventId}", id);

        try
        {
            var response = await _apiConnection.GetAsync<JsonApiSingleResponse<EventDto>>(
                $"{BaseEndpoint}/events/{id}", cancellationToken);

            if (response?.Data == null)
            {
                _logger.LogDebug("Event with ID {EventId} not found", id);
                return null;
            }

            var eventItem = CalendarMapper.MapToDomain(response.Data);
            _logger.LogDebug("Successfully retrieved event: {EventName} (ID: {EventId})", eventItem.Name, eventItem.Id);
            return eventItem;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            _logger.LogDebug("Event with ID {EventId} not found", id);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting event with ID: {EventId}", id);
            throw;
        }
    }

    /// <summary>
    /// Lists events with optional filtering, sorting, and pagination.
    /// </summary>
    public async Task<IPagedResponse<Event>> ListEventsAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Listing events with parameters: {@Parameters}", parameters);

        try
        {
            var response = await _apiConnection.GetPagedAsync<EventDto>(
                $"{BaseEndpoint}/events", parameters, cancellationToken);

            var events = response.Data.Select(CalendarMapper.MapToDomain).ToList();

            _logger.LogDebug("Successfully retrieved {Count} events", events.Count);

            return new PagedResponse<Event>
            {
                Data = events,
                Meta = response.Meta,
                Links = response.Links
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing events");
            throw;
        }
    }

    /// <summary>
    /// Creates a new event.
    /// </summary>
    public async Task<Event> CreateEventAsync(EventCreateRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Event name is required.", nameof(request));

        _logger.LogDebug("Creating event: {EventName}", request.Name);

        try
        {
            var jsonApiRequest = CalendarMapper.MapCreateRequestToJsonApi(request);

            var response = await _apiConnection.PostAsync<JsonApiSingleResponse<EventDto>>(
                $"{BaseEndpoint}/events", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
                throw new PlanningCenterApiGeneralException("Failed to create event - no data returned");

            var eventItem = CalendarMapper.MapToDomain(response.Data);
            _logger.LogInformation("Successfully created event: {EventName} (ID: {EventId})", eventItem.Name, eventItem.Id);
            return eventItem;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating event: {EventName}", request.Name);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing event.
    /// </summary>
    public async Task<Event> UpdateEventAsync(string id, EventUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Event ID cannot be null or empty.", nameof(id));

        ArgumentNullException.ThrowIfNull(request);

        _logger.LogDebug("Updating event with ID: {EventId}", id);

        try
        {
            var jsonApiRequest = CalendarMapper.MapUpdateRequestToJsonApi(request);

            var response = await _apiConnection.PatchAsync<JsonApiSingleResponse<EventDto>>(
                $"{BaseEndpoint}/events/{id}", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
                throw new PlanningCenterApiGeneralException("Failed to update event - no data returned");

            var eventItem = CalendarMapper.MapToDomain(response.Data);
            _logger.LogInformation("Successfully updated event: {EventName} (ID: {EventId})", eventItem.Name, eventItem.Id);
            return eventItem;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating event with ID: {EventId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes an event.
    /// </summary>
    public async Task DeleteEventAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Event ID cannot be null or empty.", nameof(id));

        _logger.LogDebug("Deleting event with ID: {EventId}", id);

        try
        {
            await _apiConnection.DeleteAsync($"{BaseEndpoint}/events/{id}", cancellationToken);
            _logger.LogInformation("Successfully deleted event with ID: {EventId}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting event with ID: {EventId}", id);
            throw;
        }
    }

    // Resource management

    /// <summary>
    /// Lists resources with optional filtering, sorting, and pagination.
    /// </summary>
    public async Task<IPagedResponse<Resource>> ListResourcesAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Listing resources with parameters: {@Parameters}", parameters);

        try
        {
            var response = await _apiConnection.GetPagedAsync<ResourceDto>(
                $"{BaseEndpoint}/resources", parameters, cancellationToken);

            var resources = response.Data.Select(CalendarMapper.MapResourceToDomain).ToList();

            _logger.LogDebug("Successfully retrieved {Count} resources", resources.Count);

            return new PagedResponse<Resource>
            {
                Data = resources,
                Meta = response.Meta,
                Links = response.Links
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing resources");
            throw;
        }
    }

    /// <summary>
    /// Gets a single resource by ID.
    /// </summary>
    public async Task<Resource?> GetResourceAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Resource ID cannot be null or empty.", nameof(id));

        _logger.LogDebug("Getting resource with ID: {ResourceId}", id);

        try
        {
            var response = await _apiConnection.GetAsync<JsonApiSingleResponse<ResourceDto>>(
                $"{BaseEndpoint}/resources/{id}", cancellationToken);

            if (response?.Data == null)
            {
                _logger.LogDebug("Resource with ID {ResourceId} not found", id);
                return null;
            }

            var resource = CalendarMapper.MapResourceToDomain(response.Data);
            _logger.LogDebug("Successfully retrieved resource: {ResourceName} (ID: {ResourceId})", resource.Name, resource.Id);
            return resource;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            _logger.LogDebug("Resource with ID {ResourceId} not found", id);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting resource with ID: {ResourceId}", id);
            throw;
        }
    }

    /// <summary>
    /// Creates a new resource.
    /// </summary>
    public async Task<Resource> CreateResourceAsync(ResourceCreateRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Resource name is required.", nameof(request));

        _logger.LogDebug("Creating resource: {ResourceName}", request.Name);

        try
        {
            var jsonApiRequest = CalendarMapper.MapResourceCreateRequestToJsonApi(request);

            var response = await _apiConnection.PostAsync<JsonApiSingleResponse<ResourceDto>>(
                $"{BaseEndpoint}/resources", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
                throw new PlanningCenterApiGeneralException("Failed to create resource - no data returned");

            var resource = CalendarMapper.MapResourceToDomain(response.Data);
            _logger.LogInformation("Successfully created resource: {ResourceName} (ID: {ResourceId})", resource.Name, resource.Id);
            return resource;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating resource: {ResourceName}", request.Name);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing resource.
    /// </summary>
    public async Task<Resource> UpdateResourceAsync(string id, ResourceUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Resource ID cannot be null or empty.", nameof(id));

        ArgumentNullException.ThrowIfNull(request);

        _logger.LogDebug("Updating resource with ID: {ResourceId}", id);

        try
        {
            var jsonApiRequest = CalendarMapper.MapResourceUpdateRequestToJsonApi(request);

            var response = await _apiConnection.PatchAsync<JsonApiSingleResponse<ResourceDto>>(
                $"{BaseEndpoint}/resources/{id}", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
                throw new PlanningCenterApiGeneralException("Failed to update resource - no data returned");

            var resource = CalendarMapper.MapResourceToDomain(response.Data);
            _logger.LogInformation("Successfully updated resource: {ResourceName} (ID: {ResourceId})", resource.Name, resource.Id);
            return resource;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating resource with ID: {ResourceId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a resource.
    /// </summary>
    public async Task DeleteResourceAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Resource ID cannot be null or empty.", nameof(id));

        _logger.LogDebug("Deleting resource with ID: {ResourceId}", id);

        try
        {
            await _apiConnection.DeleteAsync($"{BaseEndpoint}/resources/{id}", cancellationToken);
            _logger.LogInformation("Successfully deleted resource with ID: {ResourceId}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting resource with ID: {ResourceId}", id);
            throw;
        }
    }

    // Event filtering by date range

    /// <summary>
    /// Lists events within a specific date range.
    /// </summary>
    public async Task<IPagedResponse<Event>> ListEventsByDateRangeAsync(DateTime startDate, DateTime endDate, QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Listing events from {StartDate} to {EndDate} with parameters: {@Parameters}", startDate, endDate, parameters);

        var queryParams = parameters ?? new QueryParameters();
        
        // Add date range filters to the query parameters
        queryParams.Where ??= new Dictionary<string, object>();
        queryParams.Where["starts_at"] = $"{startDate:yyyy-MM-dd}..{endDate:yyyy-MM-dd}";

        return await ListEventsAsync(queryParams, cancellationToken);
    }

    // Pagination helpers (following PeopleService pattern)

    /// <summary>
    /// Gets all events matching the specified criteria.
    /// This method automatically handles pagination behind the scenes.
    /// </summary>
    public async Task<IReadOnlyList<Event>> GetAllEventsAsync(
        QueryParameters? parameters = null,
        PaginationOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting all events with parameters: {@Parameters}", parameters);

        var allEvents = new List<Event>();
        var currentParameters = parameters ?? new QueryParameters();
        var pageSize = options?.PageSize ?? 25;
        currentParameters.PerPage = pageSize;

        try
        {
            IPagedResponse<Event> response;
            do
            {
                response = await ListEventsAsync(currentParameters, cancellationToken);
                allEvents.AddRange(response.Data);

                // Update parameters for next page
                if (response.Links?.Next != null)
                {
                    currentParameters.Offset = (currentParameters.Offset ?? 0) + pageSize;
                }
            }
            while (response.Links?.Next != null && !cancellationToken.IsCancellationRequested);

            _logger.LogDebug("Successfully retrieved all {Count} events", allEvents.Count);
            return allEvents.AsReadOnly();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all events");
            throw;
        }
    }

    /// <summary>
    /// Streams events matching the specified criteria for memory-efficient processing.
    /// </summary>
    public async IAsyncEnumerable<Event> StreamEventsAsync(
        QueryParameters? parameters = null,
        PaginationOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Streaming events with parameters: {@Parameters}", parameters);

        var currentParameters = parameters ?? new QueryParameters();
        var pageSize = options?.PageSize ?? 25;
        currentParameters.PerPage = pageSize;

        try
        {
            IPagedResponse<Event> response;
            do
            {
                response = await ListEventsAsync(currentParameters, cancellationToken);

                foreach (var eventItem in response.Data)
                {
                    yield return eventItem;
                }

                // Update parameters for next page
                if (response.Links?.Next != null)
                {
                    currentParameters.Offset = (currentParameters.Offset ?? 0) + pageSize;
                }
            }
            while (response.Links?.Next != null && !cancellationToken.IsCancellationRequested);
        }
        finally
        {
            _logger.LogDebug("Finished streaming events");
        }
    }
}