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

    #region Address Management

    /// <summary>
    /// Adds a new address to a person.
    /// </summary>
    public async Task<Address> AddAddressAsync(string personId, AddressCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(personId))
            throw new ArgumentException("Person ID cannot be null or empty", nameof(personId));

        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.Street))
            throw new ArgumentException("Street is required", nameof(request));

        if (string.IsNullOrWhiteSpace(request.City))
            throw new ArgumentException("City is required", nameof(request));

        if (string.IsNullOrWhiteSpace(request.State))
            throw new ArgumentException("State is required", nameof(request));

        if (string.IsNullOrWhiteSpace(request.Zip))
            throw new ArgumentException("Zip is required", nameof(request));

        _logger.LogDebug("Adding address to person: {PersonId}", personId);

        // Convert request to JSON:API format
        var jsonApiRequest = new JsonApiRequest<AddressCreateDto>
        {
            Data = new AddressCreateDto
            {
                Type = "Address",
                Attributes = MapAddressCreateRequestToDto(request)
            }
        };

        var response = await _apiConnection.PostAsync<JsonApiSingleResponse<AddressDto>>(
            $"{BaseEndpoint}/people/{personId}/addresses", jsonApiRequest, cancellationToken);

        if (response?.Data == null)
        {
            throw new PlanningCenterApiGeneralException("Failed to create address - no data returned");
        }

        var address = MapAddressDtoToAddress(response.Data);
        _logger.LogInformation("Successfully added address: {AddressId} to person: {PersonId}", address.Id, personId);

        return address;
    }

    /// <summary>
    /// Updates an existing address for a person.
    /// </summary>
    public async Task<Address> UpdateAddressAsync(string personId, string addressId, AddressUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(personId))
            throw new ArgumentException("Person ID cannot be null or empty", nameof(personId));

        if (string.IsNullOrWhiteSpace(addressId))
            throw new ArgumentException("Address ID cannot be null or empty", nameof(addressId));

        if (request == null)
            throw new ArgumentNullException(nameof(request));

        _logger.LogDebug("Updating address: {AddressId} for person: {PersonId}", addressId, personId);

        // Convert request to JSON:API format
        var jsonApiRequest = new JsonApiRequest<AddressUpdateDto>
        {
            Data = new AddressUpdateDto
            {
                Type = "Address",
                Id = addressId,
                Attributes = MapAddressUpdateRequestToDto(request)
            }
        };

        var response = await _apiConnection.PatchAsync<JsonApiSingleResponse<AddressDto>>(
            $"{BaseEndpoint}/people/{personId}/addresses/{addressId}", jsonApiRequest, cancellationToken);

        if (response?.Data == null)
        {
            throw new PlanningCenterApiGeneralException("Failed to update address - no data returned");
        }

        var address = MapAddressDtoToAddress(response.Data);
        _logger.LogInformation("Successfully updated address: {AddressId} for person: {PersonId}", addressId, personId);

        return address;
    }

    /// <summary>
    /// Deletes an address from a person.
    /// </summary>
    public async Task DeleteAddressAsync(string personId, string addressId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(personId))
            throw new ArgumentException("Person ID cannot be null or empty", nameof(personId));

        if (string.IsNullOrWhiteSpace(addressId))
            throw new ArgumentException("Address ID cannot be null or empty", nameof(addressId));

        _logger.LogDebug("Deleting address: {AddressId} from person: {PersonId}", addressId, personId);

        await _apiConnection.DeleteAsync($"{BaseEndpoint}/people/{personId}/addresses/{addressId}", cancellationToken);

        _logger.LogInformation("Successfully deleted address: {AddressId} from person: {PersonId}", addressId, personId);
    }

    #endregion

    #region Email Management

    /// <summary>
    /// Adds a new email address to a person.
    /// </summary>
    public async Task<Email> AddEmailAsync(string personId, EmailCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(personId))
            throw new ArgumentException("Person ID cannot be null or empty", nameof(personId));

        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.Address))
            throw new ArgumentException("Email address is required", nameof(request));

        _logger.LogDebug("Adding email to person: {PersonId}", personId);

        // Convert request to JSON:API format
        var jsonApiRequest = new JsonApiRequest<EmailCreateDto>
        {
            Data = new EmailCreateDto
            {
                Type = "Email",
                Attributes = MapEmailCreateRequestToDto(request)
            }
        };

        var response = await _apiConnection.PostAsync<JsonApiSingleResponse<EmailDto>>(
            $"{BaseEndpoint}/people/{personId}/emails", jsonApiRequest, cancellationToken);

        if (response?.Data == null)
        {
            throw new PlanningCenterApiGeneralException("Failed to create email - no data returned");
        }

        var email = MapEmailDtoToEmail(response.Data);
        _logger.LogInformation("Successfully added email: {EmailId} to person: {PersonId}", email.Id, personId);

        return email;
    }

    /// <summary>
    /// Updates an existing email address for a person.
    /// </summary>
    public async Task<Email> UpdateEmailAsync(string personId, string emailId, EmailUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(personId))
            throw new ArgumentException("Person ID cannot be null or empty", nameof(personId));

        if (string.IsNullOrWhiteSpace(emailId))
            throw new ArgumentException("Email ID cannot be null or empty", nameof(emailId));

        if (request == null)
            throw new ArgumentNullException(nameof(request));

        _logger.LogDebug("Updating email: {EmailId} for person: {PersonId}", emailId, personId);

        // Convert request to JSON:API format
        var jsonApiRequest = new JsonApiRequest<EmailUpdateDto>
        {
            Data = new EmailUpdateDto
            {
                Type = "Email",
                Id = emailId,
                Attributes = MapEmailUpdateRequestToDto(request)
            }
        };

        var response = await _apiConnection.PatchAsync<JsonApiSingleResponse<EmailDto>>(
            $"{BaseEndpoint}/people/{personId}/emails/{emailId}", jsonApiRequest, cancellationToken);

        if (response?.Data == null)
        {
            throw new PlanningCenterApiGeneralException("Failed to update email - no data returned");
        }

        var email = MapEmailDtoToEmail(response.Data);
        _logger.LogInformation("Successfully updated email: {EmailId} for person: {PersonId}", emailId, personId);

        return email;
    }

    /// <summary>
    /// Deletes an email address from a person.
    /// </summary>
    public async Task DeleteEmailAsync(string personId, string emailId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(personId))
            throw new ArgumentException("Person ID cannot be null or empty", nameof(personId));

        if (string.IsNullOrWhiteSpace(emailId))
            throw new ArgumentException("Email ID cannot be null or empty", nameof(emailId));

        _logger.LogDebug("Deleting email: {EmailId} from person: {PersonId}", emailId, personId);

        await _apiConnection.DeleteAsync($"{BaseEndpoint}/people/{personId}/emails/{emailId}", cancellationToken);

        _logger.LogInformation("Successfully deleted email: {EmailId} from person: {PersonId}", emailId, personId);
    }

    #endregion

    #region Phone Number Management

    /// <summary>
    /// Adds a new phone number to a person.
    /// </summary>
    public async Task<PhoneNumber> AddPhoneNumberAsync(string personId, PhoneNumberCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(personId))
            throw new ArgumentException("Person ID cannot be null or empty", nameof(personId));

        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.Number))
            throw new ArgumentException("Phone number is required", nameof(request));

        _logger.LogDebug("Adding phone number to person: {PersonId}", personId);

        // Convert request to JSON:API format
        var jsonApiRequest = new JsonApiRequest<PhoneNumberCreateDto>
        {
            Data = new PhoneNumberCreateDto
            {
                Type = "PhoneNumber",
                Attributes = MapPhoneNumberCreateRequestToDto(request)
            }
        };

        var response = await _apiConnection.PostAsync<JsonApiSingleResponse<PhoneNumberDto>>(
            $"{BaseEndpoint}/people/{personId}/phone_numbers", jsonApiRequest, cancellationToken);

        if (response?.Data == null)
        {
            throw new PlanningCenterApiGeneralException("Failed to create phone number - no data returned");
        }

        var phoneNumber = MapPhoneNumberDtoToPhoneNumber(response.Data);
        _logger.LogInformation("Successfully added phone number: {PhoneNumberId} to person: {PersonId}", phoneNumber.Id, personId);

        return phoneNumber;
    }

    /// <summary>
    /// Updates an existing phone number for a person.
    /// </summary>
    public async Task<PhoneNumber> UpdatePhoneNumberAsync(string personId, string phoneId, PhoneNumberUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(personId))
            throw new ArgumentException("Person ID cannot be null or empty", nameof(personId));

        if (string.IsNullOrWhiteSpace(phoneId))
            throw new ArgumentException("Phone number ID cannot be null or empty", nameof(phoneId));

        if (request == null)
            throw new ArgumentNullException(nameof(request));

        _logger.LogDebug("Updating phone number: {PhoneNumberId} for person: {PersonId}", phoneId, personId);

        // Convert request to JSON:API format
        var jsonApiRequest = new JsonApiRequest<PhoneNumberUpdateDto>
        {
            Data = new PhoneNumberUpdateDto
            {
                Type = "PhoneNumber",
                Id = phoneId,
                Attributes = MapPhoneNumberUpdateRequestToDto(request)
            }
        };

        var response = await _apiConnection.PatchAsync<JsonApiSingleResponse<PhoneNumberDto>>(
            $"{BaseEndpoint}/people/{personId}/phone_numbers/{phoneId}", jsonApiRequest, cancellationToken);

        if (response?.Data == null)
        {
            throw new PlanningCenterApiGeneralException("Failed to update phone number - no data returned");
        }

        var phoneNumber = MapPhoneNumberDtoToPhoneNumber(response.Data);
        _logger.LogInformation("Successfully updated phone number: {PhoneNumberId} for person: {PersonId}", phoneId, personId);

        return phoneNumber;
    }

    /// <summary>
    /// Deletes a phone number from a person.
    /// </summary>
    public async Task DeletePhoneNumberAsync(string personId, string phoneId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(personId))
            throw new ArgumentException("Person ID cannot be null or empty", nameof(personId));

        if (string.IsNullOrWhiteSpace(phoneId))
            throw new ArgumentException("Phone number ID cannot be null or empty", nameof(phoneId));

        _logger.LogDebug("Deleting phone number: {PhoneNumberId} from person: {PersonId}", phoneId, personId);

        await _apiConnection.DeleteAsync($"{BaseEndpoint}/people/{personId}/phone_numbers/{phoneId}", cancellationToken);

        _logger.LogInformation("Successfully deleted phone number: {PhoneNumberId} from person: {PersonId}", phoneId, personId);
    }

    #endregion

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

    #region Address Mapping Methods

    private static Address MapAddressDtoToAddress(AddressDto dto)
    {
        return new Address
        {
            Id = dto.Id,
            Street = dto.Attributes.Street ?? string.Empty,
            Street2 = dto.Attributes.Street2,
            City = dto.Attributes.City ?? string.Empty,
            State = dto.Attributes.State ?? string.Empty,
            Zip = dto.Attributes.Zip ?? string.Empty,
            Country = dto.Attributes.Country ?? "US",
            Location = dto.Attributes.Location ?? "Home",
            IsPrimary = dto.Attributes.Primary,
            CreatedAt = dto.Attributes.CreatedAt,
            UpdatedAt = dto.Attributes.UpdatedAt,
            DataSource = "People"
        };
    }

    private static AddressCreateAttributesDto MapAddressCreateRequestToDto(AddressCreateRequest request)
    {
        return new AddressCreateAttributesDto
        {
            Street = request.Street,
            Street2 = request.Street2,
            City = request.City,
            State = request.State,
            Zip = request.Zip,
            Country = request.Country,
            Location = request.Location,
            Primary = request.IsPrimary
        };
    }

    private static AddressUpdateAttributesDto MapAddressUpdateRequestToDto(AddressUpdateRequest request)
    {
        return new AddressUpdateAttributesDto
        {
            Street = request.Street,
            Street2 = request.Street2,
            City = request.City,
            State = request.State,
            Zip = request.Zip,
            Country = request.Country,
            Location = request.Location,
            Primary = request.IsPrimary
        };
    }

    #endregion

    #region Email Mapping Methods

    private static Email MapEmailDtoToEmail(EmailDto dto)
    {
        return new Email
        {
            Id = dto.Id,
            Address = dto.Attributes.Address ?? string.Empty,
            Location = dto.Attributes.Location ?? "Home",
            IsPrimary = dto.Attributes.Primary,
            IsBlocked = dto.Attributes.Blocked,
            CreatedAt = dto.Attributes.CreatedAt,
            UpdatedAt = dto.Attributes.UpdatedAt,
            DataSource = "People"
        };
    }

    private static EmailCreateAttributesDto MapEmailCreateRequestToDto(EmailCreateRequest request)
    {
        return new EmailCreateAttributesDto
        {
            Address = request.Address,
            Location = request.Location,
            Primary = request.IsPrimary
        };
    }

    private static EmailUpdateAttributesDto MapEmailUpdateRequestToDto(EmailUpdateRequest request)
    {
        return new EmailUpdateAttributesDto
        {
            Address = request.Address,
            Location = request.Location,
            Primary = request.IsPrimary
        };
    }

    #endregion

    #region Phone Number Mapping Methods

    private static PhoneNumber MapPhoneNumberDtoToPhoneNumber(PhoneNumberDto dto)
    {
        return new PhoneNumber
        {
            Id = dto.Id,
            Number = dto.Attributes.Number ?? string.Empty,
            Location = dto.Attributes.Location ?? "Mobile",
            IsPrimary = dto.Attributes.Primary,
            CanReceiveSms = dto.Attributes.Carrier, // Note: API uses 'carrier' field for SMS capability
            CreatedAt = dto.Attributes.CreatedAt,
            UpdatedAt = dto.Attributes.UpdatedAt,
            DataSource = "People"
        };
    }

    private static PhoneNumberCreateAttributesDto MapPhoneNumberCreateRequestToDto(PhoneNumberCreateRequest request)
    {
        return new PhoneNumberCreateAttributesDto
        {
            Number = request.Number,
            Location = request.Location,
            Primary = request.IsPrimary,
            Carrier = request.CanReceiveSms
        };
    }

    private static PhoneNumberUpdateAttributesDto MapPhoneNumberUpdateRequestToDto(PhoneNumberUpdateRequest request)
    {
        return new PhoneNumberUpdateAttributesDto
        {
            Number = request.Number,
            Location = request.Location,
            Primary = request.IsPrimary,
            Carrier = request.CanReceiveSms
        };
    }

    #endregion

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

