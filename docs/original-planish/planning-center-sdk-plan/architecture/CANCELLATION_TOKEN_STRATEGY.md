# Cancellation Token Strategy for Planning Center SDK

## Overview

The Planning Center SDK implements comprehensive cancellation token support throughout all asynchronous operations. This enables consumers to cancel long-running operations, implement timeouts, and provide responsive user experiences while preventing resource leaks and unnecessary API calls.

## Cancellation Token Usage Principles

### 1. **Universal Async Method Support**
Every public async method in the SDK accepts a `CancellationToken` parameter with a default value of `default`.

### 2. **Propagation Through Call Chain**
Cancellation tokens are propagated through the entire call chain from public API down to HTTP requests.

### 3. **Graceful Cancellation**
Operations are cancelled gracefully with proper cleanup and meaningful exceptions.

### 4. **Resource Management**
Cancellation prevents resource leaks and unnecessary network calls.

## Implementation Patterns

### Service Interface Pattern
```csharp
public interface IPeopleService
{
    // All async methods include CancellationToken with default value
    Task<Core.Person> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Core.Person>> ListAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Core.Person> CreateAsync(PersonCreateRequest request, CancellationToken cancellationToken = default);
    Task<Core.Person> UpdateAsync(string id, PersonUpdateRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    
    // Bulk operations with cancellation support
    Task<BulkResult<Core.Person>> BulkCreateAsync(IEnumerable<PersonCreateRequest> requests, CancellationToken cancellationToken = default);
    Task<BulkResult<Core.Person>> BulkUpdateAsync(IEnumerable<PersonUpdateRequest> requests, CancellationToken cancellationToken = default);
    
    // Long-running operations
    Task<SyncResult> SyncPeopleAsync(SyncRequest request, IProgress<SyncProgress> progress = null, CancellationToken cancellationToken = default);
    Task<ExportResult> ExportPeopleAsync(ExportRequest request, IProgress<ExportProgress> progress = null, CancellationToken cancellationToken = default);
}
```

### Fluent API Pattern
```csharp
public interface IPeopleFluentContext
{
    IPeopleFluentContext Where(Expression<Func<Core.Person, bool>> predicate);
    IPeopleFluentContext Include(Expression<Func<Core.Person, object>> include);
    IPeopleFluentContext OrderBy(Expression<Func<Core.Person, object>> orderBy);
    
    // All terminal operations support cancellation
    Task<Core.Person> GetAsync(CancellationToken cancellationToken = default);
    Task<Core.Person> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Core.Person>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default);
    Task<List<Core.Person>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
    
    // Streaming operations
    IAsyncEnumerable<Core.Person> AsAsyncEnumerable(CancellationToken cancellationToken = default);
}
```

### HTTP Client Layer Implementation
```csharp
namespace PlanningCenter.Api.Client.Http
{
    public class ApiConnection : IApiConnection
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiConnection> _logger;

        public async Task<T> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
        {
            return await ExecuteRequestAsync<T>(HttpMethod.Get, endpoint, null, cancellationToken);
        }

        public async Task<T> PostAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default)
        {
            return await ExecuteRequestAsync<T>(HttpMethod.Post, endpoint, data, cancellationToken);
        }

        public async Task<T> PutAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default)
        {
            return await ExecuteRequestAsync<T>(HttpMethod.Put, endpoint, data, cancellationToken);
        }

        public async Task<T> PatchAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default)
        {
            return await ExecuteRequestAsync<T>(HttpMethod.Patch, endpoint, data, cancellationToken);
        }

        public async Task DeleteAsync(string endpoint, CancellationToken cancellationToken = default)
        {
            await ExecuteRequestAsync<object>(HttpMethod.Delete, endpoint, null, cancellationToken);
        }

        public async Task<IPagedResponse<T>> GetPagedAsync<T>(
            string endpoint, 
            QueryParameters parameters = null, 
            CancellationToken cancellationToken = default)
        {
            // Implementation with cancellation token propagation
            var response = await ExecuteRequestAsync<PagedResponseDto<T>>(HttpMethod.Get, BuildUrl(endpoint, parameters), null, cancellationToken);
            return MapToPagedResponse(response);
        }

        private async Task<T> ExecuteRequestAsync<T>(
            HttpMethod method, 
            string endpoint, 
            object data, 
            CancellationToken cancellationToken)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object>
            {
                ["RequestId"] = Guid.NewGuid().ToString("N")[..8],
                ["Method"] = method.Method,
                ["Endpoint"] = endpoint
            });

            try
            {
                // Create request with cancellation token
                using var request = new HttpRequestMessage(method, endpoint);
                
                if (data != null)
                {
                    var json = JsonSerializer.Serialize(data);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                _logger.LogDebug("Making {Method} request to {Endpoint}", method.Method, endpoint);

                // Pass cancellation token to HTTP client
                using var response = await _httpClient.SendAsync(request, cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var exception = ApiExceptionFactory.CreateException(response, content, endpoint, method.Method);
                    throw exception;
                }

                if (typeof(T) == typeof(object))
                {
                    return default(T);
                }

                return JsonSerializer.Deserialize<T>(content);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("Request to {Endpoint} was cancelled", endpoint);
                throw; // Re-throw cancellation exceptions
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during {Method} request to {Endpoint}", method.Method, endpoint);
                throw;
            }
        }
    }
}
```

