using Microsoft.Extensions.Logging;
using PlanningCenter.Api.Client.Mapping.People;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.People;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Models.Exceptions;
using PlanningCenter.Api.Client.Models.JsonApi.People;
using PlanningCenter.Api.Client.Models.JsonApi;
using System.Linq;

namespace PlanningCenter.Api.Client.Services;

/// <summary>
/// Service implementation for the Planning Center People module.
/// Provides comprehensive people management with built-in pagination support.
/// </summary>
public class PeopleService : ServiceBase, IPeopleService
{
    private const string BaseEndpoint = "/people/v2";

    public PeopleService(
        IApiConnection apiConnection,
        ILogger<PeopleService> logger)
        : base(logger, apiConnection)
    {
    }

    /// <summary>
    /// Gets the current authenticated user's person record.
    /// This is useful for testing authentication and getting the current user's information.
    /// </summary>
    public async Task<Person> GetMeAsync(CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(
            async () =>
            {
                var response = await ApiConnection.GetAsync<JsonApiSingleResponse<PersonDto>>(
                    $"{BaseEndpoint}/me", cancellationToken);

                if (response?.Data == null)
                {
                    throw new PlanningCenterApiGeneralException("Failed to get current user - no data returned");
                }

                return PersonMapper.MapToDomain(response.Data);
            },
            "GetMe",
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Gets a single person by ID.
    /// </summary>
    public async Task<Person?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        return await GetResourceByIdAsync<PersonDto, Person>(
            $"{BaseEndpoint}/people",
            id,
            PersonMapper.MapToDomain,
            "GetPerson",
            cancellationToken);
    }

    /// <summary>
    /// Lists people with optional filtering, sorting, and pagination.
    /// Returns a paginated response with built-in navigation helpers.
    /// </summary>
    public async Task<IPagedResponse<Person>> ListAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        return await ListResourcesAsync<PersonDto, Person>(
            $"{BaseEndpoint}/people",
            parameters,
            PersonMapper.MapToDomain,
            "ListPeople",
            cancellationToken);
    }

