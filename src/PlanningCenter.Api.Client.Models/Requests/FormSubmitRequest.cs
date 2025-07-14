using System.Collections.Generic;

namespace PlanningCenter.Api.Client.Models.Requests
{
    /// <summary>
    /// Request model for submitting a form.
    /// </summary>
    public class FormSubmitRequest
    {
        /// <summary>
        /// Gets or sets the person ID associated with the form submission.
        /// </summary>
        public string? PersonId { get; set; }

        /// <summary>
        /// Gets or sets the form fields and their values.
        /// </summary>
        public Dictionary<string, object> FieldData { get; set; } = new Dictionary<string, object>();
    }
}
