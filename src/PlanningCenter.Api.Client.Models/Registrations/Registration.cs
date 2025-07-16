using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.Registrations;

/// <summary>
/// Represents an individual registration submission in Planning Center Registrations.
/// </summary>
public class Registration : PlanningCenterResource
{
    /// <summary>
    /// Gets or sets the associated signup ID.
    /// </summary>
    public string SignupId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the registration status.
    /// </summary>
    public string Status { get; set; } = "pending";
    
    /// <summary>
    /// Gets or sets the total cost of the registration.
    /// </summary>
    public decimal? TotalCost { get; set; }
    
    /// <summary>
    /// Gets or sets the amount paid.
    /// </summary>
    public decimal? AmountPaid { get; set; }
    
    /// <summary>
    /// Gets or sets the payment status.
    /// </summary>
    public string? PaymentStatus { get; set; }
    
    /// <summary>
    /// Gets or sets the payment method.
    /// </summary>
    public string? PaymentMethod { get; set; }
    
    /// <summary>
    /// Gets or sets the confirmation code.
    /// </summary>
    public string? ConfirmationCode { get; set; }
    
    /// <summary>
    /// Gets or sets the registration notes.
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Gets or sets the registrant's email address.
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// Gets or sets the registrant's first name.
    /// </summary>
    public string? FirstName { get; set; }
    
    /// <summary>
    /// Gets or sets the registrant's last name.
    /// </summary>
    public string? LastName { get; set; }
    
    /// <summary>
    /// Gets or sets the registrant's phone number.
    /// </summary>
    public string? PhoneNumber { get; set; }
    
    /// <summary>
    /// Gets or sets when the registration was completed.
    /// </summary>
    public DateTime? CompletedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the number of attendees in this registration.
    /// </summary>
    public int AttendeeCount { get; set; }
    
    /// <summary>
    /// Gets or sets whether this registration requires approval.
    /// </summary>
    public bool RequiresApproval { get; set; }
    
    /// <summary>
    /// Gets or sets the approval status.
    /// </summary>
    public string? ApprovalStatus { get; set; }
    
    /// <summary>
    /// Gets or sets when the registration was approved.
    /// </summary>
    public DateTime? ApprovedAt { get; set; }
    
    /// <summary>
    /// Gets or sets who approved the registration.
    /// </summary>
    public string? ApprovedBy { get; set; }
}