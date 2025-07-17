using System.Linq.Expressions;
using PlanningCenter.Api.Client.Fluent.QueryBuilder;
using PlanningCenter.Api.Client.Fluent.Performance;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.Fluent;
using PlanningCenter.Api.Client.Models.Publishing;
using PlanningCenter.Api.Client.Models.Requests;

namespace PlanningCenter.Api.Client.Fluent;

/// <summary>
/// Fluent API context for Planning Center Publishing.
/// Provides a LINQ-like interface for querying and filtering episodes and media content.
/// </summary>
public class PublishingFluentContext : IPublishingFluentContext
{
    private readonly IPublishingService _publishingService;
    private readonly FluentQueryBuilder<Episode> _queryBuilder = new();

    public PublishingFluentContext(IPublishingService publishingService)
    {
        _publishingService = publishingService ?? throw new ArgumentNullException(nameof(publishingService));
    }

    #region Query Building Methods

    public IPublishingFluentContext Where(Expression<Func<Episode, bool>> predicate)
    {
        _queryBuilder.Where(predicate);
        return this;
    }

    public IPublishingFluentContext Include(Expression<Func<Episode, object>> include)
    {
        _queryBuilder.Include(include);
        return this;
    }

    public IPublishingFluentContext OrderBy(Expression<Func<Episode, object>> orderBy)
    {
        _queryBuilder.OrderBy(orderBy);
        return this;
    }

    public IPublishingFluentContext OrderByDescending(Expression<Func<Episode, object>> orderBy)
    {
        _queryBuilder.OrderByDescending(orderBy);
        return this;
    }

    public IPublishingFluentContext ThenBy(Expression<Func<Episode, object>> thenBy)
    {
        _queryBuilder.ThenBy(thenBy);
        return this;
    }

    public IPublishingFluentContext ThenByDescending(Expression<Func<Episode, object>> thenBy)
    {
        _queryBuilder.ThenByDescending(thenBy);
        return this;
    }

    #endregion

    #region Execution Methods

    public async Task<Episode?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID cannot be null or empty", nameof(id));

