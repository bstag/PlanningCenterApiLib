using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using PlanningCenter.Api.Client.Fluent.QueryBuilder;
using PlanningCenter.Api.Client.Models.Core;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Fluent;

/// <summary>
/// Unit tests for FluentQueryBuilder complex filtering features.
/// </summary>
public class FluentQueryBuilderTests
{
    private readonly FluentQueryBuilder<Person> _queryBuilder;

    public FluentQueryBuilderTests()
    {
        _queryBuilder = new FluentQueryBuilder<Person>();
    }

    [Fact]
    public void WhereIn_WithStringValues_ShouldBuildCorrectFilter()
    {
        // Arrange
        var values = new[] { "John", "Jane", "Bob" };

        // Act
        var result = _queryBuilder.WhereIn("first_name", values.Cast<object>());
        var parameters = result.Build();

        // Assert
        result.Should().NotBeNull();
        parameters.Where.Should().ContainKey("first_name");
        parameters.Where["first_name"].Should().Be("John,Jane,Bob");
    }

    [Fact]
    public void WhereNotIn_WithIntegerValues_ShouldBuildCorrectFilter()
    {
        // Arrange
        var values = new[] { 1, 2, 3 };

        // Act
        var result = _queryBuilder.WhereNotIn("id", values.Cast<object>());
        var parameters = result.Build();

        // Assert
        result.Should().NotBeNull();
        parameters.Where.Should().ContainKey("id");
        parameters.Where["id"].Should().Be("!1,2,3");
    }

    [Fact]
    public void WhereNull_ShouldBuildCorrectFilter()
    {
        // Act
        var result = _queryBuilder.WhereNull("email");
        var parameters = result.Build();

        // Assert
        result.Should().NotBeNull();
        parameters.Where.Should().ContainKey("email");
        parameters.Where["email"].Should().Be("null");
    }

    #region Relationship Querying Tests

    [Fact]
    public void IncludeDeep_WithSinglePath_ShouldAddToIncludes()
    {
        // Act
        var result = _queryBuilder.IncludeDeep("households.members");
        var parameters = result.Build();

        // Assert
        result.Should().NotBeNull();
        parameters.Include.Should().Contain("households.members");
    }

    [Fact]
    public void IncludeDeep_WithMultiplePaths_ShouldAddAllToIncludes()
    {
        // Arrange
        var paths = new[] { "households.members", "field_data.field_definition", "emails" };

        // Act
        var result = _queryBuilder.IncludeDeep(paths);
        var parameters = result.Build();

        // Assert
        result.Should().NotBeNull();
        parameters.Include.Should().Contain("households.members");
        parameters.Include.Should().Contain("field_data.field_definition");
        parameters.Include.Should().Contain("emails");
    }

    [Fact]
    public void IncludeDeep_WithEmptyPath_ShouldThrowArgumentException()
    {
        // Act & Assert
        Action act = () => _queryBuilder.IncludeDeep("");
        act.Should().Throw<ArgumentException>()
            .WithMessage("Relationship path cannot be null or empty*");
    }

    [Fact]
    public void WhereHasRelationship_ShouldBuildCorrectFilter()
    {
        // Act
        var result = _queryBuilder.WhereHasRelationship("households");
        var parameters = result.Build();

        // Assert
        result.Should().NotBeNull();
        parameters.Where.Should().ContainKey("has_households");
        parameters.Where["has_households"].Should().Be("true");
    }

    [Fact]
    public void WhereDoesntHaveRelationship_ShouldBuildCorrectFilter()
    {
        // Act
        var result = _queryBuilder.WhereDoesntHaveRelationship("households");
        var parameters = result.Build();

        // Assert
        result.Should().NotBeNull();
        parameters.Where.Should().ContainKey("has_households");
        parameters.Where["has_households"].Should().Be("false");
    }

    [Fact]
    public void WhereHas_WithSingleCondition_ShouldBuildCorrectFilter()
    {
        // Act
        var result = _queryBuilder.WhereHas("households", "name", "Smith Family");
        var parameters = result.Build();

        // Assert
        result.Should().NotBeNull();
        parameters.Where.Should().ContainKey("households.name");
        parameters.Where["households.name"].Should().Be("Smith Family");
    }

    [Fact]
    public void WhereHas_WithMultipleConditions_ShouldBuildCorrectFilters()
    {
        // Arrange
        var filters = new Dictionary<string, object>
        {
            { "name", "Smith Family" },
            { "primary_contact_name", "John Smith" },
            { "member_count", 4 }
        };

        // Act
        var result = _queryBuilder.WhereHas("households", filters);
        var parameters = result.Build();

        // Assert
        result.Should().NotBeNull();
        parameters.Where.Should().ContainKey("households.name");
        parameters.Where["households.name"].Should().Be("Smith Family");
        parameters.Where.Should().ContainKey("households.primary_contact_name");
        parameters.Where["households.primary_contact_name"].Should().Be("John Smith");
        parameters.Where.Should().ContainKey("households.member_count");
        parameters.Where["households.member_count"].Should().Be("4");
    }

