using System;
using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Giving
{
    /// <summary>
    /// JSON:API DTO for PaymentSource entity.
    /// </summary>
    public class PaymentSourceDto
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = "PaymentSource";
        public PaymentSourceAttributesDto Attributes { get; set; } = new();
        public PaymentSourceRelationshipsDto? Relationships { get; set; }
    }

    /// <summary>
    /// Attributes for PaymentSource JSON:API DTO.
    /// </summary>
    public class PaymentSourceAttributesDto
    {
        public string? Name { get; set; }
        public string? PaymentMethodType { get; set; }
        public string? PaymentLastFour { get; set; }
        public string? PaymentBrand { get; set; }
        public int? ExpirationMonth { get; set; }
        public int? ExpirationYear { get; set; }
        public bool Verified { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Relationships for PaymentSource JSON:API DTO.
    /// </summary>
    public class PaymentSourceRelationshipsDto
    {
        public RelationshipData? Person { get; set; }
    }
}