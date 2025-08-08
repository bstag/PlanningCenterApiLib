using FluentAssertions;
using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Configuration;

/// <summary>
/// Unit tests for PlanningCenterOptions - configuration options for the SDK.
/// This tests the configuration validation and default values.
/// </summary>
public class PlanningCenterOptionsTests
{
    #region Constructor and Default Values Tests

    [Fact]
    public void Constructor_ShouldSetDefaultValues()
    {
        // Arrange & Act
        var options = new PlanningCenterOptions();

        // Assert
        options.BaseUrl.Should().Be("https://api.planningcenteronline.com");
        options.RequestTimeout.Should().Be(TimeSpan.FromSeconds(30));
        options.MaxRetryAttempts.Should().Be(3);
        options.RetryBaseDelay.Should().Be(TimeSpan.FromMilliseconds(500));
        options.EnableDetailedLogging.Should().BeFalse();
        options.EnableCaching.Should().BeTrue();
        options.DefaultCacheExpiration.Should().Be(TimeSpan.FromMinutes(5));
        options.MaxCacheSize.Should().Be(1000);
        options.UserAgent.Should().Be("PlanningCenter.Api.Client/1.0");
        options.TokenEndpoint.Should().Be("https://api.planningcenteronline.com/oauth/token");
    }

    #endregion

    #region Property Validation Tests

    [Fact]
    public void BaseUrl_WithValidUrl_ShouldSetCorrectly()
    {
        // Arrange
        var options = new PlanningCenterOptions();
        var validUrl = "https://custom.api.com";

        // Act
        options.BaseUrl = validUrl;

        // Assert
        options.BaseUrl.Should().Be(validUrl);
    }

    [Fact]
    public void BaseUrl_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var options = new PlanningCenterOptions();

