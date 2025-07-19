using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Exceptions;
using PlanningCenter.Api.Client.Services;
using PlanningCenter.Api.Client.Tests.Utilities;
using Xunit;

namespace PlanningCenter.Api.Client.Tests;

/// <summary>
/// Unit tests for ServiceBase - the base class for all API services.
/// This is critical infrastructure that provides common functionality.
/// </summary>
public class ServiceBaseTests
{
    private readonly MockApiConnection _mockApiConnection;
    private readonly TestService _testService;
    private readonly Mock<ILogger<TestService>> _mockLogger;

    public ServiceBaseTests()
    {
        _mockApiConnection = new MockApiConnection();
        _mockLogger = new Mock<ILogger<TestService>>();
        _testService = new TestService(_mockApiConnection, _mockLogger.Object);
        _mockLogger.Reset();
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidParameters_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var service = new TestService(_mockApiConnection, _mockLogger.Object);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var act = () => new TestService(_mockApiConnection, null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("logger");
    }

    [Fact]
    public void Constructor_WithNullApiConnection_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var act = () => new TestService(null!, _mockLogger.Object);
        act.Should().Throw<ArgumentNullException>().WithParameterName("apiConnection");
    }

    #endregion

    #region ExecuteAsync Tests

    [Fact]
    public async Task ExecuteAsync_WithSuccessfulOperation_ShouldReturnResult()
    {
        // Arrange
        var expectedResult = "test result";
        var operation = new Func<Task<string>>(() => Task.FromResult(expectedResult));

        // Act
        var result = await _testService.TestExecuteAsync(operation, "TestOperation");

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public async Task ExecuteAsync_WithFailingOperation_ShouldLogErrorAndRethrow()
    {
        // Arrange
        var exception = new InvalidOperationException("Test error");
        var operation = new Func<Task<string>>(() => throw exception);

        // Act & Assert
        var act = async () => await _testService.TestExecuteAsync(operation, "TestOperation");
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Test error");

        // Verify logging
        VerifyLoggerWasCalled(LogLevel.Error, Times.Once());
    }

    [Fact]
    public async Task ExecuteAsync_WithPlanningCenterApiException_ShouldLogApiErrorAndRethrow()
    {
        // Arrange
        var exception = new PlanningCenterApiGeneralException("API error", 500, "server_error");
        var operation = new Func<Task<string>>(() => throw exception);

        // Act & Assert
        var act = async () => await _testService.TestExecuteAsync(operation, "TestOperation");
        await act.Should().ThrowAsync<PlanningCenterApiGeneralException>().WithMessage("API error");

        // Verify API error logging
        VerifyLoggerWasCalled(LogLevel.Error, Times.Once());
    }

    [Fact]
    public async Task ExecuteAsync_WithNotFoundExceptionAndAllowNotFound_ShouldReturnDefault()
    {
        // Arrange
        var exception = new PlanningCenterApiNotFoundException("Resource", "123");
        var operation = new Func<Task<string>>(() => throw exception);

        // Act
        var result = await _testService.TestExecuteAsync(operation, "TestOperation", "123", allowNotFound: true);

        // Assert
        result.Should().BeNull();

        // Verify warning logging
        VerifyLoggerWasCalled(LogLevel.Warning, Times.Once());
    }

    [Fact]
    public async Task ExecuteAsync_WithNotFoundExceptionAndNotAllowNotFound_ShouldRethrow()
    {
        // Arrange
        var exception = new PlanningCenterApiNotFoundException("Resource", "123");
        var operation = new Func<Task<string>>(() => throw exception);

        // Act & Assert
        var act = async () => await _testService.TestExecuteAsync(operation, "TestOperation", "123", allowNotFound: false);
        await act.Should().ThrowAsync<PlanningCenterApiNotFoundException>();

        // Verify error logging
        VerifyLoggerWasCalled(LogLevel.Error, Times.Once());
    }

    #endregion

    #region ExecuteAsync (void) Tests

    [Fact]
    public async Task ExecuteAsync_VoidOperation_WithSuccessfulOperation_ShouldComplete()
    {
        // Arrange
        var operationCalled = false;
        var operation = new Func<Task>(() =>
        {
            operationCalled = true;
            return Task.CompletedTask;
        });

        // Act
        await _testService.TestExecuteAsync(operation, "TestOperation");

        // Assert
        operationCalled.Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_VoidOperation_WithFailingOperation_ShouldLogErrorAndRethrow()
    {
        // Arrange
        var exception = new InvalidOperationException("Test error");
        var operation = new Func<Task>(() => throw exception);

        // Act & Assert
        var act = async () => await _testService.TestExecuteAsync(operation, "TestOperation");
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Test error");
    }

    #endregion

    #region ExecuteGetAsync Tests

    [Fact]
    public async Task ExecuteGetAsync_WithSuccessfulOperation_ShouldReturnResult()
    {
        // Arrange
        var expectedResult = "test result";
        var operation = new Func<Task<string>>(() => Task.FromResult(expectedResult));

        // Act
        var result = await _testService.TestExecuteGetAsync(operation, "TestOperation", "123");

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public async Task ExecuteGetAsync_WithNotFoundException_ShouldReturnNull()
    {
        // Arrange
        var exception = new PlanningCenterApiNotFoundException("Resource", "123");
        var operation = new Func<Task<string>>(() => throw exception);

        // Act
        var result = await _testService.TestExecuteGetAsync(operation, "TestOperation", "123");

        // Assert
        result.Should().BeNull();

        // Verify warning logging
        VerifyLoggerWasCalled(LogLevel.Warning, Times.Once());
    }

    #endregion

    #region Validation Tests

    [Fact]
    public void ValidateNotNullOrEmpty_WithValidString_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        var act = () => TestService.TestValidateNotNullOrEmpty("valid string", "testParam");
        act.Should().NotThrow();
    }

    [Fact]
    public void ValidateNotNullOrEmpty_WithNullString_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var act = () => TestService.TestValidateNotNullOrEmpty(null, "testParam");
        act.Should().Throw<ArgumentException>().WithParameterName("testParam");
    }

    [Fact]
    public void ValidateNotNullOrEmpty_WithEmptyString_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var act = () => TestService.TestValidateNotNullOrEmpty("", "testParam");
        act.Should().Throw<ArgumentException>().WithParameterName("testParam");
    }

