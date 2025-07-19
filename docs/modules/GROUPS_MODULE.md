# Groups Module Documentation

The Groups module provides comprehensive group management capabilities for Planning Center. This module handles all group-related functionality including groups, memberships, group types, attendance tracking, and group administration.

## 📋 Module Overview

### Key Entities
- **Groups**: Small groups, classes, teams, and other organizational units
- **Group Types**: Categories that define group characteristics and settings
- **Memberships**: Individual person's participation in groups with roles and status
- **Attendance**: Tracking of group meeting attendance
- **Events**: Group meetings and activities
- **Locations**: Meeting places and venues for groups
- **Resources**: Materials and equipment used by groups

### Authentication
Requires Planning Center Groups app access with appropriate permissions.

## 🔧 Traditional Service API

### Group Management

#### Basic Group Operations
```csharp
public class GroupsService
{
    private readonly IGroupsService _groupsService;
    
    public GroupsService(IGroupsService groupsService)
    {
        _groupsService = groupsService;
    }
    
    // Get a group by ID
    public async Task<Group?> GetGroupAsync(string id)
    {
        return await _groupsService.GetGroupAsync(id);
    }
    
    // List groups with pagination
    public async Task<IPagedResponse<Group>> GetGroupsAsync()
    {
        return await _groupsService.ListGroupsAsync(new QueryParameters
        {
            PerPage = 25,
            OrderBy = "name"
        });
    }
}
```

#### Create and Update Groups
```csharp
// Create a new group
var newGroup = await _groupsService.CreateGroupAsync(new GroupCreateRequest
{
    Name = "Young Adults Bible Study",
    Description = "Weekly Bible study for young adults aged 18-30",
    GroupTypeId = "gt_123",
    Schedule = "Weekly on Thursdays at 7:00 PM",
    LocationTypePreference = "physical",
    ContactEmail = "youngadults@church.com",
    PublicChurchCenterWebEnabled = true,
    Enrollment = "open"
});

// Update a group
var updatedGroup = await _groupsService.UpdateGroupAsync("group123", new GroupUpdateRequest
{
    Name = "Young Adults Community",
    Description = "Updated description with more activities",
    Schedule = "Weekly on Thursdays at 7:30 PM",
    Enrollment = "request_to_join"
});

// Delete a group
await _groupsService.DeleteGroupAsync("group123");
```

#### Group Filtering and Search
```csharp
// Get groups by type
var bibleStudyGroups = await _groupsService.ListGroupsAsync(new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["group_type_id"] = "gt_bible_study"
    },
    OrderBy = "name"
});

// Get open enrollment groups
var openGroups = await _groupsService.ListGroupsAsync(new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["enrollment"] = "open"
    },
    OrderBy = "name"
});

// Search groups by name
var searchResults = await _groupsService.ListGroupsAsync(new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["name"] = "*Bible*" // Wildcard search
    },
    PerPage = 50
});
```

### Group Type Management

#### Group Type Operations
```csharp
// Get a group type by ID
var groupType = await _groupsService.GetGroupTypeAsync("gt123");

// List group types
var groupTypes = await _groupsService.ListGroupTypesAsync(new QueryParameters
{
    OrderBy = "name"
});

// Create a new group type
var newGroupType = await _groupsService.CreateGroupTypeAsync(new GroupTypeCreateRequest
{
    Name = "Bible Study",
    Description = "Small group Bible studies",
    ChurchCenterVisible = true,
    ChurchCenterMapVisible = true,
    Color = "#4CAF50"
});

// Update a group type
var updatedGroupType = await _groupsService.UpdateGroupTypeAsync("gt123", new GroupTypeUpdateRequest
{
    Name = "Bible Study Groups",
    Description = "Updated description for Bible study groups",
    Color = "#2196F3"
});

// Delete a group type
await _groupsService.DeleteGroupTypeAsync("gt123");
```

### Membership Management

#### Membership Operations
```csharp
// Get a membership by ID
var membership = await _groupsService.GetMembershipAsync("membership123");

// List memberships for a group
var groupMemberships = await _groupsService.ListMembershipsAsync(new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["group_id"] = "group123"
    },
    OrderBy = "person_name"
});

// List memberships for a person
var personMemberships = await _groupsService.ListMembershipsAsync(new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["person_id"] = "12345"
    },
    OrderBy = "group_name"
});

// Create a new membership (add person to group)
var newMembership = await _groupsService.CreateMembershipAsync(new MembershipCreateRequest
{
    GroupId = "group123",
    PersonId = "12345",
    Role = "member",
    JoinedAt = DateTime.Now
});

// Update a membership
var updatedMembership = await _groupsService.UpdateMembershipAsync("membership123", new MembershipUpdateRequest
{
    Role = "leader",
    AccountCenterIdentifier = "leader_access"
});

// Delete a membership (remove person from group)
await _groupsService.DeleteMembershipAsync("membership123");
```

