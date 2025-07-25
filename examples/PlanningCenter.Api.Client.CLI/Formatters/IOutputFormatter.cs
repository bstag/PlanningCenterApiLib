using PlanningCenter.Api.Client.CLI.Configuration;

namespace PlanningCenter.Api.Client.CLI.Formatters;

/// <summary>
/// Interface for output formatters
/// </summary>
public interface IOutputFormatter
{
    /// <summary>
    /// The output format this formatter supports
    /// </summary>
    OutputFormat OutputFormat { get; }

    /// <summary>
    /// Formats a collection of objects for output
    /// </summary>
    /// <typeparam name="T">The type of objects to format</typeparam>
    /// <param name="data">The data to format</param>
    /// <param name="options">Formatting options</param>
    /// <returns>Formatted string</returns>
    string Format<T>(IEnumerable<T> data, FormatterOptions? options = null);

    /// <summary>
    /// Formats a single object for output
    /// </summary>
    /// <typeparam name="T">The type of object to format</typeparam>
    /// <param name="data">The data to format</param>
    /// <param name="options">Formatting options</param>
    /// <returns>Formatted string</returns>
    string Format<T>(T data, FormatterOptions? options = null);
}

/// <summary>
/// Options for formatting output
/// </summary>
public class FormatterOptions
{
    /// <summary>
    /// Properties to include in the output (null means all properties)
    /// </summary>
    public string[]? IncludeProperties { get; set; }

    /// <summary>
    /// Properties to exclude from the output
    /// </summary>
    public string[]? ExcludeProperties { get; set; }

    /// <summary>
    /// Whether to include null values in the output
    /// </summary>
    public bool IncludeNullValues { get; set; } = false;

    /// <summary>
    /// Maximum width for table columns (table format only)
    /// </summary>
    public int MaxColumnWidth { get; set; } = 50;

    /// <summary>
    /// Whether to use indented formatting (JSON/XML)
    /// </summary>
    public bool Indent { get; set; } = true;

    /// <summary>
    /// Output file path (if specified, output will be written to file)
    /// </summary>
    public string? OutputFile { get; set; }
}