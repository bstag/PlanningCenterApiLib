namespace PlanningCenterApiLib.Giving;

public class Fund
{
    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Name { get; set; }
    public string LedgerCode { get; set; }
    public string Description { get; set; }
    public string Visibility { get; set; }
    public bool Default { get; set; }
    public string Color { get; set; }
    public bool Deletable { get; set; }
}