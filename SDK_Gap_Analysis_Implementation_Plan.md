# SDK Gap Analysis Implementation Plan

## Phase A1.1: Expression Parsing Enhancement

### Overview
This phase focuses on enhancing the `ExpressionParser.cs` to support property mappings for all Planning Center modules, ensuring proper conversion from PascalCase to snake_case for API compatibility.

### Implementation Status

#### ✅ People Module
- **Status**: COMPLETED
- **Entities**: Person, Address, Email, PhoneNumber, FieldDatum, HouseholdMembership, InactiveReason, MaritalStatus, NamePrefix, NameSuffix, PlatformNotification, SchoolOption, SocialProfile, WorkflowCard, WorkflowStep, Note, PersonApp, Condition, List, Rule, Share, Campus, Carrier, FieldDefinition, FieldOption, Household, MessageGroup, Organization, PeopleImport, PeopleImportConflict, PeopleImportHistory, Report, Tab, Workflow
- **Implementation**: All property mappings added to `ConvertPropertyNameToApiField` method

#### ✅ Calendar Module
- **Status**: COMPLETED
- **Entities**: Event, EventInstance, Attachment, Category, EventConnection, EventResourceRequest, EventTime, Location, Organization, Person, Resource, ResourceApprovalGroup, ResourceBooking, ResourceFolder, ResourceQuestion, RoomSetup, Tag, TagGroup
- **Implementation**: All property mappings added to `ConvertPropertyNameToApiField` method

#### ✅ CheckIns Module
- **Status**: COMPLETED
- **Entities**: CheckIn, Event, EventTime, Headcount, Location, LocationEventPeriod, LocationEventTime, LocationLabel, Option, Organization, Pass, Person, PersonEvent, Station, Theme
- **Implementation**: All property mappings added to `ConvertPropertyNameToApiField` method

#### ✅ Giving Module
- **Status**: COMPLETED
- **Entities**: BatchGroup, Batch, Campus, Designation, DesignationRefund, Donation, Fund, Label, Organization, PaymentMethod, PaymentSource, Person, PledgeCampaign, Pledge, RecurringDonation, RecurringDonationDesignation, Refund
- **Implementation**: All property mappings added to `ConvertPropertyNameToApiField` method

#### ✅ Groups Module
- **Status**: COMPLETED
- **Entities**: Group, GroupType, Event, Enrollment, Location, Membership, Person, Resource, Tag, TagGroup
- **Implementation**: All property mappings added to `ConvertPropertyNameToApiField` method

#### ✅ Publishing Module
- **Status**: COMPLETED
- **Entities**: Episode, Series, Speaker, Speakership, DistributionChannel
- **Implementation**: All property mappings added to `ConvertPropertyNameToApiField` method

#### ✅ Services Module
- **Status**: COMPLETED
- **Entities**: ServiceType, PlanTemplate, Plan, Series, Song, Arrangement, Key, Attachment, Item, Media, Person, Team, TeamPosition, NeededPosition, PlanPerson, Schedule, BlockoutException, Blockout, PlanTime, SplitTeamRehearsalAssignment, Folder, Tag, TeamLeader, TimePreferenceOption, AvailableSignup, SignupSheet, SignupSheetMetadata, Live, Zoom
- **Implementation**: All property mappings added to `ConvertPropertyNameToApiField` method

#### ✅ Registrations Module
- **Status**: COMPLETED
- **Entities**: Signup, Registration, Attendee, Campus, Category, SelectionType, EmergencyContact, SignupLocation, SignupTime
- **Implementation**: All property mappings added to `ConvertPropertyNameToApiField` method

#### ✅ Webhooks Module
- **Status**: COMPLETED
- **Entities**: WebhookSubscription, Event, AvailableEvent
- **Implementation**: All property mappings added to `ConvertPropertyNameToApiField` method
- **Date Completed**: Current session

### Overall Phase A1.1 Status: ✅ COMPLETED (100%)

**Summary**: All Planning Center modules now have comprehensive property mappings in the `ExpressionParser.cs` file, ensuring proper PascalCase to snake_case conversion for API compatibility. This enhancement enables the fluent query builder to work seamlessly across all supported entities and their properties.

## Phase A2.1: Calendar Fluent Context Enhancement

