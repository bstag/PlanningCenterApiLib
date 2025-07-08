# Planning Center SDK - Development Phases

Date: 2023-10-27

## 1. Introduction

This document outlines a phased approach for the development of the Planning Center .NET SDK. It builds upon the `PlanningCenter_SDK_Analysis.md` and `SDK_Implementation_Strategy.md`. The goal is to provide a structured roadmap for the implementation, allowing for iterative development, testing, and refinement.

Each phase will include development, unit testing, and initial integration testing where applicable. Documentation (XML comments) will be written concurrently with code development.

## 2. Development Phases

### Phase 0: Initial Project Setup

*   **Objective:** Create the solution and all project structures as defined in the `SDK_Implementation_Strategy.md`.
*   **Steps:**
    1.  Create `PlanningCenter.Api.Client.sln`.
    2.  Create `src` projects:
        *   `PlanningCenter.Api.Client.Models`
        *   `PlanningCenter.Api.Client.Abstractions`
        *   `PlanningCenter.Api.Client`
    3.  Create `tests` projects:
        *   `PlanningCenter.Api.Client.Tests`
        *   `PlanningCenter.Api.Client.IntegrationTests`
    4.  Create `examples` projects:
        *   `PlanningCenter.Api.Client.Examples.Console`
        *   `PlanningCenter.Api.Client.Examples.Worker`
    5.  Configure all projects to target .NET 9.
    6.  Set up basic solution-level files (e.g., `.gitignore`, `Directory.Build.props` if needed for common settings).
    7.  Create initial `README.md` for the solution.

### Phase 1: Core Infrastructure Implementation

*   **Objective:** Build the foundational components for API communication, authentication, error handling, and dependency injection.
*   **`PlanningCenter.Api.Client.Models`:**
    1.  Define base `PlanningCenterApiException.cs` and specific derived exceptions (e.g., `PlanningCenterApiValidationException`, `PlanningCenterApiNotFoundException`, etc.).
*   **`PlanningCenter.Api.Client.Abstractions`:**
    1.  Define `IApiConnection.cs` interface.
    2.  Define `IAuthenticator.cs` interface.
    3.  Define `IPagedResponse<T>.cs` interface.
*   **`PlanningCenter.Api.Client`:**
    1.  Implement `ApiConnection.cs` (`IApiConnection`):
        *   Basic GET, POST, PUT, PATCH, DELETE methods using `IHttpClientFactory`.
        *   JSON serialization/deserialization (`System.Text.Json`).
    2.  Implement `ApiExceptionFactory.cs` to translate HTTP error responses to specific exceptions. Integrate with `ApiConnection`.
    3.  Implement `OAuthAuthenticator.cs` (`IAuthenticator`):
        *   Client credentials flow for token acquisition.
        *   Token caching (in-memory initially).
        *   Token refresh/re-acquisition logic.
    4.  Implement `AuthHandler.cs` (`DelegatingHandler`) to inject the bearer token.
    5.  Implement Polly Resilience Policies in `Resilience/` and integrate them into `ApiConnection` (via `IHttpClientFactory` configuration):
        *   Retry policy.
        *   Rate limiting policy (basic version, respecting `Retry-After` if simple to implement initially).
        *   Circuit breaker policy.
    6.  Implement `DependencyInjection.cs` with `AddPlanningCenterApiClient()`:
        *   Configure `HttpClient` (base address, `AuthHandler`).
        *   Register `IApiConnection`, `IAuthenticator`.
        *   Apply Polly policies.
        *   Define `PlanningCenterApiOptions` for configuration (client ID, secret).
*   **Testing:**
    *   Unit tests for `ApiExceptionFactory`, basic `ApiConnection` (with mocked `HttpMessageHandler`), and `OAuthAuthenticator` (mocking token endpoint).

### Phase 2: People Module - Foundational Implementation

