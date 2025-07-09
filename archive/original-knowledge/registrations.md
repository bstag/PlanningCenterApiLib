## SignupTime

### Attributes

*   `id`: The unique identifier for a signup time.
*   `starts_at`: The start date and time of the signup time.
*   `ends_at`: The end date and time of the signup time.
*   `created_at`: The date and time the signup time was created.
*   `updated_at`: The date and time the signup time was last updated.

### Relationships

*   `signup`: A `SignupTime` belongs to a `Signup`.
*   `signup_location`: A `SignupTime` belongs to a `SignupLocation`.

### Actions

(No specific actions listed for SignupTime)

### Edges (Related Resources)

*   **Outbound Edges (Resources accessible from a SignupTime):**
    *   `signup`: Access the `Signup` associated with this signup time at `https://api.planningcenteronline.com/registrations/v2/signups/{signup_id}/signup_times/{signup_time_id}/signup`.
    *   `signup_location`: Access the `SignupLocation` associated with this signup time at `https://api.planningcenteronline.com/registrations/v2/signups/{signup_id}/signup_times/{signup_time_id}/signup_location`.
*   **Inbound Edges (How to access SignupTimes from other resources):**
    *   From `Signup`: `https://api.planningcenteronline.com/registrations/v2/signups/{signup_id}/signup_times`.
    *   From `SignupLocation`: `https://api.planningcenteronline.com/registrations/v2/signup_locations/{signup_location_id}/signup_times`.

### Query Parameters

*   **`can_include`**:
    *   `signup`: Include associated `signup`.
    *   `signup_location`: Include associated `signup_location`.
*   **`can_order`**:
    *   `starts_at`: Order by `starts_at` (prefix with `-` for descending).
    *   `ends_at`: Order by `ends_at` (prefix with `-` for descending).
    *   `created_at`: Order by `created_at` (prefix with `-` for descending).
    *   `updated_at`: Order by `updated_at` (prefix with `-` for descending).
*   **`can_query`**:
    *   `where[starts_at]`: Query on a specific `starts_at` (e.g., `?where[starts_at]=2000-01-01T12:00:00Z`).
    *   `where[ends_at]`: Query on a specific `ends_at` (e.g., `?where[ends_at]=2000-01-01T12:00:00Z`).
*   **`per_page`**: How many records to return per page (min=1, max=100, default=25).
*   **`offset`**: Get results from a given offset.