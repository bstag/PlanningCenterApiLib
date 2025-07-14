namespace PlanningCenter.Api.Client.Models.JsonApi.People
{
    /// <summary>
    /// Address update DTO for JSON:API requests.
    /// </summary>
    public class AddressUpdateDto
    {
        public string Type { get; set; } = "Address";
        public string Id { get; set; } = string.Empty;
        public AddressUpdateAttributesDto Attributes { get; set; } = new();
    }

    /// <summary>
    /// Address update attributes DTO.
    /// </summary>
    public class AddressUpdateAttributesDto
    {
        public string? Street { get; set; }
        public string? Street2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Zip { get; set; }
        public string? Country { get; set; }
        public string? Location { get; set; }
        public bool? Primary { get; set; }
    }
}
