namespace PlanningCenterApiLib.Groups;

public class Person
{
    public string Id { get; set; }
    public List<object> Addresses { get; set; } // Assuming array of objects
    public string AvatarUrl { get; set; }
    public bool Child { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<object> EmailAddresses { get; set; } // Assuming array of objects
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Permissions { get; set; }
    public List<object> PhoneNumbers { get; set; } // Assuming array of objects
}