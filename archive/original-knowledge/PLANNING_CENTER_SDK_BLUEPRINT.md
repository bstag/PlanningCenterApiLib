# Planning Center SDK Blueprint

## 1. Introduction

This document outlines the architectural and design blueprint for the new Planning Center .NET SDK. The goal is to create a modern, robust, and easy-to-use library for interacting with the Planning Center API, drawing inspiration from the successful patterns in the `ClickUpApiMcpLib`.

The SDK will provide two primary interaction models:
1.  A **Traditional Service-Based API** for straightforward, repository-style access to API endpoints.
2.  A **Fluent API** for building complex queries and operations in a readable, chainable manner.

## 2. Guiding Principles

*   **Developer Experience:** The SDK should be intuitive and easy to use, with clear documentation and helpful error messages.
*   **Robustness:** The SDK should be resilient to network issues and API errors, with built-in retry mechanisms and a clear exception hierarchy.
*   **Testability:** The architecture should be loosely coupled, allowing consumers to easily mock the SDK for testing their own applications.
*   **Extensibility:** The design should allow for new API endpoints and features to be added with minimal friction.

## 3. Project Structure

The solution will be organized into several projects, following the principles of separation of concerns.

*   `PlanningCenter.Api.Client.sln` (Solution File)
*   `src/`
    *   `PlanningCenter.Api.Client.Models/`: Contains all Plain Old C# Object (POCO) models.
        *   **Request Models:** DTOs for creating and updating resources.
        *   **Response Models:** DTOs for API responses.
        *   **Exceptions:** Custom exception types.
    *   `PlanningCenter.Api.Client.Abstractions/`: Defines the public contract of the SDK.
        *   **Interfaces for Services:** `IPlansService`, `IPeopleService`, etc.
        *   **Interfaces for the Fluent API:** `IPlanningCenterClient`, etc.
        *   **Core Abstractions:** `IApiConnection`, `IAuthenticator`, etc.
    *   `PlanningCenter.Api.Client/`: The main implementation of the SDK.
        *   **Services/:** Concrete implementations of the service interfaces (e.g., `PlansService`).
        *   **Fluent/:** Implementation of the Fluent API.
        *   **Http/:** `ApiConnection` and other HTTP-related classes.
        *   **Auth/:** OAuth 2.0 handling logic (`OAuthAuthenticator`).
        *   `DependencyInjection.cs`: Extension methods for `IServiceCollection`
*   `tests/`
    *   `PlanningCenter.Api.Client.Tests/`: Unit tests.
    *   `PlanningCenter.Api.Client.IntegrationTests/`: Integration tests.
*   `examples/`
    *   `PlanningCenter.Api.Client.Examples.Console/`: A console application demonstrating usage.

## 4. API Design

### 4.1. Traditional Service-Based API

This API provides a straightforward, method-per-endpoint style of interaction.

*   **Design:** Each API resource (e.g., Plans, People, Teams) will have its own service interface and implementation (e.g., `IPlansService` and `PlansService`).
*   **Usage Example:**
    ```csharp
    // Get a specific plan
    var plan = await _plansService.GetAsync(planId);

    // Create a new person
    var newPerson = new PersonCreateRequest { ... };
    var createdPerson = await _peopleService.CreateAsync(newPerson);
    ```

### 4.2. Fluent API

The Fluent API provides a more expressive and powerful way to interact with the API, especially for complex operations.

*   **Design:** A single entry point, `IPlanningCenterClient`, will expose methods for each resource, which in turn expose chainable methods for actions and configuration.
*   **Usage Example:**
    ```csharp
    // The entry point
    var client = new PlanningCenterClient(apiConnection);

    // Fluent query
    var teamMembers = await client
        .Teams("team_id")
        .People()
        .GetAllAsync();

    // Complex creation
    var newPlan = await client
        .Plans()
        .Create(new PlanCreateRequest { ... })
        .WithTeam("team_id")
        .WithPerson("person_id", as: "Worship Leader")
        .ExecuteAsync();
    ```

## 5. Data Models and Unification

A key challenge with the Planning Center API is the varying definitions of common entities like `Person` and `Campus` across different API modules (e.g., People, Check-Ins, Giving). To address this and provide a consistent experience for SDK consumers, the following approach will be adopted:

*   **Unified Core Models:** A central set of comprehensive `Person` and `Campus` models will be defined within `PlanningCenter.Api.Client.Models/Core/`. These models will include all relevant properties found across *all* API-specific implementations.
*   **API-Specific Models:** The existing API-specific models (e.g., `PlanningCenterApiLib.CheckIns.Person`, `PlanningCenterApiLib.Giving.Campus`) will remain as internal DTOs for direct deserialization from API responses.
*   **Adapter Pattern for Mapping:** An Adapter pattern will be employed to map data from the API-specific models to the unified core models. This mapping logic will reside within the `PlanningCenter.Api.Client` project, likely within dedicated mappers or conversion methods associated with the service implementations.
*   **SDK Consumer Interaction:** SDK consumers will primarily interact with the unified core models, abstracting away the underlying API-specific variations. This ensures a consistent data structure regardless of which Planning Center API module the data originated from.

**Example (Conceptual):**
```csharp
// In PlanningCenter.Api.Client.Models/Core/Person.cs
public class Person
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    // ... other common and module-specific properties
    public DateTime? Birthdate { get; set; } // Example of a property that might be module-specific
    public string Gender { get; set; }
    // ...
}

// In PlanningCenter.Api.Client/Services/PeopleService.cs (conceptual mapping)
public async Task<Core.Person> GetPersonAsync(string personId)
{
    // Internal API call returns an API-specific Person DTO
    var apiPerson = await _apiConnection.Get<Api.People.Person>($"/people/v2/people/{personId}");

    // Map to the unified core Person model
    return new Core.Person
    {
        Id = apiPerson.Id,
        FirstName = apiPerson.FirstName,
        LastName = apiPerson.LastName,
        Email = apiPerson.EmailAddresses.FirstOrDefault()?.Address,
        Birthdate = apiPerson.Birthdate,
        Gender = apiPerson.Gender,
        // ... map other properties
    };
}
```

