# Check-Ins Module Specification

## Overview

The Check-Ins module manages event attendance and check-in processes in Planning Center. It provides comprehensive functionality for tracking attendance, managing check-in stations, processing check-ins and check-outs, and generating attendance reports.

## Core Entities

### CheckIn
The primary entity representing an individual attendance record.

**Key Attributes:**
- `id` - Unique identifier
- `first_name`, `last_name` - Person identification
- `number` - Check-in number/badge number
- `security_code` - Security code for pickup
- `kind` - Check-in type (regular, guest, volunteer)
- `created_at` - Check-in timestamp
- `checked_out_at` - Check-out timestamp
- `confirmed_at` - Confirmation timestamp
- `medical_notes` - Medical information
- `emergency_contact_name` - Emergency contact
- `emergency_contact_phone_number` - Emergency phone
- `one_time_guest` - Guest status flag

**Relationships:**
- `person` - Associated person record
- `event` - Check-in event
- `event_time` - Specific event time
- `location` - Check-in location
- `options` - Selected check-in options

### Event
Represents a check-in event or service.

**Key Attributes:**
- `id` - Unique identifier
- `name` - Event name
- `frequency` - Event frequency (weekly, monthly, etc.)
- `enable_services_integration` - Services module integration
- `feature_flags` - Enabled features
- `pre_select_enabled` - Pre-selection capability
- `check_in_count_enabled` - Count tracking
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `attendance_types` - Attendance categories
- `event_periods` - Event periods
- `event_times` - Event times
- `locations` - Check-in locations
- `check_ins` - Check-in records

### EventPeriod
Represents a period within an event (e.g., different age groups or services).

**Key Attributes:**
- `id` - Unique identifier
- `name` - Period name
- `starts_at` - Start time
- `ends_at` - End time
- `regular_count` - Regular attendee count
- `guest_count` - Guest count
- `volunteer_count` - Volunteer count
- `note` - Period notes

**Relationships:**
- `event` - Parent event
- `event_times` - Associated times
- `location_event_periods` - Location associations

### Location
Represents a physical check-in location.

**Key Attributes:**
- `id` - Unique identifier
- `name` - Location name
- `kind` - Location type (room, area, etc.)
- `opened` - Open status
- `questions` - Check-in questions
- `age_min_in_months` - Minimum age in months
- `age_max_in_months` - Maximum age in months
- `age_range_by` - Age range criteria
- `grade_min` - Minimum grade
- `grade_max` - Maximum grade

**Relationships:**
- `event` - Associated event
- `location_event_periods` - Event period associations
- `location_event_times` - Event time associations
- `location_labels` - Location labels
- `check_ins` - Check-ins at this location

### Station
Represents a check-in kiosk or station.

**Key Attributes:**
- `id` - Unique identifier
- `name` - Station name
- `timeout_seconds` - Session timeout
- `input_type` - Input method (touch, keyboard, etc.)
- `input_type_options` - Input configuration
- `mode` - Station mode (check-in, check-out, etc.)
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `event` - Associated event
- `location` - Station location
- `theme` - Station theme/appearance

### AttendanceType
Represents a type of attendee tracked by headcount rather than individual check-ins.

**Key Attributes:**
- `id` - Unique identifier
- `name` - Attendance type name
- `color` - Display color
- `limit` - Maximum count
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `event` - Associated event

### Pass
Represents a check-in pass or badge.

**Key Attributes:**
- `id` - Unique identifier
- `code` - Pass code
- `kind` - Pass type
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `person` - Pass holder
- `check_in` - Associated check-in

### Theme
Represents the visual theme/appearance for check-in stations.

**Key Attributes:**
- `id` - Unique identifier
- `name` - Theme name
- `color` - Primary color
- `text_color` - Text color
- `background` - Background configuration
- `mode` - Theme mode (light, dark)
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `stations` - Stations using this theme

## API Endpoints

### Check-In Management
- `GET /check-ins/v2/check_ins` - List all check-ins
- `GET /check-ins/v2/check_ins/{id}` - Get specific check-in
- `POST /check-ins/v2/check_ins` - Create new check-in
- `PATCH /check-ins/v2/check_ins/{id}` - Update check-in
- `DELETE /check-ins/v2/check_ins/{id}` - Delete check-in

### Event Management
- `GET /check-ins/v2/events` - List events
- `GET /check-ins/v2/events/{id}` - Get specific event
- `POST /check-ins/v2/events` - Create event
- `PATCH /check-ins/v2/events/{id}` - Update event

### Location Management
- `GET /check-ins/v2/events/{event_id}/locations` - List event locations
- `GET /check-ins/v2/locations/{id}` - Get specific location
- `POST /check-ins/v2/events/{event_id}/locations` - Create location
- `PATCH /check-ins/v2/locations/{id}` - Update location

