namespace PlanningCenterApiLib.CheckIns;

public class LocationEventTime
{
    public string Id { get; set; }
    public int RegularCount { get; set; }
    public int GuestCount { get; set; }
    public int VolunteerCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}