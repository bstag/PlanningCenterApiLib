namespace PlanningCenter.Api.Client.Models.Requests
{
    /// <summary>
    /// Request model for creating a refund.
    /// </summary>
    public class RefundCreateRequest
    {
        /// <summary>
        /// Gets or sets the refund amount in cents.
        /// </summary>
        public long AmountCents { get; set; }

        /// <summary>
        /// Gets or sets the currency code (defaults to USD).
        /// </summary>
        public string AmountCurrency { get; set; } = "USD";

        /// <summary>
        /// Gets or sets the refund reason.
        /// </summary>
        public string? Reason { get; set; }
    }
}