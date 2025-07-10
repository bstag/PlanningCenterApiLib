# Calendar Models

This folder contains C# models specific to the Planning Center Calendar API.

## Attachment

An uploaded file attached to an event.

### Attributes

*   `Id`: Unique identifier for the attachment.
*   `ContentType`: MIME type of the attachment.
*   `CreatedAt`: UTC time at which the attachment was created.
*   `Description`: Description of the attachment.
*   `FileSize`: File size in bytes.
*   `Name`: Name of the attachment.
*   `UpdatedAt`: UTC time at which the attachment was updated.
*   `Url`: URL of the attachment.

### Endpoints

*   **Calendar API:** `https://api.planningcenteronline.com/calendar/v2/attachments`

## Conflict

A conflict between two events caused by overlapping event resource requests.

### Attributes

*   `Id`: Unique identifier for the conflict.
*   `CreatedAt`: UTC time at which the conflict was created.
*   `Note`: Additional information about the conflict or resolution.
*   `ResolvedAt`: UTC time at which the conflict was resolved.
*   `UpdatedAt`: UTC time at which the conflict was updated.

### Endpoints

*   **Calendar API:** `https://api.planningcenteronline.com/calendar/v2/conflicts`

## Event

An event. May contain information such as who owns the event, visibility on Church Center and a public-facing summary.

### Attributes

*   `Id`: Unique identifier for the event.
*   `ApprovalStatus`: Possible values: `A` (approved), `P` (pending), `R` (rejected).
*   `CreatedAt`: UTC time at which the event was created.
*   `Description`: A rich text public-facing summary of the event.
*   `Featured`: `true` indicates the event is featured on Church Center, `false` indicates it is not.
*   `ImageUrl`: Path to where the event image is stored.
*   `Name`: The name of the event.
*   `PercentApproved`: Calculated percentage of approved future `ReservationBlocks`.
*   `PercentRejected`: Calculated percentage of rejected future `ReservationBlocks`.
*   `RegistrationUrl`: The registration URL for the event.
*   `Summary`: A plain text public-facing summary of the event.
*   `UpdatedAt`: UTC time at which the event was updated.
*   `VisibleInChurchCenter`: `true` indicates 'Published', `false` indicates 'Hidden'.

### Endpoints

*   **Calendar API:** `https://api.planningcenteronline.com/calendar/v2/events`

## EventConnection

A connection between a Calendar event and a record in another product.

### Attributes

*   `Id`: Unique identifier for the connected record.
*   `ConnectedToId`: Unique identifier for the connected record.
*   `ConnectedToName`: Name of the record that the event is connected to.
*   `ConnectedToType`: Currently we support `signup`, `group`, `event`, and `service_type`.
*   `ProductName`: Currently we support `registrations`, `groups`, `check-ins`, and `services`.
*   `ConnectedToUrl`: A link to the connected record.

### Endpoints

*   **Calendar API:** `https://api.planningcenteronline.com/calendar/v2/events/{event_id}/event_connections`

## EventInstance

A specific occurrence of an event.

### Attributes

*   `Id`: Unique identifier for the event instance.
*   `AllDayEvent`: Indicates whether event instance lasts all day.
*   `CompactRecurrenceDescription`: Compact representation of event instance's recurrence pattern.
*   `CreatedAt`: UTC time at which the event instance was created.
*   `EndsAt`: UTC time at which the event instance ends.
*   `Location`: Representation of where the event instance takes place.
*   `Name`: Name of event. Can be overridden for specific instances.
*   `Recurrence`: For a recurring event instance, the interval of how often the event instance occurs.
*   `RecurrenceDescription`: Longer description of the event instance's recurrence pattern.
*   `StartsAt`: UTC time at which the event instance starts.
*   `UpdatedAt`: UTC time at which the event instance was updated.
*   `ChurchCenterUrl`: The URL for the event on Church Center.
*   `PublishedStartsAt`: Publicly visible start time.
*   `PublishedEndsAt`: Publicly visible end time.

