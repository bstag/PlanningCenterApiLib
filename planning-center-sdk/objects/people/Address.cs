namespace PlanningCenterApiLib.People;

public class Address
{
    public string Id { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Zip { get; set; }
    public string CountryCode { get; set; }
    public string Location { get; set; }
    public bool Primary { get; set; }
    public string StreetLine1 { get; set; }
    public string StreetLine2 { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CountryName { get; set; }
}