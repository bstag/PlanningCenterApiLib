using Microsoft.Extensions.Configuration;
using System.Linq;

namespace PlanningCenter.Api.Client.CLI.Configuration;

/// <summary>
/// Configuration settings for the CLI application
/// </summary>
public class CliConfiguration
{
    private readonly IConfiguration _configuration;
    private readonly string _configFilePath;

    public CliConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
        _configFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".planningcenter", "config.json");
    }

    /// <summary>
    /// Gets the Personal Access Token from configuration or environment
    /// </summary>
    public string? GetPersonalAccessToken()
    {
        // Check command line argument first (handled by caller)
        // Then check environment variable
        var envToken = Environment.GetEnvironmentVariable("PLANNING_CENTER_PAT");
        if (!string.IsNullOrEmpty(envToken))
            return envToken;

        // Then check configuration file
        return _configuration["PlanningCenter:PersonalAccessToken"];
    }

    /// <summary>
    /// Sets the Personal Access Token in the user configuration file
    /// </summary>
    public async Task SetPersonalAccessTokenAsync(string token)
    {
        var configDir = Path.GetDirectoryName(_configFilePath);
        if (!Directory.Exists(configDir))
        {
            Directory.CreateDirectory(configDir!);
        }

        using var document = System.Text.Json.JsonDocument.Parse(
            File.Exists(_configFilePath) ? await File.ReadAllTextAsync(_configFilePath) : "{}");
        
        var config = new Dictionary<string, object>();
        
        // Copy existing properties
        foreach (var property in document.RootElement.EnumerateObject())
        {
            config[property.Name] = JsonElementToObject(property.Value);
        }

        // Update the token in the PlanningCenter section
        if (!config.ContainsKey("PlanningCenter"))
        {
            config["PlanningCenter"] = new Dictionary<string, object>();
        }
        
        var planningCenterConfig = config["PlanningCenter"] as Dictionary<string, object> ?? new();
        planningCenterConfig["PersonalAccessToken"] = token;
        config["PlanningCenter"] = planningCenterConfig;

        // Save the config
        var json = System.Text.Json.JsonSerializer.Serialize(config, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        });
        
        await File.WriteAllTextAsync(_configFilePath, json);
    }

    private static object JsonElementToObject(System.Text.Json.JsonElement element)
    {
        return element.ValueKind switch
        {
            System.Text.Json.JsonValueKind.Object => element.EnumerateObject()
                .ToDictionary(prop => prop.Name, prop => JsonElementToObject(prop.Value)),
            System.Text.Json.JsonValueKind.Array => element.EnumerateArray()
                .Select(JsonElementToObject).ToArray(),
            System.Text.Json.JsonValueKind.String => element.GetString()!,
            System.Text.Json.JsonValueKind.Number => element.TryGetInt32(out var i) ? i : element.GetDouble(),
            System.Text.Json.JsonValueKind.True => true,
            System.Text.Json.JsonValueKind.False => false,
            System.Text.Json.JsonValueKind.Null => null!,
            _ => element.ToString()
        };
    }

    /// <summary>
    /// Gets the default output format
    /// </summary>
    public OutputFormat GetDefaultOutputFormat()
    {
        var formatString = _configuration["PlanningCenter:DefaultOutputFormat"];
        if (Enum.TryParse<OutputFormat>(formatString, true, out var format))
        {
            return format;
        }
        return OutputFormat.Table;
    }

    /// <summary>
    /// Gets the default page size for paginated results
    /// </summary>
    public int GetDefaultPageSize()
    {
        var pageSizeString = _configuration["PlanningCenter:DefaultPageSize"];
        if (int.TryParse(pageSizeString, out var pageSize) && pageSize > 0)
        {
            return pageSize;
        }
        return 25; // Default page size
    }

    /// <summary>
    /// Gets whether to enable verbose logging
    /// </summary>
    public bool GetVerboseLogging()
    {
        var verboseString = _configuration["PlanningCenter:VerboseLogging"];
        if (bool.TryParse(verboseString, out var verbose))
        {
            return verbose;
        }
        return false;
    }
}

/// <summary>
/// Output format options for the CLI
/// </summary>
public enum OutputFormat
{
    Table,
    Json,
    Csv,
    Xml
}