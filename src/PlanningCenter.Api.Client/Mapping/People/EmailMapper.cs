using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.JsonApi.People;
using PlanningCenter.Api.Client.Models.People;
using PlanningCenter.Api.Client.Models.Requests;

namespace PlanningCenter.Api.Client.Mapping.People
{
    /// <summary>
    /// Mapper for Email entities between domain models, DTOs, and request models.
    /// </summary>
    public static class EmailMapper
    {
        /// <summary>
        /// Maps an EmailDto to an Email domain model.
        /// </summary>
        public static Email MapToDomain(EmailDto dto)
        {
            return new Email
            {
                Id = dto.Id,
                Address = dto.Attributes.Address ?? string.Empty,
                Location = dto.Attributes.Location ?? "Home",
                IsPrimary = dto.Attributes.Primary,
                CreatedAt = dto.Attributes.CreatedAt,
                UpdatedAt = dto.Attributes.UpdatedAt,
                DataSource = "People"
            };
        }
        
        /// <summary>
        /// Maps an EmailCreateRequest to an EmailCreateDto for JSON:API.
        /// </summary>
        public static EmailCreateAttributesDto MapToCreateAttributes(EmailCreateRequest request)
        {
            return new EmailCreateAttributesDto
            {
                Address = request.Address,
                Location = request.Location,
                Primary = request.IsPrimary
            };
        }
        
        /// <summary>
        /// Maps an EmailUpdateRequest to an EmailUpdateDto for JSON:API.
        /// </summary>
        public static EmailUpdateAttributesDto MapToUpdateAttributes(EmailUpdateRequest request)
        {
            return new EmailUpdateAttributesDto
            {
                Address = request.Address,
                Location = request.Location,
                Primary = request.IsPrimary
            };
        }
    }
}
