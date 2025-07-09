namespace PlanningCenterApiLib.Groups;

public class TagGroup
{
    public string Id { get; set; }
    public bool DisplayPublicly { get; set; }
    public bool MultipleOptionsEnabled { get; set; }
    public string Name { get; set; }
    public int Position { get; set; }
}