### Service Implementation with Cancellation
```csharp
namespace PlanningCenter.Api.Client.Services
{
    public class PeopleService : IPeopleService
    {
        private readonly IApiConnection _apiConnection;
        private readonly PersonMapper _personMapper;
        private readonly ILogger<PeopleService> _logger;

        public async Task<Core.Person> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Retrieving person {PersonId}", id);

            try
            {
                // Pass cancellation token through the call chain
                var dto = await _apiConnection.GetAsync<People.PersonDto>($"/people/v2/people/{id}", cancellationToken);
                var person = _personMapper.Map(dto);

                _logger.LogInformation("Successfully retrieved person {PersonId}: {PersonName}", id, person.FullName);
                return person;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("Get person {PersonId} operation was cancelled", id);
                throw;
            }
        }

        public async Task<IPagedResponse<Core.Person>> ListAsync(
            QueryParameters parameters = null, 
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Listing people with parameters: {@Parameters}", parameters);

            try
            {
                var response = await _apiConnection.GetPagedAsync<People.PersonDto>("/people/v2/people", parameters, cancellationToken);
                var people = response.Data.Select(_personMapper.Map).ToList();

                _logger.LogInformation("Successfully retrieved {Count} people", people.Count);
                return new PagedResponse<Core.Person>
                {
                    Data = people,
                    Meta = response.Meta,
                    Links = response.Links
                };
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("List people operation was cancelled");
                throw;
            }
        }

        public async Task<BulkResult<Core.Person>> BulkCreateAsync(
            IEnumerable<PersonCreateRequest> requests, 
            CancellationToken cancellationToken = default)
        {
            var requestList = requests.ToList();
            _logger.LogInformation("Starting bulk create operation for {Count} people", requestList.Count);

            var results = new List<BulkResultItem<Core.Person>>();
            var semaphore = new SemaphoreSlim(5); // Limit concurrent requests

            try
            {
                var tasks = requestList.Select(async (request, index) =>
                {
                    await semaphore.WaitAsync(cancellationToken);
                    try
                    {
                        // Check for cancellation before each operation
                        cancellationToken.ThrowIfCancellationRequested();

                        var person = await CreateAsync(request, cancellationToken);
                        return new BulkResultItem<Core.Person>
                        {
                            Index = index,
                            Success = true,
                            Data = person
                        };
                    }
                    catch (OperationCanceledException)
                    {
                        throw; // Re-throw cancellation
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to create person at index {Index}", index);
                        return new BulkResultItem<Core.Person>
                        {
                            Index = index,
                            Success = false,
                            Error = ex.Message
                        };
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                var taskResults = await Task.WhenAll(tasks);
                results.AddRange(taskResults);

                _logger.LogInformation("Bulk create completed: {Successful}/{Total} successful", 
                    results.Count(r => r.Success), results.Count);

                return new BulkResult<Core.Person>
                {
                    Items = results,
                    TotalCount = results.Count,
                    SuccessCount = results.Count(r => r.Success),
                    FailureCount = results.Count(r => !r.Success)
                };
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("Bulk create operation was cancelled after processing {Count} items", results.Count);
                throw;
            }
        }

        public async Task<SyncResult> SyncPeopleAsync(
            SyncRequest request, 
            IProgress<SyncProgress> progress = null, 
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting people sync operation");

            var syncResult = new SyncResult { StartTime = DateTime.UtcNow };
            var processedCount = 0;
            var totalCount = 0;

            try
            {
                // Get total count first
                var countResponse = await _apiConnection.GetAsync<CountResponse>("/people/v2/people/count", cancellationToken);
                totalCount = countResponse.Count;

                progress?.Report(new SyncProgress 
                { 
                    Phase = "Starting", 
                    ProcessedCount = 0, 
                    TotalCount = totalCount 
                });

                // Process in batches
                var pageSize = 100;
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                for (int page = 1; page <= totalPages; page++)
                {
                    // Check for cancellation before each batch
                    cancellationToken.ThrowIfCancellationRequested();

                    var parameters = new QueryParameters
                    {
                        PerPage = pageSize,
                        Offset = (page - 1) * pageSize
                    };

                    var response = await ListAsync(parameters, cancellationToken);
                    
                    // Process batch
                    foreach (var person in response.Data)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        
                        // Sync logic here
                        await ProcessPersonForSync(person, cancellationToken);
                        processedCount++;

                        // Report progress every 10 items
                        if (processedCount % 10 == 0)
                        {
                            progress?.Report(new SyncProgress
                            {
                                Phase = "Processing",
                                ProcessedCount = processedCount,
                                TotalCount = totalCount
                            });
                        }
                    }

                    _logger.LogDebug("Completed page {Page}/{TotalPages}", page, totalPages);
                }

                syncResult.EndTime = DateTime.UtcNow;
                syncResult.ProcessedCount = processedCount;
                syncResult.Success = true;

                progress?.Report(new SyncProgress
                {
                    Phase = "Completed",
                    ProcessedCount = processedCount,
                    TotalCount = totalCount
                });

                _logger.LogInformation("People sync completed successfully: {ProcessedCount} people processed", processedCount);
                return syncResult;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                syncResult.EndTime = DateTime.UtcNow;
                syncResult.ProcessedCount = processedCount;
                syncResult.Success = false;
                syncResult.ErrorMessage = "Operation was cancelled";

                _logger.LogInformation("People sync was cancelled after processing {ProcessedCount}/{TotalCount} people", 
                    processedCount, totalCount);

                throw;
            }
        }

        private async Task ProcessPersonForSync(Core.Person person, CancellationToken cancellationToken)
        {
            // Simulate processing work
            await Task.Delay(10, cancellationToken);
            // Actual sync logic would go here
        }
    }
}
```

