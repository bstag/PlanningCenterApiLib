namespace PlanningCenterApiLib.People;

public class Workflow
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int MyReadyCardCount { get; set; }
    public int TotalReadyCardCount { get; set; }
    public int CompletedCardCount { get; set; }
    public int TotalCardsCount { get; set; }
    public int TotalReadyAndSnoozedCardCount { get; set; }
    public int TotalStepsCount { get; set; }
    public int TotalUnassignedStepsCount { get; set; }
    public int TotalUnassignedCardCount { get; set; }
    public int TotalOverdueCardCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime DeletedAt { get; set; }
    public DateTime ArchivedAt { get; set; }
    public string CampusId { get; set; }
    public string WorkflowCategoryId { get; set; }
    public int MyOverdueCardCount { get; set; }
    public int MyDueSoonCardCount { get; set; }
    public bool RecentlyViewed { get; set; }
}