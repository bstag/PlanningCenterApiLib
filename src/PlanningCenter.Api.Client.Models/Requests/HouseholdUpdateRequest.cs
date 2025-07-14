using System.Collections.Generic;

namespace PlanningCenter.Api.Client.Models.Requests
{
    /// <summary>
    /// Request model for updating a household.
    /// </summary>
    public class HouseholdUpdateRequest
    {
        /// <summary>
        /// Gets or sets the household name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the primary contact ID.
        /// </summary>
        public string? PrimaryContactId { get; set; }

        /// <summary>
        /// Gets or sets the avatar image (File UUID).
        /// </summary>
        public string? Avatar { get; set; }
    }
}
