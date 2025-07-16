namespace PlanningCenter.Api.Client.Models.Requests;

/// <summary>
/// Request model for updating a batch.
/// </summary>
public class BatchUpdateRequest
{
    /// <summary>
    /// Gets or sets the batch description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the owner ID.
    /// </summary>
    public string? OwnerId { get; set; }
}