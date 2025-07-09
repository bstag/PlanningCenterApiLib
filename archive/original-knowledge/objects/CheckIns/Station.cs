namespace PlanningCenterApiLib.CheckIns;

public class Station
{
    public string Id { get; set; }
    public bool Online { get; set; }
    public int Mode { get; set; }
    public string Name { get; set; }
    public int TimeoutSeconds { get; set; }
    public string InputType { get; set; }
    public string InputTypeOptions { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime NextShowsAt { get; set; }
    public bool OpenForCheckIn { get; set; }
    public DateTime ClosesAt { get; set; }
    public int CheckInCount { get; set; }
}