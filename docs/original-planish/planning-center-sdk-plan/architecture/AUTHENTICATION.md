# Authentication Guide

The Planning Center SDK supports multiple authentication methods to suit different application types and use cases.

## Authentication Methods

### 1. Personal Access Token (PAT) - **Recommended for Server Applications**

Personal Access Tokens are the simplest and most secure method for server-side applications, scripts, and background services.

#### Advantages
- ✅ **Simple setup** - No OAuth flow required
- ✅ **No token expiration** - PATs don't expire automatically
- ✅ **Perfect for scripts** - Ideal for automation and background tasks
- ✅ **Secure** - Uses Basic Authentication with app credentials

#### Getting Your PAT
1. Go to [Planning Center API Applications](https://api.planningcenteronline.com/oauth/applications)
2. Create a new application or use an existing one
3. Copy your **Application ID** and **Secret**
4. Format as: `app_id:secret`

#### Usage Examples

**Basic Setup:**
```csharp
// Using the dedicated PAT method
builder.Services.AddPlanningCenterApiClientWithPAT("your-app-id:your-secret");
```

**With Configuration:**
```csharp
// Add PAT authentication
builder.Services.AddPlanningCenterApiClientWithPAT(
    Environment.GetEnvironmentVariable("PLANNING_CENTER_PAT") ?? "your-app-id:your-secret");

// Configure additional options
builder.Services.Configure<PlanningCenterOptions>(options =>
{
    options.EnableDetailedLogging = true;
    options.EnableCaching = true;
    options.DefaultCacheExpiration = TimeSpan.FromMinutes(5);
    options.MaxRetryAttempts = 3;
});
```

**Environment Variable Setup:**
```bash
# Set your PAT as an environment variable
export PLANNING_CENTER_PAT="your-app-id:your-secret"
```

### 2. OAuth 2.0 - **For User-Facing Applications**

OAuth is ideal for applications that need to act on behalf of users or require user consent.

#### Advantages
- ✅ **User consent** - Users explicitly authorize your application
- ✅ **Scoped access** - Can request specific permissions
- ✅ **Secure** - Industry standard OAuth 2.0 flow
- ✅ **Token refresh** - Automatic token renewal

#### Usage Examples

**Basic OAuth Setup:**
```csharp
builder.Services.AddPlanningCenterApiClient(options =>
{
    options.ClientId = "your-oauth-client-id";
    options.ClientSecret = "your-oauth-client-secret";
    options.AccessToken = "user-access-token";  // From OAuth flow
    options.RefreshToken = "user-refresh-token"; // From OAuth flow
});
```

**Simplified OAuth Setup:**
```csharp
builder.Services.AddPlanningCenterApiClient(
    clientId: "your-oauth-client-id",
    clientSecret: "your-oauth-client-secret");
```

### 3. Access Token - **For Simple Token-Based Auth**

If you already have an access token from another source, you can use it directly.

```csharp
builder.Services.AddPlanningCenterApiClientWithToken("your-access-token");
```

## Authentication Priority

When multiple authentication methods are configured, the SDK uses this priority order:

1. **Personal Access Token** (if provided)
2. **OAuth credentials** (if ClientId and ClientSecret are provided)
3. **Access Token** (if provided)

## Configuration Options

All authentication methods support these common configuration options:

```csharp
builder.Services.Configure<PlanningCenterOptions>(options =>
{
    // API Configuration
    options.BaseUrl = "https://api.planningcenteronline.com"; // Default
    options.RequestTimeout = TimeSpan.FromSeconds(30);
    
    // Retry Configuration
    options.MaxRetryAttempts = 3;
    options.RetryBaseDelay = TimeSpan.FromSeconds(1);
    
    // Caching Configuration
    options.EnableCaching = true;
    options.DefaultCacheExpiration = TimeSpan.FromMinutes(5);
    
    // Logging Configuration
    options.EnableDetailedLogging = false; // Be careful in production
    options.UserAgent = "PlanningCenter.Api.Client/1.0";
    
    // Custom Headers
    options.DefaultHeaders.Add("X-Custom-Header", "value");
});
```

## Security Best Practices

### For Personal Access Tokens
- ✅ Store PATs in environment variables, not in code
- ✅ Use different PATs for different environments (dev, staging, prod)
- ✅ Regularly rotate your PATs
- ✅ Limit PAT scope to minimum required permissions
- ❌ Never commit PATs to version control

### For OAuth
- ✅ Store client secrets securely
- ✅ Use HTTPS for all OAuth redirects
- ✅ Implement proper token storage and encryption
- ✅ Handle token refresh gracefully
- ❌ Never expose client secrets in client-side code

## Error Handling

The SDK provides specific exceptions for authentication issues:

```csharp
try
{
    var people = await peopleService.ListAsync();
}
catch (PlanningCenterApiAuthenticationException ex)
{
    // Invalid credentials or expired token
    logger.LogError("Authentication failed: {Message}", ex.Message);
}
catch (PlanningCenterApiAuthorizationException ex)
{
    // Insufficient permissions
    logger.LogError("Authorization failed: {Message}", ex.Message);
}
```

## Testing Authentication

You can verify your authentication setup:

```csharp
// Get the authenticator service
var authenticator = serviceProvider.GetRequiredService<IAuthenticator>();

// Check if token is valid
var isValid = await authenticator.IsTokenValidAsync();
if (!isValid)
{
    await authenticator.RefreshTokenAsync(); // For OAuth only
}

// Get the current access token
var token = await authenticator.GetAccessTokenAsync();
```

## Examples

See the example projects for complete working implementations:

- **PAT Authentication**: `examples/PlanningCenter.Api.Client.PAT.Console/`
- **OAuth Authentication**: `examples/PlanningCenter.Api.Client.Console/`
- **Background Service**: `examples/PlanningCenter.Api.Client.Worker/`

## Choosing the Right Method

| Use Case | Recommended Method | Why |
|----------|-------------------|-----|
| Background services | **PAT** | Simple, no expiration, perfect for automation |
| Server-side APIs | **PAT** | Secure, straightforward, no user interaction needed |
| Scripts & automation | **PAT** | Easiest to set up and maintain |
| User-facing web apps | **OAuth** | User consent, scoped permissions |
| Mobile applications | **OAuth** | Industry standard for mobile auth |
| Desktop applications | **OAuth** | Better user experience with proper consent flow |

## Migration Guide

### From OAuth to PAT
If you're currently using OAuth for a server-side application, migrating to PAT is simple:

```csharp
// Before (OAuth)
builder.Services.AddPlanningCenterApiClient(options =>
{
    options.ClientId = "oauth-client-id";
    options.ClientSecret = "oauth-client-secret";
    options.AccessToken = "user-access-token";
});

// After (PAT)
builder.Services.AddPlanningCenterApiClientWithPAT("app-id:secret");
```

The rest of your code remains unchanged - the same service interfaces work with both authentication methods.