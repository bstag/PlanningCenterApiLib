using System;
using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.Giving
{
    /// <summary>
    /// Represents a payment source in the Planning Center Giving module.
    /// </summary>
    public class PaymentSource : PlanningCenterResource
    {
        /// <summary>
        /// Gets or sets the data source for the payment source.
        /// </summary>
        public new string DataSource { get; set; } = "Giving";

        /// <summary>
        /// Gets or sets the payment source name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the payment method type (e.g., credit_card, bank_account).
        /// </summary>
        public string? PaymentMethodType { get; set; }

        /// <summary>
        /// Gets or sets the last four digits of the payment method.
        /// </summary>
        public string? PaymentLast4 { get; set; } // Updated property name

        /// <summary>
        /// Gets or sets the payment brand (e.g., Visa, MasterCard).
        /// </summary>
        public string? PaymentBrand { get; set; }

        /// <summary>
        /// Gets or sets the expiration month for credit cards.
        /// </summary>
        public int? ExpirationMonth { get; set; }

        /// <summary>
        /// Gets or sets the expiration year for credit cards.
        /// </summary>
        public int? ExpirationYear { get; set; }

        /// <summary>
        /// Gets or sets whether this payment source is verified.
        /// </summary>
        public bool Verified { get; set; }

        /// <summary>
        /// Gets or sets the person ID who owns this payment source.
        /// </summary>
        public string? PersonId { get; set; }

        /// <summary>
        /// Gets or sets the created at date for the payment source.
        /// </summary>
        public new DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the updated at date for the payment source.
        /// </summary>
        public new DateTime? UpdatedAt { get; set; }
    }
}