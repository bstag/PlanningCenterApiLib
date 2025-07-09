namespace PlanningCenter.Api.Client.Models.Core;

/// <summary>
/// Represents a phone number for a person.
/// </summary>
public class PhoneNumber
{
    /// <summary>
    /// Unique identifier for the phone number
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// The phone number (as stored in the system)
    /// </summary>
    public string Number { get; set; } = string.Empty;
    
    /// <summary>
    /// Phone number type or location (Home, Work, Mobile, etc.)
    /// </summary>
    public string Location { get; set; } = "Mobile";
    
    /// <summary>
    /// Whether this is the primary phone number
    /// </summary>
    public bool IsPrimary { get; set; }
    
    /// <summary>
    /// Whether this phone number can receive SMS messages
    /// </summary>
    public bool CanReceiveSms { get; set; }
    
    /// <summary>
    /// Country code for international numbers
    /// </summary>
    public string? CountryCode { get; set; }
    
    /// <summary>
    /// When the phone number was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// When the phone number was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
    
    /// <summary>
    /// Gets the phone number formatted for display
    /// </summary>
    public string FormattedNumber
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Number))
                return string.Empty;
            
            // Remove all non-digit characters
            var digits = new string(Number.Where(char.IsDigit).ToArray());
            
            // Format US phone numbers
            if (digits.Length == 10)
            {
                return $"({digits.Substring(0, 3)}) {digits.Substring(3, 3)}-{digits.Substring(6, 4)}";
            }
            
            // Format US phone numbers with country code
            if (digits.Length == 11 && digits.StartsWith("1"))
            {
                var localDigits = digits.Substring(1);
                return $"+1 ({localDigits.Substring(0, 3)}) {localDigits.Substring(3, 3)}-{localDigits.Substring(6, 4)}";
            }
            
            // For other formats, return as-is or with country code
            if (!string.IsNullOrWhiteSpace(CountryCode) && !Number.StartsWith("+"))
            {
                return $"+{CountryCode} {Number}";
            }
            
            return Number;
        }
    }
    
    /// <summary>
    /// Gets the phone number in E.164 format (international standard)
    /// </summary>
    public string? E164Format
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Number))
                return null;
            
            var digits = new string(Number.Where(char.IsDigit).ToArray());
            
            // US numbers
            if (digits.Length == 10)
            {
                return $"+1{digits}";
            }
            
            if (digits.Length == 11 && digits.StartsWith("1"))
            {
                return $"+{digits}";
            }
            
            // International numbers
            if (!string.IsNullOrWhiteSpace(CountryCode))
            {
                return $"+{CountryCode}{digits}";
            }
            
            // If it already starts with +, assume it's in E.164 format
            if (Number.StartsWith("+"))
            {
                return Number;
            }
            
            return null;
        }
    }
    
    /// <summary>
    /// Gets just the digits of the phone number
    /// </summary>
    public string DigitsOnly => new string(Number.Where(char.IsDigit).ToArray());
}