using System;
using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.Services;

/// <summary>
/// Represents a plan item in Planning Center Services.
/// Follows Single Responsibility Principle - handles only item data.
/// </summary>
public class Item : PlanningCenterResource
{
    /// <summary>
    /// Gets or sets the data source for the item.
    /// </summary>
    public new string DataSource { get; set; } = "Services";

    /// <summary>
    /// Gets or sets the item title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the sequence/order in the plan.
    /// </summary>
    public int Sequence { get; set; }

    /// <summary>
    /// Gets or sets the item type (song, announcement, etc.).
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
    /// Gets or sets the plan ID this item belongs to.
    /// </summary>
    public string PlanId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the song ID if this item is a song.
    /// </summary>
    public string? SongId { get; set; }

    /// <summary>
    /// Gets or sets the arrangement ID.
    /// </summary>
    public string? ArrangementId { get; set; }

    /// <summary>
    /// Gets or sets the creation date.
    /// </summary>
    public new DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update date.
    /// </summary>
    public new DateTime? UpdatedAt { get; set; }
}