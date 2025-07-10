namespace PlanningCenterApiLib.CheckIns;

public class Person
{
    public string Id { get; set; }
    public List<object> Addresses { get; set; } // Assuming addresses is a list of objects
    public List<object> EmailAddresses { get; set; } // Assuming email_addresses is a list of objects
    public List<object> PhoneNumbers { get; set; } // Assuming phone_numbers is a list of objects
    public string AvatarUrl { get; set; }
    public string NamePrefix { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string NameSuffix { get; set; }
    public DateTime Birthdate { get; set; }
    public int Grade { get; set; }
    public string Gender { get; set; }
    public string MedicalNotes { get; set; }
    public bool Child { get; set; }
    public string Permission { get; set; }
    public bool Headcounter { get; set; }
    public DateTime LastCheckedInAt { get; set; }
    public int CheckInCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool PassedBackgroundCheck { get; set; }
    public string DemographicAvatarUrl { get; set; }
    public string Name { get; set; }
    public string TopPermission { get; set; }
    public bool IgnoreFilters { get; set; }
}