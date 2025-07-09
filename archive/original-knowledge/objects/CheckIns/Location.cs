namespace PlanningCenterApiLib.CheckIns;

public class Location
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Kind { get; set; }
    public bool Opened { get; set; }
    public string Questions { get; set; }
    public int AgeMinInMonths { get; set; }
    public int AgeMaxInMonths { get; set; }
    public string AgeRangeBy { get; set; }
    public DateTime AgeOn { get; set; }
    public string ChildOrAdult { get; set; }
    public DateTime EffectiveDate { get; set; }
    public string Gender { get; set; }
    public int GradeMin { get; set; }
    public int GradeMax { get; set; }
    public int MaxOccupancy { get; set; }
    public int MinVolunteers { get; set; }
    public int AttendeesPerVolunteer { get; set; }
    public int Position { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Milestone { get; set; }
}