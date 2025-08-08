# Technology Stack

## .NET Framework
- **Target Framework**: .NET 9.0
- **Language**: C# with modern language features
- **Nullable Reference Types**: Enabled throughout the codebase
- **Implicit Usings**: Enabled for cleaner code

## Key Dependencies
### Core Libraries
- **Microsoft.Extensions.DependencyInjection.Abstractions** (9.0.7): Dependency injection support
- **Microsoft.Extensions.Http** (9.0.7): HTTP client factory and configuration
- **Microsoft.Extensions.Logging.Abstractions** (9.0.7): Structured logging support
- **Microsoft.Extensions.Options** (9.0.7): Options pattern configuration
- **Microsoft.Extensions.Caching.Abstractions** (9.0.7): Caching abstraction layer
- **Microsoft.Extensions.Caching.Memory** (9.0.7): In-memory caching implementation
- **System.Text.Json** (9.0.7): JSON serialization/deserialization

### Testing Frameworks
- **xUnit**: Primary testing framework
- **Moq**: Mocking framework for unit tests
- **FluentAssertions**: Readable test assertions
- **AutoFixture**: Test data generation

## Architecture Patterns
- **Service Layer Pattern**: All services inherit from ServiceBase
- **Repository Pattern**: ApiConnection provides data access abstraction
- **Options Pattern**: Configuration through PlanningCenterOptions
- **Dependency Injection**: Full DI container support
- **Factory Pattern**: HTTP client factory for connection management
- **Fluent Interface**: LINQ-like chainable syntax for complex queries

## Platform Support
- **Primary Platform**: Windows (current development environment)
- **Cross-Platform**: Supports all platforms where .NET 9.0 runs (Windows, Linux, macOS)
- **Runtime**: .NET 9.0 runtime required