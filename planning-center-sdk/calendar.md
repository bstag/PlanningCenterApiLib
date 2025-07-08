# Planning Center Calendar API

This document outlines the resources available in the Planning Center Calendar API.

## Attachment

An `Attachment` is an uploaded file attached to an event.

**API Path:** `https://api.planningcenteronline.com/calendar/v2/attachments`

### Attributes

| Name | Type | Description |
| :--- | :--- | :--- |
| `id` | primary_key | Unique identifier for the attachment |
| `content_type` | string | MIME type of the attachment |
| `created_at` | date_time | UTC time at which the attachment was created |
| `description` | string | Description of the attachment |
| `file_size` | integer | File size in bytes |
| `name` | string | Set to the file name if not provided |
| `updated_at` | date_time | UTC time at which the attachment was updated |
| `url` | string | Path to where the attachment is stored |

### Relationships

*   **event**: A `to_one` relationship to the `Event` the attachment belongs to.

### Endpoints

*   **Get an event for an attachment:**
    `GET https://api.planningcenteronline.com/calendar/v2/attachments/{attachment_id}/event`
*   **List attachments for an event:**
    `GET https://api.planningcenteronline.com/calendar/v2/events/{event_id}/attachments`
*   **List all attachments:**
    `GET https://api.planningcenteronline.com/calendar/v2/attachments`

### URL Parameters

*   **Include:**
    *   `include=event`: Include the associated event.
*   **Order By:**
    *   `content_type`, `created_at`, `description`, `file_size`, `name`, `updated_at` (prefix with `-` to reverse order).
*   **Query By:**
    *   `where[content_type]`, `where[created_at]`, `where[description]`, `where[file_size]`, `where[name]`, `where[updated_at]`.
*   **Pagination:**
    *   `per_page`: Number of records per page (min: 1, max: 100, default: 25).
    *   `offset`: Get results from a given offset.

### Example Object

```json
{
  "type": "Attachment",
  "id": "1",
  "attributes": {
    "content_type": "string",
    "created_at": "2000-01-01T12:00:00Z",
    "description": "string",
    "file_size": 1,
    "name": "string",
    "updated_at": "2000-01-01T12:00:00Z",
    "url": "string"
  },
  "relationships": {
    "event": {
      "data": {
        "type": "Event",
        "id": "1"
      }
    }
  }
}
```

---

## Conflict

A conflict between two events caused by overlapping event resource requests.

If the conflict has been resolved, `resolved_at` will be present.

**API Path:** `https://api.planningcenteronline.com/calendar/v2/conflicts`

### Attributes

| Name | Type | Description |
| :--- | :--- | :--- |
| `id` | primary_key | Unique identifier for the conflict |
| `created_at` | date_time | UTC time at which the conflict was created |
| `note` | string | Additional information about the conflict or resolution |
| `resolved_at` | date_time | UTC time at which the conflict was resolved |
| `updated_at` | date_time | UTC time at which the conflict was updated |

### Relationships

| Name | Type | Association |
| :--- | :--- | :--- |
| `resource` | Resource | to_one |
| `resolved_by` | Person | to_one |
| `winner` | Event | to_one |

### Endpoints

**Outbound (from a Conflict)**

*   `/calendar/v2/conflicts/{conflict_id}/resolved_by`: Get the `Person` who resolved the conflict.
*   `/calendar/v2/conflicts/{conflict_id}/resource`: Get the `Resource` associated with the conflict.
*   `/calendar/v2/conflicts/{conflict_id}/winner`: Get the winning `Event` of the conflict.

**Inbound (to get Conflicts)**

*   `/calendar/v2/events/{event_id}/conflicts`: Get conflicts for a specific `Event`.
*   `/calendar/v2/conflicts`: Get conflicts for the `Organization`.
    *   **Filters:** `future`, `resolved`, `unresolved`
*   `/calendar/v2/resources/{resource_id}/conflicts`: Get conflicts for a specific `Resource`.

### Query Parameters

*   **Include:** `resolved_by`, `resource`, `winner`
*   **Order:** `resolved_at` (prefix with `-` to reverse order)
*   **Pagination:** `per_page` (min: 1, max: 100, default: 25), `offset`

### Example Object

```json
{
  "type": "Conflict",
  "id": "1",
  "attributes": {
    "created_at": "2000-01-01T12:00:00Z",
    "note": "string",
    "resolved_at": "2000-01-01T12:00:00Z",
    "updated_at": "2000-01-01T12:00:00Z"
  },
  "relationships": {
    "resource": {
      "data": {
        "type": "Resource",
        "id": "1"
      }
    },
    "resolved_by": {
      "data": {
        "type": "Person",
        "id": "1"
      }
    },
    "winner": {
      "data": {
        "type": "Event",
        "id": "1"
      }
    }
  }
}
```

---

## Event

An event. May contain information such as who owns the event, visibility on Church Center and a public-facing summary.

**API Path:** `https://api.planningcenteronline.com/calendar/v2/events`

### Attributes

| Name | Type | Description |
| :--- | :--- | :--- |
| `id` | primary_key | Unique identifier for the event |
| `approval_status` | string | Possible values: `A` (approved), `P` (pending), `R` (rejected). |
| `created_at` | date_time | UTC time at which the event was created |
| `description` | string | A rich text public-facing summary of the event |
| `featured` | boolean | `true` indicates the event is featured on Church Center, `false` indicates it is not |
| `image_url` | string | Path to where the event image is stored |
| `name` | string | The name of the event |
| `percent_approved` | integer | Calculated percentage of approved future `ReservationBlocks` |
| `percent_rejected` | integer | Calculated percentage of rejected future `ReservationBlocks` |
| `registration_url` | string | The registration URL for the event |
| `summary` | string | A plain text public-facing summary of the event |
| `updated_at` | date_time | UTC time at which the event was updated |
| `visible_in_church_center` | boolean | `true` indicates 'Published', `false` indicates 'Hidden' |

### Relationships

*   **owner**: A `to_one` relationship to a `Person` graph type.

### Endpoints

*   **List all events:**
    `GET https://api.planningcenteronline.com/calendar/v2/events`
*   **Get a specific event:**
    `GET https://api.planningcenteronline.com/calendar/v2/events/{event_id}`

### URL Parameters

*   **Include:**
    *   `include=attachments`: Include associated attachments.
    *   `include=feed`: Include associated feed.
    *   `include=owner`: Include associated owner.
    *   `include=tags`: Include associated tags.
