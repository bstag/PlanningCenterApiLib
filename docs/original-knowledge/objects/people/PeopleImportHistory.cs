namespace PlanningCenterApiLib.People;

public class PeopleImportHistory
{
    public string Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public object ConflictingChanges { get; set; } // This is a hash/object in JSON
    public string Kind { get; set; }
}