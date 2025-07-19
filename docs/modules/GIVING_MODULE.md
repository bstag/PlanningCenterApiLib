# Giving Module Documentation

The Giving module provides comprehensive financial management capabilities for Planning Center. This module handles all donation-related functionality including donations, pledges, recurring gifts, batches, funds, and financial reporting.

## 📋 Module Overview

### Key Entities
- **Donations**: Individual gift transactions with amounts, designations, and payment methods
- **Pledges**: Commitment to give a specific amount over time
- **Recurring Donations**: Automated recurring gifts with schedules
- **Batches**: Groups of donations for processing and reconciliation
- **Funds**: Designated accounts for specific purposes (general fund, missions, etc.)
- **Payment Sources**: Credit cards, bank accounts, and other payment methods
- **Payment Methods**: Cash, check, online, etc.
- **Refunds**: Returned donations with reasons and processing details

### Authentication
Requires Planning Center Giving app access with appropriate permissions.

## 🔧 Traditional Service API

### Donation Management

#### Basic Donation Operations
```csharp
public class GivingService
{
    private readonly IGivingService _givingService;
    
    public GivingService(IGivingService givingService)
    {
        _givingService = givingService;
    }
    
    // Get a donation by ID
    public async Task<Donation?> GetDonationAsync(string id)
    {
        return await _givingService.GetDonationAsync(id);
    }
    
    // List donations with pagination
    public async Task<IPagedResponse<Donation>> GetDonationsAsync()
    {
        return await _givingService.ListDonationsAsync(new QueryParameters
        {
            PerPage = 25,
            OrderBy = "-created_at" // Most recent first
        });
    }
}
```

#### Create and Update Donations
```csharp
// Create a new donation
var newDonation = await _givingService.CreateDonationAsync(new DonationCreateRequest
{
    PersonId = "12345",
    PaymentMethodId = "pm_123",
    PaymentSourceId = "ps_456",
    ReceivedAt = DateTime.Now,
    AmountCents = 10000, // $100.00
    Currency = "USD",
    Designations = new[]
    {
        new DonationDesignationRequest
        {
            FundId = "fund_general",
            AmountCents = 7500 // $75.00
        },
        new DonationDesignationRequest
        {
            FundId = "fund_missions",
            AmountCents = 2500 // $25.00
        }
    }
});

// Update a donation
var updatedDonation = await _givingService.UpdateDonationAsync("donation123", new DonationUpdateRequest
{
    ReceivedAt = DateTime.Now.AddDays(-1),
    Note = "Updated donation with additional information"
});

// Delete a donation
await _givingService.DeleteDonationAsync("donation123");
```

#### Donation Queries by Date and Person
```csharp
// Get donations for a specific person
var personDonations = await _givingService.ListDonationsAsync(new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["person_id"] = "12345"
    },
    OrderBy = "-received_at",
    PerPage = 50
});

// Get donations for a date range
var startDate = DateTime.Today.AddDays(-30);
var endDate = DateTime.Today;

var recentDonations = await _givingService.ListDonationsAsync(new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["received_at"] = $"{startDate:yyyy-MM-dd}..{endDate:yyyy-MM-dd}"
    },
    OrderBy = "-received_at"
});
```

### Batch Management

#### Batch Operations
```csharp
// Get a batch by ID
var batch = await _givingService.GetBatchAsync("batch123");

// List batches
var batches = await _givingService.ListBatchesAsync(new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["status"] = "open"
    },
    OrderBy = "-created_at"
});

// Create a new batch
var newBatch = await _givingService.CreateBatchAsync(new BatchCreateRequest
{
    Description = "Sunday Service - Week of " + DateTime.Today.ToString("yyyy-MM-dd"),
    CommittedAt = DateTime.Now,
    TotalCents = 0 // Will be calculated as donations are added
});

// Update a batch
var updatedBatch = await _givingService.UpdateBatchAsync("batch123", new BatchUpdateRequest
{
    Description = "Updated batch description",
    Status = "committed"
});

// Delete a batch
await _givingService.DeleteBatchAsync("batch123");
```

