using System.Linq.Expressions;
using PlanningCenter.Api.Client.Fluent.QueryBuilder;
using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.CheckIns;


namespace PlanningCenter.Api.Client.Fluent;

/// <summary>
/// Fluent API implementation for the Check-Ins module.
/// Provides LINQ-like syntax for querying and manipulating check-in data with built-in pagination support.
/// </summary>
public class CheckInsFluentContext : ICheckInsFluentContext
{
    private readonly ICheckInsService _checkInsService;
    private readonly FluentQueryBuilder<CheckIn> _queryBuilder = new();

    public CheckInsFluentContext(ICheckInsService checkInsService)
    {
        _checkInsService = checkInsService ?? throw new ArgumentNullException(nameof(checkInsService));
    }

    #region Query Building Methods

    public ICheckInsFluentContext Where(Expression<Func<CheckIn, bool>> predicate)
    {
        _queryBuilder.Where(predicate);
        return this;
    }

    public ICheckInsFluentContext Include(Expression<Func<CheckIn, object>> include)
    {
        _queryBuilder.Include(include);
        return this;
    }

    public ICheckInsFluentContext OrderBy(Expression<Func<CheckIn, object>> orderBy)
    {
        _queryBuilder.OrderBy(orderBy);
        return this;
    }

    public ICheckInsFluentContext OrderByDescending(Expression<Func<CheckIn, object>> orderBy)
    {
        _queryBuilder.OrderByDescending(orderBy);
        return this;
    }

    public ICheckInsFluentContext ThenBy(Expression<Func<CheckIn, object>> thenBy)
    {
        _queryBuilder.ThenBy(thenBy);
        return this;
    }

    public ICheckInsFluentContext ThenByDescending(Expression<Func<CheckIn, object>> thenBy)
    {
        _queryBuilder.ThenByDescending(thenBy);
        return this;
    }

    #endregion

    #region Execution Methods

    public async Task<CheckIn?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID cannot be null or empty", nameof(id));

