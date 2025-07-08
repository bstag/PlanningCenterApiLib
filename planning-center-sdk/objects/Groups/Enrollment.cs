namespace PlanningCenterApiLib.Groups;

public class Enrollment
{
    public string Id { get; set; }
    public bool AutoClosed { get; set; }
    public string AutoClosedReason { get; set; }
    public string DateLimit { get; set; }
    public bool DateLimitReached { get; set; }
    public int MemberLimit { get; set; }
    public bool MemberLimitReached { get; set; }
    public string Status { get; set; }
    public string Strategy { get; set; }
}