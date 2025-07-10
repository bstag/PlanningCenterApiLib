using FluentAssertions;
using PlanningCenter.Api.Client.Models.Exceptions;
using System.Net;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Models.Exceptions;

/// <summary>
/// Unit tests for the Planning Center API exception hierarchy.
/// </summary>
public class PlanningCenterApiExceptionTests
{
    [Fact]
    public void PlanningCenterApiException_ShouldInheritFromException()
    {
        // Arrange & Act
        var exception = new PlanningCenterApiException("Test message");

        // Assert
        exception.Should().BeAssignableTo<Exception>();
        exception.Message.Should().Be("Test message");
    }

    [Fact]
    public void PlanningCenterApiException_ShouldHaveDefaultConstructor()
    {
        // Arrange & Act
        var exception = new PlanningCenterApiException();

        // Assert
        exception.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void PlanningCenterApiException_ShouldAcceptInnerException()
    {
        // Arrange
        var innerException = new InvalidOperationException("Inner exception");

        // Act
        var exception = new PlanningCenterApiException("Test message", innerException);

        // Assert
        exception.Message.Should().Be("Test message");
        exception.InnerException.Should().BeSameAs(innerException);
    }

    [Fact]
    public void PlanningCenterApiAuthenticationException_ShouldInheritFromBase()
    {
        // Arrange & Act
        var exception = new PlanningCenterApiAuthenticationException("Auth failed");

        // Assert
        exception.Should().BeAssignableTo<PlanningCenterApiException>();
        exception.Message.Should().Be("Auth failed");
    }

    [Fact]
    public void PlanningCenterApiAuthorizationException_ShouldInheritFromBase()
    {
        // Arrange & Act
        var exception = new PlanningCenterApiAuthorizationException("Access denied");

        // Assert
        exception.Should().BeAssignableTo<PlanningCenterApiException>();
        exception.Message.Should().Be("Access denied");
    }

    [Fact]
    public void PlanningCenterApiNotFoundException_ShouldInheritFromBase()
    {
        // Arrange & Act
        var exception = new PlanningCenterApiNotFoundException("Resource not found");

        // Assert
        exception.Should().BeAssignableTo<PlanningCenterApiException>();
        exception.Message.Should().Be("Resource not found");
    }

    [Fact]
    public void PlanningCenterApiValidationException_ShouldInheritFromBase()
    {
        // Arrange & Act
        var exception = new PlanningCenterApiValidationException("Validation failed");

        // Assert
        exception.Should().BeAssignableTo<PlanningCenterApiException>();
        exception.Message.Should().Be("Validation failed");
    }

    [Fact]
    public void PlanningCenterApiRateLimitException_ShouldInheritFromBase()
    {
        // Arrange & Act
        var exception = new PlanningCenterApiRateLimitException("Rate limit exceeded");

        // Assert
        exception.Should().BeAssignableTo<PlanningCenterApiException>();
        exception.Message.Should().Be("Rate limit exceeded");
    }

    [Fact]
    public void PlanningCenterApiRateLimitException_ShouldHaveRetryAfterProperty()
    {
        // Arrange
        var retryAfter = TimeSpan.FromMinutes(5);

        // Act
        var exception = new PlanningCenterApiRateLimitException("Rate limit exceeded", retryAfter);

        // Assert
        exception.RetryAfter.Should().Be(retryAfter);
        exception.Message.Should().Be("Rate limit exceeded");
    }

    [Fact]
    public void PlanningCenterApiRateLimitException_ShouldHaveDefaultRetryAfter()
    {
        // Arrange & Act
        var exception = new PlanningCenterApiRateLimitException("Rate limit exceeded");

        // Assert
        exception.RetryAfter.Should().Be(TimeSpan.FromMinutes(1));
    }

    [Fact]
    public void PlanningCenterApiServerException_ShouldInheritFromBase()
    {
        // Arrange & Act
        var exception = new PlanningCenterApiServerException("Server error");

        // Assert
        exception.Should().BeAssignableTo<PlanningCenterApiException>();
        exception.Message.Should().Be("Server error");
    }

    [Fact]
    public void PlanningCenterApiGeneralException_ShouldInheritFromBase()
    {
        // Arrange & Act
        var exception = new PlanningCenterApiGeneralException("General error");

        // Assert
        exception.Should().BeAssignableTo<PlanningCenterApiException>();
        exception.Message.Should().Be("General error");
    }

    [Theory]
    [InlineData(HttpStatusCode.Unauthorized, typeof(PlanningCenterApiAuthenticationException))]
    [InlineData(HttpStatusCode.Forbidden, typeof(PlanningCenterApiAuthorizationException))]
    [InlineData(HttpStatusCode.NotFound, typeof(PlanningCenterApiNotFoundException))]
    [InlineData(HttpStatusCode.BadRequest, typeof(PlanningCenterApiValidationException))]
    [InlineData(HttpStatusCode.UnprocessableEntity, typeof(PlanningCenterApiValidationException))]
    [InlineData(HttpStatusCode.TooManyRequests, typeof(PlanningCenterApiRateLimitException))]
    [InlineData(HttpStatusCode.InternalServerError, typeof(PlanningCenterApiServerException))]
    [InlineData(HttpStatusCode.BadGateway, typeof(PlanningCenterApiServerException))]
    [InlineData(HttpStatusCode.ServiceUnavailable, typeof(PlanningCenterApiServerException))]
    [InlineData(HttpStatusCode.GatewayTimeout, typeof(PlanningCenterApiServerException))]
    public void CreateFromHttpStatusCode_ShouldReturnCorrectExceptionType(
        HttpStatusCode statusCode, 
        Type expectedExceptionType)
    {
        // Arrange
        var message = "Test error message";
        var endpoint = "/test/endpoint";

        // Act
        var exception = PlanningCenterApiException.CreateFromHttpStatusCode(
            statusCode, message, endpoint);

        // Assert
        exception.Should().BeOfType(expectedExceptionType);
        exception.Message.Should().Contain(message);
        exception.Message.Should().Contain(endpoint);
    }

    [Fact]
    public void CreateFromHttpStatusCode_ShouldCreateGeneralException_ForUnknownStatusCode()
    {
        // Arrange
        var statusCode = HttpStatusCode.Continue; // Unusual status code
        var message = "Test error message";
        var endpoint = "/test/endpoint";

        // Act
        var exception = PlanningCenterApiException.CreateFromHttpStatusCode(
            statusCode, message, endpoint);

        // Assert
        exception.Should().BeOfType<PlanningCenterApiGeneralException>();
        exception.Message.Should().Contain(message);
        exception.Message.Should().Contain(endpoint);
        exception.Message.Should().Contain(statusCode.ToString());
    }

    [Fact]
    public void CreateFromHttpStatusCode_ShouldIncludeRetryAfter_ForRateLimitException()
    {
        // Arrange
        var statusCode = HttpStatusCode.TooManyRequests;
        var message = "Rate limit exceeded";
        var endpoint = "/test/endpoint";
        var retryAfter = TimeSpan.FromMinutes(10);

        // Act
        var exception = PlanningCenterApiException.CreateFromHttpStatusCode(
            statusCode, message, endpoint, retryAfter);

        // Assert
        exception.Should().BeOfType<PlanningCenterApiRateLimitException>();
        var rateLimitException = (PlanningCenterApiRateLimitException)exception;
        rateLimitException.RetryAfter.Should().Be(retryAfter);
    }

    [Fact]
    public void AllExceptions_ShouldBeSerializable()
    {
        // This test ensures all exceptions can be serialized for logging/telemetry purposes
        var exceptions = new PlanningCenterApiException[]
        {
            new PlanningCenterApiException("Test"),
            new PlanningCenterApiAuthenticationException("Auth failed"),
            new PlanningCenterApiAuthorizationException("Access denied"),
            new PlanningCenterApiNotFoundException("Not found"),
            new PlanningCenterApiValidationException("Validation failed"),
            new PlanningCenterApiRateLimitException("Rate limited", TimeSpan.FromMinutes(5)),
            new PlanningCenterApiServerException("Server error"),
            new PlanningCenterApiGeneralException("General error")
        };

        foreach (var exception in exceptions)
        {
            // Arrange & Act
            var serialized = System.Text.Json.JsonSerializer.Serialize(new
            {
                Type = exception.GetType().Name,
                Message = exception.Message,
                StackTrace = exception.StackTrace
            });

            // Assert
            serialized.Should().NotBeNullOrEmpty();
            serialized.Should().Contain(exception.GetType().Name);
            serialized.Should().Contain(exception.Message);
        }
    }
}