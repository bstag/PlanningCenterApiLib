namespace PlanningCenter.Api.Client.Models.JsonApi.People
{
    /// <summary>
    /// Email creation DTO for JSON:API requests.
    /// </summary>
    public class EmailCreateDto
    {
        public string Type { get; set; } = "Email";
        public EmailCreateAttributesDto Attributes { get; set; } = new();
    }

    /// <summary>
    /// Email creation attributes DTO.
    /// </summary>
    public class EmailCreateAttributesDto
    {
        public string Address { get; set; } = string.Empty;
        public string Location { get; set; } = "Home";
        public bool Primary { get; set; }
    }
}
