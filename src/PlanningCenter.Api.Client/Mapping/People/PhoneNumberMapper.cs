using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.JsonApi.People;
using PlanningCenter.Api.Client.Models.People;
using PlanningCenter.Api.Client.Models.Requests;

namespace PlanningCenter.Api.Client.Mapping.People
{
    /// <summary>
    /// Mapper for PhoneNumber entities between domain models, DTOs, and request models.
    /// </summary>
    public static class PhoneNumberMapper
    {
        /// <summary>
        /// Maps a PhoneNumberDto to a PhoneNumber domain model.
        /// </summary>
        public static PhoneNumber MapToDomain(PhoneNumberDto dto)
        {
            return new PhoneNumber
            {
                Id = dto.Id,
                Number = dto.Attributes.Number ?? string.Empty,
                Location = dto.Attributes.Location ?? "Mobile",
                IsPrimary = dto.Attributes.Primary,
                CanReceiveSms = dto.Attributes.Carrier,
                CreatedAt = dto.Attributes.CreatedAt,
                UpdatedAt = dto.Attributes.UpdatedAt,
                DataSource = "People"
            };
        }
        
        /// <summary>
        /// Maps a PhoneNumberCreateRequest to a PhoneNumberCreateDto for JSON:API.
        /// </summary>
        public static PhoneNumberCreateAttributesDto MapToCreateAttributes(PhoneNumberCreateRequest request)
        {
            return new PhoneNumberCreateAttributesDto
            {
                Number = request.Number,
                Location = request.Location,
                Primary = request.IsPrimary,
                Carrier = request.CanReceiveSms
            };
        }
        
        /// <summary>
        /// Maps a PhoneNumberUpdateRequest to a PhoneNumberUpdateDto for JSON:API.
        /// </summary>
        public static PhoneNumberUpdateAttributesDto MapToUpdateAttributes(PhoneNumberUpdateRequest request)
        {
            return new PhoneNumberUpdateAttributesDto
            {
                Number = request.Number,
                Location = request.Location,
                Primary = request.IsPrimary,
                Carrier = request.CanReceiveSms
            };
        }
    }
}
