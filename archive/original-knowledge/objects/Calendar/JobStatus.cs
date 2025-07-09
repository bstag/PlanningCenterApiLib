namespace PlanningCenterApiLib.Calendar;

public class JobStatus
{
    public string Id { get; set; }
    public int Retries { get; set; }
    public object Errors { get; set; } // JSON object
    public string Message { get; set; }
    public DateTime StartedAt { get; set; }
    public string Status { get; set; }
}