*   **Order By:**
    *   `created_at`, `name`, `updated_at` (prefix with `-` to reverse order).
*   **Query By:**
    *   `where[approval_status]`, `where[created_at]`, `where[featured]`, `where[name]`, `where[percent_approved]`, `where[percent_rejected]`, `where[updated_at]`, `where[visible_in_church_center]`.
*   **Pagination:**
    *   `per_page`: Number of records per page (min: 1, max: 100, default: 25).
    *   `offset`: Get results from a given offset.

### Example Object

```json
{
  "type": "Event",
  "id": "1",
  "attributes": {
    "approval_status": "string",
    "created_at": "2000-01-01T12:00:00Z",
    "description": "string",
    "featured": true,
    "image_url": "string",
    "name": "string",
    "percent_approved": 1,
    "percent_rejected": 1,
    "registration_url": "string",
    "summary": "string",
    "updated_at": "2000-01-01T12:00:00Z",
    "visible_in_church_center": true
  },
  "relationships": {
    "owner": {
      "data": {
        "type": "Person",
        "id": "1"
      }
    }
  }
}
```

---

## EventConnection

A connection between a Calendar event and a record in another product.

**API Path:** `https://api.planningcenteronline.com/calendar/v2/events/{event_id}/event_connections`

### Attributes

| Name | Type | Description |
| :--- | :--- | :--- |
| `id` | `primary_key` | The unique identifier for the event connection. |
| `connected_to_id` | `primary_key` | Unique identifier for the connected record. |
| `connected_to_name` | `string` | Name of the record that the event is connected to. |
| `connected_to_type` | `string` | Type of connected record. Supports `signup`, `group`, `event`, and `service_type`. |
| `product_name` | `string` | The product the record belongs to. Supports `registrations`, `groups`, `check-ins`, and `services`. |
| `connected_to_url` | `string` | A link to the connected record. |

### Relationships

| Name | Type | Association | Description |
| :--- | :--- | :--- | :--- |
| `event` | `Event`| `to_one` | The event this connection belongs to. |

### Permissions

*   **Create, Update, Destroy:** Allowed.
*   **Assignable on Create/Update:** `connected_to_id`, `connected_to_name`, `connected_to_type`, `product_name`.

### Query Parameters

*   **Filtering:**
    *   `where[product_name]` (string): Query on a specific `product_name`.
*   **Pagination:**
    *   `per_page` (integer): Records per page (min: 1, max: 100, default: 25).
    *   `offset` (integer): Get results from a given offset.

---

## EventInstance

A specific occurrence of an event.

If the event is recurring, `recurrence` will be set and `recurrence_description` will provide an overview of the recurrence pattern.

**API Path:** `https://api.planningcenteronline.com/calendar/v2/event_instances`

### Attributes

| Name | Type | Description |
| :--- | :--- | :--- |
| `id` | primary_key | Unique identifier for the event instance |
| `all_day_event` | boolean | Indicates whether event instance lasts all day |
| `compact_recurrence_description` | string | Compact representation of event instance's recurrence pattern |
| `created_at` | date_time | UTC time at which the event instance was created |
| `ends_at` | date_time | UTC time at which the event instance ends |
| `location` | string | Representation of where the event instance takes place |
| `name` | string | Name of event. Can be overridden for specific instances |
| `recurrence` | string | For a recurring event instance, the interval of how often the event instance occurs |
| `recurrence_description` | string | Longer description of the event instance's recurrence pattern |
| `starts_at` | date_time | UTC time at which the event instance starts |
| `updated_at` | date_time | UTC time at which the event instance was updated |
| `church_center_url` | string | The URL for the event on Church Center |
| `published_starts_at` | string | Publicly visible start time |
| `published_ends_at` | string | Publicly visible end time |

### Relationships

| Name | Type | Association |
| :--- | :--- | :--- |
| `event` | Event | to_one |

### Endpoints

*   **List all event instances:**
    `GET https://api.planningcenteronline.com/calendar/v2/event_instances`
*   **Get a specific event instance:**
    `GET https://api.planningcenteronline.com/calendar/v2/event_instances/{event_instance_id}`
*   **List event instances for an event:**
    `GET https://api.planningcenteronline.com/calendar/v2/events/{event_id}/event_instances`

### URL Parameters

*   **Include:**
    *   `include=event`: Include the associated event.
    *   `include=event_times`: Include associated event_times.
    *   `include=resource_bookings`: Include associated resource_bookings.
    *   `include=tags`: Include associated tags.
*   **Order By:**
    *   `created_at`, `ends_at`, `starts_at`, `updated_at` (prefix with `-` to reverse order).
*   **Query By:**
    *   `where[created_at]`, `where[ends_at]`, `where[starts_at]`, `where[updated_at]`.
*   **Pagination:**
    *   `per_page`: Number of records per page (min: 1, max: 100, default: 25).
    *   `offset`: Get results from a given offset.

### Example Object

```json
{
  "type": "EventInstance",
  "id": "1",
  "attributes": {
    "all_day_event": true,
    "compact_recurrence_description": "string",
    "created_at": "2000-01-01T12:00:00Z",
    "ends_at": "2000-01-01T12:00:00Z",
    "location": "string",
    "name": "string",
    "recurrence": "string",
    "recurrence_description": "string",
    "starts_at": "2000-01-01T12:00:00Z",
    "updated_at": "2000-01-01T12:00:00Z",
    "church_center_url": "string",
    "published_starts_at": "string",
    "published_ends_at": "string"
  },
  "relationships": {
    "event": {
      "data": {
        "type": "Event",
        "id": "1"
      }
    }
  }
}
```

---

## EventResourceAnswer

An answer to a question in a room or resource request.

**API Path:** `https://api.planningcenteronline.com/calendar/v2/event_resource_requests/{event_resource_request_id}/answers`

### Attributes

| Name | Type | Description |
| :--- | :--- | :--- |
| `id` | primary_key | |
| `answer` | json | The answer formatted for display |
| `db_answer` | string | Only available when requested with the `?fields` param |
| `created_at` | date_time | |
| `updated_at` | date_time | |
| `question` | json | Question details as of when it was answered |

### Relationships

| Name | Type | Association |
| :--- | :--- | :--- |
| `created_by` | Person | to_one |
| `updated_by` | Person | to_one |
| `resource_question` | ResourceQuestion | to_one |
| `event_resource_request` | EventResourceRequest | to_one |

### Endpoints

