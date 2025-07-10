# Check-Ins Models

This folder contains C# models specific to the Planning Center Check-Ins API.

## CheckIn

An attendance record for an event.

### Attributes

*   `Id`: The unique identifier for the check-in.
*   `FirstName`: The first name of the person checked in.
*   `LastName`: The last name of the person checked in.
*   `MedicalNotes`: Medical notes for the person.
*   `Number`: A number associated with the check-in.
*   `SecurityCode`: A security code for the check-in.
*   `CreatedAt`: The date and time the check-in was created.
*   `UpdatedAt`: The date and time the check-in was last updated.
*   `CheckedOutAt`: The date and time the person was checked out.
*   `ConfirmedAt`: The date and time the check-in was confirmed.
*   `EmergencyContactName`: The name of the emergency contact.
*   `EmergencyContactPhoneNumber`: The phone number of the emergency contact.
*   `OneTimeGuest`: Indicates if the person is a one-time guest.
*   `Kind`: The kind of check-in (e.g., 'regular', 'guest').

### Endpoints

*   **Check-ins API:** `https://api.planningcenteronline.com/check-ins/v2/check_ins`

## AttendanceType

A kind of attendee which is tracked by headcount, not by check-in.

### Attributes

*   `Id`: The unique identifier for the attendance type.
*   `Name`: The name of the attendance type.
*   `Color`: The color associated with the attendance type.
*   `CreatedAt`: The date and time the attendance type was created.
*   `UpdatedAt`: The date and time the attendance type was last updated.
*   `Limit`: The limit for the attendance type.

### Endpoints

*   **Check-ins API:** `https://api.planningcenteronline.com/check-ins/v2/attendance_types`

## CheckInGroup

When one or more people check in, they're grouped in a `CheckInGroup`.

### Attributes

*   `Id`: The unique identifier for the check-in group.
*   `NameLabelsCount`: The number of name labels.
*   `SecurityLabelsCount`: The number of security labels.
*   `CheckInsCount`: The number of check-ins in this group.
*   `CreatedAt`: The date and time the check-in group was created.
*   `UpdatedAt`: The date and time the check-in group was last updated.
*   `PrintStatus`: The print status of the group (e.g., 'ready', 'printed', 'canceled', 'skipped').

### Endpoints

*   **Check-ins API:** `https://api.planningcenteronline.com/check-ins/v2/check_in_groups`

## CheckInTime

A CheckInTime combines an EventTime and a Location, and associates it with the parent CheckIn.

### Attributes

*   `Id`: The unique identifier for the check-in time.
*   `Kind`: The kind of check-in time.
*   `HasValidated`: Indicates if the check-in time has been validated.
*   `ServicesIntegrated`: Indicates if services are integrated.
*   `Alerts`: A list of alerts.

### Endpoints

*   **Check-ins API:** `https://api.planningcenteronline.com/check-ins/v2/check_ins/{check_in_id}/check_in_times`

## Event

A recurring event which people may attend.

### Attributes

*   `Id`: The unique identifier for the event.
*   `Name`: The name of the event.
*   `Frequency`: The frequency of the event.
*   `EnableServicesIntegration`: Indicates if services integration is enabled.
*   `CreatedAt`: The date and time the event was created.
*   `UpdatedAt`: The date and time the event was last updated.
*   `ArchivedAt`: The date and time the event was archived.
*   `IntegrationKey`: The integration key for the event.
*   `LocationTimesEnabled`: Indicates if location times are enabled.
*   `PreSelectEnabled`: Indicates if pre-select is enabled.
*   `AppSource`: The source application for the event.

### Endpoints

*   **Check-ins API:** `https://api.planningcenteronline.com/check-ins/v2/events`

## EventLabel

Says how many of a given label to print for this event and whether to print it for regulars, guests, and/or volunteers.

### Attributes

*   `Id`: The unique identifier for the event label.
*   `Quantity`: The quantity of labels to print.
*   `ForRegular`: Indicates if the label is for regulars.
*   `ForGuest`: Indicates if the label is for guests.
*   `ForVolunteer`: Indicates if the label is for volunteers.
*   `CreatedAt`: The date and time the event label was created.
*   `UpdatedAt`: The date and time the event label was last updated.

### Endpoints

*   **Check-ins API:** `https://api.planningcenteronline.com/check-ins/v2/events/{event_id}/event_labels`

## EventPeriod

A recurrence of an event, sometimes called a "session".

### Attributes

*   `Id`: The unique identifier for the event period.
*   `StartsAt`: The start date and time of the event period.
*   `EndsAt`: The end date and time of the event period.
*   `RegularCount`: The number of regular attendees.
*   `GuestCount`: The number of guest attendees.
*   `VolunteerCount`: The number of volunteer attendees.
*   `Note`: A note for the event period.
*   `CreatedAt`: The date and time the event period was created.
*   `UpdatedAt`: The date and time the event period was last updated.

