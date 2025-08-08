# Architecture Patterns and Design Decisions

## Core Design Patterns

### ServiceBase Architecture
All services inherit from a unified ServiceBase pattern providing:
- **Correlation ID Management**: Every request gets unique correlation ID for end-to-end tracking
- **Performance Monitoring**: Built-in timing and performance metrics for all operations
- **Unified Exception Handling**: Consistent error handling with detailed context
- **Automatic Retry Logic**: Configurable retry policies with exponential backoff
- **Request/Response Logging**: Structured logging with correlation IDs
- **Rate Limit Handling**: Automatic rate limit detection and intelligent backoff
- **Circuit Breaker Pattern**: Prevents cascading failures during API outages
- **Request Deduplication**: Prevents duplicate requests within configurable time windows

### Dual API Design
The SDK provides two complementary APIs:
- **Traditional Service API**: Direct service injection pattern (`IPeopleService`, `ICalendarService`, etc.)
- **Fluent API**: LINQ-like chainable syntax for complex queries and operations

### Authentication Architecture
Three authentication methods with automatic selection:
1. **Personal Access Token (PAT)**: Recommended for server applications
2. **OAuth 2.0**: For user-facing applications requiring user consent
3. **Access Token**: Direct token usage

Priority order: PersonalAccessToken > AccessToken (treated as PAT) > OAuth

### Mapping Strategy
- **Domain Models** (`Core/`): Clean, unified models exposed to consumers
- **DTOs** (`People/`, `Calendar/`, etc.): Raw API response structures
- **Request Models** (`Requests/`): Strongly-typed request objects
- **JSON:API DTOs** (`JsonApi/`): Internal serialization models (marked `internal`)
- **Mappers** (`Mapping/`): Dedicated mapping classes between all model types

### Pagination Architecture
- **IPagedResponse<T>**: Rich pagination metadata and navigation
- **Streaming Support**: Memory-efficient processing via `IAsyncEnumerable<T>`
- **Automatic Pagination**: `GetAllAsync()` methods handle pagination internally

## Module Implementation Pattern
Each Planning Center module follows consistent patterns:

1. **Service Interface**: `I{Module}Service` in Models project
2. **Service Implementation**: `{Module}Service` in Client project
3. **Domain Models**: Clean models in `Models/Core/`
4. **DTOs**: Response models in `Models/{Module}/`
5. **Mappers**: Conversion logic in `Mapping/{Module}/`
6. **Fluent Interface**: `I{Module}FluentContext` for chainable operations

## Dependency Injection Strategy
- **Extension Methods**: Service registration through `ServiceCollectionExtensions`
- **Scoped Services**: All API services registered as scoped for per-request lifecycle
- **Singleton Services**: Authenticators, cache providers, and HTTP clients as singletons
- **Factory Pattern**: HTTP client factory for connection management
- **Options Pattern**: Configuration through `PlanningCenterOptions`

## Error Handling Strategy
- **Typed Exceptions**: Specific exception types for different error scenarios
- **Status Code Mapping**: HTTP status codes map to appropriate exception types
- **Retry Logic**: Built-in exponential backoff for transient failures
- **Rate Limiting**: Automatic handling of API rate limits
- **Correlation Tracking**: Every error includes correlation ID for debugging

## Performance Architecture
- **Caching Layer**: Configurable response caching with `ICacheProvider`
- **Streaming APIs**: Memory-efficient processing of large datasets
- **Connection Pooling**: HTTP client factory manages connection pools
- **Async Patterns**: All I/O operations use async/await patterns
- **Performance Monitoring**: Built-in timing and metrics collection

## Observability & Monitoring
- **Structured Logging**: Comprehensive logging with correlation IDs
- **Performance Metrics**: Request duration, success/failure rates
- **Health Monitoring**: Circuit breaker patterns for API health
- **Request Tracking**: End-to-end request correlation
- **Cache Monitoring**: Cache hit/miss ratios and performance