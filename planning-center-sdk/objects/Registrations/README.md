# Registrations Models

This folder contains C# models specific to the Planning Center Registrations API.

## Attendee

An `Attendee` is a person registered for a signup.

### Attributes

*   `Id`: The unique identifier for the attendee.
*   `Complete`: Whether or not attendee has completed all necessary items.
*   `Active`: Whether or not the attendee is active.
*   `Canceled`: Whether or not the attendee is canceled.
*   `Waitlisted`: Whether or not the attendee is waitlisted.
*   `WaitlistedAt`: UTC time at which the attendee was waitlisted.
*   `CreatedAt`: UTC time at which the attendee was created.
*   `UpdatedAt`: UTC time at which the attendee was updated.

### Endpoints

*   **Registrations API:** `https://api.planningcenteronline.com/registrations/v2/signups/{signup_id}/attendees`

## Campus

A `Campus` is a location belonging to an Organization.

### Attributes

*   `Id`: The unique identifier for the campus.
*   `Name`: Name of the campus.
*   `Street`: Street address of the campus.
*   `City`: City where the campus is located.
*   `State`: State or province where the campus is located.
*   `Zip`: Zip code of the campus.
*   `Country`: Country where the campus is located.
*   `FullFormattedAddress`: Full formatted address of the campus.
*   `CreatedAt`: The date and time the campus was created.
*   `UpdatedAt`: The date and time the campus was updated.

### Endpoints

*   **Registrations API:** `https://api.planningcenteronline.com/registrations/v2/campuses`

## Category

A `Category` is a label used to group together and find signups more easily.

### Attributes

*   `Id`: The unique identifier for the category.
*   `Name`: Name of the category.
*   `CreatedAt`: The date and time the category was created.
*   `UpdatedAt`: The date and time the category was updated.

### Endpoints

*   **Registrations API:** `https://api.planningcenteronline.com/registrations/v2/categories`

## EmergencyContact

`Emergency_Contact` is the person assigned as the emergency contact for an attendee.

### Attributes

*   `Id`: The unique identifier for the emergency contact.
*   `Name`: The name of the emergency contact person.
*   `PhoneNumber`: Phone number of the emergency contact person.

### Endpoints

*   **Registrations API:** `https://api.planningcenteronline.com/registrations/v2/signups/{signup_id}/attendees/{attendee_id}/emergency_contact`

## Organization

The root level `Organization` record which serves as a link to `Signup`s.

### Attributes

*   `Id`: The unique identifier for an organization.
*   `Name`: Name of the Organization.
*   `CreatedAt`: The date and time the organization was created.
*   `UpdatedAt`: The date and time the organization was updated.

### Endpoints

*   **Registrations API:** `https://api.planningcenteronline.com/registrations/v2`

## Person

### Attributes

*   `Id`: The unique identifier for the person.
*   `FirstName`: The first name of the person.
*   `LastName`: The last name of the person.
*   `Name`: The full name of the person.

### Endpoints

*   **Registrations API:** `https://api.planningcenteronline.com/registrations/v2/signups/{signup_id}/attendees/{attendee_id}/person`

## Registration

### Attributes

*   `Id`: The unique identifier for the registration.
*   `CreatedAt`: The date and time the registration was created.
*   `UpdatedAt`: The date and time the registration was updated.

### Endpoints

*   **Registrations API:** `https://api.planningcenteronline.com/registrations/v2/signups/{signup_id}/registrations`

## SelectionType

`Selection_Types` are used to present the options people register for in a signup.

### Attributes

*   `Id`: The unique identifier for the selection type.
*   `Name`: Name of the selection type.
*   `PubliclyAvailable`: Whether or not the selection type is available to the public.
*   `PriceCents`: Price of selection type in cents.
*   `PriceCurrency`: Signup currency code, example `"USD"`.
*   `PriceCurrencySymbol`: Signup currency symbol, example `"$"`.
*   `PriceFormatted`: Formatted price of the selection type.
*   `CreatedAt`: The date and time the selection type was created.
*   `UpdatedAt`: The date and time the selection type was updated.

### Endpoints

*   **Registrations API:** `https://api.planningcenteronline.com/registrations/v2/signups/{signup_id}/selection_types`

## Signup

A `Signup` is an organization signup that people can register for.

### Attributes

*   `Id`: The unique identifier for the signup.
*   `Archived`: Whether the signup is archived or not.
*   `CloseAt`: UTC time at which registration closes.
*   `Description`: Description of the signup.
*   `LogoUrl`: URL for the image used for the signup.
*   `Name`: Name of the signup.
*   `NewRegistrationUrl`: The URL for new registrations.
*   `OpenAt`: UTC time at which registration opens.
*   `CreatedAt`: The date and time the signup was created.
*   `UpdatedAt`: The date and time the signup was updated.

### Endpoints

*   **Registrations API:** `https://api.planningcenteronline.com/registrations/v2/signups`

## SignupLocation

`Signup_location` is the location of a signup.

### Attributes

*   `Id`: The unique identifier for the signup location.
*   `Name`: The name of the signup location.
*   `AddressData`: The address data of the signup location, which includes details like street, city, state, and postal code.
*   `Subpremise`: The subpremise of the signup location, such as an building or room number.
*   `Latitude`: The latitude of the signup location.
*   `Longitude`: The longitude of the signup location.
*   `LocationType`: The type of location.
*   `Url`: The URL of the location.
*   `FormattedAddress`: The formatted address of the location.
*   `FullFormattedAddress`: The full formatted address of the location.
*   `CreatedAt`: The date and time the signup location was created.
*   `UpdatedAt`: The date and time the signup location was updated.

### Endpoints

*   **Registrations API:** `https://api.planningcenteronline.com/registrations/v2/signups/{signup_id}/signup_location`

## SignupTime

`Signup_time`s are associated with a signup, which can have multiple signup times.

### Attributes

*   `Id`: The unique identifier for the signup time.
*   `StartsAt`: Start date and time of signup time.
*   `EndsAt`: End date and time of signup time.
*   `AllDay`: Whether or not the signup time is all day.
*   `CreatedAt`: The date and time the signup time was created.
*   `UpdatedAt`: The date and time the signup time was updated.

### Endpoints

*   **Registrations API:** `https://api.planningcenteronline.com/registrations/v2/signups/{signup_id}/next_signup_time`
