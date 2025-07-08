namespace PlanningCenterApiLib.Calendar;

public class Person
{
    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string AvatarUrl { get; set; }
    public bool Child { get; set; }
    public string ContactData { get; set; }
    public string Gender { get; set; }
    public bool HasAccess { get; set; }
    public string NamePrefix { get; set; }
    public string NameSuffix { get; set; }
    public int PendingRequestCount { get; set; }
    public int Permissions { get; set; }
    public bool ResolvesConflicts { get; set; }
    public bool SiteAdministrator { get; set; }
    public string Status { get; set; }
    public bool CanEditPeople { get; set; }
    public bool CanEditResources { get; set; }
    public bool CanEditRooms { get; set; }
    public string EventPermissionsType { get; set; }
    public bool MemberOfApprovalGroups { get; set; }
    public string Name { get; set; }
    public string PeoplePermissionsType { get; set; }
    public string RoomPermissionsType { get; set; }
    public string ResourcesPermissionsType { get; set; }
}