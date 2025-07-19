# Project Structure and Separation of Concerns

## Overview

The Planning Center .NET SDK follows a structured, modular architecture that emphasizes separation of concerns, maintainability, and clean code principles. This document outlines the recommended project structure and organization patterns to ensure consistent implementation across all modules.

The SDK implements a dual API design, offering both a traditional service-based approach and a fluent API for more complex queries. Both APIs share the same underlying components while providing different interaction patterns for SDK consumers.

## Core Principles

### 1. **Separation of DTOs and Domain Models**
- DTOs (Data Transfer Objects) represent raw API responses and requests
- Domain models provide a clean, unified interface for SDK consumers
- Clear separation between internal implementation details and public API

### 2. **Dedicated Mapping Layer**
- Mapping logic isolated in dedicated mapper classes
- Consistent mapping patterns across all modules
- Centralized transformation rules

### 3. **Encapsulation of Implementation Details**
- Internal components not exposed to SDK consumers
- JSON:API specific models kept internal
- Public interfaces expose only domain models

### 4. **Dual API Design**
- Traditional service-based API for straightforward operations
- Fluent API for complex queries and operations
- Shared underlying components between both APIs

### 5. **Consistent File Organization**
- Logical grouping of related components
- Standardized naming conventions
- Clear directory structure

## Project Structure

```
PlanningCenter.Api.Client.Models/
  ├── Core/                      # Public domain models exposed to SDK consumers
  │   ├── Person.cs              # Domain model for Person
  │   ├── Address.cs             # Domain model for Address
  │   ├── Email.cs               # Domain model for Email
  │   └── ...                    # Other domain models
  │
  ├── People/                    # DTOs representing API responses
  │   ├── PersonDto.cs           # DTO for Person API response
  │   ├── AddressDto.cs          # DTO for Address API response
  │   └── ...                    # Other response DTOs
  │
  ├── Requests/                  # Public request models for SDK consumers
  │   ├── PersonCreateRequest.cs # Request model for creating a Person
  │   ├── PersonUpdateRequest.cs # Request model for updating a Person
  │   └── ...                    # Other request models
  │
  ├── Fluent/                    # Fluent API interfaces
  │   ├── IPeopleFluentContext.cs # Fluent interface for People module
  │   ├── IWorkflowFluentContext.cs # Fluent interface for Workflows
  │   └── ...                    # Other fluent interfaces
  │
  └── JsonApi/                   # Internal JSON:API DTOs
      ├── People/                # JSON:API DTOs for People module
      │   ├── PersonCreateDto.cs # JSON:API DTO for creating a Person
      │   ├── PersonUpdateDto.cs # JSON:API DTO for updating a Person
      │   └── ...                # Other JSON:API DTOs
      └── ...                    # Other module-specific JSON:API DTOs

PlanningCenter.Api.Client/
  ├── Mapping/                   # Dedicated mapper classes
  │   ├── People/                # Mappers for People module
  │   │   ├── PersonMapper.cs    # Mapper for Person entities
  │   │   ├── AddressMapper.cs   # Mapper for Address entities
  │   │   └── ...                # Other entity mappers
  │   └── ...                    # Other module-specific mappers
  │
  ├── Services/                  # Traditional service implementations
  │   ├── PeopleService.cs       # People module service implementation
  │   └── ...                    # Other service implementations
  │
  └── Fluent/                    # Fluent API implementations
      ├── People/                # Fluent implementations for People module
      │   ├── PeopleFluentContext.cs # Implementation of IPeopleFluentContext
      │   ├── WorkflowFluentContext.cs # Implementation of IWorkflowFluentContext
      │   └── ...                # Other fluent context implementations
      └── ...                    # Other module-specific fluent implementations
```

## Component Responsibilities

### 1. **Domain Models (Core)**
```csharp
namespace PlanningCenter.Api.Client.Models.Core
{
    /// <summary>
    /// Unified person model that combines data from all Planning Center modules.
    /// This model provides a consistent interface regardless of which module the person data came from.
    /// </summary>
    public class Person
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        // Additional properties...
    }
}
```

