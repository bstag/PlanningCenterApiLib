using Microsoft.Extensions.Logging;
using PlanningCenter.Api.Client.Mapping.Giving;
using PlanningCenter.Api.Client.Mapping.People;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.Giving;
using PlanningCenter.Api.Client.Models.Requests;
using PlanningCenter.Api.Client.Models.Exceptions;
using PlanningCenter.Api.Client.Models.JsonApi.Giving;
using PlanningCenter.Api.Client.Models.JsonApi;
using PlanningCenter.Api.Client.Models.JsonApi.People;
using PlanningCenter.Api.Client.Models.People;

namespace PlanningCenter.Api.Client.Services;

/// <summary>
/// Service implementation for the Planning Center Giving module.
/// Provides comprehensive donation and financial giving management with built-in pagination support.
/// </summary>
public class GivingService : ServiceBase, IGivingService
{
    private const string BaseEndpoint = "/giving/v2";

    public GivingService(
        IApiConnection apiConnection,
        ILogger<GivingService> logger)
        : base(logger, apiConnection)
    {
    }

    #region Donation Management

    /// <summary>
    /// Gets a single donation by ID.
    /// </summary>
    public async Task<Donation?> GetDonationAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Donation ID cannot be null or empty", nameof(id));

        Logger.LogDebug("Getting donation with ID: {DonationId}", id);

