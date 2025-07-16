using System;
using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.Giving
{
    /// <summary>
    /// Represents a refund in the Planning Center Giving module.
    /// </summary>
    public class Refund : PlanningCenterResource
    {
        /// <summary>
        /// Gets or sets the data source for the refund.
        /// </summary>
        public string DataSource { get; set; } = "Giving";

        /// <summary>
        /// Gets or sets the refund amount in cents.
        /// </summary>
        public long AmountCents { get; set; }

        /// <summary>
        /// Gets or sets the refund amount in dollars (calculated from cents).
        /// </summary>
        public decimal Amount => AmountCents / 100.0m;

        /// <summary>
        /// Gets or sets the currency code (e.g., USD).
        /// </summary>
        public string AmountCurrency { get; set; } = "USD";

        /// <summary>
        /// Gets or sets the refund fee in cents.
        /// </summary>
        public long? FeeCents { get; set; }

        /// <summary>
        /// Gets or sets the refund fee in dollars (calculated from cents).
        /// </summary>
        public decimal? Fee => FeeCents.HasValue ? FeeCents.Value / 100.0m : null;

        /// <summary>
        /// Gets or sets the refund reason.
        /// </summary>
        public string? Reason { get; set; }

        /// <summary>
        /// Gets or sets the donation ID this refund is for.
        /// </summary>
        public string? DonationId { get; set; }

        /// <summary>
        /// Gets or sets when the refund was issued.
        /// </summary>
        public DateTime? RefundedAt { get; set; }

        /// <summary>
        /// Gets or sets the created at date for the refund.
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the updated at date for the refund.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}