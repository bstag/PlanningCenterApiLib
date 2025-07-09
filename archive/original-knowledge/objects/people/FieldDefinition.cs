namespace PlanningCenterApiLib.People;

public class FieldDefinition
{
    public string Id { get; set; }
    public string DataType { get; set; }
    public string Name { get; set; }
    public int Sequence { get; set; }
    public string Slug { get; set; }
    public string Config { get; set; }
    public DateTime DeletedAt { get; set; }
    public string TabId { get; set; }
}