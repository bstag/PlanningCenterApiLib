namespace PlanningCenterApiLib.CheckIns;

public class PersonEvent
{
    public string Id { get; set; }
    public int CheckInCount { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}