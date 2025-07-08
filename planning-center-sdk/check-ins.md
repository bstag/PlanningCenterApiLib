# Planning Center Check-Ins API

This document outlines the resources available in the Planning Center Check-Ins API (Version 2025-05-28).

## AttendanceType

A kind of attendee which is tracked by _headcount_, not by check-in.

**API Path:** `https://api.planningcenteronline.com/check-ins/v2/attendance_types`

### Attributes

| Name | Type | Description |
| :--- | :--- | :--- |
| `id` | primary_key | |
| `name` | string | |
| `color` | string | |
| `created_at` | date_time | |
| `updated_at` | date_time | |
| `limit` | integer | |

### Relationships

*   **event**: A `to_one` relationship to the `Event` the attendance type belongs to.

### Endpoints

*   **List all attendance types:**
    `GET https://api.planningcenteronline.com/check-ins/v2/attendance_types`
*   **Get a specific attendance type:**
    `GET https://api.planningcenteronline.com/check-ins/v2/attendance_types/{attendance_type_id}`
*   **List attendance types for an event:**
    `GET https://api.planningcenteronline.com/check-ins/v2/events/{event_id}/attendance_types`

### URL Parameters

*   **Include:**
    *   `include=event`: Include the associated event.
*   **Query By:**
    *   `where[id]`, `where[name]`.
*   **Pagination:**
    *   `per_page`: Number of records per page (min: 1, max: 100, default: 25).
    *   `offset`: Get results from a given offset.

### Example Object

```json
{
  "type": "AttendanceType",
  "id": "1",
  "attributes": {
    "name": "string",
    "color": "string",
    "created_at": "2000-01-01T12:00:00Z",
    "updated_at": "2000-01-01T12:00:00Z",
    "limit": 1
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

