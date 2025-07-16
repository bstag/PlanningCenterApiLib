using System;
using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.Giving
{
    /// <summary>
    /// Represents a pledge in the Planning Center Giving module.
    /// </summary>
    public class Pledge : PlanningCenterResource
    {
        /// <summary>
        /// Gets or sets the data source for the pledge.
        /// </summary>
        public string DataSource { get; set; } = "Giving";

        /// <summary>
        /// Gets or sets the pledged amount in cents.
        /// </summary>
        public long AmountCents { get; set; }

        /// <summary>
        /// Gets or sets the pledged amount in dollars (calculated from cents).
        /// </summary>
        public decimal Amount => AmountCents / 100.0m;

        /// <summary>
        /// Gets or sets the currency code (e.g., USD).
        /// </summary>
        public string AmountCurrency { get; set; } = "USD";

        /// <summary>
        /// Gets or sets the amount donated towards this pledge in cents.
        /// </summary>
        public long DonatedAmountCents { get; set; }

        /// <summary>
        /// Gets or sets the amount donated towards this pledge in dollars (calculated from cents).
        /// </summary>
        public decimal DonatedAmount => DonatedAmountCents / 100.0m;

        /// <summary>
        /// Gets or sets the joint giver amount in cents.
        /// </summary>
        public long? JointGiverAmountCents { get; set; }

        /// <summary>
        /// Gets or sets the joint giver amount in dollars (calculated from cents).
        /// </summary>
        public decimal? JointGiverAmount => JointGiverAmountCents.HasValue ? JointGiverAmountCents.Value / 100.0m : null;

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

        /// <summary>
        /// Gets or sets the created at date for the pledge.
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the updated at date for the pledge.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}