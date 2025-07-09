namespace PlanningCenterApiLib.People;

public class NoteCategory
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool Locked { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string OrganizationId { get; set; }
}