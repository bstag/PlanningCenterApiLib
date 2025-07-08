namespace PlanningCenterApiLib.CheckIns;

public class Organization
{
    public string Id { get; set; }
    public string DateFormatPattern { get; set; }
    public string TimeZone { get; set; }
    public string Name { get; set; }
    public int DailyCheckIns { get; set; }
    public string AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}