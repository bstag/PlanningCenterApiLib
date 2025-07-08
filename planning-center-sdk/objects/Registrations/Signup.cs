namespace PlanningCenterApiLib.Registrations;

public class Signup
{
    public string Id { get; set; }
    public bool Archived { get; set; }
    public DateTime CloseAt { get; set; }
    public string Description { get; set; }
    public string LogoUrl { get; set; }
    public string Name { get; set; }
    public string NewRegistrationUrl { get; set; }
    public DateTime OpenAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}