    [Fact]
    public void WhereRelationshipCount_ShouldBuildCorrectFilter()
    {
        // Act
        var result = _queryBuilder.WhereRelationshipCount("households", 2);
        var parameters = result.Build();

        // Assert
        result.Should().NotBeNull();
        parameters.Where.Should().ContainKey("households_count");
        parameters.Where["households_count"].Should().Be("2");
    }

    [Fact]
    public void WhereRelationshipCountGreaterThan_ShouldBuildCorrectFilter()
    {
        // Act
        var result = _queryBuilder.WhereRelationshipCountGreaterThan("households", 1);
        var parameters = result.Build();

        // Assert
        result.Should().NotBeNull();
        parameters.Where.Should().ContainKey("households_count");
        parameters.Where["households_count"].Should().Be(">1");
    }

    [Fact]
    public void WhereRelationshipCountLessThan_ShouldBuildCorrectFilter()
    {
        // Act
        var result = _queryBuilder.WhereRelationshipCountLessThan("households", 5);
        var parameters = result.Build();

        // Assert
        result.Should().NotBeNull();
        parameters.Where.Should().ContainKey("households_count");
        parameters.Where["households_count"].Should().Be("<5");
    }

    [Fact]
    public void WhereRelationshipCountBetween_ShouldBuildCorrectFilter()
    {
        // Act
        var result = _queryBuilder.WhereRelationshipCountBetween("households", 2, 5);
        var parameters = result.Build();

        // Assert
        result.Should().NotBeNull();
        parameters.Where.Should().ContainKey("households_count");
        parameters.Where["households_count"].Should().Be("2..5");
    }

    [Fact]
    public void WhereRelationshipCount_WithNegativeCount_ShouldThrowArgumentException()
    {
        // Act & Assert
        Action act = () => _queryBuilder.WhereRelationshipCount("households", -1);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Count cannot be negative*");
    }

    [Fact]
    public void WhereRelationshipCountBetween_WithInvalidRange_ShouldThrowArgumentException()
    {
        // Act & Assert
        Action act = () => _queryBuilder.WhereRelationshipCountBetween("households", 5, 2);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Min count must be less than or equal to max count*");
    }

    [Fact]
    public void WhereHasRelationship_WithNullRelationshipName_ShouldThrowArgumentException()
    {
        // Act & Assert
        Action act = () => _queryBuilder.WhereHasRelationship(null!);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Relationship name cannot be null or empty*");
    }

    [Fact]
    public void WhereHas_WithNullRelationshipName_ShouldThrowArgumentException()
    {
        // Act & Assert
        Action act = () => _queryBuilder.WhereHas(null!, "field", "value");
        act.Should().Throw<ArgumentException>()
            .WithMessage("Relationship name cannot be null or empty*");
    }

    [Fact]
    public void WhereHas_WithNullField_ShouldThrowArgumentException()
    {
        // Act & Assert
        Action act = () => _queryBuilder.WhereHas("households", null!, "value");
        act.Should().Throw<ArgumentException>()
            .WithMessage("Field cannot be null or empty*");
    }

    [Fact]
    public void ChainedRelationshipQueries_ShouldBuildCorrectFilters()
    {
        // Act
        var result = _queryBuilder
            .IncludeDeep("households.members", "field_data.field_definition")
            .WhereHasRelationship("households")
            .WhereHas("households", "name", "Smith Family")
            .WhereRelationshipCountGreaterThan("emails", 0);
        
        var parameters = result.Build();

        // Assert
        result.Should().NotBeNull();
        
        // Check includes
        parameters.Include.Should().Contain("households.members");
        parameters.Include.Should().Contain("field_data.field_definition");
        
        // Check relationship filters
        parameters.Where.Should().ContainKey("has_households");
        parameters.Where["has_households"].Should().Be("true");
        parameters.Where.Should().ContainKey("households.name");
        parameters.Where["households.name"].Should().Be("Smith Family");
        parameters.Where.Should().ContainKey("emails_count");
        parameters.Where["emails_count"].Should().Be(">0");
    }

    #endregion

    #region Aggregation Tests

    [Fact]
    public void GroupBy_WithSingleField_ShouldBuildCorrectParameter()
    {
        // Act
        var result = _queryBuilder.GroupBy("status");
        var parameters = result.Build();

        // Assert
        result.Should().NotBeNull();
        parameters.Where.Should().ContainKey("group_by");
        parameters.Where["group_by"].Should().Be("status");
    }

    [Fact]
    public void GroupBy_WithMultipleFields_ShouldBuildCorrectParameter()
    {
        // Act
        var result = _queryBuilder.GroupBy("status", "created_at", "updated_at");
        var parameters = result.Build();

        // Assert
        result.Should().NotBeNull();
        parameters.Where.Should().ContainKey("group_by");
        parameters.Where["group_by"].Should().Be("status,created_at,updated_at");
    }

