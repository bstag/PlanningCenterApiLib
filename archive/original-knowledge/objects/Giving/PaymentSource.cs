namespace PlanningCenterApiLib.Giving;

public class PaymentSource
{
    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Name { get; set; }
    public string PaymentSourceType { get; set; }
}