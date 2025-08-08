# Development Guidelines and Best Practices

## Code Development Guidelines

### Framework Requirements
- **Target Framework**: .NET 9.0 with nullable reference types enabled
- **Modern C# Features**: Use latest C# language features appropriately
- **Async Patterns**: All I/O operations must be async with `CancellationToken` support
- **Dependency Injection**: All services must support DI container registration

### Service Implementation Requirements
- **ServiceBase Inheritance**: All services must inherit from ServiceBase for consistency
- **Interface Implementation**: Every service must implement its corresponding interface
- **Correlation ID Support**: All operations must support correlation ID tracking
- **Error Handling**: Use typed exceptions with proper context and correlation tracking
- **Performance Monitoring**: Leverage built-in performance monitoring capabilities

### Testing Requirements
- **Unit Test Coverage**: Aim for >80% code coverage on all new code
- **Integration Tests**: Add integration tests for API endpoint changes
- **Mock Usage**: Mock `IApiConnection` for unit tests, test service logic
- **Test Data**: Use `TestDataBuilder` and `AutoFixture` for consistent test data
- **Assertions**: Use `FluentAssertions` for readable, maintainable test assertions

### Documentation Standards
- **XML Documentation**: All public APIs must have comprehensive XML documentation
- **Parameter Validation**: Document all parameters with `<param>` tags
- **Exception Documentation**: Document all exceptions with `<exception>` tags
- **Usage Examples**: Include usage examples in complex API documentation
- **README Updates**: Update relevant README files when adding major features

### Configuration Guidelines
- **Options Pattern**: Use `PlanningCenterOptions` for all configuration
- **Environment Support**: Support multiple environments (dev, staging, prod)
- **Secure Defaults**: Default to secure configurations
- **Validation**: Validate configuration values with descriptive error messages

### Authentication Guidelines
- **Multiple Methods**: Support PAT, OAuth 2.0, and Access Token authentication
- **Priority Order**: PAT > Access Token > OAuth for automatic selection
- **Token Security**: Never log or expose authentication tokens
- **Refresh Logic**: Implement proper token refresh for OAuth flows

### Fluent API Guidelines
- **LINQ-like Syntax**: Follow LINQ patterns for query building
- **Method Chaining**: Support fluent method chaining throughout
- **Type Safety**: Maintain strong typing through fluent chains
- **Expression Trees**: Properly handle expression parsing for server-side filtering

### Error Handling Guidelines
- **Custom Exceptions**: Create specific exception types for different scenarios
- **Context Preservation**: Include full context (correlation ID, request details)
- **Retry Logic**: Implement intelligent retry with exponential backoff
- **Circuit Breaker**: Use circuit breaker pattern for API resilience
- **Structured Logging**: Log errors with structured data for observability

### Performance Guidelines
- **Memory Efficiency**: Use streaming APIs for large datasets
- **Caching Strategy**: Implement intelligent caching with proper invalidation
- **Connection Pooling**: Leverage HTTP client factory for connection management
- **Async All the Way**: Maintain async patterns throughout the call stack
- **Resource Disposal**: Properly dispose of resources using `using` statements

### Security Guidelines
- **Input Validation**: Validate all inputs with proper error messages
- **Output Sanitization**: Sanitize outputs to prevent injection attacks
- **Token Storage**: Store tokens securely, never in plain text
- **HTTPS Only**: Always use HTTPS for API communications
- **Rate Limiting**: Respect and handle API rate limits properly

## Module Development Checklist

### New Module Addition
1. ✅ Create service interface in Models project
2. ✅ Implement service in Client project inheriting from ServiceBase
3. ✅ Add domain models in `Models/Core/`
4. ✅ Create DTOs in `Models/{Module}/`
5. ✅ Implement mappers in `Mapping/{Module}/`
6. ✅ Add fluent interface support
7. ✅ Register service in `ServiceCollectionExtensions`
8. ✅ Add comprehensive unit tests
9. ✅ Add integration tests
10. ✅ Update documentation

### Code Review Guidelines
- **Architecture Compliance**: Verify adherence to ServiceBase pattern
- **Error Handling**: Check proper exception handling and correlation tracking
- **Test Coverage**: Ensure adequate test coverage for new functionality
- **Documentation**: Verify XML documentation completeness
- **Performance**: Review for potential performance issues
- **Security**: Check for security vulnerabilities or token exposure