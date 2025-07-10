namespace PlanningCenterApiLib.Giving;

public class RecurringDonation
{
    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime ReleaseHoldAt { get; set; }
    public int AmountCents { get; set; }
    public string Status { get; set; }
    public DateTime LastDonationReceivedAt { get; set; }
    public DateTime NextOccurrence { get; set; }
    public object Schedule { get; set; } // JSON object
    public string AmountCurrency { get; set; }
}