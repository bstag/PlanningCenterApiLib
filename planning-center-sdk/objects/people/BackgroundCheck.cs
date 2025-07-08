namespace PlanningCenterApiLib.People;

public class BackgroundCheck
{
    public string Id { get; set; }
    public bool Current { get; set; }
    public string Note { get; set; }
    public DateTime StatusUpdatedAt { get; set; }
    public string ReportUrl { get; set; }
    public DateTime ExpiresOn { get; set; }
    public string Status { get; set; }
    public DateTime CompletedAt { get; set; }
}