namespace PlanningCenterApiLib.Giving;

public class InKindDonation
{
    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Description { get; set; }
    public string ExchangeDetails { get; set; }
    public int FairMarketValueCents { get; set; }
    public DateTime ReceivedOn { get; set; }
    public string ValuationDetails { get; set; }
    public DateTime AcknowledgmentLastSentAt { get; set; }
    public string FairMarketValueCurrency { get; set; }
}