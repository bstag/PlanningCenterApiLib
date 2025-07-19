# Services Module Specification

## Overview

The Services module manages service planning and worship management in Planning Center. It provides comprehensive functionality for creating service plans, managing song libraries, scheduling teams, organizing arrangements, and coordinating worship services.

## Core Entities

### Plan
The primary entity representing a service plan or worship service.

**Key Attributes:**
- `id` - Unique identifier
- `title` - Plan title
- `dates` - Service dates
- `short_dates` - Abbreviated date display
- `planning_center_url` - Plan URL
- `sort_date` - Sort timestamp
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp
- `public` - Public visibility flag
- `rehearsal_times` - Rehearsal schedule
- `service_times` - Service schedule
- `other_times` - Additional times
- `length` - Service duration
- `notes` - Plan notes

**Relationships:**
- `service_type` - Service category
- `items` - Plan items/elements
- `team_members` - Assigned team members
- `plan_people` - People assignments
- `plan_times` - Service times
- `attachments` - Plan attachments

### ServiceType
Represents a category or type of service.

**Key Attributes:**
- `id` - Unique identifier
- `name` - Service type name
- `sequence` - Sort order
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `plans` - Plans of this type
- `teams` - Associated teams

### Item
Represents an element within a service plan (song, announcement, etc.).

**Key Attributes:**
- `id` - Unique identifier
- `title` - Item title
- `sequence` - Order in plan
- `item_type` - Type of item (song, header, media, etc.)
- `description` - Item description
- `key_name` - Musical key
- `length` - Item duration
- `html_details` - Rich text details
- `service_position` - Position in service
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `plan` - Associated plan
- `song` - Associated song (if applicable)
- `arrangement` - Associated arrangement
- `item_notes` - Item notes
- `item_times` - Item timing

### Song
Represents a song in the song library.

**Key Attributes:**
- `id` - Unique identifier
- `title` - Song title
- `author` - Song author/composer
- `copyright` - Copyright information
- `ccli_number` - CCLI license number
- `themes` - Song themes/tags
- `hidden` - Visibility flag
- `notes` - Song notes
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `arrangements` - Song arrangements
- `items` - Plan items using this song
- `attachments` - Song attachments

### Arrangement
Represents a specific arrangement of a song.

**Key Attributes:**
- `id` - Unique identifier
- `name` - Arrangement name
- `description` - Arrangement description
- `bpm` - Beats per minute
- `key_name` - Musical key
- `chord_chart` - Chord progression
- `number_chart` - Number chart
- `notes` - Arrangement notes
- `print_margin` - Print formatting
- `has_chords` - Chord chart availability
- `length` - Arrangement duration
- `meter` - Time signature
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `song` - Associated song
- `arrangement_sections` - Arrangement sections
- `items` - Plan items using this arrangement
- `keys` - Available keys

### ArrangementSection
Represents a section within an arrangement (verse, chorus, bridge, etc.).

**Key Attributes:**
- `id` - Unique identifier
- `name` - Section name (verse, chorus, bridge, etc.)
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `arrangement` - Associated arrangement

### Team
Represents a service team or ministry team.

**Key Attributes:**
- `id` - Unique identifier
- `name` - Team name
- `sequence` - Sort order
- `schedule_to` - Scheduling preference
- `default_status` - Default member status
- `default_prepare_notifications` - Notification settings
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `service_type` - Associated service type
- `team_positions` - Team positions
- `team_members` - Team members
- `people` - Associated people

### TeamPosition
Represents a position within a team.

**Key Attributes:**
- `id` - Unique identifier
- `name` - Position name
- `sequence` - Sort order
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `team` - Associated team
- `team_position_assignments` - Position assignments

### Person
Represents a person in the Services context.

**Key Attributes:**
- `id` - Unique identifier
- `first_name` - First name
- `last_name` - Last name
- `full_name` - Complete name
- `email` - Email address
- `photo_url` - Profile photo URL
- `photo_thumbnail_url` - Thumbnail photo URL
- `preferred_app_name` - App preference
- `logged_in_at` - Last login
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `team_position_assignments` - Team assignments
- `plan_people` - Plan assignments
- `blockouts` - Scheduling blockouts

