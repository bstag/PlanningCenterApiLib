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
    }
}
