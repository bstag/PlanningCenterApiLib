using Microsoft.Extensions.Logging;
using PlanningCenter.Api.Client.Mapping.Services;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Exceptions;
using PlanningCenter.Api.Client.Models.JsonApi;
using PlanningCenter.Api.Client.Models.JsonApi.Services;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace PlanningCenter.Api.Client.Services;

/// <summary>
/// Service implementation for the Planning Center Services module.
/// Follows Single Responsibility Principle - handles only Services API operations.
/// Follows Dependency Inversion Principle - depends on abstractions.
/// </summary>
public class ServicesService : ServiceBase, IServicesService
{
    private const string BaseEndpoint = "/services/v2";

    public ServicesService(IApiConnection apiConnection, ILogger<ServicesService> logger)
        : base(logger, apiConnection)
    {
    }

    // Plan management

    /// <summary>
    /// Gets a single plan by ID.
    /// </summary>
    public async Task<Plan?> GetPlanAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Plan ID cannot be null or empty.", nameof(id));

        Logger.LogDebug("Getting plan with ID: {PlanId}", id);

        try
        {
            var response = await ApiConnection.GetAsync<JsonApiSingleResponse<PlanDto>>(
                $"{BaseEndpoint}/plans/{id}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogDebug("Plan with ID {PlanId} not found", id);
                return null;
            }

            var plan = PlanMapper.MapToDomain(response.Data);
            Logger.LogDebug("Successfully retrieved plan: {PlanTitle} (ID: {PlanId})", plan.Title, plan.Id);
            return plan;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            Logger.LogDebug("Plan with ID {PlanId} not found", id);
            return null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting plan with ID: {PlanId}", id);
            throw;
        }
    }

    /// <summary>
    /// Lists plans with optional filtering, sorting, and pagination.
    /// </summary>
    public async Task<IPagedResponse<Plan>> ListPlansAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Listing plans with parameters: {@Parameters}", parameters);

        try
        {
            var response = await ApiConnection.GetPagedAsync<PlanDto>(
                $"{BaseEndpoint}/plans", parameters, cancellationToken);

            var plans = response.Data.Select(PlanMapper.MapToDomain).ToList();

            Logger.LogDebug("Successfully retrieved {Count} plans", plans.Count);

            return new PagedResponse<Plan>
            {
                Data = plans,
                Meta = response.Meta,
                Links = response.Links
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error listing plans");
            throw;
        }
    }

    /// <summary>
    /// Creates a new plan.
    /// </summary>
    public async Task<Plan> CreatePlanAsync(PlanCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("Plan title is required.", nameof(request));

        if (string.IsNullOrWhiteSpace(request.ServiceTypeId))
            throw new ArgumentException("Service type ID is required.", nameof(request));

        Logger.LogDebug("Creating plan: {PlanTitle}", request.Title);

        try
        {
            var jsonApiRequest = PlanMapper.MapCreateRequestToJsonApi(request);

            var response = await ApiConnection.PostAsync<JsonApiSingleResponse<PlanDto>>(
                $"{BaseEndpoint}/plans", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
                throw new PlanningCenterApiGeneralException("Failed to create plan - no data returned");

            var plan = PlanMapper.MapToDomain(response.Data);
            Logger.LogInformation("Successfully created plan: {PlanTitle} (ID: {PlanId})", plan.Title, plan.Id);
            return plan;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating plan: {PlanTitle}", request.Title);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing plan.
    /// </summary>
    public async Task<Plan> UpdatePlanAsync(string id, PlanUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Plan ID cannot be null or empty.", nameof(id));

        if (request == null)
            throw new ArgumentNullException(nameof(request));

        Logger.LogDebug("Updating plan with ID: {PlanId}", id);

        try
        {
            var jsonApiRequest = PlanMapper.MapUpdateRequestToJsonApi(request);

            var response = await ApiConnection.PatchAsync<JsonApiSingleResponse<PlanDto>>(
                $"{BaseEndpoint}/plans/{id}", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
                throw new PlanningCenterApiGeneralException("Failed to update plan - no data returned");

            var plan = PlanMapper.MapToDomain(response.Data);
            Logger.LogInformation("Successfully updated plan: {PlanTitle} (ID: {PlanId})", plan.Title, plan.Id);
            return plan;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating plan with ID: {PlanId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a plan.
    /// </summary>
    public async Task DeletePlanAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Plan ID cannot be null or empty.", nameof(id));

        Logger.LogDebug("Deleting plan with ID: {PlanId}", id);

        try
        {
            await ApiConnection.DeleteAsync($"{BaseEndpoint}/plans/{id}", cancellationToken);
            Logger.LogInformation("Successfully deleted plan with ID: {PlanId}", id);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting plan with ID: {PlanId}", id);
            throw;
        }
    }

    // Service type management

    /// <summary>
    /// Lists service types with optional filtering, sorting, and pagination.
    /// </summary>
    public async Task<IPagedResponse<ServiceType>> ListServiceTypesAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Listing service types with parameters: {@Parameters}", parameters);

        try
        {
            var response = await ApiConnection.GetPagedAsync<ServiceTypeDto>(
                $"{BaseEndpoint}/service_types", parameters, cancellationToken);

            var serviceTypes = response.Data.Select(PlanMapper.MapServiceTypeToDomain).ToList();

            Logger.LogDebug("Successfully retrieved {Count} service types", serviceTypes.Count);

            return new PagedResponse<ServiceType>
            {
                Data = serviceTypes,
                Meta = response.Meta,
                Links = response.Links
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error listing service types");
            throw;
        }
    }

    /// <summary>
    /// Gets a single service type by ID.
    /// </summary>
    public async Task<ServiceType?> GetServiceTypeAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Service type ID cannot be null or empty.", nameof(id));

        Logger.LogDebug("Getting service type with ID: {ServiceTypeId}", id);

        try
        {
            var response = await ApiConnection.GetAsync<JsonApiSingleResponse<ServiceTypeDto>>(
                $"{BaseEndpoint}/service_types/{id}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogDebug("Service type with ID {ServiceTypeId} not found", id);
                return null;
            }

            var serviceType = PlanMapper.MapServiceTypeToDomain(response.Data);
            Logger.LogDebug("Successfully retrieved service type: {ServiceTypeName} (ID: {ServiceTypeId})", serviceType.Name, serviceType.Id);
            return serviceType;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            Logger.LogDebug("Service type with ID {ServiceTypeId} not found", id);
            return null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting service type with ID: {ServiceTypeId}", id);
            throw;
        }
    }

    // Item management

    /// <summary>
    /// Lists items for a specific plan.
    /// </summary>
    public async Task<IPagedResponse<Item>> ListPlanItemsAsync(string planId, QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(planId))
            throw new ArgumentException("Plan ID cannot be null or empty.", nameof(planId));

        Logger.LogDebug("Listing items for plan {PlanId} with parameters: {@Parameters}", planId, parameters);

        try
        {
            var response = await ApiConnection.GetPagedAsync<ItemDto>(
                $"{BaseEndpoint}/plans/{planId}/items", parameters, cancellationToken);

            var items = response.Data.Select(PlanMapper.MapItemToDomain).ToList();

            Logger.LogDebug("Successfully retrieved {Count} items for plan {PlanId}", items.Count, planId);

            return new PagedResponse<Item>
            {
                Data = items,
                Meta = response.Meta,
                Links = response.Links
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error listing items for plan {PlanId}", planId);
            throw;
        }
    }

    /// <summary>
    /// Gets a single item by ID.
    /// </summary>
    public async Task<Item?> GetPlanItemAsync(string planId, string itemId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(planId))
            throw new ArgumentException("Plan ID cannot be null or empty.", nameof(planId));

        if (string.IsNullOrWhiteSpace(itemId))
            throw new ArgumentException("Item ID cannot be null or empty.", nameof(itemId));

        Logger.LogDebug("Getting item {ItemId} for plan {PlanId}", itemId, planId);

        try
        {
            var response = await ApiConnection.GetAsync<JsonApiSingleResponse<ItemDto>>(
                $"{BaseEndpoint}/plans/{planId}/items/{itemId}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogDebug("Item {ItemId} for plan {PlanId} not found", itemId, planId);
                return null;
            }

            var item = PlanMapper.MapItemToDomain(response.Data);
            Logger.LogDebug("Successfully retrieved item: {ItemTitle} (ID: {ItemId})", item.Title, item.Id);
            return item;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            Logger.LogDebug("Item {ItemId} for plan {PlanId} not found", itemId, planId);
            return null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting item {ItemId} for plan {PlanId}", itemId, planId);
            throw;
        }
    }

    // Song management

    /// <summary>
    /// Lists songs with optional filtering, sorting, and pagination.
    /// </summary>
    public async Task<IPagedResponse<Song>> ListSongsAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Listing songs with parameters: {@Parameters}", parameters);

        try
        {
            var response = await ApiConnection.GetPagedAsync<SongDto>(
                $"{BaseEndpoint}/songs", parameters, cancellationToken);

            var songs = response.Data.Select(PlanMapper.MapSongToDomain).ToList();

            Logger.LogDebug("Successfully retrieved {Count} songs", songs.Count);

            return new PagedResponse<Song>
            {
                Data = songs,
                Meta = response.Meta,
                Links = response.Links
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error listing songs");
            throw;
        }
    }

    /// <summary>
    /// Gets a single song by ID.
    /// </summary>
    public async Task<Song?> GetSongAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Song ID cannot be null or empty.", nameof(id));

        Logger.LogDebug("Getting song with ID: {SongId}", id);

        try
        {
            var response = await ApiConnection.GetAsync<JsonApiSingleResponse<SongDto>>(
                $"{BaseEndpoint}/songs/{id}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogDebug("Song with ID {SongId} not found", id);
                return null;
            }

            var song = PlanMapper.MapSongToDomain(response.Data);
            Logger.LogDebug("Successfully retrieved song: {SongTitle} (ID: {SongId})", song.Title, song.Id);
            return song;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            Logger.LogDebug("Song with ID {SongId} not found", id);
            return null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting song with ID: {SongId}", id);
            throw;
        }
    }

    // Pagination helpers (following PeopleService pattern)

    /// <summary>
    /// Gets all plans matching the specified criteria.
    /// This method automatically handles pagination behind the scenes.
    /// </summary>
    public async Task<IReadOnlyList<Plan>> GetAllPlansAsync(
        QueryParameters? parameters = null,
        PaginationOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Getting all plans with parameters: {@Parameters}", parameters);

        var allPlans = new List<Plan>();
        var currentParameters = parameters ?? new QueryParameters();
        var pageSize = options?.PageSize ?? 25;
        currentParameters.PerPage = pageSize;

        try
        {
            IPagedResponse<Plan> response;
            do
            {
                response = await ListPlansAsync(currentParameters, cancellationToken);
                allPlans.AddRange(response.Data);

                // Update parameters for next page
                if (response.Links?.Next != null)
                {
                    currentParameters.Offset = (currentParameters.Offset ?? 0) + pageSize;
                }
            }
            while (response.Links?.Next != null && !cancellationToken.IsCancellationRequested);

            Logger.LogDebug("Successfully retrieved all {Count} plans", allPlans.Count);
            return allPlans.AsReadOnly();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting all plans");
            throw;
        }
    }

    /// <summary>
    /// Streams plans matching the specified criteria for memory-efficient processing.
    /// </summary>
    public async IAsyncEnumerable<Plan> StreamPlansAsync(
        QueryParameters? parameters = null,
        PaginationOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Streaming plans with parameters: {@Parameters}", parameters);

        var currentParameters = parameters ?? new QueryParameters();
        var pageSize = options?.PageSize ?? 25;
        currentParameters.PerPage = pageSize;

        try
        {
            IPagedResponse<Plan> response;
            do
            {
                response = await ListPlansAsync(currentParameters, cancellationToken);

                foreach (var plan in response.Data)
                {
                    yield return plan;
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
            Logger.LogDebug("Finished streaming plans");
        }
    }

    /// <summary>
    /// Creates a new item in a plan.
    /// </summary>
    public async Task<Item> CreatePlanItemAsync(string planId, ItemCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(planId))
            throw new ArgumentException("Plan ID cannot be null or empty.", nameof(planId));

        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("Item title is required.", nameof(request));

        Logger.LogDebug("Creating item: {ItemTitle} for plan {PlanId}", request.Title, planId);

        try
        {
            var jsonApiRequest = PlanMapper.MapItemCreateRequestToJsonApi(request);

            var response = await ApiConnection.PostAsync<JsonApiSingleResponse<ItemDto>>(
                $"{BaseEndpoint}/plans/{planId}/items", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
                throw new PlanningCenterApiGeneralException("Failed to create item - no data returned");

            var item = PlanMapper.MapItemToDomain(response.Data);
            Logger.LogInformation("Successfully created item: {ItemTitle} (ID: {ItemId})", item.Title, item.Id);
            return item;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating item: {ItemTitle} for plan {PlanId}", request.Title, planId);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing item.
    /// </summary>
    public async Task<Item> UpdatePlanItemAsync(string planId, string itemId, ItemUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(planId))
            throw new ArgumentException("Plan ID cannot be null or empty.", nameof(planId));

        if (string.IsNullOrWhiteSpace(itemId))
            throw new ArgumentException("Item ID cannot be null or empty.", nameof(itemId));

        if (request == null)
            throw new ArgumentNullException(nameof(request));

        Logger.LogDebug("Updating item {ItemId} for plan {PlanId}", itemId, planId);

        try
        {
            var jsonApiRequest = PlanMapper.MapItemUpdateRequestToJsonApi(request);

            var response = await ApiConnection.PatchAsync<JsonApiSingleResponse<ItemDto>>(
                $"{BaseEndpoint}/plans/{planId}/items/{itemId}", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
                throw new PlanningCenterApiGeneralException("Failed to update item - no data returned");

            var item = PlanMapper.MapItemToDomain(response.Data);
            Logger.LogInformation("Successfully updated item: {ItemTitle} (ID: {ItemId})", item.Title, item.Id);
            return item;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating item {ItemId} for plan {PlanId}", itemId, planId);
            throw;
        }
    }

    /// <summary>
    /// Deletes an item from a plan.
    /// </summary>
    public async Task DeletePlanItemAsync(string planId, string itemId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(planId))
            throw new ArgumentException("Plan ID cannot be null or empty.", nameof(planId));

        if (string.IsNullOrWhiteSpace(itemId))
            throw new ArgumentException("Item ID cannot be null or empty.", nameof(itemId));

        Logger.LogDebug("Deleting item {ItemId} from plan {PlanId}", itemId, planId);

        try
        {
            await ApiConnection.DeleteAsync($"{BaseEndpoint}/plans/{planId}/items/{itemId}", cancellationToken);
            Logger.LogInformation("Successfully deleted item {ItemId} from plan {PlanId}", itemId, planId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting item {ItemId} from plan {PlanId}", itemId, planId);
            throw;
        }
    }

    /// <summary>
    /// Creates a new song.
    /// </summary>
    public async Task<Song> CreateSongAsync(SongCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("Song title is required.", nameof(request));

        Logger.LogDebug("Creating song: {SongTitle}", request.Title);

        try
        {
            var jsonApiRequest = PlanMapper.MapSongCreateRequestToJsonApi(request);

            var response = await ApiConnection.PostAsync<JsonApiSingleResponse<SongDto>>(
                $"{BaseEndpoint}/songs", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
                throw new PlanningCenterApiGeneralException("Failed to create song - no data returned");

            var song = PlanMapper.MapSongToDomain(response.Data);
            Logger.LogInformation("Successfully created song: {SongTitle} (ID: {SongId})", song.Title, song.Id);
            return song;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating song: {SongTitle}", request.Title);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing song.
    /// </summary>
    public async Task<Song> UpdateSongAsync(string id, SongUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Song ID cannot be null or empty.", nameof(id));

        if (request == null)
            throw new ArgumentNullException(nameof(request));

        Logger.LogDebug("Updating song with ID: {SongId}", id);

        try
        {
            var jsonApiRequest = PlanMapper.MapSongUpdateRequestToJsonApi(request);

            var response = await ApiConnection.PatchAsync<JsonApiSingleResponse<SongDto>>(
                $"{BaseEndpoint}/songs/{id}", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
                throw new PlanningCenterApiGeneralException("Failed to update song - no data returned");

            var song = PlanMapper.MapSongToDomain(response.Data);
            Logger.LogInformation("Successfully updated song: {SongTitle} (ID: {SongId})", song.Title, song.Id);
            return song;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating song with ID: {SongId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a song.
    /// </summary>
    public async Task DeleteSongAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Song ID cannot be null or empty.", nameof(id));

        Logger.LogDebug("Deleting song with ID: {SongId}", id);

        try
        {
            await ApiConnection.DeleteAsync($"{BaseEndpoint}/songs/{id}", cancellationToken);
            Logger.LogInformation("Successfully deleted song with ID: {SongId}", id);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting song with ID: {SongId}", id);
            throw;
        }
    }
}