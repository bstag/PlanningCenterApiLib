namespace PlanningCenterApiLib.Calendar;

public class EventResourceAnswer
{
    public string Id { get; set; }
    public object Answer { get; set; } // JSON object
    public string DbAnswer { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public object Question { get; set; } // JSON object
}