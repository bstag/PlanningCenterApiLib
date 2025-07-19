# Planning Center API Modules Overview

## Complete Module Coverage

The Planning Center .NET SDK provides comprehensive coverage of all 9 Planning Center API modules. This document provides an overview of each module's capabilities and key entities.

## 1. People Module 👥
**Primary Purpose:** Central person management and organizational data

**Key Entities:**
- **Person** - Individual records with demographics, contact info, custom fields
- **Household** - Family/living unit groupings
- **Address, Email, PhoneNumber** - Contact information
- **Workflow** - Business process management
- **WorkflowCard** - Individual process instances
- **Form** - Data collection forms
- **List** - People organization and communication
- **Campus** - Organizational locations

**Core Capabilities:**
- Comprehensive person data management
- Workflow automation and tracking
- Form building and submission processing
- List management for communication
- Custom field handling
- Household relationship management

**API Endpoints:** 50+ endpoints covering all aspects of people management

---

## 2. Giving Module 💰
**Primary Purpose:** Financial giving and donation management

**Key Entities:**
- **Donation** - Individual financial gifts
- **Fund** - Designated giving categories
- **Batch** - Grouped donations for processing
- **Pledge** - Commitment to give over time
- **RecurringDonation** - Scheduled ongoing gifts
- **Refund** - Returned donations
- **PaymentSource** - Stored payment methods
- **PledgeCampaign** - Fundraising campaigns

**Core Capabilities:**
- Donation tracking and management
- Fund designation and reporting
- Batch processing for accounting
- Pledge campaign management
- Recurring donation automation
- Refund processing
- Payment method management
- Comprehensive giving reports

**API Endpoints:** 40+ endpoints for complete giving management

---

## 3. Calendar Module 📅
**Primary Purpose:** Event scheduling and resource management

**Key Entities:**
- **Event** - Scheduled activities
- **EventInstance** - Specific occurrences of events
- **Resource** - Bookable items (rooms, equipment)
- **ResourceBooking** - Resource reservations
- **Conflict** - Scheduling conflicts
- **Attachment** - Event-related files
- **Feed** - Calendar subscriptions
- **Tag** - Event categorization

**Core Capabilities:**
- Event creation and management
- Resource booking and conflict resolution
- Calendar feed generation
- Event attachment handling
- Tag-based organization
- Conflict detection and resolution
- Room setup management

**API Endpoints:** 35+ endpoints for event and resource management

---

## 4. Check-Ins Module ✅
**Primary Purpose:** Event attendance and check-in management

**Key Entities:**
- **CheckIn** - Individual attendance records
- **Event** - Check-in events
- **Location** - Check-in locations
- **Station** - Check-in kiosks/stations
- **AttendanceType** - Types of attendees
- **Pass** - Check-in passes/badges
- **Theme** - Check-in interface themes

**Core Capabilities:**
- Real-time check-in processing
- Attendance tracking and reporting
- Location-based check-ins
- Station management
- Badge/pass generation
- Theme customization
- Attendance analytics

**API Endpoints:** 25+ endpoints for check-in management

---

## 5. Groups Module 👥
**Primary Purpose:** Small group and community management

**Key Entities:**
- **Group** - Small group records
- **Membership** - Group participation
- **Event** - Group meetings/events
- **Enrollment** - Group registration
- **GroupType** - Group categories
- **Location** - Meeting locations
- **Resource** - Group resources
- **Tag** - Group categorization

**Core Capabilities:**
- Group lifecycle management
- Membership tracking
- Event scheduling for groups
- Enrollment processing
- Resource allocation
- Group categorization
- Attendance tracking

**API Endpoints:** 30+ endpoints for group management

---

## 6. Registrations Module 📝
**Primary Purpose:** Event registration and attendee management

**Key Entities:**
- **Registration** - Event registrations
- **Attendee** - Registered participants
- **Signup** - Registration instances
- **Category** - Registration categories
- **SelectionType** - Registration options
- **EmergencyContact** - Emergency information

**Core Capabilities:**
- Event registration processing
- Attendee management
- Registration form building
- Payment integration
- Capacity management
- Emergency contact handling
- Registration reporting

