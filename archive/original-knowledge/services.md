
## ArrangementSections

### Attributes

*   `id`: The unique identifier for arrangement sections.
*   `name`: The name of the arrangement section.
*   `created_at`: The date and time the arrangement section was created.
*   `updated_at`: The date and time the arrangement section was last updated.

### Relationships

*   `arrangement`: `ArrangementSections` belongs to an `Arrangement`.

### Actions

(No specific actions listed for ArrangementSections)

### Edges (Related Resources)

*   **Outbound Edges (Resources accessible from ArrangementSections):**
    *   (No outbound edges listed for ArrangementSections)
*   **Inbound Edges (How to access ArrangementSections from other resources):**
    *   From `Arrangement`: `https://api.planningcenteronline.com/services/v2/songs/{song_id}/arrangements/{arrangement_id}/arrangement_sections`.

### Query Parameters

*   **`can_order`**:
    *   `name`: Order by `name` (prefix with `-` for descending).
    *   `created_at`: Order by `created_at` (prefix with `-` for descending).
    *   `updated_at`: Order by `updated_at` (prefix with `-` for descending).
*   **`can_query`**:
    *   `where[name]`: Query on a specific `name` (e.g., `?where[name]=Verse 1`).
*   **`per_page`**: How many records to return per page (min=1, max=100, default=25).
*   **`offset`**: Get results from a given offset.
