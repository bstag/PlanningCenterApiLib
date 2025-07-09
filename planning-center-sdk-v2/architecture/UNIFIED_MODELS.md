# Unified Core Models Strategy

## Problem Statement

The Planning Center API has inconsistent object definitions across different modules. For example, the `Person` object varies significantly between the People, Giving, Calendar, and other modules. This creates confusion for developers and makes it difficult to work with data that spans multiple modules.

## Solution: Unified Core Models

The SDK implements unified core models that combine all properties from module-specific variations, providing a consistent interface regardless of the data source.

## Core Model Definitions

### Person Model
```csharp
namespace PlanningCenter.Api.Client.Models.Core
{
    public class Person
    {
        // Common properties across all modules
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}".Trim();
        
        // Contact information
        public string PrimaryEmail { get; set; }
        public List<Email> Emails { get; set; } = new();
        public List<PhoneNumber> PhoneNumbers { get; set; } = new();
        public List<Address> Addresses { get; set; } = new();
        
        // Demographics
        public DateTime? Birthdate { get; set; }
        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
        public string Grade { get; set; }
        
        // Organizational relationships
        public Campus PrimaryCampus { get; set; }
        public string PrimaryCampusId { get; set; }
        public List<Household> Households { get; set; } = new();
        
        // People module specific
        public string Status { get; set; }
        public DateTime? Anniversary { get; set; }
        public string School { get; set; }
        public string MedicalNotes { get; set; }
        public List<FieldDatum> FieldData { get; set; } = new();
        
        // Giving module specific
        public decimal? TotalGiven { get; set; }
        public DateTime? LastGiftDate { get; set; }
        public decimal? AverageGiftAmount { get; set; }
        
        // Calendar module specific
        public string CalendarPermissions { get; set; }
        public List<string> CalendarRoles { get; set; } = new();
        
        // Services module specific
        public List<string> ServiceRoles { get; set; } = new();
        public List<string> TeamMemberships { get; set; } = new();
        
        // Audit fields
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        
        // Metadata
        public string AvatarUrl { get; set; }
        public Dictionary<string, object> CustomFields { get; set; } = new();
        public string SourceModule { get; set; } // Tracks which module the data came from
    }
}
```

### Campus Model
```csharp
namespace PlanningCenter.Api.Client.Models.Core
{
    public class Campus
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        
        // Contact information
        public Address Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
        
        // Organizational
        public string TimeZone { get; set; }
        public string Country { get; set; }
        public bool IsActive { get; set; }
        
        // Module-specific properties
        public string GivingDesignation { get; set; } // Giving module
        public List<string> ServiceTypes { get; set; } = new(); // Services module
        public List<string> CheckInLocations { get; set; } = new(); // Check-ins module
        
        // Audit fields
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Metadata
        public string LogoUrl { get; set; }
        public Dictionary<string, object> CustomFields { get; set; } = new();
        public string SourceModule { get; set; }
    }
}
```

### Organization Model
```csharp
namespace PlanningCenter.Api.Client.Models.Core
{
    public class Organization
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string TimeZone { get; set; }
        public string Country { get; set; }
        public string DateFormat { get; set; }
        public string TimeFormat { get; set; }
        
        // Contact information
        public string Website { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public Address Address { get; set; }
        
        // Branding
        public string LogoUrl { get; set; }
        public string PrimaryColor { get; set; }
        public string SecondaryColor { get; set; }
        
        // Settings
        public List<string> EnabledModules { get; set; } = new();
        public Dictionary<string, object> ModuleSettings { get; set; } = new();
        
        // Audit fields
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        public string SourceModule { get; set; }
    }
}
```

## Mapping Strategy

### Adapter Pattern Implementation
```csharp
public interface IModelMapper<TSource, TTarget>
{
    TTarget Map(TSource source);
    List<TTarget> Map(List<TSource> sources);
}

public class PersonMapper : 
    IModelMapper<People.PersonDto, Core.Person>,
    IModelMapper<Giving.PersonDto, Core.Person>,
    IModelMapper<Calendar.PersonDto, Core.Person>
{
    public Core.Person Map(People.PersonDto source)
    {
        return new Core.Person
        {
            Id = source.Id,
            FirstName = source.FirstName,
            LastName = source.LastName,
            PrimaryEmail = source.EmailAddresses?.FirstOrDefault()?.Address,
            Emails = source.EmailAddresses?.Select(MapEmail).ToList() ?? new(),
            PhoneNumbers = source.PhoneNumbers?.Select(MapPhoneNumber).ToList() ?? new(),
            Addresses = source.Addresses?.Select(MapAddress).ToList() ?? new(),
            Birthdate = source.Birthdate,
            Gender = source.Gender,
            MaritalStatus = source.MaritalStatus,
            Status = source.Status,
            Anniversary = source.Anniversary,
            School = source.School,
            MedicalNotes = source.MedicalNotes,
            PrimaryCampusId = source.PrimaryCampusId,
            FieldData = source.FieldData?.Select(MapFieldDatum).ToList() ?? new(),
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt,
            AvatarUrl = source.Avatar,
            SourceModule = "People"
        };
    }
    
    public Core.Person Map(Giving.PersonDto source)
    {
        return new Core.Person
        {
            Id = source.Id,
            FirstName = source.FirstName,
            LastName = source.LastName,
            PrimaryEmail = source.EmailAddresses?.FirstOrDefault()?.ToString(),
            TotalGiven = source.TotalGiven,
            LastGiftDate = source.LastGiftDate,
            AverageGiftAmount = source.AverageGiftAmount,
            PrimaryCampusId = source.PrimaryCampusId,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt,
            SourceModule = "Giving"
        };
    }
    
    public Core.Person Map(Calendar.PersonDto source)
    {
        return new Core.Person
        {
            Id = source.Id,
            FirstName = source.FirstName,
            LastName = source.LastName,
            PrimaryEmail = source.Email,
            CalendarPermissions = source.Permissions,
            CalendarRoles = source.Roles?.ToList() ?? new(),
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt,
            SourceModule = "Calendar"
        };
    }
    
    // Helper mapping methods
    private Email MapEmail(People.EmailDto source) => new()
    {
        Address = source.Address,
        Location = source.Location,
        IsPrimary = source.Primary
    };
    
    private PhoneNumber MapPhoneNumber(People.PhoneNumberDto source) => new()
    {
        Number = source.Number,
        Location = source.Location,
        IsPrimary = source.Primary
    };
    
    private Address MapAddress(People.AddressDto source) => new()
    {
        Street = source.Street,
        City = source.City,
        State = source.State,
        Zip = source.Zip,
        Location = source.Location,
        IsPrimary = source.Primary
    };
}
```

