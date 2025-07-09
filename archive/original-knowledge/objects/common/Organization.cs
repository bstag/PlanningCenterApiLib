namespace PlanningCenterApiLib.Common;

public class Organization
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string CountryCode { get; set; }
    public int DateFormat { get; set; }
    public string TimeZone { get; set; }
    public string ContactWebsite { get; set; }
    public DateTime CreatedAt { get; set; }
    public string AvatarUrl { get; set; }
    public string ChurchCenterSubdomain { get; set; }
}