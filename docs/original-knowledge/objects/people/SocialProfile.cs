namespace PlanningCenterApiLib.People;

public class SocialProfile
{
    public string Id { get; set; }
    public string Site { get; set; }
    public string Url { get; set; }
    public bool Verified { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}