#### Batch Processing
```csharp
// Add donations to a batch
public async Task ProcessDonationBatchAsync(string batchId, IEnumerable<DonationCreateRequest> donations)
{
    foreach (var donationRequest in donations)
    {
        donationRequest.BatchId = batchId;
        await _givingService.CreateDonationAsync(donationRequest);
    }
    
    // Update batch total
    var batch = await _givingService.GetBatchAsync(batchId);
    var batchDonations = await _givingService.ListDonationsAsync(new QueryParameters
    {
        Where = new Dictionary<string, object> { ["batch_id"] = batchId }
    });
    
    var totalCents = batchDonations.Data.Sum(d => d.AmountCents);
    
    await _givingService.UpdateBatchAsync(batchId, new BatchUpdateRequest
    {
        TotalCents = totalCents,
        Status = "committed"
    });
}
```

### Fund Management

#### Fund Operations
```csharp
// Get a fund by ID
var fund = await _givingService.GetFundAsync("fund123");

// List funds
var funds = await _givingService.ListFundsAsync(new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["visibility"] = "everyone"
    },
    OrderBy = "name"
});

// Create a new fund
var newFund = await _givingService.CreateFundAsync(new FundCreateRequest
{
    Name = "Building Fund",
    Description = "Funds for new building construction",
    Visibility = "everyone",
    Color = "#4CAF50",
    Default = false
});

// Update a fund
var updatedFund = await _givingService.UpdateFundAsync("fund123", new FundUpdateRequest
{
    Name = "Building & Expansion Fund",
    Description = "Updated description for building fund",
    Goal = 50000000 // $500,000.00 goal in cents
});

// Delete a fund
await _givingService.DeleteFundAsync("fund123");
```

### Pledge Management

#### Pledge Operations
```csharp
// Get a pledge by ID
var pledge = await _givingService.GetPledgeAsync("pledge123");

// List pledges
var pledges = await _givingService.ListPledgesAsync(new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["person_id"] = "12345"
    },
    OrderBy = "-created_at"
});

// Create a new pledge
var newPledge = await _givingService.CreatePledgeAsync(new PledgeCreateRequest
{
    PersonId = "12345",
    FundId = "fund_building",
    AmountCents = 120000, // $1,200.00
    JointGiverPersonId = "67890", // Spouse
    DonatedTotalCents = 0
});

// Update a pledge
var updatedPledge = await _givingService.UpdatePledgeAsync("pledge123", new PledgeUpdateRequest
{
    AmountCents = 150000, // Increased to $1,500.00
    DonatedTotalCents = 30000 // $300.00 donated so far
});

// Delete a pledge
await _givingService.DeletePledgeAsync("pledge123");
```

### Recurring Donation Management

#### Recurring Donation Operations
```csharp
// Get a recurring donation by ID
var recurringDonation = await _givingService.GetRecurringDonationAsync("rd123");

// List recurring donations
var recurringDonations = await _givingService.ListRecurringDonationsAsync(new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["status"] = "active"
    },
    OrderBy = "person_id"
});

// Create a new recurring donation
var newRecurringDonation = await _givingService.CreateRecurringDonationAsync(new RecurringDonationCreateRequest
{
    PersonId = "12345",
    PaymentSourceId = "ps_456",
    AmountCents = 25000, // $250.00
    Schedule = "monthly",
    StartDate = DateTime.Today.AddDays(1),
    Designations = new[]
    {
        new RecurringDonationDesignationRequest
        {
            FundId = "fund_general",
            AmountCents = 20000 // $200.00
        },
        new RecurringDonationDesignationRequest
        {
            FundId = "fund_missions",
            AmountCents = 5000 // $50.00
        }
    }
});

// Update a recurring donation
var updatedRecurringDonation = await _givingService.UpdateRecurringDonationAsync("rd123", new RecurringDonationUpdateRequest
{
    AmountCents = 30000, // Increased to $300.00
    Status = "active"
});

// Delete a recurring donation
await _givingService.DeleteRecurringDonationAsync("rd123");
```

### Payment Source Management

#### Payment Source Operations
```csharp
// Get a payment source by ID
var paymentSource = await _givingService.GetPaymentSourceAsync("ps123");

// List payment sources for a person
var paymentSources = await _givingService.ListPaymentSourcesAsync(new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["person_id"] = "12345"
    }
});

// Create a new payment source (typically done through secure forms)
var newPaymentSource = await _givingService.CreatePaymentSourceAsync(new PaymentSourceCreateRequest
{
    PersonId = "12345",
    Name = "Primary Checking",
    Type = "bank_account",
    // Note: Sensitive payment details would be handled securely
});

// Update a payment source
var updatedPaymentSource = await _givingService.UpdatePaymentSourceAsync("ps123", new PaymentSourceUpdateRequest
{
    Name = "Updated Account Name"
});

// Delete a payment source
await _givingService.DeletePaymentSourceAsync("ps123");
```

