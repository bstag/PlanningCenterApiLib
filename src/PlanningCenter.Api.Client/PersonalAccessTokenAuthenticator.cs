using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlanningCenter.Api.Client.Models;
using System.Text;

namespace PlanningCenter.Api.Client;

/// <summary>
/// Personal Access Token authenticator for the Planning Center API.
/// Uses Basic Authentication with the PAT in the format "app_id:secret".
/// </summary>
public class PersonalAccessTokenAuthenticator : IAuthenticator
{
    private readonly PlanningCenterOptions _options;
    private readonly ILogger<PersonalAccessTokenAuthenticator> _logger;
    private readonly string _basicAuthHeader;

    #pragma warning disable CS0067 // Event is never used - PATs don't need token refresh
    public event EventHandler<TokenRefreshedEventArgs>? TokenRefreshed;
    #pragma warning restore CS0067

    public PersonalAccessTokenAuthenticator(
        IOptions<PlanningCenterOptions> options,
        ILogger<PersonalAccessTokenAuthenticator> logger)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        if (string.IsNullOrWhiteSpace(_options.PersonalAccessToken))
        {
            throw new ArgumentException("PersonalAccessToken is required for PAT authentication", nameof(options));
        }

        // Create the Basic Auth header value
        var encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(_options.PersonalAccessToken));
        _basicAuthHeader = $"Basic {encodedCredentials}";

        _logger.LogDebug("Personal Access Token authenticator initialized");
    }

    /// <summary>
    /// Gets the Basic Authentication header value for the Personal Access Token.
    /// PATs don't expire, so this always returns the same value.
    /// </summary>
    public Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Returning Personal Access Token for authentication");
        return Task.FromResult(_basicAuthHeader);
    }

    /// <summary>
    /// Personal Access Tokens don't need to be refreshed.
    /// This method is a no-op for PAT authentication.
    /// </summary>
    public Task RefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Refresh token called for PAT (no-op)");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Personal Access Tokens are always valid until manually revoked.
    /// </summary>
    public Task<bool> IsTokenValidAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }
    
    /// <summary>
    /// Gets the authorization header value for API requests.
    /// For Personal Access Tokens, this is the Basic Authentication header.
    /// </summary>
    /// <returns>The authorization header value.</returns>
    public Task<string> GetAuthorizationHeaderAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_basicAuthHeader);
    }
}