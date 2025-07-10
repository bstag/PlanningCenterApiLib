using FluentAssertions;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Core;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Models.Pagination;

/// <summary>
/// Simplified unit tests for PagedResponse that focus on basic functionality
/// without requiring complex mock setups.
/// </summary>
public class PagedResponseSimpleTests
{
    [Fact]
    public void PagedResponse_ShouldHaveCorrectDefaultValues()
    {
        // Arrange & Act
        var response = new PagedResponse<Person>();

        // Assert
        response.Data.Should().NotBeNull();
        response.Data.Should().BeEmpty();
        response.Meta.Should().BeNull();
        response.Links.Should().BeNull();
        response.HasNextPage.Should().BeFalse();
        response.HasPreviousPage.Should().BeFalse();
        response.IsFirstPage.Should().BeTrue(); // null <= 1 is true
        response.IsLastPage.Should().BeTrue(); // null >= null is true
        response.IsEmpty.Should().BeTrue();
    }

    [Fact]
    public void PagedResponse_ShouldIndicateCorrectPageStatus_WhenMetaIsSet()
    {
        // Arrange
        var response = new PagedResponse<Person>
        {
            Data = new List<Person> { new Person { FirstName = "Test" } },
            Meta = new PagedResponseMeta
            {
                CurrentPage = 2,
                TotalPages = 5,
                Count = 1,
                TotalCount = 100
            },
            Links = new PagedResponseLinks
            {
                Next = "http://example.com/page3",
                Previous = "http://example.com/page1"
            }
        };

        // Act & Assert
        response.IsEmpty.Should().BeFalse();
        response.IsFirstPage.Should().BeFalse(); // 2 <= 1 is false
        response.IsLastPage.Should().BeFalse(); // 2 >= 5 is false
        response.HasNextPage.Should().BeTrue(); // Links.Next exists
        response.HasPreviousPage.Should().BeTrue(); // Links.Previous exists
    }

    [Fact]
    public void PagedResponse_ShouldIndicateFirstPage_WhenCurrentPageIsOne()
    {
        // Arrange
        var response = new PagedResponse<Person>
        {
            Meta = new PagedResponseMeta { CurrentPage = 1, TotalPages = 3 }
        };

        // Act & Assert
        response.IsFirstPage.Should().BeTrue();
        response.IsLastPage.Should().BeFalse();
    }

    [Fact]
    public void PagedResponse_ShouldIndicateLastPage_WhenCurrentPageEqualsTotal()
    {
        // Arrange
        var response = new PagedResponse<Person>
        {
            Meta = new PagedResponseMeta { CurrentPage = 3, TotalPages = 3 }
        };

        // Act & Assert
        response.IsFirstPage.Should().BeFalse();
        response.IsLastPage.Should().BeTrue();
    }

    [Fact]
    public async Task GetNextPageAsync_ShouldReturnNull_WhenNoApiConnection()
    {
        // Arrange
        var response = new PagedResponse<Person>
        {
            ApiConnection = null,
            Links = new PagedResponseLinks { Next = "http://example.com/page2" }
        };

        // Act
        var result = await response.GetNextPageAsync();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPreviousPageAsync_ShouldReturnNull_WhenNoApiConnection()
    {
        // Arrange
        var response = new PagedResponse<Person>
        {
            ApiConnection = null,
            Links = new PagedResponseLinks { Previous = "http://example.com/page1" }
        };

        // Act
        var result = await response.GetPreviousPageAsync();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllRemainingAsync_ShouldReturnCurrentData_WhenNoNextPage()
    {
        // Arrange
        var people = new List<Person>
        {
            new Person { FirstName = "John" },
            new Person { FirstName = "Jane" }
        };
        
        var response = new PagedResponse<Person>
        {
            Data = people,
            Links = new PagedResponseLinks { Next = null } // No next page
        };

        // Act
        var result = await response.GetAllRemainingAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(people);
    }

    [Fact]
    public async Task GetAllRemainingAsyncEnumerable_ShouldYieldCurrentData_WhenNoNextPage()
    {
        // Arrange
        var people = new List<Person>
        {
            new Person { FirstName = "John" },
            new Person { FirstName = "Jane" }
        };
        
        var response = new PagedResponse<Person>
        {
            Data = people,
            Links = new PagedResponseLinks { Next = null } // No next page
        };

        // Act
        var result = new List<Person>();
        await foreach (var person in response.GetAllRemainingAsyncEnumerable())
        {
            result.Add(person);
        }

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(people);
    }
}