    [Fact]
    public void GroupBy_WithExpression_ShouldBuildCorrectParameter()
    {
        // Act
        var result = _queryBuilder.GroupBy(p => p.Status);
        var parameters = result.Build();

        // Assert
        result.Should().NotBeNull();
        parameters.Where.Should().ContainKey("group_by");
        parameters.Where["group_by"].Should().Be("status");
    }

    [Fact]
    public void GroupBy_WithNullField_ShouldThrowArgumentException()
    {
        // Act & Assert
        Action act = () => _queryBuilder.GroupBy((string)null!);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Field cannot be null or empty*");
    }

    [Fact]
    public void GroupBy_WithEmptyFields_ShouldThrowArgumentException()
    {
        // Act & Assert
        Action act = () => _queryBuilder.GroupBy(new string[0]);
        act.Should().Throw<ArgumentException>()
            .WithMessage("At least one field must be specified*");
    }

    [Fact]
    public void GroupBy_WithNullExpression_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Action act = () => _queryBuilder.GroupBy((Expression<Func<Person, object>>)null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Having_WithSingleCondition_ShouldBuildCorrectParameter()
    {
        // Act
        var result = _queryBuilder.Having("count", 5);
        var parameters = result.Build();

        // Assert
        result.Should().NotBeNull();
        parameters.Where.Should().ContainKey("having[count]");
        parameters.Where["having[count]"].Should().Be("5");
    }

    [Fact]
    public void Having_WithMultipleConditions_ShouldBuildCorrectParameters()
    {
        // Arrange
        var conditions = new Dictionary<string, object>
        {
            { "count", 5 },
            { "avg_age", 25.5 },
            { "status", "active" }
        };

        // Act
        var result = _queryBuilder.Having(conditions);
        var parameters = result.Build();

        // Assert
        result.Should().NotBeNull();
        parameters.Where.Should().ContainKey("having[count]");
        parameters.Where["having[count]"].Should().Be("5");
        parameters.Where.Should().ContainKey("having[avg_age]");
        parameters.Where["having[avg_age]"].Should().Be("25.5");
        parameters.Where.Should().ContainKey("having[status]");
        parameters.Where["having[status]"].Should().Be("active");
    }

    [Fact]
    public void Having_WithExpression_ShouldBuildCorrectParameter()
    {
        // Act
        var result = _queryBuilder.Having(g => g.Count() > 5);
        var parameters = result.Build();

        // Assert
        result.Should().NotBeNull();
        parameters.Where.Should().ContainKey("having_expression");
        parameters.Where["having_expression"].ToString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Having_WithNullField_ShouldThrowArgumentException()
    {
        // Act & Assert
        Action act = () => _queryBuilder.Having(null!, "value");
        act.Should().Throw<ArgumentException>()
            .WithMessage("Field cannot be null or empty*");
    }

    [Fact]
    public void Having_WithNullConditions_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Action act = () => _queryBuilder.Having((Dictionary<string, object>)null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Having_WithNullExpression_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Action act = () => _queryBuilder.Having((Expression<Func<IGrouping<object, Person>, bool>>)null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ChainedGroupingAndHaving_ShouldBuildCorrectParameters()
    {
        // Act
        var result = _queryBuilder
            .GroupBy("status", "created_at")
            .Having("count", 5)
            .Having("avg_age", 25);
        
        var parameters = result.Build();

        // Assert
        result.Should().NotBeNull();
        parameters.Where.Should().ContainKey("group_by");
        parameters.Where["group_by"].Should().Be("status,created_at");
        parameters.Where.Should().ContainKey("having[count]");
        parameters.Where["having[count]"].Should().Be("5");
        parameters.Where.Should().ContainKey("having[avg_age]");
        parameters.Where["having[avg_age]"].Should().Be("25");
    }

    [Fact]
    public void ComplexAggregationQuery_ShouldBuildCorrectParameters()
    {
        // Act
        var result = _queryBuilder
            .Where("status", "active")
            .GroupBy("department", "position")
            .Having("count", 10)
            .Having(new Dictionary<string, object> { { "avg_salary", 50000 }, { "max_age", 65 } })
            .OrderBy("department")
            .Take(50);
        
        var parameters = result.Build();

        // Assert
        result.Should().NotBeNull();
        
        // Check where conditions
        parameters.Where.Should().ContainKey("status");
        parameters.Where["status"].Should().Be("active");
        
        // Check grouping
        parameters.Where.Should().ContainKey("group_by");
        parameters.Where["group_by"].Should().Be("department,position");
        
        // Check having conditions
        parameters.Where.Should().ContainKey("having[count]");
        parameters.Where["having[count]"].Should().Be("10");
        parameters.Where.Should().ContainKey("having[avg_salary]");
        parameters.Where["having[avg_salary]"].Should().Be("50000");
        parameters.Where.Should().ContainKey("having[max_age]");
        parameters.Where["having[max_age]"].Should().Be("65");
        
        // Check ordering and pagination
        parameters.OrderBy.Should().Be("department");
        parameters.PerPage.Should().Be(50);
    }

    #endregion
}