using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Exceptions;
using Xunit;

namespace PlanningCenter.Api.Client.Tests;

/// <summary>
/// Unit tests for ApiConnection - the core HTTP client component.
/// This is critical infrastructure that needs comprehensive test coverage.
/// </summary>
public class ApiConnectionTests : IDisposable
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly Mock<IAuthenticator> _mockAuthenticator;
    private readonly PlanningCenterOptions _options;
    private readonly ApiConnection _apiConnection;

    public ApiConnectionTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _mockAuthenticator = new Mock<IAuthenticator>();
        
        _options = new PlanningCenterOptions
        {
            BaseUrl = "https://api.planningcenteronline.com",
            RequestTimeout = TimeSpan.FromSeconds(30),
            MaxRetryAttempts = 3,
            RetryBaseDelay = TimeSpan.FromMilliseconds(100),
            EnableDetailedLogging = false,
            PersonalAccessToken = "test-token"
        };

        var optionsWrapper = Options.Create(_options);
        _apiConnection = new ApiConnection(_httpClient, optionsWrapper, _mockAuthenticator.Object, NullLogger<ApiConnection>.Instance);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidParameters_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var optionsWrapper = Options.Create(_options);
        var connection = new ApiConnection(_httpClient, optionsWrapper, _mockAuthenticator.Object, NullLogger<ApiConnection>.Instance);

        // Assert
        connection.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullHttpClient_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var optionsWrapper = Options.Create(_options);
        var act = () => new ApiConnection(null!, optionsWrapper, _mockAuthenticator.Object, NullLogger<ApiConnection>.Instance);
        act.Should().Throw<ArgumentNullException>().WithParameterName("httpClient");
    }

    [Fact]
    public void Constructor_WithNullOptions_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var act = () => new ApiConnection(_httpClient, null!, _mockAuthenticator.Object, NullLogger<ApiConnection>.Instance);
        act.Should().Throw<ArgumentNullException>().WithParameterName("options");
    }

    [Fact]
    public void Constructor_WithNullAuthenticator_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var optionsWrapper = Options.Create(_options);
        var act = () => new ApiConnection(_httpClient, optionsWrapper, null!, NullLogger<ApiConnection>.Instance);
        act.Should().Throw<ArgumentNullException>().WithParameterName("authenticator");
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var optionsWrapper = Options.Create(_options);
        var act = () => new ApiConnection(_httpClient, optionsWrapper, _mockAuthenticator.Object, null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("logger");
    }

    #endregion

    #region GetAsync Tests

    [Fact]
    public async Task GetAsync_WithValidResponse_ShouldReturnDeserializedObject()
    {
        // Arrange
        var expectedData = new { Id = "123", Name = "Test" };
        var jsonResponse = JsonSerializer.Serialize(expectedData);
        
        SetupHttpResponse(HttpStatusCode.OK, jsonResponse);
        SetupAuthenticator("Bearer token123");

        // Act
        var result = await _apiConnection.GetAsync<dynamic>("/test/endpoint");

        // Assert
        result.Should().NotBeNull();
        VerifyHttpRequest(HttpMethod.Get, "/test/endpoint");
    }

    [Fact]
    public async Task GetAsync_WithNotFoundResponse_ShouldThrowNotFoundException()
    {
        // Arrange
        SetupHttpResponse(HttpStatusCode.NotFound, "Not found");
        SetupAuthenticator("Bearer token123");

        // Act & Assert
        var act = async () => await _apiConnection.GetAsync<dynamic>("/test/endpoint");
        await act.Should().ThrowAsync<PlanningCenterApiNotFoundException>();
    }

    [Fact]
    public async Task GetAsync_WithUnauthorizedResponse_ShouldThrowAuthenticationException()
    {
        // Arrange
        SetupHttpResponse(HttpStatusCode.Unauthorized, "Unauthorized");
        SetupAuthenticator("Bearer token123");

        // Act & Assert
        var act = async () => await _apiConnection.GetAsync<dynamic>("/test/endpoint");
        await act.Should().ThrowAsync<PlanningCenterApiAuthenticationException>();
    }

    [Fact]
    public async Task GetAsync_WithForbiddenResponse_ShouldThrowAuthorizationException()
    {
        // Arrange
        SetupHttpResponse(HttpStatusCode.Forbidden, "Forbidden");
        SetupAuthenticator("Bearer token123");

        // Act & Assert
        var act = async () => await _apiConnection.GetAsync<dynamic>("/test/endpoint");
        await act.Should().ThrowAsync<PlanningCenterApiAuthorizationException>();
    }

    [Fact]
    public async Task GetAsync_WithTooManyRequestsResponse_ShouldThrowRateLimitException()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests)
        {
            Content = new StringContent("Rate limited"),
            Headers = { { "Retry-After", "60" } }
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        SetupAuthenticator("Bearer token123");

        // Act & Assert
        var act = async () => await _apiConnection.GetAsync<dynamic>("/test/endpoint");
        await act.Should().ThrowAsync<PlanningCenterApiRateLimitException>();
    }

    [Fact]
    public async Task GetAsync_WithServerErrorResponse_ShouldThrowServerException()
    {
        // Arrange
        SetupHttpResponse(HttpStatusCode.InternalServerError, "Server error");
        SetupAuthenticator("Bearer token123");

        // Act & Assert
        var act = async () => await _apiConnection.GetAsync<dynamic>("/test/endpoint");
        await act.Should().ThrowAsync<PlanningCenterApiServerException>();
    }

    [Fact]
    public async Task GetAsync_WithInvalidJson_ShouldThrowGeneralException()
    {
        // Arrange
        SetupHttpResponse(HttpStatusCode.OK, "invalid json {");
        SetupAuthenticator("Bearer token123");

        // Act & Assert
        var act = async () => await _apiConnection.GetAsync<dynamic>("/test/endpoint");
        await act.Should().ThrowAsync<PlanningCenterApiGeneralException>();
    }

    #endregion

    #region PostAsync Tests

    [Fact]
    public async Task PostAsync_WithValidData_ShouldReturnDeserializedObject()
    {
        // Arrange
        var requestData = new { Name = "Test", Value = 123 };
        var responseData = new { Id = "456", Name = "Test" };
        var jsonResponse = JsonSerializer.Serialize(responseData);
        
        SetupHttpResponse(HttpStatusCode.Created, jsonResponse);
        SetupAuthenticator("Bearer token123");

        // Act
        var result = await _apiConnection.PostAsync<dynamic>("/test/endpoint", requestData);

        // Assert
        result.Should().NotBeNull();
        VerifyHttpRequest(HttpMethod.Post, "/test/endpoint");
    }

    [Fact]
    public async Task PostAsync_WithValidationError_ShouldThrowValidationException()
    {
        // Arrange
        var requestData = new { Name = "" };
        SetupHttpResponse(HttpStatusCode.BadRequest, "Validation failed");
        SetupAuthenticator("Bearer token123");

        // Act & Assert
        var act = async () => await _apiConnection.PostAsync<dynamic>("/test/endpoint", requestData);
        await act.Should().ThrowAsync<PlanningCenterApiValidationException>();
    }

    #endregion

    #region PutAsync Tests

    [Fact]
    public async Task PutAsync_WithValidData_ShouldReturnDeserializedObject()
    {
        // Arrange
        var requestData = new { Name = "Updated Test" };
        var responseData = new { Id = "456", Name = "Updated Test" };
        var jsonResponse = JsonSerializer.Serialize(responseData);
        
        SetupHttpResponse(HttpStatusCode.OK, jsonResponse);
        SetupAuthenticator("Bearer token123");

        // Act
        var result = await _apiConnection.PutAsync<dynamic>("/test/endpoint/456", requestData);

        // Assert
        result.Should().NotBeNull();
        VerifyHttpRequest(HttpMethod.Put, "/test/endpoint/456");
    }

    #endregion

    #region PatchAsync Tests

    [Fact]
    public async Task PatchAsync_WithValidData_ShouldReturnDeserializedObject()
    {
        // Arrange
        var requestData = new { Name = "Patched Test" };
        var responseData = new { Id = "456", Name = "Patched Test" };
        var jsonResponse = JsonSerializer.Serialize(responseData);
        
        SetupHttpResponse(HttpStatusCode.OK, jsonResponse);
        SetupAuthenticator("Bearer token123");

        // Act
        var result = await _apiConnection.PatchAsync<dynamic>("/test/endpoint/456", requestData);

        // Assert
        result.Should().NotBeNull();
        VerifyHttpRequest(HttpMethod.Patch, "/test/endpoint/456");
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WithValidEndpoint_ShouldCompleteSuccessfully()
    {
        // Arrange
        SetupHttpResponse(HttpStatusCode.NoContent, "");
        SetupAuthenticator("Bearer token123");

        // Act
        var act = async () => await _apiConnection.DeleteAsync("/test/endpoint/456");

        // Assert
        await act.Should().NotThrowAsync();
        VerifyHttpRequest(HttpMethod.Delete, "/test/endpoint/456");
    }

    #endregion

    #region GetPagedAsync Tests

    [Fact]
    public async Task GetPagedAsync_WithValidResponse_ShouldReturnPagedResponse()
    {
        // Arrange
        var responseData = new
        {
            data = new[] { new { Id = "1", Name = "Item 1" }, new { Id = "2", Name = "Item 2" } },
            meta = new { total_count = 2, count = 2, per_page = 25, offset = 0 },
            links = new { }
        };
        var jsonResponse = JsonSerializer.Serialize(responseData);
        
        SetupHttpResponse(HttpStatusCode.OK, jsonResponse);
        SetupAuthenticator("Bearer token123");

        var parameters = new QueryParameters { PerPage = 25 };

        // Act
        var result = await _apiConnection.GetPagedAsync<dynamic>("/test/endpoint", parameters);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Meta.Should().NotBeNull();
        result.Links.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPagedAsync_WithQueryParameters_ShouldIncludeParametersInUrl()
    {
        // Arrange
        var responseData = new
        {
            data = new object[] { },
            meta = new { total_count = 0, count = 0, per_page = 10, offset = 0 },
            links = new { }
        };
        var jsonResponse = JsonSerializer.Serialize(responseData);
        
        SetupHttpResponse(HttpStatusCode.OK, jsonResponse);
        SetupAuthenticator("Bearer token123");

        var parameters = new QueryParameters 
        { 
            PerPage = 10,
            Offset = 20,
            Where = new Dictionary<string, object> { ["status"] = "active" }
        };

        // Act
        await _apiConnection.GetPagedAsync<dynamic>("/test/endpoint", parameters);

        // Assert
        _mockHttpMessageHandler.Protected()
            .Verify("SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.RequestUri!.ToString().Contains("per_page=10") &&
                    req.RequestUri.ToString().Contains("offset=20") &&
                    req.RequestUri.ToString().Contains("where")),
                ItExpr.IsAny<CancellationToken>());
    }

    #endregion

    #region Authentication Tests

    [Fact]
    public async Task GetAsync_WithBasicAuthToken_ShouldUseBasicAuthHeader()
    {
        // Arrange
        SetupHttpResponse(HttpStatusCode.OK, "{}");
        SetupAuthenticator("Basic dGVzdDp0ZXN0"); // Basic auth format

        // Act
        await _apiConnection.GetAsync<dynamic>("/test/endpoint");

        // Assert
        _mockHttpMessageHandler.Protected()
            .Verify("SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Headers.Contains("Authorization") &&
                    req.Headers.GetValues("Authorization").First() == "Basic dGVzdDp0ZXN0"),
                ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetAsync_WithBearerToken_ShouldUseBearerAuthHeader()
    {
        // Arrange
        SetupHttpResponse(HttpStatusCode.OK, "{}");
        SetupAuthenticator("token123"); // Bearer token format

        // Act
        await _apiConnection.GetAsync<dynamic>("/test/endpoint");

        // Assert
        _mockHttpMessageHandler.Protected()
            .Verify("SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Headers.Authorization != null &&
                    req.Headers.Authorization.Scheme == "Bearer" &&
                    req.Headers.Authorization.Parameter == "token123"),
                ItExpr.IsAny<CancellationToken>());
    }

    #endregion

    #region Retry Logic Tests

    [Fact]
    public async Task GetAsync_WithTransientFailure_ShouldRetryAndSucceed()
    {
        // Arrange
        var callCount = 0;
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(() =>
            {
                callCount++;
                if (callCount == 1)
                {
                    // First call fails with server error
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent("Server error")
                    });
                }
                // Second call succeeds
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{}")
                });
            });

        SetupAuthenticator("Bearer token123");

        // Act
        var result = await _apiConnection.GetAsync<dynamic>("/test/endpoint");

        // Assert
        result.Should().NotBeNull();
        callCount.Should().Be(2); // Should have retried once
    }

    [Fact]
    public async Task GetAsync_WithNonRetriableError_ShouldNotRetry()
    {
        // Arrange
        var callCount = 0;
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(() =>
            {
                callCount++;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Not found")
                });
            });

        SetupAuthenticator("Bearer token123");

        // Act & Assert
        var act = async () => await _apiConnection.GetAsync<dynamic>("/test/endpoint");
        await act.Should().ThrowAsync<PlanningCenterApiNotFoundException>();
        callCount.Should().Be(1); // Should not have retried
    }

    #endregion

    #region Helper Methods

    private void SetupHttpResponse(HttpStatusCode statusCode, string content)
    {
        var response = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(content, Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
    }

    private void SetupAuthenticator(string token)
    {
        _mockAuthenticator.Setup(a => a.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);
    }

    private void VerifyHttpRequest(HttpMethod method, string endpoint)
    {
        _mockHttpMessageHandler.Protected()
            .Verify("SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => 
                    req.Method == method && 
                    req.RequestUri!.ToString().Contains(endpoint)),
                ItExpr.IsAny<CancellationToken>());
    }

    #endregion

    #region Dispose

    public void Dispose()
    {
        _apiConnection?.Dispose();
        _httpClient?.Dispose();
    }

    #endregion
}