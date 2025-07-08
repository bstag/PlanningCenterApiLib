namespace PlanningCenterApiLib.Calendar;

public class EventTime
{
    public string Id { get; set; }
    public DateTime EndsAt { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime Name { get; set; }
    public bool VisibleOnKiosks { get; set; }
    public bool VisibleOnWidgetAndIcal { get; set; }
}