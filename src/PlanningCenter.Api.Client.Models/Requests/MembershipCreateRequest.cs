namespace PlanningCenter.Api.Client.Models.Requests;

/// <summary>
/// Request model for creating a group membership.
/// Follows Single Responsibility Principle - handles only membership creation data.
/// </summary>
public class MembershipCreateRequest
{
    /// <summary>
    /// Gets or sets the person ID.
    /// </summary>
    public string PersonId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the membership role.
    /// </summary>
    public string Role { get; set; } = "Member";

    /// <summary>
    /// Gets or sets whether the member can create events.
    /// </summary>
    public bool CanCreateEvents { get; set; } = false;

    /// <summary>
    /// Gets or sets whether the member can edit group.
    /// </summary>
    public bool CanEditGroup { get; set; } = false;

    /// <summary>
    /// Gets or sets whether the member can manage members.
    /// </summary>
    public bool CanManageMembers { get; set; } = false;

    /// <summary>
    /// Gets or sets whether the member can view members.
    /// </summary>
    public bool CanViewMembers { get; set; } = true;
}

/// <summary>
/// Request model for updating a group membership.
/// </summary>
public class MembershipUpdateRequest
{
    /// <summary>
    /// Gets or sets the membership role.
    /// </summary>
    public string? Role { get; set; }

    /// <summary>
    /// Gets or sets whether the member can create events.
    /// </summary>
    public bool? CanCreateEvents { get; set; }

    /// <summary>
    /// Gets or sets whether the member can edit group.
    /// </summary>
    public bool? CanEditGroup { get; set; }

    /// <summary>
    /// Gets or sets whether the member can manage members.
    /// </summary>
    public bool? CanManageMembers { get; set; }

    /// <summary>
    /// Gets or sets whether the member can view members.
    /// </summary>
    public bool? CanViewMembers { get; set; }
}