### Endpoints

*   **Calendar API:** `https://api.planningcenteronline.com/calendar/v2/event_instances`

## EventResourceAnswer

An answer to a question in a room or resource request.

### Attributes

*   `Id`: Unique identifier for the answer.
*   `Answer`: The answer formatted for display (JSON object).
*   `DbAnswer`: The raw answer from the database.
*   `CreatedAt`: UTC time at which the answer was created.
*   `UpdatedAt`: UTC time at which the answer was updated.
*   `Question`: Question details as of when it was answered (JSON object).

### Endpoints

*   **Calendar API:** `https://api.planningcenteronline.com/calendar/v2/event_resource_requests/{event_resource_request_id}/answers`

## EventResourceRequest

A room or resource request for a specific event.

### Attributes

*   `Id`: Unique identifier for the request.
*   `ApprovalSent`: Whether or not an email has been sent to request approval.
*   `ApprovalStatus`: Possible values: `A` (approved), `P` (pending), `R` (rejected).
*   `CreatedAt`: UTC time at which request was created.
*   `UpdatedAt`: UTC time at which request was updated.
*   `Notes`: Additional information about the room or resource request.
*   `Quantity`: How many of the rooms or resources are being requested.

### Endpoints

*   **Calendar API:** `https://api.planningcenteronline.com/calendar/v2/event_resource_requests`

## EventTime

Start and end times for each event instance.

### Attributes

*   `Id`: Unique identifier for the event time.
*   `EndsAt`: UTC time at which the event time ends.
*   `StartsAt`: UTC time at which the event time starts.
*   `Name`: Name of the event time.
*   `VisibleOnKiosks`: Set to `true` if the time is visible on kiosk.
*   `VisibleOnWidgetAndIcal`: Set to `true` if the time is visible on widget or iCal.

### Endpoints

*   **Calendar API:** `https://api.planningcenteronline.com/calendar/v2/event_instances/{event_instance_id}/event_times`

## Feed

A feed belonging to an organization.

### Attributes

*   `Id`: Unique identifier for the feed.
*   `CanDelete`: Indicates if the feed can be deleted.
*   `DefaultChurchCenterVisibility`: Possible values: `hidden` or `published`.
*   `FeedType`: Possible values: `registrations`, `groups`, `ical`, or `form`.
*   `ImportedAt`: UTC time at which the feed was imported.
*   `Name`: The name of the feed.
*   `Deleting`: Indicates if the feed is being deleted.
*   `SyncCampusTags`: Indicates if campus tags are synced.
*   `SourceId`: The ID of the source.

### Endpoints

*   **Calendar API:** `https://api.planningcenteronline.com/calendar/v2/feeds`

## JobStatus

### Attributes

*   `Id`: Unique identifier for the job status.
*   `Retries`: Number of retries for the job.
*   `Errors`: Errors encountered during the job (JSON object).
*   `Message`: Message related to the job status.
*   `StartedAt`: UTC time at which the job started.
*   `Status`: The status of the job.

### Endpoints

*   **Calendar API:** `https://api.planningcenteronline.com/calendar/v2/job_statuses`

## Person

The people in your organization with access to Calendar.

### Attributes