        // Act & Assert
        var act = () => options.BaseUrl = null!;
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void BaseUrl_WithEmptyString_ShouldThrowArgumentException()
    {
        // Arrange
        var options = new PlanningCenterOptions();

        // Act & Assert
        var act = () => options.BaseUrl = "";
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void BaseUrl_WithInvalidUrl_ShouldThrowArgumentException()
    {
        // Arrange
        var options = new PlanningCenterOptions();

        // Act & Assert
        var act = () => options.BaseUrl = "not-a-valid-url";
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void RequestTimeout_WithValidTimespan_ShouldSetCorrectly()
    {
        // Arrange
        var options = new PlanningCenterOptions();
        var timeout = TimeSpan.FromMinutes(2);

        // Act
        options.RequestTimeout = timeout;

        // Assert
        options.RequestTimeout.Should().Be(timeout);
    }

    [Fact]
    public void RequestTimeout_WithZeroValue_ShouldThrowArgumentException()
    {
        // Arrange
        var options = new PlanningCenterOptions();

        // Act & Assert
        var act = () => options.RequestTimeout = TimeSpan.Zero;
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void RequestTimeout_WithNegativeValue_ShouldThrowArgumentException()
    {
        // Arrange
        var options = new PlanningCenterOptions();

        // Act & Assert
        var act = () => options.RequestTimeout = TimeSpan.FromSeconds(-1);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void MaxRetryAttempts_WithValidValue_ShouldSetCorrectly()
    {
        // Arrange
        var options = new PlanningCenterOptions();

        // Act
        options.MaxRetryAttempts = 5;

        // Assert
        options.MaxRetryAttempts.Should().Be(5);
    }

    [Fact]
    public void MaxRetryAttempts_WithZeroValue_ShouldSetCorrectly()
    {
        // Arrange
        var options = new PlanningCenterOptions();

        // Act
        options.MaxRetryAttempts = 0;

        // Assert
        options.MaxRetryAttempts.Should().Be(0);
    }

    [Fact]
    public void MaxRetryAttempts_WithNegativeValue_ShouldThrowArgumentException()
    {
        // Arrange
        var options = new PlanningCenterOptions();

        // Act & Assert
        var act = () => options.MaxRetryAttempts = -1;
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void RetryBaseDelay_WithValidTimespan_ShouldSetCorrectly()
    {
        // Arrange
        var options = new PlanningCenterOptions();
        var delay = TimeSpan.FromSeconds(2);

        // Act
        options.RetryBaseDelay = delay;

        // Assert
        options.RetryBaseDelay.Should().Be(delay);
    }

    [Fact]
    public void RetryBaseDelay_WithZeroValue_ShouldSetCorrectly()
    {
        // Arrange
        var options = new PlanningCenterOptions();

        // Act
        options.RetryBaseDelay = TimeSpan.Zero;

        // Assert
        options.RetryBaseDelay.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void RetryBaseDelay_WithNegativeValue_ShouldThrowArgumentException()
    {
        // Arrange
        var options = new PlanningCenterOptions();

        // Act & Assert
        var act = () => options.RetryBaseDelay = TimeSpan.FromMilliseconds(-1);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void DefaultCacheExpiration_WithValidTimespan_ShouldSetCorrectly()
    {
        // Arrange
        var options = new PlanningCenterOptions();
        var expiration = TimeSpan.FromMinutes(10);

        // Act
        options.DefaultCacheExpiration = expiration;

        // Assert
        options.DefaultCacheExpiration.Should().Be(expiration);
    }

    [Fact]
    public void DefaultCacheExpiration_WithZeroValue_ShouldSetCorrectly()
    {
        // Arrange
        var options = new PlanningCenterOptions();

        // Act
        options.DefaultCacheExpiration = TimeSpan.Zero;

        // Assert
        options.DefaultCacheExpiration.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void DefaultCacheExpiration_WithNegativeValue_ShouldThrowArgumentException()
    {
        // Arrange
        var options = new PlanningCenterOptions();

        // Act & Assert
        var act = () => options.DefaultCacheExpiration = TimeSpan.FromMinutes(-1);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void MaxCacheSize_WithValidValue_ShouldSetCorrectly()
    {
        // Arrange
        var options = new PlanningCenterOptions();

        // Act
        options.MaxCacheSize = 2000;

        // Assert
        options.MaxCacheSize.Should().Be(2000);
    }

    [Fact]
    public void MaxCacheSize_WithZeroValue_ShouldSetCorrectly()
    {
        // Arrange
        var options = new PlanningCenterOptions();

        // Act
        options.MaxCacheSize = 0;

        // Assert
        options.MaxCacheSize.Should().Be(0);
    }

    [Fact]
    public void MaxCacheSize_WithNegativeValue_ShouldThrowArgumentException()
    {
        // Arrange
        var options = new PlanningCenterOptions();

        // Act & Assert
        var act = () => options.MaxCacheSize = -1;
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UserAgent_WithValidValue_ShouldSetCorrectly()
    {
        // Arrange
        var options = new PlanningCenterOptions();
        var userAgent = "CustomApp/2.0";

        // Act
        options.UserAgent = userAgent;

        // Assert
        options.UserAgent.Should().Be(userAgent);
    }

    [Fact]
    public void UserAgent_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var options = new PlanningCenterOptions();

        // Act & Assert
        var act = () => options.UserAgent = null!;
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void UserAgent_WithEmptyString_ShouldThrowArgumentException()
    {
        // Arrange
        var options = new PlanningCenterOptions();

        // Act & Assert
        var act = () => options.UserAgent = "";
        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region Authentication Properties Tests

    [Fact]
    public void ClientId_WithValidValue_ShouldSetCorrectly()
    {
        // Arrange
        var options = new PlanningCenterOptions();
        var clientId = "test-client-id";

        // Act
        options.ClientId = clientId;

        // Assert
        options.ClientId.Should().Be(clientId);
    }

    [Fact]
    public void ClientSecret_WithValidValue_ShouldSetCorrectly()
    {
        // Arrange
        var options = new PlanningCenterOptions();
        var clientSecret = "test-client-secret";

        // Act
        options.ClientSecret = clientSecret;

        // Assert
        options.ClientSecret.Should().Be(clientSecret);
    }

    [Fact]
    public void PersonalAccessToken_WithValidValue_ShouldSetCorrectly()
    {
        // Arrange
        var options = new PlanningCenterOptions();
        var pat = "app-id:secret";

        // Act
        options.PersonalAccessToken = pat;

        // Assert
        options.PersonalAccessToken.Should().Be(pat);
    }

    [Fact]
    public void AccessToken_WithValidValue_ShouldSetCorrectly()
    {
        // Arrange
        var options = new PlanningCenterOptions();
        var accessToken = "bearer-token-123";

        // Act
        options.AccessToken = accessToken;

        // Assert
        options.AccessToken.Should().Be(accessToken);
    }

    #endregion

    #region Validation Method Tests

    [Fact]
    public void Validate_WithValidOAuthConfiguration_ShouldNotThrow()
    {
        // Arrange
        var options = new PlanningCenterOptions
        {
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret"
        };

        // Act & Assert
        var act = () => options.Validate();
        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_WithValidPATConfiguration_ShouldNotThrow()
    {
        // Arrange
        var options = new PlanningCenterOptions
        {
            PersonalAccessToken = "app-id:secret"
        };

        // Act & Assert
        var act = () => options.Validate();
        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_WithValidAccessTokenConfiguration_ShouldNotThrow()
    {
        // Arrange
        var options = new PlanningCenterOptions
        {
            AccessToken = "bearer-token-123"
        };

        // Act & Assert
        var act = () => options.Validate();
        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_WithNoAuthenticationConfiguration_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var options = new PlanningCenterOptions();

        // Act & Assert
        var act = () => options.Validate();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*authentication*");
    }

    [Fact]
    public void Validate_WithIncompleteOAuthConfiguration_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var options = new PlanningCenterOptions
        {
            ClientId = "test-client-id"
            // Missing ClientSecret
        };

        // Act & Assert
        var act = () => options.Validate();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*ClientSecret*");
    }

    [Fact]
    public void Validate_WithMultipleAuthenticationMethods_ShouldUseFirstAvailable()
    {
        // Arrange
        var options = new PlanningCenterOptions
        {
            PersonalAccessToken = "app-id:secret",
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret"
        };

        // Act & Assert
        var act = () => options.Validate();
        act.Should().NotThrow(); // Should use PAT and not require OAuth completion
    }

    #endregion

    #region IsValid Property Tests

    [Fact]
    public void IsValid_WithValidConfiguration_ShouldReturnTrue()
    {
        // Arrange
        var options = new PlanningCenterOptions
        {
            PersonalAccessToken = "app-id:secret"
        };

        // Act & Assert
        options.IsValid().Should().BeTrue();
    }

    [Fact]
    public void IsValid_WithInvalidConfiguration_ShouldReturnFalse()
    {
        // Arrange
        var options = new PlanningCenterOptions();

        // Act & Assert
        options.IsValid().Should().BeFalse();
    }

    #endregion

    #region HasAuthentication Method Tests

    [Fact]
    public void HasAuthentication_WithPAT_ShouldReturnTrue()
    {
        // Arrange
        var options = new PlanningCenterOptions
        {
            PersonalAccessToken = "app-id:secret"
        };

        // Act & Assert
        options.HasAuthentication().Should().BeTrue();
    }

    [Fact]
    public void HasAuthentication_WithOAuth_ShouldReturnTrue()
    {
        // Arrange
        var options = new PlanningCenterOptions
        {
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret"
        };

        // Act & Assert
        options.HasAuthentication().Should().BeTrue();
    }

    [Fact]
    public void HasAuthentication_WithAccessToken_ShouldReturnTrue()
    {
        // Arrange
        var options = new PlanningCenterOptions
        {
            AccessToken = "bearer-token-123"
        };

        // Act & Assert
        options.HasAuthentication().Should().BeTrue();
    }

    [Fact]
    public void HasAuthentication_WithNoAuth_ShouldReturnFalse()
    {
        // Arrange
        var options = new PlanningCenterOptions();

        // Act & Assert
        options.HasAuthentication().Should().BeFalse();
    }

    #endregion

    #region Edge Cases Tests

    [Fact]
    public void BaseUrl_WithTrailingSlash_ShouldRemoveTrailingSlash()
    {
        // Arrange
        var options = new PlanningCenterOptions();

        // Act
        options.BaseUrl = "https://api.example.com/";

        // Assert
        options.BaseUrl.Should().Be("https://api.example.com");
    }

    [Fact]
    public void BaseUrl_WithMultipleTrailingSlashes_ShouldRemoveAllTrailingSlashes()
    {
        // Arrange
        var options = new PlanningCenterOptions();

        // Act
        options.BaseUrl = "https://api.example.com///";

        // Assert
        options.BaseUrl.Should().Be("https://api.example.com");
    }

    [Fact]
    public void RequestTimeout_WithVeryLargeValue_ShouldSetCorrectly()
    {
        // Arrange
        var options = new PlanningCenterOptions();
        var largeTimeout = TimeSpan.FromHours(24);

        // Act
        options.RequestTimeout = largeTimeout;

        // Assert
        options.RequestTimeout.Should().Be(largeTimeout);
    }

    [Fact]
    public void MaxRetryAttempts_WithVeryLargeValue_ShouldSetCorrectly()
    {
        // Arrange
        var options = new PlanningCenterOptions();

        // Act
        options.MaxRetryAttempts = 100;

        // Assert
        options.MaxRetryAttempts.Should().Be(100);
    }

    #endregion

    #region ToString Tests

    [Fact]
    public void ToString_ShouldReturnReadableRepresentation()
    {
        // Arrange
        var options = new PlanningCenterOptions
        {
            BaseUrl = "https://api.example.com",
            EnableCaching = true,
            MaxRetryAttempts = 5
        };

        // Act
        var result = options.ToString();

        // Assert
        result.Should().Contain("BaseUrl");
        result.Should().Contain("https://api.example.com");
        result.Should().Contain("EnableCaching");
        result.Should().Contain("True");
        result.Should().Contain("MaxRetryAttempts");
        result.Should().Contain("5");
    }

    [Fact]
    public void ToString_ShouldNotExposeSecrets()
    {
        // Arrange
        var options = new PlanningCenterOptions
        {
            PersonalAccessToken = "secret-app-id:secret-token",
            ClientSecret = "secret-client-secret",
            AccessToken = "secret-access-token"
        };

        // Act
        var result = options.ToString();

        // Assert
        result.Should().NotContain("secret-app-id");
        result.Should().NotContain("secret-token");
        result.Should().NotContain("secret-client-secret");
        result.Should().NotContain("secret-access-token");
        result.Should().Contain("***"); // Should show masked values
    }

    #endregion
}
