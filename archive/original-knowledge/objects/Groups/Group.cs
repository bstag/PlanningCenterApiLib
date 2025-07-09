namespace PlanningCenterApiLib.Groups;

public class Group
{
    public string Id { get; set; }
    public DateTime ArchivedAt { get; set; }
    public bool CanCreateConversation { get; set; }
    public bool ChatEnabled { get; set; }
    public string ContactEmail { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Description { get; set; }
    public string EventsVisibility { get; set; }
    public object HeaderImage { get; set; } // JSON object
    public bool LeadersCanSearchPeopleDatabase { get; set; }
    public string LocationTypePreference { get; set; }
    public bool MembersAreConfidential { get; set; }
    public int MembershipsCount { get; set; }
    public string Name { get; set; }
    public string PublicChurchCenterWebUrl { get; set; }
    public string Schedule { get; set; }
    public int TagIds { get; set; }
    public string VirtualLocationUrl { get; set; }
    public object WidgetStatus { get; set; } // JSON object
}