# Groups Module Specification

## Overview

The Groups module manages small groups and community organizations in Planning Center. It provides comprehensive functionality for group lifecycle management, membership tracking, event scheduling, enrollment processing, and group communication.

## Core Entities

### Group
The primary entity representing a small group or community organization.

**Key Attributes:**
- `id` - Unique identifier
- `name` - Group name
- `description` - Group description
- `archived_at` - Archive timestamp (null if active)
- `contact_email` - Group contact email
- `schedule` - Meeting schedule description
- `location_type_preference` - Preferred location type
- `virtual_location_url` - Virtual meeting URL
- `events_visibility` - Event visibility settings
- `memberships_count` - Current member count
- `members_are_confidential` - Privacy setting
- `leaders_can_search_people_database` - Permission setting
- `can_create_conversation` - Communication capability
- `chat_enabled` - Chat feature status
- `public_church_center_web_url` - Public URL
- `header_image` - Group header image
- `widget_status` - Widget configuration
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `group_type` - Group category
- `memberships` - Group memberships
- `events` - Group events
- `enrollments` - Enrollment requests
- `tags` - Group tags
- `location` - Meeting location
- `resources` - Group resources

### GroupType
Represents a category or type of group.

**Key Attributes:**
- `id` - Unique identifier
- `name` - Type name
- `description` - Type description
- `church_center_visible` - Public visibility
- `color` - Display color
- `position` - Sort order
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `groups` - Groups of this type

### Membership
Represents a person's participation in a group.

**Key Attributes:**
- `id` - Unique identifier
- `role` - Member role (member, leader, admin)
- `joined_at` - Join date
- `avatar_url` - Member avatar
- `color_identifier` - Color coding
- `first_name` - Member first name
- `last_name` - Member last name
- `email_address` - Member email
- `phone_number` - Member phone
- `account_center_identifier` - Account ID
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `group` - Associated group
- `person` - Associated person

### Event
Represents a group meeting or event.

**Key Attributes:**
- `id` - Unique identifier
- `name` - Event name
- `description` - Event description
- `location` - Event location
- `starts_at` - Start time
- `ends_at` - End time
- `repeating` - Recurring event flag
- `multi_day` - Multi-day event flag
- `virtual_location_url` - Virtual meeting URL
- `attendance_requests_enabled` - RSVP capability
- `automated_reminder_enabled` - Reminder settings
- `canceled` - Cancellation status
- `canceled_at` - Cancellation timestamp
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `group` - Associated group
- `attendances` - Event attendance records
- `event_notes` - Event notes

### Attendance
Represents individual attendance at a group event.

**Key Attributes:**
- `id` - Unique identifier
- `attended` - Attendance status
- `role` - Person's role at event
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `event` - Associated event
- `person` - Associated person

### Enrollment
Represents a request to join a group.

**Key Attributes:**
- `id` - Unique identifier
- `status` - Enrollment status (pending, approved, declined)
- `first_name` - Applicant first name
- `last_name` - Applicant last name
- `email` - Applicant email
- `phone_number` - Applicant phone
- `street` - Address street
- `city` - Address city
- `state` - Address state
- `zip` - Address zip code
- `created_at` - Application timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `group` - Target group
- `person` - Associated person (if exists)

### GroupApplication
Represents the application form for group enrollment.

**Key Attributes:**
- `id` - Unique identifier
- `archived_at` - Archive timestamp
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `group` - Associated group

### Location
Represents a meeting location for groups.

**Key Attributes:**
- `id` - Unique identifier
- `name` - Location name
- `description` - Location description
- `display_preference` - Display settings
- `full_formatted_address` - Complete address
- `latitude` - Geographic latitude
- `longitude` - Geographic longitude
- `radius` - Location radius
- `strategy` - Location strategy
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `groups` - Groups using this location

### Resource
Represents a resource available to groups.

**Key Attributes:**
- `id` - Unique identifier
- `name` - Resource name
- `description` - Resource description
- `resource_type` - Type of resource
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `groups` - Groups with access to this resource

### Tag
Represents a tag for categorizing groups.

**Key Attributes:**
- `id` - Unique identifier
- `name` - Tag name
- `color` - Tag color
- `position` - Sort order
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `tag_group` - Tag category
- `groups` - Tagged groups

### TagGroup
Represents a category for organizing tags.

**Key Attributes:**
- `id` - Unique identifier
- `name` - Category name
- `position` - Sort order
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `tags` - Tags in this category

## API Endpoints

