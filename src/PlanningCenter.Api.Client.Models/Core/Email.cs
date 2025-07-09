namespace PlanningCenter.Api.Client.Models.Core;

/// <summary>
/// Represents an email address for a person.
/// </summary>
public class Email
{
    /// <summary>
    /// Unique identifier for the email
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// The email address
    /// </summary>
    public string Address { get; set; } = string.Empty;
    
    /// <summary>
    /// Email type or location (Home, Work, etc.)
    /// </summary>
    public string Location { get; set; } = "Home";
    
    /// <summary>
    /// Whether this is the primary email address
    /// </summary>
    public bool IsPrimary { get; set; }
    
    /// <summary>
    /// Whether the email address has been verified
    /// </summary>
    public bool IsVerified { get; set; }
    
    /// <summary>
    /// Whether the email address is blocked or bouncing
    /// </summary>
    public bool IsBlocked { get; set; }
    
    /// <summary>
    /// When the email was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// When the email was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
    
    /// <summary>
    /// Validates the email address format
    /// </summary>
    public bool IsValidFormat
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Address))
                return false;
            
            try
            {
                var addr = new System.Net.Mail.MailAddress(Address);
                return addr.Address == Address;
            }
            catch
            {
                return false;
            }
        }
    }
    
    /// <summary>
    /// Gets the domain part of the email address
    /// </summary>
    public string? Domain
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Address))
                return null;
            
            var atIndex = Address.LastIndexOf('@');
            return atIndex >= 0 && atIndex < Address.Length - 1 
                ? Address.Substring(atIndex + 1) 
                : null;
        }
    }
    
    /// <summary>
    /// Gets the local part (before @) of the email address
    /// </summary>
    public string? LocalPart
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Address))
                return null;
            
            var atIndex = Address.LastIndexOf('@');
            return atIndex > 0 
                ? Address.Substring(0, atIndex) 
                : null;
        }
    }
}