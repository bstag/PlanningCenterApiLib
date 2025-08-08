
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Groups;
using PlanningCenter.Api.Client.Models.Requests;

namespace PlanningCenter.Api.Client.Abstractions;

/// <summary>
/// Service interface for the Planning Center Groups module.
/// Follows Interface Segregation Principle - focused on groups-related operations.
/// Provides comprehensive group management with built-in pagination support.
/// </summary>
public interface IGroupsService
{
    // Group management
    
    /// <summary>
    /// Gets a single group by ID.
    /// </summary>
    /// <param name="id">The group's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The group, or null if not found</returns>
    Task<Group?> GetGroupAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists groups with optional filtering, sorting, and pagination.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with groups</returns>
    Task<IPagedResponse<Group>> ListGroupsAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new group.
    /// </summary>
    /// <param name="request">The group creation request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created group</returns>
    Task<Group> CreateGroupAsync(GroupCreateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing group.
    /// </summary>
    /// <param name="id">The group's unique identifier</param>
    /// <param name="request">The group update request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The updated group</returns>
    Task<Group> UpdateGroupAsync(string id, GroupUpdateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a group.
    /// </summary>
    /// <param name="id">The group's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task DeleteGroupAsync(string id, CancellationToken cancellationToken = default);
    
    // Group type management
    
    /// <summary>
    /// Lists group types with optional filtering, sorting, and pagination.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with group types</returns>
    Task<IPagedResponse<GroupType>> ListGroupTypesAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a single group type by ID.
    /// </summary>
    /// <param name="id">The group type's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The group type, or null if not found</returns>
    Task<GroupType?> GetGroupTypeAsync(string id, CancellationToken cancellationToken = default);
    
    // Membership management
    
    /// <summary>
    /// Lists memberships for a specific group.
    /// </summary>
    /// <param name="groupId">The group's unique identifier</param>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with group memberships</returns>
    Task<IPagedResponse<Membership>> ListGroupMembershipsAsync(string groupId, QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a single membership by ID.
    /// </summary>
    /// <param name="groupId">The group's unique identifier</param>
    /// <param name="membershipId">The membership's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The membership, or null if not found</returns>
    Task<Membership?> GetGroupMembershipAsync(string groupId, string membershipId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new group membership.
    /// </summary>
    /// <param name="groupId">The group's unique identifier</param>
    /// <param name="request">The membership creation request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created membership</returns>
    Task<Membership> CreateGroupMembershipAsync(string groupId, MembershipCreateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing group membership.
    /// </summary>
    /// <param name="groupId">The group's unique identifier</param>
    /// <param name="membershipId">The membership's unique identifier</param>
    /// <param name="request">The membership update request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The updated membership</returns>
    Task<Membership> UpdateGroupMembershipAsync(string groupId, string membershipId, MembershipUpdateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a group membership.
    /// </summary>
    /// <param name="groupId">The group's unique identifier</param>
    /// <param name="membershipId">The membership's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task DeleteGroupMembershipAsync(string groupId, string membershipId, CancellationToken cancellationToken = default);
    
    // Pagination helpers that eliminate manual pagination logic
    
    /// <summary>
    /// Gets all groups matching the specified criteria.
    /// This method automatically handles pagination behind the scenes.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>All groups matching the criteria</returns>
    Task<IReadOnlyList<Group>> GetAllGroupsAsync(
        QueryParameters? parameters = null,
        PaginationOptions? options = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Streams groups matching the specified criteria for memory-efficient processing.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>An async enumerable that yields groups from all pages</returns>
    IAsyncEnumerable<Group> StreamGroupsAsync(
        QueryParameters? parameters = null,
        PaginationOptions? options = null,
        CancellationToken cancellationToken = default);
}