namespace PlanningCenter.Api.Client.Models;

/// <summary>
/// Query parameters for Planning Center API requests.
/// Supports filtering, sorting, including related data, and pagination options.
/// </summary>
public class QueryParameters
{
    /// <summary>
    /// Filtering conditions for the query.
    /// Key is the field name, value is the filter value or condition.
    /// Example: { ["status"] = "active", ["created_at"] = ">2023-01-01" }
    /// </summary>
    public Dictionary<string, object> Where { get; set; } = new();
    
    /// <summary>
    /// Related data to include in the response.
    /// Example: ["addresses", "emails", "phone_numbers"]
    /// </summary>
    public string[] Include { get; set; } = Array.Empty<string>();
    
    /// <summary>
    /// Field to sort results by. Prefix with '-' for descending order.
    /// Example: "last_name" or "-created_at"
    /// </summary>
    public string? OrderBy { get; set; }
    
    /// <summary>
    /// Number of items to return per page.
    /// If not specified, the API default will be used (typically 25).
    /// </summary>
    public int? PerPage { get; set; }
    
    /// <summary>
    /// Zero-based offset for pagination.
    /// Used internally by pagination helpers - typically you don't need to set this manually.
    /// </summary>
    public int? Offset { get; set; }
    
    /// <summary>
    /// Creates a deep copy of the query parameters.
    /// Useful for creating variations of a query without modifying the original.
    /// </summary>
    /// <returns>A new QueryParameters instance with the same values</returns>
    public QueryParameters Clone()
    {
        return new QueryParameters
        {
            Where = new Dictionary<string, object>(Where),
            Include = Include.ToArray(),
            OrderBy = OrderBy,
            PerPage = PerPage,
            Offset = Offset
        };
    }
    
    /// <summary>
    /// Converts the query parameters to a query string for HTTP requests.
    /// This is used internally by the HTTP client.
    /// </summary>
    /// <returns>URL-encoded query string</returns>
    public string ToQueryString()
    {
        var parameters = new List<string>();
        
        // Add where conditions
        foreach (var (key, value) in Where)
        {
            parameters.Add($"where[{Uri.EscapeDataString(key)}]={Uri.EscapeDataString(value.ToString() ?? "")}");
        }
        
        // Add includes
        if (Include.Length > 0)
        {
            parameters.Add($"include={Uri.EscapeDataString(string.Join(",", Include))}");
        }
        
        // Add order by
        if (!string.IsNullOrEmpty(OrderBy))
        {
            parameters.Add($"order={Uri.EscapeDataString(OrderBy)}");
        }
        
        // Add pagination
        if (PerPage.HasValue)
        {
            parameters.Add($"per_page={PerPage.Value}");
        }
        
        if (Offset.HasValue)
        {
            parameters.Add($"offset={Offset.Value}");
        }
        
        return string.Join("&", parameters);
    }
}