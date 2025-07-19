# People Module Specification

## Overview

The People module is the central hub for person management in Planning Center. It provides comprehensive functionality for managing people, households, workflows, forms, lists, and organizational data.

## Core Entities

### Person
The primary entity representing an individual in the system.

**Key Attributes:**
- `id` - Unique identifier
- `first_name`, `last_name` - Name components
- `birthdate` - Date of birth
- `gender` - Gender identity
- `status` - Active, inactive, etc.
- `anniversary` - Anniversary date
- `school` - School information
- `medical_notes` - Medical information
- `primary_campus_id` - Associated campus

**Relationships:**
- `addresses` - Physical addresses
- `emails` - Email addresses
- `phone_numbers` - Phone numbers
- `households` - Household memberships
- `field_data` - Custom field values
- `workflow_cards` - Active workflow cards

### Address
Physical address information for a person.

**Key Attributes:**
- `street`, `city`, `state`, `zip` - Address components
- `location` - Home, work, etc.
- `primary` - Primary address flag

### Email
Email address information for a person.

**Key Attributes:**
- `address` - Email address
- `location` - Home, work, etc.
- `primary` - Primary email flag

### PhoneNumber
Phone number information for a person.

**Key Attributes:**
- `number` - Phone number
- `location` - Home, work, mobile, etc.
- `primary` - Primary phone flag

### Household
Groups of people living together.

**Key Attributes:**
- `name` - Household name
- `member_count` - Number of members
- `primary_contact_id` - Primary contact person

**Relationships:**
- `people` - Household members
- `household_memberships` - Membership details

### Workflow
Business process management for people.

**Key Attributes:**
- `name` - Workflow name
- `description` - Workflow description
- `my_ready_card_count` - Cards ready for current user
- `total_ready_card_count` - Total ready cards

**Relationships:**
- `workflow_steps` - Process steps
- `workflow_cards` - Active cards

### WorkflowStep
Individual steps in a workflow process.

**Key Attributes:**
- `sequence` - Step order
- `name` - Step name
- `description` - Step description
- `expected_response_time_in_days` - Expected completion time
- `auto_snooze_value` - Auto-snooze configuration
- `auto_snooze_interval` - Auto-snooze interval

**Relationships:**
- `workflow` - Parent workflow
- `default_assignee` - Default person assigned
- `workflow_cards` - Cards in this step

### WorkflowCard
Individual instances of people in workflow processes.

**Key Attributes:**
- `snooze_until` - Snooze date
- `overdue` - Overdue flag
- `stage` - Current stage
- `completed_at` - Completion date

**Relationships:**
- `person` - Associated person
- `workflow` - Parent workflow
- `current_step` - Current workflow step
- `assignee` - Assigned person

### Form
Data collection forms.

**Key Attributes:**
- `name` - Form name
- `description` - Form description
- `active` - Active status
- `archived_at` - Archive date

**Relationships:**
- `form_category` - Form category
- `form_fields` - Form fields
- `form_submissions` - Submitted forms

### List
People lists for organization and communication.

**Key Attributes:**
- `name` - List name
- `description` - List description
- `total_people` - Number of people
- `recently_viewed` - Recently viewed flag

**Relationships:**
- `list_category` - List category
- `people` - People in list
- `list_results` - List membership records

## API Endpoints

### People Management
- `GET /people/v2/people` - List all people
- `GET /people/v2/people/{id}` - Get specific person
- `POST /people/v2/people` - Create new person
- `PATCH /people/v2/people/{id}` - Update person
- `DELETE /people/v2/people/{id}` - Delete person

### Household Management
- `GET /people/v2/households` - List households
- `GET /people/v2/households/{id}` - Get specific household
- `POST /people/v2/households` - Create household
- `PATCH /people/v2/households/{id}` - Update household

### Workflow Management
- `GET /people/v2/workflows` - List workflows
- `GET /people/v2/workflows/{id}` - Get specific workflow
- `GET /people/v2/workflows/{id}/workflow_steps` - Get workflow steps
- `GET /people/v2/workflow_cards` - List workflow cards
- `POST /people/v2/workflow_cards` - Create workflow card
- `PATCH /people/v2/workflow_cards/{id}` - Update workflow card

### Form Management
- `GET /people/v2/forms` - List forms
- `GET /people/v2/forms/{id}` - Get specific form
- `GET /people/v2/form_submissions` - List form submissions
- `POST /people/v2/form_submissions` - Submit form