*   **List answers for an event resource request:**
    `GET https://api.planningcenteronline.com/calendar/v2/event_resource_requests/{event_resource_request_id}/answers`

### Query Parameters

*   **Pagination:**
    *   `per_page`: how many records to return per page (min=1, max=100, default=25)
    *   `offset`: get results from given offset

### Example Object

```json
{
  "type": "EventResourceAnswer",
  "id": "1",
  "attributes": {
    "answer": {},
    "db_answer": "string",
    "created_at": "2000-01-01T12:00:00Z",
    "updated_at": "2000-01-01T12:00:00Z",
    "question": {}
  },
  "relationships": {
    "created_by": {
      "data": {
        "type": "Person",
        "id": "1"
      }
    },
    "updated_by": {
      "data": {
        "type": "Person",
        "id": "1"
      }
    },
    "resource_question": {
      "data": {
        "type": "ResourceQuestion",
        "id": "1"
      }
    },
    "event_resource_request": {
      "data": {
        "type": "EventResourceRequest",
        "id": "1"
      }
    }
  }
}
```

---

## EventResourceRequest

A room or resource request for a specific event.

**API Path:** `https://api.planningcenteronline.com/calendar/v2/event_resource_requests`

### Attributes

| Name | Type | Description |
| :--- | :--- | :--- |
| `id` | primary_key | Unique identifier for the request |
| `approval_sent` | boolean | Whether or not an email has been sent to request approval |
| `approval_status` | string | Possible values: `A` (approved), `P` (pending), `R` (rejected) |
| `created_at` | date_time | UTC time at which request was created |
| `updated_at` | date_time | UTC time at which request was updated |
| `notes` | string | Additional information about the room or resource request |
| `quantity` | integer | How many of the rooms or resources are being requested |

### Relationships

| Name | Type | Association |
| :--- | :--- | :--- |
| `event` | Event | to_one |
| `resource` | Resource | to_one |
| `event_resource_request` | EventResourceRequest | to_one |
| `created_by` | Person | to_one |
| `updated_by` | Person | to_one |
| `room_setup` | RoomSetup | to_one |

### Endpoints

*   **List all event resource requests:**
    `GET https://api.planningcenteronline.com/calendar/v2/event_resource_requests`
*   **Get a specific event resource request:**
    `GET https://api.planningcenteronline.com/calendar/v2/event_resource_requests/{event_resource_request_id}`
*   **List event resource requests for an event:**
    `GET https://api.planningcenteronline.com/calendar/v2/events/{event_id}/event_resource_requests`
*   **List event resource requests for a resource:**
    `GET https://api.planningcenteronline.com/calendar/v2/resources/{resource_id}/event_resource_requests`

### URL Parameters

*   **Include:**
    *   `include=created_by`
    *   `include=event`
    *   `include=resource`
    *   `include=room_setup`
    *   `include=updated_by`
*   **Order By:**
    *   `created_at`, `updated_at` (prefix with `-` to reverse order)
*   **Query By:**
    *   `where[approval_sent]`, `where[approval_status]`, `where[created_at]`, `where[updated_at]`
*   **Pagination:**
    *   `per_page`: Number of records per page (min: 1, max: 100, default: 25).
    *   `offset`: Get results from a given offset.

### Example Object

```json
{
  "type": "EventResourceRequest",
  "id": "1",
  "attributes": {
    "approval_sent": true,
    "approval_status": "string",
    "created_at": "2000-01-01T12:00:00Z",
    "updated_at": "2000-01-01T12:00:00Z",
    "notes": "string",
    "quantity": 1
  },
  "relationships": {
    "event": { "data": { "type": "Event", "id": "1" } },
    "resource": { "data": { "type": "Resource", "id": "1" } },
    "event_resource_request": { "data": { "type": "EventResourceRequest", "id": "1" } },
    "created_by": { "data": { "type": "Person", "id": "1" } },
    "updated_by": { "data": { "type": "Person", "id": "1" } },
    "room_setup": { "data": { "type": "RoomSetup", "id": "1" } }
  }
}
```

---

## Feed

A feed belonging to an organization.

**API Path:** `https://api.planningcenteronline.com/calendar/v2/feeds`

### Attributes

| Name | Type | Description |
| :--- | :--- | :--- |
| `id` | primary_key | |
| `can_delete` | boolean | |
| `default_church_center_visibility` | string | Possible values: `hidden` or `published` |
| `feed_type` | string | Possible values: `registrations`, `groups`, `ical`, or `form` |
| `imported_at` | date_time | |
| `name` | string | |
| `deleting` | boolean | |
| `sync_campus_tags` | boolean | Only available when requested with the `?fields` param |
| `source_id` | primary_key | |

### Relationships

*   **event_owner**: A `to_one` relationship to a `Person`.

### Endpoints

*   **List all feeds:**
    `GET https://api.planningcenteronline.com/calendar/v2/feeds`
*   **Get a specific feed:**
    `GET https://api.planningcenteronline.com/calendar/v2/feeds/{feed_id}`

### URL Parameters

*   **Order By:**
    *   `name` (prefix with `-` to reverse order).
*   **Query By:**
    *   `where[feed_type]` (string): Query on a specific `feed_type` (Possible values: `registrations`, `groups`, `ical`, or `form`).
    *   `where[source_id]` (primary_key): Query on a specific `source_id`.
*   **Pagination:**
    *   `per_page`: Number of records per page (min: 1, max: 100, default: 25).
    *   `offset`: Get results from a given offset.

### Example Object

```json
{
  "type": "Feed",
  "id": "1",
  "attributes": {
    "can_delete": true,
    "default_church_center_visibility": "value",
    "feed_type": "value",
    "imported_at": "2000-01-01T12:00:00Z",
    "name": "string",
    "deleting": true,
    "sync_campus_tags": true,
    "source_id": "primary_key"
  },
  "relationships": {
    "event_owner": {
      "data": {
        "type": "Person",
        "id": "1"
      }
    }
  }
}
```

---

## JobStatus

**API Path:** `https://api.planningcenteronline.com/calendar/v2/job_statuses`

### Attributes

| Name | Type | Description |
| :--- | :--- | :--- |
| `id` | primary_key | |
| `retries` | integer | |
| `errors` | json | |
| `message` | string | |
| `started_at` | date_time | |
| `status` | string | |

### Relationships

(None)

### Endpoints

*   **List all job statuses:**
    `GET https://api.planningcenteronline.com/calendar/v2/job_statuses`
*   **Get a specific job status:**
    `GET https://api.planningcenteronline.com/calendar/v2/job_statuses/{job_status_id}`

