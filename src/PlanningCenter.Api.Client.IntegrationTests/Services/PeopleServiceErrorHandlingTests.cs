using FluentAssertions;
using PlanningCenter.Api.Client.IntegrationTests.Infrastructure;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.Exceptions;
using PlanningCenter.Api.Client.Models.Requests;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PlanningCenter.Api.Client.IntegrationTests.Services;

/// <summary>
/// Integration tests for PeopleService error handling.
/// Tests how the client library handles API errors and edge cases.
/// </summary>
[Collection("Integration Tests")]
public class PeopleServiceErrorHandlingTests : PeopleServiceIntegrationTestBase
{
    public PeopleServiceErrorHandlingTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task GetAsync_ShouldReturnNull_WhenPersonDoesNotExist()
    {
        // Arrange - Use a non-existent ID
        var nonExistentId = "non-existent-id";

        // Act
        var result = await _peopleService.GetAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenRequiredFieldsAreMissing()
    {
        // Arrange - Create an invalid request with missing required fields
        var invalidRequest = new PersonCreateRequest
        {
            // Missing FirstName and LastName which are required
            Email = "test@example.com"
        };

        // Act & Assert
        await Assert.ThrowsAsync<PlanningCenterApiValidationException>(() => 
            _peopleService.CreateAsync(invalidRequest));
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotFoundException_WhenPersonDoesNotExist()
    {
        // Arrange
        var nonExistentId = "non-existent-id";
        var updateRequest = new PersonUpdateRequest
        {
            FirstName = "Updated"
        };

        // Act & Assert
        await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(() => 
            _peopleService.UpdateAsync(nonExistentId, updateRequest));
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowNotFoundException_WhenPersonDoesNotExist()
    {
        // Arrange
        var nonExistentId = "non-existent-id";

        // Act & Assert
        await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(() => 
            _peopleService.DeleteAsync(nonExistentId));
    }

    [Fact]
    public async Task AddAddressAsync_ShouldThrowValidationException_WhenRequiredFieldsAreMissing()
    {
        // Arrange - Create a test person
        var testPerson = await CreateTestPersonAsync();
        try
        {
            // Arrange - Create an invalid address request
            var invalidRequest = new AddressCreateRequest
            {
                // Missing required fields
                City = "Test City"
                // Missing Street, State, Zip
            };

            // Act & Assert
            await Assert.ThrowsAsync<PlanningCenterApiValidationException>(() => 
                _peopleService.AddAddressAsync(testPerson.Id, invalidRequest));
        }
        finally
        {
            // Cleanup
            await CleanupTestPersonAsync(testPerson.Id);
        }
    }

    [Fact]
    public async Task AddEmailAsync_ShouldThrowValidationException_WhenEmailIsInvalid()
    {
        // Arrange - Create a test person
        var testPerson = await CreateTestPersonAsync();
        try
        {
            // Arrange - Create an invalid email request
            var invalidRequest = new EmailCreateRequest
            {
                Address = "not-a-valid-email", // Invalid email format
                Location = "Home"
            };

            // Act & Assert
            await Assert.ThrowsAsync<PlanningCenterApiValidationException>(() => 
                _peopleService.AddEmailAsync(testPerson.Id, invalidRequest));
        }
        finally
        {
            // Cleanup
            await CleanupTestPersonAsync(testPerson.Id);
        }
    }

    [Fact]
    public async Task ConcurrentOperations_ShouldHandleRaceConditions()
    {
        // Arrange - Create a test person
        var testPerson = await CreateTestPersonAsync();
        try
        {
            // Act - Perform multiple operations concurrently
            var tasks = new List<Task>();
            
            // Create 3 email addresses concurrently
            for (int i = 0; i < 3; i++)
            {
                var emailRequest = new EmailCreateRequest
                {
                    Address = $"concurrent.{i}.{_random.Next(10000, 99999)}@example.com",
                    Location = "Work",
                    IsPrimary = i == 0 // Only the first one is primary
                };
                
                tasks.Add(_peopleService.AddEmailAsync(testPerson.Id, emailRequest));
            }

            // Assert - All operations should complete without exceptions
            await Task.WhenAll(tasks);
        }
        finally
        {
            // Cleanup
            await CleanupTestPersonAsync(testPerson.Id);
        }
    }
}
