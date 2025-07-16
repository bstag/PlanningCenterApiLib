namespace PlanningCenter.Api.Client.Models.Requests
{
    /// <summary>
    /// Request model for creating a fund.
    /// </summary>
    public class FundCreateRequest
    {
        /// <summary>
        /// Gets or sets the fund name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the fund description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the fund code.
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// Gets or sets whether the fund is visible to donors.
        /// </summary>
        public bool Visibility { get; set; } = true;

        /// <summary>
        /// Gets or sets whether the fund is the default fund.
        /// </summary>
        public bool Default { get; set; }

        /// <summary>
        /// Gets or sets the fund color for UI display.
        /// </summary>
        public string? Color { get; set; }
    }
}