### 2. **Response DTOs**
```csharp
namespace PlanningCenter.Api.Client.Models.People
{
    /// <summary>
    /// Data Transfer Object for Person from the Planning Center People API.
    /// This represents the raw API response structure.
    /// </summary>
    public class PersonDto
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = "Person";
        public PersonAttributesDto Attributes { get; set; } = new();
        public PersonRelationshipsDto Relationships { get; set; } = new();
        // Additional properties...
    }
}
```

### 3. **Request Models**
```csharp
namespace PlanningCenter.Api.Client.Models.Requests
{
    /// <summary>
    /// Request model for creating a new person.
    /// </summary>
    public class PersonCreateRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        // Additional properties...
    }
}
```

### 4. **Fluent API Interfaces**
```csharp
namespace PlanningCenter.Api.Client.Models.Fluent
{
    /// <summary>
    /// Fluent interface for querying and manipulating people.
    /// </summary>
    public interface IPeopleFluentContext
    {
        // Person queries
        IPeopleFluentContext Where(Expression<Func<Core.Person, bool>> predicate);
        IPeopleFluentContext Include(Expression<Func<Core.Person, object>> include);
        IPeopleFluentContext OrderBy(Expression<Func<Core.Person, object>> orderBy);
        IPeopleFluentContext OrderByDescending(Expression<Func<Core.Person, object>> orderBy);
        
        // Execution
        Task<Core.Person> GetAsync(string id, CancellationToken cancellationToken = default);
        Task<IPagedResponse<Core.Person>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default);
        Task<List<Core.Person>> GetAllAsync(CancellationToken cancellationToken = default);
        
        // Additional fluent methods...
    }
}
```

### 5. **JSON:API DTOs**
```csharp
namespace PlanningCenter.Api.Client.Models.JsonApi.People
{
    /// <summary>
    /// Person creation DTO for JSON:API requests.
    /// </summary>
    internal class PersonCreateDto
    {
        public string Type { get; set; } = "Person";
        public PersonCreateAttributesDto Attributes { get; set; } = new();
    }
}
```

### 6. **Mapper Classes**
```csharp
namespace PlanningCenter.Api.Client.Mapping.People
{
    /// <summary>
    /// Mapper for Person entities between domain models, DTOs, and request models.
    /// </summary>
    internal static class PersonMapper
    {
        /// <summary>
        /// Maps a PersonDto to a Person domain model.
        /// </summary>
        public static Core.Person MapToDomain(Models.People.PersonDto dto)
        {
            return new Core.Person
            {
                Id = dto.Id,
                FirstName = dto.Attributes.FirstName ?? string.Empty,
                LastName = dto.Attributes.LastName ?? string.Empty,
                // Additional mappings...
            };
        }
        
        /// <summary>
        /// Maps a PersonCreateRequest to a PersonCreateDto for JSON:API.
        /// </summary>
        public static Models.JsonApi.People.PersonCreateDto MapToCreateDto(Models.Requests.PersonCreateRequest request)
        {
            return new Models.JsonApi.People.PersonCreateDto
            {
                Type = "Person",
                Attributes = new Models.JsonApi.People.PersonCreateAttributesDto
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    // Additional mappings...
                }
            };
        }
        
        // Additional mapping methods...
    }
}
```

### 7. **Traditional Service Implementation**
```csharp
namespace PlanningCenter.Api.Client.Services
{
    /// <summary>
    /// Service implementation for the Planning Center People module.
    /// </summary>
    public class PeopleService : IPeopleService
    {
        private readonly IApiConnection _apiConnection;
        private readonly ILogger<PeopleService> _logger;
        
        public PeopleService(IApiConnection apiConnection, ILogger<PeopleService> logger)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<Core.Person> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            // Implementation using mappers:
            var response = await _apiConnection.GetAsync<JsonApiSingleResponse<Models.People.PersonDto>>(
                $"/people/v2/people/{id}", cancellationToken);
                
            return Mapping.People.PersonMapper.MapToDomain(response.Data);
        }
        
        // Additional service methods...
    }
}
```

