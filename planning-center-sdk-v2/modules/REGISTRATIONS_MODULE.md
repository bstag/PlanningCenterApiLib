# Registrations Module Specification

## Overview

The Registrations module manages event registration and attendee management in Planning Center. It provides comprehensive functionality for creating registration forms, processing registrations, managing attendees, handling payments, and tracking capacity limits.

## Core Entities

### Signup
The primary entity representing a registration event or opportunity.

**Key Attributes:**
- `id` - Unique identifier
- `name` - Signup name
- `description` - Signup description
- `archived` - Archive status
- `open_at` - Registration open time
- `close_at` - Registration close time
- `logo_url` - Signup logo/image
- `new_registration_url` - Public registration URL
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `category` - Signup category
- `campus` - Associated campus
- `attendees` - Registered attendees
- `registrations` - Registration records
- `selection_types` - Registration options
- `signup_location` - Event location
- `signup_times` - Event times

### Registration
Represents an individual registration submission.

**Key Attributes:**
- `id` - Unique identifier
- `created_at` - Registration timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `signup` - Associated signup
- `attendees` - Registered attendees

### Attendee
Represents a person registered for a signup.

**Key Attributes:**
- `id` - Unique identifier
- `complete` - Registration completion status
- `active` - Active status
- `canceled` - Cancellation status
- `waitlisted` - Waitlist status
- `waitlisted_at` - Waitlist timestamp
- `created_at` - Registration timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `signup` - Associated signup
- `registration` - Associated registration
- `person` - Associated person
- `emergency_contact` - Emergency contact information
- `selection_types` - Selected options

### SelectionType
Represents registration options or ticket types.

**Key Attributes:**
- `id` - Unique identifier
- `name` - Option name
- `publicly_available` - Public availability
- `price_cents` - Price in cents
- `price_currency` - Currency code
- `price_currency_symbol` - Currency symbol
- `price_formatted` - Formatted price display
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `signup` - Associated signup
- `attendees` - Attendees who selected this option

### SignupLocation
Represents the location where a signup event takes place.

**Key Attributes:**
- `id` - Unique identifier
- `name` - Location name
- `address_data` - Address information
- `subpremise` - Building/room details
- `latitude` - Geographic latitude
- `longitude` - Geographic longitude
- `location_type` - Type of location
- `url` - Location website
- `formatted_address` - Formatted address
- `full_formatted_address` - Complete address
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `signup` - Associated signup
- `signup_times` - Associated times

### SignupTime
Represents specific times for a signup event.

**Key Attributes:**
- `id` - Unique identifier
- `starts_at` - Start time
- `ends_at` - End time
- `all_day` - All-day event flag
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `signup` - Associated signup
- `signup_location` - Associated location

### Category
Represents a category for organizing signups.

**Key Attributes:**
- `id` - Unique identifier
- `name` - Category name
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `signups` - Signups in this category

### EmergencyContact
Represents emergency contact information for an attendee.

**Key Attributes:**
- `id` - Unique identifier
- `name` - Contact name
- `phone_number` - Contact phone number

**Relationships:**
- `attendee` - Associated attendee

### Campus
Represents a campus location for registrations.

**Key Attributes:**
- `id` - Unique identifier
- `name` - Campus name
- `street` - Street address
- `city` - City
- `state` - State/province
- `zip` - Postal code
- `country` - Country
- `full_formatted_address` - Complete address
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `signups` - Signups at this campus

## API Endpoints

### Signup Management
- `GET /registrations/v2/signups` - List all signups
- `GET /registrations/v2/signups/{id}` - Get specific signup
- `POST /registrations/v2/signups` - Create new signup
- `PATCH /registrations/v2/signups/{id}` - Update signup
- `DELETE /registrations/v2/signups/{id}` - Delete signup

### Registration Processing
- `GET /registrations/v2/signups/{signup_id}/registrations` - List registrations
- `GET /registrations/v2/registrations/{id}` - Get specific registration
- `POST /registrations/v2/signups/{signup_id}/registrations` - Submit registration

### Attendee Management
- `GET /registrations/v2/signups/{signup_id}/attendees` - List attendees
- `GET /registrations/v2/attendees/{id}` - Get specific attendee
- `POST /registrations/v2/signups/{signup_id}/attendees` - Add attendee
- `PATCH /registrations/v2/attendees/{id}` - Update attendee
- `DELETE /registrations/v2/attendees/{id}` - Remove attendee

### Selection Types (Options)
- `GET /registrations/v2/signups/{signup_id}/selection_types` - List options
- `GET /registrations/v2/selection_types/{id}` - Get specific option
- `POST /registrations/v2/signups/{signup_id}/selection_types` - Create option
- `PATCH /registrations/v2/selection_types/{id}` - Update option

