namespace PlanningCenterApiLib.People;

public class PeopleImportConflict
{
    public string Id { get; set; }
    public string Kind { get; set; }
    public string Name { get; set; }
    public string Message { get; set; }
    public string Data { get; set; }
    public string ConflictingChanges { get; set; }
    public bool Ignore { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}