using System;
using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.Calendar;

/// <summary>
/// Represents a calendar event in Planning Center Calendar.
/// Follows Single Responsibility Principle - handles only calendar event data.
/// </summary>
public class Event : PlanningCenterResource
{
    /// <summary>
    /// Gets or sets the data source for the event.
    /// </summary>
    public new string DataSource { get; set; } = "Calendar";

    /// <summary>
    /// Gets or sets the event name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the event summary.
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// Gets or sets the event description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets whether this is an all-day event.
    /// </summary>
    public bool AllDayEvent { get; set; }

    /// <summary>
    /// Gets or sets the event start time.
    /// </summary>
    public DateTime? StartsAt { get; set; }

    /// <summary>
    /// Gets or sets the event end time.
    /// </summary>
    public DateTime? EndsAt { get; set; }

    /// <summary>
    /// Gets or sets the location name.
    /// </summary>
    public string? LocationName { get; set; }

    /// <summary>
    /// Gets or sets the details URL.
    /// </summary>
    public string? DetailsUrl { get; set; }

    /// <summary>
    /// Gets or sets whether the event is visible in Church Center.
    /// </summary>
    public bool VisibleInChurchCenter { get; set; }

    /// <summary>
    /// Gets or sets whether registration is required.
    /// </summary>
    public bool RegistrationRequired { get; set; }

    /// <summary>
    /// Gets or sets the registration URL.
    /// </summary>
    public string? RegistrationUrl { get; set; }

    /// <summary>
    /// Gets or sets the owner name.
    /// </summary>
    public string? OwnerName { get; set; }

    /// <summary>
    /// Gets or sets the approval status.
    /// </summary>
    public string? ApprovalStatus { get; set; }

    /// <summary>
    /// Gets or sets the creation date.
    /// </summary>
    public new DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update date.
    /// </summary>
    public new DateTime? UpdatedAt { get; set; }
}