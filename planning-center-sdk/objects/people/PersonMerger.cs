namespace PlanningCenterApiLib.People;

public class PersonMerger
{
    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string PersonToKeepId { get; set; }
    public string PersonToRemoveId { get; set; }
}