        try
        {
            var response = await ApiConnection.GetAsync<JsonApiSingleResponse<DonationDto>>(
                $"{BaseEndpoint}/donations/{id}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogWarning("Donation not found: {DonationId}", id);
                return null;
            }

            var donation = GivingMapper.MapToDomain(response.Data);
            Logger.LogInformation("Successfully retrieved donation: {DonationId}", id);
            return donation;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            Logger.LogWarning("Donation not found: {DonationId}", id);
            return null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error retrieving donation: {DonationId}", id);
            throw;
        }
    }

    /// <summary>
    /// Lists donations with optional filtering, sorting, and pagination.
    /// </summary>
    public async Task<IPagedResponse<Donation>> ListDonationsAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Listing donations with parameters: {@Parameters}", parameters);

        try
        {
            var queryString = parameters?.ToQueryString() ?? string.Empty;
            var response = await ApiConnection.GetAsync<PagedResponse<DonationDto>>(
                $"{BaseEndpoint}/donations{queryString}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogWarning("No donations returned from API");
                return new PagedResponse<Donation>
                {
                    Data = new List<Donation>(),
                    Meta = new PagedResponseMeta { TotalCount = 0 },
                    Links = new PagedResponseLinks()
                };
            }

            var donations = response.Data.Select(GivingMapper.MapToDomain).ToList();
            
            var pagedResponse = new PagedResponse<Donation>
            {
                Data = donations,
                Meta = response.Meta,
                Links = response.Links
            };

            Logger.LogInformation("Successfully retrieved {Count} donations", donations.Count);
            return pagedResponse;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error listing donations");
            throw;
        }
    }

    /// <summary>
    /// Creates a new donation.
    /// </summary>
    public async Task<Donation> CreateDonationAsync(DonationCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        Logger.LogDebug("Creating donation with amount: {Amount}", request.AmountCents);

        try
        {
            var jsonApiRequest = GivingMapper.MapCreateRequestToJsonApi(request);
            var response = await ApiConnection.PostAsync<JsonApiSingleResponse<DonationDto>>(
                $"{BaseEndpoint}/donations", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException("Failed to create donation - no data returned");
            }

            var donation = GivingMapper.MapToDomain(response.Data);
            Logger.LogInformation("Successfully created donation: {DonationId}", donation.Id);
            return donation;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating donation");
            throw;
        }
    }

    /// <summary>
    /// Updates an existing donation.
    /// </summary>
    public async Task<Donation> UpdateDonationAsync(string id, DonationUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Donation ID cannot be null or empty", nameof(id));
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        Logger.LogDebug("Updating donation: {DonationId}", id);

        try
        {
            var jsonApiRequest = GivingMapper.MapUpdateRequestToJsonApi(id, request);
            var response = await ApiConnection.PatchAsync<JsonApiSingleResponse<DonationDto>>(
                $"{BaseEndpoint}/donations/{id}", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException($"Failed to update donation {id} - no data returned");
            }

            var donation = GivingMapper.MapToDomain(response.Data);
            Logger.LogInformation("Successfully updated donation: {DonationId}", id);
            return donation;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating donation: {DonationId}", id);
            throw;
        }
    }

    /// <summary>
    /// Deletes a donation.
    /// </summary>
    public async Task DeleteDonationAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Donation ID cannot be null or empty", nameof(id));

        Logger.LogDebug("Deleting donation: {DonationId}", id);

        try
        {
            await ApiConnection.DeleteAsync($"{BaseEndpoint}/donations/{id}", cancellationToken);
            Logger.LogInformation("Successfully deleted donation: {DonationId}", id);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting donation: {DonationId}", id);
            throw;
        }
    }

    #endregion

    #region Fund Management

    /// <summary>
    /// Gets a single fund by ID.
    /// </summary>
    public async Task<Fund?> GetFundAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Fund ID cannot be null or empty", nameof(id));

        Logger.LogDebug("Getting fund with ID: {FundId}", id);

        try
        {
            var response = await ApiConnection.GetAsync<JsonApiSingleResponse<FundDto>>(
                $"{BaseEndpoint}/funds/{id}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogWarning("Fund not found: {FundId}", id);
                return null;
            }

            var fund = GivingMapper.MapToDomain(response.Data);
            Logger.LogInformation("Successfully retrieved fund: {FundId}", id);
            return fund;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            Logger.LogWarning("Fund not found: {FundId}", id);
            return null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error retrieving fund: {FundId}", id);
            throw;
        }
    }

    /// <summary>
    /// Lists funds with optional filtering, sorting, and pagination.
    /// </summary>
    public async Task<IPagedResponse<Fund>> ListFundsAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Listing funds with parameters: {@Parameters}", parameters);

        try
        {
            var queryString = parameters?.ToQueryString() ?? string.Empty;
            var response = await ApiConnection.GetAsync<PagedResponse<FundDto>>(
                $"{BaseEndpoint}/funds{queryString}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogWarning("No funds returned from API");
                return new PagedResponse<Fund>
                {
                    Data = new List<Fund>(),
                    Meta = new PagedResponseMeta { TotalCount = 0 },
                    Links = new PagedResponseLinks()
                };
            }

            var funds = response.Data.Select(GivingMapper.MapToDomain).ToList();
            
            var pagedResponse = new PagedResponse<Fund>
            {
                Data = funds,
                Meta = response.Meta,
                Links = response.Links
            };

            Logger.LogInformation("Successfully retrieved {Count} funds", funds.Count);
            return pagedResponse;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error listing funds");
            throw;
        }
    }

    /// <summary>
    /// Creates a new fund.
    /// </summary>
    public async Task<Fund> CreateFundAsync(FundCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        Logger.LogDebug("Creating fund with name: {Name}", request.Name);

        try
        {
            var jsonApiRequest = GivingMapper.MapCreateRequestToJsonApi(request);
            var response = await ApiConnection.PostAsync<JsonApiSingleResponse<FundDto>>(
                $"{BaseEndpoint}/funds", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException("Failed to create fund - no data returned");
            }

            var fund = GivingMapper.MapToDomain(response.Data);
            Logger.LogInformation("Successfully created fund: {FundId}", fund.Id);
            return fund;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating fund");
            throw;
        }
    }

    /// <summary>
    /// Updates an existing fund.
    /// </summary>
    public async Task<Fund> UpdateFundAsync(string id, FundUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Fund ID cannot be null or empty", nameof(id));
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        Logger.LogDebug("Updating fund: {FundId}", id);

        try
        {
            var jsonApiRequest = GivingMapper.MapUpdateRequestToJsonApi(id, request);
            var response = await ApiConnection.PatchAsync<JsonApiSingleResponse<FundDto>>(
                $"{BaseEndpoint}/funds/{id}", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException($"Failed to update fund {id} - no data returned");
            }

            var fund = GivingMapper.MapToDomain(response.Data);
            Logger.LogInformation("Successfully updated fund: {FundId}", id);
            return fund;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating fund: {FundId}", id);
            throw;
        }
    }

    #endregion

    #region Batch Management

    /// <summary>
    /// Gets a single batch by ID.
    /// </summary>
    public async Task<Batch?> GetBatchAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Batch ID cannot be null or empty", nameof(id));

        Logger.LogDebug("Getting batch with ID: {BatchId}", id);

        try
        {
            var response = await ApiConnection.GetAsync<JsonApiSingleResponse<BatchDto>>(
                $"{BaseEndpoint}/batches/{id}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogWarning("Batch not found: {BatchId}", id);
                return null;
            }

            var batch = GivingMapper.MapToDomain(response.Data);
            Logger.LogInformation("Successfully retrieved batch: {BatchId}", id);
            return batch;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            Logger.LogWarning("Batch not found: {BatchId}", id);
            return null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error retrieving batch: {BatchId}", id);
            throw;
        }
    }

    /// <summary>
    /// Lists batches with optional filtering, sorting, and pagination.
    /// </summary>
    public async Task<IPagedResponse<Batch>> ListBatchesAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Listing batches with parameters: {@Parameters}", parameters);

        try
        {
            var queryString = parameters?.ToQueryString() ?? string.Empty;
            var response = await ApiConnection.GetAsync<PagedResponse<BatchDto>>(
                $"{BaseEndpoint}/batches{queryString}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogWarning("No batches returned from API");
                return new PagedResponse<Batch>
                {
                    Data = new List<Batch>(),
                    Meta = new PagedResponseMeta { TotalCount = 0 },
                    Links = new PagedResponseLinks()
                };
            }

            var batches = response.Data.Select(GivingMapper.MapToDomain).ToList();
            
            var pagedResponse = new PagedResponse<Batch>
            {
                Data = batches,
                Meta = response.Meta,
                Links = response.Links
            };

            Logger.LogInformation("Successfully retrieved {Count} batches", batches.Count);
            return pagedResponse;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error listing batches");
            throw;
        }
    }

    /// <summary>
    /// Creates a new batch.
    /// </summary>
    public async Task<Batch> CreateBatchAsync(BatchCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        Logger.LogDebug("Creating batch with description: {Description}", request.Description);

        try
        {
            var jsonApiRequest = GivingMapper.MapCreateRequestToJsonApi(request);
            var response = await ApiConnection.PostAsync<JsonApiSingleResponse<BatchDto>>(
                $"{BaseEndpoint}/batches", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException("Failed to create batch - no data returned");
            }

            var batch = GivingMapper.MapToDomain(response.Data);
            Logger.LogInformation("Successfully created batch: {BatchId}", batch.Id);
            return batch;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating batch");
            throw;
        }
    }

    /// <summary>
    /// Updates an existing batch.
    /// </summary>
    public async Task<Batch> UpdateBatchAsync(string id, BatchUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Batch ID cannot be null or empty", nameof(id));
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        Logger.LogDebug("Updating batch: {BatchId}", id);

        try
        {
            var jsonApiRequest = GivingMapper.MapUpdateRequestToJsonApi(id, request);
            var response = await ApiConnection.PatchAsync<JsonApiSingleResponse<BatchDto>>(
                $"{BaseEndpoint}/batches/{id}", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException($"Failed to update batch {id} - no data returned");
            }

            var batch = GivingMapper.MapToDomain(response.Data);
            Logger.LogInformation("Successfully updated batch: {BatchId}", id);
            return batch;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating batch: {BatchId}", id);
            throw;
        }
    }

    /// <summary>
    /// Commits a batch, making it final.
    /// </summary>
    public async Task<Batch> CommitBatchAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Batch ID cannot be null or empty", nameof(id));

        Logger.LogDebug("Committing batch: {BatchId}", id);

        try
        {
            var response = await ApiConnection.PostAsync<JsonApiSingleResponse<BatchDto>>(
                $"{BaseEndpoint}/batches/{id}/commit", null!, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException($"Failed to commit batch {id} - no data returned");
            }

            var batch = GivingMapper.MapToDomain(response.Data);
            Logger.LogInformation("Successfully committed batch: {BatchId}", id);
            return batch;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error committing batch: {BatchId}", id);
            throw;
        }
    }

    #endregion

    #region Pledge Management

    /// <summary>
    /// Gets a single pledge by ID.
    /// </summary>
    public async Task<Pledge?> GetPledgeAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Pledge ID cannot be null or empty", nameof(id));

        Logger.LogDebug("Getting pledge with ID: {PledgeId}", id);

        try
        {
            var response = await ApiConnection.GetAsync<JsonApiSingleResponse<PledgeDto>>(
                $"{BaseEndpoint}/pledges/{id}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogWarning("Pledge not found: {PledgeId}", id);
                return null;
            }

            var pledge = GivingMapper.MapToDomain(response.Data);
            Logger.LogInformation("Successfully retrieved pledge: {PledgeId}", id);
            return pledge;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            Logger.LogWarning("Pledge not found: {PledgeId}", id);
            return null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error retrieving pledge: {PledgeId}", id);
            throw;
        }
    }

    /// <summary>
    /// Lists pledges with optional filtering, sorting, and pagination.
    /// </summary>
    public async Task<IPagedResponse<Pledge>> ListPledgesAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Listing pledges with parameters: {@Parameters}", parameters);

        try
        {
            var queryString = parameters?.ToQueryString() ?? string.Empty;
            var response = await ApiConnection.GetAsync<PagedResponse<PledgeDto>>(
                $"{BaseEndpoint}/pledges{queryString}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogWarning("No pledges returned from API");
                return new PagedResponse<Pledge>
                {
                    Data = new List<Pledge>(),
                    Meta = new PagedResponseMeta { TotalCount = 0 },
                    Links = new PagedResponseLinks()
                };
            }

            var pledges = response.Data.Select(GivingMapper.MapToDomain).ToList();
            
            var pagedResponse = new PagedResponse<Pledge>
            {
                Data = pledges,
                Meta = response.Meta,
                Links = response.Links
            };

            Logger.LogInformation("Successfully retrieved {Count} pledges", pledges.Count);
            return pagedResponse;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error listing pledges");
            throw;
        }
    }

    /// <summary>
    /// Creates a new pledge.
    /// </summary>
    public async Task<Pledge> CreatePledgeAsync(PledgeCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        Logger.LogDebug("Creating pledge with amount: {Amount}", request.AmountCents);

        try
        {
            var jsonApiRequest = GivingMapper.MapCreateRequestToJsonApi(request);
            var response = await ApiConnection.PostAsync<JsonApiSingleResponse<PledgeDto>>(
                $"{BaseEndpoint}/pledges", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException("Failed to create pledge - no data returned");
            }

            var pledge = GivingMapper.MapToDomain(response.Data);
            Logger.LogInformation("Successfully created pledge: {PledgeId}", pledge.Id);
            return pledge;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating pledge");
            throw;
        }
    }

    /// <summary>
    /// Updates an existing pledge.
    /// </summary>
    public async Task<Pledge> UpdatePledgeAsync(string id, PledgeUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Pledge ID cannot be null or empty", nameof(id));
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        Logger.LogDebug("Updating pledge: {PledgeId}", id);

        try
        {
            var jsonApiRequest = GivingMapper.MapUpdateRequestToJsonApi(id, request);
            var response = await ApiConnection.PatchAsync<JsonApiSingleResponse<PledgeDto>>(
                $"{BaseEndpoint}/pledges/{id}", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException($"Failed to update pledge {id} - no data returned");
            }

            var pledge = GivingMapper.MapToDomain(response.Data);
            Logger.LogInformation("Successfully updated pledge: {PledgeId}", id);
            return pledge;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating pledge: {PledgeId}", id);
            throw;
        }
    }

    #endregion

    #region Recurring Donation Management

    /// <summary>
    /// Gets a single recurring donation by ID.
    /// </summary>
    public async Task<RecurringDonation?> GetRecurringDonationAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Recurring donation ID cannot be null or empty", nameof(id));

        Logger.LogDebug("Getting recurring donation with ID: {RecurringDonationId}", id);

        try
        {
            var response = await ApiConnection.GetAsync<JsonApiSingleResponse<RecurringDonationDto>>(
                $"{BaseEndpoint}/recurring_donations/{id}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogWarning("Recurring donation not found: {RecurringDonationId}", id);
                return null;
            }

            var recurringDonation = GivingMapper.MapToDomain(response.Data);
            Logger.LogInformation("Successfully retrieved recurring donation: {RecurringDonationId}", id);
            return recurringDonation;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            Logger.LogWarning("Recurring donation not found: {RecurringDonationId}", id);
            return null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error retrieving recurring donation: {RecurringDonationId}", id);
            throw;
        }
    }

    /// <summary>
    /// Lists recurring donations with optional filtering, sorting, and pagination.
    /// </summary>
    public async Task<IPagedResponse<RecurringDonation>> ListRecurringDonationsAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Listing recurring donations with parameters: {@Parameters}", parameters);

        try
        {
            var queryString = parameters?.ToQueryString() ?? string.Empty;
            var response = await ApiConnection.GetAsync<PagedResponse<RecurringDonationDto>>(
                $"{BaseEndpoint}/recurring_donations{queryString}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogWarning("No recurring donations returned from API");
                return new PagedResponse<RecurringDonation>
                {
                    Data = new List<RecurringDonation>(),
                    Meta = new PagedResponseMeta { TotalCount = 0 },
                    Links = new PagedResponseLinks()
                };
            }

            var recurringDonations = response.Data.Select(GivingMapper.MapToDomain).ToList();
            
            var pagedResponse = new PagedResponse<RecurringDonation>
            {
                Data = recurringDonations,
                Meta = response.Meta,
                Links = response.Links
            };

            Logger.LogInformation("Successfully retrieved {Count} recurring donations", recurringDonations.Count);
            return pagedResponse;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error listing recurring donations");
            throw;
        }
    }

    /// <summary>
    /// Creates a new recurring donation.
    /// </summary>
    public async Task<RecurringDonation> CreateRecurringDonationAsync(RecurringDonationCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        Logger.LogDebug("Creating recurring donation with amount: {Amount}", request.AmountCents);

        try
        {
            var jsonApiRequest = GivingMapper.MapCreateRequestToJsonApi(request);
            var response = await ApiConnection.PostAsync<JsonApiSingleResponse<RecurringDonationDto>>(
                $"{BaseEndpoint}/recurring_donations", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException("Failed to create recurring donation - no data returned");
            }

            var recurringDonation = GivingMapper.MapToDomain(response.Data);
            Logger.LogInformation("Successfully created recurring donation: {RecurringDonationId}", recurringDonation.Id);
            return recurringDonation;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating recurring donation");
            throw;
        }
    }

    /// <summary>
    /// Updates an existing recurring donation.
    /// </summary>
    public async Task<RecurringDonation> UpdateRecurringDonationAsync(string id, RecurringDonationUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Recurring donation ID cannot be null or empty", nameof(id));
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        Logger.LogDebug("Updating recurring donation: {RecurringDonationId}", id);

        try
        {
            var jsonApiRequest = GivingMapper.MapUpdateRequestToJsonApi(id, request);
            var response = await ApiConnection.PatchAsync<JsonApiSingleResponse<RecurringDonationDto>>(
                $"{BaseEndpoint}/recurring_donations/{id}", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException($"Failed to update recurring donation {id} - no data returned");
            }

            var recurringDonation = GivingMapper.MapToDomain(response.Data);
            Logger.LogInformation("Successfully updated recurring donation: {RecurringDonationId}", id);
            return recurringDonation;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating recurring donation: {RecurringDonationId}", id);
            throw;
        }
    }

    #endregion

    #region Refund Management

    /// <summary>
    /// Gets a refund for a specific donation.
    /// </summary>
    public async Task<Refund?> GetRefundAsync(string donationId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(donationId))
            throw new ArgumentException("Donation ID cannot be null or empty", nameof(donationId));

        Logger.LogDebug("Getting refund for donation: {DonationId}", donationId);

        try
        {
            var response = await ApiConnection.GetAsync<JsonApiSingleResponse<RefundDto>>(
                $"{BaseEndpoint}/donations/{donationId}/refund", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogWarning("Refund not found for donation: {DonationId}", donationId);
                return null;
            }

            var refund = GivingMapper.MapToDomain(response.Data);
            Logger.LogInformation("Successfully retrieved refund for donation: {DonationId}", donationId);
            return refund;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            Logger.LogWarning("Refund not found for donation: {DonationId}", donationId);
            return null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error retrieving refund for donation: {DonationId}", donationId);
            throw;
        }
    }

    /// <summary>
    /// Lists refunds with optional filtering, sorting, and pagination.
    /// </summary>
    public async Task<IPagedResponse<Refund>> ListRefundsAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Listing refunds with parameters: {@Parameters}", parameters);

        try
        {
            var queryString = parameters?.ToQueryString() ?? string.Empty;
            var response = await ApiConnection.GetAsync<PagedResponse<RefundDto>>(
                $"{BaseEndpoint}/refunds{queryString}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogWarning("No refunds returned from API");
                return new PagedResponse<Refund>
                {
                    Data = new List<Refund>(),
                    Meta = new PagedResponseMeta { TotalCount = 0 },
                    Links = new PagedResponseLinks()
                };
            }

            var refunds = response.Data.Select(GivingMapper.MapToDomain).ToList();
            
            var pagedResponse = new PagedResponse<Refund>
            {
                Data = refunds,
                Meta = response.Meta,
                Links = response.Links
            };

            Logger.LogInformation("Successfully retrieved {Count} refunds", refunds.Count);
            return pagedResponse;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error listing refunds");
            throw;
        }
    }

    /// <summary>
    /// Issues a refund for a donation.
    /// </summary>
    public async Task<Refund> IssueRefundAsync(string donationId, RefundCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(donationId))
            throw new ArgumentException("Donation ID cannot be null or empty", nameof(donationId));
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        Logger.LogDebug("Issuing refund for donation: {DonationId}", donationId);

        try
        {
            var jsonApiRequest = GivingMapper.MapCreateRequestToJsonApi(request);
            var response = await ApiConnection.PostAsync<JsonApiSingleResponse<RefundDto>>(
                $"{BaseEndpoint}/donations/{donationId}/refund", jsonApiRequest, cancellationToken);

            if (response?.Data == null)
            {
                throw new PlanningCenterApiGeneralException($"Failed to issue refund for donation {donationId} - no data returned");
            }

            var refund = GivingMapper.MapToDomain(response.Data);
            Logger.LogInformation("Successfully issued refund for donation: {DonationId}", donationId);
            return refund;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error issuing refund for donation: {DonationId}", donationId);
            throw;
        }
    }

    #endregion

    #region Payment Source Management

    /// <summary>
    /// Gets a single payment source by ID.
    /// </summary>
    public async Task<PaymentSource?> GetPaymentSourceAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Payment source ID cannot be null or empty", nameof(id));

        Logger.LogDebug("Getting payment source with ID: {PaymentSourceId}", id);

        try
        {
            var response = await ApiConnection.GetAsync<JsonApiSingleResponse<PaymentSourceDto>>(
                $"{BaseEndpoint}/payment_sources/{id}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogWarning("Payment source not found: {PaymentSourceId}", id);
                return null;
            }

            var paymentSource = GivingMapper.MapToDomain(response.Data);
            Logger.LogInformation("Successfully retrieved payment source: {PaymentSourceId}", id);
            return paymentSource;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            Logger.LogWarning("Payment source not found: {PaymentSourceId}", id);
            return null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error retrieving payment source: {PaymentSourceId}", id);
            throw;
        }
    }

    /// <summary>
    /// Lists payment sources with optional filtering, sorting, and pagination.
    /// </summary>
    public async Task<IPagedResponse<PaymentSource>> ListPaymentSourcesAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Listing payment sources with parameters: {@Parameters}", parameters);

        try
        {
            var queryString = parameters?.ToQueryString() ?? string.Empty;
            var response = await ApiConnection.GetAsync<PagedResponse<PaymentSourceDto>>(
                $"{BaseEndpoint}/payment_sources{queryString}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogWarning("No payment sources returned from API");
                return new PagedResponse<PaymentSource>
                {
                    Data = new List<PaymentSource>(),
                    Meta = new PagedResponseMeta { TotalCount = 0 },
                    Links = new PagedResponseLinks()
                };
            }

            var paymentSources = response.Data.Select(GivingMapper.MapToDomain).ToList();
            
            var pagedResponse = new PagedResponse<PaymentSource>
            {
                Data = paymentSources,
                Meta = response.Meta,
                Links = response.Links
            };

            Logger.LogInformation("Successfully retrieved {Count} payment sources", paymentSources.Count);
            return pagedResponse;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error listing payment sources");
            throw;
        }
    }

    #endregion

    #region Person-specific Operations

    /// <summary>
    /// Gets a person from the Giving module.
    /// </summary>
    public async Task<Person?> GetPersonAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Person ID cannot be null or empty", nameof(id));

        Logger.LogDebug("Getting person with ID: {PersonId}", id);

        try
        {
            var response = await ApiConnection.GetAsync<JsonApiSingleResponse<PersonDto>>(
                $"{BaseEndpoint}/people/{id}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogWarning("Person not found: {PersonId}", id);
                return null;
            }

            var person = PersonMapper.MapToDomain(response.Data);
            Logger.LogInformation("Successfully retrieved person: {PersonId}", id);
            return person;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            Logger.LogWarning("Person not found: {PersonId}", id);
            return null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error retrieving person: {PersonId}", id);
            throw;
        }
    }

    /// <summary>
    /// Gets donations for a specific person.
    /// </summary>
    public async Task<IPagedResponse<Donation>> GetDonationsForPersonAsync(string personId, QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(personId))
            throw new ArgumentException("Person ID cannot be null or empty", nameof(personId));

        Logger.LogDebug("Getting donations for person: {PersonId}", personId);

        try
        {
            var queryString = parameters?.ToQueryString() ?? string.Empty;
            var response = await ApiConnection.GetAsync<PagedResponse<DonationDto>>(
                $"{BaseEndpoint}/people/{personId}/donations{queryString}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogWarning("No donations returned for person: {PersonId}", personId);
                return new PagedResponse<Donation>
                {
                    Data = new List<Donation>(),
                    Meta = new PagedResponseMeta { TotalCount = 0 },
                    Links = new PagedResponseLinks()
                };
            }

            var donations = response.Data.Select(GivingMapper.MapToDomain).ToList();
            
            var pagedResponse = new PagedResponse<Donation>
            {
                Data = donations,
                Meta = response.Meta,
                Links = response.Links
            };

            Logger.LogInformation("Successfully retrieved {Count} donations for person: {PersonId}", donations.Count, personId);
            return pagedResponse;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting donations for person: {PersonId}", personId);
            throw;
        }
    }

    /// <summary>
    /// Gets pledges for a specific person.
    /// </summary>
    public async Task<IPagedResponse<Pledge>> GetPledgesForPersonAsync(string personId, QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(personId))
            throw new ArgumentException("Person ID cannot be null or empty", nameof(personId));

        Logger.LogDebug("Getting pledges for person: {PersonId}", personId);

        try
        {
            var queryString = parameters?.ToQueryString() ?? string.Empty;
            var response = await ApiConnection.GetAsync<PagedResponse<PledgeDto>>(
                $"{BaseEndpoint}/people/{personId}/pledges{queryString}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogWarning("No pledges returned for person: {PersonId}", personId);
                return new PagedResponse<Pledge>
                {
                    Data = new List<Pledge>(),
                    Meta = new PagedResponseMeta { TotalCount = 0 },
                    Links = new PagedResponseLinks()
                };
            }

            var pledges = response.Data.Select(GivingMapper.MapToDomain).ToList();
            
            var pagedResponse = new PagedResponse<Pledge>
            {
                Data = pledges,
                Meta = response.Meta,
                Links = response.Links
            };

            Logger.LogInformation("Successfully retrieved {Count} pledges for person: {PersonId}", pledges.Count, personId);
            return pagedResponse;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting pledges for person: {PersonId}", personId);
            throw;
        }
    }

    /// <summary>
    /// Gets recurring donations for a specific person.
    /// </summary>
    public async Task<IPagedResponse<RecurringDonation>> GetRecurringDonationsForPersonAsync(string personId, QueryParameters? parameters = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(personId))
            throw new ArgumentException("Person ID cannot be null or empty", nameof(personId));

        Logger.LogDebug("Getting recurring donations for person: {PersonId}", personId);

        try
        {
            var queryString = parameters?.ToQueryString() ?? string.Empty;
            var response = await ApiConnection.GetAsync<PagedResponse<RecurringDonationDto>>(
                $"{BaseEndpoint}/people/{personId}/recurring_donations{queryString}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogWarning("No recurring donations returned for person: {PersonId}", personId);
                return new PagedResponse<RecurringDonation>
                {
                    Data = new List<RecurringDonation>(),
                    Meta = new PagedResponseMeta { TotalCount = 0 },
                    Links = new PagedResponseLinks()
                };
            }

            var recurringDonations = response.Data.Select(GivingMapper.MapToDomain).ToList();
            
            var pagedResponse = new PagedResponse<RecurringDonation>
            {
                Data = recurringDonations,
                Meta = response.Meta,
                Links = response.Links
            };

            Logger.LogInformation("Successfully retrieved {Count} recurring donations for person: {PersonId}", recurringDonations.Count, personId);
            return pagedResponse;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting recurring donations for person: {PersonId}", personId);
            throw;
        }
    }

    #endregion

    #region Reporting and Analytics

    /// <summary>
    /// Gets total giving amount for a date range and optional fund.
    /// </summary>
    public async Task<long> GetTotalGivingAsync(DateTime startDate, DateTime endDate, string? fundId = null, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Getting total giving from {StartDate} to {EndDate} for fund: {FundId}", startDate, endDate, fundId);

        try
        {
            var parameters = new QueryParameters();
            parameters.AddFilter("received_at", $"{startDate:yyyy-MM-dd}..{endDate:yyyy-MM-dd}");
            
            if (!string.IsNullOrWhiteSpace(fundId))
            {
                parameters.AddFilter("fund_id", fundId);
            }

            var queryString = parameters.ToQueryString();
            var response = await ApiConnection.GetAsync<PagedResponse<DonationDto>>(
                $"{BaseEndpoint}/donations{queryString}", cancellationToken);

            if (response?.Data == null)
            {
                Logger.LogWarning("No donations returned for total giving calculation");
                return 0;
            }

            var totalCents = response.Data.Sum(d => d.Attributes.AmountCents);
            Logger.LogInformation("Total giving calculated: {TotalCents} cents", totalCents);
            return totalCents;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error calculating total giving");
            throw;
        }
    }

    #endregion

    #region Pagination Helpers

    /// <summary>
    /// Gets all donations matching the specified criteria.
    /// This method automatically handles pagination behind the scenes.
    /// </summary>
    public async Task<IReadOnlyList<Donation>> GetAllDonationsAsync(
        QueryParameters? parameters = null,
        PaginationOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Getting all donations with parameters: {@Parameters}", parameters);

        var allDonations = new List<Donation>();
        var pageSize = options?.PageSize ?? 100;
        var maxItems = options?.MaxItems ?? int.MaxValue;
        var currentPage = 0;

        try
        {
            var currentParameters = parameters ?? new QueryParameters();
            currentParameters.PerPage = pageSize;

            IPagedResponse<Donation> response;
            do
            {
                response = await ListDonationsAsync(currentParameters, cancellationToken);
                allDonations.AddRange(response.Data);
                
                currentPage++;
                if (allDonations.Count >= maxItems)
                    break;

                // Update parameters for next page
                if (!string.IsNullOrEmpty(response.Links?.Next))
                {
                    // Extract offset from next link or increment offset
                    currentParameters.Offset = (currentParameters.Offset ?? 0) + pageSize;
                }
                else
                {
                    break;
                }
            } while (!string.IsNullOrEmpty(response.Links?.Next));

            Logger.LogInformation("Retrieved {Count} total donations across {Pages} pages", allDonations.Count, currentPage);
            return allDonations.AsReadOnly();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting all donations");
            throw;
        }
    }

    /// <summary>
    /// Streams donations matching the specified criteria for memory-efficient processing.
    /// </summary>
    public async IAsyncEnumerable<Donation> StreamDonationsAsync(
        QueryParameters? parameters = null,
        PaginationOptions? options = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("Streaming donations with parameters: {@Parameters}", parameters);

        var pageSize = options?.PageSize ?? 100;
        var maxItems = options?.MaxItems ?? int.MaxValue;
        var currentPage = 0;
        var totalReturned = 0;

        var currentParameters = parameters ?? new QueryParameters();
        currentParameters.PerPage = pageSize;

        IPagedResponse<Donation> response;
        do
        {
            response = await ListDonationsAsync(currentParameters, cancellationToken);
            
            foreach (var donation in response.Data)
            {
                yield return donation;
                totalReturned++;
            }
            
            currentPage++;
            if (totalReturned >= maxItems)
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

        Logger.LogInformation("Completed streaming donations across {Pages} pages", currentPage);
    }

    #endregion
}