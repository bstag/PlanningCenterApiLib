namespace PlanningCenter.Api.Client.Models.Requests;

/// <summary>
/// Request model for updating a recurring donation.
/// </summary>
public class RecurringDonationUpdateRequest
{
    /// <summary>
    /// Gets or sets the donation amount in cents.
    /// </summary>
    public long? AmountCents { get; set; }

    /// <summary>
    /// Gets or sets the currency code.
    /// </summary>
    public string? AmountCurrency { get; set; }

    /// <summary>
    /// Gets or sets the donation status.
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Gets or sets the donation schedule.
    /// </summary>
    public string? Schedule { get; set; }

    /// <summary>
    /// Gets or sets the next occurrence date.
    /// </summary>
    public DateTime? NextOccurrence { get; set; }
}