## 6. HTTP Client and Resilience

The core of the SDK's communication will be managed by `HttpClient` and the Polly library for resilience.

*   **Dependency Injection:** An extension method `AddPlanningCenterApiClient()` will configure the `HttpClient` and register all necessary services.
*   **Authentication:**
    *   A dedicated `OAuthAuthenticator` class will handle the OAuth 2.0 flow. It will be responsible for token acquisition, storage, and transparent refresh.
    *   A `DelegatingHandler` will be used to attach the bearer token to outgoing requests.
*   **Resilience Policies (using Polly):**
    1.  **Retry Policy:** A general transient error retry policy (for 5xx errors, network issues).
    2.  **Rate Limiting Policy:** A policy to handle `429 Too Many Requests` responses. It will respect the `Retry-After` header if present, otherwise use an exponential backoff strategy.
    3.  **Circuit Breaker Policy:** A circuit breaker to prevent the application from repeatedly trying to call a service that is known to be failing.

*   **`DependencyInjection.cs` Example:**
    ```csharp
    public static IServiceCollection AddPlanningCenterApiClient(this IServiceCollection services, Action<PlanningCenterApiOptions> configureOptions)
    {
        // ... configure options

        services.AddHttpClient<IApiConnection, ApiConnection>(client =>
        {
            client.BaseAddress = new Uri("https://api.planningcenteronline.com/");
        })
        .AddHttpMessageHandler<AuthHandler>() // Handles adding the auth token
        .AddPolicyHandler(GetRetryPolicy())
        .AddPolicyHandler(GetRateLimitPolicy()) // Custom policy for rate limiting
        .AddPolicyHandler(GetCircuitBreakerPolicy());

        // ... register services and authenticator
        services.AddScoped<IAuthenticator, OAuthAuthenticator>();
        services.AddScoped<AuthHandler>();

        return services;
    }
    ```

## 7. Error Handling

A clear and predictable exception hierarchy is crucial for a good developer experience.

*   **Base Exception:** `PlanningCenterApiException` will be the base for all exceptions originating from the SDK.
*   **Specific Exceptions:**
    *   `PlanningCenterApiValidationException (400)`: For request validation errors. Will contain details about the validation failures.
    *   `PlanningCenterApiAuthenticationException (401)`: For authentication errors.
    *   `PlanningCenterApiAuthorizationException (403)`: For permission errors.
    *   `PlanningCenterApiNotFoundException (404)`: For resource not found errors.
    *   `PlanningCenterApiRateLimitException (429)`: When the rate limit is exceeded and retries have failed.
    *   `PlanningCenterApiServerException (5xx)`: For server-side errors.
*   **Exception Factory:** An `ApiExceptionFactory` will be responsible for creating the appropriate exception based on the HTTP status code and response content.

## 8. Documentation

*   **XML Comments:** All public members will be fully documented with XML comments to support IntelliSense.
*   **README.md:** A comprehensive README with clear usage examples for both API styles.
*   **Examples Project:** A working console application to demonstrate common use cases.

## 9. API Data and Documentation Structure

This section outlines the location of API data (JSON schema files) and module-specific documentation within the project.

### 9.1. API Data (JSON Schema Files)

The `api_data` directory contains JSON files representing the schema of various API resources. These files are organized by Planning Center application and API version.

*   **Location:** `planning-center-sdk/api_data/`
*   **Structure:**
    *   `planning-center-sdk/api_data/calendar/2022-07-07/vertices/*.json`
    *   `planning-center-sdk/api_data/check-ins/v2/vertices/*.json`
    *   `planning-center-sdk/api_data/giving/v2/vertices/*.json`
    *   `planning-center-sdk/api_data/groups/v2/vertices/*.json`
    *   `planning-center-sdk/api_data/people/v2/vertices/*.json`
    *   `planning-center-sdk/api_data/publishing/v2/vertices/*.json`
    *   `planning-center-sdk/api_data/registrations/v2/vertices/*.json`
    *   `planning-center-sdk/api_data/services/v2/vertices/*.json`
    *   `planning-center-sdk/api_data/webhooks/v2/vertices/*.json`

### 9.2. Module-Specific README.md Files

Each module within the `objects` directory and the top-level `planning-center-sdk` directory contains a `README.md` file providing specific documentation for that module.

*   **Locations:**
    *   `planning-center-sdk/calendar.md`
    *   `planning-center-sdk/check-ins.md`
    *   `planning-center-sdk/giving.md`
    *   `planning-center-sdk/groups.md`
    *   `planning-center-sdk/people.md`
    *   `planning-center-sdk/publishing.md`
    *   `planning-center-sdk/registrations.md`
    *   `planning-center-sdk/services.md`
    *   `planning-center-sdk/webhooks.md`
    *   `planning-center-sdk/objects/Calendar/README.md`
    *   `planning-center-sdk/objects/CheckIns/README.md`
    *   `planning-center-sdk/objects/common/README.md`
    *   `planning-center-sdk/objects/Giving/README.md`
    *   `planning-center-sdk/objects/Groups/README.md`
    *   `planning-center-sdk/objects/people/README.md`
    *   `planning-center-sdk/objects/Registrations/README.md`
