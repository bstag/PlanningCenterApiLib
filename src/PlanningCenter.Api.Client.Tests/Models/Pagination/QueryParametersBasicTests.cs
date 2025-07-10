using FluentAssertions;
using PlanningCenter.Api.Client.Models;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Models.Pagination;

/// <summary>
/// Basic unit tests for the QueryParameters model that work with current implementation.
/// </summary>
public class QueryParametersBasicTests
{
    [Fact]
    public void QueryParameters_ShouldHaveCorrectDefaultValues()
    {
        // Arrange & Act
        var parameters = new QueryParameters();

        // Assert
        parameters.Where.Should().NotBeNull();
        parameters.Where.Should().BeEmpty();
        parameters.Include.Should().NotBeNull();
        parameters.Include.Should().BeEmpty();
        parameters.OrderBy.Should().BeNull();
        parameters.PerPage.Should().BeNull();
        parameters.Offset.Should().BeNull();
    }

    [Fact]
    public void QueryParameters_ShouldAllowSettingAllProperties()
    {
        // Arrange
        var parameters = new QueryParameters();

        // Act
        parameters.Where["status"] = "active";
        parameters.Where["name"] = "John";
        parameters.Include = new[] { "addresses", "emails" };
        parameters.OrderBy = "created_at";
        parameters.PerPage = 50;
        parameters.Offset = 100;

        // Assert
        parameters.Where.Should().HaveCount(2);
        parameters.Where["status"].Should().Be("active");
        parameters.Where["name"].Should().Be("John");
        parameters.Include.Should().BeEquivalentTo(new[] { "addresses", "emails" });
        parameters.OrderBy.Should().Be("created_at");
        parameters.PerPage.Should().Be(50);
        parameters.Offset.Should().Be(100);
    }

    [Fact]
    public void ToQueryString_ShouldReturnEmptyString_WhenNoParametersAreSet()
    {
        // Arrange
        var parameters = new QueryParameters();

        // Act
        var queryString = parameters.ToQueryString();

        // Assert
        queryString.Should().BeEmpty();
    }

    [Fact]
    public void ToQueryString_ShouldIncludeWhereParameters()
    {
        // Arrange
        var parameters = new QueryParameters();
        parameters.Where["status"] = "active";
        parameters.Where["membership"] = "member";

        // Act
        var queryString = parameters.ToQueryString();

        // Assert
        queryString.Should().Contain("where[status]=active");
        queryString.Should().Contain("where[membership]=member");
        queryString.Should().Contain("&");
    }

    [Fact]
    public void ToQueryString_ShouldIncludeIncludeParameters()
    {
        // Arrange
        var parameters = new QueryParameters();
        parameters.Include = new[] { "addresses", "emails", "phone_numbers" };

        // Act
        var queryString = parameters.ToQueryString();

        // Assert
        queryString.Should().Be("include=addresses,emails,phone_numbers");
    }

    [Fact]
    public void ToQueryString_ShouldIncludeOrderParameter()
    {
        // Arrange
        var parameters = new QueryParameters();
        parameters.OrderBy = "created_at";

        // Act
        var queryString = parameters.ToQueryString();

        // Assert
        queryString.Should().Be("order=created_at");
    }

    [Fact]
    public void ToQueryString_ShouldIncludePerPageParameter()
    {
        // Arrange
        var parameters = new QueryParameters();
        parameters.PerPage = 100;

        // Act
        var queryString = parameters.ToQueryString();

        // Assert
        queryString.Should().Be("per_page=100");
    }

    [Fact]
    public void ToQueryString_ShouldIncludeOffsetParameter()
    {
        // Arrange
        var parameters = new QueryParameters();
        parameters.Offset = 50;

        // Act
        var queryString = parameters.ToQueryString();

        // Assert
        queryString.Should().Be("offset=50");
    }

    [Fact]
    public void ToQueryString_ShouldCombineAllParameters()
    {
        // Arrange
        var parameters = new QueryParameters();
        parameters.Where["status"] = "active";
        parameters.Include = new[] { "addresses", "emails" };
        parameters.OrderBy = "created_at";
        parameters.PerPage = 25;
        parameters.Offset = 0;

        // Act
        var queryString = parameters.ToQueryString();

        // Assert
        queryString.Should().Contain("where[status]=active");
        queryString.Should().Contain("include=addresses,emails");
        queryString.Should().Contain("order=created_at");
        queryString.Should().Contain("per_page=25");
        queryString.Should().Contain("offset=0");
        
        // Should have proper separators
        var parameterCount = queryString.Split('&').Length;
        parameterCount.Should().Be(5);
    }

    [Fact]
    public void ToQueryString_ShouldUrlEncodeValues()
    {
        // Arrange
        var parameters = new QueryParameters();
        parameters.Where["name"] = "John Doe";
        parameters.Where["email"] = "john@example.com";
        parameters.OrderBy = "first_name desc";

        // Act
        var queryString = parameters.ToQueryString();

        // Assert
        queryString.Should().Contain("where[name]=John%20Doe");
        queryString.Should().Contain("where[email]=john%40example.com");
        queryString.Should().Contain("order=first_name%20desc");
    }

    [Fact]
    public void Clone_ShouldCreateDeepCopy()
    {
        // Arrange
        var original = new QueryParameters();
        original.Where["status"] = "active";
        original.Where["name"] = "John";
        original.Include = new[] { "addresses", "emails" };
        original.OrderBy = "created_at";
        original.PerPage = 25;
        original.Offset = 50;

        // Act
        var clone = original.Clone();

        // Assert
        clone.Should().NotBeSameAs(original);
        clone.Where.Should().NotBeSameAs(original.Where);
        clone.Where.Should().BeEquivalentTo(original.Where);
        clone.Include.Should().BeEquivalentTo(original.Include);
        clone.OrderBy.Should().Be(original.OrderBy);
        clone.PerPage.Should().Be(original.PerPage);
        clone.Offset.Should().Be(original.Offset);

        // Verify it's a deep copy by modifying the clone
        clone.Where["status"] = "inactive";
        original.Where["status"].Should().Be("active");
    }
}