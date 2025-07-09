using PlanningCenter.Api.Client.Models.Requests;

namespace PlanningCenter.Api.Client.Models.Fluent;

/// <summary>
/// Fluent API context for creating people with related data.
/// </summary>
public interface IPeopleCreateContext
{
    /// <summary>
    /// Adds an address to the person being created.
    /// </summary>
    /// <param name="request">The address creation request</param>
    /// <returns>The creation context for method chaining</returns>
    IPeopleCreateContext WithAddress(AddressCreateRequest request);
    
    /// <summary>
    /// Adds an email to the person being created.
    /// </summary>
    /// <param name="request">The email creation request</param>
    /// <returns>The creation context for method chaining</returns>
    IPeopleCreateContext WithEmail(EmailCreateRequest request);
    
    /// <summary>
    /// Adds a phone number to the person being created.
    /// </summary>
    /// <param name="request">The phone number creation request</param>
    /// <returns>The creation context for method chaining</returns>
    IPeopleCreateContext WithPhoneNumber(PhoneNumberCreateRequest request);
    
    /// <summary>
    /// Executes the person creation with all specified related data.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created person with all related data</returns>
    Task<Core.Person> ExecuteAsync(CancellationToken cancellationToken = default);
}