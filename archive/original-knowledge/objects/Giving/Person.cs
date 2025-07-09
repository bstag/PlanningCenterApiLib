namespace PlanningCenterApiLib.Giving;

public class Person
{
    public string Id { get; set; }
    public string Permissions { get; set; }
    public List<object> EmailAddresses { get; set; } // Assuming array of objects
    public List<object> Addresses { get; set; } // Assuming array of objects
    public List<object> PhoneNumbers { get; set; } // Assuming array of objects
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int DonorNumber { get; set; }
    public DateTime FirstDonatedAt { get; set; }
}