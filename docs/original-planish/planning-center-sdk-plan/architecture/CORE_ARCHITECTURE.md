# Core Architecture for Planning Center SDK

## Overview

The Planning Center .NET SDK is built on a robust, scalable architecture that provides comprehensive API coverage, excellent developer experience, and production-ready reliability. The architecture emphasizes consistency, performance, and ease of use while supporting both simple and complex integration scenarios.

## Architectural Principles

### 1. **Unified Data Models**
- Single source of truth for entity definitions across all modules
- Consistent property names and types regardless of source module
- Rich object relationships and navigation properties
- Automatic mapping between module-specific DTOs and unified models

### 2. **Dual API Design**
- **Service-Based API**: Traditional dependency injection pattern for straightforward operations
- **Fluent API**: Chainable, expressive syntax for complex queries and operations
- Both APIs share the same underlying infrastructure and capabilities

### 3. **Built-in Pagination Support**
- Comprehensive pagination helpers that eliminate manual pagination logic
- Multiple pagination patterns: page-based, streaming, and automatic fetching
- Memory-efficient processing of large datasets
- Rich pagination metadata and navigation capabilities

### 4. **Resilience and Reliability**
- Automatic retry logic with exponential backoff
- Circuit breaker pattern for failing services
- Rate limiting and throttling support
- Comprehensive error handling and classification

### 5. **Performance Optimization**
- Multi-level caching strategies (in-memory and distributed)
- Connection pooling and HTTP client reuse
- Lazy loading and efficient data fetching
- Bulk operations for high-throughput scenarios

## Core Components

### 1. **HTTP Communication Layer**
```csharp
namespace PlanningCenter.Api.Client.Http
{
    public interface IApiConnection
    {
        // Single item operations
        Task<T> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default);
        Task<T> PostAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default);
        Task<T> PutAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default);
        Task<T> PatchAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default);
        Task DeleteAsync(string endpoint, CancellationToken cancellationToken = default);
        
        // Paginated operations
        Task<IPagedResponse<T>> GetPagedAsync<T>(
            string endpoint, 
            QueryParameters parameters = null, 
            CancellationToken cancellationToken = default);
        
        // Bulk operations
        Task<BulkResult<T>> BulkCreateAsync<T>(string endpoint, IEnumerable<object> items, CancellationToken cancellationToken = default);
        Task<BulkResult<T>> BulkUpdateAsync<T>(string endpoint, IEnumerable<object> items, CancellationToken cancellationToken = default);
        Task<BulkResult<string>> BulkDeleteAsync(string endpoint, IEnumerable<string> ids, CancellationToken cancellationToken = default);
    }
}
```

### 2. **Pagination Infrastructure**
```csharp
namespace PlanningCenter.Api.Client.Models.Responses
{
    public interface IPagedResponse<T>
    {
        IReadOnlyList<T> Data { get; }
        PagedResponseMeta Meta { get; }
        PagedResponseLinks Links { get; }
        bool HasNextPage { get; }
        bool HasPreviousPage { get; }
        
        // Built-in navigation helpers
        Task<IPagedResponse<T>> GetNextPageAsync(CancellationToken cancellationToken = default);
        Task<IPagedResponse<T>> GetPreviousPageAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> GetAllRemainingAsync(CancellationToken cancellationToken = default);
        IAsyncEnumerable<T> GetAllRemainingAsyncEnumerable(CancellationToken cancellationToken = default);
    }
}
```

