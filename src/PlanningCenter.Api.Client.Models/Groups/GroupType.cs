using System;
using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.Groups;

/// <summary>
/// Represents a group type in Planning Center Groups.
/// Follows Single Responsibility Principle - handles only group type data.
/// </summary>
public class GroupType : PlanningCenterResource
{
    /// <summary>
    /// Gets or sets the data source for the group type.
    /// </summary>
    public string DataSource { get; set; } = "Groups";

    /// <summary>
    /// Gets or sets the type name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets whether the type is visible in Church Center.
    /// </summary>
    public bool ChurchCenterVisible { get; set; }

    /// <summary>
    /// Gets or sets the display color.
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Gets or sets the sort order position.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// Gets or sets the creation date.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update date.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}