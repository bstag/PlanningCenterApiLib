namespace PlanningCenterApiLib.People;

public class Message
{
    public string Id { get; set; }
    public string Kind { get; set; }
    public string ToAddresses { get; set; }
    public string Subject { get; set; }
    public string DeliveryStatus { get; set; }
    public string RejectReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime BouncedAt { get; set; }
    public DateTime RejectionNotificationSentAt { get; set; }
    public string FromName { get; set; }
    public string FromAddress { get; set; }
    public DateTime ReadAt { get; set; }
    public string AppName { get; set; }
    public string MessageType { get; set; }
    public string File { get; set; }
}