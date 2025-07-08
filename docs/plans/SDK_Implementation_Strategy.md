# Planning Center SDK - Implementation Strategy

Date: 2023-10-27

## 1. Introduction

This document outlines the detailed implementation strategy for the Planning Center .NET SDK. It builds upon the `PLANNING_CENTER_SDK_BLUEPRINT.md` and the findings from the `PlanningCenter_SDK_Analysis.md`. The strategy covers the SDK's architecture, project structure, model definition, API interaction styles, and other critical development aspects.

## 2. Overall Architecture

The SDK will adhere to the architecture proposed in the blueprint, ensuring a modern, robust, and developer-friendly library.

*   **Target Framework:** .NET 9
*   **Core Principles:**
    *   **Separation of Concerns:** Achieved through a multi-project solution structure.
    *   **Developer Experience:** Intuitive API design, clear documentation, and helpful error messages.
    *   **Robustness:** Resilience to network issues and API errors via Polly.
    *   **Testability:** Loosely coupled components for easy mocking and testing.
    *   **Extensibility:** Design for straightforward addition of new API endpoints and features.
*   **API Interaction Styles:**
    1.  **Traditional Service-Based API:** Repository-style access for direct endpoint interaction.
    2.  **Fluent API:** Expressive, chainable interface for building complex queries and operations.

## 3. Project Breakdown and Detailed Plan

The solution will be structured as follows:

*   `PlanningCenter.Api.Client.sln` (Solution File)
*   `src/`
    *   `PlanningCenter.Api.Client.Models/`
    *   `PlanningCenter.Api.Client.Abstractions/`
    *   `PlanningCenter.Api.Client/`
*   `tests/`
    *   `PlanningCenter.Api.Client.Tests/`
    *   `PlanningCenter.Api.Client.IntegrationTests/`
*   `examples/`
    *   `PlanningCenter.Api.Client.Examples.Console/`
    *   `PlanningCenter.Api.Client.Examples.Worker/`

### 3.1. `PlanningCenter.Api.Client.Models`

This project will contain all Plain Old C# Object (POCO) models.

*   **Subfolders:**
    *   `Core/`: For unified core models.
        *   `Person.cs`: Will consolidate fields from all `Person` variants across PCO apps (People, Giving, Calendar, etc.). Based primarily on `people/v2/vertices/person.json` with additions from other modules.
        *   `Campus.cs`: Unified `Campus` model.
        *   *(Potentially others like `Organization` if significant variations exist beyond the current `common/Organization.cs`)*.
    *   `<ModuleName>/`: For API-specific Data Transfer Objects (DTOs) directly mapping to JSON schemas (e.g., `Calendar/EventDto.cs`, `Giving/DonationDto.cs`).
        *   These DTOs will be used for serialization/deserialization by `ApiConnection`.
        *   They will include all attributes and simple relationships (like foreign keys).
    *   `Requests/`: For request models used in `POST`, `PATCH`, `PUT` operations, organized by module if necessary.
    *   `Responses/`: For specialized response models if different from primary DTOs (e.g., paged results containers).
    *   `Enums/`: For any enumerations needed (e.g., status types).
    *   `Exceptions/`: Custom exception types.
        *   `PlanningCenterApiException.cs` (Base class)
        *   `PlanningCenterApiValidationException.cs` (400)
        *   `PlanningCenterApiAuthenticationException.cs` (401)
        *   `PlanningCenterApiAuthorizationException.cs` (403)
        *   `PlanningCenterApiNotFoundException.cs` (404)
        *   `PlanningCenterApiRateLimitException.cs` (429)
        *   `PlanningCenterApiServerException.cs` (5xx)
        *   *(Others as needed)*

*   **Model Generation Approach:**
    *   **Initial Generation:** Investigate using a JSON to C# POCO generator tool (e.g., `json2csharp.com`, Visual Studio's "Paste JSON as Classes", or a custom script) for an initial pass of the DTOs in `<ModuleName>/` subfolders based on the `api_data` JSON schemas. This will save significant manual effort.
    *   **Manual Refinement:** Generated DTOs will require manual review and refinement:
        *   Adjusting namespaces and class names.
        *   Ensuring correct data types (e.g., `DateTimeOffset` for date-times, `decimal` for currency if appropriate, though `AmountCents` pattern seems prevalent).
        *   Adding XML comments based on descriptions in JSON schemas.
        *   Replacing generic `object` or `List<object>` with strongly-typed classes for nested structures (e.g., `Address`, `Email` within `Person` DTOs from modules other than People).
    *   **Unified Core Models (`Core/`):** These will be crafted manually, drawing from the various module-specific DTOs and the blueprint's guidance.
    *   **Request Models:** Manually created, as they often represent a subset of fields or specific structures for API operations.

