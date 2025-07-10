# Groups Models

This folder contains C# models specific to the Planning Center Groups API.

## Attendance

Individual event attendance for a person.

### Attributes

*   `Id`: The unique identifier for the attendance.
*   `Attended`: Whether or not the person attended the event.
*   `Role`: The role of the person at the time of event (e.g., `member`, `leader`).

### Endpoints

*   **Groups API:** `https://api.planningcenteronline.com/groups/v2/events/{event_id}/attendances`

## Campus

A campus as defined in Planning Center Accounts.

### Attributes

*   `Id`: The unique identifier for the campus.
*   `Name`: The name of the campus.

### Endpoints

*   **Groups API:** `https://api.planningcenteronline.com/groups/v2/campuses`

## Enrollment

Details on how and when members can join a `Group`.

### Attributes

*   `Id`: The unique identifier for the enrollment.
*   `AutoClosed`: Whether or not enrollment has been closed automatically due to set limits.
*   `AutoClosedReason`: Brief description as to which limit automatically closed enrollment.
*   `DateLimit`: Date when enrollment should automatically close.
*   `DateLimitReached`: Whether or not the `date_limit` has been reached.
*   `MemberLimit`: Total number of members allowed before enrollment should automatically close.
*   `MemberLimitReached`: Whether or not the `member_limit` has been reached.
*   `Status`: The status of the enrollment.
*   `Strategy`: The strategy for enrollment.

### Endpoints

*   **Groups API:** `https://api.planningcenteronline.com/groups/v2/groups/{group_id}/enrollment`

## Event

An event is a meeting of a group. It has a start and end time, and can be either physical or virtual.

### Attributes

*   `Id`: The unique identifier for the event.
*   `AttendanceRequestsEnabled`: This is a group setting that applies to all the events in the group.
*   `AutomatedReminderEnabled`: If `true`, we send an event remind some specified time before the event starts to all members in the group.
*   `Canceled`: Whether or not the event is canceled.
*   `CanceledAt`: The date and time the event was canceled.
*   `Description`: The description of the event.
*   `EndsAt`: The date and time the event ends.
*   `LocationTypePreference`: The location type preference for the event.
*   `MultiDay`: Whether or not the event spans multiple days.
*   `Name`: The name of the event.
*   `RemindersSent`: Whether or not reminders have been sent for the event.
*   `RemindersSentAt`: The date and time reminders were sent for the event.
*   `Repeating`: Whether or not the event is repeating.
*   `StartsAt`: The date and time the event starts.
*   `VirtualLocationUrl`: The URL for the virtual location of the event.
*   `VisitorsCount`: The number of visitors for the event.

### Endpoints

*   **Groups API:** `https://api.planningcenteronline.com/groups/v2/events`

## EventNote

Notes that group leaders can write for an event, generally related to attendance.

### Attributes

*   `Id`: The unique identifier for the event note.
*   `Body`: The body text of the note.

### Endpoints

*   **Groups API:** `https://api.planningcenteronline.com/groups/v2/events/{event_id}/notes`

## Group

A group of people that meet together regularly.

### Attributes

*   `Id`: The unique identifier for the group.
*   `ArchivedAt`: The date and time the group was archived.
*   `CanCreateConversation`: A boolean representing the current user's authorization to start a new conversation in the group.
*   `ChatEnabled`: A boolean representing whether or not the group has Chat enabled.
*   `ContactEmail`: The contact email for the group.
*   `CreatedAt`: The date and time the group was created.
*   `Description`: The description of the group.
*   `EventsVisibility`: The visibility of events for the group.
*   `HeaderImage`: The header image for the group (JSON object).
*   `LeadersCanSearchPeopleDatabase`: Indicates if leaders can search the people database.
*   `LocationTypePreference`: The location type preference for the group.
*   `MembersAreConfidential`: Indicates if members are confidential.
*   `MembershipsCount`: The number of memberships in the group.
*   `Name`: The name of the group.
*   `PublicChurchCenterWebUrl`: The public Church Center web URL for the group.
*   `Schedule`: The schedule of the group.
*   `TagIds`: The tag IDs associated with the group.
*   `VirtualLocationUrl`: The URL for the virtual location of the group.
*   `WidgetStatus`: The widget status of the group (JSON object).

### Endpoints

*   **Groups API:** `https://api.planningcenteronline.com/groups/v2/groups`

## GroupApplication

A group application is a request to join a group which can be approved or rejected.

### Attributes

*   `Id`: The unique identifier for the group application.
*   `AppliedAt`: Timestamp when this person applied.
*   `Message`: An optional personal message from the applicant.
*   `Status`: The approval status of the application (e.g., `pending`, `approved`, or `rejected`).

