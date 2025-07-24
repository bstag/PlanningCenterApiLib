using System;
using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.Services;

/// <summary>
/// Represents a person's assignment to a specific plan in Planning Center Services.
/// Follows Single Responsibility Principle - handles only plan person assignment data.
/// </summary>
public class PlanPerson : PlanningCenterResource
{
    /// <summary>
    /// Gets or sets the data source for the plan person.
    /// </summary>
    public new string DataSource { get; set; } = "Services";

    /// <summary>
    /// Gets or sets the assignment status.
    /// Accepts one of 'C', 'U', 'D', or 'Confirmed', 'Unconfirmed', or 'Declined'.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the assignment notes.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the decline reason (if applicable).
    /// </summary>
    public string? DeclineReason { get; set; }

    /// <summary>
    /// Gets or sets the person's name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the team position name.
    /// </summary>
    public string TeamPositionName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the photo thumbnail URL.
    /// </summary>
    public string? PhotoThumbnail { get; set; }

    /// <summary>
    /// Gets or sets the name of who scheduled this person.
    /// </summary>
    public string? ScheduledByName { get; set; }

    /// <summary>
    /// Gets or sets when the status was last updated.
    /// </summary>
    public DateTime? StatusUpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets whether to prepare notification.
    /// </summary>
    public bool PrepareNotification { get; set; }

    /// <summary>
    /// Gets or sets whether the person can accept partial assignments.
    /// If the person is scheduled to a split team where they could potentially accept 1 time and decline another.
    /// </summary>
    public bool CanAcceptPartial { get; set; }

    /// <summary>
    /// Gets or sets the notification changed by name.
    /// </summary>
    public string? NotificationChangedByName { get; set; }

    /// <summary>
    /// Gets or sets the notification sender name.
    /// </summary>
    public string? NotificationSenderName { get; set; }

    /// <summary>
    /// Gets or sets when the notification was changed.
    /// </summary>
    public DateTime? NotificationChangedAt { get; set; }

    /// <summary>
    /// Gets or sets when the notification was prepared.
    /// </summary>
    public DateTime? NotificationPreparedAt { get; set; }

    /// <summary>
    /// Gets or sets when the notification was read.
    /// </summary>
    public DateTime? NotificationReadAt { get; set; }

    /// <summary>
    /// Gets or sets when the notification was sent.
    /// </summary>
    public DateTime? NotificationSentAt { get; set; }

    /// <summary>
    /// Gets or sets the person ID.
    /// </summary>
    public string? PersonId { get; set; }

    /// <summary>
    /// Gets or sets the plan ID.
    /// </summary>
    public string? PlanId { get; set; }

    /// <summary>
    /// Gets or sets the team ID.
    /// </summary>
    public string? TeamId { get; set; }

    /// <summary>
    /// Gets or sets the service type ID.
    /// </summary>
    public string? ServiceTypeId { get; set; }

    /// <summary>
    /// Gets or sets the creation date.
    /// </summary>
    public new DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update date.
    /// </summary>
    public new DateTime? UpdatedAt { get; set; }
}