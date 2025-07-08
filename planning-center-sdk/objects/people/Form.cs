namespace PlanningCenterApiLib.People;

public class Form
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Active { get; set; }
    public DateTime ArchivedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime DeletedAt { get; set; }
    public int SubmissionCount { get; set; }
    public string PublicUrl { get; set; }
    public bool RecentlyViewed { get; set; }
    public bool Archived { get; set; }
    public bool SendSubmissionNotificationToSubmitter { get; set; }
    public bool LoginRequired { get; set; }
}