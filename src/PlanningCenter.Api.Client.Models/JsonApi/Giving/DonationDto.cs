using System;
using System.Text.Json.Serialization;
using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.Giving
{
    /// <summary>
    /// JSON:API DTO for Donation entity.
    /// </summary>
    public class DonationDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        
        [JsonPropertyName("type")]
        public string Type { get; set; } = "Donation";
        
        [JsonPropertyName("attributes")]
        public DonationAttributesDto Attributes { get; set; } = new();
        
        [JsonPropertyName("relationships")]
        public DonationRelationshipsDto? Relationships { get; set; }
    }

    /// <summary>
    /// Attributes for Donation JSON:API DTO.
    /// </summary>
    public class DonationAttributesDto
    {
        [JsonPropertyName("amount_cents")]
        public long AmountCents { get; set; }
        
        [JsonPropertyName("amount_currency")]
        public string AmountCurrency { get; set; } = "USD";
        
        [JsonPropertyName("payment_method")]
        public string? PaymentMethod { get; set; }
        
        /// <summary>
        /// Last four digits of payment method (corrected property name)
        /// </summary>
        [JsonPropertyName("payment_last_4")]
        public string? PaymentLast4 { get; set; }
        
        [JsonPropertyName("payment_brand")]
        public string? PaymentBrand { get; set; }
        
        [JsonPropertyName("fee_cents")]
        public long? FeeCents { get; set; }
        
        [JsonPropertyName("payment_check_number")]
        public string? PaymentCheckNumber { get; set; }
        
        [JsonPropertyName("payment_check_dated_at")]
        public DateTime? PaymentCheckDatedAt { get; set; }
        
        [JsonPropertyName("received_at")]
        public DateTime? ReceivedAt { get; set; }
        
        [JsonPropertyName("refunded")]
        public bool Refunded { get; set; }
        
        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }
        
        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        // Missing fields from API specification
        /// <summary>
        /// Payment method sub-type
        /// </summary>
        [JsonPropertyName("payment_method_sub")]
        public string? PaymentMethodSub { get; set; }

        /// <summary>
        /// Payment status
        /// </summary>
        [JsonPropertyName("payment_status")]
        public string? PaymentStatus { get; set; }

        /// <summary>
        /// Date when donation was completed
        /// </summary>
        [JsonPropertyName("completed_at")]
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// Whether processing fee is covered
        /// </summary>
        [JsonPropertyName("fee_covered")]
        public bool? FeeCovered { get; set; }

        /// <summary>
        /// Currency for processing fee
        /// </summary>
        [JsonPropertyName("fee_currency")]
        public string? FeeCurrency { get; set; }

        /// <summary>
        /// Whether donation is refundable
        /// </summary>
        [JsonPropertyName("refundable")]
        public bool? Refundable { get; set; }
    }

    /// <summary>
    /// Relationships for Donation JSON:API DTO.
    /// </summary>
    public class DonationRelationshipsDto
    {
        [JsonPropertyName("person")]
        public RelationshipData? Person { get; set; }
        
        [JsonPropertyName("batch")]
        public RelationshipData? Batch { get; set; }
        
        [JsonPropertyName("campus")]
        public RelationshipData? Campus { get; set; }
        
        [JsonPropertyName("payment_source")]
        public RelationshipData? PaymentSource { get; set; }
        
        [JsonPropertyName("refund")]
        public RelationshipData? Refund { get; set; }
        
        [JsonPropertyName("designations")]
        public RelationshipDataCollection? Designations { get; set; }
    }
}