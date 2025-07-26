# Authentication Guide

The Planning Center API SDK for .NET supports multiple authentication methods to accommodate different application types and security requirements. This guide covers all authentication methods with detailed examples and best practices.

## Table of Contents

- [Authentication Methods Overview](#authentication-methods-overview)
- [Personal Access Token (PAT)](#personal-access-token-pat)
- [OAuth 2.0](#oauth-20)
- [Access Token](#access-token)
- [Configuration Options](#configuration-options)
- [Security Best Practices](#security-best-practices)
- [Troubleshooting](#troubleshooting)

## Authentication Methods Overview

| Method | Use Case | Security | Complexity |
|--------|----------|----------|-----------|
| **Personal Access Token (PAT)** | Server-side applications, background services | High | Low |
| **OAuth 2.0** | User-facing applications, web apps | Highest | Medium |
| **Access Token** | Simple integrations, testing | Medium | Lowest |

## Personal Access Token (PAT)

**Recommended for server-side applications and background services.**

Personal Access Tokens provide secure, long-lived authentication for applications that don't require user interaction.

### Getting a PAT

1. Log in to your Planning Center account
2. Go to **Settings** → **Developer** → **Personal Access Tokens**
3. Click **New Token**
4. Enter a description and select the required permissions
5. Copy the generated token (format: `app_id:secret`)

### Configuration

#### Method 1: Dependency Injection (Recommended)

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlanningCenter.Api.Client;

var builder = Host.CreateApplicationBuilder(args);

// Add Planning Center API client with PAT
builder.Services.AddPlanningCenterApiClientWithPAT("your-app-id:your-secret");

var host = builder.Build();

// Use services
var peopleService = host.Services.GetRequiredService<IPeopleService>();
var people = await peopleService.ListAsync();
```

#### Method 2: Configuration-based

```csharp
// appsettings.json
{
  "PlanningCenter": {
    "PersonalAccessToken": "your-app-id:your-secret"
  }
}

// Program.cs
builder.Services.AddPlanningCenterApiClient(options =>
{
    options.PersonalAccessToken = builder.Configuration["PlanningCenter:PersonalAccessToken"];
});
```

#### Method 3: Environment Variables

```csharp
// Set environment variable
// PLANNING_CENTER_PAT=your-app-id:your-secret

builder.Services.AddPlanningCenterApiClient(options =>
{
    options.PersonalAccessToken = Environment.GetEnvironmentVariable("PLANNING_CENTER_PAT");
});
```

### Example Usage

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlanningCenter.Api.Client;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddPlanningCenterApiClientWithPAT("your-app-id:your-secret");

var host = builder.Build();

// All services are available
var peopleService = host.Services.GetRequiredService<IPeopleService>();
var givingService = host.Services.GetRequiredService<IGivingService>();
var calendarService = host.Services.GetRequiredService<ICalendarService>();

// Perform operations
var people = await peopleService.ListAsync();
var donations = await givingService.ListDonationsAsync();
var events = await calendarService.ListEventsAsync();

Console.WriteLine($"Found {people.Data.Count} people");
Console.WriteLine($"Found {donations.Data.Count} donations");
Console.WriteLine($"Found {events.Data.Count} events");
```

## OAuth 2.0

**Recommended for user-facing applications that require user consent.**

OAuth 2.0 provides secure, user-authorized access to Planning Center data with fine-grained permissions.

### OAuth 2.0 Flow

1. **Register your application** in Planning Center Developer settings
2. **Redirect user** to Planning Center authorization URL
3. **Receive authorization code** from callback
4. **Exchange code** for access and refresh tokens
5. **Use access token** for API requests
6. **Refresh token** when access token expires

### Configuration

```csharp
builder.Services.AddPlanningCenterApiClient(options =>
{
    options.ClientId = "your-oauth-client-id";
    options.ClientSecret = "your-oauth-client-secret";
    options.AccessToken = "user-access-token";
    options.RefreshToken = "user-refresh-token";
    options.RedirectUri = "https://your-app.com/callback";
});
```

### OAuth 2.0 Implementation Example

```csharp
public class OAuthService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    
    public OAuthService(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
    }
    
    public string GetAuthorizationUrl(string state = null)
    {
        var clientId = _configuration["PlanningCenter:ClientId"];
        var redirectUri = _configuration["PlanningCenter:RedirectUri"];
        var scope = "people"; // Specify required scopes
        
        var url = "https://api.planningcenteronline.com/oauth/authorize" +
                 $"?client_id={clientId}" +
                 $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                 $"&response_type=code" +
                 $"&scope={scope}";
                 
        if (!string.IsNullOrEmpty(state))
        {
            url += $"&state={Uri.EscapeDataString(state)}";
        }
        
        return url;
    }
    
    public async Task<TokenResponse> ExchangeCodeForTokenAsync(string code)
    {
        var clientId = _configuration["PlanningCenter:ClientId"];
        var clientSecret = _configuration["PlanningCenter:ClientSecret"];
        var redirectUri = _configuration["PlanningCenter:RedirectUri"];
        
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.planningcenteronline.com/oauth/token")
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "authorization_code",
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret,
                ["code"] = code,
                ["redirect_uri"] = redirectUri
            })
        };
        
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TokenResponse>(json);
    }
    
    public async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
    {
        var clientId = _configuration["PlanningCenter:ClientId"];
        var clientSecret = _configuration["PlanningCenter:ClientSecret"];
        
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.planningcenteronline.com/oauth/token")
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret,
                ["refresh_token"] = refreshToken
            })
        };
        
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TokenResponse>(json);
    }
}

public class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
    
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }
    
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }
    
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
    
    [JsonPropertyName("scope")]
    public string Scope { get; set; }
}
```

### ASP.NET Core Integration

```csharp
// Controllers/AuthController.cs
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly OAuthService _oauthService;
    
    public AuthController(OAuthService oauthService)
    {
        _oauthService = oauthService;
    }
    
    [HttpGet("login")]
    public IActionResult Login()
    {
        var state = Guid.NewGuid().ToString();
        HttpContext.Session.SetString("oauth_state", state);
        
        var authUrl = _oauthService.GetAuthorizationUrl(state);
        return Redirect(authUrl);
    }
    
    [HttpGet("callback")]
    public async Task<IActionResult> Callback(string code, string state)
    {
        var sessionState = HttpContext.Session.GetString("oauth_state");
        if (state != sessionState)
        {
            return BadRequest("Invalid state parameter");
        }
        
        try
        {
            var tokens = await _oauthService.ExchangeCodeForTokenAsync(code);
            
            // Store tokens securely (database, secure session, etc.)
            HttpContext.Session.SetString("access_token", tokens.AccessToken);
            HttpContext.Session.SetString("refresh_token", tokens.RefreshToken);
            
            return Redirect("/dashboard");
        }
        catch (Exception ex)
        {
            return BadRequest($"Token exchange failed: {ex.Message}");
        }
    }
}
```

## Access Token

**For simple integrations and testing scenarios.**

Direct access token authentication is the simplest method but requires manual token management.

### Configuration

```csharp
builder.Services.AddPlanningCenterApiClient(options =>
{
    options.AccessToken = "your-access-token";
});
```

### Example Usage

```csharp
using PlanningCenter.Api.Client;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddPlanningCenterApiClient(options =>
{
    options.AccessToken = "your-access-token";
});

var host = builder.Build();
var peopleService = host.Services.GetRequiredService<IPeopleService>();

try
{
    var people = await peopleService.ListAsync();
    Console.WriteLine($"Found {people.Data.Count} people");
}
catch (UnauthorizedAccessException)
{
    Console.WriteLine("Access token is invalid or expired");
}
```

## Configuration Options

### Complete Configuration Example

```csharp
builder.Services.Configure<PlanningCenterOptions>(options =>
{
    // Authentication
    options.PersonalAccessToken = "app-id:secret";
    options.ClientId = "oauth-client-id";
    options.ClientSecret = "oauth-client-secret";
    options.AccessToken = "access-token";
    options.RefreshToken = "refresh-token";
    
    // API Configuration
    options.BaseUrl = "https://api.planningcenteronline.com";
    options.RequestTimeout = TimeSpan.FromSeconds(30);
    options.UserAgent = "MyApp/1.0";
    
    // Retry Configuration
    options.MaxRetryAttempts = 3;
    options.RetryBaseDelay = TimeSpan.FromSeconds(1);
    options.RetryMaxDelay = TimeSpan.FromSeconds(30);
    
    // Caching
    options.EnableCaching = true;
    options.DefaultCacheExpiration = TimeSpan.FromMinutes(5);
    
    // Logging
    options.EnableDetailedLogging = false; // Be careful in production
    options.LogRequestHeaders = false;
    options.LogResponseHeaders = false;
});
```

### Environment-Specific Configuration

```csharp
// appsettings.Development.json
{
  "PlanningCenter": {
    "PersonalAccessToken": "dev-app-id:dev-secret",
    "EnableDetailedLogging": true,
    "RequestTimeout": "00:01:00"
  }
}

// appsettings.Production.json
{
  "PlanningCenter": {
    "PersonalAccessToken": "prod-app-id:prod-secret",
    "EnableDetailedLogging": false,
    "RequestTimeout": "00:00:30",
    "EnableCaching": true,
    "DefaultCacheExpiration": "00:05:00"
  }
}
```

## Security Best Practices

### 1. Token Storage

```csharp
// ❌ DON'T: Store tokens in plain text
var token = "app-id:secret";

// ✅ DO: Use secure configuration
var token = builder.Configuration["PlanningCenter:PersonalAccessToken"];

// ✅ DO: Use environment variables
var token = Environment.GetEnvironmentVariable("PLANNING_CENTER_PAT");

// ✅ DO: Use Azure Key Vault or similar
var token = await keyVaultClient.GetSecretAsync("planning-center-pat");
```

### 2. Token Rotation

```csharp
public class TokenRotationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<TokenRotationService> _logger;
    
    public async Task RotateTokenIfNeededAsync()
    {
        var tokenCreatedAt = _configuration.GetValue<DateTime>("PlanningCenter:TokenCreatedAt");
        var rotationInterval = TimeSpan.FromDays(90); // Rotate every 90 days
        
        if (DateTime.UtcNow - tokenCreatedAt > rotationInterval)
        {
            _logger.LogWarning("Planning Center token is due for rotation");
            // Implement token rotation logic
        }
    }
}
```

### 3. Scope Limitation

```csharp
// ✅ DO: Request only necessary scopes
var authUrl = GetAuthorizationUrl("people:read giving:read");

// ❌ DON'T: Request all scopes unnecessarily
var authUrl = GetAuthorizationUrl("people giving calendar services groups");
```

### 4. Secure Headers

```csharp
builder.Services.Configure<PlanningCenterOptions>(options =>
{
    // ❌ DON'T: Log sensitive headers in production
    options.LogRequestHeaders = false;
    options.LogResponseHeaders = false;
    
    // ✅ DO: Use secure user agent
    options.UserAgent = "MySecureApp/1.0 (+https://myapp.com/contact)";
});
```

## Troubleshooting

### Common Issues

#### 1. Invalid Token Format

```
Error: "Invalid authentication credentials"
```

**Solution**: Ensure PAT format is `app-id:secret`

```csharp
// ❌ Wrong format
var token = "app-id";
var token = "secret";

// ✅ Correct format
var token = "app-id:secret";
```

#### 2. Expired Access Token

```
Error: "Token has expired"
```

**Solution**: Implement token refresh logic

```csharp
public async Task<T> ExecuteWithTokenRefreshAsync<T>(Func<Task<T>> operation)
{
    try
    {
        return await operation();
    }
    catch (UnauthorizedAccessException)
    {
        await RefreshTokenAsync();
        return await operation();
    }
}
```

#### 3. Insufficient Permissions

```
Error: "Insufficient scope"
```

**Solution**: Check and update OAuth scopes

```csharp
// Check required scopes for your operations
var requiredScopes = new[] { "people:read", "people:write", "giving:read" };
var authUrl = GetAuthorizationUrl(string.Join(" ", requiredScopes));
```

#### 4. Rate Limiting

```
Error: "Rate limit exceeded"
```

**Solution**: Implement exponential backoff (built into SDK)

```csharp
builder.Services.Configure<PlanningCenterOptions>(options =>
{
    options.MaxRetryAttempts = 5;
    options.RetryBaseDelay = TimeSpan.FromSeconds(2);
    options.RetryMaxDelay = TimeSpan.FromMinutes(1);
});
```

### Debug Logging

```csharp
// Enable detailed logging for troubleshooting
builder.Services.Configure<PlanningCenterOptions>(options =>
{
    options.EnableDetailedLogging = true;
});

// Configure logging
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
```

### Testing Authentication

```csharp
public async Task TestAuthenticationAsync()
{
    try
    {
        var peopleService = host.Services.GetRequiredService<IPeopleService>();
        var me = await peopleService.GetMeAsync();
        
        Console.WriteLine($"Authentication successful! Logged in as: {me.FullName}");
        Console.WriteLine($"Organization: {me.OrganizationName}");
    }
    catch (UnauthorizedAccessException ex)
    {
        Console.WriteLine($"Authentication failed: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Unexpected error: {ex.Message}");
    }
}
```

## Next Steps

- [Getting Started Guide](GETTING_STARTED.md) - Complete setup walkthrough
- [Best Practices](BEST_PRACTICES.md) - SDK usage patterns and recommendations
- [Troubleshooting](TROUBLESHOOTING.md) - Common issues and solutions
- [CLI Tool Guide](../examples/PlanningCenter.Api.Client.CLI/README.md) - Command-line interface

---

**Need Help?**

If you encounter issues not covered in this guide:

1. Check the [Troubleshooting Guide](TROUBLESHOOTING.md)
2. Review the [Planning Center API Documentation](https://developer.planning.center/docs/)
3. Search existing [GitHub Issues](https://github.com/your-repo/issues)
4. Create a new issue with detailed information about your problem