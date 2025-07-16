using System;
using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.Giving
{
    /// <summary>
    /// Represents a donation in the Planning Center Giving module.
    /// </summary>
    public class Donation : PlanningCenterResource
    {
        /// <summary>
        /// Gets or sets the data source for the donation.
        /// </summary>
        public string DataSource { get; set; } = "Giving";

        /// <summary>
        /// Gets or sets the donation amount in cents.
        /// </summary>
        public long AmountCents { get; set; }

        /// <summary>
        /// Gets or sets the donation amount in dollars (calculated from cents).
        /// </summary>
        public decimal Amount => AmountCents / 100.0m;

        /// <summary>
        /// Gets or sets the currency code (e.g., USD).
        /// </summary>
        public string AmountCurrency { get; set; } = "USD";

        /// <summary>
        /// Gets or sets the payment method (e.g., credit_card, check, cash).
        /// </summary>
        public string? PaymentMethod { get; set; }

        /// <summary>
        /// Gets or sets the last four digits of the payment method.
        /// </summary>
        public string? PaymentLastFour { get; set; }

        /// <summary>
        /// Gets or sets the payment brand (e.g., Visa, MasterCard).
        /// </summary>
        public string? PaymentBrand { get; set; }

        /// <summary>
        /// Gets or sets the processing fee in cents.
        /// </summary>
        public long? FeeCents { get; set; }

        /// <summary>
        /// Gets or sets the processing fee in dollars (calculated from cents).
        /// </summary>
        public decimal? Fee => FeeCents.HasValue ? FeeCents.Value / 100.0m : null;

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
        /// Gets or sets the donor's person ID.
        /// </summary>
        public string? PersonId { get; set; }

        /// <summary>
        /// Gets or sets the batch ID this donation belongs to.
        /// </summary>
        public string? BatchId { get; set; }

        /// <summary>
        /// Gets or sets the campus ID associated with this donation.
        /// </summary>
        public string? CampusId { get; set; }

        /// <summary>
        /// Gets or sets the payment source ID.
        /// </summary>
        public string? PaymentSourceId { get; set; }

        /// <summary>
        /// Gets or sets whether this donation has been refunded.
        /// </summary>
        public bool Refunded { get; set; }

        /// <summary>
        /// Gets or sets the refund ID if this donation has been refunded.
        /// </summary>
        public string? RefundId { get; set; }

        /// <summary>
        /// Gets or sets the created at date for the donation.
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the updated at date for the donation.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}