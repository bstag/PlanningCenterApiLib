using Moq;
using PlanningCenter.Api.Client.Models;
using System.Text.Json;

namespace PlanningCenter.Api.Client.Tests.Utilities;

/// <summary>
/// Mock API connection for unit testing services without making real HTTP calls.
/// </summary>
public class MockApiConnection
{
    private readonly Mock<IApiConnection> _mock;
    private readonly Dictionary<string, object> _responses = new();
    private readonly List<(string Method, string Endpoint, object? Data)> _requests = new();

    public MockApiConnection()
    {
        _mock = new Mock<IApiConnection>();
        SetupMockBehavior();
    }

    /// <summary>
    /// Gets the mock IApiConnection instance.
    /// </summary>
    public IApiConnection Object => _mock.Object;

    /// <summary>
    /// Gets all requests that were made to the mock.
    /// </summary>
    public IReadOnlyList<(string Method, string Endpoint, object? Data)> Requests => _requests;

    /// <summary>
    /// Sets up a response for a GET request to a specific endpoint.
    /// </summary>
    public MockApiConnection SetupGetResponse<T>(string endpoint, T response)
    {
        _responses[$"GET:{endpoint}"] = response!;
        return this;
    }

    /// <summary>
    /// Sets up a response for a POST request to a specific endpoint.
    /// </summary>
    public MockApiConnection SetupPostResponse<T>(string endpoint, T response)
    {
        _responses[$"POST:{endpoint}"] = response!;
        return this;
    }

    /// <summary>
    /// Sets up a response for a PATCH request to a specific endpoint.
    /// </summary>
    public MockApiConnection SetupPatchResponse<T>(string endpoint, T response)
    {
        _responses[$"PATCH:{endpoint}"] = response!;
        return this;
    }

    /// <summary>
    /// Sets up a DELETE request to succeed for a specific endpoint.
    /// </summary>
    public MockApiConnection SetupDeleteSuccess(string endpoint)
    {
        _responses[$"DELETE:{endpoint}"] = true;
        return this;
    }

    /// <summary>
    /// Sets up a paged response for a GET request.
    /// </summary>
    public MockApiConnection SetupPagedResponse<T>(string endpoint, IPagedResponse<T> response)
    {
        _responses[$"PAGED:{endpoint}"] = response;
        return this;
    }

    /// <summary>
    /// Sets up an exception to be thrown for a specific endpoint.
    /// </summary>
    public MockApiConnection SetupException(string method, string endpoint, Exception exception)
    {
        _responses[$"{method}:{endpoint}:EXCEPTION"] = exception;
        return this;
    }

    /// <summary>
    /// Verifies that a specific GET request was made.
    /// </summary>
    public void VerifyGetRequest(string endpoint, Times? times = null)
    {
        var actualTimes = times ?? Times.Once();
        var matchingRequests = _requests.Count(r => r.Method == "GET" && r.Endpoint == endpoint);
        
        if (actualTimes.From > matchingRequests || matchingRequests > actualTimes.To)
        {
            throw new MockException($"Expected GET request to {endpoint} {actualTimes}, but was called {matchingRequests} times");
        }
    }

    /// <summary>
    /// Verifies that a specific POST request was made.
    /// </summary>
    public void VerifyPostRequest(string endpoint, Times? times = null)
    {
        var actualTimes = times ?? Times.Once();
        var matchingRequests = _requests.Count(r => r.Method == "POST" && r.Endpoint == endpoint);
        
        if (actualTimes.From > matchingRequests || matchingRequests > actualTimes.To)
        {
            throw new MockException($"Expected POST request to {endpoint} {actualTimes}, but was called {matchingRequests} times");
        }
    }

    /// <summary>
    /// Verifies that no requests were made.
    /// </summary>
    public void VerifyNoRequests()
    {
        if (_requests.Any())
        {
            var requestList = string.Join(", ", _requests.Select(r => $"{r.Method} {r.Endpoint}"));
            throw new MockException($"Expected no requests, but found: {requestList}");
        }
    }

    /// <summary>
    /// Clears all recorded requests and responses.
    /// </summary>
    public void Reset()
    {
        _requests.Clear();
        _responses.Clear();
        _mock.Reset();
        SetupMockBehavior();
    }

    private void SetupMockBehavior()
    {
        // Setup GET requests
        _mock.Setup(x => x.GetAsync<It.IsAnyType>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns<string, CancellationToken>((endpoint, ct) =>
            {
                _requests.Add(("GET", endpoint, null));
                return GetResponseAsync<object>("GET", endpoint);
            });

        // Setup paged GET requests
        _mock.Setup(x => x.GetPagedAsync<It.IsAnyType>(It.IsAny<string>(), It.IsAny<QueryParameters>(), It.IsAny<CancellationToken>()))
            .Returns<string, QueryParameters, CancellationToken>((endpoint, parameters, ct) =>
            {
                _requests.Add(("GET", endpoint, parameters));
                return GetPagedResponseAsync<object>("PAGED", endpoint);
            });

        // Setup POST requests
        _mock.Setup(x => x.PostAsync<It.IsAnyType>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns<string, object, CancellationToken>((endpoint, data, ct) =>
            {
                _requests.Add(("POST", endpoint, data));
                return GetResponseAsync<object>("POST", endpoint);
            });

        // Setup PATCH requests
        _mock.Setup(x => x.PatchAsync<It.IsAnyType>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns<string, object, CancellationToken>((endpoint, data, ct) =>
            {
                _requests.Add(("PATCH", endpoint, data));
                return GetResponseAsync<object>("PATCH", endpoint);
            });

        // Setup DELETE requests
        _mock.Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns<string, CancellationToken>((endpoint, ct) =>
            {
                _requests.Add(("DELETE", endpoint, null));
                
                var key = $"DELETE:{endpoint}";
                if (_responses.ContainsKey($"{key}:EXCEPTION"))
                {
                    throw (Exception)_responses[$"{key}:EXCEPTION"];
                }
                
                return Task.CompletedTask;
            });
    }

    private Task<T> GetResponseAsync<T>(string method, string endpoint)
    {
        var key = $"{method}:{endpoint}";
        
        if (_responses.ContainsKey($"{key}:EXCEPTION"))
        {
            throw (Exception)_responses[$"{key}:EXCEPTION"];
        }
        
        if (_responses.TryGetValue(key, out var response))
        {
            return Task.FromResult((T)response);
        }
        
        throw new InvalidOperationException($"No response configured for {method} {endpoint}");
    }

    private Task<IPagedResponse<T>> GetPagedResponseAsync<T>(string method, string endpoint)
    {
        var key = $"{method}:{endpoint}";
        
        if (_responses.ContainsKey($"{key}:EXCEPTION"))
        {
            throw (Exception)_responses[$"{key}:EXCEPTION"];
        }
        
        if (_responses.TryGetValue(key, out var response))
        {
            return Task.FromResult((IPagedResponse<T>)response);
        }
        
        throw new InvalidOperationException($"No paged response configured for {method} {endpoint}");
    }
}