using System.Linq.Expressions;
using PlanningCenter.Api.Client.Fluent.QueryBuilder;
using PlanningCenter.Api.Client.Fluent.Performance;
using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Core;

using PlanningCenter.Api.Client.Models.Webhooks;
using PlanningCenter.Api.Client.Models.Requests;

namespace PlanningCenter.Api.Client.Fluent;

/// <summary>
/// Fluent API context for Planning Center Webhooks.
/// Provides a LINQ-like interface for querying and filtering webhook subscriptions.
/// </summary>
public class WebhooksFluentContext : IWebhooksFluentContext
{
    private readonly IWebhooksService _webhooksService;
    private readonly FluentQueryBuilder<WebhookSubscription> _queryBuilder = new();

    public WebhooksFluentContext(IWebhooksService webhooksService)
    {
        _webhooksService = webhooksService ?? throw new ArgumentNullException(nameof(webhooksService));
    }

    #region Query Building Methods

    public IWebhooksFluentContext Where(Expression<Func<WebhookSubscription, bool>> predicate)
    {
        _queryBuilder.Where(predicate);
        return this;
    }

    public IWebhooksFluentContext Include(Expression<Func<WebhookSubscription, object>> include)
    {
        _queryBuilder.Include(include);
        return this;
    }

    public IWebhooksFluentContext OrderBy(Expression<Func<WebhookSubscription, object>> orderBy)
    {
        _queryBuilder.OrderBy(orderBy);
        return this;
    }

    public IWebhooksFluentContext OrderByDescending(Expression<Func<WebhookSubscription, object>> orderBy)
    {
        _queryBuilder.OrderByDescending(orderBy);
        return this;
    }

    public IWebhooksFluentContext ThenBy(Expression<Func<WebhookSubscription, object>> thenBy)
    {
        _queryBuilder.ThenBy(thenBy);
        return this;
    }

    public IWebhooksFluentContext ThenByDescending(Expression<Func<WebhookSubscription, object>> thenBy)
    {
        _queryBuilder.ThenByDescending(thenBy);
        return this;
    }

    #endregion

    #region Execution Methods

    public async Task<WebhookSubscription?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID cannot be null or empty", nameof(id));

