using System;
using System.Collections.Generic;
using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.Services;

/// <summary>
/// Represents a service plan in Planning Center Services.
/// Follows Single Responsibility Principle - handles only plan data.
/// </summary>
public class Plan : PlanningCenterResource
{
    /// <summary>
    /// Gets or sets the data source for the plan.
    /// </summary>
    public string DataSource { get; set; } = "Services";

    /// <summary>
    /// Gets or sets the plan title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the service dates.
    /// </summary>
    public string? Dates { get; set; }

    /// <summary>
    /// Gets or sets the abbreviated date display.
    /// </summary>
    public string? ShortDates { get; set; }

    /// <summary>
    /// Gets or sets the Planning Center URL for this plan.
    /// </summary>
    public string? PlanningCenterUrl { get; set; }

    /// <summary>
    /// Gets or sets the sort date for ordering plans.
    /// </summary>
    public DateTime? SortDate { get; set; }

    /// <summary>
    /// Gets or sets whether the plan is public.
    /// </summary>
    public bool IsPublic { get; set; }

    /// <summary>
    /// Gets or sets the rehearsal times.
    /// </summary>
    public string? RehearsalTimes { get; set; }

    /// <summary>
    /// Gets or sets the service times.
    /// </summary>
    public string? ServiceTimes { get; set; }

    /// <summary>
    /// Gets or sets other times.
    /// </summary>
    public string? OtherTimes { get; set; }

    /// <summary>
    /// Gets or sets the service length in minutes.
    /// </summary>
    public int? Length { get; set; }

    /// <summary>
    /// Gets or sets the plan notes.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the service type ID.
    /// </summary>
    public string? ServiceTypeId { get; set; }

    /// <summary>
    /// Gets or sets the creation date.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update date.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}