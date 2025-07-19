using System;
using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.CheckIns;

/// <summary>
/// Represents a check-in record in Planning Center Check-Ins.
/// Follows Single Responsibility Principle - handles only check-in data.
/// </summary>
public class CheckIn : PlanningCenterResource
{
    /// <summary>
    /// Gets or sets the data source for the check-in.
    /// </summary>
    public new string DataSource { get; set; } = "CheckIns";

    /// <summary>
    /// Gets or sets the first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the check-in number/badge number.
    /// </summary>
    public string? Number { get; set; }

    /// <summary>
    /// Gets or sets the security code for pickup.
    /// </summary>
    public string? SecurityCode { get; set; }

    /// <summary>
    /// Gets or sets the check-in type.
    /// </summary>
    public string Kind { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the check-in timestamp.
    /// </summary>
    public new DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the check-out timestamp.
    /// </summary>
    public DateTime? CheckedOutAt { get; set; }

    /// <summary>
    /// Gets or sets the confirmation timestamp.
    /// </summary>
    public DateTime? ConfirmedAt { get; set; }

    /// <summary>
    /// Gets or sets the medical notes.
    /// </summary>
    public string? MedicalNotes { get; set; }

    /// <summary>
    /// Gets or sets the emergency contact name.
    /// </summary>
    public string? EmergencyContactName { get; set; }

    /// <summary>
    /// Gets or sets the emergency contact phone number.
    /// </summary>
    public string? EmergencyContactPhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets whether this is a one-time guest.
    /// </summary>
    public bool OneTimeGuest { get; set; }

    /// <summary>
    /// Gets or sets the person ID.
    /// </summary>
    public string? PersonId { get; set; }

    /// <summary>
    /// Gets or sets the event ID.
    /// </summary>
    public string? EventId { get; set; }

    /// <summary>
    /// Gets or sets the event time ID.
    /// </summary>
    public string? EventTimeId { get; set; }

    /// <summary>
    /// Gets or sets the location ID.
    /// </summary>
    public string? LocationId { get; set; }

    /// <summary>
    /// Gets or sets the last update date.
    /// </summary>
    public new DateTime? UpdatedAt { get; set; }
}