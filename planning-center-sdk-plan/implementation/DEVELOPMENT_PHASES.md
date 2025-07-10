# Planning Center SDK - Development Phases

## Overview

This document outlines the comprehensive 10-phase development approach for the Planning Center .NET SDK. Each phase builds upon the previous one, allowing for iterative development, testing, and refinement while delivering value incrementally.

## Phase 0: Enhanced Project Setup (Week 1-2)

### Objective
Create the complete solution structure with all identified modules and establish development standards.

### Deliverables
1. **Current Solution Structure** ✅ **COMPLETED**
   ```
   src/PlanningCenter.Api.sln
   ├── PlanningCenter.Api.Client/                    # Main client library
   ├── PlanningCenter.Api.Client.Abstractions/       # Interfaces and contracts  
   ├── PlanningCenter.Api.Client.Models/             # Data models
   ├── PlanningCenter.Api.Client.Tests/              # Unit tests
   └── PlanningCenter.Api.Client.IntegrationTests/   # Integration tests
   
   examples/
   ├── PlanningCenter.Api.Client.Console/             # Console example
   ├── PlanningCenter.Api.Client.Fluent.Console/      # Fluent API console example
   └── PlanningCenter.Api.Client.Worker/              # Background service example
   ```
   
   **Status**: ✅ Solution structure exists with .NET 9 target framework
   **Next**: Implement core interfaces and models

2. **Project Configuration**
   - Target .NET 9 for all projects
   - Configure NuGet package references
   - Set up solution-wide code analysis rules
   - Configure EditorConfig for consistent formatting

3. **Development Standards**
   - Create `Directory.Build.props` for common settings
   - Set up code analysis rules (StyleCop, FxCop)
   - Configure nullable reference types
   - Set up XML documentation requirements

4. **Initial Models Structure**
   ```
   PlanningCenter.Api.Client.Models/
   ├── Core/                    # Unified models
   ├── Calendar/               # Module-specific DTOs
   ├── CheckIns/
   ├── Giving/
   ├── Groups/
   ├── People/
   ├── Publishing/
   ├── Registrations/
   ├── Services/
   ├── Webhooks/
   ├── Requests/               # Request models
   ├── Responses/              # Response models
   └── Exceptions/             # Exception hierarchy
   ```

### Success Criteria
- [ ] All projects compile successfully
- [ ] Solution structure matches specification
- [ ] Code analysis passes with zero warnings
- [ ] Initial README.md created with setup instructions

---

## Phase 1: Enhanced Core Infrastructure (Week 3-5)

### Objective
Build comprehensive foundational components for API communication, authentication, caching, and webhook infrastructure.

### Deliverables

#### 1. Exception Hierarchy
```csharp
// PlanningCenter.Api.Client.Models/Exceptions/
public abstract class PlanningCenterApiException : Exception
{
    public int? StatusCode { get; }
    public string RequestId { get; }
    public string ErrorCode { get; }
}

public class PlanningCenterApiValidationException : PlanningCenterApiException
{
    public List<ValidationError> Errors { get; }
}

public class PlanningCenterApiRateLimitException : PlanningCenterApiException
{
    public TimeSpan RetryAfter { get; }
}

// Additional specific exceptions...
```

#### 2. Core Abstractions
```csharp
// PlanningCenter.Api.Client.Abstractions/Core/
public interface IApiConnection
{
    Task<T> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default);
    Task<T> PostAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default);
    Task<T> PutAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default);
    Task<T> PatchAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default);
    Task DeleteAsync(string endpoint, CancellationToken cancellationToken = default);
    Task<IPagedResponse<T>> GetPagedAsync<T>(string endpoint, QueryParameters parameters = null, CancellationToken cancellationToken = default);
}

public interface IAuthenticator
{
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
    Task RefreshTokenAsync(CancellationToken cancellationToken = default);
    Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);
    event EventHandler<TokenRefreshedEventArgs> TokenRefreshed;
}

public interface ICacheProvider
{
    Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, TimeSpan expiry, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);
}

public interface IWebhookValidator
{
    bool ValidateSignature(string payload, string signature, string secret);
    T DeserializeEvent<T>(string payload) where T : class;
}

public interface ITokenStorage
{
    Task<string> GetTokenAsync(string key);
    Task SetTokenAsync(string key, string token, TimeSpan expiry);
    Task RemoveTokenAsync(string key);
}
```

