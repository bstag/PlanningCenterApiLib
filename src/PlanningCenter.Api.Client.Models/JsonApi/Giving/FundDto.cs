using System;

namespace PlanningCenter.Api.Client.Models.JsonApi.Giving
{
    /// <summary>
    /// JSON:API DTO for Fund entity.
    /// </summary>
    public class FundDto
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = "Fund";
        public FundAttributesDto Attributes { get; set; } = new();
    }

    /// <summary>
    /// Attributes for Fund JSON:API DTO.
    /// </summary>
    public class FundAttributesDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Code { get; set; }
        public bool Visibility { get; set; } = true;
        public bool Default { get; set; }
        public string? Color { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}