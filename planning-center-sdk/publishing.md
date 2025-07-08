## Speakership

### Attributes

*   `id`: The unique identifier for a speakership.
*   `created_at`: The date and time the speakership was created.
*   `updated_at`: The date and time the speakership was last updated.

### Relationships

*   `episode`: A `Speakership` belongs to an `Episode`.
*   `speaker`: A `Speakership` belongs to a `Speaker`.

### Actions

(No specific actions listed for Speakership)

### Edges (Related Resources)

*   **Outbound Edges (Resources accessible from a Speakership):**
    *   `episode`: Access the `Episode` associated with this speakership at `https://api.planningcenteronline.com/publishing/v2/speakerships/{speakership_id}/episode`.
    *   `speaker`: Access the `Speaker` associated with this speakership at `https://api.planningcenteronline.com/publishing/v2/speakerships/{speakership_id}/speaker`.
*   **Inbound Edges (How to access Speakerships from other resources):**
    *   From `Episode`: `https://api.planningcenteronline.com/publishing/v2/episodes/{episode_id}/speakerships`.
    *   From `Speaker`: `https://api.planningcenteronline.com/publishing/v2/speakers/{speaker_id}/speakerships`.

### Query Parameters

*   **`can_order`**:
    *   `created_at`: Order by `created_at` (prefix with `-` for descending).
    *   `updated_at`: Order by `updated_at` (prefix with `-` for descending).
*   **`per_page`**: How many records to return per page (min=1, max=100, default=25).
*   **`offset`**: Get results from a given offset.