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
        throw new InvalidOperationException($"No GET stub configured for {endpoint}");
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
        // For first pass simply throw; we don't need pagination yet.
        throw new NotImplementedException();
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
