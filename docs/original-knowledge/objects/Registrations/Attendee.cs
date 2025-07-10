namespace PlanningCenterApiLib.Registrations;

public class Attendee
{
    public string Id { get; set; }
    public bool Complete { get; set; }
    public bool Active { get; set; }
    public bool Canceled { get; set; }
    public bool Waitlisted { get; set; }
    public DateTime WaitlistedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}