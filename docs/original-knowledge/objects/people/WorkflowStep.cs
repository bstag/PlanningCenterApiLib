namespace PlanningCenterApiLib.People;

public class WorkflowStep
{
    public string Id { get; set; }
    public int Sequence { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int ExpectedResponseTimeInDays { get; set; }
    public int AutoSnoozeValue { get; set; }
    public string AutoSnoozeInterval { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int AutoSnoozeDays { get; set; }
    public int MyReadyCardCount { get; set; }
    public int TotalReadyCardCount { get; set; }
    public string DefaultAssigneeId { get; set; }
}