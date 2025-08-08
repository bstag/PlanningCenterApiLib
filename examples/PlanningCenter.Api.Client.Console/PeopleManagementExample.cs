using Microsoft.Extensions.Logging;
using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Services;

namespace PlanningCenter.Api.Client.Console;

/// <summary>
/// Example demonstrating comprehensive People management functionality including
/// person CRUD operations and address/email/phone number management.
/// </summary>
public class PeopleManagementExample
{
    private readonly PeopleService _peopleService;
    private readonly ILogger<PeopleManagementExample> _logger;

    public PeopleManagementExample(PeopleService peopleService, ILogger<PeopleManagementExample> logger)
    {
        _peopleService = peopleService;
        _logger = logger;
    }

    /// <summary>
    /// Demonstrates complete people management workflow.
    /// </summary>
    public async Task RunExampleAsync()
    {
        try
        {
            _logger.LogInformation("=== People Management Example ===");

            // 1. Get current user
            await GetCurrentUserExample();

            // 2. Create a new person
            var person = await CreatePersonExample();

            // 3. Add contact information
            await AddContactInformationExample(person.Id);

            // 4. Update contact information
            await UpdateContactInformationExample(person.Id);

            // 5. List people with pagination
            await ListPeopleExample();

            // 6. Clean up (delete the test person)
            await DeletePersonExample(person.Id);

            _logger.LogInformation("=== Example completed successfully ===");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running people management example");
            throw;
        }
    }

    private async Task GetCurrentUserExample()
    {
        _logger.LogInformation("--- Getting current user ---");
        
        var currentUser = await _peopleService.GetMeAsync();
        _logger.LogInformation("Current user: {FullName} (ID: {Id})", currentUser.FullName, currentUser.Id);
    }

    private async Task<Person> CreatePersonExample()
    {
        _logger.LogInformation("--- Creating a new person ---");

        var createRequest = new PersonCreateRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            Birthdate = new DateTime(1990, 5, 15),
            Status = "active"
        };

        var person = await _peopleService.CreateAsync(createRequest);
        _logger.LogInformation("Created person: {FullName} (ID: {Id})", person.FullName, person.Id);

        return person;
    }

    private async Task AddContactInformationExample(string personId)
    {
        _logger.LogInformation("--- Adding contact information ---");

        // Add address
        var addressRequest = new AddressCreateRequest
        {
            Street = "123 Main St",
            City = "Anytown",
            State = "CA",
            Zip = "12345",
            Country = "US",
            Location = "Home",
            IsPrimary = true
        };

        var address = await _peopleService.AddAddressAsync(personId, addressRequest);
        _logger.LogInformation("Added address: {Street}, {City}, {State} {Zip} (ID: {Id})", 
            address.Street, address.City, address.State, address.Zip, address.Id);

        // Add email
        var emailRequest = new EmailCreateRequest
        {
            Address = "john.doe@example.com",
            Location = "Home",
            IsPrimary = true
        };

        var email = await _peopleService.AddEmailAsync(personId, emailRequest);
        _logger.LogInformation("Added email: {Address} (ID: {Id})", email.Address, email.Id);

        // Add phone number
        var phoneRequest = new PhoneNumberCreateRequest
        {
            Number = "+1-555-123-4567",
            Location = "Mobile",
            IsPrimary = true,
            CanReceiveSms = true
        };

        var phoneNumber = await _peopleService.AddPhoneNumberAsync(personId, phoneRequest);
        _logger.LogInformation("Added phone: {Number} (ID: {Id})", phoneNumber.Number, phoneNumber.Id);
    }

    private async Task UpdateContactInformationExample(string personId)
    {
        _logger.LogInformation("--- Updating contact information ---");

        // For this example, we'll assume we have the IDs from the previous step
        // In a real scenario, you'd get these from the person's relationships or by listing them

        // Update person details
        var updateRequest = new PersonUpdateRequest
        {
            MiddleName = "Michael",
            School = "University of California"
        };

        var updatedPerson = await _peopleService.UpdateAsync(personId, updateRequest);
        _logger.LogInformation("Updated person: {FullName}", updatedPerson.FullName);
    }

    private async Task ListPeopleExample()
    {
        _logger.LogInformation("--- Listing people with pagination ---");

        var parameters = new QueryParameters
        {
            PerPage = 5,
            Where = new Dictionary<string, object>
            {
                ["first_name"] = "John"
            }
        };

        var firstPage = await _peopleService.ListAsync(parameters);
        _logger.LogInformation("Found {Count} people on first page (Total: {Total})", 
            firstPage.Data.Count, firstPage.Meta.TotalCount);

        foreach (var person in firstPage.Data)
        {
            _logger.LogInformation("  - {FullName} (ID: {Id})", person.FullName, person.Id);
        }

        // Demonstrate pagination
        if (firstPage.HasNextPage)
        {
            var nextPage = await firstPage.GetNextPageAsync();
            if (nextPage != null)
            {
                _logger.LogInformation("Next page has {Count} people", nextPage.Data.Count);
            }
            else
            {
                _logger.LogInformation("No additional pages.");
            }
        }
    }

    private async Task DeletePersonExample(string personId)
    {
        _logger.LogInformation("--- Cleaning up test data ---");

        await _peopleService.DeleteAsync(personId);
        _logger.LogInformation("Deleted person with ID: {PersonId}", personId);
    }
}