### URL Parameters

*   **Pagination:**
    *   `per_page`: how many records to return per page (min=1, max=100, default=25)
    *   `offset`: get results from given offset

### Example Object

```json
{
  "type": "JobStatus",
  "id": "1",
  "attributes": {
    "retries": 1,
    "errors": {},
    "message": "string",
    "started_at": "2000-01-01T12:00:00Z",
    "status": "string"
  },
  "relationships": {}
}
```

---

## Organization

An administrative structure, usually representing a single church.
Contains date/time formatting and time zone preferences.

**API Path:** `https://api.planningcenteronline.com/calendar/v2`

### Attributes

| Name | Type | Description |
| :--- | :--- | :--- |
| `id` | primary_key | Unique identifier for the organization |
| `name` | string | The name of the organization |
| `time_zone` | string | The time zone of the organization |
| `twenty_four_hour_time` | boolean | - `true` indicates hours for times will use a 24-hour clock - `false` indicates hours for times will use a 12-hour clock |
| `date_format` | string | Possible values: - `%d/%m/%Y`: indicates date/month/year formatting - `%m/%d/%Y`: indicates month/date/year formatting |
| `onboarding` | boolean | Only available when requested with the `?fields` param |
| `calendar_starts_on` | string | The day of the week the calendar starts on |

### Relationships

(None)

### Endpoints

*   **Get the current organization:**
    `GET https://api.planningcenteronline.com/calendar/v2`

### URL Parameters

*   **Pagination:**
    *   `per_page`: how many records to return per page (min=1, max=100, default=25)
    *   `offset`: get results from given offset

### Example Object

```json
{
  "type": "Organization",
  "id": "1",
  "attributes": {
    "name": "string",
    "time_zone": "string",
    "twenty_four_hour_time": true,
    "date_format": "string",
    "onboarding": true,
    "calendar_starts_on": "string"
  },
  "relationships": {}
}
```

---

## Person

The people in your organization with access to Calendar.

**API Path:** `https://api.planningcenteronline.com/calendar/v2/people`

### Attributes

| Name | Type | Description |
| :--- | :--- | :--- |
| `id` | primary_key | Unique identifier for the person |
| `created_at` | date_time | UTC time at which the person was created |
| `first_name` | string | The person's first name |
| `last_name` | string | The person's last name |
| `middle_name` | string | The person's middle name |
| `updated_at` | date_time | UTC time at which the person was updated |
| `avatar_url` | string | Path to where the avatar image is stored |
| `child` | boolean | Indicates whether the person is a child |
| `contact_data` | string | An object containing the person's contact data. This can include an array of `email_addresses`, `addresses` and `phone_numbers` |
| `gender` | string | `M` indicates male, `F` indicates female |
| `has_access` | boolean | Indicates whether the person has access to Calendar |
| `name_prefix` | string | Possible values: - `Mr.` - `Mrs.` - `Ms.` - `Miss` - `Dr.` - `Rev.` |
| `name_suffix` | string | Possible values: - `Jr.` - `Sr.` - `Ph.D.` - `II` - `III` |
| `pending_request_count` | integer | If the person is a member of an approval group, the number of EventResourceRequests needing resolution. If `resolves_conflicts` is `true`, the count will also include the number of Conflicts needing resolution. |
| `permissions` | integer | Integer that corresponds to the person's permissions in Calendar |
| `resolves_conflicts` | boolean | Indicates whether the person is able to resolve Conflicts |
| `site_administrator` | boolean | Indicates whether the person is a Organization Administrator |
| `status` | string | Possible values: - `active`: The person is marked "active" in People - `inactive`: The person is marked "inactive" in People Possible values: `active`, `pending`, or `inactive` |
| `can_edit_people` | boolean | Indicates whether the person can edit other people |
| `can_edit_resources` | boolean | Indicates whether the person can edit resources |
| `can_edit_rooms` | boolean | Indicates whether the person can edit rooms |
| `event_permissions_type` | string | Event permissions for the person |
| `member_of_approval_groups` | boolean | Indicates whether the person is a member of at least one approval group Only available when requested with the `?fields` param |
| `name` | string | The person's first name, last name, and name suffix |
| `people_permissions_type` | string | People permissions for the person |
| `room_permissions_type` | string | Room permissions for the person |
| `resources_permissions_type` | string | Resource permissions for the person |

### Relationships

(None)

### Endpoints

*   **List all people:**
    `GET https://api.planningcenteronline.com/calendar/v2/people`
*   **Get a specific person:**
    `GET https://api.planningcenteronline.com/calendar/v2/people/{person_id}`

### URL Parameters

*   **Include:**
    *   `include=organization`: Include the associated organization.
*   **Order By:**
    *   `created_at`, `first_name`, `last_name`, `resolves_conflicts`, `updated_at` (prefix with `-` to reverse order).
*   **Query By:**
    *   `where[created_at]`, `where[first_name]`, `where[last_name]`, `where[middle_name]`, `where[updated_at]`.
*   **Pagination:**
    *   `per_page`: Number of records per page (min: 1, max: 100, default: 25).
    *   `offset`: Get results from a given offset.

### Example Object

```json
{
  "type": "Person",
  "id": "1",
  "attributes": {
    "created_at": "2000-01-01T12:00:00Z",
    "first_name": "string",
    "last_name": "string",
    "middle_name": "string",
    "updated_at": "2000-01-01T12:00:00Z",
    "avatar_url": "string",
    "child": true,
    "contact_data": "string",
    "gender": "string",
    "has_access": true,
    "name_prefix": "string",
    "name_suffix": "string",
    "pending_request_count": 1,
    "permissions": 1,
    "resolves_conflicts": true,
    "site_administrator": true,
    "status": "value",
    "can_edit_people": true,
    "can_edit_resources": true,
    "can_edit_rooms": true,
    "event_permissions_type": "string",
    "member_of_approval_groups": true,
    "name": "string",
    "people_permissions_type": "string",
    "room_permissions_type": "string",
    "resources_permissions_type": "string"
  },
  "relationships": {}
}
```

---

## ReportTemplate

A template for generating a report.

**API Path:** `https://api.planningcenteronline.com/calendar/v2/report_templates`

### Attributes

| Name | Type | Description |
| :--- | :--- | :--- |
| `id` | primary_key | Unique identifier for the report |
| `body` | string | The contents of the report template |
| `created_at` | string | UTC time at which the report was created |
| `description` | string | A summarization of the report |
| `title` | string | The title of the report |
| `updated_at` | string | UTC time at which the report was updated |

