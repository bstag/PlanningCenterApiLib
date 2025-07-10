namespace PlanningCenterApiLib.Calendar;

public class ResourceQuestion
{
    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Kind { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Choices { get; set; }
    public string Description { get; set; }
    public bool MultipleSelect { get; set; }
    public bool Optional { get; set; }
    public int Position { get; set; }
    public string Question { get; set; }
}