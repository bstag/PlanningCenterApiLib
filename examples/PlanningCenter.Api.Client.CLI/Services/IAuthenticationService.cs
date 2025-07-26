namespace PlanningCenter.Api.Client.CLI.Services;

/// <summary>
/// Interface for authentication service operations
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Gets the Personal Access Token from parameter or configuration
    /// </summary>
    string? GetToken(string? tokenParameter = null);

    /// <summary>
    /// Stores the Personal Access Token securely
    /// </summary>
    Task StoreTokenAsync(string token);

    /// <summary>
    /// Validates the format of a Personal Access Token
    /// </summary>
    bool ValidateTokenFormat(string token);

    /// <summary>
    /// Tests authentication with the provided token
    /// </summary>
    Task<bool> TestAuthenticationAsync(string token);
}