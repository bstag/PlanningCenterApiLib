using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.People
{
    /// <summary>
    /// Household update DTO for JSON:API requests.
    /// </summary>
    public class HouseholdUpdateDto
    {
        public string Type { get; set; } = "Household";
        public string Id { get; set; } = string.Empty;
        public HouseholdUpdateAttributesDto Attributes { get; set; } = new();
        public HouseholdUpdateRelationshipsDto? Relationships { get; set; }
    }

    /// <summary>
    /// Household update attributes DTO.
    /// </summary>
    public class HouseholdUpdateAttributesDto
    {
        public string? Name { get; set; }
        public string? Avatar { get; set; }
    }

    /// <summary>
    /// Household update relationships DTO.
    /// </summary>
    public class HouseholdUpdateRelationshipsDto
    {
        public RelationshipData? PrimaryContact { get; set; }
    }
}
