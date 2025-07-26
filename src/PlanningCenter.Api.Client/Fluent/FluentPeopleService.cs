using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PlanningCenter.Api.Client.Fluent;
using PlanningCenter.Api.Client.Mapping.People;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.People;
using PlanningCenter.Api.Client.Services;

namespace PlanningCenter.Api.Client.Fluent;

/// <summary>
/// Fluent API service for the Planning Center People module.
/// Provides fluent query building capabilities with comprehensive filtering, ordering, and includes.
/// </summary>
public class FluentPeopleService : FluentApiServiceBase<Person, PersonDto>
{
    private new const string BaseEndpoint = "/people/v2";
    
    public FluentPeopleService(
        ILogger<FluentPeopleService> logger,
        IApiConnection apiConnection)
        : base(logger, apiConnection, $"{BaseEndpoint}/people", PersonMapper.MapToDomain, "Person")
    {
    }
    
    /// <summary>
    /// Creates a new fluent query builder for people.
    /// </summary>
    /// <returns>A fluent query builder instance</returns>
    public override IFluentQueryExecutor<Person> Query()
    {
        return new FluentPeopleQueryBuilder(this, Logger, BaseEndpoint, PersonMapper.MapToDomain);
    }
    
    /// <summary>
    /// Fluent query builder specifically for people with predefined filter methods.
    /// </summary>
    public IFluentQueryBuilder<Person> ByFirstName(string firstName)
    {
        return Where("first_name", firstName);
    }
    
    /// <summary>
    /// Filter people by last name.
    /// </summary>
    public IFluentQueryBuilder<Person> ByLastName(string lastName)
    {
        return Where("last_name", lastName);
    }
    
    /// <summary>
    /// Filter people by status.
    /// </summary>
    public IFluentQueryBuilder<Person> ByStatus(string status)
    {
        return Where("status", status);
    }
    
    /// <summary>
    /// Filter people by membership status.
    /// </summary>
    public IFluentQueryBuilder<Person> ByMembershipStatus(string membershipStatus)
    {
        return Where("membership_status", membershipStatus);
    }
    
    /// <summary>
    /// Filter people by campus.
    /// </summary>
    public IFluentQueryBuilder<Person> ByCampus(string campusId)
    {
        return Where("campus_id", campusId);
    }
    
    /// <summary>
    /// Include addresses in the response.
    /// </summary>
    public IFluentQueryBuilder<Person> WithAddresses()
    {
        return Include("addresses");
    }
    
    /// <summary>
    /// Include emails in the response.
    /// </summary>
    public IFluentQueryBuilder<Person> WithEmails()
    {
        return Include("emails");
    }
    
    /// <summary>
    /// Include phone numbers in the response.
    /// </summary>
    public IFluentQueryBuilder<Person> WithPhoneNumbers()
    {
        return Include("phone_numbers");
    }
    
    /// <summary>
    /// Include households in the response.
    /// </summary>
    public IFluentQueryBuilder<Person> WithHouseholds()
    {
        return Include("households");
    }
    
    /// <summary>
    /// Include all common relationships.
    /// </summary>
    public IFluentQueryBuilder<Person> WithAllRelationships()
    {
        return Include("addresses", "emails", "phone_numbers", "households", "primary_campus");
    }
    
    /// <summary>
    /// Creates a fluent query builder with an initial filter condition.
    /// </summary>
    /// <param name="field">The field to filter by</param>
    /// <param name="value">The value to filter for</param>
    /// <returns>A fluent query builder instance with the filter applied</returns>
    public new IFluentQueryBuilder<Person> Where(string field, object value)
    {
        return Query().Where(field, value);
    }

    /// <summary>
    /// Creates a fluent query builder with multiple filter conditions.
    /// </summary>
    /// <param name="filters">Dictionary of field-value pairs to filter by</param>
    /// <returns>A fluent query builder instance with the filters applied</returns>
    public new IFluentQueryBuilder<Person> Where(Dictionary<string, object> filters)
    {
        return Query().Where(filters);
    }

