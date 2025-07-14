using PlanningCenter.Api.Client.Models.Services;
using PlanningCenter.Api.Client.Models.Requests;

namespace PlanningCenter.Api.Client.Models;

/// <summary>
/// Service interface for the Planning Center Services module.
/// Follows Interface Segregation Principle - focused on services-related operations.
/// Provides comprehensive service planning with built-in pagination support.
/// </summary>
public interface IServicesService
{
    // Plan management
    
    /// <summary>
    /// Gets a single plan by ID.
    /// </summary>
    /// <param name="id">The plan's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The plan, or null if not found</returns>
    Task<Plan?> GetPlanAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists plans with optional filtering, sorting, and pagination.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with plans</returns>
    Task<IPagedResponse<Plan>> ListPlansAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new plan.
    /// </summary>
    /// <param name="request">The plan creation request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created plan</returns>
    Task<Plan> CreatePlanAsync(PlanCreateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing plan.
    /// </summary>
    /// <param name="id">The plan's unique identifier</param>
    /// <param name="request">The plan update request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The updated plan</returns>
    Task<Plan> UpdatePlanAsync(string id, PlanUpdateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a plan.
    /// </summary>
    /// <param name="id">The plan's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task DeletePlanAsync(string id, CancellationToken cancellationToken = default);
    
    // Service type management
    
    /// <summary>
    /// Lists service types with optional filtering, sorting, and pagination.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with service types</returns>
    Task<IPagedResponse<ServiceType>> ListServiceTypesAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a single service type by ID.
    /// </summary>
    /// <param name="id">The service type's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The service type, or null if not found</returns>
    Task<ServiceType?> GetServiceTypeAsync(string id, CancellationToken cancellationToken = default);
    
    // Item management
    
    /// <summary>
    /// Lists items for a specific plan.
    /// </summary>
    /// <param name="planId">The plan's unique identifier</param>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with plan items</returns>
    Task<IPagedResponse<Item>> ListPlanItemsAsync(string planId, QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a single item by ID.
    /// </summary>
    /// <param name="planId">The plan's unique identifier</param>
    /// <param name="itemId">The item's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The item, or null if not found</returns>
    Task<Item?> GetPlanItemAsync(string planId, string itemId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new item in a plan.
    /// </summary>
    /// <param name="planId">The plan's unique identifier</param>
    /// <param name="request">The item creation request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created item</returns>
    Task<Item> CreatePlanItemAsync(string planId, ItemCreateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing item.
    /// </summary>
    /// <param name="planId">The plan's unique identifier</param>
    /// <param name="itemId">The item's unique identifier</param>
    /// <param name="request">The item update request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The updated item</returns>
    Task<Item> UpdatePlanItemAsync(string planId, string itemId, ItemUpdateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes an item from a plan.
    /// </summary>
    /// <param name="planId">The plan's unique identifier</param>
    /// <param name="itemId">The item's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task DeletePlanItemAsync(string planId, string itemId, CancellationToken cancellationToken = default);
    
    // Song management
    
    /// <summary>
    /// Lists songs with optional filtering, sorting, and pagination.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with songs</returns>
    Task<IPagedResponse<Song>> ListSongsAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a single song by ID.
    /// </summary>
    /// <param name="id">The song's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The song, or null if not found</returns>
    Task<Song?> GetSongAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new song.
    /// </summary>
    /// <param name="request">The song creation request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created song</returns>
    Task<Song> CreateSongAsync(SongCreateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing song.
    /// </summary>
    /// <param name="id">The song's unique identifier</param>
    /// <param name="request">The song update request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The updated song</returns>
    Task<Song> UpdateSongAsync(string id, SongUpdateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a song.
    /// </summary>
    /// <param name="id">The song's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task DeleteSongAsync(string id, CancellationToken cancellationToken = default);
    
    // Pagination helpers that eliminate manual pagination logic
    
    /// <summary>
    /// Gets all plans matching the specified criteria.
    /// This method automatically handles pagination behind the scenes.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>All plans matching the criteria</returns>
    Task<IReadOnlyList<Plan>> GetAllPlansAsync(
        QueryParameters? parameters = null,
        PaginationOptions? options = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Streams plans matching the specified criteria for memory-efficient processing.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>An async enumerable that yields plans from all pages</returns>
    IAsyncEnumerable<Plan> StreamPlansAsync(
        QueryParameters? parameters = null,
        PaginationOptions? options = null,
        CancellationToken cancellationToken = default);
}