### PlanPerson
Represents a person's assignment to a specific plan.

**Key Attributes:**
- `id` - Unique identifier
- `status` - Assignment status (confirmed, declined, etc.)
- `created_at` - Assignment timestamp
- `updated_at` - Last update timestamp
- `notes` - Assignment notes
- `decline_reason` - Decline reason (if applicable)

**Relationships:**
- `plan` - Associated plan
- `person` - Associated person
- `team` - Associated team
- `team_position` - Assigned position

## API Endpoints

### Plan Management
- `GET /services/v2/service_types/{service_type_id}/plans` - List plans
- `GET /services/v2/plans/{id}` - Get specific plan
- `POST /services/v2/service_types/{service_type_id}/plans` - Create plan
- `PATCH /services/v2/plans/{id}` - Update plan
- `DELETE /services/v2/plans/{id}` - Delete plan

### Item Management
- `GET /services/v2/plans/{plan_id}/items` - List plan items
- `GET /services/v2/items/{id}` - Get specific item
- `POST /services/v2/plans/{plan_id}/items` - Add item to plan
- `PATCH /services/v2/items/{id}` - Update item
- `DELETE /services/v2/items/{id}` - Remove item

### Song Library
- `GET /services/v2/songs` - List songs
- `GET /services/v2/songs/{id}` - Get specific song
- `POST /services/v2/songs` - Create song
- `PATCH /services/v2/songs/{id}` - Update song
- `DELETE /services/v2/songs/{id}` - Delete song

### Arrangement Management
- `GET /services/v2/songs/{song_id}/arrangements` - List arrangements
- `GET /services/v2/arrangements/{id}` - Get specific arrangement
- `POST /services/v2/songs/{song_id}/arrangements` - Create arrangement
- `PATCH /services/v2/arrangements/{id}` - Update arrangement
- `DELETE /services/v2/arrangements/{id}` - Delete arrangement

### Team Management
- `GET /services/v2/teams` - List teams
- `GET /services/v2/teams/{id}` - Get specific team
- `POST /services/v2/teams` - Create team
- `PATCH /services/v2/teams/{id}` - Update team

### Team Scheduling
- `GET /services/v2/plans/{plan_id}/team_members` - List plan team members
- `POST /services/v2/plans/{plan_id}/team_members` - Assign team member
- `PATCH /services/v2/plan_people/{id}` - Update assignment
- `DELETE /services/v2/plan_people/{id}` - Remove assignment

### Service Types
- `GET /services/v2/service_types` - List service types
- `GET /services/v2/service_types/{id}` - Get specific service type

## Query Parameters

### Include Parameters
- `service_type` - Include service type information
- `items` - Include plan items
- `team_members` - Include team assignments
- `song` - Include song information
- `arrangement` - Include arrangement details
- `plan_times` - Include service times

### Filtering
- `where[title]` - Filter by plan title
- `where[dates]` - Filter by service date
- `where[service_type_id]` - Filter by service type
- `where[created_at]` - Filter by creation date

### Sorting
- `order=sort_date` - Sort by service date
- `order=title` - Sort by plan title
- `order=created_at` - Sort by creation date
- `order=-sort_date` - Sort by service date (descending)

## Service Interface