### Overview
This phase focuses on upgrading the Calendar Fluent Context by adding missing LINQ operations, implementing advanced filtering, adding relationship include methods, and adding aggregation methods.

### Implementation Status

#### ✅ Calendar Fluent Context Enhancement
- **Status**: COMPLETED
- **Priority**: High
- **Estimated Effort**: 4-6 hours
- **Implementation**: All missing LINQ operations, advanced filtering methods, relationship include methods, and aggregation methods added

**Tasks Completed**:
1. ✅ Add missing LINQ operations (`Single`, `SingleOrDefault`)
2. ✅ Implement advanced filtering methods
3. ✅ Add relationship include methods
4. ✅ Add aggregation methods (`Sum`, `Average`, `Min`, `Max`, `GroupBy`)

**Files Modified**:
- `CalendarFluentContext.cs`
- `IModuleFluentContexts.cs` (interface)

**Acceptance Criteria**:
- ✅ All missing LINQ operations are implemented
- ✅ Advanced filtering methods are available
- ✅ Relationship include methods work correctly
- ✅ Aggregation methods return accurate results
- ✅ All methods follow the established fluent API patterns
- ✅ Code compiles successfully

### Overall Phase A2.1 Status: ✅ COMPLETED (100%)

## Phase A2.2: Groups Fluent Context Enhancement

### Implementation Status

#### ✅ Groups Fluent Context Enhancement
- **Status**: COMPLETED
- **Priority**: High
- **Estimated Effort**: 4-6 hours
- **Implementation**: All missing LINQ operations, specialized group filtering methods, member relationship querying, and group hierarchy navigation added

**Tasks Completed**:
1. ✅ Add missing LINQ operations (`Single`, `SingleOrDefault`)
2. ✅ Implement specialized group filtering methods (`ByGroupType`, `ByMembershipType`, `ByStatus`)
3. ✅ Add member relationship querying methods (`WithMembers`, `ByMemberCount`)
4. ✅ Add group hierarchy navigation methods (`WithParentGroup`, `WithChildGroups`)

**Files Modified**:
- `GroupsFluentContext.cs`
- `IModuleFluentContexts.cs` (interface)

### Overall Phase A2.2 Status: ✅ COMPLETED (100%)

## Phase A2.3: Services Fluent Context Enhancement

### Overview
This phase focuses on upgrading the Services Fluent Context by implementing full LINQ-like syntax matching the People module, adding service-specific filtering methods, plan and item relationship querying, and team member filtering.

### Implementation Status

#### ✅ Services Fluent Context Enhancement
- **Status**: COMPLETED
- **Priority**: High
- **Estimated Effort**: 4-6 hours
- **Implementation**: All missing LINQ operations, service-specific filtering methods, plan and item relationship querying, and team member filtering methods added

**Tasks Completed**:
1. ✅ Add service-specific filtering methods (`ByDate`, `ByStatus`, `WithSeries`, `WithoutSeries`)
2. ✅ Implement plan and item relationship querying methods (`WithPlans`, `WithPlanItems`, `ByPlanType`, `WithSongs`, `WithMedia`)
3. ✅ Add team member filtering methods (`WithTeamMembers`, `ByTeamRole`, `ByTeamPosition`, `WithConfirmedTeamMembers`, `WithDeclinedTeamMembers`, `WithMinimumTeamMembers`)
4. ✅ Add advanced aggregation methods (`CountByServiceTypeAsync`, `CountPublicPlansAsync`, `CountPrivatePlansAsync`, `AveragePlanLengthAsync`)

**Files Modified**:
- `ServicesFluentContext.cs`
- `IModuleFluentContexts.cs` (interface)

**Acceptance Criteria**:
- ✅ All service-specific filtering methods are implemented
- ✅ Plan and item relationship querying methods work correctly
- ✅ Team member filtering methods are available
- ✅ Advanced aggregation methods return accurate results
- ✅ All methods follow the established fluent API patterns
- ✅ Code compiles successfully

### Overall Phase A2.3 Status: ✅ COMPLETED (100%)

### Next Phase
Ready to proceed with Phase A2.4 or other implementation phases as defined in the project roadmap.

## Phase A2.5: Giving Fluent Context Enhancement

### Overview
This phase focuses on upgrading the Giving Fluent Context by adding missing LINQ operations, donation-specific filtering methods, designation relationship querying methods, and payment method filtering methods.

### Implementation Status

