using System;
using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.Services;

/// <summary>
/// Represents a service type in Planning Center Services.
/// Follows Single Responsibility Principle - handles only service type data.
/// </summary>
public class ServiceType : PlanningCenterResource
{
    /// <summary>
    /// Gets or sets the data source for the service type.
    /// </summary>
    public new string DataSource { get; set; } = "Services";

    /// <summary>
    /// Gets or sets the service type name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the sequence/sort order.
    /// </summary>
    public int Sequence { get; set; }

    /// <summary>
    /// Gets or sets the creation date.
    /// </summary>
    public new DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update date.
    /// </summary>
    public new DateTime? UpdatedAt { get; set; }
}