### Location and Time Management
- `GET /registrations/v2/signups/{signup_id}/signup_location` - Get location
- `POST /registrations/v2/signups/{signup_id}/signup_location` - Set location
- `GET /registrations/v2/signups/{signup_id}/signup_times` - List times
- `POST /registrations/v2/signups/{signup_id}/signup_times` - Add time

### Category Management
- `GET /registrations/v2/categories` - List categories
- `POST /registrations/v2/categories` - Create category
- `PATCH /registrations/v2/categories/{id}` - Update category

### Campus Management
- `GET /registrations/v2/campuses` - List campuses
- `GET /registrations/v2/campuses/{id}` - Get specific campus

## Query Parameters

### Include Parameters
- `category` - Include signup category
- `campus` - Include campus information
- `attendees` - Include registered attendees
- `selection_types` - Include registration options
- `signup_location` - Include location information
- `signup_times` - Include event times

### Filtering
- `where[name]` - Filter by signup name
- `where[category_id]` - Filter by category
- `where[campus_id]` - Filter by campus
- `where[archived]` - Filter by archive status
- `where[open_at]` - Filter by open date
- `where[close_at]` - Filter by close date

### Sorting
- `order=name` - Sort by signup name
- `order=open_at` - Sort by open date
- `order=close_at` - Sort by close date
- `order=created_at` - Sort by creation date
- `order=-created_at` - Sort by creation date (descending)

## Service Interface

```csharp
public interface IRegistrationsService
{
    // Signup management
    Task<Signup> GetSignupAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Signup>> ListSignupsAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Signup> CreateSignupAsync(SignupCreateRequest request, CancellationToken cancellationToken = default);
    Task<Signup> UpdateSignupAsync(string id, SignupUpdateRequest request, CancellationToken cancellationToken = default);
    Task DeleteSignupAsync(string id, CancellationToken cancellationToken = default);
    
    // Registration processing
    Task<Registration> GetRegistrationAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Registration>> ListRegistrationsAsync(string signupId, QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Registration> SubmitRegistrationAsync(string signupId, RegistrationCreateRequest request, CancellationToken cancellationToken = default);
    
    // Attendee management
    Task<Attendee> GetAttendeeAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Attendee>> ListAttendeesAsync(string signupId, QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Attendee> AddAttendeeAsync(string signupId, AttendeeCreateRequest request, CancellationToken cancellationToken = default);
    Task<Attendee> UpdateAttendeeAsync(string id, AttendeeUpdateRequest request, CancellationToken cancellationToken = default);
    Task DeleteAttendeeAsync(string id, CancellationToken cancellationToken = default);
    
    // Waitlist management
    Task<Attendee> AddToWaitlistAsync(string signupId, AttendeeCreateRequest request, CancellationToken cancellationToken = default);
    Task<Attendee> RemoveFromWaitlistAsync(string attendeeId, CancellationToken cancellationToken = default);
    Task<Attendee> PromoteFromWaitlistAsync(string attendeeId, CancellationToken cancellationToken = default);
    
    // Selection type management
    Task<SelectionType> GetSelectionTypeAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<SelectionType>> ListSelectionTypesAsync(string signupId, QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<SelectionType> CreateSelectionTypeAsync(string signupId, SelectionTypeCreateRequest request, CancellationToken cancellationToken = default);
    Task<SelectionType> UpdateSelectionTypeAsync(string id, SelectionTypeUpdateRequest request, CancellationToken cancellationToken = default);
    Task DeleteSelectionTypeAsync(string id, CancellationToken cancellationToken = default);
    
    // Location management
    Task<SignupLocation> GetSignupLocationAsync(string signupId, CancellationToken cancellationToken = default);
    Task<SignupLocation> SetSignupLocationAsync(string signupId, SignupLocationCreateRequest request, CancellationToken cancellationToken = default);
    Task<SignupLocation> UpdateSignupLocationAsync(string signupId, SignupLocationUpdateRequest request, CancellationToken cancellationToken = default);
    
    // Time management
    Task<IPagedResponse<SignupTime>> ListSignupTimesAsync(string signupId, QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<SignupTime> AddSignupTimeAsync(string signupId, SignupTimeCreateRequest request, CancellationToken cancellationToken = default);
    Task<SignupTime> UpdateSignupTimeAsync(string id, SignupTimeUpdateRequest request, CancellationToken cancellationToken = default);
    Task DeleteSignupTimeAsync(string id, CancellationToken cancellationToken = default);
    
    // Emergency contact management
    Task<EmergencyContact> GetEmergencyContactAsync(string attendeeId, CancellationToken cancellationToken = default);
    Task<EmergencyContact> SetEmergencyContactAsync(string attendeeId, EmergencyContactCreateRequest request, CancellationToken cancellationToken = default);
    Task<EmergencyContact> UpdateEmergencyContactAsync(string attendeeId, EmergencyContactUpdateRequest request, CancellationToken cancellationToken = default);
    
    // Category management
    Task<Category> GetCategoryAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Category>> ListCategoriesAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Category> CreateCategoryAsync(CategoryCreateRequest request, CancellationToken cancellationToken = default);
    Task<Category> UpdateCategoryAsync(string id, CategoryUpdateRequest request, CancellationToken cancellationToken = default);
    
    // Campus management
    Task<Campus> GetCampusAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Campus>> ListCampusesAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    
    // Person-specific operations
    Task<Core.Person> GetPersonAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Attendee>> GetAttendeesForPersonAsync(string personId, QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Signup>> GetSignupsForPersonAsync(string personId, QueryParameters parameters = null, CancellationToken cancellationToken = default);
    
    // Reporting
    Task<RegistrationReport> GenerateRegistrationReportAsync(RegistrationReportRequest request, CancellationToken cancellationToken = default);
    Task<int> GetRegistrationCountAsync(string signupId, CancellationToken cancellationToken = default);
    Task<int> GetWaitlistCountAsync(string signupId, CancellationToken cancellationToken = default);
}
```

