using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.JsonApi.Core;
using PlanningCenter.Api.Client.Models.JsonApi.People;
using PlanningCenter.Api.Client.Models.People;
using PlanningCenter.Api.Client.Models.Requests;
using System.Collections.Generic;
using System.Linq;

namespace PlanningCenter.Api.Client.Mapping.People
{
    /// <summary>
    /// Mapper for Household entities between domain models, DTOs, and request models.
    /// </summary>
    public static class HouseholdMapper
    {
        /// <summary>
        /// Maps a HouseholdDto to a Household domain model.
        /// </summary>
        public static Household MapToDomain(HouseholdDto dto)
        {
            return new Household
            {
                Id = dto.Id,
                Name = dto.Attributes.Name ?? string.Empty,
                MemberCount = dto.Attributes.MemberCount,
                PrimaryContactName = dto.Attributes.PrimaryContactName,
                PrimaryContactId = dto.Attributes.PrimaryContactId,
                Avatar = dto.Attributes.Avatar,
                CreatedAt = dto.Attributes.CreatedAt,
                UpdatedAt = dto.Attributes.UpdatedAt,
                DataSource = "People"
            };
        }
        
        /// <summary>
        /// Maps a HouseholdCreateRequest to a HouseholdCreateDto for JSON:API.
        /// </summary>
        public static HouseholdCreateDto MapToCreateDto(HouseholdCreateRequest request)
        {
            var dto = new HouseholdCreateDto
            {
                Type = "Household",
                Attributes = new HouseholdCreateAttributesDto
                {
                    Name = request.Name,
                    Avatar = request.Avatar
                },
                Relationships = new HouseholdCreateRelationshipsDto
                {
                    PrimaryContact = new RelationshipData
                    {
                        Type = "Person",
                        Id = request.PrimaryContactId
                    },
                    People = new RelationshipDataCollection
                    {
                        Data = request.PersonIds.Select(id => new RelationshipData
                        {
                            Type = "Person",
                            Id = id
                        }).ToList()
                    }
                }
            };
            
            return dto;
        }
        
        /// <summary>
        /// Maps a HouseholdUpdateRequest to a HouseholdUpdateDto for JSON:API.
        /// </summary>
        public static HouseholdUpdateDto MapToUpdateDto(string id, HouseholdUpdateRequest request)
        {
            var dto = new HouseholdUpdateDto
            {
                Type = "Household",
                Id = id,
                Attributes = new HouseholdUpdateAttributesDto
                {
                    Name = request.Name,
                    Avatar = request.Avatar
                }
            };
            
            if (!string.IsNullOrEmpty(request.PrimaryContactId))
            {
                dto.Relationships = new HouseholdUpdateRelationshipsDto
                {
                    PrimaryContact = new RelationshipData
                    {
                        Type = "Person",
                        Id = request.PrimaryContactId
                    }
                };
            }
            
            return dto;
        }
    }
}