### Endpoints

*   **Check-ins API:** `https://api.planningcenteronline.com/check-ins/v2/event_periods`

## Location

A place where people may check in to for a given event.

### Attributes

*   `Id`: The unique identifier for the location.
*   `Name`: The name of the location.
*   `Kind`: The kind of location (e.g., 'Folder').
*   `Opened`: Indicates if the location is opened.
*   `Questions`: Questions associated with the location.
*   `AgeMinInMonths`: Minimum age in months for the location.
*   `AgeMaxInMonths`: Maximum age in months for the location.
*   `AgeRangeBy`: How age range is determined.
*   `AgeOn`: The date on which age is calculated.
*   `ChildOrAdult`: Indicates if the location is for children or adults.
*   `EffectiveDate`: The effective date for the location.
*   `Gender`: The gender for the location.
*   `GradeMin`: Minimum grade for the location.
*   `GradeMax`: Maximum grade for the location.
*   `MaxOccupancy`: Maximum occupancy of the location.
*   `MinVolunteers`: Minimum volunteers required for the location.
*   `AttendeesPerVolunteer`: Attendees per volunteer ratio.
*   `Position`: The position of the location.
*   `UpdatedAt`: The date and time the location was last updated.
*   `CreatedAt`: The date and time the location was created.
*   `Milestone`: The milestone for the location.

### Endpoints

*   **Check-ins API:** `https://api.planningcenteronline.com/check-ins/v2/locations`

## LocationEventPeriod

Counts check-ins for a location during a certain event period.

### Attributes

*   `Id`: The unique identifier for the location event period.
*   `RegularCount`: The number of regular attendees.
*   `GuestCount`: The number of guest attendees.
*   `VolunteerCount`: The number of volunteer attendees.
*   `CreatedAt`: The date and time the location event period was created.
*   `UpdatedAt`: The date and time the location event period was last updated.

### Endpoints

*   **Check-ins API:** `https://api.planningcenteronline.com/check-ins/v2/check_ins/{check_in_id}/event_period/{event_period_id}/location_event_periods`

## LocationEventTime

Counts check-ins for a location for a given event time. This is useful for checking occupancy.

### Attributes

*   `Id`: The unique identifier for the location event time.
*   `RegularCount`: The number of regular attendees.
*   `GuestCount`: The number of guest attendees.
*   `VolunteerCount`: The number of volunteer attendees.
*   `CreatedAt`: The date and time the location event time was created.
*   `UpdatedAt`: The date and time the location event time was last updated.

### Endpoints

*   **Check-ins API:** `https://api.planningcenteronline.com/check-ins/v2/event_times/{event_time_id}/location_event_times`

## LocationLabel

Says how many of a given label to print for this location and whether to print it for regulars, guests, and/or volunteers.

### Attributes

*   `Id`: The unique identifier for the location label.
*   `Quantity`: The quantity of labels to print.
*   `ForRegular`: Indicates if the label is for regulars.
*   `ForGuest`: Indicates if the label is for guests.
*   `ForVolunteer`: Indicates if the label is for volunteers.
*   `CreatedAt`: The date and time the location label was created.
*   `UpdatedAt`: The date and time the location label was last updated.

### Endpoints

*   **Check-ins API:** `https://api.planningcenteronline.com/check-ins/v2/labels/{label_id}/location_labels`

## Option

An option which an attendee may select when checking in.

### Attributes

*   `Id`: The unique identifier for the option.
*   `Body`: The body of the option.
*   `Quantity`: The quantity associated with the option.
*   `CreatedAt`: The date and time the option was created.
*   `UpdatedAt`: The date and time the option was last updated.

### Endpoints

*   **Check-ins API:** `https://api.planningcenteronline.com/check-ins/v2/options`

## Organization

An organization which has people and events. This contains its date format & time zone preferences.

### Attributes

*   `Id`: The unique identifier for the organization.
*   `DateFormatPattern`: The date format pattern used by the organization.
*   `TimeZone`: The time zone of the organization.
*   `Name`: The name of the organization.
*   `DailyCheckIns`: The number of daily check-ins.
*   `AvatarUrl`: The URL of the organization's avatar.
*   `CreatedAt`: The date and time the organization was created.
*   `UpdatedAt`: The date and time the organization was last updated.

### Endpoints

*   **Check-ins API:** `https://api.planningcenteronline.com/check-ins/v2/organizations`

## Pass

Enables quick lookup of a person via barcode reader.

### Attributes