### Refund Management

#### Refund Operations
```csharp
// Get a refund by ID
var refund = await _givingService.GetRefundAsync("refund123");

// List refunds
var refunds = await _givingService.ListRefundsAsync(new QueryParameters
{
    OrderBy = "-created_at",
    PerPage = 25
});

// Create a new refund
var newRefund = await _givingService.CreateRefundAsync(new RefundCreateRequest
{
    DonationId = "donation123",
    AmountCents = 5000, // $50.00 partial refund
    Reason = "Duplicate donation",
    RefundedAt = DateTime.Now
});
```

### Pagination Helpers
```csharp
// Get all donations for a person (handles pagination automatically)
var allPersonDonations = await _givingService.GetAllDonationsAsync(new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["person_id"] = "12345"
    }
});

// Stream donations for memory-efficient processing
await foreach (var donation in _givingService.StreamDonationsAsync())
{
    Console.WriteLine($"${donation.AmountCents / 100.0:F2} - {donation.ReceivedAt:yyyy-MM-dd}");
    // Process one donation at a time without loading all into memory
}
```

## 🚀 Fluent API Usage

### Basic Queries
```csharp
public class GivingFluentService
{
    private readonly IPlanningCenterClient _client;
    
    public GivingFluentService(IPlanningCenterClient client)
    {
        _client = client;
    }
    
    // Get a donation by ID
    public async Task<Donation?> GetDonationAsync(string id)
    {
        return await _client.Giving().GetAsync(id);
    }
    
    // Get first page of donations
    public async Task<IPagedResponse<Donation>> GetDonationsPageAsync()
    {
        return await _client.Giving().GetPagedAsync(pageSize: 25);
    }
}
```

### Advanced Donation Filtering
```csharp
// Get large donations from this year
var largeDonations = await _client.Giving()
    .Where(d => d.AmountCents >= 100000) // $1,000 or more
    .Where(d => d.ReceivedAt >= new DateTime(DateTime.Now.Year, 1, 1))
    .OrderByDescending(d => d.AmountCents)
    .GetAllAsync();

// Get donations for a specific fund
var missionsDonations = await _client.Giving()
    .Where(d => d.Designations.Any(des => des.FundId == "fund_missions"))
    .Where(d => d.ReceivedAt >= DateTime.Today.AddDays(-30))
    .OrderByDescending(d => d.ReceivedAt)
    .GetAllAsync();

// Get online donations
var onlineDonations = await _client.Giving()
    .Where(d => d.PaymentMethod.MethodType == "online")
    .Where(d => d.ReceivedAt >= DateTime.Today.AddDays(-7))
    .OrderByDescending(d => d.ReceivedAt)
    .GetPagedAsync(pageSize: 50);

// Get donations by person
var personDonations = await _client.Giving()
    .Where(d => d.PersonId == "12345")
    .OrderByDescending(d => d.ReceivedAt)
    .GetAllAsync();
```

### Batch and Fund Filtering
```csharp
// Get open batches
var openBatches = await _client.Giving()
    .Batches()
    .Where(b => b.Status == "open")
    .OrderByDescending(b => b.CreatedAt)
    .GetAllAsync();

// Get active funds
var activeFunds = await _client.Giving()
    .Funds()
    .Where(f => f.Visibility == "everyone")
    .Where(f => f.Archived == false)
    .OrderBy(f => f.Name)
    .GetAllAsync();

// Get funds with goals
var fundsWithGoals = await _client.Giving()
    .Funds()
    .Where(f => f.Goal > 0)
    .OrderByDescending(f => f.Goal)
    .GetAllAsync();
```

### Pledge and Recurring Donation Filtering
```csharp
// Get active pledges
var activePledges = await _client.Giving()
    .Pledges()
    .Where(p => p.AmountCents > p.DonatedTotalCents)
    .OrderBy(p => p.PersonId)
    .GetAllAsync();

// Get active recurring donations
var activeRecurring = await _client.Giving()
    .RecurringDonations()
    .Where(rd => rd.Status == "active")
    .Where(rd => rd.Schedule == "monthly")
    .OrderBy(rd => rd.PersonId)
    .GetAllAsync();

// Get recurring donations ending soon
var endingSoon = await _client.Giving()
    .RecurringDonations()
    .Where(rd => rd.EndDate.HasValue)
    .Where(rd => rd.EndDate <= DateTime.Today.AddDays(30))
    .OrderBy(rd => rd.EndDate)
    .GetAllAsync();
```

