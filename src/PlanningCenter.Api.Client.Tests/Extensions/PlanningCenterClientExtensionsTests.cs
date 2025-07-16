using FluentAssertions;
using Moq;
using PlanningCenter.Api.Client.Extensions;
using PlanningCenter.Api.Client.Fluent;
using PlanningCenter.Api.Client.Models;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Extensions;

/// <summary>
/// Unit tests for PlanningCenterClientExtensions.
/// Tests the extension methods that provide fluent API access.
/// </summary>
public class PlanningCenterClientExtensionsTests
{
    private readonly Mock<IPlanningCenterClient> _mockClient;

    public PlanningCenterClientExtensionsTests()
    {
        _mockClient = new Mock<IPlanningCenterClient>();
    }

    [Fact]
    public void Fluent_ShouldReturnFluentClient_WhenClientIsProvided()
    {
        // Act
        var result = _mockClient.Object.Fluent();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<PlanningCenterFluentClient>();
    }

    [Fact]
    public void Fluent_ShouldThrowArgumentNullException_WhenClientIsNull()
    {
        // Arrange
        IPlanningCenterClient? nullClient = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => nullClient!.Fluent());
    }

    [Fact]
    public void Fluent_ShouldReturnSameInstanceType_WhenCalledMultipleTimes()
    {
        // Act
        var result1 = _mockClient.Object.Fluent();
        var result2 = _mockClient.Object.Fluent();

        // Assert
        result1.Should().BeOfType<PlanningCenterFluentClient>();
        result2.Should().BeOfType<PlanningCenterFluentClient>();
        // Note: They won't be the same instance since we create a new one each time,
        // but they should be the same type
    }
}