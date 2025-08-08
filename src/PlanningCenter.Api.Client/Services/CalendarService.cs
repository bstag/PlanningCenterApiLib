using Microsoft.Extensions.Logging;
using PlanningCenter.Api.Client.Mapping.Calendar;
using PlanningCenter.Api.Client.Abstractions;
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
public class CalendarService : ServiceBase, ICalendarService
{
    private const string BaseEndpoint = "/calendar/v2";

    public CalendarService(IApiConnection apiConnection, ILogger<CalendarService> logger)
        : base(logger, apiConnection)
    {
    }

    // Event management

    /// <summary>
    /// Gets a single event by ID.
    /// </summary>
    public async Task<Event?> GetEventAsync(string id, CancellationToken cancellationToken = default)
    {
        return await GetResourceByIdAsync<EventDto, Event>(
            $"{BaseEndpoint}/events",
            id,
            CalendarMapper.MapToDomain,
            "GetEvent",
            cancellationToken);
    }

    /// <summary>
    /// Lists events with optional filtering, sorting, and pagination.
    /// </summary>
    public async Task<IPagedResponse<Event>> ListEventsAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(
            async () =>
            {
                var response = await ApiConnection.GetPagedAsync<EventDto>(
                    $"{BaseEndpoint}/events", parameters, cancellationToken);

                var events = response.Data.Select(CalendarMapper.MapToDomain).ToList();

                return new PagedResponse<Event>
                {
                    Data = events,
                    Meta = response.Meta,
                    Links = response.Links
                };
            },
            "ListEvents",
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Creates a new event.
    /// </summary>
    public async Task<Event> CreateEventAsync(EventCreateRequest request, CancellationToken cancellationToken = default)
    {
        ValidateNotNull(request, nameof(request));
        ValidateNotNullOrEmpty(request.Name, nameof(request.Name));

        return await ExecuteAsync(
            async () =>
            {
                var jsonApiRequest = CalendarMapper.MapCreateRequestToJsonApi(request);

                var response = await ApiConnection.PostAsync<JsonApiSingleResponse<EventDto>>(
                    $"{BaseEndpoint}/events", jsonApiRequest, cancellationToken);

                if (response?.Data == null)
                    throw new PlanningCenterApiGeneralException("Failed to create event - no data returned");

                return CalendarMapper.MapToDomain(response.Data);
            },
            "CreateEvent",
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Updates an existing event.
    /// </summary>
    public async Task<Event> UpdateEventAsync(string id, EventUpdateRequest request, CancellationToken cancellationToken = default)
    {
        ValidateNotNullOrEmpty(id, nameof(id));
        ValidateNotNull(request, nameof(request));

        return await ExecuteAsync(
            async () =>
            {
                var jsonApiRequest = CalendarMapper.MapUpdateRequestToJsonApi(request);

                var response = await ApiConnection.PatchAsync<JsonApiSingleResponse<EventDto>>(
                    $"{BaseEndpoint}/events/{id}", jsonApiRequest, cancellationToken);

                if (response?.Data == null)
                    throw new PlanningCenterApiGeneralException("Failed to update event - no data returned");

                return CalendarMapper.MapToDomain(response.Data);
            },
            "UpdateEvent",
            id,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Deletes an event.
    /// </summary>
    public async Task DeleteEventAsync(string id, CancellationToken cancellationToken = default)
    {
        ValidateNotNullOrEmpty(id, nameof(id));

        await ExecuteAsync(
            async () =>
            {
                await ApiConnection.DeleteAsync($"{BaseEndpoint}/events/{id}", cancellationToken);
                return true; // ExecuteAsync requires a return value
            },
            "DeleteEvent",
            id,
            cancellationToken: cancellationToken);
    }

    // Resource management

    /// <summary>
    /// Lists resources with optional filtering, sorting, and pagination.
    /// </summary>
    public async Task<IPagedResponse<Resource>> ListResourcesAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(
            async () =>
            {
                var response = await ApiConnection.GetPagedAsync<ResourceDto>(
                    $"{BaseEndpoint}/resources", parameters, cancellationToken);

                var resources = response.Data.Select(CalendarMapper.MapResourceToDomain).ToList();

                return new PagedResponse<Resource>
                {
                    Data = resources,
                    Meta = response.Meta,
                    Links = response.Links
                };
            },
            "ListResources",
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Gets a single resource by ID.
    /// </summary>
    public async Task<Resource?> GetResourceAsync(string id, CancellationToken cancellationToken = default)
    {
        return await GetResourceByIdAsync<ResourceDto, Resource>(
            $"{BaseEndpoint}/resources",
            id,
            CalendarMapper.MapResourceToDomain,
            "GetResource",
            cancellationToken);
    }

    /// <summary>
    /// Creates a new resource.
    /// </summary>
    public async Task<Resource> CreateResourceAsync(ResourceCreateRequest request, CancellationToken cancellationToken = default)
    {
        ValidateNotNull(request, nameof(request));
        ValidateNotNullOrEmpty(request.Name, nameof(request.Name));

        return await ExecuteAsync(
            async () =>
            {
                var jsonApiRequest = CalendarMapper.MapResourceCreateRequestToJsonApi(request);

                var response = await ApiConnection.PostAsync<JsonApiSingleResponse<ResourceDto>>(
                    $"{BaseEndpoint}/resources", jsonApiRequest, cancellationToken);

                if (response?.Data == null)
                    throw new PlanningCenterApiGeneralException("Failed to create resource - no data returned");

                return CalendarMapper.MapResourceToDomain(response.Data);
            },
            "CreateResource",
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Updates an existing resource.
    /// </summary>
    public async Task<Resource> UpdateResourceAsync(string id, ResourceUpdateRequest request, CancellationToken cancellationToken = default)
    {
        ValidateNotNullOrEmpty(id, nameof(id));
        ValidateNotNull(request, nameof(request));

        return await ExecuteAsync(
            async () =>
            {
                var jsonApiRequest = CalendarMapper.MapResourceUpdateRequestToJsonApi(request);

                var response = await ApiConnection.PatchAsync<JsonApiSingleResponse<ResourceDto>>(
                    $"{BaseEndpoint}/resources/{id}", jsonApiRequest, cancellationToken);

                if (response?.Data == null)
                    throw new PlanningCenterApiGeneralException("Failed to update resource - no data returned");

                return CalendarMapper.MapResourceToDomain(response.Data);
            },
            "UpdateResource",
            id,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Deletes a resource.
    /// </summary>
    public async Task DeleteResourceAsync(string id, CancellationToken cancellationToken = default)
    {
        ValidateNotNullOrEmpty(id, nameof(id));

        await ExecuteAsync(
            async () =>
            {
                await ApiConnection.DeleteAsync($"{BaseEndpoint}/resources/{id}", cancellationToken);
                return true; // ExecuteAsync requires a return value
            },
            "DeleteResource",
            id,
            cancellationToken: cancellationToken);
    }

    // Event filtering by date range

    /// <summary>
    /// Lists events within a specific date range.
    /// </summary>
    public async Task<IPagedResponse<Event>> ListEventsByDateRangeAsync(DateTime startDate, DateTime endDate, QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
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
        return await ExecuteAsync(
            async () =>
            {
                var allEvents = new List<Event>();
                var currentParameters = parameters ?? new QueryParameters();
                var pageSize = options?.PageSize ?? 25;
                currentParameters.PerPage = pageSize;

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

                return (IReadOnlyList<Event>)allEvents.AsReadOnly();
            },
            "GetAllEvents",
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Streams events matching the specified criteria for memory-efficient processing.
    /// </summary>
    public async IAsyncEnumerable<Event> StreamEventsAsync(
        QueryParameters? parameters = null,
        PaginationOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Streaming events with parameters: {@Parameters}", parameters);

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
            Logger.LogDebug("Finished streaming events");
        }
    }
}