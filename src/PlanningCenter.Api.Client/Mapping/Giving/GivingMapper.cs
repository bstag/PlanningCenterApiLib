using PlanningCenter.Api.Client.Models.JsonApi.Core;
using PlanningCenter.Api.Client.Models.JsonApi.Giving;
using PlanningCenter.Api.Client.Models.Giving;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Models.JsonApi;
using System.Collections.Generic;
using System.Linq;

namespace PlanningCenter.Api.Client.Mapping.Giving
{
    /// <summary>
    /// Mapper for Giving entities between domain models and DTOs.
    /// </summary>
    public static class GivingMapper
    {
        #region Donation Mapping

        /// <summary>
        /// Maps a DonationDto to a Donation domain model.
        /// </summary>
        public static Donation MapToDomain(DonationDto dto)
        {
            return new Donation
            {
                Id = dto.Id,
                AmountCents = dto.Attributes.AmountCents,
                AmountCurrency = dto.Attributes.AmountCurrency,
                PaymentMethod = dto.Attributes.PaymentMethod,
                PaymentLast4 = dto.Attributes.PaymentLast4, // Updated property name
                PaymentBrand = dto.Attributes.PaymentBrand,
                FeeCents = dto.Attributes.FeeCents,
                PaymentCheckNumber = dto.Attributes.PaymentCheckNumber,
                PaymentCheckDatedAt = dto.Attributes.PaymentCheckDatedAt,
                ReceivedAt = dto.Attributes.ReceivedAt,
                Refunded = dto.Attributes.Refunded,
                PersonId = dto.Relationships?.Person?.Id,
                BatchId = dto.Relationships?.Batch?.Id,
                CampusId = dto.Relationships?.Campus?.Id,
                PaymentSourceId = dto.Relationships?.PaymentSource?.Id,
                RefundId = dto.Relationships?.Refund?.Id,
                CreatedAt = dto.Attributes.CreatedAt,
                UpdatedAt = dto.Attributes.UpdatedAt,
                DataSource = "Giving"
            };
        }

        /// <summary>
        /// Maps a DonationCreateRequest to a JSON:API request.
        /// </summary>
        public static JsonApiRequest<DonationCreateDto> MapCreateRequestToJsonApi(DonationCreateRequest request)
        {
            var dto = new DonationCreateDto
            {
                Type = "Donation",
                Attributes = new DonationCreateAttributesDto
                {
                    AmountCents = request.AmountCents,
                    AmountCurrency = request.AmountCurrency,
                    PaymentMethod = request.PaymentMethod,
                    PaymentLast4 = request.PaymentLast4, // Updated property name
                    PaymentBrand = request.PaymentBrand,
                    FeeCents = request.FeeCents,
                    PaymentCheckNumber = request.PaymentCheckNumber,
                    PaymentCheckDatedAt = request.PaymentCheckDatedAt,
                    ReceivedAt = request.ReceivedAt
                }
            };

            if (!string.IsNullOrEmpty(request.PersonId) || !string.IsNullOrEmpty(request.BatchId) || 
                !string.IsNullOrEmpty(request.CampusId) || !string.IsNullOrEmpty(request.PaymentSourceId))
            {
                dto.Relationships = new DonationCreateRelationshipsDto();

                if (!string.IsNullOrEmpty(request.PersonId))
                    dto.Relationships.Person = new RelationshipData { Type = "Person", Id = request.PersonId };

                if (!string.IsNullOrEmpty(request.BatchId))
                    dto.Relationships.Batch = new RelationshipData { Type = "Batch", Id = request.BatchId };

                if (!string.IsNullOrEmpty(request.CampusId))
                    dto.Relationships.Campus = new RelationshipData { Type = "Campus", Id = request.CampusId };

                if (!string.IsNullOrEmpty(request.PaymentSourceId))
                    dto.Relationships.PaymentSource = new RelationshipData { Type = "PaymentSource", Id = request.PaymentSourceId };
            }

            return new JsonApiRequest<DonationCreateDto> { Data = dto };
        }

