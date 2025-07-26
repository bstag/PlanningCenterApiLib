using Microsoft.Extensions.Logging;
using PlanningCenter.Api.Client.Fluent;
using PlanningCenter.Api.Client.Mapping.Calendar;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Calendar;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.JsonApi.Calendar;
using PlanningCenter.Api.Client.Services;

namespace PlanningCenter.Api.Client.Fluent;

/// <summary>
/// Fluent Calendar API service providing comprehensive query building capabilities for events and resources.
/// </summary>
public class FluentCalendarService : FluentApiServiceBase<Event, EventDto>, IFluentCalendarService
{
    private new const string BaseEndpoint = "/calendar/v2";
    
    /// <summary>
    /// Initializes a new instance of the FluentCalendarService class.
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="apiConnection">The API connection instance</param>
    public FluentCalendarService(
        ILogger<FluentCalendarService> logger,
        IApiConnection apiConnection)
        : base(logger, apiConnection, $"{BaseEndpoint}/events", CalendarMapper.MapToDomain, "Event")
    {
    }
    
    /// <summary>
    /// Creates a new fluent query builder for events.
    /// </summary>
    /// <returns>A fluent query builder instance</returns>
    public override IFluentQueryExecutor<Event> Query()
    {
        return new FluentEventQueryBuilder(this, Logger, BaseEndpoint, CalendarMapper.MapToDomain);
    }

    #region Event Filtering Methods

    /// <summary>
    /// Filter events by name.
    /// </summary>
    /// <param name="name">The event name to filter by</param>
    /// <returns>A fluent query builder instance with the filter applied</returns>
    public IFluentQueryBuilder<Event> ByName(string name)
    {
        return Query().Where("name", name);
    }

    /// <summary>
    /// Filter events by date range.
    /// </summary>
    /// <param name="startDate">The start date</param>
    /// <param name="endDate">The end date</param>
    /// <returns>A fluent query builder instance with the filter applied</returns>
    public IFluentQueryBuilder<Event> ByDateRange(DateTime startDate, DateTime endDate)
    {
        return Query()
            .Where("starts_at", $">={startDate:yyyy-MM-dd}")
            .Where("ends_at", $"<={endDate:yyyy-MM-dd}");
    }

    /// <summary>
    /// Filter events by location.
    /// </summary>
    /// <param name="location">The location name to filter by</param>
    /// <returns>A fluent query builder instance with the filter applied</returns>
    public IFluentQueryBuilder<Event> ByLocation(string location)
    {
        return Query().Where("location_name", location);
    }

    /// <summary>
    /// Filter events by approval status.
    /// </summary>
    /// <param name="approvalStatus">The approval status to filter by</param>
    /// <returns>A fluent query builder instance with the filter applied</returns>
    public IFluentQueryBuilder<Event> ByApprovalStatus(string approvalStatus)
    {
        return Query().Where("approval_status", approvalStatus);
    }

    /// <summary>
    /// Filter events by visibility in Church Center.
    /// </summary>
    /// <param name="visible">Whether the event is visible in Church Center</param>
    /// <returns>A fluent query builder instance with the filter applied</returns>
    public IFluentQueryBuilder<Event> ByChurchCenterVisibility(bool visible)
    {
        return Query().Where("visible_in_church_center", visible);
    }

    /// <summary>
    /// Filter events by registration requirement.
    /// </summary>
    /// <param name="required">Whether registration is required</param>
    /// <returns>A fluent query builder instance with the filter applied</returns>
    public IFluentQueryBuilder<Event> ByRegistrationRequired(bool required)
    {
        return Query().Where("registration_required", required);
    }

    /// <summary>
    /// Filter events by owner.
    /// </summary>
    /// <param name="ownerName">The owner name to filter by</param>
    /// <returns>A fluent query builder instance with the filter applied</returns>
    public IFluentQueryBuilder<Event> ByOwner(string ownerName)
    {
        return Query().Where("owner_name", ownerName);
    }

