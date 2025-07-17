using System.Linq.Expressions;
using PlanningCenter.Api.Client.Fluent.QueryBuilder;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.Fluent;
using PlanningCenter.Api.Client.Models.Registrations;
using PlanningCenter.Api.Client.Models.Requests;

namespace PlanningCenter.Api.Client.Fluent;

/// <summary>
/// Fluent API context for Planning Center Registrations.
/// Provides a LINQ-like interface for querying and filtering signups.
/// </summary>
public class RegistrationsFluentContext : IRegistrationsFluentContext
{
    private readonly IRegistrationsService _registrationsService;
    private readonly FluentQueryBuilder<Signup> _queryBuilder = new();

    public RegistrationsFluentContext(IRegistrationsService registrationsService)
    {
        _registrationsService = registrationsService ?? throw new ArgumentNullException(nameof(registrationsService));
    }

    #region Query Building Methods

    public IRegistrationsFluentContext Where(Expression<Func<Signup, bool>> predicate)
    {
        _queryBuilder.Where(predicate);
        return this;
    }

    public IRegistrationsFluentContext Include(Expression<Func<Signup, object>> include)
    {
        _queryBuilder.Include(include);
        return this;
    }

    public IRegistrationsFluentContext OrderBy(Expression<Func<Signup, object>> orderBy)
    {
        _queryBuilder.OrderBy(orderBy);
        return this;
    }

    public IRegistrationsFluentContext OrderByDescending(Expression<Func<Signup, object>> orderBy)
    {
        _queryBuilder.OrderByDescending(orderBy);
        return this;
    }

    public IRegistrationsFluentContext ThenBy(Expression<Func<Signup, object>> thenBy)
    {
        _queryBuilder.ThenBy(thenBy);
        return this;
    }

    public IRegistrationsFluentContext ThenByDescending(Expression<Func<Signup, object>> thenBy)
    {
        _queryBuilder.ThenByDescending(thenBy);
        return this;
    }

    #endregion

    #region Execution Methods

    public async Task<Signup?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID cannot be null or empty", nameof(id));

