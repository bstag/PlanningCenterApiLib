# Giving Module Specification

## Overview

The Giving module manages donations, funds, batches, pledges, and all financial giving-related functionality in Planning Center. It provides comprehensive tools for tracking donations, managing funds, processing refunds, and generating giving reports.

## Core Entities

### Donation
The primary entity representing a financial gift.

**Key Attributes:**
- `id` - Unique identifier
- `amount_cents` - Donation amount in cents
- `amount_currency` - Currency code (USD, etc.)
- `payment_method` - Credit card, check, cash, etc.
- `payment_last_four` - Last four digits of payment method
- `payment_brand` - Visa, MasterCard, etc.
- `fee_cents` - Processing fee in cents
- `payment_check_number` - Check number if applicable
- `payment_check_dated_at` - Check date
- `received_at` - Date donation was received
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `person` - Donor information
- `batch` - Associated batch
- `campus` - Associated campus
- `payment_source` - Payment source details
- `designations` - Fund designations
- `refund` - Refund information if applicable

### Fund
Represents a specific fund or designation for donations.

**Key Attributes:**
- `id` - Unique identifier
- `name` - Fund name
- `description` - Fund description
- `visibility` - Public, private, etc.
- `default` - Default fund flag
- `color` - Display color
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `campus` - Associated campus
- `designations` - Donation designations

### Batch
Groups donations for processing and reporting.

**Key Attributes:**
- `id` - Unique identifier
- `description` - Batch description
- `committed_at` - When batch was committed
- `total_cents` - Total batch amount in cents
- `total_currency` - Currency code
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `batch_group` - Parent batch group
- `donations` - Donations in batch
- `owner` - Batch owner/creator

### BatchGroup
Groups related batches together.

**Key Attributes:**
- `id` - Unique identifier
- `description` - Group description
- `committed` - Committed status
- `total_cents` - Total group amount in cents
- `total_currency` - Currency code
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `batches` - Batches in group
- `owner` - Group owner

### Pledge
Represents a commitment to give over time.

**Key Attributes:**
- `id` - Unique identifier
- `amount_cents` - Pledge amount in cents
- `amount_currency` - Currency code
- `joint_giver_amount_cents` - Joint giver amount
- `donated_total_cents` - Amount donated so far
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `person` - Pledger
- `joint_giver` - Joint pledger if applicable
- `pledge_campaign` - Associated campaign
- `fund` - Target fund

### PledgeCampaign
Represents a fundraising campaign.

**Key Attributes:**
- `id` - Unique identifier
- `name` - Campaign name
- `description` - Campaign description
- `goal_cents` - Campaign goal in cents
- `goal_currency` - Currency code
- `starts_at` - Campaign start date
- `ends_at` - Campaign end date
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `pledges` - Campaign pledges
- `fund` - Target fund

### RecurringDonation
Represents ongoing scheduled donations.

**Key Attributes:**
- `id` - Unique identifier
- `amount_cents` - Recurring amount in cents
- `amount_currency` - Currency code
- `schedule` - Frequency (weekly, monthly, etc.)
- `next_occurrence` - Next scheduled donation
- `last_donation_at` - Last donation date
- `status` - Active, paused, cancelled
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `person` - Donor
- `payment_source` - Payment method
- `designations` - Fund allocations

### PaymentSource
Represents stored payment methods.

**Key Attributes:**
- `id` - Unique identifier
- `name` - Payment source name
- `brand` - Card brand or bank name
- `last_four` - Last four digits
- `expiration` - Expiration date
- `verified` - Verification status
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `person` - Owner
- `donations` - Associated donations
- `recurring_donations` - Associated recurring donations

### Refund
Represents refunded donations.

**Key Attributes:**
- `id` - Unique identifier
- `amount_cents` - Refund amount in cents
- `amount_currency` - Currency code
- `fee_cents` - Processing fee refunded
- `fee_currency` - Fee currency
- `refunded_at` - Refund date
- `created_at` - Creation timestamp
- `updated_at` - Last update timestamp

**Relationships:**
- `donation` - Original donation
- `designation_refunds` - Fund-specific refunds

## API Endpoints

### Donation Management
- `GET /giving/v2/donations` - List all donations
- `GET /giving/v2/donations/{id}` - Get specific donation
- `POST /giving/v2/donations` - Create new donation
- `PATCH /giving/v2/donations/{id}` - Update donation
- `DELETE /giving/v2/donations/{id}` - Delete donation

### Fund Management
- `GET /giving/v2/funds` - List funds
- `GET /giving/v2/funds/{id}` - Get specific fund
- `POST /giving/v2/funds` - Create fund
- `PATCH /giving/v2/funds/{id}` - Update fund

