# ServiceBase Migration Example

This document demonstrates how to migrate existing services to use the new ServiceBase pattern with logging and exception handling improvements.

## Before: Traditional Service Pattern

```csharp
public class TraditionalService : IExampleService
{
    private readonly IApiConnection _apiConnection;
    private readonly ILogger<TraditionalService> _logger;

    public TraditionalService(IApiConnection apiConnection, ILogger<TraditionalService> logger)
    {
        _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Person?> GetPersonAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Person ID cannot be null or empty", nameof(id));

        _logger.LogDebug("Getting person with ID: {PersonId}", id);

        try
        {
            var response = await _apiConnection.GetAsync<JsonApiSingleResponse<PersonDto>>(
                $"/people/v2/people/{id}", cancellationToken);

            if (response?.Data == null)
            {
                _logger.LogWarning("Person not found: {PersonId}", id);
                return null;
            }

            var person = PersonMapper.MapToDomain(response.Data);
            _logger.LogInformation("Successfully retrieved person: {PersonId}", id);
            return person;
        }
        catch (PlanningCenterApiNotFoundException)
        {
            _logger.LogWarning("Person not found: {PersonId}", id);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting person with ID: {PersonId}", id);
            throw;
        }
    }

    public async Task DeletePersonAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Person ID cannot be null or empty", nameof(id));

        _logger.LogDebug("Deleting person: {PersonId}", id);

        try
        {
            await _apiConnection.DeleteAsync($"/people/v2/people/{id}", cancellationToken);
            _logger.LogInformation("Successfully deleted person: {PersonId}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting person with ID: {PersonId}", id);
            throw;
        }
    }
}
```

## After: ServiceBase Pattern

```csharp
public class ModernService : ServiceBase, IExampleService
{
    private const string BaseEndpoint = "/people/v2";

    public ModernService(IApiConnection apiConnection, ILogger<ModernService> logger)
        : base(logger, apiConnection)
    {
    }

    public async Task<Person?> GetPersonAsync(string id, CancellationToken cancellationToken = default)
    {
        ValidateNotNullOrEmpty(id, nameof(id));

        return await ExecuteGetAsync(
            async () =>
            {
                var response = await ApiConnection.GetAsync<JsonApiSingleResponse<PersonDto>>(
                    $"{BaseEndpoint}/people/{id}", cancellationToken);

                if (response?.Data == null)
                {
                    throw new PlanningCenterApiNotFoundException($"Person with ID {id} not found");
                }

                return PersonMapper.MapToDomain(response.Data);
            },
            "GetPerson",
            id,
            cancellationToken);
    }

    public async Task DeletePersonAsync(string id, CancellationToken cancellationToken = default)
    {
        ValidateNotNullOrEmpty(id, nameof(id));

        await ExecuteAsync(
            async () => await ApiConnection.DeleteAsync($"{BaseEndpoint}/people/{id}", cancellationToken),
            "DeletePerson",
            id,
            cancellationToken);
    }
}
```

## Key Benefits of ServiceBase Pattern

### 1. **Automatic Correlation ID Management**
Every operation gets a correlation ID for request tracing:
```
[12:34:56 INF] Operation GetPerson completed successfully in 150ms [CorrelationId: abc12345]
```

### 2. **Performance Monitoring**
Automatic timing and performance logging:
```
[12:34:56 DBG] Starting operation GetPerson [CorrelationId: abc12345]
[12:34:56 INF] Operation GetPerson completed successfully in 150ms [CorrelationId: abc12345]
```

### 3. **Standardized Exception Handling**
Consistent error handling across all services:
```
[12:34:56 WRN] Resource not found: person-123 [CorrelationId: abc12345]
[12:34:56 ERR] API error in GetPerson for resource person-123: Invalid request [CorrelationId: abc12345]
```

### 4. **Built-in Validation**
Standardized parameter validation:
```csharp
ValidateNotNullOrEmpty(id, nameof(id));
ValidateNotNull(request, nameof(request));
```

### 5. **Reduced Boilerplate**
- No manual exception handling
- No manual logging setup
- No manual correlation ID management
- No manual performance tracking

## Migration Steps

1. **Change base class**: `: IService` → `: ServiceBase, IService`
2. **Update constructor**: Call `base(logger, apiConnection)`
3. **Remove field declarations**: `_logger` and `_apiConnection` are now inherited
4. **Wrap operations**: Use `ExecuteAsync` or `ExecuteGetAsync`
5. **Use validation methods**: Replace manual validation with `ValidateNotNullOrEmpty`
6. **Remove manual logging**: The base class handles all logging automatically

## Method Patterns

### GET Operations (nullable return)
```csharp
public async Task<T?> GetAsync(string id, CancellationToken cancellationToken = default)
{
    ValidateNotNullOrEmpty(id, nameof(id));
    
    return await ExecuteGetAsync(
        async () => {
            // API call logic
            return result;
        },
        "OperationName",
        id,
        cancellationToken);
}
```

### POST/PUT Operations (non-nullable return)
```csharp
public async Task<T> CreateAsync(CreateRequest request, CancellationToken cancellationToken = default)
{
    ValidateNotNull(request, nameof(request));
    
    return await ExecuteAsync(
        async () => {
            // API call logic
            return result;
        },
        "OperationName",
        null,
        cancellationToken);
}
```

### DELETE Operations (void return)
```csharp
public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
{
    ValidateNotNullOrEmpty(id, nameof(id));
    
    await ExecuteAsync(
        async () => await ApiConnection.DeleteAsync($"/endpoint/{id}", cancellationToken),
        "OperationName",
        id,
        cancellationToken);
}
```

## Testing the Migration

After migrating, you should see improved logging with correlation IDs and performance metrics:

```
[12:34:56 DBG] Starting operation GetPerson for resource person-123 [CorrelationId: abc12345]
[12:34:56 INF] Operation GetPerson completed successfully in 150ms [CorrelationId: abc12345]
```

The ServiceBase pattern ensures consistent, traceable, and performant operations across all services in the Planning Center SDK.