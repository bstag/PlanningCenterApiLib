using ConsoleTables;
using PlanningCenter.Api.Client.CLI.Configuration;
using System.Reflection;

namespace PlanningCenter.Api.Client.CLI.Formatters;

/// <summary>
/// Formatter for table output using ConsoleTables
/// </summary>
public class TableFormatter : IOutputFormatter
{
    public OutputFormat OutputFormat => OutputFormat.Table;

    public string Format<T>(IEnumerable<T> data, FormatterOptions? options = null)
    {
        options ??= new FormatterOptions();
        
        var dataList = data.ToList();
        if (!dataList.Any())
        {
            return "No data found.";
        }

        var properties = GetPropertiesToDisplay<T>(options);
        if (!properties.Any())
        {
            return "No properties to display.";
        }

        var table = new ConsoleTable(properties.Select(p => p.Name).ToArray());
        
        foreach (var item in dataList)
        {
            var values = new object[properties.Length];
            for (int i = 0; i < properties.Length; i++)
            {
                var value = properties[i].GetValue(item);
                values[i] = FormatValue(value, options.MaxColumnWidth);
            }
            table.AddRow(values);
        }

        var output = table.ToString();
        
        if (!string.IsNullOrEmpty(options.OutputFile))
        {
            File.WriteAllText(options.OutputFile, output);
            return $"Table output written to {options.OutputFile}";
        }

        return output;
    }

    public string Format<T>(T data, FormatterOptions? options = null)
    {
        if (data == null)
        {
            return "No data found.";
        }

        return Format(new[] { data }, options);
    }

    private PropertyInfo[] GetPropertiesToDisplay<T>(FormatterOptions options)
    {
        var type = typeof(T);
        var allProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead)
            .ToArray();

        // Filter properties based on options
        var properties = allProperties.AsEnumerable();

        if (options.IncludeProperties != null && options.IncludeProperties.Any())
        {
            properties = properties.Where(p => options.IncludeProperties.Contains(p.Name, StringComparer.OrdinalIgnoreCase));
        }

        if (options.ExcludeProperties != null && options.ExcludeProperties.Any())
        {
            properties = properties.Where(p => !options.ExcludeProperties.Contains(p.Name, StringComparer.OrdinalIgnoreCase));
        }

        return properties.ToArray();
    }

    private object FormatValue(object? value, int maxWidth)
    {
        if (value == null)
        {
            return "";
        }

        var stringValue = value.ToString() ?? "";
        
        // Truncate long values
        if (stringValue.Length > maxWidth)
        {
            return stringValue.Substring(0, maxWidth - 3) + "...";
        }

        // Format dates
        if (value is DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        if (value is DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.ToString("yyyy-MM-dd HH:mm:ss zzz");
        }

        // Format booleans
        if (value is bool boolean)
        {
            return boolean ? "Yes" : "No";
        }

        return stringValue;
    }
}