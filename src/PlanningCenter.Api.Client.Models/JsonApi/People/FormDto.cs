using System;

namespace PlanningCenter.Api.Client.Models.JsonApi.People
{
    /// <summary>
    /// Form DTO for JSON:API responses.
    /// </summary>
    public class FormDto
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = "Form";
        public FormAttributesDto Attributes { get; set; } = new();
    }

    /// <summary>
    /// Form attributes DTO.
    /// </summary>
    public class FormAttributesDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsArchived { get; set; }
        public int SubmissionCount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ArchivedAt { get; set; }
    }
}
