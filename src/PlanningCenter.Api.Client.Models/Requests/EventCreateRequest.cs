using System;

namespace PlanningCenter.Api.Client.Models.Requests;

/// <summary>
/// Request model for creating a calendar event.
/// Follows Single Responsibility Principle - handles only event creation data.
/// </summary>
public class EventCreateRequest
{
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
    public bool AllDayEvent { get; set; } = false;

    /// <summary>
    /// Gets or sets the event start time.
    /// </summary>
    public DateTime StartsAt { get; set; }

    /// <summary>
    /// Gets or sets the event end time.
    /// </summary>
    public DateTime? EndsAt { get; set; }

    /// <summary>
    /// Gets or sets the location name.
    /// </summary>
    public string? LocationName { get; set; }

    /// <summary>
    /// Gets or sets whether the event is visible in Church Center.
    /// </summary>
    public bool VisibleInChurchCenter { get; set; } = true;

    /// <summary>
    /// Gets or sets whether registration is required.
    /// </summary>
    public bool RegistrationRequired { get; set; } = false;

    /// <summary>
    /// Gets or sets the registration URL.
    /// </summary>
    public string? RegistrationUrl { get; set; }
}

/// <summary>
/// Request model for updating a calendar event.
/// </summary>
public class EventUpdateRequest
{
    /// <summary>
    /// Gets or sets the event name.
    /// </summary>
    public string? Name { get; set; }

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
    public bool? AllDayEvent { get; set; }

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
    /// Gets or sets whether the event is visible in Church Center.
    /// </summary>
    public bool? VisibleInChurchCenter { get; set; }

    /// <summary>
    /// Gets or sets whether registration is required.
    /// </summary>
    public bool? RegistrationRequired { get; set; }

    /// <summary>
    /// Gets or sets the registration URL.
    /// </summary>
    public string? RegistrationUrl { get; set; }
}