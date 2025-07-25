using PlanningCenter.Api.Client.CLI.Configuration;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace PlanningCenter.Api.Client.CLI.Formatters;

/// <summary>
/// Formatter for XML output
/// </summary>
public class XmlFormatter : IOutputFormatter
{
    public OutputFormat OutputFormat => OutputFormat.Xml;

    public string Format<T>(IEnumerable<T> data, FormatterOptions? options = null)
    {
        options ??= new FormatterOptions();
        
        var dataList = data.ToList();
        if (!dataList.Any())
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<result>No data found.</result>";
        }

        var rootElement = new XElement("items");
        
        foreach (var item in dataList)
        {
            var itemElement = CreateElementFromObject(item, "item", options);
            rootElement.Add(itemElement);
        }

        var document = new XDocument(
            new XDeclaration("1.0", "UTF-8", null),
            rootElement
        );

        var output = FormatXmlDocument(document, options);
        
        if (!string.IsNullOrEmpty(options.OutputFile))
        {
            File.WriteAllText(options.OutputFile, output, Encoding.UTF8);
            return $"XML output written to {options.OutputFile}";
        }

        return output;
    }

    public string Format<T>(T data, FormatterOptions? options = null)
    {
        options ??= new FormatterOptions();
        
        if (data == null)
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<result>No data found.</result>";
        }

        var rootElement = CreateElementFromObject(data, "item", options);
        var document = new XDocument(
            new XDeclaration("1.0", "UTF-8", null),
            rootElement
        );

        var output = FormatXmlDocument(document, options);
        
        if (!string.IsNullOrEmpty(options.OutputFile))
        {
            File.WriteAllText(options.OutputFile, output, Encoding.UTF8);
            return $"XML output written to {options.OutputFile}";
        }

        return output;
    }

    private XElement CreateElementFromObject(object? obj, string elementName, FormatterOptions options)
    {
        var element = new XElement(elementName);
        
        if (obj == null)
        {
            if (options.IncludeNullValues)
            {
                element.Add(new XAttribute("nil", "true"));
            }
            return element;
        }

        var type = obj.GetType();
        var properties = GetPropertiesToDisplay(type, options);

        foreach (var property in properties)
        {
            var value = property.GetValue(obj);
            var propertyElement = CreateElementFromValue(value, property.Name, options);
            element.Add(propertyElement);
        }

        return element;
    }

    private XElement CreateElementFromValue(object? value, string elementName, FormatterOptions options)
    {
        var element = new XElement(SanitizeElementName(elementName));
        
        if (value == null)
        {
            if (options.IncludeNullValues)
            {
                element.Add(new XAttribute("nil", "true"));
            }
            return element;
        }

        // Handle primitive types
        if (IsPrimitiveType(value.GetType()))
        {
            element.Value = FormatValue(value);
            return element;
        }

        // Handle collections
        if (value is System.Collections.IEnumerable enumerable && !(value is string))
        {
            foreach (var item in enumerable)
            {
                var itemElement = CreateElementFromValue(item, "item", options);
                element.Add(itemElement);
            }
            return element;
        }

        // Handle complex objects
        var objectElement = CreateElementFromObject(value, elementName, options);
        return objectElement;
    }

    private PropertyInfo[] GetPropertiesToDisplay(Type type, FormatterOptions options)
    {
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

    private bool IsPrimitiveType(Type type)
    {
        return type.IsPrimitive || 
               type == typeof(string) || 
               type == typeof(DateTime) || 
               type == typeof(DateTimeOffset) || 
               type == typeof(decimal) || 
               type == typeof(Guid) ||
               (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && 
                IsPrimitiveType(Nullable.GetUnderlyingType(type)!));
    }

    private string FormatValue(object value)
    {
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

        return value.ToString() ?? "";
    }

    private string SanitizeElementName(string name)
    {
        // Replace invalid XML element name characters
        var sanitized = new StringBuilder();
        
        for (int i = 0; i < name.Length; i++)
        {
            char c = name[i];
            if (char.IsLetter(c) || c == '_' || (i > 0 && (char.IsDigit(c) || c == '-' || c == '.')))
            {
                sanitized.Append(c);
            }
            else
            {
                sanitized.Append('_');
            }
        }

        var result = sanitized.ToString();
        
        // Ensure it starts with a letter or underscore
        if (result.Length > 0 && !char.IsLetter(result[0]) && result[0] != '_')
        {
            result = "_" + result;
        }

        return string.IsNullOrEmpty(result) ? "element" : result;
    }

    private string FormatXmlDocument(XDocument document, FormatterOptions options)
    {
        var settings = new XmlWriterSettings
        {
            Indent = options.Indent,
            IndentChars = "  ",
            NewLineChars = "\n",
            Encoding = Encoding.UTF8,
            OmitXmlDeclaration = false
        };

        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter, settings);
        
        document.Save(xmlWriter);
        return stringWriter.ToString();
    }
}