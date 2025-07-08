namespace PlanningCenterApiLib.Calendar;

public class EventResourceRequest
{
    public string Id { get; set; }
    public bool ApprovalSent { get; set; }
    public string ApprovalStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Notes { get; set; }
    public int Quantity { get; set; }
}