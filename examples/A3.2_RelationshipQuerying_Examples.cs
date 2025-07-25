using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlanningCenter.Api.Client.Fluent;
using PlanningCenter.Api.Client.Models.People;

namespace PlanningCenter.Api.Client.Examples;

/// <summary>
/// Comprehensive examples demonstrating A3.2 Relationship Querying features.
/// These examples show how to work with related entities through deep includes,
/// relationship filtering, and relationship counting operations.
/// </summary>
public class A32RelationshipQueryingExamples
{
    private readonly PlanningCenterFluentClient _client;

    public A32RelationshipQueryingExamples(PlanningCenterFluentClient client)
    {
        _client = client;
    }

    #region Deep Relationship Includes

    /// <summary>
    /// Example 1: Basic deep relationship includes
    /// Demonstrates including nested relationships in a single query.
    /// </summary>
    public async Task Example1_BasicDeepIncludes()
    {
        Console.WriteLine("=== Example 1: Basic Deep Includes ===");

        // Include people with their household members
        var peopleWithHouseholdMembers = await _client.People
            .IncludeDeep("households.members")
            .Take(10)
            .ExecuteAsync();

        Console.WriteLine($"Found {peopleWithHouseholdMembers.Data.Count} people with household member data");
        
        // Include multiple deep relationships
        var peopleWithCompleteData = await _client.People
            .IncludeDeep(
                "households.members",
                "field_data.field_definition", 
                "emails",
                "phone_numbers",
                "addresses"
            )
            .Take(5)
            .ExecuteAsync();

        Console.WriteLine($"Found {peopleWithCompleteData.Data.Count} people with complete relationship data");
    }

    /// <summary>
    /// Example 2: Type-safe LINQ expression includes
    /// Shows how to use strongly-typed includes for better IntelliSense and compile-time safety.
    /// </summary>
    public async Task Example2_TypeSafeIncludes()
    {
        Console.WriteLine("=== Example 2: Type-Safe Includes ===");

        // Type-safe includes using LINQ expressions
        var people = await _client.People
            .Include(p => p.Households)
            .Include(p => p.FieldData)
            .Include(p => p.Emails)
            .Take(10)
            .ExecuteAsync();

        Console.WriteLine($"Found {people.Data.Count} people with type-safe includes");
    }

    #endregion

    #region Relationship Existence Filtering

    /// <summary>
    /// Example 3: Filtering by relationship existence
    /// Demonstrates finding entities based on whether they have specific relationships.
    /// </summary>
    public async Task Example3_RelationshipExistence()
    {
        Console.WriteLine("=== Example 3: Relationship Existence Filtering ===");

        // Find people who have households
        var peopleWithHouseholds = await _client.People
            .WhereHasRelationship("households")
            .Take(20)
            .ExecuteAsync();

        Console.WriteLine($"Found {peopleWithHouseholds.Data.Count} people with households");

        // Find people who don't have email addresses
        var peopleWithoutEmails = await _client.People
            .WhereDoesntHaveRelationship("emails")
            .Take(10)
            .ExecuteAsync();

        Console.WriteLine($"Found {peopleWithoutEmails.Data.Count} people without email addresses");

        // Combine existence filters
        var peopleWithHouseholdsButNoEmails = await _client.People
            .WhereHasRelationship("households")
            .WhereDoesntHaveRelationship("emails")
            .Take(10)
            .ExecuteAsync();

        Console.WriteLine($"Found {peopleWithHouseholdsButNoEmails.Data.Count} people with households but no emails");
    }

    #endregion

    #region Relationship Condition Filtering

