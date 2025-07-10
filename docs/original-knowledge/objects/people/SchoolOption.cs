namespace PlanningCenterApiLib.People;

public class SchoolOption
{
    public string Id { get; set; }
    public string Value { get; set; }
    public int Sequence { get; set; }
    public string BeginningGrade { get; set; }
    public string EndingGrade { get; set; }
    public List<object> SchoolTypes { get; set; } // Assuming school_types is a list of objects
}