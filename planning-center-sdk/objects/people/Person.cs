namespace PlanningCenterApiLib.People;

public class Person
{
    public string Id { get; set; }
    public string Avatar { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string DemographicAvatarUrl { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
    public int RemoteId { get; set; }
    public bool AccountingAdministrator { get; set; }
    public DateTime Anniversary { get; set; }
    public DateTime Birthdate { get; set; }
    public bool Child { get; set; }
    public string GivenName { get; set; }
    public int Grade { get; set; }
    public int GraduationYear { get; set; }
    public string MiddleName { get; set; }
    public string Nickname { get; set; }
    public string PeoplePermissions { get; set; }
    public bool SiteAdministrator { get; set; }
    public string Gender { get; set; }
    public DateTime InactivatedAt { get; set; }
    public string MedicalNotes { get; set; }
    public string Membership { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool CanCreateForms { get; set; }
    public bool CanEmailLists { get; set; }
    public object DirectorySharedInfo { get; set; } // This is a hash/object in JSON
    public string DirectoryStatus { get; set; }
    public bool PassedBackgroundCheck { get; set; }
    public object ResourcePermissionFlags { get; set; } // This is a hash/object in JSON
    public string SchoolType { get; set; }
    public string LoginIdentifier { get; set; }
    public bool MfaConfigured { get; set; }
    public string StripeCustomerIdentifier { get; set; }
}