#### ✅ Giving Fluent Context Enhancement
- **Status**: COMPLETED
- **Priority**: High
- **Estimated Effort**: 4-6 hours
- **Implementation**: All missing LINQ operations, donation-specific filtering methods, designation relationship querying, and payment method filtering methods added. Also resolved FluentQueryBuilder interface implementation issues.

**Tasks Completed**:
1. ✅ Add missing LINQ operations (`SingleAsync`, `SingleOrDefaultAsync` with predicate overloads)
2. ✅ Implement donation-specific filtering methods (`ByStatus`, `ByBatch`, `WithMaximumAmount`, `ByAmountRange`)
3. ✅ Add designation relationship querying methods (`WithDesignations`, `ByDesignation`, `WithMultipleDesignations`, `ByDesignationCount`)
4. ✅ Add payment method filtering methods (`ByPaymentMethod`, `ByTransactionId`, `CashOnly`, `CheckOnly`, `CreditCardOnly`, `AchOnly`)
5. ✅ Add advanced aggregation methods (`AverageAmountAsync`, `MaxAmountAsync`, `MinAmountAsync`, `CountByFundAsync`, `CountByPaymentMethodAsync`)
6. ✅ Fixed FluentQueryBuilder to implement IFluentQueryBuilder<T> interface
7. ✅ Added string-based Include, Where, OrderBy methods to FluentQueryBuilder
8. ✅ Resolved compilation errors and ensured successful build

**Files Modified**:
- `GivingFluentContext.cs`
- `IModuleFluentContexts.cs` (interface)
- `FluentQueryBuilder.cs` (interface implementation fixes)

**Acceptance Criteria**:
- ✅ All missing LINQ operations with predicate overloads are implemented
- ✅ Donation-specific filtering methods are available
- ✅ Designation relationship querying methods work correctly
- ✅ Payment method filtering methods provide comprehensive filtering options
- ✅ Advanced aggregation methods return accurate results
- ✅ All methods follow the established fluent API patterns
- ✅ Code compiles successfully
- ✅ FluentQueryBuilder properly implements required interfaces

### Overall Phase A2.5 Status: ✅ COMPLETED (100%)

**Summary**: The Giving Fluent Context now has comprehensive LINQ-like syntax with specialized donation filtering, designation relationship querying, payment method filtering, and advanced aggregation capabilities. Resolved critical interface implementation issues in FluentQueryBuilder to ensure proper compilation. The implementation provides developers with an intuitive and powerful interface for working with giving data in Planning Center.

### Next Phase
Ready to proceed with Phase A2.6 (Publishing Fluent Context Enhancement) or other implementation phases as defined in the project roadmap.

## Phase A2.6: Publishing Fluent Context Enhancement
### Overview

This phase focuses on upgrading the Publishing Fluent Context by adding missing LINQ operations, publishing-specific filtering methods, series and episode relationship querying methods, speaker filtering methods, and media management capabilities.

### Implementation Status
#### 🔄 Publishing Fluent Context Enhancement

- **Status**: IN PROGRESS
- **Priority**: High
- **Estimated Effort**: 4-6 hours
- **Implementation**: Adding missing LINQ operations, publishing-specific filtering methods, series/episode relationship querying, speaker filtering, and media management methods

**Tasks**:
1. ⏳ Add missing LINQ operations (already completed `SingleAsync`, `SingleOrDefaultAsync` with predicate overloads)
2. ⏳ Implement publishing-specific filtering methods (`Draft`, `BySpeaker`, `BySeriesId`, `WithoutSeries`)
3. ⏳ Add series and episode relationship querying methods (`InSeries`, `Episodes`, `Series`, `Speakers`)
4. ⏳ Add speaker filtering methods (`BySpeaker`, `WithSpeakers`, `WithoutSpeakers`)
5. ⏳ Add media management methods (`WithMedia`, `WithoutMedia`, `ByMediaType`, `WithDownloadableMedia`, `WithStreamableMedia`)
6. ⏳ Add advanced aggregation methods (`TotalSeriesAsync`, `TotalSpeakersAsync`, `AverageEpisodeCountAsync`)

**Files to Modify**:
- `PublishingFluentContext.cs`
- `IModuleFluentContexts.cs` (interface)