### Fluent API Implementation with Cancellation
```csharp
namespace PlanningCenter.Api.Client.Fluent
{
    public class PeopleFluentContext : IPeopleFluentContext
    {
        private readonly IPeopleService _peopleService;
        private readonly QueryBuilder _queryBuilder;

        public PeopleFluentContext(IPeopleService peopleService)
        {
            _peopleService = peopleService;
            _queryBuilder = new QueryBuilder();
        }

        public IPeopleFluentContext Where(Expression<Func<Core.Person, bool>> predicate)
        {
            _queryBuilder.AddWhere(predicate);
            return this;
        }

        public IPeopleFluentContext Include(Expression<Func<Core.Person, object>> include)
        {
            _queryBuilder.AddInclude(include);
            return this;
        }

        public async Task<Core.Person> GetAsync(CancellationToken cancellationToken = default)
        {
            var parameters = _queryBuilder.Build();
            var response = await _peopleService.ListAsync(parameters, cancellationToken);
            return response.Data.FirstOrDefault();
        }

        public async Task<Core.Person> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _peopleService.GetAsync(id, cancellationToken);
        }

        public async Task<IPagedResponse<Core.Person>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default)
        {
            var parameters = _queryBuilder.Build();
            parameters.PerPage = pageSize;
            return await _peopleService.ListAsync(parameters, cancellationToken);
        }

        public async Task<List<Core.Person>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var allPeople = new List<Core.Person>();
            var pageSize = 100;
            var page = 1;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var parameters = _queryBuilder.Build();
                parameters.PerPage = pageSize;
                parameters.Offset = (page - 1) * pageSize;

                var response = await _peopleService.ListAsync(parameters, cancellationToken);
                
                if (!response.Data.Any())
                    break;

                allPeople.AddRange(response.Data);

                if (response.Data.Count < pageSize)
                    break;

                page++;
            }

            return allPeople;
        }

        public async IAsyncEnumerable<Core.Person> AsAsyncEnumerable([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var pageSize = 100;
            var page = 1;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var parameters = _queryBuilder.Build();
                parameters.PerPage = pageSize;
                parameters.Offset = (page - 1) * pageSize;

                var response = await _peopleService.ListAsync(parameters, cancellationToken);

                if (!response.Data.Any())
                    yield break;

                foreach (var person in response.Data)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    yield return person;
                }

                if (response.Data.Count < pageSize)
                    yield break;

                page++;
            }
        }
    }
}
```