### List Management
- `GET /people/v2/lists` - List all lists
- `GET /people/v2/lists/{id}` - Get specific list
- `POST /people/v2/lists` - Create list
- `GET /people/v2/lists/{id}/list_results` - Get list members

## Query Parameters

### Include Parameters
- `addresses` - Include person addresses
- `emails` - Include person emails
- `phone_numbers` - Include person phone numbers
- `households` - Include household information
- `field_data` - Include custom field data
- `workflow_cards` - Include active workflow cards
- `primary_campus` - Include primary campus

### Filtering
- `where[first_name]` - Filter by first name
- `where[last_name]` - Filter by last name
- `where[status]` - Filter by status
- `where[created_at]` - Filter by creation date
- `where[updated_at]` - Filter by update date
- `where[birthdate]` - Filter by birthdate

### Sorting
- `order=first_name` - Sort by first name
- `order=last_name` - Sort by last name
- `order=created_at` - Sort by creation date
- `order=updated_at` - Sort by update date
- `order=-created_at` - Sort by creation date (descending)

## Service Interface

```csharp
public interface IPeopleService
{
    // Person management
    Task<Core.Person> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Core.Person>> ListAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Core.Person> CreateAsync(PersonCreateRequest request, CancellationToken cancellationToken = default);
    Task<Core.Person> UpdateAsync(string id, PersonUpdateRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    
    // Address management
    Task<Address> AddAddressAsync(string personId, AddressCreateRequest request, CancellationToken cancellationToken = default);
    Task<Address> UpdateAddressAsync(string personId, string addressId, AddressUpdateRequest request, CancellationToken cancellationToken = default);
    Task DeleteAddressAsync(string personId, string addressId, CancellationToken cancellationToken = default);
    
    // Email management
    Task<Email> AddEmailAsync(string personId, EmailCreateRequest request, CancellationToken cancellationToken = default);
    Task<Email> UpdateEmailAsync(string personId, string emailId, EmailUpdateRequest request, CancellationToken cancellationToken = default);
    Task DeleteEmailAsync(string personId, string emailId, CancellationToken cancellationToken = default);
    
    // Phone number management
    Task<PhoneNumber> AddPhoneNumberAsync(string personId, PhoneNumberCreateRequest request, CancellationToken cancellationToken = default);
    Task<PhoneNumber> UpdatePhoneNumberAsync(string personId, string phoneId, PhoneNumberUpdateRequest request, CancellationToken cancellationToken = default);
    Task DeletePhoneNumberAsync(string personId, string phoneId, CancellationToken cancellationToken = default);
    
    // Household management
    Task<Household> GetHouseholdAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Household>> ListHouseholdsAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Household> CreateHouseholdAsync(HouseholdCreateRequest request, CancellationToken cancellationToken = default);
    Task<Household> UpdateHouseholdAsync(string id, HouseholdUpdateRequest request, CancellationToken cancellationToken = default);
    
    // Workflow management
    Task<IPagedResponse<Workflow>> ListWorkflowsAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Workflow> GetWorkflowAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<WorkflowCard>> ListWorkflowCardsAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<WorkflowCard> CreateWorkflowCardAsync(WorkflowCardCreateRequest request, CancellationToken cancellationToken = default);
    Task<WorkflowCard> UpdateWorkflowCardAsync(string id, WorkflowCardUpdateRequest request, CancellationToken cancellationToken = default);
    Task DeleteWorkflowCardAsync(string id, CancellationToken cancellationToken = default);
    
    // Form management
    Task<IPagedResponse<Form>> ListFormsAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Form> GetFormAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<FormSubmission>> ListFormSubmissionsAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<FormSubmission> SubmitFormAsync(FormSubmissionCreateRequest request, CancellationToken cancellationToken = default);
    
    // List management
    Task<IPagedResponse<List>> ListPeopleListsAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<List> GetPeopleListAsync(string id, CancellationToken cancellationToken = default);
    Task<List> CreatePeopleListAsync(ListCreateRequest request, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Core.Person>> GetListMembersAsync(string listId, QueryParameters parameters = null, CancellationToken cancellationToken = default);
    
    // Bulk operations
    Task<BulkResult<Core.Person>> BulkCreateAsync(IEnumerable<PersonCreateRequest> requests, CancellationToken cancellationToken = default);
    Task<BulkResult<Core.Person>> BulkUpdateAsync(IEnumerable<PersonUpdateRequest> requests, CancellationToken cancellationToken = default);
    Task<BulkResult<string>> BulkDeleteAsync(IEnumerable<string> personIds, CancellationToken cancellationToken = default);
    
    // Long-running operations
    Task<SyncResult> SyncPeopleAsync(SyncRequest request, IProgress<SyncProgress> progress = null, CancellationToken cancellationToken = default);
    Task<ExportResult> ExportPeopleAsync(ExportRequest request, IProgress<ExportProgress> progress = null, CancellationToken cancellationToken = default);
}
```