### Relationships

(None)

### Endpoints

*   **List all report templates:**
    `GET https://api.planningcenteronline.com/calendar/v2/report_templates`
*   **Get a specific report template:**
    `GET https://api.planningcenteronline.com/calendar/v2/report_templates/{report_template_id}`

### URL Parameters

*   **Order By:**
    *   `created_at`, `title`, `updated_at` (prefix with `-` to reverse order).
*   **Query By:**
    *   `where[created_at]`, `where[title]`, `where[updated_at]`.
*   **Pagination:**
    *   `per_page`: how many records to return per page (min=1, max=100, default=25).
    *   `offset`: get results from given offset.

### Example Object

```json
{
  "type": "ReportTemplate",
  "id": "1",
  "attributes": {
    "body": "string",
    "created_at": "string",
    "description": "string",
    "title": "string",
    "updated_at": "string"
  },
  "relationships": {}
}
```

---

## RequiredApproval

Represents the relationship between a Resource and a Resource Approval Group.

**API Path:** `https://api.planningcenteronline.com/calendar/v2/resource_approval_groups/{resource_approval_group_id}/required_approvals`

### Attributes

| Name | Type | Description |
| :--- | :--- | :--- |
| `id` | primary_key | |

### Relationships

*   **resource**: A `to_one` relationship to a `Resource`.

### Endpoints

*   **List all required approvals for a resource approval group:**
    `GET https://api.planningcenteronline.com/calendar/v2/resource_approval_groups/{resource_approval_group_id}/required_approvals`
*   **Get a specific required approval:**
    `GET https://api.planningcenteronline.com/calendar/v2/resource_approval_groups/{resource_approval_group_id}/required_approvals/{required_approval_id}`
*   **List required approvals for a resource:**
    `GET https://api.planningcenteronline.com/calendar/v2/resources/{resource_id}/required_approvals`
*   **List required approvals for an event:**
    `GET https://api.planningcenteronline.com/calendar/v2/events/{event_id}/required_approvals`

### URL Parameters

*   **Include:**
    *   `include=resource`: Include the associated resource.
*   **Pagination:**
    *   `per_page`: how many records to return per page (min=1, max=100, default=25)
    *   `offset`: get results from given offset

### Example Object

```json
{
  "type": "RequiredApproval",
  "id": "1",
  "attributes": {},
  "relationships": {}
}
```

---

## Resource

A room or resource that can be requested for use as part of an event.

**API Path:** `https://api.planningcenteronline.com/calendar/v2/resources`

### Attributes

| Name | Type | Description |
| :--- | :--- | :--- |
| `id` | primary_key | Unique identifier for the room or resource |
| `created_at` | date_time | UTC time at which the room or resource was created |
| `kind` | string | The type of resource, can either be `Room` or `Resource` |
| `name` | string | The name of the room or resource |
| `serial_number` | string | The serial number of the resource |
| `updated_at` | date_time | UTC time at which the room or resource was updated |
| `description` | string | Description of the room or resource |
| `expires_at` | date_time | UTC time at which the resource expires |
| `home_location` | string | Where the resource is normally kept |
| `image` | string | Path to where resource image is stored |
| `quantity` | integer | The quantity of the resource |
| `path_name` | string | A string representing the location of the resource if it is nested within a folder (e.g., `Folder1/Subfolder`) |

### Relationships

*   **resource_approval_groups**: A `to_many` relationship to a list of resource approval groups.
*   **resource_folder**: A `to_one` relationship to the resource folder this resource belongs to.
*   **resource_questions**: A `to_many` relationship to a list of resource questions.
*   **room_setups**: A `to_many` relationship to a list of room setups.

### Endpoints

*   **List all resources:**
    `GET https://api.planningcenteronline.com/calendar/v2/resources`
*   **Get a specific resource:**
    `GET https://api.planningcenteronline.com/calendar/v2/resources/{resource_id}`
*   **List resources for a resource folder:**
    `GET https://api.planningcenteronline.com/calendar/v2/resource_folders/{resource_folder_id}/resources`
*   **List resources for a resource approval group:**
    `GET https://api.planningcenteronline.com/calendar/v2/resource_approval_groups/{resource_approval_group_id}/resources`

### URL Parameters

*   **Include:**
    *   `include=resource_approval_groups`: Include associated resource_approval_groups.
    *   `include=resource_folder`: Include associated resource_folder.
    *   `include=resource_questions`: Include associated resource_questions.
    *   `include=room_setups`: Include associated room_setups.
*   **Order By:**
    *   `created_at`, `expires_at`, `name`, `updated_at` (prefix with `-` to reverse order).
*   **Query By:**
    *   `where[created_at]`, `where[kind]`, `where[name]`, `where[path_name]`, `where[serial_number]`, `where[updated_at]`.
*   **Pagination:**
    *   `per_page`: Number of records per page (min: 1, max: 100, default: 25).
    *   `offset`: Get results from a given offset.

### Example Object

```json
{
  "type": "Resource",
  "id": "1",
  "attributes": {
    "created_at": "2000-01-01T12:00:00Z",
    "kind": "string",
    "name": "string",
    "serial_number": "string",
    "updated_at": "2000-01-01T12:00:00Z",
    "description": "string",
    "expires_at": "2000-01-01T12:00:00Z",
    "home_location": "string",
    "image": "string",
    "quantity": 1,
    "path_name": "string"
  },
  "relationships": {}
}
```

---

## ResourceApprovalGroup

A group of people that can be attached to a room or resource in order to require their approval for booking.

**API Path:** `https://api.planningcenteronline.com/calendar/v2/resource_approval_groups`

### Attributes

| Name | Type | Description |
| :--- | :--- | :--- |
| `id` | primary_key | Unique identifier for the approval group |
| `created_at` | date_time | UTC time at which the approval group was created |
| `name` | string | Name of the approval group |
| `updated_at` | date_time | UTC time at which the approval group was updated |
| `form_count` | integer | Only available when requested with the `?fields` param |
| `resource_count` | integer | The number of resources in the approval group Only available when requested with the `?fields` param |
| `room_count` | integer | The number of rooms in the approval group Only available when requested with the `?fields` param |

### Relationships

*   **people**: A `to_many` relationship to a list of people in the approval group.
*   **resources**: A `to_many` relationship to a list of resources in the approval group.

### Endpoints

