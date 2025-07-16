using System;
using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Giving
{
    /// <summary>
    /// JSON:API DTO for Donation entity.
    /// </summary>
    public class DonationDto
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = "Donation";
        public DonationAttributesDto Attributes { get; set; } = new();
        public DonationRelationshipsDto? Relationships { get; set; }
    }

    /// <summary>
    /// Attributes for Donation JSON:API DTO.
    /// </summary>
    public class DonationAttributesDto
    {
        public long AmountCents { get; set; }
        public string AmountCurrency { get; set; } = "USD";
        public string? PaymentMethod { get; set; }
        public string? PaymentLastFour { get; set; }
        public string? PaymentBrand { get; set; }
        public long? FeeCents { get; set; }
        public string? PaymentCheckNumber { get; set; }
        public DateTime? PaymentCheckDatedAt { get; set; }
        public DateTime? ReceivedAt { get; set; }
        public bool Refunded { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Relationships for Donation JSON:API DTO.
    /// </summary>
    public class DonationRelationshipsDto
    {
        public RelationshipData? Person { get; set; }
        public RelationshipData? Batch { get; set; }
        public RelationshipData? Campus { get; set; }
        public RelationshipData? PaymentSource { get; set; }
        public RelationshipData? Refund { get; set; }
        public RelationshipDataCollection? Designations { get; set; }
    }
}