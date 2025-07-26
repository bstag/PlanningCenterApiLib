using System.Text.Json.Serialization;

namespace PlanningCenter.Api.Client.Models.JsonApi.Groups
{
    /// <summary>
    /// Represents the header image object for a group, containing various image sizes.
    /// </summary>
    public class HeaderImageDto
    {
        [JsonPropertyName("thumbnail")]
        public string? Thumbnail { get; set; }

        [JsonPropertyName("medium")]
        public string? Medium { get; set; }

        [JsonPropertyName("original")]
        public string? Original { get; set; }
    }
}
