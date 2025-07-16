using Microsoft.Extensions.Logging;
using PlanningCenter.Api.Client.Mapping.Registrations;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.Registrations;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Models.Exceptions;
using PlanningCenter.Api.Client.Models.JsonApi;
using PlanningCenter.Api.Client.Models.JsonApi.Registrations;

namespace PlanningCenter.Api.Client.Services;

/// <summary>
/// Service implementation for the Planning Center Registrations module.
/// Provides comprehensive event registration and attendee management with built-in pagination support.
/// </summary>
public class RegistrationsService : IRegistrationsService
{
    private readonly IApiConnection _apiConnection;
    private readonly ILogger<RegistrationsService> _logger;
    private const string BaseEndpoint = "/registrations/v2";

    public RegistrationsService(
        IApiConnection apiConnection,
        ILogger<RegistrationsService> logger)
    {
        _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #region Signup Management

    /// <summary>
    /// Gets a single signup by ID.
    /// </summary>
    public async Task<Signup?> GetSignupAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Signup ID cannot be null or empty", nameof(id));

        _logger.LogDebug("Getting signup with ID: {SignupId}", id);

        try
        {
            var response = await _apiConnection.GetAsync<JsonApiSingleResponse<SignupDto>>(
                $"{BaseEndpoint}/signups/{id}", cancellationToken);

            if (response?.Data == null)
            {
                _logger.LogWarning("Signup not found: {SignupId}", id);
                return null;
            }

            var signup = RegistrationsMapper.MapToDomain(response.Data);
            _logger.LogInformation("Successfully retrieved signup: {SignupId}", id);
            return signup;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            _logger.LogWarning("Signup not found: {SignupId}", id);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving signup: {SignupId}", id);
            throw;
        }
    }

    /// <summary>
    /// Lists signups with optional filtering, sorting, and pagination.
    /// </summary>
    public async Task<IPagedResponse<Signup>> ListSignupsAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Listing signups with parameters: {@Parameters}", parameters);