        /// <summary>
        /// Maps a DonationUpdateRequest to a JSON:API request.
        /// </summary>
        public static JsonApiRequest<DonationUpdateDto> MapUpdateRequestToJsonApi(string id, DonationUpdateRequest request)
        {
            var dto = new DonationUpdateDto
            {
                Type = "Donation",
                Id = id,
                Attributes = new DonationUpdateAttributesDto
                {
                    AmountCents = request.AmountCents,
                    PaymentMethod = request.PaymentMethod,
                    PaymentCheckNumber = request.PaymentCheckNumber,
                    PaymentCheckDatedAt = request.PaymentCheckDatedAt,
                    ReceivedAt = request.ReceivedAt
                }
            };

            if (!string.IsNullOrEmpty(request.BatchId))
            {
                dto.Relationships = new DonationUpdateRelationshipsDto
                {
                    Batch = new RelationshipData { Type = "Batch", Id = request.BatchId }
                };
            }

            return new JsonApiRequest<DonationUpdateDto> { Data = dto };
        }

        #endregion

        #region Fund Mapping

        /// <summary>
        /// Maps a FundDto to a Fund domain model.
        /// </summary>
        public static Fund MapToDomain(FundDto dto)
        {
            return new Fund
            {
                Id = dto.Id,
                Name = dto.Attributes.Name,
                Description = dto.Attributes.Description,
                Code = dto.Attributes.Code,
                Visibility = dto.Attributes.Visibility,
                Default = dto.Attributes.Default,
                Color = dto.Attributes.Color,
                CreatedAt = dto.Attributes.CreatedAt,
                UpdatedAt = dto.Attributes.UpdatedAt,
                DataSource = "Giving"
            };
        }

        /// <summary>
        /// Maps a FundCreateRequest to a JSON:API request.
        /// </summary>
        public static JsonApiRequest<FundCreateDto> MapCreateRequestToJsonApi(FundCreateRequest request)
        {
            return new JsonApiRequest<FundCreateDto>
            {
                Data = new FundCreateDto
                {
                    Type = "Fund",
                    Attributes = new FundCreateAttributesDto
                    {
                        Name = request.Name,
                        Description = request.Description,
                        Code = request.Code,
                        Visibility = request.Visibility,
                        Default = request.Default,
                        Color = request.Color
                    }
                }
            };
        }

        /// <summary>
        /// Maps a FundUpdateRequest to a JSON:API request.
        /// </summary>
        public static JsonApiRequest<FundUpdateDto> MapUpdateRequestToJsonApi(string id, FundUpdateRequest request)
        {
            return new JsonApiRequest<FundUpdateDto>
            {
                Data = new FundUpdateDto
                {
                    Type = "Fund",
                    Id = id,
                    Attributes = new FundUpdateAttributesDto
                    {
                        Name = request.Name,
                        Description = request.Description,
                        Code = request.Code,
                        Visibility = request.Visibility,
                        Default = request.Default,
                        Color = request.Color
                    }
                }
            };
        }

        #endregion

        #region Batch Mapping

        /// <summary>
        /// Maps a BatchDto to a Batch domain model.
        /// </summary>
        public static Batch MapToDomain(BatchDto dto)
        {
            return new Batch
            {
                Id = dto.Id,
                Description = dto.Attributes.Description,
                Status = dto.Attributes.Status,
                CommittedAt = dto.Attributes.CommittedAt,
                TotalCents = dto.Attributes.TotalCents,
                TotalCount = dto.Attributes.TotalCount,
                OwnerId = dto.Relationships?.Owner?.Id,
                CreatedAt = dto.Attributes.CreatedAt,
                UpdatedAt = dto.Attributes.UpdatedAt,
                DataSource = "Giving"
            };
        }

        /// <summary>
        /// Maps a BatchCreateRequest to a JSON:API request.
        /// </summary>
        public static JsonApiRequest<BatchCreateDto> MapCreateRequestToJsonApi(BatchCreateRequest request)
        {
            var dto = new BatchCreateDto
            {
                Type = "Batch",
                Attributes = new BatchCreateAttributesDto
                {
                    Description = request.Description
                }
            };

            if (!string.IsNullOrEmpty(request.OwnerId))
            {
                dto.Relationships = new BatchCreateRelationshipsDto
                {
                    Owner = new RelationshipData { Type = "Person", Id = request.OwnerId }
                };
            }

            return new JsonApiRequest<BatchCreateDto> { Data = dto };
        }