    /// <summary>
    /// Example 4: Filtering by conditions within relationships
    /// Shows how to filter based on properties of related entities.
    /// </summary>
    public async Task Example4_RelationshipConditions()
    {
        Console.WriteLine("=== Example 4: Relationship Condition Filtering ===");

        // Find people in households with a specific name
        var smithFamilyMembers = await _client.People
            .WhereHas("households", "name", "Smith Family")
            .IncludeDeep("households")
            .ExecuteAsync();

        Console.WriteLine($"Found {smithFamilyMembers.Data.Count} Smith family members");

        // Find people with Gmail addresses
        var gmailUsers = await _client.People
            .WhereHas("emails", "address", "*@gmail.com")
            .Include("emails")
            .Take(15)
            .ExecuteAsync();

        Console.WriteLine($"Found {gmailUsers.Data.Count} people with Gmail addresses");

        // Multiple conditions on the same relationship
        var specificHouseholds = await _client.People
            .WhereHas("households", new Dictionary<string, object>
            {
                { "name", "*Family" }, // Households ending with "Family"
                { "member_count", ">2" }, // More than 2 members
                { "primary_contact_name", "John*" } // Primary contact starts with "John"
            })
            .IncludeDeep("households.members")
            .ExecuteAsync();

        Console.WriteLine($"Found {specificHouseholds.Data.Count} people in specific household conditions");
    }

    #endregion

    #region Relationship Counting

    /// <summary>
    /// Example 5: Filtering by relationship counts
    /// Demonstrates various ways to filter based on the number of related entities.
    /// </summary>
    public async Task Example5_RelationshipCounting()
    {
        Console.WriteLine("=== Example 5: Relationship Counting ===");

        // Find people with exactly 2 households
        var peopleWithTwoHouseholds = await _client.People
            .WhereRelationshipCount("households", 2)
            .Include("households")
            .ExecuteAsync();

        Console.WriteLine($"Found {peopleWithTwoHouseholds.Data.Count} people with exactly 2 households");

        // Find people with more than 1 email address
        var peopleWithMultipleEmails = await _client.People
            .WhereRelationshipCountGreaterThan("emails", 1)
            .Include("emails")
            .Take(20)
            .ExecuteAsync();

        Console.WriteLine($"Found {peopleWithMultipleEmails.Data.Count} people with multiple email addresses");

        // Find people with fewer than 3 phone numbers
        var peopleWithFewPhones = await _client.People
            .WhereRelationshipCountLessThan("phone_numbers", 3)
            .Include("phone_numbers")
            .Take(25)
            .ExecuteAsync();

        Console.WriteLine($"Found {peopleWithFewPhones.Data.Count} people with fewer than 3 phone numbers");

        // Find people with 1-3 addresses
        var peopleWithModerateAddresses = await _client.People
            .WhereRelationshipCountBetween("addresses", 1, 3)
            .Include("addresses")
            .Take(30)
            .ExecuteAsync();

        Console.WriteLine($"Found {peopleWithModerateAddresses.Data.Count} people with 1-3 addresses");
    }

    #endregion

    #region Complex Relationship Queries

    /// <summary>
    /// Example 6: Complex relationship queries
    /// Demonstrates combining multiple relationship querying features for sophisticated filtering.
    /// </summary>
    public async Task Example6_ComplexRelationshipQueries()
    {
        Console.WriteLine("=== Example 6: Complex Relationship Queries ===");

        // Find active family members with complete contact information
        var activeFamilyMembersWithContacts = await _client.People
            .IncludeDeep("households.members", "emails", "phone_numbers", "addresses")
            .WhereHasRelationship("households")
            .WhereHas("households", "status", "active")
            .WhereRelationshipCountGreaterThan("emails", 0)
            .WhereRelationshipCountGreaterThan("phone_numbers", 0)
            .WhereHasRelationship("addresses")
            .Take(50)
            .ExecuteAsync();

        Console.WriteLine($"Found {activeFamilyMembersWithContacts.Data.Count} active family members with complete contact info");

        // Find people in large families with multiple contact methods
        var largeFamilyMembers = await _client.People
            .IncludeDeep("households.members")
            .WhereHas("households", "member_count", ">4")
            .WhereRelationshipCountBetween("emails", 1, 3)
            .WhereRelationshipCountBetween("phone_numbers", 1, 2)
            .OrderBy("last_name")
            .Take(25)
            .ExecuteAsync();

        Console.WriteLine($"Found {largeFamilyMembers.Data.Count} members of large families with multiple contacts");
    }

    #endregion

    #region Practical Use Cases

