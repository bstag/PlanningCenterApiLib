namespace PlanningCenterApiLib.CheckIns;

public class LocationEventPeriod
{
    public string Id { get; set; }
    public int RegularCount { get; set; }
    public int GuestCount { get; set; }
    public int VolunteerCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}