namespace PlanningCenterApiLib.Giving;

public class BatchGroup
{
    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Description { get; set; }
    public bool Committed { get; set; }
    public int TotalCents { get; set; }
    public string TotalCurrency { get; set; }
    public string Status { get; set; }
}