#### 3. HTTP Communication Layer
```csharp
// PlanningCenter.Api.Client/Http/
public class ApiConnection : IApiConnection
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiConnection> _logger;
    
    // Implementation with JSON serialization, error handling, etc.
}

public class ApiExceptionFactory
{
    public static PlanningCenterApiException CreateException(HttpResponseMessage response, string content)
    {
        // Create appropriate exception based on status code and content
    }
}
```

#### 4. Authentication Implementation
```csharp
// PlanningCenter.Api.Client/Auth/
public class OAuthAuthenticator : IAuthenticator
{
    private readonly HttpClient _httpClient;
    private readonly ITokenStorage _tokenStorage;
    private readonly PlanningCenterApiOptions _options;
    
    // OAuth 2.0 client credentials flow implementation
}

public class AuthHandler : DelegatingHandler
{
    private readonly IAuthenticator _authenticator;
    
    // Inject bearer token into requests
}

// Token storage implementations
public class InMemoryTokenStorage : ITokenStorage { }
public class FileTokenStorage : ITokenStorage { }
public class DistributedTokenStorage : ITokenStorage { }
```

#### 5. Caching Implementation
```csharp
// PlanningCenter.Api.Client/Caching/
public class InMemoryCacheProvider : ICacheProvider { }
public class DistributedCacheProvider : ICacheProvider { }
public class CacheKeyGenerator
{
    public static string GenerateKey(string prefix, params object[] parameters);
}
```

#### 6. Webhook Infrastructure
```csharp
// PlanningCenter.Api.Client/Webhooks/
public class WebhookValidator : IWebhookValidator
{
    public bool ValidateSignature(string payload, string signature, string secret)
    {
        // HMAC-SHA256 signature validation
    }
}

public interface IWebhookEventHandler<T> where T : class
{
    Task HandleAsync(T eventData, CancellationToken cancellationToken = default);
}
```

#### 7. Resilience Policies
```csharp
// PlanningCenter.Api.Client/Resilience/
public static class RetryPolicies
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy();
}

public static class RateLimitPolicies
{
    public static IAsyncPolicy<HttpResponseMessage> GetRateLimitPolicy();
}

public static class CircuitBreakerPolicies
{
    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy();
}
```

#### 8. Dependency Injection
```csharp
// PlanningCenter.Api.Client/DependencyInjection.cs
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPlanningCenterApiClient(
        this IServiceCollection services,
        Action<PlanningCenterApiOptions> configureOptions)
    {
        // Configure all services, HTTP client, policies, etc.
    }
}
```

### Testing Requirements
- Unit tests for all core components with 90%+ coverage
- Mock-based testing for HTTP communication
- Integration tests for authentication flow
- Performance tests for caching implementations

### Success Criteria
- [ ] All core abstractions implemented
- [ ] Authentication flow working with test credentials
- [ ] HTTP client properly configured with resilience policies
- [ ] Caching working with in-memory and distributed providers
- [ ] Webhook signature validation working
- [ ] Dependency injection properly configured
- [ ] All unit tests passing

---

## Phase 2: People Module - Complete Implementation (Week 6-8)

### Objective
Implement the comprehensive People module as the foundation for all other modules, establishing patterns for models, services, and fluent API.

### Deliverables

#### 1. People DTOs and Models
```csharp
// PlanningCenter.Api.Client.Models/People/
public class PersonDto { /* All People API fields */ }
public class AddressDto { /* Address fields */ }
public class EmailDto { /* Email fields */ }
public class PhoneNumberDto { /* Phone number fields */ }
public class HouseholdDto { /* Household fields */ }
public class WorkflowDto { /* Workflow fields */ }
public class WorkflowStepDto { /* Workflow step fields */ }
public class WorkflowCardDto { /* Workflow card fields */ }
public class FormDto { /* Form fields */ }
public class ListDto { /* List fields */ }

// Core unified models
// PlanningCenter.Api.Client.Models/Core/
public class Person { /* Unified person model */ }
public class Address { /* Unified address model */ }
public class Email { /* Unified email model */ }
// ... other core models
```

#### 2. Request/Response Models
```csharp
// PlanningCenter.Api.Client.Models/Requests/
public class PersonCreateRequest { }
public class PersonUpdateRequest { }
public class AddressCreateRequest { }
public class WorkflowCardCreateRequest { }
// ... other request models

// PlanningCenter.Api.Client.Models/Responses/
public class PagedResponse<T> : IPagedResponse<T> { }
public class QueryParameters { }
```

