using System;
using System.Collections.Generic;

namespace PlanningCenter.Api.Client.Models.Requests
{
    /// <summary>
    /// Request model for creating a donation.
    /// </summary>
    public class DonationCreateRequest
    {
        /// <summary>
        /// Gets or sets the donation amount in cents.
        /// </summary>
        public long AmountCents { get; set; }

        /// <summary>
        /// Gets or sets the currency code (defaults to USD).
        /// </summary>
        public string AmountCurrency { get; set; } = "USD";

        /// <summary>
        /// Gets or sets the payment method (e.g., credit_card, check, cash).
        /// </summary>
        public string? PaymentMethod { get; set; }

        /// <summary>
        /// Gets or sets the last four digits of the payment method.
        /// </summary>
        public string? PaymentLast4 { get; set; } // Updated property name

        /// <summary>
        /// Gets or sets the payment brand (e.g., Visa, MasterCard).
        /// </summary>
        public string? PaymentBrand { get; set; }

        /// <summary>
        /// Gets or sets the processing fee in cents.
        /// </summary>
        public long? FeeCents { get; set; }

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
        /// Gets or sets the fund designations for this donation.
        /// </summary>
        public List<DonationDesignation> Designations { get; set; } = new List<DonationDesignation>();
    }

    /// <summary>
    /// Represents a fund designation for a donation.
    /// </summary>
    public class DonationDesignation
    {
        /// <summary>
        /// Gets or sets the fund ID.
        /// </summary>
        public string FundId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the amount in cents for this designation.
        /// </summary>
        public long AmountCents { get; set; }
    }
}