    /// <summary>
    /// Filter all-day events.
    /// </summary>
    /// <param name="allDay">Whether to filter for all-day events</param>
    /// <returns>A fluent query builder instance with the filter applied</returns>
    public IFluentQueryBuilder<Event> ByAllDay(bool allDay)
    {
        return Query().Where("all_day_event", allDay);
    }

    #endregion

    #region Event Include Methods

    /// <summary>
    /// Include event instances in the response.
    /// </summary>
    /// <returns>A fluent query builder instance with the include applied</returns>
    public IFluentQueryBuilder<Event> WithEventInstances()
    {
        return Query().Include("event_instances");
    }

    /// <summary>
    /// Include event resource requests in the response.
    /// </summary>
    /// <returns>A fluent query builder instance with the include applied</returns>
    public IFluentQueryBuilder<Event> WithResourceRequests()
    {
        return Query().Include("event_resource_requests");
    }

    /// <summary>
    /// Include owner information in the response.
    /// </summary>
    /// <returns>A fluent query builder instance with the include applied</returns>
    public IFluentQueryBuilder<Event> WithOwner()
    {
        return Query().Include("owner");
    }

    /// <summary>
    /// Include tags in the response.
    /// </summary>
    /// <returns>A fluent query builder instance with the include applied</returns>
    public IFluentQueryBuilder<Event> WithTags()
    {
        return Query().Include("tags");
    }

    /// <summary>
    /// Include all common relationships.
    /// </summary>
    /// <returns>A fluent query builder instance with all includes applied</returns>
    public IFluentQueryBuilder<Event> WithAllRelationships()
    {
        return Query().Include("event_instances", "event_resource_requests", "owner", "tags");
    }

    #endregion

    #region Event Ordering Methods

    /// <summary>
    /// Order events by start date in ascending order.
    /// </summary>
    /// <returns>A fluent query builder instance with the ordering applied</returns>
    public IFluentQueryBuilder<Event> OrderByStartDate()
    {
        return Query().OrderBy("starts_at");
    }

    /// <summary>
    /// Order events by start date in descending order.
    /// </summary>
    /// <returns>A fluent query builder instance with the ordering applied</returns>
    public IFluentQueryBuilder<Event> OrderByStartDateDescending()
    {
        return Query().OrderByDescending("starts_at");
    }

    /// <summary>
    /// Order events by name in ascending order.
    /// </summary>
    /// <returns>A fluent query builder instance with the ordering applied</returns>
    public IFluentQueryBuilder<Event> OrderByName()
    {
        return Query().OrderBy("name");
    }

    /// <summary>
    /// Order events by name in descending order.
    /// </summary>
    /// <returns>A fluent query builder instance with the ordering applied</returns>
    public IFluentQueryBuilder<Event> OrderByNameDescending()
    {
        return Query().OrderByDescending("name");
    }

    /// <summary>
    /// Order events by creation date in ascending order.
    /// </summary>
    /// <returns>A fluent query builder instance with the ordering applied</returns>
    public IFluentQueryBuilder<Event> OrderByCreatedDate()
    {
        return Query().OrderBy("created_at");
    }

    /// <summary>
    /// Order events by creation date in descending order.
    /// </summary>
    /// <returns>A fluent query builder instance with the ordering applied</returns>
    public IFluentQueryBuilder<Event> OrderByCreatedDateDescending()
    {
        return Query().OrderByDescending("created_at");
    }

    #endregion

    #region Resource Query Methods

    /// <summary>
    /// Creates a new fluent query builder for resources.
    /// </summary>
    /// <returns>A fluent query builder instance for resources</returns>
    public IFluentQueryExecutor<Resource> QueryResources()
    {
        return new FluentResourceQueryBuilder(this, Logger, BaseEndpoint, CalendarMapper.MapResourceToDomain);
    }

