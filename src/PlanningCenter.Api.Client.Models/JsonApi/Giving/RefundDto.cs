using System;
using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Giving
{
    /// <summary>
    /// JSON:API DTO for Refund entity.
    /// </summary>
    public class RefundDto
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = "Refund";
        public RefundAttributesDto Attributes { get; set; } = new();
        public RefundRelationshipsDto? Relationships { get; set; }
    }

    /// <summary>
    /// Attributes for Refund JSON:API DTO.
    /// </summary>
    public class RefundAttributesDto
    {
        public long AmountCents { get; set; }
        public string AmountCurrency { get; set; } = "USD";
        public long? FeeCents { get; set; }
        public string? Reason { get; set; }
        public DateTime? RefundedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Relationships for Refund JSON:API DTO.
    /// </summary>
    public class RefundRelationshipsDto
    {
        public RelationshipData? Donation { get; set; }
    }
}