# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Common Development Commands

### Build and Test
```bash
# Build the entire solution
dotnet build src/PlanningCenter.Api.sln

# Run all unit tests
dotnet test src/PlanningCenter.Api.Client.Tests/PlanningCenter.Api.Client.Tests.csproj

# Run integration tests (requires configuration)
dotnet test src/PlanningCenter.Api.Client.IntegrationTests/PlanningCenter.Api.Client.IntegrationTests.csproj

# Run specific test project
dotnet test src/PlanningCenter.Api.Client.Tests/

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Development Workflow
```bash
# Restore dependencies
dotnet restore src/PlanningCenter.Api.sln

# Build specific project
dotnet build src/PlanningCenter.Api.Client/PlanningCenter.Api.Client.csproj

# Run console examples
dotnet run --project examples/PlanningCenter.Api.Client.Console/
dotnet run --project examples/PlanningCenter.Api.Client.Fluent.Console/

# Run background worker example
dotnet run --project examples/PlanningCenter.Api.Client.Worker/
```

## Code Architecture

### Project Structure
The solution follows a modular architecture with clear separation of concerns:

- **PlanningCenter.Api.Client**: Core SDK implementation with services and authentication
- **PlanningCenter.Api.Client.Models**: Domain models, DTOs, and request/response models
- **PlanningCenter.Api.Client.Abstractions**: Interfaces and contracts
- **PlanningCenter.Api.Client.Tests**: Unit tests using xUnit, Moq, and FluentAssertions
- **PlanningCenter.Api.Client.IntegrationTests**: Integration tests for API endpoints

### Key Design Patterns

#### Dual API Design
The SDK provides two complementary APIs:
- **Traditional Service API**: Direct service injection pattern (`IPeopleService`, `ICalendarService`, etc.)
- **Fluent API**: LINQ-like chainable syntax for complex queries and operations

#### Mapping Strategy
- **Domain Models** (`Core/`): Clean, unified models exposed to consumers
- **DTOs** (`People/`, `Calendar/`, etc.): Raw API response structures
- **Request Models** (`Requests/`): Strongly-typed request objects
- **JSON:API DTOs** (`JsonApi/`): Internal serialization models (marked `internal`)
- **Mappers** (`Mapping/`): Dedicated mapping classes between all model types

#### Authentication Support
Three authentication methods with automatic selection:
1. **Personal Access Token (PAT)**: Recommended for server applications
2. **OAuth 2.0**: For user-facing applications
3. **Access Token**: Direct token usage

#### Pagination Architecture
- **IPagedResponse<T>**: Rich pagination metadata and navigation
- **Streaming Support**: Memory-efficient processing via `IAsyncEnumerable<T>`
- **Automatic Pagination**: `GetAllAsync()` methods handle pagination internally

### Service Registration
The SDK uses dependency injection with extension methods:
```csharp
// PAT authentication (recommended)
services.AddPlanningCenterApiClientWithPAT("app-id:secret");

