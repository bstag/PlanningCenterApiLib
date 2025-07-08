## Refund

A `Refund` record holds information pertaining to a refunded `Donation`.

### Attributes

*   `id`: The unique identifier for a refund.
*   `created_at`: The date and time at which a refund was created. Example: `2000-01-01T12:00:00Z`
*   `updated_at`: The date and time at which a refund was last updated. Example: `2000-01-01T12:00:00Z`
*   `amount_cents`: The number of cents being refunded.
*   `amount_currency`: The currency of `amount_cents`.
*   `fee_cents`: The payment processing fee returned by Stripe, if any.
*   `refunded_at`: The date and time the refund was processed. Example: `2000-01-01T12:00:00Z`
*   `fee_currency`: The currency of the fee.

### Relationships

*   `donation`: A `Refund` belongs to a `Donation`.

### Actions

(No specific actions listed for Refund)

### Edges (Related Resources)

*   **Outbound Edges (Resources accessible from a Refund):**
    *   `designation_refunds`: Access `DesignationRefund`s for this refund at `https://api.planningcenteronline.com/giving/v2/donations/{donation_id}/refund/designation_refunds`.
*   **Inbound Edges (How to access Refunds from other resources):**
    *   From `Donation`: `https://api.planningcenteronline.com/giving/v2/donations/{donation_id}/refund`.
    *   From `Organization`: `https://api.planningcenteronline.com/giving/v2/refunds`.

### Query Parameters

*   **`can_include`**:
    *   `designation_refunds`: Include associated `designation_refunds`.
*   **`can_order`**:
    *   `created_at`: Order by `created_at` (prefix with `-` for descending).
    *   `amount_cents`: Order by `amount_cents` (prefix with `-` for descending).
*   **`can_query`**:
    *   `where[created_at]`: Query on a specific `created_at` (e.g., `?where[created_at]=2000-01-01T12:00:00Z`).
    *   `where[amount_cents]`: Query on a specific `amount_cents` (e.g., `?where[amount_cents]=1000`).
*   **`per_page`**: How many records to return per page (min=1, max=100, default=25).
*   **`offset`**: Get results from a given offset.