*   `Id`: The unique identifier for the pass.
*   `Code`: The code of the pass.
*   `Kind`: The kind of pass (e.g., 'barcode' or 'pkpass').
*   `CreatedAt`: The date and time the pass was created.
*   `UpdatedAt`: The date and time the pass was last updated.

### Endpoints

*   **Check-ins API:** `https://api.planningcenteronline.com/check-ins/v2/passes`

## Person

An attendee, volunteer or administrator.

### Attributes

*   `Id`: The unique identifier for the person.
*   `Addresses`: A list of addresses associated with the person.
*   `EmailAddresses`: A list of email addresses associated with the person.
*   `PhoneNumbers`: A list of phone numbers associated with the person.
*   `AvatarUrl`: The URL of the person's avatar.
*   `NamePrefix`: The person's name prefix.
*   `FirstName`: The person's first name.
*   `MiddleName`: The person's middle name.
*   `LastName`: The person's last name.
*   `NameSuffix`: The person's name suffix.
*   `Birthdate`: The person's birthdate.
*   `Grade`: The person's grade.
*   `Gender`: The person's gender.
*   `MedicalNotes`: Medical notes for the person.
*   `Child`: Indicates if the person is a child.
*   `Permission`: The person's permission level.
*   `Headcounter`: Indicates if the person is a headcounter.
*   `LastCheckedInAt`: The date and time the person last checked in.
*   `CheckInCount`: The number of check-ins for the person.
*   `CreatedAt`: The date and time the person record was created.
*   `UpdatedAt`: The date and time the person record was last updated.
*   `PassedBackgroundCheck`: Indicates if the person passed a background check.
*   `DemographicAvatarUrl`: URL for a demographic avatar.
*   `Name`: The person's full name.
*   `TopPermission`: The person's top permission level.
*   `IgnoreFilters`: Indicates if filters should be ignored for this person.

### Endpoints

*   **Check-ins API:** `https://api.planningcenteronline.com/check-ins/v2/people`

## PersonEvent

Counts a person's attendance for a given event.

### Attributes

*   `Id`: The unique identifier for the person event.
*   `CheckInCount`: The number of check-ins for the person in this event.
*   `UpdatedAt`: The date and time the person event was last updated.
*   `CreatedAt`: The date and time the person event was created.

### Endpoints

*   **Check-ins API:** `https://api.planningcenteronline.com/check-ins/v2/events/{event_id}/person_events`

## PreCheck

### Attributes

*   `Id`: The unique identifier for the pre-check.

### Endpoints

*   **Check-ins API:** `https://api.planningcenteronline.com/check-ins/v2`

## RosterListPerson

### Attributes

*   `Id`: The unique identifier for the roster list person.
*   `FirstName`: The first name of the person.
*   `LastName`: The last name of the person.
*   `Name`: The full name of the person.
*   `DemographicAvatarUrl`: The URL of the demographic avatar.
*   `Grade`: The grade of the person.
*   `Gender`: The gender of the person.
*   `MedicalNotes`: Medical notes for the person.
*   `Birthdate`: The birthdate of the person.

### Endpoints

*   **Check-ins API:** `https://api.planningcenteronline.com/check-ins/v2`

## Station

A device where people can be checked in.

### Attributes

*   `Id`: The unique identifier for the station.
*   `Online`: Indicates if the station is online.
*   `Mode`: The mode of the station.
*   `Name`: The name of the station.
*   `TimeoutSeconds`: The timeout in seconds for the station.
*   `InputType`: The input type of the station (e.g., 'scanner', 'keypad').
*   `InputTypeOptions`: The input type options for the station.
*   `CreatedAt`: The date and time the station was created.
*   `UpdatedAt`: The date and time the station was last updated.
*   `NextShowsAt`: The next show time for the station.
*   `OpenForCheckIn`: Indicates if the station is open for check-in.
*   `ClosesAt`: The closing time for the station.
*   `CheckInCount`: The number of check-ins for the station.

### Endpoints

*   **Check-ins API:** `https://api.planningcenteronline.com/check-ins/v2/stations`

## Theme

A custom style which may be applied to stations.

### Attributes

*   `Id`: The unique identifier for the theme.
*   `ImageThumbnail`: The URL of the image thumbnail.
*   `Name`: The name of the theme.
*   `Color`: The color of the theme.
*   `TextColor`: The text color of the theme.
*   `Image`: The URL of the image.
*   `CreatedAt`: The date and time the theme was created.
*   `UpdatedAt`: The date and time the theme was last updated.
*   `BackgroundColor`: The background color of the theme.
*   `Mode`: The mode of the theme.

### Endpoints

*   **Check-ins API:** `https://api.planningcenteronline.com/check-ins/v2/themes`
