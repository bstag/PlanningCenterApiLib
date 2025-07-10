namespace PlanningCenterApiLib.Registrations;

public class SelectionType
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool PubliclyAvailable { get; set; }
    public int PriceCents { get; set; }
    public string PriceCurrency { get; set; }
    public string PriceCurrencySymbol { get; set; }
    public string PriceFormatted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}