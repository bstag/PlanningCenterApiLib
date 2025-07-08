namespace PlanningCenterApiLib.Groups;

public class Event
{
    public string Id { get; set; }
    public bool AttendanceRequestsEnabled { get; set; }
    public bool AutomatedReminderEnabled { get; set; }
    public bool Canceled { get; set; }
    public DateTime CanceledAt { get; set; }
    public string Description { get; set; }
    public DateTime EndsAt { get; set; }
    public string LocationTypePreference { get; set; }
    public bool MultiDay { get; set; }
    public string Name { get; set; }
    public bool RemindersSent { get; set; }
    public DateTime RemindersSentAt { get; set; }
    public bool Repeating { get; set; }
    public DateTime StartsAt { get; set; }
    public string VirtualLocationUrl { get; set; }
    public int VisitorsCount { get; set; }
}