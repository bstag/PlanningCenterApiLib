using Microsoft.Extensions.Logging;
using PlanningCenter.Api.Client.CLI.Configuration;
using PlanningCenter.Api.Client.CLI.Formatters;
using PlanningCenter.Api.Client.CLI.Services;
using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;
using System.CommandLine;
using System.CommandLine.Invocation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace PlanningCenter.Api.Client.CLI.Commands;

/// <summary>
/// Base class for all CLI commands providing common functionality
/// </summary>
public abstract class BaseCommand : Command
{
    protected readonly ILogger Logger;
    protected readonly IAuthenticationService AuthenticationService;
    protected readonly IServiceProvider ServiceProvider;
    protected readonly IEnumerable<IOutputFormatter> Formatters;
    protected readonly CliConfiguration Configuration;

    protected BaseCommand(
        string name,
        string description,
        ILogger logger,
        IAuthenticationService authenticationService,
        IServiceProvider serviceProvider,
        IEnumerable<IOutputFormatter> formatters,
        CliConfiguration configuration) : base(name, description)
    {
        Logger = logger;
        AuthenticationService = authenticationService;
        ServiceProvider = serviceProvider;
        Formatters = formatters;
        Configuration = configuration;
    }

    /// <summary>
    /// Gets the output formatter for the specified format
    /// </summary>
    protected IOutputFormatter GetFormatter(OutputFormat format)
    {
        var formatter = Formatters.FirstOrDefault(f => f.OutputFormat == format);
        if (formatter == null)
        {
            throw new InvalidOperationException($"No formatter found for format: {format}");
        }
        return formatter;
    }

    /// <summary>
    /// Configures authentication for the current request
    /// </summary>
    protected void ConfigureAuthentication(string? token, bool? enableDetailedLogging = null)
    {
        try
        {
            // Get token from parameter or configuration
            var authToken = AuthenticationService.GetToken(token);
            
            if (string.IsNullOrEmpty(authToken))
            {
                throw new InvalidOperationException("No Personal Access Token provided. Use --token parameter or run 'config set-token' command.");
            }

            // Update the options with the actual token and detailed logging setting
            var options = ServiceProvider.GetRequiredService<IOptions<PlanningCenterOptions>>();
            options.Value.PersonalAccessToken = authToken;
            
            if (enableDetailedLogging.HasValue)
            {
                options.Value.EnableDetailedLogging = enableDetailedLogging.Value;
                Logger.LogDebug("Detailed logging {Status}", enableDetailedLogging.Value ? "enabled" : "disabled");
            }
            
            Logger.LogDebug("Successfully configured authentication");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to configure authentication");
            throw;
        }
    }

    /// <summary>
    /// Gets a service instance with authentication configured
    /// </summary>
    protected T GetService<T>(string? token, bool? enableDetailedLogging = null) where T : class
    {
        ConfigureAuthentication(token, enableDetailedLogging);
        return ServiceProvider.GetRequiredService<T>();
    }

    /// <summary>
    /// Formats and outputs data using the specified formatter
    /// </summary>
    protected void OutputData<T>(IEnumerable<T> data, OutputFormat format, FormatterOptions? options = null)
    {
        try
        {
            var formatter = GetFormatter(format);
            var output = formatter.Format(data, options);
            Console.WriteLine(output);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to format output data");
            Console.WriteLine($"Error formatting output: {ex.Message}");
        }
    }

