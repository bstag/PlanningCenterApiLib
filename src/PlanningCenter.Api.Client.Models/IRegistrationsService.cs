using PlanningCenter.Api.Client.Models.Registrations;
using PlanningCenter.Api.Client.Models.Requests;

namespace PlanningCenter.Api.Client.Models;

/// <summary>
/// Service interface for the Planning Center Registrations module.
/// Provides comprehensive event registration and attendee management with built-in pagination support.
/// </summary>
public interface IRegistrationsService
{
    // Signup management
    
    /// <summary>
    /// Gets a single signup by ID.
    /// </summary>
    /// <param name="id">The signup's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The signup, or null if not found</returns>
    Task<Signup?> GetSignupAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists signups with optional filtering, sorting, and pagination.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with signups</returns>
    Task<IPagedResponse<Signup>> ListSignupsAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new signup.
    /// </summary>
    /// <param name="request">The signup creation request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created signup</returns>
    Task<Signup> CreateSignupAsync(SignupCreateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing signup.
    /// </summary>
    /// <param name="id">The signup's unique identifier</param>
    /// <param name="request">The signup update request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The updated signup</returns>
    Task<Signup> UpdateSignupAsync(string id, SignupUpdateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a signup.
    /// </summary>
    /// <param name="id">The signup's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task DeleteSignupAsync(string id, CancellationToken cancellationToken = default);
    
    // Registration processing
    
    /// <summary>
    /// Gets a single registration by ID.
    /// </summary>
    /// <param name="id">The registration's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The registration, or null if not found</returns>
    Task<Registration?> GetRegistrationAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists registrations for a specific signup.
    /// </summary>
    /// <param name="signupId">The signup's unique identifier</param>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with registrations</returns>
    Task<IPagedResponse<Registration>> ListRegistrationsAsync(string signupId, QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Submits a new registration for a signup.
    /// </summary>
    /// <param name="signupId">The signup's unique identifier</param>
    /// <param name="request">The registration submission request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created registration</returns>
    Task<Registration> SubmitRegistrationAsync(string signupId, RegistrationCreateRequest request, CancellationToken cancellationToken = default);
    
    // Attendee management
    
    /// <summary>
    /// Gets a single attendee by ID.
    /// </summary>
    /// <param name="id">The attendee's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The attendee, or null if not found</returns>
    Task<Attendee?> GetAttendeeAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists attendees for a specific signup.
    /// </summary>
    /// <param name="signupId">The signup's unique identifier</param>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with attendees</returns>
    Task<IPagedResponse<Attendee>> ListAttendeesAsync(string signupId, QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Adds a new attendee to a signup.
    /// </summary>
    /// <param name="signupId">The signup's unique identifier</param>
    /// <param name="request">The attendee creation request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created attendee</returns>
    Task<Attendee> AddAttendeeAsync(string signupId, AttendeeCreateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing attendee.
    /// </summary>
    /// <param name="id">The attendee's unique identifier</param>
    /// <param name="request">The attendee update request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The updated attendee</returns>
    Task<Attendee> UpdateAttendeeAsync(string id, AttendeeUpdateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Removes an attendee from a signup.
    /// </summary>
    /// <param name="id">The attendee's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task DeleteAttendeeAsync(string id, CancellationToken cancellationToken = default);
    
    // Waitlist management
    
    /// <summary>
    /// Adds an attendee to the waitlist for a signup.
    /// </summary>
    /// <param name="signupId">The signup's unique identifier</param>
    /// <param name="request">The attendee creation request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The waitlisted attendee</returns>
    Task<Attendee> AddToWaitlistAsync(string signupId, AttendeeCreateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Removes an attendee from the waitlist.
    /// </summary>
    /// <param name="attendeeId">The attendee's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The updated attendee</returns>
    Task<Attendee> RemoveFromWaitlistAsync(string attendeeId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Promotes an attendee from the waitlist to confirmed status.
    /// </summary>
    /// <param name="attendeeId">The attendee's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The promoted attendee</returns>
    Task<Attendee> PromoteFromWaitlistAsync(string attendeeId, CancellationToken cancellationToken = default);
    
    // Selection type management
    
    /// <summary>
    /// Gets a single selection type by ID.
    /// </summary>
    /// <param name="id">The selection type's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The selection type, or null if not found</returns>
    Task<SelectionType?> GetSelectionTypeAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists selection types for a specific signup.
    /// </summary>
    /// <param name="signupId">The signup's unique identifier</param>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with selection types</returns>
    Task<IPagedResponse<SelectionType>> ListSelectionTypesAsync(string signupId, QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new selection type for a signup.
    /// </summary>
    /// <param name="signupId">The signup's unique identifier</param>
    /// <param name="request">The selection type creation request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created selection type</returns>
    Task<SelectionType> CreateSelectionTypeAsync(string signupId, SelectionTypeCreateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing selection type.
    /// </summary>
    /// <param name="id">The selection type's unique identifier</param>
    /// <param name="request">The selection type update request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The updated selection type</returns>
    Task<SelectionType> UpdateSelectionTypeAsync(string id, SelectionTypeUpdateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a selection type.
    /// </summary>
    /// <param name="id">The selection type's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task DeleteSelectionTypeAsync(string id, CancellationToken cancellationToken = default);
    
    // Location management
    
    /// <summary>
    /// Gets the location for a specific signup.
    /// </summary>
    /// <param name="signupId">The signup's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The signup location, or null if not found</returns>
    Task<SignupLocation?> GetSignupLocationAsync(string signupId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Sets the location for a signup.
    /// </summary>
    /// <param name="signupId">The signup's unique identifier</param>
    /// <param name="request">The location creation request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created signup location</returns>
    Task<SignupLocation> SetSignupLocationAsync(string signupId, SignupLocationCreateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates the location for a signup.
    /// </summary>
    /// <param name="signupId">The signup's unique identifier</param>
    /// <param name="request">The location update request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The updated signup location</returns>
    Task<SignupLocation> UpdateSignupLocationAsync(string signupId, SignupLocationUpdateRequest request, CancellationToken cancellationToken = default);
    
    // Time management
    
    /// <summary>
    /// Lists signup times for a specific signup.
    /// </summary>
    /// <param name="signupId">The signup's unique identifier</param>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with signup times</returns>
    Task<IPagedResponse<SignupTime>> ListSignupTimesAsync(string signupId, QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Adds a new time to a signup.
    /// </summary>
    /// <param name="signupId">The signup's unique identifier</param>
    /// <param name="request">The signup time creation request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created signup time</returns>
    Task<SignupTime> AddSignupTimeAsync(string signupId, SignupTimeCreateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing signup time.
    /// </summary>
    /// <param name="id">The signup time's unique identifier</param>
    /// <param name="request">The signup time update request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The updated signup time</returns>
    Task<SignupTime> UpdateSignupTimeAsync(string id, SignupTimeUpdateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a signup time.
    /// </summary>
    /// <param name="id">The signup time's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task DeleteSignupTimeAsync(string id, CancellationToken cancellationToken = default);
    
    // Emergency contact management
    
    /// <summary>
    /// Gets the emergency contact for an attendee.
    /// </summary>
    /// <param name="attendeeId">The attendee's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The emergency contact, or null if not found</returns>
    Task<EmergencyContact?> GetEmergencyContactAsync(string attendeeId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Sets the emergency contact for an attendee.
    /// </summary>
    /// <param name="attendeeId">The attendee's unique identifier</param>
    /// <param name="request">The emergency contact creation request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created emergency contact</returns>
    Task<EmergencyContact> SetEmergencyContactAsync(string attendeeId, EmergencyContactCreateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates the emergency contact for an attendee.
    /// </summary>
    /// <param name="attendeeId">The attendee's unique identifier</param>
    /// <param name="request">The emergency contact update request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The updated emergency contact</returns>
    Task<EmergencyContact> UpdateEmergencyContactAsync(string attendeeId, EmergencyContactUpdateRequest request, CancellationToken cancellationToken = default);
    
    // Category management
    
    /// <summary>
    /// Gets a single category by ID.
    /// </summary>
    /// <param name="id">The category's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The category, or null if not found</returns>
    Task<Category?> GetCategoryAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists categories with optional filtering, sorting, and pagination.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with categories</returns>
    Task<IPagedResponse<Category>> ListCategoriesAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new category.
    /// </summary>
    /// <param name="request">The category creation request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created category</returns>
    Task<Category> CreateCategoryAsync(CategoryCreateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing category.
    /// </summary>
    /// <param name="id">The category's unique identifier</param>
    /// <param name="request">The category update request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The updated category</returns>
    Task<Category> UpdateCategoryAsync(string id, CategoryUpdateRequest request, CancellationToken cancellationToken = default);
    
    // Campus management
    
    /// <summary>
    /// Gets a single campus by ID.
    /// </summary>
    /// <param name="id">The campus's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The campus, or null if not found</returns>
    Task<Campus?> GetCampusAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists campuses with optional filtering, sorting, and pagination.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with campuses</returns>
    Task<IPagedResponse<Campus>> ListCampusesAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    // Person-specific operations
    
    /// <summary>
    /// Gets a person from the Registrations module.
    /// </summary>
    /// <param name="id">The person's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The person, or null if not found</returns>
    Task<Core.Person?> GetPersonAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets attendee records for a specific person.
    /// </summary>
    /// <param name="personId">The person's unique identifier</param>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with the person's attendee records</returns>
    Task<IPagedResponse<Attendee>> GetAttendeesForPersonAsync(string personId, QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets signups that a person has registered for.
    /// </summary>
    /// <param name="personId">The person's unique identifier</param>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with signups the person has registered for</returns>
    Task<IPagedResponse<Signup>> GetSignupsForPersonAsync(string personId, QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    // Reporting
    
    /// <summary>
    /// Generates a registration report for a signup.
    /// </summary>
    /// <param name="request">The registration report request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The generated registration report</returns>
    Task<RegistrationReport> GenerateRegistrationReportAsync(RegistrationReportRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the registration count for a signup.
    /// </summary>
    /// <param name="signupId">The signup's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The number of registrations</returns>
    Task<int> GetRegistrationCountAsync(string signupId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the waitlist count for a signup.
    /// </summary>
    /// <param name="signupId">The signup's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The number of people on the waitlist</returns>
    Task<int> GetWaitlistCountAsync(string signupId, CancellationToken cancellationToken = default);
    
    // Pagination helpers that eliminate manual pagination logic
    
    /// <summary>
    /// Gets all signups matching the specified criteria.
    /// This method automatically handles pagination behind the scenes.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>All signups matching the criteria</returns>
    Task<IReadOnlyList<Signup>> GetAllSignupsAsync(
        QueryParameters? parameters = null,
        PaginationOptions? options = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Streams signups matching the specified criteria for memory-efficient processing.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>An async enumerable that yields signups from all pages</returns>
    IAsyncEnumerable<Signup> StreamSignupsAsync(
        QueryParameters? parameters = null,
        PaginationOptions? options = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Registration report data.
/// </summary>
public class RegistrationReport
{
    /// <summary>
    /// Gets or sets the signup ID.
    /// </summary>
    public string SignupId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the signup name.
    /// </summary>
    public string SignupName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the total registration count.
    /// </summary>
    public int TotalRegistrations { get; set; }
    
    /// <summary>
    /// Gets or sets the waitlist count.
    /// </summary>
    public int WaitlistCount { get; set; }
    
    /// <summary>
    /// Gets or sets the report generation timestamp.
    /// </summary>
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Gets or sets additional report data.
    /// </summary>
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

/// <summary>
/// Request for generating registration reports.
/// </summary>
public class RegistrationReportRequest
{
    /// <summary>
    /// Gets or sets the signup ID to generate a report for.
    /// </summary>
    public string SignupId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets whether to include attendee details.
    /// </summary>
    public bool IncludeAttendeeDetails { get; set; } = false;
    
    /// <summary>
    /// Gets or sets whether to include waitlist information.
    /// </summary>
    public bool IncludeWaitlist { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the report format.
    /// </summary>
    public string Format { get; set; } = "json";
}