**API Endpoints:** 20+ endpoints for registration management

---

## 7. Publishing Module 📺
**Primary Purpose:** Media content and sermon management

**Key Entities:**
- **Episode** - Individual content pieces
- **Series** - Content collections
- **Speaker** - Content presenters
- **Media** - Audio/video files
- **Speakership** - Speaker-episode relationships

**Core Capabilities:**
- Content library management
- Series organization
- Speaker profile management
- Media file handling
- Content distribution
- Metadata management
- Publishing workflows

**API Endpoints:** 15+ endpoints for content management

---

## 8. Services Module 🎵
**Primary Purpose:** Service planning and worship management

**Key Entities:**
- **Plan** - Service plans
- **Item** - Plan items/elements
- **Song** - Song library
- **Arrangement** - Song arrangements
- **Team** - Service teams
- **Person** - Team members
- **ArrangementSection** - Song sections

**Core Capabilities:**
- Service plan creation
- Song library management
- Team scheduling
- Arrangement management
- Item sequencing
- Template management
- Team communication

**API Endpoints:** 25+ endpoints for service planning

---

## 9. Webhooks Module 🔗
**Primary Purpose:** Real-time event notifications and integrations

**Key Entities:**
- **WebhookSubscription** - Notification subscriptions
- **Event** - Webhook events
- **AvailableEvent** - Available event types

**Core Capabilities:**
- Subscription management
- Event filtering
- Signature verification
- Delivery tracking
- Retry logic
- Event routing
- Integration support

**API Endpoints:** 10+ endpoints for webhook management

---

## Cross-Module Integration

### Unified Person Model
The SDK provides a unified `Core.Person` model that consolidates person data from all modules:
- **People Module:** Complete demographic and contact information
- **Giving Module:** Donation history and giving patterns
- **Calendar Module:** Event permissions and roles
- **Services Module:** Team memberships and service roles
- **Check-Ins Module:** Attendance history
- **Groups Module:** Group memberships
- **Registrations Module:** Event registrations

### Shared Entities
Several entities appear across multiple modules with consistent interfaces:
- **Campus** - Organizational locations (People, Giving, Groups)
- **Organization** - Top-level organizational data (all modules)
- **Person** - Individual records (all modules)

### Common Patterns
All modules follow consistent patterns for:
- **Authentication** - OAuth 2.0 with automatic token management
- **Pagination** - Consistent paging with links and metadata
- **Filtering** - Standardized query parameters
- **Includes** - Related data loading
- **Error Handling** - Consistent exception hierarchy
- **Caching** - Intelligent caching strategies

## API Coverage Statistics

| Module | Entities | Endpoints | DTOs | Core Models |
|--------|----------|-----------|------|-------------|
| People | 15+ | 50+ | 25+ | 8 |
| Giving | 12+ | 40+ | 20+ | 6 |
| Calendar | 10+ | 35+ | 15+ | 5 |
| Check-Ins | 8+ | 25+ | 12+ | 4 |
| Groups | 10+ | 30+ | 15+ | 5 |
| Registrations | 8+ | 20+ | 10+ | 4 |
| Publishing | 5+ | 15+ | 8+ | 3 |
| Services | 8+ | 25+ | 12+ | 4 |
| Webhooks | 3+ | 10+ | 5+ | 2 |
| **Total** | **79+** | **250+** | **122+** | **41** |

## Development Priority

Based on usage patterns and dependencies, the recommended implementation order is:

1. **People** (Foundation) - Core person management
2. **Giving** (High Value) - Financial tracking
3. **Calendar** (Complex) - Event and resource management
4. **Check-Ins** (Dependent) - Builds on People and Calendar
5. **Groups** (Dependent) - Builds on People
6. **Registrations** (Dependent) - Builds on People and Calendar
7. **Publishing** (Independent) - Content management
8. **Services** (Complex) - Service planning
9. **Webhooks** (Integration) - Real-time notifications

This comprehensive coverage ensures that the SDK can handle any Planning Center integration scenario while maintaining consistency and ease of use across all modules.