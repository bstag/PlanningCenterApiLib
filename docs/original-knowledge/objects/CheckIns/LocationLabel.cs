namespace PlanningCenterApiLib.CheckIns;

public class LocationLabel
{
    public string Id { get; set; }
    public int Quantity { get; set; }
    public bool ForRegular { get; set; }
    public bool ForGuest { get; set; }
    public bool ForVolunteer { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}