        /// <summary>
        /// Maps a BatchUpdateRequest to a JSON:API request.
        /// </summary>
        public static JsonApiRequest<BatchUpdateDto> MapUpdateRequestToJsonApi(string id, BatchUpdateRequest request)
        {
            var dto = new BatchUpdateDto
            {
                Type = "Batch",
                Id = id,
                Attributes = new BatchUpdateAttributesDto
                {
                    Description = request.Description
                }
            };

            if (!string.IsNullOrEmpty(request.OwnerId))
            {
                dto.Relationships = new BatchUpdateRelationshipsDto
                {
                    Owner = new RelationshipData { Type = "Person", Id = request.OwnerId }
                };
            }

            return new JsonApiRequest<BatchUpdateDto> { Data = dto };
        }

        #endregion

        #region Pledge Mapping

        /// <summary>
        /// Maps a PledgeDto to a Pledge domain model.
        /// </summary>
        public static Pledge MapToDomain(PledgeDto dto)
        {
            return new Pledge
            {
                Id = dto.Id,
                AmountCents = dto.Attributes.AmountCents,
                AmountCurrency = dto.Attributes.AmountCurrency,
                DonatedAmountCents = dto.Attributes.DonatedAmountCents,
                JointGiverAmountCents = dto.Attributes.JointGiverAmountCents,
                PersonId = dto.Relationships?.Person?.Id,
                FundId = dto.Relationships?.Fund?.Id,
                PledgeCampaignId = dto.Relationships?.PledgeCampaign?.Id,
                JointGiverId = dto.Relationships?.JointGiver?.Id,
                CreatedAt = dto.Attributes.CreatedAt,
                UpdatedAt = dto.Attributes.UpdatedAt,
                DataSource = "Giving"
            };
        }

        /// <summary>
        /// Maps a PledgeCreateRequest to a JSON:API request.
        /// </summary>
        public static JsonApiRequest<PledgeCreateDto> MapCreateRequestToJsonApi(PledgeCreateRequest request)
        {
            var dto = new PledgeCreateDto
            {
                Type = "Pledge",
                Attributes = new PledgeCreateAttributesDto
                {
                    AmountCents = request.AmountCents,
                    AmountCurrency = request.AmountCurrency,
                    JointGiverAmountCents = request.JointGiverAmountCents
                }
            };

            if (!string.IsNullOrEmpty(request.PersonId) || !string.IsNullOrEmpty(request.FundId) ||
                !string.IsNullOrEmpty(request.PledgeCampaignId) || !string.IsNullOrEmpty(request.JointGiverId))
            {
                dto.Relationships = new PledgeCreateRelationshipsDto();

                if (!string.IsNullOrEmpty(request.PersonId))
                    dto.Relationships.Person = new RelationshipData { Type = "Person", Id = request.PersonId };

                if (!string.IsNullOrEmpty(request.FundId))
                    dto.Relationships.Fund = new RelationshipData { Type = "Fund", Id = request.FundId };

                if (!string.IsNullOrEmpty(request.PledgeCampaignId))
                    dto.Relationships.PledgeCampaign = new RelationshipData { Type = "PledgeCampaign", Id = request.PledgeCampaignId };

                if (!string.IsNullOrEmpty(request.JointGiverId))
                    dto.Relationships.JointGiver = new RelationshipData { Type = "Person", Id = request.JointGiverId };
            }

            return new JsonApiRequest<PledgeCreateDto> { Data = dto };
        }

