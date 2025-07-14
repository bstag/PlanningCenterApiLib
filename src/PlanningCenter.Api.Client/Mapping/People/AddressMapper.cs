using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.JsonApi.People;
using PlanningCenter.Api.Client.Models.People;
using PlanningCenter.Api.Client.Models.Requests;

namespace PlanningCenter.Api.Client.Mapping.People
{
    /// <summary>
    /// Mapper for Address entities between domain models, DTOs, and request models.
    /// </summary>
    public static class AddressMapper
    {
        /// <summary>
        /// Maps an AddressDto to an Address domain model.
        /// </summary>
        public static Address MapToDomain(AddressDto dto)
        {
            return new Address
            {
                Id = dto.Id,
                Street = dto.Attributes.Street ?? string.Empty,
                Street2 = dto.Attributes.Street2,
                City = dto.Attributes.City ?? string.Empty,
                State = dto.Attributes.State ?? string.Empty,
                Zip = dto.Attributes.Zip ?? string.Empty,
                Country = dto.Attributes.Country ?? "US",
                Location = dto.Attributes.Location ?? "Home",
                IsPrimary = dto.Attributes.Primary,
                CreatedAt = dto.Attributes.CreatedAt,
                UpdatedAt = dto.Attributes.UpdatedAt,
                DataSource = "People"
            };
        }
        
        /// <summary>
        /// Maps an AddressCreateRequest to an AddressCreateDto for JSON:API.
        /// </summary>
        public static AddressCreateAttributesDto MapToCreateAttributes(AddressCreateRequest request)
        {
            return new AddressCreateAttributesDto
            {
                Street = request.Street,
                Street2 = request.Street2,
                City = request.City,
                State = request.State,
                Zip = request.Zip,
                Country = request.Country,
                Location = request.Location,
                Primary = request.IsPrimary
            };
        }
        
        /// <summary>
        /// Maps an AddressUpdateRequest to an AddressUpdateDto for JSON:API.
        /// </summary>
        public static AddressUpdateAttributesDto MapToUpdateAttributes(AddressUpdateRequest request)
        {
            return new AddressUpdateAttributesDto
            {
                Street = request.Street,
                Street2 = request.Street2,
                City = request.City,
                State = request.State,
                Zip = request.Zip,
                Country = request.Country,
                Location = request.Location,
                Primary = request.IsPrimary
            };
        }
    }
}
