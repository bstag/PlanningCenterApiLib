using System.Collections.Generic;
using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.People
{
    /// <summary>
    /// Household creation DTO for JSON:API requests.
    /// </summary>
    public class HouseholdCreateDto
    {
        public string Type { get; set; } = "Household";
        public HouseholdCreateAttributesDto Attributes { get; set; } = new();
        public HouseholdCreateRelationshipsDto? Relationships { get; set; }
    }

    /// <summary>
    /// Household creation attributes DTO.
    /// </summary>
    public class HouseholdCreateAttributesDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Avatar { get; set; }
    }

    /// <summary>
    /// Household creation relationships DTO.
    /// </summary>
    public class HouseholdCreateRelationshipsDto
    {
        public RelationshipData? PrimaryContact { get; set; }
        public RelationshipDataCollection? People { get; set; }
    }
}
