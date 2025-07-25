using FluentAssertions;
using PlanningCenter.Api.Client.IntegrationTests.Infrastructure;
using PlanningCenter.Api.Client.Models.Exceptions;
using PlanningCenter.Api.Client.Models.Requests;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PlanningCenter.Api.Client.IntegrationTests.Services;

[Collection(nameof(TestCollection))]
public class RegistrationsServiceErrorHandlingTests : RegistrationsServiceIntegrationTestBase
{
    public RegistrationsServiceErrorHandlingTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task GetSignupAsync_ShouldReturnNull_WhenSignupDoesNotExist()
    {
        SkipIfNoAuthentication();

        // Arrange
        var nonExistentId = "999999999";

        // Act
        var result = await _registrationsService.GetSignupAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetRegistrationAsync_ShouldReturnNull_WhenRegistrationDoesNotExist()
    {
        SkipIfNoAuthentication();

        // Arrange
        var nonExistentId = "999999999";

        // Act
        var result = await _registrationsService.GetRegistrationAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetSelectionTypeAsync_ShouldReturnNull_WhenSelectionTypeDoesNotExist()
    {
        SkipIfNoAuthentication();

        // Arrange
        var nonExistentId = "999999999";

        // Act
        var result = await _registrationsService.GetSelectionTypeAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateSignupAsync_ShouldThrowValidationException_WhenRequiredFieldsAreMissing()
    {
        SkipIfNoAuthentication();

        // Arrange - Create an invalid signup request
        var invalidRequest = new SignupCreateRequest
        {
            // Missing Name which is required
            Description = "Test Description",
            OpenAt = DateTime.UtcNow.AddDays(-1),
            CloseAt = DateTime.UtcNow.AddDays(30)
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PlanningCenterApiValidationException>(
            () => _registrationsService.CreateSignupAsync(invalidRequest));

        exception.Should().NotBeNull();
        exception.Message.Should().ContainEquivalentOf("validation");
    }

    [Fact]
    public async Task SubmitRegistrationAsync_ShouldThrowValidationException_WhenRequiredFieldsAreMissing()
    {
        SkipIfNoAuthentication();

        // Arrange - Create an invalid registration request
        var invalidRequest = new RegistrationCreateRequest
        {
            // Missing required fields
            FirstName = "", // Empty first name
            LastName = "User"
        };

        // Create a test signup first
        var testSignup = await CreateTestSignupAsync();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PlanningCenterApiValidationException>(
            () => _registrationsService.SubmitRegistrationAsync(testSignup.Id, invalidRequest));

        exception.Should().NotBeNull();
        exception.Message.Should().ContainEquivalentOf("validation");
    }

    [Fact]
    public async Task CreateSelectionTypeAsync_ShouldThrowValidationException_WhenRequiredFieldsAreMissing()
    {
        SkipIfNoAuthentication();

        // Arrange - Create an invalid selection type request
        var invalidRequest = new SelectionTypeCreateRequest
        {
            // Missing Name which is required
            Description = "Test Description"
        };

        // Create a test signup first
        var testSignup = await CreateTestSignupAsync();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PlanningCenterApiValidationException>(
            () => _registrationsService.CreateSelectionTypeAsync(testSignup.Id, invalidRequest));

        exception.Should().NotBeNull();
        exception.Message.Should().ContainEquivalentOf("validation");
    }

    [Fact]
    public async Task SubmitRegistrationAsync_ShouldThrowValidationException_WhenSignupDoesNotExist()
    {
        SkipIfNoAuthentication();

        // Arrange - Create a registration request with non-existent signup
        var invalidRequest = new RegistrationCreateRequest
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com"
        };

        var nonExistentSignupId = "999999999";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PlanningCenterApiValidationException>(
            () => _registrationsService.SubmitRegistrationAsync(nonExistentSignupId, invalidRequest));

        exception.Should().NotBeNull();
        exception.Message.Should().ContainEquivalentOf("validation");
    }

    [Fact]
    public async Task CreateSelectionTypeAsync_ShouldThrowValidationException_WhenSignupDoesNotExist()
    {
        SkipIfNoAuthentication();

        // Arrange - Create a selection type request with non-existent signup
        var invalidRequest = new SelectionTypeCreateRequest
        {
            Name = "Test Selection Type",
            Description = "Test Description"
        };

        var nonExistentSignupId = "999999999";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PlanningCenterApiValidationException>(
            () => _registrationsService.CreateSelectionTypeAsync(nonExistentSignupId, invalidRequest));

        exception.Should().NotBeNull();
        exception.Message.Should().ContainEquivalentOf("validation");
    }

    [Fact]
    public async Task UpdateSignupAsync_ShouldThrowNotFoundException_WhenSignupDoesNotExist()
    {
        SkipIfNoAuthentication();

        // Arrange
        var nonExistentId = "999999999";
        var updateRequest = new SignupUpdateRequest
        {
            Name = "Updated Signup Name"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(
            () => _registrationsService.UpdateSignupAsync(nonExistentId, updateRequest));

        exception.Should().NotBeNull();
        exception.Message.Should().ContainEquivalentOf("not found");
    }

    [Fact]
    public async Task UpdateAttendeeAsync_ShouldThrowNotFoundException_WhenAttendeeDoesNotExist()
    {
        SkipIfNoAuthentication();

        // Arrange
        var nonExistentId = "999999999";
        var updateRequest = new AttendeeUpdateRequest
        {
            FirstName = "Updated Name"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(
            () => _registrationsService.UpdateAttendeeAsync(nonExistentId, updateRequest));

        exception.Should().NotBeNull();
        exception.Message.Should().ContainEquivalentOf("not found");
    }

    [Fact]
    public async Task UpdateSelectionTypeAsync_ShouldThrowNotFoundException_WhenSelectionTypeDoesNotExist()
    {
        SkipIfNoAuthentication();

        // Arrange
        var nonExistentId = "999999999";
        var updateRequest = new SelectionTypeUpdateRequest
        {
            Name = "Updated Selection Type Name"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(
            () => _registrationsService.UpdateSelectionTypeAsync(nonExistentId, updateRequest));

        exception.Should().NotBeNull();
        exception.Message.Should().ContainEquivalentOf("not found");
    }

    [Fact]
    public async Task DeleteSignupAsync_ShouldThrowNotFoundException_WhenSignupDoesNotExist()
    {
        SkipIfNoAuthentication();

        // Arrange
        var nonExistentId = "999999999";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(
            () => _registrationsService.DeleteSignupAsync(nonExistentId));

        exception.Should().NotBeNull();
        exception.Message.Should().ContainEquivalentOf("not found");
    }

    [Fact]
    public async Task DeleteAttendeeAsync_ShouldThrowNotFoundException_WhenAttendeeDoesNotExist()
    {
        SkipIfNoAuthentication();

        // Arrange
        var nonExistentId = "999999999";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(
            () => _registrationsService.DeleteAttendeeAsync(nonExistentId));

        exception.Should().NotBeNull();
        exception.Message.Should().ContainEquivalentOf("not found");
    }

    [Fact]
    public async Task DeleteSelectionTypeAsync_ShouldThrowNotFoundException_WhenSelectionTypeDoesNotExist()
    {
        SkipIfNoAuthentication();

        // Arrange
        var nonExistentId = "999999999";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(
            () => _registrationsService.DeleteSelectionTypeAsync(nonExistentId));

        exception.Should().NotBeNull();
        exception.Message.Should().ContainEquivalentOf("not found");
    }

    [Fact]
    public async Task ListRegistrationsAsync_ShouldThrowNotFoundException_WhenSignupDoesNotExist()
    {
        SkipIfNoAuthentication();

        // Arrange
        var nonExistentId = "999999999";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(
            () => _registrationsService.ListRegistrationsAsync(nonExistentId));

        exception.Should().NotBeNull();
        exception.Message.Should().ContainEquivalentOf("not found");
    }

    [Fact]
    public async Task ListSelectionTypesAsync_ShouldThrowNotFoundException_WhenSignupDoesNotExist()
    {
        SkipIfNoAuthentication();

        // Arrange
        var nonExistentId = "999999999";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(
            () => _registrationsService.ListSelectionTypesAsync(nonExistentId));

        exception.Should().NotBeNull();
        exception.Message.Should().ContainEquivalentOf("not found");
    }
}