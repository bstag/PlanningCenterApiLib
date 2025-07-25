using System.Text.Json.Serialization;

namespace PlanningCenter.Api.Client.Models.JsonApi.Core
{
    /// <summary>
    /// Represents a JSON:API relationship data object.
    /// Contains the type and id of a related resource.
    /// </summary>
    public class RelationshipData
    {
        /// <summary>
        /// Gets or sets the ID of the related resource.
        /// This MUST be a string according to JSON:API specification.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the type of the related resource.
        /// This MUST match the type used in the resource object.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the meta information for this relationship data.
        /// Contains non-standard meta-information about the relationship.
        /// </summary>
        [JsonPropertyName("meta")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, object>? Meta { get; set; }

        /// <summary>
        /// Creates a new RelationshipData instance.
        /// </summary>
        public RelationshipData() { }

        /// <summary>
        /// Creates a new RelationshipData instance with the specified type and id.
        /// </summary>
        /// <param name="type">The resource type</param>
        /// <param name="id">The resource id</param>
        public RelationshipData(string type, string id)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }

        /// <summary>
        /// Returns a string representation of the relationship data.
        /// </summary>
        public override string ToString()
        {
            return $"{Type}:{Id}";
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is RelationshipData other)
            {
                return Type == other.Type && Id == other.Id;
            }
            return false;
        }

        /// <summary>
        /// Returns a hash code for the current object.
        /// </summary>
        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Id);
        }
    }

    /// <summary>
    /// Represents a collection of relationship data objects.
    /// Used for to-many relationships.
    /// </summary>
    public class RelationshipDataCollection
    {
        /// <summary>
        /// Gets or sets the collection of relationship data objects.
        /// </summary>
        [JsonPropertyName("data")]
        public List<RelationshipData> Data { get; set; } = new List<RelationshipData>();

        /// <summary>
        /// Gets or sets the meta information for this relationship collection.
        /// </summary>
        [JsonPropertyName("meta")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, object>? Meta { get; set; }

        /// <summary>
        /// Adds a relationship data object to the collection.
        /// </summary>
        /// <param name="relationshipData">The relationship data to add</param>
        public void Add(RelationshipData relationshipData)
        {
            Data.Add(relationshipData ?? throw new ArgumentNullException(nameof(relationshipData)));
        }

        /// <summary>
        /// Adds a relationship data object to the collection using type and id.
        /// </summary>
        /// <param name="type">The resource type</param>
        /// <param name="id">The resource id</param>
        public void Add(string type, string id)
        {
            Data.Add(new RelationshipData(type, id));
        }
    }
}
