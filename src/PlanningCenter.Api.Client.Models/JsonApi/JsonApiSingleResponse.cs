

namespace PlanningCenter.Api.Client.Models.JsonApi
{
    /// <summary>
    /// JSON:API response wrapper for single resources.
    /// </summary>
    public class JsonApiSingleResponse<T>
    {
        public T? Data { get; set; }
        public PagedResponseMeta? Meta { get; set; }
        public PagedResponseLinks? Links { get; set; }
    }
}
