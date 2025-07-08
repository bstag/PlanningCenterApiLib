namespace PlanningCenterApiLib.CheckIns;

public class CheckInTime
{
    public string Id { get; set; }
    public string Kind { get; set; }
    public bool HasValidated { get; set; }
    public bool ServicesIntegrated { get; set; }
    public List<object> Alerts { get; set; } // Assuming alerts is a list of objects
}