namespace PlanningCenterApiLib.People;

public class Condition
{
    public string Id { get; set; }
    public string Application { get; set; }
    public string DefinitionClass { get; set; }
    public string Comparison { get; set; }
    public string Settings { get; set; }
    public string DefinitionIdentifier { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}