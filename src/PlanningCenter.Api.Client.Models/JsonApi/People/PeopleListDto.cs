using System;

namespace PlanningCenter.Api.Client.Models.JsonApi.People
{
    /// <summary>
    /// PeopleList DTO for JSON:API responses.
    /// </summary>
    public class PeopleListDto
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = "List";
        public PeopleListAttributesDto Attributes { get; set; } = new();
    }

    /// <summary>
    /// PeopleList attributes DTO.
    /// </summary>
    public class PeopleListAttributesDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int PeopleCount { get; set; }
        
        /// <summary>
        /// Alias for PeopleCount expected by legacy tests.
        /// </summary>
        public int TotalCount { get => PeopleCount; set => PeopleCount = value; }
        public string Status { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