**Acceptance Criteria**:
- ⏳ All publishing-specific filtering methods are implemented
- ⏳ Series and episode relationship querying methods work correctly
- ⏳ Speaker filtering methods are available
- ⏳ Media management methods are functional
- ⏳ Advanced aggregation methods return accurate results
- ⏳ All methods follow the established fluent API patterns
- ⏳ Code compiles successfully

## Phase A3.1: Complex Filtering

### Overview
This phase implements advanced filtering capabilities including nested AND/OR query combinations, IN/NOT IN operations, date range filtering helpers, and null/not null filtering across all fluent contexts.

### Implementation Status

#### ✅ Complex Filtering Enhancement
- **Status**: COMPLETED
- **Priority**: High
- **Estimated Effort**: 6-8 hours
- **Implementation**: All complex filtering features successfully implemented with comprehensive test coverage

**Tasks Completed**:
1. ✅ Nested AND/OR query combinations (`WhereAnd`, `WhereOr` methods)
2. ✅ IN/NOT IN operations (`WhereIn`, `WhereNotIn` methods)
3. ✅ Date range filtering helpers (`WhereDateRange` methods)
4. ✅ Null/not null filtering (`WhereNull`, `WhereNotNull` methods)
5. ✅ Numeric range filtering (`WhereBetween` method)
6. ✅ Enhanced ExpressionParser with new FilterOperator support
7. ✅ Updated IFluentQueryBuilder interface with new method signatures
8. ✅ Comprehensive unit test coverage (20+ tests, all passing)
9. ✅ Complete documentation and usage examples

**Files Modified**:
- `FluentQueryBuilder.cs` - Core implementation
- `IFluentQueryBuilder.cs` - Interface definitions
- `ExpressionParser.cs` - Enhanced expression parsing
- `FluentQueryBuilderTests.cs` - Comprehensive test coverage
- Documentation files

**Acceptance Criteria**:
- ✅ All complex filtering methods are implemented
- ✅ Nested AND/OR combinations work correctly
- ✅ IN/NOT IN operations handle collections properly
- ✅ Date range filtering supports DateTime and nullable DateTime
- ✅ Null/not null filtering works across all property types
- ✅ All methods follow established fluent API patterns
- ✅ Comprehensive test coverage with all tests passing
- ✅ Code compiles successfully

### Overall Phase A3.1 Status: ✅ COMPLETED (100%)

**Summary**: Complex Filtering capabilities have been fully implemented, providing developers with sophisticated query construction tools including logical combinations, collection-based filtering, date ranges, and null checks. All features are thoroughly tested and documented.

## Phase A3.2: Relationship Querying

### Overview
This phase implements comprehensive relationship querying capabilities including deep relationship includes, relationship filtering, and relationship counting operations across all fluent contexts.

### Implementation Status

#### ✅ Relationship Querying Enhancement
- **Status**: COMPLETED
- **Priority**: High
- **Estimated Effort**: 6-8 hours
- **Implementation**: All relationship querying features successfully implemented with comprehensive test coverage

**Tasks Completed**:
1. ✅ Deep relationship includes (`IncludeDeep` method for multi-level navigation)
2. ✅ Relationship existence filtering (`WhereHasRelationship`, `WhereDoesntHaveRelationship`)
3. ✅ Relationship condition filtering (`WhereHas` with single and multiple conditions)
4. ✅ Relationship counting operations (`WhereRelationshipCount*` methods)
5. ✅ Enhanced IFluentQueryBuilder interface with 8 new method signatures
6. ✅ Implementation in both FluentQueryBuilder and FluentQueryBuilderBase classes
7. ✅ Comprehensive unit test coverage (20 tests, all passing)
8. ✅ Complete documentation with practical examples

**Files Modified**:
- `IFluentQueryBuilder.cs` - Interface definitions for relationship querying
- `FluentQueryBuilder.cs` - Core relationship querying implementation
- `FluentQueryBuilderBase.cs` - Base class implementation
- `FluentQueryBuilderTests.cs` - Comprehensive test coverage
- `A3.2_Relationship_Querying_Implementation.md` - Complete documentation

