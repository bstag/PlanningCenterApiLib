namespace PlanningCenter.Api.Client.Models.JsonApi.People
{
    /// <summary>
    /// Phone number update DTO for JSON:API requests.
    /// </summary>
    public class PhoneNumberUpdateDto
    {
        public string Type { get; set; } = "PhoneNumber";
        public string Id { get; set; } = string.Empty;
        public PhoneNumberUpdateAttributesDto Attributes { get; set; } = new();
    }

    /// <summary>
    /// Phone number update attributes DTO.
    /// </summary>
    public class PhoneNumberUpdateAttributesDto
    {
        public string? Number { get; set; }
        public string? Location { get; set; }
        public bool? Primary { get; set; }
        public bool? Carrier { get; set; } // SMS capability
    }
}
