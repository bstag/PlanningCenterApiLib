# Planning Center API Library - Logging Standards

This document defines the logging standards and best practices for the Planning Center API Library.

## Overview

The Planning Center API Library uses structured logging with correlation IDs and performance monitoring to provide comprehensive observability for API operations.

## Logging Levels

### Debug
Use for detailed diagnostic information that is only needed when diagnosing problems.

```csharp
_logger.LogDebug("Starting operation {OperationName} for resource {ResourceId} [CorrelationId: {CorrelationId}]", 
    operationName, resourceId, correlationId);
```

### Information
Use for general information about the normal operation of the application.

```csharp
_logger.LogInformation("Operation {OperationName} completed successfully in {ElapsedMs}ms [CorrelationId: {CorrelationId}]",
    operationName, stopwatch.ElapsedMilliseconds, correlationId);
```

### Warning
Use for potentially harmful situations that should be investigated.

```csharp
_logger.LogWarning("Resource not found: {ResourceId} [CorrelationId: {CorrelationId}]", 
    resourceId, correlationId);
```

### Error
Use for error events that might still allow the application to continue running.

```csharp
_logger.LogError(exception, "Operation {OperationName} failed for resource {ResourceId} [CorrelationId: {CorrelationId}]",
    operationName, resourceId, correlationId);
```

## Structured Logging Format

### Standard Format
All log messages should follow this structured format:

```
{LogLevel}: {Message} [CorrelationId: {CorrelationId}]
```

### Required Parameters
- **OperationName**: The name of the operation being performed
- **CorrelationId**: The correlation ID for request tracing
- **ResourceId**: The ID of the resource being operated on (when applicable)

### Optional Parameters
- **ElapsedMs**: Execution time in milliseconds
- **ResourceType**: The type of resource (Person, Event, etc.)
- **UserId**: The user performing the operation
- **Parameters**: Serialized operation parameters

## Correlation IDs

### Purpose
Correlation IDs enable tracking of requests across multiple service calls and operations.

### Implementation
Use the `CorrelationContext` to manage correlation IDs:

```csharp
// Generate a new correlation ID
var correlationId = CorrelationContext.GenerateNew();

// Use existing or generate new
var correlationId = CorrelationContext.GetOrGenerate();

// Use in logging
_logger.LogInformation("Operation completed [CorrelationId: {CorrelationId}]", correlationId);
```

### Scoping
Use `CorrelationScope` for managing correlation context:

```csharp
using var scope = CorrelationScope.Create();
// All operations within this scope will use the same correlation ID
```

## Performance Monitoring

### Performance Tracking
Use `PerformanceMonitor` for tracking operation performance:

```csharp
var result = await PerformanceMonitor.TrackAsync(
    _logger,
    "GetPerson",
    () => _apiConnection.GetAsync<Person>($"/people/{id}"),
    id,
    cancellationToken);
```

### Performance Scopes
For more complex operations, use performance scopes:

```csharp
using var perfScope = PerformanceMonitor.CreateScope(_logger, "ComplexOperation", resourceId);
try
{
    // Perform operation
    perfScope.Success();
}
catch (Exception ex)
{
    perfScope.Failure(ex);
    throw;
}
```

## Exception Handling

### Standard Exception Handling
Use the `ServiceBase` class for standardized exception handling:

```csharp
public async Task<Person?> GetPersonAsync(string id, CancellationToken cancellationToken = default)
{
    ValidateNotNullOrEmpty(id, nameof(id));
    
    return await ExecuteGetAsync(
        () => _apiConnection.GetAsync<Person>($"/people/{id}"),
        "GetPerson",
        id,
        cancellationToken);
}
```

### Global Exception Handling
For operations outside of services, use the global exception handler:

```csharp
return await GlobalExceptionHandler.WrapAsync(
    _logger,
    async () => await operation(),
    "OperationName",
    resourceId);
```

## Best Practices

### 1. Consistent Parameter Naming
- Use `{OperationName}` for operation names
- Use `{ResourceId}` for resource identifiers
- Use `{CorrelationId}` for correlation IDs
- Use `{ElapsedMs}` for execution times

### 2. Structured Logging
Always use structured logging with named parameters:

```csharp
// ✅ Good
_logger.LogInformation("User {UserId} accessed resource {ResourceId}", userId, resourceId);

// ❌ Bad
_logger.LogInformation($"User {userId} accessed resource {resourceId}");
```

