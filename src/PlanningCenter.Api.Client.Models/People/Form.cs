using System;
using System.Collections.Generic;
using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Models.People
{
    /// <summary>
    /// Represents a form in the Planning Center People module.
    /// </summary>
    public class Form : PlanningCenterResource
    {
        /// <summary>
        /// Gets or sets the data source for the form.
        /// </summary>
        public new string DataSource { get; set; } = "People";

        /// <summary>
        /// Gets or sets the name of the form.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the form.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets whether the form is archived.
        /// </summary>
        public bool IsArchived { get; set; }

        /// <summary>
        /// Gets or sets the submission count for the form.
        /// </summary>
        public int SubmissionCount { get; set; }

        /// <summary>
        /// Gets or sets the created at date for the form.
        /// </summary>
        public new DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the updated at date for the form.
        /// </summary>
        public new DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the archived at date for the form.
        /// </summary>
        public DateTime? ArchivedAt { get; set; }
    }

    /// <summary>
    /// Represents a form submission in the Planning Center People module.
    /// </summary>
    public class FormSubmission : PlanningCenterResource
    {
        /// <summary>
        /// Gets or sets the data source for the form submission.
        /// </summary>
        public new string DataSource { get; set; } = "People";

        /// <summary>
        /// Gets or sets the person ID associated with the form submission.
        /// </summary>
        public string? PersonId { get; set; }

        /// <summary>
        /// Gets or sets the form ID associated with the form submission.
        /// </summary>
        public string FormId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the created at date for the form submission.
        /// </summary>
        public new DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the updated at date for the form submission.
        /// </summary>
        public new DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the form fields and their values.
        /// </summary>
        public Dictionary<string, object>? FieldData { get; set; }
    }
}
