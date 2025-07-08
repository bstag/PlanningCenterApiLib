namespace PlanningCenterApiLib.People;

public class WorkflowCard
{
    public string Id { get; set; }
    public DateTime SnoozeUntil { get; set; }
    public bool Overdue { get; set; }
    public string Stage { get; set; }
    public int CalculatedDueAtInDaysAgo { get; set; }
    public bool StickyAssignment { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CompletedAt { get; set; }
    public DateTime FlaggedForNotificationAt { get; set; }
    public DateTime RemovedAt { get; set; }
    public DateTime MovedToStepAt { get; set; }
}