using System.Linq.Expressions;
using System.Reflection;
using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Fluent.ExpressionParsing;

/// <summary>
/// Advanced expression parser that converts LINQ expressions to Planning Center API parameters.
/// Supports complex expressions including comparisons, method calls, and property access.
/// </summary>
public static class ExpressionParser
{
    /// <summary>
    /// Converts a boolean expression to API filter parameters for Person entities.
    /// </summary>
    public static FilterResult ParseFilter(Expression<Func<Person, bool>> expression)
    {
        if (expression == null) throw new ArgumentNullException(nameof(expression));

        try
        {
            return ParseBooleanExpression(expression.Body, typeof(Person));
        }
        catch
        {
            // If parsing fails, return empty result rather than throwing
            // This allows graceful degradation to client-side filtering
            return new FilterResult();
        }
    }

    /// <summary>
    /// Converts a property access expression to API include parameter for Person entities.
    /// </summary>
    public static string ParseInclude(Expression<Func<Person, object>> expression)
    {
        if (expression == null) throw new ArgumentNullException(nameof(expression));

        try
        {
            return ParsePropertyAccess(expression.Body, typeof(Person));
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Converts a property access expression to API sort parameter for Person entities.
    /// </summary>
    public static string ParseSort(Expression<Func<Person, object>> expression)
    {
        if (expression == null) throw new ArgumentNullException(nameof(expression));

        try
        {
            return ParsePropertyAccess(expression.Body, typeof(Person)) ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Generic method to convert a boolean expression to API filter parameters.
    /// </summary>
    public static FilterResult ParseFilter<T>(Expression<Func<T, bool>> expression) where T : class
    {
        if (expression == null) throw new ArgumentNullException(nameof(expression));

        try
        {
            return ParseBooleanExpression(expression.Body, typeof(T));
        }
        catch
        {
            return new FilterResult();
        }
    }

    /// <summary>
    /// Generic method to convert a property access expression to API include parameter.
    /// </summary>
    public static string ParseInclude<T>(Expression<Func<T, object>> expression) where T : class
    {
        if (expression == null) throw new ArgumentNullException(nameof(expression));

        try
        {
            return ParsePropertyAccess(expression.Body, typeof(T)) ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Generic method to convert a property access expression to API sort parameter.
    /// </summary>
    public static string ParseSort<T>(Expression<Func<T, object>> expression) where T : class
    {
        if (expression == null) throw new ArgumentNullException(nameof(expression));

        try
        {
            return ParsePropertyAccess(expression.Body, typeof(T)) ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    private static FilterResult ParseBooleanExpression(Expression expression, Type entityType)
    {
        switch (expression)
        {
            case BinaryExpression binary:
                return ParseBinaryExpression(binary, entityType);
            
            case MethodCallExpression method:
                return ParseMethodCallExpression(method, entityType);
            
            case UnaryExpression unary when unary.NodeType == ExpressionType.Not:
                var innerResult = ParseBooleanExpression(unary.Operand, entityType);
                return innerResult.Negate();
            
            default:
                return new FilterResult();
        }
    }

    private static FilterResult ParseBinaryExpression(BinaryExpression binary, Type entityType)
    {
        // Handle logical operators (AND/OR)
        if (binary.NodeType == ExpressionType.AndAlso)
        {
            var left = ParseBooleanExpression(binary.Left, entityType);
            var right = ParseBooleanExpression(binary.Right, entityType);
            return FilterResult.Combine(left, right, LogicalOperator.And);
        }

        if (binary.NodeType == ExpressionType.OrElse)
        {
            var left = ParseBooleanExpression(binary.Left, entityType);
            var right = ParseBooleanExpression(binary.Right, entityType);
            return FilterResult.Combine(left, right, LogicalOperator.Or);
        }

        // Handle comparison operators
        string apiFieldName;
        
        // Check if left side is a method call (e.g., p.Emails.Count())
        if (binary.Left is MethodCallExpression methodCall)
        {
            // Handle collection navigation methods
            switch (methodCall.Method.Name)
            {
                case "Count":
                case "Any":
                    // For LINQ extension methods, the collection is passed as the first argument
                    if (methodCall.Object is MemberExpression member)
                    {
                        var propertyPath = ExtractNestedPropertyPath(member);
                        apiFieldName = ConvertNestedPropertyToApiField(propertyPath, entityType);
                    }
                    else if (methodCall.Arguments.Count > 0 && methodCall.Arguments[0] is MemberExpression argMember)
                    {
                        // Handle LINQ extension methods where collection is first argument
                        var propertyPath = ExtractNestedPropertyPath(argMember);
                        apiFieldName = ConvertNestedPropertyToApiField(propertyPath, entityType);
                    }
                    else
                    {
                        return new FilterResult();
                    }
                    break;
                default:
                    var propertyName = ExtractPropertyName(binary.Left);
                    if (string.IsNullOrEmpty(propertyName))
                        return new FilterResult();
                    apiFieldName = ConvertPropertyNameToApiField(propertyName, entityType);
                    break;
            }
        }
        else
        {
            var propertyName = ExtractPropertyName(binary.Left);
            if (string.IsNullOrEmpty(propertyName))
                return new FilterResult();
            apiFieldName = ConvertPropertyNameToApiField(propertyName, entityType);
        }
        
        var value = ExtractValue(binary.Right);
        var filterOperator = ConvertExpressionTypeToFilterOperator(binary.NodeType);

        // Handle null comparisons
        if (value == null)
        {
            // Convert == null to IsNull, != null to IsNotNull
            if (binary.NodeType == ExpressionType.Equal)
            {
                filterOperator = FilterOperator.IsNull;
            }
            else if (binary.NodeType == ExpressionType.NotEqual)
            {
                filterOperator = FilterOperator.IsNotNull;
            }
            else
            {
                return new FilterResult(); // Other operators with null don't make sense
            }
        }

        return new FilterResult
        {
            Field = apiFieldName,
            Operator = filterOperator,
            Value = value
        };
    }

    private static FilterResult ParseMethodCallExpression(MethodCallExpression method, Type entityType)
    {
        // Handle collection navigation methods (Any, Count, etc.)
        switch (method.Method.Name)
        {
            case "Any":
            case "Count":
            case "First":
            case "FirstOrDefault":
            case "Single":
            case "SingleOrDefault":
                if (method.Object is MemberExpression member)
                {
                    var propertyPath = ExtractNestedPropertyPath(member);
                    var collectionFieldName = ConvertNestedPropertyToApiField(propertyPath, entityType);
                    return new FilterResult
                    {
                        Field = collectionFieldName,
                        Operator = FilterOperator.Equal,
                        Value = "exists"
                    };
                }
                else if (method.Arguments.Count > 0 && method.Arguments[0] is MemberExpression argMember)
                {
                    // Handle LINQ extension methods where collection is first argument
                    var propertyPath = ExtractNestedPropertyPath(argMember);
                    var collectionFieldName = ConvertNestedPropertyToApiField(propertyPath, entityType);
                    return new FilterResult
                    {
                        Field = collectionFieldName,
                        Operator = FilterOperator.Equal,
                        Value = "exists"
                    };
                }
                break;
                
            // Handle IN/NOT IN operations using Contains on collections
            case "Contains" when method.Object != null && IsCollection(method.Object.Type):
                // Handle collection.Contains(property) - IN operation
                if (method.Arguments.Count > 0)
                {
                    var propertyName = ExtractPropertyName(method.Arguments[0]);
                    if (!string.IsNullOrEmpty(propertyName))
                    {
                        var apiFieldName = ConvertPropertyNameToApiField(propertyName, entityType);
                        var collectionValue = ExtractValue(method.Object);
                        return new FilterResult
                        {
                            Field = apiFieldName,
                            Operator = FilterOperator.In,
                            Value = collectionValue
                        };
                    }
                }
                break;
        }
        
        // Handle string methods on properties
        var methodPropertyName = ExtractPropertyName(method.Object);
        if (string.IsNullOrEmpty(methodPropertyName))
            return new FilterResult();

        var methodApiFieldName = ConvertPropertyNameToApiField(methodPropertyName, entityType);

        switch (method.Method.Name)
        {
            case "Contains":
                var containsValue = ExtractValue(method.Arguments[0]);
                return new FilterResult
                {
                    Field = methodApiFieldName,
                    Operator = FilterOperator.Contains,
                    Value = containsValue
                };

            case "StartsWith":
                var startsWithValue = ExtractValue(method.Arguments[0]);
                return new FilterResult
                {
                    Field = methodApiFieldName,
                    Operator = FilterOperator.StartsWith,
                    Value = startsWithValue
                };

            case "EndsWith":
                var endsWithValue = ExtractValue(method.Arguments[0]);
                return new FilterResult
                {
                    Field = methodApiFieldName,
                    Operator = FilterOperator.EndsWith,
                    Value = endsWithValue
                };

            // Handle null checks
            case "IsNull":
                return new FilterResult
                {
                    Field = methodApiFieldName,
                    Operator = FilterOperator.IsNull,
                    Value = null
                };

            case "IsNotNull":
                return new FilterResult
                {
                    Field = methodApiFieldName,
                    Operator = FilterOperator.IsNotNull,
                    Value = null
                };

            default:
                return new FilterResult();
        }
    }

    /// <summary>
    /// Checks if a type represents a collection.
    /// </summary>
    private static bool IsCollection(Type type)
    {
        return type != typeof(string) && 
               (type.IsArray || 
                type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)));
    }

    private static string ParsePropertyAccess(Expression expression, Type entityType)
    {
        // Handle unboxing for object return types
        if (expression is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
        {
            expression = unary.Operand;
        }

        if (expression is MemberExpression member)
        {
            // Handle nested property access (e.g., person.households.members)
            var propertyPath = ExtractNestedPropertyPath(member);
            if (propertyPath.Count > 1)
            {
                return ConvertNestedPropertyToApiField(propertyPath, entityType);
            }
            
            var propertyName = member.Member.Name;
            return ConvertPropertyNameToApiField(propertyName, entityType);
        }

        // Handle method calls on collections (e.g., person.households.Any())
        if (expression is MethodCallExpression methodCall)
        {
            return ParseCollectionNavigationExpression(methodCall, entityType);
        }

        return string.Empty;
    }

    private static string ExtractPropertyName(Expression? expression)
    {
        if (expression is MemberExpression member)
        {
            // Handle nested property access
            var propertyPath = ExtractNestedPropertyPath(member);
            return string.Join(".", propertyPath);
        }
        
        // Handle method calls on collections (e.g., p.Emails.Count())
        if (expression is MethodCallExpression methodCall)
        {
            if (methodCall.Object is MemberExpression methodMember)
            {
                var propertyPath = ExtractNestedPropertyPath(methodMember);
                return string.Join(".", propertyPath);
            }
            else if (methodCall.Arguments.Count > 0 && methodCall.Arguments[0] is MemberExpression argMember)
            {
                // Handle LINQ extension methods where collection is first argument
                var propertyPath = ExtractNestedPropertyPath(argMember);
                return string.Join(".", propertyPath);
            }
        }

        return string.Empty;
    }

    /// <summary>
    /// Extracts the full property path from a nested member expression.
    /// For example: person.households.members -> ["person", "households", "members"]
    /// </summary>
    private static List<string> ExtractNestedPropertyPath(MemberExpression member)
    {
        var path = new List<string>();
        var current = member;
        
        while (current != null)
        {
            path.Insert(0, current.Member.Name);
            
            if (current.Expression is MemberExpression parentMember)
            {
                current = parentMember;
            }
            else if (current.Expression is ParameterExpression)
            {
                // We've reached the parameter (e.g., "person" in person.households)
                // Don't include the parameter name in the path
                break;
            }
            else
            {
                break;
            }
        }
        
        return path;
    }

    /// <summary>
    /// Converts nested property paths to Planning Center API include/filter syntax.
    /// </summary>
    private static string ConvertNestedPropertyToApiField(List<string> propertyPath, Type entityType)
    {
        var apiPath = new List<string>();
        
        foreach (var property in propertyPath)
        {
            var apiField = ConvertSinglePropertyToApiField(property, entityType);
            apiPath.Add(apiField);
        }
        
        return string.Join(".", apiPath);
    }

    /// <summary>
    /// Handles collection navigation expressions like person.households.Any() or person.emails.Count().
    /// </summary>
    private static string ParseCollectionNavigationExpression(MethodCallExpression methodCall, Type entityType)
    {
        // Handle LINQ methods on collections
        switch (methodCall.Method.Name)
        {
            case "Any":
            case "Count":
            case "First":
            case "FirstOrDefault":
            case "Single":
            case "SingleOrDefault":
                if (methodCall.Object is MemberExpression member)
                {
                    var propertyPath = ExtractNestedPropertyPath(member);
                    return ConvertNestedPropertyToApiField(propertyPath, entityType);
                }
                break;
                
            case "Where":
                // Handle filtered collections like person.households.Where(h => h.Primary)
                if (methodCall.Object is MemberExpression collectionMember)
                {
                    var propertyPath = ExtractNestedPropertyPath(collectionMember);
                    var basePath = ConvertNestedPropertyToApiField(propertyPath, entityType);
                    
                    // Extract the filter condition if possible
                    if (methodCall.Arguments.Count > 0 && methodCall.Arguments[0] is LambdaExpression lambda)
                    {
                        var filterResult = ParseBooleanExpression(lambda.Body, entityType);
                        if (!filterResult.IsEmpty)
                        {
                            return $"{basePath}[{filterResult.Field}={filterResult.Value}]";
                        }
                    }
                    
                    return basePath;
                }
                break;
        }
        
        return string.Empty;
    }

    /// <summary>
    /// Converts a single property name to API field format, with context-aware mapping.
    /// </summary>
    private static string ConvertSinglePropertyToApiField(string propertyName, Type entityType)
    {
        // Use the existing mapping logic but make it more flexible for nested properties
        return ConvertPropertyNameToApiField(propertyName, entityType);
    }

    private static object? ExtractValue(Expression expression)
    {
        try
        {
            // Handle constants
            if (expression is ConstantExpression constant)
            {
                return constant.Value;
            }

            // Handle member access (e.g., DateTime.Now, variables)
            if (expression is MemberExpression member)
            {
                if (member.Expression is ConstantExpression memberConstant)
                {
                    var container = memberConstant.Value;
                    if (member.Member is FieldInfo field)
                    {
                        return field.GetValue(container);
                    }
                    if (member.Member is PropertyInfo property)
                    {
                        return property.GetValue(container);
                    }
                }
            }

            // Handle method calls (e.g., DateTime.Now.AddDays(-30))
            if (expression is MethodCallExpression methodCall)
            {
                var compiled = Expression.Lambda(methodCall).Compile();
                return compiled.DynamicInvoke();
            }

            // Fallback: compile and execute the expression
            var lambda = Expression.Lambda(expression);
            var compiledLambda = lambda.Compile();
            return compiledLambda.DynamicInvoke();
        }
        catch
        {
            return string.Empty;
        }
    }

    private static string ConvertPropertyNameToApiField(string propertyName, Type entityType)
    {
        // Convert C# property names to Planning Center API field names
        // This follows the convention of converting PascalCase to snake_case
        
        // Handle specific property mappings based on entity type
        if (entityType.Name == "Person")
        {
            var personMappings = new Dictionary<string, string>
            {
                { "FirstName", "first_name" },
                { "LastName", "last_name" },
                { "MiddleName", "middle_name" },
                { "EmailAddress", "email" },
                { "PhoneNumber", "phone_number" },
                { "CreatedAt", "created_at" },
                { "UpdatedAt", "updated_at" },
                { "Id", "id" },
                { "Nickname", "nickname" },
                { "Birthdate", "birthdate" },
                { "Anniversary", "anniversary" },
                { "Gender", "gender" },
                { "Grade", "grade" },
                { "Child", "child" },
                { "Status", "status" },
                { "SchoolType", "school_type" },
                { "GraduationYear", "graduation_year" },
                { "Site", "site" },
                { "MedicalConditions", "medical_conditions" },
                { "MedicalNotes", "medical_notes" },
                { "Avatar", "avatar" },
                { "Name", "name" },
                { "DemographicAvatarUrl", "demographic_avatar_url" },
                { "DirectoryAvatarUrl", "directory_avatar_url" },
                { "PassedBackgroundCheck", "passed_background_check" },
                { "CanBeEmailed", "can_be_emailed" },
                { "CanBeSmsed", "can_be_smsed" },
                { "ReceivedRemoteAccess", "received_remote_access" },
                { "SiteId", "site_id" },
                { "PrimaryCampusId", "primary_campus_id" },
                { "PeopleImports", "people_imports" },
                { "Households", "households" },
                { "InactiveReason", "inactive_reason" },
                { "MembershipDate", "membership_date" },
                { "Emails", "emails" },
                { "Addresses", "addresses" },
                { "PhoneNumbers", "phone_numbers" },
                { "FieldData", "field_data" },
                { "NamePrefix", "name_prefix" },
                { "NameSuffix", "name_suffix" },
                { "OrganizationName", "organization_name" },
                { "Workplace", "workplace" },
                { "JobTitle", "job_title" },
                { "RemoteAccess", "remote_access" },
                { "AccountCenterIdentifier", "account_center_identifier" },
                { "IncludedSmsContacts", "included_sms_contacts" },
                { "ExcludedSmsContacts", "excluded_sms_contacts" },
                { "PlatformNotifications", "platform_notifications" },
                { "Apps", "apps" },
                { "ConnectedPeople", "connected_people" },
                { "HouseholdMemberships", "household_memberships" },
                { "MaritalStatus", "marital_status" },
                { "MessageGroupMemberships", "message_group_memberships" },
                { "Messages", "messages" },
                { "Notes", "notes" },
                { "PersonApps", "person_apps" },
                { "PrimaryCampus", "primary_campus" },
                { "ProfilePicture", "profile_picture" },
                { "SchoolOptions", "school_options" },
                { "SocialProfiles", "social_profiles" },
                { "Workflows", "workflows" },
                // Navigation properties for nested access
                { "Household", "household" },
                { "Address", "address" },
                { "City", "city" },
                { "State", "state" },
                { "Zip", "zip" },
                { "Primary", "primary" },
                { "Members", "members" }
            };

            if (personMappings.TryGetValue(propertyName, out var personMappedName))
                return personMappedName;
        }
        else if (entityType.Name == "Event")
        {
            var eventMappings = new Dictionary<string, string>
            {
                { "Name", "name" },
                { "Summary", "summary" },
                { "Description", "description" },
                { "Location", "location" },
                { "StartsAt", "starts_at" },
                { "EndsAt", "ends_at" },
                { "AllDayEvent", "all_day_event" },
                { "CreatedAt", "created_at" },
                { "UpdatedAt", "updated_at" },
                { "Id", "id" },
                { "Visible", "visible" },
                { "Approved", "approved" },
                { "Details", "details" },
                { "Recurrence", "recurrence" },
                { "RecurrenceDescription", "recurrence_description" },
                { "PercentComplete", "percent_complete" },
                { "Completed", "completed" },
                { "Color", "color" },
                { "Url", "url" },
                { "HtmlUrl", "html_url" },
                { "IcalUrl", "ical_url" },
                { "Owner", "owner" },
                { "EventInstances", "event_instances" },
                { "EventResourceRequests", "event_resource_requests" },
                { "Attachments", "attachments" },
                { "Tags", "tags" },
                // Navigation properties for nested access
                { "Instances", "instances" },
                { "Resources", "resources" },
                { "Requests", "requests" }
            };

            if (eventMappings.TryGetValue(propertyName, out var eventMappedName))
                return eventMappedName;
        }
        else if (entityType.Name == "Group")
        {
            var groupMappings = new Dictionary<string, string>
            {
                { "Name", "name" },
                { "Description", "description" },
                { "ArchivedAt", "archived_at" },
                { "ContactEmail", "contact_email" },
                { "Schedule", "schedule" },
                { "LocationTypePreference", "location_type_preference" },
                { "VirtualLocationUrl", "virtual_location_url" },
                { "EventsVisibility", "events_visibility" },
                { "MembershipsCount", "memberships_count" },
                { "MembersAreConfidential", "members_are_confidential" },
                { "LeadersCanSearchPeopleDatabase", "leaders_can_search_people_database" },
                { "CanCreateConversation", "can_create_conversation" },
                { "ChatEnabled", "chat_enabled" },
                { "PublicChurchCenterWebUrl", "public_church_center_web_url" },
                { "HeaderImage", "header_image" },
                { "WidgetStatus", "widget_status" },
                { "GroupTypeId", "group_type_id" },
                { "LocationId", "location_id" },
                { "CreatedAt", "created_at" },
                { "UpdatedAt", "updated_at" },
                { "Id", "id" }
            };

            if (groupMappings.TryGetValue(propertyName, out var groupMappedName))
                return groupMappedName;
        }
        else if (entityType.Name == "Membership")
        {
            var membershipMappings = new Dictionary<string, string>
            {
                { "Role", "role" },
                { "JoinedAt", "joined_at" },
                { "CanCreateEvents", "can_create_events" },
                { "CanEditGroup", "can_edit_group" },
                { "CanManageMembers", "can_manage_members" },
                { "CanViewMembers", "can_view_members" },
                { "PersonId", "person_id" },
                { "GroupId", "group_id" },
                { "CreatedAt", "created_at" },
                { "UpdatedAt", "updated_at" },
                { "Id", "id" }
            };

            if (membershipMappings.TryGetValue(propertyName, out var membershipMappedName))
                return membershipMappedName;
        }
        else if (entityType.Name == "GroupType")
        {
            var groupTypeMappings = new Dictionary<string, string>
            {
                { "Name", "name" },
                { "Description", "description" },
                { "ChurchCenterVisible", "church_center_visible" },
                { "Color", "color" },
                { "Position", "position" },
                { "CreatedAt", "created_at" },
                { "UpdatedAt", "updated_at" },
                { "Id", "id" }
            };

            if (groupTypeMappings.TryGetValue(propertyName, out var groupTypeMappedName))
                return groupTypeMappedName;
        }
        else if (entityType.Name == "Plan")
        {
            var planMappings = new Dictionary<string, string>
            {
                { "Title", "title" },
                { "Dates", "dates" },
                { "ShortDates", "short_dates" },
                { "PlanningCenterUrl", "planning_center_url" },
                { "SortDate", "sort_date" },
                { "IsPublic", "is_public" },
                { "ServiceTimes", "service_times" },
                { "OtherTimes", "other_times" },
                { "Length", "length" },
                { "Notes", "notes" },
                { "ServiceTypeId", "service_type_id" },
                { "CreatedAt", "created_at" },
                { "UpdatedAt", "updated_at" },
                { "Id", "id" }
            };

            if (planMappings.TryGetValue(propertyName, out var planMappedName))
                return planMappedName;
        }
        else if (entityType.Name == "Item")
        {
            var itemMappings = new Dictionary<string, string>
            {
                { "Title", "title" },
                { "Sequence", "sequence" },
                { "ItemType", "item_type" },
                { "Description", "description" },
                { "KeyName", "key_name" },
                { "Length", "length" },
                { "ServicePosition", "service_position" },
                { "PlanId", "plan_id" },
                { "SongId", "song_id" },
                { "ArrangementId", "arrangement_id" },
                { "CreatedAt", "created_at" },
                { "UpdatedAt", "updated_at" },
                { "Id", "id" }
            };

            if (itemMappings.TryGetValue(propertyName, out var itemMappedName))
                return itemMappedName;
        }
        else if (entityType.Name == "Song")
        {
            var songMappings = new Dictionary<string, string>
            {
                { "Title", "title" },
                { "Author", "author" },
                { "Copyright", "copyright" },
                { "CcliNumber", "ccli_number" },
                { "Hidden", "hidden" },
                { "Notes", "notes" },
                { "Themes", "themes" },
                { "LastScheduledAt", "last_scheduled_at" },
                { "CreatedAt", "created_at" },
                { "UpdatedAt", "updated_at" },
                { "Id", "id" }
            };

            if (songMappings.TryGetValue(propertyName, out var songMappedName))
                return songMappedName;
        }
        else if (entityType.Name == "ServiceType")
        {
            var serviceTypeMappings = new Dictionary<string, string>
            {
                { "Name", "name" },
                { "Sequence", "sequence" },
                { "CreatedAt", "created_at" },
                { "UpdatedAt", "updated_at" },
                { "Id", "id" }
            };

            if (serviceTypeMappings.TryGetValue(propertyName, out var serviceTypeMappedName))
                return serviceTypeMappedName;
        }
        else if (entityType.Name == "CheckIn")
        {
            var checkInMappings = new Dictionary<string, string>
            {
                { "FirstName", "first_name" },
                { "LastName", "last_name" },
                { "Number", "number" },
                { "SecurityCode", "security_code" },
                { "Kind", "kind" },
                { "CheckedOutAt", "checked_out_at" },
                { "ConfirmedAt", "confirmed_at" },
                { "MedicalNotes", "medical_notes" },
                { "EmergencyContactName", "emergency_contact_name" },
                { "EmergencyContactPhoneNumber", "emergency_contact_phone_number" },
                { "OneTimeGuest", "one_time_guest" },
                { "PersonId", "person_id" },
                { "EventId", "event_id" },
                { "EventTimeId", "event_time_id" },
                { "LocationId", "location_id" },
                { "CreatedAt", "created_at" },
                { "UpdatedAt", "updated_at" },
                { "Id", "id" }
            };

            if (checkInMappings.TryGetValue(propertyName, out var checkInMappedName))
                 return checkInMappedName;
         }
         else if (entityType.Name == "Event" && entityType.Namespace?.Contains("CheckIns") == true)
         {
             var checkInsEventMappings = new Dictionary<string, string>
             {
                 { "Name", "name" },
                 { "Frequency", "frequency" },
                 { "EnableServicesIntegration", "enable_services_integration" },
                 { "Archived", "archived" },
                 { "CreatedAt", "created_at" },
                 { "UpdatedAt", "updated_at" },
                 { "Id", "id" }
             };

             if (checkInsEventMappings.TryGetValue(propertyName, out var checkInsEventMappedName))
                 return checkInsEventMappedName;
         }
         else if (entityType.Name == "Donation")
         {
             var donationMappings = new Dictionary<string, string>
             {
                 { "AmountCents", "amount_cents" },
                 { "AmountCurrency", "amount_currency" },
                 { "PaymentMethod", "payment_method" },
                 { "PaymentLast4", "payment_last4" },
                 { "PaymentBrand", "payment_brand" },
                 { "FeeCents", "fee_cents" },
                 { "PaymentCheckNumber", "payment_check_number" },
                 { "PaymentCheckDatedAt", "payment_check_dated_at" },
                 { "ReceivedAt", "received_at" },
                 { "PersonId", "person_id" },
                 { "BatchId", "batch_id" },
                 { "CampusId", "campus_id" },
                 { "PaymentSourceId", "payment_source_id" },
                 { "Refunded", "refunded" },
                 { "RefundId", "refund_id" },
                 { "CreatedAt", "created_at" },
                 { "UpdatedAt", "updated_at" },
                 { "Id", "id" }
             };

             if (donationMappings.TryGetValue(propertyName, out var donationMappedName))
                 return donationMappedName;
         }
         else if (entityType.Name == "Fund")
         {
             var fundMappings = new Dictionary<string, string>
             {
                 { "Name", "name" },
                 { "Description", "description" },
                 { "Code", "code" },
                 { "Visibility", "visibility" },
                 { "Default", "default" },
                 { "Color", "color" },
                 { "CreatedAt", "created_at" },
                 { "UpdatedAt", "updated_at" },
                 { "Id", "id" }
             };

             if (fundMappings.TryGetValue(propertyName, out var fundMappedName))
                 return fundMappedName;
         }
         else if (entityType.Name == "Batch")
         {
             var batchMappings = new Dictionary<string, string>
             {
                 { "Description", "description" },
                 { "Status", "status" },
                 { "CommittedAt", "committed_at" },
                 { "TotalCents", "total_cents" },
                 { "TotalCount", "total_count" },
                 { "OwnerId", "owner_id" },
                 { "CreatedAt", "created_at" },
                 { "UpdatedAt", "updated_at" },
                 { "Id", "id" }
             };

             if (batchMappings.TryGetValue(propertyName, out var batchMappedName))
                 return batchMappedName;
         }
         else if (entityType.Name == "PaymentSource")
         {
             var paymentSourceMappings = new Dictionary<string, string>
             {
                 { "Name", "name" },
                 { "PaymentMethodType", "payment_method_type" },
                 { "PaymentLast4", "payment_last4" },
                 { "PaymentBrand", "payment_brand" },
                 { "ExpirationMonth", "expiration_month" },
                 { "ExpirationYear", "expiration_year" },
                 { "Verified", "verified" },
                 { "PersonId", "person_id" },
                 { "CreatedAt", "created_at" },
                 { "UpdatedAt", "updated_at" },
                 { "Id", "id" }
             };

             if (paymentSourceMappings.TryGetValue(propertyName, out var paymentSourceMappedName))
                 return paymentSourceMappedName;
         }
         else if (entityType.Name == "Pledge")
         {
             var pledgeMappings = new Dictionary<string, string>
             {
                 { "AmountCents", "amount_cents" },
                 { "AmountCurrency", "amount_currency" },
                 { "DonatedAmountCents", "donated_amount_cents" },
                 { "JointGiverAmountCents", "joint_giver_amount_cents" },
                 { "PersonId", "person_id" },
                 { "FundId", "fund_id" },
                 { "PledgeCampaignId", "pledge_campaign_id" },
                 { "JointGiverId", "joint_giver_id" },
                 { "CreatedAt", "created_at" },
                 { "UpdatedAt", "updated_at" },
                 { "Id", "id" }
             };

             if (pledgeMappings.TryGetValue(propertyName, out var pledgeMappedName))
                 return pledgeMappedName;
         }
         else if (entityType.Name == "RecurringDonation")
         {
             var recurringDonationMappings = new Dictionary<string, string>
             {
                 { "AmountCents", "amount_cents" },
                 { "AmountCurrency", "amount_currency" },
                 { "Status", "status" },
                 { "Schedule", "schedule" },
                 { "NextOccurrence", "next_occurrence" },
                 { "LastDonationReceivedAt", "last_donation_received_at" },
                 { "PersonId", "person_id" },
                 { "PaymentSourceId", "payment_source_id" },
                 { "CreatedAt", "created_at" },
                 { "UpdatedAt", "updated_at" },
                 { "Id", "id" }
             };

             if (recurringDonationMappings.TryGetValue(propertyName, out var recurringDonationMappedName))
                 return recurringDonationMappedName;
         }
         else if (entityType.Name == "Refund")
         {
             var refundMappings = new Dictionary<string, string>
             {
                 { "AmountCents", "amount_cents" },
                 { "AmountCurrency", "amount_currency" },
                 { "FeeCents", "fee_cents" },
                 { "Reason", "reason" },
                 { "DonationId", "donation_id" },
                 { "RefundedAt", "refunded_at" },
                 { "CreatedAt", "created_at" },
                 { "UpdatedAt", "updated_at" },
                 { "Id", "id" }
             };

             if (refundMappings.TryGetValue(propertyName, out var refundMappedName))
                 return refundMappedName;
         }
         else if (entityType.Name == "Episode")
         {
             var episodeMappings = new Dictionary<string, string>
             {
                 { "Title", "title" },
                 { "Description", "description" },
                 { "PublishedAt", "published_at" },
                 { "LengthInSeconds", "length_in_seconds" },
                 { "VideoUrl", "video_url" },
                 { "AudioUrl", "audio_url" },
                 { "VideoDownloadUrl", "video_download_url" },
                 { "AudioDownloadUrl", "audio_download_url" },
                 { "VideoFileSizeInBytes", "video_file_size_in_bytes" },
                 { "AudioFileSizeInBytes", "audio_file_size_in_bytes" },
                 { "VideoContentType", "video_content_type" },
                 { "AudioContentType", "audio_content_type" },
                 { "ArtworkUrl", "artwork_url" },
                 { "ArtworkFileSizeInBytes", "artwork_file_size_in_bytes" },
                 { "ArtworkContentType", "artwork_content_type" },
                 { "Notes", "notes" },
                 { "SeriesId", "series_id" },
                 { "IsPublished", "is_published" },
                 { "Status", "status" },
                 { "EpisodeNumber", "episode_number" },
                 { "SeasonNumber", "season_number" },
                 { "ViewCount", "view_count" },
                 { "DownloadCount", "download_count" },
                 { "LikeCount", "like_count" },
                 { "CreatedAt", "created_at" },
                 { "UpdatedAt", "updated_at" },
                 { "Id", "id" }
             };

             if (episodeMappings.TryGetValue(propertyName, out var episodeMappedName))
                 return episodeMappedName;
         }
         else if (entityType.Name == "Series")
         {
             var seriesMappings = new Dictionary<string, string>
             {
                 { "Title", "title" },
                 { "Description", "description" },
                 { "Summary", "summary" },
                 { "PublishedAt", "published_at" },
                 { "StartDate", "start_date" },
                 { "EndDate", "end_date" },
                 { "ArtworkUrl", "artwork_url" },
                 { "ArtworkFileSizeInBytes", "artwork_file_size_in_bytes" },
                 { "ArtworkContentType", "artwork_content_type" },
                 { "IsPublished", "is_published" },
                 { "Status", "status" },
                 { "SeriesType", "series_type" },
                 { "Language", "language" },
                 { "EpisodeCount", "episode_count" },
                 { "TotalViewCount", "total_view_count" },
                 { "TotalDownloadCount", "total_download_count" },
                 { "SubscriberCount", "subscriber_count" },
                 { "Author", "author" },
                 { "Copyright", "copyright" },
                 { "WebsiteUrl", "website_url" },
                 { "RssFeedUrl", "rss_feed_url" },
                 { "IsExplicit", "is_explicit" },
                 { "CreatedAt", "created_at" },
                 { "UpdatedAt", "updated_at" },
                 { "Id", "id" }
             };

             if (seriesMappings.TryGetValue(propertyName, out var seriesMappedName))
                 return seriesMappedName;
         }
         else if (entityType.Name == "Speaker")
         {
             var speakerMappings = new Dictionary<string, string>
             {
                 { "FirstName", "first_name" },
                 { "LastName", "last_name" },
                 { "FullName", "full_name" },
                 { "DisplayName", "display_name" },
                 { "Title", "title" },
                 { "Biography", "biography" },
                 { "Email", "email" },
                 { "PhoneNumber", "phone_number" },
                 { "WebsiteUrl", "website_url" },
                 { "PhotoUrl", "photo_url" },
                 { "Active", "active" },
                 { "Organization", "organization" },
                 { "Location", "location" },
                 { "EpisodeCount", "episode_count" },
                 { "Rating", "rating" },
                 { "Notes", "notes" },
                 { "CreatedAt", "created_at" },
                 { "UpdatedAt", "updated_at" },
                 { "Id", "id" }
             };

             if (speakerMappings.TryGetValue(propertyName, out var speakerMappedName))
                 return speakerMappedName;
         }
         else if (entityType.Name == "Media")
         {
             var mediaMappings = new Dictionary<string, string>
             {
                 { "EpisodeId", "episode_id" },
                 { "FileName", "file_name" },
                 { "FileUrl", "file_url" },
                 { "DownloadUrl", "download_url" },
                 { "FileSizeInBytes", "file_size_in_bytes" },
                 { "ContentType", "content_type" },
                 { "MediaType", "media_type" },
                 { "Quality", "quality" },
                 { "DurationSeconds", "duration_seconds" },
                 { "Width", "width" },
                 { "Height", "height" },
                 { "FrameRate", "frame_rate" },
                 { "AudioBitrate", "audio_bitrate" },
                 { "VideoBitrate", "video_bitrate" },
                 { "SampleRate", "sample_rate" },
                 { "Channels", "channels" },
                 { "Format", "format" },
                 { "IsPrimary", "is_primary" },
                 { "UploadStatus", "upload_status" },
                 { "ProcessingStatus", "processing_status" },
                 { "UploadedAt", "uploaded_at" },
                 { "ProcessedAt", "processed_at" },
                 { "DownloadCount", "download_count" },
                 { "ViewCount", "view_count" },
                 { "CreatedAt", "created_at" },
                 { "UpdatedAt", "updated_at" },
                 { "Id", "id" }
             };

             if (mediaMappings.TryGetValue(propertyName, out var mediaMappedName))
                 return mediaMappedName;
         }
         else if (entityType.Name == "DistributionChannel")
         {
             var distributionChannelMappings = new Dictionary<string, string>
             {
                 { "Name", "name" },
                 { "Description", "description" },
                 { "ChannelType", "channel_type" },
                 { "Provider", "provider" },
                 { "ChannelUrl", "channel_url" },
                 { "ApiEndpoint", "api_endpoint" },
                 { "Active", "active" },
                 { "AutoDistribute", "auto_distribute" },
                 { "DistributionSchedule", "distribution_schedule" },
                 { "MaxFileSizeBytes", "max_file_size_bytes" },
                 { "MaxDurationSeconds", "max_duration_seconds" },
                 { "DistributionCount", "distribution_count" },
                 { "LastDistributedAt", "last_distributed_at" },
                 { "LastDistributionStatus", "last_distribution_status" },
                 { "CreatedAt", "created_at" },
                 { "UpdatedAt", "updated_at" },
                 { "Id", "id" }
             };

             if (distributionChannelMappings.TryGetValue(propertyName, out var distributionChannelMappedName))
                 return distributionChannelMappedName;
         }
         else if (entityType.Name == "Speakership")
         {
             var speakershipMappings = new Dictionary<string, string>
             {
                 { "EpisodeId", "episode_id" },
                 { "SpeakerId", "speaker_id" },
                 { "Role", "role" },
                 { "IsPrimary", "is_primary" },
                 { "SortOrder", "sort_order" },
                 { "Contribution", "contribution" },
                 { "StartTimeSeconds", "start_time_seconds" },
                 { "EndTimeSeconds", "end_time_seconds" },
                 { "Notes", "notes" },
                 { "CreatedAt", "created_at" },
                 { "UpdatedAt", "updated_at" },
                 { "Id", "id" }
             };

             if (speakershipMappings.TryGetValue(propertyName, out var speakershipMappedName))
                 return speakershipMappedName;
         }
         else if (entityType.Name == "Signup")
         {
             var signupMappings = new Dictionary<string, string>
             {
                 { "Name", "name" },
                 { "Description", "description" },
                 { "Archived", "archived" },
                 { "OpenAt", "open_at" },
                 { "CloseAt", "close_at" },
                 { "LogoUrl", "logo_url" },
                 { "NewRegistrationUrl", "new_registration_url" },
                 { "RegistrationLimit", "registration_limit" },
                 { "WaitlistEnabled", "waitlist_enabled" },
                 { "CategoryId", "category_id" },
                 { "CampusId", "campus_id" },
                 { "SignupLocationId", "signup_location_id" },
                 { "RegistrationCount", "registration_count" },
                 { "WaitlistCount", "waitlist_count" },
                 { "RequiresApproval", "requires_approval" },
                 { "Status", "status" },
                 { "CreatedAt", "created_at" },
                 { "UpdatedAt", "updated_at" },
                 { "Id", "id" }
             };

             if (signupMappings.TryGetValue(propertyName, out var signupMappedName))
                 return signupMappedName;
         }
         else if (entityType.Name == "Registration")
         {
             var registrationMappings = new Dictionary<string, string>
             {
                 { "SignupId", "signup_id" },
                 { "Status", "status" },
                 { "TotalCost", "total_cost" },
                 { "AmountPaid", "amount_paid" },
                 { "PaymentStatus", "payment_status" },
                 { "PaymentMethod", "payment_method" },
                 { "ConfirmationCode", "confirmation_code" },
                 { "Notes", "notes" },
                 { "Email", "email" },
                 { "FirstName", "first_name" },
                 { "LastName", "last_name" },
                 { "PhoneNumber", "phone_number" },
                 { "CompletedAt", "completed_at" },
                 { "AttendeeCount", "attendee_count" },
                 { "RequiresApproval", "requires_approval" },
                 { "ApprovalStatus", "approval_status" },
                 { "ApprovedAt", "approved_at" },
                 { "ApprovedBy", "approved_by" },
                 { "CreatedAt", "created_at" },
                 { "UpdatedAt", "updated_at" },
                 { "Id", "id" }
             };

             if (registrationMappings.TryGetValue(propertyName, out var registrationMappedName))
                 return registrationMappedName;
         }
         else if (entityType.Name == "Attendee")
         {
             var attendeeMappings = new Dictionary<string, string>
             {
                 { "SignupId", "signup_id" },
                 { "RegistrationId", "registration_id" },
                 { "PersonId", "person_id" },
                 { "FirstName", "first_name" },
                 { "LastName", "last_name" },
                 { "FullName", "full_name" },
                 { "Email", "email" },
                 { "PhoneNumber", "phone_number" },
                 { "Birthdate", "birthdate" },
                 { "Gender", "gender" },
                 { "Grade", "grade" },
                 { "School", "school" },
                 { "Status", "status" },
                 { "OnWaitlist", "on_waitlist" },
                 { "WaitlistPosition", "waitlist_position" },
                 { "CheckedIn", "checked_in" },
                 { "CheckedInAt", "checked_in_at" },
                 { "MedicalNotes", "medical_notes" },
                 { "DietaryRestrictions", "dietary_restrictions" },
                 { "SpecialNeeds", "special_needs" },
                 { "Notes", "notes" },
                 { "EmergencyContactId", "emergency_contact_id" },
                 { "CreatedAt", "created_at" },
                 { "UpdatedAt", "updated_at" },
                 { "Id", "id" }
             };

             if (attendeeMappings.TryGetValue(propertyName, out var attendeeMappedName))
                 return attendeeMappedName;
         }
         else if (entityType.Name == "Campus")
         {
             var campusMappings = new Dictionary<string, string>
             {
                 { "Name", "name" },
                 { "Description", "description" },
                 { "Timezone", "timezone" },
                 { "Address", "address" },
                 { "City", "city" },
                 { "State", "state" },
                 { "PostalCode", "postal_code" },
                 { "Country", "country" },
                 { "PhoneNumber", "phone_number" },
                 { "WebsiteUrl", "website_url" },
                 { "Active", "active" },
                 { "SortOrder", "sort_order" },
                 { "SignupCount", "signup_count" },
                 { "CreatedAt", "created_at" },
                 { "UpdatedAt", "updated_at" },
                 { "Id", "id" }
             };

             if (campusMappings.TryGetValue(propertyName, out var campusMappedName))
                 return campusMappedName;
         }
         else if (entityType.Name == "Category")
         {
             var categoryMappings = new Dictionary<string, string>
             {
                 { "Name", "name" },
                 { "Description", "description" },
                 { "Color", "color" },
                 { "SortOrder", "sort_order" },
                 { "Active", "active" },
                 { "ParentCategoryId", "parent_category_id" },
                 { "IconUrl", "icon_url" },
                 { "SignupCount", "signup_count" },
                 { "CreatedAt", "created_at" },
                 { "UpdatedAt", "updated_at" },
                 { "Id", "id" }
             };

             if (categoryMappings.TryGetValue(propertyName, out var categoryMappedName))
                 return categoryMappedName;
         }
         else if (entityType.Name == "SelectionType")
         {
             var selectionTypeMappings = new Dictionary<string, string>
             {
                 { "SignupId", "signup_id" },
                 { "Name", "name" },
                 { "Description", "description" },
                 { "Category", "category" },
                 { "Cost", "cost" },
                 { "Currency", "currency" },
                 { "Required", "required" },
                 { "AllowMultiple", "allow_multiple" },
                 { "MaxSelections", "max_selections" },
                 { "MinSelections", "min_selections" },
                 { "SelectionLimit", "selection_limit" },
                 { "SelectionCount", "selection_count" },
                 { "SortOrder", "sort_order" },
                 { "Active", "active" },
                 { "CreatedAt", "created_at" },
                 { "UpdatedAt", "updated_at" },
                 { "Id", "id" }
             };

             if (selectionTypeMappings.TryGetValue(propertyName, out var selectionTypeMappedName))
                 return selectionTypeMappedName;
         }
         else if (entityType.Name == "EmergencyContact")
         {
             var emergencyContactMappings = new Dictionary<string, string>
             {
                 { "AttendeeId", "attendee_id" },
                 { "FirstName", "first_name" },
                 { "LastName", "last_name" },
                 { "FullName", "full_name" },
                 { "Relationship", "relationship" },
                 { "PrimaryPhone", "primary_phone" },
                 { "SecondaryPhone", "secondary_phone" },
                 { "Email", "email" },
                 { "StreetAddress", "street_address" },
                 { "City", "city" },
                 { "State", "state" },
                 { "PostalCode", "postal_code" },
                 { "Country", "country" },
                 { "IsPrimary", "is_primary" },
                 { "Priority", "priority" },
                 { "Notes", "notes" },
                 { "PreferredContactMethod", "preferred_contact_method" },
                 { "BestTimeToContact", "best_time_to_contact" },
                 { "CanAuthorizeMedicalTreatment", "can_authorize_medical_treatment" },
                 { "CreatedAt", "created_at" },
                 { "UpdatedAt", "updated_at" },
                 { "Id", "id" }
             };

             if (emergencyContactMappings.TryGetValue(propertyName, out var emergencyContactMappedName))
                 return emergencyContactMappedName;
         }
         else if (entityType.Name == "SignupLocation")
         {
             var signupLocationMappings = new Dictionary<string, string>
             {
                 { "SignupId", "signup_id" },
                 { "Name", "name" },
                 { "Description", "description" },
                 { "StreetAddress", "street_address" },
                 { "City", "city" },
                 { "State", "state" },
                 { "PostalCode", "postal_code" },
                 { "Country", "country" },
                 { "FormattedAddress", "formatted_address" },
                 { "Latitude", "latitude" },
                 { "Longitude", "longitude" },
                 { "PhoneNumber", "phone_number" },
                 { "WebsiteUrl", "website_url" },
                 { "Directions", "directions" },
                 { "ParkingInfo", "parking_info" },
                 { "AccessibilityInfo", "accessibility_info" },
                 { "Capacity", "capacity" },
                 { "Notes", "notes" },
                 { "Timezone", "timezone" },
                 { "CreatedAt", "created_at" },
                 { "UpdatedAt", "updated_at" },
                 { "Id", "id" }
             };

             if (signupLocationMappings.TryGetValue(propertyName, out var signupLocationMappedName))
                 return signupLocationMappedName;
         }
         else if (entityType.Name == "SignupTime")
         {
             var signupTimeMappings = new Dictionary<string, string>
             {
                 { "SignupId", "signup_id" },
                 { "Name", "name" },
                 { "Description", "description" },
                 { "StartTime", "start_time" },
                 { "EndTime", "end_time" },
                 { "AllDay", "all_day" },
                 { "Timezone", "timezone" },
                 { "TimeType", "time_type" },
                 { "Capacity", "capacity" },
                 { "RegistrationCount", "registration_count" },
                 { "Required", "required" },
                 { "SortOrder", "sort_order" },
                 { "Active", "active" },
                 { "Location", "location" },
                 { "Room", "room" },
                 { "Instructor", "instructor" },
                 { "Cost", "cost" },
                 { "Notes", "notes" },
                 { "CreatedAt", "created_at" },
                 { "UpdatedAt", "updated_at" },
                 { "Id", "id" }
             };

             if (signupTimeMappings.TryGetValue(propertyName, out var signupTimeMappedName))
                 return signupTimeMappedName;
         }
         else if (entityType.Name == "WebhookSubscription")
         {
             var webhookSubscriptionMappings = new Dictionary<string, string>
             {
                 { "Url", "url" },
                 { "Secret", "secret" },
                 { "Active", "active" },
                 { "Name", "name" },
                 { "Description", "description" },
                 { "AvailableEventId", "available_event_id" },
                 { "OrganizationId", "organization_id" },
                 { "LastDeliveryAt", "last_delivery_at" },
                 { "LastDeliveryStatus", "last_delivery_status" },
                 { "LastDeliveryStatusCode", "last_delivery_status_code" },
                 { "LastDeliveryResponseTimeMs", "last_delivery_response_time_ms" },
                 { "LastDeliveryError", "last_delivery_error" },
                 { "TotalDeliveries", "total_deliveries" },
                 { "SuccessfulDeliveries", "successful_deliveries" },
                 { "FailedDeliveries", "failed_deliveries" },
                 { "SuccessRate", "success_rate" },
                 { "RetryPolicy", "retry_policy" },
                 { "MaxRetries", "max_retries" },
                 { "TimeoutSeconds", "timeout_seconds" },
                 { "CustomHeaders", "custom_headers" },
                 { "Metadata", "metadata" },
                 { "CreatedAt", "created_at" },
                 { "UpdatedAt", "updated_at" },
                 { "Id", "id" }
             };

             if (webhookSubscriptionMappings.TryGetValue(propertyName, out var webhookSubscriptionMappedName))
                 return webhookSubscriptionMappedName;
         }
         else if (entityType.Name == "Event")
         {
             var eventMappings = new Dictionary<string, string>
             {
                 { "WebhookSubscriptionId", "webhook_subscription_id" },
                 { "AvailableEventId", "available_event_id" },
                 { "EventName", "event_name" },
                 { "Payload", "payload" },
                 { "DeliveryStatus", "delivery_status" },
                 { "DeliveredAt", "delivered_at" },
                 { "ResponseStatusCode", "response_status_code" },
                 { "ResponseBody", "response_body" },
                 { "ResponseHeaders", "response_headers" },
                 { "ResponseTimeMs", "response_time_ms" },
                 { "ErrorMessage", "error_message" },
                 { "DeliveryAttempts", "delivery_attempts" },
                 { "MaxRetries", "max_retries" },
                 { "NextRetryAt", "next_retry_at" },
                 { "ExpiresAt", "expires_at" },
                 { "Signature", "signature" },
                 { "Version", "version" },
                 { "SourceModule", "source_module" },
                 { "ResourceId", "resource_id" },
                 { "ResourceType", "resource_type" },
                 { "Action", "action" },
                 { "TriggeredBy", "triggered_by" },
                 { "OccurredAt", "occurred_at" },
                 { "Metadata", "metadata" },
                 { "CreatedAt", "created_at" },
                 { "UpdatedAt", "updated_at" },
                 { "Id", "id" }
             };

             if (eventMappings.TryGetValue(propertyName, out var eventMappedName))
                 return eventMappedName;
         }
         else if (entityType.Name == "AvailableEvent")
         {
             var availableEventMappings = new Dictionary<string, string>
             {
                 { "Name", "name" },
                 { "Description", "description" },
                 { "Module", "module" },
                 { "Category", "category" },
                 { "Action", "action" },
                 { "ResourceType", "resource_type" },
                 { "Active", "active" },
                 { "Version", "version" },
                 { "PayloadSchema", "payload_schema" },
                 { "ExamplePayload", "example_payload" },
                 { "SubscriptionCount", "subscription_count" },
                 { "TotalTriggerCount", "total_trigger_count" },
                 { "LastTriggeredAt", "last_triggered_at" },
                 { "AverageFrequency", "average_frequency" },
                 { "Documentation", "documentation" },
                 { "RelatedEvents", "related_events" },
                 { "Metadata", "metadata" },
                 { "CreatedAt", "created_at" },
                 { "UpdatedAt", "updated_at" },
                 { "Id", "id" }
             };

             if (availableEventMappings.TryGetValue(propertyName, out var availableEventMappedName))
                 return availableEventMappedName;
         }

        // Add support for common nested properties across all entity types
        var commonMappings = new Dictionary<string, string>
        {
            { "Id", "id" },
            { "CreatedAt", "created_at" },
            { "UpdatedAt", "updated_at" },
            { "Name", "name" },
            { "Description", "description" },
            { "Primary", "primary" },
            { "Active", "active" },
            { "Visible", "visible" }
        };
        
        if (commonMappings.TryGetValue(propertyName, out var commonMappedName))
            return commonMappedName;

        // Default conversion: PascalCase to snake_case
        return string.Concat(propertyName.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString())).ToLower();
    }

    private static FilterOperator ConvertExpressionTypeToFilterOperator(ExpressionType expressionType)
    {
        return expressionType switch
        {
            ExpressionType.Equal => FilterOperator.Equal,
            ExpressionType.NotEqual => FilterOperator.NotEqual,
            ExpressionType.GreaterThan => FilterOperator.GreaterThan,
            ExpressionType.GreaterThanOrEqual => FilterOperator.GreaterThanOrEqual,
            ExpressionType.LessThan => FilterOperator.LessThan,
            ExpressionType.LessThanOrEqual => FilterOperator.LessThanOrEqual,
            _ => FilterOperator.Equal
        };
    }

    /// <summary>
    /// Converts method name to appropriate filter operator.
    /// </summary>
    private static FilterOperator ConvertMethodNameToFilterOperator(string methodName)
    {
        return methodName switch
        {
            "Contains" => FilterOperator.In,
            "StartsWith" => FilterOperator.StartsWith,
            "EndsWith" => FilterOperator.EndsWith,
            "IsNull" => FilterOperator.IsNull,
            "IsNotNull" => FilterOperator.IsNotNull,
            _ => FilterOperator.Equal
        };
    }
}

/// <summary>
/// Represents the result of parsing a filter expression.
/// </summary>
public class FilterResult
{
    public string Field { get; set; } = string.Empty;
    public FilterOperator Operator { get; set; } = FilterOperator.Equal;
    public object? Value { get; set; }
    public LogicalOperator LogicalOperator { get; set; } = LogicalOperator.And;
    public List<FilterResult> SubFilters { get; set; } = new();

    public bool IsEmpty => string.IsNullOrEmpty(Field) && !SubFilters.Any();

    public FilterResult Negate()
    {
        var negated = new FilterResult
        {
            Field = Field,
            Value = Value,
            LogicalOperator = LogicalOperator,
            SubFilters = SubFilters.Select(sf => sf.Negate()).ToList()
        };

        negated.Operator = Operator switch
        {
            FilterOperator.Equal => FilterOperator.NotEqual,
            FilterOperator.NotEqual => FilterOperator.Equal,
            FilterOperator.GreaterThan => FilterOperator.LessThanOrEqual,
            FilterOperator.GreaterThanOrEqual => FilterOperator.LessThan,
            FilterOperator.LessThan => FilterOperator.GreaterThanOrEqual,
            FilterOperator.LessThanOrEqual => FilterOperator.GreaterThan,
            FilterOperator.Contains => FilterOperator.NotContains,
            FilterOperator.NotContains => FilterOperator.Contains,
            FilterOperator.In => FilterOperator.NotIn,
            FilterOperator.NotIn => FilterOperator.In,
            FilterOperator.IsNull => FilterOperator.IsNotNull,
            FilterOperator.IsNotNull => FilterOperator.IsNull,
            _ => Operator
        };

        return negated;
    }

    public static FilterResult Combine(FilterResult left, FilterResult right, LogicalOperator logicalOperator)
    {
        if (left.IsEmpty) return right;
        if (right.IsEmpty) return left;

        return new FilterResult
        {
            LogicalOperator = logicalOperator,
            SubFilters = new List<FilterResult> { left, right }
        };
    }

    public string ToApiFilterString()
    {
        if (!SubFilters.Any())
        {
            // Simple filter
            var operatorString = Operator switch
            {
                FilterOperator.Equal => "",
                FilterOperator.NotEqual => "!",
                FilterOperator.GreaterThan => ">",
                FilterOperator.GreaterThanOrEqual => ">=",
                FilterOperator.LessThan => "<",
                FilterOperator.LessThanOrEqual => "<=",
                FilterOperator.Contains => "*",
                FilterOperator.NotContains => "!*",
                FilterOperator.StartsWith => "",
                FilterOperator.EndsWith => "",
                FilterOperator.In => "",
                FilterOperator.NotIn => "!",
                FilterOperator.IsNull => "",
                FilterOperator.IsNotNull => "!",
                FilterOperator.Between => "",
                _ => ""
            };

            var valueString = Value?.ToString() ?? "";

            if (Operator == FilterOperator.Contains || Operator == FilterOperator.NotContains)
            {
                valueString = $"*{valueString}*";
            }
            else if (Operator == FilterOperator.StartsWith)
            {
                valueString = $"{valueString}*";
            }
            else if (Operator == FilterOperator.EndsWith)
            {
                valueString = $"*{valueString}";
            }
            else if (Operator == FilterOperator.In || Operator == FilterOperator.NotIn)
            {
                // Handle array values for IN/NOT IN operations
                if (Value is IEnumerable<object> enumerable && !(Value is string))
                {
                    valueString = string.Join(",", enumerable.Select(v => v?.ToString() ?? ""));
                }
            }
            else if (Operator == FilterOperator.IsNull)
            {
                valueString = "null";
            }
            else if (Operator == FilterOperator.IsNotNull)
            {
                valueString = "null";
            }
            else if (Operator == FilterOperator.Between)
            {
                // Handle range values for BETWEEN operations
                if (Value is IEnumerable<object> rangeValues && !(Value is string))
                {
                    var values = rangeValues.Take(2).ToArray();
                    if (values.Length == 2)
                    {
                        valueString = $"{values[0]},{values[1]}";
                    }
                }
            }

            return $"{operatorString}{valueString}";
        }

        // Complex filter with sub-filters
        // Enhanced support for nested AND/OR operations
        if (SubFilters.Count == 1)
        {
            return SubFilters.First().ToApiFilterString();
        }

        // For multiple sub-filters, combine them based on logical operator
        var filterStrings = SubFilters.Select(sf => sf.ToApiFilterString()).Where(s => !string.IsNullOrEmpty(s));
        
        if (LogicalOperator == LogicalOperator.Or)
        {
            // Planning Center API uses comma separation for OR operations in some contexts
            return string.Join(",", filterStrings);
        }
        else
        {
            // For AND operations, we'll need to handle this at the query builder level
            // as Planning Center API typically handles AND through multiple filter parameters
            return filterStrings.FirstOrDefault() ?? "";
        }
    }
}

/// <summary>
/// Supported filter operators.
/// </summary>
public enum FilterOperator
{
    Equal,
    NotEqual,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
    Contains,
    NotContains,
    StartsWith,
    EndsWith,
    In,
    NotIn,
    IsNull,
    IsNotNull,
    Between
}

/// <summary>
/// Logical operators for combining filters.
/// </summary>
public enum LogicalOperator
{
    And,
    Or
}