    /// <summary>
    /// Filter resources by name.
    /// </summary>
    /// <param name="name">The resource name to filter by</param>
    /// <returns>A fluent query builder instance with the filter applied</returns>
    public IFluentQueryBuilder<Resource> ResourcesByName(string name)
    {
        return QueryResources().Where("name", name);
    }

    /// <summary>
    /// Filter resources by kind.
    /// </summary>
    /// <param name="kind">The resource kind to filter by</param>
    /// <returns>A fluent query builder instance with the filter applied</returns>
    public IFluentQueryBuilder<Resource> ResourcesByKind(string kind)
    {
        return QueryResources().Where("kind", kind);
    }

    /// <summary>
    /// Filter resources by home location.
    /// </summary>
    /// <param name="location">The home location to filter by</param>
    /// <returns>A fluent query builder instance with the filter applied</returns>
    public IFluentQueryBuilder<Resource> ResourcesByLocation(string location)
    {
        return QueryResources().Where("home_location", location);
    }

    /// <summary>
    /// Filter resources by expiration status.
    /// </summary>
    /// <param name="expires">Whether the resource expires</param>
    /// <returns>A fluent query builder instance with the filter applied</returns>
    public IFluentQueryBuilder<Resource> ResourcesByExpiration(bool expires)
    {
        return QueryResources().Where("expires", expires);
    }

    /// <summary>
    /// Filter resources by minimum quantity.
    /// </summary>
    /// <param name="minQuantity">The minimum quantity</param>
    /// <returns>A fluent query builder instance with the filter applied</returns>
    public IFluentQueryBuilder<Resource> ResourcesByMinQuantity(int minQuantity)
    {
        return QueryResources().Where("quantity", $">={minQuantity}");
    }

    #endregion

    #region Base Query Methods

    /// <summary>
    /// Creates a fluent query builder with an initial filter condition.
    /// </summary>
    /// <param name="field">The field to filter by</param>
    /// <param name="value">The value to filter for</param>
    /// <returns>A fluent query builder instance with the filter applied</returns>
    public new IFluentQueryBuilder<Event> Where(string field, object value)
    {
        return Query().Where(field, value);
    }

    /// <summary>
    /// Creates a fluent query builder with multiple filter conditions.
    /// </summary>
    /// <param name="filters">Dictionary of field-value pairs to filter by</param>
    /// <returns>A fluent query builder instance with the filters applied</returns>
    public new IFluentQueryBuilder<Event> Where(Dictionary<string, object> filters)
    {
        IFluentQueryBuilder<Event> builder = Query();
        foreach (var filter in filters)
        {
            builder = builder.Where(filter.Key, filter.Value);
        }
        return builder;
    }

    /// <summary>
    /// Creates a fluent query builder with included relationships.
    /// </summary>
    /// <param name="relationships">The relationships to include</param>
    /// <returns>A fluent query builder instance with the includes applied</returns>
    public new IFluentQueryBuilder<Event> Include(params string[] relationships)
    {
        return Query().Include(relationships);
    }

    /// <summary>
    /// Creates a fluent query builder with ordering.
    /// </summary>
    /// <param name="field">The field to order by</param>
    /// <param name="descending">Whether to order in descending order</param>
    /// <returns>A fluent query builder instance with the ordering applied</returns>
    public new IFluentQueryBuilder<Event> OrderBy(string field, bool descending = false)
    {
        return Query().OrderBy(field, descending);
    }

    /// <summary>
    /// Creates a fluent query builder with descending ordering.
    /// </summary>
    /// <param name="field">The field to order by</param>
    /// <returns>A fluent query builder instance with the descending ordering applied</returns>
    public new IFluentQueryBuilder<Event> OrderByDescending(string field)
    {
        return Query().OrderByDescending(field);
    }

    /// <summary>
    /// Creates a fluent query builder with a limit on the number of results.
    /// </summary>
    /// <param name="count">The maximum number of results to return</param>
    /// <returns>A fluent query builder instance with the limit applied</returns>
    public new IFluentQueryBuilder<Event> Take(int count)
    {
        return Query().Take(count);
    }