### Endpoints

*   **Groups API:** `https://api.planningcenteronline.com/groups/v2/group_applications`

## GroupType

A group type is a category of groups.

### Attributes

*   `Id`: The unique identifier for the group type.
*   `ChurchCenterVisible`: `true` if the group type contains any published groups. Otherwise `false`.
*   `ChurchCenterMapVisible`: `true` if the map view is visible on the public groups list page. Otherwise `false`.
*   `Color`: Hex color value.
*   `DefaultGroupSettings`: A JSON object of default settings for groups of this type.
*   `Description`: The description of the group type.
*   `Name`: The name of the group type.
*   `Position`: The position of the group type in relation to other group types.

### Endpoints

*   **Groups API:** `https://api.planningcenteronline.com/groups/v2/group_types`

## Location

A physical event location.

### Attributes

*   `Id`: The unique identifier for the location.
*   `DisplayPreference`: This preference controls how the location is displayed to non-members for public groups and events.
*   `FullFormattedAddress`: Ex: "1313 Disneyland Dr
Anaheim, CA 92802" (may be approximate or `null`).
*   `Latitude`: Ex: `33.815396` (may be approximate or `null`).
*   `Longitude`: Ex: `-117.926399` (may be approximate or `null`).
*   `Name`: Ex: "Disneyland".
*   `Radius`: The radius of the location.
*   `Strategy`: The strategy for the location.

### Endpoints

*   **Groups API:** `https://api.planningcenteronline.com/groups/v2/locations`

## Membership

The state of a `Person` belonging to a `Group`.

### Attributes

*   `Id`: The unique identifier for the membership.
*   `JoinedAt`: The date and time the person joined the group.
*   `Role`: The role of the person in the group (e.g., `member` or `leader`).

### Endpoints

*   **Groups API:** `https://api.planningcenteronline.com/groups/v2/groups/{group_id}/memberships`

## Organization

The organization represents a single church. Every other resource is scoped to this record.

### Attributes

*   `Id`: The unique identifier for an organization.
*   `Name`: The name of the organization.
*   `TimeZone`: The time zone of the organization.

### Endpoints

*   **Groups API:** `https://api.planningcenteronline.com/groups/v2`

## Owner

The owner/creator of an event note.

### Attributes

*   `Id`: The unique identifier for the owner.
*   `AvatarUrl`: The URL of the person's avatar.
*   `FirstName`: The person's first name.
*   `LastName`: The person's last name.

### Endpoints

*   **Groups API:** `https://api.planningcenteronline.com/groups/v2/events/{event_id}/notes/{note_id}/owner`

## Person

A person is a user of Planning Center.

### Attributes

*   `Id`: The unique identifier for the person.
*   `Addresses`: All the addresses associated with this person.
*   `AvatarUrl`: The URL of the person's avatar.
*   `Child`: Whether or not the person is under 13 years old.
*   `CreatedAt`: Date and time this person was first created in Planning Center.
*   `EmailAddresses`: All the email addresses associated with this person.
*   `FirstName`: The person's first name.
*   `LastName`: The person's last name.
*   `Permissions`: The permissions of the person.
*   `PhoneNumbers`: All the phone numbers associated with this person.

### Endpoints

*   **Groups API:** `https://api.planningcenteronline.com/groups/v2/people`

## Resource

A file or link resource that can be shared with a group.

### Attributes

*   `Id`: The unique identifier for the resource.
*   `Description`: The description of the resource.
*   `LastUpdated`: The date and time the resource was last updated.
*   `Name`: The name/title of the resource.
*   `Type`: Either `FileResource` or `LinkResource`.
*   `Visibility`: Possible values: `leaders` or `members`.

### Endpoints

*   **Groups API:** `https://api.planningcenteronline.com/groups/v2/group_types/{group_type_id}/resources`

## Tag

Tags are used to filter groups.

### Attributes

*   `Id`: The unique identifier for the tag.
*   `Name`: The name of the tag.
*   `Position`: The position of the tag in relation to other tags.

### Endpoints

*   **Groups API:** `https://api.planningcenteronline.com/groups/v2/tags`

## TagGroup

A way to group related tags.

### Attributes

*   `Id`: The unique identifier for the tag group.
*   `DisplayPublicly`: Whether or not this tag group is visible to the public on Church Center.
*   `MultipleOptionsEnabled`: Whether or not a group can belong to many tags within this tag group.
*   `Name`: The name of the tag group.
*   `Position`: The position of the tag group in relation to other tag groups.

### Endpoints

*   **Groups API:** `https://api.planningcenteronline.com/groups/v2/tag_groups`
