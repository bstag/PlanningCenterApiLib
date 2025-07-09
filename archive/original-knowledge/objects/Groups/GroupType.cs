namespace PlanningCenterApiLib.Groups;

public class GroupType
{
    public string Id { get; set; }
    public bool ChurchCenterVisible { get; set; }
    public bool ChurchCenterMapVisible { get; set; }
    public string Color { get; set; }
    public string DefaultGroupSettings { get; set; }
    public string Description { get; set; }
    public string Name { get; set; }
    public int Position { get; set; }
}