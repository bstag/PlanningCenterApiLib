namespace PlanningCenterApiLib.Calendar;

public class ResourceBooking
{
    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime EndsAt { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int Quantity { get; set; }
}