*   **Objective:** Implement a significant portion of the "People" module to establish patterns for models, services, and fluent API. People is chosen due to its centrality and comprehensive schema.
*   **`PlanningCenter.Api.Client.Models`:**
    1.  **People DTOs:** Generate/refine DTOs for all `people/v2/vertices/*.json` schemas (e.g., `PersonDto`, `AddressDto`, `EmailDto`, `PhoneNumberDto`, etc.) in `Models/People/`. Ensure nested objects are strongly typed.
    2.  **Core Person Model:** Create `Core/Person.cs` by consolidating fields from `People.PersonDto` and considering common fields from other modules' Person variants (as identified in `SDK_Analysis.md`).
    3.  **Request Models:** Create necessary request models for People operations (e.g., `PersonCreateRequest.cs`, `PersonUpdateRequest.cs`).
*   **`PlanningCenter.Api.Client.Abstractions`:**
    1.  Define `IPeopleService.cs` with methods for key Person operations (e.g., `GetAsync(id)`, `ListAsync(...)`, `CreateAsync(...)`, `UpdateAsync(...)`).
    2.  Define initial Fluent API interfaces: `IPlanningCenterClient.cs` (with `People()` method) and `IPeopleFluentContext.cs`.
*   **`PlanningCenter.Api.Client`:**
    1.  **Mapping:** Implement mappers in `Mapping/PeopleMappers.cs` to convert between `PersonDto` (and related DTOs like `AddressDto`) and `Core.Person` (and its related core types).
    2.  **Service:** Implement `Services/PeopleService.cs`.
    3.  **Fluent API:** Implement `PlanningCenterClient.cs` and `Fluent/PeopleFluentContext.cs` for basic Person operations.
    4.  Register `IPeopleService` and mappers in `DependencyInjection.cs`.
*   **Testing:**
    *   Unit tests for People mappers, `PeopleService` (mocking `IApiConnection`), and basic People fluent API calls.
    *   Integration tests for fetching a person, listing people, creating, and updating a person. Securely manage credentials.

### Phase 3: Giving Module - Expanding Coverage

*   **Objective:** Implement the "Giving" module, applying and refining patterns from Phase 2.
*   **`PlanningCenter.Api.Client.Models`:**
    1.  **Giving DTOs:** Generate/refine DTOs for key `giving/v2/vertices/*.json` schemas (e.g., `DonationDto`, `FundDto`, `BatchDto`, `GivingPersonDto`) in `Models/Giving/`.
    2.  **Core Models:**
        *   Refine `Core/Person.cs` if Giving-specific fields need broader exposure or special handling in the unified model.
        *   Create `Core/Campus.cs` if not already done, based on Giving and other module definitions.
        *   Create other core models if common patterns emerge (e.g., `Core/Organization.cs` if the existing one needs enhancement).
    3.  **Request Models:** Create request models for Giving operations (e.g., `DonationCreateRequest.cs`).
*   **`PlanningCenter.Api.Client.Abstractions`:**
    1.  Define `IGivingService.cs` for key Giving operations.
    2.  Extend `IPlanningCenterClient` with `Giving()` and define `IGivingFluentContext.cs`.
*   **`PlanningCenter.Api.Client`:**
    1.  **Mapping:** Implement mappers in `Mapping/GivingMappers.cs`.
    2.  **Service:** Implement `Services/GivingService.cs`.
    3.  **Fluent API:** Implement `Fluent/GivingFluentContext.cs`.
    4.  Register `IGivingService` and mappers in `DependencyInjection.cs`.
*   **Testing:**
    *   Unit tests for Giving mappers, `GivingService`, and fluent API.
    *   Integration tests for key Giving operations (e.g., list donations, get a fund).

### Phase 4: Calendar Module & Query/Include Enhancements

*   **Objective:** Implement the "Calendar" module and enhance core infrastructure for more complex queries, includes, and pagination.
*   **`PlanningCenter.Api.Client.Models`:**
    1.  **Calendar DTOs:** Generate/refine DTOs for key `calendar/.../vertices/*.json` schemas (e.g., `EventDto`, `EventInstanceDto`, `ResourceDto`) in `Models/Calendar/`.
    2.  **Request Models:** Create request models for Calendar operations.
*   **`PlanningCenter.Api.Client.Abstractions`:**
    1.  Define `ICalendarService.cs`.
    2.  Extend `IPlanningCenterClient` with `Calendar()` and define `ICalendarFluentContext.cs`.
    3.  Refine `IApiConnection` and service method signatures to better support:
        *   Passing structured query parameters (for filtering, sorting).
        *   Handling `include` parameters for sideloading related data.
        *   Returning `IPagedResponse<T>` for list endpoints.
