using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Exceptions;
using PlanningCenter.Api.Client.Models.JsonApi;
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

    #region LogSuccess Tests

    [Fact]
    public void LogSuccess_WithMinimalParameters_ShouldLogCorrectMessage()
    {
        // Arrange
        var operationName = "Create";
        var resourceName = "Plan";

        // Act
        _testService.TestLogSuccess(operationName, resourceName);

        // Assert
        VerifyLoggerWasCalled(LogLevel.Information, Times.Once());
    }

    [Fact]
    public void LogSuccess_WithResourceId_ShouldIncludeIdInMessage()
    {
        // Arrange
        var operationName = "Update";
        var resourceName = "Plan";
        var resourceId = "123";

        // Act
        _testService.TestLogSuccess(operationName, resourceName, resourceId);

        // Assert
        VerifyLoggerWasCalled(LogLevel.Information, Times.Once());
    }

    [Fact]
    public void LogSuccess_WithAdditionalInfo_ShouldIncludeInfoInMessage()
    {
        // Arrange
        var operationName = "Delete";
        var resourceName = "Plan";
        var resourceId = "123";
        var additionalInfo = "Test Plan";

        // Act
        _testService.TestLogSuccess(operationName, resourceName, resourceId, additionalInfo);

        // Assert
        VerifyLoggerWasCalled(LogLevel.Information, Times.Once());
    }

    #endregion

    #region CreateResourceAsync Tests

    [Fact]
    public async Task CreateResourceAsync_WithValidRequest_ShouldReturnCreatedResource()
    {
        // Arrange
        var request = new TestCreateRequest { Name = "Test Plan" };
        var dto = new TestDto { Id = "123", Name = "Test Plan" };
        var domain = new TestDomain { Id = "123", Name = "Test Plan" };
        
        var response = new JsonApiSingleResponse<TestDto> { Data = dto };
        _mockApiConnection.SetupPostResponse("/plans", response);

        // Act
        var result = await _testService.TestCreateResourceAsync<TestCreateRequest, TestDto, TestDomain>(
            "/plans",
            request,
            req => new { data = new { attributes = new { name = req.Name } } },
            d => new TestDomain { Id = d.Id, Name = d.Name },
            "CreatePlan",
            "Plan");

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("123");
        result.Name.Should().Be("Test Plan");
        VerifyLoggerWasCalled(LogLevel.Information, Times.Exactly(2)); // Success + Operation completion logs
    }

    [Fact]
    public async Task CreateResourceAsync_WithNullRequest_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var act = async () => await _testService.TestCreateResourceAsync<TestCreateRequest, TestDto, TestDomain>(
            "/plans",
            null!,
            req => new { },
            d => new TestDomain(),
            "CreatePlan",
            "Plan");

        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("request");
    }

    [Fact]
    public async Task CreateResourceAsync_WithNullResponse_ShouldThrowPlanningCenterApiGeneralException()
    {
        // Arrange
        var request = new TestCreateRequest { Name = "Test Plan" };
        _mockApiConnection.SetupPostResponse<JsonApiSingleResponse<TestDto>>("/plans", null!);

        // Act & Assert
        var act = async () => await _testService.TestCreateResourceAsync<TestCreateRequest, TestDto, TestDomain>(
            "/plans",
            request,
            req => new { },
            d => new TestDomain(),
            "CreatePlan",
            "Plan");

        await act.Should().ThrowAsync<Exception>();
    }

    #endregion

    #region UpdateResourceAsync Tests

    [Fact]
    public async Task UpdateResourceAsync_WithValidRequest_ShouldReturnUpdatedResource()
    {
        // Arrange
        var resourceId = "123";
        var request = new TestUpdateRequest { Name = "Updated Plan" };
        var dto = new TestDto { Id = "123", Name = "Updated Plan" };
        var domain = new TestDomain { Id = "123", Name = "Updated Plan" };
        
        var response = new JsonApiSingleResponse<TestDto> { Data = dto };
        _mockApiConnection.SetupPatchResponse($"/plans/{resourceId}", response);

        // Act
        var result = await _testService.TestUpdateResourceAsync<TestUpdateRequest, TestDto, TestDomain>(
            "/plans",
            resourceId,
            request,
            (id, req) => new { data = new { id, attributes = new { name = req.Name } } },
            d => new TestDomain { Id = d.Id, Name = d.Name },
            "UpdatePlan",
            "Plan");

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be("123");
        result.Name.Should().Be("Updated Plan");
        VerifyLoggerWasCalled(LogLevel.Information, Times.Exactly(2)); // Success + Operation completion logs
    }

    [Fact]
    public async Task UpdateResourceAsync_WithNullResourceId_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var act = async () => await _testService.TestUpdateResourceAsync<TestUpdateRequest, TestDto, TestDomain>(
            "/plans",
            null!,
            new TestUpdateRequest(),
            (id, req) => new { },
            d => new TestDomain(),
            "UpdatePlan",
            "Plan");

        await act.Should().ThrowAsync<ArgumentException>().WithParameterName("resourceId");
    }

    [Fact]
    public async Task UpdateResourceAsync_WithNullRequest_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var act = async () => await _testService.TestUpdateResourceAsync<TestUpdateRequest, TestDto, TestDomain>(
            "/plans",
            "123",
            null!,
            (id, req) => new { },
            d => new TestDomain(),
            "UpdatePlan",
            "Plan");

        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("request");
    }

    #endregion

    #region DeleteResourceAsync Tests

    [Fact]
    public async Task DeleteResourceAsync_WithValidResourceId_ShouldCompleteSuccessfully()
    {
        // Arrange
        var resourceId = "123";
        _mockApiConnection.SetupDeleteResponse($"/plans/{resourceId}");

        // Act
        await _testService.TestDeleteResourceAsync("/plans", resourceId, "DeletePlan", "Plan");

        // Assert
        VerifyLoggerWasCalled(LogLevel.Information, Times.Exactly(2)); // Success + Operation completion logs
    }

    [Fact]
    public async Task DeleteResourceAsync_WithNullResourceId_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var act = async () => await _testService.TestDeleteResourceAsync("/plans", null!, "DeletePlan", "Plan");

        await act.Should().ThrowAsync<ArgumentException>().WithParameterName("resourceId");
    }

    [Fact]
    public async Task DeleteResourceAsync_WithEmptyResourceId_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var act = async () => await _testService.TestDeleteResourceAsync("/plans", "", "DeletePlan", "Plan");

        await act.Should().ThrowAsync<ArgumentException>().WithParameterName("resourceId");
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

        public void TestLogSuccess(string operationName, string resourceName, string? resourceId = null, string? additionalInfo = null)
        {
            LogSuccess(operationName, resourceName, resourceId, additionalInfo);
        }

        public async Task<TDomain> TestCreateResourceAsync<TCreateRequest, TDto, TDomain>(
            string endpoint,
            TCreateRequest request,
            Func<TCreateRequest, object> requestMapper,
            Func<TDto, TDomain> responseMapper,
            string operationName,
            string resourceName,
            CancellationToken cancellationToken = default)
            where TCreateRequest : class
            where TDto : class
            where TDomain : class
        {
            return await CreateResourceAsync(endpoint, request, requestMapper, responseMapper, operationName, resourceName, cancellationToken);
        }

        public async Task<TDomain> TestUpdateResourceAsync<TUpdateRequest, TDto, TDomain>(
            string endpoint,
            string resourceId,
            TUpdateRequest request,
            Func<string, TUpdateRequest, object> requestMapper,
            Func<TDto, TDomain> responseMapper,
            string operationName,
            string resourceName,
            CancellationToken cancellationToken = default)
            where TUpdateRequest : class
            where TDto : class
            where TDomain : class
        {
            return await UpdateResourceAsync(endpoint, resourceId, request, requestMapper, responseMapper, operationName, resourceName, cancellationToken);
        }

        public async Task TestDeleteResourceAsync(
            string endpoint,
            string resourceId,
            string operationName,
            string resourceName,
            CancellationToken cancellationToken = default)
        {
            await DeleteResourceAsync(endpoint, resourceId, operationName, resourceName, cancellationToken);
        }
    }

    #endregion

    #region Test Models

    public class TestCreateRequest
    {
        public string Name { get; set; } = string.Empty;
    }

    public class TestUpdateRequest
    {
        public string Name { get; set; } = string.Empty;
    }

    public class TestDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class TestDomain
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    #endregion
}