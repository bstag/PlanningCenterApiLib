namespace PlanningCenterApiLib.CheckIns;

public class AttendanceType
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int Limit { get; set; }
}