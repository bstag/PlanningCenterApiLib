namespace PlanningCenter.Api.Client;

/// <summary>
/// Configuration options for the Planning Center API client.
/// </summary>
public class PlanningCenterOptions
{
    /// <summary>
    /// The base URL for the Planning Center API.
    /// Defaults to the official Planning Center API endpoint.
    /// </summary>
    public string BaseUrl { get; set; } = "https://api.planningcenteronline.com";
    
    /// <summary>
    /// OAuth client ID for authentication.
    /// </summary>
    public string? ClientId { get; set; }
    
    /// <summary>
    /// OAuth client secret for authentication.
    /// </summary>
    public string? ClientSecret { get; set; }
    
    /// <summary>
    /// Access token for API requests (if using token-based auth instead of OAuth).
    /// </summary>
    public string? AccessToken { get; set; }
    
    /// <summary>
    /// Personal Access Token for API requests.
    /// This should be in the format "app_id:secret" and will be used with Basic Authentication.
    /// If provided, this takes precedence over OAuth credentials.
    /// </summary>
    public string? PersonalAccessToken { get; set; }
    
    /// <summary>
    /// Refresh token for automatic token renewal.
    /// </summary>
    public string? RefreshToken { get; set; }
    
    /// <summary>
    /// Default timeout for HTTP requests.
    /// </summary>
    public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(30);
    
    /// <summary>
    /// Maximum number of retry attempts for failed requests.
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;
    
    /// <summary>
    /// Base delay for exponential backoff retry strategy.
    /// </summary>
    public TimeSpan RetryBaseDelay { get; set; } = TimeSpan.FromSeconds(1);
    
    /// <summary>
    /// Whether to enable response caching.
    /// </summary>
    public bool EnableCaching { get; set; } = true;
    
    /// <summary>
    /// Default cache expiration time.
    /// </summary>
    public TimeSpan DefaultCacheExpiration { get; set; } = TimeSpan.FromMinutes(5);
    
    /// <summary>
    /// Maximum number of items to keep in the cache.
    /// </summary>
    public int MaxCacheSize { get; set; } = 1000;
    
    /// <summary>
    /// User agent string to send with requests.
    /// </summary>
    public string UserAgent { get; set; } = "PlanningCenter.Api.Client/1.0";
    
    /// <summary>
    /// OAuth token endpoint URL.
    /// </summary>
    public string TokenEndpoint { get; set; } = "https://api.planningcenteronline.com/oauth/token";
    
    /// <summary>
    /// Whether to log request and response details (for debugging).
    /// Be careful with this in production as it may log sensitive data.
    /// </summary>
    public bool EnableDetailedLogging { get; set; } = false;
    
    /// <summary>
    /// Custom headers to include with all requests.
    /// </summary>
    public Dictionary<string, string> DefaultHeaders { get; set; } = new();
    
    /// <summary>
    /// Checks if the configuration is valid.
    /// </summary>
    /// <returns>True if the configuration is valid, otherwise false.</returns>
    public bool IsValid()
    {
        try
        {
            Validate();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// Checks if any authentication method is configured.
    /// </summary>
    /// <returns>True if authentication is configured, otherwise false.</returns>
    public bool HasAuthentication()
    {
        var hasOAuthCredentials = !string.IsNullOrWhiteSpace(ClientId) && !string.IsNullOrWhiteSpace(ClientSecret);
        var hasAccessToken = !string.IsNullOrWhiteSpace(AccessToken);
        var hasPersonalAccessToken = !string.IsNullOrWhiteSpace(PersonalAccessToken);
        
        return hasOAuthCredentials || hasAccessToken || hasPersonalAccessToken;
    }
    
    /// <summary>
    /// Validates the configuration options.
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(BaseUrl))
            throw new ArgumentException("BaseUrl is required", nameof(BaseUrl));
        
        if (!Uri.TryCreate(BaseUrl, UriKind.Absolute, out _))
            throw new ArgumentException("BaseUrl must be a valid absolute URI", nameof(BaseUrl));
        
        // Must have either OAuth credentials, an access token, or a personal access token
        var hasOAuthCredentials = !string.IsNullOrWhiteSpace(ClientId) && !string.IsNullOrWhiteSpace(ClientSecret);
        var hasAccessToken = !string.IsNullOrWhiteSpace(AccessToken);
        var hasPersonalAccessToken = !string.IsNullOrWhiteSpace(PersonalAccessToken);
        
        if (!hasOAuthCredentials && !hasAccessToken && !hasPersonalAccessToken)
            throw new ArgumentException("Either OAuth credentials (ClientId and ClientSecret), AccessToken, or PersonalAccessToken must be provided");
        
        if (RequestTimeout <= TimeSpan.Zero)
            throw new ArgumentException("RequestTimeout must be greater than zero", nameof(RequestTimeout));
        
        if (MaxRetryAttempts < 0)
            throw new ArgumentException("MaxRetryAttempts cannot be negative", nameof(MaxRetryAttempts));
        
        if (RetryBaseDelay <= TimeSpan.Zero)
            throw new ArgumentException("RetryBaseDelay must be greater than zero", nameof(RetryBaseDelay));
    }
}