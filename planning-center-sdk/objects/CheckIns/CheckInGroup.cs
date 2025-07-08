namespace PlanningCenterApiLib.CheckIns;

public class CheckInGroup
{
    public string Id { get; set; }
    public int NameLabelsCount { get; set; }
    public int SecurityLabelsCount { get; set; }
    public int CheckInsCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string PrintStatus { get; set; }
}