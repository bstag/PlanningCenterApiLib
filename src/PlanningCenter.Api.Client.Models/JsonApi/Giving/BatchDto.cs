using System;
using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Giving
{
    /// <summary>
    /// JSON:API DTO for Batch entity.
    /// </summary>
    public class BatchDto
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = "Batch";
        public BatchAttributesDto Attributes { get; set; } = new();
        public BatchRelationshipsDto? Relationships { get; set; }
    }

    /// <summary>
    /// Attributes for Batch JSON:API DTO.
    /// </summary>
    public class BatchAttributesDto
    {
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? CommittedAt { get; set; }
        public long TotalCents { get; set; }
        public int TotalCount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Relationships for Batch JSON:API DTO.
    /// </summary>
    public class BatchRelationshipsDto
    {
        public RelationshipData? Owner { get; set; }
    }
}