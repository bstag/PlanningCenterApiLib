using System;
using System.Text.Json.Serialization;
using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.CheckIns;

/// <summary>
/// JSON:API DTO for Planning Center Check-Ins CheckIn.
/// Follows Interface Segregation Principle - focused on JSON:API serialization.
/// </summary>
public class CheckInDto
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the resource type.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = "CheckIn";

    /// <summary>
    /// Gets or sets the check-in attributes.
    /// </summary>
    [JsonPropertyName("attributes")]
    public CheckInAttributes Attributes { get; set; } = new();

    /// <summary>
    /// Gets or sets the check-in relationships.
    /// </summary>
    [JsonPropertyName("relationships")]
    public CheckInRelationships? Relationships { get; set; }
}

/// <summary>
/// CheckIn attributes for JSON:API.
/// </summary>
public class CheckInAttributes
{
    /// <summary>
    /// Gets or sets the first name.
    /// </summary>
    [JsonPropertyName("first_name")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last name.
    /// </summary>
    [JsonPropertyName("last_name")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the check-in number/badge number.
    /// </summary>
    [JsonPropertyName("number")]
    public string? Number { get; set; }

    /// <summary>
    /// Gets or sets the security code for pickup.
    /// </summary>
    [JsonPropertyName("security_code")]
    public string? SecurityCode { get; set; }

    /// <summary>
    /// Gets or sets the check-in type.
    /// </summary>
    [JsonPropertyName("kind")]
    public string Kind { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the check-in timestamp.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the check-out timestamp.
    /// </summary>
    [JsonPropertyName("checked_out_at")]
    public DateTime? CheckedOutAt { get; set; }

    /// <summary>
    /// Gets or sets the confirmation timestamp.
    /// </summary>
    [JsonPropertyName("confirmed_at")]
    public DateTime? ConfirmedAt { get; set; }

    /// <summary>
    /// Gets or sets the medical notes.
    /// </summary>
    [JsonPropertyName("medical_notes")]
    public string? MedicalNotes { get; set; }

    /// <summary>
    /// Gets or sets the emergency contact name.
    /// </summary>
    [JsonPropertyName("emergency_contact_name")]
    public string? EmergencyContactName { get; set; }

    /// <summary>
    /// Gets or sets the emergency contact phone number.
    /// </summary>
    [JsonPropertyName("emergency_contact_phone_number")]
    public string? EmergencyContactPhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets whether this is a one-time guest.
    /// </summary>
    [JsonPropertyName("one_time_guest")]
    public bool OneTimeGuest { get; set; }

    /// <summary>
    /// Gets or sets the last update date.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// CheckIn relationships for JSON:API.
/// </summary>
public class CheckInRelationships
{
    /// <summary>
    /// Gets or sets the person relationship.
    /// </summary>
    [JsonPropertyName("person")]
    public RelationshipData? Person { get; set; }

    /// <summary>
    /// Gets or sets the event relationship.
    /// </summary>
    [JsonPropertyName("event")]
    public RelationshipData? Event { get; set; }

    /// <summary>
    /// Gets or sets the event time relationship.
    /// </summary>
    [JsonPropertyName("event_time")]
    public RelationshipData? EventTime { get; set; }

    /// <summary>
    /// Gets or sets the location relationship.
    /// </summary>
    [JsonPropertyName("location")]
    public RelationshipData? Location { get; set; }
}