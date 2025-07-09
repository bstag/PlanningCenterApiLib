namespace PlanningCenter.Api.Client.Models;

/// <summary>
/// Core HTTP communication interface for the Planning Center API.
/// Provides both single-item operations and built-in pagination support.
/// </summary>
public interface IApiConnection
{
    /// <summary>
    /// Performs a GET request to retrieve a single item
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response to</typeparam>
    /// <param name="endpoint">The API endpoint (relative to base URL)</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The deserialized response</returns>
    Task<T> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Performs a POST request to create a new item
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response to</typeparam>
    /// <param name="endpoint">The API endpoint (relative to base URL)</param>
    /// <param name="data">The data to send in the request body</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The deserialized response</returns>
    Task<T> PostAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Performs a PUT request to update an existing item
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response to</typeparam>
    /// <param name="endpoint">The API endpoint (relative to base URL)</param>
    /// <param name="data">The data to send in the request body</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The deserialized response</returns>
    Task<T> PutAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Performs a PATCH request to partially update an existing item
    /// </summary>
    /// <typeparam name="T">The type to deserialize the response to</typeparam>
    /// <param name="endpoint">The API endpoint (relative to base URL)</param>
    /// <param name="data">The data to send in the request body</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The deserialized response</returns>
    Task<T> PatchAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Performs a DELETE request to remove an item
    /// </summary>
    /// <param name="endpoint">The API endpoint (relative to base URL)</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task DeleteAsync(string endpoint, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Performs a GET request that returns paginated results with built-in pagination support.
    /// This is the core method that enables all the pagination helpers throughout the SDK.
    /// </summary>
    /// <typeparam name="T">The type of items in the paginated response</typeparam>
    /// <param name="endpoint">The API endpoint (relative to base URL)</param>
    /// <param name="parameters">Query parameters for filtering, sorting, and pagination</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with built-in navigation helpers</returns>
    Task<IPagedResponse<T>> GetPagedAsync<T>(
        string endpoint, 
        QueryParameters? parameters = null, 
        CancellationToken cancellationToken = default);
}