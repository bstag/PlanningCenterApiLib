# Planning Center SDK - Existing Assets Analysis

Date: 2023-10-27

## 1. Introduction

This document summarizes the analysis of existing assets provided for the Planning Center .NET SDK development. The goal is to understand the current state of documentation, API definitions, and preliminary object models to inform the SDK implementation strategy.

The primary sources reviewed are:
*   `planning-center-sdk/PLANNING_CENTER_SDK_BLUEPRINT.md`
*   JSON schema files in `planning-center-sdk/api_data/`
*   Existing C# object definitions in `planning-center-sdk/objects/`
*   Module-specific markdown documentation (`planning-center-sdk/*.md` and `planning-center-sdk/objects/*/README.md`)

## 2. Key Findings

### 2.1. `PLANNING_CENTER_SDK_BLUEPRINT.md`

This is a comprehensive document outlining the vision for the SDK. Key takeaways include:

*   **Dual API Design:** Support for both a traditional service-based API and a fluent API.
*   **Project Structure:** A well-defined multi-project structure (`Models`, `Abstractions`, `Client`, `Tests`, `Examples`) targeting .NET 9.
*   **Unified Core Models:** A crucial strategy to address inconsistencies in API object definitions (e.g., `Person`, `Campus`) across different Planning Center modules. An Adapter pattern is proposed for mapping.
*   **HTTP Client & Resilience:** Use of `HttpClient` with Polly for retry, rate limiting, and circuit breaker patterns.
*   **Authentication:** OAuth 2.0 handled by a dedicated `OAuthAuthenticator` and `DelegatingHandler`.
*   **Error Handling:** A defined exception hierarchy based on `PlanningCenterApiException`.
*   **Documentation:** Emphasis on XML comments, a comprehensive README, and example projects.
*   **API Data Pointers:** Correctly points to `api_data` for JSON schemas and various markdown files for module-specific documentation.

This blueprint serves as an excellent foundation for the SDK development.

### 2.2. JSON Schema Files (`planning-center-sdk/api_data/`)

These files provide the most detailed and definitive information about the API structure.

*   **Organization:** Schemas are organized by Planning Center application (Calendar, Check-Ins, Giving, Groups, People, Publishing, Registrations, Services, Webhooks) and API version. JSON files are typically located in a `vertices` subdirectory.
*   **Rich Metadata:** Each schema (`*.json`) is rich in metadata, defining:
    *   **Attributes:** Fields of the API object with type annotations and descriptions.
    *   **Relationships:** `to_one` and `to_many` relationships to other objects, detailed as `attributes` (for direct relationships like `primary_campus_id`) and `relationships` (for object graph connections).
    *   **Edges:** `outbound_edges` (resources accessible from the current object) and `inbound_edges` (how the current object can be accessed from other objects), including API paths.
    *   **Permissions:** `can_create`, `can_update`, `can_destroy`, and assignable fields.
    *   **Actions:** Specific operations that can be performed on an entity (e.g., `issue_refund` on a `Donation`).
    *   **Queryability:** Extensive details on `can_include` (for sideloading related data), `can_order` (sortable fields), `can_query` (filterable fields via `where` clauses), `per_page`, and `offset`.
*   **Object Variations:** Confirmed significant variations in the definition of common objects like `Person` across different modules (e.g., `people/v2/person.json` vs. `giving/v2/person.json` vs. `calendar/2022-07-07/person.json`). This underscores the importance of the unified core model strategy from the blueprint.
    *   The `people/v2/vertices/person.json` appears to be the most comprehensive definition and should be the primary basis for the `Core.Person` model.
*   **Completeness:** These schemas appear to be the ground truth for API object structures and capabilities.

### 2.3. Existing C# Object Definitions (`planning-center-sdk/objects/`)

These files represent an initial attempt at creating C# POCOs for the API objects.

*   **Structure:** Organized by namespace mirroring API modules (e.g., `PlanningCenterApiLib.People`). A `common` folder exists for shared objects like `Organization`.
*   **Direct Mapping:** Classes generally map directly to attributes found in the JSON schemas.
*   **Basic Types:** Uses standard C# data types.
*   **Handling of Complex JSON Structures:**
    *   Nested JSON objects are sometimes typed as `object` (e.g., `DirectorySharedInfo` in `People.Person.cs`). These will require dedicated C# classes.
    *   Arrays of complex objects are sometimes typed as `List<object>` (e.g., `EmailAddresses` in `Giving.Person.cs`). These will need specific classes (e.g., `List<EmailAddress>`). The `people` module already has some of these more specific classes like `Address.cs`, `Email.cs`.
