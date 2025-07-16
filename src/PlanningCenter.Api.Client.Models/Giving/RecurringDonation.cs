using System;
using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.Giving
{
    /// <summary>
    /// Represents a recurring donation in the Planning Center Giving module.
    /// </summary>
    public class RecurringDonation : PlanningCenterResource
    {
        /// <summary>
        /// Gets or sets the data source for the recurring donation.
        /// </summary>
        public string DataSource { get; set; } = "Giving";

        /// <summary>
        /// Gets or sets the recurring donation amount in cents.
        /// </summary>
        public long AmountCents { get; set; }

        /// <summary>
        /// Gets or sets the recurring donation amount in dollars (calculated from cents).
        /// </summary>
        public decimal Amount => AmountCents / 100.0m;

        /// <summary>
        /// Gets or sets the currency code (e.g., USD).
        /// </summary>
        public string AmountCurrency { get; set; } = "USD";

        /// <summary>
        /// Gets or sets the recurring donation status (e.g., active, paused, cancelled).
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the recurring donation schedule (e.g., weekly, monthly, yearly).
        /// </summary>
        public string Schedule { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets when the next donation is scheduled.
        /// </summary>
        public DateTime? NextOccurrence { get; set; }

        /// <summary>
        /// Gets or sets when the last donation occurred.
        /// </summary>
        public DateTime? LastDonationReceivedAt { get; set; }

        /// <summary>
        /// Gets or sets the person ID who set up this recurring donation.
        /// </summary>
        public string? PersonId { get; set; }

        /// <summary>
        /// Gets or sets the payment source ID used for this recurring donation.
        /// </summary>
        public string? PaymentSourceId { get; set; }

        /// <summary>
        /// Gets or sets the created at date for the recurring donation.
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the updated at date for the recurring donation.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}