using System;
using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.Groups;

/// <summary>
/// Represents a group membership in Planning Center Groups.
/// Follows Single Responsibility Principle - handles only membership data.
/// </summary>
public class Membership : PlanningCenterResource
{
    /// <summary>
    /// Gets or sets the data source for the membership.
    /// </summary>
    public string DataSource { get; set; } = "Groups";

    /// <summary>
    /// Gets or sets the membership role.
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the join date.
    /// </summary>
    public DateTime? JoinedAt { get; set; }

    /// <summary>
    /// Gets or sets whether the member can create events.
    /// </summary>
    public bool CanCreateEvents { get; set; }

    /// <summary>
    /// Gets or sets whether the member can edit group.
    /// </summary>
    public bool CanEditGroup { get; set; }

    /// <summary>
    /// Gets or sets whether the member can manage members.
    /// </summary>
    public bool CanManageMembers { get; set; }

    /// <summary>
    /// Gets or sets whether the member can view members.
    /// </summary>
    public bool CanViewMembers { get; set; }

    /// <summary>
    /// Gets or sets the person ID.
    /// </summary>
    public string PersonId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the group ID.
    /// </summary>
    public string GroupId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the creation date.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update date.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}