### 3. Context Scoping
Use logging scopes for related operations:

```csharp
using var scope = _logger.BeginScope(new Dictionary<string, object?>
{
    ["OperationName"] = operationName,
    ["CorrelationId"] = correlationId,
    ["ResourceId"] = resourceId
});
```

### 4. Performance Thresholds
Log performance warnings for operations that exceed thresholds:

```csharp
if (stopwatch.ElapsedMilliseconds > 2000)
{
    _logger.LogWarning("Operation {OperationName} took {ElapsedMs}ms, exceeding performance threshold [CorrelationId: {CorrelationId}]",
        operationName, stopwatch.ElapsedMilliseconds, correlationId);
}
```

### 5. Error Context
Include relevant context when logging errors:

```csharp
_logger.LogError(exception, "Failed to process {ResourceType} {ResourceId} for user {UserId} [CorrelationId: {CorrelationId}]",
    resourceType, resourceId, userId, correlationId);
```

## Configuration

### Logging Configuration
Configure logging in your application:

```csharp
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.AddApplicationInsights();
    builder.SetMinimumLevel(LogLevel.Information);
});
```

### Correlation ID Middleware
For ASP.NET Core applications, add correlation ID middleware:

```csharp
app.Use(async (context, next) =>
{
    var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault() 
                       ?? CorrelationContext.GenerateNew();
    
    using var scope = CorrelationScope.Create(correlationId);
    context.Response.Headers["X-Correlation-ID"] = correlationId;
    
    await next();
});
```

## Examples

### Service Method with Full Logging
```csharp
public async Task<Person?> GetPersonAsync(string id, CancellationToken cancellationToken = default)
{
    ValidateNotNullOrEmpty(id, nameof(id));
    
    return await ExecuteGetAsync(
        async () =>
        {
            using var scope = PerformanceMonitor.CreateScope(_logger, "GetPerson", id);
            try
            {
                var person = await _apiConnection.GetAsync<Person>($"/people/{id}", cancellationToken);
                scope.Success();
                return person;
            }
            catch (Exception ex)
            {
                scope.Failure(ex);
                throw;
            }
        },
        "GetPerson",
        id,
        cancellationToken);
}
```

### Complex Operation with Multiple Steps
```csharp
public async Task<Person> CreatePersonWithContactsAsync(PersonCreateRequest request, CancellationToken cancellationToken = default)
{
    ValidateNotNull(request, nameof(request));
    
    var correlationId = CorrelationContext.GetOrGenerate();
    
    using var scope = _logger.BeginScope(new Dictionary<string, object?>
    {
        ["OperationName"] = "CreatePersonWithContacts",
        ["CorrelationId"] = correlationId
    });
    
    return await ExecuteAsync(
        async () =>
        {
            _logger.LogInformation("Creating person with contacts [CorrelationId: {CorrelationId}]", correlationId);
            
            // Create person
            var person = await _apiConnection.PostAsync<Person>("/people", request, cancellationToken);
            _logger.LogDebug("Person created with ID: {PersonId} [CorrelationId: {CorrelationId}]", 
                person.Id, correlationId);
            
            // Add contacts
            if (request.Emails?.Any() == true)
            {
                await AddEmailsAsync(person.Id, request.Emails, cancellationToken);
            }
            
            if (request.Addresses?.Any() == true)
            {
                await AddAddressesAsync(person.Id, request.Addresses, cancellationToken);
            }
            
            _logger.LogInformation("Person with contacts created successfully: {PersonId} [CorrelationId: {CorrelationId}]", 
                person.Id, correlationId);
            
            return person;
        },
        "CreatePersonWithContacts",
        null,
        cancellationToken);
}
```

## Troubleshooting

### Common Issues

1. **Missing Correlation IDs**: Ensure correlation context is set at the beginning of operations
2. **Performance Degradation**: Use appropriate log levels and avoid logging sensitive data
3. **Log Noise**: Use structured logging and appropriate log levels to reduce noise

### Debugging

Use correlation IDs to trace requests across services:

```bash
# Filter logs by correlation ID
grep "CorrelationId: abc12345" application.log

# Search for specific operation
grep "OperationName: GetPerson" application.log
```

This logging standard ensures consistent, traceable, and performant logging across the Planning Center API Library.