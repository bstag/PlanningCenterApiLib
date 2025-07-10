namespace PlanningCenterApiLib.People;

public class Email
{
    public string Id { get; set; }
    public string Address { get; set; }
    public string Location { get; set; }
    public bool Primary { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool Blocked { get; set; }
}