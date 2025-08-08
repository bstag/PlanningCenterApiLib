# Code Style and Conventions

## C# Coding Standards

### General Principles
- **Framework**: .NET 9.0 with nullable reference types enabled
- **Async/Await**: All I/O operations are async
- **Cancellation**: All async methods support `CancellationToken`
- **Logging**: Structured logging with `ILogger<T>`
- **Error Handling**: Custom exceptions inherit from `PlanningCenterApiException`

### Naming Conventions
- **Classes**: PascalCase (e.g., `ServiceCollectionExtensions`, `PeopleService`)
- **Methods**: PascalCase (e.g., `AddPlanningCenterApiClient`, `GetAsync`)
- **Properties**: PascalCase (e.g., `PersonalAccessToken`, `BaseUrl`)
- **Fields**: camelCase with underscore prefix for private fields (e.g., `_logger`, `_httpClient`)
- **Parameters**: camelCase (e.g., `services`, `configureOptions`)
- **Local Variables**: camelCase (e.g., `options`, `logger`)

### Interface Conventions
- **Service Interfaces**: `I{Module}Service` pattern (e.g., `IPeopleService`, `IGivingService`)
- **Fluent Interfaces**: `I{Module}FluentContext` pattern for chainable operations
- **Abstract Interfaces**: `IApiConnection`, `IAuthenticator`, `ICacheProvider`

### Method Conventions
- **Async Methods**: Always end with `Async` suffix
- **CRUD Operations**: `GetAsync`, `ListAsync`, `CreateAsync`, `UpdateAsync`, `DeleteAsync`
- **Bulk Operations**: `GetAllAsync`, `StreamAsync`
- **Extension Methods**: Used for service registration and fluent APIs

### Documentation Standards
- **XML Documentation**: Comprehensive XML docs for all public APIs
- **Parameter Validation**: Arguments validated with descriptive error messages
- **Exception Documentation**: All exceptions documented with `<exception>` tags

### Dependency Injection Patterns
- **Service Registration**: Extension methods in `ServiceCollectionExtensions`
- **Scoped Services**: Services registered as `AddScoped` for per-request lifecycle
- **Singleton Services**: Authenticators and cache providers as singletons
- **Options Pattern**: Configuration through `PlanningCenterOptions`

### Error Handling Conventions
- **Custom Exceptions**: Inherit from `PlanningCenterApiException`
- **Validation**: Argument validation with `ArgumentNullException` and `ArgumentException`
- **Correlation IDs**: All operations include correlation tracking
- **Structured Logging**: Use structured logging with context

### File Organization
- **Project Structure**: Clear separation by concern (Models, Services, Abstractions)
- **Namespace Alignment**: Namespaces align with folder structure
- **Service Implementation**: Each module follows consistent service pattern
- **Mapping Strategy**: Dedicated mapping classes between model types

### Testing Conventions
- **Unit Tests**: Mock `IApiConnection` and test service logic
- **Integration Tests**: Test against real API endpoints (requires configuration)
- **Test Data**: Use `TestDataBuilder` and `AutoFixture` for test data generation
- **Assertions**: Use `FluentAssertions` for readable test assertions