        /// <summary>
        /// Maps a PledgeUpdateRequest to a JSON:API request.
        /// </summary>
        public static JsonApiRequest<PledgeUpdateDto> MapUpdateRequestToJsonApi(string id, PledgeUpdateRequest request)
        {
            var dto = new PledgeUpdateDto
            {
                Id = id,
                Type = "Pledge",
                Attributes = new PledgeUpdateAttributesDto
                {
                    AmountCents = request.AmountCents ?? 0,
                    AmountCurrency = request.AmountCurrency ?? "USD",
                    JointGiverAmountCents = request.JointGiverAmountCents
                }
            };

            return new JsonApiRequest<PledgeUpdateDto> { Data = dto };
        }

        #endregion

        #region RecurringDonation Mapping

        /// <summary>
        /// Maps a RecurringDonationDto to a RecurringDonation domain model.
        /// </summary>
        public static RecurringDonation MapToDomain(RecurringDonationDto dto)
        {
            return new RecurringDonation
            {
                Id = dto.Id,
                AmountCents = dto.Attributes.AmountCents,
                AmountCurrency = dto.Attributes.AmountCurrency,
                Status = dto.Attributes.Status,
                Schedule = dto.Attributes.Schedule,
                NextOccurrence = dto.Attributes.NextOccurrence,
                LastDonationReceivedAt = dto.Attributes.LastDonationReceivedAt,
                PersonId = dto.Relationships?.Person?.Id,
                PaymentSourceId = dto.Relationships?.PaymentSource?.Id,
                CreatedAt = dto.Attributes.CreatedAt,
                UpdatedAt = dto.Attributes.UpdatedAt,
                DataSource = "Giving"
            };
        }

        /// <summary>
        /// Maps a RecurringDonationCreateRequest to a JSON:API request.
        /// </summary>
        public static JsonApiRequest<RecurringDonationCreateDto> MapCreateRequestToJsonApi(RecurringDonationCreateRequest request)
        {
            var dto = new RecurringDonationCreateDto
            {
                Type = "RecurringDonation",
                Attributes = new RecurringDonationCreateAttributesDto
                {
                    AmountCents = request.AmountCents,
                    AmountCurrency = request.AmountCurrency,
                    Schedule = request.Schedule
                }
            };

            if (!string.IsNullOrEmpty(request.PersonId) || !string.IsNullOrEmpty(request.PaymentSourceId))
            {
                dto.Relationships = new RecurringDonationCreateRelationshipsDto();
                
                if (!string.IsNullOrEmpty(request.PersonId))
                    dto.Relationships.Person = new RelationshipData { Type = "Person", Id = request.PersonId };
                
                if (!string.IsNullOrEmpty(request.PaymentSourceId))
                    dto.Relationships.PaymentSource = new RelationshipData { Type = "PaymentSource", Id = request.PaymentSourceId };
            }

            return new JsonApiRequest<RecurringDonationCreateDto> { Data = dto };
        }

        /// <summary>
        /// Maps a RecurringDonationUpdateRequest to a JSON:API request.
        /// </summary>
        public static JsonApiRequest<RecurringDonationUpdateDto> MapUpdateRequestToJsonApi(string id, RecurringDonationUpdateRequest request)
        {
            var dto = new RecurringDonationUpdateDto
            {
                Id = id,
                Type = "RecurringDonation",
                Attributes = new RecurringDonationUpdateAttributesDto
                {
                    AmountCents = request.AmountCents ?? 0,
                    AmountCurrency = request.AmountCurrency ?? "USD",
                    Schedule = request.Schedule,
                    NextOccurrence = request.NextOccurrence,
                    Status = request.Status
                }
            };

            return new JsonApiRequest<RecurringDonationUpdateDto> { Data = dto };
        }

        #endregion

        #region Refund Mapping

        /// <summary>
        /// Maps a RefundDto to a Refund domain model.
        /// </summary>
        public static Refund MapToDomain(RefundDto dto)
        {
            return new Refund
            {
                Id = dto.Id,
                AmountCents = dto.Attributes.AmountCents,
                AmountCurrency = dto.Attributes.AmountCurrency,
                FeeCents = dto.Attributes.FeeCents,
                Reason = dto.Attributes.Reason,
                DonationId = dto.Relationships?.Donation?.Id,
                RefundedAt = dto.Attributes.RefundedAt,
                CreatedAt = dto.Attributes.CreatedAt,
                UpdatedAt = dto.Attributes.UpdatedAt,
                DataSource = "Giving"
            };
        }