    /// <summary>
    /// Creates a new person.
    /// </summary>
    public async Task<Person> CreateAsync(PersonCreateRequest request, CancellationToken cancellationToken = default)
    {
        ValidateNotNull(request, nameof(request));
        ValidateNotNullOrEmpty(request.FirstName, nameof(request.FirstName));
        ValidateNotNullOrEmpty(request.LastName, nameof(request.LastName));

        return await ExecuteAsync(
            async () =>
            {
                // Convert request to JSON:API format
                var jsonApiRequest = new JsonApiRequest<Models.JsonApi.People.PersonCreateDto>
                {
                    Data = new Models.JsonApi.People.PersonCreateDto
                    {
                        Type = "Person",
                        Attributes = PersonMapper.MapToCreateAttributes(request)
                    }
                };

                var response = await ApiConnection.PostAsync<JsonApiSingleResponse<PersonDto>>(
                    $"{BaseEndpoint}/people", jsonApiRequest, cancellationToken);

                if (response?.Data == null)
                {
                    throw new PlanningCenterApiGeneralException("Failed to create person - no data returned");
                }

                return PersonMapper.MapToDomain(response.Data);
            },
            "CreatePerson",
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Updates an existing person.
    /// </summary>
    public async Task<Person> UpdateAsync(string id, PersonUpdateRequest request, CancellationToken cancellationToken = default)
    {
        ValidateNotNullOrEmpty(id, nameof(id));
        ValidateNotNull(request, nameof(request));

        return await ExecuteAsync(
            async () =>
            {
                // Convert request to JSON:API format
                var jsonApiRequest = new JsonApiRequest<Models.JsonApi.People.PersonUpdateDto>
                {
                    Data = new Models.JsonApi.People.PersonUpdateDto
                    {
                        Type = "Person",
                        Id = id,
                        Attributes = PersonMapper.MapToUpdateAttributes(request)
                    }
                };

                var response = await ApiConnection.PatchAsync<JsonApiSingleResponse<PersonDto>>(
                    $"{BaseEndpoint}/people/{id}", jsonApiRequest, cancellationToken);

                if (response?.Data == null)
                {
                    throw new PlanningCenterApiGeneralException("Failed to update person - no data returned");
                }

                return PersonMapper.MapToDomain(response.Data);
            },
            "UpdatePerson",
            id,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Deletes a person.
    /// </summary>
    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        ValidateNotNullOrEmpty(id, nameof(id));

        await ExecuteAsync(
            async () =>
            {
                await ApiConnection.DeleteAsync($"{BaseEndpoint}/people/{id}", cancellationToken);
                return true; // ExecuteAsync requires a return value
            },
            "DeletePerson",
            id,
            cancellationToken: cancellationToken);
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
        return await ExecuteAsync(
            async () =>
            {
                var firstPage = await ListAsync(parameters, cancellationToken);

                if (!firstPage.HasNextPage)
                {
                    return (IReadOnlyList<Person>)firstPage.Data;
                }

                return (IReadOnlyList<Person>)await firstPage.GetAllRemainingAsync(cancellationToken);
            },
            "GetAllPeople",
            cancellationToken: cancellationToken);
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
        ValidateNotNullOrEmpty(personId, nameof(personId));
        ValidateNotNull(request, nameof(request));
        ValidateNotNullOrEmpty(request.Street, nameof(request.Street));
        ValidateNotNullOrEmpty(request.City, nameof(request.City));
        ValidateNotNullOrEmpty(request.State, nameof(request.State));
        ValidateNotNullOrEmpty(request.Zip, nameof(request.Zip));

        return await ExecuteAsync(
            async () =>
            {
                // Convert request to JSON:API format
                var jsonApiRequest = new JsonApiRequest<Models.JsonApi.People.AddressCreateDto>
                {
                    Data = new Models.JsonApi.People.AddressCreateDto
                    {
                        Type = "Address",
                        Attributes = AddressMapper.MapToCreateAttributes(request)
                    }
                };

                var response = await ApiConnection.PostAsync<JsonApiSingleResponse<AddressDto>>(
                    $"{BaseEndpoint}/people/{personId}/addresses", jsonApiRequest, cancellationToken);

                if (response?.Data == null)
                {
                    throw new PlanningCenterApiGeneralException("Failed to create address - no data returned");
                }

                return AddressMapper.MapToDomain(response.Data);
            },
            "AddAddress",
            personId,
            cancellationToken: cancellationToken);
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

        Logger.LogDebug("Updating address: {AddressId} for person: {PersonId}", addressId, personId);

        // Convert request to JSON:API format
        var jsonApiRequest = new JsonApiRequest<Models.JsonApi.People.AddressUpdateDto>
        {
            Data = new Models.JsonApi.People.AddressUpdateDto
            {
                Type = "Address",
                Id = addressId,
                Attributes = AddressMapper.MapToUpdateAttributes(request)
            }
        };

        var response = await ApiConnection.PatchAsync<JsonApiSingleResponse<AddressDto>>(
            $"{BaseEndpoint}/people/{personId}/addresses/{addressId}", jsonApiRequest, cancellationToken);

        if (response?.Data == null)
        {
            throw new PlanningCenterApiGeneralException("Failed to update address - no data returned");
        }

        var address = AddressMapper.MapToDomain(response.Data);
        Logger.LogInformation("Successfully updated address: {AddressId} for person: {PersonId}", addressId, personId);

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

        Logger.LogDebug("Deleting address: {AddressId} from person: {PersonId}", addressId, personId);

        await ApiConnection.DeleteAsync($"{BaseEndpoint}/people/{personId}/addresses/{addressId}", cancellationToken);

        Logger.LogInformation("Successfully deleted address: {AddressId} from person: {PersonId}", addressId, personId);
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

        Logger.LogDebug("Adding email to person: {PersonId}", personId);

        // Convert request to JSON:API format
        var jsonApiRequest = new JsonApiRequest<Models.JsonApi.People.EmailCreateDto>
        {
            Data = new Models.JsonApi.People.EmailCreateDto
            {
                Type = "Email",
                Attributes = EmailMapper.MapToCreateAttributes(request)
            }
        };

        var response = await ApiConnection.PostAsync<JsonApiSingleResponse<EmailDto>>(
            $"{BaseEndpoint}/people/{personId}/emails", jsonApiRequest, cancellationToken);

        if (response?.Data == null)
        {
            throw new PlanningCenterApiGeneralException("Failed to create email - no data returned");
        }

        var email = EmailMapper.MapToDomain(response.Data);
        Logger.LogInformation("Successfully added email: {EmailId} to person: {PersonId}", email.Id, personId);

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

        Logger.LogDebug("Updating email: {EmailId} for person: {PersonId}", emailId, personId);

        // Convert request to JSON:API format
        var jsonApiRequest = new JsonApiRequest<Models.JsonApi.People.EmailUpdateDto>
        {
            Data = new Models.JsonApi.People.EmailUpdateDto
            {
                Type = "Email",
                Id = emailId,
                Attributes = EmailMapper.MapToUpdateAttributes(request)
            }
        };

        var response = await ApiConnection.PatchAsync<JsonApiSingleResponse<EmailDto>>(
            $"{BaseEndpoint}/people/{personId}/emails/{emailId}", jsonApiRequest, cancellationToken);

        if (response?.Data == null)
        {
            throw new PlanningCenterApiGeneralException("Failed to update email - no data returned");
        }

        var email = EmailMapper.MapToDomain(response.Data);
        Logger.LogInformation("Successfully updated email: {EmailId} for person: {PersonId}", emailId, personId);

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

        Logger.LogDebug("Deleting email: {EmailId} from person: {PersonId}", emailId, personId);

        await ApiConnection.DeleteAsync($"{BaseEndpoint}/people/{personId}/emails/{emailId}", cancellationToken);

        Logger.LogInformation("Successfully deleted email: {EmailId} from person: {PersonId}", emailId, personId);
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

        Logger.LogDebug("Adding phone number to person: {PersonId}", personId);

        // Convert request to JSON:API format
        var jsonApiRequest = new JsonApiRequest<Models.JsonApi.People.PhoneNumberCreateDto>
        {
            Data = new Models.JsonApi.People.PhoneNumberCreateDto
            {
                Type = "PhoneNumber",
                Attributes = PhoneNumberMapper.MapToCreateAttributes(request)
            }
        };

        var response = await ApiConnection.PostAsync<JsonApiSingleResponse<PhoneNumberDto>>(
            $"{BaseEndpoint}/people/{personId}/phone_numbers", jsonApiRequest, cancellationToken);

        if (response?.Data == null)
        {
            throw new PlanningCenterApiGeneralException("Failed to create phone number - no data returned");
        }

        var phoneNumber = PhoneNumberMapper.MapToDomain(response.Data);
        Logger.LogInformation("Successfully added phone number: {PhoneNumberId} to person: {PersonId}", phoneNumber.Id, personId);

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

        Logger.LogDebug("Updating phone number: {PhoneNumberId} for person: {PersonId}", phoneId, personId);

        // Convert request to JSON:API format
        var jsonApiRequest = new JsonApiRequest<Models.JsonApi.People.PhoneNumberUpdateDto>
        {
            Data = new Models.JsonApi.People.PhoneNumberUpdateDto
            {
                Type = "PhoneNumber",
                Id = phoneId,
                Attributes = PhoneNumberMapper.MapToUpdateAttributes(request)
            }
        };

        var response = await ApiConnection.PatchAsync<JsonApiSingleResponse<PhoneNumberDto>>(
            $"{BaseEndpoint}/people/{personId}/phone_numbers/{phoneId}", jsonApiRequest, cancellationToken);

        if (response?.Data == null)
        {
            throw new PlanningCenterApiGeneralException("Failed to update phone number - no data returned");
        }

        var phoneNumber = PhoneNumberMapper.MapToDomain(response.Data);
        Logger.LogInformation("Successfully updated phone number: {PhoneNumberId} for person: {PersonId}", phoneId, personId);

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

        Logger.LogDebug("Deleting phone number: {PhoneNumberId} from person: {PersonId}", phoneId, personId);

        await ApiConnection.DeleteAsync($"{BaseEndpoint}/people/{personId}/phone_numbers/{phoneId}", cancellationToken);

        Logger.LogInformation("Successfully deleted phone number: {PhoneNumberId} from person: {PersonId}", phoneId, personId);
    }

    #endregion

    #region Household Management

    /// <summary>
    /// Gets a specific household by ID.
    /// </summary>
    public async Task<Household?> GetHouseholdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Household ID cannot be null or empty", nameof(id));

        Logger.LogDebug("Getting household with ID: {HouseholdId}", id);

        try
        {
            var response = await ApiConnection.GetAsync<JsonApiSingleResponse<HouseholdDto>>(
                $"{BaseEndpoint}/households/{id}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogWarning("Household not found: {HouseholdId}", id);
                return null;
            }

            var household = HouseholdMapper.MapToDomain(response.Data);
            Logger.LogInformation("Successfully retrieved household: {HouseholdId}", id);

            return household;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            Logger.LogWarning("Household not found: {HouseholdId}", id);
            return null;
        }
    }

    /// <summary>
    /// Lists households with optional filtering, sorting, and pagination.
    /// </summary>
    public async Task<IPagedResponse<Household>> ListHouseholdsAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Listing households with parameters: {@Parameters}", parameters);

        var response = await ApiConnection.GetPagedAsync<HouseholdDto>(
            $"{BaseEndpoint}/households", parameters, cancellationToken);

        // Map DTOs to domain models
        var households = response.Data.Select(HouseholdMapper.MapToDomain).ToList();

        // Create a new paged response with mapped data
        var mappedResponse = new PagedResponse<Household>
        {
            Data = households,
            Meta = response.Meta,
            Links = response.Links
        };

        // Copy navigation properties if the response is a concrete PagedResponse
        if (response is PagedResponse<HouseholdDto> concreteResponse)
        {
            mappedResponse.ApiConnection = concreteResponse.ApiConnection;
            mappedResponse.OriginalParameters = concreteResponse.OriginalParameters;
            mappedResponse.OriginalEndpoint = concreteResponse.OriginalEndpoint;
        }

        Logger.LogInformation("Successfully retrieved page {Page} with {Count} households",
            response.Meta.CurrentPage, households.Count);

        return mappedResponse;
    }

    /// <summary>
    /// Creates a new household.
    /// </summary>
    public async Task<Household> CreateHouseholdAsync(HouseholdCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Name is required", nameof(request));

        if (string.IsNullOrWhiteSpace(request.PrimaryContactId))
            throw new ArgumentException("PrimaryContactId is required", nameof(request));

        if (request.PersonIds == null || !request.PersonIds.Any())
            throw new ArgumentException("At least one person must be added to the household", nameof(request));

        Logger.LogDebug("Creating household: {Name}", request.Name);

        // Convert request to JSON:API format
        var jsonApiRequest = new JsonApiRequest<HouseholdCreateDto>
        {
            Data = HouseholdMapper.MapToCreateDto(request)
        };

        var response = await ApiConnection.PostAsync<JsonApiSingleResponse<HouseholdDto>>(
            $"{BaseEndpoint}/households", jsonApiRequest, cancellationToken);

        if (response?.Data == null)
        {
            throw new PlanningCenterApiGeneralException("Failed to create household - no data returned");
        }

        var household = HouseholdMapper.MapToDomain(response.Data);
        Logger.LogInformation("Successfully created household: {HouseholdId} - {Name}",
            household.Id, household.Name);

        return household;
    }

    /// <summary>
    /// Updates an existing household.
    /// </summary>
    public async Task<Household> UpdateHouseholdAsync(string id, HouseholdUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Household ID cannot be null or empty", nameof(id));

        if (request == null)
            throw new ArgumentNullException(nameof(request));

        Logger.LogDebug("Updating household: {HouseholdId}", id);

        // Convert request to JSON:API format
        var jsonApiRequest = new JsonApiRequest<HouseholdUpdateDto>
        {
            Data = HouseholdMapper.MapToUpdateDto(id, request)
        };

        var response = await ApiConnection.PatchAsync<JsonApiSingleResponse<HouseholdDto>>(
            $"{BaseEndpoint}/households/{id}", jsonApiRequest, cancellationToken);

        if (response?.Data == null)
        {
            throw new PlanningCenterApiGeneralException("Failed to update household - no data returned");
        }

        var household = HouseholdMapper.MapToDomain(response.Data);
        Logger.LogInformation("Successfully updated household: {HouseholdId}", household.Id);

        return household;
    }

    #endregion

    #region Workflow Management

    /// <summary>
    /// Gets a specific workflow by ID.
    /// </summary>
    public async Task<Workflow?> GetWorkflowAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Workflow ID cannot be null or empty", nameof(id));

        Logger.LogDebug("Getting workflow with ID: {WorkflowId}", id);

        try
        {
            var response = await ApiConnection.GetAsync<JsonApiSingleResponse<WorkflowDto>>(
                $"{BaseEndpoint}/workflows/{id}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogWarning("Workflow not found: {WorkflowId}", id);
                return null;
            }

            var workflow = WorkflowMapper.MapToDomain(response.Data);
            Logger.LogInformation("Successfully retrieved workflow: {WorkflowId}", id);

            return workflow;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            Logger.LogWarning("Workflow not found: {WorkflowId}", id);
            return null;
        }
    }

