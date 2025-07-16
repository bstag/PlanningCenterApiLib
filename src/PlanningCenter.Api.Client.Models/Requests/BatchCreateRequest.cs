namespace PlanningCenter.Api.Client.Models.Requests
{
    /// <summary>
    /// Request model for creating a batch.
    /// </summary>
    public class BatchCreateRequest
    {
        /// <summary>
        /// Gets or sets the batch description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the person ID who owns this batch.
        /// </summary>
        public string? OwnerId { get; set; }
    }
}