### Group Management
- `GET /groups/v2/groups` - List all groups
- `GET /groups/v2/groups/{id}` - Get specific group
- `POST /groups/v2/groups` - Create new group
- `PATCH /groups/v2/groups/{id}` - Update group
- `DELETE /groups/v2/groups/{id}` - Delete group

### Membership Management
- `GET /groups/v2/groups/{group_id}/memberships` - List group members
- `GET /groups/v2/memberships/{id}` - Get specific membership
- `POST /groups/v2/groups/{group_id}/memberships` - Add member
- `PATCH /groups/v2/memberships/{id}` - Update membership
- `DELETE /groups/v2/memberships/{id}` - Remove member

### Event Management
- `GET /groups/v2/groups/{group_id}/events` - List group events
- `GET /groups/v2/events/{id}` - Get specific event
- `POST /groups/v2/groups/{group_id}/events` - Create event
- `PATCH /groups/v2/events/{id}` - Update event
- `DELETE /groups/v2/events/{id}` - Delete event

### Attendance Tracking
- `GET /groups/v2/events/{event_id}/attendances` - List event attendance
- `POST /groups/v2/events/{event_id}/attendances` - Record attendance
- `PATCH /groups/v2/attendances/{id}` - Update attendance
- `DELETE /groups/v2/attendances/{id}` - Remove attendance

### Enrollment Management
- `GET /groups/v2/groups/{group_id}/enrollments` - List enrollment requests
- `GET /groups/v2/enrollments/{id}` - Get specific enrollment
- `POST /groups/v2/groups/{group_id}/enrollments` - Submit enrollment
- `PATCH /groups/v2/enrollments/{id}` - Update enrollment status

### Group Types and Organization
- `GET /groups/v2/group_types` - List group types
- `POST /groups/v2/group_types` - Create group type
- `GET /groups/v2/tags` - List tags
- `POST /groups/v2/tags` - Create tag
- `GET /groups/v2/tag_groups` - List tag groups

## Query Parameters

### Include Parameters
- `group_type` - Include group type information
- `memberships` - Include group memberships
- `events` - Include group events
- `location` - Include location information
- `tags` - Include group tags

### Filtering
- `where[name]` - Filter by group name
- `where[group_type_id]` - Filter by group type
- `where[archived_at]` - Filter by archive status
- `where[created_at]` - Filter by creation date
- `where[location_id]` - Filter by location

### Sorting
- `order=name` - Sort by group name
- `order=created_at` - Sort by creation date
- `order=memberships_count` - Sort by member count
- `order=-created_at` - Sort by creation date (descending)

## Service Interface