### Service Integration
```csharp
public class PeopleService : IPeopleService
{
    private readonly IApiConnection _apiConnection;
    private readonly PersonMapper _personMapper;
    
    public PeopleService(IApiConnection apiConnection, PersonMapper personMapper)
    {
        _apiConnection = apiConnection;
        _personMapper = personMapper;
    }
    
    public async Task<Core.Person> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        var dto = await _apiConnection.GetAsync<People.PersonDto>($"/people/v2/people/{id}", cancellationToken);
        return _personMapper.Map(dto);
    }
    
    public async Task<IPagedResponse<Core.Person>> ListAsync(
        QueryParameters parameters = null, 
        CancellationToken cancellationToken = default)
    {
        var response = await _apiConnection.GetPagedAsync<People.PersonDto>("/people/v2/people", parameters, cancellationToken);
        
        return new PagedResponse<Core.Person>
        {
            Data = response.Data.Select(_personMapper.Map).ToList(),
            Meta = response.Meta,
            Links = response.Links
        };
    }
}
```

## Benefits of Unified Models

### 1. Consistent Developer Experience
```csharp
// Same Person object regardless of source module
var peopleService = serviceProvider.GetService<IPeopleService>();
var givingService = serviceProvider.GetService<IGivingService>();

var personFromPeople = await peopleService.GetAsync("123");
var personFromGiving = await givingService.GetPersonAsync("123");

// Both return Core.Person with consistent interface
Console.WriteLine(personFromPeople.FullName);
Console.WriteLine(personFromGiving.FullName);
```

### 2. Cross-Module Data Correlation
```csharp
// Easy to correlate data across modules
var person = await peopleService.GetAsync("123");
var donations = await givingService.GetDonationsForPersonAsync(person.Id);

// Person object has giving-specific properties populated when from giving module
if (person.SourceModule == "Giving")
{
    Console.WriteLine($"Total given: {person.TotalGiven:C}");
}
```

### 3. Simplified Data Aggregation
```csharp
// Aggregate person data from multiple modules
public async Task<Core.Person> GetCompletePersonAsync(string personId)
{
    var tasks = new[]
    {
        peopleService.GetAsync(personId),
        givingService.GetPersonAsync(personId),
        calendarService.GetPersonAsync(personId)
    };
    
    var results = await Task.WhenAll(tasks);
    
    // Merge data from all modules into single unified person
    return MergePersonData(results);
}
```

## Handling Module-Specific Properties

### Conditional Property Access
```csharp
public static class PersonExtensions
{
    public static bool HasGivingData(this Core.Person person)
    {
        return person.TotalGiven.HasValue || person.LastGiftDate.HasValue;
    }
    
    public static bool HasCalendarData(this Core.Person person)
    {
        return !string.IsNullOrEmpty(person.CalendarPermissions) || person.CalendarRoles.Any();
    }
    
    public static bool HasServicesData(this Core.Person person)
    {
        return person.ServiceRoles.Any() || person.TeamMemberships.Any();
    }
}
```

### Source Module Tracking
```csharp
// Track which module provided the data
public enum SourceModule
{
    People,
    Giving,
    Calendar,
    CheckIns,
    Groups,
    Registrations,
    Publishing,
    Services,
    Webhooks,
    Merged // When data is combined from multiple modules
}
```

## Future Enhancements

### 1. Automatic Data Merging
- Intelligent merging of person data from multiple modules
- Conflict resolution strategies
- Data freshness tracking

### 2. Change Tracking
- Track which properties have been modified
- Support for partial updates
- Optimistic concurrency control

### 3. Validation
- Cross-module validation rules
- Data consistency checks
- Required field validation based on source module

This unified model strategy provides a consistent, developer-friendly interface while preserving the rich data available from each Planning Center module.