using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using PlanningCenter.Api.Client.Exceptions;
using PlanningCenter.Api.Client.Models.Exceptions;

namespace PlanningCenter.Api.Client.Tests.Exceptions;

/// <summary>
/// Unit tests for GlobalExceptionHandler - centralized exception handling.
/// This is critical infrastructure for consistent error handling across the SDK.
/// </summary>
public class GlobalExceptionHandlerTests
{
    private Mock<ILogger<GlobalExceptionHandlerTests>> _mockLogger;

    public GlobalExceptionHandlerTests()
    {
        _mockLogger = new Mock<ILogger<GlobalExceptionHandlerTests>>();
    }

    #region Handle Method Tests

    [Fact]
    public void Handle_WithPlanningCenterApiException_ShouldLogApiError()
    {
        // Arrange
        var exception = new PlanningCenterApiGeneralException(
            "API error", 
            500, 
            "server_error", 
            requestId: "req-123",
            requestUrl: "https://api.planningcenteronline.com/people/v2/people");

        // Act
        GlobalExceptionHandler.Handle(_mockLogger.Object, exception, "TestOperation", "resource123");

        // Assert
        _mockLogger.Verify(x => x.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("API error for TestOperation: API error")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Handle_WithPlanningCenterApiNotFoundException_ShouldLogNotFoundWarning()
    {
        // Arrange
        var exception = new PlanningCenterApiNotFoundException(
            "Person", 
            "123", 
            requestId: "req-404");

        // Act
        GlobalExceptionHandler.Handle(_mockLogger.Object, exception, "GetPerson", "123");

        // Assert
        _mockLogger.Verify(x => x.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Resource not found: 123 - Person not found")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Handle_WithPlanningCenterApiAuthenticationException_ShouldLogAuthError()
    {
        // Arrange
        var exception = new PlanningCenterApiAuthenticationException(
            "Invalid credentials", 
            requestId: "req-401");

        // Act
        GlobalExceptionHandler.Handle(_mockLogger.Object, exception, "AuthenticateUser");

        // Assert
        _mockLogger.Verify(x => x.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Authentication error for AuthenticateUser: Invalid credentials")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Handle_WithPlanningCenterApiAuthorizationException_ShouldLogAuthzError()
    {
        // Arrange
        var exception = new PlanningCenterApiAuthorizationException(
            "Insufficient permissions", 
            requestId: "req-403");

        // Act
        GlobalExceptionHandler.Handle(_mockLogger.Object, exception, "AccessResource");

        // Assert
        _mockLogger.Verify(x => x.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Authorization error for AccessResource: Insufficient permissions")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Handle_WithPlanningCenterApiRateLimitException_ShouldLogRateLimitWarning()
    {
        // Arrange
        var exception = new PlanningCenterApiRateLimitException(
            "Rate limit exceeded", 
            null, // resetTime
            TimeSpan.FromMinutes(5), // retryAfter
            requestId: "req-202");

        // Act
        GlobalExceptionHandler.Handle(_mockLogger.Object, exception, "MakeRequest");

        // Assert
        _mockLogger.Verify(x => x.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Rate limit exceeded for MakeRequest: Rate limit exceeded, Retry after: 00:05:00")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Handle_WithPlanningCenterApiValidationException_ShouldLogValidationError()
    {
        // Arrange
        var validationErrors = new Dictionary<string, List<string>>
        {
            ["name"] = ["Name is required"],
            ["email"] = ["Email is invalid", "Email is required"]
        };

        var exception = new PlanningCenterApiValidationException(
            "Validation failed", 
            validationErrors,
            400,
            "req-303"
        );

        // Act
        GlobalExceptionHandler.Handle(_mockLogger.Object, exception, "CreatePerson");

        // Assert
        _mockLogger.Verify(x => x.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Validation error for CreatePerson: name: Name is required; email: Email is invalid")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Handle_WithPlanningCenterApiServerException_ShouldLogServerError()
    {
        // Arrange
        var exception = new PlanningCenterApiServerException(
            "Internal server error", 
            500, 
            requestId: "req-500");

        // Act
        GlobalExceptionHandler.Handle(_mockLogger.Object, exception, "ProcessData");

        // Assert
        _mockLogger.Verify(x => x.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Server error for ProcessData: Internal server error")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Handle_WithGenericException_ShouldLogGeneralError()
    {
        // Arrange
        var exception = new InvalidOperationException("Something went wrong");

        // Act
        GlobalExceptionHandler.Handle(_mockLogger.Object, exception, "GenericOperation", "resource456");

        // Assert
        _mockLogger.Verify(x => x.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("General error: Unexpected error in GenericOperation for resource resource456")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Handle_WithHttpRequestException_ShouldLogNetworkError()
    {
        // Arrange
        var exception = new HttpRequestException("Network error occurred");

        // Act
        GlobalExceptionHandler.Handle(_mockLogger.Object, exception, "NetworkOperation");

        // Assert
        _mockLogger.Verify(x => x.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Network error in NetworkOperation for resource unknown")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Handle_WithTaskCanceledException_ShouldLogCancellationWarning()
    {
        // Arrange
        var exception = new TaskCanceledException("Operation was cancelled");

        // Act
        GlobalExceptionHandler.Handle(_mockLogger.Object, exception, "CancellableOperation");

        // Assert
        _mockLogger.Verify(x => x.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Operation cancelled: CancellableOperation was cancelled")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Handle_WithOperationCanceledException_ShouldLogCancellationWarning()
    {
        // Arrange
        var exception = new OperationCanceledException("Operation was cancelled");

        // Act
        GlobalExceptionHandler.Handle(_mockLogger.Object, exception, "CancellableOperation");

        // Assert
        _mockLogger.Verify(x => x.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Operation cancelled: CancellableOperation was cancelled")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    #endregion

    #region Handle with Additional Context Tests

    [Fact]
    public void Handle_WithAdditionalContext_ShouldIncludeContextInLog()
    {
        // Arrange
        var exception = new InvalidOperationException("Test error");
        var additionalContext = new Dictionary<string, object>
        {
            ["UserId"] = "user123",
            ["RequestSize"] = 1024,
            ["Timestamp"] = DateTime.UtcNow
        };

        // Act
        GlobalExceptionHandler.Handle(_mockLogger.Object, exception, "TestOperation", "resource789", additionalContext);

        // Assert
        _mockLogger.Verify(x => x.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("General error: Unexpected error in TestOperation for resource resource789")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Handle_WithNullAdditionalContext_ShouldNotThrow()
    {
        // Arrange
        var exception = new InvalidOperationException("Test error");

        // Act & Assert
        var act = () => GlobalExceptionHandler.Handle(_mockLogger.Object, exception, "TestOperation", "resource789", null);
        act.Should().NotThrow();
    }

    [Fact]
    public void Handle_WithEmptyAdditionalContext_ShouldNotThrow()
    {
        // Arrange
        var exception = new InvalidOperationException("Test error");
        var additionalContext = new Dictionary<string, object>();

        // Act & Assert
        var act = () => GlobalExceptionHandler.Handle(_mockLogger.Object, exception, "TestOperation", "resource789", additionalContext);
        act.Should().NotThrow();
    }

    #endregion

    #region Parameter Validation Tests

    [Fact]
    public void Handle_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange
        var exception = new InvalidOperationException("Test error");

        // Act & Assert
        var act = () => GlobalExceptionHandler.Handle(null!, exception, "TestOperation");
        act.Should().Throw<ArgumentNullException>().WithParameterName("logger");
    }

    [Fact]
    public void Handle_WithNullException_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var act = () => GlobalExceptionHandler.Handle(_mockLogger.Object, null!, "TestOperation");
        act.Should().Throw<ArgumentNullException>().WithParameterName("exception");
    }

    [Fact]
    public void Handle_WithNullOperationName_ShouldThrowArgumentNullException()
    {
        // Arrange
        var exception = new InvalidOperationException("Test error");

        // Act & Assert
        var act = () => GlobalExceptionHandler.Handle(_mockLogger.Object, exception, null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("operationName");
    }

    [Fact]
    public void Handle_WithEmptyOperationName_ShouldThrowArgumentException()
    {
        // Arrange
        var exception = new InvalidOperationException("Test error");

        // Act & Assert
        var act = () => GlobalExceptionHandler.Handle(_mockLogger.Object, exception, "");
        act.Should().Throw<ArgumentException>().WithParameterName("operationName");
    }

    #endregion

    #region CreateErrorContext Tests

    [Fact]
    public void CreateErrorContext_WithAllParameters_ShouldCreateCompleteContext()
    {
        // Arrange
        var exception = new PlanningCenterApiGeneralException("Test error", 400, "bad_request", "req-123", "https://api.example.com/test");

        var additionalContext = new Dictionary<string, object>
        {
            ["CustomProperty"] = "CustomValue"
        };

        // Act
        var result = GlobalExceptionHandler.CreateErrorContext(exception, "TestOperation", "resource123", additionalContext);

        // Assert
        result.Should().NotBeNull();
        result.Should().ContainKey("Operation");
        result.Should().ContainKey("ResourceId");
        result.Should().ContainKey("ExceptionType");
        result.Should().ContainKey("Message");
        result.Should().ContainKey("RequestId");
        result.Should().ContainKey("RequestUrl");
        result.Should().ContainKey("StatusCode");
        result.Should().ContainKey("ErrorCode");
        result.Should().ContainKey("CustomProperty");

        result["Operation"].Should().Be("TestOperation");
        result["ResourceId"].Should().Be("resource123");
        result["CustomProperty"].Should().Be("CustomValue");
    }

    [Fact]
    public void CreateErrorContext_WithMinimalParameters_ShouldCreateBasicContext()
    {
        // Arrange
        var exception = new InvalidOperationException("Test error");

        // Act
        var result = GlobalExceptionHandler.CreateErrorContext(exception, "TestOperation");

        // Assert
        result.Should().NotBeNull();
        result.Should().ContainKey("Operation");
        result.Should().ContainKey("ExceptionType");
        result.Should().ContainKey("Message");
        result["Operation"].Should().Be("TestOperation");
        result["ExceptionType"].Should().Be("InvalidOperationException");
        result["Message"].Should().Be("Test error");
    }

    #endregion

}