### 8. **Fluent API Implementation**
```csharp
namespace PlanningCenter.Api.Client.Fluent.People
{
    /// <summary>
    /// Implementation of the fluent interface for the People module.
    /// </summary>
    internal class PeopleFluentContext : IPeopleFluentContext
    {
        private readonly IApiConnection _apiConnection;
        private readonly ILogger<PeopleFluentContext> _logger;
        private readonly QueryBuilder _queryBuilder;
        
        public PeopleFluentContext(IApiConnection apiConnection, ILogger<PeopleFluentContext> logger)
        {
            _apiConnection = apiConnection ?? throw new ArgumentNullException(nameof(apiConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _queryBuilder = new QueryBuilder();
        }
        
        public IPeopleFluentContext Where(Expression<Func<Core.Person, bool>> predicate)
        {
            _queryBuilder.AddFilter(predicate);
            return this;
        }
        
        public async Task<Core.Person> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            // Implementation using the same mappers as the traditional service:
            var endpoint = $"/people/v2/people/{id}";
            var response = await _apiConnection.GetAsync<JsonApiSingleResponse<Models.People.PersonDto>>(
                endpoint, cancellationToken);
                
            return Mapping.People.PersonMapper.MapToDomain(response.Data);
        }
        
        // Additional fluent implementation methods...
    }
}
```

## Benefits of This Structure

### 1. **Maintainability**
- Clear separation of concerns makes code easier to maintain
- Changes to API contracts only affect DTOs and mappers
- Domain model changes are isolated from API implementation details
- Shared mapping logic between traditional and fluent APIs

### 2. **Testability**
- Mappers can be tested in isolation
- Services can be mocked for unit testing
- Clear boundaries enable focused testing strategies
- Fluent API can be tested independently from service implementations

### 3. **Extensibility**
- New API endpoints can be added without affecting domain models
- Additional mapping logic can be introduced without changing services
- New modules can follow the same consistent pattern
- Fluent API can be extended with new query capabilities

### 4. **Developer Experience**
- SDK consumers only work with clean domain models
- Implementation details are hidden from consumers
- Consistent patterns across modules reduce learning curve
- Choice between traditional and fluent APIs based on use case

## Implementation Guidelines

### 1. **Naming Conventions**
- **Domain Models**: Simple noun (e.g., `Person`, `Address`)
- **DTOs**: Noun + "Dto" suffix (e.g., `PersonDto`, `AddressDto`)
- **Request Models**: Noun + Action + "Request" (e.g., `PersonCreateRequest`, `AddressUpdateRequest`)
- **JSON:API DTOs**: Noun + Action + "Dto" (e.g., `PersonCreateDto`, `AddressUpdateDto`)
- **Mappers**: Noun + "Mapper" (e.g., `PersonMapper`, `AddressMapper`)
- **Fluent Interfaces**: "I" + Noun + "FluentContext" (e.g., `IPeopleFluentContext`, `IWorkflowFluentContext`)
- **Fluent Implementations**: Noun + "FluentContext" (e.g., `PeopleFluentContext`, `WorkflowFluentContext`)

### 2. **Visibility Guidelines**
- Domain models should be `public`
- Response DTOs should be `public` (for advanced scenarios)
- Request models should be `public`
- Fluent interfaces should be `public`
- JSON:API DTOs should be `internal`
- Mapper classes should be `internal`
- Fluent implementations should be `internal`

### 3. **Documentation Requirements**
- All public types must have XML documentation
- All public methods must have parameter and return documentation
- Internal components should have documentation for maintainers

### 4. **Testing Strategy**
- Unit tests for mappers (input/output validation)
- Unit tests for services (using mocked dependencies)
- Integration tests for end-to-end validation

### 5. **Dual API Implementation**
- Both APIs should use the same mapper classes
- Both APIs should return the same domain models
- Fluent API should translate to the same underlying API calls
- Query building should be isolated from execution logic
- Both APIs should provide consistent error handling and pagination

By following these guidelines, the Planning Center .NET SDK will maintain a clean, consistent structure that is easy to maintain, extend, and use, while providing flexibility through both traditional and fluent API approaches.
