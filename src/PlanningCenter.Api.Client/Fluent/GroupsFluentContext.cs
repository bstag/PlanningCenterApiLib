using System.Linq.Expressions;
using PlanningCenter.Api.Client.Fluent.QueryBuilder;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Fluent;
using PlanningCenter.Api.Client.Models.Groups;

namespace PlanningCenter.Api.Client.Fluent;

/// <summary>
/// Fluent API implementation for the Groups module.
/// Provides LINQ-like syntax for querying and manipulating groups data with built-in pagination support.
/// </summary>
public class GroupsFluentContext : IGroupsFluentContext
{
    private readonly IGroupsService _groupsService;
    private readonly FluentQueryBuilder<Group> _queryBuilder = new();

    public GroupsFluentContext(IGroupsService groupsService)
    {
        _groupsService = groupsService ?? throw new ArgumentNullException(nameof(groupsService));
    }

    #region Query Building Methods

    public IGroupsFluentContext Where(Expression<Func<Group, bool>> predicate)
    {
        _queryBuilder.Where(predicate);
        return this;
    }

    public IGroupsFluentContext Include(Expression<Func<Group, object>> include)
    {
        _queryBuilder.Include(include);
        return this;
    }

    public IGroupsFluentContext OrderBy(Expression<Func<Group, object>> orderBy)
    {
        _queryBuilder.OrderBy(orderBy);
        return this;
    }

    public IGroupsFluentContext OrderByDescending(Expression<Func<Group, object>> orderBy)
    {
        _queryBuilder.OrderByDescending(orderBy);
        return this;
    }

    public IGroupsFluentContext ThenBy(Expression<Func<Group, object>> thenBy)
    {
        _queryBuilder.ThenBy(thenBy);
        return this;
    }

    public IGroupsFluentContext ThenByDescending(Expression<Func<Group, object>> thenBy)
    {
        _queryBuilder.ThenByDescending(thenBy);
        return this;
    }

    #endregion

    #region Execution Methods

    public async Task<Group?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID cannot be null or empty", nameof(id));

        return await _groupsService.GetGroupAsync(id, cancellationToken);
    }

    public async Task<IPagedResponse<Group>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = pageSize;
        
        return await _groupsService.ListGroupsAsync(parameters, cancellationToken);
    }

    public async Task<IPagedResponse<Group>> GetPageAsync(int page, int pageSize = 25, CancellationToken cancellationToken = default)
    {
        if (page < 1) throw new ArgumentException("Page must be 1 or greater", nameof(page));
        if (pageSize < 1) throw new ArgumentException("Page size must be 1 or greater", nameof(pageSize));

        var parameters = _queryBuilder.Build();
        parameters.PerPage = pageSize;
        parameters.Offset = (page - 1) * pageSize;
        
        return await _groupsService.ListGroupsAsync(parameters, cancellationToken);
    }

    public async Task<IReadOnlyList<Group>> GetAllAsync(PaginationOptions? options = null, CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        return await _groupsService.GetAllGroupsAsync(parameters, options, cancellationToken);
    }

    public IAsyncEnumerable<Group> AsAsyncEnumerable(PaginationOptions? options = null, CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        return _groupsService.StreamGroupsAsync(parameters, options, cancellationToken);
    }

    #endregion

    #region LINQ-like Terminal Operations

    public async Task<Group> FirstAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 1;
        
        var response = await _groupsService.ListGroupsAsync(parameters, cancellationToken);
        
        if (!response.Data.Any())
            throw new InvalidOperationException("Sequence contains no elements");
            
        return response.Data.First();
    }

    public async Task<Group?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 1;
        
        var response = await _groupsService.ListGroupsAsync(parameters, cancellationToken);
        return response.Data.FirstOrDefault();
    }

    public async Task<Group> FirstAsync(Expression<Func<Group, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var contextWithPredicate = Where(predicate);
        return await contextWithPredicate.FirstAsync(cancellationToken);
    }

    public async Task<Group?> FirstOrDefaultAsync(Expression<Func<Group, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var contextWithPredicate = Where(predicate);
        return await contextWithPredicate.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Group> SingleAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 2;
        
        var response = await _groupsService.ListGroupsAsync(parameters, cancellationToken);
        
        if (!response.Data.Any())
            throw new InvalidOperationException("Sequence contains no elements");
            
        if (response.Data.Count > 1 || (response.Meta?.TotalCount ?? 0) > 1)
            throw new InvalidOperationException("Sequence contains more than one element");
            
        return response.Data.First();
    }

    public async Task<Group?> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 2;
        
        var response = await _groupsService.ListGroupsAsync(parameters, cancellationToken);
        
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
        
        var response = await _groupsService.ListGroupsAsync(parameters, cancellationToken);
        return response.Meta?.TotalCount ?? 0;
    }

    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 1;
        
        var response = await _groupsService.ListGroupsAsync(parameters, cancellationToken);
        return response.Data.Any();
    }

    public async Task<bool> AnyAsync(Expression<Func<Group, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var contextWithPredicate = Where(predicate);
        return await contextWithPredicate.AnyAsync(cancellationToken);
    }

    #endregion

    #region Specialized Groups Operations

    public IGroupsFluentContext ByGroupType(string groupTypeId)
    {
        if (string.IsNullOrWhiteSpace(groupTypeId))
            throw new ArgumentException("Group Type ID cannot be null or empty", nameof(groupTypeId));

        return Where(g => g.GroupTypeId == groupTypeId);
    }

    public IGroupsFluentContext ByLocation(string locationId)
    {
        if (string.IsNullOrWhiteSpace(locationId))
            throw new ArgumentException("Location ID cannot be null or empty", nameof(locationId));

        return Where(g => g.LocationId == locationId);
    }

    public IGroupsFluentContext Active()
    {
        return Where(g => g.ArchivedAt == null);
    }

    public IGroupsFluentContext Archived()
    {
        return Where(g => g.ArchivedAt != null);
    }

    public IGroupsFluentContext WithMinimumMembers(int minimumCount)
    {
        if (minimumCount < 0)
            throw new ArgumentException("Minimum count cannot be negative", nameof(minimumCount));

        return Where(g => g.MembershipsCount >= minimumCount);
    }

    public IGroupsFluentContext WithMaximumMembers(int maximumCount)
    {
        if (maximumCount < 0)
            throw new ArgumentException("Maximum count cannot be negative", nameof(maximumCount));

        return Where(g => g.MembershipsCount <= maximumCount);
    }

    public IGroupsFluentContext WithChatEnabled()
    {
        return Where(g => g.ChatEnabled);
    }

    public IGroupsFluentContext WithVirtualMeeting()
    {
        return Where(g => !string.IsNullOrEmpty(g.VirtualLocationUrl));
    }

    public IGroupsFluentContext ByNameContains(string nameFragment)
    {
        if (string.IsNullOrWhiteSpace(nameFragment))
            throw new ArgumentException("Name fragment cannot be null or empty", nameof(nameFragment));

        return Where(g => g.Name.Contains(nameFragment));
    }

    #endregion
}