using FluentAssertions;
using PlanningCenter.Api.Client.IntegrationTests.Infrastructure;
using PlanningCenter.Api.Client.Abstractions;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.People;
using PlanningCenter.Api.Client.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PlanningCenter.Api.Client.IntegrationTests.Services;

/// <summary>
/// Integration tests for PeopleService.
/// Tests the basic CRUD operations against the real Planning Center API.
/// </summary>
[Collection("Integration Tests")]
public class PeopleServiceIntegrationTests : PeopleServiceIntegrationTestBase
{
    public PeopleServiceIntegrationTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task GetMe_ShouldReturnCurrentUser()
    {
        // Act
        var result = await _peopleService.GetMeAsync();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeNullOrEmpty();
        result.FirstName.Should().NotBeNullOrEmpty();
        result.LastName.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task CreateGetUpdateDelete_ShouldPerformFullLifecycle()
    {
        // Arrange
        var randomSuffix = _random.Next(10000, 99999);
        var createRequest = new PersonCreateRequest
        {
            FirstName = $"Test{randomSuffix}",
            LastName = $"Integration{randomSuffix}",
            Email = $"test.integration.{randomSuffix}@example.com",
            Status = "inactive" // Use inactive status to avoid cluttering the real database
        };

        try
        {
            // Act - Create
            var createdPerson = await _peopleService.CreateAsync(createRequest);

            // Assert - Create
            createdPerson.Should().NotBeNull();
            createdPerson.Id.Should().NotBeNullOrEmpty();
            createdPerson.FirstName.Should().Be(createRequest.FirstName);
            createdPerson.LastName.Should().Be(createRequest.LastName);
            createdPerson.Status.Should().Be("inactive");

            // Act - Get
            var retrievedPerson = await _peopleService.GetAsync(createdPerson.Id);

            // Assert - Get
            retrievedPerson.Should().NotBeNull();
            retrievedPerson!.Id.Should().Be(createdPerson.Id);
            retrievedPerson.FirstName.Should().Be(createRequest.FirstName);
            retrievedPerson.LastName.Should().Be(createRequest.LastName);

            // Act - Update
            var updateRequest = new PersonUpdateRequest
            {
                FirstName = $"Updated{randomSuffix}",
                MiddleName = "Middle"
            };

            var updatedPerson = await _peopleService.UpdateAsync(createdPerson.Id, updateRequest);

            // Assert - Update
            updatedPerson.Should().NotBeNull();
            updatedPerson.Id.Should().Be(createdPerson.Id);
            updatedPerson.FirstName.Should().Be(updateRequest.FirstName);
            updatedPerson.MiddleName.Should().Be(updateRequest.MiddleName);
            updatedPerson.LastName.Should().Be(createRequest.LastName); // Unchanged

            // Act - Delete
            await _peopleService.DeleteAsync(createdPerson.Id);

            // Assert - Delete
            var deletedPerson = await _peopleService.GetAsync(createdPerson.Id);
            deletedPerson.Should().BeNull();
        }
        catch (Exception ex)
        {
            Assert.Fail($"Test failed with exception: {ex.Message}");
        }
    }

    [Fact]
    public async Task ListAsync_ShouldReturnPaginatedResults()
    {
        // Arrange
        var parameters = new QueryParameters
        {
            PerPage = 5
        };

        // Act
        var result = await _peopleService.ListAsync(parameters);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Meta.Should().NotBeNull();
        result.Links.Should().NotBeNull();
        
        result.Data.Count.Should().BeLessThanOrEqualTo(5);
        result.Meta.PerPage.Should().Be(5);
        result.Meta.TotalCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllResults()
    {
        // Arrange
        var parameters = new QueryParameters
        {
            PerPage = 5,
            Where = { ["status"] = "active" }
        };

        // Act
        var result = await _peopleService.GetAllAsync(parameters);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task StreamAsync_ShouldStreamResults()
    {
        // Arrange
        var parameters = new QueryParameters
        {
            PerPage = 5,
            Where = { ["status"] = "active" }
        };

        // Act
        var people = new List<Person>();
        await foreach (var person in _peopleService.StreamAsync(parameters))
        {
            people.Add(person);
            
            // Limit to 10 people to avoid long-running test
            if (people.Count >= 10)
                break;
        }

        // Assert
        people.Should().NotBeEmpty();
        people.Count.Should().BeGreaterThan(0);
    }
}
