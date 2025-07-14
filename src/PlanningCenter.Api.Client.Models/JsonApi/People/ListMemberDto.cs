using System;
using PlanningCenter.Api.Client.Models.JsonApi.Core;

namespace PlanningCenter.Api.Client.Models.JsonApi.People
{
    /// <summary>
    /// ListMember DTO for JSON:API responses.
    /// </summary>
    public class ListMemberDto
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = "ListMember";
        public ListMemberAttributesDto Attributes { get; set; } = new();
        public ListMemberRelationshipsDto? Relationships { get; set; }
    }

    /// <summary>
    /// ListMember attributes DTO.
    /// </summary>
    public class ListMemberAttributesDto
    {
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// ListMember relationships DTO.
    /// </summary>
    public class ListMemberRelationshipsDto
    {
        public RelationshipData? Person { get; set; }
        public RelationshipData? List { get; set; }
    }
}
