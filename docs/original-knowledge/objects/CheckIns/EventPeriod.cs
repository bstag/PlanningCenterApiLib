namespace PlanningCenterApiLib.CheckIns;

public class EventPeriod
{
    public string Id { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public int RegularCount { get; set; }
    public int GuestCount { get; set; }
    public int VolunteerCount { get; set; }
    public string Note { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}