        return await _webhooksService.GetSubscriptionAsync(id, cancellationToken);
    }

    public async Task<IPagedResponse<WebhookSubscription>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = pageSize;
        var optimizationInfo = _queryBuilder.GetOptimizationInfo();
        
        return await QueryPerformanceMonitor.TrackQueryExecution(
            $"Webhooks.GetPaged[{pageSize}]",
            () => _webhooksService.ListSubscriptionsAsync(parameters, cancellationToken),
            optimizationInfo);
    }

    public async Task<IPagedResponse<WebhookSubscription>> GetPageAsync(int page, int pageSize = 25, CancellationToken cancellationToken = default)
    {
        if (page < 1) throw new ArgumentException("Page must be 1 or greater", nameof(page));
        if (pageSize < 1) throw new ArgumentException("Page size must be 1 or greater", nameof(pageSize));

        var parameters = _queryBuilder.Build();
        parameters.PerPage = pageSize;
        parameters.Offset = (page - 1) * pageSize;
        var optimizationInfo = _queryBuilder.GetOptimizationInfo();
        
        return await QueryPerformanceMonitor.TrackQueryExecution(
            $"Webhooks.GetPage[{page},{pageSize}]",
            () => _webhooksService.ListSubscriptionsAsync(parameters, cancellationToken),
            optimizationInfo);
    }

    public async Task<IReadOnlyList<WebhookSubscription>> GetAllAsync(PaginationOptions? options = null, CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        var optimizationInfo = _queryBuilder.GetOptimizationInfo();
        
        return await QueryPerformanceMonitor.TrackQueryExecution(
            "Webhooks.GetAll",
            () => _webhooksService.GetAllSubscriptionsAsync(parameters, options, cancellationToken),
            optimizationInfo);
    }

    public IAsyncEnumerable<WebhookSubscription> AsAsyncEnumerable(PaginationOptions? options = null, CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        return _webhooksService.StreamSubscriptionsAsync(parameters, options, cancellationToken);
    }

    #endregion

    #region LINQ-like Terminal Operations

    public async Task<WebhookSubscription> FirstAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 1;
        
        var response = await _webhooksService.ListSubscriptionsAsync(parameters, cancellationToken);
        
        if (!response.Data.Any())
            throw new InvalidOperationException("Sequence contains no elements");
            
        return response.Data.First();
    }

    public async Task<WebhookSubscription?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 1;
        
        var response = await _webhooksService.ListSubscriptionsAsync(parameters, cancellationToken);
        return response.Data.FirstOrDefault();
    }

    public async Task<WebhookSubscription> FirstAsync(Expression<Func<WebhookSubscription, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var contextWithPredicate = Where(predicate);
        return await contextWithPredicate.FirstAsync(cancellationToken);
    }

    public async Task<WebhookSubscription?> FirstOrDefaultAsync(Expression<Func<WebhookSubscription, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var contextWithPredicate = Where(predicate);
        return await contextWithPredicate.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<WebhookSubscription> SingleAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 2; // Get 2 to check if there's more than 1
        
        var response = await _webhooksService.ListSubscriptionsAsync(parameters, cancellationToken);
        
        if (!response.Data.Any())
            throw new InvalidOperationException("Sequence contains no elements");
            
        if (response.Data.Count > 1 || (response.Meta?.TotalCount ?? 0) > 1)
            throw new InvalidOperationException("Sequence contains more than one element");
            
        return response.Data.First();
    }

    public async Task<WebhookSubscription?> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 2; // Get 2 to check if there's more than 1
        
        var response = await _webhooksService.ListSubscriptionsAsync(parameters, cancellationToken);
        
        if (!response.Data.Any())
            return null;
            
        if (response.Data.Count > 1 || (response.Meta?.TotalCount ?? 0) > 1)
            throw new InvalidOperationException("Sequence contains more than one element");
            
        return response.Data.First();
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 1; // We only need the count, not the data
        
        var response = await _webhooksService.ListSubscriptionsAsync(parameters, cancellationToken);
        return response.Meta?.TotalCount ?? 0;
    }

    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 1; // We only need to know if there's at least one
        
        var response = await _webhooksService.ListSubscriptionsAsync(parameters, cancellationToken);
        return response.Data.Any();
    }

    public async Task<bool> AnyAsync(Expression<Func<WebhookSubscription, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var contextWithPredicate = Where(predicate);
        return await contextWithPredicate.AnyAsync(cancellationToken);
    }

    public async Task<WebhookSubscription> SingleAsync(Expression<Func<WebhookSubscription, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var contextWithPredicate = Where(predicate);
        return await contextWithPredicate.SingleAsync(cancellationToken);
    }

    public async Task<WebhookSubscription?> SingleOrDefaultAsync(Expression<Func<WebhookSubscription, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var contextWithPredicate = Where(predicate);
        return await contextWithPredicate.SingleOrDefaultAsync(cancellationToken);
    }

    #endregion

    #region Subscription Status Filters

    /// <summary>
    /// Filters webhook subscriptions to only include active subscriptions.
    /// </summary>
    public IWebhooksFluentContext Active()
    {
        return Where(s => s.Active);
    }

    /// <summary>
    /// Filters webhook subscriptions to only include inactive subscriptions.
    /// </summary>
    public IWebhooksFluentContext Inactive()
    {
        return Where(s => !s.Active);
    }

    #endregion

    #region URL and Endpoint Filters

    /// <summary>
    /// Filters webhook subscriptions by URL pattern.
    /// </summary>
    public IWebhooksFluentContext ByUrlContains(string urlFragment)
    {
        if (string.IsNullOrWhiteSpace(urlFragment))
            throw new ArgumentException("URL fragment cannot be null or empty", nameof(urlFragment));

        return Where(s => s.Url.Contains(urlFragment));
    }

    /// <summary>
    /// Filters webhook subscriptions by exact URL.
    /// </summary>
    public IWebhooksFluentContext ByExactUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL cannot be null or empty", nameof(url));

        return Where(s => s.Url == url);
    }

    /// <summary>
    /// Filters webhook subscriptions by URL starting with the specified prefix.
    /// </summary>
    public IWebhooksFluentContext ByUrlStartsWith(string urlPrefix)
    {
        if (string.IsNullOrWhiteSpace(urlPrefix))
            throw new ArgumentException("URL prefix cannot be null or empty", nameof(urlPrefix));

        return Where(s => s.Url.StartsWith(urlPrefix));
    }

    #endregion

    #region Event Type Filters

    /// <summary>
    /// Filters webhook subscriptions by available event ID.
    /// </summary>
    public IWebhooksFluentContext ByEventType(string availableEventId)
    {
        if (string.IsNullOrWhiteSpace(availableEventId))
            throw new ArgumentException("Available event ID cannot be null or empty", nameof(availableEventId));

        return Where(s => s.AvailableEventId == availableEventId);
    }

    /// <summary>
    /// Filters webhook subscriptions by organization ID.
    /// </summary>
    public IWebhooksFluentContext ByOrganization(string organizationId)
    {
        if (string.IsNullOrWhiteSpace(organizationId))
            throw new ArgumentException("Organization ID cannot be null or empty", nameof(organizationId));

        return Where(s => s.OrganizationId == organizationId);
    }

    #endregion

    #region Delivery Status Filters

    /// <summary>
    /// Filters webhook subscriptions that have had recent delivery attempts.
    /// </summary>
    public IWebhooksFluentContext WithRecentDelivery(TimeSpan timeSpan)
    {
        var cutoffTime = DateTime.UtcNow - timeSpan;
        return Where(s => s.LastDeliveryAt != null && s.LastDeliveryAt > cutoffTime);
    }

    /// <summary>
    /// Filters webhook subscriptions by last delivery status.
    /// </summary>
    public IWebhooksFluentContext ByLastDeliveryStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
            throw new ArgumentException("Status cannot be null or empty", nameof(status));

        return Where(s => s.LastDeliveryStatus == status);
    }

    /// <summary>
    /// Filters webhook subscriptions that had successful last delivery.
    /// </summary>
    public IWebhooksFluentContext WithSuccessfulLastDelivery()
    {
        return Where(s => s.LastDeliveryStatusCode >= 200 && s.LastDeliveryStatusCode < 300);
    }

    /// <summary>
    /// Filters webhook subscriptions that had failed last delivery.
    /// </summary>
    public IWebhooksFluentContext WithFailedLastDelivery()
    {
        return Where(s => s.LastDeliveryStatusCode == null || s.LastDeliveryStatusCode < 200 || s.LastDeliveryStatusCode >= 300);
    }

    /// <summary>
    /// Filters webhook subscriptions by minimum success rate percentage.
    /// </summary>
    public IWebhooksFluentContext WithMinimumSuccessRate(double minimumSuccessRate)
    {
        if (minimumSuccessRate < 0 || minimumSuccessRate > 100)
            throw new ArgumentException("Success rate must be between 0 and 100", nameof(minimumSuccessRate));

        return Where(s => s.SuccessRate >= minimumSuccessRate);
    }

    /// <summary>
    /// Filters webhook subscriptions with poor delivery success rate (below threshold).
    /// </summary>
    public IWebhooksFluentContext WithPoorSuccessRate(double threshold = 50.0)
    {
        if (threshold < 0 || threshold > 100)
            throw new ArgumentException("Threshold must be between 0 and 100", nameof(threshold));

        return Where(s => s.SuccessRate < threshold && s.TotalDeliveries > 0);
    }

    #endregion

    #region Performance Filters

    /// <summary>
    /// Filters webhook subscriptions by maximum response time.
    /// </summary>
    public IWebhooksFluentContext WithMaxResponseTime(double maxResponseTimeMs)
    {
        if (maxResponseTimeMs < 0)
            throw new ArgumentException("Response time cannot be negative", nameof(maxResponseTimeMs));

        return Where(s => s.LastDeliveryResponseTimeMs != null && s.LastDeliveryResponseTimeMs <= maxResponseTimeMs);
    }

    /// <summary>
    /// Filters webhook subscriptions that are responding slowly.
    /// </summary>
    public IWebhooksFluentContext SlowResponding(double thresholdMs = 5000)
    {
        if (thresholdMs < 0)
            throw new ArgumentException("Threshold cannot be negative", nameof(thresholdMs));

        return Where(s => s.LastDeliveryResponseTimeMs != null && s.LastDeliveryResponseTimeMs > thresholdMs);
    }

    /// <summary>
    /// Filters webhook subscriptions that are responding quickly.
    /// </summary>
    public IWebhooksFluentContext FastResponding(double thresholdMs = 1000)
    {
        if (thresholdMs < 0)
            throw new ArgumentException("Threshold cannot be negative", nameof(thresholdMs));

        return Where(s => s.LastDeliveryResponseTimeMs != null && s.LastDeliveryResponseTimeMs <= thresholdMs);
    }

    #endregion

    #region Delivery Volume Filters

    /// <summary>
    /// Filters webhook subscriptions with minimum total deliveries.
    /// </summary>
    public IWebhooksFluentContext WithMinimumDeliveries(long minimumDeliveries)
    {
        if (minimumDeliveries < 0)
            throw new ArgumentException("Minimum deliveries cannot be negative", nameof(minimumDeliveries));

        return Where(s => s.TotalDeliveries >= minimumDeliveries);
    }

    /// <summary>
    /// Filters webhook subscriptions with minimum failed deliveries.
    /// </summary>
    public IWebhooksFluentContext WithMinimumFailures(long minimumFailures)
    {
        if (minimumFailures < 0)
            throw new ArgumentException("Minimum failures cannot be negative", nameof(minimumFailures));

        return Where(s => s.FailedDeliveries >= minimumFailures);
    }

    /// <summary>
    /// Filters webhook subscriptions that have had no deliveries.
    /// </summary>
    public IWebhooksFluentContext WithoutDeliveries()
    {
        return Where(s => s.TotalDeliveries == 0);
    }

    /// <summary>
    /// Filters webhook subscriptions that have had deliveries.
    /// </summary>
    public IWebhooksFluentContext WithDeliveries()
    {
        return Where(s => s.TotalDeliveries > 0);
    }

    #endregion

    #region Configuration Filters

    /// <summary>
    /// Filters webhook subscriptions by maximum retry attempts.
    /// </summary>
    public IWebhooksFluentContext WithMaxRetries(int maxRetries)
    {
        if (maxRetries < 0)
            throw new ArgumentException("Max retries cannot be negative", nameof(maxRetries));

        return Where(s => s.MaxRetries == maxRetries);
    }

    /// <summary>
    /// Filters webhook subscriptions by timeout setting.
    /// </summary>
    public IWebhooksFluentContext WithTimeout(int timeoutSeconds)
    {
        if (timeoutSeconds < 0)
            throw new ArgumentException("Timeout cannot be negative", nameof(timeoutSeconds));

        return Where(s => s.TimeoutSeconds == timeoutSeconds);
    }

    /// <summary>
    /// Filters webhook subscriptions that have custom headers configured.
    /// </summary>
    public IWebhooksFluentContext WithCustomHeaders()
    {
        return Where(s => s.CustomHeaders.Count > 0);
    }

    /// <summary>
    /// Filters webhook subscriptions that have a secret configured.
    /// </summary>
    public IWebhooksFluentContext WithSecret()
    {
        return Where(s => !string.IsNullOrEmpty(s.Secret));
    }

    /// <summary>
    /// Filters webhook subscriptions that do not have a secret configured.
    /// </summary>
    public IWebhooksFluentContext WithoutSecret()
    {
        return Where(s => string.IsNullOrEmpty(s.Secret));
    }

    #endregion

    #region Search Filters

    /// <summary>
    /// Filters webhook subscriptions containing the specified text in the name.
    /// </summary>
    public IWebhooksFluentContext ByNameContains(string nameFragment)
    {
        if (string.IsNullOrWhiteSpace(nameFragment))
            throw new ArgumentException("Name fragment cannot be null or empty", nameof(nameFragment));

        return Where(s => s.Name != null && s.Name.Contains(nameFragment));
    }

    /// <summary>
    /// Filters webhook subscriptions containing the specified text in the description.
    /// </summary>
    public IWebhooksFluentContext ByDescriptionContains(string descriptionFragment)
    {
        if (string.IsNullOrWhiteSpace(descriptionFragment))
            throw new ArgumentException("Description fragment cannot be null or empty", nameof(descriptionFragment));

        return Where(s => s.Description != null && s.Description.Contains(descriptionFragment));
    }

    #endregion

    #region Specialized Operations

    /// <summary>
    /// Gets the total number of deliveries across all webhook subscriptions in the current query.
    /// </summary>
    public async Task<long> TotalDeliveriesAsync(CancellationToken cancellationToken = default)
    {
        var optimizationInfo = _queryBuilder.GetOptimizationInfo();
        return await QueryPerformanceMonitor.TrackQueryExecution(
            "Webhooks.TotalDeliveries",
            async () =>
            {
                var subscriptions = await GetAllAsync(null, cancellationToken);
                return subscriptions.Sum(s => s.TotalDeliveries);
            },
            optimizationInfo);
    }

    /// <summary>
    /// Gets the total number of successful deliveries across all webhook subscriptions in the current query.
    /// </summary>
    public async Task<long> TotalSuccessfulDeliveriesAsync(CancellationToken cancellationToken = default)
    {
        var subscriptions = await GetAllAsync(null, cancellationToken);
        return subscriptions.Sum(s => s.SuccessfulDeliveries);
    }

    /// <summary>
    /// Gets the total number of failed deliveries across all webhook subscriptions in the current query.
    /// </summary>
    public async Task<long> TotalFailedDeliveriesAsync(CancellationToken cancellationToken = default)
    {
        var subscriptions = await GetAllAsync(null, cancellationToken);
        return subscriptions.Sum(s => s.FailedDeliveries);
    }

    /// <summary>
    /// Gets the overall success rate across all webhook subscriptions in the current query.
    /// </summary>
    public async Task<double> OverallSuccessRateAsync(CancellationToken cancellationToken = default)
    {
        var optimizationInfo = _queryBuilder.GetOptimizationInfo();
        return await QueryPerformanceMonitor.TrackQueryExecution(
            "Webhooks.OverallSuccessRate",
            async () =>
            {
                var subscriptions = await GetAllAsync(null, cancellationToken);
                var totalDeliveries = subscriptions.Sum(s => s.TotalDeliveries);
                var totalSuccessful = subscriptions.Sum(s => s.SuccessfulDeliveries);
                return totalDeliveries > 0 ? (double)totalSuccessful / totalDeliveries * 100 : 0;
            },
            optimizationInfo);
    }

    /// <summary>
    /// Gets the average response time across all webhook subscriptions in the current query.
    /// </summary>
    public async Task<double> AverageResponseTimeAsync(CancellationToken cancellationToken = default)
    {
        var subscriptions = await GetAllAsync(null, cancellationToken);
        var subscriptionsWithResponseTime = subscriptions.Where(s => s.LastDeliveryResponseTimeMs.HasValue).ToList();
        return subscriptionsWithResponseTime.Any() ? subscriptionsWithResponseTime.Average(s => s.LastDeliveryResponseTimeMs!.Value) : 0;
    }

    /// <summary>
    /// Gets the count of active webhook subscriptions in the current query.
    /// </summary>
    public async Task<int> ActiveCountAsync(CancellationToken cancellationToken = default)
    {
        var subscriptions = await GetAllAsync(null, cancellationToken);
        return subscriptions.Count(s => s.Active);
    }

    /// <summary>
    /// Gets the count of inactive webhook subscriptions in the current query.
    /// </summary>
    public async Task<int> InactiveCountAsync(CancellationToken cancellationToken = default)
    {
        var subscriptions = await GetAllAsync(null, cancellationToken);
        return subscriptions.Count(s => !s.Active);
    }

    #endregion
}