### LINQ-like Terminal Operations
```csharp
// Get largest donation this year
var largestDonation = await _client.Giving()
    .Where(d => d.ReceivedAt >= new DateTime(DateTime.Now.Year, 1, 1))
    .OrderByDescending(d => d.AmountCents)
    .FirstAsync();

// Get total donation amount for a person
var personTotal = await _client.Giving()
    .Where(d => d.PersonId == "12345")
    .Where(d => d.ReceivedAt >= new DateTime(DateTime.Now.Year, 1, 1))
    .GetAllAsync()
    .ContinueWith(task => task.Result.Sum(d => d.AmountCents));

// Count donations this month
var monthlyCount = await _client.Giving()
    .Where(d => d.ReceivedAt >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1))
    .CountAsync();

// Check if person has donated this year
var hasDonated = await _client.Giving()
    .AnyAsync(d => d.PersonId == "12345" && 
                   d.ReceivedAt >= new DateTime(DateTime.Now.Year, 1, 1));
```

### Memory-Efficient Streaming
```csharp
// Stream all donations for reporting
await foreach (var donation in _client.Giving()
    .Where(d => d.ReceivedAt >= DateTime.Today.AddDays(-365))
    .OrderBy(d => d.ReceivedAt)
    .AsAsyncEnumerable())
{
    // Process one donation at a time
    await ProcessDonationForReportAsync(donation);
}

// Stream with custom pagination for large datasets
var options = new PaginationOptions { PageSize = 100 };
await foreach (var donation in _client.Giving()
    .Where(d => d.ReceivedAt >= new DateTime(DateTime.Now.Year, 1, 1))
    .AsAsyncEnumerable(options))
{
    await ProcessDonationAsync(donation);
}
```

## 💡 Common Use Cases

### 1. Monthly Giving Report
```csharp
public async Task<GivingReport> GenerateMonthlyReportAsync(int year, int month)
{
    var startDate = new DateTime(year, month, 1);
    var endDate = startDate.AddMonths(1).AddDays(-1);
    
    var donations = await _client.Giving()
        .Where(d => d.ReceivedAt >= startDate)
        .Where(d => d.ReceivedAt <= endDate)
        .GetAllAsync();
    
    return new GivingReport
    {
        Period = $"{year}-{month:D2}",
        TotalAmount = donations.Sum(d => d.AmountCents) / 100.0m,
        DonationCount = donations.Count,
        UniqueGivers = donations.Select(d => d.PersonId).Distinct().Count(),
        AverageGift = donations.Any() ? donations.Average(d => d.AmountCents) / 100.0m : 0,
        LargestGift = donations.Any() ? donations.Max(d => d.AmountCents) / 100.0m : 0
    };
}

public class GivingReport
{
    public string Period { get; set; } = "";
    public decimal TotalAmount { get; set; }
    public int DonationCount { get; set; }
    public int UniqueGivers { get; set; }
    public decimal AverageGift { get; set; }
    public decimal LargestGift { get; set; }
}
```

### 2. Donor Stewardship
```csharp
public async Task<IReadOnlyList<DonorSummary>> GetTopDonorsAsync(int year, int count = 50)
{
    var startDate = new DateTime(year, 1, 1);
    var endDate = new DateTime(year + 1, 1, 1);
    
    var donations = await _client.Giving()
        .Where(d => d.ReceivedAt >= startDate)
        .Where(d => d.ReceivedAt < endDate)
        .GetAllAsync();
    
    return donations
        .GroupBy(d => d.PersonId)
        .Select(g => new DonorSummary
        {
            PersonId = g.Key,
            TotalAmount = g.Sum(d => d.AmountCents) / 100.0m,
            DonationCount = g.Count(),
            FirstGift = g.Min(d => d.ReceivedAt),
            LastGift = g.Max(d => d.ReceivedAt)
        })
        .OrderByDescending(ds => ds.TotalAmount)
        .Take(count)
        .ToList()
        .AsReadOnly();
}

public class DonorSummary
{
    public string PersonId { get; set; } = "";
    public decimal TotalAmount { get; set; }
    public int DonationCount { get; set; }
    public DateTime FirstGift { get; set; }
    public DateTime LastGift { get; set; }
}
```

