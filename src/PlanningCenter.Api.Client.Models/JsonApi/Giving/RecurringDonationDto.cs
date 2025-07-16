using System;
using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Giving
{
    /// <summary>
    /// JSON:API DTO for RecurringDonation entity.
    /// </summary>
    public class RecurringDonationDto
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = "RecurringDonation";
        public RecurringDonationAttributesDto Attributes { get; set; } = new();
        public RecurringDonationRelationshipsDto? Relationships { get; set; }
    }

    /// <summary>
    /// Attributes for RecurringDonation JSON:API DTO.
    /// </summary>
    public class RecurringDonationAttributesDto
    {
        public long AmountCents { get; set; }
        public string AmountCurrency { get; set; } = "USD";
        public string Status { get; set; } = string.Empty;
        public string Schedule { get; set; } = string.Empty;
        public DateTime? NextOccurrence { get; set; }
        public DateTime? LastDonationReceivedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Relationships for RecurringDonation JSON:API DTO.
    /// </summary>
    public class RecurringDonationRelationshipsDto
    {
        public RelationshipData? Person { get; set; }
        public RelationshipData? PaymentSource { get; set; }
    }
}