*   **Lack of Relationships/Navigation Properties:** The POCOs are flat and do not currently include properties to represent relationships to other entities (e.g., no `PrimaryCampus` property of type `Campus` on `People.Person`). This is a major area for development in the new SDK.
*   **Inconsistent `Person` Models:** The different `Person.cs` files for People, Giving, and Calendar modules have different property sets, further highlighting the need for the unified `Core.Person` model.
*   **Foundation:** Provides a basic set of C# classes that can be built upon, but requires significant refinement and additions to align with the full capabilities exposed in the JSON schemas and the SDK blueprint's vision.

### 2.4. Module-Specific Markdown Documentation

These files provide supplementary information about the API modules.

*   **Two Types of Markdown Files:**
    *   **Root Level (`planning-center-sdk/<module_name>.md`):** These are more API-centric, often detailing API paths, endpoints for various operations, available URL parameters (`include`, `order`, `where`, pagination), and example JSON payloads. These are generally more useful for understanding API interaction. (e.g., `planning-center-sdk/calendar.md`).
    *   **Object Level (`planning-center-sdk/objects/<ModuleName>/README.md`):** These tend to be a human-readable listing of the attributes of the existing C# classes in that directory. They offer less API detail than the root-level markdown files. (e.g., `planning-center-sdk/objects/Calendar/README.md`).
*   **Content:**
    *   Summarized object attributes.
    *   Listings of key API endpoints.
    *   Information on URL parameters.
*   **Varying Detail:** The level of detail and comprehensiveness varies significantly between these files. Some are quite detailed, while others are sparse.
*   **Value:** The root-level `*.md` files offer good supplementary context to the JSON schemas, especially regarding available query parameters and endpoint structures.

## 3. Summary of API Modules and Key Entities (Preliminary)

Based on the `api_data` and markdown files, here's a brief overview of the modules and some of their key entities:

*   **Calendar:** `Event`, `EventInstance`, `Resource`, `ResourceBooking`, `Attachment`, `Conflict`, `Tag`, `Feed`. Seems to have a distinct `Person` model.
*   **Check-Ins:** `CheckIn`, `Event`, `EventPeriod`, `Location`, `Station`, `Person`. Has its own `Person` model.
*   **Giving:** `Donation`, `Batch`, `Fund`, `Pledge`, `RecurringDonation`, `Person`, `Campus`. Has its own `Person` and `Campus` models.
*   **Groups:** `Group`, `Event`, `Membership`, `Tag`, `Location`, `Person`. Has its own `Person` and `Campus` models.
*   **People:** This is the central People module. Key entities include `Person` (the most comprehensive version), `Address`, `Email`, `PhoneNumber`, `Household`, `List`, `Workflow`, `Form`, `Note`, `Campus`.
*   **Publishing:** `Episode`, `Series`, `Speaker`, `Media`.
*   **Registrations:** `Signup`, `Attendee`, `Category`, `SelectionType`, `Person`. Has its own `Person` and `Campus` models.
*   **Services:** `ServiceType`, `Plan`, `Item`, `Song`, `Arrangement`, `Team`, `PersonTeamPositionAssignment`. Likely has its own `Person` model variant.
*   **Webhooks:** `WebhookSubscription`, `AvailableEvent`, `Event` (webhook event).

## 4. Key Challenges and Considerations for SDK Development

*   **Model Unification:** Implementing the unified `Core.Person`, `Core.Campus`, and potentially other common models, along with the necessary mapping logic, will be a critical and complex task.
*   **Strongly-Typed Models:** Converting `object` and `List<object>` properties in existing C# classes to strongly-typed classes based on JSON schema definitions.
*   **Relationship Handling:** Designing how the SDK models will represent and provide access to related entities (navigation properties, dedicated methods for fetching related data, handling `include` parameters).
*   **Comprehensive Endpoint Coverage:** Ensuring the SDK covers all relevant API endpoints and actions documented in the JSON schemas and markdown files.
*   **Query Parameter Support:** Implementing robust support for filtering, sorting, pagination, and sideloading (includes).
*   **Code Generation vs. Manual Implementation:** For models, especially from the detailed JSON schemas, a code generation strategy (even if partial or assisted) could save significant time and reduce errors, but would need to be carefully managed.

## 5. Conclusion

The existing assets provide a strong starting point, particularly the `PLANNING_CENTER_SDK_BLUEPRINT.md` and the detailed JSON schemas in `api_data`. The primary focus for the initial stages of SDK development (related to models) will be to translate the rich information from the JSON schemas into robust, unified, and interconnected C# models, going beyond the current basic POCOs. The markdown files serve as useful supplementary documentation for understanding API usage patterns and available parameters.
