using PlanningCenter.Api.Client.Models.Requests;

namespace PlanningCenter.Api.Client.Models;

/// <summary>
/// Service interface for the Planning Center People module.
/// Provides comprehensive people management with built-in pagination support.
/// </summary>
public interface IPeopleService
{
    // Basic CRUD operations
    
    /// <summary>
    /// Gets the current authenticated user's person record.
    /// This is useful for testing authentication and getting the current user's information.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The current user's person record</returns>
    Task<Core.Person> GetMeAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a single person by ID.
    /// </summary>
    /// <param name="id">The person's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The person, or null if not found</returns>
    Task<Core.Person?> GetAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists people with optional filtering, sorting, and pagination.
    /// Returns a paginated response with built-in navigation helpers.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with built-in pagination helpers</returns>
    Task<IPagedResponse<Core.Person>> ListAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new person.
    /// </summary>
    /// <param name="request">The person creation request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created person</returns>
    Task<Core.Person> CreateAsync(PersonCreateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing person.
    /// </summary>
    /// <param name="id">The person's unique identifier</param>
    /// <param name="request">The person update request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The updated person</returns>
    Task<Core.Person> UpdateAsync(string id, PersonUpdateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a person.
    /// </summary>
    /// <param name="id">The person's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    
    // Pagination helpers that eliminate manual pagination logic
    
    /// <summary>
    /// Gets all people matching the specified criteria.
    /// This method automatically handles pagination behind the scenes.
    /// Use with caution for large datasets - consider using StreamAsync for memory efficiency.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>All people matching the criteria</returns>
    Task<IReadOnlyList<Core.Person>> GetAllAsync(
        QueryParameters? parameters = null,
        PaginationOptions? options = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Streams people matching the specified criteria for memory-efficient processing.
    /// This method automatically handles pagination while using minimal memory.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>An async enumerable that yields people from all pages</returns>
    IAsyncEnumerable<Core.Person> StreamAsync(
        QueryParameters? parameters = null,
        PaginationOptions? options = null,
        CancellationToken cancellationToken = default);
    
    // Address management
    
    /// <summary>
    /// Adds an address to a person.
    /// </summary>
    /// <param name="personId">The person's unique identifier</param>
    /// <param name="request">The address creation request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created address</returns>
    Task<Core.Address> AddAddressAsync(string personId, AddressCreateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an address for a person.
    /// </summary>
    /// <param name="personId">The person's unique identifier</param>
    /// <param name="addressId">The address's unique identifier</param>
    /// <param name="request">The address update request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The updated address</returns>
    Task<Core.Address> UpdateAddressAsync(string personId, string addressId, AddressUpdateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Removes an address from a person.
    /// </summary>
    /// <param name="personId">The person's unique identifier</param>
    /// <param name="addressId">The address's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task DeleteAddressAsync(string personId, string addressId, CancellationToken cancellationToken = default);
    
    // Email management
    
    /// <summary>
    /// Adds an email address to a person.
    /// </summary>
    /// <param name="personId">The person's unique identifier</param>
    /// <param name="request">The email creation request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created email</returns>
    Task<Core.Email> AddEmailAsync(string personId, EmailCreateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an email address for a person.
    /// </summary>
    /// <param name="personId">The person's unique identifier</param>
    /// <param name="emailId">The email's unique identifier</param>
    /// <param name="request">The email update request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The updated email</returns>
    Task<Core.Email> UpdateEmailAsync(string personId, string emailId, EmailUpdateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Removes an email address from a person.
    /// </summary>
    /// <param name="personId">The person's unique identifier</param>
    /// <param name="emailId">The email's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task DeleteEmailAsync(string personId, string emailId, CancellationToken cancellationToken = default);
    
    // Phone number management
    
    /// <summary>
    /// Adds a phone number to a person.
    /// </summary>
    /// <param name="personId">The person's unique identifier</param>
    /// <param name="request">The phone number creation request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created phone number</returns>
    Task<Core.PhoneNumber> AddPhoneNumberAsync(string personId, PhoneNumberCreateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates a phone number for a person.
    /// </summary>
    /// <param name="personId">The person's unique identifier</param>
    /// <param name="phoneId">The phone number's unique identifier</param>
    /// <param name="request">The phone number update request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The updated phone number</returns>
    Task<Core.PhoneNumber> UpdatePhoneNumberAsync(string personId, string phoneId, PhoneNumberUpdateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Removes a phone number from a person.
    /// </summary>
    /// <param name="personId">The person's unique identifier</param>
    /// <param name="phoneId">The phone number's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task DeletePhoneNumberAsync(string personId, string phoneId, CancellationToken cancellationToken = default);
}