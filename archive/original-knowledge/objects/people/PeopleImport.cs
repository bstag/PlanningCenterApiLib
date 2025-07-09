namespace PlanningCenterApiLib.People;

public class PeopleImport
{
    public string Id { get; set; }
    public string Attribs { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime ProcessedAt { get; set; }
    public DateTime UndoneAt { get; set; }
}