```csharp
public interface IServicesService
{
    // Plan management
    Task<Plan> GetPlanAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Plan>> ListPlansAsync(string serviceTypeId, QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Plan> CreatePlanAsync(string serviceTypeId, PlanCreateRequest request, CancellationToken cancellationToken = default);
    Task<Plan> UpdatePlanAsync(string id, PlanUpdateRequest request, CancellationToken cancellationToken = default);
    Task DeletePlanAsync(string id, CancellationToken cancellationToken = default);
    
    // Item management
    Task<Item> GetItemAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Item>> ListItemsAsync(string planId, QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Item> AddItemAsync(string planId, ItemCreateRequest request, CancellationToken cancellationToken = default);
    Task<Item> UpdateItemAsync(string id, ItemUpdateRequest request, CancellationToken cancellationToken = default);
    Task DeleteItemAsync(string id, CancellationToken cancellationToken = default);
    Task<Item> ReorderItemAsync(string id, int newSequence, CancellationToken cancellationToken = default);
    
    // Song library
    Task<Song> GetSongAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Song>> ListSongsAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Song> CreateSongAsync(SongCreateRequest request, CancellationToken cancellationToken = default);
    Task<Song> UpdateSongAsync(string id, SongUpdateRequest request, CancellationToken cancellationToken = default);
    Task DeleteSongAsync(string id, CancellationToken cancellationToken = default);
    
    // Arrangement management
    Task<Arrangement> GetArrangementAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Arrangement>> ListArrangementsAsync(string songId, QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Arrangement> CreateArrangementAsync(string songId, ArrangementCreateRequest request, CancellationToken cancellationToken = default);
    Task<Arrangement> UpdateArrangementAsync(string id, ArrangementUpdateRequest request, CancellationToken cancellationToken = default);
    Task DeleteArrangementAsync(string id, CancellationToken cancellationToken = default);
    
    // Arrangement sections
    Task<IPagedResponse<ArrangementSection>> ListArrangementSectionsAsync(string arrangementId, QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<ArrangementSection> CreateArrangementSectionAsync(string arrangementId, ArrangementSectionCreateRequest request, CancellationToken cancellationToken = default);
    Task<ArrangementSection> UpdateArrangementSectionAsync(string id, ArrangementSectionUpdateRequest request, CancellationToken cancellationToken = default);
    Task DeleteArrangementSectionAsync(string id, CancellationToken cancellationToken = default);
    
    // Team management
    Task<Team> GetTeamAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Team>> ListTeamsAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Team> CreateTeamAsync(TeamCreateRequest request, CancellationToken cancellationToken = default);
    Task<Team> UpdateTeamAsync(string id, TeamUpdateRequest request, CancellationToken cancellationToken = default);
    
    // Team scheduling
    Task<IPagedResponse<PlanPerson>> ListPlanTeamMembersAsync(string planId, QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<PlanPerson> AssignTeamMemberAsync(string planId, PlanPersonCreateRequest request, CancellationToken cancellationToken = default);
    Task<PlanPerson> UpdateAssignmentAsync(string id, PlanPersonUpdateRequest request, CancellationToken cancellationToken = default);
    Task DeleteAssignmentAsync(string id, CancellationToken cancellationToken = default);
    
    // Service types
    Task<ServiceType> GetServiceTypeAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<ServiceType>> ListServiceTypesAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    
    // Person-specific operations
    Task<Core.Person> GetPersonAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<PlanPerson>> GetAssignmentsForPersonAsync(string personId, QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Plan>> GetPlansForPersonAsync(string personId, QueryParameters parameters = null, CancellationToken cancellationToken = default);
    
    // Plan templates
    Task<Plan> CreatePlanFromTemplateAsync(string templateId, PlanFromTemplateRequest request, CancellationToken cancellationToken = default);
    Task<Plan> SavePlanAsTemplateAsync(string planId, PlanTemplateRequest request, CancellationToken cancellationToken = default);
    
    // Reporting
    Task<ServicesReport> GenerateServicesReportAsync(ServicesReportRequest request, CancellationToken cancellationToken = default);
    Task<SongUsageReport> GenerateSongUsageReportAsync(SongUsageReportRequest request, CancellationToken cancellationToken = default);
}
```

## Fluent API Interface