        /// <summary>
        /// Maps a RefundCreateRequest to a JSON:API request.
        /// </summary>
        public static JsonApiRequest<RefundCreateDto> MapCreateRequestToJsonApi(RefundCreateRequest request)
        {
            var dto = new RefundCreateDto
            {
                Type = "Refund",
                Attributes = new RefundCreateAttributesDto
                {
                    AmountCents = request.AmountCents,
                    AmountCurrency = request.AmountCurrency,
                    Reason = request.Reason
                }
            };

            // Note: DonationId would typically be passed separately or handled differently
            // For now, we'll skip relationships since the request doesn't have DonationId

            return new JsonApiRequest<RefundCreateDto> { Data = dto };
        }

        #endregion

        #region PaymentSource Mapping

        /// <summary>
        /// Maps a PaymentSourceDto to a PaymentSource domain model.
        /// </summary>
        public static PaymentSource MapToDomain(PaymentSourceDto dto)
        {
            return new PaymentSource
            {
                Id = dto.Id,
                Name = dto.Attributes.Name,
                PaymentMethodType = dto.Attributes.PaymentMethodType,
                PaymentLast4 = dto.Attributes.PaymentLast4, // Updated property name
                PaymentBrand = dto.Attributes.PaymentBrand,
                ExpirationMonth = dto.Attributes.ExpirationMonth,
                ExpirationYear = dto.Attributes.ExpirationYear,
                Verified = dto.Attributes.Verified,
                PersonId = dto.Relationships?.Person?.Id,
                CreatedAt = dto.Attributes.CreatedAt,
                UpdatedAt = dto.Attributes.UpdatedAt,
                DataSource = "Giving"
            };
        }

        #endregion
    }

    #region Create/Update DTOs

    // Donation Create/Update DTOs
    public class DonationCreateDto
    {
        public string Type { get; set; } = "Donation";
        public DonationCreateAttributesDto Attributes { get; set; } = new();
        public DonationCreateRelationshipsDto? Relationships { get; set; }
    }

    public class DonationCreateAttributesDto
    {
        public long AmountCents { get; set; }
        public string AmountCurrency { get; set; } = "USD";
        public string? PaymentMethod { get; set; }
        public string? PaymentLast4 { get; set; } // Updated property name
        public string? PaymentBrand { get; set; }
        public long? FeeCents { get; set; }
        public string? PaymentCheckNumber { get; set; }
        public System.DateTime? PaymentCheckDatedAt { get; set; }
        public System.DateTime? ReceivedAt { get; set; }
    }

    public class DonationCreateRelationshipsDto
    {
        public RelationshipData? Person { get; set; }
        public RelationshipData? Batch { get; set; }
        public RelationshipData? Campus { get; set; }
        public RelationshipData? PaymentSource { get; set; }
    }

    public class DonationUpdateDto
    {
        public string Type { get; set; } = "Donation";
        public string Id { get; set; } = string.Empty;
        public DonationUpdateAttributesDto Attributes { get; set; } = new();
        public DonationUpdateRelationshipsDto? Relationships { get; set; }
    }

    public class DonationUpdateAttributesDto
    {
        public long? AmountCents { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentCheckNumber { get; set; }
        public System.DateTime? PaymentCheckDatedAt { get; set; }
        public System.DateTime? ReceivedAt { get; set; }
    }

    public class DonationUpdateRelationshipsDto
    {
        public RelationshipData? Batch { get; set; }
    }

    // Fund Create/Update DTOs
    public class FundCreateDto
    {
        public string Type { get; set; } = "Fund";
        public FundCreateAttributesDto Attributes { get; set; } = new();
    }

    public class FundCreateAttributesDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Code { get; set; }
        public bool Visibility { get; set; } = true;
        public bool Default { get; set; }
        public string? Color { get; set; }
    }

    public class FundUpdateDto
    {
        public string Type { get; set; } = "Fund";
        public string Id { get; set; } = string.Empty;
        public FundUpdateAttributesDto Attributes { get; set; } = new();
    }

