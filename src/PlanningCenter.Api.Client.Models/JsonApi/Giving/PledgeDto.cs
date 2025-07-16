using System;
using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Giving
{
    /// <summary>
    /// JSON:API DTO for Pledge entity.
    /// </summary>
    public class PledgeDto
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = "Pledge";
        public PledgeAttributesDto Attributes { get; set; } = new();
        public PledgeRelationshipsDto? Relationships { get; set; }
    }

    /// <summary>
    /// Attributes for Pledge JSON:API DTO.
    /// </summary>
    public class PledgeAttributesDto
    {
        public long AmountCents { get; set; }
        public string AmountCurrency { get; set; } = "USD";
        public long DonatedAmountCents { get; set; }
        public long? JointGiverAmountCents { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Relationships for Pledge JSON:API DTO.
    /// </summary>
    public class PledgeRelationshipsDto
    {
        public RelationshipData? Person { get; set; }
        public RelationshipData? Fund { get; set; }
        public RelationshipData? PledgeCampaign { get; set; }
        public RelationshipData? JointGiver { get; set; }
    }
}