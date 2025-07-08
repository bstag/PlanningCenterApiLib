namespace PlanningCenterApiLib.People;

public class Household
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int MemberCount { get; set; }
    public string PrimaryContactName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Avatar { get; set; }
    public string PrimaryContactId { get; set; }
}