*   **List all resource approval groups:**
    `GET https://api.planningcenteronline.com/calendar/v2/resource_approval_groups`
*   **Get a specific resource approval group:**
    `GET https://api.planningcenteronline.com/calendar/v2/resource_approval_groups/{resource_approval_group_id}`

### URL Parameters

*   **Include:**
    *   `include=people`: Include associated people.
    *   `include=resources`: Include associated resources.
*   **Order By:**
    *   `created_at`, `name`, `updated_at` (prefix with `-` to reverse order).
*   **Query By:**
    *   `where[created_at]`, `where[id]`, `where[name]`, `where[updated_at]`.
*   **Pagination:**
    *   `per_page`: Number of records per page (min: 1, max: 100, default: 25).
    *   `offset`: Get results from a given offset.

### Example Object

```json
{
  "type": "ResourceApprovalGroup",
  "id": "1",
  "attributes": {
    "created_at": "2000-01-01T12:00:00Z",
    "name": "string",
    "updated_at": "2000-01-01T12:00:00Z",
    "form_count": 1,
    "resource_count": 1,
    "room_count": 1
  },
  "relationships": {}
}
```

---

## ResourceBooking

A specific booking of a room or resource for an event instance.

**API Path:** `https://api.planningcenteronline.com/calendar/v2/resource_bookings`

### Attributes

| Name | Type | Description |
| :--- | :--- | :--- |
| `id` | primary_key | Unique identifier for the booking |
| `created_at` | date_time | UTC time at which the booking was created |
| `ends_at` | date_time | UTC time at which usage of the booked room or resource ends |
| `starts_at` | date_time | UTC time at which usage of the booked room or resource starts |
| `updated_at` | date_time | UTC time at which the booking was updated |
| `quantity` | integer | The quantity of the rooms or resources booked |

### Relationships

*   **event**: A `to_one` relationship to the associated event.
*   **event_instance**: A `to_one` relationship to the associated event instance.
*   **resource**: A `to_one` relationship to the associated resource.

### Endpoints

*   **List all resource bookings:**
    `GET https://api.planningcenteronline.com/calendar/v2/resource_bookings`
*   **Get a specific resource booking:**
    `GET https://api.planningcenteronline.com/calendar/v2/resource_bookings/{resource_booking_id}`
*   **List resource bookings for an event instance:**
    `GET https://api.planningcenteronline.com/calendar/v2/event_instances/{event_instance_id}/resource_bookings`
*   **List resource bookings for a resource:**
    `GET https://api.planningcenteronline.com/calendar/v2/resources/{resource_id}/resource_bookings`

### URL Parameters

*   **Include:**
    *   `include=event_instance`: Include the associated event_instance.
    *   `include=event_resource_request`: Include the associated event_resource_request.
    *   `include=resource`: Include the associated resource.
*   **Order By:**
    *   `created_at`, `ends_at`, `starts_at`, `updated_at` (prefix with `-` to reverse order).
*   **Query By:**
    *   `where[created_at]`, `where[ends_at]`, `where[starts_at]`, `where[updated_at]`.
*   **Pagination:**
    *   `per_page`: how many records to return per page (min=1, max=100, default=25).
    *   `offset`: get results from given offset.

### Example Object

```json
{
  "type": "ResourceBooking",
  "id": "1",
  "attributes": {
    "created_at": "2000-01-01T12:00:00Z",
    "ends_at": "2000-01-01T12:00:00Z",
    "starts_at": "2000-01-01T12:00:00Z",
    "updated_at": "2000-01-01T12:00:00Z",
    "quantity": 1
  },
  "relationships": {
    "event": {
      "data": {
        "type": "Event",
        "id": "1"
      }
    },
    "event_instance": {
      "data": {
        "type": "EventInstance",
        "id": "1"
      }
    },
    "resource": {
      "data": {
        "type": "Resource",
        "id": "1"
      }
    }
  }
}
```

---

## ResourceFolder

An organizational folder containing rooms or resources.

**API Path:** `https://api.planningcenteronline.com/calendar/v2/resource_folders`

### Attributes

| Name | Type | Description |
| :--- | :--- | :--- |
| `id` | primary_key | Unique identifier for the folder |
| `created_at` | date_time | UTC time at which the folder was created |
| `name` | string | The folder name |
| `updated_at` | date_time | UTC time at which the folder was updated |
| `ancestry` | string | |
| `kind` | string | The type of folder, can either be `Room` or `Resource` |
| `path_name` | string | A string representing the location of the folder if it is nested. Each parent folder is separated by `/` |

### Relationships

*   **resources**: A `to_many` relationship to a list of resources in the folder.

### Endpoints

*   **List all resource folders:**
    `GET https://api.planningcenteronline.com/calendar/v2/resource_folders`
*   **Get a specific resource folder:**
    `GET https://api.planningcenteronline.com/calendar/v2/resource_folders/{resource_folder_id}`

### URL Parameters

*   **Include:**
    *   `include=resources`: Include associated resources.
*   **Order By:**
    *   `ancestry`, `created_at`, `name`, `updated_at` (prefix with `-` to reverse order).
*   **Query By:**
    *   `where[ancestry]`, `where[created_at]`, `where[name]`, `where[path_name]`, `where[updated_at]`.
*   **Pagination:**
    *   `per_page`: Number of records per page (min: 1, max: 100, default: 25).
    *   `offset`: Get results from a given offset.

### Example Object

```json
{
  "type": "ResourceFolder",
  "id": "1",
  "attributes": {
    "created_at": "2000-01-01T12:00:00Z",
    "name": "string",
    "updated_at": "2000-01-01T12:00:00Z",
    "ancestry": "string",
    "kind": "string",
    "path_name": "string"
  },
  "relationships": {}
}
```

---

## ResourceQuestion

A question to answer when requesting to book a room or resource.

**API Path:** `https://api.planningcenteronline.com/calendar/v2/resource_questions`

### Attributes

| Name | Type | Description |
| :--- | :--- | :--- |
| `id` | primary_key | Unique identifier for the question |
| `created_at` | date_time | UTC time at which the question was created |
| `kind` | string | Possible values: - `dropdown`: predefined list of choices as an answer - `paragraph`: expected answer is a paragraph - `text`: expected answer is a sentence - `yesno`: expected answer is 'Yes' or 'No' - `section_header`: used to separate questions in the UI, no expected answer |
| `updated_at` | date_time | UTC time at which the question was updated |
| `choices` | string | If `kind` is `dropdown`, represents a string of dropdown choices separated by the `|` character |
| `description` | string | Optional description of the question |
| `multiple_select` | boolean | If `kind` is `dropdown`, `true` indicates that more than one selection is permitted |
| `optional` | boolean | - `true` indicates answering the question is not required when booking - `false` indicates answering the question is required when booking |
| `position` | integer | Position of question in list in the UI |
| `question` | string | The question to be answered |

