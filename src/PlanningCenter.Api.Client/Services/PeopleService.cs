using Microsoft.Extensions.Logging;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.People;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Models.Exceptions;

namespace PlanningCenter.Api.Client.Services;

/// <summary>
/// Service implementation for the Planning Center People module.
/// Provides comprehensive people management with built-in pagination support.
/// </summary>
public class PeopleService : IPeopleService
{
    private readonly IApiConnection _apiConnection;
    private readonly ILogger<PeopleService> _logger;
    private const string BaseEndpoint = "/people/v2";

    public PeopleService(
        IApiConnection apiConnection,
        ILogger<PeopleService> logger)
    {
        _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets the current authenticated user's person record.
    /// This is useful for testing authentication and getting the current user's information.
    /// </summary>
    public async Task<Person> GetMeAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting current user's person record");

        var response = await _apiConnection.GetAsync<JsonApiSingleResponse<PersonDto>>(
            $"{BaseEndpoint}/me", cancellationToken);

        if (response?.Data == null)
        {
            throw new PlanningCenterApiGeneralException("Failed to get current user - no data returned");
        }

        var person = MapPersonDtoToPerson(response.Data);
        _logger.LogInformation("Successfully retrieved current user: {PersonId} - {FullName}", 
            person.Id, person.FullName);
        
        return person;
    }

    /// <summary>
    /// Gets a single person by ID.
    /// </summary>
    public async Task<Person?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Person ID cannot be null or empty", nameof(id));

        _logger.LogDebug("Getting person with ID: {PersonId}", id);

