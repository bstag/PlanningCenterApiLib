using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;

namespace PlanningCenter.Api.Client.IntegrationTests.Models;

/// <summary>
/// JSON:API response wrapper for single resources.
/// This is a copy of the internal class in PlanningCenter.Api.Client for testing purposes.
/// </summary>
public class JsonApiSingleResponse<T>
{
    public T? Data { get; set; }
    public PagedResponseMeta? Meta { get; set; }
    public PagedResponseLinks? Links { get; set; }
}
