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
        public string StreetLine1 { get; set; } = string.Empty; // Updated property name
        public string? StreetLine2 { get; set; } // Updated property name
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Zip { get; set; } = string.Empty;
        public string CountryCode { get; set; } = "US"; // Updated property name
        public string? CountryName { get; set; } // New field
        public string Location { get; set; } = "Home";
        public bool Primary { get; set; }
    }
}
