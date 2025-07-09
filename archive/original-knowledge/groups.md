
## TagGroup

A `TagGroup` is a way to organize `Tag`s.

### Attributes

*   `id`: The unique identifier for a tag group.
*   `name`: The name of the tag group.
*   `position`: The position of the tag group in the list.

### Relationships

(No direct relationships listed for TagGroup)

### Actions

(No specific actions listed for TagGroup)

### Edges (Related Resources)

*   **Outbound Edges (Resources accessible from a TagGroup):**
    *   `tags`: Access `Tag`s for this tag group at `https://api.planningcenteronline.com/groups/v2/tag_groups/{tag_group_id}/tags`.
*   **Inbound Edges (How to access TagGroups from other resources):**
    *   From `Organization`: `https://api.planningcenteronline.com/groups/v2/tag_groups`.
    *   From `Tag`: `https://api.planningcenteronline.com/groups/v2/tags/{tag_id}/tag_group`.

### Query Parameters

*   **`can_order`**:
    *   `name`: Order by `name` (prefix with `-` for descending).
    *   `position`: Order by `position` (prefix with `-` for descending).
*   **`can_query`**:
    *   `where[name]`: Query on a specific `name` (e.g., `?where[name]=Demographics`).
*   **`per_page`**: How many records to return per page (min=1, max=100, default=25).
*   **`offset`**: Get results from a given offset.
