namespace PlanningCenterApiLib.Giving;

public class Refund
{
    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int AmountCents { get; set; }
    public string AmountCurrency { get; set; }
    public int FeeCents { get; set; }
    public DateTime RefundedAt { get; set; }
    public string FeeCurrency { get; set; }
}