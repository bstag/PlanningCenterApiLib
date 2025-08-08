using Microsoft.Extensions.Logging;
using PlanningCenter.Api.Client.Mapping.Groups;
using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Exceptions;
using PlanningCenter.Api.Client.Models.JsonApi;
using PlanningCenter.Api.Client.Models.JsonApi.Groups;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Models.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace PlanningCenter.Api.Client.Services;

/// <summary>
/// Service implementation for the Planning Center Groups module.
/// Follows Single Responsibility Principle - handles only Groups API operations.
/// Follows Dependency Inversion Principle - depends on abstractions.
/// </summary>
public class GroupsService : ServiceBase, IGroupsService
{
    private const string BaseEndpoint = "/groups/v2";

    public GroupsService(IApiConnection apiConnection, ILogger<GroupsService> logger)
        : base(logger, apiConnection)
    {
    }

    // Group management

    /// <summary>
    /// Gets a single group by ID.
    /// </summary>
    public async Task<Group?> GetGroupAsync(string id, CancellationToken cancellationToken = default)
    {
        ValidateNotNullOrEmpty(id, nameof(id));

        return await ExecuteGetAsync(
            async () =>
            {
                var response = await ApiConnection.GetAsync<JsonApiSingleResponse<GroupDto>>(
                    $"{BaseEndpoint}/groups/{id}", cancellationToken);

                if (response?.Data == null)
                {
                    throw new PlanningCenterApiNotFoundException($"Group with ID {id} not found");
                }

                return GroupMapper.MapToDomain(response.Data);
            },
            "GetGroup",
            id,
            cancellationToken);
    }

