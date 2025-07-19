# Services Module Documentation

The Services module provides comprehensive service planning and worship management capabilities for Planning Center. This module handles all service-related functionality including service plans, items, songs, and service types.

## 📋 Module Overview

### Key Entities
- **Plans**: Service plans with scheduling and team assignments
- **Items**: Individual elements within a service (songs, announcements, etc.)
- **Songs**: Musical pieces with arrangements and attachments
- **Service Types**: Categories of services (Sunday morning, evening, etc.)
- **Teams**: Groups of people assigned to serve in services
- **People**: Individuals involved in service planning and execution

### Authentication
Requires Planning Center Services app access with appropriate permissions.

## 🔧 Traditional Service API

### Plan Management

#### Basic Plan Operations
```csharp
public class ServicesService
{
    private readonly IServicesService _servicesService;
    
    public ServicesService(IServicesService servicesService)
    {
        _servicesService = servicesService;
    }
    
    // Get a plan by ID
    public async Task<Plan?> GetPlanAsync(string id)
    {
        return await _servicesService.GetPlanAsync(id);
    }
    
    // List plans with pagination
    public async Task<IPagedResponse<Plan>> GetPlansAsync()
    {
        return await _servicesService.ListPlansAsync(new QueryParameters
        {
            PerPage = 25,
            OrderBy = "-sort_date"
        });
    }
}
```

#### Create and Update Plans
```csharp
// Create a new plan
var newPlan = await _servicesService.CreatePlanAsync(new PlanCreateRequest
{
    ServiceTypeId = "st_123",
    Title = "Sunday Morning Worship",
    SortDate = DateTime.Now.AddDays(7).Date,
    PlanningCenterUrl = "https://services.planningcenteronline.com/plans/12345"
});

// Update a plan
var updatedPlan = await _servicesService.UpdatePlanAsync("plan123", new PlanUpdateRequest
{
    Title = "Sunday Morning Celebration",
    Notes = "Special guest speaker this week"
});
```

### Item Management

#### Item Operations
```csharp
// Get an item by ID
var item = await _servicesService.GetItemAsync("item123");

// List items for a plan
var planItems = await _servicesService.ListItemsAsync(new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["plan_id"] = "plan123"
    },
    OrderBy = "sequence"
});

// Create a new item
var newItem = await _servicesService.CreateItemAsync(new ItemCreateRequest
{
    PlanId = "plan123",
    Title = "Welcome & Announcements",
    ItemType = "item",
    Sequence = 1,
    Length = TimeSpan.FromMinutes(5)
});

// Update an item
var updatedItem = await _servicesService.UpdateItemAsync("item123", new ItemUpdateRequest
{
    Title = "Welcome & Community Updates",
    Length = TimeSpan.FromMinutes(7)
});
```

### Song Management

#### Song Operations
```csharp
// Get a song by ID
var song = await _servicesService.GetSongAsync("song123");

// List songs
var songs = await _servicesService.ListSongsAsync(new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["themes"] = "worship"
    },
    OrderBy = "title"
});

// Create a new song
var newSong = await _servicesService.CreateSongAsync(new SongCreateRequest
{
    Title = "Amazing Grace",
    Author = "John Newton",
    Copyright = "Public Domain",
    Themes = new[] { "grace", "salvation", "worship" }
});
```

## 🚀 Fluent API Usage

### Basic Queries
```csharp
public class ServicesFluentService
{
    private readonly IPlanningCenterClient _client;
    
    public ServicesFluentService(IPlanningCenterClient client)
    {
        _client = client;
    }
    
    // Get upcoming plans
    public async Task<IReadOnlyList<Plan>> GetUpcomingPlansAsync()
    {
        return await _client.Services()
            .Where(p => p.SortDate >= DateTime.Today)
            .OrderBy(p => p.SortDate)
            .GetAllAsync();
    }
}
```

### Advanced Plan Filtering
```csharp
// Get plans for specific service type
var sundayPlans = await _client.Services()
    .Where(p => p.ServiceTypeId == "st_sunday")
    .Where(p => p.SortDate >= DateTime.Today)
    .OrderBy(p => p.SortDate)
    .GetAllAsync();

// Get plans needing attention
var incompletePlans = await _client.Services()
    .Where(p => p.PlanNotesCount == 0)
    .Where(p => p.SortDate >= DateTime.Today)
    .OrderBy(p => p.SortDate)
    .GetPagedAsync(pageSize: 50);
```

### Song and Item Filtering
```csharp
// Get worship songs
var worshipSongs = await _client.Services()
    .Songs()
    .Where(s => s.Themes.Contains("worship"))
    .OrderBy(s => s.Title)
    .GetAllAsync();

// Get plan items by type
var songItems = await _client.Services()
    .Items()
    .Where(i => i.PlanId == "plan123")
    .Where(i => i.ItemType == "song")
    .OrderBy(i => i.Sequence)
    .GetAllAsync();
```

## 💡 Common Use Cases

### 1. Service Planning Dashboard
```csharp
public async Task<ServicePlanningDashboard> GetPlanningDashboardAsync()
{
    var upcomingPlans = await _client.Services()
        .Where(p => p.SortDate >= DateTime.Today)
        .Where(p => p.SortDate <= DateTime.Today.AddDays(30))
        .OrderBy(p => p.SortDate)
        .GetAllAsync();
    
    return new ServicePlanningDashboard
    {
        UpcomingPlans = upcomingPlans.Count,
        PlansThisWeek = upcomingPlans.Count(p => p.SortDate <= DateTime.Today.AddDays(7)),
        IncompletePlans = upcomingPlans.Count(p => p.PlanNotesCount == 0)
    };
}
```

### 2. Song Usage Report
```csharp
public async Task<SongUsageReport> GenerateSongUsageReportAsync(DateTime startDate, DateTime endDate)
{
    var plans = await _client.Services()
        .Where(p => p.SortDate >= startDate)
        .Where(p => p.SortDate <= endDate)
        .GetAllAsync();
    
    var songUsage = new Dictionary<string, int>();
    
    foreach (var plan in plans)
    {
        var items = await _client.Services()
            .Items()
            .Where(i => i.PlanId == plan.Id)
            .Where(i => i.ItemType == "song")
            .GetAllAsync();
        
        foreach (var item in items)
        {
            if (!string.IsNullOrEmpty(item.Title))
            {
                songUsage[item.Title] = songUsage.GetValueOrDefault(item.Title, 0) + 1;
            }
        }
    }
    
    return new SongUsageReport
    {
        Period = $"{startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}",
        TotalPlans = plans.Count,
        SongUsage = songUsage.OrderByDescending(kvp => kvp.Value).ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
    };
}
```

## 🎯 Best Practices

1. **Order by Date**: Use sort_date for chronological plan ordering
2. **Filter by Service Type**: Separate different types of services
3. **Track Item Sequences**: Maintain proper order of service elements
4. **Monitor Plan Completeness**: Check for missing notes or items
5. **Use Song Themes**: Categorize songs for easier selection

This Services module provides essential worship planning and management capabilities.