```csharp
public interface IGroupsService
{
    // Group management
    Task<Group> GetGroupAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Group>> ListGroupsAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Group> CreateGroupAsync(GroupCreateRequest request, CancellationToken cancellationToken = default);
    Task<Group> UpdateGroupAsync(string id, GroupUpdateRequest request, CancellationToken cancellationToken = default);
    Task DeleteGroupAsync(string id, CancellationToken cancellationToken = default);
    
    // Membership management
    Task<Membership> GetMembershipAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Membership>> ListMembershipsAsync(string groupId, QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Membership> AddMemberAsync(string groupId, MembershipCreateRequest request, CancellationToken cancellationToken = default);
    Task<Membership> UpdateMembershipAsync(string id, MembershipUpdateRequest request, CancellationToken cancellationToken = default);
    Task DeleteMembershipAsync(string id, CancellationToken cancellationToken = default);
    
    // Event management
    Task<Event> GetEventAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Event>> ListEventsAsync(string groupId, QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Event> CreateEventAsync(string groupId, EventCreateRequest request, CancellationToken cancellationToken = default);
    Task<Event> UpdateEventAsync(string id, EventUpdateRequest request, CancellationToken cancellationToken = default);
    Task DeleteEventAsync(string id, CancellationToken cancellationToken = default);
    
    // Attendance tracking
    Task<IPagedResponse<Attendance>> ListAttendanceAsync(string eventId, QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Attendance> RecordAttendanceAsync(string eventId, AttendanceCreateRequest request, CancellationToken cancellationToken = default);
    Task<Attendance> UpdateAttendanceAsync(string id, AttendanceUpdateRequest request, CancellationToken cancellationToken = default);
    Task DeleteAttendanceAsync(string id, CancellationToken cancellationToken = default);
    
    // Enrollment management
    Task<Enrollment> GetEnrollmentAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Enrollment>> ListEnrollmentsAsync(string groupId, QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Enrollment> SubmitEnrollmentAsync(string groupId, EnrollmentCreateRequest request, CancellationToken cancellationToken = default);
    Task<Enrollment> UpdateEnrollmentAsync(string id, EnrollmentUpdateRequest request, CancellationToken cancellationToken = default);
    Task<Enrollment> ApproveEnrollmentAsync(string id, CancellationToken cancellationToken = default);
    Task<Enrollment> DeclineEnrollmentAsync(string id, string reason = null, CancellationToken cancellationToken = default);
    
    // Group types
    Task<GroupType> GetGroupTypeAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<GroupType>> ListGroupTypesAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<GroupType> CreateGroupTypeAsync(GroupTypeCreateRequest request, CancellationToken cancellationToken = default);
    Task<GroupType> UpdateGroupTypeAsync(string id, GroupTypeUpdateRequest request, CancellationToken cancellationToken = default);
    
    // Location management
    Task<Location> GetLocationAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Location>> ListLocationsAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Location> CreateLocationAsync(LocationCreateRequest request, CancellationToken cancellationToken = default);
    Task<Location> UpdateLocationAsync(string id, LocationUpdateRequest request, CancellationToken cancellationToken = default);
    
    // Tag management
    Task<Tag> GetTagAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Tag>> ListTagsAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Tag> CreateTagAsync(TagCreateRequest request, CancellationToken cancellationToken = default);
    Task<Tag> UpdateTagAsync(string id, TagUpdateRequest request, CancellationToken cancellationToken = default);
    
    // Person-specific operations
    Task<Core.Person> GetPersonAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Membership>> GetMembershipsForPersonAsync(string personId, QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Group>> GetGroupsForPersonAsync(string personId, QueryParameters parameters = null, CancellationToken cancellationToken = default);
    
    // Reporting
    Task<GroupReport> GenerateGroupReportAsync(GroupReportRequest request, CancellationToken cancellationToken = default);
    Task<AttendanceReport> GenerateAttendanceReportAsync(string groupId, AttendanceReportRequest request, CancellationToken cancellationToken = default);
}
```

## Fluent API Interface

```csharp
public interface IGroupsFluentContext
{
    // Group queries
    IGroupFluentContext Groups();
    IGroupFluentContext Group(string groupId);
    
    // Group type queries
    IGroupTypeFluentContext GroupTypes();
    IGroupTypeFluentContext GroupType(string groupTypeId);
    
    // Location queries
    ILocationFluentContext Locations();
    ILocationFluentContext Location(string locationId);
    
    // Tag queries
    ITagFluentContext Tags();
    ITagFluentContext Tag(string tagId);
    
    // Person-specific operations
    IPersonGroupsFluentContext Person(string personId);
    
    // Reporting
    IGroupReportingFluentContext Reports();
}

public interface IGroupFluentContext
{
    IGroupFluentContext Where(Expression<Func<Group, bool>> predicate);
    IGroupFluentContext Include(Expression<Func<Group, object>> include);
    IGroupFluentContext OrderBy(Expression<Func<Group, object>> orderBy);
    IGroupFluentContext OrderByDescending(Expression<Func<Group, object>> orderBy);
    IGroupFluentContext OfType(string groupTypeId);
    IGroupFluentContext AtLocation(string locationId);
    IGroupFluentContext WithTag(string tagId);
    IGroupFluentContext Active();
    IGroupFluentContext Archived();
    
    // Group-specific operations
    IMembershipFluentContext Members();
    IEventFluentContext Events();
    IEnrollmentFluentContext Enrollments();
    
    Task<Group> GetAsync(CancellationToken cancellationToken = default);
    Task<IPagedResponse<Group>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default);
    Task<List<Group>> GetAllAsync(CancellationToken cancellationToken = default);
}

public interface IMembershipFluentContext
{
    IMembershipFluentContext Where(Expression<Func<Membership, bool>> predicate);
    IMembershipFluentContext WithRole(string role);
    IMembershipFluentContext JoinedAfter(DateTime date);
    IMembershipFluentContext JoinedBefore(DateTime date);
    
    Task<IPagedResponse<Membership>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default);
    Task<List<Membership>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
}

public interface IEventFluentContext
{
    IEventFluentContext Where(Expression<Func<Event, bool>> predicate);
    IEventFluentContext Include(Expression<Func<Event, object>> include);
    IEventFluentContext OrderBy(Expression<Func<Event, object>> orderBy);
    IEventFluentContext Upcoming();
    IEventFluentContext Past();
    IEventFluentContext OnDate(DateTime date);
    IEventFluentContext BetweenDates(DateTime startDate, DateTime endDate);
    
    // Event-specific operations
    IAttendanceFluentContext Attendance();
    
    Task<IPagedResponse<Event>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default);
    Task<List<Event>> GetAllAsync(CancellationToken cancellationToken = default);
}
```

