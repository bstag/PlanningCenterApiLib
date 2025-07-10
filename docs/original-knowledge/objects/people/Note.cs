namespace PlanningCenterApiLib.People;

public class Note
{
    public string Id { get; set; }
    public string NoteContent { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime DisplayDate { get; set; }
    public string NoteCategoryId { get; set; }
    public string OrganizationId { get; set; }
    public string PersonId { get; set; }
    public string CreatedById { get; set; }
}