### 3. **Service Layer Architecture**
```csharp
namespace PlanningCenter.Api.Client.Services
{
    public interface IModuleService<TEntity, TCreateRequest, TUpdateRequest>
        where TEntity : class
        where TCreateRequest : class
        where TUpdateRequest : class
    {
        // CRUD operations
        Task<TEntity> GetAsync(string id, CancellationToken cancellationToken = default);
        Task<IPagedResponse<TEntity>> ListAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
        Task<TEntity> CreateAsync(TCreateRequest request, CancellationToken cancellationToken = default);
        Task<TEntity> UpdateAsync(string id, TUpdateRequest request, CancellationToken cancellationToken = default);
        Task DeleteAsync(string id, CancellationToken cancellationToken = default);
        
        // Pagination helpers
        Task<IReadOnlyList<TEntity>> GetAllAsync(
            QueryParameters parameters = null,
            PaginationOptions options = null,
            CancellationToken cancellationToken = default);
        
        IAsyncEnumerable<TEntity> StreamAsync(
            QueryParameters parameters = null,
            PaginationOptions options = null,
            CancellationToken cancellationToken = default);
        
        // Bulk operations
        Task<BulkResult<TEntity>> BulkCreateAsync(IEnumerable<TCreateRequest> requests, CancellationToken cancellationToken = default);
        Task<BulkResult<TEntity>> BulkUpdateAsync(IEnumerable<TUpdateRequest> requests, CancellationToken cancellationToken = default);
        Task<BulkResult<string>> BulkDeleteAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
    }
}
```

### 4. **Fluent API Architecture**
```csharp
namespace PlanningCenter.Api.Client.Fluent
{
    public interface IFluentContext<T>
    {
        // Query building
        IFluentContext<T> Where(Expression<Func<T, bool>> predicate);
        IFluentContext<T> Include(Expression<Func<T, object>> include);
        IFluentContext<T> OrderBy(Expression<Func<T, object>> orderBy);
        IFluentContext<T> OrderByDescending(Expression<Func<T, object>> orderBy);
        
        // Pagination execution
        Task<IPagedResponse<T>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default);
        Task<IPagedResponse<T>> GetPageAsync(int page, int pageSize = 25, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> GetAllAsync(PaginationOptions options = null, CancellationToken cancellationToken = default);
        IAsyncEnumerable<T> AsAsyncEnumerable(PaginationOptions options = null, CancellationToken cancellationToken = default);
        
        // Single item operations
        Task<T> FirstAsync(CancellationToken cancellationToken = default);
        Task<T> FirstOrDefaultAsync(CancellationToken cancellationToken = default);
        Task<T> SingleAsync(CancellationToken cancellationToken = default);
        Task<T> SingleOrDefaultAsync(CancellationToken cancellationToken = default);
        
        // Aggregation
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(CancellationToken cancellationToken = default);
    }
}
```

### 5. **Authentication and Security**
```csharp
namespace PlanningCenter.Api.Client.Auth
{
    public interface IAuthenticator
    {
        Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
        Task RefreshTokenAsync(CancellationToken cancellationToken = default);
        Task<bool> IsTokenValidAsync(CancellationToken cancellationToken = default);
        event EventHandler<TokenRefreshedEventArgs> TokenRefreshed;
    }
    
    public class OAuthAuthenticator : IAuthenticator
    {
        // OAuth 2.0 implementation with automatic token refresh
        // Secure token storage and management
        // Support for multiple authentication flows
    }
}
```

### 6. **Caching Infrastructure**
```csharp
namespace PlanningCenter.Api.Client.Caching
{
    public interface ICacheProvider
    {
        Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default);
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default);
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
        Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);
        
        // Pagination-specific caching
        Task<IPagedResponse<T>> GetPagedAsync<T>(string key, CancellationToken cancellationToken = default);
        Task SetPagedAsync<T>(string key, IPagedResponse<T> value, TimeSpan? expiry = null, CancellationToken cancellationToken = default);
    }
}
```

## Data Flow Architecture

### 1. **Request Flow**
```
Client Request → Fluent API / Service API → Query Builder → HTTP Client → Planning Center API
                                                ↓
                                         Authentication Layer
                                                ↓
                                            Rate Limiting
                                                ↓
                                         Resilience Policies
                                                ↓
                                            HTTP Request
```

### 2. **Response Flow**
```
Planning Center API → HTTP Response → Error Handling → Response Mapping → Caching → Client
                                           ↓
                                    Exception Factory
                                           ↓
                                    Logging & Monitoring
                                           ↓
                                    Pagination Processing
                                           ↓
                                    Unified Model Mapping
```