#### 3. Service Interface and Implementation
```csharp
// PlanningCenter.Api.Client.Abstractions/Services/
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
    // ... other methods
    
    // Workflow management
    Task<IPagedResponse<Workflow>> ListWorkflowsAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    // ... other workflow methods
    
    // Form management
    Task<IPagedResponse<Form>> ListFormsAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    // ... other form methods
    
    // List management
    Task<IPagedResponse<List>> ListPeopleListsAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    // ... other list methods
}

// PlanningCenter.Api.Client/Services/
public class PeopleService : IPeopleService
{
    private readonly IApiConnection _apiConnection;
    private readonly PersonMapper _personMapper;
    private readonly ICacheProvider _cacheProvider;
    
    // Full implementation
}
```

#### 4. Mapping Implementation
```csharp
// PlanningCenter.Api.Client/Mapping/
public class PersonMapper
{
    public Core.Person Map(People.PersonDto source) { }
    public People.PersonDto Map(Core.Person source) { }
    public List<Core.Person> Map(List<People.PersonDto> sources) { }
    // ... other mapping methods
}

public class AddressMapper { }
public class EmailMapper { }
// ... other mappers
```

#### 5. Fluent API Implementation
```csharp
// PlanningCenter.Api.Client.Abstractions/Fluent/
public interface IPlanningCenterClient
{
    IPeopleFluentContext People();
    // ... other modules
}

public interface IPeopleFluentContext
{
    IPeopleFluentContext Where(Expression<Func<Core.Person, bool>> predicate);
    IPeopleFluentContext Include(Expression<Func<Core.Person, object>> include);
    IPeopleFluentContext OrderBy(Expression<Func<Core.Person, object>> orderBy);
    
    Task<Core.Person> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Core.Person>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default);
    
    IWorkflowFluentContext Workflows();
    IFormFluentContext Forms();
    IListFluentContext Lists();
    IHouseholdFluentContext Households();
}

// PlanningCenter.Api.Client/Fluent/
public class PlanningCenterClient : IPlanningCenterClient
{
    public IPeopleFluentContext People() => new PeopleFluentContext(_serviceProvider);
}

public class PeopleFluentContext : IPeopleFluentContext
{
    // Full fluent API implementation
}
```

### Testing Requirements
- Unit tests for all People service methods
- Unit tests for all mapping functions
- Unit tests for fluent API operations
- Integration tests for key People operations
- Performance tests for large people lists

### Success Criteria
- [ ] All People API endpoints accessible through service
- [ ] Unified Person model working with all People data
- [ ] Fluent API providing intuitive query interface
- [ ] Caching working for frequently accessed data
- [ ] All tests passing with 90%+ coverage
- [ ] Integration tests working with real API

---

## Phase 3: Giving Module - Complete Implementation (Week 9-11)

### Objective
Implement the comprehensive Giving module, applying and refining patterns from the People module.

### Deliverables

#### 1. Giving DTOs and Models
```csharp
// PlanningCenter.Api.Client.Models/Giving/
public class DonationDto { }
public class FundDto { }
public class BatchDto { }
public class PledgeDto { }
public class RecurringDonationDto { }
public class RefundDto { }
public class PaymentSourceDto { }
public class GivingPersonDto { } // Giving-specific person variant

// Core models (if needed)
public class Donation { }
public class Fund { }
public class Batch { }
// ... other core giving models
```

#### 2. Service Implementation
```csharp
public interface IGivingService
{
    // Donation management
    Task<Donation> GetDonationAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Donation>> ListDonationsAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Donation> CreateDonationAsync(DonationCreateRequest request, CancellationToken cancellationToken = default);
    
    // Fund management
    Task<Fund> GetFundAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Fund>> ListFundsAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    
    // Batch management
    Task<Batch> GetBatchAsync(string id, CancellationToken cancellationToken = default);
    Task<Batch> CommitBatchAsync(string id, CancellationToken cancellationToken = default);
    
    // Refund management
    Task<Refund> IssueRefundAsync(string donationId, RefundCreateRequest request, CancellationToken cancellationToken = default);
    
    // Person-specific operations
    Task<Core.Person> GetPersonAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Donation>> GetDonationsForPersonAsync(string personId, QueryParameters parameters = null, CancellationToken cancellationToken = default);
    
    // Reporting
    Task<decimal> GetTotalGivingAsync(DateTime startDate, DateTime endDate, string fundId = null, CancellationToken cancellationToken = default);
}
```

