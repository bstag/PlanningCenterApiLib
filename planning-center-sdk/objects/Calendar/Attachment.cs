namespace PlanningCenterApiLib.Calendar;

public class Attachment
{
    public string Id { get; set; }
    public string ContentType { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Description { get; set; }
    public int FileSize { get; set; }
    public string Name { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Url { get; set; }
}