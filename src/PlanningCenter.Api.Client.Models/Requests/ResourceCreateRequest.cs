using System;

namespace PlanningCenter.Api.Client.Models.Requests;

/// <summary>
/// Request model for creating a resource.
/// Follows Single Responsibility Principle - handles only resource creation data.
/// </summary>
public class ResourceCreateRequest
{
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
    public bool Expires { get; set; } = false;

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
}

/// <summary>
/// Request model for updating a resource.
/// </summary>
public class ResourceUpdateRequest
{
    /// <summary>
    /// Gets or sets the resource name.
    /// </summary>
    public string? Name { get; set; }

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
    public bool? Expires { get; set; }

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
}