    /// <summary>
    /// Example 7: Household management scenarios
    /// Real-world examples for managing household relationships.
    /// </summary>
    public async Task Example7_HouseholdManagement()
    {
        Console.WriteLine("=== Example 7: Household Management ===");

        // Find household primary contacts
        var primaryContacts = await _client.People
            .WhereHas("households", "primary_contact_id", "self")
            .IncludeDeep("households.members")
            .OrderBy("last_name")
            .ExecuteAsync();

        Console.WriteLine($"Found {primaryContacts.Data.Count} household primary contacts");

        // Find people in single-person households
        var singlePersonHouseholds = await _client.People
            .WhereHasRelationship("households")
            .WhereRelationshipCount("households", 1)
            .WhereHas("households", "member_count", 1)
            .Include("households")
            .ExecuteAsync();

        Console.WriteLine($"Found {singlePersonHouseholds.Data.Count} people in single-person households");

        // Find families with children (assuming child status in household)
        var familiesWithChildren = await _client.People
            .WhereHas("households", "member_count", ">2")
            .WhereHas("households", "has_children", true)
            .IncludeDeep("households.members")
            .OrderBy("households.name")
            .ExecuteAsync();

        Console.WriteLine($"Found {familiesWithChildren.Data.Count} people in families with children");
    }

    /// <summary>
    /// Example 8: Communication and outreach scenarios
    /// Examples focused on contact information and communication preferences.
    /// </summary>
    public async Task Example8_CommunicationOutreach()
    {
        Console.WriteLine("=== Example 8: Communication & Outreach ===");

        // Find people with preferred communication methods
        var emailPreferred = await _client.People
            .WhereRelationshipCountGreaterThan("emails", 0)
            .WhereHas("emails", "primary", true)
            .WhereDoesntHaveRelationship("phone_numbers")
            .Include("emails")
            .Take(20)
            .ExecuteAsync();

        Console.WriteLine($"Found {emailPreferred.Data.Count} people who prefer email communication");

        // Find people with incomplete contact information
        var incompleteContacts = await _client.People
            .Where("status", "active")
            .WhereHasRelationship("households")
            .WhereRelationshipCount("emails", 0)
            .WhereRelationshipCount("phone_numbers", 0)
            .Take(15)
            .ExecuteAsync();

        Console.WriteLine($"Found {incompleteContacts.Data.Count} active people with incomplete contact info");

        // Find people with multiple contact methods for important communications
        var multiContactPeople = await _client.People
            .WhereRelationshipCountGreaterThan("emails", 0)
            .WhereRelationshipCountGreaterThan("phone_numbers", 0)
            .WhereHasRelationship("addresses")
            .IncludeDeep("emails", "phone_numbers", "addresses")
            .OrderBy("last_name")
            .Take(30)
            .ExecuteAsync();

        Console.WriteLine($"Found {multiContactPeople.Data.Count} people with multiple contact methods");
    }

    /// <summary>
    /// Example 9: Data quality and maintenance
    /// Examples for identifying data quality issues and maintenance needs.
    /// </summary>
    public async Task Example9_DataQualityMaintenance()
    {
        Console.WriteLine("=== Example 9: Data Quality & Maintenance ===");

        // Find people with duplicate email addresses
        var potentialDuplicateEmails = await _client.People
            .WhereRelationshipCountGreaterThan("emails", 2)
            .Include("emails")
            .Take(10)
            .ExecuteAsync();

        Console.WriteLine($"Found {potentialDuplicateEmails.Data.Count} people with more than 2 email addresses (potential duplicates)");

        // Find households with missing primary contact
        var householdsNeedingPrimaryContact = await _client.People
            .WhereHasRelationship("households")
            .WhereHas("households", "primary_contact_id", "null")
            .WhereHas("households", "member_count", ">1")
            .IncludeDeep("households.members")
            .ExecuteAsync();

        Console.WriteLine($"Found {householdsNeedingPrimaryContact.Data.Count} people in households needing primary contact assignment");

        // Find people with outdated information (no recent updates)
        var outdatedProfiles = await _client.People
            .WhereHasRelationship("households")
            .WhereRelationshipCount("emails", 0)
            .WhereRelationshipCount("phone_numbers", 0)
            .Where("updated_at", "<2023-01-01")
            .OrderBy("updated_at")
            .Take(20)
            .ExecuteAsync();

        Console.WriteLine($"Found {outdatedProfiles.Data.Count} profiles that may need updating");
    }

