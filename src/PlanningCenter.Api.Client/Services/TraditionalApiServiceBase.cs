using Microsoft.Extensions.Logging;
using PlanningCenter.Api.Client.Interfaces;
using PlanningCenter.Api.Client.Models;

namespace PlanningCenter.Api.Client.Services;

/// <summary>
/// Base class for traditional API services providing standard CRUD operations.
/// </summary>
/// <typeparam name="T">The domain model type</typeparam>
/// <typeparam name="TDto">The DTO type returned by the API</typeparam>
/// <typeparam name="TCreateRequest">The create request type</typeparam>
/// <typeparam name="TUpdateRequest">The update request type</typeparam>
public abstract class TraditionalApiServiceBase<T, TDto, TCreateRequest, TUpdateRequest> : ServiceBase, IExtendedTraditionalApiService<T, TCreateRequest, TUpdateRequest>
    where T : class
    where TDto : class
    where TCreateRequest : class
    where TUpdateRequest : class
{
    protected readonly string BaseEndpoint;
    protected readonly Func<TDto, T> DtoToModelMapper;
    protected readonly Func<TCreateRequest, object> CreateRequestMapper;
    protected readonly Func<string, TUpdateRequest, object> UpdateRequestMapper;
    protected readonly string ResourceName;
    
    protected TraditionalApiServiceBase(
        ILogger logger,
        IApiConnection apiConnection,
        string baseEndpoint,
        Func<TDto, T> dtoToModelMapper,
        Func<TCreateRequest, object> createRequestMapper,
        Func<string, TUpdateRequest, object> updateRequestMapper,
        string resourceName) : base(logger, apiConnection)
    {
        BaseEndpoint = baseEndpoint ?? throw new ArgumentNullException(nameof(baseEndpoint));
        DtoToModelMapper = dtoToModelMapper ?? throw new ArgumentNullException(nameof(dtoToModelMapper));
        CreateRequestMapper = createRequestMapper ?? throw new ArgumentNullException(nameof(createRequestMapper));
        UpdateRequestMapper = updateRequestMapper ?? throw new ArgumentNullException(nameof(updateRequestMapper));
        ResourceName = resourceName ?? throw new ArgumentNullException(nameof(resourceName));
    }
    
    public virtual async Task<IPagedResponse<T>> GetAllAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        return await ListResourcesAsync<TDto, T>(
            BaseEndpoint,
            parameters,
            DtoToModelMapper,
            $"Get{ResourceName}List",
            cancellationToken);
    }
    
    public virtual async Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await GetResourceByIdAsync<TDto, T>(
            BaseEndpoint,
            id,
            DtoToModelMapper,
            $"Get{ResourceName}ById",
            cancellationToken);
    }
    
    public virtual async Task<T> CreateAsync(TCreateRequest request, CancellationToken cancellationToken = default)
    {
        return await CreateResourceAsync<TCreateRequest, TDto, T>(
            BaseEndpoint,
            request,
            CreateRequestMapper,
            DtoToModelMapper,
            $"Create{ResourceName}",
            ResourceName,
            cancellationToken);
    }
    
    public virtual async Task<T> UpdateAsync(string id, TUpdateRequest request, CancellationToken cancellationToken = default)
    {
        return await UpdateResourceAsync<TUpdateRequest, TDto, T>(
            BaseEndpoint,
            id,
            request,
            UpdateRequestMapper,
            DtoToModelMapper,
            $"Update{ResourceName}",
            ResourceName,
            cancellationToken);
    }
    
    public virtual async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await DeleteResourceAsync(
            BaseEndpoint,
            id,
            $"Delete{ResourceName}",
            ResourceName,
            cancellationToken);
    }
    
    public virtual async Task<IPagedResponse<T>> GetByFiltersAsync(Dictionary<string, object> filters, CancellationToken cancellationToken = default)
    {
        var parameters = new QueryParameters();
        foreach (var filter in filters)
        {
            parameters.Where[filter.Key] = filter.Value;
        }
        
        return await GetAllAsync(parameters, cancellationToken);
    }
    
    public virtual async Task<IPagedResponse<T>> GetWithIncludesAsync(string[] includes, CancellationToken cancellationToken = default)
    {
        var parameters = new QueryParameters
        {
            Include = includes
        };
        
        return await GetAllAsync(parameters, cancellationToken);
    }
    
    public virtual async Task<IPagedResponse<T>> GetOrderedAsync(string orderBy, CancellationToken cancellationToken = default)
    {
        var parameters = new QueryParameters
        {
            OrderBy = orderBy
        };
        
        return await GetAllAsync(parameters, cancellationToken);
    }
    
    public virtual async Task<IPagedResponse<T>> GetPageAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var parameters = new QueryParameters().WithPage(page, pageSize);
        return await GetAllAsync(parameters, cancellationToken);
    }
    
    public virtual async Task<bool> ExistsAsync(Dictionary<string, object> filters, CancellationToken cancellationToken = default)
    {
        var parameters = new QueryParameters { PerPage = 1 };
        foreach (var filter in filters)
        {
            parameters.Where[filter.Key] = filter.Value;
        }
        
        var result = await GetAllAsync(parameters, cancellationToken);
        return result.Data.Any();
    }
    
    public virtual async Task<int> CountAsync(Dictionary<string, object> filters, CancellationToken cancellationToken = default)
    {
        var parameters = new QueryParameters();
        foreach (var filter in filters)
        {
            parameters.Where[filter.Key] = filter.Value;
        }
        
        var result = await GetAllAsync(parameters, cancellationToken);
        return result.Meta?.TotalCount ?? result.Data.Count;
    }
}

/// <summary>
/// Base class for read-only traditional API services.
/// </summary>
/// <typeparam name="T">The domain model type</typeparam>
/// <typeparam name="TDto">The DTO type returned by the API</typeparam>
public abstract class ReadOnlyTraditionalApiServiceBase<T, TDto> : ServiceBase, IReadOnlyTraditionalApiService<T>
    where T : class
    where TDto : class
{
    protected readonly string BaseEndpoint;
    protected readonly Func<TDto, T> DtoToModelMapper;
    protected readonly string ResourceName;
    
    protected ReadOnlyTraditionalApiServiceBase(
        ILogger logger,
        IApiConnection apiConnection,
        string baseEndpoint,
        Func<TDto, T> dtoToModelMapper,
        string resourceName) : base(logger, apiConnection)
    {
        BaseEndpoint = baseEndpoint ?? throw new ArgumentNullException(nameof(baseEndpoint));
        DtoToModelMapper = dtoToModelMapper ?? throw new ArgumentNullException(nameof(dtoToModelMapper));
        ResourceName = resourceName ?? throw new ArgumentNullException(nameof(resourceName));
    }
    
    public virtual async Task<IPagedResponse<T>> GetAllAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        return await ListResourcesAsync<TDto, T>(
            BaseEndpoint,
            parameters,
            DtoToModelMapper,
            $"Get{ResourceName}List",
            cancellationToken);
    }
    
    public virtual async Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await GetResourceByIdAsync<TDto, T>(
            BaseEndpoint,
            id,
            DtoToModelMapper,
            $"Get{ResourceName}ById",
            cancellationToken);
    }
}