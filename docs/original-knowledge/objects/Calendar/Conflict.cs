namespace PlanningCenterApiLib.Calendar;

public class Conflict
{
    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Note { get; set; }
    public DateTime ResolvedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}