        try
        {
            var response = await _apiConnection.GetAsync<JsonApiSingleResponse<PersonDto>>(
                $"{BaseEndpoint}/people/{id}", cancellationToken);

            if (response?.Data == null)
            {
                _logger.LogWarning("Person not found: {PersonId}", id);
                return null;
            }

            var person = MapPersonDtoToPerson(response.Data);
            _logger.LogInformation("Successfully retrieved person: {PersonId}", id);
            
            return person;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            _logger.LogWarning("Person not found: {PersonId}", id);
            return null;
        }
    }

    /// <summary>
    /// Lists people with optional filtering, sorting, and pagination.
    /// Returns a paginated response with built-in navigation helpers.
    /// </summary>
    public async Task<IPagedResponse<Person>> ListAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Listing people with parameters: {@Parameters}", parameters);

        var response = await _apiConnection.GetPagedAsync<PersonDto>(
            $"{BaseEndpoint}/people", parameters, cancellationToken);

        // Map DTOs to domain models
        var people = response.Data.Select(MapPersonDtoToPerson).ToList();

        // Create a new paged response with mapped data
        var mappedResponse = new PagedResponse<Person>
        {
            Data = people,
            Meta = response.Meta,
            Links = response.Links
        };

        // Copy navigation properties if the response is a concrete PagedResponse
        if (response is PagedResponse<PersonDto> concreteResponse)
        {
            mappedResponse.ApiConnection = concreteResponse.ApiConnection;
            mappedResponse.OriginalParameters = concreteResponse.OriginalParameters;
            mappedResponse.OriginalEndpoint = concreteResponse.OriginalEndpoint;
        }

        _logger.LogInformation("Successfully retrieved page {Page} with {Count} people", 
            response.Meta.CurrentPage, people.Count);

        return mappedResponse;
    }

    /// <summary>
    /// Creates a new person.
    /// </summary>
    public async Task<Person> CreateAsync(PersonCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.FirstName))
            throw new ArgumentException("FirstName is required", nameof(request));

        if (string.IsNullOrWhiteSpace(request.LastName))
            throw new ArgumentException("LastName is required", nameof(request));

        _logger.LogDebug("Creating person: {FirstName} {LastName}", request.FirstName, request.LastName);

        // Convert request to JSON:API format
        var jsonApiRequest = new JsonApiRequest<PersonCreateDto>
        {
            Data = new PersonCreateDto
            {
                Type = "Person",
                Attributes = MapPersonCreateRequestToDto(request)
            }
        };

        var response = await _apiConnection.PostAsync<JsonApiSingleResponse<PersonDto>>(
            $"{BaseEndpoint}/people", jsonApiRequest, cancellationToken);

        if (response?.Data == null)
        {
            throw new PlanningCenterApiGeneralException("Failed to create person - no data returned");
        }

        var person = MapPersonDtoToPerson(response.Data);
        _logger.LogInformation("Successfully created person: {PersonId}", person.Id);

        return person;
    }

    /// <summary>
    /// Updates an existing person.
    /// </summary>
    public async Task<Person> UpdateAsync(string id, PersonUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Person ID cannot be null or empty", nameof(id));

        if (request == null)
            throw new ArgumentNullException(nameof(request));

        _logger.LogDebug("Updating person: {PersonId}", id);

        // Convert request to JSON:API format
        var jsonApiRequest = new JsonApiRequest<PersonUpdateDto>
        {
            Data = new PersonUpdateDto
            {
                Type = "Person",
                Id = id,
                Attributes = MapPersonUpdateRequestToDto(request)
            }
        };

        var response = await _apiConnection.PatchAsync<JsonApiSingleResponse<PersonDto>>(
            $"{BaseEndpoint}/people/{id}", jsonApiRequest, cancellationToken);

        if (response?.Data == null)
        {
            throw new PlanningCenterApiGeneralException("Failed to update person - no data returned");
        }

        var person = MapPersonDtoToPerson(response.Data);
        _logger.LogInformation("Successfully updated person: {PersonId}", id);

        return person;
    }

    /// <summary>
    /// Deletes a person.
    /// </summary>
    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Person ID cannot be null or empty", nameof(id));

        _logger.LogDebug("Deleting person: {PersonId}", id);

        await _apiConnection.DeleteAsync($"{BaseEndpoint}/people/{id}", cancellationToken);

        _logger.LogInformation("Successfully deleted person: {PersonId}", id);
    }

    /// <summary>
    /// Gets all people matching the specified criteria.
    /// This method automatically handles pagination behind the scenes.
    /// </summary>
    public async Task<IReadOnlyList<Person>> GetAllAsync(
        QueryParameters? parameters = null,
        PaginationOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all people with options: {@Options}", options);

        var firstPage = await ListAsync(parameters, cancellationToken);
        
        if (!firstPage.HasNextPage)
        {
            return firstPage.Data;
        }

        return await firstPage.GetAllRemainingAsync(cancellationToken);
    }

    /// <summary>
    /// Streams people matching the specified criteria for memory-efficient processing.
    /// This method automatically handles pagination while using minimal memory.
    /// </summary>
    public async IAsyncEnumerable<Person> StreamAsync(
        QueryParameters? parameters = null,
        PaginationOptions? options = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Streaming people with options: {@Options}", options);

        var firstPage = await ListAsync(parameters, cancellationToken);
        
        await foreach (var person in firstPage.GetAllRemainingAsyncEnumerable(cancellationToken))
        {
            yield return person;
        }
    }

    // Address management methods would go here...
    // For now, I'll implement the interface requirements with NotImplementedException
    // These will be implemented in a future iteration

    public Task<Address> AddAddressAsync(string personId, AddressCreateRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Address management will be implemented in Phase 1B Part 3");
    }

    public Task<Address> UpdateAddressAsync(string personId, string addressId, AddressUpdateRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Address management will be implemented in Phase 1B Part 3");
    }

    public Task DeleteAddressAsync(string personId, string addressId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Address management will be implemented in Phase 1B Part 3");
    }

    public Task<Email> AddEmailAsync(string personId, EmailCreateRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Email management will be implemented in Phase 1B Part 3");
    }

    public Task<Email> UpdateEmailAsync(string personId, string emailId, EmailUpdateRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Email management will be implemented in Phase 1B Part 3");
    }

    public Task DeleteEmailAsync(string personId, string emailId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Email management will be implemented in Phase 1B Part 3");
    }

    public Task<PhoneNumber> AddPhoneNumberAsync(string personId, PhoneNumberCreateRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Phone number management will be implemented in Phase 1B Part 3");
    }

    public Task<PhoneNumber> UpdatePhoneNumberAsync(string personId, string phoneId, PhoneNumberUpdateRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Phone number management will be implemented in Phase 1B Part 3");
    }

    public Task DeletePhoneNumberAsync(string personId, string phoneId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Phone number management will be implemented in Phase 1B Part 3");
    }

    #region Private Mapping Methods

    private static Person MapPersonDtoToPerson(PersonDto dto)
    {
        return new Person
        {
            Id = dto.Id,
            FirstName = dto.Attributes.FirstName ?? string.Empty,
            LastName = dto.Attributes.LastName ?? string.Empty,
            MiddleName = dto.Attributes.MiddleName,
            Nickname = dto.Attributes.Nickname,
            Gender = dto.Attributes.Gender,
            Birthdate = dto.Attributes.Birthdate,
            Anniversary = dto.Attributes.Anniversary,
            Status = dto.Attributes.Status ?? "active",
            MembershipStatus = dto.Attributes.MembershipStatus,
            MaritalStatus = dto.Attributes.MaritalStatus,
            School = dto.Attributes.School,
            Grade = dto.Attributes.Grade,
            GraduationYear = dto.Attributes.GraduationYear,
            MedicalNotes = dto.Attributes.MedicalNotes,
            EmergencyContactName = dto.Attributes.EmergencyContactName,
            EmergencyContactPhone = dto.Attributes.EmergencyContactPhone,
            AvatarUrl = dto.Attributes.AvatarUrl,
            CreatedAt = dto.Attributes.CreatedAt,
            UpdatedAt = dto.Attributes.UpdatedAt,
            DataSource = "People"
        };
    }

    private static PersonCreateAttributesDto MapPersonCreateRequestToDto(PersonCreateRequest request)
    {
        return new PersonCreateAttributesDto
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            MiddleName = request.MiddleName,
            Nickname = request.Nickname,
            Gender = request.Gender,
            Birthdate = request.Birthdate,
            Anniversary = request.Anniversary,
            Status = request.Status,
            MembershipStatus = request.MembershipStatus,
            MaritalStatus = request.MaritalStatus,
            School = request.School,
            Grade = request.Grade,
            GraduationYear = request.GraduationYear,
            MedicalNotes = request.MedicalNotes,
            EmergencyContactName = request.EmergencyContactName,
            EmergencyContactPhone = request.EmergencyContactPhone
        };
    }

    private static PersonUpdateAttributesDto MapPersonUpdateRequestToDto(PersonUpdateRequest request)
    {
        return new PersonUpdateAttributesDto
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            MiddleName = request.MiddleName,
            Nickname = request.Nickname,
            Gender = request.Gender,
            Birthdate = request.Birthdate,
            Anniversary = request.Anniversary,
            Status = request.Status,
            MembershipStatus = request.MembershipStatus,
            MaritalStatus = request.MaritalStatus,
            School = request.School,
            Grade = request.Grade,
            GraduationYear = request.GraduationYear,
            MedicalNotes = request.MedicalNotes,
            EmergencyContactName = request.EmergencyContactName,
            EmergencyContactPhone = request.EmergencyContactPhone
        };
    }

    #endregion
}

