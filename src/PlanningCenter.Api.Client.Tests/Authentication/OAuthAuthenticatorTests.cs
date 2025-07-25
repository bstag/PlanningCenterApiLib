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

namespace PlanningCenter.Api.Client.Tests.Authentication;

/// <summary>
/// Unit tests for OAuthAuthenticator - OAuth authentication implementation.
/// This is critical for OAuth-based authentication flows.
/// </summary>
public class OAuthAuthenticatorTests : IDisposable
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly PlanningCenterOptions _options;
    private readonly OAuthAuthenticator _authenticator;

    public OAuthAuthenticatorTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        
        _options = new PlanningCenterOptions
        {
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret",
            BaseUrl = "https://api.planningcenteronline.com",
            TokenEndpoint = "https://api.planningcenteronline.com/oauth/token"
        };

        var optionsWrapper = Options.Create(_options);
        _authenticator = new OAuthAuthenticator(_httpClient, optionsWrapper, NullLogger<OAuthAuthenticator>.Instance);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidParameters_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var optionsWrapper = Options.Create(_options);
        var authenticator = new OAuthAuthenticator(_httpClient, optionsWrapper, NullLogger<OAuthAuthenticator>.Instance);

        // Assert
        authenticator.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullHttpClient_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var optionsWrapper = Options.Create(_options);
        var act = () => new OAuthAuthenticator(null!, optionsWrapper, NullLogger<OAuthAuthenticator>.Instance);
        act.Should().Throw<ArgumentNullException>().WithParameterName("httpClient");
    }

    [Fact]
    public void Constructor_WithNullOptions_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var act = () => new OAuthAuthenticator(_httpClient, null!, NullLogger<OAuthAuthenticator>.Instance);
        act.Should().Throw<ArgumentNullException>().WithParameterName("options");
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var optionsWrapper = Options.Create(_options);
        var act = () => new OAuthAuthenticator(_httpClient, optionsWrapper, null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("logger");
    }

    [Fact]
    public void Constructor_WithMissingClientId_ShouldThrowArgumentException()
    {
        // Arrange
        var invalidOptions = new PlanningCenterOptions
        {
            ClientId = "",
            ClientSecret = "test-secret"
        };
        var optionsWrapper = Options.Create(invalidOptions);

        // Act & Assert
        var act = () => new OAuthAuthenticator(_httpClient, optionsWrapper, NullLogger<OAuthAuthenticator>.Instance);
        act.Should().Throw<ArgumentException>().WithMessage("*ClientId*");
    }

    [Fact]
    public void Constructor_WithMissingClientSecret_ShouldThrowArgumentException()
    {
        // Arrange
        var invalidOptions = new PlanningCenterOptions
        {
            ClientId = "test-id",
            ClientSecret = ""
        };
        var optionsWrapper = Options.Create(invalidOptions);

        // Act & Assert
        var act = () => new OAuthAuthenticator(_httpClient, optionsWrapper, NullLogger<OAuthAuthenticator>.Instance);
        act.Should().Throw<ArgumentException>().WithMessage("*ClientSecret*");
    }

    #endregion

    #region GetAccessTokenAsync Tests

    [Fact]
    public async Task GetAccessTokenAsync_WithValidCredentials_ShouldReturnAccessToken()
    {
        // Arrange
        var tokenResponse = new
        {
            access_token = "test-access-token",
            token_type = "Bearer",
            expires_in = 3600,
            scope = "people:read people:write"
        };

        SetupTokenResponse(HttpStatusCode.OK, tokenResponse);

        // Act
        var result = await _authenticator.GetAccessTokenAsync();

        // Assert
        result.Should().Be("test-access-token");
        VerifyTokenRequest();
    }

    [Fact]
    public async Task GetAccessTokenAsync_WithCachedToken_ShouldReturnCachedToken()
    {
        // Arrange
        var tokenResponse = new
        {
            access_token = "cached-token",
            token_type = "Bearer",
            expires_in = 3600,
            scope = "people:read"
        };

        SetupTokenResponse(HttpStatusCode.OK, tokenResponse);

        // Act - First call
        var firstResult = await _authenticator.GetAccessTokenAsync();
        
        // Act - Second call (should use cache)
        var secondResult = await _authenticator.GetAccessTokenAsync();

        // Assert
        firstResult.Should().Be("cached-token");
        secondResult.Should().Be("cached-token");
        
        // Should only make one HTTP request due to caching
        _mockHttpMessageHandler.Protected()
            .Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetAccessTokenAsync_WithExpiredToken_ShouldRefreshToken()
    {
        // Arrange
        var firstTokenResponse = new
        {
            access_token = "first-token",
            token_type = "Bearer",
            expires_in = 1, // Expires in 1 second
            scope = "people:read"
        };

        var secondTokenResponse = new
        {
            access_token = "refreshed-token",
            token_type = "Bearer",
            expires_in = 3600,
            scope = "people:read"
        };

        var callCount = 0;
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(() =>
            {
                callCount++;
                var response = callCount == 1 ? firstTokenResponse : secondTokenResponse;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(response), Encoding.UTF8, "application/json")
                });
            });

        // Act
        var firstResult = await _authenticator.GetAccessTokenAsync();
        
        // Wait for token to expire
        await Task.Delay(1100);
        
        var secondResult = await _authenticator.GetAccessTokenAsync();

        // Assert
        firstResult.Should().Be("first-token");
        secondResult.Should().Be("refreshed-token");
        callCount.Should().Be(2);
    }

    [Fact]
    public async Task GetAccessTokenAsync_WithInvalidCredentials_ShouldThrowAuthenticationException()
    {
        // Arrange
        var errorResponse = new
        {
            error = "invalid_client",
            error_description = "Invalid client credentials"
        };

        SetupTokenResponse(HttpStatusCode.Unauthorized, errorResponse);

        // Act & Assert
        var act = async () => await _authenticator.GetAccessTokenAsync();
        await act.Should().ThrowAsync<PlanningCenterApiAuthenticationException>()
            .WithMessage("*Invalid client credentials*");
    }

    [Fact]
    public async Task GetAccessTokenAsync_WithServerError_ShouldThrowServerException()
    {
        // Arrange
        SetupHttpResponse(HttpStatusCode.InternalServerError, "Server error");

        // Act & Assert
        var act = async () => await _authenticator.GetAccessTokenAsync();
        await act.Should().ThrowAsync<PlanningCenterApiServerException>();
    }

    [Fact]
    public async Task GetAccessTokenAsync_WithInvalidJsonResponse_ShouldThrowGeneralException()
    {
        // Arrange
        SetupHttpResponse(HttpStatusCode.OK, "invalid json {");

        // Act & Assert
        var act = async () => await _authenticator.GetAccessTokenAsync();
        await act.Should().ThrowAsync<PlanningCenterApiGeneralException>();
    }

    [Fact]
    public async Task GetAccessTokenAsync_WithMissingAccessToken_ShouldThrowAuthenticationException()
    {
        // Arrange
        var invalidResponse = new
        {
            token_type = "Bearer",
            expires_in = 3600
            // Missing access_token
        };

        SetupTokenResponse(HttpStatusCode.OK, invalidResponse);

        // Act & Assert
        var act = async () => await _authenticator.GetAccessTokenAsync();
        await act.Should().ThrowAsync<PlanningCenterApiAuthenticationException>()
            .WithMessage("*access_token*");
    }

    #endregion

    #region GetAuthorizationHeaderAsync Tests

    [Fact]
    public async Task GetAuthorizationHeaderAsync_WithValidToken_ShouldReturnBearerHeader()
    {
        // Arrange
        var tokenResponse = new
        {
            access_token = "test-token",
            token_type = "Bearer",
            expires_in = 3600
        };

        SetupTokenResponse(HttpStatusCode.OK, tokenResponse);

        // Act
        var result = await _authenticator.GetAuthorizationHeaderAsync();

        // Assert
        result.Should().Be("Bearer test-token");
    }

    [Fact]
    public async Task GetAuthorizationHeaderAsync_WithCustomTokenType_ShouldUseCustomTokenType()
    {
        // Arrange
        var tokenResponse = new
        {
            access_token = "test-token",
            token_type = "Custom",
            expires_in = 3600
        };

        SetupTokenResponse(HttpStatusCode.OK, tokenResponse);

        // Act
        var result = await _authenticator.GetAuthorizationHeaderAsync();

        // Assert
        result.Should().Be("Custom test-token");
    }

    #endregion

    #region Cancellation Tests

    [Fact]
    public async Task GetAccessTokenAsync_WithCancellation_ShouldRespectCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        var act = async () => await _authenticator.GetAccessTokenAsync(cts.Token);
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    #endregion

    #region Helper Methods

    private void SetupTokenResponse<T>(HttpStatusCode statusCode, T responseObject)
    {
        var jsonResponse = JsonSerializer.Serialize(responseObject);
        SetupHttpResponse(statusCode, jsonResponse);
    }

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

    private void VerifyTokenRequest()
    {
        _mockHttpMessageHandler.Protected()
            .Verify("SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri!.ToString().Contains("/oauth/token") &&
                    req.Content != null),
                ItExpr.IsAny<CancellationToken>());
    }

    #endregion

    #region Dispose

    public void Dispose()
    {
        _authenticator?.Dispose();
        _httpClient?.Dispose();
        GC.SuppressFinalize(this);
    }

    #endregion
}