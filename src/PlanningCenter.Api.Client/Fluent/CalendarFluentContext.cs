using System.Linq.Expressions;
using PlanningCenter.Api.Client.Fluent.QueryBuilder;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Calendar;
using PlanningCenter.Api.Client.Models.Fluent;

namespace PlanningCenter.Api.Client.Fluent;

/// <summary>
/// Fluent API implementation for the Calendar module.
/// Provides LINQ-like syntax for querying and manipulating calendar data with built-in pagination support.
/// </summary>
public class CalendarFluentContext : ICalendarFluentContext
{
    private readonly ICalendarService _calendarService;
    private readonly FluentQueryBuilder<Event> _queryBuilder = new();

    public CalendarFluentContext(ICalendarService calendarService)
    {
        _calendarService = calendarService ?? throw new ArgumentNullException(nameof(calendarService));
    }

    #region Query Building Methods

    public ICalendarFluentContext Where(Expression<Func<Event, bool>> predicate)
    {
        _queryBuilder.Where(predicate);
        return this;
    }

    public ICalendarFluentContext Include(Expression<Func<Event, object>> include)
    {
        _queryBuilder.Include(include);
        return this;
    }

    public ICalendarFluentContext OrderBy(Expression<Func<Event, object>> orderBy)
    {
        _queryBuilder.OrderBy(orderBy);
        return this;
    }

    public ICalendarFluentContext OrderByDescending(Expression<Func<Event, object>> orderBy)
    {
        _queryBuilder.OrderByDescending(orderBy);
        return this;
    }

    public ICalendarFluentContext ThenBy(Expression<Func<Event, object>> thenBy)
    {
        _queryBuilder.ThenBy(thenBy);
        return this;
    }

    public ICalendarFluentContext ThenByDescending(Expression<Func<Event, object>> thenBy)
    {
        _queryBuilder.ThenByDescending(thenBy);
        return this;
    }

    #endregion

    #region Execution Methods

    public async Task<Event?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID cannot be null or empty", nameof(id));

        return await _calendarService.GetEventAsync(id, cancellationToken);
    }

    public async Task<IPagedResponse<Event>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = pageSize;
        
        return await _calendarService.ListEventsAsync(parameters, cancellationToken);
    }

    public async Task<IPagedResponse<Event>> GetPageAsync(int page, int pageSize = 25, CancellationToken cancellationToken = default)
    {
        if (page < 1) throw new ArgumentException("Page must be 1 or greater", nameof(page));
        if (pageSize < 1) throw new ArgumentException("Page size must be 1 or greater", nameof(pageSize));

        var parameters = _queryBuilder.Build();
        parameters.PerPage = pageSize;
        parameters.Offset = (page - 1) * pageSize;
        
        return await _calendarService.ListEventsAsync(parameters, cancellationToken);
    }

    public async Task<IReadOnlyList<Event>> GetAllAsync(PaginationOptions? options = null, CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        return await _calendarService.GetAllEventsAsync(parameters, options, cancellationToken);
    }

    public IAsyncEnumerable<Event> AsAsyncEnumerable(PaginationOptions? options = null, CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        return _calendarService.StreamEventsAsync(parameters, options, cancellationToken);
    }

    #endregion

    #region LINQ-like Terminal Operations

    public async Task<Event> FirstAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 1;
        
        var response = await _calendarService.ListEventsAsync(parameters, cancellationToken);
        
        if (!response.Data.Any())
            throw new InvalidOperationException("Sequence contains no elements");
            
        return response.Data.First();
    }

    public async Task<Event?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 1;
        
        var response = await _calendarService.ListEventsAsync(parameters, cancellationToken);
        return response.Data.FirstOrDefault();
    }

    public async Task<Event> FirstAsync(Expression<Func<Event, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var contextWithPredicate = Where(predicate);
        return await contextWithPredicate.FirstAsync(cancellationToken);
    }

    public async Task<Event?> FirstOrDefaultAsync(Expression<Func<Event, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var contextWithPredicate = Where(predicate);
        return await contextWithPredicate.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 1;
        
        var response = await _calendarService.ListEventsAsync(parameters, cancellationToken);
        return response.Meta?.TotalCount ?? 0;
    }

    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 1;
        
        var response = await _calendarService.ListEventsAsync(parameters, cancellationToken);
        return response.Data.Any();
    }

    public async Task<bool> AnyAsync(Expression<Func<Event, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var contextWithPredicate = Where(predicate);
        return await contextWithPredicate.AnyAsync(cancellationToken);
    }

    #endregion

    #region Specialized Calendar Operations

    public ICalendarFluentContext ByDateRange(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
            throw new ArgumentException("Start date cannot be after end date", nameof(startDate));

        return Where(e => e.StartsAt >= startDate && e.EndsAt <= endDate);
    }

    public ICalendarFluentContext Today()
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);
        
        return Where(e => e.StartsAt >= today && e.StartsAt < tomorrow);
    }

    public ICalendarFluentContext ThisWeek()
    {
        var today = DateTime.Today;
        var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
        var endOfWeek = startOfWeek.AddDays(7);
        
        return Where(e => e.StartsAt >= startOfWeek && e.StartsAt < endOfWeek);
    }

    public ICalendarFluentContext ThisMonth()
    {
        var today = DateTime.Today;
        var startOfMonth = new DateTime(today.Year, today.Month, 1);
        var endOfMonth = startOfMonth.AddMonths(1);
        
        return Where(e => e.StartsAt >= startOfMonth && e.StartsAt < endOfMonth);
    }

    public ICalendarFluentContext Upcoming()
    {
        var now = DateTime.Now;
        
        return Where(e => e.StartsAt > now);
    }

    #endregion
}