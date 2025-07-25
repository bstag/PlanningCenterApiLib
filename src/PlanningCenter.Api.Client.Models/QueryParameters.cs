namespace PlanningCenter.Api.Client.Models;

/// <summary>
/// Query parameters for Planning Center API requests.
/// Supports filtering, sorting, including related data, field selection, and pagination options.
/// Fully compliant with JSON:API 1.0 specification.
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
    public string[]? Include { get; set; }
    
    /// <summary>
    /// Field to sort results by. Prefix with '-' for descending order.
    /// Example: "last_name" or "-created_at"
    /// </summary>
    public string? OrderBy { get; set; }
    
    /// <summary>
    /// Sparse fieldsets - specify which fields to include in the response.
    /// Key is the resource type, value is array of field names.
    /// Example: { ["people"] = ["first_name", "last_name"], ["addresses"] = ["street", "city"] }
    /// </summary>
    public Dictionary<string, string[]>? Fields { get; set; }
    
    /// <summary>
    /// Custom parameters that don't fit into standard categories.
    /// These will be added directly to the query string.
    /// </summary>
    public Dictionary<string, object> CustomParameters { get; set; } = new();
    
    /// <summary>
    /// Meta parameters for additional query information.
    /// Example: { ["can_order_by"] = "true", ["can_query_by"] = "true" }
    /// </summary>
    public Dictionary<string, object>? Meta { get; set; }
    
    /// <summary>
    /// Additional filter operators for complex filtering.
    /// </summary>
    public enum FilterOperator
    {
        Equal,
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        Contains,
        StartsWith,
        EndsWith,
        In,
        NotIn
    }
    
    /// <summary>
    /// Complex filter condition with operator support.
    /// </summary>
    public class FilterCondition
    {
        public string Field { get; set; } = string.Empty;
        public object Value { get; set; } = string.Empty;
        public FilterOperator Operator { get; set; } = FilterOperator.Equal;
        
        public FilterCondition() { }
        
        public FilterCondition(string field, object value, FilterOperator op = FilterOperator.Equal)
        {
            Field = field;
            Value = value;
            Operator = op;
        }
    }
    
    /// <summary>
    /// Advanced filter conditions with operator support.
    /// </summary>
    public List<FilterCondition> Filters { get; set; } = new();
    
    /// <summary>
    /// Alias for OrderBy to maintain compatibility with tests
    /// </summary>
    public string? Order
    {
        get => OrderBy;
        set => OrderBy = value;
    }
    
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
    /// Adds a filter condition to the query parameters.
    /// </summary>
    /// <param name="key">The field name to filter by</param>
    /// <param name="value">The value to filter for</param>
    /// <returns>This QueryParameters instance for method chaining</returns>
    public QueryParameters AddFilter(string key, object value)
    {
        Where[key] = value;
        return this;
    }
    
    /// <summary>
    /// Adds an include directive to the query parameters.
    /// </summary>
    /// <param name="include">The related data to include</param>
    /// <returns>This QueryParameters instance for method chaining</returns>
    public QueryParameters AddInclude(string include)
    {
        var currentIncludes = Include?.ToList() ?? new List<string>();
        if (!currentIncludes.Contains(include))
        {
            currentIncludes.Add(include);
        }
        Include = currentIncludes.ToArray();
        return this;
    }
    
    /// <summary>
    /// Adds a sort directive to the query parameters.
    /// </summary>
    /// <param name="sort">The field to sort by (prefix with '-' for descending)</param>
    /// <returns>This QueryParameters instance for method chaining</returns>
    public QueryParameters AddSort(string sort)
    {
        OrderBy = sort;
        return this;
    }
    
    /// <summary>
    /// Adds a parameter to the query string.
    /// </summary>
    /// <param name="key">The parameter name</param>
    /// <param name="value">The parameter value</param>
    /// <returns>This QueryParameters instance for method chaining</returns>
    public QueryParameters Add(string key, object value)
    {
        Where[key] = value;
        return this;
    }
    
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
            Include = Include?.ToArray() ?? Array.Empty<string>(),
            OrderBy = OrderBy,
            Fields = Fields?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray()),
            CustomParameters = new Dictionary<string, object>(CustomParameters),
            Meta = Meta != null ? new Dictionary<string, object>(Meta) : null,
            Filters = Filters.Select(f => new FilterCondition(f.Field, f.Value, f.Operator)).ToList(),
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
        if (Include != null && Include.Length > 0)
        {
            parameters.Add($"include={string.Join(",", Include)}");
        }
        
        // Add order by
        if (!string.IsNullOrWhiteSpace(OrderBy))
        {
            parameters.Add($"order={Uri.EscapeDataString(OrderBy)}");
        }
        
        // Add fields (sparse fieldsets)
        if (Fields != null)
        {
            foreach (var (resourceType, fieldNames) in Fields)
            {
                parameters.Add($"fields[{Uri.EscapeDataString(resourceType)}]={string.Join(",", fieldNames.Select(Uri.EscapeDataString))}");
            }
        }
        
        // Add custom parameters
        foreach (var (key, value) in CustomParameters)
        {
            parameters.Add($"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value.ToString() ?? "")}");
        }
        
        // Add meta parameters
        if (Meta != null)
        {
            foreach (var (key, value) in Meta)
            {
                parameters.Add($"meta[{Uri.EscapeDataString(key)}]={Uri.EscapeDataString(value.ToString() ?? "")}");
            }
        }
        
        // Add advanced filters
        foreach (var filter in Filters)
        {
            var operatorSuffix = filter.Operator switch
            {
                FilterOperator.NotEqual => "[not]",
                FilterOperator.GreaterThan => "[gt]",
                FilterOperator.GreaterThanOrEqual => "[gte]",
                FilterOperator.LessThan => "[lt]",
                FilterOperator.LessThanOrEqual => "[lte]",
                FilterOperator.Contains => "[contains]",
                FilterOperator.StartsWith => "[starts_with]",
                FilterOperator.EndsWith => "[ends_with]",
                FilterOperator.In => "[in]",
                FilterOperator.NotIn => "[not_in]",
                _ => ""
            };
            parameters.Add($"where[{Uri.EscapeDataString(filter.Field)}{operatorSuffix}]={Uri.EscapeDataString(filter.Value.ToString() ?? "")}");
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
     
     /// <summary>
     /// Adds a filter condition with operator support.
     /// </summary>
     /// <param name="field">The field to filter by</param>
     /// <param name="value">The value to filter for</param>
     /// <param name="operator">The filter operator</param>
     /// <returns>This QueryParameters instance for method chaining</returns>
     public QueryParameters AddFilter(string field, object value, FilterOperator @operator = FilterOperator.Equal)
     {
         Filters.Add(new FilterCondition(field, value, @operator));
         return this;
     }
     
     /// <summary>
     /// Adds multiple filter conditions.
     /// </summary>
     /// <param name="conditions">The filter conditions to add</param>
     /// <returns>This QueryParameters instance for method chaining</returns>
     public QueryParameters AddFilters(params FilterCondition[] conditions)
     {
         Filters.AddRange(conditions);
         return this;
     }
     
     /// <summary>
     /// Adds a field selection for sparse fieldsets.
     /// </summary>
     /// <param name="resourceType">The resource type</param>
     /// <param name="fields">The fields to include</param>
     /// <returns>This QueryParameters instance for method chaining</returns>
     public QueryParameters AddFields(string resourceType, params string[] fields)
     {
         Fields ??= new Dictionary<string, string[]>();
         Fields[resourceType] = fields;
         return this;
     }
     
     /// <summary>
     /// Adds a meta parameter.
     /// </summary>
     /// <param name="key">The meta parameter key</param>
     /// <param name="value">The meta parameter value</param>
     /// <returns>This QueryParameters instance for method chaining</returns>
     public QueryParameters AddMeta(string key, object value)
     {
         Meta ??= new Dictionary<string, object>();
         Meta[key] = value;
         return this;
     }
     
     /// <summary>
     /// Adds a custom parameter.
     /// </summary>
     /// <param name="key">The parameter key</param>
     /// <param name="value">The parameter value</param>
     /// <returns>This QueryParameters instance for method chaining</returns>
     public QueryParameters AddCustomParameter(string key, object value)
     {
         CustomParameters[key] = value;
         return this;
     }
     
     /// <summary>
     /// Sets the sort order.
     /// </summary>
     /// <param name="field">The field to sort by</param>
     /// <param name="descending">Whether to sort in descending order</param>
     /// <returns>This QueryParameters instance for method chaining</returns>
     public QueryParameters SetSort(string field, bool descending = false)
     {
         OrderBy = descending ? $"-{field}" : field;
         return this;
     }
     
     /// <summary>
     /// Sets the relationships to include.
     /// </summary>
     /// <param name="relationships">The relationships to include</param>
     /// <returns>This QueryParameters instance for method chaining</returns>
     public QueryParameters SetInclude(params string[] relationships)
     {
         Include = relationships;
         return this;
     }
     
     /// <summary>
     /// Sets pagination parameters.
     /// </summary>
     /// <param name="page">The page number (1-based)</param>
     /// <param name="pageSize">The number of items per page</param>
     /// <returns>This QueryParameters instance for method chaining</returns>
     public QueryParameters SetPagination(int page, int pageSize)
     {
         PerPage = pageSize;
         Offset = (page - 1) * pageSize;
         return this;
     }
     
     /// <summary>
     /// Sets the maximum number of results to return.
     /// </summary>
     /// <param name="count">The maximum number of results</param>
     /// <returns>This QueryParameters instance for method chaining</returns>
     public QueryParameters SetLimit(int count)
     {
         PerPage = count;
         Offset = null;
         return this;
     }
     
     /// <summary>
     /// Clears all parameters.
     /// </summary>
     /// <returns>This QueryParameters instance for method chaining</returns>
     public QueryParameters Clear()
     {
         Where.Clear();
         Include = null;
         OrderBy = null;
         Fields?.Clear();
         CustomParameters.Clear();
         Meta?.Clear();
         Filters.Clear();
         PerPage = null;
         Offset = null;
         return this;
     }
}

/// <summary>
/// Extension methods for QueryParameters to provide additional functionality.
/// </summary>
public static class QueryParametersExtensions
{
    /// <summary>
    /// Creates a new QueryParameters instance with pagination settings for a specific page.
    /// </summary>
    /// <param name="queryParams">The source QueryParameters instance</param>
    /// <param name="page">The page number (1-based)</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <returns>A new QueryParameters instance with updated pagination</returns>
    /// <exception cref="ArgumentException">Thrown when page or pageSize is less than 1</exception>
    public static QueryParameters WithPage(this QueryParameters queryParams, int page, int pageSize)
    {
        if (page < 1)
            throw new ArgumentException("Page must be greater than 0", nameof(page));
        if (pageSize < 1)
            throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));
            
        var clone = queryParams.Clone();
        clone.PerPage = pageSize;
        clone.Offset = (page - 1) * pageSize;
        return clone;
    }
}