#### Membership Roles and Status
```csharp
// Get group leaders
var leaders = await _groupsService.ListMembershipsAsync(new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["group_id"] = "group123",
        ["role"] = "leader"
    }
});

// Get pending membership requests
var pendingRequests = await _groupsService.ListMembershipsAsync(new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["group_id"] = "group123",
        ["joined_at"] = null // Pending requests have no joined_at date
    }
});
```

### Attendance Management

#### Attendance Operations
```csharp
// Get attendance for a group event
var attendance = await _groupsService.GetAttendanceAsync("attendance123");

// List attendance records
var attendanceRecords = await _groupsService.ListAttendanceAsync(new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["group_id"] = "group123"
    },
    OrderBy = "-occurred_at" // Most recent first
});

// Create attendance record
var newAttendance = await _groupsService.CreateAttendanceAsync(new AttendanceCreateRequest
{
    GroupId = "group123",
    PersonId = "12345",
    OccurredAt = DateTime.Now,
    AttendanceTypeId = "at_present"
});

// Update attendance
var updatedAttendance = await _groupsService.UpdateAttendanceAsync("attendance123", new AttendanceUpdateRequest
{
    AttendanceTypeId = "at_absent",
    Note = "Family emergency"
});

// Delete attendance record
await _groupsService.DeleteAttendanceAsync("attendance123");
```

### Event Management

#### Group Event Operations
```csharp
// Get a group event by ID
var groupEvent = await _groupsService.GetEventAsync("event123");

// List events for a group
var groupEvents = await _groupsService.ListEventsAsync(new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["group_id"] = "group123"
    },
    OrderBy = "-starts_at"
});

// Create a group event
var newEvent = await _groupsService.CreateEventAsync(new EventCreateRequest
{
    GroupId = "group123",
    Name = "Weekly Bible Study",
    Description = "Our regular Thursday evening Bible study",
    StartsAt = DateTime.Now.AddDays(7).Date.AddHours(19), // Next week at 7 PM
    EndsAt = DateTime.Now.AddDays(7).Date.AddHours(21),   // Until 9 PM
    LocationId = "location123",
    Repeating = true,
    RepeatingPattern = "weekly"
});

// Update a group event
var updatedEvent = await _groupsService.UpdateEventAsync("event123", new EventUpdateRequest
{
    Name = "Bible Study & Fellowship",
    Description = "Bible study followed by fellowship time",
    StartsAt = DateTime.Now.AddDays(7).Date.AddHours(19, 30) // 7:30 PM
});

// Delete a group event
await _groupsService.DeleteEventAsync("event123");
```

### Location Management

#### Location Operations
```csharp
// Get a location by ID
var location = await _groupsService.GetLocationAsync("location123");

// List locations
var locations = await _groupsService.ListLocationsAsync(new QueryParameters
{
    OrderBy = "name"
});

// Create a new location
var newLocation = await _groupsService.CreateLocationAsync(new LocationCreateRequest
{
    Name = "Fellowship Hall",
    Description = "Main fellowship hall with capacity for 50 people",
    Strategy = "physical",
    Address = "123 Church St, Anytown, ST 12345"
});

// Update a location
var updatedLocation = await _groupsService.UpdateLocationAsync("location123", new LocationUpdateRequest
{
    Name = "Fellowship Hall - Room A",
    Description = "Updated description with room designation"
});

// Delete a location
await _groupsService.DeleteLocationAsync("location123");
```

### Pagination Helpers
```csharp
// Get all groups (handles pagination automatically)
var allGroups = await _groupsService.GetAllGroupsAsync(new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["enrollment"] = "open"
    }
});

// Stream groups for memory-efficient processing
await foreach (var group in _groupsService.StreamGroupsAsync())
{
    Console.WriteLine($"{group.Name} - {group.Enrollment}");
    // Process one group at a time without loading all into memory
}
```

## 🚀 Fluent API Usage

