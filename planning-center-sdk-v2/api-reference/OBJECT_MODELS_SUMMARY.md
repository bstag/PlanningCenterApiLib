# Planning Center API Object Models Summary

## Overview

This document provides a comprehensive summary of all object models available in the Planning Center API, based on the existing C# class definitions and API documentation. The SDK implements these as both module-specific DTOs and unified core models.

## Object Model Categories

### Core Shared Models
These models appear across multiple modules and are implemented as unified core models:

#### Person
**Modules:** All modules
**Purpose:** Individual person records
**Key Properties:**
- `Id`, `FirstName`, `LastName` - Basic identification
- `Birthdate`, `Gender`, `MaritalStatus` - Demographics
- `Status`, `CreatedAt`, `UpdatedAt` - Lifecycle
- `PrimaryCampusId` - Organizational relationship

**Module Variations:**
- **People:** Complete demographic and contact information
- **Giving:** Financial giving history and patterns
- **Calendar:** Event permissions and calendar roles
- **Services:** Team memberships and service roles
- **Check-Ins:** Attendance preferences and history
- **Groups:** Group memberships and leadership roles

#### Campus
**Modules:** People, Giving, Groups, Registrations
**Purpose:** Organizational locations/sites
**Key Properties:**
- `Id`, `Name`, `Description` - Basic information
- `TimeZone`, `Country` - Location details
- `Address`, `PhoneNumber`, `Website` - Contact information

#### Organization
**Modules:** All modules
**Purpose:** Top-level organizational configuration
**Key Properties:**
- `Id`, `Name`, `CountryCode` - Basic identification
- `TimeZone`, `DateFormat` - Localization settings
- `ContactWebsite`, `AvatarUrl` - Branding
- `ChurchCenterSubdomain` - Integration settings

### Module-Specific Models

## 1. People Module Models (25+ classes)

### Core Entities
- **Person** - Central person record
- **Address** - Physical addresses
- **Email** - Email addresses  
- **PhoneNumber** - Phone numbers
- **Household** - Family/living units
- **HouseholdMembership** - Household relationships

### Workflow Management
- **Workflow** - Business processes
- **WorkflowStep** - Process steps
- **WorkflowCard** - Process instances
- **WorkflowCardActivity** - Card activities
- **WorkflowCardNote** - Card notes

### Forms and Data Collection
- **Form** - Data collection forms
- **FormCategory** - Form organization
- **FormField** - Form fields
- **FormFieldOption** - Field options
- **FormSubmission** - Submitted forms
- **FormSubmissionValue** - Submission values

### Lists and Organization
- **List** - People lists
- **ListCategory** - List organization
- **ListResult** - List memberships
- **ListShare** - List sharing
- **ListStar** - Starred lists

### Custom Data
- **FieldDefinition** - Custom field definitions
- **FieldDatum** - Custom field values
- **FieldOption** - Field options

### Communication
- **Message** - Messages
- **MessageGroup** - Message groups
- **Note** - Notes
- **NoteCategory** - Note categories

### Additional Entities
- **App** - Connected applications
- **BackgroundCheck** - Background check records
- **Carrier** - Phone carriers
- **Condition** - List conditions
- **ConnectedPerson** - Person connections
- **InactiveReason** - Inactivity reasons
- **MaritalStatus** - Marital status options
- **NamePrefix**, **NameSuffix** - Name components
- **Report** - Reports
- **Rule** - List rules
- **SchoolOption** - School options
- **ServiceTime** - Service times
- **SocialProfile** - Social media profiles
- **Tab** - Interface tabs

## 2. Giving Module Models (20+ classes)

### Financial Transactions
- **Donation** - Individual gifts
- **Batch** - Grouped donations
- **BatchGroup** - Batch collections
- **Refund** - Returned donations
- **DesignationRefund** - Fund-specific refunds

### Funds and Designations
- **Fund** - Giving funds/categories
- **Designation** - Fund allocations
- **Label** - Fund labels

### Pledges and Campaigns
- **Pledge** - Giving commitments
- **PledgeCampaign** - Fundraising campaigns

### Recurring Giving
- **RecurringDonation** - Scheduled gifts
- **RecurringDonationDesignation** - Recurring fund allocations

### Payment Processing
- **PaymentMethod** - Payment types
- **PaymentSource** - Stored payment methods

### Additional Entities
- **Campus** - Giving-specific campus data
- **InKindDonation** - Non-cash gifts
- **Note** - Donation notes
- **Person** - Giving-specific person data

## 3. Calendar Module Models (15+ classes)

### Events and Scheduling
- **Event** - Scheduled events
- **EventInstance** - Event occurrences
- **EventTime** - Event timing
- **EventConnection** - Event relationships

### Resources and Booking
- **Resource** - Bookable resources
- **ResourceBooking** - Resource reservations
- **ResourceFolder** - Resource organization
- **ResourceQuestion** - Resource questions
- **ResourceSuggestion** - Resource suggestions
- **EventResourceRequest** - Resource requests
- **EventResourceAnswer** - Resource answers

### Conflicts and Approvals
- **Conflict** - Scheduling conflicts
- **RequiredApproval** - Approval requirements
- **ResourceApprovalGroup** - Approval groups

