using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PlanningCenter.Api.Client.CLI.Configuration;

namespace PlanningCenter.Api.Client.CLI.Formatters;

/// <summary>
/// Formatter for JSON output
/// </summary>
public class JsonFormatter : IOutputFormatter
{
    public OutputFormat OutputFormat => OutputFormat.Json;

    public string Format<T>(IEnumerable<T> data, FormatterOptions? options = null)
    {
        options ??= new FormatterOptions();
        
        var settings = CreateJsonSettings(options);
        var output = JsonConvert.SerializeObject(data, settings);
        
        if (!string.IsNullOrEmpty(options.OutputFile))
        {
            File.WriteAllText(options.OutputFile, output);
            return $"JSON output written to {options.OutputFile}";
        }

        return output;
    }

    public string Format<T>(T data, FormatterOptions? options = null)
    {
        options ??= new FormatterOptions();
        
        var settings = CreateJsonSettings(options);
        var output = JsonConvert.SerializeObject(data, settings);
        
        if (!string.IsNullOrEmpty(options.OutputFile))
        {
            File.WriteAllText(options.OutputFile, output);
            return $"JSON output written to {options.OutputFile}";
        }

        return output;
    }

    private JsonSerializerSettings CreateJsonSettings(FormatterOptions options)
    {
        var settings = new JsonSerializerSettings
        {
            Formatting = options.Indent ? Formatting.Indented : Formatting.None,
            NullValueHandling = options.IncludeNullValues ? NullValueHandling.Include : NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc
        };

        // Apply property filtering if specified
        if (options.IncludeProperties != null || options.ExcludeProperties != null)
        {
            settings.ContractResolver = new PropertyFilterContractResolver(options);
        }

        return settings;
    }
}

/// <summary>
/// Custom contract resolver for filtering properties
/// </summary>
public class PropertyFilterContractResolver : CamelCasePropertyNamesContractResolver
{
    private readonly FormatterOptions _options;

    public PropertyFilterContractResolver(FormatterOptions options)
    {
        _options = options;
    }

    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        var properties = base.CreateProperties(type, memberSerialization);

        if (_options.IncludeProperties != null && _options.IncludeProperties.Any())
        {
            properties = properties
                .Where(p => _options.IncludeProperties.Contains(p.PropertyName, StringComparer.OrdinalIgnoreCase))
                .ToList();
        }

        if (_options.ExcludeProperties != null && _options.ExcludeProperties.Any())
        {
            properties = properties
                .Where(p => !_options.ExcludeProperties.Contains(p.PropertyName, StringComparer.OrdinalIgnoreCase))
                .ToList();
        }

        return properties;
    }
}