namespace PlanningCenterApiLib.People;

public class WorkflowCardActivity
{
    public string Id { get; set; }
    public string Comment { get; set; }
    public string Content { get; set; }
    public string FormSubmissionUrl { get; set; }
    public string AutomationUrl { get; set; }
    public string PersonAvatarUrl { get; set; }
    public string PersonName { get; set; }
    public string ReassignedToAvatarUrl { get; set; }
    public string ReassignedToName { get; set; }
    public string Subject { get; set; }
    public string Type { get; set; }
    public bool ContentIsHtml { get; set; }
    public DateTime CreatedAt { get; set; }
}