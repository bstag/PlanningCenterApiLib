using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PlanningCenter.Api.Client;
using PlanningCenter.Api.Client.IntegrationTests.Infrastructure;
using PlanningCenter.Api.Client.IntegrationTests.Models;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Models.Exceptions;
using PlanningCenter.Api.Client.Models.People;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace PlanningCenter.Api.Client.IntegrationTests;

/// <summary>
/// Integration tests for ApiConnection.
/// Tests the core HTTP client functionality against the real Planning Center API.
/// </summary>
[Collection("Integration Tests")]
public class ApiConnectionIntegrationTests : IClassFixture<TestFixture>
{
    private readonly TestFixture _fixture;
    private readonly IApiConnection _apiConnection;
    private const string BaseEndpoint = "/people/v2";

    public ApiConnectionIntegrationTests(TestFixture fixture)
    {
        _fixture = fixture;
        _apiConnection = _fixture.ServiceProvider.GetRequiredService<IApiConnection>();
    }

    [Fact]
    public async Task GetAsync_ShouldReturnValidResponse_WhenEndpointExists()
    {
        // Skip if no real authentication is available
        if (!_fixture.HasRealAuthentication)
        {
            return; // Skip test
        }

        // Act
        var response = await _apiConnection.GetAsync<JsonApiSingleResponse<PersonDto>>($"{BaseEndpoint}/me");

        // Assert
        response.Should().NotBeNull();
        response.Data.Should().NotBeNull();
        response.Data!.Id.Should().NotBeNullOrEmpty();
        response.Data.Type.Should().Be("Person");
        response.Data.Attributes.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAsync_ShouldThrowNotFoundException_WhenEndpointDoesNotExist()
    {
        // Skip if no real authentication is available
        if (!_fixture.HasRealAuthentication)
        {
            return; // Skip test
        }

        // Act & Assert
        await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(() => 
            _apiConnection.GetAsync<JsonApiSingleResponse<PersonDto>>($"{BaseEndpoint}/non-existent-endpoint"));
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnPaginatedResponse_WhenEndpointExists()
    {
        // Skip if no real authentication is available
        if (!_fixture.HasRealAuthentication)
        {
            return; // Skip test
        }

        // Arrange
        var parameters = new QueryParameters
        {
            PerPage = 3
        };

        // Act
        var response = await _apiConnection.GetPagedAsync<PersonDto>($"{BaseEndpoint}/people", parameters);

        // Assert
        response.Should().NotBeNull();
        response.Data.Should().NotBeNull();
        response.Meta.Should().NotBeNull();
        response.Links.Should().NotBeNull();
        
        response.Data.Count.Should().BeLessThanOrEqualTo(3);
        response.Meta.PerPage.Should().Be(3);
        response.Meta.TotalCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldHandleEmptyResults_WhenNoDataMatches()
    {
        // Skip if no real authentication is available
        if (!_fixture.HasRealAuthentication)
        {
            return; // Skip test
        }

        // Arrange
        var parameters = new QueryParameters
        {
            Where = { ["first_name"] = "ThisNameShouldNotExistInTheDatabase" + Guid.NewGuid() }
        };

        // Act
        var response = await _apiConnection.GetPagedAsync<PersonDto>($"{BaseEndpoint}/people", parameters);

        // Assert
        response.Should().NotBeNull();
        response.Data.Should().NotBeNull().And.BeEmpty();
        response.Meta.Should().NotBeNull();
        response.Meta.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowNotFoundException_WhenResourceDoesNotExist()
    {
        // Skip if no real authentication is available
        if (!_fixture.HasRealAuthentication)
        {
            return; // Skip test
        }

        // Act & Assert
        await Assert.ThrowsAsync<PlanningCenterApiNotFoundException>(() => 
            _apiConnection.DeleteAsync($"{BaseEndpoint}/people/non-existent-id"));
    }

    [Fact]
    public async Task ApiConnection_ShouldHandleRateLimiting()
    {
        // Skip if no real authentication is available
        if (!_fixture.HasRealAuthentication)
        {
            return; // Skip test
        }

        // Arrange - Make multiple requests in quick succession
        var tasks = new List<Task>();
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(_apiConnection.GetAsync<JsonApiSingleResponse<PersonDto>>($"{BaseEndpoint}/me"));
        }

        // Act & Assert - All requests should complete without rate limit exceptions
        await Task.WhenAll(tasks);
    }
}
