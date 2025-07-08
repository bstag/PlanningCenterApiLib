namespace PlanningCenterApiLib.Giving;

public class Pledge
{
    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int AmountCents { get; set; }
    public string AmountCurrency { get; set; }
    public int JointGiverAmountCents { get; set; }
    public int DonatedTotalCents { get; set; }
    public int JointGiverDonatedTotalCents { get; set; }
}