## Fluent API Interface

```csharp
public interface IRegistrationsFluentContext
{
    // Signup queries
    ISignupFluentContext Signups();
    ISignupFluentContext Signup(string signupId);
    
    // Category queries
    ICategoryFluentContext Categories();
    ICategoryFluentContext Category(string categoryId);
    
    // Campus queries
    ICampusFluentContext Campuses();
    ICampusFluentContext Campus(string campusId);
    
    // Person-specific operations
    IPersonRegistrationsFluentContext Person(string personId);
    
    // Reporting
    IRegistrationReportingFluentContext Reports();
}

public interface ISignupFluentContext
{
    ISignupFluentContext Where(Expression<Func<Signup, bool>> predicate);
    ISignupFluentContext Include(Expression<Func<Signup, object>> include);
    ISignupFluentContext OrderBy(Expression<Func<Signup, object>> orderBy);
    ISignupFluentContext OrderByDescending(Expression<Func<Signup, object>> orderBy);
    ISignupFluentContext InCategory(string categoryId);
    ISignupFluentContext AtCampus(string campusId);
    ISignupFluentContext Active();
    ISignupFluentContext Archived();
    ISignupFluentContext OpenForRegistration();
    ISignupFluentContext ClosedForRegistration();
    ISignupFluentContext OpeningAfter(DateTime date);
    ISignupFluentContext ClosingBefore(DateTime date);
    
    // Signup-specific operations
    IAttendeeFluentContext Attendees();
    IRegistrationFluentContext Registrations();
    ISelectionTypeFluentContext SelectionTypes();
    ISignupTimeFluentContext Times();
    
    Task<Signup> GetAsync(CancellationToken cancellationToken = default);
    Task<IPagedResponse<Signup>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default);
    Task<List<Signup>> GetAllAsync(CancellationToken cancellationToken = default);
}

public interface IAttendeeFluentContext
{
    IAttendeeFluentContext Where(Expression<Func<Attendee, bool>> predicate);
    IAttendeeFluentContext Include(Expression<Func<Attendee, object>> include);
    IAttendeeFluentContext Active();
    IAttendeeFluentContext Canceled();
    IAttendeeFluentContext Waitlisted();
    IAttendeeFluentContext Complete();
    IAttendeeFluentContext Incomplete();
    IAttendeeFluentContext RegisteredAfter(DateTime date);
    IAttendeeFluentContext RegisteredBefore(DateTime date);
    
    Task<IPagedResponse<Attendee>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default);
    Task<List<Attendee>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
}

public interface IRegistrationFluentContext
{
    IRegistrationFluentContext Where(Expression<Func<Registration, bool>> predicate);
    IRegistrationFluentContext Include(Expression<Func<Registration, object>> include);
    IRegistrationFluentContext SubmittedAfter(DateTime date);
    IRegistrationFluentContext SubmittedBefore(DateTime date);
    
    Task<IPagedResponse<Registration>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default);
    Task<List<Registration>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
}
```

## Usage Examples

