using System.Collections.Concurrent;
using Microsoft.Extensions.Primitives;
using PlanningCenter.Api.Client.Models;

namespace PlanningCenter.Api.Client.Tests.Utilities;

/// <summary>
/// Very small in-memory stub for <see cref="IApiConnection"/> used by unit tests.
/// Only the functionality required by the initial tests is implemented; extend as needed.
/// </summary>
public class MockApiConnection : IApiConnection
{
    private readonly ConcurrentDictionary<string, object?> _getResponses = new();
    private readonly ConcurrentDictionary<(string Verb, string Endpoint), object?> _mutationResponses = new();

    #region Setup helpers

    public void SetupGetResponse<T>(string endpoint, T response) =>
        _getResponses[endpoint] = response;

    public void SetupMutationResponse<T>(string verb, string endpoint, T response) =>
        _mutationResponses[(verb, endpoint)] = response;

    #endregion

    public Task<T> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        if (_getResponses.TryGetValue(endpoint, out var obj) && obj is T typed)
            return Task.FromResult(typed);
        
        // Try base endpoint without query parameters
        var baseEndpoint = endpoint.Split('?')[0];
        if (_getResponses.TryGetValue(baseEndpoint, out var baseObj) && baseObj is T baseTyped)
            return Task.FromResult(baseTyped);
        
        // Handle PagedResponse type conversions (including dynamic)
        if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(PagedResponse<>))
        {
            // Try exact match for any PagedResponse type and cast
            if (_getResponses.TryGetValue(endpoint, out var pagedObj) && IsPagedResponse(pagedObj))
            {
                try 
                {
                    return Task.FromResult((T)pagedObj!);
                }
                catch (InvalidCastException)
                {
                    // If direct cast fails, handle it below
                }
            }
                
            // Try base endpoint for any PagedResponse type and cast
            if (_getResponses.TryGetValue(baseEndpoint, out var basePaged) && IsPagedResponse(basePaged))
            {
                try 
                {
                    return Task.FromResult((T)basePaged!);
                }
                catch (InvalidCastException)
                {
                    // If direct cast fails, handle it below
                }
            }
        }
            
        throw new InvalidOperationException($"No GET stub configured for {endpoint}");
    }
    
    private static bool IsPagedResponse(object? obj)
    {
        return obj?.GetType().IsGenericType == true && 
               obj.GetType().GetGenericTypeDefinition() == typeof(PagedResponse<>);
    }

    public Task<T> PostAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default) =>
        ReturnMutation<T>("POST", endpoint);

    public Task<T> PutAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default) =>
        ReturnMutation<T>("PUT", endpoint);

    public Task<T> PatchAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default) =>
        ReturnMutation<T>("PATCH", endpoint);

    public Task DeleteAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        // no-op; assume success
        return Task.CompletedTask;
    }

    public Task<IPagedResponse<T>> GetPagedAsync<T>(string endpoint, QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        // Try exact match first
        if (_getResponses.TryGetValue(endpoint, out var obj) && obj is IPagedResponse<T> pagedResponse)
            return Task.FromResult(pagedResponse);
        
        // Try base endpoint without query parameters
        var baseEndpoint = endpoint.Split('?')[0];
        if (_getResponses.TryGetValue(baseEndpoint, out var baseObj) && baseObj is IPagedResponse<T> basePagedResponse)
            return Task.FromResult(basePagedResponse);
        
        // If no paged response is configured, try to create one from a regular response
        if (_getResponses.TryGetValue(endpoint, out var regularObj) && regularObj is IEnumerable<T> items)
        {
            var itemsList = items.ToList();
            var pagedResult = new PagedResponse<T>
            {
                Data = itemsList,
                Meta = new PagedResponseMeta 
                { 
                    TotalCount = itemsList.Count, 
                    Count = itemsList.Count,
                    PerPage = parameters?.PerPage ?? 25,
                    Offset = parameters?.Offset ?? 0
                },
                Links = new PagedResponseLinks()
            };
            return Task.FromResult<IPagedResponse<T>>(pagedResult);
        }
        
        // Try base endpoint for regular response
        if (_getResponses.TryGetValue(baseEndpoint, out var baseRegularObj) && baseRegularObj is IEnumerable<T> baseItems)
        {
            var itemsList = baseItems.ToList();
            var pagedResult = new PagedResponse<T>
            {
                Data = itemsList,
                Meta = new PagedResponseMeta 
                { 
                    TotalCount = itemsList.Count, 
                    Count = itemsList.Count,
                    PerPage = parameters?.PerPage ?? 25,
                    Offset = parameters?.Offset ?? 0
                },
                Links = new PagedResponseLinks()
            };
            return Task.FromResult<IPagedResponse<T>>(pagedResult);
        }
        
        throw new InvalidOperationException($"No paged GET stub configured for {endpoint}");
    }

    private Task<T> ReturnMutation<T>(string verb, string endpoint)
    {
        if (_mutationResponses.TryGetValue((verb, endpoint), out var obj) && obj is T typed)
            return Task.FromResult(typed);
        throw new InvalidOperationException($"No {verb} stub configured for {endpoint}");
    }

    // Expose some inspection helpers if needed later
    public IReadOnlyDictionary<string, object?> GetStubs => _getResponses;
}
