namespace PlanningCenterApiLib.Calendar;

public class Event
{
    public string Id { get; set; }
    public string ApprovalStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Description { get; set; }
    public bool Featured { get; set; }
    public string ImageUrl { get; set; }
    public string Name { get; set; }
    public int PercentApproved { get; set; }
    public int PercentRejected { get; set; }
    public string RegistrationUrl { get; set; }
    public string Summary { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool VisibleInChurchCenter { get; set; }
}