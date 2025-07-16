namespace PlanningCenter.Api.Client.Models.Requests;

/// <summary>
/// Request model for updating a pledge.
/// </summary>
public class PledgeUpdateRequest
{
    /// <summary>
    /// Gets or sets the pledge amount in cents.
    /// </summary>
    public long? AmountCents { get; set; }

    /// <summary>
    /// Gets or sets the currency code.
    /// </summary>
    public string? AmountCurrency { get; set; }

    /// <summary>
    /// Gets or sets the joint giver amount in cents.
    /// </summary>
    public long? JointGiverAmountCents { get; set; }
}