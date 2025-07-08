namespace PlanningCenterApiLib.Giving;

public class PledgeCampaign
{
    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public int GoalCents { get; set; }
    public string GoalCurrency { get; set; }
    public bool ShowGoalInChurchCenter { get; set; }
    public int ReceivedTotalFromPledgesCents { get; set; }
    public int ReceivedTotalOutsideOfPledgesCents { get; set; }
}