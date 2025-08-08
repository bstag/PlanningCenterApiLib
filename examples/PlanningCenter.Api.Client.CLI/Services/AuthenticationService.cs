using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using PlanningCenter.Api.Client.CLI.Configuration;
using PlanningCenter.Api.Client;
using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Services;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace PlanningCenter.Api.Client.CLI.Services;

/// <summary>
/// Service for handling authentication with Planning Center API
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly CliConfiguration _configuration;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public AuthenticationService(CliConfiguration configuration, ILogger<AuthenticationService> logger, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Gets the Personal Access Token from parameter or configuration
    /// </summary>
    /// <param name="tokenParameter">Token provided via parameter</param>
    /// <returns>The Personal Access Token or null if not found</returns>
    public string? GetToken(string? tokenParameter = null)
    {
        // Priority order:
        // 1. Parameter
        // 2. Configuration file

        if (!string.IsNullOrEmpty(tokenParameter))
        {
            _logger.LogDebug("Using Personal Access Token from parameter");
            return tokenParameter;
        }

        var token = _configuration.GetPersonalAccessToken();
        if (!string.IsNullOrEmpty(token))
        {
            _logger.LogDebug("Using Personal Access Token from configuration");
            return token;
        }

        return null;
    }

    /// <summary>
    /// Gets the Personal Access Token from various sources
    /// </summary>
    /// <param name="tokenFromCommandLine">Token provided via command line argument</param>
    /// <returns>The Personal Access Token or null if not found</returns>
    public string? GetPersonalAccessToken(string? tokenFromCommandLine = null)
    {
        // Priority order:
        // 1. Command line argument
        // 2. Environment variable
        // 3. Configuration file

        if (!string.IsNullOrEmpty(tokenFromCommandLine))
        {
            _logger.LogDebug("Using Personal Access Token from command line argument");
            return tokenFromCommandLine;
        }

        var token = _configuration.GetPersonalAccessToken();
        if (!string.IsNullOrEmpty(token))
        {
            _logger.LogDebug("Using Personal Access Token from configuration");
            return token;
        }

        return null;
    }

    /// <summary>
    /// Validates that a Personal Access Token is in the correct format
    /// </summary>
    /// <param name="token">The token to validate</param>
    /// <returns>True if the token format is valid</returns>
    public bool ValidateTokenFormat(string token)
    {
        if (string.IsNullOrEmpty(token))
            return false;

        // Personal Access Token format: app-id:secret
        var parts = token.Split(':');
        if (parts.Length != 2)
        {
            _logger.LogError("Invalid Personal Access Token format. Expected format: app-id:secret");
            return false;
        }

        if (string.IsNullOrEmpty(parts[0]) || string.IsNullOrEmpty(parts[1]))
        {
            _logger.LogError("Invalid Personal Access Token format. Both app-id and secret must be provided");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Stores a Personal Access Token in the user configuration
    /// </summary>
    /// <param name="token">The token to store</param>
    public async Task StoreTokenAsync(string token)
    {
        if (!ValidateTokenFormat(token))
        {
            throw new ArgumentException("Invalid Personal Access Token format. Expected format: app-id:secret", nameof(token));
        }

        try
        {
            await _configuration.SetPersonalAccessTokenAsync(token);
            _logger.LogInformation("Personal Access Token stored successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to store Personal Access Token");
            throw;
        }
    }

    /// <summary>
    /// Tests authentication with the Planning Center API
    /// </summary>
    /// <param name="token">The token to test</param>
    /// <returns>True if authentication is successful</returns>
    public async Task<bool> TestAuthenticationAsync(string token)
    {
        if (!ValidateTokenFormat(token))
        {
            return false;
        }

        try
        {
            // Create a direct HTTP client to test authentication
            using var httpClient = new HttpClient();
            
            // Set up Basic Authentication header
            var authBytes = Encoding.UTF8.GetBytes(token);
            var authHeader = Convert.ToBase64String(authBytes);
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader);
            
            // Test with a simple API call to get current user
            var response = await httpClient.GetAsync("https://api.planningcenteronline.com/people/v2/me");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                
                // Try to parse the response to get user info
                try
                {
                    using var jsonDoc = JsonDocument.Parse(content);
                    if (jsonDoc.RootElement.TryGetProperty("data", out var dataElement) &&
                        dataElement.TryGetProperty("attributes", out var attributesElement))
                    {
                        var name = attributesElement.TryGetProperty("name", out var nameElement) ? nameElement.GetString() : "Unknown";
                        var id = dataElement.TryGetProperty("id", out var idElement) ? idElement.GetString() : "Unknown";
                        
                        _logger.LogInformation("Authentication successful. Current user: {Name} (ID: {Id})", name, id);
                        return true;
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogDebug(ex, "Failed to parse user info from response, but authentication was successful");
                    return true; // Authentication worked even if we couldn't parse the response
                }
                
                _logger.LogInformation("Authentication successful");
                return true;
            }
            else
            {
                _logger.LogError("Authentication failed with status code: {StatusCode}", response.StatusCode);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Authentication test failed");
            return false;
        }
    }
}