using System;

namespace PlanningCenter.Api.Client.Models.Requests;

/// <summary>
/// Request model for creating a plan.
/// Follows Single Responsibility Principle - handles only plan creation data.
/// </summary>
public class PlanCreateRequest
{
    /// <summary>
    /// Gets or sets the plan title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the service type ID.
    /// </summary>
    public string ServiceTypeId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the service dates.
    /// </summary>
    public string? Dates { get; set; }

    /// <summary>
    /// Gets or sets the sort date.
    /// </summary>
    public DateTime? SortDate { get; set; }

    /// <summary>
    /// Gets or sets whether the plan is public.
    /// </summary>
    public bool IsPublic { get; set; } = false;

    /// <summary>
    /// Gets or sets the plan notes.
    /// </summary>
    public string? Notes { get; set; }

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
}

/// <summary>
/// Request model for updating a plan.
/// </summary>
public class PlanUpdateRequest
{
    /// <summary>
    /// Gets or sets the plan title.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the service dates.
    /// </summary>
    public string? Dates { get; set; }

    /// <summary>
    /// Gets or sets the sort date.
    /// </summary>
    public DateTime? SortDate { get; set; }

    /// <summary>
    /// Gets or sets whether the plan is public.
    /// </summary>
    public bool? IsPublic { get; set; }

    /// <summary>
    /// Gets or sets the plan notes.
    /// </summary>
    public string? Notes { get; set; }

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
}