### Station Management
- `GET /check-ins/v2/stations` - List stations
- `GET /check-ins/v2/stations/{id}` - Get specific station
- `POST /check-ins/v2/stations` - Create station
- `PATCH /check-ins/v2/stations/{id}` - Update station

### Attendance Tracking
- `GET /check-ins/v2/events/{event_id}/attendance_types` - List attendance types
- `POST /check-ins/v2/events/{event_id}/attendance_types` - Create attendance type
- `GET /check-ins/v2/events/{event_id}/event_periods` - List event periods
- `POST /check-ins/v2/events/{event_id}/event_periods` - Create event period

## Query Parameters

### Include Parameters
- `person` - Include person information
- `event` - Include event information
- `location` - Include location information
- `event_time` - Include event time information
- `options` - Include selected options

### Filtering
- `where[created_at]` - Filter by check-in date
- `where[first_name]` - Filter by first name
- `where[last_name]` - Filter by last name
- `where[kind]` - Filter by check-in type
- `where[security_code]` - Filter by security code

### Sorting
- `order=created_at` - Sort by check-in time
- `order=first_name` - Sort by first name
- `order=last_name` - Sort by last name
- `order=-created_at` - Sort by check-in time (descending)

## Service Interface

```csharp
public interface ICheckInsService
{
    // Check-in management
    Task<CheckIn> GetCheckInAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<CheckIn>> ListCheckInsAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<CheckIn> CreateCheckInAsync(CheckInCreateRequest request, CancellationToken cancellationToken = default);
    Task<CheckIn> UpdateCheckInAsync(string id, CheckInUpdateRequest request, CancellationToken cancellationToken = default);
    Task DeleteCheckInAsync(string id, CancellationToken cancellationToken = default);
    
    // Check-out operations
    Task<CheckIn> CheckOutAsync(string checkInId, CancellationToken cancellationToken = default);
    Task<CheckIn> CheckOutBySecurityCodeAsync(string securityCode, CancellationToken cancellationToken = default);
    
    // Event management
    Task<Event> GetEventAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Event>> ListEventsAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Event> CreateEventAsync(EventCreateRequest request, CancellationToken cancellationToken = default);
    Task<Event> UpdateEventAsync(string id, EventUpdateRequest request, CancellationToken cancellationToken = default);
    
    // Location management
    Task<Location> GetLocationAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Location>> ListLocationsAsync(string eventId, QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Location> CreateLocationAsync(string eventId, LocationCreateRequest request, CancellationToken cancellationToken = default);
    Task<Location> UpdateLocationAsync(string id, LocationUpdateRequest request, CancellationToken cancellationToken = default);
    
    // Station management
    Task<Station> GetStationAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Station>> ListStationsAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Station> CreateStationAsync(StationCreateRequest request, CancellationToken cancellationToken = default);
    Task<Station> UpdateStationAsync(string id, StationUpdateRequest request, CancellationToken cancellationToken = default);
    
    // Attendance tracking
    Task<IPagedResponse<AttendanceType>> ListAttendanceTypesAsync(string eventId, QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<AttendanceType> CreateAttendanceTypeAsync(string eventId, AttendanceTypeCreateRequest request, CancellationToken cancellationToken = default);
    
    // Event periods
    Task<IPagedResponse<EventPeriod>> ListEventPeriodsAsync(string eventId, QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<EventPeriod> CreateEventPeriodAsync(string eventId, EventPeriodCreateRequest request, CancellationToken cancellationToken = default);
    
    // Pass management
    Task<Pass> GetPassAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Pass>> ListPassesAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Pass> CreatePassAsync(PassCreateRequest request, CancellationToken cancellationToken = default);
    
    // Theme management
    Task<Theme> GetThemeAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Theme>> ListThemesAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Theme> CreateThemeAsync(ThemeCreateRequest request, CancellationToken cancellationToken = default);
    
    // Person-specific operations
    Task<Core.Person> GetPersonAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<CheckIn>> GetCheckInsForPersonAsync(string personId, QueryParameters parameters = null, CancellationToken cancellationToken = default);
    
    // Reporting
    Task<AttendanceReport> GenerateAttendanceReportAsync(AttendanceReportRequest request, CancellationToken cancellationToken = default);
    Task<int> GetAttendanceCountAsync(string eventId, DateTime date, CancellationToken cancellationToken = default);
}
```

## Fluent API Interface

