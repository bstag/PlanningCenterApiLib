using System;
using System.Collections.Generic;
using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.People
{
    /// <summary>
    /// FormSubmission DTO for JSON:API responses.
    /// </summary>
    public class FormSubmissionDto
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = "FormSubmission";
        public FormSubmissionAttributesDto Attributes { get; set; } = new();
        public FormSubmissionRelationshipsDto? Relationships { get; set; }
    }

    /// <summary>
    /// FormSubmission attributes DTO.
    /// </summary>
    public class FormSubmissionAttributesDto
    {
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Dictionary<string, object>? FieldData { get; set; }
    }

    /// <summary>
    /// FormSubmission relationships DTO.
    /// </summary>
    public class FormSubmissionRelationshipsDto
    {
        public RelationshipData? Person { get; set; }
        public RelationshipData? Form { get; set; }
    }
}