**New Methods Added**:
- `IncludeDeep(string relationshipPath)` - Deep relationship includes
- `IncludeDeep(params string[] relationshipPaths)` - Multiple deep includes
- `WhereHasRelationship(string relationshipName)` - Relationship existence
- `WhereDoesntHaveRelationship(string relationshipName)` - Relationship absence
- `WhereHas(string relationshipName, string field, object value)` - Single condition
- `WhereHas(string relationshipName, Dictionary<string, object> filters)` - Multiple conditions
- `WhereRelationshipCountEquals(string relationshipName, int count)` - Exact count
- `WhereRelationshipCountGreaterThan(string relationshipName, int count)` - Minimum count

**Acceptance Criteria**:
- ✅ Deep relationship includes support multi-level navigation
- ✅ Relationship existence filtering works correctly
- ✅ Relationship condition filtering supports single and multiple conditions
- ✅ Relationship counting operations provide accurate filtering
- ✅ All methods integrate seamlessly with existing fluent API
- ✅ Comprehensive test coverage with all tests passing (258 total Fluent API tests)
- ✅ Complete documentation with practical examples
- ✅ Code compiles successfully

### Overall Phase A3.2 Status: ✅ COMPLETED (100%)

**Summary**: Relationship Querying capabilities have been fully implemented, enabling developers to work with complex entity relationships through deep includes, relationship filtering, and counting operations. All features are thoroughly tested, documented, and integrate seamlessly with existing functionality.

## Phase A3.3: Aggregation Support

### Overview
This phase implements comprehensive aggregation capabilities including Count, Sum, Average, Min, Max operations, GroupBy functionality, and Having clause support across all fluent contexts.

### Implementation Status

#### 🔄 Aggregation Support Enhancement
- **Status**: IN PROGRESS
- **Priority**: High
- **Estimated Effort**: 6-8 hours
- **Implementation**: Adding comprehensive aggregation operations with GroupBy and Having clause support

**Tasks**:
1. ⏳ Add basic aggregation methods (`CountAsync`, `SumAsync`, `AverageAsync`, `MinAsync`, `MaxAsync`)
2. ⏳ Implement GroupBy functionality (`GroupBy` method with key selector)
3. ⏳ Add Having clause support (`Having` method for grouped results filtering)
4. ⏳ Add aggregation with conditions (`CountWhereAsync`, `SumWhereAsync`, etc.)
5. ⏳ Implement distinct aggregations (`CountDistinctAsync`, `SumDistinctAsync`)
6. ⏳ Add grouped aggregation results (`GroupedResult<TKey, TValue>` class)
7. ⏳ Enhanced IFluentQueryBuilder interface with aggregation method signatures
8. ⏳ Comprehensive unit test coverage
9. ⏳ Complete documentation with practical examples

**Files to Modify**:
- `IFluentQueryBuilder.cs` - Interface definitions for aggregation methods
- `FluentQueryBuilder.cs` - Core aggregation implementation
- `FluentQueryBuilderBase.cs` - Base class aggregation support
- `FluentQueryBuilderTests.cs` - Comprehensive test coverage
- Documentation files

**New Methods to Add**:
- `CountAsync()` - Count all records
- `CountAsync(Expression<Func<T, bool>> predicate)` - Count with condition
- `CountDistinctAsync<TProperty>(Expression<Func<T, TProperty>> selector)` - Distinct count
- `SumAsync<TProperty>(Expression<Func<T, TProperty>> selector)` - Sum operation
- `SumAsync<TProperty>(Expression<Func<T, TProperty>> selector, Expression<Func<T, bool>> predicate)` - Sum with condition
- `AverageAsync<TProperty>(Expression<Func<T, TProperty>> selector)` - Average operation
- `MinAsync<TProperty>(Expression<Func<T, TProperty>> selector)` - Minimum value
- `MaxAsync<TProperty>(Expression<Func<T, TProperty>> selector)` - Maximum value
- `GroupBy<TKey>(Expression<Func<T, TKey>> keySelector)` - Group by key
- `Having<TKey>(Expression<Func<IGrouping<TKey, T>, bool>> predicate)` - Having clause

**Acceptance Criteria**:
- ⏳ All basic aggregation methods are implemented
- ⏳ GroupBy functionality works correctly with various key types
- ⏳ Having clause filtering works on grouped results
- ⏳ Aggregation with conditions provides accurate results
- ⏳ Distinct aggregations handle duplicates properly
- ⏳ All methods integrate seamlessly with existing fluent API
- ⏳ Comprehensive test coverage with all tests passing
- ⏳ Complete documentation with practical examples
- ⏳ Code compiles successfully