### Basic Queries
```csharp
public class GroupsFluentService
{
    private readonly IPlanningCenterClient _client;
    
    public GroupsFluentService(IPlanningCenterClient client)
    {
        _client = client;
    }
    
    // Get a group by ID
    public async Task<Group?> GetGroupAsync(string id)
    {
        return await _client.Groups().GetAsync(id);
    }
    
    // Get first page of groups
    public async Task<IPagedResponse<Group>> GetGroupsPageAsync()
    {
        return await _client.Groups().GetPagedAsync(pageSize: 25);
    }
}
```

### Advanced Group Filtering
```csharp
// Get open enrollment groups
var openGroups = await _client.Groups()
    .Where(g => g.Enrollment == "open")
    .Where(g => g.PublicChurchCenterWebEnabled == true)
    .OrderBy(g => g.Name)
    .GetAllAsync();

// Get groups by type
var bibleStudyGroups = await _client.Groups()
    .Where(g => g.GroupTypeId == "gt_bible_study")
    .Where(g => g.Enrollment != "closed")
    .OrderBy(g => g.Name)
    .GetAllAsync();

// Get groups with available spots
var availableGroups = await _client.Groups()
    .Where(g => g.Enrollment == "open")
    .Where(g => g.MembershipsCount < g.MembershipsLimit)
    .OrderBy(g => g.Name)
    .GetPagedAsync(pageSize: 50);

// Get groups by location preference
var physicalGroups = await _client.Groups()
    .Where(g => g.LocationTypePreference == "physical")
    .Where(g => g.Enrollment == "open")
    .OrderBy(g => g.Name)
    .GetAllAsync();
```

### Membership Filtering
```csharp
// Get person's group memberships
var personMemberships = await _client.Groups()
    .Memberships()
    .Where(m => m.PersonId == "12345")
    .Where(m => m.JoinedAt.HasValue) // Active memberships only
    .OrderBy(m => m.GroupName)
    .GetAllAsync();

// Get group leaders
var groupLeaders = await _client.Groups()
    .Memberships()
    .Where(m => m.GroupId == "group123")
    .Where(m => m.Role == "leader")
    .OrderBy(m => m.PersonName)
    .GetAllAsync();

// Get pending membership requests
var pendingRequests = await _client.Groups()
    .Memberships()
    .Where(m => m.GroupId == "group123")
    .Where(m => !m.JoinedAt.HasValue)
    .OrderBy(m => m.CreatedAt)
    .GetAllAsync();
```

### Group Type and Event Filtering
```csharp
// Get active group types
var activeGroupTypes = await _client.Groups()
    .GroupTypes()
    .Where(gt => gt.ChurchCenterVisible == true)
    .OrderBy(gt => gt.Name)
    .GetAllAsync();

// Get upcoming group events
var upcomingEvents = await _client.Groups()
    .Events()
    .Where(e => e.StartsAt > DateTime.Now)
    .Where(e => e.GroupId == "group123")
    .OrderBy(e => e.StartsAt)
    .GetAllAsync();

// Get recurring events
var recurringEvents = await _client.Groups()
    .Events()
    .Where(e => e.Repeating == true)
    .OrderBy(e => e.Name)
    .GetAllAsync();
```

### LINQ-like Terminal Operations
```csharp
// Get largest group
var largestGroup = await _client.Groups()
    .Where(g => g.Enrollment == "open")
    .OrderByDescending(g => g.MembershipsCount)
    .FirstAsync();

// Get group by name (should be unique)
var specificGroup = await _client.Groups()
    .Where(g => g.Name == "Young Adults Bible Study")
    .SingleOrDefaultAsync();

// Count open groups
var openGroupCount = await _client.Groups()
    .Where(g => g.Enrollment == "open")
    .CountAsync();

// Check if person is in any groups
var isInGroups = await _client.Groups()
    .Memberships()
    .AnyAsync(m => m.PersonId == "12345" && m.JoinedAt.HasValue);

// Check if group has leaders
var hasLeaders = await _client.Groups()
    .Memberships()
    .AnyAsync(m => m.GroupId == "group123" && m.Role == "leader");
```

### Memory-Efficient Streaming
```csharp
// Stream all active groups
await foreach (var group in _client.Groups()
    .Where(g => g.Enrollment != "closed")
    .OrderBy(g => g.Name)
    .AsAsyncEnumerable())
{
    // Process one group at a time
    await ProcessGroupAsync(group);
}

// Stream with custom pagination
var options = new PaginationOptions { PageSize = 100 };
await foreach (var membership in _client.Groups()
    .Memberships()
    .Where(m => m.JoinedAt.HasValue)
    .AsAsyncEnumerable(options))
{
    await ProcessMembershipAsync(membership);
}
```

