using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlanningCenter.Api.Client.Fluent;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Services.People;
using PlanningCenter.Api.Client.Services.CheckIns;
using PlanningCenter.Api.Client.Services.Calendar;

namespace PlanningCenter.Api.Examples
{
    /// <summary>
    /// Examples demonstrating A3.1 Complex Filtering features in the Planning Center SDK.
    /// This showcases all the advanced query capabilities that have been implemented.
    /// </summary>
    public class A31ComplexFilteringExamples
    {
        private readonly PeopleService _peopleService;
        private readonly CheckInsService _checkInsService;
        private readonly CalendarService _calendarService;

        public A31ComplexFilteringExamples(
            PeopleService peopleService,
            CheckInsService checkInsService,
            CalendarService calendarService)
        {
            _peopleService = peopleService;
            _checkInsService = checkInsService;
            _calendarService = calendarService;
        }

        /// <summary>
        /// Example 1: IN/NOT IN Operations
        /// Find people with specific IDs or exclude certain statuses
        /// </summary>
        public async Task InNotInFilteringExample()
        {
            // Find people with specific IDs
            var specificPeople = await _peopleService.People
                .WhereIn("id", new[] { "123", "456", "789" })
                .ExecuteAsync();

            // Find people excluding inactive statuses
            var activePeople = await _peopleService.People
                .WhereNotIn("status", new[] { "inactive", "archived", "deceased" })
                .ExecuteAsync();

            // Combine IN and NOT IN filters
            var filteredPeople = await _peopleService.People
                .WhereIn("membership", new[] { "member", "regular_attender" })
                .WhereNotIn("status", new[] { "inactive" })
                .ExecuteAsync();
        }

        /// <summary>
        /// Example 2: NULL/NOT NULL Operations
        /// Filter by presence or absence of data
        /// </summary>
        public async Task NullNotNullFilteringExample()
        {
            // Find people without profile photos
            var peopleWithoutPhotos = await _peopleService.People
                .WhereNull("avatar")
                .ExecuteAsync();

            // Find people with email addresses
            var peopleWithEmails = await _peopleService.People
                .WhereNotNull("primary_email")
                .ExecuteAsync();

            // Find incomplete profiles (missing phone or email)
            var incompleteProfiles = await _peopleService.People
                .WhereNull("primary_email")
                .WhereNull("primary_phone")
                .ExecuteAsync();
        }

        /// <summary>
        /// Example 3: Date Range Filtering
        /// Filter events and check-ins by date ranges
        /// </summary>
        public async Task DateRangeFilteringExample()
        {
            var startDate = DateTime.Today.AddDays(-30);
            var endDate = DateTime.Today;

            // Find events in the last 30 days
            var recentEvents = await _calendarService.Events
                .WhereDateRange("starts_at", startDate, endDate)
                .ExecuteAsync();

            // Find check-ins for this week
            var weekStart = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            var weekEnd = weekStart.AddDays(6);
            
            var weeklyCheckIns = await _checkInsService.CheckIns
                .WhereDateRange("created_at", weekStart, weekEnd)
                .ExecuteAsync();

            // Find events with nullable date fields
            DateTime? optionalStart = DateTime.Today.AddDays(-7);
            DateTime? optionalEnd = DateTime.Today.AddDays(7);
            
            var flexibleDateEvents = await _calendarService.Events
                .WhereDateRange("ends_at", optionalStart, optionalEnd)
                .ExecuteAsync();
        }

        /// <summary>
        /// Example 4: Numeric Range Filtering (BETWEEN)
        /// Filter by numeric ranges like age, counts, etc.
        /// </summary>
        public async Task NumericRangeFilteringExample()
        {
            // Find people in a specific age range (if age field exists)
            var youngAdults = await _peopleService.People
                .WhereBetween("age", 18, 35)
                .ExecuteAsync();

            // Find events with attendance between certain numbers
            var mediumEvents = await _calendarService.Events
                .WhereBetween("attendance_count", 50, 200)
                .ExecuteAsync();

            // Combine numeric ranges with other filters
            var targetDemographic = await _peopleService.People
                .WhereBetween("age", 25, 45)
                .WhereNotNull("primary_email")
                .WhereIn("membership", new[] { "member", "regular_attender" })
                .ExecuteAsync();
        }