### Additional Entities
- **Attachment** - Event attachments
- **Feed** - Calendar feeds
- **JobStatus** - Background job status
- **Person** - Calendar-specific person data
- **ReportTemplate** - Report templates
- **RoomSetup** - Room configurations
- **Tag** - Event tags
- **TagGroup** - Tag organization

## 4. Check-Ins Module Models (12+ classes)

### Check-In Process
- **CheckIn** - Individual check-ins
- **CheckInGroup** - Check-in groups
- **CheckInTime** - Check-in times
- **PreCheck** - Pre-check-in data

### Events and Scheduling
- **Event** - Check-in events
- **EventPeriod** - Event periods
- **EventLabel** - Event labels

### Locations and Stations
- **Location** - Check-in locations
- **LocationEventPeriod** - Location periods
- **LocationEventTime** - Location times
- **LocationLabel** - Location labels
- **Station** - Check-in stations

### Additional Entities
- **AttendanceType** - Attendance categories
- **Option** - Check-in options
- **Pass** - Check-in passes
- **Person** - Check-in person data
- **PersonEvent** - Person-event relationships
- **RosterListPerson** - Roster entries
- **Theme** - Check-in themes

## 5. Groups Module Models (15+ classes)

### Group Management
- **Group** - Group records
- **GroupType** - Group categories
- **GroupApplication** - Group applications
- **Membership** - Group memberships
- **Enrollment** - Group enrollment

### Events and Activities
- **Event** - Group events
- **EventNote** - Event notes
- **Attendance** - Event attendance

### Organization
- **Tag** - Group tags
- **TagGroup** - Tag organization
- **Location** - Meeting locations
- **Resource** - Group resources

### Additional Entities
- **Campus** - Groups-specific campus data
- **Owner** - Group owners
- **Person** - Groups-specific person data

## 6. Registrations Module Models (10+ classes)

### Registration Process
- **Registration** - Event registrations
- **Attendee** - Registered attendees
- **Signup** - Registration instances
- **SignupLocation** - Registration locations
- **SignupTime** - Registration times

### Organization
- **Category** - Registration categories
- **SelectionType** - Registration options

### Additional Entities
- **Campus** - Registration campus data
- **EmergencyContact** - Emergency contacts
- **Person** - Registration person data

## 7. Publishing Module Models (5+ classes)

### Content Management
- **Episode** - Content episodes
- **Series** - Content series
- **Speaker** - Content speakers
- **Speakership** - Speaker-episode relationships
- **Media** - Media files

## 8. Services Module Models (12+ classes)

### Service Planning
- **Plan** - Service plans
- **Item** - Plan items
- **Song** - Song library
- **Arrangement** - Song arrangements
- **ArrangementSection** - Song sections

### Team Management
- **Team** - Service teams
- **Person** - Services-specific person data
- **PersonTeamPositionAssignment** - Team assignments

### Additional Entities
- **ServiceType** - Service categories
- **Speaker** - Service speakers
- **Media** - Service media

## 9. Webhooks Module Models (5+ classes)

### Webhook Management
- **WebhookSubscription** - Notification subscriptions
- **Event** - Webhook events
- **AvailableEvent** - Available event types

## Implementation Strategy

### DTO Pattern
Each module has its own set of DTOs that map directly to API responses:
```csharp
// Module-specific DTOs
namespace PlanningCenter.Api.Client.Models.People
{
    public class PersonDto { /* API-specific fields */ }
}

namespace PlanningCenter.Api.Client.Models.Giving  
{
    public class PersonDto { /* Giving-specific fields */ }
}
```

### Unified Core Models
Common entities are unified into core models:
```csharp
// Unified core models
namespace PlanningCenter.Api.Client.Models.Core
{
    public class Person 
    { 
        /* All properties from all modules */
        public string SourceModule { get; set; } // Tracks data source
    }
}
```

### Mapping Strategy
Adapters map between DTOs and core models:
```csharp
public class PersonMapper
{
    public Core.Person Map(People.PersonDto source) { /* People mapping */ }
    public Core.Person Map(Giving.PersonDto source) { /* Giving mapping */ }
    public Core.Person Map(Calendar.PersonDto source) { /* Calendar mapping */ }
}
```

## Object Model Statistics

| Module | Total Classes | Core Models | DTOs | Request Models |
|--------|---------------|-------------|------|----------------|
| People | 45+ | 8 | 25+ | 12+ |
| Giving | 20+ | 6 | 15+ | 8+ |
| Calendar | 20+ | 5 | 15+ | 6+ |
| Check-Ins | 15+ | 4 | 12+ | 5+ |
| Groups | 15+ | 5 | 12+ | 5+ |
| Registrations | 10+ | 4 | 8+ | 4+ |
| Publishing | 5+ | 3 | 5+ | 3+ |
| Services | 12+ | 4 | 10+ | 4+ |
| Webhooks | 5+ | 2 | 3+ | 2+ |
| **Total** | **147+** | **41** | **105+** | **49+** |

This comprehensive object model coverage ensures that the SDK can handle all Planning Center API data with strong typing and consistent interfaces.