// OAuth authentication
services.AddPlanningCenterApiClient(options => {
    options.ClientId = "client-id";
    options.ClientSecret = "client-secret";
});
```

## Development Guidelines

### Code Style
- **Framework**: .NET 9.0 with nullable reference types enabled
- **Async/Await**: All I/O operations are async
- **Cancellation**: All async methods support `CancellationToken`
- **Logging**: Structured logging with `ILogger<T>`
- **Error Handling**: Custom exceptions inherit from `PlanningCenterApiException`

### Testing Strategy
- **Unit Tests**: Mock `IApiConnection` and test service logic
- **Integration Tests**: Test against real API endpoints (requires configuration)
- **Test Data**: Use `TestDataBuilder` and `AutoFixture` for test data generation
- **Assertions**: Use `FluentAssertions` for readable test assertions

### Module Implementation Pattern
Each Planning Center module follows consistent patterns:

1. **Service Interface**: `I{Module}Service` in Models project
2. **Service Implementation**: `{Module}Service` in Client project
3. **Domain Models**: Clean models in `Models/Core/`
4. **DTOs**: Response models in `Models/{Module}/`
5. **Mappers**: Conversion logic in `Mapping/{Module}/`
6. **Fluent Interface**: `I{Module}FluentContext` for chainable operations

### Error Handling
- **Typed Exceptions**: Specific exception types for different error scenarios
- **Status Code Mapping**: HTTP status codes map to appropriate exception types
- **Retry Logic**: Built-in exponential backoff for transient failures
- **Rate Limiting**: Automatic handling of API rate limits

## Current Implementation Status

### Completed Modules
- **People**: Full CRUD operations, contact management, workflows
- **Calendar**: Event and resource management
- **CheckIns**: Event check-in functionality
- **Groups**: Group management and memberships
- **Services**: Service planning and scheduling (partial)

### Authentication
- ✅ Personal Access Token (PAT) authentication
- ✅ OAuth 2.0 flow with automatic token refresh
- ✅ Access token direct usage

### Features
- ✅ Comprehensive pagination support
- ✅ Memory-efficient streaming
- ✅ Automatic retry with exponential backoff
- ✅ Built-in caching with configurable expiration
- ✅ Fluent API for complex queries
- ✅ Bulk operations support
- ✅ Comprehensive error handling

## Configuration

### Options Configuration
The SDK uses `PlanningCenterOptions` for configuration:
- **BaseUrl**: API endpoint (defaults to official API)
- **Authentication**: PAT, OAuth, or access token
- **Retry Settings**: Max attempts, base delay, timeout
- **Caching**: Enable/disable and expiration settings
- **Logging**: Detailed logging for debugging (avoid in production)

### Environment Configuration
Integration tests require configuration in `appsettings.local.json`:
```json
{
  "PlanningCenter": {
    "PersonalAccessToken": "your-pat-token",
    "BaseUrl": "https://api.planningcenteronline.com"
  }
}
```

## Important Notes

- The SDK targets .NET 9.0 and uses modern C# features
- All services are designed to be thread-safe
- Pagination is built-in and handles large datasets efficiently
- The fluent API provides LINQ-like syntax but doesn't fully parse expressions to server-side filters yet
- Bulk operations are available for high-throughput scenarios
- The architecture supports both traditional DI patterns and fluent query building

## Known Build Issues

⚠️ **Current Build Status**: The solution has compilation errors that need to be addressed:

### ✅ **Fixed Issues**:
1. **Duplicate Request Classes**: Resolved duplicate class definitions in Create request files
2. **Duplicate Method Definitions**: Fixed duplicate `MapToDomain` methods in mapper classes
3. **Type Ambiguity**: Resolved `Campus` type conflicts using fully qualified names
4. **Missing Base Properties**: Added `DataSource`, `CreatedAt`, `UpdatedAt` to `PlanningCenterResource`
5. **PaginationOptions**: Fixed `MaxPages` references to use `MaxItems` instead
6. **Generic Method Calls**: Fixed numerous PostAsync/PatchAsync calls with correct type arguments

### 🔧 **Remaining Issues**:
1. **Generic Method Calls**: ~40 remaining PostAsync/PatchAsync calls missing type arguments
2. **Service Dependencies**: Fluent client service injection issues
3. **Type Inference**: Select method type inference issues with dynamic objects
4. **Missing DTOs**: Some mapper references to non-existent DTOs (RefundCreateDto, PersonDto)
5. **Method Signatures**: QueryParameters.AddFilter method signature issues

### 📋 **Build Status by Project**:
- ✅ **PlanningCenter.Api.Client.Models**: Builds successfully
- ✅ **PlanningCenter.Api.Client.Worker**: Builds successfully  
- ⚠️ **PlanningCenter.Api.Client**: 86 compilation errors (reduced from 133)
- ❌ **PlanningCenter.Api.Client.Tests**: Cannot build due to dependencies
- ❌ **PlanningCenter.Api.Client.IntegrationTests**: Cannot build due to dependencies

### 🚧 **Development Status**:
This appears to be a work-in-progress SDK with:
- Core models and interfaces implemented
- Service implementations partially complete
- Some features implemented as placeholders
- Fluent API framework in place but incomplete

### Quick Fix Commands
```bash
# Build only the models project (works)
dotnet build src/PlanningCenter.Api.Client.Models/

# Clean and rebuild attempt
dotnet clean src/PlanningCenter.Api.sln
dotnet build src/PlanningCenter.Api.sln
```