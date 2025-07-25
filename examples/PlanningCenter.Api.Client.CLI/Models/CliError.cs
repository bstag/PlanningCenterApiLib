namespace PlanningCenter.Api.Client.CLI.Models;

/// <summary>
/// Represents an error that occurred during CLI operation
/// </summary>
public class CliError
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Operation { get; set; }
    public string? StackTrace { get; set; }

    public static CliError FromException(Exception ex, string? operation = null)
    {
        return new CliError
        {
            Code = ex.GetType().Name,
            Message = ex.Message,
            Details = ex.InnerException?.Message,
            Operation = operation,
            StackTrace = ex.StackTrace
        };
    }

    public static CliError Create(string code, string message, string? operation = null)
    {
        return new CliError
        {
            Code = code,
            Message = message,
            Operation = operation
        };
    }
}