    /// <summary>
    /// Lists workflows with optional filtering, sorting, and pagination.
    /// </summary>
    public async Task<IPagedResponse<Workflow>> ListWorkflowsAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Listing workflows with parameters: {@Parameters}", parameters);

        var response = await ApiConnection.GetPagedAsync<WorkflowDto>(
            $"{BaseEndpoint}/workflows", parameters, cancellationToken);

        // Map DTOs to domain models
        var workflows = response.Data.Select(WorkflowMapper.MapToDomain).ToList();

        // Create a new paged response with mapped data
        var mappedResponse = new PagedResponse<Workflow>
        {
            Data = workflows,
            Meta = response.Meta,
            Links = response.Links
        };

        // Copy navigation properties if the response is a concrete PagedResponse
        if (response is PagedResponse<WorkflowDto> concreteResponse)
        {
            mappedResponse.ApiConnection = concreteResponse.ApiConnection;
            mappedResponse.OriginalParameters = concreteResponse.OriginalParameters;
            mappedResponse.OriginalEndpoint = concreteResponse.OriginalEndpoint;
        }

        Logger.LogInformation("Successfully retrieved page {Page} with {Count} workflows",
            response.Meta.CurrentPage, workflows.Count);

        return mappedResponse;
    }

    /// <summary>
    /// Lists workflow cards for a specific workflow with optional filtering, sorting, and pagination.
    /// </summary>
    public async Task<IPagedResponse<WorkflowCard>> ListWorkflowCardsAsync(string workflowId, QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(workflowId))
            throw new ArgumentException("Workflow ID cannot be null or empty", nameof(workflowId));

        Logger.LogDebug("Listing workflow cards for workflow {WorkflowId} with parameters: {@Parameters}", workflowId, parameters);

        var response = await ApiConnection.GetPagedAsync<WorkflowCardDto>(
            $"{BaseEndpoint}/workflows/{workflowId}/cards", parameters, cancellationToken);

        // Map DTOs to domain models
        var cards = response.Data.Select(WorkflowCardMapper.MapToDomain).ToList();

        // Create a new paged response with mapped data
        var mappedResponse = new PagedResponse<WorkflowCard>
        {
            Data = cards,
            Meta = response.Meta,
            Links = response.Links
        };

        // Copy navigation properties if the response is a concrete PagedResponse
        if (response is PagedResponse<WorkflowCardDto> concreteResponse)
        {
            mappedResponse.ApiConnection = concreteResponse.ApiConnection;
            mappedResponse.OriginalParameters = concreteResponse.OriginalParameters;
            mappedResponse.OriginalEndpoint = concreteResponse.OriginalEndpoint;
        }

        Logger.LogInformation("Successfully retrieved page {Page} with {Count} workflow cards for workflow {WorkflowId}",
            response.Meta.CurrentPage, cards.Count, workflowId);

        return mappedResponse;
    }

    /// <summary>
    /// Creates a new workflow card for a specific workflow.
    /// </summary>
    public async Task<WorkflowCard> CreateWorkflowCardAsync(string workflowId, WorkflowCardCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(workflowId))
            throw new ArgumentException("Workflow ID cannot be null or empty", nameof(workflowId));

        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.PersonId))
            throw new ArgumentException("PersonId is required", nameof(request));

        Logger.LogDebug("Creating workflow card for workflow {WorkflowId} and person {PersonId}", workflowId, request.PersonId);

        // Convert request to JSON:API format
        var jsonApiRequest = new JsonApiRequest<WorkflowCardCreateDto>
        {
            Data = WorkflowCardMapper.MapToCreateDto(request, workflowId)
        };

        var response = await ApiConnection.PostAsync<JsonApiSingleResponse<WorkflowCardDto>>(
            $"{BaseEndpoint}/workflows/{workflowId}/cards", jsonApiRequest, cancellationToken);

        if (response?.Data == null)
        {
            throw new PlanningCenterApiGeneralException("Failed to create workflow card - no data returned");
        }

        var card = WorkflowCardMapper.MapToDomain(response.Data);
        Logger.LogInformation("Successfully created workflow card: {CardId} for workflow {WorkflowId}",
            card.Id, workflowId);

        return card;
    }

    /// <summary>
    /// Updates an existing workflow card.
    /// </summary>
    public async Task<WorkflowCard> UpdateWorkflowCardAsync(string workflowId, string cardId, WorkflowCardUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(workflowId))
            throw new ArgumentException("Workflow ID cannot be null or empty", nameof(workflowId));

        if (string.IsNullOrWhiteSpace(cardId))
            throw new ArgumentException("Card ID cannot be null or empty", nameof(cardId));

        if (request == null)
            throw new ArgumentNullException(nameof(request));

        Logger.LogDebug("Updating workflow card {CardId} for workflow {WorkflowId}", cardId, workflowId);

        // Convert request to JSON:API format
        var jsonApiRequest = new JsonApiRequest<WorkflowCardUpdateDto>
        {
            Data = WorkflowCardMapper.MapToUpdateDto(cardId, request)
        };

        var response = await ApiConnection.PatchAsync<JsonApiSingleResponse<WorkflowCardDto>>(
            $"{BaseEndpoint}/workflows/{workflowId}/cards/{cardId}", jsonApiRequest, cancellationToken);

        if (response?.Data == null)
        {
            throw new PlanningCenterApiGeneralException("Failed to update workflow card - no data returned");
        }

        var card = WorkflowCardMapper.MapToDomain(response.Data);
        Logger.LogInformation("Successfully updated workflow card: {CardId} for workflow {WorkflowId}",
            card.Id, workflowId);

        return card;
    }

    /// <summary>
    /// Deletes a workflow card.
    /// </summary>
    public async Task DeleteWorkflowCardAsync(string workflowId, string cardId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(workflowId))
            throw new ArgumentException("Workflow ID cannot be null or empty", nameof(workflowId));

        if (string.IsNullOrWhiteSpace(cardId))
            throw new ArgumentException("Card ID cannot be null or empty", nameof(cardId));

        Logger.LogDebug("Deleting workflow card {CardId} from workflow {WorkflowId}", cardId, workflowId);

        await ApiConnection.DeleteAsync(
            $"{BaseEndpoint}/workflows/{workflowId}/cards/{cardId}", cancellationToken);

        Logger.LogInformation("Successfully deleted workflow card {CardId} from workflow {WorkflowId}",
            cardId, workflowId);
    }

    #endregion

    #region Form Management

    /// <summary>
    /// Gets a specific form by ID.
    /// </summary>
    public async Task<Form?> GetFormAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Form ID cannot be null or empty", nameof(id));

        Logger.LogDebug("Getting form with ID: {FormId}", id);

        try
        {
            var response = await ApiConnection.GetAsync<JsonApiSingleResponse<FormDto>>(
                $"{BaseEndpoint}/forms/{id}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogWarning("Form not found: {FormId}", id);
                return null;
            }

            var form = FormMapper.MapToDomain(response.Data);
            Logger.LogInformation("Successfully retrieved form: {FormId}", id);

            return form;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            Logger.LogWarning("Form not found: {FormId}", id);
            return null;
        }
    }

    /// <summary>
    /// Lists forms with optional filtering, sorting, and pagination.
    /// </summary>
    public async Task<IPagedResponse<Form>> ListFormsAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Listing forms with parameters: {@Parameters}", parameters);

        var response = await ApiConnection.GetPagedAsync<FormDto>(
            $"{BaseEndpoint}/forms", parameters, cancellationToken);

        // Map DTOs to domain models
        var forms = response.Data.Select(FormMapper.MapToDomain).ToList();

        // Create a new paged response with mapped data
        var mappedResponse = new PagedResponse<Form>
        {
            Data = forms,
            Meta = response.Meta,
            Links = response.Links
        };

        // Copy navigation properties if the response is a concrete PagedResponse
        if (response is PagedResponse<FormDto> concreteResponse)
        {
            mappedResponse.ApiConnection = concreteResponse.ApiConnection;
            mappedResponse.OriginalParameters = concreteResponse.OriginalParameters;
            mappedResponse.OriginalEndpoint = concreteResponse.OriginalEndpoint;
        }

        Logger.LogInformation("Successfully retrieved page {Page} with {Count} forms",
            response.Meta.CurrentPage, forms.Count);

        return mappedResponse;
    }

    /// <summary>
    /// Lists form submissions for a specific form with optional filtering, sorting, and pagination.
    /// </summary>
    public async Task<IPagedResponse<FormSubmission>> ListFormSubmissionsAsync(string formId, QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(formId))
            throw new ArgumentException("Form ID cannot be null or empty", nameof(formId));

        Logger.LogDebug("Listing form submissions for form {FormId} with parameters: {@Parameters}", formId, parameters);

        var response = await ApiConnection.GetPagedAsync<FormSubmissionDto>(
            $"{BaseEndpoint}/forms/{formId}/form_submissions", parameters, cancellationToken);

        // Map DTOs to domain models
        var submissions = response.Data.Select(FormMapper.MapToDomain).ToList();

        // Create a new paged response with mapped data
        var mappedResponse = new PagedResponse<FormSubmission>
        {
            Data = submissions,
            Meta = response.Meta,
            Links = response.Links
        };

        // Copy navigation properties if the response is a concrete PagedResponse
        if (response is PagedResponse<FormSubmissionDto> concreteResponse)
        {
            mappedResponse.ApiConnection = concreteResponse.ApiConnection;
            mappedResponse.OriginalParameters = concreteResponse.OriginalParameters;
            mappedResponse.OriginalEndpoint = concreteResponse.OriginalEndpoint;
        }

        Logger.LogInformation("Successfully retrieved page {Page} with {Count} form submissions for form {FormId}",
            response.Meta.CurrentPage, submissions.Count, formId);

        return mappedResponse;
    }

    /// <summary>
    /// Submits a form.
    /// </summary>
    public async Task<FormSubmission> SubmitFormAsync(string formId, FormSubmitRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(formId))
            throw new ArgumentException("Form ID cannot be null or empty", nameof(formId));

        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (request.FieldData == null || !request.FieldData.Any())
            throw new ArgumentException("Field data is required", nameof(request));

        Logger.LogDebug("Submitting form {FormId}", formId);

        // Convert request to JSON:API format
        var jsonApiRequest = FormMapper.MapToSubmitRequest(formId, request);

        var response = await ApiConnection.PostAsync<JsonApiSingleResponse<FormSubmissionDto>>(
            $"{BaseEndpoint}/forms/{formId}/form_submissions", jsonApiRequest, cancellationToken);

        if (response?.Data == null)
        {
            throw new PlanningCenterApiGeneralException("Failed to submit form - no data returned");
        }

        var submission = FormMapper.MapToDomain(response.Data);
        Logger.LogInformation("Successfully submitted form: {FormId}, submission ID: {SubmissionId}",
            formId, submission.Id);

        return submission;
    }

    #endregion

    #region List Management

    /// <summary>
    /// Gets a specific people list by ID.
    /// </summary>
    public async Task<PeopleList?> GetPeopleListAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("List ID cannot be null or empty", nameof(id));

        Logger.LogDebug("Getting people list with ID: {ListId}", id);

        try
        {
            var response = await ApiConnection.GetAsync<JsonApiSingleResponse<PeopleListDto>>(
                $"{BaseEndpoint}/lists/{id}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogWarning("People list not found: {ListId}", id);
                return null;
            }

            var list = PeopleListMapper.MapToDomain(response.Data);
            Logger.LogInformation("Successfully retrieved people list: {ListId}", id);

            return list;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            Logger.LogWarning("People list not found: {ListId}", id);
            return null;
        }
    }

    /// <summary>
    /// Lists people lists with optional filtering, sorting, and pagination.
    /// </summary>
    public async Task<IPagedResponse<PeopleList>> ListPeopleListsAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Listing people lists with parameters: {@Parameters}", parameters);

        var response = await ApiConnection.GetPagedAsync<PeopleListDto>(
            $"{BaseEndpoint}/lists", parameters, cancellationToken);

        // Map DTOs to domain models
        var lists = response.Data.Select(PeopleListMapper.MapToDomain).ToList();

        // Create a new paged response with mapped data
        var mappedResponse = new PagedResponse<PeopleList>
        {
            Data = lists,
            Meta = response.Meta,
            Links = response.Links
        };

        // Copy navigation properties if the response is a concrete PagedResponse
        if (response is PagedResponse<PeopleListDto> concreteResponse)
        {
            mappedResponse.ApiConnection = concreteResponse.ApiConnection;
            mappedResponse.OriginalParameters = concreteResponse.OriginalParameters;
            mappedResponse.OriginalEndpoint = concreteResponse.OriginalEndpoint;
        }

        Logger.LogInformation("Successfully retrieved page {Page} with {Count} people lists",
            response.Meta.CurrentPage, lists.Count);

        return mappedResponse;
    }

    /// <summary>
    /// Creates a new people list.
    /// </summary>
    public async Task<PeopleList> CreatePeopleListAsync(PeopleListCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Name is required", nameof(request));

        Logger.LogDebug("Creating people list with name: {ListName}", request.Name);

        // Convert request to JSON:API format
        var jsonApiRequest = PeopleListMapper.MapToCreateRequest(request);

        var response = await ApiConnection.PostAsync<JsonApiSingleResponse<PeopleListDto>>(
            $"{BaseEndpoint}/lists", jsonApiRequest, cancellationToken);

        if (response?.Data == null)
        {
            throw new PlanningCenterApiGeneralException("Failed to create people list - no data returned");
        }

        var list = PeopleListMapper.MapToDomain(response.Data);
        Logger.LogInformation("Successfully created people list: {ListId}, name: {ListName}",
            list.Id, list.Name);

        return list;
    }

    /// <summary>
    /// Lists members of a specific people list with optional filtering, sorting, and pagination.
    /// </summary>
    public async Task<IPagedResponse<ListMember>> GetListMembersAsync(string listId, QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(listId))
            throw new ArgumentException("List ID cannot be null or empty", nameof(listId));

        Logger.LogDebug("Listing members for list {ListId} with parameters: {@Parameters}", listId, parameters);

        var response = await ApiConnection.GetPagedAsync<ListMemberDto>(
            $"{BaseEndpoint}/lists/{listId}/people", parameters, cancellationToken);

        // Map DTOs to domain models
        var members = response.Data.Select(PeopleListMapper.MapToDomain).ToList();

        // Create a new paged response with mapped data
        var mappedResponse = new PagedResponse<ListMember>
        {
            Data = members,
            Meta = response.Meta,
            Links = response.Links
        };

        // Copy navigation properties if the response is a concrete PagedResponse
        if (response is PagedResponse<ListMemberDto> concreteResponse)
        {
            mappedResponse.ApiConnection = concreteResponse.ApiConnection;
            mappedResponse.OriginalParameters = concreteResponse.OriginalParameters;
            mappedResponse.OriginalEndpoint = concreteResponse.OriginalEndpoint;
        }

        Logger.LogInformation("Successfully retrieved page {Page} with {Count} list members for list {ListId}",
            response.Meta.CurrentPage, members.Count, listId);

        return mappedResponse;
    }

    /// <summary>
    /// Gets a single workflow card by ID.
    /// </summary>
    public async Task<WorkflowCard?> GetWorkflowCardAsync(string workflowId, string cardId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(workflowId))
            throw new ArgumentException("Workflow ID cannot be null or empty", nameof(workflowId));
        if (string.IsNullOrWhiteSpace(cardId))
            throw new ArgumentException("Card ID cannot be null or empty", nameof(cardId));

        Logger.LogDebug("Getting workflow card {CardId} from workflow {WorkflowId}", cardId, workflowId);

        try
        {
            var response = await ApiConnection.GetAsync<JsonApiSingleResponse<WorkflowCardDto>>(
                $"{BaseEndpoint}/workflows/{workflowId}/cards/{cardId}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogDebug("Workflow card {CardId} not found in workflow {WorkflowId}", cardId, workflowId);
                return null;
            }

            var card = WorkflowCardMapper.MapToDomain(response.Data);
            Logger.LogInformation("Successfully retrieved workflow card: {CardId} from workflow {WorkflowId}", cardId, workflowId);

            return card;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            Logger.LogDebug("Workflow card {CardId} not found in workflow {WorkflowId}", cardId, workflowId);
            return null;
        }
    }

    /// <summary>
    /// Gets a single form submission by ID.
    /// </summary>
    public async Task<FormSubmission?> GetFormSubmissionAsync(string formId, string submissionId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(formId))
            throw new ArgumentException("Form ID cannot be null or empty", nameof(formId));
        if (string.IsNullOrWhiteSpace(submissionId))
            throw new ArgumentException("Submission ID cannot be null or empty", nameof(submissionId));

        Logger.LogDebug("Getting form submission {SubmissionId} from form {FormId}", submissionId, formId);

        try
        {
            var response = await ApiConnection.GetAsync<JsonApiSingleResponse<FormSubmissionDto>>(
                $"{BaseEndpoint}/forms/{formId}/form_submissions/{submissionId}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogDebug("Form submission {SubmissionId} not found in form {FormId}", submissionId, formId);
                return null;
            }

            var submission = FormMapper.MapToDomain(response.Data);
            Logger.LogInformation("Successfully retrieved form submission: {SubmissionId} from form {FormId}", submissionId, formId);

            return submission;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            Logger.LogDebug("Form submission {SubmissionId} not found in form {FormId}", submissionId, formId);
            return null;
        }
    }

    /// <summary>
    /// Updates an existing people list.
    /// </summary>
    public async Task<PeopleList> UpdatePeopleListAsync(string id, PeopleListUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("List ID cannot be null or empty", nameof(id));
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        Logger.LogDebug("Updating people list {ListId} with name: {Name}", id, request.Name);

        var jsonApiRequest = PeopleListMapper.MapToUpdateRequest(id, request);

        var response = await ApiConnection.PatchAsync<JsonApiSingleResponse<PeopleListDto>>(
            $"{BaseEndpoint}/lists/{id}", jsonApiRequest, cancellationToken);

        if (response?.Data == null)
        {
            throw new PlanningCenterApiGeneralException("Failed to update people list - no data returned");
        }

        var list = PeopleListMapper.MapToDomain(response.Data);
        Logger.LogInformation("Successfully updated people list: {ListId}, name: {ListName}",
            list.Id, list.Name);

        return list;
    }

    /// <summary>
    /// Deletes a people list.
    /// </summary>
    public async Task DeletePeopleListAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("List ID cannot be null or empty", nameof(id));

        Logger.LogDebug("Deleting people list {ListId}", id);

        await ApiConnection.DeleteAsync($"{BaseEndpoint}/lists/{id}", cancellationToken);

        Logger.LogInformation("Successfully deleted people list: {ListId}", id);
    }

    /// <summary>
    /// Lists people in a specific list.
    /// </summary>
    public async Task<IPagedResponse<Person>> ListPeopleInListAsync(string listId, QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(listId))
            throw new ArgumentException("List ID cannot be null or empty", nameof(listId));

        Logger.LogDebug("Listing people in list {ListId} with parameters: {@Parameters}", listId, parameters);

        var response = await ApiConnection.GetPagedAsync<PersonDto>(
            $"{BaseEndpoint}/lists/{listId}/people", parameters, cancellationToken);

        // Map DTOs to domain models
        var people = response.Data.Select(PersonMapper.MapToDomain).ToList();

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

        Logger.LogInformation("Successfully retrieved page {Page} with {Count} people in list {ListId}",
            response.Meta.CurrentPage, people.Count, listId);

        return mappedResponse;
    }

    /// <summary>
    /// Adds a person to a list.
    /// </summary>
    public async Task<ListMember> AddPersonToListAsync(string listId, ListMemberCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(listId))
            throw new ArgumentException("List ID cannot be null or empty", nameof(listId));
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        Logger.LogDebug("Adding person {PersonId} to list {ListId}", request.PersonId, listId);

        var jsonApiRequest = PeopleListMapper.MapToListMemberCreateRequest(request);

        var response = await ApiConnection.PostAsync<JsonApiSingleResponse<ListMemberDto>>(
            $"{BaseEndpoint}/lists/{listId}/people", jsonApiRequest, cancellationToken);

        if (response?.Data == null)
        {
            throw new PlanningCenterApiGeneralException("Failed to add person to list - no data returned");
        }

        var member = PeopleListMapper.MapToDomain(response.Data);
        Logger.LogInformation("Successfully added person {PersonId} to list {ListId}",
            request.PersonId, listId);

        return member;
    }

    /// <summary>
    /// Removes a person from a list.
    /// </summary>
    public async Task RemovePersonFromListAsync(string listId, string personId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(listId))
            throw new ArgumentException("List ID cannot be null or empty", nameof(listId));
        if (string.IsNullOrWhiteSpace(personId))
            throw new ArgumentException("Person ID cannot be null or empty", nameof(personId));

        Logger.LogDebug("Removing person {PersonId} from list {ListId}", personId, listId);

        await ApiConnection.DeleteAsync($"{BaseEndpoint}/lists/{listId}/people/{personId}", cancellationToken);

        Logger.LogInformation("Successfully removed person {PersonId} from list {ListId}", personId, listId);
    }

    /// <summary>
    /// Deletes a household.
    /// </summary>
    public async Task DeleteHouseholdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Household ID cannot be null or empty", nameof(id));

        Logger.LogDebug("Deleting household {HouseholdId}", id);

        await ApiConnection.DeleteAsync($"{BaseEndpoint}/households/{id}", cancellationToken);

        Logger.LogInformation("Successfully deleted household: {HouseholdId}", id);
    }

    /// <summary>
    /// Lists people in a specific household.
    /// </summary>
    public async Task<IPagedResponse<Person>> ListPeopleInHouseholdAsync(string householdId, QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(householdId))
            throw new ArgumentException("Household ID cannot be null or empty", nameof(householdId));

        Logger.LogDebug("Listing people in household {HouseholdId} with parameters: {@Parameters}", householdId, parameters);

        var response = await ApiConnection.GetPagedAsync<PersonDto>(
            $"{BaseEndpoint}/households/{householdId}/people", parameters, cancellationToken);

        // Map DTOs to domain models
        var people = response.Data.Select(PersonMapper.MapToDomain).ToList();

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

        Logger.LogInformation("Successfully retrieved page {Page} with {Count} people in household {HouseholdId}",
            response.Meta.CurrentPage, people.Count, householdId);

        return mappedResponse;
    }

    #endregion

    // Private mapping methods have been removed - now using dedicated mappers directly

    // All internal DTOs have been moved to their own files in the Models.JsonApi.People namespace
}