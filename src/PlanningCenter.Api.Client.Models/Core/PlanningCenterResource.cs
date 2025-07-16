namespace PlanningCenter.Api.Client.Models.Core
{
    /// <summary>
    /// Base class for all Planning Center resources.
    /// </summary>
    public abstract class PlanningCenterResource
    {
        /// <summary>
        /// Gets or sets the unique identifier for this resource.
        /// </summary>
        public string Id { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets when this resource was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Gets or sets when this resource was last updated.
        /// </summary>
        public DateTime UpdatedAt { get; set; }
        
        /// <summary>
        /// Gets or sets the data source module for this resource.
        /// </summary>
        public string DataSource { get; set; } = string.Empty;
    }
}
