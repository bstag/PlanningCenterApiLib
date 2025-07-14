using System.Collections.Generic;

namespace PlanningCenter.Api.Client.Models.Requests
{
    /// <summary>
    /// Request model for creating a household.
    /// </summary>
    public class HouseholdCreateRequest
    {
        /// <summary>
        /// Gets or sets the household name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the primary contact ID.
        /// </summary>
        public string PrimaryContactId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of person IDs to include in the household.
        /// </summary>
        public List<string> PersonIds { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the avatar image (File UUID).
        /// </summary>
        public string? Avatar { get; set; }
    }
}
