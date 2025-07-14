namespace PlanningCenter.Api.Client.Models.Requests;

/// <summary>
/// Request model for creating a check-in.
/// Follows Single Responsibility Principle - handles only check-in creation data.
/// </summary>
public class CheckInCreateRequest
{
    /// <summary>
    /// Gets or sets the first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the person ID (if checking in an existing person).
    /// </summary>
    public string? PersonId { get; set; }

    /// <summary>
    /// Gets or sets the event ID.
    /// </summary>
    public string EventId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the event time ID.
    /// </summary>
    public string? EventTimeId { get; set; }

    /// <summary>
    /// Gets or sets the location ID.
    /// </summary>
    public string? LocationId { get; set; }

    /// <summary>
    /// Gets or sets the check-in type.
    /// </summary>
    public string Kind { get; set; } = "regular";

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
    public bool OneTimeGuest { get; set; } = false;
}

/// <summary>
/// Request model for updating a check-in.
/// </summary>
public class CheckInUpdateRequest
{
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
}