using System;
using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.Calendar;

/// <summary>
/// Represents a resource in Planning Center Calendar.
/// Follows Single Responsibility Principle - handles only resource data.
/// </summary>
public class Resource : PlanningCenterResource
{
    /// <summary>
    /// Gets or sets the data source for the resource.
    /// </summary>
    public string DataSource { get; set; } = "Calendar";

    /// <summary>
    /// Gets or sets the resource name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the resource description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the resource kind.
    /// </summary>
    public string? Kind { get; set; }

    /// <summary>
    /// Gets or sets the home location.
    /// </summary>
    public string? HomeLocation { get; set; }

    /// <summary>
    /// Gets or sets the serial number.
    /// </summary>
    public string? SerialNumber { get; set; }

    /// <summary>
    /// Gets or sets whether the resource expires.
    /// </summary>
    public bool Expires { get; set; }

    /// <summary>
    /// Gets or sets the expiration date.
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets the renewal date.
    /// </summary>
    public DateTime? RenewalAt { get; set; }

    /// <summary>
    /// Gets or sets the quantity available.
    /// </summary>
    public int? Quantity { get; set; }

    /// <summary>
    /// Gets or sets the creation date.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update date.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}