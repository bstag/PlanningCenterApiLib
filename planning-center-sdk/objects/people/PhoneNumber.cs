namespace PlanningCenterApiLib.People;

public class PhoneNumber
{
    public string Id { get; set; }
    public string Number { get; set; }
    public string Carrier { get; set; }
    public string Location { get; set; }
    public bool Primary { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string E164 { get; set; }
    public string International { get; set; }
    public string National { get; set; }
    public string CountryCode { get; set; }
    public string FormattedNumber { get; set; }
}