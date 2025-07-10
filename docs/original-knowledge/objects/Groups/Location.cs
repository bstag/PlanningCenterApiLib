namespace PlanningCenterApiLib.Groups;

public class Location
{
    public string Id { get; set; }
    public string DisplayPreference { get; set; }
    public string FullFormattedAddress { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Name { get; set; }
    public string Radius { get; set; }
    public string Strategy { get; set; }
}