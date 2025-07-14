using Microsoft.Extensions.Logging;
using PlanningCenter.Api.Client.Mapping.CheckIns;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Exceptions;
using PlanningCenter.Api.Client.Models.JsonApi;
using PlanningCenter.Api.Client.Models.JsonApi.CheckIns;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Models.CheckIns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace PlanningCenter.Api.Client.Services;

/// <summary>
/// Service implementation for the Planning Center Check-Ins module.
/// Follows Single Responsibility Principle - handles only Check-Ins API operations.
/// Follows Dependency Inversion Principle - depends on abstractions.
/// </summary>
public class CheckInsService : ICheckInsService
{
    private readonly IApiConnection _apiConnection;
    private readonly ILogger<CheckInsService> _logger;
    private const string BaseEndpoint = "/check_ins/v2";

    public CheckInsService(IApiConnection apiConnection, ILogger<CheckInsService> logger)
    {
        _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // Check-in management

    /// <summary>
    /// Gets a single check-in by ID.
    /// </summary>
    public async Task<CheckIn?> GetCheckInAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Check-in ID cannot be null or empty.", nameof(id));

        _logger.LogDebug("Getting check-in with ID: {CheckInId}", id);

        try
        {
            var response = await _apiConnection.GetAsync<JsonApiSingleResponse<CheckInDto>>(
                $"{BaseEndpoint}/check_ins/{id}", cancellationToken);

            if (response?.Data == null)
            {
                _logger.LogDebug("Check-in with ID {CheckInId} not found", id);
                return null;
            }

            var checkIn = CheckInMapper.MapToDomain(response.Data);
            _logger.LogDebug("Successfully retrieved check-in: {FirstName} {LastName} (ID: {CheckInId})", checkIn.FirstName, checkIn.LastName, checkIn.Id);
            return checkIn;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            _logger.LogDebug("Check-in with ID {CheckInId} not found", id);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting check-in with ID: {CheckInId}", id);
            throw;
        }
    }

    /// <summary>
    /// Lists check-ins with optional filtering, sorting, and pagination.
    /// </summary>
    public async Task<IPagedResponse<CheckIn>> ListCheckInsAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Listing check-ins with parameters: {@Parameters}", parameters);

        try
        {
            var response = await _apiConnection.GetPagedAsync<CheckInDto>(
                $"{BaseEndpoint}/check_ins", parameters, cancellationToken);

            var checkIns = response.Data.Select(CheckInMapper.MapToDomain).ToList();

            _logger.LogDebug("Successfully retrieved {Count} check-ins", checkIns.Count);

            return new PagedResponse<CheckIn>
            {
                Data = checkIns,
                Meta = response.Meta,
                Links = response.Links
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing check-ins");
            throw;
        }
    }

