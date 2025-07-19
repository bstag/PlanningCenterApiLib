namespace PlanningCenter.Api.Client;

/// <summary>
/// Configuration options for the Planning Center API client.
/// </summary>
public class PlanningCenterOptions
{
    private string _baseUrl = "https://api.planningcenteronline.com";
    
    /// <summary>
    /// The base URL for the Planning Center API.
    /// Defaults to the official Planning Center API endpoint.
    /// </summary>
    public string BaseUrl 
    { 
        get => _baseUrl;
        set
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value), "BaseUrl cannot be null.");
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("BaseUrl cannot be empty or whitespace.", nameof(value));
            
            // Validate URL format
            if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) || 
                (uri.Scheme != "http" && uri.Scheme != "https"))
                throw new ArgumentException("BaseUrl must be a valid HTTP or HTTPS URL.", nameof(value));
            
            _baseUrl = value.TrimEnd('/');
        }
    }

    public PlanningCenterOptions()
    {
        // Allow default construction for test scenarios; do not validate authentication here.
    }
    
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
    
    private TimeSpan _requestTimeout = TimeSpan.FromSeconds(30);
    
    /// <summary>
    /// Default timeout for HTTP requests.
    /// </summary>
    public TimeSpan RequestTimeout 
    { 
        get => _requestTimeout;
        set
        {
            if (value <= TimeSpan.Zero)
                throw new ArgumentException("RequestTimeout must be greater than zero.", nameof(value));
            _requestTimeout = value;
        }
    }
    
    private int _maxRetryAttempts = 3;
    
    /// <summary>
    /// Maximum number of retry attempts for failed requests.
    /// </summary>
    public int MaxRetryAttempts 
    { 
        get => _maxRetryAttempts;
        set
        {
            if (value < 0)
                throw new ArgumentException("MaxRetryAttempts cannot be negative.", nameof(value));
            _maxRetryAttempts = value;
        }
    }
    
    private TimeSpan _retryBaseDelay = TimeSpan.FromMilliseconds(500);
    
    /// <summary>
    /// Base delay for exponential backoff retry strategy.
    /// </summary>
    public TimeSpan RetryBaseDelay 
    { 
        get => _retryBaseDelay;
        set
        {
            if (value < TimeSpan.Zero)
                throw new ArgumentException("RetryBaseDelay cannot be negative.", nameof(value));
            _retryBaseDelay = value;
        }
    }
    
    /// <summary>
    /// Whether to enable response caching.
    /// </summary>
    public bool EnableCaching { get; set; } = true;
    
    private TimeSpan _defaultCacheExpiration = TimeSpan.FromMinutes(5);
    
    /// <summary>
    /// Default cache expiration time.
    /// </summary>
    public TimeSpan DefaultCacheExpiration 
    { 
        get => _defaultCacheExpiration;
        set
        {
            if (value < TimeSpan.Zero)
                throw new ArgumentException("DefaultCacheExpiration cannot be negative.", nameof(value));
            _defaultCacheExpiration = value;
        }
    }
    
    private int _maxCacheSize = 1000;
    
    /// <summary>
    /// Maximum number of items to keep in the cache.
    /// </summary>
    public int MaxCacheSize 
    { 
        get => _maxCacheSize;
        set
        {
            if (value < 0)
                throw new ArgumentException("MaxCacheSize cannot be negative.", nameof(value));
            _maxCacheSize = value;
        }
    }
    
    private string _userAgent = "PlanningCenter.Api.Client/1.0";
    
    /// <summary>
    /// User agent string to send with requests.
    /// </summary>
    public string UserAgent 
    { 
        get => _userAgent;
        set
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value), "UserAgent cannot be null.");
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("UserAgent cannot be empty or whitespace.", nameof(value));
            _userAgent = value;
        }
    }
    
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
        // Normalize BaseUrl by removing trailing slashes (except for scheme)
        if (string.IsNullOrWhiteSpace(BaseUrl))
            throw new ArgumentException("BaseUrl cannot be empty.", nameof(BaseUrl));
        if (!Uri.TryCreate(BaseUrl, UriKind.Absolute, out var baseUrlUri))
            throw new ArgumentException("BaseUrl must be a valid absolute URI.", nameof(BaseUrl));
        // Normalize BaseUrl by removing all trailing slashes
        // Remove all trailing slashes from BaseUrl
        BaseUrl = BaseUrl.TrimEnd('/');
        
        // Must have either OAuth credentials, an access token, or a personal access token
        var hasOAuthCredentials = !string.IsNullOrWhiteSpace(ClientId) && !string.IsNullOrWhiteSpace(ClientSecret);
        var hasAccessToken = !string.IsNullOrWhiteSpace(AccessToken);
        var hasPersonalAccessToken = !string.IsNullOrWhiteSpace(PersonalAccessToken);

        if (!hasOAuthCredentials && !hasAccessToken && !hasPersonalAccessToken)
            throw new InvalidOperationException("No authentication configured: Either OAuth credentials (ClientId and ClientSecret), AccessToken, or PersonalAccessToken must be provided. Please provide valid authentication.");

        if (RequestTimeout <= TimeSpan.Zero)
            throw new ArgumentException("RequestTimeout must be positive.", nameof(RequestTimeout));
        if (DefaultCacheExpiration < TimeSpan.Zero)
            throw new ArgumentException("DefaultCacheExpiration cannot be negative.", nameof(DefaultCacheExpiration));
        if (string.IsNullOrWhiteSpace(UserAgent))
            throw new ArgumentException("UserAgent cannot be null or empty.", nameof(UserAgent));
        if (MaxRetryAttempts < 0)
            throw new ArgumentException("MaxRetryAttempts cannot be negative.", nameof(MaxRetryAttempts));
        if (RetryBaseDelay < TimeSpan.Zero)
            throw new ArgumentException("RetryBaseDelay cannot be negative.", nameof(RetryBaseDelay));
        if (MaxCacheSize <= 0)
            throw new ArgumentException("MaxCacheSize must be positive.", nameof(MaxCacheSize));
    }

    public override string ToString()
    {
        return $"PlanningCenterOptions {{ BaseUrl = '{BaseUrl}', ClientId = '{ClientId}', ClientSecret = '***', AccessToken = '{(string.IsNullOrEmpty(AccessToken) ? "<none>" : "***")}', PersonalAccessToken = '{(string.IsNullOrEmpty(PersonalAccessToken) ? "<none>" : "***")}', RequestTimeout = {RequestTimeout.TotalMilliseconds}ms, DefaultCacheExpiration = {DefaultCacheExpiration.TotalSeconds}s, MaxCacheSize = {MaxCacheSize}, MaxRetryAttempts = {MaxRetryAttempts}, UserAgent = '{UserAgent}', EnableCaching = {EnableCaching}, EnableDetailedLogging = {EnableDetailedLogging} }}";
    }
}