/// <summary>
/// Address creation DTO for JSON:API requests.
/// </summary>
internal class AddressCreateDto
{
    public string Type { get; set; } = "Address";
    public AddressCreateAttributesDto Attributes { get; set; } = new();
}

/// <summary>
/// Address update DTO for JSON:API requests.
/// </summary>
internal class AddressUpdateDto
{
    public string Type { get; set; } = "Address";
    public string Id { get; set; } = string.Empty;
    public AddressUpdateAttributesDto Attributes { get; set; } = new();
}

/// <summary>
/// Address creation attributes DTO.
/// </summary>
internal class AddressCreateAttributesDto
{
    public string Street { get; set; } = string.Empty;
    public string? Street2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Zip { get; set; } = string.Empty;
    public string Country { get; set; } = "US";
    public string Location { get; set; } = "Home";
    public bool Primary { get; set; }
}

/// <summary>
/// Address update attributes DTO.
/// </summary>
internal class AddressUpdateAttributesDto
{
    public string? Street { get; set; }
    public string? Street2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Zip { get; set; }
    public string? Country { get; set; }
    public string? Location { get; set; }
    public bool? Primary { get; set; }
}

/// <summary>
/// Email creation DTO for JSON:API requests.
/// </summary>
internal class EmailCreateDto
{
    public string Type { get; set; } = "Email";
    public EmailCreateAttributesDto Attributes { get; set; } = new();
}

