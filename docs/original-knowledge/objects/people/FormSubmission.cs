namespace PlanningCenterApiLib.People;

public class FormSubmission
{
    public string Id { get; set; }
    public bool Verified { get; set; }
    public bool RequiresVerification { get; set; }
    public DateTime CreatedAt { get; set; }
}