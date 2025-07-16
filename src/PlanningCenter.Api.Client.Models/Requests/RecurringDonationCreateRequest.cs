namespace PlanningCenter.Api.Client.Models.Requests
{
    /// <summary>
    /// Request model for creating a recurring donation.
    /// </summary>
    public class RecurringDonationCreateRequest
    {
        /// <summary>
        /// Gets or sets the recurring donation amount in cents.
        /// </summary>
        public long AmountCents { get; set; }

        /// <summary>
        /// Gets or sets the currency code (defaults to USD).
        /// </summary>
        public string AmountCurrency { get; set; } = "USD";

        /// <summary>
        /// Gets or sets the recurring donation schedule (e.g., weekly, monthly, yearly).
        /// </summary>
        public string Schedule { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the person ID who set up this recurring donation.
        /// </summary>
        public string? PersonId { get; set; }

        /// <summary>
        /// Gets or sets the payment source ID used for this recurring donation.
        /// </summary>
        public string? PaymentSourceId { get; set; }
    }

    /// <summary>
    /// Request model for updating a recurring donation.
    /// </summary>
    public class RecurringDonationUpdateRequest
    {
        /// <summary>
        /// Gets or sets the recurring donation amount in cents.
        /// </summary>
        public long? AmountCents { get; set; }

        /// <summary>
        /// Gets or sets the recurring donation status (e.g., active, paused, cancelled).
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets the recurring donation schedule (e.g., weekly, monthly, yearly).
        /// </summary>
        public string? Schedule { get; set; }

        /// <summary>
        /// Gets or sets the payment source ID used for this recurring donation.
        /// </summary>
        public string? PaymentSourceId { get; set; }
    }
}