### 3. Fund Performance Tracking
```csharp
public async Task<IReadOnlyList<FundPerformance>> GetFundPerformanceAsync(DateTime startDate, DateTime endDate)
{
    var donations = await _client.Giving()
        .Where(d => d.ReceivedAt >= startDate)
        .Where(d => d.ReceivedAt <= endDate)
        .GetAllAsync();
    
    var funds = await _client.Giving()
        .Funds()
        .Where(f => f.Archived == false)
        .GetAllAsync();
    
    var fundPerformance = new List<FundPerformance>();
    
    foreach (var fund in funds)
    {
        var fundDonations = donations
            .Where(d => d.Designations.Any(des => des.FundId == fund.Id))
            .ToList();
        
        var totalAmount = fundDonations
            .SelectMany(d => d.Designations)
            .Where(des => des.FundId == fund.Id)
            .Sum(des => des.AmountCents) / 100.0m;
        
        fundPerformance.Add(new FundPerformance
        {
            FundId = fund.Id,
            FundName = fund.Name,
            Goal = fund.Goal / 100.0m,
            TotalRaised = totalAmount,
            PercentOfGoal = fund.Goal > 0 ? (totalAmount / (fund.Goal / 100.0m)) * 100 : 0,
            DonationCount = fundDonations.Count,
            UniqueGivers = fundDonations.Select(d => d.PersonId).Distinct().Count()
        });
    }
    
    return fundPerformance.OrderByDescending(fp => fp.TotalRaised).ToList().AsReadOnly();
}

public class FundPerformance
{
    public string FundId { get; set; } = "";
    public string FundName { get; set; } = "";
    public decimal Goal { get; set; }
    public decimal TotalRaised { get; set; }
    public decimal PercentOfGoal { get; set; }
    public int DonationCount { get; set; }
    public int UniqueGivers { get; set; }
}
```

### 4. Recurring Donation Management
```csharp
public async Task<IReadOnlyList<RecurringDonation>> GetFailedRecurringDonationsAsync()
{
    return await _client.Giving()
        .RecurringDonations()
        .Where(rd => rd.Status == "failed")
        .OrderBy(rd => rd.PersonId)
        .GetAllAsync();
}

public async Task<IReadOnlyList<RecurringDonation>> GetRecurringDonationsEndingSoonAsync(int days = 30)
{
    var cutoffDate = DateTime.Today.AddDays(days);
    
    return await _client.Giving()
        .RecurringDonations()
        .Where(rd => rd.Status == "active")
        .Where(rd => rd.EndDate.HasValue)
        .Where(rd => rd.EndDate <= cutoffDate)
        .OrderBy(rd => rd.EndDate)
        .GetAllAsync();
}
```

### 5. Donation Data Export
```csharp
public async Task ExportDonationsAsync(string filePath, DateTime startDate, DateTime endDate)
{
    using var writer = new StreamWriter(filePath);
    await writer.WriteLineAsync("Date,PersonId,Amount,Fund,PaymentMethod,BatchId");
    
    await foreach (var donation in _client.Giving()
        .Where(d => d.ReceivedAt >= startDate)
        .Where(d => d.ReceivedAt <= endDate)
        .OrderBy(d => d.ReceivedAt)
        .AsAsyncEnumerable())
    {
        foreach (var designation in donation.Designations)
        {
            var line = $"{donation.ReceivedAt:yyyy-MM-dd}," +
                      $"{donation.PersonId}," +
                      $"{designation.AmountCents / 100.0:F2}," +
                      $"{designation.FundId}," +
                      $"{donation.PaymentMethod?.MethodType}," +
                      $"{donation.BatchId}";
            
            await writer.WriteLineAsync(line);
        }
    }
}
```

### 6. Pledge Tracking
```csharp
public async Task<IReadOnlyList<PledgeProgress>> GetPledgeProgressAsync()
{
    var pledges = await _client.Giving()
        .Pledges()
        .Where(p => p.AmountCents > 0)
        .GetAllAsync();
    
    return pledges.Select(p => new PledgeProgress
    {
        PledgeId = p.Id,
        PersonId = p.PersonId,
        PledgeAmount = p.AmountCents / 100.0m,
        DonatedAmount = p.DonatedTotalCents / 100.0m,
        RemainingAmount = (p.AmountCents - p.DonatedTotalCents) / 100.0m,
        PercentComplete = p.AmountCents > 0 ? (p.DonatedTotalCents / (decimal)p.AmountCents) * 100 : 0,
        FundId = p.FundId
    }).ToList().AsReadOnly();
}

public class PledgeProgress
{
    public string PledgeId { get; set; } = "";
    public string PersonId { get; set; } = "";
    public decimal PledgeAmount { get; set; }
    public decimal DonatedAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public decimal PercentComplete { get; set; }
    public string FundId { get; set; } = "";
}
```