### Service-Based API
```csharp
// Get open signups
var openSignups = await registrationsService.ListSignupsAsync(new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["archived"] = false,
        ["open_at"] = $"<={DateTime.Now:yyyy-MM-ddTHH:mm:ssZ}",
        ["close_at"] = $">={DateTime.Now:yyyy-MM-ddTHH:mm:ssZ}"
    },
    Include = new[] { "category", "campus", "selection_types" },
    OrderBy = "open_at"
});

// Create a new signup
var newSignup = await registrationsService.CreateSignupAsync(new SignupCreateRequest
{
    Name = "Summer Camp 2024",
    Description = "Annual summer camp for kids and teens",
    CategoryId = "camps-category-id",
    CampusId = "main-campus-id",
    OpenAt = DateTime.Now.AddDays(30),
    CloseAt = DateTime.Now.AddDays(90)
});

// Add selection types (ticket options)
var earlyBirdOption = await registrationsService.CreateSelectionTypeAsync(newSignup.Id, new SelectionTypeCreateRequest
{
    Name = "Early Bird Registration",
    PriceCents = 15000, // $150.00
    PubliclyAvailable = true
});

var regularOption = await registrationsService.CreateSelectionTypeAsync(newSignup.Id, new SelectionTypeCreateRequest
{
    Name = "Regular Registration",
    PriceCents = 20000, // $200.00
    PubliclyAvailable = true
});

// Submit a registration
var registration = await registrationsService.SubmitRegistrationAsync(newSignup.Id, new RegistrationCreateRequest
{
    Attendees = new[]
    {
        new AttendeeCreateRequest
        {
            PersonId = "person123",
            SelectionTypeIds = new[] { earlyBirdOption.Id },
            EmergencyContact = new EmergencyContactCreateRequest
            {
                Name = "Jane Doe",
                PhoneNumber = "555-1234"
            }
        }
    }
});

// Get attendees for a signup
var attendees = await registrationsService.ListAttendeesAsync(newSignup.Id, new QueryParameters
{
    Include = new[] { "person", "emergency_contact", "selection_types" },
    OrderBy = "created_at"
});
```

### Fluent API
```csharp
// Complex signup query
var upcomingSignups = await client
    .Registrations()
    .Signups()
    .Active()
    .OpenForRegistration()
    .InCategory("camps-category-id")
    .Include(s => s.Category)
    .Include(s => s.Campus)
    .Include(s => s.SelectionTypes)
    .OrderBy(s => s.OpenAt)
    .GetPagedAsync(pageSize: 50);

// Get all registrations for a person
var personRegistrations = await client
    .Registrations()
    .Person("person123")
    .Attendees()
    .Active()
    .Include(a => a.Signup)
    .Include(a => a.SelectionTypes)
    .OrderByDescending(a => a.CreatedAt)
    .GetAllAsync();

// Signup-specific operations
var signupStats = await client
    .Registrations()
    .Signup("signup123")
    .Attendees()
    .Active()
    .GetCountAsync();

var waitlistCount = await client
    .Registrations()
    .Signup("signup123")
    .Attendees()
    .Waitlisted()
    .GetCountAsync();

// Category-based queries
var campSignups = await client
    .Registrations()
    .Category("camps-category-id")
    .Signups()
    .Active()
    .OpenForRegistration()
    .GetAllAsync();

// Campus-specific signups
var campusSignups = await client
    .Registrations()
    .Campus("campus123")
    .Signups()
    .Active()
    .OrderBy(s => s.OpenAt)
    .GetAllAsync();

// Time-based queries
var recentRegistrations = await client
    .Registrations()
    .Signup("signup123")
    .Registrations()
    .SubmittedAfter(DateTime.Today.AddDays(-7))
    .Include(r => r.Attendees)
    .GetAllAsync();

// Selection type analysis
var popularOptions = await client
    .Registrations()
    .Signup("signup123")
    .SelectionTypes()
    .Include(st => st.Attendees)
    .GetAllAsync();
```

## Implementation Notes

### Data Mapping
- Map Registrations-specific Person DTOs to unified Core.Person model
- Handle registration status and payment information
- Preserve attendee-specific properties and emergency contacts

### Security Considerations
- Protect sensitive attendee information
- Implement proper authorization for registration access
- Secure payment and financial data

### Caching Strategy
- Cache signup and category data (relatively static)
- Avoid caching real-time registration data
- Use time-based cache expiration for counts

### Error Handling
- Handle registration capacity limits
- Validate registration time windows
- Handle payment processing errors
- Manage waitlist overflow scenarios

### Performance Considerations
- Optimize queries for large attendee datasets
- Implement efficient registration processing
- Use pagination for attendee and registration lists
- Consider read replicas for reporting queries

### Payment Integration
- Support for multiple payment processors
- Secure payment token handling
- Refund and cancellation processing
- Payment status tracking and notifications

### Capacity Management
- Real-time capacity tracking
- Automatic waitlist management
- Overflow handling and notifications
- Capacity alerts and reporting

This module provides comprehensive registration management capabilities while maintaining security and performance standards for event registration systems.