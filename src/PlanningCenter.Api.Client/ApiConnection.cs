using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Exceptions;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Net.Http.Json;

namespace PlanningCenter.Api.Client;

/// <summary>
/// Core HTTP client implementation for the Planning Center API.
/// Provides built-in pagination support, error handling, and retry logic.
/// </summary>
public class ApiConnection : IApiConnection, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly PlanningCenterOptions _options;
    private readonly IAuthenticator _authenticator;
    private readonly ILogger<ApiConnection> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private bool _disposed;

    public ApiConnection(
        HttpClient httpClient,
        IOptions<PlanningCenterOptions> options,
        IAuthenticator authenticator,
        ILogger<ApiConnection> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _authenticator = authenticator ?? throw new ArgumentNullException(nameof(authenticator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _options.Validate();
        ConfigureHttpClient();

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
    }

    /// <summary>
    /// Performs a GET request to retrieve a single item
    /// </summary>
    public async Task<T> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("GET request to endpoint: {Endpoint}", endpoint);

        var response = await ExecuteWithRetryAsync(
            () => SendRequestAsync(HttpMethod.Get, endpoint, null, cancellationToken),
            cancellationToken);

        try
        {
            return await DeserializeResponseAsync<T>(response, cancellationToken);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize response for endpoint {Endpoint}", endpoint);
            throw new PlanningCenterApiGeneralException($"Failed to deserialize response: {ex.Message}", innerException: ex);
        }
    }

    /// <summary>
    /// Performs a POST request to create a new item
    /// </summary>
    public async Task<T> PostAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("POST request to endpoint: {Endpoint}", endpoint);

        var response = await ExecuteWithRetryAsync(
            () => SendRequestAsync(HttpMethod.Post, endpoint, data, cancellationToken),
            cancellationToken);

        try
        {
            return await DeserializeResponseAsync<T>(response, cancellationToken);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize response for endpoint {Endpoint}", endpoint);
            throw new PlanningCenterApiGeneralException($"Failed to deserialize response: {ex.Message}", innerException: ex);
        }
    }

    /// <summary>
    /// Performs a PUT request to update an existing item
    /// </summary>
    public async Task<T> PutAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("PUT request to endpoint: {Endpoint}", endpoint);

        var response = await ExecuteWithRetryAsync(
            () => SendRequestAsync(HttpMethod.Put, endpoint, data, cancellationToken),
            cancellationToken);

        try
        {
            return await DeserializeResponseAsync<T>(response, cancellationToken);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize response for endpoint {Endpoint}", endpoint);
            throw new PlanningCenterApiGeneralException($"Failed to deserialize response: {ex.Message}", innerException: ex);
        }
    }

    /// <summary>
    /// Performs a PATCH request to partially update an existing item
    /// </summary>
    public async Task<T> PatchAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("PATCH request to endpoint: {Endpoint}", endpoint);

        var response = await ExecuteWithRetryAsync(
            () => SendRequestAsync(HttpMethod.Patch, endpoint, data, cancellationToken),
            cancellationToken);

        try
        {
            return await DeserializeResponseAsync<T>(response, cancellationToken);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize response for endpoint {Endpoint}", endpoint);
            throw new PlanningCenterApiGeneralException($"Failed to deserialize response: {ex.Message}", innerException: ex);
        }
    }

    /// <summary>
    /// Performs a DELETE request to remove an item
    /// </summary>
    public async Task DeleteAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("DELETE request to endpoint: {Endpoint}", endpoint);

        await ExecuteWithRetryAsync(
            () => SendRequestAsync(HttpMethod.Delete, endpoint, null, cancellationToken),
            cancellationToken);
    }

    /// <summary>
    /// Performs a GET request that returns paginated results with built-in pagination support.
    /// This is the core method that enables all the pagination helpers throughout the SDK.
    /// </summary>
    public async Task<IPagedResponse<T>> GetPagedAsync<T>(
        string endpoint,
        QueryParameters? parameters = null,
        CancellationToken cancellationToken = default)
    {
        var queryString = parameters?.ToQueryString() ?? string.Empty;
        var fullEndpoint = string.IsNullOrEmpty(queryString) ? endpoint : $"{endpoint}?{queryString}";

        _logger.LogDebug("GET paginated request to endpoint: {Endpoint}", fullEndpoint);

        var response = await ExecuteWithRetryAsync(
            () => SendRequestAsync(HttpMethod.Get, fullEndpoint, null, cancellationToken),
            cancellationToken);

        try
        {
            JsonApiResponse<T>? apiResponse;

            if (_options.EnableDetailedLogging)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogDebug("Paginated response content: {Content}", content);
                apiResponse = JsonSerializer.Deserialize<JsonApiResponse<T>>(content, _jsonOptions);
            }
            else
            {
                using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                apiResponse = await JsonSerializer.DeserializeAsync<JsonApiResponse<T>>(stream, _jsonOptions, cancellationToken);
            }

            if (apiResponse == null)
            {
                throw new PlanningCenterApiGeneralException("Failed to deserialize paginated response");
            }

            // Create the paged response with navigation capabilities
            var pagedResponse = new PagedResponse<T>
            {
                Data = apiResponse.Data ?? new List<T>(),
                Meta = apiResponse.Meta ?? new PagedResponseMeta(),
                Links = apiResponse.Links ?? new PagedResponseLinks()
            };

            // Set internal properties for navigation
            if (pagedResponse is PagedResponse<T> concreteResponse)
            {
                concreteResponse.ApiConnection = this;
                concreteResponse.OriginalParameters = parameters;
                concreteResponse.OriginalEndpoint = endpoint;
            }

            _logger.LogInformation("Successfully retrieved page {CurrentPage} of {TotalPages} with {Count} items",
                pagedResponse.Meta.CurrentPage, pagedResponse.Meta.TotalPages, pagedResponse.Data.Count);

            return pagedResponse;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize paginated response for endpoint {Endpoint}", fullEndpoint);
            throw new PlanningCenterApiGeneralException($"Failed to deserialize paginated response: {ex.Message}", innerException: ex);
        }
    }

    private async Task<T> DeserializeResponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (_options.EnableDetailedLogging)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogDebug("Response content: {Content}", content);

            if (typeof(T) == typeof(object) || typeof(T) == typeof(System.Dynamic.ExpandoObject))
            {
                var expando = JsonSerializer.Deserialize<System.Dynamic.ExpandoObject>(content, _jsonOptions);
                if (expando == null)
                {
                    throw new PlanningCenterApiGeneralException($"Failed to deserialize response to ExpandoObject");
                }
                return (T)(object)expando;
            }
            var result = JsonSerializer.Deserialize<T>(content, _jsonOptions);
            if (result == null)
            {
                throw new PlanningCenterApiGeneralException($"Failed to deserialize response to {typeof(T).Name}");
            }
            return result;
        }
        else
        {
            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

            if (typeof(T) == typeof(object) || typeof(T) == typeof(System.Dynamic.ExpandoObject))
            {
                var expando = await JsonSerializer.DeserializeAsync<System.Dynamic.ExpandoObject>(stream, _jsonOptions, cancellationToken);
                if (expando == null)
                {
                    throw new PlanningCenterApiGeneralException($"Failed to deserialize response to ExpandoObject");
                }
                return (T)(object)expando;
            }
            var result = await JsonSerializer.DeserializeAsync<T>(stream, _jsonOptions, cancellationToken);
            if (result == null)
            {
                throw new PlanningCenterApiGeneralException($"Failed to deserialize response to {typeof(T).Name}");
            }
            return result;
        }
    }

    private void ConfigureHttpClient()
    {
        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        _httpClient.Timeout = _options.RequestTimeout;
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(_options.UserAgent);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.api+json"));

        // Add custom headers
        foreach (var (key, value) in _options.DefaultHeaders)
        {
            _httpClient.DefaultRequestHeaders.Add(key, value);
        }
    }

    private async Task<HttpResponseMessage> SendRequestAsync(
        HttpMethod method,
        string endpoint,
        object? data,
        CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(method, endpoint);

        // Add authentication
        var authValue = await _authenticator.GetAccessTokenAsync(cancellationToken);

        // Check if it's a Basic auth header (for PAT) or Bearer token (for OAuth)
        if (authValue.StartsWith("Basic "))
        {
            // For Personal Access Token - use the full Basic auth header
            request.Headers.Add("Authorization", authValue);
        }
        else
        {
            // For OAuth - use Bearer token
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authValue);
        }

        // Add request body for POST/PUT/PATCH
        if (data != null && (method == HttpMethod.Post || method == HttpMethod.Put || method == HttpMethod.Patch))
        {
            // Optimization: Use JsonContent when detailed logging is not enabled to avoid
            // intermediate string allocation and reduce memory pressure.
            if (_options.EnableDetailedLogging)
            {
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                request.Content = new StringContent(json, Encoding.UTF8, "application/vnd.api+json");
                _logger.LogDebug("Request body: {RequestBody}", json);
            }
            else
            {
                request.Content = JsonContent.Create(data, new MediaTypeHeaderValue("application/vnd.api+json"), _jsonOptions);
            }
        }

        var response = await _httpClient.SendAsync(request, cancellationToken);

        // Handle different response status codes
        if (response.IsSuccessStatusCode)
        {
            return response;
        }

        await HandleErrorResponseAsync(response, endpoint, method.Method, cancellationToken);

        // This should never be reached due to the exception thrown above
        throw new PlanningCenterApiGeneralException("Unexpected error occurred");
    }

    private async Task<HttpResponseMessage> ExecuteWithRetryAsync(
        Func<Task<HttpResponseMessage>> operation,
        CancellationToken cancellationToken)
    {
        var attempt = 0;

        while (true)
        {
            attempt++;

            try
            {
                return await operation();
            }
            catch (Exception ex) when (ShouldRetry(ex, attempt))
            {
                var delay = CalculateRetryDelay(attempt);
                _logger.LogWarning(ex, "Request failed on attempt {Attempt}, retrying in {Delay}ms",
                    attempt, delay.TotalMilliseconds);

                await Task.Delay(delay, cancellationToken);
            }
        }
    }

    private bool ShouldRetry(Exception exception, int attempt)
    {
        if (attempt >= _options.MaxRetryAttempts)
            return false;

        return exception switch
        {
            PlanningCenterApiServerException serverEx => serverEx.IsTransient,
            PlanningCenterApiRateLimitException => true,
            HttpRequestException => true,
            TaskCanceledException => false, // Don't retry cancellations
            _ => false
        };
    }

    private TimeSpan CalculateRetryDelay(int attempt)
    {
        // Exponential backoff with jitter
        var delay = TimeSpan.FromMilliseconds(
            _options.RetryBaseDelay.TotalMilliseconds * Math.Pow(2, attempt - 1));

        // Add jitter to prevent thundering herd
        var jitter = TimeSpan.FromMilliseconds(Random.Shared.Next(0, 1000));

        return delay + jitter;
    }

    private async Task HandleErrorResponseAsync(
        HttpResponseMessage response,
        string endpoint,
        string method,
        CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        string? requestId = null;
        if (response.Headers.TryGetValues("X-Request-Id", out var requestIdValues))
        {
            requestId = requestIdValues.FirstOrDefault();
        }

        _logger.LogError("HTTP {StatusCode} error for {Method} {Endpoint}: {Content}",
            response.StatusCode, method, endpoint, content);

        string? retryAfter = null;
        if (response.Headers.TryGetValues("Retry-After", out var retryAfterValues))
        {
            retryAfter = retryAfterValues.FirstOrDefault();
        }

        switch (response.StatusCode)
        {
            case HttpStatusCode.NotFound:
                throw new PlanningCenterApiNotFoundException(
                    "Resource not found",
                    "Unknown", // We'd need to parse this from the endpoint
                    "Unknown", // We'd need to parse this from the endpoint
                    requestId,
                    endpoint,
                    content);

            case HttpStatusCode.Unauthorized:
                throw new PlanningCenterApiAuthenticationException(
                    "Authentication failed",
                    "Bearer",
                    false,
                    requestId,
                    endpoint,
                    method,
                    content);

            case HttpStatusCode.Forbidden:
                throw new PlanningCenterApiAuthorizationException(
                    "Access denied",
                    null,
                    endpoint,
                    requestId,
                    endpoint,
                    method,
                    content);

            case HttpStatusCode.TooManyRequests:
                var headers = response.Headers.ToDictionary(h => h.Key, h => h.Value);
                throw PlanningCenterApiRateLimitException.FromHeaders(
                    "Rate limit exceeded",
                    headers,
                    requestId,
                    endpoint,
                    method);

            case HttpStatusCode.BadRequest:
            case HttpStatusCode.UnprocessableEntity:
                throw new PlanningCenterApiValidationException(
                    "Validation failed",
                    null, // We'd need to parse validation errors from content
                    (int)response.StatusCode,
                    requestId,
                    endpoint,
                    method,
                    content);

            case >= HttpStatusCode.InternalServerError:
                throw new PlanningCenterApiServerException(
                    $"Server error: {response.StatusCode}",
                    (int)response.StatusCode,
                    true,
                    requestId,
                    endpoint,
                    method,
                    content);

            default:
                throw new PlanningCenterApiGeneralException(
                    $"Unexpected HTTP status: {response.StatusCode}",
                    (int)response.StatusCode,
                    "unexpected_status",
                    requestId,
                    endpoint,
                    method,
                    content);
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _httpClient?.Dispose();
            _disposed = true;
        }
    }
}

/// <summary>
/// JSON:API response structure for parsing Planning Center API responses.
/// </summary>
internal class JsonApiResponse<T>
{
    public List<T>? Data { get; set; }
    public PagedResponseMeta? Meta { get; set; }
    public PagedResponseLinks? Links { get; set; }
}
