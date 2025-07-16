using System;
using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.Giving
{
    /// <summary>
    /// Represents a batch in the Planning Center Giving module.
    /// </summary>
    public class Batch : PlanningCenterResource
    {
        /// <summary>
        /// Gets or sets the data source for the batch.
        /// </summary>
        public string DataSource { get; set; } = "Giving";

        /// <summary>
        /// Gets or sets the batch description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the batch status (e.g., open, committed).
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets when the batch was committed.
        /// </summary>
        public DateTime? CommittedAt { get; set; }

        /// <summary>
        /// Gets or sets the total amount in the batch in cents.
        /// </summary>
        public long TotalCents { get; set; }

        /// <summary>
        /// Gets or sets the total amount in the batch in dollars (calculated from cents).
        /// </summary>
        public decimal Total => TotalCents / 100.0m;

        /// <summary>
        /// Gets or sets the total count of donations in the batch.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets or sets the person ID who owns this batch.
        /// </summary>
        public string? OwnerId { get; set; }

        /// <summary>
        /// Gets or sets the created at date for the batch.
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the updated at date for the batch.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}