    /// <summary>
    /// Creates a fluent query builder with pagination.
    /// </summary>
    /// <param name="page">The page number (1-based)</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <returns>A fluent query builder instance with the pagination applied</returns>
    public new IFluentQueryBuilder<Event> Page(int page, int pageSize)
    {
        return Query().Page(page, pageSize);
    }

    /// <summary>
    /// Gets all events without any filters.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A paged response containing all events</returns>
    public new Task<IPagedResponse<Event>> AllAsync(CancellationToken cancellationToken = default)
    {
        return Query().ExecuteAsync(cancellationToken);
    }

    /// <summary>
    /// Gets the first event or null if none exist.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The first event or null</returns>
    public new Task<Event?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        return Query().FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Gets a single event, throwing an exception if zero or more than one exist.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The single event</returns>
    /// <exception cref="InvalidOperationException">Thrown when zero or more than one result is found</exception>
    public new Task<Event> SingleAsync(CancellationToken cancellationToken = default)
    {
        return Query().SingleAsync(cancellationToken);
    }

    /// <summary>
    /// Gets a single event or null if none exist, throwing an exception if more than one exists.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The single event or null</returns>
    /// <exception cref="InvalidOperationException">Thrown when more than one result is found</exception>
    public new Task<Event?> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        return Query().SingleOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Gets the count of all events.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The count of events</returns>
    public new Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return Query().CountAsync(cancellationToken);
    }

    /// <summary>
    /// Checks if any events exist.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if any events exist, false otherwise</returns>
    public new Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        return Query().AnyAsync(cancellationToken);
    }

    #endregion
}

/// <summary>
/// Concrete implementation of fluent query builder for events.
/// </summary>
public class FluentEventQueryBuilder : FluentQueryBuilderBase<Event, EventDto>
{
    public FluentEventQueryBuilder(
        ServiceBase service,
        ILogger logger,
        string baseEndpoint,
        Func<EventDto, Event> mapper)
        : base(service, logger, baseEndpoint, mapper)
    {
    }
    
    protected override FluentQueryBuilderBase<Event, EventDto> CreateNew()
    {
        return new FluentEventQueryBuilder(Service, Logger, BaseEndpoint, Mapper);
    }
    
    public override async Task<IPagedResponse<Event>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (Service is FluentApiServiceBase<Event, EventDto> fluentService)
        {
            return await fluentService.ExecuteListResourcesAsync<EventDto, Event>(
                $"{BaseEndpoint}/events",
                Parameters,
                Mapper,
                "FluentQueryEvents",
                cancellationToken);
        }
        
        throw new InvalidOperationException("Service must be a FluentApiServiceBase to execute fluent queries");
    }
}

/// <summary>
/// Concrete implementation of fluent query builder for resources.
/// </summary>
public class FluentResourceQueryBuilder : FluentQueryBuilderBase<Resource, ResourceDto>
{
    public FluentResourceQueryBuilder(
        ServiceBase service,
        ILogger logger,
        string baseEndpoint,
        Func<ResourceDto, Resource> mapper)
        : base(service, logger, baseEndpoint, mapper)
    {
    }
    
    protected override FluentQueryBuilderBase<Resource, ResourceDto> CreateNew()
    {
        return new FluentResourceQueryBuilder(Service, Logger, BaseEndpoint, Mapper);
    }
    
    public override async Task<IPagedResponse<Resource>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (Service is FluentApiServiceBase<Event, EventDto> fluentService)
        {
            return await fluentService.ExecuteListResourcesAsync<ResourceDto, Resource>(
                $"{BaseEndpoint}/resources",
                Parameters,
                Mapper,
                "FluentQueryResources",
                cancellationToken);
        }
        
        throw new InvalidOperationException("Service must be a FluentApiServiceBase to execute fluent queries");
    }
}