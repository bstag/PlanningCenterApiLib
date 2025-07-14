using PlanningCenter.Api.Client.Models.JsonApi.Groups;
using PlanningCenter.Api.Client.Models.Groups;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Models.JsonApi;
using PlanningCenter.Api.Client.Models.JsonApi.Core;
using System;

namespace PlanningCenter.Api.Client.Mapping.Groups;

/// <summary>
/// Mapper for converting between Group DTOs and domain models.
/// Follows DRY principle by centralizing all group mapping logic.
/// Follows Single Responsibility Principle - handles only group mapping.
/// </summary>
public static class GroupMapper
{
    /// <summary>
    /// Maps a GroupDto to a Group domain model.
    /// </summary>
    /// <param name="dto">The DTO to map from</param>
    /// <returns>The mapped domain model</returns>
    /// <exception cref="ArgumentNullException">Thrown when dto is null</exception>
    public static Group MapToDomain(GroupDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        return new Group
        {
            Id = dto.Id,
            Name = dto.Attributes.Name ?? string.Empty,
            Description = dto.Attributes.Description,
            ArchivedAt = dto.Attributes.ArchivedAt,
            ContactEmail = dto.Attributes.ContactEmail,
            Schedule = dto.Attributes.Schedule,
            LocationTypePreference = dto.Attributes.LocationTypePreference,
            VirtualLocationUrl = dto.Attributes.VirtualLocationUrl,
            EventsVisibility = dto.Attributes.EventsVisibility,
            MembershipsCount = dto.Attributes.MembershipsCount,
            MembersAreConfidential = dto.Attributes.MembersAreConfidential,
            LeadersCanSearchPeopleDatabase = dto.Attributes.LeadersCanSearchPeopleDatabase,
            CanCreateConversation = dto.Attributes.CanCreateConversation,
            ChatEnabled = dto.Attributes.ChatEnabled,
            PublicChurchCenterWebUrl = dto.Attributes.PublicChurchCenterWebUrl,
            HeaderImage = dto.Attributes.HeaderImage,
            WidgetStatus = dto.Attributes.WidgetStatus,
            GroupTypeId = dto.Relationships?.GroupType?.Id,
            LocationId = dto.Relationships?.Location?.Id,
            CreatedAt = dto.Attributes.CreatedAt,
            UpdatedAt = dto.Attributes.UpdatedAt,
            DataSource = "Groups"
        };
    }

    /// <summary>
    /// Maps a GroupCreateRequest to a JSON:API request.
    /// </summary>
    /// <param name="request">The request to map from</param>
    /// <returns>The mapped JSON:API request</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null</exception>
    public static JsonApiRequest<GroupCreateDto> MapCreateRequestToJsonApi(GroupCreateRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        return new JsonApiRequest<GroupCreateDto>
        {
            Data = new GroupCreateDto
            {
                Type = "Group",
                Attributes = new GroupCreateAttributes
                {
                    Name = request.Name,
                    Description = request.Description,
                    ContactEmail = request.ContactEmail,
                    Schedule = request.Schedule,
                    LocationTypePreference = request.LocationTypePreference,
                    VirtualLocationUrl = request.VirtualLocationUrl,
                    MembersAreConfidential = request.MembersAreConfidential,
                    ChatEnabled = request.ChatEnabled
                },
                Relationships = new GroupCreateRelationships
                {
                    GroupType = new RelationshipData
                    {
                        Type = "GroupType",
                        Id = request.GroupTypeId
                    }
                }
            }
        };
    }

    /// <summary>
    /// Maps a GroupUpdateRequest to a JSON:API request.
    /// </summary>
    /// <param name="request">The request to map from</param>
    /// <returns>The mapped JSON:API request</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null</exception>
    public static JsonApiRequest<GroupUpdateDto> MapUpdateRequestToJsonApi(GroupUpdateRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        return new JsonApiRequest<GroupUpdateDto>
        {
            Data = new GroupUpdateDto
            {
                Type = "Group",
                Attributes = new GroupUpdateAttributes
                {
                    Name = request.Name,
                    Description = request.Description,
                    ContactEmail = request.ContactEmail,
                    Schedule = request.Schedule,
                    VirtualLocationUrl = request.VirtualLocationUrl,
                    MembersAreConfidential = request.MembersAreConfidential,
                    ChatEnabled = request.ChatEnabled
                }
            }
        };
    }

    /// <summary>
    /// Maps a GroupTypeDto to a GroupType domain model.
    /// </summary>
    /// <param name="dto">The DTO to map from</param>
    /// <returns>The mapped domain model</returns>
    /// <exception cref="ArgumentNullException">Thrown when dto is null</exception>
    public static GroupType MapGroupTypeToDomain(GroupTypeDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        return new GroupType
        {
            Id = dto.Id,
            Name = dto.Attributes.Name ?? string.Empty,
            Description = dto.Attributes.Description,
            ChurchCenterVisible = dto.Attributes.ChurchCenterVisible,
            Color = dto.Attributes.Color,
            Position = dto.Attributes.Position,
            CreatedAt = dto.Attributes.CreatedAt,
            UpdatedAt = dto.Attributes.UpdatedAt,
            DataSource = "Groups"
        };
    }

