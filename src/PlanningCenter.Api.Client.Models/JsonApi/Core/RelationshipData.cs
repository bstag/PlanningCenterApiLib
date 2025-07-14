namespace PlanningCenter.Api.Client.Models.JsonApi.Core
{
    /// <summary>
    /// Represents a JSON:API relationship data object.
    /// </summary>
    public class RelationshipData
    {
        /// <summary>
        /// Gets or sets the ID of the related resource.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the type of the related resource.
        /// </summary>
        public string Type { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents a collection of relationship data objects.
    /// </summary>
    public class RelationshipDataCollection
    {
        /// <summary>
        /// Gets or sets the collection of relationship data objects.
        /// </summary>
        public List<RelationshipData> Data { get; set; } = new List<RelationshipData>();
    }
}
