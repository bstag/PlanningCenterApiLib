namespace PlanningCenter.Api.Client.Models.JsonApi.People
{
    /// <summary>
    /// Address creation DTO for JSON:API requests.
    /// </summary>
    public class AddressCreateDto
    {
        public string Type { get; set; } = "Address";
        public AddressCreateAttributesDto Attributes { get; set; } = new();
    }

    /// <summary>
    /// Address creation attributes DTO.
    /// </summary>
    public class AddressCreateAttributesDto
    {
        public string Street { get; set; } = string.Empty;
        public string? Street2 { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Zip { get; set; } = string.Empty;
        public string Country { get; set; } = "US";
        public string Location { get; set; } = "Home";
        public bool Primary { get; set; }
    }
}
