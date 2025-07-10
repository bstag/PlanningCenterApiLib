namespace PlanningCenterApiLib.Giving;

public class Batch
{
    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CommittedAt { get; set; }
    public string Description { get; set; }
    public int DonationsCount { get; set; }
    public int TotalCents { get; set; }
    public string TotalCurrency { get; set; }
    public string Status { get; set; }
}