    /// <summary>
    /// Creates a fluent query builder with included relationships.
    /// </summary>
    /// <param name="relationships">The relationships to include</param>
    /// <returns>A fluent query builder instance with the includes applied</returns>
    public new IFluentQueryBuilder<Person> Include(params string[] relationships)
    {
        return Query().Include(relationships);
    }

    /// <summary>
    /// Creates a fluent query builder with ordering.
    /// </summary>
    /// <param name="field">The field to order by</param>
    /// <param name="descending">Whether to order in descending order</param>
    /// <returns>A fluent query builder instance with the ordering applied</returns>
    public new IFluentQueryBuilder<Person> OrderBy(string field, bool descending = false)
    {
        return Query().OrderBy(field, descending);
    }

    /// <summary>
    /// Creates a fluent query builder with descending ordering.
    /// </summary>
    /// <param name="field">The field to order by</param>
    /// <returns>A fluent query builder instance with the descending ordering applied</returns>
    public new IFluentQueryBuilder<Person> OrderByDescending(string field)
    {
        return Query().OrderByDescending(field);
    }

    /// <summary>
    /// Creates a fluent query builder with a limit on the number of results.
    /// </summary>
    /// <param name="count">The maximum number of results to return</param>
    /// <returns>A fluent query builder instance with the limit applied</returns>
    public new IFluentQueryBuilder<Person> Take(int count)
    {
        return Query().Take(count);
    }

    /// <summary>
    /// Creates a fluent query builder with pagination.
    /// </summary>
    /// <param name="page">The page number (1-based)</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <returns>A fluent query builder instance with the pagination applied</returns>
    public new IFluentQueryBuilder<Person> Page(int page, int pageSize)
    {
        return Query().Page(page, pageSize);
    }

    /// <summary>
    /// Gets all people without any filters.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A paged response containing all people</returns>
    public new Task<IPagedResponse<Person>> AllAsync(CancellationToken cancellationToken = default)
    {
        return Query().ExecuteAsync(cancellationToken);
    }

    /// <summary>
    /// Gets the first person or null if none exist.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The first person or null</returns>
    public new Task<Person?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        return Query().FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Gets a single person, throwing an exception if zero or more than one exist.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The single person</returns>
    /// <exception cref="InvalidOperationException">Thrown when zero or more than one result is found</exception>
    public new Task<Person> SingleAsync(CancellationToken cancellationToken = default)
    {
        return Query().SingleAsync(cancellationToken);
    }

    /// <summary>
    /// Gets a single person or null if none exist, throwing an exception if more than one exists.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The single person or null</returns>
    /// <exception cref="InvalidOperationException">Thrown when more than one result is found</exception>
    public new Task<Person?> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        return Query().SingleOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Gets the count of all people.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The count of people</returns>
    public new Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return Query().CountAsync(cancellationToken);
    }

    /// <summary>
    /// Checks if any people exist.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if any people exist, false otherwise</returns>
    public new Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        return Query().AnyAsync(cancellationToken);
    }
}

/// <summary>
/// Concrete implementation of fluent query builder for people.
/// </summary>
public class FluentPeopleQueryBuilder : FluentQueryBuilderBase<Person, PersonDto>
{
    public FluentPeopleQueryBuilder(
        ServiceBase service,
        ILogger logger,
        string baseEndpoint,
        Func<PersonDto, Person> mapper)
        : base(service, logger, baseEndpoint, mapper)
    {
    }
    
    protected override FluentQueryBuilderBase<Person, PersonDto> CreateNew()
    {
        return new FluentPeopleQueryBuilder(Service, Logger, BaseEndpoint, Mapper);
    }
    
    public override async Task<IPagedResponse<Person>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (Service is FluentApiServiceBase<Person, PersonDto> fluentService)
        {
            return await fluentService.ExecuteListResourcesAsync<PersonDto, Person>(
                $"{BaseEndpoint}/people",
                Parameters,
                Mapper,
                "FluentQueryPeople",
                cancellationToken);
        }
        
        throw new InvalidOperationException("Service must be a FluentApiServiceBase to execute fluent queries");
    }
}