        try
        {
            var queryString = parameters?.ToQueryString() ?? string.Empty;
            var response = await _apiConnection.GetAsync<PagedResponse<SignupDto>>(
                $"{BaseEndpoint}/signups{queryString}", cancellationToken);

            if (response?.Data == null)
            {
                _logger.LogWarning("No signups returned from API");
                return new PagedResponse<Signup>
                {
                    Data = new List<Signup>(),
                    Meta = new PagedResponseMeta { TotalCount = 0 },
                    Links = new PagedResponseLinks()
                };
            }

            var signups = response.Data.Select(RegistrationsMapper.MapToDomain).ToList();
            
            var pagedResponse = new PagedResponse<Signup>
            {
                Data = signups,
                Meta = response.Meta,
                Links = response.Links
            };

            _logger.LogInformation("Successfully retrieved {Count} signups", signups.Count);
            return pagedResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing signups");
            throw;
        }
    }

    /// <summary>
    /// Creates a new signup.
    /// </summary>
    public async Task<Signup> CreateSignupAsync(SignupCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        _logger.LogDebug("Creating signup with name: {Name}", request.Name);

        try
        {
            var jsonApiRequest = RegistrationsMapper.MapCreateRequestToJsonApi(request);
            var response = await _apiConnection.PostAsync<JsonApiRequest<SignupCreateDto>, JsonApiSingleResponse<SignupDto>>(
                $"{BaseEndpoint}/signups", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException("Failed to create signup - no data returned");
            }

            var signup = RegistrationsMapper.MapToDomain(response.Data);
            _logger.LogInformation("Successfully created signup: {SignupId}", signup.Id);
            return signup;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating signup");
            throw;
        }
    }

    /// <summary>
    /// Updates an existing signup.
    /// </summary>
    public async Task<Signup> UpdateSignupAsync(string id, SignupUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Signup ID cannot be null or empty", nameof(id));
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        _logger.LogDebug("Updating signup: {SignupId}", id);

        try
        {
            var jsonApiRequest = RegistrationsMapper.MapUpdateRequestToJsonApi(id, request);
            var response = await _apiConnection.PatchAsync<JsonApiRequest<SignupUpdateDto>, JsonApiSingleResponse<SignupDto>>(
                $"{BaseEndpoint}/signups/{id}", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException($"Failed to update signup {id} - no data returned");
            }

            var signup = RegistrationsMapper.MapToDomain(response.Data);
            _logger.LogInformation("Successfully updated signup: {SignupId}", id);
            return signup;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating signup: {SignupId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a signup.
    /// </summary>
    public async Task DeleteSignupAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Signup ID cannot be null or empty", nameof(id));

        _logger.LogDebug("Deleting signup: {SignupId}", id);

        try
        {
            await _apiConnection.DeleteAsync($"{BaseEndpoint}/signups/{id}", cancellationToken);
            _logger.LogInformation("Successfully deleted signup: {SignupId}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting signup: {SignupId}", id);
            throw;
        }
    }

    #endregion

    #region Registration Processing

    /// <summary>
    /// Gets a single registration by ID.
    /// </summary>
    public async Task<Registration?> GetRegistrationAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Registration ID cannot be null or empty", nameof(id));

        _logger.LogDebug("Getting registration with ID: {RegistrationId}", id);

        try
        {
            var response = await _apiConnection.GetAsync<JsonApiSingleResponse<RegistrationDto>>(
                $"{BaseEndpoint}/registrations/{id}", cancellationToken);

            if (response?.Data == null)
            {
                _logger.LogWarning("Registration not found: {RegistrationId}", id);
                return null;
            }

            var registration = RegistrationsMapper.MapToDomain(response.Data);
            _logger.LogInformation("Successfully retrieved registration: {RegistrationId}", id);
            return registration;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            _logger.LogWarning("Registration not found: {RegistrationId}", id);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving registration: {RegistrationId}", id);
            throw;
        }
    }

    /// <summary>
    /// Lists registrations for a specific signup.
    /// </summary>
    public async Task<IPagedResponse<Registration>> ListRegistrationsAsync(string signupId, QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(signupId))
            throw new ArgumentException("Signup ID cannot be null or empty", nameof(signupId));

        _logger.LogDebug("Listing registrations for signup: {SignupId}", signupId);

        try
        {
            var queryString = parameters?.ToQueryString() ?? string.Empty;
            var response = await _apiConnection.GetAsync<PagedResponse<RegistrationDto>>(
                $"{BaseEndpoint}/signups/{signupId}/registrations{queryString}", cancellationToken);

            if (response?.Data == null)
            {
                _logger.LogWarning("No registrations returned for signup: {SignupId}", signupId);
                return new PagedResponse<Registration>
                {
                    Data = new List<Registration>(),
                    Meta = new PagedResponseMeta { TotalCount = 0 },
                    Links = new PagedResponseLinks()
                };
            }

            var registrations = response.Data.Select(RegistrationsMapper.MapToDomain).ToList();
            
            var pagedResponse = new PagedResponse<Registration>
            {
                Data = registrations,
                Meta = response.Meta,
                Links = response.Links
            };

            _logger.LogInformation("Successfully retrieved {Count} registrations for signup: {SignupId}", registrations.Count, signupId);
            return pagedResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing registrations for signup: {SignupId}", signupId);
            throw;
        }
    }

    /// <summary>
    /// Submits a new registration for a signup.
    /// </summary>
    public async Task<Registration> SubmitRegistrationAsync(string signupId, RegistrationCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(signupId))
            throw new ArgumentException("Signup ID cannot be null or empty", nameof(signupId));
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        _logger.LogDebug("Submitting registration for signup: {SignupId}", signupId);

        try
        {
            var jsonApiRequest = RegistrationsMapper.MapCreateRequestToJsonApi(request);
            var response = await _apiConnection.PostAsync<JsonApiRequest<RegistrationCreateDto>, JsonApiSingleResponse<RegistrationDto>>(
                $"{BaseEndpoint}/signups/{signupId}/registrations", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException($"Failed to submit registration for signup {signupId} - no data returned");
            }

            var registration = RegistrationsMapper.MapToDomain(response.Data);
            _logger.LogInformation("Successfully submitted registration: {RegistrationId} for signup: {SignupId}", registration.Id, signupId);
            return registration;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting registration for signup: {SignupId}", signupId);
            throw;
        }
    }

    #endregion

    #region Attendee Management

    /// <summary>
    /// Gets a single attendee by ID.
    /// </summary>
    public async Task<Attendee?> GetAttendeeAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Attendee ID cannot be null or empty", nameof(id));

        _logger.LogDebug("Getting attendee with ID: {AttendeeId}", id);

        try
        {
            var response = await _apiConnection.GetAsync<JsonApiSingleResponse<AttendeeDto>>(
                $"{BaseEndpoint}/attendees/{id}", cancellationToken);

            if (response?.Data == null)
            {
                _logger.LogWarning("Attendee not found: {AttendeeId}", id);
                return null;
            }

            var attendee = RegistrationsMapper.MapToDomain(response.Data);
            _logger.LogInformation("Successfully retrieved attendee: {AttendeeId}", id);
            return attendee;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            _logger.LogWarning("Attendee not found: {AttendeeId}", id);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving attendee: {AttendeeId}", id);
            throw;
        }
    }

    /// <summary>
    /// Lists attendees for a specific signup.
    /// </summary>
    public async Task<IPagedResponse<Attendee>> ListAttendeesAsync(string signupId, QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(signupId))
            throw new ArgumentException("Signup ID cannot be null or empty", nameof(signupId));

        _logger.LogDebug("Listing attendees for signup: {SignupId}", signupId);

        try
        {
            var queryString = parameters?.ToQueryString() ?? string.Empty;
            var response = await _apiConnection.GetAsync<PagedResponse<AttendeeDto>>(
                $"{BaseEndpoint}/signups/{signupId}/attendees{queryString}", cancellationToken);

            if (response?.Data == null)
            {
                _logger.LogWarning("No attendees returned for signup: {SignupId}", signupId);
                return new PagedResponse<Attendee>
                {
                    Data = new List<Attendee>(),
                    Meta = new PagedResponseMeta { TotalCount = 0 },
                    Links = new PagedResponseLinks()
                };
            }

            var attendees = response.Data.Select(RegistrationsMapper.MapToDomain).ToList();
            
            var pagedResponse = new PagedResponse<Attendee>
            {
                Data = attendees,
                Meta = response.Meta,
                Links = response.Links
            };

            _logger.LogInformation("Successfully retrieved {Count} attendees for signup: {SignupId}", attendees.Count, signupId);
            return pagedResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing attendees for signup: {SignupId}", signupId);
            throw;
        }
    }

    /// <summary>
    /// Adds a new attendee to a signup.
    /// </summary>
    public async Task<Attendee> AddAttendeeAsync(string signupId, AttendeeCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(signupId))
            throw new ArgumentException("Signup ID cannot be null or empty", nameof(signupId));
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        _logger.LogDebug("Adding attendee to signup: {SignupId}", signupId);

        try
        {
            var jsonApiRequest = RegistrationsMapper.MapCreateRequestToJsonApi(request);
            var response = await _apiConnection.PostAsync<JsonApiRequest<AttendeeCreateDto>, JsonApiSingleResponse<AttendeeDto>>(
                $"{BaseEndpoint}/signups/{signupId}/attendees", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException($"Failed to add attendee to signup {signupId} - no data returned");
            }

            var attendee = RegistrationsMapper.MapToDomain(response.Data);
            _logger.LogInformation("Successfully added attendee: {AttendeeId} to signup: {SignupId}", attendee.Id, signupId);
            return attendee;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding attendee to signup: {SignupId}", signupId);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing attendee.
    /// </summary>
    public async Task<Attendee> UpdateAttendeeAsync(string id, AttendeeUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Attendee ID cannot be null or empty", nameof(id));
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        _logger.LogDebug("Updating attendee: {AttendeeId}", id);

        try
        {
            var jsonApiRequest = RegistrationsMapper.MapUpdateRequestToJsonApi(id, request);
            var response = await _apiConnection.PatchAsync<JsonApiRequest<AttendeeUpdateDto>, JsonApiSingleResponse<AttendeeDto>>(
                $"{BaseEndpoint}/attendees/{id}", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException($"Failed to update attendee {id} - no data returned");
            }

            var attendee = RegistrationsMapper.MapToDomain(response.Data);
            _logger.LogInformation("Successfully updated attendee: {AttendeeId}", id);
            return attendee;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating attendee: {AttendeeId}", id);
            throw;
        }
    }

    /// <summary>
    /// Removes an attendee from a signup.
    /// </summary>
    public async Task DeleteAttendeeAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Attendee ID cannot be null or empty", nameof(id));

        _logger.LogDebug("Deleting attendee: {AttendeeId}", id);

        try
        {
            await _apiConnection.DeleteAsync($"{BaseEndpoint}/attendees/{id}", cancellationToken);
            _logger.LogInformation("Successfully deleted attendee: {AttendeeId}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting attendee: {AttendeeId}", id);
            throw;
        }
    }

    #endregion

    #region Waitlist Management

    /// <summary>
    /// Adds an attendee to the waitlist for a signup.
    /// </summary>
    public async Task<Attendee> AddToWaitlistAsync(string signupId, AttendeeCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(signupId))
            throw new ArgumentException("Signup ID cannot be null or empty", nameof(signupId));
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        _logger.LogDebug("Adding attendee to waitlist for signup: {SignupId}", signupId);

        try
        {
            var jsonApiRequest = RegistrationsMapper.MapCreateRequestToJsonApi(request);
            var response = await _apiConnection.PostAsync<JsonApiRequest<AttendeeCreateDto>, JsonApiSingleResponse<AttendeeDto>>(
                $"{BaseEndpoint}/signups/{signupId}/waitlist", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException($"Failed to add attendee to waitlist for signup {signupId} - no data returned");
            }

            var attendee = RegistrationsMapper.MapToDomain(response.Data);
            _logger.LogInformation("Successfully added attendee: {AttendeeId} to waitlist for signup: {SignupId}", attendee.Id, signupId);
            return attendee;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding attendee to waitlist for signup: {SignupId}", signupId);
            throw;
        }
    }

    /// <summary>
    /// Removes an attendee from the waitlist.
    /// </summary>
    public async Task<Attendee> RemoveFromWaitlistAsync(string attendeeId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(attendeeId))
            throw new ArgumentException("Attendee ID cannot be null or empty", nameof(attendeeId));

        _logger.LogDebug("Removing attendee from waitlist: {AttendeeId}", attendeeId);

        try
        {
            var response = await _apiConnection.PostAsync<object, JsonApiSingleResponse<AttendeeDto>>(
                $"{BaseEndpoint}/attendees/{attendeeId}/remove_from_waitlist", null, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException($"Failed to remove attendee {attendeeId} from waitlist - no data returned");
            }

            var attendee = RegistrationsMapper.MapToDomain(response.Data);
            _logger.LogInformation("Successfully removed attendee from waitlist: {AttendeeId}", attendeeId);
            return attendee;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing attendee from waitlist: {AttendeeId}", attendeeId);
            throw;
        }
    }

    /// <summary>
    /// Promotes an attendee from the waitlist to confirmed status.
    /// </summary>
    public async Task<Attendee> PromoteFromWaitlistAsync(string attendeeId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(attendeeId))
            throw new ArgumentException("Attendee ID cannot be null or empty", nameof(attendeeId));

        _logger.LogDebug("Promoting attendee from waitlist: {AttendeeId}", attendeeId);

        try
        {
            var response = await _apiConnection.PostAsync<object, JsonApiSingleResponse<AttendeeDto>>(
                $"{BaseEndpoint}/attendees/{attendeeId}/promote_from_waitlist", null, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException($"Failed to promote attendee {attendeeId} from waitlist - no data returned");
            }

            var attendee = RegistrationsMapper.MapToDomain(response.Data);
            _logger.LogInformation("Successfully promoted attendee from waitlist: {AttendeeId}", attendeeId);
            return attendee;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error promoting attendee from waitlist: {AttendeeId}", attendeeId);
            throw;
        }
    }

    #endregion

    #region Reporting

    /// <summary>
    /// Generates a registration report for a signup.
    /// </summary>
    public async Task<RegistrationReport> GenerateRegistrationReportAsync(RegistrationReportRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        _logger.LogDebug("Generating registration report for signup: {SignupId}", request.SignupId);

        try
        {
            // Get signup details
            var signup = await GetSignupAsync(request.SignupId, cancellationToken);
            if (signup == null)
            {
                throw new PlanningCenterApiNotFoundException($"Signup {request.SignupId} not found");
            }

            // Get registration count
            var registrationCount = await GetRegistrationCountAsync(request.SignupId, cancellationToken);
            var waitlistCount = await GetWaitlistCountAsync(request.SignupId, cancellationToken);

            var report = new RegistrationReport
            {
                SignupId = request.SignupId,
                SignupName = signup.Name,
                TotalRegistrations = registrationCount,
                WaitlistCount = waitlistCount,
                GeneratedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Successfully generated registration report for signup: {SignupId}", request.SignupId);
            return report;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating registration report for signup: {SignupId}", request.SignupId);
            throw;
        }
    }

    /// <summary>
    /// Gets the registration count for a signup.
    /// </summary>
    public async Task<int> GetRegistrationCountAsync(string signupId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(signupId))
            throw new ArgumentException("Signup ID cannot be null or empty", nameof(signupId));

        _logger.LogDebug("Getting registration count for signup: {SignupId}", signupId);

        try
        {
            var parameters = new QueryParameters();
            parameters.AddFilter("status", "confirmed");
            
            var response = await ListRegistrationsAsync(signupId, parameters, cancellationToken);
            var count = response.Meta?.TotalCount ?? response.Data.Count;

            _logger.LogInformation("Registration count for signup {SignupId}: {Count}", signupId, count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting registration count for signup: {SignupId}", signupId);
            throw;
        }
    }

    /// <summary>
    /// Gets the waitlist count for a signup.
    /// </summary>
    public async Task<int> GetWaitlistCountAsync(string signupId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(signupId))
            throw new ArgumentException("Signup ID cannot be null or empty", nameof(signupId));

        _logger.LogDebug("Getting waitlist count for signup: {SignupId}", signupId);

        try
        {
            var parameters = new QueryParameters();
            parameters.AddFilter("on_waitlist", "true");
            
            var response = await ListAttendeesAsync(signupId, parameters, cancellationToken);
            var count = response.Meta?.TotalCount ?? response.Data.Count;

            _logger.LogInformation("Waitlist count for signup {SignupId}: {Count}", signupId, count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting waitlist count for signup: {SignupId}", signupId);
            throw;
        }
    }

    #endregion

    #region Pagination Helpers

    /// <summary>
    /// Gets all signups matching the specified criteria.
    /// This method automatically handles pagination behind the scenes.
    /// </summary>
    public async Task<IReadOnlyList<Signup>> GetAllSignupsAsync(
        QueryParameters? parameters = null,
        PaginationOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting all signups with parameters: {@Parameters}", parameters);

        var allSignups = new List<Signup>();
        var pageSize = options?.PageSize ?? 100;
        var maxPages = options?.MaxPages ?? int.MaxValue;
        var currentPage = 0;

        try
        {
            var currentParameters = parameters ?? new QueryParameters();
            currentParameters.PerPage = pageSize;

            IPagedResponse<Signup> response;
            do
            {
                response = await ListSignupsAsync(currentParameters, cancellationToken);
                allSignups.AddRange(response.Data);
                
                currentPage++;
                if (currentPage >= maxPages)
                    break;

                // Update parameters for next page
                if (!string.IsNullOrEmpty(response.Links?.Next))
                {
                    currentParameters.Offset = (currentParameters.Offset ?? 0) + pageSize;
                }
                else
                {
                    break;
                }
            } while (!string.IsNullOrEmpty(response.Links?.Next));

            _logger.LogInformation("Retrieved {Count} total signups across {Pages} pages", allSignups.Count, currentPage);
            return allSignups.AsReadOnly();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all signups");
            throw;
        }
    }

    /// <summary>
    /// Streams signups matching the specified criteria for memory-efficient processing.
    /// </summary>
    public async IAsyncEnumerable<Signup> StreamSignupsAsync(
        QueryParameters? parameters = null,
        PaginationOptions? options = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Streaming signups with parameters: {@Parameters}", parameters);

        var pageSize = options?.PageSize ?? 100;
        var maxPages = options?.MaxPages ?? int.MaxValue;
        var currentPage = 0;

        var currentParameters = parameters ?? new QueryParameters();
        currentParameters.PerPage = pageSize;

        IPagedResponse<Signup> response;
        do
        {
            response = await ListSignupsAsync(currentParameters, cancellationToken);
            
            foreach (var signup in response.Data)
            {
                yield return signup;
            }
            
            currentPage++;
            if (currentPage >= maxPages)
                break;

            // Update parameters for next page
            if (!string.IsNullOrEmpty(response.Links?.Next))
            {
                currentParameters.Offset = (currentParameters.Offset ?? 0) + pageSize;
            }
            else
            {
                break;
            }
        } while (!string.IsNullOrEmpty(response.Links?.Next));

        _logger.LogInformation("Completed streaming signups across {Pages} pages", currentPage);
    }

    #endregion

    #region Placeholder Methods for Interface Compliance

    // Note: These methods would need full implementation based on the actual API endpoints
    // For now, providing basic implementations to satisfy the interface

    public async Task<SelectionType?> GetSelectionTypeAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Selection type ID cannot be null or empty", nameof(id));

        _logger.LogDebug("Getting selection type with ID: {SelectionTypeId}", id);

        try
        {
            // Note: This would use actual SelectionTypeDto in a complete implementation
            var response = await _apiConnection.GetAsync<JsonApiSingleResponse<dynamic>>(
                $"{BaseEndpoint}/selection_types/{id}", cancellationToken);

            if (response?.Data == null)
            {
                _logger.LogWarning("Selection type not found: {SelectionTypeId}", id);
                return null;
            }

            // This would use RegistrationsMapper.MapToDomain(response.Data) with proper SelectionTypeDto
            var selectionType = new SelectionType
            {
                Id = id,
                Name = response.Data.attributes?.name?.ToString() ?? "Unknown Selection Type",
                SignupId = response.Data.relationships?.signup?.data?.id?.ToString() ?? string.Empty,
                DataSource = "Registrations"
            };

            _logger.LogInformation("Successfully retrieved selection type: {SelectionTypeId}", id);
            return selectionType;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            _logger.LogWarning("Selection type not found: {SelectionTypeId}", id);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving selection type: {SelectionTypeId}", id);
            throw;
        }
    }

    public async Task<IPagedResponse<SelectionType>> ListSelectionTypesAsync(string signupId, QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(signupId))
            throw new ArgumentException("Signup ID cannot be null or empty", nameof(signupId));

        _logger.LogDebug("Listing selection types for signup: {SignupId}", signupId);

        try
        {
            var queryString = parameters?.ToQueryString() ?? string.Empty;
            var response = await _apiConnection.GetAsync<PagedResponse<dynamic>>(
                $"{BaseEndpoint}/signups/{signupId}/selection_types{queryString}", cancellationToken);

            if (response?.Data == null)
            {
                _logger.LogWarning("No selection types returned for signup: {SignupId}", signupId);
                return new PagedResponse<SelectionType>
                {
                    Data = new List<SelectionType>(),
                    Meta = new PagedResponseMeta { TotalCount = 0 },
                    Links = new PagedResponseLinks()
                };
            }

            // This would use RegistrationsMapper.MapToDomain for each item with proper SelectionTypeDto
            var selectionTypes = response.Data.Select(dto => new SelectionType
            {
                Id = dto.id?.ToString() ?? string.Empty,
                Name = dto.attributes?.name?.ToString() ?? "Unknown Selection Type",
                SignupId = signupId,
                DataSource = "Registrations"
            }).ToList();
            
            var pagedResponse = new PagedResponse<SelectionType>
            {
                Data = selectionTypes,
                Meta = response.Meta,
                Links = response.Links
            };

            _logger.LogInformation("Successfully retrieved {Count} selection types for signup: {SignupId}", selectionTypes.Count, signupId);
            return pagedResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing selection types for signup: {SignupId}", signupId);
            throw;
        }
    }

    public async Task<SelectionType> CreateSelectionTypeAsync(string signupId, SelectionTypeCreateRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("SelectionType operations not yet implemented");
    }

    public async Task<SelectionType> UpdateSelectionTypeAsync(string id, SelectionTypeUpdateRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("SelectionType operations not yet implemented");
    }

    public async Task DeleteSelectionTypeAsync(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("SelectionType operations not yet implemented");
    }

    public async Task<SignupLocation?> GetSignupLocationAsync(string signupId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(signupId))
            throw new ArgumentException("Signup ID cannot be null or empty", nameof(signupId));

        _logger.LogDebug("Getting signup location for signup: {SignupId}", signupId);

        try
        {
            var response = await _apiConnection.GetAsync<JsonApiSingleResponse<dynamic>>(
                $"{BaseEndpoint}/signups/{signupId}/location", cancellationToken);

            if (response?.Data == null)
            {
                _logger.LogWarning("Signup location not found for signup: {SignupId}", signupId);
                return null;
            }

            var signupLocation = new SignupLocation
            {
                Id = response.Data.id?.ToString() ?? string.Empty,
                SignupId = signupId,
                Name = response.Data.attributes?.name?.ToString() ?? "Unknown Location",
                Description = response.Data.attributes?.description?.ToString(),
                StreetAddress = response.Data.attributes?.street_address?.ToString(),
                City = response.Data.attributes?.city?.ToString(),
                State = response.Data.attributes?.state?.ToString(),
                PostalCode = response.Data.attributes?.postal_code?.ToString(),
                Country = response.Data.attributes?.country?.ToString(),
                FormattedAddress = response.Data.attributes?.formatted_address?.ToString(),
                Latitude = response.Data.attributes?.latitude,
                Longitude = response.Data.attributes?.longitude,
                PhoneNumber = response.Data.attributes?.phone_number?.ToString(),
                WebsiteUrl = response.Data.attributes?.website_url?.ToString(),
                Directions = response.Data.attributes?.directions?.ToString(),
                ParkingInfo = response.Data.attributes?.parking_info?.ToString(),
                AccessibilityInfo = response.Data.attributes?.accessibility_info?.ToString(),
                Capacity = response.Data.attributes?.capacity,
                Notes = response.Data.attributes?.notes?.ToString(),
                Timezone = response.Data.attributes?.timezone?.ToString(),
                DataSource = "Registrations"
            };

            _logger.LogInformation("Successfully retrieved signup location for signup: {SignupId}", signupId);
            return signupLocation;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            _logger.LogWarning("Signup location not found for signup: {SignupId}", signupId);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving signup location for signup: {SignupId}", signupId);
            throw;
        }
    }

    public async Task<SignupLocation> SetSignupLocationAsync(string signupId, SignupLocationCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(signupId))
            throw new ArgumentException("Signup ID cannot be null or empty", nameof(signupId));
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        _logger.LogDebug("Setting signup location for signup: {SignupId}", signupId);

        try
        {
            var jsonApiRequest = new JsonApiRequest<dynamic>
            {
                Data = new
                {
                    type = "SignupLocation",
                    attributes = new
                    {
                        name = request.Name,
                        description = request.Description,
                        street_address = request.StreetAddress,
                        city = request.City,
                        state = request.State,
                        postal_code = request.PostalCode,
                        country = request.Country,
                        latitude = request.Latitude,
                        longitude = request.Longitude,
                        phone_number = request.PhoneNumber,
                        website_url = request.WebsiteUrl,
                        directions = request.Directions,
                        parking_info = request.ParkingInfo,
                        accessibility_info = request.AccessibilityInfo,
                        capacity = request.Capacity,
                        notes = request.Notes,
                        timezone = request.Timezone
                    },
                    relationships = new
                    {
                        signup = new
                        {
                            data = new { type = "Signup", id = signupId }
                        }
                    }
                }
            };

            var response = await _apiConnection.PostAsync<JsonApiRequest<dynamic>, JsonApiSingleResponse<dynamic>>(
                $"{BaseEndpoint}/signups/{signupId}/location", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException($"Failed to set signup location for signup {signupId} - no data returned");
            }

            var signupLocation = new SignupLocation
            {
                Id = response.Data.id?.ToString() ?? string.Empty,
                SignupId = signupId,
                Name = request.Name,
                Description = request.Description,
                StreetAddress = request.StreetAddress,
                City = request.City,
                State = request.State,
                PostalCode = request.PostalCode,
                Country = request.Country,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                PhoneNumber = request.PhoneNumber,
                WebsiteUrl = request.WebsiteUrl,
                Directions = request.Directions,
                ParkingInfo = request.ParkingInfo,
                AccessibilityInfo = request.AccessibilityInfo,
                Capacity = request.Capacity,
                Notes = request.Notes,
                Timezone = request.Timezone,
                DataSource = "Registrations"
            };

            _logger.LogInformation("Successfully set signup location: {LocationId} for signup: {SignupId}", signupLocation.Id, signupId);
            return signupLocation;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting signup location for signup: {SignupId}", signupId);
            throw;
        }
    }

    public async Task<SignupLocation> UpdateSignupLocationAsync(string signupId, SignupLocationUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(signupId))
            throw new ArgumentException("Signup ID cannot be null or empty", nameof(signupId));
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        _logger.LogDebug("Updating signup location for signup: {SignupId}", signupId);

        try
        {
            var jsonApiRequest = new JsonApiRequest<dynamic>
            {
                Data = new
                {
                    type = "SignupLocation",
                    attributes = new
                    {
                        name = request.Name,
                        description = request.Description,
                        street_address = request.StreetAddress,
                        city = request.City,
                        state = request.State,
                        postal_code = request.PostalCode,
                        country = request.Country,
                        latitude = request.Latitude,
                        longitude = request.Longitude,
                        phone_number = request.PhoneNumber,
                        website_url = request.WebsiteUrl,
                        directions = request.Directions,
                        parking_info = request.ParkingInfo,
                        accessibility_info = request.AccessibilityInfo,
                        capacity = request.Capacity,
                        notes = request.Notes,
                        timezone = request.Timezone
                    }
                }
            };

            var response = await _apiConnection.PatchAsync<JsonApiRequest<dynamic>, JsonApiSingleResponse<dynamic>>(
                $"{BaseEndpoint}/signups/{signupId}/location", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException($"Failed to update signup location for signup {signupId} - no data returned");
            }

            var signupLocation = new SignupLocation
            {
                Id = response.Data.id?.ToString() ?? string.Empty,
                SignupId = signupId,
                Name = request.Name ?? "Updated Location",
                Description = request.Description,
                StreetAddress = request.StreetAddress,
                City = request.City,
                State = request.State,
                PostalCode = request.PostalCode,
                Country = request.Country,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                PhoneNumber = request.PhoneNumber,
                WebsiteUrl = request.WebsiteUrl,
                Directions = request.Directions,
                ParkingInfo = request.ParkingInfo,
                AccessibilityInfo = request.AccessibilityInfo,
                Capacity = request.Capacity,
                Notes = request.Notes,
                Timezone = request.Timezone,
                DataSource = "Registrations"
            };

            _logger.LogInformation("Successfully updated signup location for signup: {SignupId}", signupId);
            return signupLocation;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating signup location for signup: {SignupId}", signupId);
            throw;
        }
    }

    public async Task<IPagedResponse<SignupTime>> ListSignupTimesAsync(string signupId, QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(signupId))
            throw new ArgumentException("Signup ID cannot be null or empty", nameof(signupId));

        _logger.LogDebug("Listing signup times for signup: {SignupId}", signupId);

        try
        {
            var queryString = parameters?.ToQueryString() ?? string.Empty;
            var response = await _apiConnection.GetAsync<PagedResponse<dynamic>>(
                $"{BaseEndpoint}/signups/{signupId}/signup_times{queryString}", cancellationToken);

            if (response?.Data == null)
            {
                _logger.LogWarning("No signup times returned for signup: {SignupId}", signupId);
                return new PagedResponse<SignupTime>
                {
                    Data = new List<SignupTime>(),
                    Meta = new PagedResponseMeta { TotalCount = 0 },
                    Links = new PagedResponseLinks()
                };
            }

            var signupTimes = response.Data.Select(dto => new SignupTime
            {
                Id = dto.id?.ToString() ?? string.Empty,
                SignupId = signupId,
                Name = dto.attributes?.name?.ToString() ?? "Unknown Time Slot",
                Description = dto.attributes?.description?.ToString(),
                StartTime = dto.attributes?.start_time ?? DateTime.MinValue,
                EndTime = dto.attributes?.end_time,
                AllDay = dto.attributes?.all_day ?? false,
                Timezone = dto.attributes?.timezone?.ToString(),
                TimeType = dto.attributes?.time_type?.ToString(),
                Capacity = dto.attributes?.capacity,
                RegistrationCount = dto.attributes?.registration_count ?? 0,
                Required = dto.attributes?.required ?? false,
                SortOrder = dto.attributes?.sort_order ?? 0,
                Active = dto.attributes?.active ?? true,
                Location = dto.attributes?.location?.ToString(),
                Room = dto.attributes?.room?.ToString(),
                Instructor = dto.attributes?.instructor?.ToString(),
                Cost = dto.attributes?.cost,
                Notes = dto.attributes?.notes?.ToString(),
                DataSource = "Registrations"
            }).ToList();
            
            var pagedResponse = new PagedResponse<SignupTime>
            {
                Data = signupTimes,
                Meta = response.Meta,
                Links = response.Links
            };

            _logger.LogInformation("Successfully retrieved {Count} signup times for signup: {SignupId}", signupTimes.Count, signupId);
            return pagedResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing signup times for signup: {SignupId}", signupId);
            throw;
        }
    }

    public async Task<SignupTime> AddSignupTimeAsync(string signupId, SignupTimeCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(signupId))
            throw new ArgumentException("Signup ID cannot be null or empty", nameof(signupId));
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        _logger.LogDebug("Adding signup time to signup: {SignupId}", signupId);

        try
        {
            var jsonApiRequest = new JsonApiRequest<dynamic>
            {
                Data = new
                {
                    type = "SignupTime",
                    attributes = new
                    {
                        name = request.Name,
                        description = request.Description,
                        start_time = request.StartTime,
                        end_time = request.EndTime,
                        all_day = request.AllDay,
                        timezone = request.Timezone,
                        time_type = request.TimeType,
                        capacity = request.Capacity,
                        required = request.Required,
                        sort_order = request.SortOrder,
                        active = request.Active,
                        location = request.Location,
                        room = request.Room,
                        instructor = request.Instructor,
                        cost = request.Cost,
                        notes = request.Notes
                    },
                    relationships = new
                    {
                        signup = new
                        {
                            data = new { type = "Signup", id = signupId }
                        }
                    }
                }
            };

            var response = await _apiConnection.PostAsync<JsonApiRequest<dynamic>, JsonApiSingleResponse<dynamic>>(
                $"{BaseEndpoint}/signups/{signupId}/signup_times", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException($"Failed to add signup time to signup {signupId} - no data returned");
            }

            var signupTime = new SignupTime
            {
                Id = response.Data.id?.ToString() ?? string.Empty,
                SignupId = signupId,
                Name = request.Name,
                Description = request.Description,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                AllDay = request.AllDay,
                Timezone = request.Timezone,
                TimeType = request.TimeType,
                Capacity = request.Capacity,
                Required = request.Required,
                SortOrder = request.SortOrder,
                Active = request.Active,
                Location = request.Location,
                Room = request.Room,
                Instructor = request.Instructor,
                Cost = request.Cost,
                Notes = request.Notes,
                DataSource = "Registrations"
            };

            _logger.LogInformation("Successfully added signup time: {TimeId} to signup: {SignupId}", signupTime.Id, signupId);
            return signupTime;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding signup time to signup: {SignupId}", signupId);
            throw;
        }
    }

    public async Task<SignupTime> UpdateSignupTimeAsync(string id, SignupTimeUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Signup time ID cannot be null or empty", nameof(id));
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        _logger.LogDebug("Updating signup time: {TimeId}", id);

        try
        {
            var jsonApiRequest = new JsonApiRequest<dynamic>
            {
                Data = new
                {
                    type = "SignupTime",
                    id = id,
                    attributes = new
                    {
                        name = request.Name,
                        description = request.Description,
                        start_time = request.StartTime,
                        end_time = request.EndTime,
                        all_day = request.AllDay,
                        timezone = request.Timezone,
                        time_type = request.TimeType,
                        capacity = request.Capacity,
                        required = request.Required,
                        sort_order = request.SortOrder,
                        active = request.Active,
                        location = request.Location,
                        room = request.Room,
                        instructor = request.Instructor,
                        cost = request.Cost,
                        notes = request.Notes
                    }
                }
            };

            var response = await _apiConnection.PatchAsync<JsonApiRequest<dynamic>, JsonApiSingleResponse<dynamic>>(
                $"{BaseEndpoint}/signup_times/{id}", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException($"Failed to update signup time {id} - no data returned");
            }

            var signupTime = new SignupTime
            {
                Id = id,
                Name = request.Name ?? "Updated Time Slot",
                Description = request.Description,
                StartTime = request.StartTime ?? DateTime.MinValue,
                EndTime = request.EndTime,
                AllDay = request.AllDay ?? false,
                Timezone = request.Timezone,
                TimeType = request.TimeType,
                Capacity = request.Capacity,
                Required = request.Required ?? false,
                SortOrder = request.SortOrder ?? 0,
                Active = request.Active ?? true,
                Location = request.Location,
                Room = request.Room,
                Instructor = request.Instructor,
                Cost = request.Cost,
                Notes = request.Notes,
                DataSource = "Registrations"
            };

            _logger.LogInformation("Successfully updated signup time: {TimeId}", id);
            return signupTime;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating signup time: {TimeId}", id);
            throw;
        }
    }

    public async Task DeleteSignupTimeAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Signup time ID cannot be null or empty", nameof(id));

        _logger.LogDebug("Deleting signup time: {TimeId}", id);

        try
        {
            await _apiConnection.DeleteAsync($"{BaseEndpoint}/signup_times/{id}", cancellationToken);
            _logger.LogInformation("Successfully deleted signup time: {TimeId}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting signup time: {TimeId}", id);
            throw;
        }
    }

    public async Task<EmergencyContact?> GetEmergencyContactAsync(string attendeeId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(attendeeId))
            throw new ArgumentException("Attendee ID cannot be null or empty", nameof(attendeeId));

        _logger.LogDebug("Getting emergency contact for attendee: {AttendeeId}", attendeeId);

        try
        {
            var response = await _apiConnection.GetAsync<JsonApiSingleResponse<dynamic>>(
                $"{BaseEndpoint}/attendees/{attendeeId}/emergency_contact", cancellationToken);

            if (response?.Data == null)
            {
                _logger.LogWarning("Emergency contact not found for attendee: {AttendeeId}", attendeeId);
                return null;
            }

            var emergencyContact = new EmergencyContact
            {
                Id = response.Data.id?.ToString() ?? string.Empty,
                AttendeeId = attendeeId,
                FirstName = response.Data.attributes?.first_name?.ToString() ?? "Unknown",
                LastName = response.Data.attributes?.last_name?.ToString() ?? "Contact",
                Relationship = response.Data.attributes?.relationship?.ToString(),
                PrimaryPhone = response.Data.attributes?.primary_phone?.ToString(),
                SecondaryPhone = response.Data.attributes?.secondary_phone?.ToString(),
                Email = response.Data.attributes?.email?.ToString(),
                StreetAddress = response.Data.attributes?.street_address?.ToString(),
                City = response.Data.attributes?.city?.ToString(),
                State = response.Data.attributes?.state?.ToString(),
                PostalCode = response.Data.attributes?.postal_code?.ToString(),
                Country = response.Data.attributes?.country?.ToString(),
                IsPrimary = response.Data.attributes?.is_primary ?? false,
                Priority = response.Data.attributes?.priority ?? 1,
                Notes = response.Data.attributes?.notes?.ToString(),
                PreferredContactMethod = response.Data.attributes?.preferred_contact_method?.ToString(),
                BestTimeToContact = response.Data.attributes?.best_time_to_contact?.ToString(),
                CanAuthorizeMedicalTreatment = response.Data.attributes?.can_authorize_medical_treatment ?? false,
                DataSource = "Registrations"
            };

            _logger.LogInformation("Successfully retrieved emergency contact for attendee: {AttendeeId}", attendeeId);
            return emergencyContact;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            _logger.LogWarning("Emergency contact not found for attendee: {AttendeeId}", attendeeId);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving emergency contact for attendee: {AttendeeId}", attendeeId);
            throw;
        }
    }

    public async Task<EmergencyContact> SetEmergencyContactAsync(string attendeeId, EmergencyContactCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(attendeeId))
            throw new ArgumentException("Attendee ID cannot be null or empty", nameof(attendeeId));
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        _logger.LogDebug("Setting emergency contact for attendee: {AttendeeId}", attendeeId);

        try
        {
            var jsonApiRequest = new JsonApiRequest<dynamic>
            {
                Data = new
                {
                    type = "EmergencyContact",
                    attributes = new
                    {
                        first_name = request.FirstName,
                        last_name = request.LastName,
                        relationship = request.Relationship,
                        primary_phone = request.PrimaryPhone,
                        secondary_phone = request.SecondaryPhone,
                        email = request.Email,
                        street_address = request.StreetAddress,
                        city = request.City,
                        state = request.State,
                        postal_code = request.PostalCode,
                        country = request.Country,
                        is_primary = request.IsPrimary,
                        priority = request.Priority,
                        notes = request.Notes,
                        preferred_contact_method = request.PreferredContactMethod,
                        best_time_to_contact = request.BestTimeToContact,
                        can_authorize_medical_treatment = request.CanAuthorizeMedicalTreatment
                    },
                    relationships = new
                    {
                        attendee = new
                        {
                            data = new { type = "Attendee", id = attendeeId }
                        }
                    }
                }
            };

            var response = await _apiConnection.PostAsync<JsonApiRequest<dynamic>, JsonApiSingleResponse<dynamic>>(
                $"{BaseEndpoint}/attendees/{attendeeId}/emergency_contact", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException($"Failed to set emergency contact for attendee {attendeeId} - no data returned");
            }

            var emergencyContact = new EmergencyContact
            {
                Id = response.Data.id?.ToString() ?? string.Empty,
                AttendeeId = attendeeId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Relationship = request.Relationship,
                PrimaryPhone = request.PrimaryPhone,
                SecondaryPhone = request.SecondaryPhone,
                Email = request.Email,
                StreetAddress = request.StreetAddress,
                City = request.City,
                State = request.State,
                PostalCode = request.PostalCode,
                Country = request.Country,
                IsPrimary = request.IsPrimary,
                Priority = request.Priority,
                Notes = request.Notes,
                PreferredContactMethod = request.PreferredContactMethod,
                BestTimeToContact = request.BestTimeToContact,
                CanAuthorizeMedicalTreatment = request.CanAuthorizeMedicalTreatment,
                DataSource = "Registrations"
            };

            _logger.LogInformation("Successfully set emergency contact: {ContactId} for attendee: {AttendeeId}", emergencyContact.Id, attendeeId);
            return emergencyContact;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting emergency contact for attendee: {AttendeeId}", attendeeId);
            throw;
        }
    }

    public async Task<EmergencyContact> UpdateEmergencyContactAsync(string attendeeId, EmergencyContactUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(attendeeId))
            throw new ArgumentException("Attendee ID cannot be null or empty", nameof(attendeeId));
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        _logger.LogDebug("Updating emergency contact for attendee: {AttendeeId}", attendeeId);

        try
        {
            var jsonApiRequest = new JsonApiRequest<dynamic>
            {
                Data = new
                {
                    type = "EmergencyContact",
                    attributes = new
                    {
                        first_name = request.FirstName,
                        last_name = request.LastName,
                        relationship = request.Relationship,
                        primary_phone = request.PrimaryPhone,
                        secondary_phone = request.SecondaryPhone,
                        email = request.Email,
                        street_address = request.StreetAddress,
                        city = request.City,
                        state = request.State,
                        postal_code = request.PostalCode,
                        country = request.Country,
                        is_primary = request.IsPrimary,
                        priority = request.Priority,
                        notes = request.Notes,
                        preferred_contact_method = request.PreferredContactMethod,
                        best_time_to_contact = request.BestTimeToContact,
                        can_authorize_medical_treatment = request.CanAuthorizeMedicalTreatment
                    }
                }
            };

            var response = await _apiConnection.PatchAsync<JsonApiRequest<dynamic>, JsonApiSingleResponse<dynamic>>(
                $"{BaseEndpoint}/attendees/{attendeeId}/emergency_contact", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException($"Failed to update emergency contact for attendee {attendeeId} - no data returned");
            }

            var emergencyContact = new EmergencyContact
            {
                Id = response.Data.id?.ToString() ?? string.Empty,
                AttendeeId = attendeeId,
                FirstName = request.FirstName ?? "Updated",
                LastName = request.LastName ?? "Contact",
                Relationship = request.Relationship,
                PrimaryPhone = request.PrimaryPhone,
                SecondaryPhone = request.SecondaryPhone,
                Email = request.Email,
                StreetAddress = request.StreetAddress,
                City = request.City,
                State = request.State,
                PostalCode = request.PostalCode,
                Country = request.Country,
                IsPrimary = request.IsPrimary ?? false,
                Priority = request.Priority ?? 1,
                Notes = request.Notes,
                PreferredContactMethod = request.PreferredContactMethod,
                BestTimeToContact = request.BestTimeToContact,
                CanAuthorizeMedicalTreatment = request.CanAuthorizeMedicalTreatment ?? false,
                DataSource = "Registrations"
            };

            _logger.LogInformation("Successfully updated emergency contact for attendee: {AttendeeId}", attendeeId);
            return emergencyContact;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating emergency contact for attendee: {AttendeeId}", attendeeId);
            throw;
        }
    }

    public async Task<Category?> GetCategoryAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Category ID cannot be null or empty", nameof(id));

        _logger.LogDebug("Getting category with ID: {CategoryId}", id);

        try
        {
            var response = await _apiConnection.GetAsync<JsonApiSingleResponse<dynamic>>(
                $"{BaseEndpoint}/categories/{id}", cancellationToken);

            if (response?.Data == null)
            {
                _logger.LogWarning("Category not found: {CategoryId}", id);
                return null;
            }

            var category = new Category
            {
                Id = id,
                Name = response.Data.attributes?.name?.ToString() ?? "Unknown Category",
                Description = response.Data.attributes?.description?.ToString(),
                Color = response.Data.attributes?.color?.ToString(),
                SortOrder = response.Data.attributes?.sort_order ?? 0,
                Active = response.Data.attributes?.active ?? true,
                SignupCount = response.Data.attributes?.signup_count ?? 0,
                DataSource = "Registrations"
            };

            _logger.LogInformation("Successfully retrieved category: {CategoryId}", id);
            return category;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            _logger.LogWarning("Category not found: {CategoryId}", id);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving category: {CategoryId}", id);
            throw;
        }
    }

    public async Task<IPagedResponse<Category>> ListCategoriesAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Listing categories with parameters: {@Parameters}", parameters);

        try
        {
            var queryString = parameters?.ToQueryString() ?? string.Empty;
            var response = await _apiConnection.GetAsync<PagedResponse<dynamic>>(
                $"{BaseEndpoint}/categories{queryString}", cancellationToken);

            if (response?.Data == null)
            {
                _logger.LogWarning("No categories returned from API");
                return new PagedResponse<Category>
                {
                    Data = new List<Category>(),
                    Meta = new PagedResponseMeta { TotalCount = 0 },
                    Links = new PagedResponseLinks()
                };
            }

            var categories = response.Data.Select(dto => new Category
            {
                Id = dto.id?.ToString() ?? string.Empty,
                Name = dto.attributes?.name?.ToString() ?? "Unknown Category",
                Description = dto.attributes?.description?.ToString(),
                Color = dto.attributes?.color?.ToString(),
                SortOrder = dto.attributes?.sort_order ?? 0,
                Active = dto.attributes?.active ?? true,
                SignupCount = dto.attributes?.signup_count ?? 0,
                DataSource = "Registrations"
            }).ToList();
            
            var pagedResponse = new PagedResponse<Category>
            {
                Data = categories,
                Meta = response.Meta,
                Links = response.Links
            };

            _logger.LogInformation("Successfully retrieved {Count} categories", categories.Count);
            return pagedResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing categories");
            throw;
        }
    }

    public async Task<Category> CreateCategoryAsync(CategoryCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        _logger.LogDebug("Creating category with name: {Name}", request.Name);

        try
        {
            var jsonApiRequest = new JsonApiRequest<dynamic>
            {
                Data = new
                {
                    type = "Category",
                    attributes = new
                    {
                        name = request.Name,
                        description = request.Description,
                        color = request.Color,
                        sort_order = request.SortOrder,
                        active = request.Active
                    }
                }
            };

            var response = await _apiConnection.PostAsync<JsonApiRequest<dynamic>, JsonApiSingleResponse<dynamic>>(
                $"{BaseEndpoint}/categories", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException("Failed to create category - no data returned");
            }

            var category = new Category
            {
                Id = response.Data.id?.ToString() ?? string.Empty,
                Name = request.Name,
                Description = request.Description,
                Color = request.Color,
                SortOrder = request.SortOrder,
                Active = request.Active,
                DataSource = "Registrations"
            };

            _logger.LogInformation("Successfully created category: {CategoryId}", category.Id);
            return category;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            throw;
        }
    }

    public async Task<Category> UpdateCategoryAsync(string id, CategoryUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Category ID cannot be null or empty", nameof(id));
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        _logger.LogDebug("Updating category: {CategoryId}", id);

        try
        {
            var jsonApiRequest = new JsonApiRequest<dynamic>
            {
                Data = new
                {
                    type = "Category",
                    id = id,
                    attributes = new
                    {
                        name = request.Name,
                        description = request.Description,
                        color = request.Color,
                        sort_order = request.SortOrder,
                        active = request.Active
                    }
                }
            };

            var response = await _apiConnection.PatchAsync<JsonApiRequest<dynamic>, JsonApiSingleResponse<dynamic>>(
                $"{BaseEndpoint}/categories/{id}", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException($"Failed to update category {id} - no data returned");
            }

            var category = new Category
            {
                Id = id,
                Name = request.Name ?? "Updated Category",
                Description = request.Description,
                Color = request.Color,
                SortOrder = request.SortOrder ?? 0,
                Active = request.Active ?? true,
                DataSource = "Registrations"
            };

            _logger.LogInformation("Successfully updated category: {CategoryId}", id);
            return category;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category: {CategoryId}", id);
            throw;
        }
    }

    public async Task<Campus?> GetCampusAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Campus ID cannot be null or empty", nameof(id));

        _logger.LogDebug("Getting campus with ID: {CampusId}", id);

        try
        {
            var response = await _apiConnection.GetAsync<JsonApiSingleResponse<dynamic>>(
                $"{BaseEndpoint}/campuses/{id}", cancellationToken);

            if (response?.Data == null)
            {
                _logger.LogWarning("Campus not found: {CampusId}", id);
                return null;
            }

            var campus = new Campus
            {
                Id = id,
                Name = response.Data.attributes?.name?.ToString() ?? "Unknown Campus",
                Description = response.Data.attributes?.description?.ToString(),
                Timezone = response.Data.attributes?.timezone?.ToString(),
                Address = response.Data.attributes?.address?.ToString(),
                City = response.Data.attributes?.city?.ToString(),
                State = response.Data.attributes?.state?.ToString(),
                PostalCode = response.Data.attributes?.postal_code?.ToString(),
                Country = response.Data.attributes?.country?.ToString(),
                PhoneNumber = response.Data.attributes?.phone_number?.ToString(),
                WebsiteUrl = response.Data.attributes?.website_url?.ToString(),
                Active = response.Data.attributes?.active ?? true,
                SortOrder = response.Data.attributes?.sort_order ?? 0,
                SignupCount = response.Data.attributes?.signup_count ?? 0,
                DataSource = "Registrations"
            };

            _logger.LogInformation("Successfully retrieved campus: {CampusId}", id);
            return campus;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            _logger.LogWarning("Campus not found: {CampusId}", id);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving campus: {CampusId}", id);
            throw;
        }
    }

    public async Task<IPagedResponse<Campus>> ListCampusesAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Listing campuses with parameters: {@Parameters}", parameters);

        try
        {
            var queryString = parameters?.ToQueryString() ?? string.Empty;
            var response = await _apiConnection.GetAsync<PagedResponse<dynamic>>(
                $"{BaseEndpoint}/campuses{queryString}", cancellationToken);

            if (response?.Data == null)
            {
                _logger.LogWarning("No campuses returned from API");
                return new PagedResponse<Campus>
                {
                    Data = new List<Campus>(),
                    Meta = new PagedResponseMeta { TotalCount = 0 },
                    Links = new PagedResponseLinks()
                };
            }

            var campuses = response.Data.Select(dto => new Campus
            {
                Id = dto.id?.ToString() ?? string.Empty,
                Name = dto.attributes?.name?.ToString() ?? "Unknown Campus",
                Description = dto.attributes?.description?.ToString(),
                Timezone = dto.attributes?.timezone?.ToString(),
                Address = dto.attributes?.address?.ToString(),
                City = dto.attributes?.city?.ToString(),
                State = dto.attributes?.state?.ToString(),
                PostalCode = dto.attributes?.postal_code?.ToString(),
                Country = dto.attributes?.country?.ToString(),
                PhoneNumber = dto.attributes?.phone_number?.ToString(),
                WebsiteUrl = dto.attributes?.website_url?.ToString(),
                Active = dto.attributes?.active ?? true,
                SortOrder = dto.attributes?.sort_order ?? 0,
                SignupCount = dto.attributes?.signup_count ?? 0,
                DataSource = "Registrations"
            }).ToList();
            
            var pagedResponse = new PagedResponse<Campus>
            {
                Data = campuses,
                Meta = response.Meta,
                Links = response.Links
            };

            _logger.LogInformation("Successfully retrieved {Count} campuses", campuses.Count);
            return pagedResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing campuses");
            throw;
        }
    }

    public async Task<Person?> GetPersonAsync(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Person operations not yet implemented");
    }

    public async Task<IPagedResponse<Attendee>> GetAttendeesForPersonAsync(string personId, QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Person-specific operations not yet implemented");
    }

    public async Task<IPagedResponse<Signup>> GetSignupsForPersonAsync(string personId, QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Person-specific operations not yet implemented");
    }

    #endregion
}