using System;
using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.CheckIns;

/// <summary>
/// Represents a check-in event in Planning Center Check-Ins.
/// Follows Single Responsibility Principle - handles only event data.
/// </summary>
public class Event : PlanningCenterResource
{
    /// <summary>
    /// Gets or sets the data source for the event.
    /// </summary>
    public new string DataSource { get; set; } = "CheckIns";

    /// <summary>
    /// Gets or sets the event name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the event frequency.
    /// </summary>
    public string? Frequency { get; set; }

    /// <summary>
    /// Gets or sets whether the event enables services integration.
    /// </summary>
    public bool EnableServicesIntegration { get; set; }

    /// <summary>
    /// Gets or sets whether the event is archived.
    /// </summary>
    public bool Archived { get; set; }

    /// <summary>
    /// Gets or sets the creation date.
    /// </summary>
    public new DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update date.
    /// </summary>
    public new DateTime? UpdatedAt { get; set; }
}