namespace PlanningCenter.Api.Client.Models.Requests
{
    /// <summary>
    /// Request model for creating a pledge.
    /// </summary>
    public class PledgeCreateRequest
    {
        /// <summary>
        /// Gets or sets the pledged amount in cents.
        /// </summary>
        public long AmountCents { get; set; }

        /// <summary>
        /// Gets or sets the currency code (defaults to USD).
        /// </summary>
        public string AmountCurrency { get; set; } = "USD";

        /// <summary>
        /// Gets or sets the joint giver amount in cents.
        /// </summary>
        public long? JointGiverAmountCents { get; set; }

        /// <summary>
        /// Gets or sets the person ID who made this pledge.
        /// </summary>
        public string? PersonId { get; set; }

        /// <summary>
        /// Gets or sets the fund ID this pledge is for.
        /// </summary>
        public string? FundId { get; set; }

        /// <summary>
        /// Gets or sets the pledge campaign ID.
        /// </summary>
        public string? PledgeCampaignId { get; set; }

        /// <summary>
        /// Gets or sets the joint giver person ID.
        /// </summary>
        public string? JointGiverId { get; set; }
    }

    /// <summary>
    /// Request model for updating a pledge.
    /// </summary>
    public class PledgeUpdateRequest
    {
        /// <summary>
        /// Gets or sets the pledged amount in cents.
        /// </summary>
        public long? AmountCents { get; set; }

        /// <summary>
        /// Gets or sets the joint giver amount in cents.
        /// </summary>
        public long? JointGiverAmountCents { get; set; }

        /// <summary>
        /// Gets or sets the fund ID this pledge is for.
        /// </summary>
        public string? FundId { get; set; }

        /// <summary>
        /// Gets or sets the pledge campaign ID.
        /// </summary>
        public string? PledgeCampaignId { get; set; }

        /// <summary>
        /// Gets or sets the joint giver person ID.
        /// </summary>
        public string? JointGiverId { get; set; }
    }
}