```csharp
public interface IServicesFluentContext
{
    // Plan queries
    IPlanFluentContext Plans();
    IPlanFluentContext Plan(string planId);
    
    // Song queries
    ISongFluentContext Songs();
    ISongFluentContext Song(string songId);
    
    // Team queries
    ITeamFluentContext Teams();
    ITeamFluentContext Team(string teamId);
    
    // Service type queries
    IServiceTypeFluentContext ServiceTypes();
    IServiceTypeFluentContext ServiceType(string serviceTypeId);
    
    // Person-specific operations
    IPersonServicesFluentContext Person(string personId);
    
    // Reporting
    IServicesReportingFluentContext Reports();
}

public interface IPlanFluentContext
{
    IPlanFluentContext Where(Expression<Func<Plan, bool>> predicate);
    IPlanFluentContext Include(Expression<Func<Plan, object>> include);
    IPlanFluentContext OrderBy(Expression<Func<Plan, object>> orderBy);
    IPlanFluentContext OrderByDescending(Expression<Func<Plan, object>> orderBy);
    IPlanFluentContext ForServiceType(string serviceTypeId);
    IPlanFluentContext OnDate(DateTime date);
    IPlanFluentContext BetweenDates(DateTime startDate, DateTime endDate);
    IPlanFluentContext Upcoming();
    IPlanFluentContext Past();
    IPlanFluentContext Public();
    IPlanFluentContext Private();
    
    // Plan-specific operations
    IItemFluentContext Items();
    ITeamMemberFluentContext TeamMembers();
    
    Task<Plan> GetAsync(CancellationToken cancellationToken = default);
    Task<IPagedResponse<Plan>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default);
    Task<List<Plan>> GetAllAsync(CancellationToken cancellationToken = default);
}

public interface ISongFluentContext
{
    ISongFluentContext Where(Expression<Func<Song, bool>> predicate);
    ISongFluentContext Include(Expression<Func<Song, object>> include);
    ISongFluentContext OrderBy(Expression<Func<Song, object>> orderBy);
    ISongFluentContext WithTheme(string theme);
    ISongFluentContext ByAuthor(string author);
    ISongFluentContext WithCcliNumber(string ccliNumber);
    ISongFluentContext Visible();
    ISongFluentContext Hidden();
    
    // Song-specific operations
    IArrangementFluentContext Arrangements();
    IItemFluentContext Items(); // Plans using this song
    
    Task<Song> GetAsync(CancellationToken cancellationToken = default);
    Task<IPagedResponse<Song>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default);
    Task<List<Song>> GetAllAsync(CancellationToken cancellationToken = default);
}

public interface IArrangementFluentContext
{
    IArrangementFluentContext Where(Expression<Func<Arrangement, bool>> predicate);
    IArrangementFluentContext Include(Expression<Func<Arrangement, object>> include);
    IArrangementFluentContext InKey(string key);
    IArrangementFluentContext WithBpm(int bpm);
    IArrangementFluentContext WithChords();
    IArrangementFluentContext WithoutChords();
    
    // Arrangement-specific operations
    IArrangementSectionFluentContext Sections();
    
    Task<IPagedResponse<Arrangement>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default);
    Task<List<Arrangement>> GetAllAsync(CancellationToken cancellationToken = default);
}

public interface ITeamFluentContext
{
    ITeamFluentContext Where(Expression<Func<Team, bool>> predicate);
    ITeamFluentContext Include(Expression<Func<Team, object>> include);
    ITeamFluentContext ForServiceType(string serviceTypeId);
    
    // Team-specific operations
    ITeamMemberFluentContext Members();
    ITeamPositionFluentContext Positions();
    
    Task<IPagedResponse<Team>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default);
    Task<List<Team>> GetAllAsync(CancellationToken cancellationToken = default);
}
```

## Usage Examples

### Service-Based API
```csharp
// Get upcoming plans for a service type
var upcomingPlans = await servicesService.ListPlansAsync("sunday-service-id", new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["dates"] = $">={DateTime.Today:yyyy-MM-dd}"
    },
    Include = new[] { "items", "team_members", "service_type" },
    OrderBy = "sort_date"
});

// Create a new plan
var newPlan = await servicesService.CreatePlanAsync("sunday-service-id", new PlanCreateRequest
{
    Title = "Sunday Service - Easter",
    Dates = "April 9, 2024",
    Notes = "Special Easter service with extended worship time"
});

// Add songs to the plan
var openingSong = await servicesService.AddItemAsync(newPlan.Id, new ItemCreateRequest
{
    Title = "Amazing Grace",
    ItemType = "song",
    SongId = "amazing-grace-song-id",
    ArrangementId = "amazing-grace-arrangement-id",
    Sequence = 1,
    KeyName = "G"
});

var worshipSet = await servicesService.AddItemAsync(newPlan.Id, new ItemCreateRequest
{
    Title = "Worship Set",
    ItemType = "header",
    Sequence = 2
});

// Assign team members
var worshipLeader = await servicesService.AssignTeamMemberAsync(newPlan.Id, new PlanPersonCreateRequest
{
    PersonId = "person123",
    TeamId = "worship-team-id",
    TeamPositionId = "worship-leader-position-id",
    Status = "confirmed"
});

// Create a new song
var newSong = await servicesService.CreateSongAsync(new SongCreateRequest
{
    Title = "New Song of Praise",
    Author = "John Doe",
    Copyright = "2024 Example Music",
    CcliNumber = "1234567",
    Themes = "praise, worship, celebration"
});

// Create an arrangement for the song
var arrangement = await servicesService.CreateArrangementAsync(newSong.Id, new ArrangementCreateRequest
{
    Name = "Default Arrangement",
    KeyName = "C",
    Bpm = 120,
    ChordChart = "C - F - G - C",
    HasChords = true
});
```