    #endregion

    #region Performance Optimization Examples

    /// <summary>
    /// Example 10: Performance optimization techniques
    /// Demonstrates efficient querying patterns for relationship data.
    /// </summary>
    public async Task Example10_PerformanceOptimization()
    {
        Console.WriteLine("=== Example 10: Performance Optimization ===");

        // Efficient counting without loading related data
        var householdCounts = await _client.People
            .WhereRelationshipCountGreaterThan("households", 0)
            .WhereRelationshipCountLessThan("households", 5)
            .Take(100) // Limit results for performance
            .ExecuteAsync(); // Don't include relationships, just count

        Console.WriteLine($"Efficiently found {householdCounts.Data.Count} people with 1-4 households");

        // Selective deep includes based on conditions
        var targetedIncludes = await _client.People
            .WhereHas("households", "member_count", ">3") // Pre-filter before including
            .IncludeDeep("households.members") // Only include for relevant records
            .Take(25)
            .ExecuteAsync();

        Console.WriteLine($"Targeted includes for {targetedIncludes.Data.Count} people in large households");

        // Combine relationship filters for efficient querying
        var efficientFiltering = await _client.People
            .WhereHasRelationship("households") // Existence check first
            .WhereRelationshipCountBetween("emails", 1, 2) // Count filter
            .WhereHas("households", "status", "active") // Condition filter
            .OrderBy("last_name")
            .Take(50)
            .ExecuteAsync();

        Console.WriteLine($"Efficiently filtered {efficientFiltering.Data.Count} people with combined relationship criteria");
    }

    #endregion

    /// <summary>
    /// Runs all relationship querying examples
    /// </summary>
    public async Task RunAllExamples()
    {
        Console.WriteLine("Running A3.2 Relationship Querying Examples...");
        Console.WriteLine(new string('=', 60));

        try
        {
            await Example1_BasicDeepIncludes();
            Console.WriteLine();

            await Example2_TypeSafeIncludes();
            Console.WriteLine();

            await Example3_RelationshipExistence();
            Console.WriteLine();

            await Example4_RelationshipConditions();
            Console.WriteLine();

            await Example5_RelationshipCounting();
            Console.WriteLine();

            await Example6_ComplexRelationshipQueries();
            Console.WriteLine();

            await Example7_HouseholdManagement();
            Console.WriteLine();

            await Example8_CommunicationOutreach();
            Console.WriteLine();

            await Example9_DataQualityMaintenance();
            Console.WriteLine();

            await Example10_PerformanceOptimization();
            Console.WriteLine();

            Console.WriteLine("All A3.2 Relationship Querying examples completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error running examples: {ex.Message}");
            throw;
        }
    }
}

/// <summary>
/// Extension methods to make the examples more readable
/// </summary>
public static class RelationshipQueryExtensions
{
    /// <summary>
    /// Helper method to display relationship query results
    /// </summary>
    public static void DisplayResults<T>(this IPagedResponse<T> response, string queryDescription)
    {
        Console.WriteLine($"{queryDescription}: Found {response.Data.Count} results");
        if (response.Meta != null)
        {
            Console.WriteLine($"  Total available: {response.Meta.TotalCount}");
            Console.WriteLine($"  Page {response.Meta.CurrentPage} of {response.Meta.TotalPages}");
        }
    }

    /// <summary>
    /// Helper method to safely get relationship count from query parameters
    /// </summary>
    public static string GetRelationshipCountFilter(this QueryParameters parameters, string relationshipName)
    {
        var key = $"{relationshipName}_count";
        return parameters.Where.TryGetValue(key, out var value) ? value : "Not filtered";
    }

    /// <summary>
    /// Helper method to check if a relationship existence filter is applied
    /// </summary>
    public static bool HasRelationshipExistenceFilter(this QueryParameters parameters, string relationshipName)
    {
        var key = $"has_{relationshipName}";
        return parameters.Where.ContainsKey(key);
    }
}