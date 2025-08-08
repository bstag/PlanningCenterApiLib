using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;

namespace PlanningCenter.Api.Client.Interfaces;

/// <summary>
/// Base interface for traditional API services providing standard CRUD operations.
/// </summary>
/// <typeparam name="T">The domain model type</typeparam>
/// <typeparam name="TCreateRequest">The create request type</typeparam>
/// <typeparam name="TUpdateRequest">The update request type</typeparam>
public interface ITraditionalApiService<T, TCreateRequest, TUpdateRequest>
    where T : class
    where TCreateRequest : class
    where TUpdateRequest : class
{
    /// <summary>
    /// Gets all resources with optional query parameters.
    /// </summary>
    /// <param name="parameters">Optional query parameters for filtering, sorting, and pagination</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A paged response containing the resources</returns>
    Task<IPagedResponse<T>> GetAllAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a resource by its ID.
    /// </summary>
    /// <param name="id">The resource ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The resource or null if not found</returns>
    Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new resource.
    /// </summary>
    /// <param name="request">The create request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created resource</returns>
    Task<T> CreateAsync(TCreateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing resource.
    /// </summary>
    /// <param name="id">The resource ID</param>
    /// <param name="request">The update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated resource</returns>
    Task<T> UpdateAsync(string id, TUpdateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a resource by its ID.
    /// </summary>
    /// <param name="id">The resource ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for traditional API services with read-only operations.
/// </summary>
/// <typeparam name="T">The domain model type</typeparam>
public interface IReadOnlyTraditionalApiService<T> where T : class
{
    /// <summary>
    /// Gets all resources with optional query parameters.
    /// </summary>
    /// <param name="parameters">Optional query parameters for filtering, sorting, and pagination</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A paged response containing the resources</returns>
    Task<IPagedResponse<T>> GetAllAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a resource by its ID.
    /// </summary>
    /// <param name="id">The resource ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The resource or null if not found</returns>
    Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for traditional API services with extended query capabilities.
/// </summary>
/// <typeparam name="T">The domain model type</typeparam>
/// <typeparam name="TCreateRequest">The create request type</typeparam>
/// <typeparam name="TUpdateRequest">The update request type</typeparam>
public interface IExtendedTraditionalApiService<T, TCreateRequest, TUpdateRequest> : ITraditionalApiService<T, TCreateRequest, TUpdateRequest>
    where T : class
    where TCreateRequest : class
    where TUpdateRequest : class
{
    /// <summary>
    /// Gets resources with specific filter conditions.
    /// </summary>
    /// <param name="filters">Dictionary of field-value pairs to filter by</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A paged response containing the filtered resources</returns>
    Task<IPagedResponse<T>> GetByFiltersAsync(Dictionary<string, object> filters, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets resources with included relationships.
    /// </summary>
    /// <param name="includes">The relationships to include</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A paged response containing the resources with included relationships</returns>
    Task<IPagedResponse<T>> GetWithIncludesAsync(string[] includes, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets resources ordered by the specified field.
    /// </summary>
    /// <param name="orderBy">The field to order by (prefix with '-' for descending)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A paged response containing the ordered resources</returns>
    Task<IPagedResponse<T>> GetOrderedAsync(string orderBy, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a specific page of resources.
    /// </summary>
    /// <param name="page">The page number (1-based)</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A paged response containing the requested page of resources</returns>
    Task<IPagedResponse<T>> GetPageAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if any resources exist matching the specified filters.
    /// </summary>
    /// <param name="filters">Dictionary of field-value pairs to filter by</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if any matching resources exist, false otherwise</returns>
    Task<bool> ExistsAsync(Dictionary<string, object> filters, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the count of resources matching the specified filters.
    /// </summary>
    /// <param name="filters">Dictionary of field-value pairs to filter by</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The count of matching resources</returns>
    Task<int> CountAsync(Dictionary<string, object> filters, CancellationToken cancellationToken = default);
}