namespace PlanningCenterApiLib.CheckIns;

public class CheckIn
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MedicalNotes { get; set; }
    public int Number { get; set; }
    public string SecurityCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CheckedOutAt { get; set; }
    public DateTime ConfirmedAt { get; set; }
    public string EmergencyContactName { get; set; }
    public string EmergencyContactPhoneNumber { get; set; }
    public bool OneTimeGuest { get; set; }
    public string Kind { get; set; }
}