### Batch Management
- `GET /giving/v2/batches` - List batches
- `GET /giving/v2/batches/{id}` - Get specific batch
- `POST /giving/v2/batches` - Create batch
- `PATCH /giving/v2/batches/{id}` - Update batch
- `POST /giving/v2/batches/{id}/commit` - Commit batch

### Pledge Management
- `GET /giving/v2/pledges` - List pledges
- `GET /giving/v2/pledges/{id}` - Get specific pledge
- `POST /giving/v2/pledges` - Create pledge
- `PATCH /giving/v2/pledges/{id}` - Update pledge

### Recurring Donation Management
- `GET /giving/v2/recurring_donations` - List recurring donations
- `GET /giving/v2/recurring_donations/{id}` - Get specific recurring donation
- `POST /giving/v2/recurring_donations` - Create recurring donation
- `PATCH /giving/v2/recurring_donations/{id}` - Update recurring donation

### Refund Management
- `GET /giving/v2/refunds` - List refunds
- `GET /giving/v2/donations/{id}/refund` - Get donation refund
- `POST /giving/v2/donations/{id}/refund` - Issue refund

## Query Parameters

### Include Parameters
- `person` - Include donor information
- `batch` - Include batch information
- `campus` - Include campus information
- `designations` - Include fund designations
- `payment_source` - Include payment source
- `refund` - Include refund information

### Filtering
- `where[amount_cents]` - Filter by amount
- `where[received_at]` - Filter by received date
- `where[payment_method]` - Filter by payment method
- `where[created_at]` - Filter by creation date
- `where[person_id]` - Filter by donor
- `where[fund_id]` - Filter by fund

### Sorting
- `order=received_at` - Sort by received date
- `order=amount_cents` - Sort by amount
- `order=created_at` - Sort by creation date
- `order=-received_at` - Sort by received date (descending)

## Service Interface

```csharp
public interface IGivingService
{
    // Donation management
    Task<Donation> GetDonationAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Donation>> ListDonationsAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Donation> CreateDonationAsync(DonationCreateRequest request, CancellationToken cancellationToken = default);
    Task<Donation> UpdateDonationAsync(string id, DonationUpdateRequest request, CancellationToken cancellationToken = default);
    Task DeleteDonationAsync(string id, CancellationToken cancellationToken = default);
    
    // Fund management
    Task<Fund> GetFundAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Fund>> ListFundsAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Fund> CreateFundAsync(FundCreateRequest request, CancellationToken cancellationToken = default);
    Task<Fund> UpdateFundAsync(string id, FundUpdateRequest request, CancellationToken cancellationToken = default);
    
    // Batch management
    Task<Batch> GetBatchAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Batch>> ListBatchesAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Batch> CreateBatchAsync(BatchCreateRequest request, CancellationToken cancellationToken = default);
    Task<Batch> UpdateBatchAsync(string id, BatchUpdateRequest request, CancellationToken cancellationToken = default);
    Task<Batch> CommitBatchAsync(string id, CancellationToken cancellationToken = default);
    
    // Pledge management
    Task<Pledge> GetPledgeAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Pledge>> ListPledgesAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Pledge> CreatePledgeAsync(PledgeCreateRequest request, CancellationToken cancellationToken = default);
    Task<Pledge> UpdatePledgeAsync(string id, PledgeUpdateRequest request, CancellationToken cancellationToken = default);
    
    // Recurring donation management
    Task<RecurringDonation> GetRecurringDonationAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<RecurringDonation>> ListRecurringDonationsAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<RecurringDonation> CreateRecurringDonationAsync(RecurringDonationCreateRequest request, CancellationToken cancellationToken = default);
    Task<RecurringDonation> UpdateRecurringDonationAsync(string id, RecurringDonationUpdateRequest request, CancellationToken cancellationToken = default);
    
    // Refund management
    Task<Refund> GetRefundAsync(string donationId, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Refund>> ListRefundsAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Refund> IssueRefundAsync(string donationId, RefundCreateRequest request, CancellationToken cancellationToken = default);
    
    // Payment source management
    Task<PaymentSource> GetPaymentSourceAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<PaymentSource>> ListPaymentSourcesAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    
    // Person-specific operations
    Task<Core.Person> GetPersonAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Donation>> GetDonationsForPersonAsync(string personId, QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Pledge>> GetPledgesForPersonAsync(string personId, QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<IPagedResponse<RecurringDonation>> GetRecurringDonationsForPersonAsync(string personId, QueryParameters parameters = null, CancellationToken cancellationToken = default);
    
    // Reporting
    Task<GivingReport> GenerateGivingReportAsync(GivingReportRequest request, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalGivingAsync(DateTime startDate, DateTime endDate, string fundId = null, CancellationToken cancellationToken = default);
}
```

## Fluent API Interface

