using CsvHelper;
using CsvHelper.Configuration;
using PlanningCenter.Api.Client.CLI.Configuration;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace PlanningCenter.Api.Client.CLI.Formatters;

/// <summary>
/// Formatter for CSV output
/// </summary>
public class CsvFormatter : IOutputFormatter
{
    public OutputFormat OutputFormat => OutputFormat.Csv;

    public string Format<T>(IEnumerable<T> data, FormatterOptions? options = null)
    {
        options ??= new FormatterOptions();
        
        var dataList = data.ToList();
        if (!dataList.Any())
        {
            return "No data found.";
        }

        using var stringWriter = new StringWriter();
        using var csvWriter = new CsvWriter(stringWriter, CreateCsvConfiguration(options));
        
        // Write headers
        var properties = GetPropertiesToDisplay<T>(options);
        foreach (var property in properties)
        {
            csvWriter.WriteField(property.Name);
        }
        csvWriter.NextRecord();

        // Write data
        foreach (var item in dataList)
        {
            foreach (var property in properties)
            {
                var value = property.GetValue(item);
                csvWriter.WriteField(FormatValue(value, options));
            }
            csvWriter.NextRecord();
        }

        var output = stringWriter.ToString();
        
        if (!string.IsNullOrEmpty(options.OutputFile))
        {
            File.WriteAllText(options.OutputFile, output, Encoding.UTF8);
            return $"CSV output written to {options.OutputFile}";
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

    private CsvConfiguration CreateCsvConfiguration(FormatterOptions options)
    {
        return new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            TrimOptions = TrimOptions.Trim,
            ShouldQuote = args => args.Field.Contains(',') || args.Field.Contains('"') || args.Field.Contains('\n')
        };
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

    private string FormatValue(object? value, FormatterOptions options)
    {
        if (value == null)
        {
            return options.IncludeNullValues ? "null" : "";
        }

        // Format dates in ISO format
        if (value is DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }

        if (value is DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz");
        }

        // Format booleans
        if (value is bool boolean)
        {
            return boolean.ToString().ToLowerInvariant();
        }

        // Handle collections
        if (value is System.Collections.IEnumerable enumerable && !(value is string))
        {
            var items = enumerable.Cast<object>().Select(x => x?.ToString() ?? "");
            return string.Join(";", items);
        }

        return value.ToString() ?? "";
    }
}