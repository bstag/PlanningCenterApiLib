using System;

namespace PlanningCenter.Api.Client.Models.People
{
    /// <summary>
    /// Represents a household in Planning Center People.
    /// </summary>
    public class Household
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the household name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the number of members in the household.
        /// </summary>
        public int MemberCount { get; set; }

        /// <summary>
        /// Gets or sets the primary contact name.
        /// </summary>
        public string? PrimaryContactName { get; set; }

        /// <summary>
        /// Gets or sets the primary contact ID.
        /// </summary>
        public string? PrimaryContactId { get; set; }

        /// <summary>
        /// Gets or sets the avatar URL.
        /// </summary>
        public string? Avatar { get; set; }

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the last update date.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the data source.
        /// </summary>
        public string DataSource { get; set; } = "People";
    }
}
