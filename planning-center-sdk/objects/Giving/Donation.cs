namespace PlanningCenterApiLib.Giving;

public class Donation
{
    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string PaymentMethodSub { get; set; }
    public string PaymentLast4 { get; set; }
    public string PaymentBrand { get; set; }
    public int PaymentCheckNumber { get; set; }
    public DateTime PaymentCheckDatedAt { get; set; }
    public int FeeCents { get; set; }
    public string PaymentMethod { get; set; }
    public DateTime ReceivedAt { get; set; }
    public int AmountCents { get; set; }
    public string PaymentStatus { get; set; }
    public DateTime CompletedAt { get; set; }
    public bool FeeCovered { get; set; }
    public string AmountCurrency { get; set; }
    public string FeeCurrency { get; set; }
    public bool Refunded { get; set; }
    public bool Refundable { get; set; }
}