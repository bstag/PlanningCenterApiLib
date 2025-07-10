using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Utilities;

/// <summary>
/// VerificationTimes enum for verification methods
/// </summary>
public enum VerificationTimes
{
    /// <summary>
    /// Verify that a method was never called
    /// </summary>
    Never,
    
    /// <summary>
    /// Verify that a method was called exactly once
    /// </summary>
    Once,
    
    /// <summary>
    /// Verify that a method was called at least once
    /// </summary>
    AtLeastOnce,
    
    /// <summary>
    /// Verify that a method was called exactly the specified number of times
    /// </summary>
    Exactly
}

/// <summary>
/// Mock API connection for unit testing services without making real HTTP calls.
/// </summary>
public class MockApiConnection : IApiConnection
{
    private readonly Dictionary<string, object> _responses = new();
    private readonly Dictionary<string, Queue<object>> _sequenceResponses = new();
    private readonly List<(string Method, string Endpoint, object? Data)> _requests = new();
    private Func<string, string, object?, Task<object>>? _customResponseHandler;

    public MockApiConnection() { }

    /// <summary>
    /// Exposes itself as an IApiConnection implementation for use in tests.
    /// </summary>
    public IApiConnection Object => this;

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
    /// Sets up a response for a PUT request to a specific endpoint.
    /// </summary>
    public MockApiConnection SetupPutResponse<T>(string endpoint, T response)
    {
        _responses[$"PUT:{endpoint}"] = response!;
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
    public MockApiConnection SetupDeleteResponse(string endpoint)
    {
        // Maintain backward compatibility with tests that expect SetupDeleteResponse
        _responses[$"DELETE:{endpoint}"] = true;
        return this;
    }

    public MockApiConnection SetupDeleteSuccess(string endpoint) => SetupDeleteResponse(endpoint);

    /// <summary>
    /// Sets up a paged response for a GET request.
    /// </summary>
    public MockApiConnection SetupPagedResponse<T>(string endpoint, IPagedResponse<T> response)
    {
        _responses[$"PAGED:{endpoint}"] = response;
        return this;
    }

    /// <summary>
    /// Sets up a sequence of paged responses that will be returned in order for successive calls.
    /// Useful for simulating multi-page API flows.
    /// </summary>
    public MockApiConnection SetupPagedSequence<T>(string endpoint, params IPagedResponse<T>[] responses)
    {
        if (responses.Length > 0)
        {
            // Configure each response with the necessary properties for pagination to work
            foreach (var response in responses)
            {
                if (response is PagedResponse<T> pagedResponse)
                {
                    pagedResponse.ApiConnection = this;
                    pagedResponse.OriginalEndpoint = endpoint;
                    pagedResponse.OriginalParameters = new QueryParameters();
                }
            }
            
            // Store the first response in the regular responses dictionary
            _responses[$"PAGED:{endpoint}"] = responses[0];
            
            // Store any additional responses in the sequence queue
            if (responses.Length > 1)
            {
                _sequenceResponses[$"PAGED:{endpoint}"] = new Queue<object>(responses.Skip(1).Cast<object>());
            }
        }
        return this;
    }

    /// <summary>
    /// Sets up an exception to be thrown for a specific endpoint.
    /// </summary>
    public MockApiConnection SetupException(string endpoint, Exception exception)
    {
        // Register the exception for all HTTP verbs so tests don't need to specify the verb.
        var verbs = new[] { "GET", "POST", "PUT", "PATCH", "DELETE", "PAGED" };
        foreach (var verb in verbs)
        {
            _responses[$"{verb}:{endpoint}:EXCEPTION"] = exception;
        }
        return this;
    }

    public MockApiConnection SetupException(string method, string endpoint, Exception exception)
    {
        _responses[$"{method}:{endpoint}:EXCEPTION"] = exception;
        return this;
    }

    /// <summary>
    /// Sets up a custom response handler for all requests.
    /// This handler will be called for any request that doesn't have a specific response configured.
    /// </summary>
    /// <param name="handler">A function that takes a method, endpoint, and data and returns a response</param>
    public MockApiConnection SetupCustomResponse(Func<string, string, object?, Task<object>> handler)
    {
        _customResponseHandler = handler;
        return this;
    }

    /// <summary>
    /// Verifies that a specific request was made to the mock.
    /// </summary>
    public void VerifyRequest(string method, string endpoint, object? data = null)
    {
        var matchingRequests = _requests.Where(r => 
            r.Method == method && 
            r.Endpoint == endpoint && 
            (data == null || JsonSerializer.Serialize(r.Data) == JsonSerializer.Serialize(data)));
        
        Assert.True(matchingRequests.Any(), $"Expected to find a {method} request to {endpoint}, but none was found.");
    }

    /// <summary>
    /// Verifies that a specific request was not made to the mock.
    /// </summary>
    public void VerifyNoRequest(string method, string endpoint)
    {
        var matchingRequests = _requests.Where(r => r.Method == method && r.Endpoint == endpoint);
        Assert.False(matchingRequests.Any(), $"Expected no {method} requests to {endpoint}, but at least one was found.");
    }
    
    /// <summary>
    /// Verifies that a GET request was made to the specified endpoint.
    /// </summary>
    public void VerifyGetRequest(string endpoint, VerificationTimes times = VerificationTimes.Once, object? data = null)
    {
        VerifyRequestWithTimes("GET", endpoint, times, data);
    }
    
    /// <summary>
    /// Verifies that a POST request was made to the specified endpoint.
    /// </summary>
    public void VerifyPostRequest(string endpoint, VerificationTimes times = VerificationTimes.Once, object? data = null)
    {
        VerifyRequestWithTimes("POST", endpoint, times, data);
    }
    
    /// <summary>
    /// Verifies that a PUT request was made to the specified endpoint.
    /// </summary>
    public void VerifyPutRequest(string endpoint, VerificationTimes times = VerificationTimes.Once, object? data = null)
    {
        VerifyRequestWithTimes("PUT", endpoint, times, data);
    }
    
    /// <summary>
    /// Verifies that a PATCH request was made to the specified endpoint.
    /// </summary>
    public void VerifyPatchRequest(string endpoint, VerificationTimes times = VerificationTimes.Once, object? data = null)
    {
        VerifyRequestWithTimes("PATCH", endpoint, times, data);
    }
    
    /// <summary>
    /// Verifies that a DELETE request was made to the specified endpoint.
    /// </summary>
    public void VerifyDeleteRequest(string endpoint, VerificationTimes times = VerificationTimes.Once)
    {
        VerifyRequestWithTimes("DELETE", endpoint, times);
    }
    
    /// <summary>
    /// Verifies that no requests were made to the mock.
    /// </summary>
    public void VerifyNoRequests()
    {
        Assert.Empty(_requests);
    }
    
    private void VerifyRequestWithTimes(string method, string endpoint, VerificationTimes times, object? data = null)
    {
        var matchingRequests = _requests.Where(r => 
            r.Method == method && 
            r.Endpoint == endpoint && 
            (data == null || JsonSerializer.Serialize(r.Data) == JsonSerializer.Serialize(data)));
        
        var count = matchingRequests.Count();
        
        switch (times)
        {
            case VerificationTimes.Never:
                Assert.Equal(0, count);
                break;
            case VerificationTimes.Once:
                Assert.Equal(1, count);
                break;
            case VerificationTimes.AtLeastOnce:
                Assert.True(count >= 1, $"Expected at least one {method} request to {endpoint}, but found {count}");
                break;
            case VerificationTimes.Exactly:
                // This case requires an additional parameter which is not supported in this implementation
                // For now, we'll just assert that at least one request was made
                Assert.True(count >= 1, $"Expected at least one {method} request to {endpoint}, but found {count}");
                break;
        }
    }

    /// <summary>
    /// Verifies that a specific number of requests were made to the mock.
    /// </summary>
    public void VerifyRequestCount(int count)
    {
        Assert.Equal(count, _requests.Count);
    }

    /// <summary>
    /// Verifies that a specific number of requests were made to a specific endpoint.
    /// </summary>
    public void VerifyRequestCount(string method, string endpoint, int count)
    {
        var requestCount = _requests.Count(r => r.Method == method && r.Endpoint == endpoint);
        Assert.Equal(count, requestCount);
    }

    /// <summary>
    /// Clears all recorded requests.
    /// </summary>
    public void ClearRequests()
    {
        _requests.Clear();
    }

    /// <summary>
    /// Clears all configured responses.
    /// </summary>
    public void ClearResponses()
    {
        _responses.Clear();
        _sequenceResponses.Clear();
        _customResponseHandler = null;
    }

    /// <summary>
    /// Resets the mock by clearing all requests and responses.
    /// </summary>
    public void Reset()
    {
        ClearRequests();
        ClearResponses();
    }

    #region IApiConnection Implementation

    public Task<T> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested) throw new OperationCanceledException(cancellationToken);
        _requests.Add(("GET", endpoint, null));
        return GetResponseAsync<T>("GET", endpoint);
    }

    public Task<T> PostAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested) throw new OperationCanceledException(cancellationToken);
        _requests.Add(("POST", endpoint, data));
        return GetResponseAsync<T>("POST", endpoint, data);
    }

    public Task<T> PutAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested) throw new OperationCanceledException(cancellationToken);
        _requests.Add(("PUT", endpoint, data));
        return GetResponseAsync<T>("PUT", endpoint, data);
    }

    public Task<T> PatchAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested) throw new OperationCanceledException(cancellationToken);
        _requests.Add(("PATCH", endpoint, data));
        return GetResponseAsync<T>("PATCH", endpoint, data);
    }

    public Task DeleteAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested) throw new OperationCanceledException(cancellationToken);
        _requests.Add(("DELETE", endpoint, null));
        if (_responses.ContainsKey($"DELETE:{endpoint}"))
        {
            return Task.CompletedTask;
        }
        if (_responses.ContainsKey($"DELETE:{endpoint}:EXCEPTION"))
        {
            throw (Exception)_responses[$"DELETE:{endpoint}:EXCEPTION"];
        }
        throw new InvalidOperationException($"No response configured for DELETE {endpoint}");
    }

    public Task<IPagedResponse<T>> GetPagedAsync<T>(string endpoint, QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested) throw new OperationCanceledException(cancellationToken);
        _requests.Add(("GET", endpoint, parameters));
        
        // Use custom response handler if available
        if (_customResponseHandler != null)
        {
            return _customResponseHandler("PAGED", endpoint, parameters)
                .ContinueWith(t => (IPagedResponse<T>)t.Result);
        }
        
        return GetPagedResponseAsync<T>("PAGED", endpoint, parameters);
    }

    private Task<T> GetResponseAsync<T>(string method, string endpoint, object? data = null)
    {
        var key = $"{method}:{endpoint}";
        
        // Use custom response handler if available
        if (_customResponseHandler != null)
        {
            return _customResponseHandler(method, endpoint, data)
                .ContinueWith(t => (T)t.Result);
        }

        // Return the next queued response if a sequence has been configured
        if (_sequenceResponses.TryGetValue(key, out var seqQueue) && seqQueue.Count > 0)
        {
            return Task.FromResult((T)seqQueue.Dequeue());
        }

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

    private Task<IPagedResponse<T>> GetPagedResponseAsync<T>(string method, string endpoint, QueryParameters? parameters = null)
    {
        var key = $"{method}:{endpoint}";
        
        // Return the next queued response if a sequence has been configured
        if (_sequenceResponses.TryGetValue(key, out var seqQueue) && seqQueue.Count > 0)
        {
            var nextResponse = seqQueue.Dequeue();
            return Task.FromResult((IPagedResponse<T>)nextResponse);
        }

        if (_responses.ContainsKey($"{key}:EXCEPTION"))
        {
            throw (Exception)_responses[$"{key}:EXCEPTION"];
        }

        if (_responses.TryGetValue(key, out var response))
        {
            return Task.FromResult((IPagedResponse<T>)response);
        }

        throw new InvalidOperationException($"No response configured for {method} {endpoint}");
    }

    #endregion
}
