using System.Text.RegularExpressions;

namespace PlanningCenter.Api.Client.CLI.Utilities;

/// <summary>
/// Helper class for common validation operations
/// </summary>
public static class ValidationHelper
{
    private static readonly Regex TokenFormatRegex = new(@"^[a-f0-9]{32}:[a-f0-9]{64}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex IdRegex = new(@"^[0-9]+$", RegexOptions.Compiled);

    /// <summary>
    /// Validates Personal Access Token format
    /// </summary>
    public static bool IsValidTokenFormat(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        return TokenFormatRegex.IsMatch(token);
    }

    /// <summary>
    /// Validates email address format
    /// </summary>
    public static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        return EmailRegex.IsMatch(email);
    }

    /// <summary>
    /// Validates Planning Center ID format (numeric)
    /// </summary>
    public static bool IsValidId(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return false;
        }

        return IdRegex.IsMatch(id);
    }

    /// <summary>
    /// Validates that a string is not null or empty
    /// </summary>
    public static bool IsNotEmpty(string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// Validates that a number is within a specified range
    /// </summary>
    public static bool IsInRange(int value, int min, int max)
    {
        return value >= min && value <= max;
    }

    /// <summary>
    /// Validates page size parameter
    /// </summary>
    public static bool IsValidPageSize(int pageSize)
    {
        return IsInRange(pageSize, 1, 100);
    }

    /// <summary>
    /// Validates page number parameter
    /// </summary>
    public static bool IsValidPageNumber(int page)
    {
        return page >= 1;
    }

    /// <summary>
    /// Validates file path for output
    /// </summary>
    public static bool IsValidOutputPath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return false;
        }

        try
        {
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                return false;
            }

            // Check if the path contains invalid characters
            var invalidChars = Path.GetInvalidPathChars();
            return !path.Any(c => invalidChars.Contains(c));
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Validates that a collection is not null or empty
    /// </summary>
    public static bool IsNotEmpty<T>(IEnumerable<T>? collection)
    {
        return collection != null && collection.Any();
    }

    /// <summary>
    /// Validates URL format
    /// </summary>
    public static bool IsValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return false;
        }

        return Uri.TryCreate(url, UriKind.Absolute, out var result) &&
               (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }

    /// <summary>
    /// Validates date format
    /// </summary>
    public static bool IsValidDate(string? dateString)
    {
        if (string.IsNullOrWhiteSpace(dateString))
        {
            return false;
        }

        return DateTime.TryParse(dateString, out _);
    }

    /// <summary>
    /// Validates that a date is not in the future
    /// </summary>
    public static bool IsNotFutureDate(DateTime date)
    {
        return date <= DateTime.UtcNow;
    }

    /// <summary>
    /// Validates that a date range is valid (start before end)
    /// </summary>
    public static bool IsValidDateRange(DateTime start, DateTime end)
    {
        return start <= end;
    }
}