# Planning Center SDK Modules Documentation

This folder contains comprehensive specifications for all 9 Planning Center API modules. Each module document provides detailed information about entities, endpoints, service interfaces, fluent API design, and implementation guidance.

## 📋 Complete Module Coverage

### 1. [People Module](PEOPLE_MODULE.md) 👥
**Central person management and organizational data**
- **Entities:** Person, Household, Address, Email, PhoneNumber, Workflow, Form, List
- **Endpoints:** 50+ endpoints for complete people management
- **Key Features:** Workflow automation, form processing, list management, custom fields
- **Implementation Priority:** 🔴 Critical (Phase 2)

### 2. [Giving Module](GIVING_MODULE.md) 💰
**Financial giving and donation management**
- **Entities:** Donation, Fund, Batch, Pledge, RecurringDonation, Refund, PaymentSource
- **Endpoints:** 40+ endpoints for financial tracking
- **Key Features:** Donation processing, fund management, pledge campaigns, refund handling
- **Implementation Priority:** 🔴 Critical (Phase 3)

### 3. [Calendar Module](CALENDAR_MODULE.md) 📅
**Event scheduling and resource management**
- **Entities:** Event, EventInstance, Resource, ResourceBooking, Conflict, Attachment
- **Endpoints:** 35+ endpoints for event management
- **Key Features:** Resource booking, conflict resolution, calendar feeds, event attachments
- **Implementation Priority:** 🔴 Critical (Phase 4)

### 4. [Check-Ins Module](CHECKINS_MODULE.md) ✅
**Event attendance and check-in management**
- **Entities:** CheckIn, Event, Location, Station, AttendanceType, Pass, Theme
- **Endpoints:** 25+ endpoints for attendance tracking
- **Key Features:** Real-time check-ins, attendance reporting, station management, badge generation
- **Implementation Priority:** 🟡 Important (Phase 5)

### 5. [Groups Module](GROUPS_MODULE.md) 👥
**Small group and community management**
- **Entities:** Group, Membership, Event, Enrollment, GroupType, Location, Tag
- **Endpoints:** 30+ endpoints for group management
- **Key Features:** Group lifecycle, membership tracking, event scheduling, enrollment processing
- **Implementation Priority:** 🟡 Important (Phase 5)

### 6. [Registrations Module](REGISTRATIONS_MODULE.md) 📝
**Event registration and attendee management**
- **Entities:** Signup, Registration, Attendee, SelectionType, SignupLocation, SignupTime
- **Endpoints:** 20+ endpoints for registration processing
- **Key Features:** Registration forms, payment processing, capacity management, waitlist handling
- **Implementation Priority:** 🟡 Important (Phase 6)

### 7. [Publishing Module](PUBLISHING_MODULE.md) 📺
**Media content and sermon management**
- **Entities:** Episode, Series, Speaker, Speakership, Media
- **Endpoints:** 15+ endpoints for content management
- **Key Features:** Content library, media handling, speaker management, content distribution
- **Implementation Priority:** 🟢 Standard (Phase 7)

### 8. [Services Module](SERVICES_MODULE.md) 🎵
**Service planning and worship management**
- **Entities:** Plan, Item, Song, Arrangement, Team, TeamPosition, Person
- **Endpoints:** 25+ endpoints for service planning
- **Key Features:** Service plans, song library, team scheduling, arrangement management
- **Implementation Priority:** 🟢 Standard (Phase 7)

### 9. [Webhooks Module](WEBHOOKS_MODULE.md) 🔗
**Real-time event notifications and integrations**
- **Entities:** WebhookSubscription, AvailableEvent, Event
- **Endpoints:** 10+ endpoints for webhook management
- **Key Features:** Event subscriptions, signature validation, delivery tracking, retry logic
- **Implementation Priority:** 🔵 Integration (Phase 8)

## 🏗️ Module Architecture Patterns

### Consistent Design Patterns
All modules follow the same architectural patterns:

#### 1. **Unified Core Models**
```csharp
// Example: Person model unified across all modules
namespace PlanningCenter.Api.Client.Models.Core
{
    public class Person
    {
        // Common properties from all modules
        public string SourceModule { get; set; } // Tracks data origin
    }
}
```

#### 2. **Module-Specific DTOs**
```csharp
// Example: Module-specific person DTOs
namespace PlanningCenter.Api.Client.Models.People
{
    public class PersonDto { /* People-specific fields */ }
}

namespace PlanningCenter.Api.Client.Models.Giving
{
    public class PersonDto { /* Giving-specific fields */ }
}
```

