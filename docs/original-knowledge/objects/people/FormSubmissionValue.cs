namespace PlanningCenterApiLib.People;

public class FormSubmissionValue
{
    public string Id { get; set; }
    public string DisplayValue { get; set; }
    public List<object> Attachments { get; set; } // Assuming attachments is a list of objects
}