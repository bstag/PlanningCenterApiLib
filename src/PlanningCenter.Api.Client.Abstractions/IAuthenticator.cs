namespace PlanningCenter.Api.Client.Abstractions;

/// <summary>
/// Interface for handling Planning Center API authentication.
/// Supports OAuth 2.0 with automatic token refresh and secure token management.
/// </summary>
public interface IAuthenticator
{
    /// <summary>
    /// Gets a valid access token for API requests.
    /// Automatically refreshes the token if it's expired or about to expire.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A valid access token</returns>
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Manually refreshes the access token using the refresh token.
    /// This is typically called automatically by GetAccessTokenAsync when needed.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task RefreshTokenAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if the current access token is valid and not expired.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>True if the token is valid, false otherwise</returns>
    Task<bool> IsTokenValidAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Event raised when the access token is refreshed.
    /// Useful for logging or updating cached credentials.
    /// </summary>
    event EventHandler<TokenRefreshedEventArgs>? TokenRefreshed;
}

/// <summary>
/// Event arguments for the TokenRefreshed event.
/// </summary>
public class TokenRefreshedEventArgs : EventArgs
{
    /// <summary>
    /// The new access token
    /// </summary>
    public string AccessToken { get; }
    
    /// <summary>
    /// When the token expires
    /// </summary>
    public DateTime ExpiresAt { get; }
    
    /// <summary>
    /// The reason for the token refresh
    /// </summary>
    public string Reason { get; }
    
    public TokenRefreshedEventArgs(string accessToken, DateTime expiresAt, string reason)
    {
        AccessToken = accessToken;
        ExpiresAt = expiresAt;
        Reason = reason;
    }
}