#region JSON:API Request/Response Models

/// <summary>
/// JSON:API request wrapper for single resources.
/// </summary>
internal class JsonApiRequest<T>
{
    public T Data { get; set; } = default!;
}

/// <summary>
/// JSON:API response wrapper for single resources.
/// </summary>
internal class JsonApiSingleResponse<T>
{
    public T? Data { get; set; }
    public PagedResponseMeta? Meta { get; set; }
    public PagedResponseLinks? Links { get; set; }
}

/// <summary>
/// Person creation DTO for JSON:API requests.
/// </summary>
internal class PersonCreateDto
{
    public string Type { get; set; } = "Person";
    public PersonCreateAttributesDto Attributes { get; set; } = new();
}

/// <summary>
/// Person update DTO for JSON:API requests.
/// </summary>
internal class PersonUpdateDto
{
    public string Type { get; set; } = "Person";
    public string Id { get; set; } = string.Empty;
    public PersonUpdateAttributesDto Attributes { get; set; } = new();
}

/// <summary>
/// Person creation attributes DTO.
/// </summary>
internal class PersonCreateAttributesDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string? Nickname { get; set; }
    public string? Gender { get; set; }
    public DateTime? Birthdate { get; set; }
    public DateTime? Anniversary { get; set; }
    public string Status { get; set; } = "active";
    public string? MembershipStatus { get; set; }
    public string? MaritalStatus { get; set; }
    public string? School { get; set; }
    public string? Grade { get; set; }
    public int? GraduationYear { get; set; }
    public string? MedicalNotes { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
}

/// <summary>
/// Person update attributes DTO.
/// </summary>
internal class PersonUpdateAttributesDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
    public string? Nickname { get; set; }
    public string? Gender { get; set; }
    public DateTime? Birthdate { get; set; }
    public DateTime? Anniversary { get; set; }
    public string? Status { get; set; }
    public string? MembershipStatus { get; set; }
    public string? MaritalStatus { get; set; }
    public string? School { get; set; }
    public string? Grade { get; set; }
    public int? GraduationYear { get; set; }
    public string? MedicalNotes { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
}

#endregion