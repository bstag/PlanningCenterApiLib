using PlanningCenter.Api.Client.Models.Giving;
using PlanningCenter.Api.Client.Models.Requests;

namespace PlanningCenter.Api.Client.Models;

/// <summary>
/// Service interface for the Planning Center Giving module.
/// Provides comprehensive donation and financial giving management with built-in pagination support.
/// </summary>
public interface IGivingService
{
    // Donation management
    
    /// <summary>
    /// Gets a single donation by ID.
    /// </summary>
    /// <param name="id">The donation's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The donation, or null if not found</returns>
    Task<Donation?> GetDonationAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists donations with optional filtering, sorting, and pagination.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with donations</returns>
    Task<IPagedResponse<Donation>> ListDonationsAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new donation.
    /// </summary>
    /// <param name="request">The donation creation request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created donation</returns>
    Task<Donation> CreateDonationAsync(DonationCreateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing donation.
    /// </summary>
    /// <param name="id">The donation's unique identifier</param>
    /// <param name="request">The donation update request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The updated donation</returns>
    Task<Donation> UpdateDonationAsync(string id, DonationUpdateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes a donation.
    /// </summary>
    /// <param name="id">The donation's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    Task DeleteDonationAsync(string id, CancellationToken cancellationToken = default);
    
    // Fund management
    
    /// <summary>
    /// Gets a single fund by ID.
    /// </summary>
    /// <param name="id">The fund's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The fund, or null if not found</returns>
    Task<Fund?> GetFundAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists funds with optional filtering, sorting, and pagination.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with funds</returns>
    Task<IPagedResponse<Fund>> ListFundsAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new fund.
    /// </summary>
    /// <param name="request">The fund creation request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created fund</returns>
    Task<Fund> CreateFundAsync(FundCreateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing fund.
    /// </summary>
    /// <param name="id">The fund's unique identifier</param>
    /// <param name="request">The fund update request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The updated fund</returns>
    Task<Fund> UpdateFundAsync(string id, FundUpdateRequest request, CancellationToken cancellationToken = default);
    
    // Batch management
    
    /// <summary>
    /// Gets a single batch by ID.
    /// </summary>
    /// <param name="id">The batch's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The batch, or null if not found</returns>
    Task<Batch?> GetBatchAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists batches with optional filtering, sorting, and pagination.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with batches</returns>
    Task<IPagedResponse<Batch>> ListBatchesAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new batch.
    /// </summary>
    /// <param name="request">The batch creation request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created batch</returns>
    Task<Batch> CreateBatchAsync(BatchCreateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing batch.
    /// </summary>
    /// <param name="id">The batch's unique identifier</param>
    /// <param name="request">The batch update request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The updated batch</returns>
    Task<Batch> UpdateBatchAsync(string id, BatchUpdateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Commits a batch, making it final.
    /// </summary>
    /// <param name="id">The batch's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The committed batch</returns>
    Task<Batch> CommitBatchAsync(string id, CancellationToken cancellationToken = default);
    
    // Pledge management
    
    /// <summary>
    /// Gets a single pledge by ID.
    /// </summary>
    /// <param name="id">The pledge's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The pledge, or null if not found</returns>
    Task<Pledge?> GetPledgeAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists pledges with optional filtering, sorting, and pagination.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with pledges</returns>
    Task<IPagedResponse<Pledge>> ListPledgesAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new pledge.
    /// </summary>
    /// <param name="request">The pledge creation request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created pledge</returns>
    Task<Pledge> CreatePledgeAsync(PledgeCreateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing pledge.
    /// </summary>
    /// <param name="id">The pledge's unique identifier</param>
    /// <param name="request">The pledge update request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The updated pledge</returns>
    Task<Pledge> UpdatePledgeAsync(string id, PledgeUpdateRequest request, CancellationToken cancellationToken = default);
    
    // Recurring donation management
    
    /// <summary>
    /// Gets a single recurring donation by ID.
    /// </summary>
    /// <param name="id">The recurring donation's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The recurring donation, or null if not found</returns>
    Task<RecurringDonation?> GetRecurringDonationAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists recurring donations with optional filtering, sorting, and pagination.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with recurring donations</returns>
    Task<IPagedResponse<RecurringDonation>> ListRecurringDonationsAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new recurring donation.
    /// </summary>
    /// <param name="request">The recurring donation creation request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created recurring donation</returns>
    Task<RecurringDonation> CreateRecurringDonationAsync(RecurringDonationCreateRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing recurring donation.
    /// </summary>
    /// <param name="id">The recurring donation's unique identifier</param>
    /// <param name="request">The recurring donation update request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The updated recurring donation</returns>
    Task<RecurringDonation> UpdateRecurringDonationAsync(string id, RecurringDonationUpdateRequest request, CancellationToken cancellationToken = default);
    
    // Refund management
    
    /// <summary>
    /// Gets a refund for a specific donation.
    /// </summary>
    /// <param name="donationId">The donation's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The refund, or null if not found</returns>
    Task<Refund?> GetRefundAsync(string donationId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists refunds with optional filtering, sorting, and pagination.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with refunds</returns>
    Task<IPagedResponse<Refund>> ListRefundsAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Issues a refund for a donation.
    /// </summary>
    /// <param name="donationId">The donation's unique identifier</param>
    /// <param name="request">The refund creation request</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The created refund</returns>
    Task<Refund> IssueRefundAsync(string donationId, RefundCreateRequest request, CancellationToken cancellationToken = default);
    
    // Payment source management
    
    /// <summary>
    /// Gets a single payment source by ID.
    /// </summary>
    /// <param name="id">The payment source's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The payment source, or null if not found</returns>
    Task<PaymentSource?> GetPaymentSourceAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lists payment sources with optional filtering, sorting, and pagination.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with payment sources</returns>
    Task<IPagedResponse<PaymentSource>> ListPaymentSourcesAsync(QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    // Person-specific operations
    
    /// <summary>
    /// Gets a person from the Giving module.
    /// </summary>
    /// <param name="id">The person's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The person, or null if not found</returns>
    Task<Core.Person?> GetPersonAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets donations for a specific person.
    /// </summary>
    /// <param name="personId">The person's unique identifier</param>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with the person's donations</returns>
    Task<IPagedResponse<Donation>> GetDonationsForPersonAsync(string personId, QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets pledges for a specific person.
    /// </summary>
    /// <param name="personId">The person's unique identifier</param>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with the person's pledges</returns>
    Task<IPagedResponse<Pledge>> GetPledgesForPersonAsync(string personId, QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets recurring donations for a specific person.
    /// </summary>
    /// <param name="personId">The person's unique identifier</param>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>A paginated response with the person's recurring donations</returns>
    Task<IPagedResponse<RecurringDonation>> GetRecurringDonationsForPersonAsync(string personId, QueryParameters? parameters = null, CancellationToken cancellationToken = default);
    
    // Reporting and analytics
    
    /// <summary>
    /// Gets total giving amount for a date range and optional fund.
    /// </summary>
    /// <param name="startDate">The start date for the range</param>
    /// <param name="endDate">The end date for the range</param>
    /// <param name="fundId">Optional fund ID to filter by</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The total giving amount in cents</returns>
    Task<long> GetTotalGivingAsync(DateTime startDate, DateTime endDate, string? fundId = null, CancellationToken cancellationToken = default);
    
    // Pagination helpers that eliminate manual pagination logic
    
    /// <summary>
    /// Gets all donations matching the specified criteria.
    /// This method automatically handles pagination behind the scenes.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>All donations matching the criteria</returns>
    Task<IReadOnlyList<Donation>> GetAllDonationsAsync(
        QueryParameters? parameters = null,
        PaginationOptions? options = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Streams donations matching the specified criteria for memory-efficient processing.
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and sorting</param>
    /// <param name="options">Pagination options for performance tuning</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>An async enumerable that yields donations from all pages</returns>
    IAsyncEnumerable<Donation> StreamDonationsAsync(
        QueryParameters? parameters = null,
        PaginationOptions? options = null,
        CancellationToken cancellationToken = default);
}