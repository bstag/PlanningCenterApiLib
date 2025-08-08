using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;
using Xunit;

namespace PlanningCenter.Api.Client.Tests.Authentication;

/// <summary>
/// Unit tests for PersonalAccessTokenAuthenticator - PAT authentication implementation.
/// This is critical for Personal Access Token authentication flows.
/// </summary>
public class PersonalAccessTokenAuthenticatorTests
{
    private readonly PlanningCenterOptions _options;

    public PersonalAccessTokenAuthenticatorTests()
    {
        _options = new PlanningCenterOptions
        {
            PersonalAccessToken = "test-app-id:test-secret"
        };
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidParameters_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var optionsWrapper = Options.Create(_options);
        var authenticator = new PersonalAccessTokenAuthenticator(optionsWrapper, NullLogger<PersonalAccessTokenAuthenticator>.Instance);

        // Assert
        authenticator.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullOptions_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var act = () => new PersonalAccessTokenAuthenticator(null!, NullLogger<PersonalAccessTokenAuthenticator>.Instance);
        act.Should().Throw<ArgumentNullException>().WithParameterName("options");
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var optionsWrapper = Options.Create(_options);
        var act = () => new PersonalAccessTokenAuthenticator(optionsWrapper, null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("logger");
    }

    [Fact]
    public void Constructor_WithMissingPersonalAccessToken_ShouldThrowArgumentException()
    {
        // Arrange
        var invalidOptions = new PlanningCenterOptions
        {
            PersonalAccessToken = ""
        };
        var optionsWrapper = Options.Create(invalidOptions);

        // Act & Assert
        var act = () => new PersonalAccessTokenAuthenticator(optionsWrapper, NullLogger<PersonalAccessTokenAuthenticator>.Instance);
        act.Should().Throw<ArgumentException>().WithMessage("*PersonalAccessToken*");
    }

    [Fact]
    public void Constructor_WithInvalidPersonalAccessTokenFormat_ShouldThrowArgumentException()
    {
        // Arrange
        var invalidOptions = new PlanningCenterOptions
        {
            PersonalAccessToken = "invalid-format" // Should be "app-id:secret"
        };
        var optionsWrapper = Options.Create(invalidOptions);

        // Act & Assert
        var act = () => new PersonalAccessTokenAuthenticator(optionsWrapper, NullLogger<PersonalAccessTokenAuthenticator>.Instance);
        act.Should().Throw<ArgumentException>().WithMessage("*format*app-id:secret*");
    }

    [Fact]
    public void Constructor_WithEmptyAppId_ShouldThrowArgumentException()
    {
        // Arrange
        var invalidOptions = new PlanningCenterOptions
        {
            PersonalAccessToken = ":test-secret" // Empty app ID
        };
        var optionsWrapper = Options.Create(invalidOptions);

        // Act & Assert
        var act = () => new PersonalAccessTokenAuthenticator(optionsWrapper, NullLogger<PersonalAccessTokenAuthenticator>.Instance);
        act.Should().Throw<ArgumentException>().WithMessage("*app-id*");
    }

    [Fact]
    public void Constructor_WithEmptySecret_ShouldThrowArgumentException()
    {
        // Arrange
        var invalidOptions = new PlanningCenterOptions
        {
            PersonalAccessToken = "test-app-id:" // Empty secret
        };
        var optionsWrapper = Options.Create(invalidOptions);

        // Act & Assert
        var act = () => new PersonalAccessTokenAuthenticator(optionsWrapper, NullLogger<PersonalAccessTokenAuthenticator>.Instance);
        act.Should().Throw<ArgumentException>().WithMessage("*secret*");
    }

    #endregion

    #region GetAccessTokenAsync Tests

    [Fact]
    public async Task GetAccessTokenAsync_WithValidPAT_ShouldReturnBase64EncodedToken()
    {
        // Arrange
        var optionsWrapper = Options.Create(_options);
        var authenticator = new PersonalAccessTokenAuthenticator(optionsWrapper, NullLogger<PersonalAccessTokenAuthenticator>.Instance);

        // Act
        var result = await authenticator.GetAccessTokenAsync();

        // Assert
        result.Should().NotBeNullOrEmpty();
        
        // Verify it's a valid Base64 string
        var act = () => Convert.FromBase64String(result);
        act.Should().NotThrow();
        
        // Verify the decoded content
        var decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(result));
        decoded.Should().Be("test-app-id:test-secret");
    }

    [Fact]
    public async Task GetAccessTokenAsync_CalledMultipleTimes_ShouldReturnSameToken()
    {
        // Arrange
        var optionsWrapper = Options.Create(_options);
        var authenticator = new PersonalAccessTokenAuthenticator(optionsWrapper, NullLogger<PersonalAccessTokenAuthenticator>.Instance);

        // Act
        var firstResult = await authenticator.GetAccessTokenAsync();
        var secondResult = await authenticator.GetAccessTokenAsync();

        // Assert
        firstResult.Should().Be(secondResult);
    }

