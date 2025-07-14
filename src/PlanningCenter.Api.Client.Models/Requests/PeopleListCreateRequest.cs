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

    /// <summary>
    /// Request model for updating a people list.
    /// </summary>
    public class PeopleListUpdateRequest
    {
        /// <summary>
        /// Gets or sets the name of the list.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the list.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets whether the list is public.
        /// </summary>
        public bool? IsPublic { get; set; }
    }

    /// <summary>
    /// Request model for adding a person to a list.
    /// </summary>
    public class ListMemberCreateRequest
    {
        /// <summary>
        /// Gets or sets the person ID to add to the list.
        /// </summary>
        public string PersonId { get; set; } = string.Empty;
    }
}
