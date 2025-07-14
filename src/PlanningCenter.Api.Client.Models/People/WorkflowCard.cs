using System;

namespace PlanningCenter.Api.Client.Models.People
{
    /// <summary>
    /// Represents a workflow card in Planning Center People.
    /// </summary>
    public class WorkflowCard
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the workflow card stage.
        /// </summary>
        public string Stage { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the workflow card status.
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the workflow card notes.
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Gets or sets the workflow card snooze until date.
        /// </summary>
        public DateTime? SnoozeUntil { get; set; }

        /// <summary>
        /// Gets or sets the workflow card due date.
        /// </summary>
        public DateTime? DueAt { get; set; }

        /// <summary>
        /// Gets or sets the workflow card completion date.
        /// </summary>
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// Gets or sets the workflow card assignee ID.
        /// </summary>
        public string? AssigneeId { get; set; }

        /// <summary>
        /// Gets or sets the workflow card person ID.
        /// </summary>
        public string? PersonId { get; set; }

        /// <summary>
        /// Gets or sets the workflow card workflow ID.
        /// </summary>
        public string WorkflowId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the workflow card step ID.
        /// </summary>
        public string? StepId { get; set; }

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