    [Fact]
    public async Task GetAccessTokenAsync_WithCancellation_ShouldRespectCancellationToken()
    {
        // Arrange
        var optionsWrapper = Options.Create(_options);
        var authenticator = new PersonalAccessTokenAuthenticator(optionsWrapper, NullLogger<PersonalAccessTokenAuthenticator>.Instance);
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var act = async () => await authenticator.GetAccessTokenAsync(cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    #endregion

    #region GetAuthorizationHeaderAsync Tests

    [Fact]
    public async Task GetAuthorizationHeaderAsync_WithValidPAT_ShouldReturnBasicAuthHeader()
    {
        // Arrange
        var optionsWrapper = Options.Create(_options);
        var authenticator = new PersonalAccessTokenAuthenticator(optionsWrapper, NullLogger<PersonalAccessTokenAuthenticator>.Instance);

        // Act
        var result = await authenticator.GetAuthorizationHeaderAsync();

        // Assert
        result.Should().StartWith("Basic ");
        
        // Extract and verify the Base64 part
        var base64Part = result.Substring("Basic ".Length);
        var decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(base64Part));
        decoded.Should().Be("test-app-id:test-secret");
    }

    [Fact]
    public async Task GetAuthorizationHeaderAsync_CalledMultipleTimes_ShouldReturnSameHeader()
    {
        // Arrange
        var optionsWrapper = Options.Create(_options);
        var authenticator = new PersonalAccessTokenAuthenticator(optionsWrapper, NullLogger<PersonalAccessTokenAuthenticator>.Instance);

        // Act
        var firstResult = await authenticator.GetAuthorizationHeaderAsync();
        var secondResult = await authenticator.GetAuthorizationHeaderAsync();

        // Assert
        firstResult.Should().Be(secondResult);
    }

    [Fact]
    public async Task GetAuthorizationHeaderAsync_WithCancellation_ShouldRespectCancellationToken()
    {
        // Arrange
        var optionsWrapper = Options.Create(_options);
        var authenticator = new PersonalAccessTokenAuthenticator(optionsWrapper, NullLogger<PersonalAccessTokenAuthenticator>.Instance);
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var act = async () => await authenticator.GetAuthorizationHeaderAsync(cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    #endregion

    #region Edge Cases Tests

    [Fact]
    public void Constructor_WithSpecialCharactersInPAT_ShouldHandleCorrectly()
    {
        // Arrange
        var specialOptions = new PlanningCenterOptions
        {
            PersonalAccessToken = "app-id-with-dashes:secret_with_underscores_and_123"
        };
        var optionsWrapper = Options.Create(specialOptions);

        // Act & Assert
        var act = () => new PersonalAccessTokenAuthenticator(optionsWrapper, NullLogger<PersonalAccessTokenAuthenticator>.Instance);
        act.Should().NotThrow();
    }

    [Fact]
    public async Task GetAccessTokenAsync_WithSpecialCharacters_ShouldEncodeCorrectly()
    {
        // Arrange
        var specialOptions = new PlanningCenterOptions
        {
            PersonalAccessToken = "app-id-123:secret_with_special_chars_456"
        };
        var optionsWrapper = Options.Create(specialOptions);
        var authenticator = new PersonalAccessTokenAuthenticator(optionsWrapper, NullLogger<PersonalAccessTokenAuthenticator>.Instance);

        // Act
        var result = await authenticator.GetAccessTokenAsync();

        // Assert
        result.Should().NotBeNullOrEmpty();
        
        // Verify the decoded content
        var decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(result));
        decoded.Should().Be("app-id-123:secret_with_special_chars_456");
    }

    [Fact]
    public void Constructor_WithMultipleColonsInPAT_ShouldUseFirstColonAsSeparator()
    {
        // Arrange
        var multiColonOptions = new PlanningCenterOptions
        {
            PersonalAccessToken = "app-id:secret:with:colons"
        };
        var optionsWrapper = Options.Create(multiColonOptions);

        // Act & Assert
        var act = () => new PersonalAccessTokenAuthenticator(optionsWrapper, NullLogger<PersonalAccessTokenAuthenticator>.Instance);
        act.Should().NotThrow();
    }

    [Fact]
    public async Task GetAccessTokenAsync_WithMultipleColons_ShouldPreserveSecretIntegrity()
    {
        // Arrange
        var multiColonOptions = new PlanningCenterOptions
        {
            PersonalAccessToken = "app-id:secret:with:colons"
        };
        var optionsWrapper = Options.Create(multiColonOptions);
        var authenticator = new PersonalAccessTokenAuthenticator(optionsWrapper, NullLogger<PersonalAccessTokenAuthenticator>.Instance);

        // Act
        var result = await authenticator.GetAccessTokenAsync();

        // Assert
        var decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(result));
        decoded.Should().Be("app-id:secret:with:colons");
    }

    #endregion
}