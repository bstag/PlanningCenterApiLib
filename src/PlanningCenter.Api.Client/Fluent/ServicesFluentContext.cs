using System.Linq.Expressions;
using PlanningCenter.Api.Client.Fluent.QueryBuilder;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Fluent;
using PlanningCenter.Api.Client.Models.Services;

namespace PlanningCenter.Api.Client.Fluent;

/// <summary>
/// Fluent API implementation for the Services module.
/// Provides LINQ-like syntax for querying and manipulating service plans data with built-in pagination support.
/// </summary>
public class ServicesFluentContext : IServicesFluentContext
{
    private readonly IServicesService _servicesService;
    private readonly FluentQueryBuilder<Plan> _queryBuilder = new();

    public ServicesFluentContext(IServicesService servicesService)
    {
        _servicesService = servicesService ?? throw new ArgumentNullException(nameof(servicesService));
    }

    #region Query Building Methods

    public IServicesFluentContext Where(Expression<Func<Plan, bool>> predicate)
    {
        _queryBuilder.Where(predicate);
        return this;
    }

    public IServicesFluentContext Include(Expression<Func<Plan, object>> include)
    {
        _queryBuilder.Include(include);
        return this;
    }

    public IServicesFluentContext OrderBy(Expression<Func<Plan, object>> orderBy)
    {
        _queryBuilder.OrderBy(orderBy);
        return this;
    }

    public IServicesFluentContext OrderByDescending(Expression<Func<Plan, object>> orderBy)
    {
        _queryBuilder.OrderByDescending(orderBy);
        return this;
    }

    public IServicesFluentContext ThenBy(Expression<Func<Plan, object>> thenBy)
    {
        _queryBuilder.ThenBy(thenBy);
        return this;
    }

    public IServicesFluentContext ThenByDescending(Expression<Func<Plan, object>> thenBy)
    {
        _queryBuilder.ThenByDescending(thenBy);
        return this;
    }

    #endregion

    #region Execution Methods

    public async Task<Plan?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID cannot be null or empty", nameof(id));

        return await _servicesService.GetPlanAsync(id, cancellationToken);
    }

    public async Task<IPagedResponse<Plan>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = pageSize;
        
        return await _servicesService.ListPlansAsync(parameters, cancellationToken);
    }

    public async Task<IPagedResponse<Plan>> GetPageAsync(int page, int pageSize = 25, CancellationToken cancellationToken = default)
    {
        if (page < 1) throw new ArgumentException("Page must be 1 or greater", nameof(page));
        if (pageSize < 1) throw new ArgumentException("Page size must be 1 or greater", nameof(pageSize));

        var parameters = _queryBuilder.Build();
        parameters.PerPage = pageSize;
        parameters.Offset = (page - 1) * pageSize;
        
        return await _servicesService.ListPlansAsync(parameters, cancellationToken);
    }

    public async Task<IReadOnlyList<Plan>> GetAllAsync(PaginationOptions? options = null, CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        return await _servicesService.GetAllPlansAsync(parameters, options, cancellationToken);
    }

    public IAsyncEnumerable<Plan> AsAsyncEnumerable(PaginationOptions? options = null, CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        return _servicesService.StreamPlansAsync(parameters, options, cancellationToken);
    }

    #endregion

    #region LINQ-like Terminal Operations

    public async Task<Plan> FirstAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 1;
        
        var response = await _servicesService.ListPlansAsync(parameters, cancellationToken);
        
        if (!response.Data.Any())
            throw new InvalidOperationException("Sequence contains no elements");
            
        return response.Data.First();
    }

    public async Task<Plan?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 1;
        
        var response = await _servicesService.ListPlansAsync(parameters, cancellationToken);
        return response.Data.FirstOrDefault();
    }

    public async Task<Plan> FirstAsync(Expression<Func<Plan, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var contextWithPredicate = Where(predicate);
        return await contextWithPredicate.FirstAsync(cancellationToken);
    }

    public async Task<Plan?> FirstOrDefaultAsync(Expression<Func<Plan, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var contextWithPredicate = Where(predicate);
        return await contextWithPredicate.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Plan> SingleAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 2;
        
        var response = await _servicesService.ListPlansAsync(parameters, cancellationToken);
        
        if (!response.Data.Any())
            throw new InvalidOperationException("Sequence contains no elements");
            
        if (response.Data.Count > 1 || (response.Meta?.TotalCount ?? 0) > 1)
            throw new InvalidOperationException("Sequence contains more than one element");
            
        return response.Data.First();
    }

    public async Task<Plan?> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 2;
        
        var response = await _servicesService.ListPlansAsync(parameters, cancellationToken);
        
        if (!response.Data.Any())
            return null;
            
        if (response.Data.Count > 1 || (response.Meta?.TotalCount ?? 0) > 1)
            throw new InvalidOperationException("Sequence contains more than one element");
            
        return response.Data.First();
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 1;
        
        var response = await _servicesService.ListPlansAsync(parameters, cancellationToken);
        return response.Meta?.TotalCount ?? 0;
    }

    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 1;
        
        var response = await _servicesService.ListPlansAsync(parameters, cancellationToken);
        return response.Data.Any();
    }

    public async Task<bool> AnyAsync(Expression<Func<Plan, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var contextWithPredicate = Where(predicate);
        return await contextWithPredicate.AnyAsync(cancellationToken);
    }

    #endregion

    #region Specialized Services Operations

    public IServicesFluentContext ByServiceType(string serviceTypeId)
    {
        if (string.IsNullOrWhiteSpace(serviceTypeId))
            throw new ArgumentException("Service Type ID cannot be null or empty", nameof(serviceTypeId));

        return Where(p => p.ServiceTypeId == serviceTypeId);
    }

    public IServicesFluentContext ByDateRange(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
            throw new ArgumentException("Start date cannot be after end date", nameof(startDate));

        return Where(p => p.SortDate >= startDate && p.SortDate <= endDate);
    }

    public IServicesFluentContext Upcoming()
    {
        var now = DateTime.Now;
        return Where(p => p.SortDate > now);
    }

    public IServicesFluentContext Past()
    {
        var now = DateTime.Now;
        return Where(p => p.SortDate < now);
    }

    public IServicesFluentContext ThisWeek()
    {
        var today = DateTime.Today;
        var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
        var endOfWeek = startOfWeek.AddDays(7);
        
        return Where(p => p.SortDate >= startOfWeek && p.SortDate < endOfWeek);
    }

    public IServicesFluentContext ThisMonth()
    {
        var today = DateTime.Today;
        var startOfMonth = new DateTime(today.Year, today.Month, 1);
        var endOfMonth = startOfMonth.AddMonths(1);
        
        return Where(p => p.SortDate >= startOfMonth && p.SortDate < endOfMonth);
    }

    public IServicesFluentContext Public()
    {
        return Where(p => p.IsPublic);
    }

    public IServicesFluentContext Private()
    {
        return Where(p => !p.IsPublic);
    }

    public IServicesFluentContext WithMinimumLength(int minimumMinutes)
    {
        if (minimumMinutes < 0)
            throw new ArgumentException("Minimum length cannot be negative", nameof(minimumMinutes));

        return Where(p => p.Length >= minimumMinutes);
    }

    public IServicesFluentContext ByTitleContains(string titleFragment)
    {
        if (string.IsNullOrWhiteSpace(titleFragment))
            throw new ArgumentException("Title fragment cannot be null or empty", nameof(titleFragment));

        return Where(p => p.Title.Contains(titleFragment));
    }

    #endregion
}