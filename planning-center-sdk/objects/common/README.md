# Common Models

This folder contains C# models that are used across multiple Planning Center API endpoints.

## Organization

Represents a single church organization in Planning Center.

### Attributes

*   `Id`: The unique identifier for the organization.
*   `Name`: The name of the organization.
*   `CountryCode`: The country code of the organization.
*   `DateFormat`: The date format used by the organization.
*   `TimeZone`: The time zone of the organization.
*   `ContactWebsite`: The contact website of the organization.
*   `CreatedAt`: The date and time the organization was created.
*   `AvatarUrl`: The URL of the organization's avatar.
*   `ChurchCenterSubdomain`: The Church Center subdomain for the organization.

### Endpoints

*   **People API:** `https://api.planningcenteronline.com/people/v2`
*   **Check-ins API:** `https://api.planningcenteronline.com/check-ins/v2`
