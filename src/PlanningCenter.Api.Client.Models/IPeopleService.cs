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
    
    // Workflow management
    
    /// <summary>
    /// Lists workflows with optional filtering, sorting, and pagination.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with workflows</returns>
    Task<IPagedResponse<People.Workflow>> ListWorkflowsAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a single workflow by ID.
    /// </summary>
    /// <param name="id">The workflow's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The workflow, or null if not found</returns>
    Task<People.Workflow?> GetWorkflowAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists workflow cards with optional filtering, sorting, and pagination.
    /// </summary>
    /// <param name="workflowId">The workflow's unique identifier</param>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with workflow cards</returns>
    Task<IPagedResponse<People.WorkflowCard>> ListWorkflowCardsAsync(string workflowId, QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a single workflow card by ID.
    /// </summary>
    /// <param name="workflowId">The workflow's unique identifier</param>
    /// <param name="cardId">The workflow card's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The workflow card, or null if not found</returns>
    Task<People.WorkflowCard?> GetWorkflowCardAsync(string workflowId, string cardId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new workflow card.
    /// </summary>
    /// <param name="workflowId">The workflow's unique identifier</param>
    /// <param name="request">The workflow card creation request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created workflow card</returns>
    Task<People.WorkflowCard> CreateWorkflowCardAsync(string workflowId, WorkflowCardCreateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing workflow card.
    /// </summary>
    /// <param name="workflowId">The workflow's unique identifier</param>
    /// <param name="cardId">The workflow card's unique identifier</param>
    /// <param name="request">The workflow card update request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The updated workflow card</returns>
    Task<People.WorkflowCard> UpdateWorkflowCardAsync(string workflowId, string cardId, WorkflowCardUpdateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a workflow card.
    /// </summary>
    /// <param name="workflowId">The workflow's unique identifier</param>
    /// <param name="cardId">The workflow card's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task DeleteWorkflowCardAsync(string workflowId, string cardId, CancellationToken cancellationToken = default);
    
    // Form management
    
    /// <summary>
    /// Lists forms with optional filtering, sorting, and pagination.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with forms</returns>
    Task<IPagedResponse<People.Form>> ListFormsAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a single form by ID.
    /// </summary>
    /// <param name="id">The form's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The form, or null if not found</returns>
    Task<People.Form?> GetFormAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists form submissions for a specific form.
    /// </summary>
    /// <param name="formId">The form's unique identifier</param>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with form submissions</returns>
    Task<IPagedResponse<People.FormSubmission>> ListFormSubmissionsAsync(string formId, QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a single form submission by ID.
    /// </summary>
    /// <param name="formId">The form's unique identifier</param>
    /// <param name="submissionId">The form submission's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The form submission, or null if not found</returns>
    Task<People.FormSubmission?> GetFormSubmissionAsync(string formId, string submissionId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Submits a form with the provided data.
    /// </summary>
    /// <param name="formId">The form's unique identifier</param>
    /// <param name="request">The form submission request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created form submission</returns>
    Task<People.FormSubmission> SubmitFormAsync(string formId, FormSubmitRequest request, CancellationToken cancellationToken = default);
    
    // List management
    
    /// <summary>
    /// Lists people lists with optional filtering, sorting, and pagination.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with people lists</returns>
    Task<IPagedResponse<People.PeopleList>> ListPeopleListsAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a single people list by ID.
    /// </summary>
    /// <param name="id">The list's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The people list, or null if not found</returns>
    Task<People.PeopleList?> GetPeopleListAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new people list.
    /// </summary>
    /// <param name="request">The list creation request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created people list</returns>
    Task<People.PeopleList> CreatePeopleListAsync(PeopleListCreateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing people list.
    /// </summary>
    /// <param name="id">The list's unique identifier</param>
    /// <param name="request">The list update request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The updated people list</returns>
    Task<People.PeopleList> UpdatePeopleListAsync(string id, PeopleListUpdateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a people list.
    /// </summary>
    /// <param name="id">The list's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task DeletePeopleListAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists people in a specific list.
    /// </summary>
    /// <param name="listId">The list's unique identifier</param>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with people in the list</returns>
    Task<IPagedResponse<Core.Person>> ListPeopleInListAsync(string listId, QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Adds a person to a list.
    /// </summary>
    /// <param name="listId">The list's unique identifier</param>
    /// <param name="request">The list member creation request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created list membership</returns>
    Task<People.ListMember> AddPersonToListAsync(string listId, ListMemberCreateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Removes a person from a list.
    /// </summary>
    /// <param name="listId">The list's unique identifier</param>
    /// <param name="personId">The person's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task RemovePersonFromListAsync(string listId, string personId, CancellationToken cancellationToken = default);
    
    // Household management
    
    /// <summary>
    /// Lists households with optional filtering, sorting, and pagination.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with households</returns>
    Task<IPagedResponse<People.Household>> ListHouseholdsAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a single household by ID.
    /// </summary>
    /// <param name="id">The household's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The household, or null if not found</returns>
    Task<People.Household?> GetHouseholdAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new household.
    /// </summary>
    /// <param name="request">The household creation request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created household</returns>
    Task<People.Household> CreateHouseholdAsync(HouseholdCreateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing household.
    /// </summary>
    /// <param name="id">The household's unique identifier</param>
    /// <param name="request">The household update request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The updated household</returns>
    Task<People.Household> UpdateHouseholdAsync(string id, HouseholdUpdateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a household.
    /// </summary>
    /// <param name="id">The household's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task DeleteHouseholdAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists people in a specific household.
    /// </summary>
    /// <param name="householdId">The household's unique identifier</param>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with people in the household</returns>
    Task<IPagedResponse<Core.Person>> ListPeopleInHouseholdAsync(string householdId, QueryParameters? parameters = null, CancellationToken cancellationToken = default);
}