        /// <summary>
        /// Example 5: Complex AND/OR Logic
        /// Combine multiple conditions with logical operators
        /// </summary>
        public async Task ComplexLogicalFilteringExample()
        {
            // Using WhereAnd for explicit AND logic
            var strictCriteria = await _peopleService.People
                .WhereAnd(
                    p => p.Status == "active",
                    p => p.Membership == "member"
                )
                .ExecuteAsync();

            // Using WhereOr for OR logic
            var flexibleCriteria = await _peopleService.People
                .WhereOr(
                    p => p.Status == "active",
                    p => p.Status == "visitor"
                )
                .ExecuteAsync();

            // Complex combination of AND/OR with other filters
            var complexQuery = await _peopleService.People
                .WhereOr(
                    p => p.Membership == "member",
                    p => p.Membership == "regular_attender"
                )
                .WhereNotNull("primary_email")
                .WhereNotIn("status", new[] { "inactive", "archived" })
                .ExecuteAsync();
        }

        /// <summary>
        /// Example 6: Advanced Combined Filtering
        /// Showcase multiple A3.1 features working together
        /// </summary>
        public async Task AdvancedCombinedFilteringExample()
        {
            var thirtyDaysAgo = DateTime.Today.AddDays(-30);
            var today = DateTime.Today;

            // Complex query combining all A3.1 features
            var comprehensiveQuery = await _peopleService.People
                // OR logic for membership types
                .WhereOr(
                    p => p.Membership == "member",
                    p => p.Membership == "regular_attender"
                )
                // Must have contact info
                .WhereNotNull("primary_email")
                // Exclude inactive statuses
                .WhereNotIn("status", new[] { "inactive", "archived", "deceased" })
                // Recent activity (if last_activity field exists)
                .WhereDateRange("last_activity", thirtyDaysAgo, today)
                // Age range (if age field exists)
                .WhereBetween("age", 18, 65)
                // Include related data
                .Include("emails", "phone_numbers", "households")
                // Sort and paginate
                .OrderBy("last_name")
                .OrderBy("first_name")
                .Page(1, 50)
                .ExecuteAsync();

            Console.WriteLine($"Found {comprehensiveQuery.Data.Count} people matching complex criteria");
        }

        /// <summary>
        /// Example 7: Building Dynamic Queries
        /// Show how to build queries dynamically based on conditions
        /// </summary>
        public async Task DynamicQueryBuildingExample(string? searchTerm, DateTime? startDate, List<string>? statuses)
        {
            var query = _peopleService.People.AsQueryable();

            // Add search term if provided
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.FirstName.Contains(searchTerm) || p.LastName.Contains(searchTerm));
            }

            // Add date filter if provided
            if (startDate.HasValue)
            {
                query = query.WhereDateRange("created_at", startDate.Value, DateTime.Today);
            }

            // Add status filter if provided
            if (statuses != null && statuses.Count > 0)
            {
                query = query.WhereIn("status", statuses);
            }
            else
            {
                // Default to active statuses
                query = query.WhereNotIn("status", new[] { "inactive", "archived" });
            }

            // Always exclude people without contact info
            query = query.WhereNotNull("primary_email");

            var results = await query.ExecuteAsync();
            Console.WriteLine($"Dynamic query returned {results.Data.Count} results");
        }

        /// <summary>
        /// Example 8: Performance Optimization with Complex Filters
        /// Show best practices for complex queries
        /// </summary>
        public async Task OptimizedComplexQueryExample()
        {
            // Use specific fields and limit results for better performance
            var optimizedQuery = await _peopleService.People
                // Most selective filters first
                .WhereIn("id", new[] { "123", "456", "789", "101", "102" })
                .WhereNotNull("primary_email")
                // Date range on indexed field
                .WhereDateRange("updated_at", DateTime.Today.AddDays(-7), DateTime.Today)
                // Limit results
                .Take(100)
                // Only include necessary relationships
                .Include("emails")
                .ExecuteAsync();

            Console.WriteLine($"Optimized query returned {optimizedQuery.Data.Count} results");
        }
    }

    /// <summary>
    /// Extension methods to make the examples more readable
    /// </summary>
    public static class FluentQueryExtensions
    {
        /// <summary>
        /// Extension method to start a queryable context (for demonstration)
        /// </summary>
        public static IFluentQueryBuilder<T> AsQueryable<T>(this IFluentQueryExecutor<T> executor)
        {
            return executor;
        }
    }
}