### Fluent Group Creation
```csharp
// Create a group with fluent syntax
var newGroup = await _client.Groups()
    .CreateGroup(new GroupCreateRequest
    {
        Name = "New Members Class",
        GroupTypeId = "gt_class",
        Enrollment = "open"
    })
    .WithDescription("Introduction class for new church members")
    .WithSchedule("Sundays at 9:00 AM for 6 weeks")
    .WithLocation("location123")
    .MakeVisible()
    .ExecuteAsync();
```

## 💡 Common Use Cases

### 1. Group Directory
```csharp
public async Task<IReadOnlyList<GroupSummary>> GetGroupDirectoryAsync()
{
    var groups = await _client.Groups()
        .Where(g => g.PublicChurchCenterWebEnabled == true)
        .Where(g => g.Enrollment == "open")
        .Include(g => g.GroupType)
        .Include(g => g.Memberships)
        .OrderBy(g => g.GroupType.Name)
        .ThenBy(g => g.Name)
        .GetAllAsync();
    
    return groups.Select(g => new GroupSummary
    {
        Id = g.Id,
        Name = g.Name,
        Description = g.Description,
        GroupTypeName = g.GroupType?.Name ?? "",
        Schedule = g.Schedule,
        MemberCount = g.MembershipsCount,
        MemberLimit = g.MembershipsLimit,
        HasAvailableSpots = g.MembershipsLimit == null || g.MembershipsCount < g.MembershipsLimit,
        ContactEmail = g.ContactEmail
    }).ToList().AsReadOnly();
}

public class GroupSummary
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string GroupTypeName { get; set; } = "";
    public string Schedule { get; set; } = "";
    public int MemberCount { get; set; }
    public int? MemberLimit { get; set; }
    public bool HasAvailableSpots { get; set; }
    public string ContactEmail { get; set; } = "";
}
```

### 2. Member Management Dashboard
```csharp
public async Task<GroupMembershipReport> GetGroupMembershipReportAsync(string groupId)
{
    var group = await _client.Groups().GetAsync(groupId);
    var memberships = await _client.Groups()
        .Memberships()
        .Where(m => m.GroupId == groupId)
        .Where(m => m.JoinedAt.HasValue)
        .OrderBy(m => m.PersonName)
        .GetAllAsync();
    
    var pendingRequests = await _client.Groups()
        .Memberships()
        .Where(m => m.GroupId == groupId)
        .Where(m => !m.JoinedAt.HasValue)
        .OrderBy(m => m.CreatedAt)
        .GetAllAsync();
    
    return new GroupMembershipReport
    {
        GroupName = group?.Name ?? "",
        ActiveMembers = memberships.Count,
        Leaders = memberships.Count(m => m.Role == "leader"),
        PendingRequests = pendingRequests.Count,
        MembershipLimit = group?.MembershipsLimit,
        AvailableSpots = group?.MembershipsLimit - memberships.Count,
        Members = memberships.Select(m => new MemberInfo
        {
            PersonId = m.PersonId,
            PersonName = m.PersonName,
            Role = m.Role,
            JoinedAt = m.JoinedAt ?? DateTime.MinValue
        }).ToList()
    };
}

public class GroupMembershipReport
{
    public string GroupName { get; set; } = "";
    public int ActiveMembers { get; set; }
    public int Leaders { get; set; }
    public int PendingRequests { get; set; }
    public int? MembershipLimit { get; set; }
    public int? AvailableSpots { get; set; }
    public List<MemberInfo> Members { get; set; } = new();
}

public class MemberInfo
{
    public string PersonId { get; set; } = "";
    public string PersonName { get; set; } = "";
    public string Role { get; set; } = "";
    public DateTime JoinedAt { get; set; }
}
```

