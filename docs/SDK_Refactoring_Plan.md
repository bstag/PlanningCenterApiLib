# Planning Center API SDK Refactoring Plan

## 1. Executive Summary

This document outlines a comprehensive refactoring plan for the Planning Center API .NET SDK. The current implementation of the service layer suffers from significant code duplication, inconsistent use of the `ServiceBase` class, and a lack of adherence to DRY (Don't Repeat Yourself) and SOLID principles. These issues make the SDK difficult to maintain, extend, and debug.

The proposed refactoring will focus on the following key areas:

1.  **Enhancing the `ServiceBase` Class:** We will complete the implementation of the `ServiceBase` class by adding generic helper methods for all common CRUD operations (Create, Update, Delete) and for handling pagination.
2.  **Refactoring All Services to Use `ServiceBase`:** All existing services will be refactored to use the new generic helper methods from the enhanced `ServiceBase`. This will dramatically reduce code duplication and centralize the core API interaction logic.
3.  **Standardizing Mappers:** We will introduce a standard interface for mappers to ensure consistency and predictability across the SDK.
4.  **Introducing a `IPlannableResource` Interface:** To eliminate the use of reflection and improve type safety, we will introduce a common interface for all domain models.

By implementing these changes, we will create a more robust, maintainable, and consistent SDK that is easier to use and extend.

## 2. `ServiceBase` Enhancements

The `ServiceBase` class will be enhanced to provide a complete set of generic helper methods for all common API operations.

### 2.1. Add Generic `CreateResourceAsync`, `UpdateResourceAsync`, and `DeleteResourceAsync` Methods

These methods will encapsulate the logic for creating, updating, and deleting resources, including:

*   Input validation
*   Request object mapping
*   API interaction (POST, PATCH, DELETE)
*   Response handling and mapping
*   Consistent logging and exception handling

### 2.2. Add Generic Pagination Helpers

The `GetAllAsync` and `StreamAsync` methods, which are currently duplicated in every service, will be moved into `ServiceBase` as generic helper methods. These methods will handle all the complexities of pagination, including:

*   Fetching pages of data
*   Managing page offsets and limits
*   Providing both a list-based and a streaming interface

## 3. Service-by-Service Refactoring Plan

Each service will be refactored to use the new generic helper methods from `ServiceBase`. This will involve removing the duplicated CRUD and pagination logic from each service and replacing it with calls to the appropriate `ServiceBase` helper methods.

### 3.1. `CalendarService`

*   **Refactor `CreateEventAsync`, `UpdateEventAsync`, `DeleteEventAsync`:** Replace the duplicated logic with calls to `CreateResourceAsync`, `UpdateResourceAsync`, and `DeleteResourceAsync`.
*   **Refactor `CreateResourceAsync`, `UpdateResourceAsync`, `DeleteResourceAsync`:** Replace the duplicated logic with calls to `CreateResourceAsync`, `UpdateResourceAsync`, and `DeleteResourceAsync`.
*   **Refactor `GetAllEventsAsync`, `StreamEventsAsync`:** Replace the duplicated pagination logic with calls to the new generic pagination helpers in `ServiceBase`.

### 3.2. `CheckInsService`

*   **Methods to Consolidate:**
    *   `ListCheckInsAsync`, `CreateCheckInAsync`, `UpdateCheckInAsync`, `GetEventAsync`, `ListEventsAsync`, and `ListEventCheckInsAsync` will be refactored to use the generic `ServiceBase` helpers (`ListResourcesAsync`, `CreateResourceAsync`, `UpdateResourceAsync`, `GetResourceByIdAsync`).
*   **Logic to Move to `ServiceBase`:**
    *   The duplicated pagination logic in `GetAllCheckInsAsync` and `StreamCheckInsAsync` will be moved to `ServiceBase` and these methods will be updated to call the new generic helpers.
*   **Unique Logic to Preserve:**
    *   The `CheckOutAsync` method contains unique logic (a `PATCH` to a custom endpoint with an empty body). This method will be preserved in the service, but its implementation will be simplified to use the base `ExecuteAsync` method for consistent error handling and logging.

### 3.3. `GivingService`

*   **Methods to Consolidate:**
    *   All CRUD methods (`Get...`, `List...`, `Create...`, `Update...`, `Delete...`) for all entities (Donations, Funds, Batches, Pledges, Recurring Donations, Refunds, and Payment Sources) will be refactored to use the generic `ServiceBase` helpers.
    *   Person-specific operations like `GetDonationsForPersonAsync` will also use the `ListResourcesAsync` helper with the appropriate endpoint.
*   **Logic to Move to `ServiceBase`:**
    *   The duplicated pagination logic in `GetAllDonationsAsync` and `StreamDonationsAsync` will be moved to `ServiceBase` and these methods will be updated to call the new generic helpers.
*   **Unique Logic to Preserve:**
    *   `CommitBatchAsync` is a custom action (`POST` to `/batches/{id}/commit`) and will be preserved, but its implementation will be simplified to use the base `ExecuteAsync` method.
    *   `GetTotalGivingAsync` contains aggregation logic. The API call within this method will be replaced with a call to the refactored `ListDonationsAsync` to benefit from centralized error handling.

### 3.4. `GroupsService`

*   **Methods to Consolidate:**
    *   All CRUD methods for `Group`, `GroupType`, and `Membership` (e.g., `GetGroupAsync`, `ListGroupsAsync`, `CreateGroupAsync`, `ListGroupMembershipsAsync`, etc.) will be refactored to use the generic `ServiceBase` helpers.
*   **Logic to Move to `ServiceBase`:**
    *   The duplicated pagination logic in `GetAllGroupsAsync` and `StreamGroupsAsync` will be moved to `ServiceBase` and these methods will be updated to call the new generic helpers.
*   **Redundant Code to Remove:**
    *   The `ListGroupsWithLoggingAsync` method is redundant and will be removed. Standardized logging will be provided by the `ServiceBase` helpers.

### 3.5. `PeopleService`

*   **Methods to Consolidate:**
    *   All CRUD methods for `Person`, `Address`, `Email`, `PhoneNumber`, `Household`, `Workflow`, `Form`, and `PeopleList` entities will be refactored to use the generic `ServiceBase` helpers. The existing `GetAsync` and `ListAsync` methods already use these helpers and can serve as a model.
*   **Logic to Move to `ServiceBase`:**
    *   The duplicated pagination logic in `GetAllAsync` and `StreamAsync` will be moved to `ServiceBase` and these methods will be updated to call the new generic helpers.
*   **Unique Logic to Preserve:**
    *   `GetMeAsync` calls a special `/me` endpoint and will be preserved. Its implementation will be simplified to use the base `ExecuteGetAsync` helper.

### 3.6. `PublishingService`

*   **Methods to Consolidate:**
    *   All CRUD methods for `Episode`, `Series`, `Speaker`, `Speakership`, and `Media` entities will be refactored to use the generic `ServiceBase` helpers.
*   **Logic to Move to `ServiceBase`:**
    *   The duplicated pagination logic in `GetAllEpisodesAsync` and `StreamEpisodesAsync` will be moved to `ServiceBase`.
*   **Unique Logic to Preserve:**
    *   Custom actions like `PublishEpisodeAsync`, `UnpublishEpisodeAsync`, `DistributeEpisodeAsync`, etc., will be preserved but simplified to use the base `ExecuteAsync` method.
    *   Analytics and reporting methods (`GetEpisodeAnalyticsAsync`, `GetSeriesAnalyticsAsync`, `GeneratePublishingReportAsync`) will be preserved, with their internal API calls updated to use the new helpers where applicable.
*   **Placeholder Methods to Implement:**
    *   All placeholder methods will be fully implemented using the new `ServiceBase` helpers, ensuring complete and consistent API coverage.

### 3.7. `RegistrationsService`

*   **Methods to Consolidate:**
    *   All CRUD methods for `Signup`, `Registration`, `Attendee`, `SelectionType`, `SignupLocation`, `SignupTime`, `EmergencyContact`, `Category`, and `Campus` entities will be refactored to use the generic `ServiceBase` helpers.
*   **Logic to Move to `ServiceBase`:**
    *   The duplicated pagination logic in `GetAllSignupsAsync` and `StreamSignupsAsync` will be moved to `ServiceBase`.
*   **Unique Logic to Preserve:**
    *   Waitlist management methods (`AddToWaitlistAsync`, `RemoveFromWaitlistAsync`, `PromoteFromWaitlistAsync`) are custom actions and will be preserved, but simplified to use the base `ExecuteAsync` method.
    *   Reporting methods (`GenerateRegistrationReportAsync`, etc.) will be preserved, with their internal API calls updated to use the new helpers where applicable.
*   **Placeholder Methods to Implement:**
    *   All placeholder methods will be fully implemented using the new `ServiceBase` helpers.

### 3.8. `ServicesService`

*   **Methods to Consolidate:**
    *   The `CreatePlanAsync`, `UpdatePlanAsync`, and `DeletePlanAsync` methods already use the `ServiceBase` helpers and serve as a model. The remaining CRUD methods for `Plan`, `ServiceType`, `Item`, and `Song` will be refactored to use the generic helpers.
*   **Logic to Move to `ServiceBase`:**
    *   The duplicated pagination logic in `GetAllPlansAsync` and `StreamPlansAsync` will be moved to `ServiceBase`.
*   **Unique Logic to Preserve:**
    *   None. All methods in this service are standard CRUD or pagination operations.

### 3.9. `WebhooksService`

*   **Refactor All CRUD Methods:** All `Get...`, `List...`, `Create...`, `Update...`, and `Delete...` methods for `WebhookSubscription` and `AvailableEvent` will be refactored.
*   **Refactor Pagination:** The `GetAllSubscriptionsAsync` and `StreamSubscriptionsAsync` methods will be replaced.
*   **Implement Placeholder Methods:** The placeholder methods in this service will be fully implemented.

## 4. Mapper and Model Standardization

### 4.1. Standardize Mappers

To improve consistency, we will introduce a generic `IMapper` interface. All existing mappers will be updated to implement this interface.

### 4.2. Introduce `IPlannableResource`

To eliminate reflection and improve type safety, we will introduce an `IPlannableResource` interface with an `Id` property. All domain models will be updated to implement this interface.

## 5. Conclusion

This refactoring plan provides a clear path forward for improving the quality of the Planning Center API .NET SDK. By addressing the systemic issues of code duplication and inconsistency, we will create a more robust, maintainable, and developer-friendly SDK. This will enable us to add new features and fix bugs more quickly and with greater confidence.
