using System;
using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.Groups;

/// <summary>
/// Represents a group in Planning Center Groups.
/// Follows Single Responsibility Principle - handles only group data.
/// </summary>
public class Group : PlanningCenterResource
{
    /// <summary>
    /// Gets or sets the data source for the group.
    /// </summary>
    public new string DataSource { get; set; } = "Groups";

    /// <summary>
    /// Gets or sets the group name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the group description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the archive timestamp (null if active).
    /// </summary>
    public DateTime? ArchivedAt { get; set; }

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
    /// Gets or sets the event visibility settings.
    /// </summary>
    public string? EventsVisibility { get; set; }

    /// <summary>
    /// Gets or sets the current member count.
    /// </summary>
    public int MembershipsCount { get; set; }

    /// <summary>
    /// Gets or sets whether members are confidential.
    /// </summary>
    public bool MembersAreConfidential { get; set; }

    /// <summary>
    /// Gets or sets whether leaders can search people database.
    /// </summary>
    public bool LeadersCanSearchPeopleDatabase { get; set; }

    /// <summary>
    /// Gets or sets whether the group can create conversations.
    /// </summary>
    public bool CanCreateConversation { get; set; }

    /// <summary>
    /// Gets or sets whether chat is enabled.
    /// </summary>
    public bool ChatEnabled { get; set; }

    /// <summary>
    /// Gets or sets the public Church Center web URL.
    /// </summary>
    public string? PublicChurchCenterWebUrl { get; set; }

    /// <summary>
    /// Gets or sets the group header image.
    /// </summary>
    public string? HeaderImage { get; set; }

    /// <summary>
    /// Gets or sets the widget status.
    /// </summary>
    public string? WidgetStatus { get; set; }

    /// <summary>
    /// Gets or sets the group type ID.
    /// </summary>
    public string? GroupTypeId { get; set; }

    /// <summary>
    /// Gets or sets the location ID.
    /// </summary>
    public string? LocationId { get; set; }

    /// <summary>
    /// Gets or sets the created at date for the group.
    /// </summary>
    public new DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the updated at date for the group.
    /// </summary>
    public new DateTime? UpdatedAt { get; set; }
}