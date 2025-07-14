using System;

namespace PlanningCenter.Api.Client.Models.People
{
    /// <summary>
    /// Represents a workflow in Planning Center People.
    /// </summary>
    public class Workflow
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the workflow name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the count of ready cards assigned to the current user.
        /// </summary>
        public int MyReadyCardCount { get; set; }

        /// <summary>
        /// Gets or sets the total count of ready cards.
        /// </summary>
        public int TotalReadyCardCount { get; set; }

        /// <summary>
        /// Gets or sets the count of completed cards.
        /// </summary>
        public int CompletedCardCount { get; set; }

        /// <summary>
        /// Gets or sets the total count of cards.
        /// </summary>
        public int TotalCardsCount { get; set; }

        /// <summary>
        /// Gets or sets the total count of ready and snoozed cards.
        /// </summary>
        public int TotalReadyAndSnoozedCardCount { get; set; }

        /// <summary>
        /// Gets or sets the total count of steps.
        /// </summary>
        public int TotalStepsCount { get; set; }

        /// <summary>
        /// Gets or sets the total count of unassigned steps.
        /// </summary>
        public int TotalUnassignedStepsCount { get; set; }

        /// <summary>
        /// Gets or sets the total count of unassigned cards.
        /// </summary>
        public int TotalUnassignedCardCount { get; set; }

        /// <summary>
        /// Gets or sets the total count of overdue cards.
        /// </summary>
        public int TotalOverdueCardCount { get; set; }

        /// <summary>
        /// Gets or sets the count of overdue cards assigned to the current user.
        /// </summary>
        public int? MyOverdueCardCount { get; set; }

        /// <summary>
        /// Gets or sets the count of due soon cards assigned to the current user.
        /// </summary>
        public int? MyDueSoonCardCount { get; set; }

        /// <summary>
        /// Gets or sets whether the workflow was recently viewed.
        /// </summary>
        public bool? RecentlyViewed { get; set; }

        /// <summary>
        /// Gets or sets the campus ID.
        /// </summary>
        public string? CampusId { get; set; }

        /// <summary>
        /// Gets or sets the workflow category ID.
        /// </summary>
        public string? WorkflowCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the last update date.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the deletion date.
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Gets or sets the archival date.
        /// </summary>
        public DateTime? ArchivedAt { get; set; }

        /// <summary>
        /// Gets or sets the data source.
        /// </summary>
        public string DataSource { get; set; } = "People";
    }
}