#### 3. Enhanced Person Mapping
```csharp
public class PersonMapper
{
    // Add Giving-specific mapping
    public Core.Person Map(Giving.PersonDto source)
    {
        return new Core.Person
        {
            // Map giving-specific properties like TotalGiven, LastGiftDate, etc.
            SourceModule = "Giving"
        };
    }
}
```

#### 4. Fluent API Extensions
```csharp
public interface IGivingFluentContext
{
    IDonationFluentContext Donations();
    IFundFluentContext Funds();
    IBatchFluentContext Batches();
    IPledgeFluentContext Pledges();
    IPersonGivingFluentContext Person(string personId);
    IReportingFluentContext Reports();
}

public interface IDonationFluentContext
{
    IDonationFluentContext Where(Expression<Func<Donation, bool>> predicate);
    IDonationFluentContext ForPerson(string personId);
    IDonationFluentContext ForFund(string fundId);
    IDonationFluentContext ReceivedBetween(DateTime startDate, DateTime endDate);
    
    Task<IPagedResponse<Donation>> GetPagedAsync(int pageSize = 25, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalAmountAsync(CancellationToken cancellationToken = default);
}
```

### Success Criteria
- [ ] All Giving API endpoints accessible
- [ ] Financial data properly secured and validated
- [ ] Giving-specific Person data integrated with unified model
- [ ] Fluent API supporting complex giving queries
- [ ] All tests passing including financial calculations

---

## Phase 4: Calendar Module - Advanced Features (Week 12-14)

### Objective
Implement the Calendar module with enhanced query capabilities, includes, and pagination features.

### Deliverables

#### 1. Calendar Models
```csharp
// Calendar-specific models
public class EventDto { }
public class EventInstanceDto { }
public class ResourceDto { }
public class ConflictDto { }
public class AttachmentDto { }
public class CalendarPersonDto { } // Calendar-specific person variant
```

#### 2. Enhanced API Connection
```csharp
// Enhanced ApiConnection with better include and query support
public class ApiConnection : IApiConnection
{
    // Enhanced methods for complex queries
    public async Task<IPagedResponse<T>> GetPagedAsync<T>(
        string endpoint, 
        QueryParameters parameters = null, 
        CancellationToken cancellationToken = default)
    {
        // Handle include parameters, filtering, sorting
        // Support for JSON:API include syntax
        // Proper pagination with links
    }
}
```

#### 3. Calendar Service
```csharp
public interface ICalendarService
{
    // Event management
    Task<Event> GetEventAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Event>> ListEventsAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Event> CreateEventAsync(EventCreateRequest request, CancellationToken cancellationToken = default);
    
    // Resource management
    Task<Resource> GetResourceAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<Resource>> ListResourcesAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    
    // Conflict management
    Task<IPagedResponse<Conflict>> ListConflictsAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
    Task<Conflict> ResolveConflictAsync(string id, ConflictResolutionRequest request, CancellationToken cancellationToken = default);
    
    // Attachment management
    Task<Attachment> UploadAttachmentAsync(string eventId, AttachmentUploadRequest request, CancellationToken cancellationToken = default);
}
```

### Success Criteria
- [ ] Complex include parameters working
- [ ] Pagination with proper link handling
- [ ] Calendar-specific features implemented
- [ ] Resource conflict detection working

---

## Phase 5-7: Additional Modules Implementation (Week 15-23)

### Phase 5: Check-Ins and Groups Modules (Week 15-17)
### Phase 6: Registrations Module (Week 18-20)  
### Phase 7: Publishing and Services Modules (Week 21-23)

Each module follows the established patterns:
1. Create module-specific DTOs
2. Implement service interface and implementation
3. Add mapping for unified models where applicable
4. Implement fluent API context
5. Add comprehensive testing
6. Update dependency injection

---

## Phase 8: Webhooks Implementation (Week 24-26)

### Objective
Complete webhook subscription and event handling system.

### Deliverables

#### 1. Webhook Models
```csharp
public class WebhookSubscription { }
public class WebhookEvent { }
public class AvailableEvent { }
```

