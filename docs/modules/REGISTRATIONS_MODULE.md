# Registrations Module Documentation

The Registrations module provides comprehensive event registration and attendee management capabilities for Planning Center. This module handles all registration-related functionality including event registrations, attendees, and signup management.

## 📋 Module Overview

### Key Entities
- **Registrations**: Event registration records with attendee information
- **Attendees**: Individual people registered for events
- **Events**: Registerable events and activities
- **Signups**: Registration forms and processes
- **Categories**: Event categorization and organization

### Authentication
Requires Planning Center Registrations app access with appropriate permissions.

## 🔧 Traditional Service API

### Registration Management

#### Basic Registration Operations
```csharp
public class RegistrationsService
{
    private readonly IRegistrationsService _registrationsService;
    
    public RegistrationsService(IRegistrationsService registrationsService)
    {
        _registrationsService = registrationsService;
    }
    
    // Get a registration by ID
    public async Task<Registration?> GetRegistrationAsync(string id)
    {
        return await _registrationsService.GetRegistrationAsync(id);
    }
    
    // List registrations with pagination
    public async Task<IPagedResponse<Registration>> GetRegistrationsAsync()
    {
        return await _registrationsService.ListRegistrationsAsync(new QueryParameters
        {
            PerPage = 25,
            OrderBy = "-created_at"
        });
    }
}
```

#### Create and Update Registrations
```csharp
// Create a new registration
var newRegistration = await _registrationsService.CreateRegistrationAsync(new RegistrationCreateRequest
{
    EventId = "event123",
    PersonId = "12345",
    TotalPaidCents = 5000, // $50.00
    CompletedAt = DateTime.Now
});

// Update a registration
var updatedRegistration = await _registrationsService.UpdateRegistrationAsync("reg123", new RegistrationUpdateRequest
{
    TotalPaidCents = 7500, // Updated to $75.00
    Note = "Payment updated"
});
```

### Attendee Management

#### Attendee Operations
```csharp
// Get an attendee by ID
var attendee = await _registrationsService.GetAttendeeAsync("attendee123");

// List attendees for an event
var eventAttendees = await _registrationsService.ListAttendeesAsync(new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["event_id"] = "event123"
    },
    OrderBy = "last_name"
});

// Create a new attendee
var newAttendee = await _registrationsService.CreateAttendeeAsync(new AttendeeCreateRequest
{
    EventId = "event123",
    FirstName = "John",
    LastName = "Doe",
    Email = "john.doe@example.com",
    Status = "registered"
});
```

## 🚀 Fluent API Usage

### Basic Queries
```csharp
public class RegistrationsFluentService
{
    private readonly IPlanningCenterClient _client;
    
    public RegistrationsFluentService(IPlanningCenterClient client)
    {
        _client = client;
    }
    
    // Get registrations for an event
    public async Task<IReadOnlyList<Registration>> GetEventRegistrationsAsync(string eventId)
    {
        return await _client.Registrations()
            .Where(r => r.EventId == eventId)
            .OrderBy(r => r.CreatedAt)
            .GetAllAsync();
    }
}
```

### Advanced Registration Filtering
```csharp
// Get completed registrations
var completedRegistrations = await _client.Registrations()
    .Where(r => r.CompletedAt.HasValue)
    .Where(r => r.TotalPaidCents > 0)
    .OrderByDescending(r => r.CompletedAt)
    .GetAllAsync();

// Get pending registrations
var pendingRegistrations = await _client.Registrations()
    .Where(r => !r.CompletedAt.HasValue)
    .OrderBy(r => r.CreatedAt)
    .GetPagedAsync(pageSize: 50);
```

## 💡 Common Use Cases

### 1. Event Registration Report
```csharp
public async Task<EventRegistrationReport> GenerateEventReportAsync(string eventId)
{
    var registrations = await _client.Registrations()
        .Where(r => r.EventId == eventId)
        .GetAllAsync();
    
    var attendees = await _client.Registrations()
        .Attendees()
        .Where(a => a.EventId == eventId)
        .GetAllAsync();
    
    return new EventRegistrationReport
    {
        EventId = eventId,
        TotalRegistrations = registrations.Count,
        CompletedRegistrations = registrations.Count(r => r.CompletedAt.HasValue),
        TotalAttendees = attendees.Count,
        TotalRevenue = registrations.Sum(r => r.TotalPaidCents) / 100.0m
    };
}
```

### 2. Registration Status Dashboard
```csharp
public async Task<RegistrationDashboard> GetRegistrationDashboardAsync()
{
    var allRegistrations = await _client.Registrations()
        .Where(r => r.CreatedAt >= DateTime.Today.AddDays(-30))
        .GetAllAsync();
    
    return new RegistrationDashboard
    {
        TotalRegistrations = allRegistrations.Count,
        CompletedRegistrations = allRegistrations.Count(r => r.CompletedAt.HasValue),
        PendingRegistrations = allRegistrations.Count(r => !r.CompletedAt.HasValue),
        TotalRevenue = allRegistrations.Sum(r => r.TotalPaidCents) / 100.0m
    };
}
```

## 🎯 Best Practices

1. **Track Payment Status**: Monitor completed vs pending registrations
2. **Validate Email Addresses**: Ensure proper contact information
3. **Handle Cancellations**: Properly process registration cancellations
4. **Monitor Capacity**: Track event capacity and availability
5. **Generate Reports**: Regular reporting on registration metrics

This Registrations module provides essential event registration and attendee management capabilities.