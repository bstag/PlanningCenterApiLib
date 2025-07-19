# Check-Ins Module Documentation

The Check-Ins module provides comprehensive attendance and event management capabilities for Planning Center. This module handles all check-in related functionality including event check-ins, attendance tracking, location management, and station configuration.

## 📋 Module Overview

### Key Entities
- **Check-Ins**: Individual attendance records for people at events
- **Events**: Scheduled activities that people can check into
- **Locations**: Physical or virtual spaces where events occur
- **Stations**: Check-in kiosks or devices used for processing attendance
- **Event Times**: Specific time slots within events
- **Attendance Types**: Categories of attendance (present, absent, etc.)

### Authentication
Requires Planning Center Check-Ins app access with appropriate permissions.

## 🔧 Traditional Service API

### Check-In Management

#### Basic Check-In Operations
```csharp
public class CheckInsService
{
    private readonly ICheckInsService _checkInsService;
    
    public CheckInsService(ICheckInsService checkInsService)
    {
        _checkInsService = checkInsService;
    }
    
    // Get a check-in by ID
    public async Task<CheckIn?> GetCheckInAsync(string id)
    {
        return await _checkInsService.GetCheckInAsync(id);
    }
    
    // List check-ins with pagination
    public async Task<IPagedResponse<CheckIn>> GetCheckInsAsync()
    {
        return await _checkInsService.ListCheckInsAsync(new QueryParameters
        {
            PerPage = 25,
            OrderBy = "-created_at"
        });
    }
}
```

#### Create and Update Check-Ins
```csharp
// Create a new check-in
var newCheckIn = await _checkInsService.CreateCheckInAsync(new CheckInCreateRequest
{
    PersonId = "12345",
    EventId = "event123",
    EventTimeId = "time456",
    LocationId = "location789",
    CheckedInAt = DateTime.Now,
    AttendanceTypeId = "present",
    SecurityCode = "ABC123"
});

// Update a check-in
var updatedCheckIn = await _checkInsService.UpdateCheckInAsync("checkin123", new CheckInUpdateRequest
{
    AttendanceTypeId = "late",
    Note = "Arrived 15 minutes late"
});

// Delete a check-in
await _checkInsService.DeleteCheckInAsync("checkin123");
```

#### Check-In Queries
```csharp
// Get check-ins for a specific event
var eventCheckIns = await _checkInsService.ListCheckInsAsync(new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["event_id"] = "event123"
    },
    OrderBy = "checked_in_at"
});

// Get check-ins for a person
var personCheckIns = await _checkInsService.ListCheckInsAsync(new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["person_id"] = "12345"
    },
    OrderBy = "-checked_in_at"
});

// Get check-ins for today
var today = DateTime.Today;
var todayCheckIns = await _checkInsService.ListCheckInsAsync(new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["checked_in_at"] = $"{today:yyyy-MM-dd}..{today.AddDays(1):yyyy-MM-dd}"
    }
});
```

### Event Management

#### Event Operations
```csharp
// Get an event by ID
var checkInEvent = await _checkInsService.GetEventAsync("event123");

// List events
var events = await _checkInsService.ListEventsAsync(new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["frequency"] = "weekly"
    },
    OrderBy = "name"
});

// Create a new event
var newEvent = await _checkInsService.CreateEventAsync(new EventCreateRequest
{
    Name = "Sunday Morning Service",
    Frequency = "weekly",
    Enable = true,
    CheckInStart = TimeSpan.FromMinutes(-30), // 30 minutes before
    CheckInEnd = TimeSpan.FromMinutes(60),    // 60 minutes after start
    LocationId = "location123"
});

// Update an event
var updatedEvent = await _checkInsService.UpdateEventAsync("event123", new EventUpdateRequest
{
    Name = "Sunday Worship Service",
    Enable = true,
    CheckInStart = TimeSpan.FromMinutes(-45)
});
```

### Location Management

#### Location Operations
```csharp
// Get a location by ID
var location = await _checkInsService.GetLocationAsync("location123");

// List locations
var locations = await _checkInsService.ListLocationsAsync(new QueryParameters
{
    OrderBy = "name"
});

// Create a new location
var newLocation = await _checkInsService.CreateLocationAsync(new LocationCreateRequest
{
    Name = "Main Sanctuary",
    Kind = "room",
    Opened = true,
    Questions = "Any allergies or special needs?"
});

// Update a location
var updatedLocation = await _checkInsService.UpdateLocationAsync("location123", new LocationUpdateRequest
{
    Name = "Sanctuary - Main Campus",
    Opened = true
});
```