## Fluent API Interface

```csharp
public interface IPeopleFluentContext
{
    // Person queries
    IPeopleFluentContext Where(Expression<Func<Core.Person, bool>> predicate);
    IPeopleFluentContext Include(Expression<Func<Core.Person, object>> include);
    IPeopleFluentContext OrderBy(Expression<Func<Core.Person, object>> orderBy);
    IPeopleFluentContext OrderByDescending(Expression<Func<Core.Person, object>> orderBy);
    
    // Execution
    Task<Core.Person> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Core.Person>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default);
    Task<List<Core.Person>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
    
    // Streaming
    IAsyncEnumerable<Core.Person> AsAsyncEnumerable(CancellationToken cancellationToken = default);
    
    // Creation
    IPeopleCreateContext Create(PersonCreateRequest request);
    
    // Workflow operations
    IWorkflowFluentContext Workflows();
    IWorkflowFluentContext Workflow(string workflowId);
    
    // Form operations
    IFormFluentContext Forms();
    IFormFluentContext Form(string formId);
    
    // List operations
    IListFluentContext Lists();
    IListFluentContext List(string listId);
    
    // Household operations
    IHouseholdFluentContext Households();
    IHouseholdFluentContext Household(string householdId);
}
```

## Usage Examples

### Service-Based API
```csharp
// Get a person with related data
var person = await peopleService.GetAsync("123");

// List people with filtering
var people = await peopleService.ListAsync(new QueryParameters
{
    Where = new Dictionary<string, object>
    {
        ["status"] = "active",
        ["created_at"] = ">2023-01-01"
    },
    Include = new[] { "addresses", "emails", "phone_numbers" },
    OrderBy = "last_name"
});

// Create a new person
var newPerson = await peopleService.CreateAsync(new PersonCreateRequest
{
    FirstName = "John",
    LastName = "Doe",
    Email = "john.doe@example.com"
});

// Add an address
var address = await peopleService.AddAddressAsync("123", new AddressCreateRequest
{
    Street = "123 Main St",
    City = "Anytown",
    State = "CA",
    Zip = "12345",
    Location = "Home",
    Primary = true
});
```

### Fluent API
```csharp
// Complex person query with cancellation
var people = await client
    .People()
    .Where(p => p.Status == "active")
    .Where(p => p.CreatedAt > DateTime.Now.AddDays(-30))
    .Include(p => p.Addresses)
    .Include(p => p.Emails)
    .Include(p => p.PhoneNumbers)
    .OrderBy(p => p.LastName)
    .ThenBy(p => p.FirstName)
    .GetPagedAsync(pageSize: 50, cancellationToken);

// Streaming large datasets with cancellation
await foreach (var person in client
    .People()
    .Where(p => p.Status == "active")
    .AsAsyncEnumerable(cancellationToken))
{
    // Process each person individually
    await ProcessPersonAsync(person, cancellationToken);
}

// Workflow operations
var workflowCards = await client
    .People()
    .Workflows()
    .Where(w => w.Name == "New Member Process")
    .Cards()
    .Where(c => c.AssigneeId == currentUserId)
    .Where(c => !c.Overdue)
    .GetAllAsync();

// Form submissions
var submissions = await client
    .People()
    .Forms()
    .Where(f => f.Name == "Visitor Information")
    .Submissions()
    .Where(s => s.CreatedAt > DateTime.Today)
    .GetPagedAsync();
```

## Implementation Notes

### Data Mapping
- Map module-specific Person DTOs to unified Core.Person model
- Handle nested objects (addresses, emails, phone numbers) properly
- Preserve all module-specific properties in unified model

### Caching Strategy
- Cache frequently accessed people data
- Invalidate cache on person updates
- Use person ID as cache key

### Error Handling
- Handle person not found scenarios
- Validate required fields for person creation
- Handle duplicate email/phone number scenarios

### Performance Considerations
- Use pagination for large people lists
- Implement efficient include parameter handling
- Optimize queries for common filtering scenarios

This module provides comprehensive people management capabilities while maintaining consistency with the unified model approach.