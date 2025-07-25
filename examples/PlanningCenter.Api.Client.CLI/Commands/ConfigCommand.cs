using Microsoft.Extensions.Logging;
using PlanningCenter.Api.Client.CLI.Configuration;
using PlanningCenter.Api.Client.CLI.Formatters;
using PlanningCenter.Api.Client.CLI.Services;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;

namespace PlanningCenter.Api.Client.CLI.Commands;

/// <summary>
/// Command for managing CLI configuration
/// </summary>
public class ConfigCommand : BaseCommand
{
    private readonly IPlanningCenterClientFactory _clientFactory;
    
    public ConfigCommand(
        ILogger<ConfigCommand> logger,
        IAuthenticationService authenticationService,
        IPlanningCenterClientFactory clientFactory,
        IServiceProvider serviceProvider,
        IEnumerable<IOutputFormatter> formatters,
        CliConfiguration configuration)
        : base("config", "Manage CLI configuration settings", logger, authenticationService, serviceProvider, formatters, configuration)
    {
        _clientFactory = clientFactory;
        AddSetTokenCommand();
        AddGetTokenCommand();
        AddTestTokenCommand();
        AddShowConfigCommand();
    }

    private void AddSetTokenCommand()
    {
        var setTokenCommand = new Command("set-token", "Set the Personal Access Token")
        {
            new Argument<string>("token", "The Personal Access Token to store")
        };

        setTokenCommand.SetHandler(async (string token) =>
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    Console.WriteLine("Error: Token cannot be empty.");
                    return;
                }

                // Validate token format
                if (!AuthenticationService.ValidateTokenFormat(token))
                {
                    Console.WriteLine("Error: Invalid token format. Personal Access Token should be in the format 'app_id:secret'.");
                    return;
                }

                // Store the token
                await AuthenticationService.StoreTokenAsync(token);
                Console.WriteLine("Personal Access Token has been stored successfully.");

                // Test the token
                Console.WriteLine("Testing token...");
                var isValid = await AuthenticationService.TestAuthenticationAsync(token);
                
                if (isValid)
                {
                    Console.WriteLine("✓ Token is valid and authentication successful.");
                }
                else
                {
                    Console.WriteLine("⚠ Token was stored but authentication test failed. Please verify your token.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "setting token");
            }
        }, setTokenCommand.Arguments.OfType<Argument<string>>().First());

        AddCommand(setTokenCommand);
    }

    private void AddGetTokenCommand()
    {
        var getTokenCommand = new Command("get-token", "Display the stored Personal Access Token (masked for security)");

        getTokenCommand.SetHandler(async () =>
        {
            try
            {
                var token = await AuthenticationService.GetTokenAsync();
                
                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("No Personal Access Token is currently stored.");
                    Console.WriteLine("Use 'config set-token <token>' to set one.");
                }
                else
                {
                    // Mask the token for security (show only first 8 and last 4 characters)
                    var maskedToken = MaskToken(token);
                    Console.WriteLine($"Stored token: {maskedToken}");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "retrieving token");
            }
        });

        AddCommand(getTokenCommand);
    }

    private void AddTestTokenCommand()
    {
        var testTokenCommand = new Command("test-token", "Test the stored Personal Access Token")
        {
            new Option<string?>("--token", "Test a specific token instead of the stored one")
        };

        testTokenCommand.SetHandler(async (string? token) =>
        {
            try
            {
                var testToken = token ?? await AuthenticationService.GetTokenAsync();
                
                if (string.IsNullOrEmpty(testToken))
                {
                    Console.WriteLine("No token provided and no stored token found.");
                    Console.WriteLine("Use --token parameter or set a token with 'config set-token <token>'.");
                    return;
                }

                Console.WriteLine("Testing authentication...");
                var isValid = await AuthenticationService.TestAuthenticationAsync(testToken);
                
                if (isValid)
                {
                    Console.WriteLine("✓ Authentication successful! Token is valid.");
                    
                    // Try to get some basic info to show the token works
                    try
                    {
                        var client = await _clientFactory.CreateClientAsync(testToken);
                        var userInfo = await client.GetCurrentUserAsync();
                         
                         if (userInfo != null)
                         {
                             Console.WriteLine($"✓ Connected as: {userInfo.Name}");
                             Console.WriteLine($"✓ Email: {userInfo.Email}");
                             Console.WriteLine($"✓ User ID: {userInfo.Id}");
                         }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogDebug(ex, "Failed to get user info during token test");
                        Console.WriteLine("✓ Token is valid but couldn't retrieve user details.");
                    }
                }
                else
                {
                    Console.WriteLine("✗ Authentication failed. Please check your token.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "testing token");
            }
        }, testTokenCommand.Options.OfType<Option<string?>>().First());

        AddCommand(testTokenCommand);
    }

    private void AddShowConfigCommand()
    {
        var showConfigCommand = new Command("show", "Display current configuration settings");

        showConfigCommand.SetHandler(async () =>
        {
            try
            {
                Console.WriteLine("Current Configuration:");
                Console.WriteLine("=====================");
                
                // Token status
                var token = await AuthenticationService.GetTokenAsync();
                var tokenStatus = string.IsNullOrEmpty(token) ? "Not set" : MaskToken(token);
                Console.WriteLine($"Personal Access Token: {tokenStatus}");
                
                // Other settings
                Console.WriteLine($"Default Output Format: {Configuration.GetDefaultOutputFormat()}");
                Console.WriteLine($"Default Page Size: {Configuration.GetDefaultPageSize()}");
                Console.WriteLine($"Verbose Logging: {Configuration.GetVerboseLogging()}");
                
                // Available formatters
                Console.WriteLine();
                Console.WriteLine("Available Output Formats:");
                foreach (var formatter in Formatters)
                {
                    Console.WriteLine($"  - {formatter.OutputFormat.ToString().ToLowerInvariant()}");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "showing configuration");
            }
        });

        AddCommand(showConfigCommand);
    }

    private string MaskToken(string token)
    {
        if (string.IsNullOrEmpty(token) || token.Length < 12)
        {
            return "***masked***";
        }

        // Show first 8 characters, mask middle, show last 4
        var start = token.Substring(0, 8);
        var end = token.Substring(token.Length - 4);
        return $"{start}***{end}";
    }
}