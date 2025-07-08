namespace PlanningCenterApiLib.Calendar;

public class ResourceApprovalGroup
{
    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Name { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int FormCount { get; set; }
    public int ResourceCount { get; set; }
    public int RoomCount { get; set; }
}