## 📊 Advanced Features

### Financial Calculations
```csharp
// Calculate year-over-year growth
public async Task<decimal> CalculateYearOverYearGrowthAsync(int currentYear)
{
    var currentYearStart = new DateTime(currentYear, 1, 1);
    var currentYearEnd = new DateTime(currentYear + 1, 1, 1);
    var previousYearStart = new DateTime(currentYear - 1, 1, 1);
    var previousYearEnd = new DateTime(currentYear, 1, 1);
    
    var currentYearTotal = await _client.Giving()
        .Where(d => d.ReceivedAt >= currentYearStart)
        .Where(d => d.ReceivedAt < currentYearEnd)
        .GetAllAsync()
        .ContinueWith(task => task.Result.Sum(d => d.AmountCents));
    
    var previousYearTotal = await _client.Giving()
        .Where(d => d.ReceivedAt >= previousYearStart)
        .Where(d => d.ReceivedAt < previousYearEnd)
        .GetAllAsync()
        .ContinueWith(task => task.Result.Sum(d => d.AmountCents));
    
    if (previousYearTotal == 0) return 0;
    
    return ((decimal)(currentYearTotal - previousYearTotal) / previousYearTotal) * 100;
}
```

### Batch Operations
```csharp
// Create multiple donations in a batch
var batch = _client.Giving().Batch();

batch.CreateDonation(new DonationCreateRequest
{
    PersonId = "12345",
    AmountCents = 10000,
    ReceivedAt = DateTime.Now
});

batch.CreateDonation(new DonationCreateRequest
{
    PersonId = "67890",
    AmountCents = 5000,
    ReceivedAt = DateTime.Now
});

batch.CreateBatch(new BatchCreateRequest
{
    Description = "Sunday Service Batch",
    CommittedAt = DateTime.Now
});

var results = await batch.ExecuteAsync();
```

### Performance Monitoring
```csharp
// Monitor query performance
var result = await _client.Giving()
    .Where(d => d.ReceivedAt >= DateTime.Today.AddDays(-365))
    .ExecuteWithDebugInfoAsync();

Console.WriteLine($"Query took {result.ExecutionTime.TotalMilliseconds}ms");
Console.WriteLine($"Returned {result.Data?.Data.Count ?? 0} donations");
```

## 🎯 Best Practices

1. **Use Date Ranges**: Always filter by date ranges to avoid loading unnecessary historical data.

2. **Handle Money Carefully**: All amounts are in cents to avoid floating-point precision issues.

3. **Validate Designations**: Ensure donation designations add up to the total donation amount.

4. **Secure Payment Data**: Never log or store sensitive payment information.

5. **Use Batch Processing**: Group related donations into batches for better organization.

6. **Monitor Recurring Donations**: Regularly check for failed recurring donations.

7. **Track Fund Performance**: Monitor fund goals and progress for stewardship.

8. **Stream Large Datasets**: Use streaming for financial reports with large date ranges.

9. **Handle Refunds Properly**: Always create refund records for audit trails.

10. **Validate Permissions**: Ensure users have appropriate access to financial data.

### Error Handling
```csharp
public async Task<Donation?> SafeCreateDonationAsync(DonationCreateRequest request)
{
    try
    {
        return await _client.Giving().CreateDonation(request).ExecuteAsync();
    }
    catch (PlanningCenterApiValidationException ex)
    {
        // Handle validation errors (e.g., invalid amounts, missing required fields)
        _logger.LogWarning(ex, "Validation error when creating donation: {Errors}", ex.FormattedErrors);
        throw;
    }
    catch (PlanningCenterApiAuthorizationException ex)
    {
        // Handle permission errors
        _logger.LogError(ex, "Authorization error when creating donation: {Message}", ex.Message);
        throw;
    }
    catch (PlanningCenterApiException ex)
    {
        // Log API error with correlation ID
        _logger.LogError(ex, "API error when creating donation: {ErrorMessage} [RequestId: {RequestId}]", 
            ex.Message, ex.RequestId);
        throw;
    }
}
```

This Giving module documentation provides comprehensive coverage of financial management capabilities with both traditional and fluent API usage patterns, including practical examples for common giving scenarios.