## Consumer Usage Patterns

### Basic Cancellation
```csharp
public class PersonController : ControllerBase
{
    private readonly IPeopleService _peopleService;

    [HttpGet("{id}")]
    public async Task<ActionResult<Core.Person>> GetPerson(string id, CancellationToken cancellationToken)
    {
        try
        {
            // Pass the request's cancellation token
            var person = await _peopleService.GetAsync(id, cancellationToken);
            return Ok(person);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, "Request was cancelled"); // Client Closed Request
        }
    }

    [HttpGet]
    public async Task<ActionResult<IPagedResponse<Core.Person>>> GetPeople(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        var parameters = new QueryParameters
        {
            PerPage = pageSize,
            Offset = (page - 1) * pageSize
        };

        var people = await _peopleService.ListAsync(parameters, cancellationToken);
        return Ok(people);
    }
}
```

### Timeout-Based Cancellation
```csharp
public class PeopleReportService
{
    private readonly IPeopleService _peopleService;

    public async Task<List<Core.Person>> GetAllPeopleWithTimeoutAsync(TimeSpan timeout)
    {
        using var cts = new CancellationTokenSource(timeout);
        
        try
        {
            return await _peopleService.ListAsync(cancellationToken: cts.Token)
                .ContinueWith(t => t.Result.Data.ToList(), cts.Token);
        }
        catch (OperationCanceledException)
        {
            throw new TimeoutException($"Operation timed out after {timeout.TotalSeconds} seconds");
        }
    }
}
```

### Combined Cancellation Tokens
```csharp
public class BulkOperationService
{
    private readonly IPeopleService _peopleService;

    public async Task<BulkResult<Core.Person>> ProcessPeopleWithCombinedCancellationAsync(
        IEnumerable<PersonCreateRequest> requests,
        CancellationToken userCancellationToken,
        TimeSpan timeout)
    {
        // Combine user cancellation with timeout
        using var timeoutCts = new CancellationTokenSource(timeout);
        using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(
            userCancellationToken, timeoutCts.Token);

        try
        {
            return await _peopleService.BulkCreateAsync(requests, combinedCts.Token);
        }
        catch (OperationCanceledException) when (timeoutCts.Token.IsCancellationRequested)
        {
            throw new TimeoutException($"Bulk operation timed out after {timeout.TotalSeconds} seconds");
        }
        catch (OperationCanceledException) when (userCancellationToken.IsCancellationRequested)
        {
            throw new OperationCanceledException("Operation was cancelled by user");
        }
    }
}
```

### Progress Reporting with Cancellation
```csharp
public class SyncService
{
    private readonly IPeopleService _peopleService;

    public async Task<SyncResult> SyncPeopleWithProgressAsync(
        SyncRequest request,
        IProgress<SyncProgress> progress,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _peopleService.SyncPeopleAsync(request, progress, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            progress?.Report(new SyncProgress 
            { 
                Phase = "Cancelled", 
                ProcessedCount = 0, 
                TotalCount = 0 
            });
            throw;
        }
    }
}
```

### Fluent API with Cancellation
```csharp
public class FluentApiExamples
{
    private readonly IPlanningCenterClient _client;

    public async Task<List<Core.Person>> GetActivePeopleAsync(CancellationToken cancellationToken)
    {
        return await _client
            .People()
            .Where(p => p.Status == "active")
            .Include(p => p.Addresses)
            .Include(p => p.Emails)
            .OrderBy(p => p.LastName)
            .GetAllAsync(cancellationToken);
    }

    public async IAsyncEnumerable<Core.Person> StreamActivePeopleAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var person in _client
            .People()
            .Where(p => p.Status == "active")
            .AsAsyncEnumerable(cancellationToken))
        {
            yield return person;
        }
    }
}
```

## Webhook Operations with Cancellation

### Webhook Service Implementation
```csharp
public class WebhooksService : IWebhooksService
{
    private readonly IApiConnection _apiConnection;

    public async Task<WebhookSubscription> CreateSubscriptionAsync(
        WebhookSubscriptionCreateRequest request, 
        CancellationToken cancellationToken = default)
    {
        var dto = await _apiConnection.PostAsync<WebhookSubscriptionDto>(
            "/webhooks/v2/webhook_subscriptions", request, cancellationToken);
        
        return _webhookMapper.Map(dto);
    }

    public async Task<WebhookTestResult> TestSubscriptionAsync(
        string id, 
        CancellationToken cancellationToken = default)
    {
        // This might be a long-running operation, so cancellation is important
        var result = await _apiConnection.PostAsync<WebhookTestResultDto>(
            $"/webhooks/v2/webhook_subscriptions/{id}/test", null, cancellationToken);
        
        return _webhookMapper.Map(result);
    }
}
```

