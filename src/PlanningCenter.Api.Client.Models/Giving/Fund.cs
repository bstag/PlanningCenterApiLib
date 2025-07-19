using System;
using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.Giving
{
    /// <summary>
    /// Represents a fund in the Planning Center Giving module.
    /// </summary>
    public class Fund : PlanningCenterResource
    {
        /// <summary>
        /// Gets or sets the data source for the fund.
        /// </summary>
        public new string DataSource { get; set; } = "Giving";

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

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        public new DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the last update date.
        /// </summary>
        public new DateTime? UpdatedAt { get; set; }
    }
}