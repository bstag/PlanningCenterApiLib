using System;

namespace PlanningCenter.Api.Client.Models.JsonApi
{
    /// <summary>
    /// JSON:API request wrapper for single resources.
    /// </summary>
    public class JsonApiRequest<T>
    {
        public T Data { get; set; } = default!;
    }
}
