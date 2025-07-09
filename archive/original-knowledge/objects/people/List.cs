namespace PlanningCenterApiLib.People;

public class List
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool AutoRefresh { get; set; }
    public string Status { get; set; }
    public bool HasInactiveResults { get; set; }
    public bool IncludeInactive { get; set; }
    public string Returns { get; set; }
    public bool ReturnOriginalIfNone { get; set; }
    public string Subset { get; set; }
    public bool AutomationsActive { get; set; }
    public int AutomationsCount { get; set; }
    public int PausedAutomationsCount { get; set; }
    public string Description { get; set; }
    public bool Invalid { get; set; }
    public string AutoRefreshFrequency { get; set; }
    public string NameOrDescription { get; set; }
    public bool RecentlyViewed { get; set; }
    public DateTime RefreshedAt { get; set; }
    public bool Starred { get; set; }
    public int TotalPeople { get; set; }
    public DateTime BatchCompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}