    /// <summary>
    /// Maps a MembershipDto to a Membership domain model.
    /// </summary>
    /// <param name="dto">The DTO to map from</param>
    /// <returns>The mapped domain model</returns>
    /// <exception cref="ArgumentNullException">Thrown when dto is null</exception>
    public static Membership MapMembershipToDomain(MembershipDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        return new Membership
        {
            Id = dto.Id,
            Role = dto.Attributes.Role ?? string.Empty,
            JoinedAt = dto.Attributes.JoinedAt,
            CanCreateEvents = dto.Attributes.CanCreateEvents,
            CanEditGroup = dto.Attributes.CanEditGroup,
            CanManageMembers = dto.Attributes.CanManageMembers,
            CanViewMembers = dto.Attributes.CanViewMembers,
            PersonId = dto.Relationships?.Person?.Id ?? string.Empty,
            GroupId = dto.Relationships?.Group?.Id ?? string.Empty,
            CreatedAt = dto.Attributes.CreatedAt,
            UpdatedAt = dto.Attributes.UpdatedAt,
            DataSource = "Groups"
        };
    }

    /// <summary>
    /// Maps a MembershipCreateRequest to a JSON:API request.
    /// </summary>
    /// <param name="request">The request to map from</param>
    /// <returns>The mapped JSON:API request</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null</exception>
    public static JsonApiRequest<MembershipCreateDto> MapMembershipCreateRequestToJsonApi(MembershipCreateRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        return new JsonApiRequest<MembershipCreateDto>
        {
            Data = new MembershipCreateDto
            {
                Type = "Membership",
                Attributes = new MembershipCreateAttributes
                {
                    Role = request.Role,
                    CanCreateEvents = request.CanCreateEvents,
                    CanEditGroup = request.CanEditGroup,
                    CanManageMembers = request.CanManageMembers,
                    CanViewMembers = request.CanViewMembers
                },
                Relationships = new MembershipCreateRelationships
                {
                    Person = new RelationshipData
                    {
                        Type = "Person",
                        Id = request.PersonId
                    }
                }
            }
        };
    }

    /// <summary>
    /// Maps a MembershipUpdateRequest to a JSON:API request.
    /// </summary>
    /// <param name="request">The request to map from</param>
    /// <returns>The mapped JSON:API request</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null</exception>
    public static JsonApiRequest<MembershipUpdateDto> MapMembershipUpdateRequestToJsonApi(MembershipUpdateRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        return new JsonApiRequest<MembershipUpdateDto>
        {
            Data = new MembershipUpdateDto
            {
                Type = "Membership",
                Attributes = new MembershipUpdateAttributes
                {
                    Role = request.Role,
                    CanCreateEvents = request.CanCreateEvents,
                    CanEditGroup = request.CanEditGroup,
                    CanManageMembers = request.CanManageMembers,
                    CanViewMembers = request.CanViewMembers
                }
            }
        };
    }
}

// Supporting DTOs for create/update operations
public class GroupCreateDto
{
    public string Type { get; set; } = "Group";
    public GroupCreateAttributes Attributes { get; set; } = new();
    public GroupCreateRelationships? Relationships { get; set; }
}

public class GroupCreateAttributes
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ContactEmail { get; set; }
    public string? Schedule { get; set; }
    public string? LocationTypePreference { get; set; }
    public string? VirtualLocationUrl { get; set; }
    public bool MembersAreConfidential { get; set; }
    public bool ChatEnabled { get; set; }
}

public class GroupCreateRelationships
{
    public RelationshipData? GroupType { get; set; }
}

public class GroupUpdateDto
{
    public string Type { get; set; } = "Group";
    public GroupUpdateAttributes Attributes { get; set; } = new();
}

public class GroupUpdateAttributes
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ContactEmail { get; set; }
    public string? Schedule { get; set; }
    public string? VirtualLocationUrl { get; set; }
    public bool? MembersAreConfidential { get; set; }
    public bool? ChatEnabled { get; set; }
}

// Membership DTOs
public class MembershipCreateDto
{
    public string Type { get; set; } = "Membership";
    public MembershipCreateAttributes Attributes { get; set; } = new();
    public MembershipCreateRelationships? Relationships { get; set; }
}

public class MembershipCreateAttributes
{
    public string Role { get; set; } = string.Empty;
    public bool CanCreateEvents { get; set; }
    public bool CanEditGroup { get; set; }
    public bool CanManageMembers { get; set; }
    public bool CanViewMembers { get; set; }
}

public class MembershipCreateRelationships
{
    public RelationshipData? Person { get; set; }
}

public class MembershipUpdateDto
{
    public string Type { get; set; } = "Membership";
    public MembershipUpdateAttributes Attributes { get; set; } = new();
}

public class MembershipUpdateAttributes
{
    public string? Role { get; set; }
    public bool? CanCreateEvents { get; set; }
    public bool? CanEditGroup { get; set; }
    public bool? CanManageMembers { get; set; }
    public bool? CanViewMembers { get; set; }
}