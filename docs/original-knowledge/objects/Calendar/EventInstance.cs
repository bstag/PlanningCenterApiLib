namespace PlanningCenterApiLib.Calendar;

public class EventInstance
{
    public string Id { get; set; }
    public bool AllDayEvent { get; set; }
    public string CompactRecurrenceDescription { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime EndsAt { get; set; }
    public string Location { get; set; }
    public string Name { get; set; }
    public string Recurrence { get; set; }
    public string RecurrenceDescription { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string ChurchCenterUrl { get; set; }
    public string PublishedStartsAt { get; set; }
    public string PublishedEndsAt { get; set; }
}