*   `Id`: Unique identifier for the person.
*   `CreatedAt`: UTC time at which the person was created.
*   `FirstName`: The person's first name.
*   `LastName`: The person's last name.
*   `MiddleName`: The person's middle name.
*   `UpdatedAt`: UTC time at which the person was updated.
*   `AvatarUrl`: Path to where the avatar image is stored.
*   `Child`: Indicates whether the person is a child.
*   `ContactData`: An object containing the person's contact data.
*   `Gender`: `M` indicates male, `F` indicates female.
*   `HasAccess`: Indicates whether the person has access to Calendar.
*   `NamePrefix`: Possible values: `Mr.`, `Mrs.`, `Ms.`, `Miss`, `Dr.`, `Rev.`.
*   `NameSuffix`: Possible values: `Jr.`, `Sr.`, `Ph.D.`, `II`, `III`.
*   `PendingRequestCount`: If the person is a member of an approval group, the number of EventResourceRequests needing resolution.
*   `Permissions`: Integer that corresponds to the person's permissions in Calendar.
*   `ResolvesConflicts`: Indicates whether the person is able to resolve Conflicts.
*   `SiteAdministrator`: Indicates whether the person is a Organization Administrator.
*   `Status`: Possible values: `active`, `pending`, or `inactive`.
*   `CanEditPeople`: Indicates whether the person can edit other people.
*   `CanEditResources`: Indicates whether the person can edit resources.
*   `CanEditRooms`: Indicates whether the person can edit rooms.
*   `EventPermissionsType`: Event permissions for the person.
*   `MemberOfApprovalGroups`: Indicates whether the person is a member of at least one approval group.
*   `Name`: The person's first name, last name, and name suffix.
*   `PeoplePermissionsType`: People permissions for the person.
*   `RoomPermissionsType`: Room permissions for the person.
*   `ResourcesPermissionsType`: Resource permissions for the person.

### Endpoints

*   **Calendar API:** `https://api.planningcenteronline.com/calendar/v2/people`

## ReportTemplate

A template for generating a report.

### Attributes

*   `Id`: Unique identifier for the report.
*   `Body`: The contents of the report template.
*   `CreatedAt`: UTC time at which the report was created.
*   `Description`: A summarization of the report.
*   `Title`: The title of the report.
*   `UpdatedAt`: UTC time at which the report was updated.

### Endpoints

*   **Calendar API:** `https://api.planningcenteronline.com/calendar/v2/report_templates`

## RequiredApproval

Represents the relationship between a Resource and a Resource Approval Group.

### Attributes

*   `Id`: Unique identifier for the required approval.

### Endpoints

*   **Calendar API:** `https://api.planningcenteronline.com/calendar/v2/resource_approval_groups/{resource_approval_group_id}/required_approvals`

## Resource

A room or resource that can be requested for use as part of an event.

### Attributes

*   `Id`: Unique identifier for the room or resource.
*   `CreatedAt`: UTC time at which the room or resource was created.
*   `Kind`: The type of resource, can either be `Room` or `Resource`.
*   `Name`: The name of the room or resource.
*   `SerialNumber`: The serial number of the resource.
*   `UpdatedAt`: UTC time at which the room or resource was updated.
*   `Description`: Description of the room or resource.
*   `ExpiresAt`: UTC time at which the resource expires.
*   `HomeLocation`: Where the resource is normally kept.
*   `Image`: Path to where resource image is stored.
*   `Quantity`: The quantity of the resource.
*   `PathName`: A string representing the location of the resource if it is nested within a folder (e.g., `Folder1/Subfolder`).

### Endpoints

*   **Calendar API:** `https://api.planningcenteronline.com/calendar/v2/resources`

## ResourceApprovalGroup

A group of people that can be attached to a room or resource in order to require their approval for booking.

### Attributes

*   `Id`: Unique identifier for the approval group.
*   `CreatedAt`: UTC time at which the approval group was created.
*   `Name`: Name of the approval group.
*   `UpdatedAt`: UTC time at which the approval group was updated.
*   `FormCount`: Number of forms in the approval group.
*   `ResourceCount`: The number of resources in the approval group.
*   `RoomCount`: The number of rooms in the approval group.

### Endpoints

*   **Calendar API:** `https://api.planningcenteronline.com/calendar/v2/resource_approval_groups`

## ResourceBooking

A specific booking of a room or resource for an event instance.

### Attributes

*   `Id`: Unique identifier for the booking.
*   `CreatedAt`: UTC time at which the booking was created.
*   `EndsAt`: UTC time at which usage of the booked room or resource ends.
*   `StartsAt`: UTC time at which usage of the booked room or resource starts.
*   `UpdatedAt`: UTC time at which the booking was updated.
*   `Quantity`: The quantity of the rooms or resources booked.

### Endpoints

