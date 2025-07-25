using Microsoft.Extensions.Logging;
using PlanningCenter.Api.Client.Fluent;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Services;

namespace PlanningCenter.Api.Client.Fluent;

/// <summary>
/// Base class for fluent API services providing fluent query building capabilities.
/// </summary>
/// <typeparam name="T">The domain model type</typeparam>
/// <typeparam name="TDto">The DTO type returned by the API</typeparam>
public abstract class FluentApiServiceBase<T, TDto> : ServiceBase
    where T : class
    where TDto : class
{
    protected readonly string BaseEndpoint;
    protected readonly Func<TDto, T> DtoToModelMapper;
    protected readonly string ResourceName;
    
    protected FluentApiServiceBase(
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
    
    /// <summary>
    /// Creates a new fluent query builder for this resource type.
    /// </summary>
    /// <returns>A fluent query builder instance</returns>
    public abstract IFluentQueryExecutor<T> Query();
    
    /// <summary>
    /// Creates a fluent query builder with an initial filter condition.
    /// </summary>
    /// <param name="field">The field to filter by</param>
    /// <param name="value">The value to filter for</param>
    /// <returns>A fluent query builder instance with the filter applied</returns>
    public virtual IFluentQueryBuilder<T> Where(string field, object value)
    {
        return Query().Where(field, value);
    }
    
    /// <summary>
    /// Creates a fluent query builder with multiple filter conditions.
    /// </summary>
    /// <param name="filters">Dictionary of field-value pairs to filter by</param>
    /// <returns>A fluent query builder instance with the filters applied</returns>
    public virtual IFluentQueryBuilder<T> Where(Dictionary<string, object> filters)
    {
        return Query().Where(filters);
    }
    
    /// <summary>
    /// Creates a fluent query builder with included relationships.
    /// </summary>
    /// <param name="relationships">The relationships to include</param>
    /// <returns>A fluent query builder instance with the includes applied</returns>
    public virtual IFluentQueryBuilder<T> Include(params string[] relationships)
    {
        return Query().Include(relationships);
    }
    
    /// <summary>
    /// Creates a fluent query builder with ordering.
    /// </summary>
    /// <param name="field">The field to order by</param>
    /// <param name="descending">Whether to order in descending order</param>
    /// <returns>A fluent query builder instance with the ordering applied</returns>
    public virtual IFluentQueryBuilder<T> OrderBy(string field, bool descending = false)
    {
        return Query().OrderBy(field, descending);
    }
    
    /// <summary>
    /// Creates a fluent query builder with descending ordering.
    /// </summary>
    /// <param name="field">The field to order by</param>
    /// <returns>A fluent query builder instance with the descending ordering applied</returns>
    public virtual IFluentQueryBuilder<T> OrderByDescending(string field)
    {
        return Query().OrderByDescending(field);
    }
    
    /// <summary>
    /// Creates a fluent query builder with a limit on the number of results.
    /// </summary>
    /// <param name="count">The maximum number of results to return</param>
    /// <returns>A fluent query builder instance with the limit applied</returns>
    public virtual IFluentQueryBuilder<T> Take(int count)
    {
        return Query().Take(count);
    }
    
    /// <summary>
    /// Creates a fluent query builder with pagination.
    /// </summary>
    /// <param name="page">The page number (1-based)</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <returns>A fluent query builder instance with the pagination applied</returns>
    public virtual IFluentQueryBuilder<T> Page(int page, int pageSize)
    {
        return Query().Page(page, pageSize);
    }
    
    /// <summary>
    /// Gets all resources without any filters.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A paged response containing all resources</returns>
    public virtual async Task<IPagedResponse<T>> AllAsync(CancellationToken cancellationToken = default)
    {
        return await Query().ExecuteAsync(cancellationToken);
    }
    
    /// <summary>
    /// Gets the first resource or null if none exist.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The first resource or null</returns>
    public virtual async Task<T?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        return await Query().FirstOrDefaultAsync(cancellationToken);
    }
    
    /// <summary>
    /// Gets a single resource, throwing an exception if zero or more than one exist.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The single resource</returns>
    /// <exception cref="InvalidOperationException">Thrown when zero or more than one result is found</exception>
    public virtual async Task<T> SingleAsync(CancellationToken cancellationToken = default)
    {
        return await Query().SingleAsync(cancellationToken);
    }
    
    /// <summary>
    /// Gets a single resource or null if none exist, throwing an exception if more than one exists.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The single resource or null</returns>
    /// <exception cref="InvalidOperationException">Thrown when more than one result is found</exception>
    public virtual async Task<T?> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        return await Query().SingleOrDefaultAsync(cancellationToken);
    }
    
    /// <summary>
    /// Gets the count of all resources.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The count of resources</returns>
    public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await Query().CountAsync(cancellationToken);
    }
    
    /// <summary>
    /// Checks if any resources exist.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if any resources exist, false otherwise</returns>
    public virtual async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        return await Query().AnyAsync(cancellationToken);
    }
    
    /// <summary>
    /// Public wrapper method to expose ListResourcesAsync to fluent query builders.
    /// </summary>
    /// <param name="endpoint">The API endpoint</param>
    /// <param name="parameters">Query parameters</param>
    /// <param name="mapper">Function to map from DTO to domain model</param>
    /// <param name="operationName">Name of the operation for logging</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A paged response containing the mapped domain models</returns>
    public async Task<IPagedResponse<TResult>> ExecuteListResourcesAsync<TResultDto, TResult>(
        string endpoint,
        QueryParameters? parameters,
        Func<TResultDto, TResult> mapper,
        string operationName,
        CancellationToken cancellationToken = default)
    {
        return await ListResourcesAsync<TResultDto, TResult>(endpoint, parameters, mapper, operationName, cancellationToken);
    }
}

/// <summary>
/// Concrete implementation of fluent query builder for a specific resource type.
/// </summary>
/// <typeparam name="T">The domain model type</typeparam>
/// <typeparam name="TDto">The DTO type returned by the API</typeparam>
public class FluentQueryBuilder<T, TDto> : FluentQueryBuilderBase<T, TDto>
    where T : class
    where TDto : class
{
    public FluentQueryBuilder(
        ServiceBase service,
        ILogger logger,
        string baseEndpoint,
        Func<TDto, T> mapper) : base(service, logger, baseEndpoint, mapper)
    {
    }
    
    protected override FluentQueryBuilderBase<T, TDto> CreateNew()
    {
        return new FluentQueryBuilder<T, TDto>(Service, Logger, BaseEndpoint, Mapper);
    }
    
    public override async Task<IPagedResponse<T>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (Service is FluentApiServiceBase<T, TDto> fluentService)
        {
            return await fluentService.ExecuteListResourcesAsync<TDto, T>(
                BaseEndpoint,
                Parameters,
                Mapper,
                "FluentQuery",
                cancellationToken);
        }
        
        throw new InvalidOperationException("Service must be a FluentApiServiceBase to execute fluent queries");
    }
}