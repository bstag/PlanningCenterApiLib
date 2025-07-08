namespace PlanningCenterApiLib.Calendar;

public class Resource
{
    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Kind { get; set; }
    public string Name { get; set; }
    public string SerialNumber { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Description { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string HomeLocation { get; set; }
    public string Image { get; set; }
    public int Quantity { get; set; }
    public string PathName { get; set; }
}