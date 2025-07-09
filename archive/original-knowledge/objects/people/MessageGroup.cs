namespace PlanningCenterApiLib.People;

public class MessageGroup
{
    public string Id { get; set; }
    public string Uuid { get; set; }
    public string MessageType { get; set; }
    public string FromAddress { get; set; }
    public string Subject { get; set; }
    public int MessageCount { get; set; }
    public bool SystemMessage { get; set; }
    public bool TransactionalMessage { get; set; }
    public bool ContainsUserGeneratedContent { get; set; }
    public DateTime CreatedAt { get; set; }
    public string ReplyToName { get; set; }
    public string ReplyToAddress { get; set; }
}