    /// <summary>
    /// Creates a new check-in.
    /// </summary>
    public async Task<CheckIn> CreateCheckInAsync(CheckInCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.FirstName))
            throw new ArgumentException("First name is required.", nameof(request));

        if (string.IsNullOrWhiteSpace(request.LastName))
            throw new ArgumentException("Last name is required.", nameof(request));

        if (string.IsNullOrWhiteSpace(request.EventId))
            throw new ArgumentException("Event ID is required.", nameof(request));

        _logger.LogDebug("Creating check-in: {FirstName} {LastName}", request.FirstName, request.LastName);

        try
        {
            var jsonApiRequest = CheckInMapper.MapCreateRequestToJsonApi(request);

            var response = await _apiConnection.PostAsync<JsonApiSingleResponse<CheckInDto>>(
                $"{BaseEndpoint}/check_ins", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
                throw new PlanningCenterApiGeneralException("Failed to create check-in - no data returned");

            var checkIn = CheckInMapper.MapToDomain(response.Data);
            _logger.LogInformation("Successfully created check-in: {FirstName} {LastName} (ID: {CheckInId})", checkIn.FirstName, checkIn.LastName, checkIn.Id);
            return checkIn;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating check-in: {FirstName} {LastName}", request.FirstName, request.LastName);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing check-in.
    /// </summary>
    public async Task<CheckIn> UpdateCheckInAsync(string id, CheckInUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Check-in ID cannot be null or empty.", nameof(id));

        if (request == null)
            throw new ArgumentNullException(nameof(request));

        _logger.LogDebug("Updating check-in with ID: {CheckInId}", id);

        try
        {
            var jsonApiRequest = CheckInMapper.MapUpdateRequestToJsonApi(request);

            var response = await _apiConnection.PatchAsync<JsonApiSingleResponse<CheckInDto>>(
                $"{BaseEndpoint}/check_ins/{id}", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
                throw new PlanningCenterApiGeneralException("Failed to update check-in - no data returned");

            var checkIn = CheckInMapper.MapToDomain(response.Data);
            _logger.LogInformation("Successfully updated check-in: {FirstName} {LastName} (ID: {CheckInId})", checkIn.FirstName, checkIn.LastName, checkIn.Id);
            return checkIn;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating check-in with ID: {CheckInId}", id);
            throw;
        }
    }

    /// <summary>
    /// Checks out a person.
    /// </summary>
    public async Task<CheckIn> CheckOutAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Check-in ID cannot be null or empty.", nameof(id));

        _logger.LogDebug("Checking out check-in with ID: {CheckInId}", id);

        try
        {
            var response = await _apiConnection.PatchAsync<JsonApiSingleResponse<CheckInDto>>(
                $"{BaseEndpoint}/check_ins/{id}/check_out", new { }, cancellationToken);

            if (response?.Data == null)
                throw new PlanningCenterApiGeneralException("Failed to check out - no data returned");

            var checkIn = CheckInMapper.MapToDomain(response.Data);
            _logger.LogInformation("Successfully checked out: {FirstName} {LastName} (ID: {CheckInId})", checkIn.FirstName, checkIn.LastName, checkIn.Id);
            return checkIn;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking out check-in with ID: {CheckInId}", id);
            throw;
        }
    }

    // Event management

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

            var events = response.Data.Select(CheckInMapper.MapEventToDomain).ToList();

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

            var eventItem = CheckInMapper.MapEventToDomain(response.Data);
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
    /// Lists check-ins for a specific event.
    /// </summary>
    public async Task<IPagedResponse<CheckIn>> ListEventCheckInsAsync(string eventId, QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(eventId))
            throw new ArgumentException("Event ID cannot be null or empty.", nameof(eventId));

        _logger.LogDebug("Listing check-ins for event {EventId} with parameters: {@Parameters}", eventId, parameters);

        try
        {
            var response = await _apiConnection.GetPagedAsync<CheckInDto>(
                $"{BaseEndpoint}/events/{eventId}/check_ins", parameters, cancellationToken);

            var checkIns = response.Data.Select(CheckInMapper.MapToDomain).ToList();

            _logger.LogDebug("Successfully retrieved {Count} check-ins for event {EventId}", checkIns.Count, eventId);

            return new PagedResponse<CheckIn>
            {
                Data = checkIns,
                Meta = response.Meta,
                Links = response.Links
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing check-ins for event {EventId}", eventId);
            throw;
        }
    }

    // Pagination helpers (following PeopleService pattern)

    /// <summary>
    /// Gets all check-ins matching the specified criteria.
    /// This method automatically handles pagination behind the scenes.
    /// </summary>
    public async Task<IReadOnlyList<CheckIn>> GetAllCheckInsAsync(
        QueryParameters? parameters = null,
        PaginationOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting all check-ins with parameters: {@Parameters}", parameters);

        var allCheckIns = new List<CheckIn>();
        var currentParameters = parameters ?? new QueryParameters();
        var pageSize = options?.PageSize ?? 25;
        currentParameters.PerPage = pageSize;

        try
        {
            IPagedResponse<CheckIn> response;
            do
            {
                response = await ListCheckInsAsync(currentParameters, cancellationToken);
                allCheckIns.AddRange(response.Data);

                // Update parameters for next page
                if (response.Links?.Next != null)
                {
                    currentParameters.Offset = (currentParameters.Offset ?? 0) + pageSize;
                }
            }
            while (response.Links?.Next != null && !cancellationToken.IsCancellationRequested);

            _logger.LogDebug("Successfully retrieved all {Count} check-ins", allCheckIns.Count);
            return allCheckIns.AsReadOnly();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all check-ins");
            throw;
        }
    }

    /// <summary>
    /// Streams check-ins matching the specified criteria for memory-efficient processing.
    /// </summary>
    public async IAsyncEnumerable<CheckIn> StreamCheckInsAsync(
        QueryParameters? parameters = null,
        PaginationOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Streaming check-ins with parameters: {@Parameters}", parameters);

        var currentParameters = parameters ?? new QueryParameters();
        var pageSize = options?.PageSize ?? 25;
        currentParameters.PerPage = pageSize;

        try
        {
            IPagedResponse<CheckIn> response;
            do
            {
                response = await ListCheckInsAsync(currentParameters, cancellationToken);

                foreach (var checkIn in response.Data)
                {
                    yield return checkIn;
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
            _logger.LogDebug("Finished streaming check-ins");
        }
    }
}