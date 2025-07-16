using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.Fluent;
using PlanningCenter.Api.Client.Models.Requests;

namespace PlanningCenter.Api.Client.Fluent;

/// <summary>
/// Fluent API implementation for creating people with related data.
/// Allows chaining of related data creation in a single fluent operation.
/// </summary>
public class PeopleCreateContext : IPeopleCreateContext
{
    private readonly IPeopleService _peopleService;
    private readonly PersonCreateRequest _personRequest;
    private readonly List<AddressCreateRequest> _addressRequests = new();
    private readonly List<EmailCreateRequest> _emailRequests = new();
    private readonly List<PhoneNumberCreateRequest> _phoneNumberRequests = new();

    public PeopleCreateContext(IPeopleService peopleService, PersonCreateRequest personRequest)
    {
        _peopleService = peopleService ?? throw new ArgumentNullException(nameof(peopleService));
        _personRequest = personRequest ?? throw new ArgumentNullException(nameof(personRequest));
    }

    public IPeopleCreateContext WithAddress(AddressCreateRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        
        _addressRequests.Add(request);
        return this;
    }

    public IPeopleCreateContext WithEmail(EmailCreateRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        
        _emailRequests.Add(request);
        return this;
    }

    public IPeopleCreateContext WithPhoneNumber(PhoneNumberCreateRequest request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        
        _phoneNumberRequests.Add(request);
        return this;
    }

    public async Task<Person> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        // Step 1: Create the person
        var person = await _peopleService.CreateAsync(_personRequest, cancellationToken);
        
        // Step 2: Add all related data
        var tasks = new List<Task>();
        
        // Add addresses
        foreach (var addressRequest in _addressRequests)
        {
            tasks.Add(_peopleService.AddAddressAsync(person.Id, addressRequest, cancellationToken));
        }
        
        // Add emails
        foreach (var emailRequest in _emailRequests)
        {
            tasks.Add(_peopleService.AddEmailAsync(person.Id, emailRequest, cancellationToken));
        }
        
        // Add phone numbers
        foreach (var phoneRequest in _phoneNumberRequests)
        {
            tasks.Add(_peopleService.AddPhoneNumberAsync(person.Id, phoneRequest, cancellationToken));
        }
        
        // Wait for all related data to be created
        if (tasks.Any())
        {
            await Task.WhenAll(tasks);
        }
        
        // Step 3: Return the created person (optionally refresh to get related data)
        // For now, return the original person. In a full implementation, you might
        // want to refresh the person data to include the newly created related entities.
        return person;
    }
}