```csharp
public interface IGivingFluentContext
{
    // Donation queries
    IDonationFluentContext Donations();
    IDonationFluentContext Donation(string donationId);
    
    // Fund queries
    IFundFluentContext Funds();
    IFundFluentContext Fund(string fundId);
    
    // Batch queries
    IBatchFluentContext Batches();
    IBatchFluentContext Batch(string batchId);
    
    // Pledge queries
    IPledgeFluentContext Pledges();
    IPledgeFluentContext Pledge(string pledgeId);
    
    // Recurring donation queries
    IRecurringDonationFluentContext RecurringDonations();
    IRecurringDonationFluentContext RecurringDonation(string recurringDonationId);
    
    // Person-specific operations
    IPersonGivingFluentContext Person(string personId);
    
    // Reporting
    IReportingFluentContext Reports();
}

public interface IDonationFluentContext
{
    IDonationFluentContext Where(Expression<Func<Donation, bool>> predicate);
    IDonationFluentContext Include(Expression<Func<Donation, object>> include);
    IDonationFluentContext OrderBy(Expression<Func<Donation, object>> orderBy);
    IDonationFluentContext OrderByDescending(Expression<Func<Donation, object>> orderBy);
    IDonationFluentContext ForPerson(string personId);
    IDonationFluentContext ForFund(string fundId);
    IDonationFluentContext InBatch(string batchId);
    IDonationFluentContext ReceivedBetween(DateTime startDate, DateTime endDate);
    
    Task<Donation> GetAsync(CancellationToken cancellationToken = default);
    Task<IPagedResponse<Donation>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default);
    Task<List<Donation>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<decimal> GetTotalAmountAsync(CancellationToken cancellationToken = default);
}
```

## Usage Examples

### Service-Based API
```csharp
// Get a donation with related data
var donation = await givingService.GetDonationAsync("123");

// List donations for a person
var donations = await givingService.GetDonationsForPersonAsync("person123", new QueryParameters
{
    Include = new[] { "fund", "batch", "payment_source" },
    OrderBy = "-received_at"
});

// Create a new donation
var newDonation = await givingService.CreateDonationAsync(new DonationCreateRequest
{
    PersonId = "person123",
    AmountCents = 10000, // $100.00
    PaymentMethod = "credit_card",
    ReceivedAt = DateTime.Now,
    Designations = new[]
    {
        new DesignationRequest { FundId = "fund123", AmountCents = 10000 }
    }
});

// Issue a refund
var refund = await givingService.IssueRefundAsync("donation123", new RefundCreateRequest
{
    AmountCents = 5000, // Partial refund of $50.00
    Reason = "Duplicate donation"
});
```

### Fluent API
```csharp
// Complex donation query
var donations = await client
    .Giving()
    .Donations()
    .Where(d => d.ReceivedAt >= DateTime.Now.AddMonths(-12))
    .Where(d => d.AmountCents >= 10000) // $100 or more
    .Include(d => d.Person)
    .Include(d => d.Designations)
    .OrderByDescending(d => d.ReceivedAt)
    .GetPagedAsync(pageSize: 50);

// Get total giving for a person
var totalGiving = await client
    .Giving()
    .Person("person123")
    .Donations()
    .ReceivedBetween(DateTime.Now.AddYears(-1), DateTime.Now)
    .GetTotalAmountAsync();

// Fund-specific donations
var fundDonations = await client
    .Giving()
    .Fund("fund123")
    .Donations()
    .Where(d => d.ReceivedAt >= DateTime.Now.AddMonths(-3))
    .Include(d => d.Person)
    .GetAllAsync();

// Batch operations
var batch = await client
    .Giving()
    .Batches()
    .Create(new BatchCreateRequest { Description = "Sunday Service 2024-01-15" })
    .AddDonation(new DonationCreateRequest { /* ... */ })
    .AddDonation(new DonationCreateRequest { /* ... */ })
    .CommitAsync();

// Pledge campaign reporting
var campaignTotal = await client
    .Giving()
    .PledgeCampaign("campaign123")
    .Pledges()
    .GetTotalAmountAsync();
```

## Implementation Notes

### Data Mapping
- Map Giving-specific Person DTOs to unified Core.Person model
- Handle monetary amounts consistently (cents vs. dollars)
- Preserve payment method details and security information

### Security Considerations
- Mask sensitive payment information (card numbers, etc.)
- Implement proper authorization for financial data access
- Log all financial transactions for audit purposes

### Caching Strategy
- Cache fund and campaign data (relatively static)
- Avoid caching sensitive payment information
- Use time-based cache expiration for donation totals

### Error Handling
- Handle payment processing errors gracefully
- Validate monetary amounts and currency codes
- Handle refund limitations and business rules

### Performance Considerations
- Optimize queries for large donation datasets
- Implement efficient aggregation for reporting
- Use pagination for donation lists
- Consider read replicas for reporting queries

This module provides comprehensive giving management capabilities while maintaining security and performance standards for financial data.