*   **Naming Convention:**
    *   API-specific DTOs might be suffixed with `Dto` (e.g., `PeoplePersonDto.cs`) if their names would otherwise clash with unified core models or to clearly distinguish their purpose.
    *   Unified models will have simple names (e.g., `Core.Person.cs`).

### 3.2. `PlanningCenter.Api.Client.Abstractions`

This project defines the public contract of the SDK.

*   **Interfaces for Services:**
    *   `IPeopleService.cs`, `IGivingService.cs`, `ICalendarService.cs`, etc. (one per PCO module).
    *   Methods will typically accept request objects (from `Models/Requests/`) and return unified core models (e.g., `Task<Core.Person> GetPersonAsync(string id)`), or `IEnumerable<Core.Model>` for lists.
    *   Will include methods for all CRUD operations and specific actions outlined in JSON schemas and markdown docs.
*   **Interfaces for Fluent API:**
    *   `IPlanningCenterClient.cs`: The main entry point (e.g., `client.People()`, `client.Giving()`).
    *   `IPeopleFluentContext.cs`, `IGivingFluentContext.cs`, etc.: Returned by `client.People()`, exposing module-specific actions.
    *   `IQueryableFluent<TModel, TResourceIdentifier>.cs`: For chainable query operations (e.g., `.WithId(id).Include(p => p.Addresses).GetAsync()`).
    *   `ICreateFluent<TModel, TRequest>.cs`, `IUpdateFluent<TModel, TRequest>.cs`: For chainable create/update operations.
*   **Core Abstractions:**
    *   `IApiConnection.cs`: Handles HTTP communication (GET, POST, PUT, PATCH, DELETE).
        *   Methods like `Task<TResponse> GetAsync<TResponse>(string endpoint, Dictionary<string, string> queryParameters = null)`.
    *   `IAuthenticator.cs`: Interface for authentication (e.g., `Task<string> GetAccessTokenAsync()`).
    *   `IPagedResponse<T>.cs`: Interface for responses that support pagination.
    *   `IApiRateLimiter.cs` (Optional, if Polly policies need more abstraction).
*   **All public members will have comprehensive XML comments.**

### 3.3. `PlanningCenter.Api.Client`

This project is the main implementation of the SDK.

*   **Subfolders:**
    *   `Services/`: Concrete implementations of service interfaces (e.g., `PeopleService.cs`).
        *   Inject `IApiConnection` and mappers.
        *   Responsible for:
            *   Constructing API endpoint paths.
            *   Preparing request objects.
            *   Calling `IApiConnection`.
            *   Mapping DTO responses to unified core models (or returning DTOs if no unified model applies).
    *   `Fluent/`: Implementation of the Fluent API interfaces.
        *   `PlanningCenterClient.cs`.
        *   Context-specific classes (e.g., `PeopleFluentContext.cs`).
        *   Query builder classes.
        *   These will often delegate to the traditional services or `IApiConnection`.
    *   `Http/`:
        *   `ApiConnection.cs`: Implements `IApiConnection`. Uses `IHttpClientFactory` to get `HttpClient` instances. Manages serialization/deserialization (likely using `System.Text.Json`).
        *   `AuthHandler.cs`: `DelegatingHandler` to attach bearer tokens. Injects `IAuthenticator`.
        *   `ApiExceptionFactory.cs`: Creates specific exceptions based on HTTP status codes and response content.
    *   `Auth/`:
        *   `OAuthAuthenticator.cs`: Implements `IAuthenticator`. Handles OAuth 2.0 token acquisition, storage (consider secure storage options or delegate to consumer), and transparent refresh.
        *   `TokenStorage/` (optional): Interfaces/implementations for token storage if built-in.
    *   `Mapping/`: Contains mapper classes or extension methods for converting between API-specific DTOs and unified core models.
        *   Tools like AutoMapper could be considered, or manual mapping for more control. Given the potential complexity and nuances, manual mapping might be preferred initially.
    *   `Resilience/`: Polly policy configurations.
        *   `HttpRetryPolicies.cs`: Defines `IAsyncPolicy<HttpResponseMessage>` for retries.
        *   `HttpRateLimitPolicies.cs`: Defines policies for handling 429s (respecting `Retry-After`).
        *   `HttpCircuitBreakerPolicies.cs`: Defines circuit breaker policies.
    *   `DependencyInjection.cs`: Extension methods for `IServiceCollection` (`AddPlanningCenterApiClient()`).
        *   Configures `HttpClient` via `IHttpClientFactory`.
        *   Adds `AuthHandler`.
        *   Applies Polly policies.
        *   Registers `IApiConnection`, `IAuthenticator`, all services, and mappers.
        *   Takes `PlanningCenterApiOptions` (e.g., Client ID, Client Secret, Base URL if configurable).