    [Fact]
    public void ValidateNotNullOrEmpty_WithWhitespaceString_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var act = () => TestService.TestValidateNotNullOrEmpty("   ", "testParam");
        act.Should().Throw<ArgumentException>().WithParameterName("testParam");
    }

    [Fact]
    public void ValidateNotNull_WithValidObject_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        var act = () => TestService.TestValidateNotNull(new object(), "testParam");
        act.Should().NotThrow();
    }

    [Fact]
    public void ValidateNotNull_WithNullObject_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var act = () => TestService.TestValidateNotNull<object>(null, "testParam");
        act.Should().Throw<ArgumentNullException>().WithParameterName("testParam");
    }

    #endregion

    #region Helper Methods

    private void VerifyLoggerWasCalled(LogLevel logLevel, Times times)
    {
        // This method verifies that the logger was called with the specified log level.
        // The mock should be reset in the test constructor to ensure a clean state for each test.
        
        // Verify that the logger was called with the specified log level
        _mockLogger.Verify(
            x => x.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times);
    }

    #endregion

    #region Test Service Implementation

    /// <summary>
    /// Test implementation of ServiceBase to expose protected methods for testing.
    /// </summary>
    public class TestService : ServiceBase
    {
        public TestService(IApiConnection apiConnection, ILogger<TestService> logger) 
            : base(logger, apiConnection)
        {
        }

        public async Task<T> TestExecuteAsync<T>(
            Func<Task<T>> operation,
            string operationName,
            string? resourceId = null,
            bool allowNotFound = false,
            CancellationToken cancellationToken = default)
        {
            return await ExecuteAsync(operation, operationName, resourceId, allowNotFound, cancellationToken);
        }

        public async Task TestExecuteAsync(
            Func<Task> operation,
            string operationName,
            string? resourceId = null,
            CancellationToken cancellationToken = default)
        {
            await ExecuteAsync(operation, operationName, resourceId, cancellationToken);
        }

        public async Task<T?> TestExecuteGetAsync<T>(
            Func<Task<T>> operation,
            string operationName,
            string resourceId,
            CancellationToken cancellationToken = default)
            where T : class
        {
            return await ExecuteGetAsync(operation, operationName, resourceId, cancellationToken);
        }

        public static void TestValidateNotNullOrEmpty(string? value, string paramName)
        {
            ValidateNotNullOrEmpty(value, paramName);
        }

        public static void TestValidateNotNull<T>(T? value, string paramName) where T : class
        {
            ValidateNotNull(value, paramName);
        }
    }

    #endregion
}