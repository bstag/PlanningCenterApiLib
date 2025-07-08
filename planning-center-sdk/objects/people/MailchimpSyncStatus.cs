namespace PlanningCenterApiLib.People;

public class MailchimpSyncStatus
{
    public string Id { get; set; }
    public string Status { get; set; }
    public string Error { get; set; }
    public int Progress { get; set; }
    public DateTime CompletedAt { get; set; }
    public int SegmentId { get; set; }
}