        return await _checkInsService.GetCheckInAsync(id, cancellationToken);
    }

    public async Task<IPagedResponse<CheckIn>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = pageSize;
        
        return await _checkInsService.ListCheckInsAsync(parameters, cancellationToken);
    }

    public async Task<IPagedResponse<CheckIn>> GetPageAsync(int page, int pageSize = 25, CancellationToken cancellationToken = default)
    {
        if (page < 1) throw new ArgumentException("Page must be 1 or greater", nameof(page));
        if (pageSize < 1) throw new ArgumentException("Page size must be 1 or greater", nameof(pageSize));

        var parameters = _queryBuilder.Build();
        parameters.PerPage = pageSize;
        parameters.Offset = (page - 1) * pageSize;
        
        return await _checkInsService.ListCheckInsAsync(parameters, cancellationToken);
    }

    public async Task<IReadOnlyList<CheckIn>> GetAllAsync(PaginationOptions? options = null, CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        return await _checkInsService.GetAllCheckInsAsync(parameters, options, cancellationToken);
    }

    public IAsyncEnumerable<CheckIn> AsAsyncEnumerable(PaginationOptions? options = null, CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        return _checkInsService.StreamCheckInsAsync(parameters, options, cancellationToken);
    }

    #endregion

    #region LINQ-like Terminal Operations

    public async Task<CheckIn> FirstAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 1;
        
        var response = await _checkInsService.ListCheckInsAsync(parameters, cancellationToken);
        
        if (!response.Data.Any())
            throw new InvalidOperationException("Sequence contains no elements");
            
        return response.Data.First();
    }

    public async Task<CheckIn?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 1;
        
        var response = await _checkInsService.ListCheckInsAsync(parameters, cancellationToken);
        return response.Data.FirstOrDefault();
    }

    public async Task<CheckIn> FirstAsync(Expression<Func<CheckIn, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var contextWithPredicate = Where(predicate);
        return await contextWithPredicate.FirstAsync(cancellationToken);
    }

    public async Task<CheckIn?> FirstOrDefaultAsync(Expression<Func<CheckIn, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var contextWithPredicate = Where(predicate);
        return await contextWithPredicate.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<CheckIn> SingleAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 2;
        
        var response = await _checkInsService.ListCheckInsAsync(parameters, cancellationToken);
        
        if (!response.Data.Any())
            throw new InvalidOperationException("Sequence contains no elements");
            
        if (response.Data.Count > 1 || (response.Meta?.TotalCount ?? 0) > 1)
            throw new InvalidOperationException("Sequence contains more than one element");
            
        return response.Data.First();
    }

    public async Task<CheckIn?> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 2;
        
        var response = await _checkInsService.ListCheckInsAsync(parameters, cancellationToken);
        
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
        
        var response = await _checkInsService.ListCheckInsAsync(parameters, cancellationToken);
        return response.Meta?.TotalCount ?? 0;
    }

    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 1;
        
        var response = await _checkInsService.ListCheckInsAsync(parameters, cancellationToken);
        return response.Data.Any();
    }

    public async Task<bool> AnyAsync(Expression<Func<CheckIn, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var contextWithPredicate = Where(predicate);
        return await contextWithPredicate.AnyAsync(cancellationToken);
    }

    public async Task<Dictionary<TKey, List<CheckIn>>> GroupByAsync<TKey>(Expression<Func<CheckIn, TKey>> keySelector, CancellationToken cancellationToken = default) where TKey : notnull
    {
        if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
        
        var allCheckIns = await GetAllAsync(cancellationToken: cancellationToken);
        var compiledSelector = keySelector.Compile();
        
        return allCheckIns
            .GroupBy(compiledSelector)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    public ICheckInsFluentContext Include(params string[] relationships)
    {
        _queryBuilder.Include(relationships);
        return this;
    }

    #endregion

    #region Specialized Check-Ins Operations

    public ICheckInsFluentContext ByPerson(string personId)
    {
        if (string.IsNullOrWhiteSpace(personId))
            throw new ArgumentException("Person ID cannot be null or empty", nameof(personId));

        return Where(c => c.PersonId == personId);
    }

    public ICheckInsFluentContext ByEvent(string eventId)
    {
        if (string.IsNullOrWhiteSpace(eventId))
            throw new ArgumentException("Event ID cannot be null or empty", nameof(eventId));

        return Where(c => c.EventId == eventId);
    }

    public ICheckInsFluentContext ByEventTime(string eventTimeId)
    {
        if (string.IsNullOrWhiteSpace(eventTimeId))
            throw new ArgumentException("Event Time ID cannot be null or empty", nameof(eventTimeId));

        return Where(c => c.EventTimeId == eventTimeId);
    }

    public ICheckInsFluentContext ByLocation(string locationId)
    {
        if (string.IsNullOrWhiteSpace(locationId))
            throw new ArgumentException("Location ID cannot be null or empty", nameof(locationId));

        return Where(c => c.LocationId == locationId);
    }

    public ICheckInsFluentContext ByDateRange(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
            throw new ArgumentException("Start date cannot be after end date", nameof(startDate));

        return Where(c => c.CreatedAt >= startDate && c.CreatedAt <= endDate);
    }

    public ICheckInsFluentContext Today()
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);
        
        return Where(c => c.CreatedAt >= today && c.CreatedAt < tomorrow);
    }

    public ICheckInsFluentContext ThisWeek()
    {
        var today = DateTime.Today;
        var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
        var endOfWeek = startOfWeek.AddDays(7);
        
        return Where(c => c.CreatedAt >= startOfWeek && c.CreatedAt < endOfWeek);
    }

    public ICheckInsFluentContext CheckedIn()
    {
        return Where(c => c.CheckedOutAt == null);
    }

    public ICheckInsFluentContext CheckedOut()
    {
        return Where(c => c.CheckedOutAt != null);
    }

    public ICheckInsFluentContext Confirmed()
    {
        return Where(c => c.ConfirmedAt != null);
    }

    public ICheckInsFluentContext Unconfirmed()
    {
        return Where(c => c.ConfirmedAt == null);
    }

    public ICheckInsFluentContext Guests()
    {
        return Where(c => c.OneTimeGuest);
    }

    public ICheckInsFluentContext Members()
    {
        return Where(c => !c.OneTimeGuest);
    }

    public ICheckInsFluentContext WithMedicalNotes()
    {
        return Where(c => !string.IsNullOrEmpty(c.MedicalNotes));
    }

    public ICheckInsFluentContext ByNameContains(string nameFragment)
    {
        if (string.IsNullOrWhiteSpace(nameFragment))
            throw new ArgumentException("Name fragment cannot be null or empty", nameof(nameFragment));

        return Where(c => c.FirstName.Contains(nameFragment) || c.LastName.Contains(nameFragment));
    }

    public ICheckInsFluentContext ByKind(string kind)
    {
        if (string.IsNullOrWhiteSpace(kind))
            throw new ArgumentException("Kind cannot be null or empty", nameof(kind));

        return Where(c => c.Kind == kind);
    }
    
    // Advanced aggregation methods
    
    public async Task<int> CountByEventAsync(string eventId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(eventId))
            throw new ArgumentException("Event ID cannot be null or empty", nameof(eventId));

        var contextWithFilter = ByEvent(eventId);
        return await contextWithFilter.CountAsync(cancellationToken);
    }

    public async Task<int> CountByLocationAsync(string locationId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(locationId))
            throw new ArgumentException("Location ID cannot be null or empty", nameof(locationId));

        var contextWithFilter = ByLocation(locationId);
        return await contextWithFilter.CountAsync(cancellationToken);
    }

    public async Task<int> CountCheckedInAsync(CancellationToken cancellationToken = default)
    {
        var contextWithFilter = CheckedIn();
        return await contextWithFilter.CountAsync(cancellationToken);
    }

    public async Task<int> CountCheckedOutAsync(CancellationToken cancellationToken = default)
    {
        var contextWithFilter = CheckedOut();
        return await contextWithFilter.CountAsync(cancellationToken);
    }

    public async Task<int> CountGuestsAsync(CancellationToken cancellationToken = default)
    {
        var contextWithFilter = Guests();
        return await contextWithFilter.CountAsync(cancellationToken);
    }

    public async Task<int> CountMembersAsync(CancellationToken cancellationToken = default)
    {
        var contextWithFilter = Members();
        return await contextWithFilter.CountAsync(cancellationToken);
    }

    public async Task<int> CountWithMedicalNotesAsync(CancellationToken cancellationToken = default)
    {
        var contextWithFilter = WithMedicalNotes();
        return await contextWithFilter.CountAsync(cancellationToken);
    }

    #endregion
}