#### 3. **Service Interface Pattern**
```csharp
public interface IModuleService
{
    Task<Entity> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Entity>> ListAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Entity> CreateAsync(EntityCreateRequest request, CancellationToken cancellationToken = default);
    Task<Entity> UpdateAsync(string id, EntityUpdateRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
```

#### 4. **Fluent API Pattern**
```csharp
public interface IModuleFluentContext
{
    IEntityFluentContext Entities();
    IEntityFluentContext Entity(string entityId);
}

public interface IEntityFluentContext
{
    IEntityFluentContext Where(Expression<Func<Entity, bool>> predicate);
    IEntityFluentContext Include(Expression<Func<Entity, object>> include);
    IEntityFluentContext OrderBy(Expression<Func<Entity, object>> orderBy);
    
    Task<Entity> GetAsync(CancellationToken cancellationToken = default);
    Task<IPagedResponse<Entity>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default);
    Task<List<Entity>> GetAllAsync(CancellationToken cancellationToken = default);
}
```

## 📊 Implementation Statistics

### Module Complexity Analysis
| Module | Entities | Endpoints | Core Models | Complexity | Est. Dev Time |
|--------|----------|-----------|-------------|------------|---------------|
| People | 15+ | 50+ | 8 | High | 3 weeks |
| Giving | 12+ | 40+ | 6 | High | 3 weeks |
| Calendar | 10+ | 35+ | 5 | High | 3 weeks |
| Check-Ins | 8+ | 25+ | 4 | Medium | 2 weeks |
| Groups | 10+ | 30+ | 5 | Medium | 2 weeks |
| Registrations | 8+ | 20+ | 4 | Medium | 2 weeks |
| Publishing | 5+ | 15+ | 3 | Low | 1 week |
| Services | 8+ | 25+ | 4 | Medium | 2 weeks |
| Webhooks | 3+ | 10+ | 2 | Low | 1 week |
| **Total** | **79+** | **250+** | **41** | - | **19 weeks** |

### Feature Coverage
- **Complete API Coverage:** All documented endpoints
- **Unified Models:** Consistent data access across modules
- **Dual API Design:** Both service-based and fluent APIs
- **Advanced Features:** Caching, webhooks, bulk operations
- **Production Ready:** Error handling, resilience, monitoring

## 🚀 Implementation Roadmap

### Phase-Based Implementation
1. **Phase 2-4:** Core Modules (People, Giving, Calendar) - Weeks 6-14
2. **Phase 5-6:** Important Modules (Check-Ins, Groups, Registrations) - Weeks 15-20
3. **Phase 7:** Standard Modules (Publishing, Services) - Weeks 21-23
4. **Phase 8:** Integration Module (Webhooks) - Weeks 24-26

### Cross-Module Features
- **Unified Person Model** spans all modules
- **Campus and Organization** models shared across modules
- **Webhook Events** available from all modules
- **Consistent Error Handling** across all modules
- **Shared Caching Strategies** for performance

## 🔧 Development Guidelines

### Module Implementation Checklist
For each module, ensure:
- [ ] All entities documented with relationships
- [ ] Service interface complete with all operations
- [ ] Fluent API context designed for complex queries
- [ ] Request/response models defined
- [ ] Mapping strategy for unified models
- [ ] Error handling scenarios covered
- [ ] Caching strategy defined
- [ ] Security considerations addressed
- [ ] Performance optimizations planned
- [ ] Usage examples provided

### Quality Standards
- **Documentation:** 100% public API documented
- **Test Coverage:** 90%+ for all modules
- **Performance:** <200ms average response time
- **Reliability:** >99.9% success rate
- **Security:** Proper authorization and data protection

## 📚 Usage Examples

### Service-Based API
```csharp
// Cross-module operations
var person = await peopleService.GetAsync("123");
var donations = await givingService.GetDonationsForPersonAsync(person.Id);
var checkIns = await checkInsService.GetCheckInsForPersonAsync(person.Id);
```

### Fluent API
```csharp
// Complex cross-module queries
var activeMembers = await client
    .People()
    .Where(p => p.Status == "active")
    .Include(p => p.Households)
    .GetAllAsync();

var recentDonors = await client
    .Giving()
    .Donations()
    .ReceivedAfter(DateTime.Now.AddDays(-30))
    .Include(d => d.Person)
    .GetAllAsync();
```

## 🎯 Next Steps

1. **Review Module Priority** - Determine which modules are most critical for your use case
2. **Start Implementation** - Begin with Phase 2 (People Module) 
3. **Follow Patterns** - Use the established patterns for consistency
4. **Test Thoroughly** - Implement comprehensive testing for each module
5. **Document Progress** - Update implementation status as modules are completed

This comprehensive module documentation provides everything needed to implement a complete Planning Center .NET SDK with full API coverage and production-ready capabilities.