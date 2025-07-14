using System;

namespace PlanningCenter.Api.Client.Models.Requests
{
    /// <summary>
    /// Request model for creating a workflow card.
    /// </summary>
    public class WorkflowCardCreateRequest
    {
        /// <summary>
        /// Gets or sets the person ID associated with the workflow card.
        /// </summary>
        public string PersonId { get; set; } = string.Empty;

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
    }
}
