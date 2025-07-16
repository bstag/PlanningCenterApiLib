namespace PlanningCenter.Api.Client.Models.Requests;

/// <summary>
/// Request model for updating a donation.
/// </summary>
public class DonationUpdateRequest
{
    /// <summary>
    /// Gets or sets the donation amount in cents.
    /// </summary>
    public long? AmountCents { get; set; }

    /// <summary>
    /// Gets or sets the payment method.
    /// </summary>
    public string? PaymentMethod { get; set; }

    /// <summary>
    /// Gets or sets the check number if payment method is check.
    /// </summary>
    public string? PaymentCheckNumber { get; set; }

    /// <summary>
    /// Gets or sets the check date if payment method is check.
    /// </summary>
    public DateTime? PaymentCheckDatedAt { get; set; }

    /// <summary>
    /// Gets or sets when the donation was received.
    /// </summary>
    public DateTime? ReceivedAt { get; set; }

    /// <summary>
    /// Gets or sets the batch ID to move the donation to.
    /// </summary>
    public string? BatchId { get; set; }
}