### Relationships

*   **resource**: A `to_one` relationship to the `Resource` this question belongs to.

### Endpoints

*   **List all resource questions:**
    `GET https://api.planningcenteronline.com/calendar/v2/resource_questions`
*   **Get a specific resource question:**
    `GET https://api.planningcenteronline.com/calendar/v2/resource_questions/{resource_question_id}`
*   **List resource questions for a resource:**
    `GET https://api.planningcenteronline.com/calendar/v2/resources/{resource_id}/resource_questions`

### URL Parameters

*   **Order By:**
    *   `created_at`, `updated_at` (prefix with `-` to reverse order).
*   **Query By:**
    *   `where[created_at]`, `where[kind]`, `where[updated_at]`.
*   **Pagination:**
    *   `per_page`: Number of records per page (min: 1, max: 100, default: 25).
    *   `offset`: Get results from a given offset.

### Example Object

```json
{
  "type": "ResourceQuestion",
  "id": "1",
  "attributes": {
    "created_at": "2000-01-01T12:00:00Z",
    "kind": "string",
    "updated_at": "2000-01-01T12:00:00Z",
    "choices": "string",
    "description": "string",
    "multiple_select": true,
    "optional": true,
    "position": 1,
    "question": "string"
  },
  "relationships": {
    "resource": {
      "data": {
        "type": "Resource",
        "id": "1"
      }
    }
  }
}
```

---

## ResourceSuggestion

A resource and quantity suggested by a room setup.

**API Path:** `https://api.planningcenteronline.com/calendar/v2/room_setups/{room_setup_id}/resource_suggestions`

### Attributes

| Name | Type | Description |
| :--- | :--- | :--- |
| `id` | primary_key | Unique identifier for the suggestion |
| `created_at` | date_time | UTC time at which the suggestion was created |
| `quantity` | integer | How many resources should be requested |
| `updated_at` | date_time | UTC time at which the suggestion was updated |

### Relationships

*   **resource**: A `to_one` relationship to a `Resource`.
*   **room_setup**: A `to_one` relationship to a `RoomSetup`.

### Endpoints

*   **List all resource suggestions for a room setup:**
    `GET https://api.planningcenteronline.com/calendar/v2/room_setups/{room_setup_id}/resource_suggestions`
*   **Get a specific resource suggestion:**
    `GET https://api.planningcenteronline.com/calendar/v2/room_setups/{room_setup_id}/resource_suggestions/{resource_suggestion_id}`

### URL Parameters

*   **Include:**
    *   `include=resource`: Include the associated resource.
*   **Pagination:**
    *   `per_page`: how many records to return per page (min=1, max=100, default=25).
    *   `offset`: get results from given offset.

### Example Object

```json
{
  "type": "ResourceSuggestion",
  "id": "1",
  "attributes": {
    "created_at": "2000-01-01T12:00:00Z",
    "quantity": 1,
    "updated_at": "2000-01-01T12:00:00Z"
  },
  "relationships": {
    "resource": {
      "data": {
        "type": "Resource",
        "id": "1"
      }
    },
    "room_setup": {
      "data": {
        "type": "RoomSetup",
        "id": "1"
      }
    }
  }
}
```

---

## Tag

An `Tag` is an organizational tag that can be applied to events.

**API Path:** `https://api.planningcenteronline.com/calendar/v2/tags`

### Attributes

| Name | Type | Description |
| :--- | :--- | :--- |
| `id` | primary_key | Unique identifier for the tag |
| `church_center_category` | boolean | `true` indicates that this tag is used as a category on Church Center |
| `color` | string | Hex color code of the tag |
| `created_at` | date_time | UTC time at which the tag was created |
| `name` | string | The tag name |
| `position` | float | If the tag belongs to a TagGroup, position indicates place in list within TagGroup in the UI. If the tag does not belong to a TagGroup, position indicates place in list under "Individual Tags" in the UI. |
| `updated_at` | date_time | UTC time at which the tag was updated |

### Relationships

*   **tag_group**: A `to_one` relationship to the `TagGroup` the tag belongs to.

### Endpoints

*   **List all tags:**
    `GET https://api.planningcenteronline.com/calendar/v2/tags`
*   **Get a specific tag:**
    `GET https://api.planningcenteronline.com/calendar/v2/tags/{tag_id}`
*   **List tags for an event:**
    `GET https://api.planningcenteronline.com/calendar/v2/events/{event_id}/tags`
*   **List tags for an event instance:**
    `GET https://api.planningcenteronline.com/calendar/v2/event_instances/{event_instance_id}/tags`
*   **List tags for a tag group:**
    `GET https://api.planningcenteronline.com/calendar/v2/tag_groups/{tag_group_id}/tags`

### URL Parameters

*   **Include:**
    *   `include=tag_group`: Include the associated tag_group.
*   **Order By:**
    *   `name`, `position` (prefix with `-` to reverse order).
*   **Query By:**
    *   `where[church_center_category]`, `where[color]`, `where[created_at]`, `where[id]`, `where[name]`, `where[position]`, `where[updated_at]`.
*   **Pagination:**
    *   `per_page`: Number of records per page (min: 1, max: 100, default: 25).
    *   `offset`: Get results from a given offset.

### Example Object

```json
{
  "type": "Tag",
  "id": "1",
  "attributes": {
    "church_center_category": true,
    "color": "string",
    "created_at": "2000-01-01T12:00:00Z",
    "name": "string",
    "position": 1.42,
    "updated_at": "2000-01-01T12:00:00Z"
  },
  "relationships": {
    "tag_group": {
      "data": {
        "type": "TagGroup",
        "id": "1"
      }
    }
  }
}
```

---

## TagGroup

A grouping of tags for organizational purposes.

**API Path:** `https://api.planningcenteronline.com/calendar/v2/tag_groups`

### Attributes

| Name | Type | Description |
| :--- | :--- | :--- |
| `id` | primary_key | Unique identifier for the tag group |
| `created_at` | date_time | UTC time at which the tag group was created |
| `name` | string | The name of the tag group |
| `updated_at` | date_time | UTC time at which the tag group was updated |
| `required` | boolean | - `true` indicates tag from this tag group must be applied when creating an event |

