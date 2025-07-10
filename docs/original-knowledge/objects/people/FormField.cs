namespace PlanningCenterApiLib.People;

public class FormField
{
    public string Id { get; set; }
    public string Label { get; set; }
    public string Description { get; set; }
    public bool Required { get; set; }
    public string Settings { get; set; }
    public string FieldType { get; set; }
    public int Sequence { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}