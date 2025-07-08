namespace PlanningCenterApiLib.CheckIns;

public class Event
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Frequency { get; set; }
    public bool EnableServicesIntegration { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime ArchivedAt { get; set; }
    public string IntegrationKey { get; set; }
    public bool LocationTimesEnabled { get; set; }
    public bool PreSelectEnabled { get; set; }
    public string AppSource { get; set; }
}