### 3.4. `PlanningCenter.Api.Client.Tests`

Unit tests for the SDK.

*   **Framework:** MSTest, xUnit, or NUnit (developer preference, ensure consistency).
*   **Mocking:** Moq or NSubstitute.
*   **Coverage:**
    *   Service implementations (mocking `IApiConnection` and mappers).
    *   Fluent API logic.
    *   Model mapping logic.
    *   `ApiConnection` (mocking `HttpMessageHandler` to test request/response handling without actual HTTP calls).
    *   `OAuthAuthenticator` (mocking token endpoint calls).
    *   `ApiExceptionFactory`.
    *   Polly policy behavior (where testable in isolation).
*   **Structure:** Mirror the `PlanningCenter.Api.Client` project structure for test classes.

### 3.5. `PlanningCenter.Api.Client.IntegrationTests`

Integration tests making actual calls to the Planning Center API.

*   **Scope:** Test key functionalities for each module, focusing on:
    *   Authentication flow.
    *   CRUD operations for major entities using both traditional and fluent APIs.
    *   Querying with various parameters (filtering, sorting, includes, pagination).
*   **Configuration:**
    *   Requires a mechanism to securely manage API credentials (Client ID, Client Secret, test user credentials if needed). This should NOT be hardcoded. Options: User secrets, environment variables, configuration files excluded from source control.
    *   Tests should ideally run against a dedicated test PCO account or sandbox environment if available to avoid impacting production data.
*   **Data Management:** Tests may need to create and clean up their own data or rely on a pre-configured test data set.

### 3.6. Example Projects (`examples/`)

*   **`PlanningCenter.Api.Client.Examples.Console`:**
    *   Demonstrate SDK setup and DI.
    *   Showcase common use cases for the traditional service API (e.g., get a person, list donations for a fund).
    *   Showcase common use cases for the Fluent API (e.g., complex queries, creating entities with relationships).
    *   Include examples of error handling.
*   **`PlanningCenter.Api.Client.Examples.Worker`:**
    *   Demonstrate SDK usage in a background service context (e.g., periodically fetching new data, processing items from a queue).
    *   Highlight considerations for long-running processes and token management.

## 4. Service Implementation Approach (Traditional API)

*   Each service (e.g., `PeopleService`) will implement its corresponding interface (e.g., `IPeopleService`).
*   Services will take `IApiConnection` and necessary mappers as constructor dependencies.
*   Methods will:
    1.  Accept parameters (e.g., IDs, request objects).
    2.  Construct the appropriate API endpoint URL (potentially using a helper for base URL and path concatenation).
    3.  Prepare query parameters based on method arguments.
    4.  If the method involves a request body (POST, PUT, PATCH), use the provided request model.
    5.  Call the relevant method on `IApiConnection` (e.g., `_apiConnection.GetAsync<PeoplePersonDto>($"/people/v2/people/{id}")`).
    6.  The `ApiConnection` will handle deserializing the JSON response into the specified DTO type.
    7.  The service method will then map the DTO to the unified core model (e.g., `_personMapper.MapToCore(personDto)`).
    8.  Return the unified core model or a collection of them.
    9.  Error handling will primarily be managed by `ApiConnection` which uses `ApiExceptionFactory`, so services will mostly let exceptions propagate unless specific handling is needed.

## 5. Fluent API Design

