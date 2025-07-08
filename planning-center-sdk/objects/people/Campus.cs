namespace PlanningCenterApiLib.People;

public class Campus
{
    public string Id { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Description { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Zip { get; set; }
    public string Country { get; set; }
    public string PhoneNumber { get; set; }
    public string Website { get; set; }
    public bool TwentyFourHourTime { get; set; }
    public int DateFormat { get; set; }
    public bool ChurchCenterEnabled { get; set; }
    public string ContactEmailAddress { get; set; }
    public string TimeZone { get; set; }
    public bool GeolocationSetManually { get; set; }
    public string TimeZoneRaw { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string AvatarUrl { get; set; }
}