    /// <summary>
    /// Formats and outputs a single data item using the specified formatter
    /// </summary>
    protected void OutputData<T>(T data, OutputFormat format, FormatterOptions? options = null)
    {
        try
        {
            var formatter = GetFormatter(format);
            var output = formatter.Format(data, options);
            Console.WriteLine(output);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to format output data");
            Console.WriteLine($"Error formatting output: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles common command errors and outputs user-friendly messages
    /// </summary>
    protected void HandleError(Exception ex, string operation)
    {
        Logger.LogError(ex, "Command failed during {Operation}", operation);
        
        var message = ex switch
        {
            UnauthorizedAccessException => "Authentication failed. Please check your Personal Access Token.",
            HttpRequestException httpEx when httpEx.Message.Contains("401") => "Authentication failed. Please check your Personal Access Token.",
            HttpRequestException httpEx when httpEx.Message.Contains("403") => "Access denied. You don't have permission to perform this operation.",
            HttpRequestException httpEx when httpEx.Message.Contains("404") => "Resource not found. Please check the ID or parameters.",
            HttpRequestException httpEx when httpEx.Message.Contains("429") => "Rate limit exceeded. Please wait and try again.",
            HttpRequestException => "Network error occurred. Please check your connection and try again.",
            ArgumentException argEx => $"Invalid argument: {argEx.Message}",
            InvalidOperationException => ex.Message,
            _ => $"An unexpected error occurred: {ex.Message}"
        };

        Console.WriteLine($"Error: {message}");
        
        if (Logger.IsEnabled(LogLevel.Debug))
        {
            Console.WriteLine($"Details: {ex}");
        }
    }

    /// <summary>
    /// Creates formatter options from command parameters
    /// </summary>
    protected FormatterOptions CreateFormatterOptions(
        string[]? includeProperties = null,
        string[]? excludeProperties = null,
        bool includeNullValues = false,
        int maxColumnWidth = 50,
        bool indent = true,
        string? outputFile = null)
    {
        return new FormatterOptions
        {
            IncludeProperties = includeProperties,
            ExcludeProperties = excludeProperties,
            IncludeNullValues = includeNullValues,
            MaxColumnWidth = maxColumnWidth,
            Indent = indent,
            OutputFile = outputFile
        };
    }

    /// <summary>
    /// Validates that required parameters are provided
    /// </summary>
    protected void ValidateRequiredParameter(object? value, string parameterName)
    {
        if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
        {
            throw new ArgumentException($"Required parameter '{parameterName}' is missing or empty.");
        }
    }

    /// <summary>
    /// Parses a comma-separated string into an array
    /// </summary>
    protected string[]? ParseCommaSeparatedString(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        return input.Split(',', StringSplitOptions.RemoveEmptyEntries)
                   .Select(s => s.Trim())
                   .Where(s => !string.IsNullOrEmpty(s))
                   .ToArray();
    }

    /// <summary>
    /// Parses where conditions from a string into a dictionary
    /// </summary>
    protected Dictionary<string, object> ParseWhereConditions(string whereClause)
    {
        var conditions = new Dictionary<string, object>();
        
        if (string.IsNullOrWhiteSpace(whereClause))
        {
            return conditions;
        }

        // Parse simple key=value pairs separated by commas
        var pairs = whereClause.Split(',', StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var pair in pairs)
        {
            var parts = pair.Split('=', 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                var key = parts[0].Trim();
                var value = parts[1].Trim();
                
                // Remove quotes if present
                if ((value.StartsWith('"') && value.EndsWith('"')) ||
                    (value.StartsWith('\'') && value.EndsWith('\'')))
                {
                    value = value[1..^1];
                }
                
                conditions[key] = value;
            }
        }
        
        return conditions;
    }

    /// <summary>
    /// Safely gets the token option value from global options
    /// </summary>
    protected string? GetTokenOption(InvocationContext context)
    {
        var tokenOption = context.ParseResult.RootCommandResult.Command.Options
            .OfType<Option<string?>>().FirstOrDefault(o => o.HasAlias("--token"));
        return tokenOption != null ? context.ParseResult.GetValueForOption(tokenOption) : null;
    }

    /// <summary>
    /// Safely gets the format option value from global options
    /// </summary>
    protected OutputFormat GetFormatOption(InvocationContext context)
    {
        var formatOption = context.ParseResult.RootCommandResult.Command.Options
            .OfType<Option<OutputFormat>>().FirstOrDefault(o => o.HasAlias("--format"));
        return formatOption != null ? context.ParseResult.GetValueForOption(formatOption) : OutputFormat.Json;
    }

    /// <summary>
    /// Safely gets the detailed logging option value from global options
    /// </summary>
    protected bool GetDetailedLoggingOption(InvocationContext context)
    {
        var detailedLoggingOption = context.ParseResult.RootCommandResult.Command.Options
            .OfType<Option<bool>>().FirstOrDefault(o => o.HasAlias("--detailed-logging"));
        return detailedLoggingOption != null ? context.ParseResult.GetValueForOption(detailedLoggingOption) : false;
    }

    /// <summary>
    /// Safely gets an option value by index from a command's options
    /// </summary>
    protected T GetOptionValue<T>(InvocationContext context, Command command, int optionIndex, T defaultValue = default(T)!)
    {
        if (optionIndex < 0 || optionIndex >= command.Options.Count)
            return defaultValue;
            
        var option = command.Options[optionIndex] as Option<T>;
        if (option == null)
            return defaultValue;
            
        var value = context.ParseResult.GetValueForOption(option);
        return value ?? defaultValue;
    }

    /// <summary>
    /// Safely gets an argument value by index from a command's arguments
    /// </summary>
    protected T GetArgumentValue<T>(InvocationContext context, Command command, int argumentIndex, T defaultValue = default(T)!)
    {
        if (argumentIndex < 0 || argumentIndex >= command.Arguments.Count)
            return defaultValue;
            
        var argument = command.Arguments[argumentIndex] as Argument<T>;
        if (argument == null)
            return defaultValue;
            
        var value = context.ParseResult.GetValueForArgument(argument);
        return value ?? defaultValue;
    }
}