        return await _publishingService.GetEpisodeAsync(id, cancellationToken);
    }

    public async Task<IPagedResponse<Episode>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = pageSize;
        return await _publishingService.ListEpisodesAsync(parameters, cancellationToken);
    }

    public async Task<IPagedResponse<Episode>> GetPageAsync(int page, int pageSize = 25, CancellationToken cancellationToken = default)
    {
        if (page < 1) throw new ArgumentException("Page must be 1 or greater", nameof(page));
        if (pageSize < 1) throw new ArgumentException("Page size must be 1 or greater", nameof(pageSize));

        var parameters = _queryBuilder.Build();
        parameters.PerPage = pageSize;
        parameters.Offset = (page - 1) * pageSize;
        
        return await _publishingService.ListEpisodesAsync(parameters, cancellationToken);
    }

    public async Task<IReadOnlyList<Episode>> GetAllAsync(PaginationOptions? options = null, CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        return await _publishingService.GetAllEpisodesAsync(parameters, options, cancellationToken);
    }

    public IAsyncEnumerable<Episode> AsAsyncEnumerable(PaginationOptions? options = null, CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        return _publishingService.StreamEpisodesAsync(parameters, options, cancellationToken);
    }

    #endregion

    #region LINQ-like Terminal Operations

    public async Task<Episode> FirstAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 1;
        
        var response = await _publishingService.ListEpisodesAsync(parameters, cancellationToken);
        
        if (!response.Data.Any())
            throw new InvalidOperationException("Sequence contains no elements");
            
        return response.Data.First();
    }

    public async Task<Episode?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 1;
        
        var response = await _publishingService.ListEpisodesAsync(parameters, cancellationToken);
        return response.Data.FirstOrDefault();
    }

    public async Task<Episode> FirstAsync(Expression<Func<Episode, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var contextWithPredicate = Where(predicate);
        return await contextWithPredicate.FirstAsync(cancellationToken);
    }

    public async Task<Episode?> FirstOrDefaultAsync(Expression<Func<Episode, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var contextWithPredicate = Where(predicate);
        return await contextWithPredicate.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Episode> SingleAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 2; // Get 2 to check if there's more than 1
        
        var response = await _publishingService.ListEpisodesAsync(parameters, cancellationToken);
        
        if (!response.Data.Any())
            throw new InvalidOperationException("Sequence contains no elements");
            
        if (response.Data.Count > 1 || (response.Meta?.TotalCount ?? 0) > 1)
            throw new InvalidOperationException("Sequence contains more than one element");
            
        return response.Data.First();
    }

    public async Task<Episode?> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 2; // Get 2 to check if there's more than 1
        
        var response = await _publishingService.ListEpisodesAsync(parameters, cancellationToken);
        
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
        
        var response = await _publishingService.ListEpisodesAsync(parameters, cancellationToken);
        return response.Meta?.TotalCount ?? 0;
    }

    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        var parameters = _queryBuilder.Build();
        parameters.PerPage = 1; // We only need to know if there's at least one
        
        var response = await _publishingService.ListEpisodesAsync(parameters, cancellationToken);
        return response.Data.Any();
    }

    public async Task<bool> AnyAsync(Expression<Func<Episode, bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        
        var contextWithPredicate = Where(predicate);
        return await contextWithPredicate.AnyAsync(cancellationToken);
    }

    #endregion

    #region Publication Status Filters

    /// <summary>
    /// Filters episodes to only include published episodes.
    /// </summary>
    public IPublishingFluentContext Published()
    {
        return Where(e => e.IsPublished);
    }

    /// <summary>
    /// Filters episodes to only include unpublished/draft episodes.
    /// </summary>
    public IPublishingFluentContext Unpublished()
    {
        return Where(e => !e.IsPublished);
    }

    /// <summary>
    /// Filters episodes by status.
    /// </summary>
    public IPublishingFluentContext ByStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
            throw new ArgumentException("Status cannot be null or empty", nameof(status));

        return Where(e => e.Status == status);
    }

    #endregion

    #region Date and Time Filters

    /// <summary>
    /// Filters episodes published after the specified date.
    /// </summary>
    public IPublishingFluentContext PublishedAfter(DateTime date)
    {
        return Where(e => e.PublishedAt != null && e.PublishedAt > date);
    }

    /// <summary>
    /// Filters episodes published before the specified date.
    /// </summary>
    public IPublishingFluentContext PublishedBefore(DateTime date)
    {
        return Where(e => e.PublishedAt != null && e.PublishedAt < date);
    }

    /// <summary>
    /// Filters episodes published within the specified date range.
    /// </summary>
    public IPublishingFluentContext PublishedBetween(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
            throw new ArgumentException("Start date must be before end date");

        return Where(e => e.PublishedAt != null && e.PublishedAt >= startDate && e.PublishedAt <= endDate);
    }

    /// <summary>
    /// Filters episodes published today.
    /// </summary>
    public IPublishingFluentContext PublishedToday()
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);
        return Where(e => e.PublishedAt != null && e.PublishedAt >= today && e.PublishedAt < tomorrow);
    }

    /// <summary>
    /// Filters episodes published this week.
    /// </summary>
    public IPublishingFluentContext PublishedThisWeek()
    {
        var now = DateTime.Now;
        var startOfWeek = now.AddDays(-(int)now.DayOfWeek);
        var endOfWeek = startOfWeek.AddDays(7);
        return PublishedBetween(startOfWeek, endOfWeek);
    }

    /// <summary>
    /// Filters episodes published this month.
    /// </summary>
    public IPublishingFluentContext PublishedThisMonth()
    {
        var now = DateTime.Now;
        var startOfMonth = new DateTime(now.Year, now.Month, 1);
        var endOfMonth = startOfMonth.AddMonths(1);
        return PublishedBetween(startOfMonth, endOfMonth);
    }

    #endregion

    #region Content Type and Media Filters

    /// <summary>
    /// Filters episodes that have video content.
    /// </summary>
    public IPublishingFluentContext WithVideo()
    {
        return Where(e => !string.IsNullOrEmpty(e.VideoUrl));
    }

    /// <summary>
    /// Filters episodes that have audio content.
    /// </summary>
    public IPublishingFluentContext WithAudio()
    {
        return Where(e => !string.IsNullOrEmpty(e.AudioUrl));
    }

    /// <summary>
    /// Filters episodes that have artwork.
    /// </summary>
    public IPublishingFluentContext WithArtwork()
    {
        return Where(e => !string.IsNullOrEmpty(e.ArtworkUrl));
    }

    /// <summary>
    /// Filters episodes that have downloadable video.
    /// </summary>
    public IPublishingFluentContext WithVideoDownload()
    {
        return Where(e => !string.IsNullOrEmpty(e.VideoDownloadUrl));
    }

    /// <summary>
    /// Filters episodes that have downloadable audio.
    /// </summary>
    public IPublishingFluentContext WithAudioDownload()
    {
        return Where(e => !string.IsNullOrEmpty(e.AudioDownloadUrl));
    }

    /// <summary>
    /// Filters episodes by minimum duration in seconds.
    /// </summary>
    public IPublishingFluentContext WithMinimumDuration(int durationInSeconds)
    {
        if (durationInSeconds < 0)
            throw new ArgumentException("Duration cannot be negative", nameof(durationInSeconds));

        return Where(e => e.LengthInSeconds != null && e.LengthInSeconds >= durationInSeconds);
    }

    /// <summary>
    /// Filters episodes by maximum duration in seconds.
    /// </summary>
    public IPublishingFluentContext WithMaximumDuration(int durationInSeconds)
    {
        if (durationInSeconds < 0)
            throw new ArgumentException("Duration cannot be negative", nameof(durationInSeconds));

        return Where(e => e.LengthInSeconds != null && e.LengthInSeconds <= durationInSeconds);
    }

    #endregion

    #region Series and Organization Filters

    /// <summary>
    /// Filters episodes by series ID.
    /// </summary>
    public IPublishingFluentContext BySeries(string seriesId)
    {
        if (string.IsNullOrWhiteSpace(seriesId))
            throw new ArgumentException("Series ID cannot be null or empty", nameof(seriesId));

        return Where(e => e.SeriesId == seriesId);
    }

    /// <summary>
    /// Filters episodes by episode number.
    /// </summary>
    public IPublishingFluentContext ByEpisodeNumber(int episodeNumber)
    {
        if (episodeNumber < 1)
            throw new ArgumentException("Episode number must be positive", nameof(episodeNumber));

        return Where(e => e.EpisodeNumber == episodeNumber);
    }

    /// <summary>
    /// Filters episodes by season number.
    /// </summary>
    public IPublishingFluentContext BySeason(int seasonNumber)
    {
        if (seasonNumber < 1)
            throw new ArgumentException("Season number must be positive", nameof(seasonNumber));

        return Where(e => e.SeasonNumber == seasonNumber);
    }

    #endregion

    #region Search and Tag Filters

    /// <summary>
    /// Filters episodes containing the specified text in the title.
    /// </summary>
    public IPublishingFluentContext ByTitleContains(string titleFragment)
    {
        if (string.IsNullOrWhiteSpace(titleFragment))
            throw new ArgumentException("Title fragment cannot be null or empty", nameof(titleFragment));

        return Where(e => e.Title.Contains(titleFragment));
    }

    /// <summary>
    /// Filters episodes containing the specified text in the description.
    /// </summary>
    public IPublishingFluentContext ByDescriptionContains(string descriptionFragment)
    {
        if (string.IsNullOrWhiteSpace(descriptionFragment))
            throw new ArgumentException("Description fragment cannot be null or empty", nameof(descriptionFragment));

        return Where(e => e.Description != null && e.Description.Contains(descriptionFragment));
    }

    /// <summary>
    /// Filters episodes that have any of the specified tags.
    /// </summary>
    public IPublishingFluentContext WithTags(params string[] tags)
    {
        if (tags == null || tags.Length == 0)
            throw new ArgumentException("At least one tag must be specified", nameof(tags));

        return Where(e => tags.Any(tag => e.Tags.Contains(tag)));
    }

    /// <summary>
    /// Filters episodes that have all of the specified tags.
    /// </summary>
    public IPublishingFluentContext WithAllTags(params string[] tags)
    {
        if (tags == null || tags.Length == 0)
            throw new ArgumentException("At least one tag must be specified", nameof(tags));

        return Where(e => tags.All(tag => e.Tags.Contains(tag)));
    }

    /// <summary>
    /// Filters episodes that belong to any of the specified categories.
    /// </summary>
    public IPublishingFluentContext InCategories(params string[] categories)
    {
        if (categories == null || categories.Length == 0)
            throw new ArgumentException("At least one category must be specified", nameof(categories));

        return Where(e => categories.Any(category => e.Categories.Contains(category)));
    }

    #endregion

    #region Engagement and Analytics Filters

    /// <summary>
    /// Filters episodes with minimum view count.
    /// </summary>
    public IPublishingFluentContext WithMinimumViews(long minimumViews)
    {
        if (minimumViews < 0)
            throw new ArgumentException("Minimum views cannot be negative", nameof(minimumViews));

        return Where(e => e.ViewCount >= minimumViews);
    }

    /// <summary>
    /// Filters episodes with minimum download count.
    /// </summary>
    public IPublishingFluentContext WithMinimumDownloads(long minimumDownloads)
    {
        if (minimumDownloads < 0)
            throw new ArgumentException("Minimum downloads cannot be negative", nameof(minimumDownloads));

        return Where(e => e.DownloadCount >= minimumDownloads);
    }

    /// <summary>
    /// Filters episodes with minimum like count.
    /// </summary>
    public IPublishingFluentContext WithMinimumLikes(long minimumLikes)
    {
        if (minimumLikes < 0)
            throw new ArgumentException("Minimum likes cannot be negative", nameof(minimumLikes));

        return Where(e => e.LikeCount >= minimumLikes);
    }

    /// <summary>
    /// Filters episodes that have any engagement (views, downloads, or likes).
    /// </summary>
    public IPublishingFluentContext WithEngagement()
    {
        return Where(e => e.ViewCount > 0 || e.DownloadCount > 0 || e.LikeCount > 0);
    }

    #endregion

    #region Specialized Operations

    /// <summary>
    /// Gets the total view count across all episodes in the current query.
    /// </summary>
    public async Task<long> TotalViewsAsync(CancellationToken cancellationToken = default)
    {
        var episodes = await GetAllAsync(null, cancellationToken);
        return episodes.Sum(e => e.ViewCount);
    }

    /// <summary>
    /// Gets the total download count across all episodes in the current query.
    /// </summary>
    public async Task<long> TotalDownloadsAsync(CancellationToken cancellationToken = default)
    {
        var episodes = await GetAllAsync(null, cancellationToken);
        return episodes.Sum(e => e.DownloadCount);
    }

    /// <summary>
    /// Gets the total like count across all episodes in the current query.
    /// </summary>
    public async Task<long> TotalLikesAsync(CancellationToken cancellationToken = default)
    {
        var episodes = await GetAllAsync(null, cancellationToken);
        return episodes.Sum(e => e.LikeCount);
    }

    /// <summary>
    /// Gets the average duration in seconds across all episodes in the current query.
    /// </summary>
    public async Task<double> AverageDurationAsync(CancellationToken cancellationToken = default)
    {
        var episodes = await GetAllAsync(null, cancellationToken);
        var episodesWithDuration = episodes.Where(e => e.LengthInSeconds.HasValue).ToList();
        return episodesWithDuration.Any() ? episodesWithDuration.Average(e => e.LengthInSeconds!.Value) : 0;
    }

    /// <summary>
    /// Gets the total duration in seconds across all episodes in the current query.
    /// </summary>
    public async Task<long> TotalDurationAsync(CancellationToken cancellationToken = default)
    {
        var episodes = await GetAllAsync(null, cancellationToken);
        return episodes.Where(e => e.LengthInSeconds.HasValue).Sum(e => e.LengthInSeconds!.Value);
    }

    #endregion
}