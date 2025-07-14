namespace PlanningCenter.Api.Client.Models.Requests;

/// <summary>
/// Request model for creating a plan item.
/// Follows Single Responsibility Principle - handles only item creation data.
/// </summary>
public class ItemCreateRequest
{
    /// <summary>
    /// Gets or sets the item title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the sequence/order in the plan.
    /// </summary>
    public int Sequence { get; set; }

    /// <summary>
    /// Gets or sets the item type.
    /// </summary>
    public string? ItemType { get; set; }

    /// <summary>
    /// Gets or sets the item description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the key signature.
    /// </summary>
    public string? KeyName { get; set; }

    /// <summary>
    /// Gets or sets the length in minutes.
    /// </summary>
    public int? Length { get; set; }

    /// <summary>
    /// Gets or sets the service position.
    /// </summary>
    public string? ServicePosition { get; set; }

    /// <summary>
    /// Gets or sets the song ID if this item is a song.
    /// </summary>
    public string? SongId { get; set; }

    /// <summary>
    /// Gets or sets the arrangement ID.
    /// </summary>
    public string? ArrangementId { get; set; }
}

/// <summary>
/// Request model for updating a plan item.
/// </summary>
public class ItemUpdateRequest
{
    /// <summary>
    /// Gets or sets the item title.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the sequence/order in the plan.
    /// </summary>
    public int? Sequence { get; set; }

    /// <summary>
    /// Gets or sets the item description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the key signature.
    /// </summary>
    public string? KeyName { get; set; }

    /// <summary>
    /// Gets or sets the length in minutes.
    /// </summary>
    public int? Length { get; set; }

    /// <summary>
    /// Gets or sets the service position.
    /// </summary>
    public string? ServicePosition { get; set; }
}