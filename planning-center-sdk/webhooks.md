## WebhookSubscription

### Attributes

*   `id`: The unique identifier for a webhook subscription.
*   `url`: The URL of the webhook.
*   `created_at`: The date and time the webhook subscription was created.
*   `updated_at`: The date and time the webhook subscription was last updated.
*   `secret`: The secret for the webhook.
*   `active`: Whether the webhook subscription is active.
*   `last_delivery_at`: The date and time of the last delivery.
*   `last_delivery_status`: The status of the last delivery.

### Relationships

*   `available_event`: A `WebhookSubscription` belongs to an `AvailableEvent`.
*   `organization`: A `WebhookSubscription` belongs to an `Organization`.

### Actions

(No specific actions listed for WebhookSubscription)

### Edges (Related Resources)

*   **Outbound Edges (Resources accessible from a WebhookSubscription):**
    *   `events`: Access `Event`s for this webhook subscription at `https://api.planningcenteronline.com/webhooks/v2/webhook_subscriptions/{webhook_subscription_id}/events`.
    *   `available_event`: Access the `AvailableEvent` associated with this webhook subscription at `https://api.planningcenteronline.com/webhooks/v2/webhook_subscriptions/{webhook_subscription_id}/available_event`.
*   **Inbound Edges (How to access WebhookSubscriptions from other resources):**
    *   From `Organization`: `https://api.planningcenteronline.com/webhooks/v2/webhook_subscriptions`.
    *   From `AvailableEvent`: `https://api.planningcenteronline.com/webhooks/v2/available_events/{available_event_id}/webhook_subscriptions`.
    *   From `Event`: `https://api.planningcenteronline.com/webhooks/v2/events/{event_id}/webhook_subscription`.

### Query Parameters

*   **`can_include`**:
    *   `events`: Include associated `events`.
    *   `available_event`: Include associated `available_event`.
*   **`can_order`**:
    *   `created_at`: Order by `created_at` (prefix with `-` for descending).
    *   `updated_at`: Order by `updated_at` (prefix with `-` for descending).
    *   `url`: Order by `url` (prefix with `-` for descending).
*   **`can_query`**:
    *   `where[url]`: Query on a specific `url` (e.g., `?where[url]=https://example.com/webhook`).
    *   `where[active]`: Query on active status (e.g., `?where[active]=true`).
    *   `where[last_delivery_status]`: Query on a specific `last_delivery_status` (e.g., `?where[last_delivery_status]=success`).
*   **`per_page`**: How many records to return per page (min=1, max=100, default=25).
*   **`offset`**: Get results from a given offset.