using System;

namespace PlanningCenter.Api.Client.Models.Requests
{
    /// <summary>
    /// Request model for updating a workflow card.
    /// </summary>
    public class WorkflowCardUpdateRequest
    {
        /// <summary>
        /// Gets or sets the assignee ID for the workflow card.
        /// </summary>
        public string? AssigneeId { get; set; }

        /// <summary>
        /// Gets or sets the step ID for the workflow card.
        /// </summary>
        public string? StepId { get; set; }

        /// <summary>
        /// Gets or sets the notes for the workflow card.
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Gets or sets the due date for the workflow card.
        /// </summary>
        public DateTime? DueAt { get; set; }

        /// <summary>
        /// Gets or sets the status for the workflow card.
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets the snooze until date for the workflow card.
        /// </summary>
        public DateTime? SnoozeUntil { get; set; }
    }
}