### 3. **Pagination Flow**
```
Initial Request → First Page Response → IPagedResponse<T>
                                            ↓
                                    Pagination Helpers
                                            ↓
                    ┌─────────────────────────────────────────┐
                    ↓                                         ↓
            GetNextPageAsync()                        GetAllPagesAsync()
                    ↓                                         ↓
            Next Page Request                     Automatic Page Fetching
                    ↓                                         ↓
            Single Page Response                  Complete Dataset
```

## Module Integration

### 1. **Module Service Registration**
```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPlanningCenterApiClient(
        this IServiceCollection services,
        Action<PlanningCenterOptions> configureOptions)
    {
        services.Configure(configureOptions);
        
        // Core infrastructure
        services.AddSingleton<IApiConnection, ApiConnection>();
        services.AddSingleton<IAuthenticator, OAuthAuthenticator>();
        services.AddSingleton<ICacheProvider, InMemoryCacheProvider>();
        
        // Module services
        services.AddScoped<IPeopleService, PeopleService>();
        services.AddScoped<IGivingService, GivingService>();
        services.AddScoped<ICalendarService, CalendarService>();
        // ... other modules
        
        // Fluent API
        services.AddScoped<IPlanningCenterClient, PlanningCenterClient>();
        
        // Pagination helpers
        services.AddSingleton<PaginationHelper>();
        
        return services;
    }
}
```

### 2. **Unified Client Interface**
```csharp
namespace PlanningCenter.Api.Client
{
    public interface IPlanningCenterClient
    {
        // Module access
        IPeopleFluentContext People();
        IGivingFluentContext Giving();
        ICalendarFluentContext Calendar();
        ICheckInsFluentContext CheckIns();
        IGroupsFluentContext Groups();
        IRegistrationsFluentContext Registrations();
        IPublishingFluentContext Publishing();
        IServicesFluentContext Services();
        IWebhooksFluentContext Webhooks();
        
        // Global operations
        Task<HealthCheckResult> CheckHealthAsync(CancellationToken cancellationToken = default);
        Task<ApiLimitsInfo> GetApiLimitsAsync(CancellationToken cancellationToken = default);
        
        // Bulk cross-module operations
        Task<BulkResult<Core.Person>> BulkSyncPeopleAsync(
            BulkSyncRequest request,
            IProgress<BulkSyncProgress> progress = null,
            CancellationToken cancellationToken = default);
    }
}
```

## Performance Characteristics

### 1. **Pagination Performance**
- **Memory Efficiency**: Streaming operations use constant memory regardless of dataset size
- **Network Optimization**: Intelligent page size calculation based on data characteristics
- **Caching**: Automatic caching of frequently accessed pages
- **Prefetching**: Optional prefetching of next pages for improved perceived performance

### 2. **Scalability Features**
- **Connection Pooling**: Efficient HTTP connection reuse
- **Rate Limiting**: Built-in respect for API rate limits
- **Bulk Operations**: Optimized batch processing for high-throughput scenarios
- **Async/Await**: Full asynchronous operation support throughout

### 3. **Monitoring and Observability**
- **Structured Logging**: Comprehensive logging with correlation IDs
- **Performance Metrics**: Built-in performance counters and timing
- **Health Checks**: Endpoint health monitoring
- **Error Tracking**: Detailed error classification and reporting

## Security Considerations

### 1. **Authentication Security**
- Secure token storage with encryption at rest
- Automatic token refresh with secure retry logic
- Support for token rotation and revocation
- Audit logging of authentication events

### 2. **Data Protection**
- No sensitive data in logs or error messages
- Secure HTTP communication (TLS 1.2+)
- Input validation and sanitization
- Protection against injection attacks

### 3. **API Security**
- Request signing for webhook verification
- Rate limiting compliance
- Secure credential management
- OWASP security best practices

This architecture provides a solid foundation for building robust, scalable applications that integrate with the Planning Center API while offering excellent developer experience and production-ready reliability.