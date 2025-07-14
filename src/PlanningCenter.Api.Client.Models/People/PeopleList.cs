using System;
using System.Collections.Generic;
using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.People
{
    /// <summary>
    /// Represents a people list in the Planning Center People module.
    /// </summary>
    public class PeopleList : PlanningCenterResource
    {
        /// <summary>
        /// Gets or sets the data source for the list.
        /// </summary>
        public string DataSource { get; set; } = "People";

        /// <summary>
        /// Gets or sets the name of the list.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the list.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the number of people in the list.
        /// </summary>
        public int PeopleCount { get; set; }

        /// <summary>
        /// Gets or sets the total count field returned by People API (legacy alias).
        /// </summary>
        public int TotalCount { get => PeopleCount; set => PeopleCount = value; }

        /// <summary>
        /// Gets or sets the status of the list.
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether the list is public.
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        /// Gets or sets the created at date for the list.
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the updated at date for the list.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Represents a list member in the Planning Center People module.
    /// </summary>
    public class ListMember : PlanningCenterResource
    {
        /// <summary>
        /// Gets or sets the data source for the list member.
        /// </summary>
        public string DataSource { get; set; } = "People";

        /// <summary>
        /// Gets or sets the list ID associated with the member.
        /// </summary>
        public string ListId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the person ID associated with the member.
        /// </summary>
        public string PersonId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the created at date for the list member.
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the updated at date for the list member.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
