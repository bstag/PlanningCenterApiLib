namespace PlanningCenter.Api.Client.Models.JsonApi.People
{
    /// <summary>
    /// Email update DTO for JSON:API requests.
    /// </summary>
    public class EmailUpdateDto
    {
        public string Type { get; set; } = "Email";
        public string Id { get; set; } = string.Empty;
        public EmailUpdateAttributesDto Attributes { get; set; } = new();
    }

    /// <summary>
    /// Email update attributes DTO.
    /// </summary>
    public class EmailUpdateAttributesDto
    {
        public string? Address { get; set; }
        public string? Location { get; set; }
        public bool? Primary { get; set; }
    }
}
