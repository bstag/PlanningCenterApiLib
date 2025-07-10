namespace PlanningCenterApiLib.Registrations;

public class SignupLocation
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string AddressData { get; set; }
    public string Subpremise { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }
    public string LocationType { get; set; }
    public string Url { get; set; }
    public string FormattedAddress { get; set; }
    public string FullFormattedAddress { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}