### Relationships

*   **events**: A `to_many` relationship to a list of events in the tag group.
*   **tags**: A `to_many` relationship to a list of tags in the tag group.

### Endpoints

*   **List all tag groups:**
    `GET https://api.planningcenteronline.com/calendar/v2/tag_groups`
*   **Get a specific tag group:**
    `GET https://api.planningcenteronline.com/calendar/v2/tag_groups/{tag_group_id}`

### URL Parameters

*   **Include:**
    *   `include=events`: Include associated events.
    *   `include=tags`: Include associated tags.
*   **Order By:**
    *   `name` (prefix with `-` to reverse order).
*   **Query By:**
    *   `where[created_at]`, `where[name]`, `where[updated_at]`.
*   **Pagination:**
    *   `per_page`: Number of records per page (min: 1, max: 100, default: 25).
    *   `offset`: Get results from a given offset.

### Example Object

```json
{
  "type": "TagGroup",
  "id": "1",
  "attributes": {
    "created_at": "2000-01-01T12:00:00Z",
    "name": "string",
    "updated_at": "2000-01-01T12:00:00Z",
    "required": true
  },
  "relationships": {}
}
```

---

## Tag

An `Tag` is an organizational tag that can be applied to events.

**API Path:** `https://api.planningcenteronline.com/calendar/v2/tags`

### Attributes

| Name | Type | Description |
| :--- | :--- | :--- |
| `id` | primary_key | Unique identifier for the tag |
| `church_center_category` | boolean | `true` indicates that this tag is used as a category on Church Center |
| `color` | string | Hex color code of the tag |
| `created_at` | date_time | UTC time at which the tag was created |
| `name` | string | The tag name |
| `position` | float | If the tag belongs to a TagGroup, position indicates place in list within TagGroup in the UI. If the tag does not belong to a TagGroup, position indicates place in list under "Individual Tags" in the UI. |
| `updated_at` | date_time | UTC time at which the tag was updated |

### Relationships

*   **tag_group**: A `to_one` relationship to the `TagGroup` the tag belongs to.

### Endpoints

*   **List all tags:**
    `GET https://api.planningcenteronline.com/calendar/v2/tags`
*   **Get a specific tag:**
    `GET https://api.planningcenteronline.com/calendar/v2/tags/{tag_id}`
*   **List tags for an event:**
    `GET https://api.planningcenteronline.com/calendar/v2/events/{event_id}/tags`
*   **List tags for an event instance:**
    `GET https://api.planningcenteronline.com/calendar/v2/event_instances/{event_instance_id}/tags`
*   **List tags for a tag group:**
    `GET https://api.planningcenteronline.com/calendar/v2/tag_groups/{tag_group_id}/tags`

### URL Parameters

*   **Include:**
    *   `include=tag_group`: Include the associated tag_group.
*   **Order By:**
    *   `name`, `position` (prefix with `-` to reverse order).
*   **Query By:**
    *   `where[church_center_category]`, `where[color]`, `where[created_at]`, `where[id]`, `where[name]`, `where[position]`, `where[updated_at]`.
*   **Pagination:**
    *   `per_page`: Number of records per page (min: 1, max: 100, default: 25).
    *   `offset`: Get results from a given offset.

### Example Object

```json
{
  "type": "Tag",
  "id": "1",
  "attributes": {
    "church_center_category": true,
    "color": "string",
    "created_at": "2000-01-01T12:00:00Z",
    "name": "string",
    "position": 1.42,
    "updated_at": "2000-01-01T12:00:00Z"
  },
  "relationships": {
    "tag_group": {
      "data": {
        "type": "TagGroup",
        "id": "1"
      }
    }
  }
}
```

---

## RoomSetup

A diagram and list of suggested resources useful for predefined room setups.

**API Path:** `https://api.planningcenteronline.com/calendar/v2/room_setups`

### Attributes

| Name | Type | Description |
| :--- | :--- | :--- |
| `id` | primary_key | Unique identifier for the room setup |
| `created_at` | date_time | UTC time at which the room setup was created |
| `name` | string | The name of the room setup |
| `updated_at` | date_time | UTC time at which the room setup was updated |
| `description` | string | A description of the room setup |
| `diagram` | string | An object containing `url` and `thumbnail`. `url` is path to where room setup is stored. `thumbnail` contains `url` path to where thumbnail is stored. |
| `diagram_url` | string | Path to where room setup is stored |
| `diagram_thumbnail_url` | string | Path to where thumbnail version of room setup is stored |

### Relationships

*   **room_setup**: A `to_one` relationship to the shared room setup this is linked to.
*   **resource_suggestions**: A `to_many` relationship to a list of suggested resources for this room setup.
*   **containing_resource**: A `to_one` relationship to the resource this room setup is attached to.

### Endpoints

*   **List all room setups:**
    `GET https://api.planningcenteronline.com/calendar/v2/room_setups`
*   **Get a specific room setup:**
    `GET https://api.planningcenteronline.com/calendar/v2/room_setups/{room_setup_id}`
*   **List room setups for a resource:**
    `GET https://api.planningcenteronline.com/calendar/v2/resources/{resource_id}/room_setups`

### URL Parameters

*   **Include:**
    *   `include=containing_resource`: Include the associated containing_resource.
    *   `include=resource_suggestions`: Include associated resource_suggestions.
*   **Order By:**
    *   `created_at`, `name`, `updated_at` (prefix with `-` to reverse order).
*   **Query By:**
    *   `where[created_at]`, `where[name]`, `where[updated_at]`.
*   **Pagination:**
    *   `per_page`: Number of records per page (min: 1, max: 100, default: 25).
    *   `offset`: Get results from a given offset.

### Example Object

```json
{
  "type": "RoomSetup",
  "id": "1",
  "attributes": {
    "created_at": "2000-01-01T12:00:00Z",
    "name": "string",
    "updated_at": "2000-01-01T12:00:00Z",
    "description": "string",
    "diagram": "string",
    "diagram_url": "string",
    "diagram_thumbnail_url": "string"
  },
  "relationships": {
    "room_setup": {
      "data": {
        "type": "RoomSetup",
        "id": "1"
      }
    },
    "resource_suggestions": {
      "data": [
        {
          "type": "ResourceSuggestion",
          "id": "1"
        }
      ]
    },
    "containing_resource": {
      "data": {
        "type": "Resource",
        "id": "1"
      }
    }
  }
}
```
