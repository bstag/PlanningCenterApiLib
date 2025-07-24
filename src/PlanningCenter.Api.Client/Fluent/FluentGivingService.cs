using Microsoft.Extensions.Logging;
using PlanningCenter.Api.Client.Fluent;
using PlanningCenter.Api.Client.Mapping.Giving;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.Giving;
using PlanningCenter.Api.Client.Models.JsonApi.Giving;
using PlanningCenter.Api.Client.Services;

namespace PlanningCenter.Api.Client.Fluent;

/// <summary>
/// Fluent API service for the Planning Center Giving module.
/// Provides fluent query building capabilities for donations with comprehensive filtering, ordering, and includes.
/// </summary>
public class FluentGivingService : FluentApiServiceBase<Donation, DonationDto>
{
    private const string BaseEndpoint = "/giving/v2";
    
    public FluentGivingService(
        ILogger<FluentGivingService> logger,
        IApiConnection apiConnection)
        : base(logger, apiConnection, $"{BaseEndpoint}/donations", GivingMapper.MapToDomain, "Donation")
    {
    }
    
    /// <summary>
    /// Creates a new fluent query builder for donations.
    /// </summary>
    /// <returns>A fluent query builder instance</returns>
    public override IFluentQueryExecutor<Donation> Query()
    {
        return new FluentDonationQueryBuilder(this, Logger, BaseEndpoint, GivingMapper.MapToDomain);
    }
    
    /// <summary>
    /// Filter donations by person ID.
    /// </summary>
    public IFluentQueryBuilder<Donation> ByPerson(string personId)
    {
        return Where("person_id", personId);
    }
    
    /// <summary>
    /// Filter donations by batch ID.
    /// </summary>
    public IFluentQueryBuilder<Donation> ByBatch(string batchId)
    {
        return Where("batch_id", batchId);
    }
    
    /// <summary>
    /// Filter donations by campus ID.
    /// </summary>
    public IFluentQueryBuilder<Donation> ByCampus(string campusId)
    {
        return Where("campus_id", campusId);
    }
    
    /// <summary>
    /// Filter donations by payment method.
    /// </summary>
    public IFluentQueryBuilder<Donation> ByPaymentMethod(string paymentMethod)
    {
        return Where("payment_method", paymentMethod);
    }
    
    /// <summary>
    /// Filter donations by payment status.
    /// </summary>
    public IFluentQueryBuilder<Donation> ByPaymentStatus(string paymentStatus)
    {
        return Where("payment_status", paymentStatus);
    }
    
    /// <summary>
    /// Filter donations by date range.
    /// </summary>
    public IFluentQueryBuilder<Donation> ByDateRange(DateTime startDate, DateTime endDate)
    {
        return Where(new Dictionary<string, object>
        {
            { "received_at", $"{startDate:yyyy-MM-dd}..{endDate:yyyy-MM-dd}" }
        });
    }
    
    /// <summary>
    /// Filter donations by minimum amount in cents.
    /// </summary>
    public IFluentQueryBuilder<Donation> ByMinAmount(long amountCents)
    {
        return Where("amount_cents", $">={amountCents}");
    }
    
    /// <summary>
    /// Filter donations by maximum amount in cents.
    /// </summary>
    public IFluentQueryBuilder<Donation> ByMaxAmount(long amountCents)
    {
        return Where("amount_cents", $"<={amountCents}");
    }
    
    /// <summary>
    /// Filter refunded donations.
    /// </summary>
    public IFluentQueryBuilder<Donation> RefundedOnly()
    {
        return Where("refunded", true);
    }
    
    /// <summary>
    /// Filter non-refunded donations.
    /// </summary>
    public IFluentQueryBuilder<Donation> NotRefunded()
    {
        return Where("refunded", false);
    }
    
    /// <summary>
    /// Include person information in the response.
    /// </summary>
    public IFluentQueryBuilder<Donation> WithPerson()
    {
        return Include("person");
    }
    
    /// <summary>
    /// Include batch information in the response.
    /// </summary>
    public IFluentQueryBuilder<Donation> WithBatch()
    {
        return Include("batch");
    }
    
    /// <summary>
    /// Include campus information in the response.
    /// </summary>
    public IFluentQueryBuilder<Donation> WithCampus()
    {
        return Include("campus");
    }
    
    /// <summary>
    /// Include payment source information in the response.
    /// </summary>
    public IFluentQueryBuilder<Donation> WithPaymentSource()
    {
        return Include("payment_source");
    }
    
    /// <summary>
    /// Include designations in the response.
    /// </summary>
    public IFluentQueryBuilder<Donation> WithDesignations()
    {
        return Include("designations");
    }
    
    /// <summary>
    /// Include refund information in the response.
    /// </summary>
    public IFluentQueryBuilder<Donation> WithRefund()
    {
        return Include("refund");
    }
    
    /// <summary>
    /// Include all common relationships.
    /// </summary>
    public IFluentQueryBuilder<Donation> WithAllRelationships()
    {
        return Include("person", "batch", "campus", "payment_source", "designations", "refund");
    }
    
    /// <summary>
    /// Order by donation amount (ascending).
    /// </summary>
    public IFluentQueryBuilder<Donation> OrderByAmount()
    {
        return OrderBy("amount_cents");
    }
    
    /// <summary>
    /// Order by donation amount (descending).
    /// </summary>
    public IFluentQueryBuilder<Donation> OrderByAmountDescending()
    {
        return OrderByDescending("amount_cents");
    }
    
