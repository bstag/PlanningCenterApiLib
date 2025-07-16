namespace PlanningCenter.Api.Client.Models.Requests;

/// <summary>
/// Request model for updating a fund.
/// </summary>
public class FundUpdateRequest
{
    /// <summary>
    /// Gets or sets the fund name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the fund description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the fund code.
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Gets or sets whether the fund is visible.
    /// </summary>
    public bool? Visibility { get; set; }

    /// <summary>
    /// Gets or sets whether this is the default fund.
    /// </summary>
    public bool? Default { get; set; }

    /// <summary>
    /// Gets or sets the fund color.
    /// </summary>
    public string? Color { get; set; }
}