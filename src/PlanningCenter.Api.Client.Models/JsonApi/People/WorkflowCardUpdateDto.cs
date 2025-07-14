using System;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.People
{
    /// <summary>
    /// WorkflowCard update DTO for JSON:API requests.
    /// </summary>
    public class WorkflowCardUpdateDto
    {
        public string Type { get; set; } = "WorkflowCard";
        public string Id { get; set; } = string.Empty;
        public WorkflowCardUpdateAttributesDto Attributes { get; set; } = new();
        public WorkflowCardUpdateRelationshipsDto? Relationships { get; set; }
    }

    /// <summary>
    /// WorkflowCard update attributes DTO.
    /// </summary>
    public class WorkflowCardUpdateAttributesDto
    {
        public string? Notes { get; set; }
        public DateTime? DueAt { get; set; }
        public string? Status { get; set; }
        public DateTime? SnoozeUntil { get; set; }
    }

    /// <summary>
    /// WorkflowCard update relationships DTO.
    /// </summary>
    public class WorkflowCardUpdateRelationshipsDto
    {
        public RelationshipData? Assignee { get; set; }
        public RelationshipData? Step { get; set; }
    }
}