#### 2. Webhook Service
```csharp
public interface IWebhooksService
{
    Task<WebhookSubscription> CreateSubscriptionAsync(WebhookSubscriptionCreateRequest request, CancellationToken cancellationToken = default);
    Task<WebhookSubscription> UpdateSubscriptionAsync(string id, WebhookSubscriptionUpdateRequest request, CancellationToken cancellationToken = default);
    Task DeleteSubscriptionAsync(string id, CancellationToken cancellationToken = default);
    Task<IPagedResponse<WebhookEvent>> ListEventsAsync(QueryParameters parameters = null, CancellationToken cancellationToken = default);
}
```

#### 3. Event Handling Framework
```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPlanningCenterWebhooks(
        this IServiceCollection services,
        Action<WebhookOptions> configureOptions)
    {
        // Configure webhook event handlers
    }
}
```

---

## Phase 9: Advanced Features (Week 27-30)

### Objective
Implement sophisticated SDK capabilities for production use.

### Deliverables

#### 1. Real-time Synchronization
```csharp
public interface ISyncService
{
    Task<SyncResult> SyncAsync(SyncRequest request, CancellationToken cancellationToken = default);
    Task<ChangeSet> GetChangesAsync(DateTime since, CancellationToken cancellationToken = default);
}
```

#### 2. Bulk Operations
```csharp
public interface IBulkOperationService
{
    Task<BulkResult<T>> BulkCreateAsync<T>(IEnumerable<T> items, BulkOptions options = null, CancellationToken cancellationToken = default);
    Task<BulkResult<T>> BulkUpdateAsync<T>(IEnumerable<T> items, BulkOptions options = null, CancellationToken cancellationToken = default);
}
```

#### 3. Advanced Fluent API
```csharp
// Complex query building
var result = await client
    .People()
    .WithWorkflow("onboarding")
    .InStep("initial-contact")
    .AssignedTo(currentUserId)
    .DueWithin(TimeSpan.FromDays(7))
    .Include(p => p.Addresses)
    .GetAsync();
```

#### 4. Performance Monitoring
```csharp
public interface IPerformanceMonitor
{
    void RecordApiCall(string endpoint, TimeSpan duration, bool success);
    Task<PerformanceReport> GetReportAsync(TimeSpan period);
}
```

---

## Phase 10: Documentation and Examples (Week 31-33)

### Objective
Complete comprehensive documentation and real-world examples.

### Deliverables

#### 1. Enhanced Example Projects
- **Console Application:** Comprehensive examples for all modules
- **Fluent Console Application:** Advanced fluent API scenarios
- **Worker Service:** Background processing examples
- **ASP.NET Core Web API:** Integration examples
- **Blazor Application:** Interactive examples

#### 2. Documentation
- Complete XML documentation for all public APIs
- Comprehensive README with getting started guide
- Module-specific documentation
- Migration guides
- Best practices guide
- Troubleshooting guide

#### 3. NuGet Packages
- Main package: `PlanningCenter.Api.Client`
- Extensions: `PlanningCenter.Api.Client.Extensions.DependencyInjection`
- Webhooks: `PlanningCenter.Api.Client.Webhooks`
- Caching: `PlanningCenter.Api.Client.Caching.Redis`

### Success Criteria
- [ ] All documentation complete and accurate
- [ ] Example projects demonstrate all major features
- [ ] NuGet packages properly configured
- [ ] Performance benchmarks documented
- [ ] Migration guides available

---

## Quality Gates

Each phase must meet these criteria before proceeding:

### Code Quality
- [ ] 90%+ test coverage
- [ ] Zero code analysis warnings
- [ ] All public APIs documented
- [ ] Performance benchmarks within targets

### Functionality
- [ ] All planned features implemented
- [ ] Integration tests passing
- [ ] Error handling comprehensive
- [ ] Caching working effectively

### Documentation
- [ ] Phase deliverables documented
- [ ] API changes documented
- [ ] Examples updated
- [ ] Breaking changes noted

## Risk Mitigation

### Technical Risks
- **API Changes:** Implement versioning and backward compatibility
- **Performance Issues:** Early performance testing and optimization
- **Security Concerns:** Regular security reviews and testing

### Project Risks
- **Scope Creep:** Strict adherence to phase deliverables
- **Resource Constraints:** Prioritized development with clear MVP
- **Quality Issues:** Comprehensive testing at each phase

This phased approach ensures steady progress while maintaining high quality and allowing for early feedback and course correction.