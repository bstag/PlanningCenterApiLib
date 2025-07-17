using System.Linq.Expressions;
using PlanningCenter.Api.Client.Fluent.QueryBuilder;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Fluent;
using PlanningCenter.Api.Client.Models.Giving;

namespace PlanningCenter.Api.Client.Fluent;

/// <summary>
/// Fluent API implementation for the Giving module.
/// Provides LINQ-like syntax for querying and manipulating giving data with built-in pagination support.
/// </summary>
public class GivingFluentContext : IGivingFluentContext
{
    private readonly IGivingService _givingService;
    private readonly FluentQueryBuilder<Donation> _queryBuilder = new();

    public GivingFluentContext(IGivingService givingService)
    {
        _givingService = givingService ?? throw new ArgumentNullException(nameof(givingService));
    }

    #region Query Building Methods

    public IGivingFluentContext Where(Expression<Func<Donation, bool>> predicate)
    {
        _queryBuilder.Where(predicate);
        return this;
    }

    public IGivingFluentContext Include(Expression<Func<Donation, object>> include)
    {
        _queryBuilder.Include(include);
        return this;
    }

    public IGivingFluentContext OrderBy(Expression<Func<Donation, object>> orderBy)
    {
        _queryBuilder.OrderBy(orderBy);
        return this;
    }

    public IGivingFluentContext OrderByDescending(Expression<Func<Donation, object>> orderBy)
    {
        _queryBuilder.OrderByDescending(orderBy);
        return this;
    }

    public IGivingFluentContext ThenBy(Expression<Func<Donation, object>> thenBy)
    {
        _queryBuilder.ThenBy(thenBy);
        return this;
    }

    public IGivingFluentContext ThenByDescending(Expression<Func<Donation, object>> thenBy)
    {
        _queryBuilder.ThenByDescending(thenBy);
        return this;
    }

    #endregion

    #region Execution Methods

    public async Task<Donation?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID cannot be null or empty", nameof(id));

        return await _givingService.GetDonationAsync(id, cancellationToken);
    }

    public async Task<IPagedResponse<Donation>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = pageSize;
        
        return await _givingService.ListDonationsAsync(parameters, cancellationToken);
    }

    public async Task<IPagedResponse<Donation>> GetPageAsync(int page, int pageSize = 25, CancellationToken cancellationToken = default)
    {
        if (page < 1) throw new ArgumentException("Page must be 1 or greater", nameof(page));
        if (pageSize < 1) throw new ArgumentException("Page size must be 1 or greater", nameof(pageSize));

        var parameters = _queryBuilder.Build();
        parameters.PerPage = pageSize;
        parameters.Offset = (page - 1) * pageSize;
        
        return await _givingService.ListDonationsAsync(parameters, cancellationToken);
    }

    public async Task<IReadOnlyList<Donation>> GetAllAsync(PaginationOptions? options = null, CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        return await _givingService.GetAllDonationsAsync(parameters, options, cancellationToken);
    }

    public IAsyncEnumerable<Donation> AsAsyncEnumerable(PaginationOptions? options = null, CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        return _givingService.StreamDonationsAsync(parameters, options, cancellationToken);
    }

    #endregion

    #region LINQ-like Terminal Operations

    public async Task<Donation> FirstAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 1;
        
        var response = await _givingService.ListDonationsAsync(parameters, cancellationToken);
        
        if (!response.Data.Any())
            throw new InvalidOperationException("Sequence contains no elements");
            
        return response.Data.First();
    }

    public async Task<Donation?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 1;
        
        var response = await _givingService.ListDonationsAsync(parameters, cancellationToken);
        return response.Data.FirstOrDefault();
    }

    public async Task<Donation> FirstAsync(Expression<Func<Donation, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var contextWithPredicate = Where(predicate);
        return await contextWithPredicate.FirstAsync(cancellationToken);
    }

    public async Task<Donation?> FirstOrDefaultAsync(Expression<Func<Donation, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var contextWithPredicate = Where(predicate);
        return await contextWithPredicate.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Donation> SingleAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 2;
        
        var response = await _givingService.ListDonationsAsync(parameters, cancellationToken);
        
        if (!response.Data.Any())
            throw new InvalidOperationException("Sequence contains no elements");
            
        if (response.Data.Count > 1 || (response.Meta?.TotalCount ?? 0) > 1)
            throw new InvalidOperationException("Sequence contains more than one element");
            
        return response.Data.First();
    }

    public async Task<Donation?> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 2;
        
        var response = await _givingService.ListDonationsAsync(parameters, cancellationToken);
        
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
        
        var response = await _givingService.ListDonationsAsync(parameters, cancellationToken);
        return response.Meta?.TotalCount ?? 0;
    }

    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 1;
        
        var response = await _givingService.ListDonationsAsync(parameters, cancellationToken);
        return response.Data.Any();
    }

    public async Task<bool> AnyAsync(Expression<Func<Donation, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var contextWithPredicate = Where(predicate);
        return await contextWithPredicate.AnyAsync(cancellationToken);
    }

    #endregion

    #region Specialized Giving Operations

    public IGivingFluentContext ByFund(string fundId)
    {
        if (string.IsNullOrWhiteSpace(fundId))
            throw new ArgumentException("Fund ID cannot be null or empty", nameof(fundId));

        // Note: Fund filtering might need to be done through query parameters or relations
        // This is a placeholder implementation that adds the fund filter via query parameters
        _queryBuilder.WithParameter("filter[fund_id]", fundId);
        return this;
    }

    public IGivingFluentContext ByPerson(string personId)
    {
        if (string.IsNullOrWhiteSpace(personId))
            throw new ArgumentException("Person ID cannot be null or empty", nameof(personId));

        return Where(d => d.PersonId == personId);
    }

    public IGivingFluentContext ByDateRange(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
            throw new ArgumentException("Start date cannot be after end date", nameof(startDate));

        return Where(d => d.ReceivedAt >= startDate && d.ReceivedAt <= endDate);
    }

    public IGivingFluentContext WithMinimumAmount(long minimumAmount)
    {
        if (minimumAmount < 0)
            throw new ArgumentException("Minimum amount cannot be negative", nameof(minimumAmount));

        return Where(d => d.AmountCents >= minimumAmount);
    }

    public async Task<long> TotalAmountAsync(CancellationToken cancellationToken = default)
    {
        var donations = await GetAllAsync(cancellationToken: cancellationToken);
        return donations.Sum(d => d.AmountCents);
    }

    #endregion
}