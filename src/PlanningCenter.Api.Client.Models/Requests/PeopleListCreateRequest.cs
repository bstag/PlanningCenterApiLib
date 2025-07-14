namespace PlanningCenter.Api.Client.Models.Requests
{
    /// <summary>
    /// Request model for creating a people list.
    /// </summary>
    public class PeopleListCreateRequest
    {
        /// <summary>
        /// Gets or sets the name of the list.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the list.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets whether the list is public.
        /// </summary>
        public bool IsPublic { get; set; }
    }
}
