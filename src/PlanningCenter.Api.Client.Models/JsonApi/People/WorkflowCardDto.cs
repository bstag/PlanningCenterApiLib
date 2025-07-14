using System;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.People
{
    /// <summary>
    /// WorkflowCard DTO for JSON:API responses.
    /// </summary>
    public class WorkflowCardDto
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = "WorkflowCard";
        public WorkflowCardAttributesDto Attributes { get; set; } = new();
        public WorkflowCardRelationshipsDto? Relationships { get; set; }
    }

    /// <summary>
    /// WorkflowCard attributes DTO.
    /// </summary>
    public class WorkflowCardAttributesDto
    {
        public string Stage { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime? SnoozeUntil { get; set; }
        public DateTime? DueAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? AssigneeId { get; set; }
        public string? PersonId { get; set; }
        public string WorkflowId { get; set; } = string.Empty;
        public string? StepId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// WorkflowCard relationships DTO.
    /// </summary>
    public class WorkflowCardRelationshipsDto
    {
        public RelationshipData? Assignee { get; set; }
        public RelationshipData? Person { get; set; }
        public RelationshipData? Workflow { get; set; }
        public RelationshipData? Step { get; set; }
    }
}
