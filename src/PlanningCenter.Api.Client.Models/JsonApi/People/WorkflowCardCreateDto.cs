using System;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.People
{
    /// <summary>
    /// WorkflowCard creation DTO for JSON:API requests.
    /// </summary>
    public class WorkflowCardCreateDto
    {
        public string Type { get; set; } = "WorkflowCard";
        public WorkflowCardCreateAttributesDto Attributes { get; set; } = new();
        public WorkflowCardCreateRelationshipsDto? Relationships { get; set; }
    }

    /// <summary>
    /// WorkflowCard creation attributes DTO.
    /// </summary>
    public class WorkflowCardCreateAttributesDto
    {
        public string? Notes { get; set; }
        public DateTime? DueAt { get; set; }
    }

    /// <summary>
    /// WorkflowCard creation relationships DTO.
    /// </summary>
    public class WorkflowCardCreateRelationshipsDto
    {
        public RelationshipData? Person { get; set; }
        public RelationshipData? Assignee { get; set; }
        public RelationshipData? Step { get; set; }
    }
}