    /// <summary>
    /// Order by received date (ascending).
    /// </summary>
    public IFluentQueryBuilder<Donation> OrderByReceivedDate()
    {
        return OrderBy("received_at");
    }
    
    /// <summary>
    /// Order by received date (descending).
    /// </summary>
    public IFluentQueryBuilder<Donation> OrderByReceivedDateDescending()
    {
        return OrderByDescending("received_at");
    }
    
    /// <summary>
    /// Creates a fluent query builder with an initial filter condition.
    /// </summary>
    /// <param name="field">The field to filter by</param>
    /// <param name="value">The value to filter for</param>
    /// <returns>A fluent query builder instance with the filter applied</returns>
    public IFluentQueryBuilder<Donation> Where(string field, object value)
    {
        return Query().Where(field, value);
    }

    /// <summary>
    /// Creates a fluent query builder with multiple filter conditions.
    /// </summary>
    /// <param name="filters">Dictionary of field-value pairs to filter by</param>
    /// <returns>A fluent query builder instance with the filters applied</returns>
    public IFluentQueryBuilder<Donation> Where(Dictionary<string, object> filters)
    {
        return Query().Where(filters);
    }

    /// <summary>
    /// Creates a fluent query builder with included relationships.
    /// </summary>
    /// <param name="relationships">The relationships to include</param>
    /// <returns>A fluent query builder instance with the includes applied</returns>
    public IFluentQueryBuilder<Donation> Include(params string[] relationships)
    {
        return Query().Include(relationships);
    }

    /// <summary>
    /// Creates a fluent query builder with ordering.
    /// </summary>
    /// <param name="field">The field to order by</param>
    /// <param name="descending">Whether to order in descending order</param>
    /// <returns>A fluent query builder instance with the ordering applied</returns>
    public IFluentQueryBuilder<Donation> OrderBy(string field, bool descending = false)
    {
        return Query().OrderBy(field, descending);
    }

    /// <summary>
    /// Creates a fluent query builder with descending ordering.
    /// </summary>
    /// <param name="field">The field to order by</param>
    /// <returns>A fluent query builder instance with the descending ordering applied</returns>
    public IFluentQueryBuilder<Donation> OrderByDescending(string field)
    {
        return Query().OrderByDescending(field);
    }

    /// <summary>
    /// Creates a fluent query builder with a limit on the number of results.
    /// </summary>
    /// <param name="count">The maximum number of results to return</param>
    /// <returns>A fluent query builder instance with the limit applied</returns>
    public IFluentQueryBuilder<Donation> Take(int count)
    {
        return Query().Take(count);
    }

    /// <summary>
    /// Creates a fluent query builder with pagination.
    /// </summary>
    /// <param name="page">The page number (1-based)</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <returns>A fluent query builder instance with the pagination applied</returns>
    public IFluentQueryBuilder<Donation> Page(int page, int pageSize)
    {
        return Query().Page(page, pageSize);
    }

    /// <summary>
    /// Gets all donations without any filters.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A paged response containing all donations</returns>
    public Task<IPagedResponse<Donation>> AllAsync(CancellationToken cancellationToken = default)
    {
        return Query().ExecuteAsync(cancellationToken);
    }

    /// <summary>
    /// Gets the first donation or null if none exist.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The first donation or null</returns>
    public Task<Donation?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        return Query().FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Gets a single donation, throwing an exception if zero or more than one exist.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The single donation</returns>
    /// <exception cref="InvalidOperationException">Thrown when zero or more than one result is found</exception>
    public Task<Donation> SingleAsync(CancellationToken cancellationToken = default)
    {
        return Query().SingleAsync(cancellationToken);
    }

    /// <summary>
    /// Gets a single donation or null if none exist, throwing an exception if more than one exists.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The single donation or null</returns>
    /// <exception cref="InvalidOperationException">Thrown when more than one result is found</exception>
    public Task<Donation?> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        return Query().SingleOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Gets the count of all donations.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The count of donations</returns>
    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return Query().CountAsync(cancellationToken);
    }

    /// <summary>
    /// Checks if any donations exist.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if any donations exist, false otherwise</returns>
    public Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        return Query().AnyAsync(cancellationToken);
    }
}

/// <summary>
/// Concrete implementation of fluent query builder for donations.
/// </summary>
public class FluentDonationQueryBuilder : FluentQueryBuilderBase<Donation, DonationDto>
{
    public FluentDonationQueryBuilder(
        ServiceBase service,
        ILogger logger,
        string baseEndpoint,
        Func<DonationDto, Donation> mapper)
        : base(service, logger, baseEndpoint, mapper)
    {
    }
    
    protected override FluentQueryBuilderBase<Donation, DonationDto> CreateNew()
    {
        return new FluentDonationQueryBuilder(Service, Logger, BaseEndpoint, Mapper);
    }
    
    public override async Task<IPagedResponse<Donation>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (Service is FluentApiServiceBase<Donation, DonationDto> fluentService)
        {
            return await fluentService.ExecuteListResourcesAsync<DonationDto, Donation>(
                $"{BaseEndpoint}/donations",
                Parameters,
                Mapper,
                "FluentQueryDonations",
                cancellationToken);
        }
        
        throw new InvalidOperationException("Service must be a FluentApiServiceBase to execute fluent queries");
    }
}