        return await _registrationsService.GetSignupAsync(id, cancellationToken);
    }

    public async Task<IPagedResponse<Signup>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = pageSize;
        return await _registrationsService.ListSignupsAsync(parameters, cancellationToken);
    }

    public async Task<IPagedResponse<Signup>> GetPageAsync(int page, int pageSize = 25, CancellationToken cancellationToken = default)
    {
        if (page < 1) throw new ArgumentException("Page must be 1 or greater", nameof(page));
        if (pageSize < 1) throw new ArgumentException("Page size must be 1 or greater", nameof(pageSize));

        var parameters = _queryBuilder.Build();
        parameters.PerPage = pageSize;
        parameters.Offset = (page - 1) * pageSize;
        
        return await _registrationsService.ListSignupsAsync(parameters, cancellationToken);
    }

    public async Task<IReadOnlyList<Signup>> GetAllAsync(PaginationOptions? options = null, CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        return await _registrationsService.GetAllSignupsAsync(parameters, options, cancellationToken);
    }

    public IAsyncEnumerable<Signup> AsAsyncEnumerable(PaginationOptions? options = null, CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        return _registrationsService.StreamSignupsAsync(parameters, options, cancellationToken);
    }

    #endregion

    #region LINQ-like Terminal Operations

    public async Task<Signup> FirstAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 1;
        
        var response = await _registrationsService.ListSignupsAsync(parameters, cancellationToken);
        
        if (!response.Data.Any())
            throw new InvalidOperationException("Sequence contains no elements");
            
        return response.Data.First();
    }

    public async Task<Signup?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 1;
        
        var response = await _registrationsService.ListSignupsAsync(parameters, cancellationToken);
        return response.Data.FirstOrDefault();
    }

    public async Task<Signup> FirstAsync(Expression<Func<Signup, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var contextWithPredicate = Where(predicate);
        return await contextWithPredicate.FirstAsync(cancellationToken);
    }

    public async Task<Signup?> FirstOrDefaultAsync(Expression<Func<Signup, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var contextWithPredicate = Where(predicate);
        return await contextWithPredicate.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Signup> SingleAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 2; // Get 2 to check if there's more than 1
        
        var response = await _registrationsService.ListSignupsAsync(parameters, cancellationToken);
        
        if (!response.Data.Any())
            throw new InvalidOperationException("Sequence contains no elements");
            
        if (response.Data.Count > 1 || (response.Meta?.TotalCount ?? 0) > 1)
            throw new InvalidOperationException("Sequence contains more than one element");
            
        return response.Data.First();
    }

    public async Task<Signup?> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 2; // Get 2 to check if there's more than 1
        
        var response = await _registrationsService.ListSignupsAsync(parameters, cancellationToken);
        
        if (!response.Data.Any())
            return null;
            
        if (response.Data.Count > 1 || (response.Meta?.TotalCount ?? 0) > 1)
            throw new InvalidOperationException("Sequence contains more than one element");
            
        return response.Data.First();
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 1; // We only need the count, not the data
        
        var response = await _registrationsService.ListSignupsAsync(parameters, cancellationToken);
        return response.Meta?.TotalCount ?? 0;
    }

    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 1; // We only need to know if there's at least one
        
        var response = await _registrationsService.ListSignupsAsync(parameters, cancellationToken);
        return response.Data.Any();
    }

    public async Task<bool> AnyAsync(Expression<Func<Signup, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var contextWithPredicate = Where(predicate);
        return await contextWithPredicate.AnyAsync(cancellationToken);
    }

    #endregion

    #region Signup Status Filters

    /// <summary>
    /// Filters signups to only include active (non-archived) signups.
    /// </summary>
    public IRegistrationsFluentContext Active()
    {
        return Where(s => !s.Archived);
    }

    /// <summary>
    /// Filters signups to only include archived signups.
    /// </summary>
    public IRegistrationsFluentContext Archived()
    {
        return Where(s => s.Archived);
    }

    /// <summary>
    /// Filters signups by status.
    /// </summary>
    public IRegistrationsFluentContext ByStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
            throw new ArgumentException("Status cannot be null or empty", nameof(status));

        return Where(s => s.Status == status);
    }

    #endregion

    #region Registration Timing Filters

    /// <summary>
    /// Filters signups to only include those currently open for registration.
    /// </summary>
    public IRegistrationsFluentContext OpenForRegistration()
    {
        var now = DateTime.UtcNow;
        return Where(s => s.OpenAt <= now && (s.CloseAt == null || s.CloseAt > now));
    }

    /// <summary>
    /// Filters signups to only include those that are closed for registration.
    /// </summary>
    public IRegistrationsFluentContext ClosedForRegistration()
    {
        var now = DateTime.UtcNow;
        return Where(s => s.CloseAt != null && s.CloseAt <= now);
    }

    /// <summary>
    /// Filters signups to only include those opening after the specified date.
    /// </summary>
    public IRegistrationsFluentContext OpeningAfter(DateTime date)
    {
        return Where(s => s.OpenAt != null && s.OpenAt > date);
    }

    /// <summary>
    /// Filters signups to only include those closing before the specified date.
    /// </summary>
    public IRegistrationsFluentContext ClosingBefore(DateTime date)
    {
        return Where(s => s.CloseAt != null && s.CloseAt < date);
    }

    /// <summary>
    /// Filters signups to only include those within the specified date range.
    /// </summary>
    public IRegistrationsFluentContext ByDateRange(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
            throw new ArgumentException("Start date must be before end date");

        return Where(s => (s.OpenAt == null || s.OpenAt >= startDate) && 
                         (s.CloseAt == null || s.CloseAt <= endDate));
    }

    #endregion

    #region Capacity and Waitlist Filters

    /// <summary>
    /// Filters signups to only include those with waitlist enabled.
    /// </summary>
    public IRegistrationsFluentContext WithWaitlistEnabled()
    {
        return Where(s => s.WaitlistEnabled);
    }

    /// <summary>
    /// Filters signups to only include those without waitlist.
    /// </summary>
    public IRegistrationsFluentContext WithoutWaitlist()
    {
        return Where(s => !s.WaitlistEnabled);
    }

    /// <summary>
    /// Filters signups to only include those with a registration limit.
    /// </summary>
    public IRegistrationsFluentContext WithRegistrationLimit()
    {
        return Where(s => s.RegistrationLimit != null && s.RegistrationLimit > 0);
    }

    /// <summary>
    /// Filters signups to only include those without a registration limit (unlimited).
    /// </summary>
    public IRegistrationsFluentContext Unlimited()
    {
        return Where(s => s.RegistrationLimit == null || s.RegistrationLimit <= 0);
    }

    /// <summary>
    /// Filters signups to only include those with registrations.
    /// </summary>
    public IRegistrationsFluentContext WithRegistrations()
    {
        return Where(s => s.RegistrationCount > 0);
    }

    /// <summary>
    /// Filters signups to only include those with people on the waitlist.
    /// </summary>
    public IRegistrationsFluentContext WithWaitlist()
    {
        return Where(s => s.WaitlistCount > 0);
    }

    /// <summary>
    /// Filters signups to only include those with minimum registration count.
    /// </summary>
    public IRegistrationsFluentContext WithMinimumRegistrations(int minimumCount)
    {
        if (minimumCount < 0)
            throw new ArgumentException("Minimum count cannot be negative", nameof(minimumCount));

        return Where(s => s.RegistrationCount >= minimumCount);
    }

    /// <summary>
    /// Filters signups to only include those that are full (at registration limit).
    /// </summary>
    public IRegistrationsFluentContext Full()
    {
        return Where(s => s.RegistrationLimit != null && s.RegistrationCount >= s.RegistrationLimit);
    }

    /// <summary>
    /// Filters signups to only include those that have available spots.
    /// </summary>
    public IRegistrationsFluentContext HasAvailableSpots()
    {
        return Where(s => s.RegistrationLimit == null || s.RegistrationCount < s.RegistrationLimit);
    }

    #endregion

    #region Approval and Requirements

    /// <summary>
    /// Filters signups to only include those requiring approval.
    /// </summary>
    public IRegistrationsFluentContext RequiringApproval()
    {
        return Where(s => s.RequiresApproval);
    }

    /// <summary>
    /// Filters signups to only include those not requiring approval.
    /// </summary>
    public IRegistrationsFluentContext NotRequiringApproval()
    {
        return Where(s => !s.RequiresApproval);
    }

    #endregion

    #region Association Filters

    /// <summary>
    /// Filters signups by category ID.
    /// </summary>
    public IRegistrationsFluentContext ByCategory(string categoryId)
    {
        if (string.IsNullOrWhiteSpace(categoryId))
            throw new ArgumentException("Category ID cannot be null or empty", nameof(categoryId));

        return Where(s => s.CategoryId == categoryId);
    }

    /// <summary>
    /// Filters signups by campus ID.
    /// </summary>
    public IRegistrationsFluentContext ByCampus(string campusId)
    {
        if (string.IsNullOrWhiteSpace(campusId))
            throw new ArgumentException("Campus ID cannot be null or empty", nameof(campusId));

        return Where(s => s.CampusId == campusId);
    }

    /// <summary>
    /// Filters signups by location ID.
    /// </summary>
    public IRegistrationsFluentContext ByLocation(string locationId)
    {
        if (string.IsNullOrWhiteSpace(locationId))
            throw new ArgumentException("Location ID cannot be null or empty", nameof(locationId));

        return Where(s => s.SignupLocationId == locationId);
    }

    #endregion

    #region Search Filters

    /// <summary>
    /// Filters signups to only include those containing the specified text in the name.
    /// </summary>
    public IRegistrationsFluentContext ByNameContains(string nameFragment)
    {
        if (string.IsNullOrWhiteSpace(nameFragment))
            throw new ArgumentException("Name fragment cannot be null or empty", nameof(nameFragment));

        return Where(s => s.Name.Contains(nameFragment));
    }

    /// <summary>
    /// Filters signups to only include those containing the specified text in the description.
    /// </summary>
    public IRegistrationsFluentContext ByDescriptionContains(string descriptionFragment)
    {
        if (string.IsNullOrWhiteSpace(descriptionFragment))
            throw new ArgumentException("Description fragment cannot be null or empty", nameof(descriptionFragment));

        return Where(s => s.Description != null && s.Description.Contains(descriptionFragment));
    }

    #endregion

    #region Specialized Operations

    /// <summary>
    /// Gets the total registration count across all signups in the current query.
    /// </summary>
    public async Task<int> TotalRegistrationsAsync(CancellationToken cancellationToken = default)
    {
        var signups = await GetAllAsync(null, cancellationToken);
        return signups.Sum(s => s.RegistrationCount);
    }

    /// <summary>
    /// Gets the total waitlist count across all signups in the current query.
    /// </summary>
    public async Task<int> TotalWaitlistAsync(CancellationToken cancellationToken = default)
    {
        var signups = await GetAllAsync(null, cancellationToken);
        return signups.Sum(s => s.WaitlistCount);
    }

    /// <summary>
    /// Gets the average registration count per signup in the current query.
    /// </summary>
    public async Task<double> AverageRegistrationsAsync(CancellationToken cancellationToken = default)
    {
        var signups = await GetAllAsync(null, cancellationToken);
        return signups.Any() ? signups.Average(s => s.RegistrationCount) : 0;
    }

    #endregion
}