## Usage Examples

### Service-Based API
```csharp
// Get groups of a specific type
var smallGroups = await groupsService.ListGroupsAsync(new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["group_type_id"] = "small-groups-type-id",
        ["archived_at"] = null // Active groups only
    },
    Include = new[] { "group_type", "location", "memberships" },
    OrderBy = "name"
});

// Create a new group
var newGroup = await groupsService.CreateGroupAsync(new GroupCreateRequest
{
    Name = "Young Adults Bible Study",
    Description = "Weekly Bible study for young adults",
    GroupTypeId = "bible-study-type-id",
    ContactEmail = "leader@example.com",
    Schedule = "Thursdays at 7:00 PM",
    LocationId = "location123"
});

// Add a member to a group
var membership = await groupsService.AddMemberAsync("group123", new MembershipCreateRequest
{
    PersonId = "person456",
    Role = "member"
});

// Create a group event
var groupEvent = await groupsService.CreateEventAsync("group123", new EventCreateRequest
{
    Name = "Weekly Meeting",
    Description = "Our regular weekly gathering",
    StartsAt = DateTime.Now.AddDays(7).Date.AddHours(19), // Next week at 7 PM
    EndsAt = DateTime.Now.AddDays(7).Date.AddHours(21), // Next week at 9 PM
    Location = "Community Room A"
});

// Record attendance
var attendance = await groupsService.RecordAttendanceAsync("event789", new AttendanceCreateRequest
{
    PersonId = "person456",
    Attended = true,
    Role = "member"
});
```

### Fluent API
```csharp
// Complex group query
var activeGroups = await client
    .Groups()
    .Groups()
    .Active()
    .OfType("small-groups-type-id")
    .Where(g => g.MembershipsCount < 15) // Groups with room for more members
    .Include(g => g.GroupType)
    .Include(g => g.Location)
    .Include(g => g.Memberships)
    .OrderBy(g => g.Name)
    .GetPagedAsync(pageSize: 50);

// Get all groups for a person
var personGroups = await client
    .Groups()
    .Person("person123")
    .Groups()
    .Active()
    .Include(g => g.GroupType)
    .Include(g => g.Location)
    .GetAllAsync();

// Group event management
var upcomingEvents = await client
    .Groups()
    .Group("group123")
    .Events()
    .Upcoming()
    .OrderBy(e => e.StartsAt)
    .GetAllAsync();

// Attendance tracking
var eventAttendance = await client
    .Groups()
    .Group("group123")
    .Events()
    .Event("event456")
    .Attendance()
    .Where(a => a.Attended == true)
    .GetCountAsync();

// Enrollment management
var pendingEnrollments = await client
    .Groups()
    .Group("group123")
    .Enrollments()
    .Where(e => e.Status == "pending")
    .OrderBy(e => e.CreatedAt)
    .GetAllAsync();

// Tag-based group discovery
var taggedGroups = await client
    .Groups()
    .Groups()
    .WithTag("newcomers")
    .Active()
    .Include(g => g.Location)
    .GetAllAsync();

// Location-based queries
var locationGroups = await client
    .Groups()
    .Location("location123")
    .Groups()
    .Active()
    .OrderBy(g => g.Name)
    .GetAllAsync();
```

## Implementation Notes

### Data Mapping
- Map Groups-specific Person DTOs to unified Core.Person model
- Handle group membership roles and permissions
- Preserve group-specific properties and relationships

### Security Considerations
- Implement proper authorization for group access
- Protect member privacy based on group settings
- Secure enrollment and membership management

### Caching Strategy
- Cache group type and location data (relatively static)
- Cache group membership for performance
- Use time-based cache expiration for attendance data

### Error Handling
- Handle group capacity limits
- Validate membership roles and permissions
- Handle enrollment approval workflows

### Performance Considerations
- Optimize queries for large group datasets
- Implement efficient membership queries
- Use pagination for group and member lists
- Consider read replicas for reporting queries

### Communication Features
- Support for group messaging and notifications
- Event reminder systems
- Enrollment notification workflows
- Member communication preferences

This module provides comprehensive group management capabilities while maintaining privacy and security standards for community organization systems.