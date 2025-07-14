using System;
using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.People
{
    /// <summary>
    /// Household DTO for JSON:API responses.
    /// </summary>
    public class HouseholdDto
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = "Household";
        public HouseholdAttributesDto Attributes { get; set; } = new();

        /// <summary>
        /// Related resource relationships
        /// </summary>
        public HouseholdRelationshipsDto Relationships { get; set; } = new();
    }

    /// <summary>
    /// Household attributes DTO.
    /// </summary>
    public class HouseholdAttributesDto
    {
        public string Name { get; set; } = string.Empty;
        public int MemberCount { get; set; }
        public string? PrimaryContactName { get; set; }
        public string? PrimaryContactId { get; set; }
        public string? Avatar { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Household relationships DTO.
    /// </summary>
    public class HouseholdRelationshipsDto
    {
        /// <summary>
        /// The primary contact for the household.
        /// </summary>
        public RelationshipData? PrimaryContact { get; set; }

        /// <summary>
        /// Collection of household members.
        /// </summary>
        public RelationshipDataCollection? Members { get; set; }
    }
}
