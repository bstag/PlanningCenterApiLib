namespace PlanningCenter.Api.Client.Models.Requests;

/// <summary>
/// Request model for creating a group.
/// Follows Single Responsibility Principle - handles only group creation data.
/// </summary>
public class GroupCreateRequest
{
    /// <summary>
    /// Gets or sets the group name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the group description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the group type ID.
    /// </summary>
    public string GroupTypeId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the group contact email.
    /// </summary>
    public string? ContactEmail { get; set; }

    /// <summary>
    /// Gets or sets the meeting schedule description.
    /// </summary>
    public string? Schedule { get; set; }

    /// <summary>
    /// Gets or sets the preferred location type.
    /// </summary>
    public string? LocationTypePreference { get; set; }

    /// <summary>
    /// Gets or sets the virtual meeting URL.
    /// </summary>
    public string? VirtualLocationUrl { get; set; }

    /// <summary>
    /// Gets or sets whether members are confidential.
    /// </summary>
    public bool MembersAreConfidential { get; set; } = false;

    /// <summary>
    /// Gets or sets whether chat is enabled.
    /// </summary>
    public bool ChatEnabled { get; set; } = true;
}

/// <summary>
/// Request model for updating a group.
/// </summary>
public class GroupUpdateRequest
{
    /// <summary>
    /// Gets or sets the group name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the group description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the group contact email.
    /// </summary>
    public string? ContactEmail { get; set; }

    /// <summary>
    /// Gets or sets the meeting schedule description.
    /// </summary>
    public string? Schedule { get; set; }

    /// <summary>
    /// Gets or sets the virtual meeting URL.
    /// </summary>
    public string? VirtualLocationUrl { get; set; }

    /// <summary>
    /// Gets or sets whether members are confidential.
    /// </summary>
    public bool? MembersAreConfidential { get; set; }

    /// <summary>
    /// Gets or sets whether chat is enabled.
    /// </summary>
    public bool? ChatEnabled { get; set; }
}