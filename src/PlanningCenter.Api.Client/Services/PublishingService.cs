using Microsoft.Extensions.Logging;
using PlanningCenter.Api.Client.Mapping.Publishing;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Publishing;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Models.Exceptions;
using PlanningCenter.Api.Client.Models.JsonApi;
using PlanningCenter.Api.Client.Models.JsonApi.Publishing;

namespace PlanningCenter.Api.Client.Services;

/// <summary>
/// Service implementation for the Planning Center Publishing module.
/// Provides comprehensive media content and sermon management with built-in pagination support.
/// </summary>
public class PublishingService : ServiceBase, IPublishingService
{
    private const string BaseEndpoint = "/publishing/v2";

    public PublishingService(
        IApiConnection apiConnection,
        ILogger<PublishingService> logger)
        : base(logger, apiConnection)
    {
    }

    #region Episode Management

    /// <summary>
    /// Gets a single episode by ID.
    /// </summary>
    public async Task<Episode?> GetEpisodeAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Episode ID cannot be null or empty", nameof(id));

        Logger.LogDebug("Getting episode with ID: {EpisodeId}", id);

        try
        {
            var response = await ApiConnection.GetAsync<JsonApiSingleResponse<EpisodeDto>>(
                $"{BaseEndpoint}/episodes/{id}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogWarning("Episode not found: {EpisodeId}", id);
                return null;
            }

            var episode = PublishingMapper.MapToDomain(response.Data);

            Logger.LogInformation("Successfully retrieved episode: {EpisodeId}", id);
            return episode;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            Logger.LogWarning("Episode not found: {EpisodeId}", id);
            return null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error retrieving episode: {EpisodeId}", id);
            throw;
        }
    }

    /// <summary>
    /// Lists episodes with optional filtering, sorting, and pagination.
    /// </summary>
    public async Task<IPagedResponse<Episode>> ListEpisodesAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Listing episodes with parameters: {@Parameters}", parameters);

        try
        {
            var queryString = parameters?.ToQueryString() ?? string.Empty;
            
            var response = await ApiConnection.GetAsync<PagedResponse<EpisodeDto>>(
                $"{BaseEndpoint}/episodes{queryString}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogWarning("No episodes returned from API");
                return new PagedResponse<Episode>
                {
                    Data = new List<Episode>(),
                    Meta = new PagedResponseMeta { TotalCount = 0 },
                    Links = new PagedResponseLinks()
                };
            }

            var episodes = response.Data.Select(PublishingMapper.MapToDomain).ToList();
            
            var pagedResponse = new PagedResponse<Episode>
            {
                Data = episodes,
                Meta = response.Meta,
                Links = response.Links
            };

            Logger.LogInformation("Successfully retrieved {Count} episodes", episodes.Count);
            return pagedResponse;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error listing episodes");
            throw;
        }
    }

    /// <summary>
    /// Creates a new episode.
    /// </summary>
    public async Task<Episode> CreateEpisodeAsync(EpisodeCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        Logger.LogDebug("Creating episode with title: {Title}", request.Title);

        try
        {
            var jsonApiRequest = PublishingMapper.MapCreateRequestToJsonApi(request);
            var response = await ApiConnection.PostAsync<JsonApiSingleResponse<EpisodeDto>>(
                $"{BaseEndpoint}/episodes", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException("Failed to create episode - no data returned");
            }

            var episode = PublishingMapper.MapToDomain(response.Data);

            Logger.LogInformation("Successfully created episode: {EpisodeId}", episode.Id);
            return episode;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating episode");
            throw;
        }
    }

    /// <summary>
    /// Updates an existing episode.
    /// </summary>
    public async Task<Episode> UpdateEpisodeAsync(string id, EpisodeUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Episode ID cannot be null or empty", nameof(id));
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        Logger.LogDebug("Updating episode: {EpisodeId}", id);

        try
        {
            var jsonApiRequest = PublishingMapper.MapUpdateRequestToJsonApi(id, request);
            var response = await ApiConnection.PatchAsync<JsonApiSingleResponse<EpisodeDto>>(
                $"{BaseEndpoint}/episodes/{id}", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException($"Failed to update episode {id} - no data returned");
            }

            var episode = PublishingMapper.MapToDomain(response.Data);

            Logger.LogInformation("Successfully updated episode: {EpisodeId}", id);
            return episode;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating episode: {EpisodeId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes an episode.
    /// </summary>
    public async Task DeleteEpisodeAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Episode ID cannot be null or empty", nameof(id));

        Logger.LogDebug("Deleting episode: {EpisodeId}", id);

        try
        {
            await ApiConnection.DeleteAsync($"{BaseEndpoint}/episodes/{id}", cancellationToken);
            Logger.LogInformation("Successfully deleted episode: {EpisodeId}", id);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting episode: {EpisodeId}", id);
            throw;
        }
    }

    #endregion

    #region Content Publishing

    /// <summary>
    /// Publishes an episode, making it publicly available.
    /// </summary>
    public async Task<Episode> PublishEpisodeAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Episode ID cannot be null or empty", nameof(id));

        Logger.LogDebug("Publishing episode: {EpisodeId}", id);

        try
        {
            var response = await ApiConnection.PostAsync<JsonApiSingleResponse<dynamic>>(
                $"{BaseEndpoint}/episodes/{id}/publish", null!, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException($"Failed to publish episode {id} - no data returned");
            }

            // This would use PublishingMapper.MapToDomain(response.Data) in a complete implementation
            var episode = new Episode
            {
                Id = id,
                Title = "Published Episode",
                IsPublished = true,
                DataSource = "Publishing"
            };

            Logger.LogInformation("Successfully published episode: {EpisodeId}", id);
            return episode;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error publishing episode: {EpisodeId}", id);
            throw;
        }
    }

    /// <summary>
    /// Unpublishes an episode, making it private.
    /// </summary>
    public async Task<Episode> UnpublishEpisodeAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Episode ID cannot be null or empty", nameof(id));

        Logger.LogDebug("Unpublishing episode: {EpisodeId}", id);

        try
        {
            var response = await ApiConnection.PostAsync<JsonApiSingleResponse<dynamic>>(
                $"{BaseEndpoint}/episodes/{id}/unpublish", null!, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException($"Failed to unpublish episode {id} - no data returned");
            }

            // This would use PublishingMapper.MapToDomain(response.Data) in a complete implementation
            var episode = new Episode
            {
                Id = id,
                Title = "Unpublished Episode",
                IsPublished = false,
                DataSource = "Publishing"
            };

            Logger.LogInformation("Successfully unpublished episode: {EpisodeId}", id);
            return episode;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error unpublishing episode: {EpisodeId}", id);
            throw;
        }
    }

    #endregion

    #region Pagination Helpers

    /// <summary>
    /// Gets all episodes matching the specified criteria.
    /// This method automatically handles pagination behind the scenes.
    /// </summary>
    public async Task<IReadOnlyList<Episode>> GetAllEpisodesAsync(
        QueryParameters? parameters = null,
        PaginationOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Getting all episodes with parameters: {@Parameters}", parameters);

        var allEpisodes = new List<Episode>();
        var pageSize = options?.PageSize ?? 100;
        var maxPages = options?.MaxItems ?? int.MaxValue;
        var currentPage = 0;

        try
        {
            var currentParameters = parameters ?? new QueryParameters();
            currentParameters.PerPage = pageSize;

            IPagedResponse<Episode> response;
            do
            {
                response = await ListEpisodesAsync(currentParameters, cancellationToken);
                allEpisodes.AddRange(response.Data);
                
                currentPage++;
                if (currentPage >= maxPages)
                    break;

                // Update parameters for next page
                if (!string.IsNullOrEmpty(response.Links?.Next))
                {
                    currentParameters.Offset = (currentParameters.Offset ?? 0) + pageSize;
                }
                else
                {
                    break;
                }
            } while (!string.IsNullOrEmpty(response.Links?.Next));

            Logger.LogInformation("Retrieved {Count} total episodes across {Pages} pages", allEpisodes.Count, currentPage);
            return allEpisodes.AsReadOnly();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting all episodes");
            throw;
        }
    }

    /// <summary>
    /// Streams episodes matching the specified criteria for memory-efficient processing.
    /// </summary>
    public async IAsyncEnumerable<Episode> StreamEpisodesAsync(
        QueryParameters? parameters = null,
        PaginationOptions? options = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Streaming episodes with parameters: {@Parameters}", parameters);

        var pageSize = options?.PageSize ?? 100;
        var maxPages = options?.MaxItems ?? int.MaxValue;
        var currentPage = 0;

        var currentParameters = parameters ?? new QueryParameters();
        currentParameters.PerPage = pageSize;

        IPagedResponse<Episode> response;
        do
        {
            response = await ListEpisodesAsync(currentParameters, cancellationToken);
            
            foreach (var episode in response.Data)
            {
                yield return episode;
            }
            
            currentPage++;
            if (currentPage >= maxPages)
                break;

            // Update parameters for next page
            if (!string.IsNullOrEmpty(response.Links?.Next))
            {
                currentParameters.Offset = (currentParameters.Offset ?? 0) + pageSize;
            }
            else
            {
                break;
            }
        } while (!string.IsNullOrEmpty(response.Links?.Next));

        Logger.LogInformation("Completed streaming episodes across {Pages} pages", currentPage);
    }

    #endregion

    #region Placeholder Methods for Interface Compliance

    // Note: These methods would need full implementation based on the actual API endpoints
    // For now, providing basic implementations to satisfy the interface

    public async Task<Series?> GetSeriesAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Series ID cannot be null or empty", nameof(id));

        Logger.LogDebug("Getting series with ID: {SeriesId}", id);

        try
        {
            var response = await ApiConnection.GetAsync<JsonApiSingleResponse<dynamic>>(
                $"{BaseEndpoint}/series/{id}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogWarning("Series not found: {SeriesId}", id);
                return null;
            }

            var series = PublishingMapper.MapToDomain(response.Data);
            Logger.LogInformation("Successfully retrieved series: {SeriesId}", id);
            return series;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            Logger.LogWarning("Series not found: {SeriesId}", id);
            return null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error retrieving series: {SeriesId}", id);
            throw;
        }
    }

    public async Task<IPagedResponse<Series>> ListSeriesAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Listing series with parameters: {@Parameters}", parameters);

        try
        {
            var queryString = parameters?.ToQueryString() ?? string.Empty;
            var response = await ApiConnection.GetAsync<PagedResponse<dynamic>>(
                $"{BaseEndpoint}/series{queryString}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogWarning("No series returned from API");
                return new PagedResponse<Series>
                {
                    Data = new List<Series>(),
                    Meta = new PagedResponseMeta { TotalCount = 0 },
                    Links = new PagedResponseLinks()
                };
            }

            var series = response.Data.Select<dynamic, Series>(dto => PublishingMapper.MapToDomain((SeriesDto)dto)).ToList();
            
            var pagedResponse = new PagedResponse<Series>
            {
                Data = series,
                Meta = response.Meta,
                Links = response.Links
            };

            Logger.LogInformation("Successfully retrieved {Count} series", series.Count);
            return pagedResponse;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error listing series");
            throw;
        }
    }

    public async Task<Series> CreateSeriesAsync(SeriesCreateRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Series operations not yet implemented");
    }

    public async Task<Series> UpdateSeriesAsync(string id, SeriesUpdateRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Series operations not yet implemented");
    }

    public async Task DeleteSeriesAsync(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Series operations not yet implemented");
    }

    public async Task<Speaker?> GetSpeakerAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Speaker ID cannot be null or empty", nameof(id));

        Logger.LogDebug("Getting speaker with ID: {SpeakerId}", id);

        try
        {
            var response = await ApiConnection.GetAsync<JsonApiSingleResponse<dynamic>>(
                $"{BaseEndpoint}/speakers/{id}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogWarning("Speaker not found: {SpeakerId}", id);
                return null;
            }

            var speaker = PublishingMapper.MapToDomain(response.Data);
            Logger.LogInformation("Successfully retrieved speaker: {SpeakerId}", id);
            return speaker;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            Logger.LogWarning("Speaker not found: {SpeakerId}", id);
            return null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error retrieving speaker: {SpeakerId}", id);
            throw;
        }
    }

    public async Task<IPagedResponse<Speaker>> ListSpeakersAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Listing speakers with parameters: {@Parameters}", parameters);

        try
        {
            var queryString = parameters?.ToQueryString() ?? string.Empty;
            var response = await ApiConnection.GetAsync<PagedResponse<dynamic>>(
                $"{BaseEndpoint}/speakers{queryString}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogWarning("No speakers returned from API");
                return new PagedResponse<Speaker>
                {
                    Data = new List<Speaker>(),
                    Meta = new PagedResponseMeta { TotalCount = 0 },
                    Links = new PagedResponseLinks()
                };
            }

            var speakers = response.Data.Select<dynamic, Speaker>(dto => PublishingMapper.MapToDomain((SpeakerDto)dto)).ToList();
            
            var pagedResponse = new PagedResponse<Speaker>
            {
                Data = speakers,
                Meta = response.Meta,
                Links = response.Links
            };

            Logger.LogInformation("Successfully retrieved {Count} speakers", speakers.Count);
            return pagedResponse;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error listing speakers");
            throw;
        }
    }

    public async Task<Speaker> CreateSpeakerAsync(SpeakerCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        Logger.LogDebug("Creating speaker with name: {FirstName} {LastName}", request.FirstName, request.LastName);

        try
        {
            var jsonApiRequest = new JsonApiRequest<dynamic>
            {
                Data = new
                {
                    type = "Speaker",
                    attributes = new
                    {
                        first_name = request.FirstName,
                        last_name = request.LastName,
                        display_name = request.DisplayName,
                        title = request.Title,
                        biography = request.Biography,
                        email = request.Email,
                        phone_number = request.PhoneNumber,
                        website_url = request.WebsiteUrl,
                        photo_url = request.PhotoUrl,
                        organization = request.Organization,
                        location = request.Location,
                        specialties = request.Specialties,
                        active = request.Active
                    }
                }
            };

            var response = await ApiConnection.PostAsync<JsonApiSingleResponse<dynamic>>(
                $"{BaseEndpoint}/speakers", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException("Failed to create speaker - no data returned");
            }

            var speaker = PublishingMapper.MapToDomain((SpeakerDto)response.Data);
            Logger.LogInformation("Successfully created speaker: {SpeakerId}", speaker.Id);
            return speaker;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating speaker");
            throw;
        }
    }

    public async Task<Speaker> UpdateSpeakerAsync(string id, SpeakerUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Speaker ID cannot be null or empty", nameof(id));
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        Logger.LogDebug("Updating speaker: {SpeakerId}", id);

        try
        {
            var jsonApiRequest = new JsonApiRequest<dynamic>
            {
                Data = new
                {
                    type = "Speaker",
                    id = id,
                    attributes = new
                    {
                        first_name = request.FirstName,
                        last_name = request.LastName,
                        display_name = request.DisplayName,
                        title = request.Title,
                        biography = request.Biography,
                        email = request.Email,
                        phone_number = request.PhoneNumber,
                        website_url = request.WebsiteUrl,
                        photo_url = request.PhotoUrl,
                        organization = request.Organization,
                        location = request.Location,
                        specialties = request.Specialties,
                        active = request.Active
                    }
                }
            };

            var response = await ApiConnection.PatchAsync<JsonApiSingleResponse<dynamic>>(
                $"{BaseEndpoint}/speakers/{id}", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException($"Failed to update speaker {id} - no data returned");
            }

            var speaker = PublishingMapper.MapToDomain(response.Data);
            Logger.LogInformation("Successfully updated speaker: {SpeakerId}", id);
            return speaker;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating speaker: {SpeakerId}", id);
            throw;
        }
    }

    public async Task DeleteSpeakerAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Speaker ID cannot be null or empty", nameof(id));

        Logger.LogDebug("Deleting speaker: {SpeakerId}", id);

        try
        {
            await ApiConnection.DeleteAsync($"{BaseEndpoint}/speakers/{id}", cancellationToken);
            Logger.LogInformation("Successfully deleted speaker: {SpeakerId}", id);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting speaker: {SpeakerId}", id);
            throw;
        }
    }

    public async Task<IPagedResponse<Speakership>> ListSpeakershipsAsync(string episodeId, QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Speakership operations not yet implemented");
    }

    public async Task<IPagedResponse<Speakership>> ListSpeakerEpisodesAsync(string speakerId, QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Speakership operations not yet implemented");
    }

    public async Task<Speakership> AddSpeakerToEpisodeAsync(string episodeId, string speakerId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Speakership operations not yet implemented");
    }

    public async Task DeleteSpeakershipAsync(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Speakership operations not yet implemented");
    }

    public async Task<Media?> GetMediaAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Media ID cannot be null or empty", nameof(id));

        Logger.LogDebug("Getting media with ID: {MediaId}", id);

        try
        {
            var response = await ApiConnection.GetAsync<JsonApiSingleResponse<dynamic>>(
                $"{BaseEndpoint}/media/{id}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogWarning("Media not found: {MediaId}", id);
                return null;
            }

            var media = PublishingMapper.MapToDomain(response.Data);
            Logger.LogInformation("Successfully retrieved media: {MediaId}", id);
            return media;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            Logger.LogWarning("Media not found: {MediaId}", id);
            return null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error retrieving media: {MediaId}", id);
            throw;
        }
    }

    public async Task<IPagedResponse<Media>> ListMediaAsync(string episodeId, QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(episodeId))
            throw new ArgumentException("Episode ID cannot be null or empty", nameof(episodeId));

        Logger.LogDebug("Listing media for episode: {EpisodeId}", episodeId);

        try
        {
            var queryString = parameters?.ToQueryString() ?? string.Empty;
            var response = await ApiConnection.GetAsync<PagedResponse<dynamic>>(
                $"{BaseEndpoint}/episodes/{episodeId}/media{queryString}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogWarning("No media returned for episode: {EpisodeId}", episodeId);
                return new PagedResponse<Media>
                {
                    Data = new List<Media>(),
                    Meta = new PagedResponseMeta { TotalCount = 0 },
                    Links = new PagedResponseLinks()
                };
            }

            var media = response.Data.Select<dynamic, Media>(dto => PublishingMapper.MapToDomain((MediaDto)dto)).ToList();
            
            var pagedResponse = new PagedResponse<Media>
            {
                Data = media,
                Meta = response.Meta,
                Links = response.Links
            };

            Logger.LogInformation("Successfully retrieved {Count} media files for episode: {EpisodeId}", media.Count, episodeId);
            return pagedResponse;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error listing media for episode: {EpisodeId}", episodeId);
            throw;
        }
    }

    public async Task<Media> UploadMediaAsync(string episodeId, MediaUploadRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(episodeId))
            throw new ArgumentException("Episode ID cannot be null or empty", nameof(episodeId));
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        Logger.LogDebug("Uploading media to episode: {EpisodeId}", episodeId);

        try
        {
            var jsonApiRequest = new JsonApiRequest<dynamic>
            {
                Data = new
                {
                    type = "Media",
                    attributes = new
                    {
                        file_name = request.FileName,
                        content_type = request.ContentType,
                        file_size_in_bytes = request.FileSizeInBytes,
                        media_type = request.MediaType,
                        quality = request.Quality,
                        is_primary = request.IsPrimary
                    },
                    relationships = new
                    {
                        episode = new
                        {
                            data = new { type = "Episode", id = episodeId }
                        }
                    }
                }
            };

            var response = await ApiConnection.PostAsync<JsonApiSingleResponse<dynamic>>(
                $"{BaseEndpoint}/episodes/{episodeId}/media", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException($"Failed to upload media to episode {episodeId} - no data returned");
            }

            var media = PublishingMapper.MapToDomain((MediaDto)response.Data);
            Logger.LogInformation("Successfully uploaded media: {MediaId} to episode: {EpisodeId}", media.Id, episodeId);
            return media;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error uploading media to episode: {EpisodeId}", episodeId);
            throw;
        }
    }

    public async Task<Media> UpdateMediaAsync(string id, MediaUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Media ID cannot be null or empty", nameof(id));
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        Logger.LogDebug("Updating media: {MediaId}", id);

        try
        {
            var jsonApiRequest = new JsonApiRequest<dynamic>
            {
                Data = new
                {
                    type = "Media",
                    id = id,
                    attributes = new
                    {
                        file_name = request.FileName,
                        quality = request.Quality,
                        is_primary = request.IsPrimary
                    }
                }
            };

            var response = await ApiConnection.PatchAsync<JsonApiSingleResponse<dynamic>>(
                $"{BaseEndpoint}/media/{id}", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException($"Failed to update media {id} - no data returned");
            }

            var media = PublishingMapper.MapToDomain(response.Data);
            Logger.LogInformation("Successfully updated media: {MediaId}", id);
            return media;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating media: {MediaId}", id);
            throw;
        }
    }

    public async Task DeleteMediaAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Media ID cannot be null or empty", nameof(id));

        Logger.LogDebug("Deleting media: {MediaId}", id);

        try
        {
            await ApiConnection.DeleteAsync($"{BaseEndpoint}/media/{id}", cancellationToken);
            Logger.LogInformation("Successfully deleted media: {MediaId}", id);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting media: {MediaId}", id);
            throw;
        }
    }

    public async Task<Series> PublishSeriesAsync(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Series publishing operations not yet implemented");
    }

    public async Task<Series> UnpublishSeriesAsync(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Series publishing operations not yet implemented");
    }

    public async Task<IPagedResponse<DistributionChannel>> ListDistributionChannelsAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Distribution operations not yet implemented");
    }

    public async Task<DistributionResult> DistributeEpisodeAsync(string episodeId, string channelId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Distribution operations not yet implemented");
    }

    public async Task<DistributionResult> DistributeSeriesAsync(string seriesId, string channelId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Distribution operations not yet implemented");
    }

    public async Task<EpisodeAnalytics> GetEpisodeAnalyticsAsync(string episodeId, AnalyticsRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Analytics operations not yet implemented");
    }

    public async Task<SeriesAnalytics> GetSeriesAnalyticsAsync(string seriesId, AnalyticsRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Analytics operations not yet implemented");
    }

    public async Task<PublishingReport> GeneratePublishingReportAsync(PublishingReportRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Reporting operations not yet implemented");
    }

    #endregion
}