## Error Handling with Cancellation

### Cancellation-Aware Exception Handling
```csharp
public class RobustPeopleService
{
    private readonly IPeopleService _peopleService;
    private readonly ILogger<RobustPeopleService> _logger;

    public async Task<Core.Person> GetPersonSafelyAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _peopleService.GetAsync(id, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("Get person {PersonId} operation was cancelled", id);
            throw; // Re-throw cancellation exceptions
        }
        catch (PlanningCenterApiNotFoundException)
        {
            _logger.LogWarning("Person {PersonId} not found", id);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving person {PersonId}", id);
            throw;
        }
    }
}
```

## Testing Cancellation Behavior

### Unit Tests for Cancellation
```csharp
[Test]
public async Task GetAsync_WhenCancelled_ThrowsOperationCancelledException()
{
    // Arrange
    var cts = new CancellationTokenSource();
    var mockApiConnection = new Mock<IApiConnection>();
    
    mockApiConnection
        .Setup(x => x.GetAsync<People.PersonDto>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
        .Returns(async (string endpoint, CancellationToken ct) =>
        {
            await Task.Delay(1000, ct); // Simulate long operation
            return new People.PersonDto();
        });

    var service = new PeopleService(mockApiConnection.Object, Mock.Of<PersonMapper>(), Mock.Of<ILogger<PeopleService>>());

    // Act & Assert
    cts.Cancel(); // Cancel immediately
    
    await service.Invoking(s => s.GetAsync("123", cts.Token))
        .Should().ThrowAsync<OperationCanceledException>();
}

[Test]
public async Task BulkCreateAsync_WhenCancelledMidOperation_StopsProcessing()
{
    // Arrange
    var cts = new CancellationTokenSource();
    var requests = Enumerable.Range(1, 100).Select(i => new PersonCreateRequest()).ToList();
    var processedCount = 0;

    var mockService = new Mock<IPeopleService>();
    mockService
        .Setup(x => x.CreateAsync(It.IsAny<PersonCreateRequest>(), It.IsAny<CancellationToken>()))
        .Returns(async (PersonCreateRequest req, CancellationToken ct) =>
        {
            await Task.Delay(100, ct);
            Interlocked.Increment(ref processedCount);
            
            if (processedCount == 10)
                cts.Cancel(); // Cancel after 10 items
                
            return new Core.Person();
        });

    // Act & Assert
    await mockService.Object.Invoking(s => s.BulkCreateAsync(requests, cts.Token))
        .Should().ThrowAsync<OperationCanceledException>();

    processedCount.Should().BeLessOrEqualTo(15); // Allow for some in-flight operations
}
```

## Best Practices Summary

### ✅ Do's
- **Always include `CancellationToken cancellationToken = default`** in all async method signatures
- **Propagate cancellation tokens** through the entire call chain
- **Check for cancellation** before expensive operations using `cancellationToken.ThrowIfCancellationRequested()`
- **Handle `OperationCanceledException`** appropriately in catch blocks
- **Use `[EnumeratorCancellation]`** attribute for `IAsyncEnumerable` methods
- **Combine cancellation tokens** when you have multiple cancellation sources
- **Log cancellation events** for debugging and monitoring

### ❌ Don'ts
- **Don't ignore cancellation tokens** - always pass them through
- **Don't catch and suppress `OperationCanceledException`** unless you have a specific reason
- **Don't create new cancellation tokens** unnecessarily - use the provided ones
- **Don't forget to dispose `CancellationTokenSource`** when creating them
- **Don't use cancellation tokens for non-cancellable operations**
- **Don't assume cancellation is immediate** - operations may complete before cancellation takes effect

### Performance Considerations
- **Minimal overhead** - cancellation token checks are very fast
- **Early cancellation** - check for cancellation before expensive operations
- **Resource cleanup** - ensure proper disposal of resources when cancelled
- **Graceful degradation** - handle partial completion in bulk operations

This comprehensive cancellation token strategy ensures that all SDK operations can be cancelled gracefully, providing responsive user experiences and preventing resource leaks in long-running operations.