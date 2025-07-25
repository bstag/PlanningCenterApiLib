using System.Globalization;
using System.Text.Json;

namespace PlanningCenter.Api.Client.CLI.Utilities;

/// <summary>
/// Helper class for parsing common CLI input formats
/// </summary>
public static class ParsingHelper
{
    /// <summary>
    /// Parses a comma-separated string into an array
    /// </summary>
    public static string[]? ParseCommaSeparatedString(string? input)
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
    /// Parses where conditions from a string format like "key1=value1,key2=value2"
    /// </summary>
    public static Dictionary<string, object> ParseWhereConditions(string? whereClause)
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
                value = RemoveQuotes(value);
                
                // Try to parse as different types
                conditions[key] = ParseValue(value);
            }
        }

        return conditions;
    }

    /// <summary>
    /// Parses include parameters from a string format like "field1,field2.subfield,field3"
    /// </summary>
    public static string[]? ParseIncludeParameters(string? include)
    {
        return ParseCommaSeparatedString(include);
    }

    /// <summary>
    /// Parses order parameters from a string format like "field1,-field2,field3"
    /// </summary>
    public static string? ParseOrderParameter(string? order)
    {
        if (string.IsNullOrWhiteSpace(order))
        {
            return null;
        }

        return order.Trim();
    }

    /// <summary>
    /// Parses a JSON string into a dictionary
    /// </summary>
    public static Dictionary<string, object>? ParseJsonToDictionary(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            var jsonDocument = JsonDocument.Parse(json);
            return ParseJsonElement(jsonDocument.RootElement) as Dictionary<string, object>;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>
    /// Parses a date string in various formats
    /// </summary>
    public static DateTime? ParseDate(string? dateString)
    {
        if (string.IsNullOrWhiteSpace(dateString))
        {
            return null;
        }

        // Try common date formats
        var formats = new[]
        {
            "yyyy-MM-dd",
            "yyyy-MM-ddTHH:mm:ss",
            "yyyy-MM-ddTHH:mm:ssZ",
            "yyyy-MM-ddTHH:mm:ss.fffZ",
            "MM/dd/yyyy",
            "dd/MM/yyyy",
            "MM-dd-yyyy",
            "dd-MM-yyyy"
        };

        foreach (var format in formats)
        {
            if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            {
                return result;
            }
        }

        // Try general parsing
        if (DateTime.TryParse(dateString, out var generalResult))
        {
            return generalResult;
        }

        return null;
    }

    /// <summary>
    /// Parses a boolean string (true/false, yes/no, 1/0)
    /// </summary>
    public static bool? ParseBoolean(string? boolString)
    {
        if (string.IsNullOrWhiteSpace(boolString))
        {
            return null;
        }

        var normalized = boolString.Trim().ToLowerInvariant();
        
        return normalized switch
        {
            "true" or "yes" or "y" or "1" or "on" => true,
            "false" or "no" or "n" or "0" or "off" => false,
            _ => null
        };
    }

    /// <summary>
    /// Parses an integer with validation
    /// </summary>
    public static int? ParseInteger(string? intString, int? min = null, int? max = null)
    {
        if (string.IsNullOrWhiteSpace(intString))
        {
            return null;
        }

        if (!int.TryParse(intString.Trim(), out var result))
        {
            return null;
        }

        if (min.HasValue && result < min.Value)
        {
            return null;
        }

        if (max.HasValue && result > max.Value)
        {
            return null;
        }

        return result;
    }

    /// <summary>
    /// Parses key-value pairs from a string format like "key1:value1,key2:value2"
    /// </summary>
    public static Dictionary<string, string> ParseKeyValuePairs(string? input, char pairSeparator = ',', char keyValueSeparator = ':')
    {
        var result = new Dictionary<string, string>();
        
        if (string.IsNullOrWhiteSpace(input))
        {
            return result;
        }

        var pairs = input.Split(pairSeparator, StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var pair in pairs)
        {
            var parts = pair.Split(keyValueSeparator, 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                var key = parts[0].Trim();
                var value = RemoveQuotes(parts[1].Trim());
                result[key] = value;
            }
        }

        return result;
    }

    /// <summary>
    /// Removes surrounding quotes from a string
    /// </summary>
    private static string RemoveQuotes(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        if ((value.StartsWith('"') && value.EndsWith('"')) ||
            (value.StartsWith('\'') && value.EndsWith('\'')))
        {
            return value.Length > 2 ? value.Substring(1, value.Length - 2) : string.Empty;
        }

        return value;
    }

    /// <summary>
    /// Attempts to parse a value as the most appropriate type
    /// </summary>
    private static object ParseValue(string value)
    {
        // Try boolean
        var boolValue = ParseBoolean(value);
        if (boolValue.HasValue)
        {
            return boolValue.Value;
        }

        // Try integer
        if (int.TryParse(value, out var intValue))
        {
            return intValue;
        }

        // Try decimal
        if (decimal.TryParse(value, out var decimalValue))
        {
            return decimalValue;
        }

        // Try date
        var dateValue = ParseDate(value);
        if (dateValue.HasValue)
        {
            return dateValue.Value;
        }

        // Return as string
        return value;
    }

    /// <summary>
    /// Recursively parses a JSON element into appropriate .NET types
    /// </summary>
    private static object? ParseJsonElement(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Object => element.EnumerateObject()
                .ToDictionary(prop => prop.Name, prop => ParseJsonElement(prop.Value)),
            JsonValueKind.Array => element.EnumerateArray()
                .Select(ParseJsonElement)
                .ToArray(),
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.TryGetInt32(out var intVal) ? intVal : element.GetDecimal(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            _ => element.ToString()
        };
    }
}