## 🚀 Fluent API Usage

### Basic Queries
```csharp
public class CheckInsFluentService
{
    private readonly IPlanningCenterClient _client;
    
    public CheckInsFluentService(IPlanningCenterClient client)
    {
        _client = client;
    }
    
    // Get a check-in by ID
    public async Task<CheckIn?> GetCheckInAsync(string id)
    {
        return await _client.CheckIns().GetAsync(id);
    }
    
    // Get first page of check-ins
    public async Task<IPagedResponse<CheckIn>> GetCheckInsPageAsync()
    {
        return await _client.CheckIns().GetPagedAsync(pageSize: 25);
    }
}
```

### Advanced Check-In Filtering
```csharp
// Get today's check-ins
var todayCheckIns = await _client.CheckIns()
    .Where(c => c.CheckedInAt >= DateTime.Today)
    .Where(c => c.CheckedInAt < DateTime.Today.AddDays(1))
    .OrderBy(c => c.CheckedInAt)
    .GetAllAsync();

// Get check-ins for specific event
var eventCheckIns = await _client.CheckIns()
    .Where(c => c.EventId == "event123")
    .Where(c => c.AttendanceTypeId == "present")
    .OrderBy(c => c.CheckedInAt)
    .GetAllAsync();

// Get late arrivals
var lateArrivals = await _client.CheckIns()
    .Where(c => c.AttendanceTypeId == "late")
    .Where(c => c.CheckedInAt >= DateTime.Today)
    .OrderBy(c => c.CheckedInAt)
    .GetPagedAsync(pageSize: 50);
```

### Event and Location Filtering
```csharp
// Get active events
var activeEvents = await _client.CheckIns()
    .Events()
    .Where(e => e.Enable == true)
    .OrderBy(e => e.Name)
    .GetAllAsync();

// Get open locations
var openLocations = await _client.CheckIns()
    .Locations()
    .Where(l => l.Opened == true)
    .OrderBy(l => l.Name)
    .GetAllAsync();
```

## 💡 Common Use Cases

### 1. Attendance Report
```csharp
public async Task<AttendanceReport> GenerateAttendanceReportAsync(string eventId, DateTime date)
{
    var startDate = date.Date;
    var endDate = startDate.AddDays(1);
    
    var checkIns = await _client.CheckIns()
        .Where(c => c.EventId == eventId)
        .Where(c => c.CheckedInAt >= startDate)
        .Where(c => c.CheckedInAt < endDate)
        .GetAllAsync();
    
    return new AttendanceReport
    {
        EventId = eventId,
        Date = date,
        TotalCheckIns = checkIns.Count,
        PresentCount = checkIns.Count(c => c.AttendanceTypeId == "present"),
        LateCount = checkIns.Count(c => c.AttendanceTypeId == "late")
    };
}
```

### 2. Real-Time Check-In Dashboard
```csharp
public async Task<CheckInDashboard> GetCheckInDashboardAsync()
{
    var today = DateTime.Today;
    var todayCheckIns = await _client.CheckIns()
        .Where(c => c.CheckedInAt >= today)
        .Where(c => c.CheckedInAt < today.AddDays(1))
        .GetAllAsync();
    
    return new CheckInDashboard
    {
        TotalToday = todayCheckIns.Count,
        LastHour = todayCheckIns.Count(c => c.CheckedInAt >= DateTime.Now.AddHours(-1)),
        ByLocation = todayCheckIns.GroupBy(c => c.LocationId)
            .ToDictionary(g => g.Key, g => g.Count())
    };
}
```

## 🎯 Best Practices

1. **Filter by Date**: Always use date ranges for check-in queries
2. **Monitor Real-Time**: Use streaming for live attendance tracking
3. **Validate Security Codes**: Ensure proper security code generation
4. **Handle Time Zones**: Be aware of time zone considerations
5. **Track Attendance Types**: Use appropriate attendance categories

This Check-Ins module provides essential attendance tracking capabilities for events and services.