### 3. Attendance Tracking
```csharp
public async Task<AttendanceReport> GenerateAttendanceReportAsync(string groupId, DateTime startDate, DateTime endDate)
{
    var attendance = await _client.Groups()
        .Attendance()
        .Where(a => a.GroupId == groupId)
        .Where(a => a.OccurredAt >= startDate)
        .Where(a => a.OccurredAt <= endDate)
        .OrderBy(a => a.OccurredAt)
        .GetAllAsync();
    
    var events = await _client.Groups()
        .Events()
        .Where(e => e.GroupId == groupId)
        .Where(e => e.StartsAt >= startDate)
        .Where(e => e.StartsAt <= endDate)
        .OrderBy(e => e.StartsAt)
        .GetAllAsync();
    
    var attendanceByPerson = attendance
        .GroupBy(a => a.PersonId)
        .ToDictionary(g => g.Key, g => g.Count());
    
    return new AttendanceReport
    {
        GroupId = groupId,
        Period = $"{startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}",
        TotalEvents = events.Count,
        TotalAttendanceRecords = attendance.Count,
        AverageAttendance = events.Count > 0 ? attendance.Count / (double)events.Count : 0,
        AttendanceByPerson = attendanceByPerson
    };
}

public class AttendanceReport
{
    public string GroupId { get; set; } = "";
    public string Period { get; set; } = "";
    public int TotalEvents { get; set; }
    public int TotalAttendanceRecords { get; set; }
    public double AverageAttendance { get; set; }
    public Dictionary<string, int> AttendanceByPerson { get; set; } = new();
}
```

### 4. Group Enrollment Management
```csharp
public async Task<EnrollmentSummary> GetEnrollmentSummaryAsync()
{
    var groups = await _client.Groups()
        .Where(g => g.PublicChurchCenterWebEnabled == true)
        .GetAllAsync();
    
    return new EnrollmentSummary
    {
        OpenGroups = groups.Count(g => g.Enrollment == "open"),
        RequestToJoinGroups = groups.Count(g => g.Enrollment == "request_to_join"),
        ClosedGroups = groups.Count(g => g.Enrollment == "closed"),
        GroupsWithAvailableSpots = groups.Count(g => 
            g.MembershipsLimit == null || g.MembershipsCount < g.MembershipsLimit),
        GroupsAtCapacity = groups.Count(g => 
            g.MembershipsLimit.HasValue && g.MembershipsCount >= g.MembershipsLimit)
    };
}

public class EnrollmentSummary
{
    public int OpenGroups { get; set; }
    public int RequestToJoinGroups { get; set; }
    public int ClosedGroups { get; set; }
    public int GroupsWithAvailableSpots { get; set; }
    public int GroupsAtCapacity { get; set; }
}
```

### 5. Group Leader Dashboard
```csharp
public async Task<LeaderDashboard> GetLeaderDashboardAsync(string personId)
{
    var leaderMemberships = await _client.Groups()
        .Memberships()
        .Where(m => m.PersonId == personId)
        .Where(m => m.Role == "leader")
        .Where(m => m.JoinedAt.HasValue)
        .GetAllAsync();
    
    var groupIds = leaderMemberships.Select(m => m.GroupId).ToList();
    var dashboard = new LeaderDashboard();
    
    foreach (var groupId in groupIds)
    {
        var group = await _client.Groups().GetAsync(groupId);
        var members = await _client.Groups()
            .Memberships()
            .Where(m => m.GroupId == groupId)
            .Where(m => m.JoinedAt.HasValue)
            .CountAsync();
        
        var pendingRequests = await _client.Groups()
            .Memberships()
            .Where(m => m.GroupId == groupId)
            .Where(m => !m.JoinedAt.HasValue)
            .CountAsync();
        
        dashboard.Groups.Add(new LeaderGroupInfo
        {
            GroupId = groupId,
            GroupName = group?.Name ?? "",
            MemberCount = members,
            PendingRequests = pendingRequests
        });
    }
    
    return dashboard;
}

public class LeaderDashboard
{
    public List<LeaderGroupInfo> Groups { get; set; } = new();
    public int TotalGroupsLed => Groups.Count;
    public int TotalMembers => Groups.Sum(g => g.MemberCount);
    public int TotalPendingRequests => Groups.Sum(g => g.PendingRequests);
}

public class LeaderGroupInfo
{
    public string GroupId { get; set; } = "";
    public string GroupName { get; set; } = "";
    public int MemberCount { get; set; }
    public int PendingRequests { get; set; }
}
```

### 6. Group Data Export
```csharp
public async Task ExportGroupDataAsync(string filePath)
{
    using var writer = new StreamWriter(filePath);
    await writer.WriteLineAsync("GroupName,GroupType,Enrollment,MemberCount,MemberLimit,Schedule,ContactEmail");
    
    await foreach (var group in _client.Groups()
        .Include(g => g.GroupType)
        .OrderBy(g => g.Name)
        .AsAsyncEnumerable())
    {
        var line = $"\"{group.Name}\"," +
                   $"\"{group.GroupType?.Name}\"," +
                   $"{group.Enrollment}," +
                   $"{group.MembershipsCount}," +
                   $"{group.MembershipsLimit?.ToString() ?? ""}," +
                   $"\"{group.Schedule}\"," +
                   $"{group.ContactEmail}";
        
        await writer.WriteLineAsync(line);
    }
}
```