*   **`PlanningCenter.Api.Client`:**
    1.  **Mapping:** Implement mappers in `Mapping/CalendarMappers.cs`.
    2.  **Service:** Implement `Services/CalendarService.cs`.
    3.  **Fluent API:** Implement `Fluent/CalendarFluentContext.cs`.
    4.  **`ApiConnection` Enhancements:**
        *   Logic to translate structured query options into URL query strings.
        *   Handle deserialization of "included" data from JSON:API responses and assist services/mappers in linking them to primary objects.
        *   Implement `PagedResponse<T>` class.
    5.  Register `ICalendarService` in `DependencyInjection.cs`.
*   **Testing:**
    *   Unit tests for Calendar, query enhancements in `ApiConnection`.
    *   Integration tests for Calendar operations, including tests with `include` and pagination.

### Phase 5: Remaining Modules - Iterative Implementation

*   **Objective:** Implement the remaining API modules one by one or in small groups.
*   **Modules:** Check-Ins, Groups, Publishing, Registrations, Services, Webhooks.
*   **For each module:**
    1.  **Models:** Generate/refine DTOs, create request models.
    2.  **Abstractions:** Define service interface, extend fluent client, define fluent context interface.
    3.  **Client:** Implement mappers, service, fluent context. Register service.
    4.  **Testing:** Write unit and integration tests.
*   **Priority:** Can be determined by user requirements or perceived complexity/usage.

### Phase 6: Advanced Features and Refinements

*   **Objective:** Implement any advanced features from the blueprint and refine existing ones.
*   **Potential Areas:**
    *   **Complex Fluent Queries:** If a more advanced expression-based fluent querying is desired (beyond simple chained methods), implement the necessary parsing and translation logic.
    *   **Advanced Rate Limiting:** More sophisticated handling if the basic `Retry-After` is insufficient (e.g., proactive rate limiting based on known limits).
    *   **Token Storage:** Implement pluggable token storage mechanisms for `OAuthAuthenticator` if persistent or shared caching is needed beyond in-memory.
    *   **Webhooks:** Full implementation for webhook event deserialization and signature verification if covered by the API.

### Phase 7: Examples, Documentation, and Finalization

*   **Objective:** Complete example projects, finalize all documentation, and prepare for potential release.
*   **`examples/` projects:**
    1.  Flesh out `PlanningCenter.Api.Client.Examples.Console` with comprehensive examples for all major modules and API interaction styles.
    2.  Complete `PlanningCenter.Api.Client.Examples.Worker` with realistic scenarios.
*   **Documentation:**
    1.  Ensure all public members in `Abstractions` and `Models` (core/request/response) have complete XML comments.
    2.  Write/Update the main `README.md` for the SDK with detailed installation, configuration, and usage instructions for all major features.
    3.  (Optional) Generate HTML documentation using DocFX.
*   **Code Review:** Conduct a thorough review of the entire SDK.
*   **Performance Testing:** Basic checks for obvious performance bottlenecks.
*   **Refinement:** Address any issues found during reviews and testing.
*   **Packaging:** (If applicable) Prepare NuGet package specifications.

### Phase 8: Submission

*   **Objective:** Submit the completed documentation and (future) codebase.
*   **Steps:**
    1.  Ensure all documentation generated in `/docs/plans/` is complete and coherent.
    2.  (For future code implementation) Ensure all tests pass.
    3.  (For future code implementation) Create a final commit with a comprehensive message summarizing the implemented SDK.
    4.  Use the `submit` tool.

## 3. General Considerations During All Phases

*   **AGENTS.md:** If any `AGENTS.md` files are present in the repository (or added later), their instructions must be followed for any touched files.
*   **User Feedback:** Incorporate user feedback if provided during the documentation or development process.
*   **Testing:** Maintain a high level of unit test coverage. Integration tests should cover critical paths.
*   **Clean Code:** Adhere to .NET coding standards and best practices.

This phased approach allows for manageable chunks of work, early validation of architectural decisions, and progressive delivery of functionality.
