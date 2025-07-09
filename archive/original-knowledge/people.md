## WorkflowStep

A Step

### Attributes

*   `id`: The unique identifier for a workflow step.
*   `sequence`: The sequence of the step.
*   `name`: The name of the step.
*   `description`: The description of the step.
*   `expected_response_time_in_days`: The expected response time in days.
*   `auto_snooze_value`: The value for auto snooze. Must be a positive number.
*   `auto_snooze_interval`: The interval for auto snooze. Possible values: `days`, `weeks`, `months`.
*   `created_at`: The date and time the step was created.
*   `updated_at`: The date and time the step was last updated.
*   `auto_snooze_days`: The number of days for auto snooze.
*   `my_ready_card_count`: The number of ready cards assigned to the current user for this step.
*   `total_ready_card_count`: The total number of ready cards for this step.
*   `default_assignee_id`: The ID of the default assignee for this step.

### Relationships

*   `default_assignee`: A `WorkflowStep` has a `DefaultAssignee` (Person).
*   `workflow`: A `WorkflowStep` belongs to a `Workflow`.

### Actions

(No specific actions listed for WorkflowStep)

### Edges (Related Resources)

*   **Outbound Edges (Resources accessible from a WorkflowStep):**
    *   `workflow_cards`: Access `WorkflowCard`s in this step at `https://api.planningcenteronline.com/people/v2/workflows/{workflow_id}/steps/{workflow_step_id}/workflow_cards`.
    *   `default_assignee`: Access the `Person` who is the default assignee for this step at `https://api.planningcenteronline.com/people/v2/workflows/{workflow_id}/steps/{workflow_step_id}/default_assignee`.
    *   `assignee_summary`: Access `WorkflowStepAssigneeSummary` for this step at `https://api.planningcenteronline.com/people/v2/workflows/{workflow_id}/steps/{workflow_step_id}/assignee_summary`.
*   **Inbound Edges (How to access WorkflowSteps from other resources):**
    *   From `Workflow`: `https://api.planningcenteronline.com/people/v2/workflows/{workflow_id}/workflow_steps`.
    *   From `WorkflowCard`: `https://api.planningcenteronline.com/people/v2/workflow_cards/{workflow_card_id}/current_step`.

### Query Parameters

*   **`can_include`**:
    *   `workflow_cards`: Include associated `workflow_cards`.
    *   `default_assignee`: Include associated `default_assignee`.
    *   `assignee_summary`: Include associated `assignee_summary`.
*   **`can_order`**:
    *   `sequence`: Order by `sequence` (prefix with `-` for descending).
    *   `name`: Order by `name` (prefix with `-` for descending).
    *   `created_at`: Order by `created_at` (prefix with `-` for descending).
    *   `updated_at`: Order by `updated_at` (prefix with `-` for descending).
*   **`can_query`**:
    *   `where[name]`: Query on a specific `name` (e.g., `?where[name]=Initial Contact`).
    *   `where[expected_response_time_in_days]`: Query on a specific `expected_response_time_in_days` (e.g., `?where[expected_response_time_in_days]=7`).
    *   `where[auto_snooze_value]`: Query on a specific `auto_snooze_value` (e.g., `?where[auto_snooze_value]=3`).
    *   `where[auto_snooze_interval]`: Query on a specific `auto_snooze_interval` (e.g., `?where[auto_snooze_interval]=days`).
    *   `where[my_ready_card_count]`: Query on a specific `my_ready_card_count` (e.g., `?where[my_ready_card_count]=5`).
    *   `where[total_ready_card_count]`: Query on a specific `total_ready_card_count` (e.g., `?where[total_ready_card_count]=10`).
*   **`per_page`**: How many records to return per page (min=1, max=100, default=25).
*   **`offset`**: Get results from a given offset.