using Microsoft.Extensions.Logging;
using PlanningCenter.Api.Client.Correlation;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Exceptions;
using PlanningCenter.Api.Client.Models.JsonApi;
using PlanningCenter.Api.Client.Monitoring;

namespace PlanningCenter.Api.Client.Services;

/// <summary>
/// Base class for Planning Center API services with standardized logging and exception handling.
/// </summary>
public abstract class ServiceBase
{
    protected readonly ILogger Logger;
    protected readonly IApiConnection ApiConnection;

    protected ServiceBase(ILogger logger, IApiConnection apiConnection)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        ApiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
    }

    /// <summary>
    /// Executes an operation with standardized exception handling and performance monitoring.
    /// </summary>
    /// <typeparam name="T">The return type of the operation</typeparam>
    /// <param name="operation">The operation to execute</param>
    /// <param name="operationName">The name of the operation for logging</param>
    /// <param name="resourceId">Optional resource identifier</param>
    /// <param name="allowNotFound">Whether to return null for NotFound exceptions instead of throwing</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of the operation</returns>
    protected async Task<T> ExecuteAsync<T>(
        Func<Task<T>> operation,
        string operationName,
        string? resourceId = null,
        bool allowNotFound = false,
        CancellationToken cancellationToken = default)
    {
        return await PerformanceMonitor.TrackAsync(
            Logger,
            operationName,
            async () =>
            {
                try
                {
                    return await operation();
                }
                catch (PlanningCenterApiNotFoundException) when (allowNotFound)
                {
                    var correlationId = CorrelationContext.Current;
                    Logger.LogWarning("Resource not found: {ResourceId} [CorrelationId: {CorrelationId}]", 
                        resourceId ?? "unknown", correlationId);
                    return default(T)!;
                }
                catch (PlanningCenterApiException ex)
                {
                    var correlationId = CorrelationContext.Current;
                    Logger.LogError(ex, "API error in {OperationName} for resource {ResourceId}: {ErrorMessage} [CorrelationId: {CorrelationId}]",
                        operationName, resourceId ?? "unknown", ex.Message, correlationId);
                    throw;
                }
                catch (Exception ex)
                {
                    var correlationId = CorrelationContext.Current;
                    Logger.LogError(ex, "Unexpected error in {OperationName} for resource {ResourceId} [CorrelationId: {CorrelationId}]",
                        operationName, resourceId ?? "unknown", correlationId);
                    throw;
                }
            },
            resourceId,
            cancellationToken);
    }

    /// <summary>
    /// Executes an operation without a return value with standardized exception handling and performance monitoring.
    /// </summary>
    /// <param name="operation">The operation to execute</param>
    /// <param name="operationName">The name of the operation for logging</param>
    /// <param name="resourceId">Optional resource identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    protected async Task ExecuteAsync(
        Func<Task> operation,
        string operationName,
        string? resourceId = null,
        CancellationToken cancellationToken = default)
    {
        await PerformanceMonitor.TrackAsync(
            Logger,
            operationName,
            operation,
            resourceId,
            cancellationToken);
    }

    /// <summary>
    /// Executes a get operation that may return null for not found resources.
    /// </summary>
    /// <typeparam name="T">The return type of the operation</typeparam>
    /// <param name="operation">The operation to execute</param>
    /// <param name="operationName">The name of the operation for logging</param>
    /// <param name="resourceId">The resource identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of the operation or null if not found</returns>
    protected async Task<T?> ExecuteGetAsync<T>(
        Func<Task<T>> operation,
        string operationName,
        string resourceId,
        CancellationToken cancellationToken = default)
        where T : class
    {
        return await ExecuteAsync(operation, operationName, resourceId, allowNotFound: true, cancellationToken);
    }

    /// <summary>
    /// Validates that the provided parameter is not null or empty.
    /// </summary>
    /// <param name="value">The value to validate</param>
    /// <param name="paramName">The parameter name</param>
    /// <exception cref="ArgumentException">Thrown when the value is null or empty</exception>
    protected static void ValidateNotNullOrEmpty(string? value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{paramName} cannot be null or empty", paramName);
        }
    }

    /// <summary>
    /// Validates that the provided parameter is not null.
    /// </summary>
    /// <param name="value">The value to validate</param>
    /// <param name="paramName">The parameter name</param>
    /// <exception cref="ArgumentNullException">Thrown when the value is null</exception>
    protected static void ValidateNotNull<T>(T? value, string paramName) where T : class
    {
        if (value == null)
        {
            throw new ArgumentNullException(paramName);
        }
    }
    
    /// <summary>
    /// Generic method to list resources with standardized error handling and pagination.
    /// </summary>
    /// <typeparam name="TDto">The DTO type returned by the API</typeparam>
    /// <typeparam name="TDomain">The domain model type to return</typeparam>
    /// <param name="endpoint">The API endpoint</param>
    /// <param name="parameters">Query parameters</param>
    /// <param name="mapper">Function to map from DTO to domain model</param>
    /// <param name="operationName">Name of the operation for logging</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A paged response containing the mapped domain models</returns>
    protected async Task<IPagedResponse<TDomain>> ListResourcesAsync<TDto, TDomain>(
        string endpoint,
        QueryParameters? parameters,
        Func<TDto, TDomain> mapper,
        string operationName,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(
            async () =>
            {
                var response = await ApiConnection.GetPagedAsync<TDto>(
                    endpoint, parameters, cancellationToken);

                var items = response.Data.Select(mapper).ToList();

                return new PagedResponse<TDomain>
                {
                    Data = items,
                    Meta = response.Meta,
                    Links = response.Links
                };
            },
            operationName,
            cancellationToken: cancellationToken);
    }
    
    /// <summary>
    /// Generic method to get a resource by ID with standardized error handling.
    /// </summary>
    /// <typeparam name="TDto">The DTO type returned by the API</typeparam>
    /// <typeparam name="TDomain">The domain model type to return</typeparam>
    /// <param name="endpoint">The API endpoint</param>
    /// <param name="resourceId">The ID of the resource to retrieve</param>
    /// <param name="mapper">Function to map from DTO to domain model</param>
    /// <param name="operationName">Name of the operation for logging</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The mapped domain model or null if not found</returns>
    protected async Task<TDomain?> GetResourceByIdAsync<TDto, TDomain>(
        string endpoint,
        string resourceId,
        Func<TDto, TDomain> mapper,
        string operationName,
        CancellationToken cancellationToken = default)
        where TDto : class
        where TDomain : class
    {
        ValidateNotNullOrEmpty(resourceId, nameof(resourceId));
        
        return await ExecuteGetAsync<TDomain>(
            async () =>
            {
                var envelope = await ApiConnection.GetAsync<JsonApiSingleResponse<TDto>>(
                    $"{endpoint}/{resourceId}", cancellationToken);

                return envelope?.Data != null ? mapper(envelope.Data) : default(TDomain)!;
            },
            operationName,
            resourceId,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Logs a successful operation with standardized format and correlation ID.
    /// </summary>
    /// <param name="operationName">The name of the operation</param>
    /// <param name="resourceName">The name of the resource type</param>
    /// <param name="resourceId">The ID of the resource</param>
    /// <param name="additionalInfo">Optional additional information to log</param>
    protected void LogSuccess(string operationName, string resourceName, string? resourceId = null, string? additionalInfo = null)
    {
        var correlationId = CorrelationContext.Current;
        var message = $"Successfully {operationName.ToLowerInvariant()} {resourceName.ToLowerInvariant()}";
        
        if (!string.IsNullOrEmpty(resourceId))
        {
            message += $" (ID: {resourceId})";
        }
        
        if (!string.IsNullOrEmpty(additionalInfo))
        {
            message += $": {additionalInfo}";
        }
        
        message += $" [CorrelationId: {correlationId}]";
        
        Logger.LogInformation(message);
    }

    /// <summary>
    /// Generic method to create a resource with standardized error handling and logging.
    /// </summary>
    /// <typeparam name="TCreateRequest">The create request type</typeparam>
    /// <typeparam name="TDto">The DTO type returned by the API</typeparam>
    /// <typeparam name="TDomain">The domain model type to return</typeparam>
    /// <param name="endpoint">The API endpoint</param>
    /// <param name="request">The create request</param>
    /// <param name="requestMapper">Function to map from create request to JSON API format</param>
    /// <param name="responseMapper">Function to map from DTO to domain model</param>
    /// <param name="operationName">Name of the operation for logging</param>
    /// <param name="resourceName">Name of the resource type for logging</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created domain model</returns>
    protected async Task<TDomain> CreateResourceAsync<TCreateRequest, TDto, TDomain>(
        string endpoint,
        TCreateRequest request,
        Func<TCreateRequest, object> requestMapper,
        Func<TDto, TDomain> responseMapper,
        string operationName,
        string resourceName,
        CancellationToken cancellationToken = default)
        where TCreateRequest : class
        where TDto : class
        where TDomain : class
    {
        ValidateNotNull(request, nameof(request));
        
        return await ExecuteAsync(
            async () =>
            {
                var jsonApiRequest = requestMapper(request);
                
                var response = await ApiConnection.PostAsync<JsonApiSingleResponse<TDto>>(
                    endpoint, jsonApiRequest, cancellationToken);
                
                if (response?.Data == null)
                    throw new PlanningCenterApiGeneralException($"Failed to create {resourceName.ToLowerInvariant()} - no data returned");
                
                var domainModel = responseMapper(response.Data);
                
                // Extract ID for logging if the domain model has an Id property
                var idProperty = typeof(TDomain).GetProperty("Id");
                var resourceId = idProperty?.GetValue(domainModel)?.ToString();
                
                LogSuccess(operationName, resourceName, resourceId);
                
                return domainModel;
            },
            operationName,
            resourceId: null,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Generic method to update a resource with standardized error handling and logging.
    /// </summary>
    /// <typeparam name="TUpdateRequest">The update request type</typeparam>
    /// <typeparam name="TDto">The DTO type returned by the API</typeparam>
    /// <typeparam name="TDomain">The domain model type to return</typeparam>
    /// <param name="endpoint">The API endpoint</param>
    /// <param name="resourceId">The ID of the resource to update</param>
    /// <param name="request">The update request</param>
    /// <param name="requestMapper">Function to map from update request to JSON API format</param>
    /// <param name="responseMapper">Function to map from DTO to domain model</param>
    /// <param name="operationName">Name of the operation for logging</param>
    /// <param name="resourceName">Name of the resource type for logging</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated domain model</returns>
    protected async Task<TDomain> UpdateResourceAsync<TUpdateRequest, TDto, TDomain>(
        string endpoint,
        string resourceId,
        TUpdateRequest request,
        Func<string, TUpdateRequest, object> requestMapper,
        Func<TDto, TDomain> responseMapper,
        string operationName,
        string resourceName,
        CancellationToken cancellationToken = default)
        where TUpdateRequest : class
        where TDto : class
        where TDomain : class
    {
        ValidateNotNullOrEmpty(resourceId, nameof(resourceId));
        ValidateNotNull(request, nameof(request));
        
        return await ExecuteAsync(
            async () =>
            {
                var jsonApiRequest = requestMapper(resourceId, request);
                
                var response = await ApiConnection.PatchAsync<JsonApiSingleResponse<TDto>>(
                    $"{endpoint}/{resourceId}", jsonApiRequest, cancellationToken);
                
                if (response?.Data == null)
                    throw new PlanningCenterApiGeneralException($"Failed to update {resourceName.ToLowerInvariant()} - no data returned");
                
                var domainModel = responseMapper(response.Data);
                
                LogSuccess(operationName, resourceName, resourceId);
                
                return domainModel;
            },
            operationName,
            resourceId,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Generic method to delete a resource with standardized error handling and logging.
    /// </summary>
    /// <param name="endpoint">The API endpoint</param>
    /// <param name="resourceId">The ID of the resource to delete</param>
    /// <param name="operationName">Name of the operation for logging</param>
    /// <param name="resourceName">Name of the resource type for logging</param>
    /// <param name="cancellationToken">Cancellation token</param>
    protected async Task DeleteResourceAsync(
        string endpoint,
        string resourceId,
        string operationName,
        string resourceName,
        CancellationToken cancellationToken = default)
    {
        ValidateNotNullOrEmpty(resourceId, nameof(resourceId));
        
        await ExecuteAsync(
            async () =>
            {
                await ApiConnection.DeleteAsync($"{endpoint}/{resourceId}", cancellationToken);
                
                LogSuccess(operationName, resourceName, resourceId);
            },
            operationName,
            resourceId,
            cancellationToken);
    }
}