```csharp
public interface ICheckInsFluentContext
{
    // Check-in queries
    ICheckInFluentContext CheckIns();
    ICheckInFluentContext CheckIn(string checkInId);
    
    // Event queries
    IEventFluentContext Events();
    IEventFluentContext Event(string eventId);
    
    // Location queries
    ILocationFluentContext Locations();
    ILocationFluentContext Location(string locationId);
    
    // Station queries
    IStationFluentContext Stations();
    IStationFluentContext Station(string stationId);
    
    // Person-specific operations
    IPersonCheckInsFluentContext Person(string personId);
    
    // Reporting
    IAttendanceReportingFluentContext Reports();
}

public interface ICheckInFluentContext
{
    ICheckInFluentContext Where(Expression<Func<CheckIn, bool>> predicate);
    ICheckInFluentContext Include(Expression<Func<CheckIn, object>> include);
    ICheckInFluentContext OrderBy(Expression<Func<CheckIn, object>> orderBy);
    ICheckInFluentContext OrderByDescending(Expression<Func<CheckIn, object>> orderBy);
    ICheckInFluentContext ForEvent(string eventId);
    ICheckInFluentContext ForLocation(string locationId);
    ICheckInFluentContext ForPerson(string personId);
    ICheckInFluentContext OnDate(DateTime date);
    ICheckInFluentContext BetweenDates(DateTime startDate, DateTime endDate);
    ICheckInFluentContext OfKind(string kind);
    
    Task<CheckIn> GetAsync(CancellationToken cancellationToken = default);
    Task<IPagedResponse<CheckIn>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default);
    Task<List<CheckIn>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
}

public interface IEventFluentContext
{
    IEventFluentContext Where(Expression<Func<Event, bool>> predicate);
    IEventFluentContext Include(Expression<Func<Event, object>> include);
    
    // Event-specific operations
    ICheckInFluentContext CheckIns();
    ILocationFluentContext Locations();
    IEventPeriodFluentContext Periods();
    IAttendanceTypeFluentContext AttendanceTypes();
    
    Task<Event> GetAsync(CancellationToken cancellationToken = default);
    Task<IPagedResponse<Event>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default);
}
```

## Usage Examples

### Service-Based API
```csharp
// Get check-ins for today
var todayCheckIns = await checkInsService.ListCheckInsAsync(new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["created_at"] = $">={DateTime.Today:yyyy-MM-dd}",
        ["created_at"] = $"<{DateTime.Today.AddDays(1):yyyy-MM-dd}"
    },
    Include = new[] { "person", "location", "event" },
    OrderBy = "-created_at"
});

// Create a new check-in
var checkIn = await checkInsService.CreateCheckInAsync(new CheckInCreateRequest
{
    PersonId = "person123",
    EventId = "event456",
    LocationId = "location789",
    Kind = "regular",
    EmergencyContactName = "Jane Doe",
    EmergencyContactPhoneNumber = "555-1234"
});

// Check someone out
var checkedOut = await checkInsService.CheckOutAsync(checkIn.Id);

// Get attendance count for an event
var attendanceCount = await checkInsService.GetAttendanceCountAsync("event456", DateTime.Today);
```

### Fluent API
```csharp
// Complex check-in query
var checkIns = await client
    .CheckIns()
    .CheckIns()
    .ForEvent("event123")
    .OnDate(DateTime.Today)
    .OfKind("regular")
    .Include(c => c.Person)
    .Include(c => c.Location)
    .OrderByDescending(c => c.CreatedAt)
    .GetPagedAsync(pageSize: 50);

// Get all check-ins for a person this month
var personCheckIns = await client
    .CheckIns()
    .Person("person123")
    .CheckIns()
    .BetweenDates(DateTime.Today.AddDays(-30), DateTime.Today)
    .Include(c => c.Event)
    .Include(c => c.Location)
    .GetAllAsync();

// Event-specific operations
var eventStats = await client
    .CheckIns()
    .Event("event123")
    .CheckIns()
    .OnDate(DateTime.Today)
    .GetCountAsync();

// Location-based queries
var locationCheckIns = await client
    .CheckIns()
    .Location("location456")
    .CheckIns()
    .BetweenDates(DateTime.Today.AddDays(-7), DateTime.Today)
    .GetAllAsync();

// Station management
var stations = await client
    .CheckIns()
    .Stations()
    .Where(s => s.Mode == "check-in")
    .Include(s => s.Location)
    .Include(s => s.Theme)
    .GetAllAsync();
```

## Implementation Notes

### Data Mapping
- Map Check-Ins-specific Person DTOs to unified Core.Person model
- Handle check-in specific properties (security codes, medical notes)
- Preserve attendance tracking and timing information

### Security Considerations
- Protect sensitive information (medical notes, emergency contacts)
- Implement proper authorization for check-in operations
- Secure security codes and pass information

### Caching Strategy
- Cache event and location data (relatively static)
- Avoid caching real-time check-in data
- Use time-based cache expiration for attendance counts

### Error Handling
- Handle check-in conflicts (duplicate check-ins)
- Validate age restrictions and location capacity
- Handle station connectivity issues gracefully

### Performance Considerations
- Optimize queries for large check-in datasets
- Implement efficient real-time check-in processing
- Use pagination for check-in lists
- Consider read replicas for reporting queries

### Real-Time Features
- Support for real-time check-in notifications
- Live attendance count updates
- Station status monitoring
- Automatic check-out processing

This module provides comprehensive check-in management capabilities while maintaining security and performance standards for attendance tracking systems.