    /// <summary>
    /// Lists groups with optional filtering, sorting, and pagination.
    /// </summary>
    public async Task<IPagedResponse<Group>> ListGroupsAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(
            async () =>
            {
                var response = await ApiConnection.GetPagedAsync<GroupDto>(
                    $"{BaseEndpoint}/groups", parameters, cancellationToken);

                var groups = response.Data.Select(GroupMapper.MapToDomain).ToList();

                return new PagedResponse<Group>
                {
                    Data = groups,
                    Meta = response.Meta,
                    Links = response.Links
                };
            },
            "ListGroups",
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Lists groups with optional filtering, sorting, and pagination.
    /// </summary>
    public async Task<IPagedResponse<Group>> ListGroupsWithLoggingAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Listing groups with parameters: {@Parameters}", parameters);

        try
        {
            var response = await ApiConnection.GetPagedAsync<GroupDto>(
                $"{BaseEndpoint}/groups", parameters, cancellationToken);

            var groups = response.Data.Select(GroupMapper.MapToDomain).ToList();

            Logger.LogDebug("Successfully retrieved {Count} groups", groups.Count);

            return new PagedResponse<Group>
            {
                Data = groups,
                Meta = response.Meta,
                Links = response.Links
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error listing groups");
            throw;
        }
    }

    /// <summary>
    /// Creates a new group.
    /// </summary>
    public async Task<Group> CreateGroupAsync(GroupCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Group name is required.", nameof(request));

        if (string.IsNullOrWhiteSpace(request.GroupTypeId))
            throw new ArgumentException("Group type ID is required.", nameof(request));

        Logger.LogDebug("Creating group: {GroupName}", request.Name);

        try
        {
            var jsonApiRequest = GroupMapper.MapCreateRequestToJsonApi(request);

            var response = await ApiConnection.PostAsync<JsonApiSingleResponse<GroupDto>>(
                $"{BaseEndpoint}/groups", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
                throw new PlanningCenterApiGeneralException("Failed to create group - no data returned");

            var group = GroupMapper.MapToDomain(response.Data);
            Logger.LogInformation("Successfully created group: {GroupName} (ID: {GroupId})", group.Name, group.Id);
            return group;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating group: {GroupName}", request.Name);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing group.
    /// </summary>
    public async Task<Group> UpdateGroupAsync(string id, GroupUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Group ID cannot be null or empty.", nameof(id));

        if (request == null)
            throw new ArgumentNullException(nameof(request));

        Logger.LogDebug("Updating group with ID: {GroupId}", id);

        try
        {
            var jsonApiRequest = GroupMapper.MapUpdateRequestToJsonApi(request);

            var response = await ApiConnection.PatchAsync<JsonApiSingleResponse<GroupDto>>(
                $"{BaseEndpoint}/groups/{id}", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
                throw new PlanningCenterApiGeneralException("Failed to update group - no data returned");

            var group = GroupMapper.MapToDomain(response.Data);
            Logger.LogInformation("Successfully updated group: {GroupName} (ID: {GroupId})", group.Name, group.Id);
            return group;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating group with ID: {GroupId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a group.
    /// </summary>
    public async Task DeleteGroupAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Group ID cannot be null or empty.", nameof(id));

        Logger.LogDebug("Deleting group with ID: {GroupId}", id);

        try
        {
            await ApiConnection.DeleteAsync($"{BaseEndpoint}/groups/{id}", cancellationToken);
            Logger.LogInformation("Successfully deleted group with ID: {GroupId}", id);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting group with ID: {GroupId}", id);
            throw;
        }
    }

    // Pagination helpers (following PeopleService pattern)

    /// <summary>
    /// Gets all groups matching the specified criteria.
    /// This method automatically handles pagination behind the scenes.
    /// </summary>
    public async Task<IReadOnlyList<Group>> GetAllGroupsAsync(
        QueryParameters? parameters = null,
        PaginationOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Getting all groups with parameters: {@Parameters}", parameters);

        var allGroups = new List<Group>();
        var currentParameters = parameters ?? new QueryParameters();
        var pageSize = options?.PageSize ?? 25;
        currentParameters.PerPage = pageSize;

        try
        {
            IPagedResponse<Group> response;
            do
            {
                response = await ListGroupsAsync(currentParameters, cancellationToken);
                allGroups.AddRange(response.Data);

                // Update parameters for next page
                if (response.Links?.Next != null)
                {
                    currentParameters.Offset = (currentParameters.Offset ?? 0) + pageSize;
                }
            }
            while (response.Links?.Next != null && !cancellationToken.IsCancellationRequested);

            Logger.LogDebug("Successfully retrieved all {Count} groups", allGroups.Count);
            return allGroups.AsReadOnly();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting all groups");
            throw;
        }
    }

    /// <summary>
    /// Streams groups matching the specified criteria for memory-efficient processing.
    /// </summary>
    public async IAsyncEnumerable<Group> StreamGroupsAsync(
        QueryParameters? parameters = null,
        PaginationOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Streaming groups with parameters: {@Parameters}", parameters);

        var currentParameters = parameters ?? new QueryParameters();
        var pageSize = options?.PageSize ?? 25;
        currentParameters.PerPage = pageSize;

        try
        {
            IPagedResponse<Group> response;
            do
            {
                response = await ListGroupsAsync(currentParameters, cancellationToken);

                foreach (var group in response.Data)
                {
                    yield return group;
                }

                // Update parameters for next page
                if (response.Links?.Next != null)
                {
                    currentParameters.Offset = (currentParameters.Offset ?? 0) + pageSize;
                }
            }
            while (response.Links?.Next != null && !cancellationToken.IsCancellationRequested);
        }
        finally
        {
            Logger.LogDebug("Finished streaming groups");
        }
    }

    // Group type management

    /// <summary>
    /// Lists group types with optional filtering, sorting, and pagination.
    /// </summary>
    public async Task<IPagedResponse<GroupType>> ListGroupTypesAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Listing group types with parameters: {@Parameters}", parameters);

        try
        {
            var response = await ApiConnection.GetPagedAsync<GroupTypeDto>(
                $"{BaseEndpoint}/group_types", parameters, cancellationToken);

            var groupTypes = response.Data.Select(GroupMapper.MapGroupTypeToDomain).ToList();

            Logger.LogDebug("Successfully retrieved {Count} group types", groupTypes.Count);

            return new PagedResponse<GroupType>
            {
                Data = groupTypes,
                Meta = response.Meta,
                Links = response.Links
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error listing group types");
            throw;
        }
    }

    /// <summary>
    /// Gets a single group type by ID.
    /// </summary>
    public async Task<GroupType?> GetGroupTypeAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Group type ID cannot be null or empty.", nameof(id));

        Logger.LogDebug("Getting group type with ID: {GroupTypeId}", id);

        try
        {
            var response = await ApiConnection.GetAsync<JsonApiSingleResponse<GroupTypeDto>>(
                $"{BaseEndpoint}/group_types/{id}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogDebug("Group type with ID {GroupTypeId} not found", id);
                return null;
            }

            var groupType = GroupMapper.MapGroupTypeToDomain(response.Data);
            Logger.LogDebug("Successfully retrieved group type: {GroupTypeName} (ID: {GroupTypeId})", groupType.Name, groupType.Id);
            return groupType;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            Logger.LogDebug("Group type with ID {GroupTypeId} not found", id);
            return null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting group type with ID: {GroupTypeId}", id);
            throw;
        }
    }

    // Membership management

    /// <summary>
    /// Lists memberships for a specific group.
    /// </summary>
    public async Task<IPagedResponse<Membership>> ListGroupMembershipsAsync(string groupId, QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(groupId))
            throw new ArgumentException("Group ID cannot be null or empty.", nameof(groupId));

        Logger.LogDebug("Listing memberships for group {GroupId} with parameters: {@Parameters}", groupId, parameters);

        try
        {
            var response = await ApiConnection.GetPagedAsync<MembershipDto>(
                $"{BaseEndpoint}/groups/{groupId}/memberships", parameters, cancellationToken);

            var memberships = response.Data.Select(GroupMapper.MapMembershipToDomain).ToList();

            Logger.LogDebug("Successfully retrieved {Count} memberships for group {GroupId}", memberships.Count, groupId);

            return new PagedResponse<Membership>
            {
                Data = memberships,
                Meta = response.Meta,
                Links = response.Links
            };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error listing memberships for group {GroupId}", groupId);
            throw;
        }
    }

    /// <summary>
    /// Gets a single membership by ID.
    /// </summary>
    public async Task<Membership?> GetGroupMembershipAsync(string groupId, string membershipId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(groupId))
            throw new ArgumentException("Group ID cannot be null or empty.", nameof(groupId));

        if (string.IsNullOrWhiteSpace(membershipId))
            throw new ArgumentException("Membership ID cannot be null or empty.", nameof(membershipId));

        Logger.LogDebug("Getting membership {MembershipId} for group {GroupId}", membershipId, groupId);

        try
        {
            var response = await ApiConnection.GetAsync<JsonApiSingleResponse<MembershipDto>>(
                $"{BaseEndpoint}/groups/{groupId}/memberships/{membershipId}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogDebug("Membership {MembershipId} for group {GroupId} not found", membershipId, groupId);
                return null;
            }

            var membership = GroupMapper.MapMembershipToDomain(response.Data);
            Logger.LogDebug("Successfully retrieved membership: {MembershipRole} (ID: {MembershipId})", membership.Role, membership.Id);
            return membership;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            Logger.LogDebug("Membership {MembershipId} for group {GroupId} not found", membershipId, groupId);
            return null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting membership {MembershipId} for group {GroupId}", membershipId, groupId);
            throw;
        }
    }

    /// <summary>
    /// Creates a new group membership.
    /// </summary>
    public async Task<Membership> CreateGroupMembershipAsync(string groupId, MembershipCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(groupId))
            throw new ArgumentException("Group ID cannot be null or empty.", nameof(groupId));

        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.PersonId))
            throw new ArgumentException("Person ID is required.", nameof(request));

        Logger.LogDebug("Creating membership for person {PersonId} in group {GroupId}", request.PersonId, groupId);

        try
        {
            var jsonApiRequest = GroupMapper.MapMembershipCreateRequestToJsonApi(request);

            var response = await ApiConnection.PostAsync<JsonApiSingleResponse<MembershipDto>>(
                $"{BaseEndpoint}/groups/{groupId}/memberships", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
                throw new PlanningCenterApiGeneralException("Failed to create membership - no data returned");

            var membership = GroupMapper.MapMembershipToDomain(response.Data);
            Logger.LogInformation("Successfully created membership: {MembershipRole} (ID: {MembershipId})", membership.Role, membership.Id);
            return membership;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating membership for person {PersonId} in group {GroupId}", request.PersonId, groupId);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing group membership.
    /// </summary>
    public async Task<Membership> UpdateGroupMembershipAsync(string groupId, string membershipId, MembershipUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(groupId))
            throw new ArgumentException("Group ID cannot be null or empty.", nameof(groupId));

        if (string.IsNullOrWhiteSpace(membershipId))
            throw new ArgumentException("Membership ID cannot be null or empty.", nameof(membershipId));

        if (request == null)
            throw new ArgumentNullException(nameof(request));

        Logger.LogDebug("Updating membership {MembershipId} for group {GroupId}", membershipId, groupId);

        try
        {
            var jsonApiRequest = GroupMapper.MapMembershipUpdateRequestToJsonApi(request);

            var response = await ApiConnection.PatchAsync<JsonApiSingleResponse<MembershipDto>>(
                $"{BaseEndpoint}/groups/{groupId}/memberships/{membershipId}", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
                throw new PlanningCenterApiGeneralException("Failed to update membership - no data returned");

            var membership = GroupMapper.MapMembershipToDomain(response.Data);
            Logger.LogInformation("Successfully updated membership: {MembershipRole} (ID: {MembershipId})", membership.Role, membership.Id);
            return membership;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating membership {MembershipId} for group {GroupId}", membershipId, groupId);
            throw;
        }
    }

    /// <summary>
    /// Deletes a group membership.
    /// </summary>
    public async Task DeleteGroupMembershipAsync(string groupId, string membershipId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(groupId))
            throw new ArgumentException("Group ID cannot be null or empty.", nameof(groupId));

        if (string.IsNullOrWhiteSpace(membershipId))
            throw new ArgumentException("Membership ID cannot be null or empty.", nameof(membershipId));

        Logger.LogDebug("Deleting membership {MembershipId} from group {GroupId}", membershipId, groupId);

        try
        {
            await ApiConnection.DeleteAsync($"{BaseEndpoint}/groups/{groupId}/memberships/{membershipId}", cancellationToken);
            Logger.LogInformation("Successfully deleted membership {MembershipId} from group {GroupId}", membershipId, groupId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting membership {MembershipId} from group {GroupId}", membershipId, groupId);
            throw;
        }
    }
}