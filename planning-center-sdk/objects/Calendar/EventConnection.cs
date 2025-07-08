namespace PlanningCenterApiLib.Calendar;

public class EventConnection
{
    public string Id { get; set; }
    public string ConnectedToId { get; set; }
    public string ConnectedToName { get; set; }
    public string ConnectedToType { get; set; }
    public string ProductName { get; set; }
    public string ConnectedToUrl { get; set; }
}