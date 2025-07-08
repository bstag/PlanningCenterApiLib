namespace PlanningCenterApiLib.People;

public class CustomSender
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string EmailAddress { get; set; }
    public DateTime VerifiedAt { get; set; }
    public DateTime VerificationRequestedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool Verified { get; set; }
    public bool Expired { get; set; }
    public string VerificationStatus { get; set; }
}