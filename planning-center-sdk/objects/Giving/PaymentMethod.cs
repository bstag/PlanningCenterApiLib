namespace PlanningCenterApiLib.Giving;

public class PaymentMethod
{
    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string MethodType { get; set; }
    public string MethodSubtype { get; set; }
    public string Last4 { get; set; }
    public string Brand { get; set; }
    public DateTime Expiration { get; set; }
    public bool Verified { get; set; }
}