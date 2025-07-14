namespace PlanningCenter.Api.Client.Models.JsonApi.People
{
    /// <summary>
    /// Phone number creation DTO for JSON:API requests.
    /// </summary>
    public class PhoneNumberCreateDto
    {
        public string Type { get; set; } = "PhoneNumber";
        public PhoneNumberCreateAttributesDto Attributes { get; set; } = new();
    }

    /// <summary>
    /// Phone number creation attributes DTO.
    /// </summary>
    public class PhoneNumberCreateAttributesDto
    {
        public string Number { get; set; } = string.Empty;
        public string Location { get; set; } = "Mobile";
        public bool Primary { get; set; }
        public bool Carrier { get; set; } = true; // SMS capability
    }
}