/// <summary>
/// Email update DTO for JSON:API requests.
/// </summary>
internal class EmailUpdateDto
{
    public string Type { get; set; } = "Email";
    public string Id { get; set; } = string.Empty;
    public EmailUpdateAttributesDto Attributes { get; set; } = new();
}

/// <summary>
/// Email creation attributes DTO.
/// </summary>
internal class EmailCreateAttributesDto
{
    public string Address { get; set; } = string.Empty;
    public string Location { get; set; } = "Home";
    public bool Primary { get; set; }
}

/// <summary>
/// Email update attributes DTO.
/// </summary>
internal class EmailUpdateAttributesDto
{
    public string? Address { get; set; }
    public string? Location { get; set; }
    public bool? Primary { get; set; }
}

/// <summary>
/// PhoneNumber creation DTO for JSON:API requests.
/// </summary>
internal class PhoneNumberCreateDto
{
    public string Type { get; set; } = "PhoneNumber";
    public PhoneNumberCreateAttributesDto Attributes { get; set; } = new();
}

/// <summary>
/// PhoneNumber update DTO for JSON:API requests.
/// </summary>
internal class PhoneNumberUpdateDto
{
    public string Type { get; set; } = "PhoneNumber";
    public string Id { get; set; } = string.Empty;
    public PhoneNumberUpdateAttributesDto Attributes { get; set; } = new();
}

/// <summary>
/// PhoneNumber creation attributes DTO.
/// </summary>
internal class PhoneNumberCreateAttributesDto
{
    public string Number { get; set; } = string.Empty;
    public string Location { get; set; } = "Mobile";
    public bool Primary { get; set; }
    public bool Carrier { get; set; } = true; // SMS capability
}

/// <summary>
/// PhoneNumber update attributes DTO.
/// </summary>
internal class PhoneNumberUpdateAttributesDto
{
    public string? Number { get; set; }
    public string? Location { get; set; }
    public bool? Primary { get; set; }
    public bool? Carrier { get; set; } // SMS capability
}

#endregion