*   **Entry Point:** `IPlanningCenterClient` will have methods like `People()`, `Giving()`, returning context-specific fluent interfaces (e.g., `IPeopleFluentContext`).
*   **Context Interfaces:** `IPeopleFluentContext` would offer methods like `WithId(string id)`, `GetAll()`, `Create(PersonCreateRequest request)`.
*   **Chainable Operations:**
    *   `GetByIdAsync(string id)`: Simple retrieval.
    *   `GetAllAsync(Action<IQueryParameters> params = null)`: For listing resources, allowing lambda for query params.
    *   For complex queries: `client.People().Query().Where(p => p.FirstName == "John").Include(p => p.Emails).OrderBy(p => p.LastName).ToListAsync()`. This implies a more complex query builder that translates expressions to API query strings. Initially, a simpler approach with explicit methods like `.FilterByName("John").IncludeEmails().OrderByLastName().GetAsync()` might be more feasible. The blueprint suggests a simpler fluent approach: `client.Teams("team_id").People().GetAllAsync()`. This will be the initial target.
*   **Execution:** Methods like `ExecuteAsync()`, `GetAsync()`, `ToListAsync()` will trigger the actual API call.
*   **Underlying Mechanism:** Fluent API implementations will build up request parameters and then delegate to either the traditional services or directly to `IApiConnection`.

## 6. Authentication Handling (OAuth 2.0)

*   `OAuthAuthenticator` will manage the client credentials grant flow (or authorization code flow if user interaction is needed for initial setup, though client credentials is more common for server-to-server SDKs).
*   It will be responsible for:
    *   Requesting an access token from PCO's token endpoint.
    *   Caching the token.
    *   Detecting token expiry and transparently refreshing it using a refresh token (if PCO API supports refresh tokens with client credentials) or by re-requesting with client credentials.
*   `AuthHandler` (DelegatingHandler) will inject `IAuthenticator` and call `GetAccessTokenAsync()` to add the `Authorization: Bearer <token>` header to each outgoing request.
*   Configuration (`PlanningCenterApiOptions`) will hold Client ID and Client Secret.

## 7. Error Handling Strategy

*   As defined in the blueprint, `ApiConnection` will inspect HTTP responses.
*   If the status code indicates an error (4xx or 5xx), `ApiExceptionFactory` will be used.
*   `ApiExceptionFactory` will:
    *   Attempt to deserialize any error details from the response body.
    *   Create and throw the appropriate specific exception (e.g., `PlanningCenterApiNotFoundException` for 404) populated with error details.
*   All SDK-originated exceptions will derive from `PlanningCenterApiException`.

## 8. Resilience Strategy (Polly)

*   Policies will be configured in `DependencyInjection.cs` and applied to the `HttpClient`.
*   **Retry Policy:** Handle transient errors (e.g., 500, 502, 503, 504, `HttpRequestException`). Use exponential backoff with jitter.
*   **Rate Limiting Policy:** For `429 Too Many Requests`.
    *   If `Retry-After` header is present (integer seconds or HTTP date), wait for that duration.
    *   Otherwise, use an exponential backoff.
*   **Circuit Breaker Policy:** After a configured number of consecutive failures, the circuit will open for a duration, preventing further calls and failing fast. This helps prevent overwhelming a struggling service.

## 9. Documentation Strategy (for SDK Code)

*   **XML Comments:** All public types, methods, properties, and enum members in `PlanningCenter.Api.Client.Abstractions` and `PlanningCenter.Api.Client.Models` (especially core models and request/response models) will have comprehensive XML documentation comments. This will enable IntelliSense in consuming projects and allow for documentation generation.
*   **README.md (Root of SDK):** Will be the primary source of documentation for SDK consumers. It will include:
    *   Installation instructions (NuGet).
    *   DI setup.
    *   Authentication configuration.
    *   Detailed usage examples for both traditional and fluent APIs covering common scenarios.
    *   Error handling guidance.
    *   Information on pagination and rate limits.
*   **Example Projects:** Will serve as live, runnable documentation.
*   **DocFX (Optional):** Consider using DocFX or a similar tool to generate HTML documentation from XML comments and markdown files for a more formal documentation website if desired.

## 10. Iterative Development

Given the size of the Planning Center API, development will be iterative:
1.  **Core Infrastructure:** First, implement the core projects, `IApiConnection`, `IAuthenticator`, basic error handling, DI, and resilience policies.
2.  **Key Module(s) First:** Implement models (DTOs and Core), services, and fluent API for one or two key modules (e.g., People, Giving) to establish patterns and validate the architecture.
3.  **Expand Module Coverage:** Incrementally add support for other modules.
4.  **Continuous Testing:** Write unit and integration tests alongside development for each module.
5.  **Refine and Polish:** Regularly review and refine the API design and implementation.

This strategy aims to build a high-quality, maintainable, and easy-to-use SDK for the Planning Center API.