### Fluent API
```csharp
// Complex plan query
var easterPlans = await client
    .Services()
    .Plans()
    .ForServiceType("sunday-service-id")
    .BetweenDates(new DateTime(2024, 4, 1), new DateTime(2024, 4, 30))
    .Include(p => p.Items)
    .Include(p => p.TeamMembers)
    .OrderBy(p => p.SortDate)
    .GetAllAsync();

// Get all plans for a person
var personPlans = await client
    .Services()
    .Person("person123")
    .Plans()
    .Upcoming()
    .Include(p => p.ServiceType)
    .Include(p => p.TeamMembers)
    .OrderBy(p => p.SortDate)
    .GetAllAsync();

// Song library queries
var worshipSongs = await client
    .Services()
    .Songs()
    .WithTheme("worship")
    .Visible()
    .Include(s => s.Arrangements)
    .OrderBy(s => s.Title)
    .GetAllAsync();

// Popular songs analysis
var frequentlyUsedSongs = await client
    .Services()
    .Songs()
    .Where(s => s.Items.Count() > 10) // Used in more than 10 plans
    .Include(s => s.Items)
    .OrderByDescending(s => s.Items.Count())
    .GetAllAsync();

// Team scheduling
var teamAssignments = await client
    .Services()
    .Team("worship-team-id")
    .Members()
    .Where(tm => tm.Plans.Any(p => p.SortDate >= DateTime.Today))
    .Include(tm => tm.Person)
    .Include(tm => tm.Plans)
    .GetAllAsync();

// Arrangement management
var songArrangements = await client
    .Services()
    .Song("song123")
    .Arrangements()
    .InKey("G")
    .WithChords()
    .Include(a => a.Sections)
    .OrderBy(a => a.Name)
    .GetAllAsync();

// Service type operations
var serviceTypePlans = await client
    .Services()
    .ServiceType("sunday-service-id")
    .Plans()
    .Upcoming()
    .Include(p => p.Items)
    .OrderBy(p => p.SortDate)
    .GetAllAsync();

// Plan item management
var planItems = await client
    .Services()
    .Plan("plan123")
    .Items()
    .Where(i => i.ItemType == "song")
    .Include(i => i.Song)
    .Include(i => i.Arrangement)
    .OrderBy(i => i.Sequence)
    .GetAllAsync();
```

## Implementation Notes

### Data Mapping
- Map Services-specific Person DTOs to unified Core.Person model
- Handle musical notation and chord chart formatting
- Preserve plan structure and item sequencing

### Security Considerations
- Implement proper authorization for plan access
- Protect copyrighted song information
- Secure team scheduling and assignment data

### Caching Strategy
- Cache song library and arrangement data
- Cache team and service type information
- Use time-based cache expiration for plan data

### Error Handling
- Handle plan scheduling conflicts
- Validate musical key and chord chart formats
- Handle team assignment conflicts and availability

### Performance Considerations
- Optimize queries for large song libraries
- Implement efficient plan and item queries
- Use pagination for song and plan lists
- Consider read replicas for reporting queries

### Musical Features
- Support for chord chart formatting and display
- Key transposition capabilities
- Tempo and timing management
- Song arrangement versioning

### Team Coordination
- Automated scheduling and conflict detection
- Notification systems for plan changes
- Team availability tracking
- Position-based assignment management

### Plan Templates
- Reusable plan templates for recurring services
- Template customization and variation
- Bulk plan creation from templates
- Template sharing and collaboration

This module provides comprehensive service planning capabilities while maintaining security and performance standards for worship management systems.