    public class FundUpdateAttributesDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Code { get; set; }
        public bool? Visibility { get; set; }
        public bool? Default { get; set; }
        public string? Color { get; set; }
    }

    // Batch Create/Update DTOs
    public class BatchCreateDto
    {
        public string Type { get; set; } = "Batch";
        public BatchCreateAttributesDto Attributes { get; set; } = new();
        public BatchCreateRelationshipsDto? Relationships { get; set; }
    }

    public class BatchCreateAttributesDto
    {
        public string? Description { get; set; }
    }

    public class BatchCreateRelationshipsDto
    {
        public RelationshipData? Owner { get; set; }
    }

    public class BatchUpdateDto
    {
        public string Type { get; set; } = "Batch";
        public string Id { get; set; } = string.Empty;
        public BatchUpdateAttributesDto Attributes { get; set; } = new();
        public BatchUpdateRelationshipsDto? Relationships { get; set; }
    }

    public class BatchUpdateAttributesDto
    {
        public string? Description { get; set; }
    }

    public class BatchUpdateRelationshipsDto
    {
        public RelationshipData? Owner { get; set; }
    }

    // Pledge Create/Update DTOs
    public class PledgeCreateDto
    {
        public string Type { get; set; } = "Pledge";
        public PledgeCreateAttributesDto Attributes { get; set; } = new();
        public PledgeCreateRelationshipsDto? Relationships { get; set; }
    }

    public class PledgeCreateAttributesDto
    {
        public long AmountCents { get; set; }
        public string AmountCurrency { get; set; } = "USD";
        public long? JointGiverAmountCents { get; set; }
    }

    public class PledgeCreateRelationshipsDto
    {
        public RelationshipData? Person { get; set; }
        public RelationshipData? Fund { get; set; }
        public RelationshipData? PledgeCampaign { get; set; }
        public RelationshipData? JointGiver { get; set; }
    }

    public class PledgeUpdateDto
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = "Pledge";
        public PledgeUpdateAttributesDto Attributes { get; set; } = new();
    }

    public class PledgeUpdateAttributesDto
    {
        public long AmountCents { get; set; }
        public string AmountCurrency { get; set; } = "USD";
        public string? Status { get; set; }
        public long? JointGiverAmountCents { get; set; }
    }

    // RecurringDonation Create/Update DTOs
    public class RecurringDonationCreateDto
    {
        public string Type { get; set; } = "RecurringDonation";
        public RecurringDonationCreateAttributesDto Attributes { get; set; } = new();
        public RecurringDonationCreateRelationshipsDto? Relationships { get; set; }
    }

    public class RecurringDonationCreateAttributesDto
    {
        public long AmountCents { get; set; }
        public string AmountCurrency { get; set; } = "USD";
        public string? Schedule { get; set; }
        public DateTime? NextOccurrence { get; set; }
    }

    public class RecurringDonationCreateRelationshipsDto
    {
        public RelationshipData? Person { get; set; }
        public RelationshipData? PaymentSource { get; set; }
    }

    public class RecurringDonationUpdateDto
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = "RecurringDonation";
        public RecurringDonationUpdateAttributesDto Attributes { get; set; } = new();
    }

    public class RecurringDonationUpdateAttributesDto
    {
        public long AmountCents { get; set; }
        public string AmountCurrency { get; set; } = "USD";
        public string? Schedule { get; set; }
        public DateTime? NextOccurrence { get; set; }
        public string? Status { get; set; }
    }

    // Refund Create DTOs
    public class RefundCreateDto
    {
        public string Type { get; set; } = "Refund";
        public RefundCreateAttributesDto Attributes { get; set; } = new();
        public RefundCreateRelationshipsDto? Relationships { get; set; }
    }

    public class RefundCreateAttributesDto
    {
        public long AmountCents { get; set; }
        public string AmountCurrency { get; set; } = "USD";
        public long? FeeCents { get; set; }
        public string? Reason { get; set; }
    }

    public class RefundCreateRelationshipsDto
    {
        public RelationshipData? Donation { get; set; }
    }

    #endregion
}