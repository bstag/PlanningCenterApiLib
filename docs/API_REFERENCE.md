# Planning Center API SDK - Complete API Reference

This document provides comprehensive reference documentation for all services in the Planning Center API SDK.

## Table of Contents

1. [Authentication](#authentication)
2. [Service Interfaces](#service-interfaces)
3. [Common Patterns](#common-patterns)
4. [People Service](#people-service)
5. [Giving Service](#giving-service)
6. [Calendar Service](#calendar-service)
7. [Check-Ins Service](#check-ins-service)
8. [Groups Service](#groups-service)
9. [Registrations Service](#registrations-service)
10. [Services Service](#services-service)
11. [Publishing Service](#publishing-service)
12. [Webhooks Service](#webhooks-service)

## Authentication

The SDK supports three authentication methods:

### Personal Access Token (PAT)
Recommended for server-side applications and personal scripts.

```csharp
services.AddPlanningCenterApiClientWithPAT("your-app-id:your-secret");
```

### OAuth 2.0
For user-facing applications requiring user consent.

```csharp
services.AddPlanningCenterApiClient("client-id", "client-secret");
```

### Access Token
For simple token-based authentication.

```csharp
services.AddPlanningCenterApiClientWithToken("your-access-token");
```

## Service Interfaces

All services follow consistent patterns:

- **CRUD Operations**: Create, Read, Update, Delete
- **Pagination**: Manual and automatic pagination support
- **Streaming**: Memory-efficient data processing
- **Error Handling**: Comprehensive exception types
- **Logging**: Structured logging throughout

## Common Patterns

### Pagination

All list operations support pagination:

```csharp
// Manual pagination
var page = await service.ListAsync(new QueryParameters { PerPage = 25 });

// Automatic pagination (loads all data)
var allItems = await service.GetAllAsync();

// Streaming (memory-efficient)
await foreach (var item in service.StreamAsync())
{
    // Process item
}
```

### Query Parameters

```csharp
var parameters = new QueryParameters
{
    Where = new Dictionary<string, object> { ["status"] = "active" },
    Include = new[] { "addresses", "emails" },
    Order = "last_name",
    PerPage = 50,
    Offset = 0
};
```

### Error Handling

```csharp
try
{
    var result = await service.GetAsync("123");
}
catch (PlanningCenterApiNotFoundException ex)
{
    // Resource not found
}
catch (PlanningCenterApiValidationException ex)
{
    // Validation errors
}
catch (PlanningCenterApiException ex)
{
    // General API errors
}
```

## People Service

Manages people, households, contact information, workflows, and forms.

### Core Operations

```csharp
// Get current user
var me = await peopleService.GetMeAsync();

// CRUD operations
var person = await peopleService.GetAsync("123");
var people = await peopleService.ListAsync();
var created = await peopleService.CreateAsync(new PersonCreateRequest { ... });
var updated = await peopleService.UpdateAsync("123", new PersonUpdateRequest { ... });
await peopleService.DeleteAsync("123");
```

### Contact Information

```csharp
// Email management
var email = await peopleService.AddEmailAsync("person-id", new EmailCreateRequest
{
    Address = "john@example.com",
    Location = "Home",
    Primary = true
});

// Phone number management
var phone = await peopleService.AddPhoneNumberAsync("person-id", new PhoneNumberCreateRequest
{
    Number = "555-123-4567",
    Location = "Mobile",
    Primary = true
});

// Address management
var address = await peopleService.AddAddressAsync("person-id", new AddressCreateRequest
{
    Street = "123 Main St",
    City = "Anytown",
    State = "CA",
    Zip = "12345"
});
```

### Workflows

```csharp
// List workflows
var workflows = await peopleService.ListWorkflowsAsync();

// Get workflow
var workflow = await peopleService.GetWorkflowAsync("workflow-id");

// Manage workflow cards
var cards = await peopleService.ListWorkflowCardsAsync("workflow-id");
var card = await peopleService.CreateWorkflowCardAsync("workflow-id", new WorkflowCardCreateRequest { ... });
```

### Forms

```csharp
// List forms
var forms = await peopleService.ListFormsAsync();

// Get form submissions
var submissions = await peopleService.ListFormSubmissionsAsync("form-id");

// Submit form
var submission = await peopleService.SubmitFormAsync("form-id", new FormSubmitRequest { ... });
```

### Households

```csharp
// Household management
var households = await peopleService.ListHouseholdsAsync();
var household = await peopleService.CreateHouseholdAsync(new HouseholdCreateRequest { ... });
var peopleInHousehold = await peopleService.ListPeopleInHouseholdAsync("household-id");
```

### Lists

```csharp
// People lists
var lists = await peopleService.ListPeopleListsAsync();
var list = await peopleService.CreatePeopleListAsync(new PeopleListCreateRequest { ... });
var peopleInList = await peopleService.ListPeopleInListAsync("list-id");

// Add/remove people from lists
await peopleService.AddPersonToListAsync("list-id", new ListMemberCreateRequest { ... });
await peopleService.RemovePersonFromListAsync("list-id", "person-id");
```

## Giving Service

Manages donations, pledges, funds, batches, and payment processing.

### Donations

```csharp
// CRUD operations
var donation = await givingService.GetDonationAsync("donation-id");
var donations = await givingService.ListDonationsAsync();
var created = await givingService.CreateDonationAsync(new DonationCreateRequest { ... });
var updated = await givingService.UpdateDonationAsync("donation-id", new DonationUpdateRequest { ... });
await givingService.DeleteDonationAsync("donation-id");
```

### Funds

```csharp
// Fund management
var fund = await givingService.GetFundAsync("fund-id");
var funds = await givingService.ListFundsAsync();
var created = await givingService.CreateFundAsync(new FundCreateRequest { ... });
var updated = await givingService.UpdateFundAsync("fund-id", new FundUpdateRequest { ... });
```

### Batches

```csharp
// Batch management
var batch = await givingService.GetBatchAsync("batch-id");
var batches = await givingService.ListBatchesAsync();
var created = await givingService.CreateBatchAsync(new BatchCreateRequest { ... });
var updated = await givingService.UpdateBatchAsync("batch-id", new BatchUpdateRequest { ... });

// Commit batch
var committed = await givingService.CommitBatchAsync("batch-id");
```

### Pledges

```csharp
// Pledge management
var pledge = await givingService.GetPledgeAsync("pledge-id");
var pledges = await givingService.ListPledgesAsync();
var created = await givingService.CreatePledgeAsync(new PledgeCreateRequest { ... });
var updated = await givingService.UpdatePledgeAsync("pledge-id", new PledgeUpdateRequest { ... });
```

### Recurring Donations

```csharp
// Recurring donation management
var recurring = await givingService.GetRecurringDonationAsync("recurring-id");
var recurringDonations = await givingService.ListRecurringDonationsAsync();
var created = await givingService.CreateRecurringDonationAsync(new RecurringDonationCreateRequest { ... });
var updated = await givingService.UpdateRecurringDonationAsync("recurring-id", new RecurringDonationUpdateRequest { ... });
```

### Refunds

```csharp
// Refund management
var refund = await givingService.GetRefundAsync("donation-id");
var refunds = await givingService.ListRefundsAsync();
var issued = await givingService.IssueRefundAsync("donation-id", new RefundCreateRequest { ... });
```

### Payment Sources

```csharp
// Payment source management
var paymentSource = await givingService.GetPaymentSourceAsync("source-id");
var paymentSources = await givingService.ListPaymentSourcesAsync();
```

### Person-Specific Operations

```csharp
// Get person from giving module
var person = await givingService.GetPersonAsync("person-id");

// Get giving data for specific person
var donations = await givingService.GetDonationsForPersonAsync("person-id");
var pledges = await givingService.GetPledgesForPersonAsync("person-id");
var recurringDonations = await givingService.GetRecurringDonationsForPersonAsync("person-id");
```

### Reporting

```csharp
// Total giving amount
var total = await givingService.GetTotalGivingAsync(
    DateTime.Now.AddYears(-1), 
    DateTime.Now, 
    fundId: "optional-fund-id");
```

## Calendar Service

Manages calendar events, resources, and scheduling.

### Events

```csharp
// CRUD operations
var calendarEvent = await calendarService.GetEventAsync("event-id");
var events = await calendarService.ListEventsAsync();
var created = await calendarService.CreateEventAsync(new EventCreateRequest { ... });
var updated = await calendarService.UpdateEventAsync("event-id", new EventUpdateRequest { ... });
await calendarService.DeleteEventAsync("event-id");

// Date range queries
var eventsInRange = await calendarService.ListEventsByDateRangeAsync(
    DateTime.Now, 
    DateTime.Now.AddDays(30));
```

### Resources

```csharp
// Resource management
var resource = await calendarService.GetResourceAsync("resource-id");
var resources = await calendarService.ListResourcesAsync();
var created = await calendarService.CreateResourceAsync(new ResourceCreateRequest { ... });
var updated = await calendarService.UpdateResourceAsync("resource-id", new ResourceUpdateRequest { ... });
await calendarService.DeleteResourceAsync("resource-id");
```

## Check-Ins Service

Manages event check-ins, locations, and attendance tracking.

### Check-Ins

```csharp
// CRUD operations
var checkIn = await checkInsService.GetCheckInAsync("checkin-id");
var checkIns = await checkInsService.ListCheckInsAsync();
var created = await checkInsService.CreateCheckInAsync(new CheckInCreateRequest { ... });
var updated = await checkInsService.UpdateCheckInAsync("checkin-id", new CheckInUpdateRequest { ... });
await checkInsService.DeleteCheckInAsync("checkin-id");

// Check-out
await checkInsService.CheckOutAsync("checkin-id");
```

### Events

```csharp
// Event management
var checkInEvent = await checkInsService.GetEventAsync("event-id");
var events = await checkInsService.ListEventsAsync();
var created = await checkInsService.CreateEventAsync(new EventCreateRequest { ... });
var updated = await checkInsService.UpdateEventAsync("event-id", new EventUpdateRequest { ... });
await checkInsService.DeleteEventAsync("event-id");
```

### Locations

```csharp
// Location management
var location = await checkInsService.GetLocationAsync("location-id");
var locations = await checkInsService.ListLocationsAsync();
var created = await checkInsService.CreateLocationAsync(new LocationCreateRequest { ... });
var updated = await checkInsService.UpdateLocationAsync("location-id", new LocationUpdateRequest { ... });
await checkInsService.DeleteLocationAsync("location-id");
```

### Attendance

```csharp
// Attendance tracking
var attendance = await checkInsService.GetAttendanceAsync("attendance-id");
var attendanceRecords = await checkInsService.ListAttendanceAsync();
```

## Groups Service

Manages small groups, memberships, and group management.

### Groups

```csharp
// CRUD operations
var group = await groupsService.GetGroupAsync("group-id");
var groups = await groupsService.ListGroupsAsync();
var created = await groupsService.CreateGroupAsync(new GroupCreateRequest { ... });
var updated = await groupsService.UpdateGroupAsync("group-id", new GroupUpdateRequest { ... });
await groupsService.DeleteGroupAsync("group-id");
```

### Group Types

```csharp
// Group type management
var groupType = await groupsService.GetGroupTypeAsync("type-id");
var groupTypes = await groupsService.ListGroupTypesAsync();
var created = await groupsService.CreateGroupTypeAsync(new GroupTypeCreateRequest { ... });
var updated = await groupsService.UpdateGroupTypeAsync("type-id", new GroupTypeUpdateRequest { ... });
await groupsService.DeleteGroupTypeAsync("type-id");
```

### Memberships

```csharp
// Membership management
var membership = await groupsService.GetMembershipAsync("membership-id");
var memberships = await groupsService.ListMembershipsAsync();
var created = await groupsService.CreateMembershipAsync(new MembershipCreateRequest { ... });
var updated = await groupsService.UpdateMembershipAsync("membership-id", new MembershipUpdateRequest { ... });
await groupsService.DeleteMembershipAsync("membership-id");

// Group-specific memberships
var groupMemberships = await groupsService.ListGroupMembershipsAsync("group-id");
```

### Events

```csharp
// Group event management
var groupEvent = await groupsService.GetGroupEventAsync("event-id");
var events = await groupsService.ListGroupEventsAsync();
var created = await groupsService.CreateGroupEventAsync(new GroupEventCreateRequest { ... });
var updated = await groupsService.UpdateGroupEventAsync("event-id", new GroupEventUpdateRequest { ... });
await groupsService.DeleteGroupEventAsync("event-id");
```

## Registrations Service

Manages event registrations, attendees, and signups.

### Registrations

```csharp
// CRUD operations
var registration = await registrationsService.GetRegistrationAsync("registration-id");
var registrations = await registrationsService.ListRegistrationsAsync();
var created = await registrationsService.CreateRegistrationAsync(new RegistrationCreateRequest { ... });
var updated = await registrationsService.UpdateRegistrationAsync("registration-id", new RegistrationUpdateRequest { ... });
await registrationsService.DeleteRegistrationAsync("registration-id");
```

### Attendees

```csharp
// Attendee management
var attendee = await registrationsService.GetAttendeeAsync("attendee-id");
var attendees = await registrationsService.ListAttendeesAsync();
var created = await registrationsService.CreateAttendeeAsync(new AttendeeCreateRequest { ... });
var updated = await registrationsService.UpdateAttendeeAsync("attendee-id", new AttendeeUpdateRequest { ... });
await registrationsService.DeleteAttendeeAsync("attendee-id");
```

### Signups

```csharp
// Signup management
var signup = await registrationsService.GetSignupAsync("signup-id");
var signups = await registrationsService.ListSignupsAsync();
var created = await registrationsService.CreateSignupAsync(new SignupCreateRequest { ... });
var updated = await registrationsService.UpdateSignupAsync("signup-id", new SignupUpdateRequest { ... });
await registrationsService.DeleteSignupAsync("signup-id");
```

### Categories

```csharp
// Category management
var category = await registrationsService.GetCategoryAsync("category-id");
var categories = await registrationsService.ListCategoriesAsync();
var created = await registrationsService.CreateCategoryAsync(new CategoryCreateRequest { ... });
var updated = await registrationsService.UpdateCategoryAsync("category-id", new CategoryUpdateRequest { ... });
await registrationsService.DeleteCategoryAsync("category-id");
```

### Emergency Contacts

```csharp
// Emergency contact management
var emergencyContact = await registrationsService.GetEmergencyContactAsync("contact-id");
var emergencyContacts = await registrationsService.ListEmergencyContactsAsync();
var created = await registrationsService.CreateEmergencyContactAsync(new EmergencyContactCreateRequest { ... });
var updated = await registrationsService.UpdateEmergencyContactAsync("contact-id", new EmergencyContactUpdateRequest { ... });
await registrationsService.DeleteEmergencyContactAsync("contact-id");
```

### Counting and Statistics

```csharp
// Registration counts
var registrationCount = await registrationsService.GetRegistrationCountAsync("event-id");
var waitlistCount = await registrationsService.GetWaitlistCountAsync("event-id");
var capacity = await registrationsService.GetCapacityAsync("event-id");
```

## Services Service

Manages worship services, plans, items, and scheduling.

### Plans

```csharp
// CRUD operations
var plan = await servicesService.GetPlanAsync("plan-id");
var plans = await servicesService.ListPlansAsync();
var created = await servicesService.CreatePlanAsync(new PlanCreateRequest { ... });
var updated = await servicesService.UpdatePlanAsync("plan-id", new PlanUpdateRequest { ... });
await servicesService.DeletePlanAsync("plan-id");
```

### Items

```csharp
// Plan item management
var item = await servicesService.GetItemAsync("item-id");
var items = await servicesService.ListItemsAsync();
var created = await servicesService.CreateItemAsync(new ItemCreateRequest { ... });
var updated = await servicesService.UpdateItemAsync("item-id", new ItemUpdateRequest { ... });
await servicesService.DeleteItemAsync("item-id");

// Plan-specific items
var planItems = await servicesService.ListPlanItemsAsync("plan-id");
```

### Songs

```csharp
// Song management
var song = await servicesService.GetSongAsync("song-id");
var songs = await servicesService.ListSongsAsync();
var created = await servicesService.CreateSongAsync(new SongCreateRequest { ... });
var updated = await servicesService.UpdateSongAsync("song-id", new SongUpdateRequest { ... });
await servicesService.DeleteSongAsync("song-id");
```

### Service Types

```csharp
// Service type management
var serviceType = await servicesService.GetServiceTypeAsync("type-id");
var serviceTypes = await servicesService.ListServiceTypesAsync();
var created = await servicesService.CreateServiceTypeAsync(new ServiceTypeCreateRequest { ... });
var updated = await servicesService.UpdateServiceTypeAsync("type-id", new ServiceTypeUpdateRequest { ... });
await servicesService.DeleteServiceTypeAsync("type-id");
```

## Publishing Service

Manages media content, episodes, series, and speakers.

### Episodes

```csharp
// CRUD operations
var episode = await publishingService.GetEpisodeAsync("episode-id");
var episodes = await publishingService.ListEpisodesAsync();
var created = await publishingService.CreateEpisodeAsync(new EpisodeCreateRequest { ... });
var updated = await publishingService.UpdateEpisodeAsync("episode-id", new EpisodeUpdateRequest { ... });
await publishingService.DeleteEpisodeAsync("episode-id");

// Publishing
var published = await publishingService.PublishEpisodeAsync("episode-id");
var unpublished = await publishingService.UnpublishEpisodeAsync("episode-id");
```

### Series

```csharp
// Series management
var series = await publishingService.GetSeriesAsync("series-id");
var seriesList = await publishingService.ListSeriesAsync();
var created = await publishingService.CreateSeriesAsync(new SeriesCreateRequest { ... });
var updated = await publishingService.UpdateSeriesAsync("series-id", new SeriesUpdateRequest { ... });
await publishingService.DeleteSeriesAsync("series-id");
```

### Speakers

```csharp
// Speaker management
var speaker = await publishingService.GetSpeakerAsync("speaker-id");
var speakers = await publishingService.ListSpeakersAsync();
var created = await publishingService.CreateSpeakerAsync(new SpeakerCreateRequest { ... });
var updated = await publishingService.UpdateSpeakerAsync("speaker-id", new SpeakerUpdateRequest { ... });
await publishingService.DeleteSpeakerAsync("speaker-id");
```

### Media

```csharp
// Media management
var media = await publishingService.GetMediaAsync("media-id");
var mediaList = await publishingService.ListMediaAsync("episode-id");
var uploaded = await publishingService.UploadMediaAsync("episode-id", new MediaUploadRequest { ... });
var updated = await publishingService.UpdateMediaAsync("media-id", new MediaUpdateRequest { ... });
await publishingService.DeleteMediaAsync("media-id");
```

### Speakerships

```csharp
// Speaker-episode relationships
var speakerships = await publishingService.ListSpeakershipsAsync("episode-id");
var speakerEpisodes = await publishingService.ListSpeakerEpisodesAsync("speaker-id");
var added = await publishingService.AddSpeakerToEpisodeAsync("episode-id", "speaker-id");
await publishingService.DeleteSpeakershipAsync("speakership-id");
```

## Webhooks Service

Manages webhook subscriptions, events, and delivery management.

### Subscriptions

```csharp
// CRUD operations
var subscription = await webhooksService.GetSubscriptionAsync("subscription-id");
var subscriptions = await webhooksService.ListSubscriptionsAsync();
var created = await webhooksService.CreateSubscriptionAsync(new WebhookSubscriptionCreateRequest { ... });
var updated = await webhooksService.UpdateSubscriptionAsync("subscription-id", new WebhookSubscriptionUpdateRequest { ... });
await webhooksService.DeleteSubscriptionAsync("subscription-id");
```

### Events

```csharp
// Event management
var webhookEvent = await webhooksService.GetEventAsync("event-id");
var events = await webhooksService.ListEventsAsync("subscription-id");
```

### Available Events

```csharp
// List available webhook events
var availableEvents = await webhooksService.ListAvailableEventsAsync();
var moduleEvents = await webhooksService.ListAvailableEventsByModuleAsync("people");
```

### Event Validation

```csharp
// Validate webhook signatures
var isValid = await webhooksService.ValidateEventSignatureAsync(
    requestBody, 
    signature, 
    secret);
```

### Subscription Management

```csharp
// Test subscription
var testResult = await webhooksService.TestSubscriptionAsync("subscription-id");

// Subscription analytics
var analytics = await webhooksService.GetSubscriptionAnalyticsAsync("subscription-id", new AnalyticsRequest { ... });
```

### Bulk Operations

```csharp
// Bulk subscription management
var bulkResult = await webhooksService.BulkCreateSubscriptionsAsync(new[] { ... });
var bulkUpdateResult = await webhooksService.BulkUpdateSubscriptionsAsync(new[] { ... });
var bulkDeleteResult = await webhooksService.BulkDeleteSubscriptionsAsync(new[] { "sub1", "sub2" });
```

## Error Handling

The SDK provides comprehensive error handling with specific exception types:

```csharp
try
{
    var result = await service.GetAsync("123");
}
catch (PlanningCenterApiNotFoundException ex)
{
    // 404 - Resource not found
    Console.WriteLine($"Resource not found: {ex.Message}");
}
catch (PlanningCenterApiValidationException ex)
{
    // 400 - Validation errors
    Console.WriteLine($"Validation errors: {string.Join(", ", ex.Errors)}");
}
catch (PlanningCenterApiAuthenticationException ex)
{
    // 401 - Authentication failed
    Console.WriteLine($"Authentication failed: {ex.Message}");
}
catch (PlanningCenterApiAuthorizationException ex)
{
    // 403 - Authorization failed
    Console.WriteLine($"Authorization failed: {ex.Message}");
}
catch (PlanningCenterApiRateLimitException ex)
{
    // 429 - Rate limit exceeded
    Console.WriteLine($"Rate limit exceeded. Retry after: {ex.RetryAfter}");
}
catch (PlanningCenterApiServerException ex)
{
    // 500+ - Server errors
    Console.WriteLine($"Server error: {ex.Message}");
}
catch (PlanningCenterApiException ex)
{
    // General API errors
    Console.WriteLine($"API error: {ex.Message}");
}
```

## Best Practices

1. **Use pagination helpers** for large datasets
2. **Implement proper error handling** for production applications
3. **Use streaming APIs** for memory-efficient processing
4. **Configure authentication** securely
5. **Enable logging** for debugging and monitoring
6. **Use cancellation tokens** for long-running operations
7. **Handle rate limiting** gracefully
8. **Cache responses** when appropriate
9. **Use dependency injection** for testability
10. **Follow async/await patterns** consistently

This completes the comprehensive API reference for the Planning Center SDK. Each service provides full CRUD operations, pagination support, and consistent error handling patterns.