*   **Calendar API:** `https://api.planningcenteronline.com/calendar/v2/resource_bookings`

## ResourceFolder

An organizational folder containing rooms or resources.

### Attributes

*   `Id`: Unique identifier for the folder.
*   `CreatedAt`: UTC time at which the folder was created.
*   `Name`: The folder name.
*   `UpdatedAt`: UTC time at which the folder was updated.
*   `Ancestry`: The ancestry of the folder.
*   `Kind`: The type of folder, can either be `Room` or `Resource`.
*   `PathName`: A string representing the location of the folder if it is nested.

### Endpoints

*   **Calendar API:** `https://api.planningcenteronline.com/calendar/v2/resource_folders`

## ResourceQuestion

A question to answer when requesting to book a room or resource.

### Attributes

*   `Id`: Unique identifier for the question.
*   `CreatedAt`: UTC time at which the question was created.
*   `Kind`: Possible values: `dropdown`, `paragraph`, `text`, `yesno`, `section_header`.
*   `UpdatedAt`: UTC time at which the question was updated.
*   `Choices`: If `kind` is `dropdown`, represents a string of dropdown choices separated by the `|` character.
*   `Description`: Optional description of the question.
*   `MultipleSelect`: If `kind` is `dropdown`, `true` indicates that more than one selection is permitted.
*   `Optional`: `true` indicates answering the question is not required when booking, `false` indicates answering the question is required when booking.
*   `Position`: Position of question in list in the UI.
*   `Question`: The question to be answered.

### Endpoints

*   **Calendar API:** `https://api.planningcenteronline.com/calendar/v2/resource_questions`

## ResourceSuggestion

A resource and quantity suggested by a room setup.

### Attributes

*   `Id`: Unique identifier for the suggestion.
*   `CreatedAt`: UTC time at which the suggestion was created.
*   `Quantity`: How many resources should be requested.
*   `UpdatedAt`: UTC time at which the suggestion was updated.

### Endpoints

*   **Calendar API:** `https://api.planningcenteronline.com/calendar/v2/room_setups/{room_setup_id}/resource_suggestions`

## RoomSetup

A diagram and list of suggested resources useful for predefined room setups.

### Attributes

*   `Id`: Unique identifier for the room setup.
*   `CreatedAt`: UTC time at which the room setup was created.
*   `Name`: The name of the room setup.
*   `UpdatedAt`: UTC time at which the room setup was updated.
*   `Description`: A description of the room setup.
*   `Diagram`: An object containing `url` and `thumbnail`.
*   `DiagramUrl`: Path to where room setup is stored.
*   `DiagramThumbnailUrl`: Path to where thumbnail version of room setup is stored.

### Endpoints

*   **Calendar API:** `https://api.planningcenteronline.com/calendar/v2/room_setups`

## Tag

An organizational tag that can be applied to events.

### Attributes

*   `Id`: Unique identifier for the tag.
*   `ChurchCenterCategory`: `true` indicates that this tag is used as a category on Church Center.
*   `Color`: Hex color code of the tag.
*   `CreatedAt`: UTC time at which the tag was created.
*   `Name`: The tag name.
*   `Position`: If the tag belongs to a TagGroup, position indicates place in list within TagGroup in the UI. If the tag does not belong to a TagGroup, position indicates place in list under "Individual Tags" in the UI.
*   `UpdatedAt`: UTC time at which the tag was updated.

### Endpoints

*   **Calendar API:** `https://api.planningcenteronline.com/calendar/v2/tags`

## TagGroup

A grouping of tags for organizational purposes.

### Attributes

*   `Id`: Unique identifier for the tag group.
*   `CreatedAt`: UTC time at which the tag group was created.
*   `Name`: The name of the tag group.
*   `UpdatedAt`: UTC time at which the tag group was updated.
*   `Required`: `true` indicates tag from this tag group must be applied when creating an event.

### Endpoints

*   **Calendar API:** `https://api.planningcenteronline.com/calendar/v2/tag_groups`
