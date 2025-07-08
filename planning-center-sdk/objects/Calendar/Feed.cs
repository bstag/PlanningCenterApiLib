namespace PlanningCenterApiLib.Calendar;

public class Feed
{
    public string Id { get; set; }
    public bool CanDelete { get; set; }
    public string DefaultChurchCenterVisibility { get; set; }
    public string FeedType { get; set; }
    public DateTime ImportedAt { get; set; }
    public string Name { get; set; }
    public bool Deleting { get; set; }
    public bool SyncCampusTags { get; set; }
    public string SourceId { get; set; }
}