## 📊 Advanced Features

### Group Analytics
```csharp
// Calculate group growth over time
public async Task<GroupGrowthReport> CalculateGroupGrowthAsync(DateTime startDate, DateTime endDate)
{
    var memberships = await _client.Groups()
        .Memberships()
        .Where(m => m.JoinedAt >= startDate)
        .Where(m => m.JoinedAt <= endDate)
        .Where(m => m.JoinedAt.HasValue)
        .GetAllAsync();
    
    var monthlyGrowth = memberships
        .GroupBy(m => new { m.JoinedAt.Value.Year, m.JoinedAt.Value.Month })
        .Select(g => new MonthlyGrowth
        {
            Year = g.Key.Year,
            Month = g.Key.Month,
            NewMembers = g.Count()
        })
        .OrderBy(mg => mg.Year)
        .ThenBy(mg => mg.Month)
        .ToList();
    
    return new GroupGrowthReport
    {
        Period = $"{startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}",
        TotalNewMembers = memberships.Count,
        MonthlyBreakdown = monthlyGrowth
    };
}

public class GroupGrowthReport
{
    public string Period { get; set; } = "";
    public int TotalNewMembers { get; set; }
    public List<MonthlyGrowth> MonthlyBreakdown { get; set; } = new();
}

public class MonthlyGrowth
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int NewMembers { get; set; }
}
```

### Batch Operations
```csharp
// Create multiple groups in a batch
var batch = _client.Groups().Batch();

batch.CreateGroup(new GroupCreateRequest
{
    Name = "Men's Bible Study",
    GroupTypeId = "gt_bible_study",
    Enrollment = "open"
});

batch.CreateGroup(new GroupCreateRequest
{
    Name = "Women's Bible Study",
    GroupTypeId = "gt_bible_study",
    Enrollment = "open"
});

batch.CreateMembership(new MembershipCreateRequest
{
    GroupId = "group123",
    PersonId = "12345",
    Role = "member"
});

var results = await batch.ExecuteAsync();
```

### Performance Monitoring
```csharp
// Monitor query performance
var result = await _client.Groups()
    .Where(g => g.Enrollment == "open")
    .Include(g => g.Memberships)
    .ExecuteWithDebugInfoAsync();

Console.WriteLine($"Query took {result.ExecutionTime.TotalMilliseconds}ms");
Console.WriteLine($"Returned {result.Data?.Data.Count ?? 0} groups");
```

## 🎯 Best Practices

1. **Filter by Enrollment Status**: Always consider enrollment status when displaying groups to users.

2. **Check Membership Limits**: Verify available spots before allowing new members to join.

3. **Use Group Types**: Organize groups with appropriate group types for better categorization.

4. **Track Attendance**: Regularly record attendance for group engagement insights.

5. **Manage Pending Requests**: Process membership requests promptly for better user experience.

6. **Stream Large Datasets**: Use streaming for reports with many groups or memberships.

7. **Include Related Data**: Use `Include()` to fetch related data efficiently.

8. **Monitor Group Health**: Track membership trends and attendance patterns.

9. **Handle Role Changes**: Properly manage leadership transitions and role updates.

10. **Validate Permissions**: Ensure users have appropriate access to group data.

### Error Handling
```csharp
public async Task<Group?> SafeCreateGroupAsync(GroupCreateRequest request)
{
    try
    {
        return await _client.Groups().CreateGroup(request).ExecuteAsync();
    }
    catch (PlanningCenterApiValidationException ex)
    {
        // Handle validation errors (e.g., invalid group type, missing required fields)
        _logger.LogWarning(ex, "Validation error when creating group: {Errors}", ex.FormattedErrors);
        throw;
    }
    catch (PlanningCenterApiAuthorizationException ex)
    {
        // Handle permission errors
        _logger.LogError(ex, "Authorization error when creating group: {Message}", ex.Message);
        throw;
    }
    catch (PlanningCenterApiException ex)
    {
        // Log API error with correlation ID
        _logger.LogError(ex, "API error when creating group: {ErrorMessage} [RequestId: {RequestId}]", 
            ex.Message, ex.RequestId);
        throw;
    }
}
```

This Groups module documentation provides comprehensive coverage of group management capabilities with both traditional and fluent API usage patterns, including practical examples for common group administration scenarios.